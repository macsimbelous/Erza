using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ange
{
    public partial class FullScreenForm : Form
    {
        public string FilePath;
        public FullScreenForm()
        {
            InitializeComponent();
        }

        private void FullScreenForm_Load(object sender, EventArgs e)
        {
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.pictureBox1.ImageLocation = this.FilePath;
        }
    }
}
