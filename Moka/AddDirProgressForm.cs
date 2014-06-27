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
using System.Text.RegularExpressions;
using System.IO;

namespace Moka
{
    public partial class AddDirProgressForm : Form
    {
        bool abort = false;
        bool end = false;
        Thread thread;
        SynchronizationContext synchronizationContext;
        public BindingList<CImage> imgs;
        public List<String> files;
        public AddDirProgressForm()
        {
            InitializeComponent();

        }
        private void LongRunningTask()
        {
            for (int index = 0; index < files.Count; index++)
            {
                if (abort)
                {
                    return;
                }
                if (Moka.Form1.IsImageFile(files[index]))
                {
                    CImage img = new CImage();
                    img.file = files[index];
                    img.tags = get_tags_file(files[index]);
                    img.hash = Moka.Form1.md5_enc(files[index]);
                    imgs.Add(img);
                    ChangeProgress(index);
                }
            }
            synchronizationContext.Post(EndProgress, true);
        }
        private void RefreshProgress(object progress) // это для вызова  через Пост/Сенд
        {
            RefreshProgress((int)progress);
        }
        private void RefreshProgress(int progress) // это для вызова из потока ЮИ напрямую
        {
            progressBar1.Value = (int)progress;
            this.label1.Text = "Файлов обработано: " + ((int)progress).ToString();
        }
        private void ChangeProgress(int progress)
        {
            synchronizationContext.Post(RefreshProgress, progress);
        }
        private void EndProgress(object status)
        {
            EndProgress((bool)status);
        }
        private void EndProgress(bool status)
        {
            end = true;
            //this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
        private List<string> get_tags_file(string file)
        {
            if (file != String.Empty)
            {
                string t = file.Substring(file.LastIndexOf('\\') + 1);
                t = t.Substring(0, t.LastIndexOf('.'));
                List<string> tags = new List<string>();
                Regex kona_rx = new Regex("Konachan.com - [0-9]+", RegexOptions.Compiled);
                //Regex kona_rx2 = new Regex("Konachan.com - [0-9]+[ -]", RegexOptions.Compiled);
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

        private void AddDirProgressForm_Load(object sender, EventArgs e)
        {
            end = false;
            this.progressBar1.Maximum = files.Count;
            this.progressBar1.Minimum = 0;
            this.progressBar1.Step = 1;
            this.progressBar1.Value = 0;
            imgs = new BindingList<CImage>();
            synchronizationContext = SynchronizationContext.Current;
            thread = new Thread(LongRunningTask);
            thread.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //end_state = false;
            //thread.Abort();
            //this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void AddDirProgressForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (end)
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                return;
            }
            else
            {
                if (MessageBox.Show("Вы уверены что хотите прервать обработку файлов?", "Запрос подтверждения", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
                {
                    if (end)
                    {
                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                        return;
                    }
                    else
                    {
                        abort = true;
                        this.DialogResult = System.Windows.Forms.DialogResult.Cancel;

                        //this.Close();
                        return;
                    }
                }
                else
                {
                    if (end)
                    {
                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                        return;
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                }
            }
        }
    }
}
