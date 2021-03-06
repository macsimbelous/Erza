﻿/* Copyright © Macsim Belous 2012 */
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
    along with Foobar.  If not, see <http://www.gnu.org/licenses/>.*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using System.Threading;
using System.Globalization;
using System.Data.SQLite;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.Json;
using ErzaLib;

namespace GetIdol
{
    class Program
    {
        static CookieCollection sankaku_cookies = null;
        static string AuthToken = null;
        static string RawCookies = null;
        static int StartPage = 1;
        static int MaxPage = 0;
        static List<string> Tags = null;
        static GetidolConfig config = null;
        static SQLiteConnection connection = null;
        static SQLiteConnection getidoldb = null;
        static int count_complit = 0;
        static int count_deleted = 0;
        static int count_error = 0;
        static int count_skip = 0;
        static string store_file = null;
        static bool ClearCacheFlag = false;
        static bool OnlyCacheFlag = false;
        static int PreviewWidth = 300;
        static int PreviewHeight = 225;
        //string previews = "data source=C:\\utils\\data\\previews.sqlite";
        static string previews = "data source=E:\\previews.sqlite";
        static SQLiteConnection prev_conn = new SQLiteConnection(previews);
        static readonly object prevlock = new object();
        static readonly object prevlock2 = new object();
        static int thread_count = 0;
        static void Main(string[] args)
        {
            bool restore_cache = false;
            string tags = null;
            LoadSettings();
            ParseArgs(args);
            ServicePointManager.ServerCertificateValidationCallback = ValidationCallback;
            getidoldb = new SQLiteConnection("data source=C:\\utils\\data\\getidol.sqlite");
            getidoldb.Open();
            connection = new SQLiteConnection(Program.config.ConnectionString);
            connection.Open();
            prev_conn.Open();
            List<CacheItem> post_ids;
            string current_path = Directory.GetCurrentDirectory();
            if (ClearCacheFlag)
            {
                Console.Write("Очишаю кэш...");
                ClearCache();
                VacuumGetIdolDB();
                Console.WriteLine("Готово");
                return;
            }
            Console.Write("Авторизуемся на Санкаке...");
            int temp = 0;
            for (; ; )
            {
                //sankaku_cookies = GetSankakuCookies(Program.config.BaseURL + "user/authenticate");
                AuthToken = GetAuthTokenFromSankaku("https://capi-v2.sankakucomplex.com/auth/token", config.UserAgent, config.SankakuLogin, config.SankakuPassword);
                if (AuthToken != null)
                {
                    Console.WriteLine("Готово");
                    break;
                }
                else
                {
                    if (temp < Program.config.LimitErrors)
                    {
                        temp++;
                        Thread.Sleep(Program.config.TimeOut);
                        Console.WriteLine("Сбой!");
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("Сбой!\nНе удалось получить куки!");
                        return;
                    }
                }
            }
            if (Tags.Count > 0)
            {
                StringBuilder tags_builder = new StringBuilder();
                for (int i = 0; i < Tags.Count; i++)
                {
                    if (i == 0)
                    {
                        tags_builder.Append(WebUtility.UrlEncode(Tags[i]));
                    }
                    else
                    {
                        tags_builder.Append("+");
                        tags_builder.Append(WebUtility.UrlEncode(Tags[i]));
                    }
                }
                tags = tags_builder.ToString();
                Console.WriteLine($"Импортируем тег {tags} с санкаки");
                post_ids = GetImageInfoFromSankaku(tags);
                Console.Write("Сохраняю список в кэш...");
                AddItemsToCache(post_ids);
                Console.WriteLine("Готово");
                if (OnlyCacheFlag)
                {
                    Console.WriteLine("Указана опция --only-cache, прекращаю работу");
                    return;
                }
            }
            else
            {
                Console.Write("Востанавливаю список из кэша...");
                post_ids = GetItemsFromCache();
                if (post_ids.Count <= 0)
                {
                    Console.WriteLine("Готово\nКэш пуст!");
                    return;
                }
                restore_cache = true;
                Console.WriteLine($"Готово\nВостанавлено {post_ids.Count} элемента.");
            }
            Console.WriteLine("\nНачинаю загрузку картинок\n");
            if (restore_cache)
            {
                List<string> list = GetTagsFromCache();
                foreach (string tag in list)
                {
                    Directory.CreateDirectory(current_path + "\\" + tag);
                }
            }
            else
            {
                Directory.CreateDirectory(current_path + "\\" + tags);
            }
            for (int i = 0; i < post_ids.Count; i++)
            {
                Console.WriteLine($"\nПост {post_ids[i].PostID} {(i + 1)}/{post_ids.Count}");
                for (int index = 0; index < Program.config.LimitErrors; index++)
                {
                    if (ExistPostIDFromPosts(post_ids[i].PostID))
                    {
                        Console.WriteLine("Этот пост уже был ранее скачан.");
                        RemoveItemFromCache(post_ids[i].PostID);
                        break;
                    }
                    //DateTime start = DateTime.Now;
                    if (DownloadImageFromSankaku(post_ids[i], current_path + "\\" + post_ids[i].Tag, sankaku_cookies))
                    {
                        //MyWait(start, 5000);
                        //count_complit++;
                        break;
                    }
                    //MyWait(start, 7000);
                    if (index == 0)
                    {
                        count_error++;
                    }
                }
            }
            Console.WriteLine("Успешно скачано: {0}\nСкачано ренее: {1}\nУдалено ранее: {2}\nОшибочно: {3}\nВсего: {4}", count_complit, count_skip, count_deleted, count_error, post_ids.Count);
            VacuumGetIdolDB();
            connection.Close();
            getidoldb.Close();
            while (true)
            {
                if (thread_count > 0)
                {
                    Thread.Sleep(0);
                    continue;
                }
                else
                {
                    break;
                }
            }
            prev_conn.Close();
            return;
        }
        static void LoadSettings()
        {
            Program.config = new GetidolConfig();
            //Параметры по умолчанию
            Program.config.BaseURL = "https://chan.sankakucomplex.com/";
            Program.config.TimeOut = 5 * 1000;
            Program.config.TimeOutError = (5 * 60) * 1000;
            Program.config.ConnectionString = @"data source=C:\utils\data\erza.sqlite";
            Program.config.UseDB = false;
            Program.config.UserAgent = "Mozilla / 5.0(Windows NT 6.2; WOW64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 34.0.1847.116 Safari / 537.36";
            Program.config.LimitErrors = 2;
            Program.config.SankakuLogin = null;
            Program.config.SankakuPassword = null;
            Program.config.UseProxy = false;
            Program.config.ProxyAddress = null;
            Program.config.ProxyPort = 3128;
            Program.config.ProxyLogin = null;
            Program.config.ProxyPassword = null;
            Program.config.DownloadPath = ".";
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(GetidolConfig));
            //jsonFormatter.WriteObject(System.IO.File.Create(".\\test.json"), Program.config);
            if (File.Exists(@"C:\utils\cfg\Getidol.json"))
            {
                using (FileStream fs = new FileStream(@"C:\utils\cfg\Getidol.json", FileMode.Open))
                {
                    Program.config = (GetidolConfig)jsonFormatter.ReadObject(fs);
                }
                return;
            }
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Getidol\\Getidol.json"))
            {
                using (FileStream fs = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Getidol\\Getidol.json", FileMode.Open))
                {
                    Program.config = (GetidolConfig)jsonFormatter.ReadObject(fs);
                }
                return;
            }
            if (File.Exists(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]) + "\\Getidol.json"))
            {
                using (FileStream fs = new FileStream(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]) + "\\Getidol.json", FileMode.Open))
                {
                    Program.config = (GetidolConfig)jsonFormatter.ReadObject(fs);
                }
                return;
            }
            Console.WriteLine("Конфигурационный файл не найден!\nЗагружены настройки по умолчанью.");
        }
        static void ParseArgs(string[] args)
        {
            string start_page_string = "--start-page=";
            string max_page_string = "--max-page=";
            string clear_cache_string = "--clear-cache";
            string only_cache_string = "--only-cache";
            Program.Tags = new List<string>();
            foreach (string param in args)
            {
                if (param == clear_cache_string)
                {
                    ClearCacheFlag = true;
                    continue;
                }
                if (param == only_cache_string)
                {
                    OnlyCacheFlag = true;
                    continue;
                }
                if (param.Length >= start_page_string.Length)
                {
                    if (param.Substring(0, start_page_string.Length) == start_page_string)
                    {
                        if (param.Length > start_page_string.Length)
                        {
                            Program.StartPage = int.Parse(param.Substring(start_page_string.Length));
                            if(Program.StartPage < 1)
                            {
                                Console.WriteLine("Параметр {0} не может быть меньше 1", param);
                                Environment.Exit(1);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Не правильно задан параметр {0}", param);
                            Environment.Exit(1);
                        }
                        continue;
                    }
                }
                if (param.Length >= max_page_string.Length)
                {
                    if (param.Substring(0, max_page_string.Length) == max_page_string)
                    {
                        if (param.Length > max_page_string.Length)
                        {
                            Program.MaxPage = int.Parse(param.Substring(max_page_string.Length));
                            if (Program.MaxPage < 0)
                            {
                                Console.WriteLine("Параметр {0} не может быть меньше 0", param);
                                Environment.Exit(1);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Не правильно задан параметр {0}", param);
                            Environment.Exit(1);
                        }
                        continue;
                    }
                }
                Program.Tags.Add(param);
            }
        }
        static bool DownloadImageFromSankaku(CacheItem Item, string dir, CookieCollection cookies)
        {
            Thread.Sleep(Program.config.TimeOut);
            string post = GetPostPage(Item.PostID);
            if (post == null) { return false; }
            JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions();
            jsonSerializerOptions.IgnoreNullValues = true;
            SankakuJson[] sankakuJson = JsonSerializer.Deserialize<SankakuJson[]>(post, jsonSerializerOptions);
            if(sankakuJson.Length == 0) { return false; }
            string url = sankakuJson[0].file_url;
            //string url = GetOriginalUrlFromPostPage(post);
            if (url == null)
            {
                Console.WriteLine("URL Картинки не получен!");
                return false;
            }
            string filename = GetFileName(dir, url);
            /*if (!IsImageFile(filename))
            {
                return true;
            }*/
            Console.Write("Получаем теги и добавляем в БД...");
            //DateTime start_db = DateTime.Now;
            string hash = Path.GetFileNameWithoutExtension(url);
            //int tc = GetTagsFromSankaku(hash, post);
            List<string> tags = new List<string>();
            foreach(tag t in sankakuJson[0].tags)
            {
                tags.Add(t.name);
            }
            ImageInfo img = new ImageInfo();
            img.Hash = sankakuJson[0].md5;
            img.Tags.AddRange(tags);
            img.IsDeleted = false;
            SQLiteTransaction transact = Program.connection.BeginTransaction();
            ErzaDB.LoadImageToErza(img, Program.connection);
            transact.Commit();
            //DateTime stop_db = DateTime.Now;
            //Console.WriteLine("{0} секунд", (stop_db - start_db).TotalSeconds);
            Console.WriteLine($"{tags.Count} OK");
            if (ExistImage(hash))
            {
                Console.WriteLine("Уже скачан: {0}", store_file);
                AddPostIDToPosts(Item.PostID, hash);
                RemoveItemFromCache(Item.PostID);
                //count_skip++;
                return true;
            }
            //ErzaDB.SetImagePath(hash, filename, connection);
            Console.WriteLine("Начинаем закачку {0}.", url);
            FileInfo fi = new FileInfo(filename);
            //ВРЕМЕННО!!!!!!!!
            //if (fi.Exists)
            //{
                //Console.WriteLine("Уже скачан.");
                //return true;
            //}
            Thread.Sleep(Program.config.TimeOut - 2000);
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
                //httpWRQ.Referer = Program.config.BaseURL + "post/show/" + Item.PostID.ToString();
                httpWRQ.UserAgent = Program.config.UserAgent;
                httpWRQ.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                httpWRQ.Headers.Add("Accept-Encoding: identity");
                //httpWRQ.CookieContainer = new CookieContainer();
                //httpWRQ.CookieContainer.Add(cookies);
                //httpWRQ.Headers.Add(HttpRequestHeader.Cookie, RawCookies);
                httpWRQ.Timeout = 60 * 1000;
                wrp = httpWRQ.GetResponse();
                if (fi.Exists)
                {
                    if (wrp.ContentLength == fi.Length)
                    {
                        Console.WriteLine("Уже скачан: {0}", filename);
                        count_skip++;
                        //wrp.Close();
                        ErzaDB.SetImagePath(hash, filename.ToLower(), connection);
                        AddPostIDToPosts(Item.PostID, hash);
                        RemoveItemFromCache(Item.PostID);
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
                }
                if (cnt < wrp.ContentLength)
                {
                    Console.WriteLine("\nОбрыв! Закачка не завершена!");
                    return false;
                }
                else
                {
                    Console.WriteLine("\nЗакачка завершена.");
                    count_complit++;
                    ErzaDB.SetImagePath(hash, filename.ToLower(), connection);
                    AddPostIDToPosts(Item.PostID, hash);
                    RemoveItemFromCache(Item.PostID);
                    //ThreadPool.QueueUserWorkItem(CreatePreview, filename);
                    return true;
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
        static string GetPostPage(long npost)
        {
            Random rnd = new Random();
            string strURL = Program.config.BaseURL + "?tags=id:" + npost.ToString();
            Console.WriteLine("Загружаем и парсим пост: " + strURL);
            while (true)
            {
                try
                {
                    return DownloadStringFromSankaku(strURL, null);
                }
                catch (WebException we)
                {
                    Console.WriteLine(we.Message);
                    Thread.Sleep(Program.config.TimeOutError);
                    return null;
                }
            }
        }
        static string GetOriginalUrlFromPostPage(string post)
        {
            string file_url = "<li>Original: <a href=\"";
            Regex rx = new Regex(file_url + @"\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?", RegexOptions.Compiled);
            try
            {
                Match match = rx.Match(post);
                if (match.Success)
                {
                    string url = match.Value.Substring(file_url.Length);
                    return "https:" + url.Replace("amp;", String.Empty);
                }
                else
                {
                    Regex rx_swf = new Regex("<p><a href=\"" + @"\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?" + "\">Save this file \\(right click and save as\\)</a></p>", RegexOptions.Compiled);
                    Match match_swf = rx_swf.Match(post);
                    if (match_swf.Success)
                    {
                        string url = match_swf.Value.Substring(12).Replace("\">Save this file (right click and save as)</a></p>", String.Empty);
                        return "https:" + url.Replace("amp;", String.Empty);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (ArgumentNullException)
            {
                return null;
            }
        }
        static List<CacheItem> GetImageInfoFromSankaku(string tag)
        {
            List<CacheItem> imgs = new List<CacheItem>();

            int error = 0;
            int page_count = 1;
            while (true)
            {
                try
                {
                    if ((MaxPage > 0) && (page_count == StartPage + MaxPage))
                    {
                        Console.WriteLine("Достигнут лимит страниц.");
                        break;
                    }
                    Thread.Sleep(Program.config.TimeOut);
                    string url = String.Format("{0}?tags={1}&page={2}", Program.config.BaseURL, tag, page_count);
                    Console.WriteLine("({0}) Загружаем и парсим: {1}", imgs.Count, url);
                    string json = DownloadStringFromSankaku(url, null);
                    if (page_count >= StartPage)
                    {
                        //imgs.AddRange(ParseHTML_sankaku(text));
                        JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions();
                        jsonSerializerOptions.IgnoreNullValues = true;
                        SankakuJson[] sankakuJson = JsonSerializer.Deserialize<SankakuJson[]>(json, jsonSerializerOptions);
                        
                        //List<long> temp = ParseHTML_sankaku(text);
                        foreach (SankakuJson sj in sankakuJson)
                        {
                            CacheItem item = new CacheItem();
                            item.PostID = sj.id;
                            item.Referer = url;
                            item.Tag = tag;
                            imgs.Add(item);
                        }
                        if(sankakuJson.Length < 20) { break; }
                    }
                    else
                    {
                        Console.WriteLine("Страница пропущена.");
                    }
                    page_count++;
                    error = 0;
                }
                catch (WebException we)
                {
                    Console.WriteLine("Ошибка: " + we.Message);
                    if (we.Response != null)
                    {
                        HttpWebResponse errorResponse = we.Response as HttpWebResponse;
                        if (errorResponse.StatusCode == HttpStatusCode.NotFound)
                        {
                            Console.WriteLine("Ошибка 404! прекращаем получение ссылок.");
                            break;
                        }
                        if (errorResponse.StatusCode == HttpStatusCode.BadRequest)
                        {
                            Console.WriteLine("Ошибка 400! прекращаем получение ссылок.");
                            break;
                        }
                    }
                    error++;
                    if (error >= Program.config.LimitErrors)
                    {
                        Console.WriteLine("Достигнут лимит ошибок!\nПрекращаю работу!");
                        Environment.Exit(1);
                    }
                    Console.WriteLine("Таймаут {0} секунд", Program.config.TimeOutError / 1000);
                    Thread.Sleep(Program.config.TimeOutError);
                }
            }
            return imgs;
        }
        static string GetNextPage(string text)
        {
            Regex next_page = new Regex("next-page-url=\".*?\"", RegexOptions.Compiled);
            Match match = next_page.Match(text);
            if (match.Success)
            {
                string temp = match.Value;
                temp = temp.Replace("next-page-url=\"", String.Empty);
                temp = temp.Replace("amp;", String.Empty);
                temp = Program.config.BaseURL + temp.Substring(1, temp.Length - 2);
                return temp;
            }
            else return null;
        }
        static CookieCollection GetSankakuCookies(string url)
        {
            try
            {
                HttpWebRequest loginRequest = (HttpWebRequest)WebRequest.Create(url);
                if (Program.config.UseProxy)
                {
                    WebProxy myProxy = new WebProxy(Program.config.ProxyAddress, Program.config.ProxyPort);
                    myProxy.Credentials = new NetworkCredential(Program.config.ProxyLogin, Program.config.ProxyPassword);
                    loginRequest.Proxy = myProxy;
                }
                loginRequest.UserAgent = Program.config.UserAgent;
                loginRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                loginRequest.ContentType = "application/x-www-form-urlencoded";
                loginRequest.Headers.Add("Accept-Encoding: identity");
                loginRequest.CookieContainer = new CookieContainer();
                loginRequest.Method = "POST";
                string PostData = String.Format("user%5Bname%5D={0}&user%5Bpassword%5D={1}", Program.config.SankakuLogin, Program.config.SankakuPassword);
                Encoding encoding = Encoding.UTF8;
                byte[] byte1 = encoding.GetBytes(PostData);
                loginRequest.ContentLength = byte1.Length;
                using (Stream st = loginRequest.GetRequestStream())
                {
                    st.Write(byte1, 0, byte1.Length);
                    st.Close();
                }
                loginRequest.AllowAutoRedirect = false;
                HttpWebResponse loginResponse = (HttpWebResponse)loginRequest.GetResponse();
                RawCookies = loginResponse.Headers["Set-Cookie"];
                return loginResponse.Cookies;
            }
            catch (WebException we)
            {
                Console.WriteLine(we.Message);
                return null;
            }
        }
        static string DownloadStringFromSankaku(string url, string referer)
        {
            HttpWebRequest downloadRequest = (HttpWebRequest)WebRequest.Create(url);
            if (Program.config.UseProxy)
            {
                WebProxy myProxy = new WebProxy(Program.config.ProxyAddress, Program.config.ProxyPort);
                myProxy.Credentials = new NetworkCredential(Program.config.ProxyLogin, Program.config.ProxyPassword);
                downloadRequest.Proxy = myProxy;
            }
            downloadRequest.UserAgent = Program.config.UserAgent;
            downloadRequest.Headers.Add("authorization:Bearer " + AuthToken);
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
        static string GetAuthTokenFromSankaku(string URL, string UserAgent, string Login, string Password)
        {
            HttpWebRequest downloadRequest = (HttpWebRequest)WebRequest.Create(URL);
            if (Program.config.UseProxy)
            {
                WebProxy myProxy = new WebProxy(Program.config.ProxyAddress, Program.config.ProxyPort);
                myProxy.Credentials = new NetworkCredential(Program.config.ProxyLogin, Program.config.ProxyPassword);
                downloadRequest.Proxy = myProxy;
            }
            downloadRequest.UserAgent = UserAgent;
            downloadRequest.Method = "POST";
            auth_sankaku authsan = new auth_sankaku();
            authsan.login = Login;
            authsan.password = Password;
            string PostData = JsonSerializer.Serialize<auth_sankaku>(authsan);
            downloadRequest.ContentLength = PostData.Length;
            downloadRequest.ContentType = "application/json";

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] loginDataBytes = encoding.GetBytes(PostData);
            downloadRequest.ContentLength = loginDataBytes.Length;
            Stream stream = downloadRequest.GetRequestStream();
            stream.Write(loginDataBytes, 0, loginDataBytes.Length);

            string source;
            using (StreamReader reader = new StreamReader(downloadRequest.GetResponse().GetResponseStream()))
            {
                source = reader.ReadToEnd();
            }
            taken_class token = JsonSerializer.Deserialize<taken_class>(source);
            return token.access_token;
        }
        static List<long> ParseHTML_sankaku(string html)
        {
            List<long> temp = new List<long>();
            Regex rx_digit = new Regex("[0-9]+", RegexOptions.Compiled);
            Regex rx = new Regex(@"PostModeMenu\.click\([0-9]*\)", RegexOptions.Compiled);
            MatchCollection matches = rx.Matches(html);
            foreach (Match match in matches)
            {
                temp.Add(long.Parse(rx_digit.Match(match.Value).Value));
            }
            return temp;
        }
        static string GetFileName(string dir, string url)
        {
            int temp = url.IndexOf('?');
            if (temp >= 0)
            {
                url = url.Substring(0, temp);
            }
            string extension = Path.GetExtension(url);
            if (extension == ".jpeg")
            {
                return dir + "\\" + Path.GetFileNameWithoutExtension(url) + ".jpg";
            }
            else
            {
                return dir + "\\" + Path.GetFileName(url);
            }
        }
        static bool ValidationCallback(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        static bool ExistImage(string hash_string)
        {
            ImageInfo inf = ErzaDB.GetImageWithOutTags(hash_string, connection);
            if(inf == null) { return false; }
            if (inf.IsDeleted)
            {
                count_deleted++;
                store_file = "Удалён!";
                return true;
            }
            if(inf.FilePath == null)
            {
                return false;
            }
            else
            {
                store_file = inf.FilePath;
                count_skip++;
                return true;
            }
            /*
            if (hash_string == null)
            {
                throw new ArgumentNullException("hexString");
            }
            if ((hash_string.Length & 1) != 0)
            {
                throw new ArgumentOutOfRangeException("hexString", hash_string, "hexString must contain an even number of characters.");
            }
            byte[] hash = new byte[hash_string.Length / 2];
            for (int i = 0; i < hash_string.Length; i += 2)
            {
                hash[i / 2] = byte.Parse(hash_string.Substring(i, 2), NumberStyles.HexNumber);
            }
            //using (SQLiteConnection connection = new SQLiteConnection(Program.config.ConnectionString))
            //{
                //connection.Open();
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.CommandText = "select * from hash_tags where hash = @hash";
                    command.Parameters.AddWithValue("hash", hash);
                    command.Connection = connection;
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        if (System.Convert.ToBoolean(reader["is_deleted"]))
                        {
                            reader.Close();
                            count_deleted++;
                            store_file = "Удалён!";
                            return true;
                        }
                        if (Convert.IsDBNull(reader["file_name"]))
                        {
                            reader.Close();
                            return false;
                        }
                        store_file = System.Convert.ToString(reader["file_name"]);
                        reader.Close();
                        count_skip++;
                        return true;
                    }
                    else
                    {
                        reader.Close();
                        return false;
                    }
                }
                }
            //}*/
        }
        static int GetTagsFromSankaku(string md5, string post)
        {
            try
            {
                List<string> tags = new List<string>();
                Regex rx = new Regex("<title>(.+)</title>");
                Match match = rx.Match(post);
                if (match.Success)
                {
                    string temp = match.Value.Substring(("<title>").Length);
                    temp = temp.Replace(" | Sankaku Channel</title>", String.Empty);
                    tags.AddRange(temp.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries));
                    for(int i = 0; i < tags.Count; i++)
                    {
                        tags[i] = tags[i].Replace(' ', '_');
                    }
                }
                else
                {
                    return 0;
                }
                if(tags.Count <= 0) { return 0; }
                ImageInfo img = new ImageInfo();
                img.Hash = md5;
                img.Tags.AddRange(tags);
                img.IsDeleted = false;
                SQLiteTransaction transact = Program.connection.BeginTransaction();
                ErzaDB.LoadImageToErza(img, Program.connection);
                transact.Commit();
                return tags.Count;
            }
            catch (Exception ex)
            {
                //Thread.Sleep(60000);
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return 0;
            }
        }
        static bool IsImageFile(string FilePath)
        {
            string ext = Path.GetExtension(FilePath);
            switch (ext.ToLower())
            {
                case ".jpg":
                    return true;
                case ".jpeg":
                    return true;
                case ".png":
                    return true;
                case ".bmp":
                    return true;
                case ".gif":
                    return true;
                case ".tif":
                    return true;
                case ".tiff":
                    return true;
            }
            return false;
        }
        static bool ExistPostIDFromPosts(long PostID)
        {
            string sql = "SELECT post_id FROM posts WHERE post_id = @post_id";
            using (SQLiteCommand command = new SQLiteCommand(sql, getidoldb))
            {
                command.Parameters.AddWithValue("post_id", PostID);
                object o = command.ExecuteScalar();
                if (o == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        static void AddPostIDToPosts(long PostID, string Hash)
        {
            string sql = "INSERT INTO posts (post_id, hash) VALUES (@post_id, @hash);";
            using (SQLiteCommand command = new SQLiteCommand(sql, getidoldb))
            {
                command.Parameters.AddWithValue("post_id", PostID);
                command.Parameters.AddWithValue("hash", Hash);
                command.ExecuteNonQuery();
            }
        }
        static void RemoveItemFromCache(long PostID)
        {
            using (SQLiteCommand command = new SQLiteCommand(getidoldb))
            {
                command.CommandText = "DELETE FROM cache WHERE post_id = @post_id";
                command.Parameters.AddWithValue("post_id", PostID);
                command.ExecuteNonQuery();
            }
        }
        static void RemoveItemFromCache(CacheItem Item)
        {
            using (SQLiteCommand command = new SQLiteCommand(getidoldb))
            {
                command.CommandText = "DELETE FROM cache WHERE post_id = @post_id";
                command.Parameters.AddWithValue("post_id", Item.PostID);
                command.ExecuteNonQuery();
            }
        }
        static void AddPostIDsToCache(List<long> PostIDs)
        {
            SQLiteTransaction transact = getidoldb.BeginTransaction();
            string sql = "INSERT INTO cache (post_id) VALUES (@post_id);";
            foreach (long post_id in PostIDs)
            {
                using (SQLiteCommand command = new SQLiteCommand(sql, getidoldb))
                {
                    command.Parameters.AddWithValue("post_id", post_id);
                    command.ExecuteNonQuery();
                }
            }
            transact.Commit();
        }
        static void AddItemsToCache(List<CacheItem> Items)
        {
            SQLiteTransaction transact = getidoldb.BeginTransaction();
            string sql = "INSERT INTO cache (post_id, referer, tag) VALUES (@post_id, @referer, @tag);";
            foreach (CacheItem item in Items)
            {
                using (SQLiteCommand command = new SQLiteCommand(sql, getidoldb))
                {
                    command.Parameters.AddWithValue("post_id", item.PostID);
                    command.Parameters.AddWithValue("referer", item.Referer);
                    command.Parameters.AddWithValue("tag", item.Tag);
                    command.ExecuteNonQuery();
                }
            }
            transact.Commit();
        }
        static List<long> GetPostIDsFromCache()
        {
            List<long> post_ids = new List<long>();
            using (SQLiteCommand command = new SQLiteCommand(getidoldb))
            {
                command.CommandText = "SELECT post_id FROM cache";
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        post_ids.Add(reader.GetInt64(0));
                    }
                }
            }
            return post_ids;
        }
        static List<CacheItem> GetItemsFromCache()
        {
            List<CacheItem> post_ids = new List<CacheItem>();
            using (SQLiteCommand command = new SQLiteCommand(getidoldb))
            {
                command.CommandText = "SELECT post_id, referer, tag FROM cache";
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CacheItem item = new CacheItem();
                        item.PostID = reader.GetInt64(0);
                        item.Referer = reader.GetString(1);
                        item.Tag = reader.GetString(2);
                        post_ids.Add(item);
                    }
                }
            }
            return post_ids;
        }
        static void VacuumGetIdolDB()
        {
            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                command.CommandText = "VACUUM";
                command.ExecuteNonQuery();
            }
        }
        static void ClearCache()
        {
            using (SQLiteCommand command = new SQLiteCommand(getidoldb))
            {
                command.CommandText = "DELETE FROM cache";
                command.ExecuteNonQuery();
            }
        }
        static List<string> GetTagsFromCache()
        {
            List<string> tags = new List<string>();
            using (SQLiteCommand command = new SQLiteCommand(getidoldb))
            {
                command.CommandText = "SELECT tag FROM cache  GROUP BY tag";
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tags.Add(reader.GetString(0));
                    }
                }
            }
            return tags;
        }
        static void CreatePreview(Object o)
        {
            lock (prevlock2)
            {
                thread_count++;
            }
            string FilePath = (string)o;
            string Hash = Path.GetFileNameWithoutExtension(FilePath);
            using (Bitmap bitmap = PreviewDB.CreatePreview(FilePath, PreviewWidth, PreviewHeight))
            {
                byte[] array = PreviewDB.PreviewToJpeg(bitmap);
                lock (prevlock)
                {
                    PreviewDB.LoadPreviewToDB(Hash, array, prev_conn);
                }
            }
            lock (prevlock2)
            {
                thread_count--;
            }
        }
    }
    [DataContract]
    public class GetidolConfig
    {
        [DataMember(Name = "BaseURL", IsRequired = true)]
        public string BaseURL;
        [DataMember(Name = "ConnectionString", IsRequired = true)]
        public string ConnectionString;
        [DataMember(Name = "UseDB", IsRequired = true)]
        public bool UseDB;
        [DataMember(Name = "DownloadPath", IsRequired = true)]
        public string DownloadPath;
        [DataMember(Name = "LimitErrors", IsRequired = true)]
        public int LimitErrors;
        [DataMember(Name = "TimeOut", IsRequired = true)]
        public int TimeOut;
        [DataMember(Name = "TimeOutError", IsRequired = true)]
        public int TimeOutError;
        [DataMember(Name = "SankakuLogin", IsRequired = true)]
        public string SankakuLogin;
        [DataMember(Name = "SankakuPassword", IsRequired = true)]
        public string SankakuPassword;
        [DataMember(Name = "UseProxy", IsRequired = true)]
        public bool UseProxy;
        [DataMember(Name = "ProxyAddress", IsRequired = true)]
        public string ProxyAddress;
        [DataMember(Name = "ProxyPort", IsRequired = true)]
        public int ProxyPort;
        [DataMember(Name = "ProxyLogin", IsRequired = true)]
        public string ProxyLogin;
        [DataMember(Name = "ProxyPassword", IsRequired = true)]
        public string ProxyPassword;
        [DataMember(Name = "UserAgent", IsRequired = true)]
        public string UserAgent;
    }
    public class CacheItem
    {
        public long PostID;
        public string Referer;
        public string Tag;
    }
    public class SankakuJson
    {
        public int id { get; set; }
        public string rating { get; set; }
        public string status { get; set; }
        public author_class author { get; set; }
        public string sample_url { get; set; }
        public int sample_width { get; set; }
        public int sample_height { get; set; }
        public string preview_url { get; set; }
        public int preview_width { get; set; }
        public int preview_height { get; set; }
        public string file_url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int file_size { get; set; }
        public string file_type { get; set; }
        public created_at_class created_at { get; set; }
        public bool has_children { get; set; }
        public bool has_comments { get; set; }
        public bool has_notes { get; set; }
        public bool is_favorited { get; set; }
        public int? user_vote { get; set; }
        public string md5 { get; set; }
        public int? parent_id { get; set; }
        public int change { get; set; }
        public int fav_count { get; set; }
        //public string recommended_posts { get; set; }
        public int recommended_score { get; set; }
        public int vote_count { get; set; }
        public int total_score { get; set; }
        public int? comment_count { get; set; }
        public string source { get; set; }
        public bool in_visible_pool { get; set; }
        public bool is_premium { get; set; }
        public float? sequence { get; set; }
        public IList<tag> tags { get; set; }
    }
    public class author_class
    {
        public int id { get; set; }
        public string name { get; set; }
        public string avatar { get; set; }
        public string avatar_rating { get; set; }
    }
    public class created_at_class
    {
        public string json_class { get; set; }
        public int s { get; set; }
        public int n { get; set; }
    }
    public class tag
    {
        public int id { get; set; }
        public string name_en { get; set; }
        public string name_ja { get; set; }
        public int type { get; set; }
        public int count { get; set; }
        public int post_count { get; set; }
        public int pool_count { get; set; }
        public string locale { get; set; }
        public string rating { get; set; }
        public string name { get; set; }
    }
    public class auth_sankaku
    {
        public string login { get; set; }
        public string password { get; set; }
    }
    public class taken_class
    {
        public string access_token { get; set; }
    }
}
