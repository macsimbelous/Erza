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

namespace GetGelbooru
{
    class Program
    {
        static CookieCollection gelbooru_cookies = null;
        static ErzaConfig config = null;
        static void Main(string[] args)
        {
            LoadSettings();
            List<ImageInfo> il = new List<ImageInfo>();
            if (args.Length <= 0)
            {
                Console.WriteLine("Не заданы теги!");
                return;
            }
            foreach(string tag in args)
            {
                Console.WriteLine("Импортируем тег " + tag + " с гелбуры");
                il = SliyanieLists(il, get_hash_gelbooru(WebUtility.UrlEncode(tag)));
            }
            if (il.Count <= 0)
            {
                Console.WriteLine("Ничего ненайдено.");
                return;
            }
            Console.Write("\n\n\n\t\tНАЧИНАЕТСЯ ЗАГРУЗКА\n\n\n");
            int count_complit = 0;
            int count_deleted = 0;
            int count_error = 0;
            int count_skip = 0;
            Directory.CreateDirectory(Program.config.DownloadPath);
            for (int i = 0; i < il.Count; i++)
            {
                Console.WriteLine("\n###### {0}/{1} ######", (i + 1), il.Count);
                long r = DownloadImage(il[i], Program.config.DownloadPath);
                if (r == 0)
                {
                    count_complit++;
                }
                else
                {
                    count_error++;
                }
            }
        }
        static List<ImageInfo> get_hash_gelbooru(string tag)
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
            List<ImageInfo> img_list = new List<ImageInfo>();
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
            for (;;)
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
        static long DownloadImage(ImageInfo img, string dir)
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
                string extension = Path.GetExtension(img.FilePath);
                DateTime start = DateTime.Now;
                if (extension == ".jpeg")
                {
                    long result = DownloadFile(img.FilePath, dir + "\\" + img.Hash + ".jpg", GetReferer(img.FilePath, img.ImageID));
                    if (result == 0)
                    {
                        MyWait(start, 2500);
                        return 0;
                    }
                }
                else
                {
                    long result = DownloadFile(img.FilePath, dir + "\\" + img.Hash + extension, GetReferer(img.FilePath, img.ImageID));
                    if (result == 0)
                    {
                        MyWait(start, 2500);
                        return 0;
                    }
                }
            }
            return 1;
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
