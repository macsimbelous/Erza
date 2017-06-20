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
        public Form1 main_form;
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
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
                case Keys.Right:
                    if (this.Index < (this.Result.Count - 1))
                    {
                        this.Index++;
                        if (this.pictureBox1.Image != null) { this.pictureBox1.Image.Dispose(); }
                        this.pictureBox1.Image = Image.FromFile(this.Result[this.Index].FilePath);
                    }
                    break;
                case Keys.Left:
                    if (this.Index > 0)
                    {
                        this.Index--;
                        if (this.pictureBox1.Image != null) { this.pictureBox1.Image.Dispose(); }
                        this.pictureBox1.Image = Image.FromFile(this.Result[this.Index].FilePath);
                    }
                    break;
                case Keys.Delete:
                    main_form.DeleteImage(this.Index);
                    if (this.Result.Count > 0)
                    {
                        if (this.Index >= this.Result.Count)
                        {
                            this.Index = this.Result.Count - 1;
                        }
                        if (this.pictureBox1.Image != null) { this.pictureBox1.Image.Dispose(); }
                        this.pictureBox1.Image = Image.FromFile(this.Result[this.Index].FilePath);
                    }
                    break;
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
