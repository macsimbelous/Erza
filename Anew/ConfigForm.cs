using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Anew
{
    public partial class ConfigForm : Form
    {
        public bool all_new = true;
        public bool new_only = true;
        public bool non_tag = true;
        public string tag;
        public ConfigForm()
        {
            InitializeComponent();
            
        }

        private void Non_tag_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            if (this.Non_tag_checkBox.Checked)
            {
                this.tag_textBox.Enabled = false;
            }
            else
            {
                this.tag_textBox.Enabled = true;
            }
        }

        private void cancel_button_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void ok_button_Click(object sender, EventArgs e)
        {
            this.all_new = this.all_new_checkBox.Checked;
            this.tag = this.tag_textBox.Text;
            this.new_only = this.new_only_checkBox.Checked;
            this.non_tag = this.Non_tag_checkBox.Checked;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void all_new_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            if (cb.Checked)
            {
                this.tag_textBox.Enabled = false;
                this.new_only_checkBox.Enabled = false;
                this.Non_tag_checkBox.Enabled = false;
            }
            else
            {
                this.tag_textBox.Enabled = false;
                this.new_only_checkBox.Enabled = true;
                this.Non_tag_checkBox.Enabled = true;
                this.Non_tag_checkBox.Checked = true;
            }
        }

        private void ConfigForm_Load(object sender, EventArgs e)
        {
            this.Non_tag_checkBox.Checked = non_tag;
            this.new_only_checkBox.Checked = new_only;
            this.tag_textBox.Enabled = false;
            this.tag_textBox.Text = tag;
            this.all_new_checkBox.Checked = all_new;
        }
    }
}
