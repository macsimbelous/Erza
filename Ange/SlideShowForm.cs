using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ErzaLib;

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
        public void LoadImage(int Index)
        {
            string ext = Path.GetExtension(this.Result[Index].FilePath).ToLower();
            if (ext == ".gif")
            {
                LoadImagelAsync(Index);
            }
            else
            {
                this.pictureBox1.ImageLocation = this.Result[Index].FilePath;
            }
        }
        public async void LoadImagelAsync(int Index)
        {
            await Task.Run(() =>
            {
                this.pictureBox1.ImageLocation = this.Result[Index].FilePath;
            });
        }
        private void SlideShowForm_Load(object sender, EventArgs e)
        {
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            LoadImage(this.Index);
            //this.pictureBox1.ImageLocation = this.Result[this.Index].FilePath;
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
                    LoadImage(this.Index);
                }
            }
            if (e.KeyCode == Keys.Left)
            {
                if (this.Index > 0)
                {
                    this.Index--;
                    //this.pictureBox1.ImageLocation = this.Result[this.Index].FilePath;
                    LoadImage(this.Index);
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
                LoadImage(this.Index);
            }
            else
            {
                if (this.Index < (this.Result.Count - 1))
                {
                    this.Index++;
                    //this.pictureBox1.ImageLocation = this.Result[this.Index].FilePath;
                    LoadImage(this.Index);
                }
                else
                {
                    this.Index = 0;
                    //this.pictureBox1.ImageLocation = this.Result[this.Index].FilePath;
                    LoadImage(this.Index);
                }
            }
        }
    }
}
