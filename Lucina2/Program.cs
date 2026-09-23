using AngleSharp;
//using AngleSharp.Parser.Html;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Lucina
{
    class Program
    {
        static WebProxy proxy = null;
        static int count_tags = 0;
        //static string USER_AGENT = "Mozilla / 5.0 (Windows NT 10.0; Win64; x64) AppleWebKit / 537.36 (KHTML, like Gecko) Chrome / 124.0.0.0 YaBrowser / 24.6.0.0 Safari / 537.36";
        static string USER_AGENT = "Lucina / 2.0.0.0";
        static SQLiteConnection connection = null;
        private static HttpClient client;
        static void Main(string[] args)
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler()
            {
                Proxy = new WebProxy(File.ReadAllText(@"C:\utils\cfg\erza\proxy.txt"), false),
                PreAuthenticate = false,
                UseDefaultCredentials = false,
            };
            //httpClientHandler.Credentials = new NetworkCredential(proxyServerSettings.UserName, proxyServerSettings.Password);
            client = new HttpClient(httpClientHandler);
            // Добавляем User-Agent в заголовки по умолчанию
            client.DefaultRequestHeaders.UserAgent.ParseAdd(USER_AGENT);
            connection = new SQLiteConnection("data source=C:\\utils\\data\\erza.sqlite");
            connection.Open();
            List<Tag> TagList;

            TagList = GetTagsFromDanbooru();
            LoadToSQLite(TagList);
            TagList.Clear();

            TagList = GetTagsFromKonachan();
            LoadToSQLite(TagList);
            TagList.Clear();

            TagList = GetTagsYandere();
            LoadToSQLite(TagList);
            TagList.Clear();

            TagList = GetTagsFromGelbooru();
            LoadToSQLite(TagList);
            TagList.Clear();

            //GetTagsFromSankaku();
            //Console.WriteLine($"Тегов получено: {count}");
            //Console.WriteLine("Обновляем теги в базе данных");

            connection.Close();
            Console.WriteLine($"Получено и сохранено в базе данных {Program.count_tags} тегов");
        }
        #region Danbooru
        static List<Tag> GetTagsFromDanbooru()
        {
            string API_KEY = File.ReadAllText(@"C:\utils\cfg\erza\danbooru-apikey.txt");
            string LOGIN = File.ReadAllText(@"C:\utils\cfg\erza\danbooru-login.txt");
            const int DANBOORU_LIMIT_POSTS = 1000;
            int nPage = 1;
            List<Tag> img_list = new List<Tag>();
            int count_errors = 0;
            for (; ; )
            {
                string strURL = String.Format($"https://danbooru.donmai.us/tags.xml?page={nPage}&limit={DANBOORU_LIMIT_POSTS}&login={LOGIN}&api_key={API_KEY}");
                Console.WriteLine("({0}/ХЗ) Загружаем и парсим: {1}", img_list.Count, strURL);
                try
                {
                    string xml = DownloadString(strURL, null);
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
            const int KONACHAN_LIMIT_POSTS = 1000;
            List<Tag> TagList = new List<Tag>();
            string strURL = String.Format($"https://konachan.com/tag.xml?limit={KONACHAN_LIMIT_POSTS}");
            Console.Write($"Загружаем и парсим: {strURL}");
            try
            {
                DateTime start = DateTime.Now;
                string xml = DownloadString(strURL, null);
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
        #region Yandere
        static List<Tag> GetTagsYandere()
        {
            const int YANDERE_LIMIT_POSTS = 1000;
            int nPage = 1;                //Счетчик страниц
            List<Tag> TagList = new List<Tag>();
            for (; ; )
            {
                string strURL = String.Format($"https://yande.re/tag.xml?page={nPage}&limit={YANDERE_LIMIT_POSTS}");
                Console.Write($"Загружаем и парсим: {strURL}");
                try
                {
                    System.Uri uri = new System.Uri(strURL);
                    DateTime start = DateTime.Now;
                    string xml = DownloadString(strURL, null);
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
        static List<Tag> GetTagsFromGelbooru()
        {
            List<Tag> TagList = new List<Tag>();
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
                    string page = DownloadString(url, url);
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
                        //LoadToSQLite(list);
                        TagList.AddRange(list);
                        pid = pid + list.Count;
                        if(pid > (max_pid + 20)) { break; }
                        errors = 0;
                        Thread.Sleep(5000);
                    }
                }
                catch (System.Exception e)
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
            return TagList;
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
                        string pid = a_element.GetAttribute("href");
                        int index = pid.LastIndexOf("=");
                        if (index != -1)
                        {
                            pid = pid.Substring(index+1);
                            return Convert.ToInt32(pid);
                        }
                        else
                        {
                            return -1;
                        }
                        
                    }
                }
            }
            catch (System.Exception e)
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
                                if(td.Length <= 0) { continue; }
                                Tag tag = new Tag();
                                tag.Site = "gelbooru.com";
                                //первая ячейка
                                //tag.Count = Convert.ToInt64(td[0].InnerHtml);
                                //вторая ячейка
                                //if (td[0].InnerHtml == "Name") { continue; }
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
                            catch (System.Exception e)
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
                case "metadata":
                    return 9;
                case "studio":
                    return 2;
            }
            return 0;
        }
        static void LoadToSQLite(List<Tag> TagList)
        {
            using (SQLiteTransaction transaction = connection.BeginTransaction()) { 
                for (int i = 0; i < TagList.Count; i++)
                {
                    Program.count_tags++;
                    Console.Write($"[{Program.count_tags}] {TagList[i].Name}");
                    if (String.IsNullOrWhiteSpace(TagList[i].Name)) continue;
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
                transaction.Commit();
            }
        }
        static bool ExistTagSQLite(Tag TagInfo, SQLiteConnection Connection)
        {
            using (SQLiteCommand comm = new SQLiteCommand("SELECT tag_id FROM tags WHERE tag = @tag", Connection))
            {
                comm.Parameters.AddWithValue("tag", TagInfo.Name);
                object o = comm.ExecuteScalar();
                if (o != null && o != DBNull.Value)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        static string DownloadString(string Url, string Referer)
        {
            //string responseBody = client.GetStringAsync(Url).GetAwaiter().GetResult();
            return DownloadStringAsync(Url, Referer).GetAwaiter().GetResult(); ;
        }
        private static async Task<string> DownloadStringAsync(string Url, string Referer)
        {
            // 1. Создаем объект запроса
            using (var request = new HttpRequestMessage(HttpMethod.Get, Url))
            {
                // 2. Добавляем заголовок Referer
                if(Referer != null) request.Headers.Referrer = new System.Uri(Referer);

                // 3. Добавляем заголовок Cookie (в формате "name1=value1; name2=value2")
                //request.Headers.Add("Cookie", Cookies.ToString());

                try
                {
                    // 4. Отправляем запрос
                    using (HttpResponseMessage response = await client.SendAsync(request))
                    {
                        // Проверяем статус ответа (выдаст ошибку, если статус не 200-299)
                        response.EnsureSuccessStatusCode();

                        // 5. Читаем строку из ответа
                        string responseBody = await response.Content.ReadAsStringAsync();
                        return responseBody;
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Ошибка: {e.Message}");
                    return null;
                }
            }
        }
        private static void SetCookieHandler()
        {
            // Создаем контейнер для кук
            var cookieContainer = new CookieContainer();

            // Добавляем начальную куку вручную для конкретного домена
            cookieContainer.Add(new System.Uri("https://example.com"), new Cookie("session_id", "12345"));

            var handler = new HttpClientHandler
            {
                CookieContainer = cookieContainer,
                UseCookies = true // Включает автоматическую работу с куками
            };

            // Передаем обработчик в HttpClient (делать это нужно ОДИН раз при старте)
            client = new HttpClient(handler);
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
