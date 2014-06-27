/* Copyright © Macsim Belous 2012 */
/* This file is part of Anew.

    Foobar is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Foobar is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Foobar.  If not, see <http://www.gnu.org/licenses/>.*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
//using Microsoft.VisualBasic;
using System.Drawing.Imaging;

namespace Anew
{
    public partial class Form1 : Form
    {
        private string image_info;
        private List<ulong> id_array;
        private int id_array_index;
        private ulong current_id;
        //private ulong prev_id;
        private byte[] hash;
        private string tags;
        private List<string> tags_array;
        private string file;
        private bool is_changed = false;
        public bool new_only = true;
        public bool non_tag = true;
        public bool all_new = true;
        public string tag;
        public Form1()
        {
            InitializeComponent();
        }
        /*private void LoadImage()
        {
            using (SQLiteConnection connection = new SQLiteConnection(Anew.Properties.Settings.Default.ConnectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.CommandText = "select * from hash_tags where (is_deleted = \"false\") AND ([file_name]  IS NOT NULL) AND (tags = \"\" OR tags IS NULL) LIMIT 1";
                    //command.Parameters.Add("hash", DbType.Binary, 16).Value = il[i2].hash;
                    command.Connection = connection;
                    SQLiteDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        this.current_id = System.Convert.ToUInt64(reader["id"]);
                        this.hash = (byte[])reader["hash"];
                        this.tags = System.Convert.ToString(reader["tags"]);
                        this.file = System.Convert.ToString(reader["file_name"]);
                    }
                    reader.Close();
                }
            }
            this.tags_array = new List<string>();
            string[] temp = this.tags.Split(' ');
            for(int index = 0; index < temp.Length; index++)
            {
                if (temp[index].Length <= 0) { continue; } else { this.tags_array.Add(temp[index]); }
            }
            try
            {
                this.pictureBox1.Load(this.file);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //this.Text = "Anew - " + this.file;
            if (this.tags_array.IndexOf("bdsm") >= 0) { this.bdsm_checkBox.Checked = true; } else { this.bdsm_checkBox.Checked = false; }
            if (this.tags_array.IndexOf("guro") >= 0) { this.guro_checkBox.Checked = true; } else { this.guro_checkBox.Checked = false; }
            if (this.tags_array.IndexOf("bestiality") >= 0) { this.bestiality_checkBox.Checked = true; } else { this.bestiality_checkBox.Checked = false; }
            if (this.tags_array.IndexOf("trap") >= 0) { this.trap_checkBox.Checked = true; } else { this.trap_checkBox.Checked = false; }
            if (this.tags_array.IndexOf("futanary") >= 0) { this.futanary_checkBox.Checked = true; } else { this.futanary_checkBox.Checked = false; }
            if (this.tags_array.IndexOf("yuri") >= 0) { this.yuri_checkBox.Checked = true; } else { this.yuri_checkBox.Checked = false; }
            this.is_changed = false;
        }*/
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
        private void LoadImage(ulong id)
        {
            using (SQLiteConnection connection = new SQLiteConnection(Anew.Properties.Settings.Default.ConnectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.CommandText = "select * from hash_tags where id = @id";
                    command.Parameters.Add("id", DbType.UInt64).Value = id;
                    command.Connection = connection;
                    SQLiteDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        this.current_id = System.Convert.ToUInt64(reader["id"]);
                        this.hash = (byte[])reader["hash"];
                        this.tags = System.Convert.ToString(reader["tags"]);
                        this.file = System.Convert.ToString(reader["file_name"]);
                    }
                    reader.Close();
                }
            }
            this.tags_array = new List<string>();
            string[] temp = this.tags.Split(' ');
            for (int index = 0; index < temp.Length; index++)
            {
                if (temp[index].Length <= 0) { continue; } else { this.tags_array.Add(temp[index]); }
            }
            try
            {
                //this.pictureBox1.Load(this.file);
                using (FileStream stream = new FileStream(this.file, FileMode.Open, FileAccess.Read))
                {
                    //this.pictureBox1.Image = Image.FromStream(stream);
                    Image img = Image.FromStream(stream);
                    this.image_info = "Размер: " + stream.Length.ToString() + " Формат: " + GetImageFormat(img);
                    FrameDimension dimension =new FrameDimension(img.FrameDimensionsList[0]);
                    int frameCount = img.GetFrameCount(dimension);
                    if (frameCount > 1)
                    {
                        img.SelectActiveFrame(dimension, 0);
                        MemoryStream ms = new MemoryStream();
                        img.Save(ms, ImageFormat.Bmp);
                        Image outImg = Image.FromStream(ms);
                        this.pictureBox1.Image = outImg;
                    }
                    else
                    {
                        this.pictureBox1.Image = img;
                    }

                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                this.pictureBox1.Image = Anew.Properties.Resources.NoImage;
                this.image_info = ex.Message;
            }
            //this.Text = "Anew - " + this.file;
            if (this.tags_array.IndexOf("bdsm") >= 0) { this.bdsm_checkBox.Checked = true; } else { this.bdsm_checkBox.Checked = false; }
            if (this.tags_array.IndexOf("guro") >= 0) { this.guro_checkBox.Checked = true; } else { this.guro_checkBox.Checked = false; }
            if (this.tags_array.IndexOf("bestiality") >= 0) { this.bestiality_checkBox.Checked = true; } else { this.bestiality_checkBox.Checked = false; }
            if (this.tags_array.IndexOf("trap") >= 0) { this.trap_checkBox.Checked = true; } else { this.trap_checkBox.Checked = false; }
            if (this.tags_array.IndexOf("futanary") >= 0) { this.futanary_checkBox.Checked = true; } else { this.futanary_checkBox.Checked = false; }
            if (this.tags_array.IndexOf("yuri") >= 0) { this.yuri_checkBox.Checked = true; } else { this.yuri_checkBox.Checked = false; }
            if (this.tags_array.IndexOf("hentai") >= 0) { this.hentai_checkBox.Checked = true; } else { this.hentai_checkBox.Checked = false; }
            if (this.tags_array.IndexOf("wallpaper") >= 0) { this.wallpaper_checkBox.Checked = true; } else { this.wallpaper_checkBox.Checked = false; }
            this.is_changed = false;
            this.toolStripStatusLabel1.Text = "Изображений " + (this.id_array_index + 1).ToString() + "\\" + this.id_array.Count.ToString() + " Разрешение " + this.pictureBox1.Image.Width.ToString() +"x"+ this.pictureBox1.Image.Height.ToString() + " " + this.image_info + " Теги: " + this.tags;
        }
        private void UpdateImage()
        {
            using (SQLiteConnection connection = new SQLiteConnection(Anew.Properties.Settings.Default.ConnectionString))
            {
                connection.Open();
                using (SQLiteCommand update_command = new SQLiteCommand(connection))
                {
                    update_command.CommandText = "UPDATE hash_tags SET tags = @tags, is_new = @is_new WHERE id = @id";
                    update_command.Parameters.Add("id", DbType.UInt64).Value = this.current_id;
                    update_command.Parameters.Add("is_new", DbType.Boolean).Value = false;
                    if (this.tags_array.Count > 0)
                    {
                        string ts = String.Empty;
                        for (int i = 0; i < tags_array.Count; i++)
                        {
                            if (i > 0)
                            {
                                ts = ts + " ";
                            }
                            ts = ts + tags_array[i];
                        }
                        update_command.Parameters.Add("tags", DbType.String, 4000).Value = ts;
                    }
                    else
                    {
                        update_command.Parameters.Add("tags", DbType.String, 4000).Value = null;
                    }
                    update_command.ExecuteNonQuery();
                }
            }
        }
        private void DeleteImage(ulong item_id)
        {
            using (SQLiteConnection connection = new SQLiteConnection(Anew.Properties.Settings.Default.ConnectionString))
            {
                connection.Open();
                using (SQLiteCommand update_command = new SQLiteCommand(connection))
                {
                    update_command.CommandText = "UPDATE hash_tags SET is_deleted = @is_deleted, tags = @tags, file_name = @file_name, is_new = @is_new WHERE id = @id";
                    update_command.Parameters.Add("id", DbType.UInt64).Value = item_id;
                    update_command.Parameters.Add("is_deleted", DbType.Boolean).Value = true;
                    update_command.Parameters.Add("tags", DbType.String, 4000).Value = null;
                    update_command.Parameters.Add("file_name", DbType.String, 4000).Value = null;
                    update_command.Parameters.Add("is_new", DbType.Boolean).Value = false;
                    update_command.ExecuteNonQuery();
                }
            }
            try
            {
                //System.IO.File.Delete(this.file);
                //System.IO.FileInfo fi = new System.IO.FileInfo(this.file);
                //fi.Delete();
                //FileIO.FileSystem.DeleteDirectory("file.txt", FileIO.UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                RecybleBin.Send(this.file);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            GetImageIDs();
            if (this.id_array.Count > 0)
            {
                this.id_array_index = 0;
                LoadImage(this.id_array[this.id_array_index]);
            }
            else
            {
                this.toolStripStatusLabel1.Text = "Изображений 0";
            }
        }

        private void delete_button_Click(object sender, EventArgs e)
        {
            delete();
        }

        private void next_button_Click(object sender, EventArgs e)
        {
            if (this.is_changed)
            {
                UpdateImage();
                //this.prev_id = this.current_id;
                this.id_array_index++;
                if (this.id_array_index >= this.id_array.Count)
                {
                    MessageBox.Show("Список изображений закончелся.");
                    this.id_array_index--;
                }
                else
                {
                    LoadImage(this.id_array[this.id_array_index]);
                }
            }
            else
            {
                using (SQLiteConnection connection = new SQLiteConnection(Anew.Properties.Settings.Default.ConnectionString))
                {
                    connection.Open();
                    using (SQLiteCommand update_command = new SQLiteCommand(connection))
                    {
                        update_command.CommandText = "UPDATE hash_tags SET is_new = @is_new WHERE id = @id";
                        update_command.Parameters.Add("id", DbType.UInt64).Value = this.current_id;
                        update_command.Parameters.Add("is_new", DbType.Boolean).Value = false;
                        update_command.ExecuteNonQuery();
                    }
                }
                this.id_array_index++;
                if (this.id_array_index >= this.id_array.Count)
                {
                    MessageBox.Show("Список изображений закончелся.");
                    this.id_array_index--;
                }
                else
                {
                    LoadImage(this.id_array[this.id_array_index]);
                }
            }
            
        }
        private void tags_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            this.is_changed = true;
            CheckBox temp = (CheckBox)sender;
            if (temp.Checked)
            {
                if (this.tags_array.IndexOf((string)temp.Tag) < 0)
                {
                    this.tags_array.Add((string)temp.Tag);
                }
                temp.BackColor = System.Drawing.SystemColors.ActiveCaption;
            }
            else
            {
                if (this.tags_array.IndexOf((string)temp.Tag) >= 0)
                {
                    this.tags_array.Remove((string)temp.Tag);
                }
                temp.BackColor = System.Drawing.SystemColors.Control;
            }
        }

        private void prev_button_Click(object sender, EventArgs e)
        {
            if (this.is_changed)
            {
                UpdateImage();
                this.id_array_index--;
                if (this.id_array_index < 0)
                {
                    MessageBox.Show("Список изображений закончелся.");
                    this.id_array_index = 0;
                }
                else
                {
                    LoadImage(this.id_array[this.id_array_index]);
                }
            }
            else
            {
                this.id_array_index--;
                if (this.id_array_index < 0)
                {
                    MessageBox.Show("Список изображений закончелся.");
                    this.id_array_index = 0;
                }
                else
                {
                    LoadImage(this.id_array[this.id_array_index]);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ConfigForm cf = new ConfigForm();
            cf.tag = this.tag;
            cf.all_new = this.all_new;
            cf.new_only = this.new_only;
            cf.non_tag = this.non_tag;
            if (cf.ShowDialog() == DialogResult.OK)
            {
                this.tag = cf.tag;
                this.new_only = cf.new_only;
                this.non_tag = cf.non_tag;
                this.all_new = cf.all_new;
                GetImageIDs();
                if (this.id_array.Count > 0)
                {
                    this.id_array_index = 0;
                    LoadImage(this.id_array[this.id_array_index]);
                }
            }
        }
        private void GetImageIDs()
        {
            this.id_array = new List<ulong>();
            if (this.all_new)
            {
                using (SQLiteConnection connection = new SQLiteConnection(Anew.Properties.Settings.Default.ConnectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand())
                    {
                        command.CommandText = "select id from hash_tags where (is_deleted = @is_deleted) AND ([file_name]  IS NOT NULL) AND (is_new = @is_new)";
                        command.Parameters.Add("is_deleted", DbType.Boolean).Value = false;
                        command.Parameters.Add("is_new", DbType.Boolean).Value = true;
                        command.Connection = connection;
                        SQLiteDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            this.id_array.Add(System.Convert.ToUInt64(reader["id"]));
                        }
                        reader.Close();
                    }
                }
            }
            else
            {
                if (this.non_tag)
                {
                    if (this.new_only) 
                    {
                        using (SQLiteConnection connection = new SQLiteConnection(Anew.Properties.Settings.Default.ConnectionString))
                        {
                            connection.Open();
                            using (SQLiteCommand command = new SQLiteCommand())
                            {
                                command.CommandText = "select id from hash_tags where (is_deleted = @is_deleted) AND ([file_name]  IS NOT NULL) AND (tags = \"\" OR tags IS NULL) AND (is_new = @is_new)";
                                command.Parameters.Add("is_deleted", DbType.Boolean).Value = false;
                                command.Parameters.Add("is_new", DbType.Boolean).Value = true;
                                command.Connection = connection;
                                SQLiteDataReader reader = command.ExecuteReader();
                                while (reader.Read())
                                {
                                    this.id_array.Add(System.Convert.ToUInt64(reader["id"]));
                                }
                                reader.Close();
                            }
                        }
                    }
                    else
                    {
                        using (SQLiteConnection connection = new SQLiteConnection(Anew.Properties.Settings.Default.ConnectionString))
                        {
                            connection.Open();
                            using (SQLiteCommand command = new SQLiteCommand())
                            {
                                command.CommandText = "select id from hash_tags where (is_deleted = @is_deleted) AND ([file_name]  IS NOT NULL) AND (tags = \"\" OR tags IS NULL)";
                                command.Parameters.Add("is_deleted", DbType.Boolean).Value = false;
                                command.Connection = connection;
                                SQLiteDataReader reader = command.ExecuteReader();
                                while (reader.Read())
                                {
                                    this.id_array.Add(System.Convert.ToUInt64(reader["id"]));
                                }
                                reader.Close();
                            }
                        }
                    }
                }
                else
                {
                    if (this.new_only) 
                    {
                        using (SQLiteConnection connection = new SQLiteConnection(Anew.Properties.Settings.Default.ConnectionString))
                        {
                            connection.Open();
                            using (SQLiteCommand command = new SQLiteCommand())
                            {
                                command.CommandText = "select id from hash_tags where (is_deleted = @is_deleted) AND ([file_name]  IS NOT NULL) AND (tags = @tags) AND (is_new = @is_new)";
                                command.Parameters.Add("is_deleted", DbType.Boolean).Value = false;
                                command.Parameters.Add("is_new", DbType.Boolean).Value = true;
                                command.Parameters.Add("tags", DbType.String, 4000).Value = this.tag;
                                command.Connection = connection;
                                SQLiteDataReader reader = command.ExecuteReader();
                                while (reader.Read())
                                {
                                    this.id_array.Add(System.Convert.ToUInt64(reader["id"]));
                                }
                                reader.Close();
                            }
                        }
                    }
                    else
                    {
                        using (SQLiteConnection connection = new SQLiteConnection(Anew.Properties.Settings.Default.ConnectionString))
                        {
                            connection.Open();
                            using (SQLiteCommand command = new SQLiteCommand())
                            {
                                command.CommandText = "select id from hash_tags where (is_deleted = @is_deleted) AND ([file_name]  IS NOT NULL) AND (tags = @tags)";
                                command.Parameters.Add("is_deleted", DbType.Boolean).Value = false;
                                command.Parameters.Add("tags", DbType.String, 4000).Value = this.tag;
                                command.Connection = connection;
                                SQLiteDataReader reader = command.ExecuteReader();
                                while (reader.Read())
                                {
                                    this.id_array.Add(System.Convert.ToUInt64(reader["id"]));
                                }
                                reader.Close();
                            }
                        }
                    }
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                delete();
                e.Handled = true;
            }
            switch (e.KeyCode)
            {
                case Keys.PageDown:
                    NextImage();
                    e.Handled = true;
                    break;
                case Keys.PageUp:
                    PrevImage();
                    e.Handled = true;
                    break;
            }
        }
        private void PrevImage()
        {
            if (this.is_changed)
            {
                UpdateImage();
                this.id_array_index--;
                if (this.id_array_index < 0)
                {
                    MessageBox.Show("Список изображений закончелся.");
                    this.id_array_index = 0;
                }
                else
                {
                    LoadImage(this.id_array[this.id_array_index]);
                }
            }
            else
            {
                this.id_array_index--;
                if (this.id_array_index < 0)
                {
                    MessageBox.Show("Список изображений закончелся.");
                    this.id_array_index = 0;
                }
                else
                {
                    LoadImage(this.id_array[this.id_array_index]);
                }
            }
        }
        private void NextImage()
        {
            if (this.is_changed)
            {
                UpdateImage();
                //this.prev_id = this.current_id;
                this.id_array_index++;
                if (this.id_array_index >= this.id_array.Count)
                {
                    MessageBox.Show("Список изображений закончелся.");
                    this.id_array_index--;
                }
                else
                {
                    LoadImage(this.id_array[this.id_array_index]);
                }
            }
            else
            {
                using (SQLiteConnection connection = new SQLiteConnection(Anew.Properties.Settings.Default.ConnectionString))
                {
                    connection.Open();
                    using (SQLiteCommand update_command = new SQLiteCommand(connection))
                    {
                        update_command.CommandText = "UPDATE hash_tags SET is_new = @is_new WHERE id = @id";
                        update_command.Parameters.Add("id", DbType.UInt64).Value = this.current_id;
                        update_command.Parameters.Add("is_new", DbType.Boolean).Value = false;
                        update_command.ExecuteNonQuery();
                    }
                }
                this.id_array_index++;
                if (this.id_array_index >= this.id_array.Count)
                {
                    MessageBox.Show("Список изображений закончелся.");
                    this.id_array_index--;
                }
                else
                {
                    LoadImage(this.id_array[this.id_array_index]);
                }
            }
        }
        private void delete()
        {
            if (MessageBox.Show("Вы действительно хотите удалить это изображение?", "Подтверждение.", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.pictureBox1.Image.Dispose();
                this.pictureBox1.Image = null;
                DeleteImage(this.id_array[this.id_array_index]);
                this.id_array_index++;
                if (this.id_array_index >= this.id_array.Count)
                {
                    MessageBox.Show("Список изображений закончелся.");
                    this.id_array_index--;
                }
                else
                {
                    LoadImage(this.id_array[this.id_array_index]);
                }
            }
        }

        private void Copy_wallpaper_buttom_Click(object sender, EventArgs e)
        {
            try
            {
                System.IO.File.Copy(this.file, Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\skydrive\\wallpapers\\" + System.IO.Path.GetFileName(this.file));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(this.file);
        }

    }
    public class RecybleBin
    {
        /// <summary>
        /// Possible flags for the SHFileOperation method.
        /// </summary>
        [Flags]
        public enum FileOperationFlags : ushort
        {
            /// <summary>
            /// Do not show a dialog during the process
            /// </summary>
            FOF_SILENT = 0x0004,
            /// <summary>
            /// Do not ask the user to confirm selection
            /// </summary>
            FOF_NOCONFIRMATION = 0x0010,
            /// <summary>
            /// Delete the file to the recycle bin.  (Required flag to send a file to the bin
            /// </summary>
            FOF_ALLOWUNDO = 0x0040,
            /// <summary>
            /// Do not show the names of the files or folders that are being recycled.
            /// </summary>
            FOF_SIMPLEPROGRESS = 0x0100,
            /// <summary>
            /// Surpress errors, if any occur during the process.
            /// </summary>
            FOF_NOERRORUI = 0x0400,
            /// <summary>
            /// Warn if files are too big to fit in the recycle bin and will need
            /// to be deleted completely.
            /// </summary>
            FOF_WANTNUKEWARNING = 0x4000,
        }

        /// <summary>
        /// File Operation Function Type for SHFileOperation
        /// </summary>
        public enum FileOperationType : uint
        {
            /// <summary>
            /// Move the objects
            /// </summary>
            FO_MOVE = 0x0001,
            /// <summary>
            /// Copy the objects
            /// </summary>
            FO_COPY = 0x0002,
            /// <summary>
            /// Delete (or recycle) the objects
            /// </summary>
            FO_DELETE = 0x0003,
            /// <summary>
            /// Rename the object(s)
            /// </summary>
            FO_RENAME = 0x0004,
        }

        /// <summary>
        /// SHFILEOPSTRUCT for SHFileOperation from COM
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        private struct SHFILEOPSTRUCT_x86
        {
            public IntPtr hwnd;
            [MarshalAs(UnmanagedType.U4)]
            public FileOperationType wFunc;
            public string pFrom;
            public string pTo;
            public FileOperationFlags fFlags;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fAnyOperationsAborted;
            public IntPtr hNameMappings;
            public string lpszProgressTitle;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SHFILEOPSTRUCT_x64
        {
            public IntPtr hwnd;
            [MarshalAs(UnmanagedType.U4)]
            public FileOperationType wFunc;
            public string pFrom;
            public string pTo;
            public FileOperationFlags fFlags;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fAnyOperationsAborted;
            public IntPtr hNameMappings;
            public string lpszProgressTitle;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto, EntryPoint = "SHFileOperation")]
        private static extern int SHFileOperation_x86(ref SHFILEOPSTRUCT_x86 FileOp);

        [DllImport("shell32.dll", CharSet = CharSet.Auto, EntryPoint = "SHFileOperation")]
        private static extern int SHFileOperation_x64(ref SHFILEOPSTRUCT_x64 FileOp);

        private static bool IsWOW64Process()
        {
            return IntPtr.Size == 8;
        }

        /// <summary>
        /// Send file to recycle bin
        /// </summary>
        /// <param name="path">Location of directory or file to recycle</param>
        /// <param name="flags">FileOperationFlags to add in addition to FOF_ALLOWUNDO</param>
        public static bool Send(string path, FileOperationFlags flags)
        {
            try
            {
                if (IsWOW64Process())
                {
                    SHFILEOPSTRUCT_x64 fs = new SHFILEOPSTRUCT_x64();
                    fs.wFunc = FileOperationType.FO_DELETE;
                    // important to double-terminate the string.
                    fs.pFrom = path + '\0' + '\0';
                    fs.fFlags = FileOperationFlags.FOF_ALLOWUNDO | flags;
                    SHFileOperation_x64(ref fs);
                }
                else
                {
                    SHFILEOPSTRUCT_x86 fs = new SHFILEOPSTRUCT_x86();
                    fs.wFunc = FileOperationType.FO_DELETE;
                    // important to double-terminate the string.
                    fs.pFrom = path + '\0' + '\0';
                    fs.fFlags = FileOperationFlags.FOF_ALLOWUNDO | flags;
                    SHFileOperation_x86(ref fs);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Send file to recycle bin.  Display dialog, display warning if files are too big to fit (FOF_WANTNUKEWARNING)
        /// </summary>
        /// <param name="path">Location of directory or file to recycle</param>
        public static bool Send(string path)
        {
            return Send(path, FileOperationFlags.FOF_NOCONFIRMATION | FileOperationFlags.FOF_WANTNUKEWARNING);
        }

        /// <summary>
        /// Send file silently to recycle bin.  Surpress dialog, surpress errors, delete if too large.
        /// </summary>
        /// <param name="path">Location of directory or file to recycle</param>
        public static bool SendSilent(string path)
        {
            return Send(path, FileOperationFlags.FOF_NOCONFIRMATION | FileOperationFlags.FOF_NOERRORUI | FileOperationFlags.FOF_SILENT);
        }
    }
}
