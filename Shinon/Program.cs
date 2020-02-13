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
using System.Drawing.Imaging;
using System.Drawing;
using ErzaLib;
using System.Text.RegularExpressions;
using TagLib;
using ImageDimensions;

namespace Shinon
{
    class Program
    {
        static bool USE_PROXY = true;
        static WebProxy proxy;
        static int Width = 800;
        static int Height = 800;
        static SQLiteConnection Connection;
        static void Main(string[] args)
        {
            proxy = new WebProxy("nalsjn.ru", 3128);
            proxy.Credentials = new NetworkCredential(System.IO.File.ReadAllText(@"C:\utils\cfg\Shinon\login.txt"), System.IO.File.ReadAllText(@"C:\utils\cfg\Shinon\password.txt"));
            IIqdbClient client = new IqdbClient();
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
            }
            int error = 0;
            for(int i = 0; i < imgs.Count; i++)
            {
                try
                {
                    if (imgs[i].FilePath == null || !System.IO.File.Exists(imgs[i].FilePath))
                    {
                        Console.WriteLine($"[{i}/{imgs.Count}] Отсутствует!");
                        continue;
                    }
                    Console.WriteLine($"[{i}/{imgs.Count}] Ишем дубликаты для {imgs[i].FilePath}...");
                    if (ImageIsBig(imgs[i].FilePath))
                    {
                        Console.Write($"Слишком большой, уменьшаем...");
                        Stream stream = CreatePreviewStream(imgs[i].FilePath);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("ОК");
                        Console.ResetColor();
                        imgs[i].Tags = GetTagsFromIqdb(stream, client);
                    }
                    else
                    {
                        imgs[i].Tags = GetTagsFromIqdb(imgs[i].FilePath, client);
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
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.ResetColor();
                    error++;
                }
                RemoveItemFromCache(imgs[i].ImageID);
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
        static List<string> GetTagsFromIqdb(Stream stream, IIqdbClient Client)
        {
            List<string> tags = new List<string>();
            var searchResults = Client.SearchFile(stream);
            foreach (IqdbApi.Models.Match item in searchResults.Result.Matches)
            {
                if (item.Similarity > 90)
                {
                    Console.WriteLine($"URL {item.Url} Тегов {item.Tags.Count}");
                    tags.AddRange(item.Tags);
                }
            }
            tags = tags.Distinct().ToList();
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
                    if (item.Similarity < 90) { continue; }
                    string[] temp = null;
                    switch (item.Source)
                    {
                        case IqdbApi.Enums.Source.Danbooru:
                            temp = GetImageInfoFromDanbooru(item.Url, proxy);
                            if(temp != null)
                                tags.AddRange(tags);
                            break;
                        case IqdbApi.Enums.Source.Konachan:
                            temp = GetImageInfoFromKonachan(item.Url, proxy);
                            if (temp != null)
                                tags.AddRange(tags);
                            break;
                        case IqdbApi.Enums.Source.Yandere:
                            temp = GetImageInfoFromYandere(item.Url, proxy);
                            if (temp != null)
                                tags.AddRange(tags);
                            break;
                        case IqdbApi.Enums.Source.Gelbooru:
                            temp = GetImageInfoFromGelbooru(item.Url, proxy);
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
                command.CommandText = "SELECT images.image_id, images.hash, images.file_path FROM shinon_cache LEFT OUTER JOIN images on shinon_cache.image_id = images.image_id;";
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
        #region Create Preview
        static Stream CreatePreviewStream(string FilePath)
        {
            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 80L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            Bitmap preview = CreateThumbnail(FilePath, Width, Height);
            MemoryStream stream = new MemoryStream();
            preview.Save(stream, jpgEncoder, myEncoderParameters);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
        static Bitmap CreateThumbnail(string lcFilename, int lnWidth, int lnHeight)
        {
            Bitmap bmpOut = null;
            try
            {
                Bitmap loBMP = new Bitmap(lcFilename);
                ImageFormat loFormat = loBMP.RawFormat;

                int lnNewWidth = 0;
                int lnNewHeight = 0;

                if (loBMP.Width < lnWidth && loBMP.Height < lnHeight)
                    return loBMP;

                float temp = (float)loBMP.Width / (float)lnWidth;
                if ((int)((float)loBMP.Height / temp) > lnHeight)
                {
                    temp = (float)loBMP.Height / (float)lnHeight;
                    lnNewWidth = (int)((float)loBMP.Width / temp);
                    lnNewHeight = lnHeight;
                }
                else
                {
                    lnNewWidth = lnWidth;
                    lnNewHeight = (int)((float)loBMP.Height / temp);
                }
                bmpOut = new Bitmap(lnNewWidth, lnNewHeight);
                Graphics g = Graphics.FromImage(bmpOut);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.FillRectangle(Brushes.White, 0, 0, lnNewWidth, lnNewHeight);
                g.DrawImage(loBMP, 0, 0, lnNewWidth, lnNewHeight);

                loBMP.Dispose();
            }
            catch
            {
                return null;
            }

            return bmpOut;
        }
        static ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
        #endregion
        #region Myparsers
        static string[] GetImageInfoFromDanbooru(string hash, WebProxy proxy)
        {
            WebClient Client = new WebClient();
            if (USE_PROXY)
            {
                Client.Proxy = proxy;
            }
            string post = hash.Replace("//danbooru.donmai.us/posts/", String.Empty);
            string strURL = String.Format("http://danbooru.donmai.us/posts/{0}.xml?login={1}&api_key={2}", post, "macsimbelous", "KlKXxNoiLFiamylZi1E6iIZGV3x5ylouv-YEBN49U64");
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
                ex.GetType();
                Console.WriteLine(ex.Message);
                //error++;
                return null;
            }
            catch (XmlException ex)
            {
                Console.WriteLine(ex.Message);
                //error++;
                ex.GetType();
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
        static string[] GetImageInfoFromKonachan(string hash, WebProxy proxy)
        {
            WebClient Client;
            Client = new WebClient();
            if (USE_PROXY)
            {
                Client.Proxy = proxy;
            }
            string strURL = String.Format("http://konachan.com/post.xml?tags=id:{0}", hash.Replace("//konachan.com/post/show/", String.Empty));
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
                    return null;
                }
                return null;
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
                //error++;
                return null;
            }
            catch (XmlException ex)
            {
                Console.WriteLine(ex.Message);
                //error++;
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
        static string[] GetImageInfoFromYandere(string hash, WebProxy proxy)
        {
            WebClient Client;
            Client = new WebClient();
            if (USE_PROXY)
            {
                Client.Proxy = proxy;
            }
            string strURL = "https://yande.re/post.xml?tags=id:" + hash.Replace("https://yande.re/post/show/", String.Empty);
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
                    return null;
                }
                return null;
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
                //error++;
                return null;
            }
            catch (XmlException ex)
            {
                Console.WriteLine(ex.Message);
                //error++;
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
        static string[] GetImageInfoFromGelbooru(string hash, WebProxy proxy)
        {
            WebClient Client;
            Client = new WebClient();
            if (USE_PROXY)
            {
                Client.Proxy = proxy;
            }
            string strURL = String.Format("https://gelbooru.com/index.php?page=dapi&s=post&q=index&id={0}", hash.Replace("//gelbooru.com/index.php?page=post&s=view&id=", String.Empty));
            try
            {
                Uri uri = new Uri(strURL);
                XmlDocument mXML = new XmlDocument();
                //string s = Client.DownloadString(uri);
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
                    return null;
                }
                return null;
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
                //error++;
                return null;
            }
            catch (XmlException ex)
            {
                Console.WriteLine(ex.Message);
                //error++;
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
            string ua = "Mozilla / 5.0(Windows NT 6.2; WOW64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 34.0.1847.116 Safari / 537.36";
            //WebProxy proxy = new WebProxy("128.199.50.53", 8888);
            //proxy.Credentials = new NetworkCredential("maksim", "48sf54ro");
            //sankaku_cookies = GetSankakuCookies(BaseURL + "user/authenticate", proxy, ua);
            try
            {
                string strURL = "https:" + BaseURL;
                string post = DownloadStringFromSankaku(strURL, BaseURL, null, proxy, ua);
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
                Console.WriteLine(ex.Message);
                //error++;
                //Thread.Sleep(120000);
                return null;
            }
        }
        static string GetSankakuCookies(string url, WebProxy proxy, string ua)
        {
            try
            {
                HttpWebRequest loginRequest = (HttpWebRequest)WebRequest.Create(url);
                if (USE_PROXY)
                {
                    loginRequest.Proxy = proxy;
                }

                loginRequest.UserAgent = ua;
                loginRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                loginRequest.ContentType = "application/x-www-form-urlencoded";
                loginRequest.Headers.Add("Accept-Encoding: identity");
                loginRequest.CookieContainer = new CookieContainer();
                loginRequest.Method = "POST";
                string PostData = String.Format("user%5Bname%5D={0}&user%5Bpassword%5D={1}", "macsimbelous", "050782");
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
                return loginResponse.Headers["Set-Cookie"];
            }
            catch (WebException we)
            {
                Console.WriteLine(we.Message);
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
        static List<int> ParseHTML_sankaku(string html)
        {
            List<int> temp = new List<int>();
            Regex rx_digit = new Regex("[0-9]+", RegexOptions.Compiled);
            Regex rx = new Regex(@"PostModeMenu\.click\([0-9]*\)", RegexOptions.Compiled);
            MatchCollection matches = rx.Matches(html);
            foreach (Match match in matches)
            {
                temp.Add(int.Parse(rx_digit.Match(match.Value).Value));
            }
            return temp;
        }
        #endregion
        static bool ImageIsBig(string Path)
        {
            FileInfo fi = new FileInfo(Path);
            if(fi.Length >= 1000000)
            {
                return true;
            }
            try
            {
                Size s = GetImageSize2(Path);
                if (s.Width >= 7500 || s.Height >= 7500)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Не удалось определить разрешение!");
                Console.ResetColor();
                return true;
            }
        }
        static Size GetImageSize(string Path)
        {
            try
            {
                return ImageHelper.GetDimensions(Path);
            }
            catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ImageHelper.GetDimensions не сработал переходим к второму варианту...");
                Console.ResetColor();
            }
            TagLib.File file = TagLib.File.Create(Path);
            var image = file as TagLib.Image.File;
            if (image.Properties != null)
            {
                Size s = new Size();
                s.Height = image.Properties.PhotoHeight;
                s.Width = image.Properties.PhotoWidth;
                return s;
            }
            throw new Exception("TagLib не сработал");
        }
        static Size GetImageSize2(string Path)
        {
            using (Stream stream = System.IO.File.OpenRead(Path))
            {
                using (Image sourceImage = Image.FromStream(stream, false, false))
                {
                    Size s = new Size(sourceImage.Width, sourceImage.Height);
                    return s;
                }
            }
        }
    }
}
