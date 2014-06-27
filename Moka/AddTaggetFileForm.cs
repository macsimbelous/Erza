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
using System.Text.RegularExpressions;
using System.IO;

namespace Moka
{
    public partial class AddTaggetFileForm : Form
    {
        public BindingList<CImage> img_list;
        public AddTaggetFileForm()
        {
            InitializeComponent();
        }

        private void add_file_button_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                CImage img = new CImage();
                img.file = this.openFileDialog1.FileName;
                img.tags = get_tags_file(this.openFileDialog1.FileName);
                img.hash = Moka.Form1.md5_enc(this.openFileDialog1.FileName);
                img_list.Add(img);
                this.label1.Text = "Файлы (" + img_list.Count.ToString() + ")";
            }
        }

        private void AddTaggetFileForm_Load(object sender, EventArgs e)
        {
            img_list = new BindingList<CImage>();
            this.files_listBox.DataSource = img_list;
        }
        private List<string> get_tags_file(string file)
        {
            if (file != String.Empty)
            {
                string t = file.Substring(file.LastIndexOf('\\') + 1);
                t = t.Substring(0, t.LastIndexOf('.'));
                List<string> tags = new List<string>();
                Regex kona_rx = new Regex("Konachan.com - [0-9]+", RegexOptions.Compiled);
                Regex moe_rx = new Regex("moe [0-9]+", RegexOptions.Compiled);
                Regex mjv_rx = new Regex("MJV-ART.ORG_-_[0-9]+[-][0-9]+[x][0-9]+[-]", RegexOptions.Compiled);
                Regex mjv_rx2 = new Regex("MJV-ART.ORG_-_[0-9]+[_][0-9]+[x][0-9]+[-]", RegexOptions.Compiled);
                if (kona_rx.IsMatch(t))
                {
                    t = kona_rx.Replace(t, String.Empty, 1);
                    string[] t2 = t.Split(' ');
                    for (int i = 0; i < t2.Length; i++)
                    {
                        if (t2[i].Length > 0)
                        {
                            if (t2[i] == "-")
                            {
                                continue;
                            }
                            string s = "sample_url=";
                            if (t2[i].Length >= s.Length)
                            {
                                if (s == t2[i].Substring(0, s.Length))
                                {
                                    continue;
                                }
                            }
                            tags.Add(t2[i]);
                        }
                    }
                    return tags;
                }
                if (moe_rx.IsMatch(t))
                {
                    t = moe_rx.Replace(t, String.Empty, 1);
                    string[] t2 = t.Split(' ');
                    for (int i = 0; i < t2.Length; i++)
                    {
                        if (t2[i].Length > 0)
                        {
                            tags.Add(t2[i]);
                        }
                    }
                    return tags;
                }
                if (mjv_rx.IsMatch(t))
                {
                    t = mjv_rx.Replace(t, String.Empty, 1);
                    t = t.Replace('+', '_');
                    string[] t2 = t.Split('-');
                    for (int i = 0; i < t2.Length; i++)
                    {
                        if (t2[i].Length > 0)
                        {
                            tags.Add(t2[i]);
                        }
                    }
                    return tags;
                }
                if (mjv_rx2.IsMatch(t))
                {
                    t = mjv_rx2.Replace(t, String.Empty, 1);
                    t = t.Replace('+', '_');
                    string[] t2 = t.Split('-');
                    for (int i = 0; i < t2.Length; i++)
                    {
                        if (t2[i].Length > 0)
                        {
                            tags.Add(t2[i]);
                        }
                    }
                    return tags;
                }
                return tags;
            }
            else
            {
                return new List<string>();
            }
        }

        private void del_file_button_Click(object sender, EventArgs e)
        {
            img_list.Remove((CImage)this.files_listBox.SelectedItem);
        }

        private void clear_button_Click(object sender, EventArgs e)
        {
            img_list.Clear();
        }

        private void Cancel_button_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void add_dir_button_Click(object sender, EventArgs e)
        {
            this.files_listBox.DataSource = null;
            if (this.folderBrowserDialog1.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                if (this.dir_checkBox.Checked == true)
                {
                    List<String> temp = Moka.Form1.get_files(this.folderBrowserDialog1.SelectedPath);
                    /*for (int index = 0; index < temp.Count; index++)
                    {
                        if (Moka.Form1.IsImageFile(temp[index]))
                        {
                            CImage img = new CImage();
                            img.file = temp[index];
                            img.tags = get_tags_file(temp[index]);
                            //img.hash = Moka.Form1.md5_enc(temp[index]);
                            img_list.Add(img);
                        }
                    }*/
                    AddDirProgressForm form = new AddDirProgressForm();
                    form.files = temp;
                    if (form.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                    {
                        for (int i = 0; i < form.imgs.Count; i++)
                        {
                            img_list.Add(form.imgs[i]);
                        }
                    }
                }
                else
                {
                    string[] names = System.IO.Directory.GetFiles(this.folderBrowserDialog1.SelectedPath);
                    /*for (int index = 0; index < names.Length; index++)
                    {
                        if (Moka.Form1.IsImageFile(names[index]))
                        {
                            CImage img = new CImage();
                            img.file = names[index];
                            img.tags = get_tags_file(names[index]);
                            //img.hash = Moka.Form1.md5_enc(names[index]);
                            img_list.Add(img);
                        }
                    }*/
                    AddDirProgressForm form = new AddDirProgressForm();
                    form.files = new List<string>(names);
                    if (form.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                    {
                        for (int i = 0; i < form.imgs.Count; i++)
                        {
                            img_list.Add(form.imgs[i]);
                        }
                    }
                }
            }
            this.files_listBox.DataSource = img_list;
            this.label1.Text = "Файлы (" + img_list.Count.ToString() + ")";
        }

        private void OK_button_Click(object sender, EventArgs e)
        {
            if (this.files_listBox.Items.Count <= 0)
            {
                MessageBox.Show("Добавь картинки!");
                return;
            }
            if (this.tags_textBox.Text != String.Empty)
            {
                List<string> tag_list = new List<string>();
                string[] t = this.tags_textBox.Text.Split(' ');
                for (int i = 0; i < t.Length; i++)
                {
                    if (t[i].Length > 0)
                    {
                        tag_list.Add(t[i]);
                    }
                }
                for (int i = 0; i < img_list.Count; i++)
                {
                    img_list[i].tags.AddRange(tag_list);
                }
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
