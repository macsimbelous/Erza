/* Copyright © Macsim Belous 2012 */
/* This file is part of Erza.

    Foobar is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Foobar is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Foobar.  If not, see <https://www.gnu.org/licenses/>.*/
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
