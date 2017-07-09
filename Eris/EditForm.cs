using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Eris
{
    public partial class EditForm : Form
    {
        public long TagID;
        public string TagName;
        public string TagLocation;
        public string TagDescription;
        public string TagType;
        public int TagCount;
        public EditForm()
        {
            InitializeComponent();
        }

        private void cancel_button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void EditForm_Load(object sender, EventArgs e)
        {
            this.TagID__textBox.Text = TagID.ToString();
        }
    }
}
