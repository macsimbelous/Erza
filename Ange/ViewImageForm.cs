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
using ErzaLib;
using System.Data.SQLite;

namespace Ange
{
    public partial class ViewImageForm : Form
    {
        public List<ImageInfo> Result;
        public int Index = 0;
        public string SelectedTag = null;
        public SQLiteConnection Erza;
        public Form1 main_form;
        public ViewImageForm()
        {
            InitializeComponent();
        }

        private void ViewImageForm_Load(object sender, EventArgs e)
        {
            LoadImage();
        }
        private void LoadImage()
        {
            string ImageFormat;
            long FileSize;
            try
            {
                if (this.pictureBox1.Image != null) { this.pictureBox1.Image.Dispose(); }
                if (System.IO.File.Exists(this.Result[this.Index].FilePath))
                {
                    this.pictureBox1.Image = Image.FromFile(this.Result[this.Index].FilePath);
                    ImageFormat = GetImageFormat(this.pictureBox1.Image);
                    this.format_label.Text = "Формат: " + ImageFormat;
                    this.resolution_label.Text = String.Format($"Разрешение: {this.pictureBox1.Image.Size.Width} x {this.pictureBox1.Image.Size.Height}");
                    /*if (this.Result[this.Index].Tags.Count == 0)
                    {
                        Result[this.Index].Tags = ErzaDB.GetTagsByImageID(Result[this.Index].ImageID, ((Form1)this.Parent).Erza);
                    }*/
                    FileSize = new System.IO.FileInfo(this.Result[this.Index].FilePath).Length;
                    this.size_label.Text = "Размер: " + FileSize.ToString();
                    this.listBox1.Items.AddRange(ErzaDB.GetTagsByImageID(Result[this.Index].ImageID, this.Erza).ToArray());
                    this.tags_count_label.Text = "Количество тегов: " + this.listBox1.Items.Count.ToString();
                }
                else
                {
                    this.pictureBox1.Image = (Image)Properties.Resources.noimage;
                    ImageFormat = String.Empty;
                    FileSize = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private string GetImageFormat(Image image)
        {
            string imageFormat = String.Empty;
            if (System.Drawing.Imaging.ImageFormat.Bmp.Equals(image.RawFormat))
            {
                imageFormat = "BMP";
            }

            if (System.Drawing.Imaging.ImageFormat.Emf.Equals(image.RawFormat))
            {
                imageFormat = "EMF";
            }

            if (System.Drawing.Imaging.ImageFormat.Gif.Equals(image.RawFormat))
            {
                imageFormat = "GIF";
            }

            if (System.Drawing.Imaging.ImageFormat.Icon.Equals(image.RawFormat))
            {
                imageFormat = "ICON";
            }

            if (System.Drawing.Imaging.ImageFormat.Jpeg.Equals(image.RawFormat))
            {
                imageFormat = "JPEG";
            }

            if (System.Drawing.Imaging.ImageFormat.Png.Equals(image.RawFormat))
            {
                imageFormat = "PNG";
            }

            if (System.Drawing.Imaging.ImageFormat.Tiff.Equals(image.RawFormat))
            {
                imageFormat = "TIFF";
            }

            if (System.Drawing.Imaging.ImageFormat.Wmf.Equals(image.RawFormat))
            {
                imageFormat = "WMF";
            }
            if (imageFormat.Length == 0)
            {
                imageFormat = "Unknown";
            }
            return imageFormat;
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            SelectedTag = (string)this.listBox1.SelectedItem;
            this.Close();
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Escape:
                    this.Close();
                    break;
                case Keys.Right:
                    if (this.Index < (this.Result.Count - 1))
                    {
                        this.Index++;
                        //if (this.pictureBox1.Image != null) { this.pictureBox1.Image.Dispose(); }
                        //this.pictureBox1.Image = Image.FromFile(this.Result[this.Index].FilePath);
                        LoadImage();
                    }
                    break;
                case Keys.Left:
                    if (this.Index > 0)
                    {
                        this.Index--;
                        //if (this.pictureBox1.Image != null) { this.pictureBox1.Image.Dispose(); }
                        //this.pictureBox1.Image = Image.FromFile(this.Result[this.Index].FilePath);
                        LoadImage();
                    }
                    break;
                case Keys.Delete:
                    if (MessageBox.Show("Удалить изображение " + this.Result[this.Index].FilePath + "?", "Предупреждение!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    {
                        if (this.pictureBox1.Image != null) { this.pictureBox1.Image.Dispose(); }
                        main_form.DeleteImage(this.Index);
                        if (this.Result.Count > 0)
                        {
                            if (this.Index >= this.Result.Count)
                            {
                                this.Index = this.Result.Count - 1;
                            }
                            this.pictureBox1.Image = Image.FromFile(this.Result[this.Index].FilePath);
                        }
                    }
                    break;
            }
            return true;
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            FullScreenForm form = new FullScreenForm();
            form.Result = Result;
            form.Index = this.Index;
            form.main_form = this.main_form;
            form.ShowDialog();
            this.Index = form.Index;
            LoadImage();
        }

        private void viewinfullscreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FullScreenForm form = new FullScreenForm();
            form.Result = Result;
            form.Index = this.Index;
            form.main_form = this.main_form;
            form.ShowDialog();
            this.Index = form.Index;
            LoadImage();
        }

        private void edittagsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditTagsForm form = new EditTagsForm();
            form.Connection = Erza;
            form.EditImage = this.Result[this.Index];
            form.ShowDialog();
            LoadImage();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Удалить изображение " + this.Result[this.Index].FilePath + "?", "Предупреждение!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                if (this.pictureBox1.Image != null) { this.pictureBox1.Image.Dispose(); }
                main_form.DeleteImage(this.Index);
                if (this.Result.Count > 0)
                {
                    if (this.Index >= this.Result.Count)
                    {
                        this.Index = this.Result.Count - 1;
                    }
                    this.pictureBox1.Image = Image.FromFile(this.Result[this.Index].FilePath);
                }
            }
        }

        private void nextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.Index < (this.Result.Count - 1))
            {
                this.Index++;
                LoadImage();
            }
        }

        private void prevToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.Index > 0)
            {
                this.Index--;
                LoadImage();
            }
        }
    }
}
