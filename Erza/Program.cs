/* Copyright © Macsim Belous 2012 */
/* This file is part of Erza.

    Foobar is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Foobar is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Foobar.  If not, see <https://www.gnu.org/licenses/>.*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Data;
using System.Net;
using System.Xml;
using System.Globalization;
using System.Web.Script.Serialization;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Security.Cryptography;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using ErzaLib;
using NLog;

namespace Erza
{
    class Program
    {
        public static Logger log;
        static bool use_sqlite = true;
        static bool download_images = true;
        static bool clear_cache = false;
        static string sqlite_connection_string = null;
        static string download_dir = null;
        static bool use_sub_dir = true;
        static List<string> tags;
        static public ErzaCache cache;
        static CookieCollection sankaku_cookies = null;
        static int LIMIT_ERRORS = 4;
        static void Main(string[] args)
        {
            try
            {
                #region Logger
                try
                {
                    log = LogManager.GetCurrentClassLogger();

                    log.Trace("Version: {0}", Environment.Version.ToString());
                    log.Trace("OS: {0}", Environment.OSVersion.ToString());
                    log.Trace("Command: {0}", Environment.CommandLine.ToString());

                    NLog.Targets.FileTarget tar = (NLog.Targets.FileTarget)LogManager.Configuration.FindTargetByName("run_log");
                    tar.DeleteOldFileOnStartup = false;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Ошибка работы с логом!\n{0}", e.Message);
                }
                #endregion
                LoadSettings();
                List<ImageInfo> il = new List<ImageInfo>();
                if (args.Length <= 0)
                {
                    cache = new ErzaCache(Erza.Default.CacheConnectionString);
                    if (cache.GetCountItem() <= 0)
                    {
                        Console.WriteLine("Кэш пуст!\nЗадайте теги.");
                        log.Debug("Кэш пуст! Задайте теги.");
#if DEBUG
                    Console.ReadKey();
#endif
                        log = null;
                        return;
                    }
                    Console.WriteLine("Востанавливаю список из кэша.");
                    log.Info("Востанавливаю список из кэша.");
                    il = cache.GetItems();
                    download_dir = cache.GetDownloadDir();
                    if (Erza.Default.sankaku)
                    {
                        int temp = 0;
                        for (; ; )
                        {
                            sankaku_cookies = GetSankakuCookies("https://chan.sankakucomplex.com/");
                            if (sankaku_cookies != null)
                            {
                                break;
                            }
                            else
                            {
                                if (temp < LIMIT_ERRORS)
                                {
                                    temp++;
                                    Thread.Sleep(5000);
                                    continue;
                                }
                                else
                                {
                                    Console.Write("Не удалось получить куки!");
                                    log.Info("Не удалось получить куки!");
                                    log = null;
                                    Environment.Exit(1);
                                }
                            }
                        }
                    }
                }
                else
                {
                    Program.tags = new List<string>();
                    ParseArgs(args);
                    cache = new ErzaCache(Erza.Default.CacheConnectionString);
                    if (cache.GetCountItem() > 0)
                    {
                        Console.WriteLine("Кэш не пуст!");
                        log.Info("Кэш не пуст!");
                        if (Program.clear_cache)
                        {
                            Console.WriteLine("Очишаю кэш.");
                            log.Debug("Очишаю кэш.");
                            cache.Clear();
                        }
                        else
                        {
                            Console.WriteLine("Используйте опцию --clearcache для очистки кэша или запустите меня без параметров для закачки нескачанных файлов.");
                            log = null;
                            return;
                        }
                    }
                    if (tags.Count <= 0)
                    {
                        Console.WriteLine("Не заданы теги!");
                        log.Info("Не заданы теги!");
#if DEBUG
                    Console.ReadKey();
#endif
                        log = null;
                        return;
                    }
                    log.Info("Начинаю импорт тегов: " + string.Join(" ", Program.tags));
                    if (Program.use_sub_dir) { download_dir = download_dir + @"\" + Program.tags[0]; }
                    ServicePointManager.ServerCertificateValidationCallback = ValidationCallback;
                    for (int i = 0; i < Program.tags.Count; i++)
                    {
                        if (Erza.Default.konachan)
                        {
                            Console.WriteLine("Импортируем тег " + Program.tags[i] + " с коначан");
                            log.Debug("Импортируем тег " + Program.tags[i] + " с коначан");
                            il = SliyanieLists(il, get_hash_konachan(Program.tags[i]));
                        }
                        if (Erza.Default.danbooru)
                        {
                            Console.WriteLine("Импортируем тег " + Program.tags[i] + " с данбуры");
                            log.Debug("Импортируем тег " + Program.tags[i] + " с данбуры");
                            il = SliyanieLists(il, get_hash_danbooru_new_api(Program.tags[i]));
                        }
                        if (Erza.Default.imouto)
                        {
                            Console.WriteLine("Импортируем тег " + Program.tags[i] + " с сестрёнки");
                            log.Debug("Импортируем тег " + Program.tags[i] + " с сестрёнки");
                            il = SliyanieLists(il, get_hash_imouto(Program.tags[i]));
                        }
                        if (Erza.Default.gelbooru)
                        {
                            Console.WriteLine("Импортируем тег " + Program.tags[i] + " с гелбуры");
                            log.Debug("Импортируем тег " + Program.tags[i] + " с гелбуры");
                            il = SliyanieLists(il, get_hash_gelbooru(Program.tags[i]));
                        }
                        if (Erza.Default.sankaku)
                        {
                            Console.WriteLine("Импортируем тег " + Program.tags[i] + " с санкаки");
                            log.Debug("Импортируем тег " + Program.tags[i] + " с санкаки");
                            il = SliyanieLists(il, GetImageInfoFromSankaku(Program.tags[i]));
                        }
                    }
                    if (il.Count <= 0)
                    {
                        Console.WriteLine("Ничего ненайдено.");
                        log.Info("Ничего ненайдено.");
#if DEBUG
                    Console.ReadKey();
#endif
                        log = null;
                        return;
                    }
                    #region SQLite
                    if (Program.use_sqlite)
                    {
                        Console.WriteLine("Добавляем хэши в базу данных SQLite");
                        log.Debug("Добавляем хэши в базу данных SQLite");
                        ImagesDB idb = new ImagesDB(Program.sqlite_connection_string);
                        idb.ProgressCallBack = new ImagesDB.ProgressCallBackT(ProgressSQLiteCallBack);
                        DateTime start = DateTime.Now;
                        idb.AddImages(il);
                        DateTime finish = DateTime.Now;
                        Console.WriteLine("\nХэшей добавлено: {0} за: {1} секунд ({2} в секунду)", il.Count.ToString(), (finish - start).TotalSeconds.ToString("0.00"), (il.Count / (finish - start).TotalSeconds).ToString("0.00"));
                    }
                    #endregion
                    Console.WriteLine("Кэшируем записи");
                    log.Debug("Кэшируем записи");
                    cache.AddItems(il);
                    cache.SetDownloadDir(download_dir);
                }
                #region Download
                if (Program.download_images)
                {
                    Console.Write("\n\n\n\t\tНАЧИНАЕТСЯ ЗАГРУЗКА\n\n\n");
                    log.Debug("НАЧИНАЕТСЯ ЗАГРУЗКА");
                    int num6 = download(il, download_dir);
                }
                #endregion
                if (cache.GetCountItem() > 0)
                {
                    Console.WriteLine("В кэше остались записи!");
                    log.Debug("В кэше остались записи: " + cache.GetCountItem().ToString());
                    log = null;
                    return;
                }
                else
                {
                    Console.WriteLine("В кэше ничего не осталось!\nСжимаю его для экономии места");
                    cache.Vacuum();
                    log.Debug("Работа завершена успешно");
                }
            }
            catch (Exception ex)
            {
                log.Info("НЕ ОБРАБОТАННОЕ ИСКЛЮЧЕНИЕ: " + ex.Message);
            }
            log = null;
#if DEBUG
            Console.ReadKey();
#endif
        }
        static void ProgressSQLiteCallBack(string hash, int Count, int Total)
        {
            Console.Write("Обрабатываю хэш {0} ({1}/{2})\r", hash, Count, Total);
        }
        static void LoadSettings()
        {
            Program.clear_cache = Erza.Default.ClearCache;
            Program.download_dir = Erza.Default.download_dir;
            Program.download_images = Erza.Default.download;
            Program.use_sqlite = Erza.Default.use_sqlite;
            Program.sqlite_connection_string = Erza.Default.ConnectionString;
        }
        static void ParseArgs(string[] args)
        {
            string sqlite_string = "--sqlite";
            string nosqlite_string = "--nosqlite";
            string sqlite_path_string = "--sqlite-path=";
            string dir_string = "--dir=";
            string download_string = "--download";
            string nodownload_string = "--nodownload";
            string clear_cache = "--clearcache";
            string nosubdir_string = "--nosubdir";
            foreach (string param in args)
            {
                if (param == nosubdir_string)
                {
                    Program.use_sub_dir = false;
                    continue;
                }
                if (param == clear_cache)
                {
                    Program.clear_cache = true;
                    continue;
                }
                if (param == sqlite_string)
                {
                    Program.use_sqlite = true;
                    continue;
                }
                if (param == nosqlite_string)
                {
                    Program.use_sqlite = false;
                    continue;
                }
                if (param == download_string)
                {
                    Program.download_images = true;
                    continue;
                }
                if (param == nodownload_string)
                {
                    Program.download_images = false;
                    continue;
                }
                if (param.Length >= sqlite_path_string.Length)
                {
                    if (param.Substring(0, sqlite_path_string.Length) == sqlite_path_string)
                    {
                        Program.sqlite_connection_string = "data source=" + param.Substring(sqlite_path_string.Length);
                        continue;
                    }
                }
                if (param.Length >= dir_string.Length)
                {
                    if (param.Substring(0, dir_string.Length) == dir_string)
                    {
                        Program.download_dir = param.Substring(dir_string.Length);
                        continue;
                    }
                }
                Program.tags.Add(param);
            }
        }
        static List<ImageInfo> get_hash_danbooru_new_api(string tag)
        {
            const int DANBOORU_LIMIT_POSTS = 100;
            int nPage = 1;                //Счетчик страниц
            List<ImageInfo> img_list = new List<ImageInfo>();
            int count_errors = 0;
            for (; ; )
            {
                WebClient Client = new WebClient();
                string strURL = String.Format("http://danbooru.donmai.us/posts.xml?tags={0}&page={1}&login={2}&api_key={3}&limit={4}", tag, nPage, Erza.Default.login, Erza.Default.api_key, DANBOORU_LIMIT_POSTS);
                Console.WriteLine("({0}/ХЗ) Загружаем и парсим: {1}", img_list.Count, strURL);
                try
                {
                    Uri uri = new Uri(strURL);
                    DateTime start = DateTime.Now;
                    string xml = Client.DownloadString(uri);
                    if (xml == null) {
                        if (count_errors < LIMIT_ERRORS)
                        {
                            count_errors++;
                            continue;
                        }
                        else
                        {
                            log.Info("get_hash_danbooru_new_api: XML IS NULL");
                            break;
                        }
                    }
                    List<ImageInfo> list = ParseXMLDanBooru_new_api(xml);
                    if (list.Count <= 0)
                    {
                        break;
                    }
                    else
                    {
                        img_list.AddRange(list);
                        nPage++;
                    }
                    MyWait(start, 5000);
                    //Thread.Sleep(1200);
                }
                catch (WebException we)
                {
                    Console.WriteLine("Ошибка: " + we.Message);
                    if (we.Response != null)
                    {
                        if (((HttpWebResponse)we.Response).StatusCode == HttpStatusCode.InternalServerError)
                        {
                            break;
                        }
                    }
                    Thread.Sleep(10000);
                    continue;
                }
                finally
                {
                    if (Client != null)
                    {
                        Client.Dispose();
                        Client = null;
                    }
                }
            }
            return img_list;
        }
        static List<ImageInfo> get_hash_konachan(string tag)
        {
            const int KONACHAN_LIMIT_POSTS = 100;
            int nPostsCount = 0;          //Счетчик постов для скачивания
            int nPage = 1;                //Счетчик страниц
            WebClient Client;
            List<ImageInfo> img_list = new List<ImageInfo>();
            int count = 0;
            while (true)
            {
                nPostsCount = posts_count(String.Format("http://konachan.com/post.xml?tags={0}&limit=1", tag));
                if (nPostsCount >= 0)
                {
                    break;
                }
                count++;
                if (count >= LIMIT_ERRORS)
                {
                    log.Info("Не удалось получить количество постов!");
                    Console.WriteLine("Не удалось получить количество постов!");
                }
            }
            if (nPostsCount == 0)
            {
                Console.WriteLine("Ничего ненайдено!");
                return img_list;
            }
            Client = new WebClient();
            int count_errors = 0;
            for (; ; )
            {
                string strURL = String.Format("http://konachan.com/post.xml?tags={0}&page={1}&limit={2}", tag, nPage, KONACHAN_LIMIT_POSTS);
                Console.WriteLine("({0}/{1}) Загружаем и парсим: {2}", img_list.Count, nPostsCount, strURL);
                try
                {
                    Uri uri = new Uri(strURL);
                    DateTime start = DateTime.Now;
                    string xml = Client.DownloadString(uri);
                    if (xml == null)
                    {
                        if (count_errors < LIMIT_ERRORS)
                        {
                            count_errors++;
                            continue;
                        }
                        else
                        {
                            log.Info("get_hash_konachan: XML IS NULL");
                            break;
                        }
                    }
                    List<ImageInfo> list = ParseXMLKonachan(xml);
                    if (list.Count <= 0)
                    {
                        break;
                    }
                    else
                    {
                        img_list.AddRange(list);
                        nPage++;
                    }
                    MyWait(start, 5000);
                }
                catch (WebException we)
                {
                    Console.WriteLine("Ошибка: " + we.Message);
                    Thread.Sleep(10000);
                    //return;
                    continue;
                }
            }
            return img_list;
        }
        static List<ImageInfo> get_hash_imouto(string tag)
        {
            const int YANDERE_LIMIT_POSTS = 100;
            int nPostsCount = 0;          //Счетчик постов для скачивания
            int nPage = 1;                //Счетчик страниц
            WebClient Client;
            List<ImageInfo> img_list = new List<ImageInfo>();
            int count = 0;
            while (true)
            {
                nPostsCount = posts_count(String.Format("https://yande.re/post.xml?tags={0}&limit=1", tag));
                if (nPostsCount >= 0)
                {
                    break;
                }
                count++;
                if (count >= LIMIT_ERRORS)
                {
                    log.Info("Не удалось получить количество постов!");
                    Console.WriteLine("Не удалось получить количество постов!");
                }
            }
            if (nPostsCount == 0)
            {
                Console.WriteLine("Ничего ненайдено!");
                return img_list;
            }
            Client = new WebClient();
            int count_errors = 0;
            for (; ; )
            {
                string strURL = String.Format("https://yande.re/post.xml?tags={0}&page={1}&limit={2}", tag, nPage, YANDERE_LIMIT_POSTS);
                Console.WriteLine("({0}/{1}) Загружаем и парсим: {2}", img_list.Count, nPostsCount , strURL);
                try
                {
                    Uri uri = new Uri(strURL);
                    DateTime start = DateTime.Now;
                    string xml = Client.DownloadString(uri);
                    if (xml == null)
                    {
                        if (count_errors < LIMIT_ERRORS)
                        {
                            count_errors++;
                            continue;
                        }
                        else
                        {
                            log.Info("get_hash_imouto: XML IS NULL");
                            break;
                        }
                    }
                    List<ImageInfo> list = ParseXMLYandere(xml);
                    if (list.Count <= 0)
                    {
                        break;
                    }
                    else
                    {
                        img_list.AddRange(list);
                        nPage++;
                    }
                    MyWait(start, 5000);
                }
                catch (WebException we)
                {
                    Console.WriteLine("Ошибка: " + we.Message);
                    log.Debug("get_hash_imouto: " + we.Message);
                    Thread.Sleep(60000);
                    continue;
                }
            }
            Client.Dispose();
            return img_list;
        }
        static List<ImageInfo> GetImageInfoFromYandere_json(string tag)
        {
            const int YANDERE_LIMIT_POSTS = 100;
            int nPostsCount = 0;          //Счетчик постов для скачивания
            int nPage = 1;                //Счетчик страниц
            WebClient Client;
            List<ImageInfo> img_list = new List<ImageInfo>();
            Client = new WebClient();
            for (; ; )
            {
                string strURL = String.Format("https://yande.re/post.json?tags={0}&page={1}&limit={2}", tag, nPage, YANDERE_LIMIT_POSTS);
                Console.WriteLine("({0}/{1}) Загружаем и парсим: {2}", img_list.Count, nPostsCount, strURL);
                try
                {
                    Uri uri = new Uri(strURL);
                    DateTime start = DateTime.Now;
                    string json = Client.DownloadString(uri);
                    List<ImageInfo> list = ParseYandere_json(json);
                    if (list.Count <= 0)
                    {
                        break;
                    }
                    else
                    {
                        img_list.AddRange(list);
                        nPage++;
                    }
                    MyWait(start, 5000);
                }
                catch (WebException we)
                {
                    Console.WriteLine("Ошибка: " + we.Message);
                    log.Debug("GetImageInfoFromYandere_json: " + we.Message);
                    Thread.Sleep(60000);
                    continue;
                }
            }
            Client.Dispose();
            return img_list;
        }
        static List<ImageInfo> ParseYandere_json(string json)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<json_image> result = ser.Deserialize<List<json_image>>(json);
            List<ImageInfo> temp = new List<ImageInfo>();
            foreach (json_image ji in result)
            {
                ImageInfo img = new ImageInfo();
                img.SetHashString(ji.md5);
                img.AddStringOfTags(ji.tags);
                img.yandere_post_id = ji.id;
                img.urls.Add(ji.file_url);
                img.yandere_url = ji.file_url;
                temp.Add(img);
            }
            return temp;
        }
        static List<ImageInfo> get_hash_gelbooru(string tag)
        {
            const int GELBOORU_LIMIT_POSTS = 100;
            int nPostsCount = 0;          //Счетчик постов для скачивания
            int nPage = 0;                //Счетчик страниц
            WebClient Client;
            List<ImageInfo> img_list = new List<ImageInfo>();
            int count = 0;
            while (true)
            {
                nPostsCount = posts_count(String.Format("http://gelbooru.com/index.php?page=dapi&s=post&q=index&tags={0}&pid=0&limit=1", tag));
                if (nPostsCount >= 0)
                {
                    break;
                }
                count++;
                if (count >= LIMIT_ERRORS)
                {
                    log.Info("Не удалось получить количество постов!");
                    Console.WriteLine("Не удалось получить количество постов!");
                }
            }
            if (nPostsCount == 0)
            {
                Console.WriteLine("Ничего ненайдено!");
                return img_list;
            }
            Client = new WebClient();
            int count_errors = 0;
            for (; ; )
            {
                string strURL = String.Format("http://gelbooru.com/index.php?page=dapi&s=post&q=index&tags={0}&pid={1}&limit={2}", tag, nPage, GELBOORU_LIMIT_POSTS);
                Console.WriteLine("({0}/{1}) Загружаем и парсим: {2}", img_list.Count, nPostsCount, strURL);
                try
                {
                    Uri uri = new Uri(strURL);
                    DateTime start = DateTime.Now;
                    string xml = Client.DownloadString(uri);
                    if (xml == null)
                    {
                        if (count_errors < LIMIT_ERRORS)
                        {
                            count_errors++;
                            continue;
                        }
                        else
                        {
                            log.Info("get_hash_gelbooru: XML IS NULL");
                            break;
                        }
                    }
                    List<ImageInfo> list = ParseXMLGelBooru(xml);
                    if (list.Count <= 0)
                    {
                        break;
                    }
                    else
                    {
                        img_list.AddRange(list);
                        nPage++;
                    }
                    MyWait(start, 5000);
                }
                catch (WebException we)
                {
                    Console.WriteLine("Ошибка: " + we.Message);
                    Thread.Sleep(10000);
                    continue;
                }
            }
            return img_list;
        }
        static List<ImageInfo> GetImageInfoFromSankaku(string tag)
        {
            if (sankaku_cookies == null)
            {
                int temp = 0;
                for (; ; )
                {
                    sankaku_cookies = GetSankakuCookies("https://chan.sankakucomplex.com/");
                    if (sankaku_cookies != null)
                    {
                        break;
                    }
                    else
                    {
                        if (temp < 4)
                        {
                            temp++;
                            Thread.Sleep(5000);
                            continue;
                        }
                        else
                        {
                            Console.Write("Не удалось получить куки!");
                            log.Info("Не удалось получить куки!");
                            log = null;
                            Environment.Exit(1);
                        }
                    }
                }
            }
            string BaseURL = "https://chan.sankakucomplex.com/post/index";
            List<ImageInfo> imgs = new List<ImageInfo>();
            int i = 1;
            while (true)
            {
                Console.Write("({0}/ХЗ) ", imgs.Count);
                DateTime start = DateTime.Now;
                string text = DownloadHTML(BaseURL, tag, i, sankaku_cookies);
                MyWait(start, 5000);
                if (text != null)
                {
                    List<ImageInfo> posts = ParseHTML_sankaku(text);
                    if (posts.Count > 0)
                    {
                        imgs.AddRange(posts);
                        i++;
                        continue;
                    }
                    else break;
                }
                else break;
            }
            return imgs;
        }
        public static CookieCollection GetSankakuCookies(string url)
        {
            try
            {
                HttpWebRequest loginRequest = (HttpWebRequest)WebRequest.Create(url);
                loginRequest.UserAgent = Erza.Default.UserAgent;
                loginRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                loginRequest.Headers.Add("Accept-Encoding: identity");
                loginRequest.CookieContainer = new CookieContainer();
                HttpWebResponse loginResponse = (HttpWebResponse)loginRequest.GetResponse();
                return loginResponse.Cookies;
            }
            catch (WebException we)
            {
                Console.WriteLine(we.Message);
                log.Debug("GetSankakuCookies" + we.Message);
                return null;
            }
        }
        public static string DownloadStringFromSankaku(string url, string referer, CookieCollection cookies)
        {
            HttpWebRequest downloadRequest = (HttpWebRequest)WebRequest.Create(url);
            downloadRequest.UserAgent = Erza.Default.UserAgent;
            downloadRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            downloadRequest.Headers.Add("Accept-Encoding: identity");
            downloadRequest.CookieContainer = new CookieContainer();
            downloadRequest.CookieContainer.Add(cookies);
            if (referer != null)
            {
                downloadRequest.Referer = referer;
            }
            string source;
            using (StreamReader reader = new StreamReader(downloadRequest.GetResponse().GetResponseStream()))
            {
                source = reader.ReadToEnd();
            }
            return source;
        }
        static string DownloadHTML(string m_strBaseURL, string m_strTags, int nPage, CookieCollection cookies)
        {
            int count_503 = 0;
            string strURL = String.Format("{0}?tags={1}&page={2}", m_strBaseURL, m_strTags, nPage);
            Console.WriteLine("Загружаем и парсим: " + strURL);
            log.Debug("Загружаем и парсим: " + strURL);
            while (true)
            {
                try
                {
                    return DownloadStringFromSankaku(strURL, null, cookies);
                }
                catch (WebException we)
                {
                    Console.WriteLine("Ошибка: " + we.Message);
                    log.Debug("DownloadHTML: " + we.Message);
                    Thread.Sleep(60000);
                    if (we.Response == null) { continue; }
                    if (((HttpWebResponse)we.Response).StatusCode == HttpStatusCode.ServiceUnavailable) {
                        if (count_503 < 4)
                        {
                            count_503++;
                            continue;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }
        static List<ImageInfo> ParseHTML_sankaku(string html)
        {
            List<ImageInfo> temp = new List<ImageInfo>();
            Regex rx = new Regex("Post.register\\((.*?)\\);", RegexOptions.Compiled);
            MatchCollection matches = rx.Matches(html);
            foreach (Match match in matches)
            {
                string json = match.Value.Substring(14, match.Value.Length - 16);
                //json = json.Remove(json.Length-2);
                ImageInfo img = parse_json_one_sankaku(json);
                if (img.sankaku_post_id == 0) { continue; }
                temp.Add(img);
            }
            return temp;
        }
        static ImageInfo parse_json_one_sankaku(string json)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            json_image result = ser.Deserialize<json_image>(json);
            ImageInfo img = new ImageInfo();
            img.SetHashString(result.md5);
            img.AddStringOfTags(result.tags);
            img.sankaku_post_id = result.id;
            return img;
        }
        static List<ImageInfo> parse_json(string json)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<json_image> result = ser.Deserialize<List<json_image>>(json);
            List<ImageInfo> temp = new List<ImageInfo>();
            for (int i = 0; i < result.Count; i++)
            {
                ImageInfo img = new ImageInfo();
                //img.hash = HexStringToBytes(result[i].md5);
                img.SetHashString(result[i].md5);
                img.AddStringOfTags(result[i].tags);
                img.sankaku_post_id = result[i].id;
                if (result[i].file_url != null)
                {
                    img.urls.Add(result[i].file_url);
                }
                else
                {
                    img.urls.Add("https://cs.sankakucomplex.com/data/" + img.GetHashString().Substring(0, 2) + "/" + img.GetHashString().Substring(2, 2) + "/" + img.GetHashString() + ".jpg");
                    img.urls.Add("https://cs.sankakucomplex.com/data/" + img.GetHashString().Substring(0, 2) + "/" + img.GetHashString().Substring(2, 2) + "/" + img.GetHashString() + ".png");
                    img.urls.Add("https://cs.sankakucomplex.com/data/" + img.GetHashString().Substring(0, 2) + "/" + img.GetHashString().Substring(2, 2) + "/" + img.GetHashString() + ".gif");
                    img.urls.Add("https://cs.sankakucomplex.com/data/" + img.GetHashString().Substring(0, 2) + "/" + img.GetHashString().Substring(2, 2) + "/" + img.GetHashString() + ".swf");
                    img.urls.Add("https://cs.sankakucomplex.com/data/" + img.GetHashString().Substring(0, 2) + "/" + img.GetHashString().Substring(2, 2) + "/" + img.GetHashString() + ".jpeg");
                    //img.file_url = img.urls[0];
                }
                temp.Add(img);
            }
            return temp;
        }
        static int posts_count(string url)
        {
            int nLocalPostsCount = 0;
            WebClient Client = new WebClient();
            try
            {
                Uri uri = new Uri(url);
                string xml = Client.DownloadString(uri);
                if (xml == null)
                {
                    return -1;
                }
                XmlDocument mXML = new XmlDocument();
                mXML.LoadXml(xml);
                XmlNodeList nodeList = mXML.GetElementsByTagName("posts");
                XmlNode node = nodeList.Item(0);
                //Определяем число постов
                for (int i = 0; i < node.Attributes.Count; i++)
                {
                    if (node.Attributes[i].Name == "count") nLocalPostsCount = Convert.ToInt32(node.Attributes[i].Value);
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
                log.Debug("posts_count " + ex.Message);
                nLocalPostsCount = -1;
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine(ex.Message);
                log.Debug("posts_count " + ex.Message);
                nLocalPostsCount = -1;
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex.Message);
                log.Debug("posts_count " + ex.Message);
                nLocalPostsCount = -1;
            }
            catch (XmlException ex)
            {
                Console.WriteLine(ex.Message);
                log.Debug("posts_count " + ex.Message);
                nLocalPostsCount = -1;
            }
            finally
            {
                Client.Dispose();
            }
            return nLocalPostsCount;
        }
        static List<ImageInfo> ParseXMLKonachan(string strXML)
        {
            List<ImageInfo> list = new List<ImageInfo>();
            XmlDocument mXML = new XmlDocument();
            try
            {
                mXML.LoadXml(strXML);
            }
            catch (XmlException e)
            {
                Console.WriteLine(e.Message);
                return list;
            }
            XmlNodeList nodeList = mXML.GetElementsByTagName("post");
            //Парсим посты
            for (int i = 0; i < nodeList.Count; i++)
            {
                ImageInfo mImgDescriptor = new ImageInfo();
                XmlNode node = nodeList.Item(i);
                for (int j = 0; j < node.Attributes.Count; j++)
                {
                    //Тэги
                    if (node.Attributes[j].Name == "tags")
                    {
                        mImgDescriptor.AddStringOfTags(node.Attributes[j].Value);
                        //mImgDescriptor.tags = new List<string>(mImgDescriptor.tags_string.Split(' ')); //Получаем массив тэгов
                    }
                    if (node.Attributes[j].Name == "md5")
                    {
                        mImgDescriptor.SetHashString(node.Attributes[j].Value);
                        //mImgDescriptor.hash = HexStringToBytes(node.Attributes[j].Value);
                    }
                    if (node.Attributes[j].Name == "file_url")
                    {
                        mImgDescriptor.urls.Add(node.Attributes[j].Value);
                        mImgDescriptor.konachan_url = node.Attributes[j].Value;
                    }
                    if (node.Attributes[j].Name == "id")
                    {
                        mImgDescriptor.konachan_post_id = System.Convert.ToInt32(node.Attributes[j].Value);
                    }
                }
                list.Add(mImgDescriptor); //Добавляем в список
            }
            return list;
        }
        static List<ImageInfo> ParseXMLGelBooru(string strXML)
        {
            List<ImageInfo> list = new List<ImageInfo>();
            XmlDocument mXML = new XmlDocument();
            try
            {
                mXML.LoadXml(strXML);
            }
            catch (XmlException e)
            {
                Console.WriteLine(e.Message);
                return list;
            }
            XmlNodeList nodeList = mXML.GetElementsByTagName("post");
            //Парсим посты
            for (int i = 0; i < nodeList.Count; i++)
            {
                ImageInfo mImgDescriptor = new ImageInfo();
                XmlNode node = nodeList.Item(i);
                for (int j = 0; j < node.Attributes.Count; j++)
                {
                    //Тэги
                    if (node.Attributes[j].Name == "tags")
                    {
                        mImgDescriptor.AddStringOfTags(node.Attributes[j].Value);
                        //mImgDescriptor.tags = new List<string>(mImgDescriptor.tags_string.Split(' ')); //Получаем массив тэгов
                    }
                    if (node.Attributes[j].Name == "md5")
                    {
                        mImgDescriptor.SetHashString(node.Attributes[j].Value);
                        //mImgDescriptor.hash = HexStringToBytes(node.Attributes[j].Value);
                    }
                    if (node.Attributes[j].Name == "file_url")
                    {
                        mImgDescriptor.urls.Add(node.Attributes[j].Value);
                        mImgDescriptor.gelbooru_url = node.Attributes[j].Value;
                    }
                    if (node.Attributes[j].Name == "id")
                    {
                        mImgDescriptor.gelbooru_post_id = System.Convert.ToInt32(node.Attributes[j].Value);
                    }
                }
                list.Add(mImgDescriptor);
            }
            return list;
        }
        static List<ImageInfo> ParseXMLYandere(string strXML)
        {
            List<ImageInfo> list = new List<ImageInfo>();
            XmlDocument mXML = new XmlDocument();
            try
            {
                mXML.LoadXml(strXML);
            }
            catch (XmlException e)
            {
                Console.WriteLine(e.Message);
                return list;
            }
            XmlNodeList nodeList = mXML.GetElementsByTagName("post");
            //Парсим посты
            for (int i = 0; i < nodeList.Count; i++)
            {
                ImageInfo mImgDescriptor = new ImageInfo();
                XmlNode node = nodeList.Item(i);
                for (int j = 0; j < node.Attributes.Count; j++)
                {
                    //Тэги
                    if (node.Attributes[j].Name == "tags")
                    {
                        mImgDescriptor.AddStringOfTags(node.Attributes[j].Value);
                        //mImgDescriptor.tags = new List<string>(mImgDescriptor.tags_string.Split(' ')); //Получаем массив тэгов
                    }
                    if (node.Attributes[j].Name == "md5")
                    {
                        mImgDescriptor.SetHashString(node.Attributes[j].Value);
                        //mImgDescriptor.hash = HexStringToBytes(node.Attributes[j].Value);
                    }
                    if (node.Attributes[j].Name == "file_url")
                    {
                        mImgDescriptor.urls.Add(node.Attributes[j].Value);
                        mImgDescriptor.yandere_url = node.Attributes[j].Value;
                    }
                    if (node.Attributes[j].Name == "id")
                    {
                        mImgDescriptor.yandere_post_id = System.Convert.ToInt32(node.Attributes[j].Value);
                    }
                }
                list.Add(mImgDescriptor);
            }
            return list;
        }
        static List<ImageInfo> ParseXMLDanBooru_new_api(string strXML)
        {
            List<ImageInfo> list = new List<ImageInfo>();
            XmlDocument mXML = new XmlDocument();
            try
            {
                mXML.LoadXml(strXML);
            }
            catch (XmlException e)
            {
                Console.WriteLine(e.Message);
                return list;
            }
            XmlNodeList nodeList = mXML.GetElementsByTagName("post");
            //Парсим посты
            foreach (XmlNode node in nodeList)
            {
                ImageInfo mImgDescriptor = new ImageInfo();
                //Console.WriteLine(node.OuterXml);
                XmlElement md5 = node["md5"];
                if (md5 == null) { continue; }
                mImgDescriptor.SetHashString(md5.InnerText);
                //mImgDescriptor.hash = HexStringToBytes(md5.InnerText);
                XmlElement tags = node["tag-string"];
                mImgDescriptor.AddStringOfTags(tags.InnerText);
                XmlElement id = node["id"];
                mImgDescriptor.danbooru_post_id = System.Convert.ToInt32(id.InnerText);
                XmlElement ext = node["file-ext"];
                if (ext.InnerText.ToLower() == "txt") { break; }
                if (ext.InnerText.ToLower().Length > 4) { break; }
                if (ext.InnerText.ToLower().LastIndexOf('?') > -1) { break; }
                mImgDescriptor.urls.Add("http://danbooru.donmai.us/data/" + md5.InnerText + "." + ext.InnerText);
                mImgDescriptor.danbooru_url = "http://danbooru.donmai.us/data/" + md5.InnerText + "." + ext.InnerText;
                list.Add(mImgDescriptor);
            }
            return list;
        }
        static byte[] HexStringToBytes(string hexString)
        {
            if (hexString == null)
            {
                throw new ArgumentNullException("hexString");
            }

            if ((hexString.Length & 1) != 0)
            {
                throw new ArgumentOutOfRangeException("hexString", hexString, "hexString must contain an even number of characters.");
            }

            byte[] result = new byte[hexString.Length / 2];

            for (int i = 0; i < hexString.Length; i += 2)
            {
                result[i / 2] = byte.Parse(hexString.Substring(i, 2), NumberStyles.HexNumber);
            }

            return result;
        }
        static int FindHash(List<ImageInfo> list, ImageInfo img)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].hash.SequenceEqual(img.hash))
                {
                    return i;
                }
            }
            return -1;
        }
        static List<ImageInfo> SliyanieLists(List<ImageInfo> list, List<ImageInfo> temp)
        {
            for (int temp_i = 0; temp_i < temp.Count; temp_i++)
            {
                int t = FindHash(list, temp[temp_i]);
                if (t < 0)
                {
                    //temp[temp_i].urls.Add(temp[temp_i].file_url);
                    list.Add(temp[temp_i]);
                }
                else
                {
                    /*for (int temp_i3 = 0; temp_i3 < temp[temp_i].tags.Count; temp_i3++)
                    {
                        if (list[t].tags.IndexOf(temp[temp_i].tags[temp_i3]) < 0)
                        {
                            list[t].tags.Add(temp[temp_i].tags[temp_i3]);
                        }
                    }*/
                    list[t].AddTags(temp[temp_i].tags);
                    list[t].urls.AddRange(temp[temp_i].urls);
                    if (temp[temp_i].sankaku_post_id > 0) { list[t].sankaku_post_id = temp[temp_i].sankaku_post_id; }
                    if (temp[temp_i].gelbooru_post_id > 0) { list[t].gelbooru_post_id = temp[temp_i].gelbooru_post_id; }
                    if (temp[temp_i].danbooru_post_id > 0) { list[t].danbooru_post_id = temp[temp_i].danbooru_post_id; }
                    if (temp[temp_i].yandere_post_id > 0) { list[t].yandere_post_id = temp[temp_i].yandere_post_id; }
                    if (temp[temp_i].konachan_post_id > 0) { list[t].konachan_post_id = temp[temp_i].konachan_post_id; }

                    if (temp[temp_i].danbooru_url != String.Empty) { list[t].danbooru_url = temp[temp_i].danbooru_url; }
                    if (temp[temp_i].gelbooru_url != String.Empty) { list[t].gelbooru_url = temp[temp_i].gelbooru_url; }
                    if (temp[temp_i].konachan_url != String.Empty) { list[t].konachan_url = temp[temp_i].konachan_url; }
                    if (temp[temp_i].yandere_url != String.Empty) { list[t].yandere_url = temp[temp_i].yandere_url; }
                    if (temp[temp_i].sankaku_url != String.Empty) { list[t].sankaku_url = temp[temp_i].sankaku_url; }
                }
            }
            return list;
        }
        private static bool ValidationCallback(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        private static void MyWait(DateTime start, int delay)
        {
            int current = (int)((DateTime.Now - start).TotalMilliseconds);
            //Console.Write("TIME {0}, ", current);
            if (current < delay)
            {
                #if DEBUG
                Console.WriteLine("TIME {0}, WAIT {1}", current, delay - current);
                #endif
                Thread.Sleep(delay - current);
                return;
            }
            else
            {
                return;
            }
        }
        #region DownloadFile
        static int download(List<ImageInfo> list, string dir)
        {
            int count_complit = 0;
            int count_deleted = 0;
            int count_error = 0;
            int count_skip = 0;
            ImagesDB idb = new ImagesDB(Program.sqlite_connection_string);
            //List<string> DownloadedList = new List<string>();
            //DownloadedList.Sort();
            Directory.CreateDirectory(dir);
            for (int i = 0; i < list.Count; i++)
            {
                Console.WriteLine("\n###### {0}/{1} ######", (i+1), list.Count);
                //if(DownloadedList.BinarySearch(list[i].GetHashString()) >= 0){int g = 1+1;}
                if (Program.use_sqlite)
                {
                    ImageInfo img = idb.ExistImage(list[i].hash);
                    if (img != null)
                    {
                        if (img.is_deleted)
                        {
                            Console.WriteLine("Этот фаил уже был ранее удалён.");
                            count_deleted++;
                            cache.DeleteItem(list[i].hash);
                            continue;
                        }
                        if (img.file != null)
                        {
                            Console.WriteLine("Этот фаил уже был ранее скачан.");
                            Console.WriteLine(img.file);
                            count_skip++;
                            cache.DeleteItem(list[i].hash);
                            continue;
                        }
                    }
                }
                long r = DownloadImage(list[i], dir);
                if (r == 0)
                { 
                    count_complit++;
                    cache.DeleteItem(list[i].hash);
                    //InsertHashToDownloadedList(DownloadedList, list[i].GetHashString());
                }
                else
                {
                    count_error++;
                }
            }
            Console.WriteLine("Успешно скачано: {0}\nСкачано ренее: {1}\nУдалено ранее: {2}\nОшибочно: {3}\nВсего: {4}", count_complit, count_skip, count_deleted, count_error, list.Count);
            log.Info(String.Format("Успешно скачано: {0} Скачано ренее: {1} Удалено ранее: {2} Ошибочно: {3} Всего: {4}", count_complit, count_skip, count_deleted, count_error, list.Count));
            return 0;
        }
        static long DownloadImage(ImageInfo img, string dir)
        {
            //CookieCollection cookies = GetSankakuCookies("https://chan.sankakucomplex.com/");
            int cnt;
            if (Erza.Default.download_povtor < 1)
            {
                cnt = 1;
            }
            else
            {
                cnt = Erza.Default.download_povtor;
            }
            for (int index = 0; index < cnt; index++)
            {
                foreach (string url in img.urls)
                {
                    string extension = Path.GetExtension(url);
                    DateTime start = DateTime.Now;
                    if (extension == ".jpeg")
                    {
                        long result = DownloadFile(url, dir + "\\" + img.GetHashString() + ".jpg", GetReferer(url, img));
                        if (result == 0)
                        {
                            MyWait(start, 2500);
                            return 0;
                        }
                    }
                    else
                    {
                        long result = DownloadFile(url, dir + "\\" + img.GetHashString() + extension, GetReferer(url, img));
                        if (result == 0)
                        {
                            MyWait(start, 2500);
                            return 0;
                        }
                    }
                }
                if (img.sankaku_post_id > 0)
                {
                    DateTime start = DateTime.Now;
                    if (DownloadImageFromSankaku(img, dir, sankaku_cookies))
                    {
                        MyWait(start, 5000);
                        return 0;
                    }
                    MyWait(start, 5000);
                }
            }
            return 1;
        }
        static string GetReferer(string url, ImageInfo img)
        {
            if (url.LastIndexOf("https://chan.sankakustatic.com/data/") >= 0)
            {
                Uri uri = new Uri("https://chan.sankakucomplex.com/post/show/" + img.sankaku_post_id.ToString());
                return uri.AbsoluteUri;
            }
            if (url.LastIndexOf("gelbooru.com/") >= 0)
            {
                Uri uri = new Uri("https://gelbooru.com/index.php?page=post&s=view&id=" + img.gelbooru_post_id.ToString());
                return uri.AbsoluteUri;
            }
            if (url.LastIndexOf("https://konachan.com/") >= 0)
            {
                Uri uri = new Uri("https://konachan.com/post/show/" + img.konachan_post_id.ToString() + "/" + img.GetStringOfTags().Replace(' ', '-'));
                return uri.AbsoluteUri;
            }
            if (url.LastIndexOf("https://sonohara.donmai.us/") >= 0)
            {
                Uri uri = new Uri("https://danbooru.donmai.us/post/show/" + img.danbooru_post_id.ToString() + "/" + img.GetStringOfTags().Replace(' ', '-'));
                return uri.AbsoluteUri;
            }
            if (url.LastIndexOf("https://yande.re/") >= 0)
            {
                Uri uri = new Uri("https://yande.re/post/show/" + img.yandere_post_id.ToString() + "/" + img.GetStringOfTags().Replace(' ', '-'));
                return uri.AbsoluteUri;
            }
            return String.Empty;
        }
        static long DownloadFile(string url, string filename, string referer)
        {
            Console.WriteLine("Начинаем закачку {0}.", url);
            FileInfo fi = new FileInfo(filename);
            HttpWebRequest httpWRQ = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            WebResponse wrp = null;
            Stream rStream = null;
            try
            {
                httpWRQ.Referer = referer;
                httpWRQ.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
                httpWRQ.Timeout = 60 * 1000;
                wrp = httpWRQ.GetResponse();
                if (fi.Exists)
                {
                    if (wrp.ContentLength == fi.Length)
                    {
                        Console.WriteLine("Уже скачан.");
                        //wrp.Close();
                        return 0;
                    }
                    else
                    {
                        fi.Delete();
                    }
                }
                long cnt = 0;
                rStream = wrp.GetResponseStream();
                rStream.ReadTimeout = 60 * 1000;
                using (FileStream fs = new FileStream(filename, FileMode.Create))
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead;
                    DateTime start = DateTime.Now;
                    while ((bytesRead = rStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fs.Write(buffer, 0, bytesRead);
                        cnt += bytesRead;
                        DateTime pred = DateTime.Now;
                        Console.Write("\rСкачано " + cnt.ToString("#,###,###") + " из " + wrp.ContentLength.ToString("#,###,###") + " байт Скорость: " + ((cnt / (pred - start).TotalSeconds) / 1024).ToString("0.00") + " Килобайт в секунду.");
                    }
                    //DateTime finish = DateTime.Now;
                    //Console.WriteLine("Средняя скорость загрузки составила {0} байт в секунду.", ((cnt / (finish - start).TotalSeconds) / 1024).ToString("0.00"));
                }
                if (cnt < wrp.ContentLength)
                {
                    Console.WriteLine("\nОбрыв! Закачка не завершена!");
                    //rStream.Close();
                    //wrp.Close();
                    return 1;
                }
                else
                {
                    Console.WriteLine("\nЗакачка завершена.");
                    //rStream.Close();
                    //wrp.Close();
                    return 0;
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
                return 1;
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                return 1;
            }
            finally
            {
                if (wrp != null)
                {
                    wrp.Close();
                }
                if (rStream != null)
                {
                    rStream.Close();
                }
            }
        }
        static bool DownloadImageFromSankaku(ImageInfo img, string dir, CookieCollection cookies)
        {
            string post = GetPostPage(img.sankaku_post_id, cookies);
            if (post == null) { return false; }
            string url = GetOriginalUrlFromPostPage(post);
            if (url == null) 
            {
                Console.WriteLine("URL Картинки не получен!");
                log.Debug("URL Картинки не получен!");
                return false;
            }
            string filename = GetFileName(img, dir, url);

            Console.WriteLine("Начинаем закачку {0}.", url);
            //Console.WriteLine("Размер на сервере {0} байт.", filesize.ToString("#,###,###"));
            FileInfo fi = new FileInfo(filename);
            HttpWebRequest httpWRQ = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            WebResponse wrp = null;
            Stream rStream = null;
            try
            {
                httpWRQ.Referer = "https://chan.sankakucomplex.com/post/show/" + img.sankaku_post_id.ToString();
                httpWRQ.UserAgent = Erza.Default.UserAgent;
                httpWRQ.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                httpWRQ.Headers.Add("Accept-Encoding: identity");
                httpWRQ.CookieContainer = new CookieContainer();
                httpWRQ.CookieContainer.Add(cookies);
                httpWRQ.Timeout = 60 * 1000;
                wrp = httpWRQ.GetResponse();
                if (fi.Exists)
                {
                    if (wrp.ContentLength == fi.Length)
                    {
                        Console.WriteLine("Уже скачан.");
                        //wrp.Close();
                        return true;
                    }
                    else
                    {
                        fi.Delete();
                    }
                }
                long cnt = 0;
                rStream = wrp.GetResponseStream();
                rStream.ReadTimeout = 60 * 1000;
                using (FileStream fs = new FileStream(filename, FileMode.Create))
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead;
                    DateTime start = DateTime.Now;
                    while ((bytesRead = rStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fs.Write(buffer, 0, bytesRead);
                        cnt += bytesRead;
                        DateTime pred = DateTime.Now;
                        Console.Write("\rСкачано " + cnt.ToString("#,###,###") + " из " + wrp.ContentLength.ToString("#,###,###") + " байт Скорость: " + ((cnt / (pred - start).TotalSeconds) / 1024).ToString("0.00") + " Килобайт в секунду.");
                    }
                    //DateTime finish = DateTime.Now;
                    //Console.WriteLine("Средняя скорость загрузки составила {0} байт в секунду.", ((cnt / (finish - start).TotalSeconds) / 1024).ToString("0.00"));
                }
                if (cnt < wrp.ContentLength)
                {
                    Console.WriteLine("\nОбрыв! Закачка не завершена!");
                    //rStream.Close();
                    //wrp.Close();
                    return false;
                }
                else
                {
                    Console.WriteLine("\nЗакачка завершена.");
                    //rStream.Close();
                    //wrp.Close();
                    return true;
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
                log.Debug(ex.Message);
                return false;
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                log.Debug(ex.Message);
                return false;
            }
            finally
            {
                if (wrp != null)
                {
                    wrp.Close();
                }
                if (rStream != null)
                {
                    rStream.Close();
                }
            }
        }
        static string GetPostPage(int npost, CookieCollection cookies)
        {
            string strURL = "https://chan.sankakucomplex.com/post/show/" + npost.ToString();
            Console.WriteLine("Загружаем и парсим пост: " + strURL);
            //WebClient Client = new WebClient();
            //Uri uri = new Uri(strURL);
            //Client.Headers.Add("user-agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
            while (true)
            {
                try
                {
                    return DownloadStringFromSankaku(strURL, null, cookies);
                    //return Client.DownloadString(uri);
                }
                catch (WebException we)
                {
                    Console.WriteLine(we.Message);
                    log.Debug(we.Message);
                    Thread.Sleep(10000);
                    return null;
                }
            }
        }
        static string GetOriginalUrlFromPostPage(string post)
        {
            string file_url = "<li>Original: <a href=\"";
            //Regex rx = new Regex(file_url + "(?:(?:ht|f)tps?://)?(?:[\\-\\w]+:[\\-\\w]+@)?(?:[0-9a-z][\\-0-9a-z]*[0-9a-z]\\.)+[a-z]{2,6}(?::\\d{1,5})?(?:[?/\\\\#][?!^$.(){}:|=[\\]+\\-/\\\\*;&~#@,%\\wА-Яа-я]*)?", RegexOptions.Compiled);
            Regex rx = new Regex(file_url + @"\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?", RegexOptions.Compiled);
            try
            {
                Match match = rx.Match(post);
                if (match.Success)
                {
                    string url = match.Value.Substring(file_url.Length);
                    return "https:" + url;
                }
                else
                {
                    Regex rx_swf = new Regex("<p><a href=\"" + @"(?<protocol>http(s)?)://(?<server>([A-Za-z0-9-]+\.)*(?<basedomain>[A-Za-z0-9-]+\.[A-Za-z0-9]+))+((:)?(?<port>[0-9]+)?(/?)(?<path>(?<dir>[A-Za-z0-9\._\-/]+)(/){0,1}[A-Za-z0-9.-/_]*)){0,1}" + "\" >Save this flash \\(right click and save\\)</a></p>", RegexOptions.Compiled);
                    Match match_swf = rx_swf.Match(post);
                    if (match_swf.Success)
                    {
                        string url = match_swf.Value.Substring(12).Replace("\" >Save this flash (right click and save)</a></p>", String.Empty);
                        return "https:" + url;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (ArgumentNullException ex)
            {
                //Console.WriteLine(ex.ToString());
                log.Debug("GetOriginalUrlFromPostPage " + ex.Message);
                return null;
            }
        }
        static string GetFileName(ImageInfo img, string dir, string url)
        {
            int temp = url.IndexOf('?');
            if (temp >= 0)
            {
                url = url.Substring(0, temp);
            }
            string extension = Path.GetExtension(url);
            if (extension == ".jpeg")
            {
                return dir + "\\" + img.GetHashString() + ".jpg";
            }
            else
            {
                return dir + "\\" + img.GetHashString() + extension;
            }
        }
        static void InsertHashToDownloadedList(List<string> DownloadedList, string hash)
        {
            int index = DownloadedList.BinarySearch(hash);
            if (index < 0)
            {
                DownloadedList.Insert(~index, hash);
            }
        }
        #endregion
    }
    class json_image
    {
        private int _height;
        private int _width;
        private int _sample_height;
        private int _sample_width;
        private int _score;
        private object _created_at;
        private string _file_url;
        private string _status;
        private string _preview_url;
        private int _file_size;
        private int _preview_width;
        private int _preview_height;
        private string _author;
        private string _tags;
        private bool _has_comments;
        private int _parent_id;
        private string _rating;
        private string _source;
        private int _creator_id;
        private bool _has_notes;
        private string _md5;
        private int _change;
        private int _id;
        private bool _has_children;
        private string _sample_url;
        public int height { get { return _height; } set { _height = value; } }
        public int width { get { return _width; } set { _width = value; } }
        public int sample_height { get { return _sample_height; } set { _sample_height = value; } }
        public int sample_width { get { return _sample_width; } set { _sample_width = value; } }
        public int score { get { return _score; } set { _score = value; } }
        public object created_at { get { return _created_at; } set { _created_at = value; } }
        public string file_url { get { return _file_url; } set { _file_url = value; } }
        public string status { get { return _status; } set { _status = value; } }
        public string preview_url { get { return _preview_url; } set { _preview_url = value; } }
        public object file_size
        {
            get { return _file_size; }
            set
            {
                if (value == null) { _file_size = 0; }
                else { _file_size = (int)value; }
            }
        }
        public int preview_width { get { return _preview_width; } set { _preview_width = value; } }
        public int preview_height { get { return _preview_height; } set { _preview_height = value; } }
        public string author { get { return _author; } set { _author = value; } }
        public string tags { get { return _tags; } set { _tags = value; } }
        public bool has_comments { get { return _has_comments; } set { _has_comments = value; } }
        public object parent_id
        {
            get { return _parent_id; }
            set
            {
                if (value == null) { _parent_id = 0; }
                else
                {
                    _parent_id = System.Convert.ToInt32(value);
                }
            }
        }
        public string rating { get { return _rating; } set { _rating = value; } }
        public string source { get { return _source; } set { _source = value; } }
        public int creator_id { get { return _creator_id; } set { _creator_id = value; } }
        public bool has_notes { get { return _has_notes; } set { _has_notes = value; } }
        public string md5 { get { return _md5; } set { _md5 = value; } }
        public int change { get { return _change; } set { _change = value; } }
        public int id { get { return _id; } set { _id = value; } }
        public bool has_children { get { return _has_children; } set { _has_children = value; } }
        public string sample_url { get { return _sample_url; } set { _sample_url = value; } }
    }
    internal class AcceptAllCertificatePolicy : ICertificatePolicy
    {
        public AcceptAllCertificatePolicy()
        {
        }

        public bool CheckValidationResult(ServicePoint sPoint, X509Certificate cert, WebRequest wRequest, int certProb)
        {
            // Always accept
            return true;
        }
    }
}
