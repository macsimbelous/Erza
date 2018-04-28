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

namespace Ange
{
    public partial class FullScreenForm : Form
    {
        public List<ImageInfo> Result;
        public int Index = 0;
        public Form1 main_form;
        public long FileSize = 0;
        public string ImageFormat = null;
        public string TagString;
        public FullScreenForm()
        {
            InitializeComponent();
        }

        private void FullScreenForm_Load(object sender, EventArgs e)
        {
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            //this.pictureBox1.ImageLocation = this.Result[this.Index].FilePath;
            //if (this.pictureBox1.Image != null) { this.pictureBox1.Image.Dispose(); }
            //this.pictureBox1.Image = Image.FromFile(this.Result[this.Index].FilePath);
            LoadImage(this.Index);
        }

        private void FullScreenForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
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
                        LoadImage(this.Index);
                    }
                    break;
                case Keys.Left:
                    if (this.Index > 0)
                    {
                        this.Index--;
                        //if (this.pictureBox1.Image != null) { this.pictureBox1.Image.Dispose(); }
                        //this.pictureBox1.Image = Image.FromFile(this.Result[this.Index].FilePath);
                        LoadImage(this.Index);
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
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (this.pictureBox1.Image != null)
            {
                string s = String.Format("Ширина: {0}\nВысота: {1}\nРазмер: {2:N0}\nФормат: {3}\nТеги:\n{4}", this.pictureBox1.Image.Width, this.pictureBox1.Image.Height, FileSize, ImageFormat, TagString);
                g.DrawString(s, new Font("Arial", 10), System.Drawing.Brushes.White, new Point(0, 0));
            }
        }
        private void LoadImage(int Index)
        {
            try
            {
                if (this.pictureBox1.Image != null) { this.pictureBox1.Image.Dispose(); }
                if (System.IO.File.Exists(this.Result[this.Index].FilePath))
                {
                    this.pictureBox1.Image = Image.FromFile(this.Result[this.Index].FilePath);
                    ImageFormat = GetImageFormat(this.pictureBox1.Image);
                    Result[this.Index].Tags = ErzaDB.GetTagsByImageIDToString(Result[this.Index].ImageID, main_form.Erza);
                    StringBuilder tag_string = new StringBuilder();
                    for (int i = 0; i < Result[this.Index].Tags.Count; i++)
                    {

                        tag_string.Append(Result[this.Index].Tags[i]);
                        if (i < Result[this.Index].Tags.Count - 1)
                        {
                            tag_string.Append('\n');
                        }
                    }
                    TagString = tag_string.ToString();
                    FileSize = new System.IO.FileInfo(this.Result[this.Index].FilePath).Length;
                }
                else
                {
                    this.pictureBox1.Image = (Image)Properties.Resources.noimage;
                    ImageFormat = String.Empty;
                    TagString = String.Empty;
                    FileSize = 0;
                }
            }
            catch (Exception ex)
            {
                TagString = ex.Message;
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
    }
}
