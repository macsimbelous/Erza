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
    public partial class SlideShowForm : Form
    {
        public List<ImageInfo> Result;
        public int Index = 0;
        public SlideShowForm()
        {
            InitializeComponent();
        }

        private void SlideShowForm_Load(object sender, EventArgs e)
        {
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.pictureBox1.ImageLocation = this.Result[this.Index].FilePath;
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
                    this.pictureBox1.ImageLocation = this.Result[this.Index].FilePath;
                }
            }
            if (e.KeyCode == Keys.Left)
            {
                if (this.Index > 0)
                {
                    this.Index--;
                    this.pictureBox1.ImageLocation = this.Result[this.Index].FilePath;
                }
            }
            this.timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.Index < (this.Result.Count - 1))
            {
                this.Index++;
                this.pictureBox1.ImageLocation = this.Result[this.Index].FilePath;
            }
            else
            {
                this.Index = 0;
                this.pictureBox1.ImageLocation = this.Result[this.Index].FilePath;
            }
        }
    }
}
