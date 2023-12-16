using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ErzaLib;
using WebP.Net;

namespace Ange
{
    public partial class SlideShowForm : Form
    {
        public List<ImageInfo> Result;
        public int Index = 0;
        public bool RandomShow = false;
        private Random rnd;
        public SlideShowForm()
        {
            InitializeComponent();
        }

        private void SlideShowForm_Load(object sender, EventArgs e)
        {
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            //this.pictureBox1.ImageLocation = this.Result[this.Index].FilePath;
            LoadImageAsync(this.Result[this.Index].FilePath);
            this.rnd = new Random();
            this.timer1.Enabled = true;
        }

        private void SlideShowForm_KeyDown(object sender, KeyEventArgs e)
        {
            this.timer1.Enabled = false;
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            if (e.KeyCode == Keys.Right)
            {
                if (this.Index < (this.Result.Count - 1))
                {
                    this.Index++;
                    //this.pictureBox1.ImageLocation = this.Result[this.Index].FilePath;
                    LoadImageAsync(this.Result[this.Index].FilePath);
                }
            }
            if (e.KeyCode == Keys.Left)
            {
                if (this.Index > 0)
                {
                    this.Index--;
                    //this.pictureBox1.ImageLocation = this.Result[this.Index].FilePath;
                    LoadImageAsync(this.Result[this.Index].FilePath);
                }
            }
            this.timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.RandomShow)
            {
                this.Index = rnd.Next(this.Result.Count);
                //this.pictureBox1.ImageLocation = this.Result[this.Index].FilePath;
                LoadImageAsync(this.Result[this.Index].FilePath);
            }
            else
            {
                if (this.Index < (this.Result.Count - 1))
                {
                    this.Index++;
                    //this.pictureBox1.ImageLocation = this.Result[this.Index].FilePath;
                    LoadImageAsync(this.Result[this.Index].FilePath);
                }
                else
                {
                    this.Index = 0;
                    //this.pictureBox1.ImageLocation = this.Result[this.Index].FilePath;
                    LoadImageAsync(this.Result[this.Index].FilePath);
                }
            }
        }
        private async void LoadImageAsync(string path)
        {
            if (Path.GetExtension(path).ToLower() == "gif")
            {
                await Task.Run(() => LoadImage(path));
            }
            else
            {
                LoadImage(path);
            }
        }
        private void LoadImage(string path)
        {
            try
            {
                if (Path.GetExtension(path).ToLower() == ".webp")
                {
                    using var webp = new WebPObject(File.ReadAllBytes(path));
                    this.pictureBox1.Image = webp.GetImage();
                }
                else
                {
                    this.pictureBox1.ImageLocation = path;
                }
                
            }
            catch (Exception e)
            {
                this.Text = e.Message;
            }
        }
    }
}
