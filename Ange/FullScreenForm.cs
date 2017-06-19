using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ErzaLib;

namespace Ange
{
    public partial class FullScreenForm : Form
    {
        public List<ImageInfo> Result;
        public int Index = 0;
        public FullScreenForm()
        {
            InitializeComponent();
        }

        private void FullScreenForm_Load(object sender, EventArgs e)
        {
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            //this.pictureBox1.ImageLocation = this.Result[this.Index].FilePath;
            if (this.pictureBox1.Image != null) { this.pictureBox1.Image.Dispose(); }
            this.pictureBox1.Image = Image.FromFile(this.Result[this.Index].FilePath);
        }

        private void FullScreenForm_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            if(e.KeyCode == Keys.Right)
            {
                if(this.Index < (this.Result.Count - 1))
                {
                    this.Index++;
                    //this.pictureBox1.ImageLocation = this.Result[this.Index].FilePath;
                    if (this.pictureBox1.Image != null) { this.pictureBox1.Image.Dispose(); }
                    this.pictureBox1.Image = Image.FromFile(this.Result[this.Index].FilePath);
                }
                return;
            }
            if (e.KeyCode == Keys.Left)
            {
                if (this.Index > 0)
                {
                    this.Index--;
                    //this.pictureBox1.ImageLocation = this.Result[this.Index].FilePath;
                    if (this.pictureBox1.Image != null) { this.pictureBox1.Image.Dispose(); }
                    this.pictureBox1.Image = Image.FromFile(this.Result[this.Index].FilePath);
                }
                return;
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (this.pictureBox1.Image != null)
            {
                g.DrawString(String.Format("Ширина: {0}\nВысота: {1}", this.pictureBox1.Image.Width, this.pictureBox1.Image.Height),
                    new Font("Arial", 10), System.Drawing.Brushes.White, new Point(0, 0));
            }
        }
    }
}
