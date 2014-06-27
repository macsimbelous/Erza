/* Copyright © Macsim Belous 2012 */
/* This file is part of Moka.

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
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Xml;
using System.Globalization;
using System.Web.Script.Serialization;

namespace Moka
{
    public partial class ImportHashForm : Form
    {
        //Копия тэгов
        string m_strTags = null;
        bool abort = true;
        Thread thread;
        SynchronizationContext synchronizationContext;
        public List<CImage> img_list;
        //int nPostsCount = 0;          //Счетчик постов для скачивания
        public ImportHashForm()
        {
            InitializeComponent();
        }
        private void LongRunningTask(object o)
        {
            List<string> tags = new List<string>();
            string[] t = m_strTags.Split(' ');
            for (int i = 0; i < t.Length; i++)
            {
                if (t[i].Length > 0)
                {
                    tags.Add(t[i]);
                }
            }
            if (tags.Count <= 0)
            {
                synchronizationContext.Post(ConsoleMsg, "Не заданы теги!");
                synchronizationContext.Post(AbortProgress, null);
                return;
            }
            img_list = new List<CImage>();
            for (int i = 0; i < tags.Count; i++)
            {
                if (this.sankaku_checkBox.Checked)
                {
                    synchronizationContext.Post(ConsoleMsg, "Импортируем тег " + tags[i] + " с санкаки");
                    List<CImage> temp = get_hash_sankaku(tags[i]);
                    if (temp == null)
                    {
                        return;
                    }
                    img_list.AddRange(temp);
                }
                if (this.danbooru_checkBox.Checked)
                {
                    synchronizationContext.Post(ConsoleMsg, "Импортируем тег " + tags[i] + " с данбуры");
                    List<CImage> temp = get_hash_danbooru(tags[i]);
                    if (temp == null)
                    {
                        return;
                    }
                    img_list.AddRange(temp);
                }
                if (this.gelbooru_checkBox.Checked)
                {
                    synchronizationContext.Post(ConsoleMsg, "Импортируем тег " + tags[i] + " с гелбуры");
                    List<CImage> temp = get_hash_gelbooru(tags[i]);
                    if (temp == null)
                    {
                        return;
                    }
                    img_list.AddRange(temp);
                }
                if (this.imouto_checkBox.Checked)
                {
                    synchronizationContext.Post(ConsoleMsg, "Импортируем тег " + tags[i] + " с сестрёнки");
                    List<CImage> temp = get_hash_imouto(tags[i]);
                    if (temp == null)
                    {
                        return;
                    }
                    img_list.AddRange(temp);
                }
                if (this.konachan_checkBox.Checked)
                {
                    synchronizationContext.Post(ConsoleMsg, "Импортируем тег " + tags[i] + " с коначан");
                    List<CImage> temp = get_hash_konachan(tags[i]);
                    if (temp == null)
                    {
                        return;
                    }
                    img_list.AddRange(temp);
                }
            }
            if (img_list.Count <= 0)
            {
                synchronizationContext.Post(AbortProgress, null);
                return;
            }

            synchronizationContext.Post(EndProgress, true);
        }
        private List<CImage> get_hash_danbooru(string tag)
        {
            //bool bFirstEntry = true;       //признак первой записи
            int nPostsCount = 0;          //Счетчик постов для скачивания
            int nPage = 0;                //Счетчик страниц
            WebClient Client;
            List<CImage> img_list = new List<CImage>();
            bool islogin = UserLoginPOST("http://danbooru.donmai.us/user/authenticate", "url=&user%5Bname%5D=macsimbelous&user%5Bpassword%5D=050782&commit=Login", "You are now logged in");
            if (!islogin)
            {
                synchronizationContext.Post(ConsoleMsg, "В авторизации отказано!");
                return img_list;
            }
            synchronizationContext.Post(ConsoleMsg, "Авторизация успешно завершена!");
            nPostsCount = posts_count("http://danbooru.donmai.us/post/index.xml?tags=%%tags%%&page=%%page%%&limit=100", tag);
            if (nPostsCount <= 0)
            {
                synchronizationContext.Post(ConsoleMsg, "Ничего ненайдено!");
                return img_list;
            }
            Client = new WebClient();
            //bFirstEntry = true;
            for (; ; )
            {
                if (abort == true)
                {
                    synchronizationContext.Post(AbortProgress, null);
                    return null;
                }
                //string xml;
                string strURL = GetQueryString("http://danbooru.donmai.us/post/index.xml?tags=%%tags%%&page=%%page%%&limit=100", tag, nPage, img_list.Count);
                synchronizationContext.Post(ConsoleMsg, "Загружаем и парсим фаил: " + strURL + " (" + img_list.Count + "/" + nPostsCount + ")");
                try
                {
                    Uri uri = new Uri(strURL);
                    string xml = Client.DownloadString(uri);
                    List<CImage> list = ParseXMLBooru(xml);
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
                    synchronizationContext.Post(ConsoleMsg, "Ошибка: " + we.Message);
                    if (nPage > 1000)
                    {
                        break;
                    }
                    //return;
                    continue;
                }
            }
            return img_list;
        }
        private List<CImage> get_hash_konachan(string tag)
        {
            //bool bFirstEntry = true;       //признак первой записи
            int nPostsCount = 0;          //Счетчик постов для скачивания
            int nPage = 0;                //Счетчик страниц
            WebClient Client;
            List<CImage> img_list = new List<CImage>();
            nPostsCount = posts_count("http://konachan.com/post/index.xml?tags=%%tags%%&page=%%page%%", tag);
            if (nPostsCount <= 0)
            {
                synchronizationContext.Post(ConsoleMsg, "Ничего ненайдено!");
                return img_list;
            }
            Client = new WebClient();
            //bFirstEntry = true;
            for (; ; )
            {
                if (abort == true)
                {
                    synchronizationContext.Post(AbortProgress, null);
                    return null;
                }
                //string xml;
                string strURL = GetQueryString("http://konachan.com/post/index.xml?tags=%%tags%%&page=%%page%%", tag, nPage, img_list.Count);
                synchronizationContext.Post(ConsoleMsg, "Загружаем и парсим фаил: " + strURL + " (" + img_list.Count + "/" + nPostsCount + ")");
                try
                {
                    Uri uri = new Uri(strURL);
                    string xml = Client.DownloadString(uri);
                    List<CImage> list = ParseXMLBooru(xml);
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
                    synchronizationContext.Post(ConsoleMsg, "Ошибка: " + we.Message);
                    //return;
                    continue;
                }
            }
            return img_list;
        }
        private List<CImage> get_hash_imouto(string tag)
        {
            //bool bFirstEntry = true;       //признак первой записи
            int nPostsCount = 0;          //Счетчик постов для скачивания
            int nPage = 0;                //Счетчик страниц
            WebClient Client;
            List<CImage> img_list = new List<CImage>();
            nPostsCount = posts_count("http://oreno.imouto.org/post/index.xml?tags=%%tags%%&page=%%page%%", tag);
            if (nPostsCount <= 0)
            {
                synchronizationContext.Post(ConsoleMsg, "Ничего ненайдено!");
                return img_list;
            }
            Client = new WebClient();
            //bFirstEntry = true;
            for (; ; )
            {
                if (abort == true)
                {
                    synchronizationContext.Post(AbortProgress, null);
                    return null;
                }
                //string xml;
                string strURL = GetQueryString("http://oreno.imouto.org/post/index.xml?tags=%%tags%%&page=%%page%%", tag, nPage, img_list.Count);
                synchronizationContext.Post(ConsoleMsg, "Загружаем и парсим фаил: " + strURL + " (" + img_list.Count + "/" + nPostsCount + ")");
                try
                {
                    Uri uri = new Uri(strURL);
                    string xml = Client.DownloadString(uri);
                    List<CImage> list = ParseXMLBooru(xml);
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
                    synchronizationContext.Post(ConsoleMsg, "Ошибка: " + we.Message);
                    //return;
                    continue;
                }
            }
            return img_list;
        }
        private List<CImage> get_hash_gelbooru(string tag)
        {
            //bool bFirstEntry = true;       //признак первой записи
            int nPostsCount = 0;          //Счетчик постов для скачивания
            int nPage = 0;                //Счетчик страниц
            WebClient Client;
            List<CImage> img_list = new List<CImage>();
            nPostsCount = posts_count("http://gelbooru.com/index.php?page=dapi&s=post&q=index&tags=%%tags%%&pid=%%pid%%", tag);
            if (nPostsCount <= 0)
            {
                synchronizationContext.Post(ConsoleMsg, "Ничего ненайдено!");
                return img_list;
            }
            Client = new WebClient();
            //bFirstEntry = true;
            for (; ; )
            {
                if (abort == true)
                {
                    synchronizationContext.Post(AbortProgress, null);
                    return null;
                }
                //string xml;
                string strURL = GetQueryString("http://gelbooru.com/index.php?page=dapi&s=post&q=index&tags=%%tags%%&pid=%%pid%%", tag, nPage, img_list.Count);
                synchronizationContext.Post(ConsoleMsg, "Загружаем и парсим фаил: " + strURL + " (" + img_list.Count + "/" + nPostsCount + ")");
                try
                {
                    Uri uri = new Uri(strURL);
                    string xml = Client.DownloadString(uri);
                    List<CImage> list = ParseXMLBooru(xml);
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
                    synchronizationContext.Post(ConsoleMsg, "Ошибка: " + we.Message);
                    //return;
                    continue;
                }
            }
            return img_list;
        }
        private List<CImage> get_hash_sankaku(string tag)
        {
            int nPage = 1;                //Счетчик страниц
            WebClient Client;
            List<CImage> img_list = new List<CImage>();
            Client = new WebClient();
            string BaseURL = "http://chan.sankakucomplex.com/post/index.json";
            for (; ; )
            {
                if (abort == true)
                {
                    synchronizationContext.Post(AbortProgress, null);
                    return null;
                }
                string strURL = BaseURL + "?tags=" + tag + "&page=" + nPage + "&limit=100";
                synchronizationContext.Post(ConsoleMsg, "Загружаем и парсим фаил: " + strURL + " (" + img_list.Count + "/ХЗ)");
                try
                {
                    Uri uri = new Uri(strURL);
                    string xml = Client.DownloadString(uri);
                    List<CImage> list = parse_json(xml);
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
                    synchronizationContext.Post(ConsoleMsg, "Ошибка: " + we.Message);
                    //return;
                    continue;
                }
            }
            return img_list;
        }
        private List<CImage> parse_json(string json)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<json_image> result = ser.Deserialize<List<json_image>>(json);
            List<CImage> temp = new List<CImage>();
            for (int i = 0; i < result.Count; i++)
            {
                CImage img = new CImage();
                img.hash = HexStringToBytes(result[i].md5);
                img.hash_str = result[i].md5;
                img.tags_string = result[i].tags;
                temp.Add(img);
            }
            return temp;
        }
        //Форимируем строку запроса для текущего поисковика
        private string GetQueryString(string strBaseUrl, string strTags, int nPage, int nPost)
        {
            string strURL = strBaseUrl;
            strURL = strURL.Replace("%%tags%%", strTags);
            strURL = strURL.Replace("%%pid%%", Convert.ToString(nPage));
            strURL = strURL.Replace("%%page%%", Convert.ToString(nPage + 1));
            //strURL = strURL + "&limit=100";
            return strURL;
        }
        private int posts_count(string url, string tag)
        {
            WebClient Client = new WebClient();
            Uri uri = new Uri(GetQueryString(url, tag, 0, 0));
            string xml = Client.DownloadString(uri);
            XmlDocument mXML = new XmlDocument();
            mXML.LoadXml(xml);
            XmlNodeList nodeList = mXML.GetElementsByTagName("posts");
            XmlNode node = nodeList.Item(0);
            //Определяем число постов
            int nLocalPostsCount = 0;
            for (int i = 0; i < node.Attributes.Count; i++)
            {
                if (node.Attributes[i].Name == "count") nLocalPostsCount = Convert.ToInt32(node.Attributes[i].Value);
            }
            return nLocalPostsCount;
        }
        //Регистрация методом POST (для данборы)
        private bool UserLoginPOST(string strUrl, string strParams, string strGoodResponse)
        {
            PostSubmitter post = new PostSubmitter();
            post.Url = strUrl;
            post.m_parameters = strParams;
            post.Type = PostSubmitter.PostTypeEnum.Post;
            string result = post.Post();

            if (result.Contains(strGoodResponse)) return true;
            return false;
        }
        private List<CImage> ParseXMLBooru(string strXML)
        {
            List<CImage> list = new List<CImage>();
            XmlDocument mXML = new XmlDocument();
            try
            {
                mXML.LoadXml(strXML);
            }
            catch (XmlException e)
            {
                synchronizationContext.Post(ConsoleMsg, e.Message);
                return list;
            }
            XmlNodeList nodeList = mXML.GetElementsByTagName("post");
            //Парсим посты
            for (int i = 0; i < nodeList.Count; i++)
            {
                CImage mImgDescriptor = new CImage();
                XmlNode node = nodeList.Item(i);
                for (int j = 0; j < node.Attributes.Count; j++)
                {
                    //Тэги
                    if (node.Attributes[j].Name == "tags")
                    {
                        mImgDescriptor.tags_string = node.Attributes[j].Value;
                        //mImgDescriptor.tags = new List<string>(mImgDescriptor.tags_string.Split(' ')); //Получаем массив тэгов
                    }
                    if (node.Attributes[j].Name == "md5")
                    {
                        mImgDescriptor.hash_str = node.Attributes[j].Value;
                        mImgDescriptor.hash = HexStringToBytes(node.Attributes[j].Value);
                    }
                }
                list.Add(mImgDescriptor); //Добавляем в список
            }
            return list;
        }
        private void Cancel_button_Click(object sender, EventArgs e)
        {
            //abort = true;
            this.Close();
        }

        private void EndProgress(object status)
        {
            //this.nPostsCount = 0;
            DateTime start;
            DateTime finish;
            AddImagesSQLiteProgressForm form_progress_sqlite = new AddImagesSQLiteProgressForm();
            form_progress_sqlite.imgs = new Queue<CImage>(img_list);
            start = DateTime.Now;
            if (form_progress_sqlite.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                finish = DateTime.Now;
                synchronizationContext.Post(ConsoleMsg, "Добавлено " + img_list.Count.ToString() + " изображений за " + (finish - start).TotalSeconds.ToString() + " секунд");
            }
            else
            {
                synchronizationContext.Post(ConsoleMsg, "Операция прервана!");
            }
            this.Get_button.Enabled = true;
            this.stop_button.Enabled = false;
            abort = true;
            return;
        }
        public void ConsoleMsg(object strMsg)
        {
            listBox1.Items.Add((string)strMsg);
            listBox1.Refresh();
            listBox1.SetSelected(listBox1.Items.Count - 1, true);
        }
        private void AbortProgress(object o)
        {
            this.Get_button.Enabled = true;
            this.stop_button.Enabled = false;
            ConsoleMsg("Операция прервана!");
            abort = true;
        }
        private void Get_button_Click(object sender, EventArgs e)
        {
            if (this.tags_textBox.Text.Length < 1)
            {
                ConsoleMsg("Неуказан тег.");
                return;
            }
            img_list = new List<CImage>();
            this.abort = false;
            this.Get_button.Enabled = false;
            this.stop_button.Enabled = true;
            m_strTags = this.tags_textBox.Text;
            synchronizationContext = SynchronizationContext.Current;
            thread = new Thread(LongRunningTask);
            thread.Start(null);
            return;
        }

        private void ImportHashForm_Load(object sender, EventArgs e)
        {
            this.danbooru_checkBox.Checked = Settings1.Default.danbooru;
            this.gelbooru_checkBox.Checked = Settings1.Default.gelbooru;
            this.imouto_checkBox.Checked = Settings1.Default.imouto;
            this.konachan_checkBox.Checked = Settings1.Default.konachan;
            this.sankaku_checkBox.Checked = Settings1.Default.sankaku;
        }

        private void stop_button_Click(object sender, EventArgs e)
        {
            abort = true;
        }
        private byte[] HexStringToBytes(string hexString)
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
        private void ImportHashForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (abort == true)
            {
                //thread.Abort();
                //this.Close();
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                return;
            }
            else
            {
                if (MessageBox.Show("Прервать операцию?", "Запрос", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    abort = true;
                    thread.Abort();
                    //this.Close();
                }
                else
                {
                    e.Cancel = true;
                }
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
}
