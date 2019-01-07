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
using System.Drawing.Drawing2D;
using System.Data.SQLite;
using System.IO;
using ErzaLib;
using System.Drawing.Imaging;

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
            Result[e.ItemIndex].Tags = ErzaDB.GetTagsByImageIDToString(Result[e.ItemIndex].ImageID, Erza);
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
            if(this.radAutoCompleteBox1.Text.Length == 0)
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
                string[] temp = this.radAutoCompleteBox1.Text.Split(' ');
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
                this.Result = ErzaDB.GetImagesByPartTag(this.radAutoCompleteBox1.Text, this.Erza);
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
                this.Result.Add(ErzaDB.GetImageWithOutTags(this.radAutoCompleteBox1.Text, this.Erza));
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
            //this.Erza = new SQLiteConnection("data source="+ Application.StartupPath + "\\Erza.sqlite");
            this.Erza = new SQLiteConnection("data source=C:\\utils\\data\\Erza.sqlite");
            this.Erza.Open();
            //this.Previews = new SQLiteConnection("data source=" + Application.StartupPath + "\\Previews.sqlite");
            this.Previews = new SQLiteConnection("data source=C:\\utils\\data\\Previews.sqlite");
            this.Previews.Open();
            this.Result = new List<ImageInfo>();
            this.brush = new SolidBrush(Color.Orange);
            //Загружвем список тегов
            List<string> tags = new List<string>();
            using (SQLiteCommand command = new SQLiteCommand(Erza))
            {
                command.CommandText = "SELECT tag FROM tags ORDER BY tag ASC";
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string tag = reader.GetString(0);
                        if (String.IsNullOrEmpty(tag))
                        {
                            continue;
                        }
                        if (String.IsNullOrWhiteSpace(tag))
                        {
                            continue;
                        }
                        if (tag.IndexOf('\n') >= 0)
                        {
                            continue;
                        }
                        if (tag.IndexOf(' ') >= 0)
                        {
                            continue;
                        }
                        if (tag.IndexOf('\t') >= 0)
                        {
                            continue;
                        }
                        if (tag.IndexOf('\r') >= 0)
                        {
                            continue;
                        }
                        if (tag.IndexOf((char)12288) >= 0)
                        {
                            continue;
                        }
                        tags.Add(reader.GetString(0));
                    }
                }
            }
            this.radAutoCompleteBox1.AutoCompleteDataSource = tags;
            this.radAutoCompleteBox1.Delimiter = ' ';
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Previews.Close();
            this.Erza.Close();
            this.brush.Dispose();
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            /*if (this.listView1.SelectedIndices.Count > 0)
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
            }*/
            ViewImageInWindow();
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

        private void view_fullscreen_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedIndices.Count > 0)
            {
                int i = this.listView1.SelectedIndices[0];
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
                string dest_path = "E:\\Wallpapers\\" + Path.GetFileName(this.Result[i].FilePath);
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
            /*if(e.KeyCode == Keys.Enter && this.listView1.SelectedIndices.Count > 0)
            {
                int i = this.listView1.SelectedIndices[0];
                FullScreenForm form = new FullScreenForm();
                form.Result = Result;
                form.Index = i;
                form.main_form = this;
                form.ShowDialog();
                this.listView1.EnsureVisible(form.Index);
            }*/
            if (e.KeyCode == Keys.Enter)
            {
                ViewImageInWindow();
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
        private void openOuterSoftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedIndices.Count > 0)
            {
                int i = this.listView1.SelectedIndices[0];
                System.Diagnostics.Process.Start(Result[i].FilePath);
            }
            
        }
        private void MoveAllToDirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                foreach (ImageInfo img in this.Result)
                {
                    File.Move(img.FilePath, this.folderBrowserDialog1.SelectedPath + "\\" + Path.GetFileName(img.FilePath));
                }
            }
        }
        private void copyAllToDirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                foreach (ImageInfo img in this.Result)
                {
                    File.Copy(img.FilePath, this.folderBrowserDialog1.SelectedPath + "\\" + Path.GetFileName(img.FilePath));
                }
            }
        }
        private void view_in_window_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewImageInWindow();
        }
        private void ViewImageInWindow()
        {
            if (this.listView1.SelectedIndices.Count > 0)
            {
                int index = this.listView1.SelectedIndices[0];
                //ImageInfo img = ErzaDB.GetImageWithOutTags(Result[i].Hash, Erza);
                ViewImageForm form = new ViewImageForm();
                form.Result = Result;
                form.Index = index;
                form.Erza = this.Erza;
                form.main_form = this;
                form.ShowDialog();
                if (form.SelectedTags != null && form.SelectedTags.Count > 0)
                {
                    //this.textBox1.Text = form.SelectedTag;
                    StringBuilder sb = new StringBuilder();
                    for(int i=0;i< form.SelectedTags.Count; i++)
                    {
                        if (i == 0)
                        {
                            sb.Append(form.SelectedTags[i]);
                        }
                        else
                        {
                            sb.Append(' ');
                            sb.Append(form.SelectedTags[i]);
                        }
                    }
                    this.radAutoCompleteBox1.Text = sb.ToString();
                    this.search_button.PerformClick();
                }
                else
                {
                    this.listView1.Refresh();
                    this.listView1.EnsureVisible(form.Index);
                }
            }
        }

        private void recreate_preview_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedIndices.Count > 0)
            {
                ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                EncoderParameter myEncoderParameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 80L);
                myEncoderParameters.Param[0] = myEncoderParameter;
                int i = this.listView1.SelectedIndices[0];
                using (Bitmap preview = CreateThumbnail(Result[i].FilePath, 200, 150))
                {
                    if (preview != null)
                    {
                        using (MemoryStream stream = new MemoryStream())
                        {
                            preview.Save(stream, jpgEncoder, myEncoderParameters);
                            UpdatePreviewToDB(Result[i].Hash, stream.ToArray(), Previews);
                        }
                        this.listView1.Items[i].Tag = preview;
                        this.listView1.RedrawItems(i, i, false);
                    }
                    else
                    {
                        MessageBox.Show(Result[i].FilePath + " Ошибка!");
                    }
                }
            }
        }
        public static void UpdatePreviewToDB(string Hash, byte[] Preview, SQLiteConnection Connection)
        {
            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = "UPDATE previews SET preview = @preview WHERE hash = @hash";
                command.Parameters.AddWithValue("hash", Hash);
                command.Parameters.AddWithValue("preview", Preview);
                command.ExecuteNonQuery();
            }
        }
        public Bitmap CreateThumbnail(string lcFilename, int lnWidth, int lnHeight)
        {
            System.Drawing.Bitmap bmpOut = null;
            try
            {
                Bitmap loBMP = new Bitmap(lcFilename);
                ImageFormat loFormat = loBMP.RawFormat;

                //decimal lnRatio;
                int lnNewWidth = 0;
                int lnNewHeight = 0;

                //*** If the image is smaller than a thumbnail just return it
                if (loBMP.Width < lnWidth && loBMP.Height < lnHeight)
                    return loBMP;

                float temp = (float)loBMP.Width / (float)lnWidth;
                if ((int)((float)loBMP.Height / temp) > lnHeight)
                {
                    temp = (float)loBMP.Height / (float)lnHeight;
                    lnNewWidth = (int)((float)loBMP.Width / temp);
                    lnNewHeight = 150;
                }
                else
                {
                    lnNewWidth = 200;
                    lnNewHeight = (int)((float)loBMP.Height / temp);
                }
                /*if (loBMP.Width > loBMP.Height)
                {
                    lnRatio = (decimal)lnWidth / loBMP.Width;
                    lnNewWidth = lnWidth;
                    decimal lnTemp = loBMP.Height * lnRatio;
                    lnNewHeight = (int)lnTemp;
                }
                else
                {
                    lnRatio = (decimal)lnHeight / loBMP.Height;
                    lnNewHeight = lnHeight;
                    decimal lnTemp = loBMP.Width * lnRatio;
                    lnNewWidth = (int)lnTemp;
                }*/
                bmpOut = new Bitmap(lnNewWidth, lnNewHeight);
                Graphics g = Graphics.FromImage(bmpOut);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.FillRectangle(Brushes.White, 0, 0, lnNewWidth, lnNewHeight);
                g.DrawImage(loBMP, 0, 0, lnNewWidth, lnNewHeight);

                loBMP.Dispose();
            }
            catch
            {
                return null;
            }

            return bmpOut;
        }
        public ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
        private void radAutoCompleteBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.search_button.PerformClick();
            }
        }
    }
}
