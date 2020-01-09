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
using System.Runtime.InteropServices;
using Manina.Windows.Forms;

namespace Ange
{
    public partial class Form1 : Form
    {
        private const int PreviewWidth = 300;
        private const int PreviewHeight = 225;
        public static SQLiteConnection Previews;
        public SQLiteConnection Erza;
        public static List<ImageInfo> Result;
        SolidBrush brush;
        private CustomAdaptor adaptor;
        public Form1()
        {
            InitializeComponent();
            //ListView_SetSpacing(this.imageListView1, PreviewWidth +12, PreviewHeight + 24);
            //imageList1.ImageSize = new Size(300, 225);
        }

        private void imageListView1_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
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
            using (SQLiteCommand command = new SQLiteCommand(Form1.Previews))
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
        
        private void imageListView1_DrawItem(object sender, DrawListViewItemEventArgs e)
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
            //StringFormat drawFormat = new StringFormat();
            //drawFormat.Alignment = StringAlignment.Center;
            //e.Graphics.DrawString(e.Item.Text, new Font("Arial", 8f), new SolidBrush(Color.Black), e.Bounds.X + (e.Bounds.Width / 2f), e.Bounds.Y + 156f, drawFormat);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //this.Erza = new SQLiteConnection("data source="+ Application.StartupPath + "\\Erza.sqlite");
            this.Erza = new SQLiteConnection("data source=C:\\utils\\data\\Erza.sqlite");
            this.Erza.Open();
            //this.Previews = new SQLiteConnection("data source=" + Application.StartupPath + "\\Previews.sqlite");
            //this.Previews = new SQLiteConnection("data source=C:\\utils\\data\\Previews.sqlite");
            Form1.Previews = new SQLiteConnection("data source=E:\\Previews.sqlite");
            Form1.Previews.Open();
            Form1.Result = new List<ImageInfo>();
            this.brush = new SolidBrush(Color.Orange);
            adaptor = new CustomAdaptor();
            //this.imageListView1.ThumbnailSize = new Size(300, 225);
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

            AutoCompleteStringCollection ts = new AutoCompleteStringCollection();
            ts.AddRange(tags.ToArray());
            this.tag_toolStripComboBox.AutoCompleteCustomSource = ts;
            this.tag_toolStripComboBox.AutoCompleteMode = AutoCompleteMode.Suggest;
            this.tag_toolStripComboBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            this.option_toolStripComboBox.SelectedIndex = 0;
            this.DoubleBuffered = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form1.Previews.Close();
            this.Erza.Close();
            this.brush.Dispose();
        }

