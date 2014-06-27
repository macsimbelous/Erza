/* Copyright © Macsim Belous 2012 */
/* This file is part of Rena.

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
using System.Security.Cryptography;
using System.IO;
using System.Threading;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace Rena
{
    public partial class Form1 : Form
    {
        enum HashType { MD5, SHA1, SHA256 };
        bool abort = false;
        Thread thread;
        SynchronizationContext synchronizationContext;
        List<String> files;
        TaskbarManager instanceTaskBar = TaskbarManager.Instance;
        public Form1()
        {
            InitializeComponent();
        }
        private void LongRunningTask(object o)
        {
            switch ((HashType)o)
            {
                case HashType.MD5:
                    md5_enc(files);
                    break;
                case HashType.SHA1:
                    sha1_enc(files);
                    break;
                case HashType.SHA256:
                    sha256_enc(files);
                    break;
            }
            synchronizationContext.Post(EndProgress, true);
        }
        private void RefreshProgress(object progress) // это для вызова  через Пост/Сенд
        {
            progressBar1.Value = (int)progress;
            instanceTaskBar.SetProgressValue((int)progress, files.Count);
        }
        private void EndProgress(object status)
        {
            this.start_button.Enabled = true;
            this.stop_button.Enabled = false;
            instanceTaskBar.SetOverlayIcon(Rena.Properties.Resources.Icon1, "Готово");
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }
        //Пишем сообщение в консоль
        public void ConsoleMsg(object strMsg)
        {
            listBox1.Items.Add((string)strMsg);
            listBox1.Refresh();
            listBox1.SetSelected(listBox1.Items.Count - 1, true);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < 1)
            {
                ConsoleMsg("Неуказан каталог.");
                return;
            }
            if ('\\' == textBox1.Text[textBox1.Text.Length - 1])
            {
                textBox1.Text = textBox1.Text.Substring(0, textBox1.Text.Length - 1);
            }
            files = new List<string>();
            //List<String> files;
            if (this.checkBox_subdir.Checked == true) 
            {
                String[] temp = System.IO.Directory.GetFiles(textBox1.Text, "*.*", SearchOption.AllDirectories);
                for (int index = 0; index < temp.Length; index++)
                {
                    if (IsImageFile(temp[index]))
                    {
                        files.Add(temp[index]);
                    }
                }
            }
            else
            {
                //files = new List<string>();
                string[] names = System.IO.Directory.GetFiles(textBox1.Text, "*.*", SearchOption.TopDirectoryOnly);
                for (int index = 0; index < names.Length; index++)
                {
                    if (IsImageFile(names[index]))
                    {
                        files.Add(names[index]);
                    }
                }
            }
            this.progressBar1.Maximum = files.Count;
            this.progressBar1.Minimum = 0;
            this.progressBar1.Step = 1;
            this.progressBar1.Value = 0;
            this.abort = false;
            this.start_button.Enabled = false;
            this.stop_button.Enabled = true;
            instanceTaskBar.SetOverlayIcon(null, "");
            if (this.radioButton_SHA1.Checked == true)
            {
                synchronizationContext = SynchronizationContext.Current;
                thread = new Thread(LongRunningTask);
                thread.Start(HashType.SHA1);
                return;
            }
            if (this.radioButton_MD5.Checked == true)
            {
                synchronizationContext = SynchronizationContext.Current;
                thread = new Thread(LongRunningTask);
                thread.Start(HashType.MD5);
                //md5_enc(files);
                return;
            }
            if (this.radioButton_SHA256.Checked == true)
            {
                synchronizationContext = SynchronizationContext.Current;
                thread = new Thread(LongRunningTask);
                thread.Start(HashType.SHA256);
                return;
            }
        }
        private void sha1_enc(List<String> fs)
        {
            SHA1 hash_enc = new SHA1Managed();
            int c = 0;
            int dup = 0;
            for (int i = 0; i < fs.Count; i++)
            {
                if (this.abort == true)
                {
                    break;
                }
                if (IsImageFile(fs[i]))
                {
                    FileStream fsData = new FileStream(fs[i], FileMode.Open, FileAccess.Read);
                    byte[] hash = hash_enc.ComputeHash(fsData);
                    fsData.Close();
                    string t = fs[i].Substring(0, fs[i].LastIndexOf('\\') + 1);
                    t = t + BitConverter.ToString(hash).Replace("-", string.Empty);
                    t = t + fs[i].Substring(fs[i].LastIndexOf('.'));
                    try
                    {
                        System.IO.File.Move(fs[i], t.ToLower());
                        
                        synchronizationContext.Post(ConsoleMsg, fs[i].Substring(fs[i].LastIndexOf('\\') + 1) + " -> " + t.Substring(fs[i].LastIndexOf('\\') + 1).ToLower());
                    }
                    catch (IOException e)
                    {
                        synchronizationContext.Post(ConsoleMsg, "ДУБЛИКАТ!!!" + fs[i]);
                        dup++;
                        //this.Refresh();
                    }
                    synchronizationContext.Post(RefreshProgress, i);
                    c++;
                }
            }
            synchronizationContext.Post(ConsoleMsg, "Файлов обработано: " + (c - dup).ToString() + " Дубликатов: " + dup.ToString());
            return;
        }
        private void md5_enc(List<String> fs)
        {
            MD5 hash_enc = MD5.Create();
            int c = 0;
            int dup = 0;
            for (int i = 0; i < fs.Count; i++)
            {
                if (this.abort == true)
                {
                    break;
                }
                if (IsImageFile(fs[i]))
                {
                    
                    FileStream fsData = new FileStream(fs[i], FileMode.Open, FileAccess.Read);
                    byte[] hash = hash_enc.ComputeHash(fsData);
                    fsData.Close();
                    string t = fs[i].Substring(0, fs[i].LastIndexOf('\\') + 1);
                    t = t + BitConverter.ToString(hash).Replace("-", string.Empty);
                    t = t + fs[i].Substring(fs[i].LastIndexOf('.'));
                    try
                    {
                        System.IO.File.Move(fs[i], t.ToLower());
                        synchronizationContext.Post(ConsoleMsg, fs[i].Substring(fs[i].LastIndexOf('\\') + 1) + " -> " + t.Substring(fs[i].LastIndexOf('\\') + 1).ToLower());
                    }
                    catch (IOException e)
                    {
                        synchronizationContext.Post(ConsoleMsg, "ДУБЛИКАТ!!! " + fs[i]);
                        dup++;
                        //this.Refresh();
                    }
                    synchronizationContext.Post(RefreshProgress, i+1);
                    c++;
                }
            }
            synchronizationContext.Post(ConsoleMsg, "Файлов обработано: " + (c-dup).ToString() + " Дубликатов: " + dup.ToString());
            return;
        }
        private void sha256_enc(List<String> fs)
        {
            SHA256 hash_enc = new SHA256Managed();
            int c = 0;
            int dup = 0;
            for (int i = 0; i < fs.Count; i++)
            {
                if (this.abort == true)
                {
                    break;
                }
                if (IsImageFile(fs[i]))
                {
                    
                    FileStream fsData = new FileStream(fs[i], FileMode.Open, FileAccess.Read);
                    byte[] hash = hash_enc.ComputeHash(fsData);
                    fsData.Close();
                    string t = fs[i].Substring(0, fs[i].LastIndexOf('\\') + 1);
                    t = t + BitConverter.ToString(hash).Replace("-", string.Empty);
                    t = t + fs[i].Substring(fs[i].LastIndexOf('.'));
                    try
                    {
                        System.IO.File.Move(fs[i], t.ToLower());
                        
                        synchronizationContext.Post(ConsoleMsg, fs[i].Substring(fs[i].LastIndexOf('\\') + 1) + " -> " + t.Substring(fs[i].LastIndexOf('\\') + 1).ToLower());
                    }
                    catch (IOException e)
                    {
                        synchronizationContext.Post(ConsoleMsg, "ДУБЛИКАТ!!!" + fs[i]);
                        dup++;
                        //this.Refresh();
                    }
                    synchronizationContext.Post(RefreshProgress, i);
                    c++;
                }
            }
            synchronizationContext.Post(ConsoleMsg, "Файлов обработано: " + (c - dup).ToString() + " Дубликатов: " + dup.ToString());
            return;
        }
        private bool IsImageFile(string s)
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
        private List<String> get_files(string NamePath)
        {
            //StringList Dirs = new StringList();
            List<String> Dirs = new List<string>();
            //folderBrowserDialog1.ShowDialog();
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
            return fs;
        }

        private void stop_button_Click(object sender, EventArgs e)
        {
            this.abort = true;
            this.start_button.Enabled = true;
            this.stop_button.Enabled = false;
        }
    }
}
