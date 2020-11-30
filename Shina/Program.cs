/* Copyright © Maksim Belous 2016 */
/* This file is part of Shina.

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
using System.Threading;
using System.Net;
using System.Net.Security;
using System.IO;
using System.Text.RegularExpressions;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using Npgsql;
using NpgsqlTypes;
using Nalsjn;
using AngleSharp;
//using AngleSharp.Parser.Html;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shina
{
    class Program
    {
        static int error = 0;
#if DEBUG
        static bool USE_PROXY = true;
#else
        static bool USE_PROXY = false;
#endif
        static MiniLogger log = null;
        static void Main(string[] args)
        {
            log = new MiniLogger("./Shina.log", false, "|");
            ServicePointManager.ServerCertificateValidationCallback = Validator;
            //строка подключения
            NpgsqlConnectionStringBuilder csb = new NpgsqlConnectionStringBuilder();
#if DEBUG
            csb.Host = "138.197.190.23";
#else
            csb.Host = "127.0.0.1";
#endif
            csb.Port = 5432;
            csb.Username = "erza";
            csb.Password = "48sf54ro";
            csb.Database = "erza";
            //csb.SslMode = SslMode.Require;
            NpgsqlConnection Connection = new NpgsqlConnection(csb.ConnectionString);
            Connection.Open();
            //Sankaku
            string BaseURL = "https://capi-v2.sankakucomplex.com/posts";
            string UserAgent = "Mozilla / 5.0(Windows NT 6.2; WOW64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 34.0.1847.116 Safari / 537.36";
            WebProxy proxy = new WebProxy("138.197.190.23", 3128);
            proxy.Credentials = new NetworkCredential("maksim", "48sf54ro");
            //string RawCookies = GetSankakuCookies(BaseURL + "user/authenticate", proxy, UserAgent);
            int success = 0;
            //string[] hashs = File.ReadAllLines("./hashs.txt");
            string[] hashs = ReadHashsFromPostgres(Connection).ToArray();
            for (int i = 0; i < hashs.Length; i++)
            {
                try
                {
                    List<string> tags = new List<string>();
                    string[] temp;
                    int SankakuPostId;
                    Console.WriteLine("Запрашивая теги для хэша: {0}\t{1}\\{2}", hashs[i], i + 1, hashs.Length);
                    //Sankaku
                    //temp = GetTagsFromSankakuByAngle(hashs[i], BaseURL, UserAgent, proxy, out SankakuPostId);
                    temp = GetTagsFromSankakuByAPI(hashs[i], BaseURL, UserAgent, proxy, out SankakuPostId);
                    if (temp != null)
                    {
                        tags.AddRange(temp);
                        Console.WriteLine("Sankaku: {0}", temp.Length);
                    }
                    else { Console.WriteLine("Sankaku: 0"); }
                    //Danbooru
                    temp = GetImageInfoFromDanbooru(hashs[i], proxy);
                    if (temp != null)
                    {
                        tags.AddRange(temp);
                        Console.WriteLine("Danbooru: {0}", temp.Length);
                    }
                    else { Console.WriteLine("Danbooru: 0"); }
                    //Konachan
                    temp = GetImageInfoFromKonachan(hashs[i], proxy);
                    if (temp != null)
                    {
                        tags.AddRange(temp);
                        Console.WriteLine("Konachan: {0}", temp.Length);
                    }
                    else { Console.WriteLine("Konachan: 0"); }
                    //Yandere
                    temp = GetImageInfoFromYandere(hashs[i], proxy);
                    if (temp != null)
                    {
                        tags.AddRange(temp);
                        Console.WriteLine("Yandere: {0}", temp.Length);
                    }
                    else { Console.WriteLine("Yandere: 0"); }
                    //Gelbooru
                    temp = GetImageInfoFromGelbooru(hashs[i], proxy);
                    if (temp != null)
                    {
                        tags.AddRange(temp);
                        Console.WriteLine("Gelbooru: {0}", temp.Length);
                    }
                    else { Console.WriteLine("Gelbooru: 0"); }
                    Console.Write("Всего тегов: {0} ", tags.Count);
                    tags = tags.Distinct().ToList();
                    Console.Write(" уникальных: {0}\n", tags.Count);
                    if (tags.Count > 0)
                    {
                        //File.WriteAllLines("./tags/" + hashs[i], tags, Encoding.UTF8);
                        AddTagsToPostgres(hashs[i], SankakuPostId, tags, Connection);
                        success++;
                    }
                    else
                    {
                        MarkToProcessed(hashs[i], Connection);
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.ResetColor();
                    log.Write(ex.Message);
                    log.Write(ex.StackTrace);
                }
                Thread.Sleep(7000);
            }
            Console.WriteLine("Запрошено:\t{0}\nПолучено:\t{1}\nОшибочно:\t{2}", hashs.Length, success, error);
            Connection.Close();
            log.Close();
        }
        public static bool Validator(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            //Console.WriteLine("colback!");
            return true;
        }
        static List<string> ReadHashsFromPostgres(NpgsqlConnection Connection)
        {
            List<string> list = new List<string>();
            using (NpgsqlCommand comm = new NpgsqlCommand("SELECT hash FROM public.shina WHERE processed = false", Connection))
            {
                using(NpgsqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(reader.GetString(0));
                    }
                }
            }
            return list;
        }
        static void AddTagsToPostgres(string Hash, int SankakuPostId, List<string> Tags, NpgsqlConnection Connection)
        {
            using (NpgsqlCommand comm = new NpgsqlCommand("UPDATE public.shina SET sankaku_post_id = @sankaku_post_id, tags = @tags, processed = @processed WHERE hash = @hash", Connection))
            {
                comm.Parameters.AddWithValue("@hash", Hash);
                comm.Parameters.AddWithValue("@tags", GetStringOfTags(Tags));
                if (SankakuPostId < 0)
                {
                    comm.Parameters.AddWithValue("@sankaku_post_id", DBNull.Value);
                }
                else
                {
                    comm.Parameters.AddWithValue("@sankaku_post_id", SankakuPostId);
                }
                comm.Parameters.AddWithValue("@processed", true);
                comm.ExecuteNonQuery();
            }
        }
        static void MarkToProcessed(string Hash, NpgsqlConnection Connection)
        {
            using (NpgsqlCommand comm = new NpgsqlCommand("UPDATE public.shina SET processed = @processed WHERE hash = @hash", Connection))
            {
                comm.Parameters.AddWithValue("@hash", Hash);
                comm.Parameters.AddWithValue("@processed", true);
                comm.ExecuteNonQuery();
            }
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
        static string[] GetImageInfoFromDanbooru(string hash, WebProxy proxy)
        {
            WebClient Client = new WebClient();
            if (USE_PROXY)
            {
                Client.Proxy = proxy;
            }
            string strURL = String.Format("http://danbooru.donmai.us/posts.xml?tags=md5:{0}&login={1}&api_key={2}", hash, "macsimbelous", "KlKXxNoiLFiamylZi1E6iIZGV3x5ylouv-YEBN49U64");
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
                error++;
                return null;
            }
            catch (XmlException ex)
            {
                Console.WriteLine(ex.Message);
                error++;
                ex.GetType();
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
        static string[] GetImageInfoFromKonachan(string hash, WebProxy proxy)
        {
            WebClient Client;
            Client = new WebClient();
            if (USE_PROXY)
            {
                Client.Proxy = proxy;
            }
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
                error++;
                return null;
            }
            catch (XmlException ex)
            {
                Console.WriteLine(ex.Message);
                error++;
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
        static string[] GetImageInfoFromYandere(string hash, WebProxy proxy)
        {
            WebClient Client;
            Client = new WebClient();
            if (USE_PROXY)
            {
                Client.Proxy = proxy;
            }
            string strURL = String.Format("https://yande.re/post.xml?tags=md5:{0}", hash);
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
                error++;
                return null;
            }
            catch (XmlException ex)
            {
                Console.WriteLine(ex.Message);
                error++;
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
        static string[] GetImageInfoFromGelbooru(string hash, WebProxy proxy)
        {
            WebClient Client;
            Client = new WebClient();
            if (USE_PROXY)
            {
                Client.Proxy = proxy;
            }
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
                error++;
                return null;
            }
            catch (XmlException ex)
            {
                Console.WriteLine(ex.Message);
                error++;
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
#region Sankaku
        static string[] GetTagsFromSankaku(string md5, string BaseURL, string UserAgent, WebProxy proxy, string RawCookies, out int SankakuPostId)
        {
            SankakuPostId = -1;
            //string BaseURL = "https://chan.sankakucomplex.com/";
            //string ua = "Mozilla / 5.0(Windows NT 6.2; WOW64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 34.0.1847.116 Safari / 537.36";
            //WebProxy proxy = new WebProxy("128.199.50.53", 8888);
            //proxy.Credentials = new NetworkCredential("maksim", "48sf54ro");
            //sankaku_cookies = GetSankakuCookies(BaseURL + "user/authenticate", proxy, ua);
            try
            {
                string text = DownloadStringFromSankaku(BaseURL + "?tags=md5:" + md5, BaseURL, RawCookies, proxy, UserAgent);
                List<int> posts = ParseHTML_sankaku(text);
                string tags;
                if (posts.Count > 0)
                {
                    string strURL = BaseURL + "post/show/" + posts[0].ToString();
                    string post = DownloadStringFromSankaku(strURL, BaseURL, RawCookies, proxy, UserAgent);
                    Regex rx = new Regex("<input id=post_old_tags name=\"post\\[old_tags\\]\" type=hidden value=\"(.+)\">");
                    Match match = rx.Match(post);
                    if (match.Success)
                    {
                        tags = match.Value.Substring(("<input id=post_old_tags name=\"post\\[old_tags\\]\" type=hidden value=\"").Length);
                        tags = tags.Substring(0, tags.Length - 2);
                        SankakuPostId = posts[0];
                        return tags.Split(' ');
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                error++;
                Thread.Sleep(120000);
                return null;
            }
            return null;
        }
        static string[] GetTagsFromSankakuByAngle(string md5, string BaseURL, string UserAgent, WebProxy proxy, out int SankakuPostId)
        {
            SankakuPostId = -1;
            var parser = new HtmlParser();
            try
            {
                string page = DownloadString("https://chan.sankakucomplex.com/?tags=md5:" + md5, BaseURL, proxy, UserAgent);
                var search = parser.ParseDocument(page);
                var elem = search.QuerySelector("span.thumb");
                if (elem != null)
                {


                    string post_id = elem.Id.Replace("p", String.Empty);
                    string strURL = BaseURL + "post/show/" + post_id;
                    string post = DownloadString(strURL, BaseURL, proxy, UserAgent);
                    var document = parser.ParseDocument(post);
                    string tags = document.Title.Replace(" | Sankaku Channel", String.Empty);
                    string[] arr = tags.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < arr.Length; i++)
                    {
                        arr[i] = arr[i].Replace(' ', '_');
                        //Console.WriteLine(arr[i]);
                    }
                    return arr;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                error++;
                Thread.Sleep(120000);
                return null;
            }
        }
        static string[] GetTagsFromSankakuByAPI(string md5, string BaseURL, string UserAgent, WebProxy proxy, out int SankakuPostId)
        {
            SankakuPostId = -1;
            try
            {
                string text = DownloadStringFromSankaku(BaseURL + "?tags=md5:" + md5, null, null, proxy, UserAgent);
                JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions();
                jsonSerializerOptions.IgnoreNullValues = true;
                SankakuJson[] sankakuJson = JsonSerializer.Deserialize<SankakuJson[]>(text, jsonSerializerOptions);

                List<string> tags = new List<string>();
                if (sankakuJson.Length > 0)
                {
                    SankakuPostId = sankakuJson[0].id;
                    foreach(tag t in sankakuJson[0].tags)
                    {
                        tags.Add(t.name);
                    }
                }
                return tags.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                error++;
                Thread.Sleep(120000);
                return null;
            }
        }
        static string DownloadString(string Url, string Referer, WebProxy Proxy, string UserAgent)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("user-agent", UserAgent);
                client.Headers.Add("referer", Referer);
                client.Proxy = Proxy;
                return client.DownloadString(Url);
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
            if (RawCookies != null)
            {
                downloadRequest.Headers.Add(HttpRequestHeader.Cookie, RawCookies);
            }
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
        public int recommended_posts { get; set; }
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
}
