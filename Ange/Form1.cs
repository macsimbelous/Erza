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
        SQLiteConnection Erza;
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
                this.listView1.VirtualListSize = this.Result.Count;
                this.toolStripStatusLabel1.Text = "Изображений найдено: " + this.Result.Count.ToString();
                return;
            }
            if (this.tag_radioButton.Checked)
            {
                string[] tags = this.textBox1.Text.Split(' ');
                if (tags.Length > 1)
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
                    this.Result = ErzaDB.GetImagesByTag(this.textBox1.Text, this.Erza);
                    this.listView1.VirtualListSize = this.Result.Count;
                }
                this.toolStripStatusLabel1.Text = "Изображений найдено: " + this.Result.Count.ToString();
                return;
            }
            if (this.part_tag_radioButton.Checked)
            {
                this.Result = ErzaDB.GetImagesByPartTag(this.textBox1.Text, this.Erza);
                this.listView1.VirtualListSize = this.Result.Count;
                this.toolStripStatusLabel1.Text = "Изображений найдено: " + this.Result.Count.ToString();
                return;
            }
            if (this.md5_radioButton.Checked)
            {
                this.Result.Clear();
                this.Result.Add(ErzaDB.GetImageWithOutTags(this.textBox1.Text, this.Erza));
                this.listView1.VirtualListSize = this.Result.Count;
                this.toolStripStatusLabel1.Text = "Изображений найдено: " + this.Result.Count.ToString();
                return;
            }
        }

        private void listView1_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            if ((e.State & ListViewItemStates.Selected) != 0)
            {
                // Draw the background and focus rectangle for a selected item.
                e.Graphics.FillRectangle(Brushes.Maroon, e.Bounds);
                e.DrawFocusRectangle();
            }
            else
            {
                // Draw the background for an unselected item.
                /*using (LinearGradientBrush brush =
                    new LinearGradientBrush(e.Bounds, Color.Orange,
                    Color.Maroon, LinearGradientMode.Horizontal))
                {
                    e.Graphics.FillRectangle(brush, e.Bounds);
                }*/
                e.Graphics.FillRectangle(this.brush, e.Bounds);
            }
            ListViewItem item = e.Item;
            Image img = (Image)item.Tag;
            //Image img = Image.FromFile(files[e.ItemIndex]);
            float x = (e.Bounds.Width / 2f) - ((float)img.Width / 2f);
            float y = (e.Bounds.Height / 2f) - ((float)img.Height / 2f);
            //e.Graphics.DrawImage(Image.FromFile(@"D:\prev\изображение 111_result.jpg"), new PointF(0F, 0f));
            e.Graphics.DrawImage(img, new RectangleF(e.Bounds.X + x, e.Bounds.Y + y, (float)img.Width, (float)img.Height));
            // Draw the item text for views other than the Details view.
            StringFormat drawFormat = new StringFormat();
            //drawFormat.FormatFlags = StringFormatFlags.DirectionVertical;
            drawFormat.Alignment = StringAlignment.Center;
            e.Graphics.DrawString(e.Item.Text, new Font("Arial", 8f), new SolidBrush(Color.Black), e.Bounds.X + (e.Bounds.Width / 2f), e.Bounds.Y + 156f, drawFormat);
            if (listView1.View != View.Details)
            {
                //e.DrawText();
            }
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
            int i = ((ListView)sender).SelectedIndices[0];
            //ImageInfo img = ErzaDB.GetImageWithOutTags(Result[i].Hash, Erza);
            FullScreenForm form = new FullScreenForm();
            form.Result = Result;
            form.Index = i;
            form.ShowDialog();
        }

        private void slideshow_button_Click(object sender, EventArgs e)
        {
            SlideShowForm form = new SlideShowForm();
            form.Result = this.Result;
            form.Index = 0;
            form.ShowDialog();
        }

        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int i = ((ListView)sender).SelectedIndices[0];
            //ImageInfo img = ErzaDB.GetImageWithOutTags(Result[i].Hash, Erza);
            FullScreenForm form = new FullScreenForm();
            form.Result = Result;
            form.Index = i;
            form.ShowDialog();
            this.listView1.EnsureVisible(form.Index);
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

        }

        private void copytowallToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {

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
            if(e.KeyCode == Keys.Enter)
            {
                int i = ((ListView)sender).SelectedIndices[0];
                //ImageInfo img = ErzaDB.GetImageWithOutTags(Result[i].Hash, Erza);
                FullScreenForm form = new FullScreenForm();
                form.Result = Result;
                form.Index = i;
                form.ShowDialog();
            }
        }
    }
}
