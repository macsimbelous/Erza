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
using Ange.Properties;
using ErzaLib2;
using ImageMagick;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using OpenCvSharp;

namespace Ange
{
    public partial class ViewImageForm : Form
    {
        public List<ImageInfo> Result;
        public int Index = 0;
        //public string SelectedTag = null;
        public List<string> SelectedTags = null;
        public SQLiteConnection Erza;
        //public Form1 main_form;
        BindingList<TagInfo> Tags;
        private List<string> Tags_Cache = null;
        FileStream fs = null;
        public bool ResultChanged = false;
        //Colors
        SolidBrush GeneralColor = new SolidBrush(Color.Black);
        SolidBrush ArtistColor = new SolidBrush(Color.FromArgb(235, 156, 0));
        SolidBrush StudioColor = new SolidBrush(Color.Magenta);
        SolidBrush CopyrightColor = new SolidBrush(Color.DarkMagenta);
        SolidBrush CharacterColor = new SolidBrush(Color.Green);
        SolidBrush CharacterColor2 = new SolidBrush(Color.LightGreen);
        SolidBrush CircleColor = new SolidBrush(Color.Aquamarine);
        SolidBrush FaultsColor = new SolidBrush(Color.Red);
        SolidBrush MediumColor = new SolidBrush(Color.Blue);
        SolidBrush MetaColor = new SolidBrush(Color.Violet);
        const int ImageSize = 448; // MOAT Tagger uses 448x448 input size
        const float Threshold = 0.35f; // Standard F1 threshold recommendation
        public ViewImageForm()
        {
            InitializeComponent();
        }

