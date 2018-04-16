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

namespace Eris
{
    public partial class EditForm : Form
    {
        public long TagID;
        public string TagName;
        public string TagNameRus;
        public string TagDescription;
        public long TagType;
        public long TagCount;
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
            this.id_tag_textBox.Text = TagID.ToString();
            this.count_links_textBox.Text = TagCount.ToString();
            this.tag_textBox.Text = TagName;
            this.tag_rus_textBox.Text = TagNameRus;
            this.type_tag_comboBox.Text = TagType.ToString();
            this.description_tag_textBox.Text = TagDescription;

            List<TypeTag> types = new List<TypeTag>();
            types.Add(new TypeTag(0, "General"));
            types.Add(new TypeTag(1, "Artist"));
            types.Add(new TypeTag(3, "Copyright"));
            types.Add(new TypeTag(4, "Character"));
            types.Add(new TypeTag(5, "Circle"));
            types.Add(new TypeTag(6, "Faults"));
            types.Add(new TypeTag(8, "Medium"));
            types.Add(new TypeTag(9, "Meta"));
            types.Add(new TypeTag(2, "Studio"));
            this.type_tag_comboBox.DataSource = types;
            //this.type_tag_comboBox.DisplayMember = "Name";
            //this.type_tag_comboBox.ValueMember = "Name";
            foreach (TypeTag t in types)
            {
                if (t.Type == TagType)
                {
                    this.type_tag_comboBox.SelectedItem = t;
                }
            }
        }

        private void save_button_Click(object sender, EventArgs e)
        {
            long.TryParse(this.id_tag_textBox.Text, out this.TagID);
            long.TryParse(this.count_links_textBox.Text, out this.TagCount);
            this.TagName = this.tag_textBox.Text;
            this.TagNameRus = this.tag_rus_textBox.Text;
            //long.TryParse(this.type_tag_comboBox.Text, out this.TagType);
            TypeTag t = (TypeTag)this.type_tag_comboBox.SelectedItem;
            this.TagType = t.Type;
            this.TagDescription = this.description_tag_textBox.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
    public class TypeTag
    {
        public long Type;
        public string Name;
        public string NameRus;
        public TypeTag(long Type, string Name)
        {
            this.Type = Type;
            this.Name = Name;
        }
        public TypeTag(long Type, string Name, string NameRus)
        {
            this.Type = Type;
            this.Name = Name;
            this.NameRus = NameRus;
        }
        override public string ToString()
        {
            return this.Name;
        }
    }
}
