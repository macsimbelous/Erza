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
    public partial class PHashOptionsForm : Form
    {
        public double similar = 0.9;
        public PHashOptionsForm()
        {
            InitializeComponent();
        }

        private void cancel_button_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ok_button_Click(object sender, EventArgs e)
        {
            similar = (double)numericUpDown1.Value / 100;
            Properties.Settings.Default.similar = (int)numericUpDown1.Value;
            Properties.Settings.Default.Save();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void PHashOptionsForm_Load(object sender, EventArgs e)
        {
            numericUpDown1.Value = Properties.Settings.Default.similar;
        }
    }
}
