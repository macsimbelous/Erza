using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using ErzaLib;

namespace Ange
{
    public partial class EditTagsForm : Form
    {
        public SQLiteConnection Connection;
        public ImageInfo EditImage;
        private bool ChangeTags = false;
        public EditTagsForm()
        {
            InitializeComponent();
        }

        private void EditTagsForm_Load(object sender, EventArgs e)
        {
            this.pictureBox1.ImageLocation = EditImage.FilePath;
            //sadsadsa
            List<string> tags = ErzaDB.GetTagsByImageID(EditImage.ImageID, Connection);
            tags.Sort();
            this.ImageTags_listBox.BeginUpdate();
            this.ImageTags_listBox.Items.AddRange(tags.ToArray());
            this.ImageTags_listBox.EndUpdate();
            //ajsdkajs
            List<string> all_tags = ErzaDB.GetAllTags(Connection);
            all_tags.Sort();
            this.AllTags_listBox.BeginUpdate();
            this.AllTags_listBox.Items.AddRange(all_tags.ToArray());
            this.AllTags_listBox.EndUpdate();
        }

        private void DeleteTag_button_Click(object sender, EventArgs e)
        {
            if (this.ImageTags_listBox.SelectedIndices.Count > 0)
            {
                this.ChangeTags = true;
                this.ImageTags_listBox.Items.RemoveAt(this.ImageTags_listBox.SelectedIndex);
            }
        }

        private void AddTag_button_Click(object sender, EventArgs e)
        {
            if (this.AllTags_listBox.SelectedIndices.Count > 0)
            {
                this.ChangeTags = true;
                this.ImageTags_listBox.Items.Add(this.AllTags_listBox.Items[this.AllTags_listBox.SelectedIndex]);
            }
        }

        private void EditTagsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.ChangeTags)
            {
                DialogResult result = MessageBox.Show("Сохранить изменения?", "Сохранение", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                switch (result)
                {
                    case DialogResult.Yes:
                        SaveTags();
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                    case DialogResult.No:
                        return;
                }
            }
        }
        private void SaveTags()
        {
            List<long> tag_ids = new List<long>();
            SQLiteTransaction transact = Connection.BeginTransaction();
            foreach (string tag in this.ImageTags_listBox.Items)
            {
                long t = ErzaDB.GetTagID(tag, Connection);
                if (t >= 0)
                {
                    tag_ids.Add(t);
                }
                else
                {
                    ErzaDB.AddTag(tag, Connection);
                    tag_ids.Add(ErzaDB.GetTagID(tag, Connection));
                }
            }
            tag_ids = tag_ids.Except(ErzaDB.GetTagIDsFromImageTags(EditImage.ImageID, Connection)).ToList();
            if (tag_ids.Count > 0)
            {
                ErzaDB.AddImageTags(EditImage.ImageID, tag_ids, Connection);
            }
            transact.Commit();
        }
    }
}
