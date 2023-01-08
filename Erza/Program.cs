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
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using ErzaLib;

namespace Erza
{
    class Program
    {
        static ErzaConfig config = null;
        static bool use_sub_dir = true;
        static List<string> tags;
        static CookieCollection gelbooru_cookies = null;
        static SQLiteConnection connection = null;
        static void Main(string[] args)
        {
            try
            {
                LoadSettings();
                List<ImageInfo2> il = new List<ImageInfo2>();
                Program.tags = new List<string>();
                ParseArgs(args);
                if (tags.Count <= 0)
                {
                    Console.WriteLine("Не заданы теги!");
                    return;
                }
                if (Program.config.DownloadPath == ".")
                {
                    Program.config.DownloadPath = Directory.GetCurrentDirectory();
                }
                if (Program.use_sub_dir)
                {
                    Program.config.DownloadPath = Program.config.DownloadPath + @"\" + WebUtility.UrlEncode(Program.tags[0]);
                }
                ServicePointManager.ServerCertificateValidationCallback = ValidationCallback;
                for (int i = 0; i < Program.tags.Count; i++)
                {
                    if (Program.config.UseKonachan)
                    {
                        Console.WriteLine("Импортируем тег " + Program.tags[i] + " с коначан");
                        il = SliyanieLists(il, get_hash_konachan(WebUtility.UrlEncode(Program.tags[i])));
                    }
                    if (Program.config.UseDanbooru)
                    {
                        Console.WriteLine("Импортируем тег " + Program.tags[i] + " с данбуры");
                        il = SliyanieLists(il, get_hash_danbooru_new_api(WebUtility.UrlEncode(Program.tags[i])));
                    }
                    if (Program.config.UseYandere)
                    {
                        Console.WriteLine("Импортируем тег " + Program.tags[i] + " с сестрёнки");
                        il = SliyanieLists(il, get_hash_imouto(WebUtility.UrlEncode(Program.tags[i])));
                    }
                    if (Program.config.UseGelbooru)
                    {
                        Console.WriteLine("Импортируем тег " + Program.tags[i] + " с гелбуры");
                        il = SliyanieLists(il, get_hash_gelbooru(WebUtility.UrlEncode(Program.tags[i])));
                    }
                }
                if (il.Count <= 0)
                {
                    Console.WriteLine("Ничего ненайдено.");
                    return;
                }
                connection = new SQLiteConnection(Program.config.ConnectionString);
                connection.Open();
                #region SQLite
                if (Program.config.UseDB)
                {
                    Console.WriteLine("Добавляем хэши в базу данных SQLite");
                    DateTime start = DateTime.Now;
                    SQLiteTransaction transact = connection.BeginTransaction();
                    for (int i = 0; i < il.Count; i++)
                    {
                        ErzaLib.ErzaDB.LoadImageToErza(il[i], connection);
                        Console.Write("Обрабатываю хэш {0} ({1}/{2})\r", il[i].Hash, i, il.Count);
                    }
                    transact.Commit();
                    DateTime finish = DateTime.Now;
                    Console.WriteLine("\nХэшей добавлено: {0} за: {1} секунд ({2} в секунду)", il.Count.ToString(), (finish - start).TotalSeconds.ToString("0.00"), (il.Count / (finish - start).TotalSeconds).ToString("0.00"));
                }
                #endregion
                #region Download
                if (Program.config.Download)
                {
                    Console.Write("\n\n\n\t\tНАЧИНАЕТСЯ ЗАГРУЗКА\n\n\n");
                    int num6 = download(il, Program.config.DownloadPath);
                }
                #endregion
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("НЕ ОБРАБОТАННОЕ ИСКЛЮЧЕНИЕ: " + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }
        static void LoadSettings()
        {
            Program.config = new ErzaConfig();
            //Параметры по умолчанию
            Program.config.ConnectionString = @"data source=C:\utils\data\erza.sqlite";
            Program.config.UseDanbooru = true;
            Program.config.UseDB = true;
            Program.config.UseGelbooru = true;
            Program.config.UseKonachan = true;
            Program.config.UseYandere = true;
            Program.config.UserAgent = "Mozilla / 5.0(Windows NT 6.2; WOW64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 34.0.1847.116 Safari / 537.36";
            Program.config.LimitError = 4;
            Program.config.GelbooruLogin = "";
            Program.config.GelbooruPassword = "";
            Program.config.DanbooruLogin = "";
            Program.config.DanbooruPassword = "";
            Program.config.DanbooruAPIKey = "";
            Program.config.Download = true;
            Program.config.DownloadPath = ".";
            Program.config.UseProxy = false;
            Program.config.ProxyAddress = null;
            Program.config.ProxyPort = 0;
            Program.config.ProxyLogin = null;
            Program.config.ProxyPassword = null;
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(ErzaConfig));
            if (File.Exists(@"C:\utils\cfg\Erza.json"))
            {
                using (FileStream fs = new FileStream(@"C:\utils\cfg\Erza.json", FileMode.Open))
                {
                    Program.config = (ErzaConfig)jsonFormatter.ReadObject(fs);
                }
                return;
            }
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Erza\\Erza.json"))
            {
                using (FileStream fs = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Erza\\Erza.json", FileMode.Open))
                {
                    Program.config = (ErzaConfig)jsonFormatter.ReadObject(fs);
                }
                return;
            }
            Console.WriteLine("Конфигурационный файл не найден!\nЗагружены настройки по умолчанью.");
        }
        static void ParseArgs(string[] args)
        {
            string sqlite_string = "--sqlite";
            string nosqlite_string = "--nosqlite";
            string sqlite_path_string = "--sqlite-path=";
            string dir_string = "--dir=";
            string download_string = "--download";
            string nodownload_string = "--nodownload";
            string nosubdir_string = "--nosubdir";
            foreach (string param in args)
            {
                if (param == nosubdir_string)
                {
                    Program.use_sub_dir = false;
                    continue;
                }
                if (param == sqlite_string)
                {
                    Program.config.UseDB = true;
                    continue;
                }
                if (param == nosqlite_string)
                {
                    Program.config.UseDB = false;
                    continue;
                }
                if (param == download_string)
                {
                    Program.config.Download = true;
                    continue;
                }
                if (param == nodownload_string)
                {
                    Program.config.Download = false;
                    continue;
                }
                if (param.Length >= sqlite_path_string.Length)
                {
                    if (param.Substring(0, sqlite_path_string.Length) == sqlite_path_string)
                    {
                        Program.config.ConnectionString = "data source=" + param.Substring(sqlite_path_string.Length);
                        continue;
                    }
                }
                if (param.Length >= dir_string.Length)
                {
                    if (param.Substring(0, dir_string.Length) == dir_string)
                    {
                        Program.config.DownloadPath = param.Substring(dir_string.Length);
                        continue;
                    }
                }
                Program.tags.Add(param);
            }
        }
        static List<ImageInfo2> get_hash_danbooru_new_api(string tag)
        {
            const int DANBOORU_LIMIT_POSTS = 100;
            int nPage = 1;                //Счетчик страниц
            List<ImageInfo2> img_list = new List<ImageInfo2>();
            int count_errors = 0;
            for (; ; )
            {
                WebClient Client = new WebClient();
                Client.Headers["User-Agent"] = Program.config.UserAgent;
                if (Program.config.UseProxy)
                {
                    WebProxy myProxy = new WebProxy(Program.config.ProxyAddress, Program.config.ProxyPort);
                    myProxy.Credentials = new NetworkCredential(Program.config.ProxyLogin, Program.config.ProxyPassword);
                    Client.Proxy = myProxy;
                }
                string strURL = String.Format("https://danbooru.donmai.us/posts.xml?tags={0}&page={1}&login={2}&api_key={3}&limit={4}", tag, nPage, Program.config.DanbooruLogin, Program.config.DanbooruAPIKey, DANBOORU_LIMIT_POSTS);
                Console.WriteLine("({0}/ХЗ) Загружаем и парсим: {1}", img_list.Count, strURL);
                try
                {
                    Uri uri = new Uri(strURL);
                    DateTime start = DateTime.Now;
                    string xml = Client.DownloadString(uri);
                    if (xml == null) {
                        if (count_errors < Program.config.LimitError)
                        {
                            count_errors++;
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                    List<ImageInfo2> list = ParseXMLDanBooru_new_api(xml);
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
        static List<ImageInfo2> get_hash_konachan(string tag)
        {
            const int KONACHAN_LIMIT_POSTS = 100;
            int nPostsCount = 0;          //Счетчик постов для скачивания
            int nPage = 1;                //Счетчик страниц
            List<ImageInfo2> img_list = new List<ImageInfo2>();
            int count = 0;
            while (true)
            {
                nPostsCount = posts_count(String.Format("http://konachan.com/post.xml?tags={0}&limit=1", tag));
                if (nPostsCount >= 0)
                {
                    break;
                }
                count++;
                if (count >= Program.config.LimitError)
                {
                    Console.WriteLine("Не удалось получить количество постов!");
                }
            }
            if (nPostsCount == 0)
            {
                Console.WriteLine("Ничего ненайдено!");
                return img_list;
            }
            WebClient Client = new WebClient();
            if (Program.config.UseProxy)
            {
                WebProxy myProxy = new WebProxy(Program.config.ProxyAddress, Program.config.ProxyPort);
                myProxy.Credentials = new NetworkCredential(Program.config.ProxyLogin, Program.config.ProxyPassword);
                Client.Proxy = myProxy;
            }
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
                        if (count_errors < Program.config.LimitError)
                        {
                            count_errors++;
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                    List<ImageInfo2> list = ParseXMLKonachan(xml);
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
        static List<ImageInfo2> get_hash_imouto(string tag)
        {
            const int YANDERE_LIMIT_POSTS = 100;
            int nPostsCount = 0;          //Счетчик постов для скачивания
            int nPage = 1;                //Счетчик страниц
            List<ImageInfo2> img_list = new List<ImageInfo2>();
            int count = 0;
            while (true)
            {
                nPostsCount = posts_count(String.Format("https://yande.re/post.xml?tags={0}&limit=1", tag));
                if (nPostsCount >= 0)
                {
                    break;
                }
                count++;
                if (count >= Program.config.LimitError)
                {
                    Console.WriteLine("Не удалось получить количество постов!");
                }
            }
            if (nPostsCount == 0)
            {
                Console.WriteLine("Ничего ненайдено!");
                return img_list;
            }
            WebClient Client = new WebClient();
            if (Program.config.UseProxy)
            {
                WebProxy myProxy = new WebProxy(Program.config.ProxyAddress, Program.config.ProxyPort);
                myProxy.Credentials = new NetworkCredential(Program.config.ProxyLogin, Program.config.ProxyPassword);
                Client.Proxy = myProxy;
            }
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
                        if (count_errors < Program.config.LimitError)
                        {
                            count_errors++;
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                    List<ImageInfo2> list = ParseXMLYandere(xml);
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
                    Thread.Sleep(60000);
                    continue;
                }
            }
            Client.Dispose();
            return img_list;
        }
        static List<ImageInfo2> get_hash_gelbooru(string tag)
        {
            if (gelbooru_cookies == null)
            {
                gelbooru_cookies = GetGelbooruCookies(Program.config.GelbooruLogin, Program.config.GelbooruPassword);
            }
            if(gelbooru_cookies == null)
            {
                Console.WriteLine("Не удалось авторизоваться на Gelbooru!");
                return null;
            }
            const int GELBOORU_LIMIT_POSTS = 100;
            int nPostsCount = 0;          //Счетчик постов для скачивания
            int nPage = 0;                //Счетчик страниц
            List<ImageInfo2> img_list = new List<ImageInfo2>();
            int count = 0;
            while (true)
            {
                nPostsCount = posts_count_gelbooru(String.Format("http://gelbooru.com/index.php?page=dapi&s=post&q=index&tags={0}&pid=0&limit=1", tag));
                if (nPostsCount >= 0)
                {
                    break;
                }
                count++;
                if (count >= Program.config.LimitError)
                {
                    Console.WriteLine("Не удалось получить количество постов!");
                }
            }
            if (nPostsCount == 0)
            {
                Console.WriteLine("Ничего ненайдено!");
                return img_list;
            }
            int count_errors = 0;
            for (; ; )
            {
                string strURL = String.Format("http://gelbooru.com/index.php?page=dapi&s=post&q=index&tags={0}&pid={1}&limit={2}", tag, nPage, GELBOORU_LIMIT_POSTS);
                Console.WriteLine("({0}/{1}) Загружаем и парсим: {2}", img_list.Count, nPostsCount, strURL);
                try
                {
                    Uri uri = new Uri(strURL);
                    DateTime start = DateTime.Now;
                    string xml = DownloadStringFromGelbooru(strURL, "http://gelbooru.com/", gelbooru_cookies);
                    if (xml == null)
                    {
                        if (count_errors < Program.config.LimitError)
                        {
                            count_errors++;
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                    List<ImageInfo2> list = ParseXMLGelBooru(xml);
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
        static CookieCollection GetGelbooruCookies(string user, string password)
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create("http://gelbooru.com/index.php?page=account&s=login&code=00");
                if (Program.config.UseProxy)
                {
                    WebProxy myProxy = new WebProxy(Program.config.ProxyAddress, Program.config.ProxyPort);
                    myProxy.Credentials = new NetworkCredential(Program.config.ProxyLogin, Program.config.ProxyPassword);
                    httpWebRequest.Proxy = myProxy;
                }
                httpWebRequest.CookieContainer = new CookieContainer();
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                string PostData = String.Format("user={0}&pass={1}", user, password);
                Encoding encoding = Encoding.UTF8;
                byte[] byte1 = encoding.GetBytes(PostData);
                httpWebRequest.ContentLength = byte1.Length;
                using (Stream st = httpWebRequest.GetRequestStream()) {
                    st.Write(byte1, 0, byte1.Length);
                    st.Close();
                }
                httpWebRequest.AllowAutoRedirect = false;
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                //using (StreamReader reader = new StreamReader(httpWebResponse.GetResponseStream()))
                //{
                //string s = reader.ReadToEnd();
                //}
                return httpWebResponse.Cookies;
            }
            catch (WebException we)
            {
                Console.WriteLine(we.Message);
                return null;
            }
        }
        static int posts_count_gelbooru(string url)
        {
            int nLocalPostsCount = 0;
            try
            {
                Uri uri = new Uri(url);
                //Client.Headers.Add("Cookie", "user_id = 42820; pass_hash = 12b71a982c0c189c7a0c9ac25d9713213296f616");
                //string xml = Client.DownloadString(uri);
                string xml = DownloadStringFromGelbooru(url, "http://gelbooru.com/", gelbooru_cookies);
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
                nLocalPostsCount = -1;
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine(ex.Message);
                nLocalPostsCount = -1;
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex.Message);
                nLocalPostsCount = -1;
            }
            catch (XmlException ex)
            {
                Console.WriteLine(ex.Message);
                nLocalPostsCount = -1;
            }
            return nLocalPostsCount;
        }
        static int posts_count(string url)
        {
            int nLocalPostsCount = 0;
            WebClient Client = new WebClient();
            if (Program.config.UseProxy)
            {
                WebProxy myProxy = new WebProxy(Program.config.ProxyAddress, Program.config.ProxyPort);
                myProxy.Credentials = new NetworkCredential(Program.config.ProxyLogin, Program.config.ProxyPassword);
                Client.Proxy = myProxy;
            }
            try
            {
                Uri uri = new Uri(url);
                string xml = Client.DownloadString(uri);
                //string xml = DownloadStringFromGelbooru(url, "http://gelbooru.com/", gelbooru_cookies);
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
                nLocalPostsCount = -1;
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine(ex.Message);
                nLocalPostsCount = -1;
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex.Message);
                nLocalPostsCount = -1;
            }
            catch (XmlException ex)
            {
                Console.WriteLine(ex.Message);
                nLocalPostsCount = -1;
            }
            finally
            {
                Client.Dispose();
            }
            return nLocalPostsCount;
        }
        public static string DownloadStringFromGelbooru(string url, string referer, CookieCollection cookies)
        {
            HttpWebRequest downloadRequest = (HttpWebRequest)WebRequest.Create(url);
            if (Program.config.UseProxy)
            {
                WebProxy myProxy = new WebProxy(Program.config.ProxyAddress, Program.config.ProxyPort);
                myProxy.Credentials = new NetworkCredential(Program.config.ProxyLogin, Program.config.ProxyPassword);
                downloadRequest.Proxy = myProxy;
            }
            downloadRequest.UserAgent = Program.config.UserAgent;
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
        static List<ImageInfo2> ParseXMLKonachan(string strXML)
        {
            List<ImageInfo2> list = new List<ImageInfo2>();
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
                ImageInfo2 mImgDescriptor = new ImageInfo2();
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
                        //mImgDescriptor.konachan_url = "http:" + node.Attributes[j].Value;
                        mImgDescriptor.konachan_url = node.Attributes[j].Value;
                        mImgDescriptor.urls.Add(mImgDescriptor.konachan_url);
                    }
                    if (node.Attributes[j].Name == "id")
                    {
                        mImgDescriptor.konachan_post_id = System.Convert.ToInt32(node.Attributes[j].Value);
                    }
                    if (node.Attributes[j].Name == "height")
                    {
                        mImgDescriptor.Height = System.Convert.ToInt32(node.Attributes[j].Value);
                    }
                    if (node.Attributes[j].Name == "width")
                    {
                        mImgDescriptor.Width = System.Convert.ToInt32(node.Attributes[j].Value);
                    }
                }
                list.Add(mImgDescriptor); //Добавляем в список
            }
            return list;
        }
        static List<ImageInfo2> ParseXMLGelBooru(string strXML)
        {
            List<ImageInfo2> list = new List<ImageInfo2>();
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
                ImageInfo2 mImgDescriptor = new ImageInfo2();
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
                        //mImgDescriptor.gelbooru_url = "http:" + node.Attributes[j].Value.Replace("simg4.", String.Empty);
                        mImgDescriptor.gelbooru_url = node.Attributes[j].Value.Replace("simg4.", String.Empty);
                        mImgDescriptor.gelbooru_url = mImgDescriptor.gelbooru_url.Replace("simg3.", String.Empty);
                        mImgDescriptor.urls.Add(mImgDescriptor.gelbooru_url);
                        
                    }
                    if (node.Attributes[j].Name == "id")
                    {
                        mImgDescriptor.gelbooru_post_id = System.Convert.ToInt32(node.Attributes[j].Value);
                    }
                    if (node.Attributes[j].Name == "height")
                    {
                        mImgDescriptor.Height = System.Convert.ToInt32(node.Attributes[j].Value);
                    }
                    if (node.Attributes[j].Name == "width")
                    {
                        mImgDescriptor.Width = System.Convert.ToInt32(node.Attributes[j].Value);
                    }
                }
                list.Add(mImgDescriptor);
            }
            return list;
        }
        static List<ImageInfo2> ParseXMLYandere(string strXML)
        {
            List<ImageInfo2> list = new List<ImageInfo2>();
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
                ImageInfo2 mImgDescriptor = new ImageInfo2();
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
                    if (node.Attributes[j].Name == "height")
                    {
                        mImgDescriptor.Height = System.Convert.ToInt32(node.Attributes[j].Value);
                    }
                    if (node.Attributes[j].Name == "width")
                    {
                        mImgDescriptor.Width = System.Convert.ToInt32(node.Attributes[j].Value);
                    }
                }
                list.Add(mImgDescriptor);
            }
            return list;
        }
        static List<ImageInfo2> ParseXMLDanBooru_new_api(string strXML)
        {
            List<ImageInfo2> list = new List<ImageInfo2>();
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
                ImageInfo2 mImgDescriptor = new ImageInfo2();
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
                //mImgDescriptor.urls.Add("http://danbooru.donmai.us/data/" + md5.InnerText + "." + ext.InnerText);
                //mImgDescriptor.danbooru_url = "http://danbooru.donmai.us/data/" + md5.InnerText + "." + ext.InnerText;
                XmlElement url = node["file-url"];
                //mImgDescriptor.FilePath = "http://danbooru.donmai.us" + url.InnerText;
                /*if (url.InnerText.IndexOf("https://raikou1.donmai.us") == 0)
                {
                    mImgDescriptor.urls.Add(url.InnerText);
                    mImgDescriptor.danbooru_url = url.InnerText;
                }
                else if (url.InnerText.IndexOf("https://hijiribe.donmai.us") == 0)
                {
                    mImgDescriptor.urls.Add(url.InnerText);
                    mImgDescriptor.danbooru_url = url.InnerText;
                }
                else
                {
                    mImgDescriptor.urls.Add("http://danbooru.donmai.us" + url.InnerText);
                    mImgDescriptor.danbooru_url = "http://danbooru.donmai.us" + url.InnerText;
                }*/
                if ((url.InnerText.IndexOf("https://") == 0) || (url.InnerText.IndexOf("http://") == 0))
                {
                    mImgDescriptor.urls.Add(url.InnerText);
                    mImgDescriptor.danbooru_url = url.InnerText;
                }
                else
                {
                    mImgDescriptor.urls.Add("https://danbooru.donmai.us" + url.InnerText);
                    mImgDescriptor.danbooru_url = "https://danbooru.donmai.us" + url.InnerText;
                }
                XmlElement height = node["image-height"];
                XmlElement width = node["image-width"];
                if (!String.IsNullOrEmpty(height.InnerText) & !String.IsNullOrEmpty(width.InnerText))
                {
                    mImgDescriptor.Height = System.Convert.ToInt32(height.InnerText);
                    mImgDescriptor.Width = System.Convert.ToInt32(width.InnerText);
                }
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
        static int FindHash(List<ImageInfo2> list, ImageInfo2 img)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Hash.SequenceEqual(img.Hash))
                {
                    return i;
                }
            }
            return -1;
        }
        static List<ImageInfo2> SliyanieLists(List<ImageInfo2> list, List<ImageInfo2> temp)
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
                    list[t].AddTags(temp[temp_i].Tags);
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
        static int download(List<ImageInfo2> list, string dir)
        {
            int count_complit = 0;
            int count_deleted = 0;
            int count_error = 0;
            int count_skip = 0;
            Directory.CreateDirectory(dir);
            for (int i = 0; i < list.Count; i++)
            {
                Console.WriteLine("\n###### {0}/{1} ######", (i+1), list.Count);
                if (Program.config.UseDB)
                {
                    ImageInfo img = ErzaLib.ErzaDB.GetImageWithOutTags(list[i].Hash, connection);
                    if (img != null)
                    {
                        if (img.IsDeleted)
                        {
                            Console.WriteLine("Этот фаил уже был ранее удалён.");
                            count_deleted++;
                            continue;
                        }
                        if (img.FilePath != null)
                        {
                            Console.WriteLine("Этот фаил уже был ранее скачан.");
                            Console.WriteLine(img.FilePath);
                            count_skip++;
                            continue;
                        }
                    }
                }
                long r = DownloadImage(list[i], dir);
                if (r == 0)
                { 
                    count_complit++;
                }
                else
                {
                    count_error++;
                }
            }
            Console.WriteLine("Успешно скачано: {0}\nСкачано ренее: {1}\nУдалено ранее: {2}\nОшибочно: {3}\nВсего: {4}", count_complit, count_skip, count_deleted, count_error, list.Count);
            return 0;
        }
        static long DownloadImage(ImageInfo2 img, string dir)
        {
            int cnt;
            if (Program.config.LimitError < 1)
            {
                cnt = 1;
            }
            else
            {
                cnt = Program.config.LimitError;
            }
            for (int index = 0; index < cnt; index++)
            {
                foreach (string url in img.urls)
                {
                    string extension = Path.GetExtension(url);
                    DateTime start = DateTime.Now;
                    string filename;
                    if (extension == ".jpeg")
                    {
                        filename = dir + "\\" + img.Hash + ".jpg";
                        long result = DownloadFile(url, filename, GetReferer(url, img));
                        if (result == 0)
                        {
                            if (Program.config.UseDB)
                            {
                                ErzaDB.SetImagePath(Path.GetFileNameWithoutExtension(filename), filename.ToLower(), connection);
                            }
                            MyWait(start, 2500);
                            return 0;
                        }
                    }
                    else
                    {
                        filename = dir + "\\" + img.Hash + extension;
                        long result = DownloadFile(url, filename, GetReferer(url, img));
                        if (result == 0)
                        {
                            if (Program.config.UseDB)
                            {
                                ErzaDB.SetImagePath(Path.GetFileNameWithoutExtension(filename), filename.ToLower(), connection);
                            }
                            MyWait(start, 2500);
                            return 0;
                        }
                    }
                }
            }
            return 1;
        }
        static string GetReferer(string url, ImageInfo2 img)
        {
            if (url.LastIndexOf("gelbooru.com/") >= 0)
            {
                Uri uri = new Uri("http://gelbooru.com/index.php?page=post&s=view&id=" + img.gelbooru_post_id.ToString());
                return uri.AbsoluteUri;
            }
            if (url.LastIndexOf("http://konachan.com/") >= 0)
            {
                Uri uri = new Uri("http://konachan.com/post/show/" + img.konachan_post_id.ToString() + "/" + img.GetStringOfTags().Replace(' ', '-'));
                return uri.AbsoluteUri;
            }
            if (url.LastIndexOf("https://danbooru.donmai.us/") >= 0)
            {
                Uri uri = new Uri("https://danbooru.donmai.us/posts/" + img.danbooru_post_id.ToString());
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
            if (Program.config.UseProxy)
            {
                WebProxy myProxy = new WebProxy(Program.config.ProxyAddress, Program.config.ProxyPort);
                myProxy.Credentials = new NetworkCredential(Program.config.ProxyLogin, Program.config.ProxyPassword);
                httpWRQ.Proxy = myProxy;
            }
            WebResponse wrp = null;
            Stream rStream = null;
            try
            {
                httpWRQ.Referer = referer;
                httpWRQ.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:84.0) Gecko/20100101 Firefox/84.0";
                httpWRQ.Timeout = 60 * 1000;
                wrp = httpWRQ.GetResponse();
                if (fi.Exists)
                {
                    if (wrp.ContentLength == fi.Length)
                    {
                        Console.WriteLine("Уже скачан.");
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
                }
                if (cnt < wrp.ContentLength)
                {
                    Console.WriteLine("\nОбрыв! Закачка не завершена!");
                    return 1;
                }
                else
                {
                    Console.WriteLine("\nЗакачка завершена.");
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
        #endregion
    }
    class ImageInfo2 : ErzaLib.ImageInfo
    {
        public List<string> urls = new List<string>();
        public int sankaku_post_id = 0;
        public int danbooru_post_id = 0;
        public int gelbooru_post_id = 0;
        public int konachan_post_id = 0;
        public int yandere_post_id = 0;
        public string sankaku_url = null;
        public string danbooru_url = null;
        public string gelbooru_url = null;
        public string konachan_url = null;
        public string yandere_url = null;
        public void SetHashString(string hash_string)
        {
            this.Hash = hash_string;
        }
        public override string ToString()
        {
            if (this.FilePath != String.Empty)
            {
                return this.FilePath.Substring(this.FilePath.LastIndexOf('\\') + 1);
            }
            else
            {
                return "No File!";
            }
        }
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
    [DataContract]
    public class ErzaConfig
    {
        [DataMember]
        public string ConnectionString;
        [DataMember]
        public bool UseGelbooru;
        [DataMember]
        public bool UseDanbooru;
        [DataMember]
        public bool UseYandere;
        [DataMember]
        public bool UseKonachan;
        [DataMember]
        public bool UseDB;
        [DataMember]
        public bool Download;
        [DataMember]
        public string DownloadPath;
        [DataMember]
        public int LimitError;
        [DataMember]
        public string DanbooruLogin;
        [DataMember]
        public string DanbooruPassword;
        [DataMember]
        public string DanbooruAPIKey;
        [DataMember]
        public string GelbooruLogin;
        [DataMember]
        public string GelbooruPassword;
        [DataMember]
        public string UserAgent;
        [DataMember]
        public bool UseProxy;
        [DataMember]
        public string ProxyAddress;
        [DataMember]
        public int ProxyPort;
        [DataMember]
        public string ProxyLogin;
        [DataMember]
        public string ProxyPassword;

        public ErzaConfig()
        {
        }
    }
}
