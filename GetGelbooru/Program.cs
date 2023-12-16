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
using System.Threading.Tasks;
using ErzaLib;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.Xml;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Text.RegularExpressions;
using System.Data.SQLite;
using AngleSharp;
//using AngleSharp.Parser.Html;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;

namespace GetGelbooru
{
    class Program
    {
        static ErzaConfig config = null;
        static int count_complit = 0;
        static int count_deleted = 0;
        static int count_error = 0;
        static int count_skip = 0;
        static SQLiteConnection connection = null;
        static List<string> Tags = null;
        static int StartPid = 0;
        static int MaxPage = 0;
        static void Main(string[] args)
        {
            LoadSettings();
            List<string> post_links = new List<string>();
            if (args.Length <= 0)
            {
                Console.WriteLine("Не заданы теги!");
                return;
            }
            ParseArgs(args);
            if(Program.config.DownloadPath == ".")
            {
                Program.config.DownloadPath = Directory.GetCurrentDirectory();
            }
            ServicePointManager.ServerCertificateValidationCallback = ValidationCallback;
            StringBuilder tags_bilder = new StringBuilder();
            for (int i = 0; i < Tags.Count; i++)
            {
                if (i == 0)
                {
                    tags_bilder.Append(WebUtility.UrlEncode(Tags[i]));
                }
                else
                {
                    tags_bilder.Append("+");
                    tags_bilder.Append(WebUtility.UrlEncode(Tags[i]));
                }
            }
            CookieCollection cookies = GetGelbooruCookies(Program.config.GelbooruLogin, Program.config.GelbooruPassword);
            if (cookies == null)
            {
                Console.WriteLine("Не удалось авторизоваться на Gelbooru!");
                return;
            }
            Console.WriteLine("Получаем ссылки на порсты.");
            post_links.AddRange(GetPosts(tags_bilder.ToString(), cookies));
            if (post_links.Count <= 0)
            {
                Console.WriteLine("Ничего ненайдено.");
                return;
            }
            Console.Write("\n\t\tНАЧИНАЕТСЯ ЗАГРУЗКА\n\n");
            connection = new SQLiteConnection(Program.config.ConnectionString);
            connection.Open();
            string path = Program.config.DownloadPath + "\\" + tags_bilder.ToString();
            Directory.CreateDirectory(path);
            for (int i = 0; i < post_links.Count; i++)
            {
                Console.WriteLine("###### {0}/{1} ######", (i + 1), post_links.Count);
                for (int ie = 0; ie < Program.config.LimitError; ie++)
                {
                    if (DownloadImage(post_links[i], path, "https://gelbooru.com/index.php?page=post&s=list&tags=" + tags_bilder.ToString(), cookies))
                    {
                        count_complit++;
                        Thread.Sleep(2500);
                        break;
                    }
                    else
                    {
                        if (ie == Program.config.LimitError - 1)
                        {
                            count_error++;
                        }
                        Thread.Sleep(2500);
                    }
                }
            }
            Console.WriteLine($"\nУспешно скачано: {count_complit}\nСкачано ренее: {count_skip}\nУдалено ранее: {count_deleted}\nОшибочно: {count_error}\nВсего: {post_links.Count}");
            connection.Close();
        }
        static void ParseArgs(string[] args)
        {
            string start_page_string = "--start-pid=";
            string max_page_string = "--max-page=";
            Program.Tags = new List<string>();
            foreach (string param in args)
            {
                if (param.Length >= start_page_string.Length)
                {
                    if (param.Substring(0, start_page_string.Length) == start_page_string)
                    {
                        if (param.Length > start_page_string.Length)
                        {
                            Program.StartPid = int.Parse(param.Substring(start_page_string.Length));
                            if (Program.StartPid < 0)
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
        static List<string> GetPosts(string Tag, CookieCollection Cookies)
        {
            int pid = Program.StartPid;                //Счетчик постов
            int page_count = 0;
            List<string> post_list = new List<string>();
            int errors = 0;
            for (; ; )
            {
                if ((MaxPage > 0) && (page_count >= MaxPage))
                {
                    Console.WriteLine("Достигнут лимит страниц.");
                    break;
                }
                string url = String.Format("https://gelbooru.com/index.php?page=post&s=list&tags={0}&pid={1}", Tag, pid);
                Console.WriteLine($"({pid}) Загружаем и парсим: {url}");
                try
                {
                    string page = DownloadStringFromGelbooru(url, "https://gelbooru.com/", Cookies);
                    if (page == null)
                    {
                        if (errors < Program.config.LimitError)
                        {
                            errors++;
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                    List<string> list = ParseListPage(page);
                    if (list.Count <= 0)
                    {
                        break;
                    }
                    else
                    {
                        post_list.AddRange(list);
                        pid = pid + list.Count;
                        page_count++;
                    }
                    Thread.Sleep(2500);
                }
                catch (WebException we)
                {
                    Console.WriteLine("Ошибка: " + we.Message);
                    Thread.Sleep(10000);
                    continue;
                }
            }
            return post_list;
        }
        static List<string> ParseListPage(string Page)
        {
            List<string> links = new List<string>();
            var parser = new HtmlParser();
            var document = parser.ParseDocument(Page);
            foreach (IElement element in document.QuerySelectorAll("article"))
            {
                if (element.GetAttribute("class") == "thumbnail-preview")
                {
                    foreach (IElement link_element in element.QuerySelectorAll("a"))
                    {
                        links.Add(link_element.GetAttribute("href"));
                    }
                }
            }
            return links;
        }
        static string ParsePostPage(string Page)
        {
            var parser = new HtmlParser();
            var document = parser.ParseDocument(Page);
            foreach (IElement element in document.QuerySelectorAll("a"))
            {
                if ("Original image" == element.InnerHtml)
                {
                    return element.GetAttribute("href");
                }
            }
            return null;
        }
        static bool DownloadImage(string PostUrl, string Directory, string Referer, CookieCollection Cookies)
        {
            string post = DownloadString(PostUrl, Referer, Cookies, Program.config.UserAgent);
            if (post == null)
            {
                Console.WriteLine("Пост не получен!");
                return false;
            }
            string url = ParsePostPage(post);
            if (url == null)
            {
                Console.WriteLine("URL Картинки не получен!");
                return false;
            }
            string filename = GetFileName(Directory, url);
            /*if (!IsImageFile(filename))
            {
                return true;
            }*/
            Console.Write("Добавляем информацию в базу данных...");
            string hash = Path.GetFileNameWithoutExtension(url);
            UpdateDB(hash, post);
            Console.WriteLine("OK");
            if (ExistImage(hash))
            {
                //Console.WriteLine("Уже скачан: {0}", store_file);
                //count_skip++;
                return true;
            }
            //ErzaDB.SetImagePath(hash, filename, connection);
            Console.WriteLine("Начинаем закачку {0}.", url);
            bool result = DownloadFile(url, filename, Referer, Cookies, Program.config.UserAgent);
            if (result)
            {
                ErzaDB.SetImagePath(hash, filename.ToLower(), connection);
            }
            return result;
        }
        public static string DownloadString(string Url, string Referer, CookieCollection Cookies, string UserAgent)
        {
            try
            {
                HttpWebRequest downloadRequest = (HttpWebRequest)WebRequest.Create(Url);
                if (Program.config.UseProxy)
                {
                    WebProxy myProxy = new WebProxy(Program.config.ProxyAddress, Program.config.ProxyPort);
                    myProxy.Credentials = new NetworkCredential(Program.config.ProxyLogin, Program.config.ProxyPassword);
                    downloadRequest.Proxy = myProxy;
                }
                downloadRequest.UserAgent = UserAgent;
                downloadRequest.CookieContainer = new CookieContainer();
                downloadRequest.CookieContainer.Add(Cookies);
                if (Referer != null)
                {
                    downloadRequest.Referer = Referer;
                }
                string source;
                using (StreamReader reader = new StreamReader(downloadRequest.GetResponse().GetResponseStream()))
                {
                    source = reader.ReadToEnd();
                }
                return source;
            }
            catch (Exception)
            {
                return null;
            }
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
        static void UpdateDB(string Md5, string Page)
        {
            try
            {
                List<string> tags = new List<string>();
                var parser = new HtmlParser();
                var document = parser.ParseDocument(Page);
                foreach (IElement element in document.QuerySelectorAll("textarea"))
                {
                    if (element.GetAttribute("name") == "tags")
                    {
                        tags.AddRange(element.InnerHtml.Split(' '));
                    }
                }
                if (tags.Count <= 0) { return; }
                ImageInfo img = new ImageInfo();
                img.Hash = Md5;
                img.Tags.AddRange(tags);
                img.IsDeleted = false;
                SQLiteTransaction transact = Program.connection.BeginTransaction();
                ErzaDB.LoadImageToErza(img, Program.connection);
                transact.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return;
            }
            return;
        }
        static CookieCollection GetGelbooruCookies(string user, string password)
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create("https://gelbooru.com/index.php?page=account&s=login&code=00");
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
                using (Stream st = httpWebRequest.GetRequestStream())
                {
                    st.Write(byte1, 0, byte1.Length);
                    st.Close();
                }
                httpWebRequest.AllowAutoRedirect = false;
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                //using (StreamReader reader = new StreamReader(httpWebResponse.GetResponseStream()))
                //{
                //string s = reader.ReadToEnd();
                //}
                //string RawCookies = httpWebResponse.Headers["Set-Cookie"];
                return httpWebResponse.Cookies;
            }
            catch (WebException we)
            {
                Console.WriteLine(we.Message);
                return null;
            }
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
        static bool ExistImage(string hash_string)
        {
            ImageInfo inf = ErzaDB.GetImageWithOutTags(hash_string, connection);
            if (inf == null) { return false; }
            if (inf.IsDeleted)
            {
                count_deleted++;
                Console.WriteLine("Скачан ранее: Удалён!");
                return true;
            }
            if (inf.FilePath == null)
            {
                return false;
            }
            else
            {
                count_skip++;
                Console.WriteLine($"Скачан ранее: {inf.FilePath}");
                return true;
            }
        }
        static void LoadSettings()
        {
            Program.config = new ErzaConfig();
            //Параметры по умолчанию
            Program.config.ConnectionString = @"data source=C:\utils\data\erza.sqlite";
            Program.config.UseDB = true;
            Program.config.UserAgent = "Mozilla / 5.0(Windows NT 6.2; WOW64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 34.0.1847.116 Safari / 537.36";
            Program.config.LimitError = 4;
            Program.config.GelbooruLogin = "";
            Program.config.GelbooruPassword = "";
            Program.config.DownloadPath = ".";
            Program.config.UseProxy = false;
            Program.config.ProxyAddress = null;
            Program.config.ProxyPort = 0;
            Program.config.ProxyLogin = null;
            Program.config.ProxyPassword = null;
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(ErzaConfig));
            if (File.Exists(@"C:\utils\cfg\GetGelbooru.json"))
            {
                using (FileStream fs = new FileStream(@"C:\utils\cfg\GetGelbooru.json", FileMode.Open))
                {
                    Program.config = (ErzaConfig)jsonFormatter.ReadObject(fs);
                }
                return;
            }
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Erza\\GetGelbooru.json"))
            {
                using (FileStream fs = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Erza\\GetGelbooru.json", FileMode.Open))
                {
                    Program.config = (ErzaConfig)jsonFormatter.ReadObject(fs);
                }
                return;
            }
            Console.WriteLine("Конфигурационный файл не найден!\nЗагружены настройки по умолчанью.");
        }
        static bool ValidationCallback(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        static bool DownloadFile(string Url, string FilePath, string Referer, CookieCollection Cookies, string UserAgent)
        {
            FileInfo fi = new FileInfo(FilePath);
            HttpWebRequest httpWRQ = (HttpWebRequest)HttpWebRequest.Create(new Uri(Url));
            if (Program.config.UseProxy)
            {
                WebProxy myProxy = new WebProxy(Program.config.ProxyAddress, Program.config.ProxyPort);
                myProxy.Credentials = new NetworkCredential(Program.config.ProxyLogin, Program.config.ProxyPassword);
                httpWRQ.Proxy = myProxy;
            }
            httpWRQ.Referer = Referer;
            httpWRQ.UserAgent = UserAgent;
            //httpWRQ.Timeout = 60 * 1000;
            httpWRQ.CookieContainer = new CookieContainer();
            httpWRQ.CookieContainer.Add(Cookies);
            WebResponse wrp = null;
            Stream rStream = null;
            try
            {
                wrp = httpWRQ.GetResponse();
                if (fi.Exists)
                {
                    if (wrp.ContentLength == fi.Length)
                    {
                        Console.WriteLine("Уже скачан.");
                        return true;
                    }
                    else
                    {
                        fi.Delete();
                    }
                }
                long cnt = 0;
                rStream = wrp.GetResponseStream();
                //rStream.ReadTimeout = 60 * 1000;
                using (FileStream fs = new FileStream(FilePath, FileMode.Create))
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead;
                    DateTime start = DateTime.Now;
                    while ((bytesRead = rStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fs.Write(buffer, 0, bytesRead);
                        cnt += bytesRead;
                        DateTime pred = DateTime.Now;
                        //Console.Write("\rСкачано " + cnt.ToString("#,###,###") + " из " + wrp.ContentLength.ToString("#,###,###") + " байт Скорость: " + ((cnt / (pred - start).TotalSeconds) / 1024).ToString("0.00") + " Килобайт в секунду.");
                        Console.Write($"\rСкачано {cnt, 9} из {wrp.ContentLength, 9} байт Скорость: {((cnt / (pred - start).TotalSeconds) / 1024), 6:f2} Килобайт в секунду.");
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
        static bool IsImageFile(string FilePath)
        {
            string ext = Path.GetExtension(FilePath);
            switch (ext.ToLower())
            {
                case ".jpg":
                    return true;
                case ".jpeg":
                    return true;
                case ".jpe":
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
                case ".webp":
                    return true;
            }
            return false;
        }
    }
    [DataContract]
    public class ErzaConfig
    {
        [DataMember]
        public string ConnectionString;
        [DataMember]
        public bool UseDB;
        [DataMember]
        public string DownloadPath;
        [DataMember]
        public int LimitError;
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
