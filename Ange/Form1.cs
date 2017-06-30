using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Data.SQLite;
using System.IO;
using ErzaLib;

namespace Ange
{
    public partial class Form1 : Form
    {
        SQLiteConnection Previews;
        public SQLiteConnection Erza;
        List<ImageInfo> Result;
        SolidBrush brush;
        public Form1()
        {
            InitializeComponent();
        }

        private void listView1_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            ListViewItem item = new ListViewItem();
            item.Tag = GetPreview(Result[e.ItemIndex].Hash);
            item.Text = Result[e.ItemIndex].Hash;
            Result[e.ItemIndex].Tags = ErzaDB.GetTagsByImageID(Result[e.ItemIndex].ImageID, Erza);
            StringBuilder tag_string = new StringBuilder();
            for (int i = 0; i < Result[e.ItemIndex].Tags.Count; i++)
            {
                if (i > 0)
                {
                    tag_string.Append(' ');
                }
                tag_string.Append(Result[e.ItemIndex].Tags[i]);
            }
            item.ToolTipText = String.Format("{0}\n{1}x{2}\n{3}", Result[e.ItemIndex].Hash, Result[e.ItemIndex].Width, Result[e.ItemIndex].Height, tag_string.ToString());
            e.Item = item;
            //e.Item = new ListViewItem(e.ItemIndex.ToString(), 0);
        }
        private Image GetPreview(string hash)
        {
            using (SQLiteCommand command = new SQLiteCommand(this.Previews))
            {
                command.CommandText = "SELECT preview FROM previews WHERE hash = @hash;";
                command.Parameters.AddWithValue("hash", hash);
                byte[] tmp = (byte[])command.ExecuteScalar();
                if (tmp != null)
                {
                    using (MemoryStream stream = new MemoryStream(tmp))
                    {
                        return Image.FromStream(stream);
                    }
                }
                else
                {
                    return (Image)Properties.Resources.noimage;
                }
            }
        }
        private void search_button_Click(object sender, EventArgs e)
        {
            listView1.VirtualListSize = 0;
            if(this.textBox1.Text.Length == 0)
            {
                this.Result = ErzaDB.GetAllImages(this.Erza);
                if (this.Result.Count > 0)
                {
                    this.listView1.VirtualListSize = this.Result.Count;
                    this.listView1.EnsureVisible(0);
                }
                this.toolStripStatusLabel1.Text = "Изображений найдено: " + this.Result.Count.ToString();
                return;
            }
            if (this.tag_radioButton.Checked)
            {
                string[] temp = this.textBox1.Text.Split(' ');
                List<string> tags = new List<string>();
                foreach(string tag in temp)
                {
                    if (!String.IsNullOrEmpty(tag))
                    {
                        tags.Add(tag);
                    }
                }
                if (tags.Count > 1)
                {
                    if (this.search_condition_checkBox.Checked)
                    {

                        this.Result = ErzaDB.GetImagesByTags(new List<string>(tags), true, this.Erza);
                        this.listView1.VirtualListSize = this.Result.Count;
                    }
                    else
                    {
                        this.Result = ErzaDB.GetImagesByTags(new List<string>(tags), false, this.Erza);
                        this.listView1.VirtualListSize = this.Result.Count;
                    }
                }
                else
                {
                    if (tags.Count == 1)
                    {
                        this.Result = ErzaDB.GetImagesByTag(tags[0], this.Erza);
                    }
                    else
                    {
                        this.Result = ErzaDB.GetAllImages(this.Erza);
                    }
                    this.listView1.VirtualListSize = this.Result.Count;
                }
                if (this.Result.Count > 0)
                {
                    this.listView1.EnsureVisible(0);
                }
                this.toolStripStatusLabel1.Text = "Изображений найдено: " + this.Result.Count.ToString();
                return;
            }
            if (this.part_tag_radioButton.Checked)
            {
                this.Result = ErzaDB.GetImagesByPartTag(this.textBox1.Text, this.Erza);
                this.listView1.VirtualListSize = this.Result.Count;
                if (this.Result.Count > 0)
                {
                    this.listView1.EnsureVisible(0);
                }
                this.toolStripStatusLabel1.Text = "Изображений найдено: " + this.Result.Count.ToString();
                return;
            }
            if (this.md5_radioButton.Checked)
            {
                this.Result.Clear();
                this.Result.Add(ErzaDB.GetImageWithOutTags(this.textBox1.Text, this.Erza));
                this.listView1.VirtualListSize = this.Result.Count;
                if (this.Result.Count > 0)
                {
                    this.listView1.EnsureVisible(0);
                }
                this.toolStripStatusLabel1.Text = "Изображений найдено: " + this.Result.Count.ToString();
                return;
            }
        }

