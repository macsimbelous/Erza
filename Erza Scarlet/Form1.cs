/* Copyright © Macsim Belous 2012 */
/* This file is part of Erza Scarlet.

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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Net;
using System.Xml;
using System.Globalization;
using System.Web.Script.Serialization;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Security.Cryptography;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.WindowsAPICodePack.Taskbar;
using System.Text.RegularExpressions;
using ErzaLib;

namespace Erza_Scarlet
{
    public partial class Form1 : Form
    {
        SynchronizationContext synchronizationContext;
        TaskbarManager instanceTaskBar = TaskbarManager.Instance;
        Thread thread;
        bool end = true;
        int sankaku_count = 0;
        int danbooru_count = 0;
        int gelbooru_count = 0;
        int konachan_count = 0;
        int yande_re_count = 0;
        int all_sites_unical_count = 0;
        int count_complit = 0;
        int count_deleted = 0;
        int count_error = 0;
        int count_files = 0;
        int count_skip = 0;
        public Form1()
        {
            InitializeComponent();
        }
        private void LongRunningTask(object o)
        {
            Grabber grab = new Grabber();
            grab.StatusCallBack = new Grabber.StatusCallBackT(StatusCallback);
            grab.UserAgent = this.UserAgent_textBox.Text;
            grab.LoginForDanbooru = this.login_textBox.Text;
            grab.ApiKeyForDanbooru = this.password_textBox.Text;
            grab.UseDanbooru = this.danbooru_checkBox.Checked;
            grab.UseGelbooru = this.gelbooru_checkBox.Checked;
            grab.UseKonachan = this.konachan_checkBox.Checked;
            grab.UseSankaku = this.sankakucomplex_checkBox.Checked;
            grab.UseYandere = this.yande_re_checkBox.Checked;
            grab.Tags = new List<string>(this.tags_textBox.Text.Split(' '));
            List<ImageInfo> il;
            try
            {
                il = grab.Grab();
            }
            catch (GrabberException ex)
            {
                synchronizationContext.Post(StatusRefresh, "ИСКЛЮЧЕНИЕ: " + ex.Message);
                synchronizationContext.Post(EndProgress, true);
                return;
            }
            #region SQLite
            if (this.add_info_db_checkBox.Checked)
            {
                synchronizationContext.Post(StatusRefresh, "Добавляем хэши в базу данных SQLite");
                
                ImagesDB idb = new ImagesDB(this.connection_string_db_textBox.Text);
                idb.ProgressCallBack = new ImagesDB.ProgressCallBackT(ProgressSQLiteCallBack);
                DateTime start = DateTime.Now;
                idb.AddImages(il);
                DateTime finish = DateTime.Now;
                synchronizationContext.Post(StatusRefresh, "Хэшей добавлено: " + il.Count.ToString() + " за: " + (finish - start).TotalSeconds.ToString("0.00") + " секунд (" + (il.Count / (finish - start).TotalSeconds) + " в секунду)");
            }
            #endregion
            #region Download
            if (this.downloading_checkBox.Checked)
            {
                synchronizationContext.Post(StatusRefresh, "НАЧИНАЕТСЯ ЗАГРУЗКА!");
                if (this.create_sub_dir_checkBox.Checked) 
                {
                    download(il, this.download_dir_textBox.Text + @"\" + grab.Tags[0], grab.sankaku_cookies);
                }
                else
                {
                    download(il, this.download_dir_textBox.Text, grab.sankaku_cookies);
                }
                //download(il, this.download_dir_textBox.Text + @"\" + grab.Tags[0], grab.sankaku_cookies);
            }
            #endregion
            synchronizationContext.Post(EndProgress, true);
        }
        private void ProgressSQLiteCallBack(string hash, int Count, int Total)
        {
            //Console.Write("Обрабатываю хэш {0} ({1}/{2})\r", hash, Count, Total);
            ProgressInfo pi = new ProgressInfo();
            pi.maximum = Total;
            pi.value = Count;
            synchronizationContext.Post(RefreshFileProgress, pi);
        }
        private void StatusCallback(ImageBoard CurrentSite, int CountImages, int TotalImages, int UnicalImagesForAllBoards, string StatusString)
        {
            switch (CurrentSite)
            {
                case ImageBoard.Danbooru:
                    this.danbooru_count = CountImages;
                    break;
                case ImageBoard.Gelbooru:
                    this.gelbooru_count = CountImages;
                    break;
                case ImageBoard.Konachan:
                    this.konachan_count = CountImages;
                    break;
                case ImageBoard.Sankaku:
                    this.sankaku_count = CountImages;
                    break;
                case ImageBoard.Yandere:
                    this.yande_re_count = CountImages;
                    break;
                default:
                    break;
            }
            if (UnicalImagesForAllBoards >= 0)
            {
                this.all_sites_unical_count = UnicalImagesForAllBoards;
            }
            synchronizationContext.Post(StatusRefresh, StatusString);
        }
        private void StatusRefresh(object o)
        {
            RefreshStatistic(null);
            this.toolStripStatusLabel1.Text = (string)o;
            this.toolStripStatusLabel1.ToolTipText = (string)o;
        }
        private void RefreshAllProgress(object progress)
        {
            ProgressInfo pi = (ProgressInfo)progress;
            all_progressBar.Maximum = pi.maximum;
            all_progressBar.Value = pi.value;
            this.all_progres_label.Text = "Все: " + pi.value.ToString() + "\\" + pi.maximum.ToString();
            instanceTaskBar.SetProgressValue(pi.value, pi.maximum);
        }
        private void RefreshFileProgress(object progress)
        {
            ProgressInfo pi = (ProgressInfo)progress;
            this.file_progressBar.Maximum = pi.maximum;
            this.file_progressBar.Value = pi.value;
            this.file_progres_label.Text = "Фаил: " + pi.value.ToString("#,###,###") + "\\" + pi.maximum.ToString("#,###,###") + " Байт; " + pi.opt_info;
            //instanceTaskBar.SetProgressValue((int)progress, files.Count);
        }
        private void RefreshStatistic(object o)
        {
            this.stat_sites_label.Text = String.Format("Сайты: Yande_re {0} Konachan {1} Danbooru {2} Sankaku {3} Gelbooru {4} Всего {5} Уникальных {6}", this.yande_re_count, this.konachan_count, this.danbooru_count, this.sankaku_count, this.gelbooru_count, (this.yande_re_count + this.konachan_count + this.danbooru_count + this.sankaku_count + this.gelbooru_count), this.all_sites_unical_count);
            this.stat_files_label.Text = String.Format("Файлы: Скачано {0} Скачано ранее {4} Удалено ранее {1} Ошибочно {2} Всего {3}", count_complit, count_deleted, count_error, count_files, count_skip);
            //this.stat_sites_label.Refresh();
        }
        private void EndProgress(object status)
        {
            this.start_button.Enabled = true;
            this.stop_button.Enabled = false;
            this.end = true;
            enable_settings();
            //synchronizationContext.Post(StatusRefresh, "Обработка всех файлов завершена!");
            instanceTaskBar.SetOverlayIcon(Erza_Scarlet.Properties.Resources.ErzaComplit, "Готово");
        }
        byte[] HexStringToBytes(string hexString)
        {
            if (hexString == null)
            {
                throw new ArgumentNullException("hexString");
            }

            if ((hexString.Length & 1) != 0)
            {
                throw new ArgumentOutOfRangeException("hexString", hexString, "hexString must contain an even number of characters.");
            }

            byte[] result = new byte[hexString.Length / 2];

            for (int i = 0; i < hexString.Length; i += 2)
            {
                result[i / 2] = byte.Parse(hexString.Substring(i, 2), NumberStyles.HexNumber);
            }

            return result;
        }
        #region DownloadFile
        int download(List<ImageInfo> list, string dir, CookieCollection cookies)
        {
            ImagesDB idb = new ImagesDB(this.connection_string_db_textBox.Text);
            ProgressInfo pi = new ProgressInfo();
            pi.maximum = list.Count;
            Directory.CreateDirectory(dir);
            for (int i = 0; i < list.Count; i++)
            {
                this.count_files++;
                synchronizationContext.Post(RefreshStatistic, null);
                if (this.add_info_db_checkBox.Checked)
                {
                    ImageInfo img = idb.ExistImage(list[i].hash);
                    if (img != null)
                    {
                        if (img.is_deleted)
                        {
                            synchronizationContext.Post(StatusRefresh, "Этот фаил уже был удалён!");
                            count_deleted++;
                            pi.value = i + 1;
                            synchronizationContext.Post(RefreshAllProgress, pi);
                            //cache.DeleteItem(list[i].hash);
                            continue;
                        }
                        if (img.file != null)
                        {
                            synchronizationContext.Post(StatusRefresh, "Этот фаил уже был ранее скачан.");
                            Console.WriteLine(img.file);
                            count_skip++;
                            pi.value = i + 1;
                            synchronizationContext.Post(RefreshAllProgress, pi);
                            continue;
                        }
                    }
                }
                if (DownloadImage(list[i], dir, cookies) == 0) { count_complit++; }
                else
                {
                    count_error++;
                }
                pi.value = i+1;
                synchronizationContext.Post(RefreshAllProgress, pi);
            }
            synchronizationContext.Post(StatusRefresh, String.Format("Успешно скачано: {0} Скачано ренее: {1} Удалено ранее: {2} Ошибочно: {3} Всего: {4}", count_complit, count_skip, count_deleted, count_error, list.Count));
            return 0;
        }
        int DownloadImage(ImageInfo img, string dir, CookieCollection cookies)
        {
            int cnt;
            if ((int)this.dovnload_povtor_numericUpDown.Value < 1)
            {
                cnt = 1;
            }
            else
            {
                cnt = (int)this.dovnload_povtor_numericUpDown.Value;
            }
            for (int index = 0; index < cnt; index++)
            {
                DateTime start1 = DateTime.Now;
                foreach (string url in img.urls)
                {
                    string extension = Path.GetExtension(url);
                    DateTime start = DateTime.Now;
                    if (extension == ".jpeg")
                    {
                        if (DownloadFile(url, dir + "\\" + img.GetHashString() + ".jpg", GetReferer(url, img)) == 0)
                        {
                            MyWait(start, 2500);
                            return 0;
                        }
                    }
                    else
                    {
                        if (DownloadFile(url, dir + "\\" + img.GetHashString() + extension, GetReferer(url, img)) == 0)
                        {
                            MyWait(start, 2500);
                            return 0;
                        }
                    }
                }
                if (img.sankaku_post_id > 0)
                {
                    DateTime start = DateTime.Now;
                    if (DownloadImageFromSankaku(img, dir, cookies))
                    {
                        MyWait(start, 5000);
                        return 0;
                    }
                    MyWait(start, 5000);
                }
                MyWait(start1, 2500);
            }
            return 1;
        }
        string GetReferer(string url, ImageInfo img)
        {
            if (url.LastIndexOf("http://chan.sankakustatic.com/data/") >= 0)
            {
                Uri uri = new Uri("http://chan.sankakucomplex.com/post/show/" + img.sankaku_post_id.ToString());
                return uri.AbsoluteUri;
            }
            if (url.LastIndexOf("gelbooru.com/") >= 0)
            {
                Uri uri = new Uri("http://gelbooru.com/index.php?page=post&s=view&id=" + img.gelbooru_post_id.ToString());
                return uri.AbsoluteUri;
            }
            if (url.LastIndexOf("http://konachan.com/") >= 0)
            {
                Uri uri = new Uri("http://konachan.com/post/show/" + img.konachan_post_id.ToString() + "/" + img.GetStringOfTags().Replace(' ', '-'));
                return uri.AbsoluteUri;
            }
            if (url.LastIndexOf("http://sonohara.donmai.us/") >= 0)
            {
                Uri uri = new Uri("http://danbooru.donmai.us/post/show/" + img.danbooru_post_id.ToString() + "/" + img.GetStringOfTags().Replace(' ', '-'));
                return uri.AbsoluteUri;
            }
            if (url.LastIndexOf("https://yande.re/") >= 0)
            {
                Uri uri = new Uri("https://yande.re/post/show/" + img.yandere_post_id.ToString() + "/" + img.GetStringOfTags().Replace(' ', '-'));
                return uri.AbsoluteUri;
            }
            return String.Empty;
        }
        long DownloadFile(string url, string filename, string referer)
        {
            synchronizationContext.Post(StatusRefresh, String.Format("Начинаем закачку {0}.", url));
            FileInfo fi = new FileInfo(filename);
            HttpWebRequest httpWRQ = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            WebResponse wrp = null;
            Stream rStream = null;
            try
            {
                httpWRQ.Referer = referer;
                httpWRQ.UserAgent = this.UserAgent_textBox.Text;
                httpWRQ.Timeout = 60 * 1000;
                wrp = httpWRQ.GetResponse();
                if (fi.Exists)
                {
                    if (wrp.ContentLength == fi.Length)
                    {
                        synchronizationContext.Post(StatusRefresh, "Уже скачан.");
                        //wrp.Close();
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
                    ProgressInfo pi = new ProgressInfo();
                    pi.maximum = (int)wrp.ContentLength;
                    DateTime start = DateTime.Now;
                    while ((bytesRead = rStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fs.Write(buffer, 0, bytesRead);
                        cnt += bytesRead;
                        DateTime pred = DateTime.Now;
                        pi.value = (int)cnt;
                        pi.opt_info = "Скорость: " + ((cnt / (pred - start).TotalSeconds) / 1000).ToString("0.00") + " Килобайт.";
                        synchronizationContext.Post(RefreshFileProgress, pi);
                        //Console.Write("\rСкачано " + cnt.ToString("#,###,###") + " из " + wrp.ContentLength.ToString("#,###,###") + " байт Скорость: " + ((cnt / (pred - start).TotalSeconds) / 1024).ToString("0.00") + " Килобайт в секунду.");
                    }
                    //DateTime finish = DateTime.Now;
                    //Console.WriteLine("Средняя скорость загрузки составила {0} байт в секунду.", ((cnt / (finish - start).TotalSeconds) / 1024).ToString("0.00"));
                }
                if (cnt < wrp.ContentLength)
                {
                    synchronizationContext.Post(StatusRefresh, "Обрыв! Закачка не завершена!");
                    //rStream.Close();
                    //wrp.Close();
                    return 1;
                }
                else
                {
                    synchronizationContext.Post(StatusRefresh, "Закачка завершена.");
                    //rStream.Close();
                    //wrp.Close();
                    return 0;
                }
            }
            catch (WebException ex)
            {
                synchronizationContext.Post(StatusRefresh, ex.Message);
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
        private bool DownloadImageFromSankaku(ImageInfo img, string dir, CookieCollection cookies)
        {
            string post = GetPostPage(img.sankaku_post_id, cookies);
            if (post == null) { return false; }
            string url = GetOriginalUrlFromPostPage(post);
            if (url == null)
            {
                synchronizationContext.Post(StatusRefresh, "URL Картинки не получен!");
                return false;
            }
            string filename = GetFileName(img, dir, url);

            synchronizationContext.Post(StatusRefresh, String.Format("Начинаем закачку {0}.", url));
            FileInfo fi = new FileInfo(filename);
            HttpWebRequest httpWRQ = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            WebResponse wrp = null;
            Stream rStream = null;
            try
            {
                httpWRQ.Referer = "http://chan.sankakucomplex.com/post/show/" + img.sankaku_post_id.ToString();
                httpWRQ.UserAgent = this.UserAgent_textBox.Text;
                httpWRQ.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                httpWRQ.Headers.Add("Accept-Encoding: identity");
                httpWRQ.CookieContainer = new CookieContainer();
                httpWRQ.CookieContainer.Add(cookies);
                httpWRQ.Timeout = 60 * 1000;
                wrp = httpWRQ.GetResponse();
                if (fi.Exists)
                {
                    if (wrp.ContentLength == fi.Length)
                    {
                        synchronizationContext.Post(StatusRefresh, "Уже скачан.");
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
                    ProgressInfo pi = new ProgressInfo();
                    pi.maximum = (int)wrp.ContentLength;
                    DateTime start = DateTime.Now;
                    while ((bytesRead = rStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fs.Write(buffer, 0, bytesRead);
                        cnt += bytesRead;
                        DateTime pred = DateTime.Now;
                        pi.value = (int)cnt;
                        pi.opt_info = "Скорость: " + ((cnt / (pred - start).TotalSeconds) / 1000).ToString("0.00") + " Килобайт.";
                        synchronizationContext.Post(RefreshFileProgress, pi);
                    }
                }
                if (cnt < wrp.ContentLength)
                {
                    synchronizationContext.Post(StatusRefresh, "Обрыв! Закачка не завершена!");
                    //rStream.Close();
                    //wrp.Close();
                    return false;
                }
                else
                {
                    synchronizationContext.Post(StatusRefresh, "Закачка завершена.");
                    //rStream.Close();
                    //wrp.Close();
                    return true;
                }
            }
            catch (WebException ex)
            {
                synchronizationContext.Post(StatusRefresh, ex.Message);
                return false;
            }
            catch (IOException ex)
            {
                synchronizationContext.Post(StatusRefresh, ex.Message);
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
        private string GetPostPage(int npost, CookieCollection cookies)
        {
            string strURL = "http://chan.sankakucomplex.com/post/show/" + npost.ToString();
            synchronizationContext.Post(StatusRefresh, "Загружаем и парсим пост: " + strURL);
            //WebClient Client = new WebClient();
            //Uri uri = new Uri(strURL);
            //Client.Headers.Add("user-agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
            while (true)
            {
                try
                {
                    return DownloadStringFromSankaku(strURL, null, cookies);
                    //return Client.DownloadString(uri);
                }
                catch (WebException we)
                {
                    synchronizationContext.Post(StatusRefresh, we.Message);
                    //log.Debug(we.Message);
                    Thread.Sleep(10000);
                    return null;
                }
            }
        }
        private string GetOriginalUrlFromPostPage(string post)
        {
            string file_url = "<li>Original: <a href=\"";
            Regex rx = new Regex(file_url + "(?:(?:ht|f)tps?://)?(?:[\\-\\w]+:[\\-\\w]+@)?(?:[0-9a-z][\\-0-9a-z]*[0-9a-z]\\.)+[a-z]{2,6}(?::\\d{1,5})?(?:[?/\\\\#][?!^$.(){}:|=[\\]+\\-/\\\\*;&~#@,%\\wА-Яа-я]*)?", RegexOptions.Compiled);
            try
            {
                Match match = rx.Match(post);
                if (match.Success)
                {
                    string url = match.Value.Substring(file_url.Length);
                    return url;
                }
                else
                {
                    Regex rx_swf = new Regex("<p><a href=\"" + @"(?<protocol>http(s)?)://(?<server>([A-Za-z0-9-]+\.)*(?<basedomain>[A-Za-z0-9-]+\.[A-Za-z0-9]+))+((:)?(?<port>[0-9]+)?(/?)(?<path>(?<dir>[A-Za-z0-9\._\-/]+)(/){0,1}[A-Za-z0-9.-/_]*)){0,1}" + "\" >Save this flash \\(right click and save\\)</a></p>", RegexOptions.Compiled);
                    Match match_swf = rx_swf.Match(post);
                    if (match_swf.Success)
                    {
                        string url = match_swf.Value.Substring(12).Replace("\" >Save this flash (right click and save)</a></p>", String.Empty);
                        return url;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (ArgumentNullException ex)
            {
                synchronizationContext.Post(StatusRefresh, ex.ToString());
                return null;
            }
        }
        private string GetFileName(ImageInfo img, string dir, string url)
        {
            int temp = url.IndexOf('?');
            if (temp >= 0)
            {
                url = url.Substring(0, temp);
            }
            string extension = Path.GetExtension(url);
            if (extension == ".jpeg")
            {
                return dir + "\\" + img.GetHashString() + ".jpg";
            }
            else
            {
                return dir + "\\" + img.GetHashString() + extension;
            }
        }
        private string DownloadStringFromSankaku(string url, string referer, CookieCollection cookies)
        {
            HttpWebRequest downloadRequest = (HttpWebRequest)WebRequest.Create(url);
            downloadRequest.UserAgent = this.UserAgent_textBox.Text;
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
        #endregion
        private static bool ValidationCallback(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        private void MyWait(DateTime start, int delay)
        {
            int current = (int)((DateTime.Now - start).TotalMilliseconds);
            if (current < delay)
            {
                //Console.WriteLine("WAIT {0}", delay - current);
                Thread.Sleep(delay - current);
                return;
            }
            else
            {
                return;
            }
        }

        private void start_button_Click(object sender, EventArgs e)
        {
            List<string> tags = new List<string>();
            if (this.tags_textBox.Text.Length <= 0)
            {
                MessageBox.Show("Не заданы теги!");
                return;
            }
            else
            {
                string[] temp_tags = this.tags_textBox.Text.Split(' ');
                foreach (string s in temp_tags)
                {
                    if (s.Length > 0)
                    {
                        tags.Add(s);
                    }
                }
                if (tags.Count <= 0)
                {
                    MessageBox.Show("Не заданы теги!");
                    return;
                }
            }
            if (this.downloading_checkBox.Checked)
            {
                if (this.download_dir_textBox.Text.Length <= 0)
                {
                    MessageBox.Show("Не выбран каталог для загрузки!");
                    return;
                }
            }
            if (this.add_info_db_checkBox.Checked)
            {
                if (this.add_info_db_checkBox.Text.Length <= 0)
                {
                    MessageBox.Show("Не указана строка подключения!");
                    return;
                }
            }
            this.end = false;
            this.start_button.Enabled = false;
            this.stop_button.Enabled = true;
            disable_settings();
            //instanceTaskBar.SetOverlayIcon(Erza_Scarlet.Properties.Resources.Clear, String.Empty);
            this.file_progressBar.Minimum = 0;
            this.file_progressBar.Value = 0;
            this.file_progressBar.Maximum = 100;
            this.file_progres_label.Text = "Фаил";
            this.all_progressBar.Minimum = 0;
            this.all_progressBar.Value = 0;
            this.all_progressBar.Maximum = 100;
            this.all_progres_label.Text = "Всего";
            this.sankaku_count = 0;
            this.danbooru_count = 0;
            this.gelbooru_count = 0;
            this.konachan_count = 0;
            this.yande_re_count = 0;
            this.all_sites_unical_count = 0;
            this.count_complit = 0;
            this.count_deleted = 0;
            this.count_error = 0;
            this.count_files = 0;
            synchronizationContext = SynchronizationContext.Current;
            thread = new Thread(LongRunningTask);
            thread.Start(tags);
            return;
        }

        private void stop_button_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Прервать операцию?", "Запрос!", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                try
                {
                    thread.Abort();
                }
                catch (Exception ex)
                {
                    this.toolStripStatusLabel1.Text = ex.Message;
                }
                thread.Join();
                this.start_button.Enabled = true;
                this.stop_button.Enabled = false;
                this.end = true;
                this.toolStripStatusLabel1.Text = "Операция прервана!";
                enable_settings();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Erza_Scarlet.Properties.Settings.Default.Reload();
            //База данных
            this.add_info_db_checkBox.Checked = Erza_Scarlet.Properties.Settings.Default.add_info_db;
            this.connection_string_db_textBox.Text = Erza_Scarlet.Properties.Settings.Default.connection_string;
            //Скачивание
            this.downloading_checkBox.Checked = Erza_Scarlet.Properties.Settings.Default.download;
            this.download_dir_textBox.Text = Erza_Scarlet.Properties.Settings.Default.download_dir;
            this.dovnload_povtor_numericUpDown.Value = Erza_Scarlet.Properties.Settings.Default.download_povtor;
            this.create_sub_dir_checkBox.Checked = Erza_Scarlet.Properties.Settings.Default.create_sub_dir;
            this.UserAgent_textBox.Text = Erza_Scarlet.Properties.Settings.Default.UserAgent;
            //Сайты
            this.sankakucomplex_checkBox.Checked = Erza_Scarlet.Properties.Settings.Default.sankaku;
            this.konachan_checkBox.Checked = Erza_Scarlet.Properties.Settings.Default.konachan;
            this.danbooru_checkBox.Checked = Erza_Scarlet.Properties.Settings.Default.danbooru;
            this.yande_re_checkBox.Checked = Erza_Scarlet.Properties.Settings.Default.yande_re;
            this.gelbooru_checkBox.Checked = Erza_Scarlet.Properties.Settings.Default.gelbooru;
            this.login_textBox.Text = Erza_Scarlet.Properties.Settings.Default.LoginForDanbooru;
            this.password_textBox.Text = Erza_Scarlet.Properties.Settings.Default.ApiKeyForDanbooru;
            this.stop_button.Enabled = false;
        }
        private void save_settings()
        {
            //База данных
            Erza_Scarlet.Properties.Settings.Default.add_info_db = this.add_info_db_checkBox.Checked;
            Erza_Scarlet.Properties.Settings.Default.connection_string = this.connection_string_db_textBox.Text;
            //Скачивание
            Erza_Scarlet.Properties.Settings.Default.download = this.downloading_checkBox.Checked;
            Erza_Scarlet.Properties.Settings.Default.download_dir = this.download_dir_textBox.Text;
            Erza_Scarlet.Properties.Settings.Default.download_povtor = this.dovnload_povtor_numericUpDown.Value;
            Erza_Scarlet.Properties.Settings.Default.create_sub_dir = this.create_sub_dir_checkBox.Checked;
            Erza_Scarlet.Properties.Settings.Default.UserAgent = this.UserAgent_textBox.Text;
            //Сайты
            Erza_Scarlet.Properties.Settings.Default.sankaku = this.sankakucomplex_checkBox.Checked;
            Erza_Scarlet.Properties.Settings.Default.konachan = this.konachan_checkBox.Checked;
            Erza_Scarlet.Properties.Settings.Default.danbooru = this.danbooru_checkBox.Checked;
            Erza_Scarlet.Properties.Settings.Default.yande_re = this.yande_re_checkBox.Checked;
            Erza_Scarlet.Properties.Settings.Default.gelbooru = this.gelbooru_checkBox.Checked;
            Erza_Scarlet.Properties.Settings.Default.LoginForDanbooru = this.login_textBox.Text;
            Erza_Scarlet.Properties.Settings.Default.ApiKeyForDanbooru = this.password_textBox.Text;
            Erza_Scarlet.Properties.Settings.Default.Save();
        }
        private void disable_settings()
        {
            this.tags_textBox.Enabled = false;
            this.select_db_button.Enabled = false;
            this.connection_string_db_textBox.Enabled = false;
            this.select_dwn_dir_button.Enabled = false;
            this.download_dir_textBox.Enabled = false;
            this.danbooru_checkBox.Enabled = false;
            this.yande_re_checkBox.Enabled = false;
            this.konachan_checkBox.Enabled = false;
            this.sankakucomplex_checkBox.Enabled = false;
            this.gelbooru_checkBox.Enabled = false;
            this.add_info_db_checkBox.Enabled = false;
            this.create_sub_dir_checkBox.Enabled = false;
            this.downloading_checkBox.Enabled = false;
            this.password_textBox.Enabled = false;
            this.login_textBox.Enabled = false;
            this.dovnload_povtor_numericUpDown.Enabled = false;
            this.UserAgent_textBox.Enabled = false;
        }
        private void enable_settings()
        {
            this.tags_textBox.Enabled = true;
            this.select_db_button.Enabled = true;
            this.connection_string_db_textBox.Enabled = true;
            this.select_dwn_dir_button.Enabled = true;
            this.download_dir_textBox.Enabled = true;
            this.danbooru_checkBox.Enabled = true;
            this.yande_re_checkBox.Enabled = true;
            this.konachan_checkBox.Enabled = true;
            this.sankakucomplex_checkBox.Enabled = true;
            this.gelbooru_checkBox.Enabled = true;
            this.add_info_db_checkBox.Enabled = true;
            this.create_sub_dir_checkBox.Enabled = true;
            this.downloading_checkBox.Enabled = true;
            this.password_textBox.Enabled = true;
            this.login_textBox.Enabled = true;
            this.dovnload_povtor_numericUpDown.Enabled = true;
            this.UserAgent_textBox.Enabled = true;
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            save_settings();
            if (end == true)
            {
                return;
            }
            else
            {
                if (MessageBox.Show("Прервать операцию?", "Запрос!", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    thread.Abort();
                    thread.Join();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void select_dwn_dir_button_Click(object sender, EventArgs e)
        {
            if (this.select_dwn_folder_dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.download_dir_textBox.Text = this.select_dwn_folder_dialog.SelectedPath;
            }
        }

        private void select_db_button_Click(object sender, EventArgs e)
        {
            if (this.select_db_dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.connection_string_db_textBox.Text = "data source=" + this.select_db_dialog.FileName;
            }
        }
    }
    public class ProgressInfo
    {
        public int value = 0;
        public int maximum = 100;
        public int start = 0;
        public string opt_info = String.Empty;
    }
    public class CImage
    {
        public bool is_new = true;
        public bool is_deleted = false;
        public long id;
        public int post_id;
        public byte[] hash;
        public string file = null;
        //public string file_url;
        public List<string> tags = new List<string>();
        public List<string> urls = new List<string>();
        public string hash_str;
        public string tags_string
        {
            get
            {
                string s = String.Empty;
                for (int i = 0; i < tags.Count; i++)
                {
                    if (i > 0)
                    {
                        s = s + " ";
                    }
                    s = s + tags[i];
                }
                return s;
            }
            set
            {
                string[] t = value.Split(' ');
                for (int i = 0; i < t.Length; i++)
                {
                    if (t[i].Length > 0)
                    {
                        tags.Add(t[i]);
                    }
                }
            }
        }
        public override string ToString()
        {
            if (this.file != String.Empty)
            {
                return file.Substring(file.LastIndexOf('\\') + 1);
            }
            else
            {
                return "No File!";
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
    internal class AcceptAllCertificatePolicy : ICertificatePolicy
    {
        public AcceptAllCertificatePolicy()
        {
        }

        public bool CheckValidationResult(ServicePoint sPoint, X509Certificate cert, WebRequest wRequest, int certProb)
        {
            // Always accept
            return true;
        }
    }
}
