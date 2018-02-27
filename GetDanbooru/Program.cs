/* Copyright © Macsim Belous 2012 */
/* This file is part of GetDanbooru.

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
using System.Net;
using System.Xml;
using System.Globalization;
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

namespace GetDanbooru
{
    class Program
    {
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
            foreach (string tag in args)
            {
                Console.WriteLine("Импортируем тег " + tag + " с Данбуру");
                il = SliyanieLists(il, get_hash_danbooru_new_api(WebUtility.UrlEncode(tag)));
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
            Program.config.DownloadPath = Program.config.DownloadPath + "\\" + args[0];
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
        static void LoadSettings()
        {
            Program.config = new ErzaConfig();
            //Параметры по умолчанию
            Program.config.ConnectionString = @"data source=C:\utils\data\erza.sqlite";
            Program.config.UseDB = true;
            Program.config.UserAgent = "Mozilla / 5.0(Windows NT 6.2; WOW64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 34.0.1847.116 Safari / 537.36";
            Program.config.LimitError = 4;
            Program.config.DanbooruLogin = "";
            Program.config.DanbooruPassword = "";
            Program.config.DanbooruAPIKey = "";
            Program.config.DownloadPath = ".";
            Program.config.UseProxy = false;
            Program.config.ProxyAddress = null;
            Program.config.ProxyPort = 0;
            Program.config.ProxyLogin = null;
            Program.config.ProxyPassword = null;
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(ErzaConfig));
            if (File.Exists(@"C:\utils\cfg\GetDanbooru.json"))
            {
                using (FileStream fs = new FileStream(@"C:\utils\cfg\GetDanbooru.json", FileMode.Open))
                {
                    Program.config = (ErzaConfig)jsonFormatter.ReadObject(fs);
                }
                return;
            }
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Erza\\GetDanbooru.json"))
            {
                using (FileStream fs = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Erza\\GetDanbooru.json", FileMode.Open))
                {
                    Program.config = (ErzaConfig)jsonFormatter.ReadObject(fs);
                }
                return;
            }
            Console.WriteLine("Конфигурационный файл не найден!\nЗагружены настройки по умолчанью.");
        }
        static List<ImageInfo> get_hash_danbooru_new_api(string tag)
        {
            const int DANBOORU_LIMIT_POSTS = 100;
            int nPage = 1;                //Счетчик страниц
            List<ImageInfo> img_list = new List<ImageInfo>();
            int count_errors = 0;
            for (;;)
            {
                WebClient Client = new WebClient();
                if (Program.config.UseProxy)
                {
                    WebProxy myProxy = new WebProxy(Program.config.ProxyAddress, Program.config.ProxyPort);
                    myProxy.Credentials = new NetworkCredential(Program.config.ProxyLogin, Program.config.ProxyPassword);
                    Client.Proxy = myProxy;
                }
                string strURL = String.Format("http://danbooru.donmai.us/posts.xml?tags={0}&page={1}&login={2}&api_key={3}&limit={4}", tag, nPage, Program.config.DanbooruLogin, Program.config.DanbooruAPIKey, DANBOORU_LIMIT_POSTS);
                Console.WriteLine("({0}/ХЗ) Загружаем и парсим: {1}", img_list.Count, strURL);
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
                mImgDescriptor.Hash = md5.InnerText;
                //mImgDescriptor.hash = HexStringToBytes(md5.InnerText);
                XmlElement tags = node["tag-string"];
                mImgDescriptor.AddStringOfTags(tags.InnerText);
                XmlElement id = node["id"];
                mImgDescriptor.ImageID = System.Convert.ToInt32(id.InnerText);
                XmlElement ext = node["file-ext"];
                if (ext.InnerText.ToLower() == "txt") { break; }
                if (ext.InnerText.ToLower().Length > 4) { break; }
                if (ext.InnerText.ToLower().LastIndexOf('?') > -1) { break; }
                //mImgDescriptor.FilePath = "http://danbooru.donmai.us/data/" + md5.InnerText + "." + ext.InnerText;
                XmlElement url = node["file-url"];
                mImgDescriptor.FilePath = "http://danbooru.donmai.us" + url.InnerText;
                XmlElement height = node["image-height"];
                XmlElement width = node["image-width"];
                mImgDescriptor.Height = System.Convert.ToInt32(height.InnerText);
                mImgDescriptor.Width = System.Convert.ToInt32(width.InnerText);
                list.Add(mImgDescriptor);
            }
            return list;
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
                    long result = DownloadFile(img.FilePath, dir + "\\" + img.Hash + ".jpg", GetReferer(img.FilePath, img));
                    if (result == 0)
                    {
                        MyWait(start, 2500);
                        return 0;
                    }
                }
                else
                {
                    long result = DownloadFile(img.FilePath, dir + "\\" + img.Hash + extension, GetReferer(img.FilePath, img));
                    if (result == 0)
                    {
                        MyWait(start, 2500);
                        return 0;
                    }
                }
            }
            return 1;
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
        static string GetReferer(string url, ImageInfo img)
        {
            if (url.LastIndexOf("http://sonohara.donmai.us/") >= 0)
            {
                Uri uri = new Uri("http://danbooru.donmai.us/post/show/" + img.ImageID.ToString() + "/" + img.GetStringOfTags().Replace(' ', '-'));
                return uri.AbsoluteUri;
            }
            return String.Empty;
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
        public string DanbooruLogin;
        [DataMember]
        public string DanbooruPassword;
        [DataMember]
        public string DanbooruAPIKey;
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
