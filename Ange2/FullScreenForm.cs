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
using ErzaLib2;
using ImageMagick;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ange
{
    public partial class FullScreenForm : Form
    {
        public List<ImageInfo> Result;
        public int Index = 0;
        //public Form1 main_form;
        public long FileSize = 0;
        public string ImageFormat = null;
        public string TagString;
        FileStream fs = null;
        public bool ResultChanged = false;
        // Переменные для зума и панорамирования
        private float zoomFactor = 1.0f;
        private PointF panOffset = new PointF(0, 0);
        private System.Drawing.Point startPanPoint = new System.Drawing.Point(0, 0);
        private bool isPanning = false;

        // Исходный коэффициент масштаба, при котором картинка идеально вписана
        private float initialZoom = 1.0f;

        private Image CurrentImage = null;
        public FullScreenForm()
        {
            InitializeComponent();
            // Включаем поддержку колесика мыши для PictureBox
            pictureBox1.MouseWheel += pictureBox1_MouseWheel;
        }

        private void FullScreenForm_Load(object sender, EventArgs e)
        {
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            //this.pictureBox1.ImageLocation = this.Result[this.Index].FilePath;
            //if (this.pictureBox1.Image != null) { this.pictureBox1.Image.Dispose(); }
            //this.pictureBox1.Image = Image.FromFile(this.Result[this.Index].FilePath);
            LoadImage();
            if (CurrentImage != null) ResetZoomAndPan();
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
                    if (MessageBox.Show("Удалить изображение " + Result[Index].FilePath + "?", "Предупреждение!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    {
                        if (CurrentImage != null)
                        {
                            CurrentImage.Dispose();
                            CurrentImage = null;
                        }
                        if (pictureBox1.Image != null)
                        {
                            pictureBox1.Enabled = false;
                            pictureBox1.Image.Dispose();
                        }
                        if (fs != null) { fs.Close(); }
                        DeleteImage(Index);
                        if (Result.Count > 0)
                        {
                            if (Index >= Result.Count)
                            {
                                Index = Result.Count - 1;
                            }
                            //this.pictureBox1.Image = Image.FromFile(this.Result[this.Index].FilePath);
                            LoadImage();
                        }
                        else
                        {
                            Close();
                        }
                    }
                    break;
                case Keys.Add:
                    ZoomImageForKey(1.1f);
                    break;
                case Keys.Oemplus:
                    ZoomImageForKey(1.1f);
                    break;
                case Keys.D0:
                    ResetZoomAndPan();
                    break;
                case Keys.NumPad0:
                    ResetZoomAndPan();
                    break;
                case Keys.Subtract:
                    ZoomImageForKey(0.9f);
                    break;
                case Keys.OemMinus:
                    ZoomImageForKey(0.9f);
                    break;
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (CurrentImage == null)
            {
                if (this.pictureBox1.Image != null)
                {
                    string s = String.Format("Ширина: {0}\nВысота: {1}\nРазмер: {2:N0}\nФормат: {3}\nТеги:\n{4}", this.pictureBox1.Image.Width, this.pictureBox1.Image.Height, FileSize, ImageFormat, TagString);
                    g.DrawString(s, new Font("Arial", 10), System.Drawing.Brushes.White, new Point(0, 0));
                }
            }
            else
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                Matrix matrix = new Matrix();
                matrix.Translate(panOffset.X, panOffset.Y);
                matrix.Scale(zoomFactor, zoomFactor);

                g.Transform = matrix;
                g.DrawImage(CurrentImage, 0, 0);
                string s = String.Format("Ширина: {0}\nВысота: {1}\nРазмер: {2:N0}\nФормат: {3}\nТеги:\n{4}", this.pictureBox1.Width, this.pictureBox1.Height, FileSize, ImageFormat, TagString);
                g.DrawString(s, new Font("Arial", 10), System.Drawing.Brushes.White, new Point(0, 0));
            }
        }
        private void LoadImage()
        {
            try
            {
                zoomFactor = 1.0f;
                panOffset = new PointF(0, 0);
                startPanPoint = new System.Drawing.Point(0, 0);
                isPanning = false;
                if (CurrentImage != null)
                {
                    CurrentImage.Dispose();
                    CurrentImage = null;
                }
                if (this.pictureBox1.Image != null) { this.pictureBox1.Image.Dispose(); }
                if (fs != null) { fs.Close(); }
                if (System.IO.File.Exists(this.Result[this.Index].FilePath))
                {
                    if (Path.GetExtension(this.Result[this.Index].FilePath).ToLower() == ".webp")
                    {
                        using (var magickImage = new MagickImage(Result[this.Index].FilePath))
                        {
                            //pictureBox1.Image = magickImage.ToBitmap();
                            CurrentImage = magickImage.ToBitmap();
                        }
                        ImageFormat = "WEBP";
                    }
                    else if (Path.GetExtension(this.Result[this.Index].FilePath).ToLower() == ".avif")
                    {
                        using (var magickImage = new MagickImage(Result[this.Index].FilePath))
                        {
                            //pictureBox1.Image = magickImage.ToBitmap();
                            CurrentImage = magickImage.ToBitmap();
                        }
                        ImageFormat = "AVIF";
                    }
                    else if (Path.GetExtension(this.Result[this.Index].FilePath).ToLower() == ".gif")
                    {
                        fs = new System.IO.FileStream(this.Result[this.Index].FilePath, FileMode.Open, FileAccess.Read);
                        this.pictureBox1.Image = Image.FromStream(fs);
                        ImageFormat = GetImageFormat(this.pictureBox1.Image);
                    }
                    else
                    {
                        fs = new System.IO.FileStream(this.Result[this.Index].FilePath, FileMode.Open, FileAccess.Read);
                        //this.pictureBox1.Image = Image.FromStream(fs);
                        //CurrentImage = Image.FromStream(fs);
                        //fs.Close();
                        Bitmap b = new Bitmap(fs);
                        b.SetResolution(this.DeviceDpi, this.DeviceDpi);
                        CurrentImage = b;
                        ImageFormat = GetImageFormat(this.pictureBox1.Image);
                    }
                    pictureBox1.Enabled = true;
                    ResetZoomAndPan();
                    Result[this.Index].Tags = ErzaDB.GetTagsByImageIDToString(Result[this.Index].ImageID, Form1.Erza);
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

        private void FullScreenForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (fs != null) { fs.Close(); }
            try
            {
                if (CurrentImage != null)
                {
                    CurrentImage.Dispose();
                    CurrentImage = null;
                }
                if (pictureBox1.Image != null)
                {
                    pictureBox1.Image.Dispose();
                }
            }
            catch { }
        }
        private void DeleteImage(int Index)
        {
            ErzaDB.DeleteImage(Result[Index].ImageID, Form1.Erza);
            //File.Delete(Form1.Result[Index].FilePath);
            RecybleBin.Send(Result[Index].FilePath);
            Result.RemoveAt(Index);
            ResultChanged = true;
        }

        private void pictureBox1_Resize(object sender, EventArgs e)
        {
            // Пересчитываем центрирование при изменении размеров окна
            ResetZoomAndPan();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && CurrentImage != null)
            {
                isPanning = true;
                startPanPoint = e.Location;
                pictureBox1.Cursor = Cursors.Hand;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isPanning)
            {
                float dx = e.X - startPanPoint.X;
                float dy = e.Y - startPanPoint.Y;

                panOffset.X += dx;
                panOffset.Y += dy;

                startPanPoint = e.Location;

                // Применяем ограничения на перетаскивание
                KeepImageInBounds();

                pictureBox1.Invalidate();
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isPanning = false;
                pictureBox1.Cursor = Cursors.Default;
            }
        }
        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (CurrentImage == null) return;

            float zoomDelta = e.Delta > 0 ? 1.1f : 0.9f;

            // Ограничения масштаба относительно исходного "вписанного" размера (от 0.5x до 15x)
            float newZoom = zoomFactor * zoomDelta;
            if (newZoom < initialZoom * 0.5f || newZoom > initialZoom * 15.0f) return;

            float mouseX = e.X;
            float mouseY = e.Y;

            // Корректируем смещение, чтобы зум шел в точку курсора
            panOffset.X = mouseX - zoomDelta * (mouseX - panOffset.X);
            panOffset.Y = mouseY - zoomDelta * (mouseY - panOffset.Y);

            zoomFactor = newZoom;

            // Проверяем ограничения после зума, чтобы картинка не улетела
            KeepImageInBounds();

            pictureBox1.Invalidate();
        }
        // Функция сброса и автоматического вписывания по центру
        private void ResetZoomAndPan()
        {
            if (CurrentImage == null || pictureBox1.Width == 0 || pictureBox1.Height == 0) return;

            // Вычисляем масштаб, чтобы картинка вписалась в PictureBox с сохранением пропорций
            float ratioX = (float)pictureBox1.Width / (float)CurrentImage.Width;
            float ratioY = (float)pictureBox1.Height / (float)CurrentImage.Height;

            // Используем Math.Min для вписывания (Zoom) или Math.Max, если нужно заполнить (Stretch)
            initialZoom = Math.Min(ratioX, ratioY);
            zoomFactor = initialZoom;

            // Центрируем картинку
            float posX = ((float)pictureBox1.Width - ((float)CurrentImage.Width * zoomFactor)) / 2f;
            float posY = ((float)pictureBox1.Height - ((float)CurrentImage.Height * zoomFactor)) / 2f;

            panOffset = new PointF(posX, posY);

            pictureBox1.Invalidate();
        }
        // Метод контроля границ панорамирования
        private void KeepImageInBounds()
        {
            if (CurrentImage == null) return;

            // Текущие размеры картинки с учетом зума
            float currentWidth = (float)CurrentImage.Width * zoomFactor;
            float currentHeight = (float)CurrentImage.Height * zoomFactor;

            // Правило: на экране должно оставаться минимум 50 пикселей изображения
            float margin = 50f;

            // Ограничения для левой/правой границы
            if (panOffset.X > (float)pictureBox1.Width - margin)
                panOffset.X = (float)pictureBox1.Width - margin;

            if (panOffset.X < -currentWidth + margin)
                panOffset.X = -currentWidth + margin;

            // Ограничения для верхней/нижней границы
            if (panOffset.Y > (float)pictureBox1.Height - margin)
                panOffset.Y = (float)pictureBox1.Height - margin;

            if (panOffset.Y < -currentHeight + margin)
                panOffset.Y = -currentHeight + margin;
        }
        private void ZoomImageForKey(float zoomDelta)
        {
            // Ограничения масштаба относительно исходного "вписанного" размера (от 0.5x до 15x)
            float newZoom = zoomFactor * zoomDelta;
            if (newZoom < initialZoom * 0.5f || newZoom > initialZoom * 15.0f) return;

            float mouseX = (float)pictureBox1.Height / 2f;
            float mouseY = (float)pictureBox1.Width / 2f;

            // Корректируем смещение, чтобы зум шел в точку курсора
            panOffset.X = mouseX - zoomDelta * (mouseX - panOffset.X);
            panOffset.Y = mouseY - zoomDelta * (mouseY - panOffset.Y);

            zoomFactor = newZoom;

            // Проверяем ограничения после зума, чтобы картинка не улетела
            KeepImageInBounds();

            pictureBox1.Invalidate();
        }
    }
}
