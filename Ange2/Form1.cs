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
using AutocompleteMenuNS;

namespace Ange
{
    public partial class Form1 : Form
    {
        private const int PreviewWidth = 300;
        private const int PreviewHeight = 225;
        public static SQLiteConnection Previews;
        public static SQLiteConnection Erza;
        //SolidBrush brush;
        private CustomAdaptor adaptor;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //Form1.Erza = new SQLiteConnection("data source="+ Application.StartupPath + "\\Erza.sqlite");
            Form1.Erza = new SQLiteConnection("data source=C:\\utils\\data\\Erza.sqlite");
            Form1.Erza.Open();
            //this.Previews = new SQLiteConnection("data source=" + Application.StartupPath + "\\Previews.sqlite");
            //this.Previews = new SQLiteConnection("data source=C:\\utils\\data\\Previews.sqlite");
            Form1.Previews = new SQLiteConnection("data source=E:\\Previews.sqlite");
            Form1.Previews.Open();
            //this.brush = new SolidBrush(Color.Orange);
            adaptor = new CustomAdaptor();
            //this.imageListView1.ThumbnailSize = new Size(300, 225);
            //Загружвем список тегов
            autocompleteMenu1.MaximumSize = new System.Drawing.Size(350, 200);
            var columnWidth = new int[] { 250, 100 };
            //List<string> tags = new List<string>();
            using (SQLiteCommand command = new SQLiteCommand(Erza))
            {
                command.CommandText = "SELECT tag, count FROM tags ORDER BY tag ASC";
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
                        //tags.Add(reader.GetString(0));
                        //tags.Add(tag);
                        autocompleteMenu1.AddItem(new MulticolumnAutocompleteItem(new[] { tag, reader.GetInt64(1).ToString() }, tag) { ColumnWidth = columnWidth });
                    }
                }
            }
            this.option_comboBox.SelectedIndex = 0;
            this.DoubleBuffered = true;
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form1.Previews.Close();
            Form1.Erza.Close();
            //this.brush.Dispose();
        }
        private void view_fullscreen_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.imageListView1.SelectedItems.Count > 0)
            {
                imageListView1.SuspendLayout();
                int index = this.imageListView1.SelectedItems[0].Index;
                FullScreenForm form = new FullScreenForm();
                form.Result = new List<ImageInfo>();
                foreach (ImageListViewItem item in this.imageListView1.Items)
                {
                    form.Result.Add((ImageInfo)item.VirtualItemKey);
                }
                form.Index = index;
                form.ShowDialog();
                if (form.ResultChanged)
                {
                    foreach (ImageInfo img in form.Result)
                    {
                        imageListView1.Items.Add(img, img.Hash, adaptor);
                    }
                }
                //this.imageListView1.Items[form.Index].Selected = true;
                imageListView1.ResumeLayout();
                this.imageListView1.EnsureVisible(form.Index);
            }
        }
        private void slideshowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SlideShowForm form = new SlideShowForm();
            form.Result = new List<ImageInfo>();
            foreach (ImageListViewItem item in this.imageListView1.Items)
            {
                form.Result.Add((ImageInfo)item.VirtualItemKey);
            }
            form.Index = this.imageListView1.SelectedItems[0].Index;
            form.ShowDialog();
        }
        private void edittagsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.imageListView1.SelectedItems.Count > 0)
            {
                EditTagsForm form = new EditTagsForm();
                form.Connection = Erza;
                form.EditImage = (ImageInfo)this.imageListView1.SelectedItems[0].VirtualItemKey;
                form.ShowDialog();
            }
        }
        private void copytowallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.imageListView1.SelectedItems.Count > 0)
            {
                foreach (ImageListViewItem item in this.imageListView1.SelectedItems)
                {
                    ImageInfo img = (ImageInfo)item.VirtualItemKey;
                    string dest_path = "E:\\Wallpapers\\" + Path.GetFileName(img.FilePath);
                    try
                    {
                        if (File.Exists(dest_path))
                        {
                            if (MessageBox.Show("Целевой фаил уже сушествует, перезаписать?", "Предупреждение!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                            {
                                File.Delete(dest_path);
                                File.Copy(img.FilePath, dest_path);
                            }
                        }
                        else
                        {
                            File.Copy(img.FilePath, dest_path);
                        }
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteImage();
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
                Clipboard.SetText(((ImageInfo)this.imageListView1.SelectedItems[0].VirtualItemKey).Hash);
            }
        }
        private void copytodirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.imageListView1.SelectedItems.Count > 0)
            {
                if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    foreach (ImageListViewItem item in this.imageListView1.SelectedItems)
                    {
                        ImageInfo img = (ImageInfo)item.VirtualItemKey;
                        string dest_path = this.folderBrowserDialog1.SelectedPath + "\\" + Path.GetFileName(img.FilePath);
                        try
                        {
                            if (File.Exists(dest_path))
                            {
                                if (MessageBox.Show("Целевой фаил уже сушествует, перезаписать?", "Предупреждение!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                                {
                                    File.Delete(dest_path);
                                    File.Copy(img.FilePath, dest_path);
                                }
                            }
                            else
                            {
                                File.Copy(img.FilePath, dest_path);
                            }
                        }
                        catch (IOException ex)
                        {
                            MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }
        private void DeleteImage()
        {
            if (this.imageListView1.SelectedItems.Count > 0)
            {
                if (MessageBox.Show("Удалить выбранные(" + this.imageListView1.SelectedItems.Count .ToString() + ") изображения?", "Предупреждение!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    RemoveImages();
                }
            }
        }
        private void RemoveImages()
        {
            imageListView1.SuspendLayout();
            List<ImageListViewItem> selitems = new List<ImageListViewItem>();
            foreach (ImageListViewItem item in this.imageListView1.SelectedItems)
            {
                selitems.Add(item);
                ImageInfo img = (ImageInfo)item.VirtualItemKey;
                ErzaDB.DeleteImage(img.ImageID, Erza);
                RecybleBin.Send(img.FilePath);
            }
            foreach (ImageListViewItem item in selitems)
            {
                this.imageListView1.Items.Remove(item);
            }
            this.imageListView1.ClearSelection();
            imageListView1.ResumeLayout();
            //this.imageListView1.Refresh();
        }
        private void openOuterSoftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.imageListView1.SelectedItems.Count > 0)
            {
                ImageInfo img = (ImageInfo)this.imageListView1.SelectedItems[0].VirtualItemKey;
                System.Diagnostics.Process.Start(img.FilePath);
            }
            
        }
        private void MoveAllToDirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                foreach (ImageListViewItem item in this.imageListView1.Items)
                {
                    ImageInfo img = (ImageInfo)item.VirtualItemKey;
                    File.Move(img.FilePath, this.folderBrowserDialog1.SelectedPath + "\\" + Path.GetFileName(img.FilePath));
                }
            }
        }
        private void copyAllToDirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                foreach (ImageListViewItem item in this.imageListView1.Items)
                {
                    ImageInfo img = (ImageInfo)item.VirtualItemKey;
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
                int index = this.imageListView1.SelectedItems[0].Index;
                ViewImageForm form = new ViewImageForm();
                form.Result = new List<ImageInfo>();
                foreach (ImageListViewItem item in this.imageListView1.Items)
                {
                    form.Result.Add((ImageInfo)item.VirtualItemKey);
                }
                form.Index = index;
                form.Erza = Form1.Erza;
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
                    this.tags_textBox.Text = sb.ToString();
                    this.search_button.PerformClick();
                }
                else
                {
                    if (form.ResultChanged)
                    {
                        imageListView1.SuspendLayout();
                        imageListView1.Items.Clear();
                        foreach (ImageInfo img in form.Result)
                        {
                            imageListView1.Items.Add(img, img.Hash, adaptor);
                        }
                        imageListView1.ResumeLayout();
                        this.imageListView1.Refresh();
                    }
                    //this.imageListView1.Items[form.Index].Selected = true;
                    //this.imageListView1.Items[form.Index].Update();
                    //this.imageListView1.Items.FocusedItem = this.imageListView1.Items[form.Index];
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
                    ImageInfo img = (ImageInfo)item.VirtualItemKey;
                    using (Bitmap preview = CreateThumbnail(img.FilePath, PreviewWidth, PreviewHeight))
                    {
                        if (preview != null)
                        {
                            using (MemoryStream stream = new MemoryStream())
                            {
                                preview.Save(stream, jpgEncoder, myEncoderParameters);
                                UpdatePreviewToDB(img.Hash, stream.ToArray(), Previews);
                            }
                            item.Update();
                        }
                        else
                        {
                            MessageBox.Show(img.FilePath + " Ошибка!");
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
            System.Drawing.Bitmap bmpOut;
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
        private void search_toolStripButton_Click(object sender, EventArgs e)
        {

        }
        private bool SelectIsTags()
        {
            if(this.option_comboBox.Text == "Теги")
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
            if (this.option_comboBox.Text == "Теги ИЛИ")
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
            if (this.option_comboBox.Text == "Часть тега")
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
            if (this.option_comboBox.Text == "MD5")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #region Custom Item Adaptor
        private class CustomAdaptor : ImageListView.ImageListViewItemAdaptor
        {
            public override Image GetThumbnail(object key, Size size, UseEmbeddedThumbnails useEmbeddedThumbnails, bool useExifOrientation)
            {
                ImageInfo img = (ImageInfo)key;
                return GetPreview(img.Hash);
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
            private Image GetPreview(long image_id)
            {
                using (SQLiteCommand command = new SQLiteCommand(Form1.Previews))
                {
                    command.CommandText = "SELECT preview FROM previews WHERE image_id = @image_id;";
                    command.Parameters.AddWithValue("image_id", image_id);
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
                ImageInfo img = (ImageInfo)key;
                return img.Hash;
            }
            public override string GetSourceImage(object key)
            {
                ImageInfo img = (ImageInfo)key;
                return img.FilePath;
            }
            /// <summary>
            /// Returns the details for the given item.
            /// </summary>
            /// <param name="key">Item key.</param>
            /// <returns>An array of tuples containing item details or null if an error occurs.</returns>
            public override Utility.Tuple<ColumnType, string, object>[] GetDetails(object key)
            {
                ImageInfo img = (ImageInfo)key;
                List<Utility.Tuple<ColumnType, string, object>> details = new List<Utility.Tuple<ColumnType, string, object>>();
                // Get file info
                //details.Add(new Utility.Tuple<ColumnType, string, object>(ColumnType.DateCreated, string.Empty, string.Empty));
                //details.Add(new Utility.Tuple<ColumnType, string, object>(ColumnType.DateAccessed, string.Empty, string.Empty));
                //details.Add(new Utility.Tuple<ColumnType, string, object>(ColumnType.DateModified, string.Empty, string.Empty));
                //details.Add(new Utility.Tuple<ColumnType, string, object>(ColumnType.FileSize, string.Empty, string.Empty));
                //details.Add(new Utility.Tuple<ColumnType, string, object>(ColumnType.FilePath, string.Empty, string.Empty));
                //details.Add(new Utility.Tuple<ColumnType, string, object>(ColumnType.FolderName, string.Empty, string.Empty));
                List<TagInfo> tag_list = ErzaDB.GetTagsByImageID(img.ImageID, Erza);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < tag_list.Count; i++)
                {
                    if (i == 0)
                    {
                        sb.Append(tag_list[i].Tag);
                    }
                    else
                    {
                        sb.Append(' ');
                        sb.Append(tag_list[i].Tag);
                    }
                }
                details.Add(new Utility.Tuple<ColumnType, string, object>(ColumnType.UserComment, string.Empty, sb.ToString()));
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

        private void slide_show_button_Click(object sender, EventArgs e)
        {
            SlideShowForm form = new SlideShowForm();
            form.Result = new List<ImageInfo>();
            foreach (ImageListViewItem item in this.imageListView1.Items)
            {
                form.Result.Add((ImageInfo)item.VirtualItemKey);
            }
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

        private void search_button_Click(object sender, EventArgs e)
        {
            if (this.tags_textBox.Text.Length == 0)
            {
                imageListView1.SuspendLayout();
                imageListView1.Items.Clear();
                List<ImageInfo> Result = ErzaDB.GetAllImages(Form1.Erza);
                foreach (ImageInfo img in Result)
                {
                    imageListView1.Items.Add(img, img.Hash, adaptor);
                }
                imageListView1.ResumeLayout();
                imageListView1.EnsureVisible(0);
                this.toolStripStatusLabel1.Text = "Изображений найдено: " + Result.Count.ToString();
                return;
            }
            if (SelectIsTags())
            {
                string[] temp = this.tags_textBox.Text.Split(' ');
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
                    List<ImageInfo> Result = ErzaDB.GetImagesByTags(new List<string>(tags), false, Form1.Erza);
                    foreach (ImageInfo img in Result)
                    {
                        imageListView1.Items.Add(img, img.Hash, adaptor);
                    }
                    imageListView1.ResumeLayout();
                    imageListView1.EnsureVisible(0);
                }
                else
                {
                    imageListView1.SuspendLayout();
                    imageListView1.Items.Clear();
                    List<ImageInfo> Result;
                    if (tags.Count == 1)
                    {
                        Result = ErzaDB.GetImagesByTag(tags[0], Form1.Erza);
                    }
                    else
                    {
                        Result = ErzaDB.GetAllImages(Form1.Erza);
                    }
                    foreach (ImageInfo img in Result)
                    {
                        imageListView1.Items.Add(img, img.Hash, adaptor);
                    }
                    imageListView1.ResumeLayout();
                    imageListView1.EnsureVisible(0);
                }
                if (imageListView1.Items.Count > 0)
                {
                    imageListView1.EnsureVisible(0);
                }
                this.toolStripStatusLabel1.Text = "Изображений найдено: " + imageListView1.Items.Count.ToString();
                return;
            }
            if (SelectIsTagsOr())
            {
                string[] temp = this.tags_textBox.Text.Split(' ');
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
                    List<ImageInfo> Result = ErzaDB.GetImagesByTags(new List<string>(tags), true, Form1.Erza);
                    foreach (ImageInfo img in Result)
                    {
                        imageListView1.Items.Add(img, img.Hash, adaptor);
                    }
                    imageListView1.ResumeLayout();
                    imageListView1.EnsureVisible(0);
                }
                else
                {
                    imageListView1.SuspendLayout();
                    imageListView1.Items.Clear();
                    List<ImageInfo> Result;
                    if (tags.Count == 1)
                    {
                        Result = ErzaDB.GetImagesByTag(tags[0], Form1.Erza);
                    }
                    else
                    {
                        Result = ErzaDB.GetAllImages(Form1.Erza);
                    }
                    foreach (ImageInfo img in Result)
                    {
                        imageListView1.Items.Add(img, img.Hash, adaptor);
                    }
                    imageListView1.ResumeLayout();
                    imageListView1.EnsureVisible(0);
                }
                if (imageListView1.Items.Count > 0)
                {
                    this.imageListView1.EnsureVisible(0);
                }
                this.toolStripStatusLabel1.Text = "Изображений найдено: " + imageListView1.Items.Count.ToString();
                return;
            }
            if (SelectIsPartTag())
            {
                imageListView1.SuspendLayout();
                imageListView1.Items.Clear();
                List<ImageInfo> Result = ErzaDB.GetImagesByPartTag(this.tags_textBox.Text, Form1.Erza);
                foreach (ImageInfo img in Result)
                {
                    imageListView1.Items.Add(img, img.Hash, adaptor);
                }
                imageListView1.ResumeLayout();
                imageListView1.EnsureVisible(0);
                if (imageListView1.Items.Count > 0)
                {
                    this.imageListView1.EnsureVisible(0);
                }
                this.toolStripStatusLabel1.Text = "Изображений найдено: " + imageListView1.Items.Count.ToString();
                return;
            }
            if (SelectIsMD5())
            {
                ImageInfo img = ErzaDB.GetImageWithOutTags(this.tags_textBox.Text, Form1.Erza);
                if (img != null)
                {
                    imageListView1.SuspendLayout();
                    imageListView1.Items.Clear();
                    imageListView1.Items.Add(img, img.Hash, adaptor);
                    imageListView1.ResumeLayout();
                    imageListView1.EnsureVisible(0);
                    if (imageListView1.Items.Count > 0)
                    {
                        this.imageListView1.EnsureVisible(0);
                    }
                    this.toolStripStatusLabel1.Text = "Изображений найдено: " + imageListView1.Items.Count.ToString();
                }
                else
                {
                    imageListView1.SuspendLayout();
                    imageListView1.Items.Clear();
                    imageListView1.ResumeLayout();
                }
                return;
            }
        }
    }

}
