/* Copyright © Macsim Belous 2014 */
/* This file is part of ErzaLib.

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
using System.Net;
using System.Xml;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace ErzaLib
{
    public class Grabber
    {
        private int LIMIT_ERRORS = 4;
        public List<string> Tags = null;
        public string ApiKeyForDanbooru = null;
        public string LoginForDanbooru = null;
        public bool UseKonachan = true;
        public bool UseDanbooru = true;
        public bool UseYandere = true;
        public bool UseGelbooru = true;
        public bool UseSankaku = true;
        public string UserAgent = null;
        public CookieCollection sankaku_cookies = null;
        public delegate void StatusCallBackT(ImageBoard CurrentSite, int CountImages, int TotalImages, int UnicalImagesForAllBoards, string StatusString);
        public StatusCallBackT StatusCallBack = null;
        public List<ImageInfo> Grab()
        {
            if (StatusCallBack == null) { return null; }
            List<ImageInfo> il = new List<ImageInfo>();
            foreach (string tag in Tags)
            {
                if (UseKonachan)
                {
                    StatusCallBack(ImageBoard.Konachan, 0, 0, il.Count, "Импортируем тег " + tag + " с коначан");
                    il = SliyanieLists(il, GetImagesFromKonachan(tag));
                }
                if (UseDanbooru)
                {
                    StatusCallBack(ImageBoard.Danbooru, 0, 0, il.Count, "Импортируем тег " + tag + " с данбуры");
                    il = SliyanieLists(il, GetImagesFromDanbooru(tag));
                }
                if (UseYandere)
                {
                    StatusCallBack(ImageBoard.Yandere, 0, 0, il.Count, "Импортируем тег " + tag + " с яндере");
                    il = SliyanieLists(il, GetImagesFromYandere(tag));
                }
                if (UseGelbooru)
                {
                    StatusCallBack(ImageBoard.Gelbooru, 0, 0, il.Count, "Импортируем тег " + tag + " с гелбуры");
                    il = SliyanieLists(il, GetImagesFromGelbooru(tag));
                }
                if (UseSankaku)
                {
                    StatusCallBack(ImageBoard.Sankaku, 0, 0, il.Count, "Импортируем тег " + tag + " с санкаки");
                    il = SliyanieLists(il, GetImagesFromSankaku(tag));
                }
            }
            return il;
        }
        public ImageInfo GetImageInfoFromImageBoards(string hash)
        {
            ImageInfo img = null;
            if (UseDanbooru)
            {
                ImageInfo temp_img = null;
                temp_img = GetImageInfoFromDanbooru(hash);
                if (temp_img != null)
                {
                    if (img != null)
                    {
                        img.AddTags(temp_img.tags);
                    }
                    else
                    {
                        img = temp_img;
                    }
                }
            }
            if (UseKonachan)
            {
                ImageInfo temp_img = null;
                temp_img = GetImageInfoFromKonachan(hash);
                if (temp_img != null)
                {
                    if (img != null)
                    {
                        img.AddTags(temp_img.tags);
                    }
                    else
                    {
                        img = temp_img;
                    }
                }
            }
            if (UseYandere)
            {
                ImageInfo temp_img = null;
                temp_img = GetImageInfoFromYandere(hash);
                if (temp_img != null)
                {
                    if (img != null)
                    {
                        img.AddTags(temp_img.tags);
                    }
                    else
                    {
                        img = temp_img;
                    }
                }
            }
            if (UseGelbooru)
            {
                ImageInfo temp_img = null;
                temp_img = GetImageInfoFromGelbooru(hash);
                if (temp_img != null)
                {
                    if (img != null)
                    {
                        img.AddTags(temp_img.tags);
                    }
                    else
                    {
                        img = temp_img;
                    }
                }
            }
            return img;
        }
        
        private List<ImageInfo> GetImagesFromKonachan(string tag)
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
                    StatusCallBack(ImageBoard.Konachan, 0, 0, -1, "Не удалось получить количество постов!");
                    throw new GrabberException("Не удалось получить количество постов!");
                }
            }
            if (nPostsCount == 0)
            {
                StatusCallBack(ImageBoard.Konachan, 0, 0, -1, "Ничего ненайдено!");
                return img_list;
            }
            Client = new WebClient();
            int count_errors = 0;
            for (; ; )
            {
                string strURL = String.Format("http://konachan.com/post.xml?tags={0}&page={1}&limit={2}", tag, nPage, KONACHAN_LIMIT_POSTS);
                StatusCallBack(ImageBoard.Konachan, img_list.Count, nPostsCount, -1, String.Format("({0}/{1}) Загружаем и парсим: {2}", img_list.Count, nPostsCount, strURL));
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
                            StatusCallBack(ImageBoard.Konachan, img_list.Count, nPostsCount, -1, "get_hash_konachan: XML IS NULL");
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
                        StatusCallBack(ImageBoard.Konachan, img_list.Count, nPostsCount, -1, null);
                    }
                    MyWait(start, 5000);
                }
                catch (WebException we)
                {
                    StatusCallBack(ImageBoard.Konachan, img_list.Count, nPostsCount, -1, "Ошибка: " + we.Message);
                    Thread.Sleep(10000);
                    continue;
                }
            }
            return img_list;
        }
        private List<ImageInfo> ParseXMLKonachan(string strXML)
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
        private List<ImageInfo> GetImagesFromDanbooru(string tag)
        {
            const int DANBOORU_LIMIT_POSTS = 100;
            int nPage = 1;                //Счетчик страниц
            List<ImageInfo> img_list = new List<ImageInfo>();
            int count_errors = 0;
            for (; ; )
            {
                WebClient Client = new WebClient();
                string strURL = String.Format("http://danbooru.donmai.us/posts.xml?tags={0}&page={1}&login={2}&api_key={3}&limit={4}", tag, nPage, LoginForDanbooru, ApiKeyForDanbooru, DANBOORU_LIMIT_POSTS);
                StatusCallBack(ImageBoard.Danbooru, img_list.Count, -1, -1, String.Format("({0}/ХЗ) Загружаем и парсим: {1}", img_list.Count, strURL));
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
                            StatusCallBack(ImageBoard.Danbooru, img_list.Count, -1, -1, "get_hash_danbooru_new_api: XML IS NULL");
                            break;
                        }
                    }
                    List<ImageInfo> list = ParseXMLDanbooru(xml);
                    if (list.Count <= 0)
                    {
                        break;
                    }
                    else
                    {
                        img_list.AddRange(list);
                        nPage++;
                        StatusCallBack(ImageBoard.Danbooru, img_list.Count, -1, -1, null);
                    }
                    MyWait(start, 5000);
                }
                catch (WebException we)
                {
                    StatusCallBack(ImageBoard.Danbooru, img_list.Count, 0, -1, "Ошибка: " + we.Message);
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
        private List<ImageInfo> ParseXMLDanbooru(string strXML)
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
                XmlElement md5 = node["md5"];
                if (md5 == null) { continue; }
                mImgDescriptor.SetHashString(md5.InnerText);
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
        private List<ImageInfo> GetImagesFromYandere(string tag)
        {
            const int YANDERE_LIMIT_POSTS = 100;
            int nPostsCount = 0;          //Счетчик постов для скачивания
            int nPage = 1;                //Счетчик страниц
            WebClient Client;
            List<ImageInfo> img_list = new List<ImageInfo>();
            int count = 0;
            while (true)
            {
                nPostsCount = posts_count(String.Format("http://yande.re/post.xml?tags={0}&limit=1", tag));
                if (nPostsCount >= 0)
                {
                    break;
                }
                count++;
                if (count >= LIMIT_ERRORS)
                {
                    StatusCallBack(ImageBoard.Yandere, -1, 0, -1, "Не удалось получить количество постов!");
                    throw new GrabberException("Не удалось получить количество постов!");
                }
            }
            if (nPostsCount == 0)
            {
                StatusCallBack(ImageBoard.Yandere, 0, 0, -1, "Ничего ненайдено!");
                return img_list;
            }
            Client = new WebClient();
            int count_errors = 0;
            for (; ; )
            {
                string strURL = String.Format("http://yande.re/post.xml?tags={0}&page={1}&limit={2}", tag, nPage, YANDERE_LIMIT_POSTS);
                StatusCallBack(ImageBoard.Yandere, img_list.Count, nPostsCount, -1, String.Format("({0}/{1}) Загружаем и парсим: {2}", img_list.Count, nPostsCount, strURL));
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
                            StatusCallBack(ImageBoard.Yandere, img_list.Count, nPostsCount, -1, "get_hash_imouto: XML IS NULL");
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
                        StatusCallBack(ImageBoard.Yandere, img_list.Count, nPostsCount, -1, null);
                    }
                    MyWait(start, 5000);
                }
                catch (WebException we)
                {
                    StatusCallBack(ImageBoard.Yandere, img_list.Count, nPostsCount, -1, we.Message);
                    Thread.Sleep(60000);
                    continue;
                }
            }
            Client.Dispose();
            return img_list;
        }
        private List<ImageInfo> ParseXMLYandere(string strXML)
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
            for (int i = 0; i < nodeList.Count; i++)
            {
                ImageInfo mImgDescriptor = new ImageInfo();
                XmlNode node = nodeList.Item(i);
                for (int j = 0; j < node.Attributes.Count; j++)
                {
                    if (node.Attributes[j].Name == "tags")
                    {
                        mImgDescriptor.AddStringOfTags(node.Attributes[j].Value);
                    }
                    if (node.Attributes[j].Name == "md5")
                    {
                        mImgDescriptor.SetHashString(node.Attributes[j].Value);
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
        private List<ImageInfo> GetImagesFromGelbooru(string tag)
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
                    StatusCallBack(ImageBoard.Gelbooru, -1, 0, -1, "Не удалось получить количество постов!");
                    throw new GrabberException("Не удалось получить количество постов!");
                }
            }
            if (nPostsCount == 0)
            {
                StatusCallBack(ImageBoard.Gelbooru, 0, 0, -1, "Ничего ненайдено!");
                return img_list;
            }
            Client = new WebClient();
            int count_errors = 0;
            for (; ; )
            {
                string strURL = String.Format("http://gelbooru.com/index.php?page=dapi&s=post&q=index&tags={0}&pid={1}&limit={2}", tag, nPage, GELBOORU_LIMIT_POSTS);
                StatusCallBack(ImageBoard.Gelbooru, img_list.Count, nPostsCount, -1, String.Format("({0}/{1}) Загружаем и парсим: {2}", img_list.Count, nPostsCount, strURL));
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
                            StatusCallBack(ImageBoard.Gelbooru, img_list.Count, nPostsCount, -1, "get_hash_gelbooru: XML IS NULL");
                            break;
                        }
                    }
                    List<ImageInfo> list = ParseXMLGelbooru(xml);
                    if (list.Count <= 0)
                    {
                        break;
                    }
                    else
                    {
                        img_list.AddRange(list);
                        nPage++;
                        StatusCallBack(ImageBoard.Gelbooru, img_list.Count, nPostsCount, -1, null);
                    }
                    MyWait(start, 5000);
                }
                catch (WebException we)
                {
                    StatusCallBack(ImageBoard.Gelbooru, img_list.Count, nPostsCount, -1, we.Message);
                    Thread.Sleep(10000);
                    continue;
                }
            }
            return img_list;
        }
        private List<ImageInfo> ParseXMLGelbooru(string strXML)
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
            for (int i = 0; i < nodeList.Count; i++)
            {
                ImageInfo mImgDescriptor = new ImageInfo();
                XmlNode node = nodeList.Item(i);
                for (int j = 0; j < node.Attributes.Count; j++)
                {
                    if (node.Attributes[j].Name == "tags")
                    {
                        mImgDescriptor.AddStringOfTags(node.Attributes[j].Value);
                    }
                    if (node.Attributes[j].Name == "md5")
                    {
                        mImgDescriptor.SetHashString(node.Attributes[j].Value);
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
        private List<ImageInfo> GetImagesFromSankaku(string tag)
        {
            if (sankaku_cookies == null)
            {
                int temp = 0;
                for (; ; )
                {
                    sankaku_cookies = GetSankakuCookies("http://chan.sankakucomplex.com/");
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
                            StatusCallBack(ImageBoard.Sankaku, -1, -1, -1, "Не удалось получить куки!");
                            throw new GrabberException("Не удалось получить куки!");
                            //return null;
                        }
                    }
                }
            }
            string BaseURL = "http://chan.sankakucomplex.com/post/index";
            List<ImageInfo> imgs = new List<ImageInfo>();
            int i = 1;
            while (true)
            {
                //StatusCallBack(ImageBoard.Sankaku, imgs.Count, -1, -1, String.Format("({0}/ХЗ) ", imgs.Count));
                DateTime start = DateTime.Now;
                StatusCallBack(ImageBoard.Sankaku, imgs.Count, -1, -1, "Загружаем и парсим: " + String.Format("{0}?tags={1}&page={2}", BaseURL, tag, i));
                string text = DownloadHTML(BaseURL, tag, i, sankaku_cookies);
                MyWait(start, 5000);
                if (text != null)
                {
                    List<ImageInfo> posts = ParseHTML_sankaku(text);
                    if (posts.Count > 0)
                    {
                        imgs.AddRange(posts);
                        i++;
                        StatusCallBack(ImageBoard.Sankaku, imgs.Count, -1, -1, null);
                        continue;
                    }
                    else break;
                }
                else break;
            }
            return imgs;
        }
        private CookieCollection GetSankakuCookies(string url)
        {
            try
            {
                HttpWebRequest loginRequest = (HttpWebRequest)WebRequest.Create(url);
                loginRequest.UserAgent = UserAgent;
                loginRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                loginRequest.Headers.Add("Accept-Encoding: identity");
                loginRequest.CookieContainer = new CookieContainer();
                HttpWebResponse loginResponse = (HttpWebResponse)loginRequest.GetResponse();
                return loginResponse.Cookies;
            }
            catch (WebException we)
            {
                StatusCallBack(ImageBoard.Sankaku, -1, -1, -1, "GetSankakuCookies" + we.Message);
                return null;
            }
        }
        private string DownloadHTML(string m_strBaseURL, string m_strTags, int nPage, CookieCollection cookies)
        {
            int count_503 = 0;
            string strURL = String.Format("{0}?tags={1}&page={2}", m_strBaseURL, m_strTags, nPage);
            //StatusCallBack(ImageBoard.Sankaku, 0, -1, -1, "Загружаем и парсим: " + strURL);
            while (true)
            {
                try
                {
                    return DownloadStringFromSankaku(strURL, null, cookies);
                }
                catch (WebException we)
                {
                    //StatusCallBack(ImageBoard.Sankaku, 0, 0, -1, "DownloadHTML: " + we.Message);
                    Thread.Sleep(60000);
                    if (we.Response == null) { continue; }
                    if (((HttpWebResponse)we.Response).StatusCode == HttpStatusCode.ServiceUnavailable)
                    {
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
        private string DownloadStringFromSankaku(string url, string referer, CookieCollection cookies)
        {
            HttpWebRequest downloadRequest = (HttpWebRequest)WebRequest.Create(url);
            downloadRequest.UserAgent = UserAgent;
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
        private List<ImageInfo> ParseHTML_sankaku(string html)
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
        private ImageInfo parse_json_one_sankaku(string json)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            json_image result = ser.Deserialize<json_image>(json);
            ImageInfo img = new ImageInfo();
            img.SetHashString(result.md5);
            img.AddStringOfTags(result.tags);
            img.sankaku_post_id = result.id;
            return img;
        }
        private int posts_count(string url)
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
                StatusCallBack(ImageBoard.Unknown, 0, 0, -1, "posts_count " + ex.Message);
                nLocalPostsCount = -1;
            }
            catch (NullReferenceException ex)
            {
                StatusCallBack(ImageBoard.Unknown, 0, 0, -1, "posts_count " + ex.Message);
                nLocalPostsCount = -1;
            }
            catch (ArgumentNullException ex)
            {
                StatusCallBack(ImageBoard.Unknown, 0, 0, -1, "posts_count " + ex.Message);
                nLocalPostsCount = -1;
            }
            catch (XmlException ex)
            {
                StatusCallBack(ImageBoard.Unknown, 0, 0, -1, "posts_count " + ex.Message);
                nLocalPostsCount = -1;
            }
            finally
            {
                Client.Dispose();
            }
            return nLocalPostsCount;
        }
        private ImageInfo GetImageInfoFromDanbooru(string hash)
        {
            WebClient Client = new WebClient();
            string strURL = String.Format("http://danbooru.donmai.us/posts.xml?tags=md5:{0}&login={1}&api_key={2}", hash, LoginForDanbooru, ApiKeyForDanbooru);
            try
            {
                Uri uri = new Uri(strURL);
                string tags = null;
                XmlDocument mXML = new XmlDocument();
                mXML.LoadXml(Client.DownloadString(uri));
                XmlNodeList nodeList = mXML.GetElementsByTagName("post");
                foreach (XmlNode node in nodeList)
                {
                    XmlElement xml_tags = node["tag-string"];
                    tags = xml_tags.InnerText;
                    ImageInfo img = new ImageInfo();
                    img.AddStringOfTags(tags);
                    img.SetHashString(hash);
                    return img;
                }
                return null;
            }
            catch (WebException we)
            {
                we.GetType();
                return null;
            }
            catch (XmlException e)
            {
                e.GetType();
                return null;
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
        private ImageInfo GetImageInfoFromKonachan(string hash)
        {
            WebClient Client;
            Client = new WebClient();
            string strURL = String.Format("http://konachan.com/post.xml?tags=md5:{0}", hash);
            try
            {
                Uri uri = new Uri(strURL);
                XmlDocument mXML = new XmlDocument();
                mXML.LoadXml(Client.DownloadString(uri));
                XmlNodeList nodeList = mXML.GetElementsByTagName("post");
                foreach (XmlNode node in nodeList)
                {
                    for (int j = 0; j < node.Attributes.Count; j++)
                    {
                        if (node.Attributes[j].Name == "tags")
                        {
                            ImageInfo img = new ImageInfo();
                            img.AddStringOfTags(node.Attributes[j].Value);
                            img.SetHashString(hash);
                            return img;
                        }
                    }
                    return null;
                }
                return null;
            }
            catch (WebException)
            {
                return null;
            }
            catch (XmlException)
            {
                return null;
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
        private ImageInfo GetImageInfoFromYandere(string hash)
        {
            WebClient Client;
            Client = new WebClient();
            string strURL = String.Format("http://yande.re/post.xml?tags=md5:{0}", hash);
            try
            {
                Uri uri = new Uri(strURL);
                XmlDocument mXML = new XmlDocument();
                mXML.LoadXml(Client.DownloadString(uri));
                XmlNodeList nodeList = mXML.GetElementsByTagName("post");
                foreach (XmlNode node in nodeList)
                {
                    for (int j = 0; j < node.Attributes.Count; j++)
                    {
                        if (node.Attributes[j].Name == "tags")
                        {
                            ImageInfo img = new ImageInfo();
                            img.AddStringOfTags(node.Attributes[j].Value);
                            img.SetHashString(hash);
                            return img;
                        }
                    }
                    return null;
                }
                return null;
            }
            catch (WebException)
            {
                return null;
            }
            catch (XmlException)
            {
                return null;
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
        private ImageInfo GetImageInfoFromGelbooru(string hash)
        {
            WebClient Client;
            Client = new WebClient();
            string strURL = String.Format("http://gelbooru.com/index.php?page=dapi&s=post&q=index&tags=md5:{0}", hash);
            try
            {
                Uri uri = new Uri(strURL);
                XmlDocument mXML = new XmlDocument();
                mXML.LoadXml(Client.DownloadString(uri));
                XmlNodeList nodeList = mXML.GetElementsByTagName("post");
                foreach (XmlNode node in nodeList)
                {
                    for (int j = 0; j < node.Attributes.Count; j++)
                    {
                        if (node.Attributes[j].Name == "tags")
                        {
                            ImageInfo img = new ImageInfo();
                            img.AddStringOfTags(node.Attributes[j].Value);
                            img.SetHashString(hash);
                            return img;
                        }
                    }
                    return null;
                }
                return null;
            }
            catch (WebException)
            {
                return null;
            }
            catch (XmlException)
            {
                return null;
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
        private List<ImageInfo> SliyanieLists(List<ImageInfo> list, List<ImageInfo> temp)
        {
            for (int temp_i = 0; temp_i < temp.Count; temp_i++)
            {
                int t = FindHash(list, temp[temp_i]);
                if (t < 0)
                {
                    list.Add(temp[temp_i]);
                }
                else
                {
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
        private int FindHash(List<ImageInfo> list, ImageInfo img)
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
        private void MyWait(DateTime start, int delay)
        {
            int current = (int)((DateTime.Now - start).TotalMilliseconds);
            if (current < delay)
            {
                Thread.Sleep(delay - current);
                return;
            }
            else
            {
                return;
            }
        }
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
    public enum ImageBoard { Konachan, Yandere, Danbooru, Gelbooru, Sankaku, Unknown }
    [Serializable()]
    public class GrabberException : System.Exception
    {
        public GrabberException() : base() { }
        public GrabberException(string message) : base(message) { }
        public GrabberException(string message, System.Exception inner) : base(message, inner) { }
        protected GrabberException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }
    }
}
