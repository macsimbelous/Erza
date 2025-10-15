using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;
//using AngleSharp.Parser.Html;
using AngleSharp.Dom;
using System.Net;
using System.Threading;
using System.Xml;
using System.IO;
using System.Data.SQLite;
using System.Xml.Linq;
using AngleSharp.Html.Parser;

namespace Lucina
{
    class Program
    {
        static WebProxy proxy = null;
        static int count_tags = 0;
        static string USER_AGENT = "Mozilla / 5.0 (Windows NT 10.0; Win64; x64) AppleWebKit / 537.36 (KHTML, like Gecko) Chrome / 124.0.0.0 YaBrowser / 24.6.0.0 Safari / 537.36";
        static SQLiteConnection connection = null;
        static void Main(string[] args)
        {
            //https://chan.sankakucomplex.com/tag/index?order=date&page=2
            Console.WriteLine(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
#if DEBUG
            proxy = new WebProxy("127.0.0.1", 8888);
#else
            proxy = new WebProxy("77.73.71.83", 8888);
#endif
            connection = new SQLiteConnection("data source=C:\\utils\\data\\erza.sqlite");
            connection.Open();
            List<Tag> TagList;

            //TagList = GetTagsFromDanbooru();
            //LoadToPostgres(TagList);
            //TagList.Clear();

            TagList = GetTagsFromKonachan();
            //LoadToPostgres(TagList);
            LoadToSQLite(TagList);
            TagList.Clear();

            TagList = GetTagsYandere();
            //LoadToPostgres(TagList);
            LoadToSQLite(TagList);
            TagList.Clear();

            GetTagsFromGelbooru();

            //GetTagsFromSankaku();
            //Console.WriteLine($"Тегов получено: {count}");
            //Console.WriteLine("Обновляем теги в базе данных");

            connection.Close();
            Console.WriteLine($"Получено и сохранено в базе данных {Program.count_tags} тегов");
        }
        #region Danbooru
        static List<Tag> GetTagsFromDanbooru()
        {
            const string API_KEY = "KlKXxNoiLFiamylZi1E6iIZGV3x5ylouv-YEBN49U64";
            const string LOGIN = "macsimbelous";
            const int DANBOORU_LIMIT_POSTS = 1000;
            int nPage = 1;
            List<Tag> img_list = new List<Tag>();
            int count_errors = 0;
            WebClient Client = new WebClient();
            //Client.Headers.Add("User-Agent", USER_AGENT);
            Client.Headers["User-Agent"] = USER_AGENT;
            //Client.Headers["User-Agent"] = "Licina / 2.0 (Windows NT 10.0; Win64; x64)";
            if (Program.proxy != null)
            {
                Client.Proxy = Program.proxy;
            }
            for (; ; )
            {
                string strURL = String.Format($"https://danbooru.donmai.us/tags.xml?page={nPage}&limit={DANBOORU_LIMIT_POSTS}&login={LOGIN}&api_key={API_KEY}");
                Console.WriteLine("({0}/ХЗ) Загружаем и парсим: {1}", img_list.Count, strURL);
                try
                {
                    string xml = Client.DownloadString(strURL);
                    if (xml == null)
                    {
                        if (count_errors < 4)
                        {
                            count_errors++;
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                    List<Tag> list = ParseXMLDanBooru(xml);
                    if (list.Count <= 0)
                    {
                        break;
                    }
                    else
                    {
                        img_list.AddRange(list);
                        nPage++;
                    }
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
            }
            Client.Dispose();
            return img_list;
        }
        static List<Tag> ParseXMLDanBooru(string strXML)
        {
            List<Tag> list = new List<Tag>();
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
            XmlNodeList nodeList = mXML.GetElementsByTagName("tag");
            //Парсим посты
            foreach (XmlNode node in nodeList)
            {
                Tag mImgDescriptor = new Tag();

                XmlElement name = node["name"];
                if (name == null) { continue; }
                mImgDescriptor.Name = name.InnerText;

                XmlElement count = node["post-count"];
                mImgDescriptor.Count = System.Convert.ToInt64(count.InnerText);

                XmlElement type = node["category"];
                mImgDescriptor.Type = System.Convert.ToInt64(type.InnerText);
                mImgDescriptor.TypeName = GetTypeNameDanbooru(mImgDescriptor.Type);

                mImgDescriptor.Site = "danbooru.donmai.us";
                mImgDescriptor.Language = "eng";
                list.Add(mImgDescriptor);
            }
            return list;
        }
        static string GetTypeNameDanbooru(long Type)
        {
            switch (Type)
            {
                case 0:
                    return "general";
                case 1:
                    return "artist";
                case 3:
                    return "copyright";
                case 4:
                    return "character";
                case 5:
                    return "meta";
            }
            return "general";
        }
        #endregion
        #region Konachan
        static List<Tag> GetTagsFromKonachan()
        {
            const int KONACHAN_LIMIT_POSTS = 0;
            List<Tag> TagList = new List<Tag>();
            using (WebClient Client = new WebClient())
            {
                Client.Headers.Add("User-Agent", USER_AGENT);
                if (Program.proxy != null)
                {
                    Client.Proxy = Program.proxy;
                }
                string strURL = String.Format($"https://konachan.com/tag.xml?limit={KONACHAN_LIMIT_POSTS}");
                Console.Write($"Загружаем и парсим: {strURL}");
                try
                {
                    Uri uri = new Uri(strURL);
                    DateTime start = DateTime.Now;
                    string xml = Client.DownloadString(uri);
                    if (xml == null)
                    {
                        Console.WriteLine(" Ошибка! xml = null");
                        return TagList;
                    }
                    XmlDocument mXML = new XmlDocument();
                    try
                    {
                        mXML.LoadXml(xml);
                    }
                    catch (XmlException e)
                    {
                        Console.WriteLine(e.Message);
                        return TagList;
                    }
                    XmlNodeList nodeList = mXML.GetElementsByTagName("tag");
                    //Парсим посты
                    for (int i = 0; i < nodeList.Count; i++)
                    {
                        Tag t = new Tag();
                        XmlNode node = nodeList.Item(i);
                        for (int j = 0; j < node.Attributes.Count; j++)
                        {
                            //Тэги
                            if (node.Attributes[j].Name == "name")
                            {
                                t.Name = node.Attributes[j].Value;
                                //mImgDescriptor.tags = new List<string>(mImgDescriptor.tags_string.Split(' ')); //Получаем массив тэгов
                            }
                            if (node.Attributes[j].Name == "type")
                            {
                                t.Type = System.Convert.ToInt64(node.Attributes[j].Value);
                            }
                            if (node.Attributes[j].Name == "count")
                            {
                                t.Count = System.Convert.ToInt64(node.Attributes[j].Value);
                            }
                        }
                        t.Site = "konachan.com";
                        t.Language = "eng";
                        t.TypeName = GetTypeNameKonachan(t.Type);
                        TagList.Add(t);
                    }
                    return TagList;
                }
                catch (WebException we)
                {
                    Console.WriteLine(" Ошибка: " + we.Message);
                    Thread.Sleep(60000);
                    return TagList;
                }
            }
        }
        static string GetTypeNameKonachan(long Type)
        {
            switch (Type)
            {
                case 0:
                    return "general";
                case 1:
                    return "artist";
                case 3:
                    return "copyright";
                case 4:
                    return "character";
                case 5:
                    return "style";
                case 6:
                    return "circle";
            }
            return "general";
        }
        #endregion
        #region Sankakku
        static int GetTagsFromSankaku()
        {
            int count = 0;
            int pid = 1;                //Счетчик постов
            List<string> post_list = new List<string>();
            int errors = 0;
            for (; ; )
            {
                //if (pid == 3) { break; }
                string url = String.Format($"https://chan.sankakucomplex.com/tag/index?page={pid}");
                Console.WriteLine($"({pid}) Загружаем и парсим: {url}");
                try
                {
                    //string page = client.DownloadString(url);
                    string page = DownloadString(url, url, null, USER_AGENT);
                    List<Tag> list = ParseTagsPage(page);
                    if (list.Count <= 0)
                    {
                        break;
                    }
                    else
                    {
                        //LoadToPostgres(list);
                        pid++;
                        count = count + list.Count;
                        errors = 0;
                        Thread.Sleep(5000);
                    }
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Ошибка:\n{e.Message}");
                    Console.ResetColor();
                    if (errors < 100)
                    {
                        errors++;
                        Thread.Sleep(300000); //5 минут
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return count;
        }
        static List<Tag> ParseTagsPage(string Page)
        {
            List<Tag> tags = new List<Tag>();
            var parser = new HtmlParser();
            var document = parser.ParseDocument(Page);
            foreach (IElement element in document.QuerySelectorAll("table"))
            {
                if (element.GetAttribute("class") == "highlightable")
                {
                    foreach (IElement tbody_element in element.QuerySelectorAll("tbody"))
                    {
                        foreach (IElement trelement in tbody_element.QuerySelectorAll("tr"))
                        {
                            try
                            {
                                var td = trelement.QuerySelectorAll("td");
                                Tag tag = new Tag();
                                Tag tag_jpn = new Tag();
                                tag.Site = "chan.sankakucomplex.com";
                                tag_jpn.Site = tag.Site;
                                //первая ячейка
                                tag.Count = Convert.ToInt64(td[0].InnerHtml);
                                tag_jpn.Count = tag.Count;
                                //вторая ячейка
                                foreach (IElement a_element in td[1].QuerySelectorAll("a"))
                                {
                                    if (a_element.InnerHtml != "?")
                                    {
                                        string s = a_element.InnerHtml.Replace("\n", String.Empty);
                                        tag.Name = s.Replace(" ", String.Empty);
                                        //tag.Name = a_element.InnerHtml;
                                    }
                                }
                                //третья ячейка
                                foreach (IElement a_element in td[2].QuerySelectorAll("a"))
                                {
                                    if (a_element.InnerHtml != "?")
                                    {
                                        string s = a_element.InnerHtml.Replace("\n", String.Empty);
                                        tag_jpn.Name = s.Replace(" ", String.Empty);
                                        //tag.NameJpn = a_element.InnerHtml;
                                    }
                                }
                                //четвёртая ячейка
                                List<string> temp = new List<string>();
                                foreach (IElement a_element in td[3].QuerySelectorAll("a"))
                                {
                                    if (a_element.InnerHtml != "?")
                                    {
                                        temp.Add(a_element.InnerHtml);
                                    }
                                }
                                if (temp.Count > 0)
                                {
                                    tag.Parents = GetStringOfTags(temp);
                                    tag_jpn.Parents = tag.Parents;
                                }
                                //пятая ячейка
                                temp.Clear();
                                foreach (IElement a_element in td[4].QuerySelectorAll("a"))
                                {
                                    if (a_element.InnerHtml != "?")
                                    {
                                        temp.Add(a_element.InnerHtml);
                                    }
                                }
                                if (temp.Count > 0)
                                {
                                    tag.Children = GetStringOfTags(temp);
                                    tag_jpn.Children = tag.Children;
                                }
                                //шестая ячейка
                                string t = td[5].InnerHtml.Replace("\n", String.Empty);
                                tag.TypeName = t.ToLower();
                                tag.Type = GetTypeCode(tag.TypeName);

                                tag_jpn.Type = tag.Type;
                                tag_jpn.TypeName = tag.TypeName;
                                tag.Language = "eng";
                                tag_jpn.Language = "jpn";
                                if (!String.IsNullOrEmpty(tag.Name))
                                {
                                    tags.Add(tag);
                                }
                                if (!String.IsNullOrEmpty(tag_jpn.Name))
                                {
                                    tags.Add(tag_jpn);
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                        }
                    }
                }
            }
            return tags;
        }
        #endregion
        #region Yandere
        static List<Tag> GetTagsYandere()
        {
            const int YANDERE_LIMIT_POSTS = 1000;
            int nPage = 1;                //Счетчик страниц
            List<Tag> TagList = new List<Tag>();
            WebClient Client = new WebClient();
            Client.Headers.Add("User-Agent", USER_AGENT);
            if (Program.proxy != null)
            {
                Client.Proxy = Program.proxy;
            }
            for (; ; )
            {
                string strURL = String.Format($"https://yande.re/tag.xml?page={nPage}&limit={YANDERE_LIMIT_POSTS}");
                Console.Write($"Загружаем и парсим: {strURL}");
                try
                {
                    Uri uri = new Uri(strURL);
                    DateTime start = DateTime.Now;
                    string xml = Client.DownloadString(uri);
                    if (xml == null)
                    {
                        Console.WriteLine(" Ошибка! xml = null");
                        continue;
                    }
                    List<Tag> list = ParseXMLYandere(xml);
                    if (list.Count <= 0)
                    {
                        Console.WriteLine(" Все теги получены");
                        break;
                    }
                    else
                    {
                        TagList.AddRange(list);
                        Console.WriteLine($" Успех {TagList.Count}");
                        nPage++;
                    }
                }
                catch (WebException we)
                {
                    Console.WriteLine(" Ошибка: " + we.Message);
                    Thread.Sleep(60000);
                    continue;
                }
            }
            Client.Dispose();
            return TagList;
        }
        static List<Tag> ParseXMLYandere(string strXML)
        {
            List<Tag> list = new List<Tag>();
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
            XmlNodeList nodeList = mXML.GetElementsByTagName("tag");
            //Парсим посты
            for (int i = 0; i < nodeList.Count; i++)
            {
                Tag t = new Tag();
                XmlNode node = nodeList.Item(i);
                for (int j = 0; j < node.Attributes.Count; j++)
                {
                    //Тэги
                    if (node.Attributes[j].Name == "name")
                    {
                        t.Name = node.Attributes[j].Value;
                        //mImgDescriptor.tags = new List<string>(mImgDescriptor.tags_string.Split(' ')); //Получаем массив тэгов
                    }
                    if (node.Attributes[j].Name == "type")
                    {
                        t.Type = System.Convert.ToInt64(node.Attributes[j].Value);
                    }
                    if (node.Attributes[j].Name == "count")
                    {
                        t.Count = System.Convert.ToInt64(node.Attributes[j].Value);
                    }
                }
                t.Site = "yande.re";
                t.Language = "eng";
                t.TypeName = GetTypeNameYandere(t.Type);
                list.Add(t);
            }
            return list;
        }
        static string GetTypeNameYandere(long Type)
        {
            switch (Type)
            {
                case 0:
                    return "general";
                case 1:
                    return "artist";
                case 3:
                    return "copyright";
                case 4:
                    return "character";
                case 5:
                    return "circle";
                case 6:
                    return "faults";
            }
            return "general";
        }
        #endregion
        #region Gelbooru
        static int GetTagsFromGelbooru()
        {
            int pid = 0;
            int max_pid = 0;
            List<string> post_list = new List<string>();
            int errors = 0;
            bool init_page = true;
            for (; ; )
            {
                string url = String.Format($"https://gelbooru.com/index.php?page=tags&s=list&pid={pid}");
                Console.WriteLine($"({pid}) Загружаем и парсим: {url}");
                try
                {
                    //string page = client.DownloadString(url);
                    string page = DownloadString(url, url, null, USER_AGENT);
                    if (init_page)
                    {
                        init_page = false;
                        max_pid = GetMaxPIDGelbooru(page);
                    }
                    List<Tag> list = ParseTagsPageGelbooru(page);
                    if (list.Count <= 0)
                    {
                        break;
                    }
                    else
                    {
                        //LoadToPostgres(list);
                        LoadToSQLite(list);
                        pid = pid + list.Count;
                        if(pid > (max_pid + 20)) { break; }
                        errors = 0;
                        Thread.Sleep(5000);
                    }
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Ошибка:\n{e.Message}");
                    Console.ResetColor();
                    if (errors < 100)
                    {
                        errors++;
                        Thread.Sleep(300000); //5 минут
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return pid;
        }
        static int GetMaxPIDGelbooru(string Page)
        {
            try
            {
                var parser = new HtmlParser();
                var document = parser.ParseDocument(Page);
                foreach (IElement a_element in document.QuerySelectorAll("a"))
                {
                    if(a_element.InnerHtml == "»")
                    {
                        return Convert.ToInt32(a_element.InnerHtml);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return 0;
        }
        static List<Tag> ParseTagsPageGelbooru(string Page)
        {
            List<Tag> tags = new List<Tag>();
            var parser = new HtmlParser();
            var document = parser.ParseDocument(Page);
            foreach (IElement element in document.QuerySelectorAll("table"))
            {
                //string s222 = element.GetAttribute("class");
                if (element.GetAttribute("class") == "highlightable")
                {
                    foreach (IElement tbody_element in element.QuerySelectorAll("tbody"))
                    {
                        foreach (IElement trelement in tbody_element.QuerySelectorAll("tr"))
                        {
                            try
                            {
                                var td = trelement.QuerySelectorAll("td");
                                Tag tag = new Tag();
                                tag.Site = "gelbooru.com";
                                //первая ячейка
                                //tag.Count = Convert.ToInt64(td[0].InnerHtml);
                                //вторая ячейка
                                if (td[0].InnerHtml == "Name") { continue; }
                                foreach (IElement a_element in td[0].QuerySelectorAll("a"))
                                {
                                    string s = a_element.InnerHtml.Replace("\n", String.Empty);
                                    tag.Name = s.Replace(" ", "_");
                                }
                                foreach (IElement a_element in td[0].QuerySelectorAll("span"))
                                {
                                    if (a_element.GetAttribute("class") == "tag-count")
                                    {
                                        tag.Count = Convert.ToInt64(a_element.InnerHtml);
                                    }
                                }
                                //третья ячейка
                                string t = td[1].InnerHtml.Replace(",", String.Empty).Split(' ')[0];
                                tag.TypeName = t.ToLower();
                                tag.Type = GetTypeCode(tag.TypeName);
                                tag.Language = "eng";
                                tags.Add(tag);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                        }
                    }
                }
            }
            return tags;
        }
        #endregion

        static long GetTypeCode(string Type)
        {
            switch (Type)
            {
                case "general":
                    return 0;
                case "artist":
                    return 1;
                case "copyright":
                    return 3;
                case "character":
                    return 4;
                case "circle":
                    return 5;
                case "faults":
                    return 6;
                case "medium":
                    return 8;
                case "meta":
                    return 9;
                case "studio":
                    return 2;
            }
            return 0;
        }

        static string GetStringOfTags(List<string> Tags)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Tags.Count; i++)
            {
                if (i == 0)
                {
                    sb.Append(Tags[i]);
                }
                else
                {
                    sb.Append(' ');
                    sb.Append(Tags[i]);
                }
            }
            return sb.ToString();
        }
        static void LoadToSQLite(List<Tag> TagList)
        {
            for (int i = 0; i < TagList.Count; i++)
            {
                Program.count_tags++;
                Console.Write($"[{Program.count_tags}] {TagList[i].Name}");
                //SetTypeTag(TagList[i].Name, TagList[i].Type, connection);
                if (ExistTagSQLite(TagList[i], connection))
                {
                    using (SQLiteCommand comm = new SQLiteCommand("UPDATE tags SET type = @type WHERE tag = @tag", connection))
                    {
                        comm.Parameters.AddWithValue("type", TagList[i].Type);
                        comm.Parameters.AddWithValue("tag", TagList[i].Name);
                        comm.ExecuteNonQuery();
                    }
                }
                else
                {
                    using (SQLiteCommand comm = new SQLiteCommand("INSERT INTO tags(type, tag) VALUES (@type, @tag);", connection))
                    {
                        comm.Parameters.AddWithValue("@type", TagList[i].Type);
                        comm.Parameters.AddWithValue("@tag", TagList[i].Name);
                        comm.ExecuteNonQuery();
                    }
                }
                Console.WriteLine(" Успех");
            }
        }
        static bool ExistTagSQLite(Tag TagInfo, SQLiteConnection Connection)
        {
            using (SQLiteCommand comm = new SQLiteCommand("SELECT tag_id FROM tags WHERE tag = @tag", Connection))
            {
                comm.Parameters.AddWithValue("tag", TagInfo.Name);
                object o = comm.ExecuteScalar();
                if (o != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        static string DownloadString(string Url, string Referer, CookieCollection Cookies, string UserAgent)
        {
            HttpWebRequest downloadRequest = (HttpWebRequest)WebRequest.Create(Url);
            if (Program.proxy != null)
            {
                downloadRequest.Proxy = Program.proxy;
            }
            downloadRequest.UserAgent = UserAgent;
            //downloadRequest.CookieContainer = new CookieContainer();
            //downloadRequest.CookieContainer.Add(Cookies);
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
    }
    public class Tag
    {
        public long Type = 0;
        public string Site = null;
        public string Name = null;
        public string Language = null;
        public long Count = 0;
        public string Parents = null;
        public string Children = null;
        public string TypeName = null;
        public Tag()
        {
        }
        public Tag(long Type, string Site, string Name)
        {
            this.Type = Type;
            this.Name = Name;
            this.Site = Site;
        }
        public Tag(long Type, long Count, string Site, string Name, string Language, string Parents, string Children, string TypeName)
        {
            this.Type = Type;
            this.Name = Name;
            this.Language = Language;
            this.Site = Site;
            this.Parents = Parents;
            this.Children = Children;
            this.TypeName = TypeName;
            this.Count = Count;
        }
        override public string ToString()
        {
            return this.Name;
        }
    }
}
