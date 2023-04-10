using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using IqdbApi;
using System.IO;
using System.Net;
using System.Xml;
using ErzaLib;
using System.Text.RegularExpressions;
using System.Threading;
using System.Net.Http;
using AngleSharp.Dom.Events;
using AngleSharp.Dom;
using AngleSharp;
using System.Net.Http.Headers;

namespace Shinon
{
    class Program
    {
        static string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/111.0.0.0 Safari/537.36";
        static bool USE_PROXY = true;
        static WebProxy proxy;
        static int Width = 800;
        static int Height = 800;
        //SauceNao API key.
        static string apiKey = "1bd2ae9f569df998e36cdc2ee9791fec4a157439";
        static SQLiteConnection Connection;
        static void Main(string[] args)
        {
            proxy = new WebProxy("nalsjn.ru", 8888);
            proxy.Credentials = new NetworkCredential(System.IO.File.ReadAllText(@"C:\utils\cfg\Shinon\login.txt"), System.IO.File.ReadAllText(@"C:\utils\cfg\Shinon\password.txt"));
            HttpClient saucenao_client = new HttpClient();
            saucenao_client.BaseAddress = new Uri("https://saucenao.com/");
            saucenao_client.DefaultRequestHeaders.Add("User-Agent", UserAgent);
            IIqdbClient iqdb_client = null;
            iqdb_client = new IqdbClient();
            Connection = new SQLiteConnection(@"data source=C:\utils\data\erza.sqlite");
            Connection.Open();
            List<ImageInfo> imgs;
            if (CountItemOnCache() <= 0)
            {
                imgs = GetImagesWithoutTags();
                Console.Write("Добавляем в Кэш...");
                SQLiteTransaction transact = Program.Connection.BeginTransaction();
                foreach (ImageInfo img in imgs)
                {
                    InsertItemFromCache(img.ImageID);
                }
                transact.Commit();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("ОК");
                Console.ResetColor();
            }
            else
            {
                imgs = GetImagesFromCache();
                List<ImageInfo> temp = GetNewImages();
                Console.Write("Добавляем в Кэш...");
                SQLiteTransaction transact = Program.Connection.BeginTransaction();
                foreach (ImageInfo img in temp)
                {
                    InsertItemFromCache(img.ImageID);
                }
                transact.Commit();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("ОК");
                Console.ResetColor();
                imgs.InsertRange(0, temp);
            }
            int error = 0;
            for (int i = 0; i < imgs.Count; i++)
            {
                //Thread.Sleep(15000);
                try
                {
                    if (imgs[i].FilePath == null || !System.IO.File.Exists(imgs[i].FilePath))
                    {
                        Console.WriteLine($"[{i}/{imgs.Count}] Отсутствует!");
                        continue;
                    }
                    Console.WriteLine($"[{i}/{imgs.Count}] Ишем дубликаты для {imgs[i].FilePath}...");
                    try
                    {
                        string temp = "E:\\previews\\" + imgs[i].Hash[0] + "\\" + imgs[i].Hash[1] + "\\" + imgs[i].Hash + ".jpg";
                        imgs[i].Tags = GetTagsFromIqdb2(temp, iqdb_client);
                        //imgs[i].Tags.AddRange(GetTagsFromSauceNao(temp, saucenao_client));
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Не удалось получить теги!");
                        Console.WriteLine(ex.Message);
                        Console.ResetColor();
                        continue;
                    }

                    Console.WriteLine($"Найдено {imgs[i].Tags.Count} тегов");
                    if (imgs[i].Tags.Count > 0)
                    {
                        Console.Write("Добавляем в БД...");
                        SQLiteTransaction transact = Program.Connection.BeginTransaction();
                        ErzaDB.LoadImageToErza(imgs[i], Program.Connection);
                        transact.Commit();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("ОК");
                        Console.ResetColor();
                    }
                    RemoveItemFromCache(imgs[i].ImageID);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.ResetColor();
                    error++;
                }
            }
            Console.WriteLine($"Ошибок: {error}");
            Connection.Close();
        }
        static List<string> GetTagsFromIqdb(string FilePath, IIqdbClient Client)
        {
            List<string> tags = new List<string>();
            using (var fs = new FileStream(FilePath, FileMode.Open))
            {
                var searchResults = Client.SearchFile(fs);
                if (searchResults.Result != null && searchResults.Result.Matches != null)
                {
                    foreach (IqdbApi.Models.Match item in searchResults.Result.Matches)
                    {
                        if (item.Similarity > 90)
                        {
                            Console.WriteLine($"URL {item.Url} Тегов {item.Tags.Count}");
                            tags.AddRange(item.Tags);
                        }
                    }
                    tags = tags.Distinct().ToList();
                }
            }
            return tags;
        }
        static List<string> GetTagsFromIqdb(Stream stream, IIqdbClient Client)
        {
            List<string> tags = new List<string>();
            var searchResults = Client.SearchFile(stream);
            if (searchResults.Result != null && searchResults.Result.Matches != null)
            {
                foreach (IqdbApi.Models.Match item in searchResults.Result.Matches)
                {
                    if (item.Similarity > 90)
                    {
                        Console.WriteLine($"URL {item.Url} Тегов {item.Tags.Count}");
                        tags.AddRange(item.Tags);
                    }
                }
                tags = tags.Distinct().ToList();
            }
            return tags;
        }
        static List<string> GetTagsFromIqdb2(string FilePath, IIqdbClient Client)
        {
            List<string> tags = new List<string>();
            using (var fs = new FileStream(FilePath, FileMode.Open))
            {
                var searchResults = Client.SearchFile(fs);
                foreach (IqdbApi.Models.Match item in searchResults.Result.Matches)
                {
                    if (item.Similarity <= 90) { continue; }
                    tags.AddRange(item.Tags);
                    string[] temp = null;
                    switch (item.Source)
                    {
                        case IqdbApi.Enums.Source.Danbooru:
                            temp = GetTagsFromDanbooru(item.Url, proxy);
                            if (temp != null)
                                tags.AddRange(tags);
                            break;
                        case IqdbApi.Enums.Source.Konachan:
                            temp = GetTagsFromKonachan(item.Url, proxy);
                            if (temp != null)
                                tags.AddRange(tags);
                            break;
                        case IqdbApi.Enums.Source.Yandere:
                            temp = GetTagsFromYandere(item.Url, proxy);
                            if (temp != null)
                                tags.AddRange(tags);
                            break;
                        case IqdbApi.Enums.Source.Gelbooru:
                            temp = GetTagsFromGelbooru(item.Url, proxy);
                            if (temp != null)
                                tags.AddRange(tags);
                            break;
                        case IqdbApi.Enums.Source.SankakuChannel:
                            temp = GetTagsFromSankaku(item.Url, proxy);
                            if (temp != null)
                                tags.AddRange(tags);
                            break;
                    }

                }
                tags = tags.Distinct().ToList();
            }
            return tags;
        }
        static List<string> GetTagsFromSauceNao(string FilePath, HttpClient Client)
        {
            List<string> tags = new List<string>();
            string post;
            using (var multipartFormContent = new MultipartFormDataContent())
            {
                var fileStreamContent = new StreamContent(System.IO.File.OpenRead(FilePath));
                fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                multipartFormContent.Add(fileStreamContent, name: "file", fileName: Path.GetFileName(FilePath));
                var response = Client.PostAsync("search.php", multipartFormContent);
                response.Result.EnsureSuccessStatusCode();
                post = response.Result.Content.ReadAsStringAsync().Result;
            }
            IConfiguration config = Configuration.Default;
            IBrowsingContext context = BrowsingContext.New(config);
            IDocument document = context.OpenAsync(req => req.Content(post)).Result;
            var links = document.QuerySelectorAll("a").ToList();
            foreach (var link in links)
            {
                String url = link.Attributes.GetNamedItem("href").Value;
                string[] temp_tags = null;
                if (url.IndexOf("https://danbooru.donmai.us/") >= 0)
                {
                    temp_tags = GetTagsFromDanbooru(url, proxy);
                    if (temp_tags != null) tags.AddRange(tags);
                }
                if (url.IndexOf("https://konachan.com/") >= 0)
                {
                    temp_tags = GetTagsFromKonachan(url, proxy);
                    if (temp_tags != null) tags.AddRange(tags);
                }
                if (url.IndexOf("https://yande.re/") >= 0)
                {
                    temp_tags = GetTagsFromYandere(url, proxy);
                    if (temp_tags != null) tags.AddRange(tags);
                }
                if (url.IndexOf("https://gelbooru.com/") >= 0)
                {
                    temp_tags = GetTagsFromGelbooru(url, proxy);
                    if (temp_tags != null) tags.AddRange(tags);
                }
                if (url.IndexOf("https://chan.sankakucomplex.com/") >= 0)
                {
                    temp_tags = GetTagsFromSankaku(url, proxy);
                    if (temp_tags != null) tags.AddRange(tags);
                }
            }
            return tags.Distinct().ToList();
        }
        static List<ImageInfo> GetImagesWithoutTags()
        {
            List<ImageInfo> imgs = new List<ImageInfo>();
            using (SQLiteCommand command = new SQLiteCommand())
            {
                command.CommandText = "SELECT images.image_id, images.hash, images.file_path FROM images LEFT OUTER JOIN image_tags on images.image_id = image_tags.image_id WHERE images.is_deleted = 0 AND image_tags.image_id IS NULL;";
                command.Connection = Connection;
                SQLiteDataReader reader = command.ExecuteReader();
                int count = 0;
                while (reader.Read())
                {
                    ImageInfo img = new ImageInfo();
                    img.Hash = (string)reader["hash"];
                    img.ImageID = (long)reader["image_id"];
                    img.FilePath = (string)reader["file_path"];
                    imgs.Add(img);
                    count++;
                }
                reader.Close();
            }
            return imgs;
        }
        static int CountItemOnCache()
        {
            using (SQLiteCommand command = new SQLiteCommand())
            {
                command.CommandText = "SELECT count(*) FROM shinon_cache;";
                command.Connection = Connection;
                int count = Convert.ToInt32(command.ExecuteScalar());
                return count;
            }
        }
        static List<ImageInfo> GetImagesFromCache()
        {
            List<ImageInfo> imgs = new List<ImageInfo>();
            using (SQLiteCommand command = new SQLiteCommand())
            {
                command.CommandText = "SELECT images.image_id, images.hash, images.file_path FROM shinon_cache LEFT OUTER JOIN images on shinon_cache.image_id = images.image_id WHERE images.file_path IS NOT NULL;";
                command.Connection = Connection;
                SQLiteDataReader reader = command.ExecuteReader();
                //int count = 0;
                while (reader.Read())
                {
                    ImageInfo img = new ImageInfo();
                    img.Hash = (string)reader["hash"];
                    img.ImageID = (long)reader["image_id"];
                    img.FilePath = (string)reader["file_path"];
                    imgs.Add(img);
                    //count++;
                }
                reader.Close();
            }
            return imgs;
        }
        static List<ImageInfo> GetNewImages()
        {
            List<ImageInfo> imgs = new List<ImageInfo>();
            using (SQLiteCommand command = new SQLiteCommand())
            {
                command.CommandText = "SELECT images.image_id, images.hash, images.file_path FROM images LEFT OUTER JOIN shinon_cache on images.image_id = shinon_cache.image_id LEFT OUTER JOIN image_tags on images.image_id = image_tags.image_id WHERE shinon_cache.image_id IS NULL AND images.is_deleted = 0 AND image_tags.image_id IS NULL;";
                command.Connection = Connection;
                SQLiteDataReader reader = command.ExecuteReader();
                //int count = 0;
                while (reader.Read())
                {
                    ImageInfo img = new ImageInfo();
                    img.Hash = (string)reader["hash"];
                    img.ImageID = (long)reader["image_id"];
                    img.FilePath = (string)reader["file_path"];
                    imgs.Add(img);
                    //count++;
                }
                reader.Close();
            }
            return imgs;
        }
        static void InsertItemFromCache(long ImageID)
        {
            using (SQLiteCommand command = new SQLiteCommand())
            {
                command.CommandText = "INSERT INTO shinon_cache (image_id) VALUES (@image_id);";
                command.Parameters.AddWithValue("image_id", ImageID);
                command.Connection = Connection;
                command.ExecuteNonQuery();
            }
        }
        static void RemoveItemFromCache(long ImageID)
        {
            using (SQLiteCommand command = new SQLiteCommand())
            {
                command.CommandText = "DELETE FROM shinon_cache WHERE image_id = @image_id;";
                command.Parameters.AddWithValue("image_id", ImageID);
                command.Connection = Connection;
                command.ExecuteNonQuery();
            }
        }
        #region Myparsers
        static string[] GetTagsFromDanbooru(string PostID, WebProxy proxy)
        {
            WebClient Client = new WebClient();
            Client.Headers["User-Agent"] = UserAgent;
            if (USE_PROXY)
            {
                Client.Proxy = proxy;
            }
            string strURL = String.Format("{0}.xml", PostID).Replace("/show/", "/");
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
                    return tags.Split(' ');
                }
                return null;
            }
            catch (WebException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"DanBooru: {ex.Message} {strURL}");
                Console.ResetColor();
                return null;
            }
            catch (XmlException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"DanBooru: {ex.Message}");
                Console.ResetColor();
                return null;
            }
            finally
            {
                if (Client != null)
                {
                    Client.Dispose();
                }
            }
        }
        static string[] GetTagsFromKonachan(string PostID, WebProxy proxy)
        {
            WebClient Client = new WebClient();
            Client.Headers["User-Agent"] = UserAgent;
            if (USE_PROXY)
            {
                Client.Proxy = proxy;
            }
            string temp = PostID.Substring(PostID.LastIndexOf('/')+1);
            string strURL = String.Format("http://konachan.com/post.xml?tags=id:{0}", temp);
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
                            return node.Attributes[j].Value.Split(' ');
                        }
                    }
                }
                return null;
            }
            catch (WebException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"KonaChan: {ex.Message}");
                Console.ResetColor();
                return null;
            }
            catch (XmlException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"KonaChan: {ex.Message}");
                Console.ResetColor();
                return null;
            }
            finally
            {
                if (Client != null)
                {
                    Client.Dispose();
                }
            }
        }
        static string[] GetTagsFromYandere(string PostID, WebProxy proxy)
        {
            WebClient Client = new WebClient();
            Client.Headers["User-Agent"] = UserAgent;
            if (USE_PROXY)
            {
                Client.Proxy = proxy;
            }
            string temp = PostID.Substring(PostID.LastIndexOf('/')+1);
            string strURL = "https://yande.re/post.xml?tags=id:" + temp;
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
                            return node.Attributes[j].Value.Split(' ');
                        }
                    }
                }
                return null;
            }
            catch (WebException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Yandere: {ex.Message}");
                Console.ResetColor();
                return null;
            }
            catch (XmlException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Yandere: {ex.Message}");
                Console.ResetColor();
                return null;
            }
            finally
            {
                if (Client != null)
                {
                    Client.Dispose();
                }
            }
        }
        static string[] GetTagsFromGelbooru(string PostID, WebProxy proxy)
        {
            WebClient Client = new WebClient();
            Client.Headers["User-Agent"] = UserAgent;
            if (USE_PROXY)
            {
                Client.Proxy = proxy;
            }
            //string strURL = PostID.Replace("page=post", "page=dapi");
            try
            {
                Uri uri = new Uri(PostID);
                string post = Client.DownloadString(uri);
                List<string> tags = new List<string>();
                Regex rx = new Regex("<title>(.+)</title>");
                Match match = rx.Match(post);
                if (match.Success)
                {
                    string temp = match.Value.Substring(("<title>").Length);
                    temp = temp.Replace(" - Image View -  | Gelbooru - Free Anime and Hentai Gallery</title>", String.Empty);
                    tags.AddRange(temp.Split(','));
                    for (int i = 0; i < tags.Count; i++)
                    {
                        tags[i] = tags[i].TrimStart(' ').Replace(' ', '_');
                        tags[i] = tags[i].Replace("&amp;#039;", "'");
                    }
                    return tags.ToArray();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"GelBooru: {ex.Message}");
                Console.ResetColor();
                return null;
            }
            finally
            {
                if (Client != null)
                {
                    Client.Dispose();
                }
            }
        }
        static string[] GetTagsFromSankaku(string BaseURL, WebProxy proxy)
        {
            //string BaseURL = "https://chan.sankakucomplex.com/";
            //string ua = "Mozilla / 5.0(Windows NT 6.2; WOW64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 34.0.1847.116 Safari / 537.36";
            //WebProxy proxy = new WebProxy("128.199.50.53", 8888);
            //proxy.Credentials = new NetworkCredential("maksim", "48sf54ro");
            //sankaku_cookies = GetSankakuCookies(BaseURL + "user/authenticate", proxy, ua);
            try
            {
                string post = DownloadStringFromSankaku(BaseURL, BaseURL, null, proxy, UserAgent);
                List<string> tags = new List<string>();
                //string tags_string = null;
                Regex rx = new Regex("<title>(.+)</title>");
                Match match = rx.Match(post);
                if (match.Success)
                {
                    string temp = match.Value.Substring(("<title>").Length);
                    temp = temp.Replace(" | Sankaku Channel</title>", String.Empty);
                    tags.AddRange(temp.Split(','));
                    for (int i = 0; i < tags.Count; i++)
                    {
                        tags[i] = tags[i].TrimStart(' ').Replace(' ', '_');
                    }
                    return tags.ToArray();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"SanKaku: {ex.Message}");
                Console.ResetColor();
                return null;
            }
        }
        static string DownloadStringFromSankaku(string url, string referer, string RawCookies, WebProxy proxy, string ua)
        {
            HttpWebRequest downloadRequest = (HttpWebRequest)WebRequest.Create(url);
            if (USE_PROXY)
            {
                downloadRequest.Proxy = proxy;
            }

            downloadRequest.UserAgent = ua;
            downloadRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            downloadRequest.Headers.Add("Accept-Encoding: identity");
            //downloadRequest.Headers.Add(HttpRequestHeader.Cookie, RawCookies);
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
        #endregion
    }
}