        private void imageListView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ViewImageInWindow();
        }

        private void slideshow_button_Click(object sender, EventArgs e)
        {
            SlideShowForm form = new SlideShowForm();
            form.Result = Form1.Result;
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
            if (this.imageListView1.SelectedItems.Count > 0)
            {
                imageListView1.SuspendLayout();
                int key = (int)this.imageListView1.SelectedItems[0].VirtualItemKey;
                FullScreenForm form = new FullScreenForm();
                form.Result = Result;
                form.Index = key;
                form.main_form = this;
                form.ShowDialog();
                //imageListView1.SuspendLayout();
                imageListView1.Items.Clear();
                for (int i = 0; i < Form1.Result.Count; i++)
                {
                    imageListView1.Items.Add(i, Form1.Result[i].Hash, adaptor);
                }
                imageListView1.ResumeLayout();
                this.imageListView1.Refresh();
                this.imageListView1.EnsureVisible(form.Index);
            }
        }

        private void slideshowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int i = (int)this.imageListView1.SelectedItems[0].VirtualItemKey;
            SlideShowForm form = new SlideShowForm();
            form.Result = Result;
            form.Index = i;
            form.ShowDialog();
        }

        private void edittagsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.imageListView1.SelectedItems.Count > 0)
            {
                EditTagsForm form = new EditTagsForm();
                form.Connection = Erza;
                int i = (int)this.imageListView1.SelectedItems[0].VirtualItemKey;
                form.EditImage = Form1.Result[i];
                form.ShowDialog();
            }
        }

        private void copytowallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.imageListView1.SelectedItems.Count > 0)
            {
                int i = (int)this.imageListView1.SelectedItems[0].VirtualItemKey;
                string dest_path = "E:\\Wallpapers\\" + Path.GetFileName(Form1.Result[i].FilePath);
                try
                {
                    if (File.Exists(dest_path))
                    {
                        if (MessageBox.Show("Целевой фаил уже сушествует, перезаписать?", "Предупреждение!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                        {
                            File.Delete(dest_path);
                            File.Copy(Form1.Result[i].FilePath, dest_path);
                        }
                    }
                    else
                    {
                        File.Copy(Form1.Result[i].FilePath, dest_path);
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
            if (this.imageListView1.SelectedItems.Count > 0)
            {
                if (MessageBox.Show("Удалить изображение " + Form1.Result[(int)this.imageListView1.SelectedItems[0].VirtualItemKey].FilePath + "?", "Предупреждение!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    DeleteImage((int)this.imageListView1.SelectedItems[0].VirtualItemKey);
                }
            }
        }

        private void imageListView1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    ViewImageInWindow();
                    break;
                case Keys.Home:
                    this.imageListView1.EnsureVisible(0);
                    break;
                case Keys.End:
                    this.imageListView1.EnsureVisible(this.imageListView1.Items.Count);
                    break;
            }
        }
        private void copyhashToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.imageListView1.SelectedItems.Count > 0)
            {
                int i = (int)this.imageListView1.SelectedItems[0].VirtualItemKey;
                Clipboard.SetText(Form1.Result[i].Hash);
            }
        }
        private void copytodirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.imageListView1.SelectedItems.Count > 0)
            {
                if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    int i = (int)this.imageListView1.SelectedItems[0].VirtualItemKey;
                    string dest_path = this.folderBrowserDialog1.SelectedPath + "\\" + Path.GetFileName(Form1.Result[i].FilePath);
                    try
                    {
                        if (File.Exists(dest_path))
                        {
                            if (MessageBox.Show("Целевой фаил уже сушествует, перезаписать?", "Предупреждение!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                            {
                                File.Delete(dest_path);
                                File.Copy(Form1.Result[i].FilePath, dest_path);
                            }
                        }
                        else
                        {
                            File.Copy(Form1.Result[i].FilePath, dest_path);
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
            ErzaDB.DeleteImage(Form1.Result[Index].ImageID, Erza);
            //File.Delete(Form1.Result[Index].FilePath);
            RecybleBin.Send(Form1.Result[Index].FilePath);
            Form1.Result.RemoveAt(Index);
            this.imageListView1.Refresh();
        }
        private void RemoveImages()
        {
            if (this.imageListView1.SelectedItems.Count > 0)
            {
                imageListView1.SuspendLayout();
                List<int> keys = new List<int>();
                foreach(ImageListViewItem item in this.imageListView1.SelectedItems)
                {
                    keys.Add((int)item.VirtualItemKey);
                }
                keys.Sort();
                keys.Reverse();
                foreach(int key in keys)
                {
                    ErzaDB.DeleteImage(Form1.Result[key].ImageID, Erza);
                    RecybleBin.Send(Form1.Result[key].FilePath);
                    Form1.Result.RemoveAt(key);
                }
                foreach (ImageListViewItem item in this.imageListView1.SelectedItems)
                {
                    this.imageListView1.Items.Remove(item);
                }
                this.imageListView1.ClearSelection();
                imageListView1.ResumeLayout();
                this.imageListView1.Refresh();
                //this.imageListView1.EnsureVisible(form.Index);
            }
        }
        private void openOuterSoftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.imageListView1.SelectedItems.Count > 0)
            {
                int i = (int)this.imageListView1.SelectedItems[0].VirtualItemKey;
                System.Diagnostics.Process.Start(Result[i].FilePath);
            }
            
        }
        private void MoveAllToDirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                foreach (ImageInfo img in Form1.Result)
                {
                    File.Move(img.FilePath, this.folderBrowserDialog1.SelectedPath + "\\" + Path.GetFileName(img.FilePath));
                }
            }
        }
        private void copyAllToDirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                foreach (ImageInfo img in Form1.Result)
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
            if (this.imageListView1.SelectedItems.Count > 0)
            {
                imageListView1.SuspendLayout();
                int index = (int)this.imageListView1.SelectedItems[0].VirtualItemKey;
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
                    this.tags_toolStripSpringTextBox.Text = sb.ToString();
                    this.search_toolStripButton.PerformClick();
                }
                else
                {
                    //imageListView1.SuspendLayout();
                    imageListView1.Items.Clear();
                    for (int i = 0; i < Form1.Result.Count; i++)
                    {
                        imageListView1.Items.Add(i, Form1.Result[i].Hash, adaptor);
                    }
                    imageListView1.ResumeLayout();
                    this.imageListView1.Refresh();
                    this.imageListView1.EnsureVisible(form.Index);
                }
            }
        }

        private void recreate_preview_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.imageListView1.SelectedItems.Count > 0)
            {
                ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                EncoderParameter myEncoderParameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 80L);
                myEncoderParameters.Param[0] = myEncoderParameter;
                foreach (ImageListViewItem item in this.imageListView1.SelectedItems)
                {
                    //int i = this.imageListView1.SelectedItems[0].Index;
                    using (Bitmap preview = CreateThumbnail(Result[(int)item.VirtualItemKey].FilePath, PreviewWidth, PreviewHeight))
                    {
                        if (preview != null)
                        {
                            using (MemoryStream stream = new MemoryStream())
                            {
                                preview.Save(stream, jpgEncoder, myEncoderParameters);
                                UpdatePreviewToDB(Result[(int)item.VirtualItemKey].Hash, stream.ToArray(), Previews);
                            }
                            item.Update();
                        }
                        else
                        {
                            MessageBox.Show(Result[(int)item.VirtualItemKey].FilePath + " Ошибка!");
                        }
                    }
                }
            }
        }
        public void UpdatePreviewToDB(string Hash, byte[] Preview, SQLiteConnection Connection)
        {
            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                //command.CommandText = "UPDATE previews SET preview = @preview WHERE hash = @hash";
                command.CommandText = "INSERT INTO previews(hash, preview) VALUES(@hash, @preview) ON CONFLICT(hash) DO UPDATE SET preview = @preview;";
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
                    lnNewHeight = lnHeight;
                }
                else
                {
                    lnNewWidth = lnWidth;
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
        private void toolStripSpringTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.search_toolStripButton.PerformClick();

            }
        }

        private void plus_toolStripButton_Click(object sender, EventArgs e)
        {
            if (tag_toolStripComboBox.Text.Length > 0)
            {
                if (this.tags_toolStripSpringTextBox.Text.Length <= 0)
                {
                    this.tags_toolStripSpringTextBox.Text = tag_toolStripComboBox.Text;
                }
                else
                {
                    this.tags_toolStripSpringTextBox.Text = this.tags_toolStripSpringTextBox.Text + " " + this.tag_toolStripComboBox.Text;
                }
            }
        }

        private void search_toolStripButton_Click(object sender, EventArgs e)
        {
            //imageListView1.VirtualListSize = 0;
            if (this.tags_toolStripSpringTextBox.Text.Length == 0)
            {
                imageListView1.SuspendLayout();
                imageListView1.Items.Clear();
                Form1.Result = ErzaDB.GetAllImages(this.Erza);
                if (Form1.Result.Count > 0)
                {
                    for (int i=0;i< Form1.Result.Count;i++)
                    {
                        imageListView1.Items.Add(i, Form1.Result[i].Hash, adaptor);
                    }
                }
                imageListView1.ResumeLayout();
                imageListView1.EnsureVisible(0);
                this.toolStripStatusLabel1.Text = "Изображений найдено: " + Form1.Result.Count.ToString();
                return;
            }
            if (SelectIsTags())
            {
                string[] temp = this.tags_toolStripSpringTextBox.Text.Split(' ');
                List<string> tags = new List<string>();
                foreach (string tag in temp)
                {
                    if (!String.IsNullOrEmpty(tag))
                    {
                        tags.Add(tag);
                    }
                }
                if (tags.Count > 1)
                {
                    imageListView1.SuspendLayout();
                    imageListView1.Items.Clear();
                    Form1.Result = ErzaDB.GetImagesByTags(new List<string>(tags), false, this.Erza);
                    for (int i = 0; i < Form1.Result.Count; i++)
                    {
                        imageListView1.Items.Add(i, Form1.Result[i].Hash, adaptor);
                    }
                    imageListView1.ResumeLayout();
                    imageListView1.EnsureVisible(0);
                }
                else
                {
                    imageListView1.SuspendLayout();
                    imageListView1.Items.Clear();
                    if (tags.Count == 1)
                    {
                        Form1.Result = ErzaDB.GetImagesByTag(tags[0], this.Erza);
                    }
                    else
                    {
                        Form1.Result = ErzaDB.GetAllImages(this.Erza);
                    }
                    for (int i = 0; i < Form1.Result.Count; i++)
                    {
                        imageListView1.Items.Add(i, Form1.Result[i].Hash, adaptor);
                    }
                    imageListView1.ResumeLayout();
                    imageListView1.EnsureVisible(0);
                }
                if (Form1.Result.Count > 0)
                {
                    imageListView1.EnsureVisible(0);
                }
                this.toolStripStatusLabel1.Text = "Изображений найдено: " + Form1.Result.Count.ToString();
                return;
            }
            if (SelectIsTagsOr())
            {
                string[] temp = this.tags_toolStripSpringTextBox.Text.Split(' ');
                List<string> tags = new List<string>();
                foreach (string tag in temp)
                {
                    if (!String.IsNullOrEmpty(tag))
                    {
                        tags.Add(tag);
                    }
                }
                if (tags.Count > 1)
                {
                    imageListView1.SuspendLayout();
                    imageListView1.Items.Clear();
                    Form1.Result = ErzaDB.GetImagesByTags(new List<string>(tags), true, this.Erza);
                    for (int i = 0; i < Form1.Result.Count; i++)
                    {
                        imageListView1.Items.Add(i, Form1.Result[i].Hash, adaptor);
                    }
                    imageListView1.ResumeLayout();
                    imageListView1.EnsureVisible(0);
                }
                else
                {
                    imageListView1.SuspendLayout();
                    imageListView1.Items.Clear();
                    if (tags.Count == 1)
                    {
                        Form1.Result = ErzaDB.GetImagesByTag(tags[0], this.Erza);
                    }
                    else
                    {
                        Form1.Result = ErzaDB.GetAllImages(this.Erza);
                    }
                    for (int i = 0; i < Form1.Result.Count; i++)
                    {
                        imageListView1.Items.Add(i, Form1.Result[i].Hash, adaptor);
                    }
                    imageListView1.ResumeLayout();
                    imageListView1.EnsureVisible(0);
                }
                if (Form1.Result.Count > 0)
                {
                    this.imageListView1.EnsureVisible(0);
                }
                this.toolStripStatusLabel1.Text = "Изображений найдено: " + Form1.Result.Count.ToString();
                return;
            }
            if (SelectIsPartTag())
            {
                imageListView1.SuspendLayout();
                imageListView1.Items.Clear();
                Form1.Result = ErzaDB.GetImagesByPartTag(this.tags_toolStripSpringTextBox.Text, this.Erza);
                for (int i = 0; i < Form1.Result.Count; i++)
                {
                    imageListView1.Items.Add(i, Form1.Result[i].Hash, adaptor);
                }
                imageListView1.ResumeLayout();
                imageListView1.EnsureVisible(0);
                if (Form1.Result.Count > 0)
                {
                    this.imageListView1.EnsureVisible(0);
                }
                this.toolStripStatusLabel1.Text = "Изображений найдено: " + Form1.Result.Count.ToString();
                return;
            }
            if (SelectIsMD5())
            {
                ImageInfo img = ErzaDB.GetImageWithOutTags(this.tags_toolStripSpringTextBox.Text, this.Erza);
                if (img != null)
                {
                    imageListView1.SuspendLayout();
                    imageListView1.Items.Clear();
                    Form1.Result.Clear();
                    Form1.Result.Add(img);
                    for (int i = 0; i < Form1.Result.Count; i++)
                    {
                        imageListView1.Items.Add(i, Form1.Result[i].Hash, adaptor);
                    }
                    imageListView1.ResumeLayout();
                    imageListView1.EnsureVisible(0);
                    if (Form1.Result.Count > 0)
                    {
                        this.imageListView1.EnsureVisible(0);
                    }
                    this.toolStripStatusLabel1.Text = "Изображений найдено: " + Form1.Result.Count.ToString();
                }
                else
                {
                    imageListView1.SuspendLayout();
                    imageListView1.Items.Clear();
                    Form1.Result.Clear();
                    imageListView1.ResumeLayout();
                }
                return;
            }
        }
        private bool SelectIsTags()
        {
            if(this.option_toolStripComboBox.Text == "Теги")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool SelectIsTagsOr()
        {
            if (this.option_toolStripComboBox.Text == "Теги ИЛИ")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool SelectIsPartTag()
        {
            if (this.option_toolStripComboBox.Text == "Часть тега")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool SelectIsMD5()
        {
            if (this.option_toolStripComboBox.Text == "MD5")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void slideshow_toolStripButton_Click(object sender, EventArgs e)
        {
            SlideShowForm form = new SlideShowForm();
            form.Result = Form1.Result;
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
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public int MakeLong(short lowPart, short highPart)
        {
            return (int)(((ushort)lowPart) | (uint)(highPart << 16));
        }

        public void ListView_SetSpacing(ListView listview, short cx, short cy)
        {
            const int LVM_FIRST = 0x1000;
            const int LVM_SETICONSPACING = LVM_FIRST + 53;
            // http://msdn.microsoft.com/en-us/library/bb761176(VS.85).aspx
            // minimum spacing = 4
            SendMessage(listview.Handle, LVM_SETICONSPACING,
            IntPtr.Zero, (IntPtr)MakeLong(cx, cy));

            // http://msdn.microsoft.com/en-us/library/bb775085(VS.85).aspx
            // DOESN'T WORK!
            // can't find ListView_SetIconSpacing in dll comctl32.dll
            //ListView_SetIconSpacing(listView.Handle, 5, 5);
        }
        #region Custom Item Adaptor
        /// <summary>
        /// A custom item adaptor.
        /// </summary>
        private class CustomAdaptor : ImageListView.ImageListViewItemAdaptor
        {
            public override Image GetThumbnail(object key, Size size, UseEmbeddedThumbnails useEmbeddedThumbnails, bool useExifOrientation)
            {
                int i = (int)key;
                /*if (!string.IsNullOrEmpty(file))
                {
                    using (Image img = Image.FromFile(file))
                    {
                        Bitmap thumb = new Bitmap(img, size);
                        return thumb;
                    }
                }

                return null;*/
                //return Form1.thumb;
                return GetPreview(Path.GetFileNameWithoutExtension(Result[i].Hash));

            }
            private Image GetPreview(string hash)
            {
                using (SQLiteCommand command = new SQLiteCommand(Form1.Previews))
                {
                    command.CommandText = "SELECT preview FROM previews WHERE hash = @hash;";
                    command.Parameters.AddWithValue("hash", hash);
                    byte[] tmp = (byte[])command.ExecuteScalar();
                    if (tmp != null)
                    {
                        using (MemoryStream stream = new MemoryStream(tmp))
                        {
                            return new Bitmap(Image.FromStream(stream));
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            public override string GetUniqueIdentifier(object key, Size size, UseEmbeddedThumbnails useEmbeddedThumbnails, bool useExifOrientation)
            {
                int i = (int)key;
                return Result[i].Hash;
            }
            public override string GetSourceImage(object key)
            {
                int i = (int)key;
                return Result[i].Hash;
            }

            /// <summary>
            /// Returns the details for the given item.
            /// </summary>
            /// <param name="key">Item key.</param>
            /// <returns>An array of tuples containing item details or null if an error occurs.</returns>
            public override Utility.Tuple<ColumnType, string, object>[] GetDetails(object key)
            {

                int i = (int)key;
                List<Utility.Tuple<ColumnType, string, object>> details = new List<Utility.Tuple<ColumnType, string, object>>();

                // Get file info
                //details.Add(new Utility.Tuple<ColumnType, string, object>(ColumnType.DateCreated, string.Empty, string.Empty));
                //details.Add(new Utility.Tuple<ColumnType, string, object>(ColumnType.DateAccessed, string.Empty, string.Empty));
                //details.Add(new Utility.Tuple<ColumnType, string, object>(ColumnType.DateModified, string.Empty, string.Empty));
                //details.Add(new Utility.Tuple<ColumnType, string, object>(ColumnType.FileSize, string.Empty, string.Empty));
                //details.Add(new Utility.Tuple<ColumnType, string, object>(ColumnType.FilePath, string.Empty, string.Empty));
                //details.Add(new Utility.Tuple<ColumnType, string, object>(ColumnType.FolderName, string.Empty, string.Empty));

                details.Add(new Utility.Tuple<ColumnType, string, object>(ColumnType.UserComment, string.Empty, Result[i].GetStringOfTags()));
                return details.ToArray();
            }
            public override void Dispose()
            {
                ;
            }
        }
        #endregion

        private void imageListView1_ItemDoubleClick(object sender, ItemClickEventArgs e)
        {
            ViewImageInWindow();
        }
    }

}
