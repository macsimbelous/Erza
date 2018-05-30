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
using System.Data.SQLite;

namespace Ange
{
    public partial class AddTagForm : Form
    {
        public List<string> Tags;
        public string NewTag;
        public SQLiteConnection Erza;
        public AddTagForm()
        {
            InitializeComponent();
        }

        private void AddTagForm_Load(object sender, EventArgs e)
        {
            //Tags.Sort();
            //BindingList<string> temp = new BindingList<string>(Tags);
            //this.comboBox1.DataSource = temp;
            //this.comboBox1.DataSource = Tags;
            this.comboBox1.AutoCompleteMode = AutoCompleteMode.None;
        }

        private void OK_button_Click(object sender, EventArgs e)
        {
            this.NewTag = this.comboBox1.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Cancel_button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void comboBox1_TextUpdate(object sender, EventArgs e)
        {
            string ts = this.comboBox1.Text;
            List<string> temp;
            if (ts.Length < 3)
            {
                temp = ErzaDB.SearchTags(ts, true, true, Erza);
            }
            else
            {
                temp = ErzaDB.SearchTags(ts, false, true, Erza);
            }
            this.comboBox1.DataSource = temp;
            this.comboBox1.DroppedDown = true;
            this.comboBox1.Text = ts;
            this.comboBox1.SelectionStart = ts.Length;
            Cursor.Current = Cursors.Default;
        }
    }
}