        private void listView1_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            if ((e.State & ListViewItemStates.Selected) != 0)
            {
                e.Graphics.FillRectangle(Brushes.Maroon, e.Bounds);
                e.DrawFocusRectangle();
            }
            else
            {
                e.Graphics.FillRectangle(this.brush, e.Bounds);
            }
            ListViewItem item = e.Item;
            Image img = (Image)item.Tag;
            float x = (e.Bounds.Width / 2f) - ((float)img.Width / 2f);
            float y = (e.Bounds.Height / 2f) - ((float)img.Height / 2f);
            e.Graphics.DrawImage(img, new RectangleF(e.Bounds.X + x, e.Bounds.Y + y, (float)img.Width, (float)img.Height));
            StringFormat drawFormat = new StringFormat();
            drawFormat.Alignment = StringAlignment.Center;
            e.Graphics.DrawString(e.Item.Text, new Font("Arial", 8f), new SolidBrush(Color.Black), e.Bounds.X + (e.Bounds.Width / 2f), e.Bounds.Y + 156f, drawFormat);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Erza = new SQLiteConnection("data source="+ Application.StartupPath + "\\Erza.sqlite");
            this.Erza.Open();
            this.Previews = new SQLiteConnection("data source=" + Application.StartupPath + "\\Previews.sqlite");
            this.Previews.Open();
            this.Result = new List<ImageInfo>();
            this.brush = new SolidBrush(Color.Orange);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Previews.Close();
            this.Erza.Close();
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.listView1.SelectedIndices.Count > 0)
            {
                int i = ((ListView)sender).SelectedIndices[0];
                //ImageInfo img = ErzaDB.GetImageWithOutTags(Result[i].Hash, Erza);
                FullScreenForm form = new FullScreenForm();
                form.Result = Result;
                form.Index = i;
                form.main_form = this;
                form.ShowDialog();
                this.listView1.Refresh();
                this.listView1.EnsureVisible(form.Index);
            }
        }

        private void slideshow_button_Click(object sender, EventArgs e)
        {
            SlideShowForm form = new SlideShowForm();
            form.Result = this.Result;
            form.Index = 0;
            if (MessageBox.Show("Выводить слайды в случайном порядке?", "Слайдшоу", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                form.RandomShow = true;
            }
            else
            {
                form.RandomShow = false;
            }
            form.ShowDialog();
        }

        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedIndices.Count > 0)
            {
                int i = this.listView1.SelectedIndices[0];
                //ImageInfo img = ErzaDB.GetImageWithOutTags(Result[i].Hash, Erza);
                FullScreenForm form = new FullScreenForm();
                form.Result = Result;
                form.Index = i;
                form.main_form = this;
                form.ShowDialog();
                this.listView1.Refresh();
                this.listView1.EnsureVisible(form.Index);
            }
        }

        private void slideshowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int i = this.listView1.SelectedIndices[0];
            SlideShowForm form = new SlideShowForm();
            form.Result = Result;
            form.Index = i;
            form.ShowDialog();
        }

        private void edittagsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedIndices.Count > 0)
            {
                EditTagsForm form = new EditTagsForm();
                form.Connection = Erza;
                int i = this.listView1.SelectedIndices[0];
                form.EditImage = this.Result[i];
                form.ShowDialog();
            }
        }

        private void copytowallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedIndices.Count > 0)
            {
                int i = this.listView1.SelectedIndices[0];
                string dest_path = "I:\\Wallpapers\\" + Path.GetFileName(this.Result[i].FilePath);
                try
                {
                    if (File.Exists(dest_path))
                    {
                        if (MessageBox.Show("Целевой фаил уже сушествует, перезаписать?", "Предупреждение!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                        {
                            File.Delete(dest_path);
                            File.Copy(this.Result[i].FilePath, dest_path);
                        }
                    }
                    else
                    {
                        File.Copy(this.Result[i].FilePath, dest_path);
                    }
                }
                catch (IOException ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedIndices.Count > 0)
            {
                if (MessageBox.Show("Удалить изображение " + this.Result[this.listView1.SelectedIndices[0]].FilePath + "?", "Предупреждение!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    DeleteImage(this.listView1.SelectedIndices[0]);
                }
            }
        }

        private void tag_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (this.tag_radioButton.Checked)
            {
                this.search_condition_checkBox.Enabled = true;
            }
            else
            {
                this.search_condition_checkBox.Enabled = false;
            }
        }

        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter && this.listView1.SelectedIndices.Count > 0)
            {
                int i = this.listView1.SelectedIndices[0];
                FullScreenForm form = new FullScreenForm();
                form.Result = Result;
                form.Index = i;
                form.main_form = this;
                form.ShowDialog();
                this.listView1.EnsureVisible(form.Index);
            }
        }

        private void copyhashToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int i = this.listView1.SelectedIndices[0];
            Clipboard.SetText(this.Result[i].Hash);
        }

        private void copytodirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedIndices.Count > 0)
            {
                if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    int i = this.listView1.SelectedIndices[0];
                    string dest_path = this.folderBrowserDialog1.SelectedPath + "\\" + Path.GetFileName(this.Result[i].FilePath);
                    try
                    {
                        if (File.Exists(dest_path))
                        {
                            if (MessageBox.Show("Целевой фаил уже сушествует, перезаписать?", "Предупреждение!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                            {
                                File.Delete(dest_path);
                                File.Copy(this.Result[i].FilePath, dest_path);
                            }
                        }
                        else
                        {
                            File.Copy(this.Result[i].FilePath, dest_path);
                        }
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        public void DeleteImage(int Index)
        {
            ErzaDB.DeleteImage(this.Result[Index].ImageID, Erza);
            //File.Delete(this.Result[Index].FilePath);
            RecybleBin.Send(this.Result[Index].FilePath);
            this.listView1.VirtualListSize--;
            this.Result.RemoveAt(Index);
            this.listView1.Refresh();
            if(Index >= this.listView1.VirtualListSize && this.listView1.VirtualListSize > 0)
            {
                Index = this.listView1.VirtualListSize - 1;
            }
            if (this.listView1.VirtualListSize > 0)
            {
                this.listView1.EnsureVisible(Index);
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                this.search_button.PerformClick();
            }
        }

        private void openOuterSoftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedIndices.Count > 0)
            {
                int i = this.listView1.SelectedIndices[0];
                System.Diagnostics.Process.Start(Result[i].FilePath);
            }
            
        }
    }
}
