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
//using System.Data.SQLite;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Text.RegularExpressions;

namespace GetGelbooru
{
    class Program
    {
        static CookieCollection gelbooru_cookies = null;
        static ErzaConfig config = null;
        static int count_complit = 0;
        static int count_deleted = 0;
        static int count_error = 0;
        static int count_skip = 0;
        static void Main(string[] args)
        {
            LoadSettings();
            //List<ImageInfo> il = new List<ImageInfo>();
            List<int> post_ids = new List<int>();
            if (args.Length <= 0)
            {
                Console.WriteLine("Не заданы теги!");
                return;
            }
            foreach(string tag in args)
            {
                Console.WriteLine("Импортируем тег " + tag + " с гелбуры");
                //il = SliyanieLists(il, get_hash_gelbooru(WebUtility.UrlEncode(tag)));
                post_ids.AddRange(get_hash_gelbooru(WebUtility.UrlEncode(tag)));
            }
            if (post_ids.Count <= 0)
            {
                Console.WriteLine("Ничего ненайдено.");
                return;
            }
            Console.Write("\n\n\n\t\tНАЧИНАЕТСЯ ЗАГРУЗКА\n\n\n");
            
            Program.config.DownloadPath = Program.config.DownloadPath + "\\" + args[0];
            Directory.CreateDirectory(Program.config.DownloadPath);
            for (int i = 0; i < post_ids.Count; i++)
            {
                Console.WriteLine("\n###### {0}/{1} ######", (i + 1), post_ids.Count);
                if (DownloadImageFromSankaku(post_ids[i], Program.config.DownloadPath, gelbooru_cookies))
                {
                    count_complit++;
                }
                else
                {
                    count_error++;
                }
            }
        }
        static List<int> get_hash_gelbooru(string tag)
        {
            gelbooru_cookies = GetGelbooruCookies(Program.config.GelbooruLogin, Program.config.GelbooruPassword);
            if (gelbooru_cookies == null)
            {
                Console.WriteLine("Не удалось авторизоваться на Gelbooru!");
                return null;
            }
            const int GELBOORU_LIMIT_POSTS = 100;
            int nPostsCount = 0;          //Счетчик постов для скачивания
            int nPage = 0;                //Счетчик страниц
            List<int> img_list = new List<int>();
            /*int count = 0;
            while (true)
            {
                nPostsCount = posts_count_gelbooru(String.Format("https://gelbooru.com/index.php?page=dapi&s=post&q=index&tags={0}&pid=0&limit=1", tag));
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
            }*/
            int count_errors = 0;
            for (;;)
            {
                //string strURL = String.Format("http://gelbooru.com/index.php?page=dapi&s=post&q=index&tags={0}&pid={1}&limit={2}", tag, nPage, GELBOORU_LIMIT_POSTS);
                string strURL = String.Format("http://gelbooru.com/index.php?page=post&s=list&tags={0}&pid={1}", tag, nPage);
                Console.WriteLine("({0}/{1}) Загружаем и парсим: {2}", img_list.Count, nPostsCount, strURL);
                try
                {
                    //Uri uri = new Uri(strURL);
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
                    //List<ImageInfo> list = ParseXMLGelBooru(xml);
                    List<int> list = ParseHTML_sankaku(xml);
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
        static bool DownloadImageFromSankaku(int post_id, string dir, CookieCollection cookies)
        {
            //Thread.Sleep(Program.config.TimeOut);
            string post = GetPostPage(post_id, cookies);
            if (post == null) { return false; }
            string url = GetOriginalUrlFromPostPage(post);
            if (url == null)
            {
                Console.WriteLine("URL Картинки не получен!");
                return false;
            }
            string filename = GetFileName(dir, url);
            if (IsImageFile(filename))
            {
                Console.Write("Добавляем информацию в базу данных...");
                //DateTime start_db = DateTime.Now;
                //GetTagsFromSankaku(Path.GetFileNameWithoutExtension(url), post);
                //DateTime stop_db = DateTime.Now;
                //Console.WriteLine("{0} секунд", (stop_db - start_db).TotalSeconds);
                Console.WriteLine("OK");
            }
            /*if (ExistImage(Path.GetFileNameWithoutExtension(url)))
            {
                Console.WriteLine("Уже скачан: {0}", store_file);
                //count_skip++;
                return true;
            }*/
            Console.WriteLine("Начинаем закачку {0}.", url);
            FileInfo fi = new FileInfo(filename);
            //ВРЕМЕННО!!!!!!!!
            //if (fi.Exists)
            //{
            //Console.WriteLine("Уже скачан.");
            //return true;
            //}
            //Thread.Sleep(Program.config.TimeOut - 2000);
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
                httpWRQ.Referer = "https://gelbooru.com/";
                httpWRQ.UserAgent = Program.config.UserAgent;
                httpWRQ.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                httpWRQ.Headers.Add("Accept-Encoding: identity");
                httpWRQ.CookieContainer = new CookieContainer();
                httpWRQ.CookieContainer.Add(cookies);
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
        static string GetPostPage(int npost, CookieCollection cookies)
        {
            Random rnd = new Random();
            //string strURL = Program.config.BaseURL + "post/show/" + npost.ToString();
            string strURL = "https://gelbooru.com/index.php?page=post&s=view&id=" + npost.ToString();
            Console.WriteLine("Загружаем и парсим пост: " + strURL);
            while (true)
            {
                try
                {
                    return DownloadStringFromGelbooru(strURL, "https://gelbooru.com/", cookies);
                }
                catch (WebException we)
                {
                    Console.WriteLine(we.Message);
                    Thread.Sleep(5000);
                    return null;
                }
            }
        }
        static string GetOriginalUrlFromPostPage(string post)
        {
            string file_url = "<li>Original: <a href=\"";
            string prefix = "<a href=\"";
            string postfix = "\" style=\"font-weight: bold;\">Original image</a>";
            Regex rx = new Regex(prefix + @"\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?" + postfix, RegexOptions.Compiled);
            Regex url_rx = new Regex(@"(?<protocol>http(s)?)://(?<server>([A-Za-z0-9-]+\.)*(?<basedomain>[A-Za-z0-9-]+\.[A-Za-z0-9]+))+((:)?(?<port>[0-9]+)?(/?)(?<path>(?<dir>[A-Za-z0-9\._\-/]+)(/){0,1}[A-Za-z0-9.-/_]*)){0,1}", RegexOptions.Compiled);
            try
            {
                Match match = rx.Match(post);
                if (match.Success)
                {
                    Match url_match = url_rx.Match(match.Value);
                    if (url_match.Success)
                    {
                        return url_match.Value;
                    }
                }
                return null;
            }
            catch (ArgumentNullException)
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
        static void LoadSettings()
        {
            Program.config = new ErzaConfig();
            //Параметры по умолчанию
            Program.config.ConnectionString = @"data source=C:\utils\erza\erza.sqlite";
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
            if (File.Exists(".\\GetGelbooru.json"))
            {
                using (FileStream fs = new FileStream(".\\GetGelbooru.json", FileMode.Open))
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
                        mImgDescriptor.Hash = node.Attributes[j].Value;
                        //mImgDescriptor.hash = HexStringToBytes(node.Attributes[j].Value);
                    }
                    if (node.Attributes[j].Name == "file_url")
                    {
                        mImgDescriptor.FilePath = "http:" + node.Attributes[j].Value;

                    }
                    if (node.Attributes[j].Name == "id")
                    {
                        mImgDescriptor.ImageID = System.Convert.ToInt64(node.Attributes[j].Value);
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
        static List<ImageInfo> SliyanieLists(List<ImageInfo> list, List<ImageInfo> temp)
        {
            for (int temp_i = 0; temp_i < temp.Count; temp_i++)
            {
                int t = FindHash(list, temp[temp_i]);
                if (t < 0)
                {
                    list.Add(temp[temp_i]);
                }
            }
            return list;
        }
        static int FindHash(List<ImageInfo> list, ImageInfo img)
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
        
        static string GetReferer(string url, long PostID)
        {
            if (url.LastIndexOf("gelbooru.com/") >= 0)
            {
                Uri uri = new Uri("http://gelbooru.com/index.php?page=post&s=view&id=" + PostID.ToString());
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
                httpWRQ.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
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
        static List<int> ParseHTML_sankaku(string html)
        {
            List<int> temp = new List<int>();
            Regex rx_digit = new Regex("[0-9]+", RegexOptions.Compiled);
            Regex rx = new Regex("alt=\"Image: [0-9]*\"", RegexOptions.Compiled);
            MatchCollection matches = rx.Matches(html);
            foreach (Match match in matches)
            {
                temp.Add(int.Parse(rx_digit.Match(match.Value).Value));
            }
            return temp;
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
