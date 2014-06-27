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
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;
using System.Globalization;
using System.Diagnostics;
using System.Data.SQLite;

namespace Moka
{
    public partial class Form1 : Form
    {
        //string sql_server = "Data Source=ASUKA;Initial Catalog=Moka;Integrated Security=True";
        //string database = Settings1.Default.database;
        public Form1()
        {
            InitializeComponent();
        }

        private void Find_button_Click(object sender, EventArgs e)
        {
            find_sqlite();
            return;
        }
        private void find_sqlite()
        {
            List<string> tags = new List<string>();
            List<long> tag_ids = new List<long>();
            List<long> image_ids = new List<long>();
            string[] t = this.Find_textBox.Text.Split(' ');
            for (int i = 0; i < t.Length; i++)
            {
                if (t[i].Length > 0)
                {
                    tags.Add(t[i]);
                }
            }
            if (tags.Count <= 0)
            {
                return;
            }
            //Получаем ИД тегов
            using (SQLiteConnection connection = new SQLiteConnection(Settings1.Default.ConnectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM hash_tags WHERE";
                for (int i = 0; i < tags.Count; i++)
                {
                    if (i != 0)
                    {
                        sql = sql + " OR";
                    }
                    sql = sql + " (tags like '% " + tags[i].Replace("'", "''") + " %' or tags like '" + tags[i].Replace("'", "''") + " %' or tags like '% " + tags[i].Replace("'", "''") + "' or tags = '" + tags[i].Replace("'", "''") + "')";
                }
                SQLiteCommand command = new SQLiteCommand(sql, connection);
                SQLiteDataReader reader = command.ExecuteReader();
                byte[] b = new byte[16];
                BindingList<CImage> imgs = new BindingList<CImage>();
                while (reader.Read())
                {
                    CImage img = new CImage();
                    if (Convert.IsDBNull((object)reader["file_name"]))
                    {
                        img.file = null;
                    }
                    else
                    {
                        img.file = (string)reader["file_name"];
                    }
                    img.id = (long)reader[0];
                    img.hash = (byte[])reader["hash"];
                    img.tags_string = (string)reader["tags"];
                    imgs.Add(img);
                }
                this.Result_listBox.DataSource = imgs;
                this.toolStripStatusLabel2.Text = "Изображений найдено: " + imgs.Count.ToString();
                reader.Close();
            }
        }

        public static bool IsImageFile(string s)
        {
            int t = s.LastIndexOf('.');
            if (t >= 0)
            {
                string ext = s.Substring(t).ToLower();
                switch (ext)
                {
                    case ".jpg":
                        return true;
                    //break;
                    case ".jpeg":
                        return true;
                    //break;
                    case ".png":
                        return true;
                    //break;
                    case ".bmp":
                        return true;
                    //break;
                    case ".gif":
                        return true;
                    //break;
                    case ".tif":
                        return true;
                    //break;
                    case ".tiff":
                        return true;
                    //break;

                }
            }
            return false;
        }
        public static byte[] md5_enc(string file)
        {
            MD5 hash_enc = MD5.Create();
            int c = 0;
            
                if (IsImageFile(file))
                {
                    c++;
                    FileStream fsData = new FileStream(file, FileMode.Open, FileAccess.Read);
                    byte[] hash = hash_enc.ComputeHash(fsData);
                    fsData.Close();
                    return hash;
                }
            return null;
        }
        public static List<String> get_files(string NamePath)
        {
            /*List<String> Dirs = new List<string>();
            List<String> fs = new List<string>();
            Dirs.Add(NamePath);
            for (int dirs_count = 0; dirs_count < Dirs.Count; dirs_count++)
            {
                string[] names = Directory.GetFiles(Dirs[dirs_count]);
                for (int index = 0; index < names.Length; index++)
                {
                    fs.Add(names[index]);
                }
                names = Directory.GetDirectories(Dirs[dirs_count]);
                for (int index = 0; index < names.Length; index++)
                {
                    Dirs.Add(names[index]);
                }
            }
            return fs;*/
            List<String> fs = new List<string>();
            fs.AddRange(Directory.GetFiles(NamePath, "*.*", SearchOption.AllDirectories));
            return fs;
        }
        private void add_tagget_file_button_Click(object sender, EventArgs e)
        {
            DateTime start;
            DateTime finish;
            AddTaggetFileForm form = new AddTaggetFileForm();
            if (form.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                AddImagesSQLiteProgressForm form_progress_sqlite = new AddImagesSQLiteProgressForm();
                form_progress_sqlite.imgs = new Queue<CImage>(form.img_list);
                start = DateTime.Now;
                if (form_progress_sqlite.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    finish = DateTime.Now;
                    this.toolStripStatusLabel2.Text = "Добавлено " + form.img_list.Count.ToString() + " изображений за " + (finish - start).TotalSeconds.ToString() + " секунд";
                }
                else
                {
                    this.toolStripStatusLabel2.Text = "Операция прервана!";
                }
                return;
            }
        }
        
        private void SlideShow_button_Click(object sender, EventArgs e)
        {
            if (this.random_checkBox.Checked)
            {
                Random random = new Random();
                int n = ((BindingList<CImage>)this.Result_listBox.DataSource).Count;
                while (n > 1)
                {
                    n--;
                    int k = random.Next(n + 1);
                    CImage value = ((BindingList<CImage>)this.Result_listBox.DataSource)[k];
                    ((BindingList<CImage>)this.Result_listBox.DataSource)[k] = ((BindingList<CImage>)this.Result_listBox.DataSource)[n];
                    ((BindingList<CImage>)this.Result_listBox.DataSource)[n] = value;
                }
            }
            using (TextWriter file = new StreamWriter(".\\" + "sharli" + ".txt"))
            {
                for (int i2 = 0; i2 < ((BindingList<CImage>)this.Result_listBox.DataSource).Count; i2++)
                {
                    if (((BindingList<CImage>)this.Result_listBox.DataSource)[i2].file != String.Empty)
                    {
                        file.WriteLine(((BindingList<CImage>)this.Result_listBox.DataSource)[i2].file);
                    }
                }
            }
            Process myProcess = new Process();
            myProcess.StartInfo.FileName = "sharli.exe";
            myProcess.StartInfo.Arguments = ".\\" + "sharli" + ".txt";
            myProcess.EnableRaisingEvents = true;
            myProcess.Start();
            myProcess.WaitForExit();
            System.IO.File.Delete(".\\" + "sharli" + ".txt");
        }

        private void Result_listBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            try
            {

                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo(((CImage)this.Result_listBox.SelectedItem).file);
                startInfo.UseShellExecute = true;
                process.StartInfo = startInfo;
                process.Start();
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                process.Close();
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //string s = Settings1.Default.database.Substring(13);
            //s = s.Substring(0, s.Length - 1);
            //System.IO.File.Copy(s, Settings1.Default.backup_db, true);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ConfigForm form = new ConfigForm();
            form.ShowDialog(this);
        }

        private void import_hash_button_Click(object sender, EventArgs e)
        {
            ImportHashForm form = new ImportHashForm();
            if (form.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                this.toolStripStatusLabel2.Text = "Изображений добавлено: " + form.img_list.Count.ToString();
            }
            else
            {
                this.toolStripStatusLabel2.Text = "Операция прервана!";
            }
        }
    }
    public class CImage
    {
        public long id;
        public byte[] hash;
        public string file;
        public List<string> tags = new List<string>();
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
            if (this.file != null)
            {
                return file.Substring(file.LastIndexOf('\\') + 1);
            }
            else
            {
                return "No File!";
            }
        }
    }
    
        
}