        private void ViewImageForm_Load(object sender, EventArgs e)
        {
            LoadImage();
            listBox1.DrawMode = DrawMode.OwnerDrawVariable;
        }
        private void LoadImage()
        {
            if (Result[this.Index].Favorited)
            {
                add_to_favorited_button.Image = (System.Drawing.Image)Resources.ResourceManager.GetObject("favourite");
            }
            else
            {
                add_to_favorited_button.Image = (System.Drawing.Image)Resources.ResourceManager.GetObject("not_favorited");
            }
            string ImageFormat;
            long FileSize;
            try
            {
                if (this.pictureBox1.Image != null) { this.pictureBox1.Image.Dispose(); }
                if (fs != null) { fs.Close(); }
                if (System.IO.File.Exists(this.Result[this.Index].FilePath))
                {
                    //this.pictureBox1.Image = Image.FromFile(this.Result[this.Index].FilePath);

                    if (Path.GetExtension(this.Result[this.Index].FilePath).ToLower() == ".webp")
                    {
                        using (var magickImage = new MagickImage(Result[this.Index].FilePath))
                        {
                            pictureBox1.Image = magickImage.ToBitmap();
                        }
                        ImageFormat = "WEBP";
                    }
                    else if (Path.GetExtension(this.Result[this.Index].FilePath).ToLower() == ".avif")
                    {
                        using (var magickImage = new MagickImage(Result[this.Index].FilePath))
                        {
                            pictureBox1.Image = magickImage.ToBitmap();
                        }
                        ImageFormat = "AVIF";
                    }
                    else
                    {
                        fs = new System.IO.FileStream(this.Result[this.Index].FilePath, FileMode.Open, FileAccess.Read);
                        this.pictureBox1.Image = Image.FromStream(fs);
                        //fs.Close();
                        ImageFormat = GetImageFormat(this.pictureBox1.Image);
                    }
                    pictureBox1.Enabled = true;
                    this.format_label.Text = "Формат: " + ImageFormat;
                    this.resolution_label.Text = String.Format($"Разрешение: {this.pictureBox1.Image.Size.Width} x {this.pictureBox1.Image.Size.Height}");
                    /*if (this.Result[this.Index].Tags.Count == 0)
                    {
                        Result[this.Index].Tags = ErzaDB.GetTagsByImageID(Result[this.Index].ImageID, ((Form1)this.Parent).Erza);
                    }*/
                    FileSize = new System.IO.FileInfo(this.Result[this.Index].FilePath).Length;
                    //this.size_label.Text = "Размер: " + FileSize.ToString();
                    this.size_label.Text = String.Format($"Размер: {FileSize:### ### ###} байт");
                    //this.listBox1.Items.AddRange(ErzaDB.GetTagsByImageID(Result[this.Index].ImageID, this.Erza).ToArray());
                    List<TagInfo> temp = ErzaDB.GetTagsByImageID(Result[this.Index].ImageID, this.Erza);
                    //temp = ErzaDB.CountTags(temp, this.Erza);
                    this.Tags = new BindingList<TagInfo>(temp.OrderBy(tag => tag.Tag).ToList());
                    this.listBox1.DataSource = this.Tags;
                    this.tags_count_label.Text = "Количество тегов: " + this.listBox1.Items.Count.ToString();
                    this.Text = "Просмотр " + this.Result[this.Index].Hash;
                }
                else
                {
                    this.pictureBox1.Image = (Image)Properties.Resources.noimage;
                }
            }
            catch (Exception ex)
            {
                this.Text = "Просмотр " + this.Result[this.Index].Hash + " - " + ex.Message;
                if (this.pictureBox1.Image != null) { this.pictureBox1.Image.Dispose(); }
                //MessageBox.Show(ex.Message);
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
            this.SelectedTags = new List<string>();
            foreach (var item in listBox1.SelectedItems)
            {
                TagInfo t = (TagInfo)item;
                this.SelectedTags.Add(t.Tag);
            }
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
                        if (this.pictureBox1.Image != null)
                        {
                            pictureBox1.Enabled = false;
                            this.pictureBox1.Image.Dispose();
                        }
                        if (fs != null) { fs.Close(); }
                        DeleteImage(this.Index);
                        if (this.Result.Count > 0)
                        {
                            if (this.Index >= this.Result.Count)
                            {
                                this.Index = this.Result.Count - 1;
                            }
                            //this.pictureBox1.Image = Image.FromFile(this.Result[this.Index].FilePath);
                            LoadImage();
                        }
                        else
                        {
                            this.Close();
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
            //form.main_form = this.main_form;
            form.ShowDialog();
            this.Index = form.Index;
            LoadImage();
        }

        private void viewinfullscreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FullScreenForm form = new FullScreenForm();
            form.Result = Result;
            form.Index = this.Index;
            //form.main_form = this.main_form;
            form.ShowDialog();
            this.Index = form.Index;
            LoadImage();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Удалить изображение " + this.Result[this.Index].FilePath + "?", "Предупреждение!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                if (this.pictureBox1.Image != null)
                {
                    pictureBox1.Enabled = false;
                    this.pictureBox1.Image.Dispose();
                }
                if (fs != null) { fs.Close(); }
                DeleteImage(this.Index);
                if (this.Result.Count > 0)
                {
                    if (this.Index >= this.Result.Count)
                    {
                        this.Index = this.Result.Count - 1;
                    }
                    //this.pictureBox1.Image = Image.FromFile(this.Result[this.Index].FilePath);
                    LoadImage();
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

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            if (e.Index >= 0 && e.Index < listBox1.Items.Count)
            {
                Graphics g = e.Graphics;
                SolidBrush foregroundBrush = new SolidBrush(Color.White);
                bool selected;
                switch (((TagInfo)listBox1.Items[e.Index]).Type)
                {
                    case TagType.General:
                        foregroundBrush = GeneralColor;
                        break;
                    case TagType.Artist:
                        foregroundBrush = ArtistColor;
                        break;
                    case TagType.Studio:
                        foregroundBrush = StudioColor;
                        break;
                    case TagType.Copyright:
                        foregroundBrush = CopyrightColor;
                        break;
                    case TagType.Character:
                        //foregroundBrush = CharacterColor;
                        selected = ((e.State & DrawItemState.Selected) == DrawItemState.Selected);
                        if (selected)
                        {
                            foregroundBrush = CharacterColor2;
                        }
                        else
                        {
                            foregroundBrush = CharacterColor;
                        }
                        break;
                    case TagType.Circle:
                        foregroundBrush = CircleColor;
                        break;
                    case TagType.Faults:
                        foregroundBrush = FaultsColor;
                        break;
                    case TagType.Medium:
                        selected = ((e.State & DrawItemState.Selected) == DrawItemState.Selected);
                        if (selected)
                        {
                            foregroundBrush = GeneralColor;
                        }
                        else
                        {
                            foregroundBrush = MediumColor;
                        }
                        break;
                    case TagType.Meta:
                        foregroundBrush = MetaColor;
                        break;
                }
                g.DrawString(((TagInfo)listBox1.Items[e.Index]).Tag + " - " + ((TagInfo)listBox1.Items[e.Index]).Count.ToString(), e.Font, foregroundBrush, listBox1.GetItemRectangle(e.Index).Location);
            }
            e.DrawFocusRectangle();
        }

        private void Search_button_Click(object sender, EventArgs e)
        {
            this.SelectedTags = new List<string>();
            foreach (var item in listBox1.SelectedItems)
            {
                TagInfo t = (TagInfo)item;
                this.SelectedTags.Add(t.Tag);
            }
            this.Close();
        }

        private void RemoveTag_button_Click(object sender, EventArgs e)
        {
            foreach (var item in listBox1.SelectedItems)
            {
                TagInfo t = (TagInfo)item;
                ErzaDB.DeleteTagFromImage(t.Tag, Result[this.Index].ImageID, Erza);
            }
            List<TagInfo> temp = ErzaDB.GetTagsByImageID(Result[this.Index].ImageID, this.Erza);
            this.Tags = new BindingList<TagInfo>(temp.OrderBy(tag => tag.Tag).ToList());
            this.listBox1.DataSource = this.Tags;
        }

        private void AddTag_button_Click(object sender, EventArgs e)
        {
            AddTagForm form = new AddTagForm();
            form.Erza = this.Erza;
            /*if(this.Tags_Cache == null) 
            {
                this.Tags_Cache = ErzaDB.GetAllTags(Erza);
            }
            form.Tags = this.Tags_Cache;*/
            if (form.ShowDialog() == DialogResult.OK)
            {
                Result[this.Index].AddTags(form.NewTags);
                ErzaDB.LoadImageToErza(Result[this.Index], Erza);
                //ErzaDB.AddTagToImage(Result[this.Index].ImageID, form.NewTag, Erza);
                List<TagInfo> temp = ErzaDB.GetTagsByImageID(Result[this.Index].ImageID, this.Erza);
                this.Tags = new BindingList<TagInfo>(temp.OrderBy(tag => tag.Tag).ToList());
                this.listBox1.DataSource = this.Tags;
            }
        }
        private void DeleteImage(int Index)
        {
            ErzaDB.DeleteImage(Result[Index].ImageID, Form1.Erza);
            //File.Delete(Form1.Result[Index].FilePath);
            RecybleBin.Send(Result[Index].FilePath);
            Result.RemoveAt(Index);
            ResultChanged = true;
        }
        private void ViewImageForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (fs != null) { fs.Close(); }
            try
            {
                this.pictureBox1.Image.Dispose();
            }
            catch { }
        }

        private void add_to_favorited_button_Click(object sender, EventArgs e)
        {
            if (Result[Index].Favorited)
            {
                ErzaDB.SetImageFavorit(Result[Index].ImageID, false, Form1.Erza);
                add_to_favorited_button.Image = (System.Drawing.Image)Resources.ResourceManager.GetObject("not_favorited");
                Result[Index].Favorited = false;
            }
            else
            {
                ErzaDB.SetImageFavorit(Result[Index].ImageID, true, Form1.Erza);
                add_to_favorited_button.Image = (System.Drawing.Image)Resources.ResourceManager.GetObject("favourite");
                Result[Index].Favorited = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string modelPath = Settings.Default.TaggerModel;
            string csvPath = Settings.Default.SelectedTags;
            string imagePath = Result[Index].FilePath;
            if (!File.Exists(modelPath) || !File.Exists(csvPath) || !File.Exists(imagePath))
            {
                //Console.WriteLine("Please ensure model.onnx, selected_tags.csv, and input.jpg exist.");
                MessageBox.Show("Please ensure model.onnx, selected_tags.csv, and input.jpg exist.");
                return;
            }

            // 1. Load the tag names mapping from the CSV file
            //Console.WriteLine("Loading tags CSV...");
            var tags = LoadTags(csvPath);

            // 2. Preprocess the image
            //Console.WriteLine("Preprocessing image...");
            float[] inputData = PreprocessImage(imagePath);

            // 3. Prepare the ONNX Tensor Input
            var inputMeta = new List<NamedOnnxValue>
            {
                //NamedOnnxValue.CreateFromTensor("input_1:0", new DenseTensor<float>(inputData, new[] { 1, ImageSize, ImageSize, 3 }))
                NamedOnnxValue.CreateFromTensor("input", new DenseTensor<float>(inputData, new[] { 1, ImageSize, ImageSize, 3 }))
            };

            // 4. Run Inference via ONNX Runtime
            //Console.WriteLine("Running inference...");
            using var session = new InferenceSession(modelPath);
            //int gpuDeviceId = 0; // The GPU device ID to execute on
            //using var gpuSessionOptoins = SessionOptions.MakeSessionOptionWithCudaProvider(gpuDeviceId);
            //using var session = new InferenceSession(modelPath, gpuSessionOptoins);

            /*foreach (var input in session.InputMetadata)
            {
                Console.WriteLine($"Expected Input Name: {input.Key}");
            }*/
            using var results = session.Run(inputMeta);

            // Get output data (typically named "output_1" or the first item)
            var outputTensor = results.First().AsTensor<float>();
            float[] probabilities = outputTensor.ToArray();

            // 5. Match results with tags and filter by threshold
            //var predictedTags = new List<(string Tag, float Confidence, int Category)>();
            List<string> temp = new List<string>();
            for (int i = 0; i < probabilities.Length; i++)
            {
                if (probabilities[i] >= Threshold && i < tags.Count)
                {
                    //predictedTags.Add((tags[i].Name, probabilities[i], tags[i].Category));
                    temp.Add(tags[i].Name);
                }
            }
            Result[this.Index].AddTags(temp.ToList());
            Result[this.Index].Tags = Result[this.Index].Tags.Distinct().ToList();
            ErzaDB.LoadImageToErza(Result[this.Index], Erza);
            //ErzaDB.AddTagToImage(Result[this.Index].ImageID, form.NewTag, Erza);
            List<TagInfo> temp2 = ErzaDB.GetTagsByImageID(Result[this.Index].ImageID, this.Erza);
            this.Tags = new BindingList<TagInfo>(temp2.OrderBy(tag => tag.Tag).ToList());
            this.listBox1.DataSource = this.Tags;
            // 6. Display categorized results
            //Console.WriteLine("\n--- Inference Results ---");

            // Category 0: General tags, Category 4: Character tags, Category 9: Rating tags
            //var ratings = predictedTags.Where(t => t.Category == 9).OrderByDescending(t => t.Confidence);
            //var characters = predictedTags.Where(t => t.Category == 4).OrderByDescending(t => t.Confidence);
            //var general = predictedTags.Where(t => t.Category == 0).OrderByDescending(t => t.Confidence);

            //Console.WriteLine("\n[Ratings]");
            //foreach (var r in ratings) Console.WriteLine($"  {r.Tag}: {r.Confidence:P2}");

            //Console.WriteLine("\n[Characters]");
            //foreach (var c in characters) Console.WriteLine($"  {c.Tag}: {c.Confidence:P2}");

            //Console.WriteLine("\n[General Tags]");
            //foreach (var g in general) Console.WriteLine($"  {g.Tag}: {g.Confidence:P2}");
            //foreach (var g in predictedTags) Console.Write($"{g.Tag}, ");
        }
        private float[] PreprocessImage(string path)
        {
            // Load using OpenCvSharp
            using Mat src = Cv2.ImRead(path, ImreadModes.Color);
            if (src.Empty()) throw new Exception("Could not load image.");

            // Resize to 448x448
            using Mat resized = new Mat();
            Cv2.Resize(src, resized, new OpenCvSharp.Size(ImageSize, ImageSize), 0, 0, InterpolationFlags.Cubic);

            // Convert BGR (OpenCV Default) to RGB
            using Mat rgb = new Mat();
            Cv2.CvtColor(resized, rgb, ColorConversionCodes.BGR2RGB);

            // Convert byte array to float array normalized to 0.0f - 255.0f (WD models don't use 0-1 scaling)
            // Format: NHWC -> [1, 448, 448, 3]
            float[] floatBuffer = new float[ImageSize * ImageSize * 3];
            int index = 0;

            for (int y = 0; y < ImageSize; y++)
            {
                for (int x = 0; x < ImageSize; x++)
                {
                    Vec3b pixel = rgb.At<Vec3b>(y, x);
                    floatBuffer[index++] = pixel.Item0; // R
                    floatBuffer[index++] = pixel.Item1; // G
                    floatBuffer[index++] = pixel.Item2; // B
                }
            }

            return floatBuffer;
        }

        private List<(string Name, int Category)> LoadTags(string csvPath)
        {
            var tagsList = new List<(string Name, int Category)>();
            var lines = File.ReadAllLines(csvPath);

            // Skip the header (tag_id, name, category, count)
            foreach (var line in lines.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                // Simple splitting logic. Be aware some tags may contain commas, but Danbooru tags use underscores.
                var parts = line.Split(',');
                if (parts.Length >= 3)
                {
                    string name = parts[1].Trim('"');
                    int category = int.Parse(parts[2]);
                    tagsList.Add((name, category));
                }
            }
            return tagsList;
        }
    }
}
