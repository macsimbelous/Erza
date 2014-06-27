/* Copyright © Macsim Belous 2012 */
/* This file is part of Moka.

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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Threading;

namespace Moka
{
    public partial class AddImagesSQLiteProgressForm : Form
    {
        public Queue<CImage> imgs;
        bool end = false;
        bool abort = false;
        Thread thread;
        SynchronizationContext synchronizationContext;
        SQLiteConnection connection;
        SQLiteTransaction transaction;
        private int count_is_deleted = 0;
        public AddImagesSQLiteProgressForm()
        {
            InitializeComponent();
        }
        private void LongRunningTask()
        {
            //MessageBox.Show("sdfs");
            connection = new SQLiteConnection(Settings1.Default.ConnectionString);
            connection.Open();
            transaction = connection.BeginTransaction();
            for (; ; )
            {
                if (abort)
                {
                    break;
                }
                //CImage img;
                if (imgs.Count > 0)
                {
                    AddImageDB(imgs.Dequeue());
                    //NewImageDB_sqlite(imgs.Dequeue());
                    synchronizationContext.Post(RefreshProgress, this.progressBar1.Maximum - imgs.Count);
                }
                    else
                    {
                        break;
                    }

                    //NewImageDB_sqlite(img);
            }
            transaction.Commit();
            connection.Close();
            synchronizationContext.Post(EndProgress, null);
        }
        private void RefreshProgress(object progress)
        {
            progressBar1.Value = (int)progress;
            this.label1.Text = "Изображений добавлено: " + ((int)progress).ToString();
        }
        private void EndProgress(object status)
        {
            //transaction.Commit();
            end = true;
            if (abort)
            {
                this.DialogResult = System.Windows.Forms.DialogResult.Abort;
            }
            else
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
            this.Close();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AddImagesSQLiteProgressForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (end == true)
            {
                return;
            }
            else
            {
                //CancelForm form = new CancelForm();
                if (MessageBox.Show("Прервать операцию?", "Zapros", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    if (end == true)
                    {
                        return;
                    }
                    else
                    {
                        abort = true;
                    }
                }
                else
                {
                    if (end == false)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }
        private void AddImageDB(CImage img)
        {
            //Console.WriteLine("Обрабатываю хэш " + il[i2].hash_str + " (" + (i2 + 1).ToString() + "/" + il.Count.ToString() + ")");
            using (SQLiteCommand command = new SQLiteCommand())
            {
                command.CommandText = "select * from hash_tags where hash = @hash";
                command.Parameters.AddWithValue("hash", img.hash);
                command.Connection = connection;
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    if (System.Convert.ToBoolean(reader["is_deleted"]))
                    {
                        System.IO.Directory.CreateDirectory(Settings1.Default.is_deleted_dir);
                        System.IO.File.Move(img.file, Settings1.Default.is_deleted_dir + "\\" + System.IO.Path.GetFileName(img.file));
                        this.count_is_deleted++;
                        reader.Close();
                        return;
                    }
                    string tags = System.Convert.ToString(reader["tags"]);
                    reader.Close();
                    List<string> t = new List<string>(tags.Split(' '));
                    
                    for (int i = 0; i < img.tags.Count; i++)
                    {
                        if (t.IndexOf(img.tags[i]) < 0)
                        {
                            t.Add(img.tags[i]);
                        }
                    }
                    List<string> t2 = new List<string>();
                    for (int d = 0; d < t.Count; d++)
                    {
                        if (t[d].Length > 0) { t2.Add(t[d]); }
                    }
                    tags = String.Empty;
                    for (int i4 = 0; i4 < t2.Count; i4++)
                    {
                        if (i4 > 0)
                        {
                            tags = tags + " ";
                        }
                        tags = tags + t2[i4];
                    }
                    if (tags.Length == 0) { tags = null; }
                    using (SQLiteCommand update_command = new SQLiteCommand(connection))
                    {
                        if (img.file != null)
                        {
                            update_command.CommandText = "UPDATE hash_tags SET tags = @tags, file_name = @file_name WHERE hash = @hash";
                            update_command.Parameters.AddWithValue("file_name", img.file);
                            update_command.Parameters.AddWithValue("hash", img.hash);
                            update_command.Parameters.AddWithValue("tags", tags);
                            update_command.ExecuteNonQuery();
                        }
                        else
                        {
                            update_command.CommandText = "UPDATE hash_tags SET tags = @tags WHERE hash = @hash";
                            update_command.Parameters.AddWithValue("hash", img.hash);
                            update_command.Parameters.AddWithValue("tags", tags);
                            update_command.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    reader.Close();
                    using (SQLiteCommand insert_command = new SQLiteCommand(connection))
                    {
                        string tags = img.tags_string;
                        if (tags.Length == 0) { tags = null; }
                        if (img.file != null)
                        {
                            insert_command.CommandText = "insert into hash_tags (hash, tags, file_name, is_new, is_deleted) values (@hash, @tags, @file_name, @is_new, @is_deleted)";
                            insert_command.Parameters.AddWithValue("hash", img.hash);
                            insert_command.Parameters.AddWithValue("tags", tags);
                            insert_command.Parameters.AddWithValue("file_name", img.file);
                            insert_command.Parameters.AddWithValue("is_new", true);
                            insert_command.Parameters.AddWithValue("is_deleted", false);
                            insert_command.ExecuteNonQuery();
                        }
                        else
                        {
                            insert_command.CommandText = "insert into hash_tags (hash, tags, is_new, is_deleted) values (@hash, @tags, @is_new, @is_deleted)";
                            insert_command.Parameters.AddWithValue("hash", img.hash);
                            insert_command.Parameters.AddWithValue("tags", tags);
                            insert_command.Parameters.AddWithValue("is_new", true);
                            insert_command.Parameters.AddWithValue("is_deleted", false);
                            insert_command.Parameters.AddWithValue("file_name", null);
                            insert_command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
        private void AddImagesSQLiteProgressForm_Load(object sender, EventArgs e)
        {
            //MessageBox.Show("sdfs");
            
            this.progressBar1.Maximum = imgs.Count;
            this.progressBar1.Minimum = 0;
            this.progressBar1.Step = 1;
            this.progressBar1.Value = 0;
            synchronizationContext = SynchronizationContext.Current;
            //connection = new SQLiteConnection(Settings1.Default.ConnectionString);
            //transaction = connection.BeginTransaction();
            thread = new Thread(LongRunningTask);
            thread.Start();
            
        }
        #region SQLite
        private void UpdateImageDB_sqlite(long image_id, CImage new_image)
        {
            string update = "UPDATE images SET file_name = @file_name WHERE image_id = @image_id";
            using (SQLiteCommand command = new SQLiteCommand(update, connection))
            {
                command.Parameters.Add("image_id", DbType.Int64).Value = image_id;
                command.Parameters.Add("file_name", DbType.String, 4000, "file_name").Value = new_image.file;
                command.ExecuteNonQuery();
            }
        }
        private void add_image_tags_sqlite(long tag_id, long image_id)
        {
            using (SQLiteCommand command = new SQLiteCommand("SELECT tag_id, image_id FROM image_tags WHERE (image_id = @image_id) AND (tag_id = @tag_id)", connection))
            {
                command.Parameters.Add("image_id", DbType.Int64).Value = image_id;
                command.Parameters.Add("tag_id", DbType.Int64).Value = tag_id;
                object o = command.ExecuteScalar();
                if (o == null)
                {
                    string ins = "INSERT INTO image_tags (tag_id, image_id) VALUES ( @tag_id, @image_id)";
                    using (SQLiteCommand ins_command = new SQLiteCommand(ins, connection))
                    {
                        ins_command.Parameters.Add("tag_id", DbType.Int64).Value = tag_id;
                        ins_command.Parameters.Add("image_id", DbType.Int64).Value = image_id;
                        ins_command.ExecuteNonQuery();
                    }
                }
            }
            return;
        }
        private long AddTagDB_sqlite(string tag)
        {
            long t;
            using (SQLiteCommand command = new SQLiteCommand("SELECT tag_id FROM tags WHERE (tag = @tag)", connection))
            {
                command.Parameters.Add("tag", DbType.StringFixedLength, 128).Value = tag;
                object o = command.ExecuteScalar();
                if (o != null)
                {
                    t = System.Convert.ToInt64(o);
                }
                else
                {
                    string ins = "INSERT INTO tags (tag) VALUES (@tag); select last_insert_rowid();";
                    using (SQLiteCommand ins_command = new SQLiteCommand(ins, connection))
                    {
                        ins_command.Parameters.Add("tag", DbType.StringFixedLength, 128).Value = tag;
                        t = System.Convert.ToInt64(ins_command.ExecuteScalar());
                    }
                }
            }
            return t;
        }
        private void NewImageDB_sqlite(CImage img)
        {
            using (SQLiteCommand command = new SQLiteCommand("SELECT image_id FROM images WHERE (hash = @hash)", connection))
            {
                command.Parameters.Add("hash", DbType.Binary).Value = img.hash;
                object o = command.ExecuteScalar();
                if (o != null)
                {
                    long image_id = System.Convert.ToInt64(o);
                    if (img.file != null)
                    {
                        UpdateImageDB_sqlite(image_id, img);
                    }
                    for (int i = 0; i < img.tags.Count; i++)
                    {
                        long tag_id = AddTagDB_sqlite(img.tags[i]);
                        add_image_tags_sqlite(tag_id, image_id);
                    }
                }
                else
                {
                    string ins = "INSERT INTO images (hash, file_name) VALUES (@hash, @file_name); select last_insert_rowid();";
                    using (SQLiteCommand ins_command = new SQLiteCommand(ins, connection))
                    {
                        ins_command.Parameters.Add("hash", DbType.Binary).Value = img.hash;
                        ins_command.Parameters.Add("file_name", DbType.String, 4000, "file_name").Value = img.file;
                        long image_id = System.Convert.ToInt64(ins_command.ExecuteScalar());
                        for (int i = 0; i < img.tags.Count; i++)
                        {
                            long tag_id = AddTagDB_sqlite(img.tags[i]);
                            add_image_tags_sqlite(tag_id, image_id);
                        }
                    }
                }
            }
            return;
        }
        #endregion
    }
}
