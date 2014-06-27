/* Copyright © Macsim Belous 2012 */
/* This file is part of ErzaLib.

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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;

namespace ErzaLib
{
    public class ImagesDB
    {
        private string ConnectionString = null;
        public delegate void ProgressCallBackT(string hash, int Count, int Total);
        public ProgressCallBackT ProgressCallBack = null;
        public ImagesDB(string ConnectionString)
        {
            this.ConnectionString = ConnectionString;
        }
        public bool AddImage(ImageInfo img)
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                AddImagesWithConnection(img, connection);
            }
            return true;
        }
        public void AddImages(List<ImageInfo> imgs)
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                //DateTime start = DateTime.Now;
                connection.Open();
                SQLiteTransaction transact = connection.BeginTransaction();
                for (int i2 = 0; i2 < imgs.Count; i2++)
                {
                    //Console.Write("Обрабатываю хэш {0} ({1}/{2})\r", imgs[i2].GetHashString(), (i2 + 1), imgs.Count);
                    ProgressCallBack(imgs[i2].GetHashString(), (i2 + 1), imgs.Count);
                    AddImagesWithConnection(imgs[i2], connection);
                }
                transact.Commit();
                //DateTime finish = DateTime.Now;
                //Console.WriteLine("\nХэшей добавлено: " + imgs.Count.ToString() + " за: " + (finish - start).TotalSeconds.ToString("0.00") + " секунд (" + (imgs.Count / (finish - start).TotalSeconds).ToString("0.00") + " в секунду)");
                //Console.WriteLine("\nХэшей добавлено: {0} за: {1} секунд ({2} в секунду)", imgs.Count.ToString(), (finish - start).TotalSeconds.ToString("0.00"), (imgs.Count / (finish - start).TotalSeconds).ToString("0.00"));
            }
        }
        private bool AddImagesWithConnection(ImageInfo img, SQLiteConnection connection)
        {
            using (SQLiteCommand command = new SQLiteCommand())
            {
                command.CommandText = "select * from hash_tags where hash = @hash";
                command.Parameters.AddWithValue("hash", img.hash);
                command.Connection = connection;
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    ulong id;
                    if (System.Convert.ToBoolean(reader["is_deleted"]))
                    {
                        reader.Close();
                        return false;
                    }
                    else
                    {
                        if (!Convert.IsDBNull(reader["tags"]))
                        {
                            img.AddStringOfTags(System.Convert.ToString(reader["tags"]));
                        }
                        id = System.Convert.ToUInt64(reader["id"]);
                        reader.Close();
                    }
                    if (img.file == null)
                    {
                        using (SQLiteCommand update_command = new SQLiteCommand(connection))
                        {
                            update_command.CommandText = "UPDATE hash_tags SET tags = @tags WHERE id = @id";
                            update_command.Parameters.AddWithValue("id", id);
                            update_command.Parameters.AddWithValue("tags", img.GetStringOfTags());
                            update_command.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        using (SQLiteCommand update_command = new SQLiteCommand(connection))
                        {
                            update_command.CommandText = "UPDATE hash_tags SET tags = @tags, file_name = @file_name WHERE id = @id";
                            update_command.Parameters.AddWithValue("id", id);
                            update_command.Parameters.AddWithValue("tags", img.GetStringOfTags());
                            update_command.Parameters.AddWithValue("file_name", img.file);
                            update_command.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    if (img.file == null)
                    {
                        using (SQLiteCommand insert_command = new SQLiteCommand(connection))
                        {
                            insert_command.CommandText = "insert into hash_tags (hash, tags, is_new, is_deleted) values (@hash, @tags, @is_new, @is_deleted)";
                            insert_command.Parameters.AddWithValue("hash", img.hash);
                            insert_command.Parameters.AddWithValue("tags", img.GetStringOfTags());
                            insert_command.Parameters.AddWithValue("is_new", true);
                            insert_command.Parameters.AddWithValue("is_deleted", false);
                            insert_command.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        using (SQLiteCommand insert_command = new SQLiteCommand(connection))
                        {
                            insert_command.CommandText = "insert into hash_tags (hash, tags, is_new, is_deleted, file_name) values (@hash, @tags, @is_new, @is_deleted, @file_name)";
                            insert_command.Parameters.AddWithValue("hash", img.hash);
                            insert_command.Parameters.AddWithValue("tags", img.GetStringOfTags());
                            insert_command.Parameters.AddWithValue("is_new", true);
                            insert_command.Parameters.AddWithValue("is_deleted", false);
                            insert_command.Parameters.AddWithValue("file_name", img.file);
                            insert_command.ExecuteNonQuery();
                        }
                    }
                }
            }
            return true;
        }
        public ImageInfo ExistImage(byte[] hash)
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.CommandText = "select * from hash_tags where hash = @hash";
                    command.Parameters.AddWithValue("hash", hash);
                    command.Connection = connection;
                    SQLiteDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                            ImageInfo img = new ImageInfo();
                            if (!Convert.IsDBNull(reader["tags"]))
                            {
                                img.AddStringOfTags(System.Convert.ToString(reader["tags"]));
                            }
                            if (!Convert.IsDBNull(reader["file_name"]))
                            {
                                img.file = System.Convert.ToString(reader["file_name"]);
                            }
                            img.db_id = System.Convert.ToUInt64(reader["id"]);
                            img.is_deleted = System.Convert.ToBoolean(reader["is_deleted"]);
                            img.is_new = System.Convert.ToBoolean(reader["is_new"]);
                            reader.Close();
                            return img;
                     }
                    else
                    {
                        return null;
                    }
                }
            }
        }
        public void DeleteImage(byte[] hash)
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (SQLiteCommand update_command = new SQLiteCommand(connection))
                {
                    update_command.CommandText = "UPDATE hash_tags SET is_deleted = @is_deleted, tags = @tags, file_name = @file_name, is_new = @is_new WHERE hash = @hash";
                    update_command.Parameters.AddWithValue("hash", hash);
                    update_command.Parameters.AddWithValue("is_deleted", true);
                    update_command.Parameters.AddWithValue("tags", null);
                    update_command.Parameters.AddWithValue("file_name", null);
                    update_command.Parameters.AddWithValue("is_new", false);
                    update_command.ExecuteNonQuery();
                }
            }
        }
        public void VipeImageDB(byte[] hash)
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (SQLiteCommand update_command = new SQLiteCommand(connection))
                {
                    update_command.CommandText = "DELETE FROM hash_tags WHERE hash = @hash";
                    update_command.Parameters.AddWithValue("hash", hash);
                    update_command.ExecuteNonQuery();
                }
            }
        }
        public List<ImageInfo> GetAllImagesWithOutTags()
        {
            List<ImageInfo> il = new List<ImageInfo>();
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                SQLiteTransaction transact = connection.BeginTransaction();
                ulong count_rows;
                //Определяем число записей
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.CommandText = "select count(*) from hash_tags where (is_deleted = 0) AND (tags IS NULL)";
                    command.Connection = connection;
                    count_rows = System.Convert.ToUInt64(command.ExecuteScalar());
                }
                //Считываем записи
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.CommandText = "select hash from hash_tags where (is_deleted = 0) AND (tags IS NULL)";
                    command.Connection = connection;
                    SQLiteDataReader reader = command.ExecuteReader();
                    int count = 0;
                    while (reader.Read())
                    {
                        ImageInfo img = new ImageInfo();
                        img.hash = (byte[])reader["hash"];
                        il.Add(img);
                        count++;
                        //Console.Write("Считано: {0} из {1}\r", count, count_rows);
                        ProgressCallBack(null, count, (int)count_rows);
                    }
                }
                transact.Commit();
            }
            return il;
        }
        public void UpdateTagsForImage(ImageInfo img)
        {
            //DateTime start = DateTime.Now;
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (SQLiteCommand update_command = new SQLiteCommand(connection))
                {
                    update_command.CommandText = "UPDATE hash_tags SET tags = @tags WHERE hash = @hash";
                    update_command.Parameters.AddWithValue("hash", img.hash);
                    update_command.Parameters.AddWithValue("tags", img.GetStringOfTags());
                    update_command.ExecuteNonQuery();
                }
            }
            //DateTime finish = DateTime.Now;
        }
    }
    [Serializable()]
    public class ImageDBException : System.Exception
    {
        public ImageDBException() : base() { }
        public ImageDBException(string message) : base(message) { }
        public ImageDBException(string message, System.Exception inner) : base(message, inner) { }
        protected ImageDBException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }
    }
}
