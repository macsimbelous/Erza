using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Threading;
using System.Data.SQLite;
using AngleSharp.Html.Parser;
using ErzaLib;
using AngleSharp.Dom;

namespace Maple
{
    class Program
    {
        static string BaseURL;
        static int TimeOut;
        static int TimeOutError;
        static string ConnectionString;
        static string UserAgent;
        static int LimitErrors;
        static bool UseProxy;
        static string ProxyAddress;
        static int ProxyPort;
        static string ProxyLogin;
        static string ProxyPassword;
        static int StartPage = 1;
        static int MaxPage = 0;
        static void Main(string[] args)
        {
            LoadSettings();
            WebProxy proxy = new WebProxy("138.197.190.23", 3128);
            proxy.Credentials = new NetworkCredential("maksim", "48sf54ro");
            string current_path = Directory.GetCurrentDirectory();
            StringBuilder tags_builder = new StringBuilder();
            for (int i = 0; i < args.Length; i++)
            {
                if (i == 0)
                {
                    tags_builder.Append(WebUtility.UrlEncode(args[i]));
                }
                else
                {
                    tags_builder.Append("+");
                    tags_builder.Append(WebUtility.UrlEncode(args[i]));
                }
            }
            string tags = tags_builder.ToString();
            Console.WriteLine($"Импортируем тег {tags} с санкаки");
            List<CacheItem> post_ids = GetImageInfoFromSankaku(tags, proxy);
            Console.Write("Сохраняю список в кэш...");
            Console.WriteLine("Готово");
            Console.WriteLine(post_ids[post_ids.Count-1].PostID);
        }
        static List<CacheItem> GetImageInfoFromSankaku(string Tags, WebProxy Proxy)
        {
            List<CacheItem> imgs = new List<CacheItem>();
            int pagen = 1;
            var parser = new HtmlParser();
            string prefix = String.Empty;
            while (true)
            {
                List<CacheItem> ids = new List<CacheItem>();
                string url = String.Format("{0}?tags={1}&page={2}", Program.BaseURL, Tags, pagen);
                Console.WriteLine($"Запрашиваем {url}");
                string page = null;
                try
                {
                    page = DownloadString(url, BaseURL, Proxy, UserAgent);
                }
                catch (WebException we)
                {
                    Console.WriteLine("Ошибка: " + we.Message);
                    if (we.Response != null)
                    {
                        HttpWebResponse errorResponse = we.Response as HttpWebResponse;
                        if (errorResponse.StatusCode == HttpStatusCode.NotFound)
                        {
                            break;
                        }
                    }
                }
                var search = parser.ParseDocument(page);
                foreach (IElement link_element in search.QuerySelectorAll("span.thumb"))
                {
                    CacheItem item = new CacheItem();
                    item.PostID = Int64.Parse(link_element.Id.Replace("p", String.Empty));
                    ids.Add(item);
                }
                if(ids.Count < 20)
                {
                    imgs.AddRange(ids);
                    break;
                }
                imgs.AddRange(ids);
                pagen++;
                Thread.Sleep(3000);
            }
            return imgs;
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
        static void LoadSettings()
        {
            //Параметры по умолчанию
            Program.BaseURL = "https://chan.sankakucomplex.com/";
            Program.TimeOut = 5 * 1000;
            Program.TimeOutError = (5 * 60) * 1000;
            Program.ConnectionString = @"data source=C:\utils\data\erza.sqlite";
            Program.UserAgent = "Mozilla / 5.0(Windows NT 6.2; WOW64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 34.0.1847.116 Safari / 537.36";
            Program.LimitErrors = 2;
            //Program.SankakuLogin = null;
            //Program.SankakuPassword = null;
            Program.UseProxy = false;
            Program.ProxyAddress = null;
            Program.ProxyPort = 3128;
            Program.ProxyLogin = null;
            Program.ProxyPassword = null;

            Console.WriteLine("Конфигурационный файл не найден!\nЗагружены настройки по умолчанью.");
        }
    }
    public class CacheItem
    {
        public long PostID;
        public string Referer;
        public string Tag;
    }
}
