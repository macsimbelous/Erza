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
    along with Foobar.  If not, see <http://www.gnu.org/licenses/>.*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;
using ErzaLib;

namespace Erza
{
    class ErzaCache
    {
        private string ConnectionString;
        public TimeSpan WorkOfTime;
        public ErzaCache(String ConnectionString)
        {
            this.ConnectionString = ConnectionString;
        }
        public int GetCountItem()
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "SELECT count(*) FROM cache";
                    return System.Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }
        public void Vacuum()
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "VACUUM";
                    command.ExecuteNonQuery();
                }
            }
        }
        public void AddItems(List<ImageInfo> imgs)
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                DateTime start = DateTime.Now;
                connection.Open();
                SQLiteTransaction transact = connection.BeginTransaction();
                foreach (ImageInfo img in imgs)
                {
                    using (SQLiteCommand insert_command = new SQLiteCommand(connection))
                    {
                        insert_command.CommandText = "insert into cache (hash, tags, sankaku_post_id, danbooru_post_id, gelbooru_post_id, konachan_post_id, yandere_post_id, sankaku_url, danbooru_url, gelbooru_url, konachan_url, yandere_url) values (@hash, @tags, @sankaku_post_id, @danbooru_post_id, @gelbooru_post_id, @konachan_post_id, @yandere_post_id, @sankaku_url, @danbooru_url, @gelbooru_url, @konachan_url, @yandere_url)";
                        insert_command.Parameters.AddWithValue("hash", img.hash);
                        insert_command.Parameters.AddWithValue("tags", img.GetStringOfTags());
                        insert_command.Parameters.AddWithValue("sankaku_post_id", img.sankaku_post_id);
                        insert_command.Parameters.AddWithValue("danbooru_post_id", img.danbooru_post_id);
                        insert_command.Parameters.AddWithValue("gelbooru_post_id", img.gelbooru_post_id);
                        insert_command.Parameters.AddWithValue("konachan_post_id", img.konachan_post_id);
                        insert_command.Parameters.AddWithValue("yandere_post_id", img.yandere_post_id);
                        insert_command.Parameters.AddWithValue("sankaku_url", img.sankaku_url != null ? (object)img.sankaku_url : DBNull.Value);
                        insert_command.Parameters.AddWithValue("danbooru_url", img.danbooru_url != null ? (object)img.danbooru_url : DBNull.Value);
                        insert_command.Parameters.AddWithValue("gelbooru_url", img.gelbooru_url != null ? (object)img.gelbooru_url : DBNull.Value);
                        insert_command.Parameters.AddWithValue("konachan_url", img.konachan_url != null ? (object)img.konachan_url : DBNull.Value);
                        insert_command.Parameters.AddWithValue("yandere_url", img.yandere_url != null ? (object)img.yandere_url : DBNull.Value);
                        insert_command.Parameters.AddWithValue("is_deleted", img.is_deleted);
                        insert_command.Parameters.AddWithValue("file_name", img.file != null ? (object)img.file : DBNull.Value);
                        insert_command.ExecuteNonQuery();
                    }
                }
                transact.Commit();
                WorkOfTime = DateTime.Now - start;
            }
        }
        public List<ImageInfo> GetItems()
        {
            List<ImageInfo> imgs = new List<ImageInfo>();
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                DateTime start = DateTime.Now;
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.CommandText = "select * from cache";
                    command.Connection = connection;
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        ImageInfo img = new ImageInfo();
                        img.hash = (byte[])reader["hash"];
                        img.AddStringOfTags(System.Convert.ToString(reader["tags"]));
                        img.sankaku_post_id = System.Convert.ToInt32(reader["sankaku_post_id"]);
                        img.danbooru_post_id = System.Convert.ToInt32(reader["danbooru_post_id"]);
                        img.gelbooru_post_id = System.Convert.ToInt32(reader["gelbooru_post_id"]);
                        img.konachan_post_id = System.Convert.ToInt32(reader["konachan_post_id"]);
                        img.yandere_post_id = System.Convert.ToInt32(reader["yandere_post_id"]);
                        if (!Convert.IsDBNull(reader["danbooru_url"]))
                        {
                            img.danbooru_url = System.Convert.ToString(reader["danbooru_url"]);
                            img.urls.Add(img.danbooru_url); 
                        }
                        if (!Convert.IsDBNull(reader["konachan_url"]))
                        {
                            img.konachan_url = System.Convert.ToString(reader["konachan_url"]);
                            img.urls.Add(img.konachan_url); 
                        }
                        if (!Convert.IsDBNull(reader["yandere_url"]))
                        {
                            img.yandere_url = System.Convert.ToString(reader["yandere_url"]);
                            img.urls.Add(img.yandere_url); 
                        }
                        if (!Convert.IsDBNull(reader["gelbooru_url"]))
                        {
                            img.gelbooru_url = System.Convert.ToString(reader["gelbooru_url"]);
                            img.urls.Add(img.gelbooru_url);
                        }
                        if (!Convert.IsDBNull(reader["sankaku_url"]))
                        {
                            img.sankaku_url = System.Convert.ToString(reader["sankaku_url"]);
                            img.urls.Add(img.sankaku_url);
                        }
                        if (!Convert.IsDBNull(reader["file_name"]))
                        {
                            img.file = System.Convert.ToString(reader["file_name"]);
                        }
                        img.is_deleted = System.Convert.ToBoolean(reader["is_deleted"]);
                        imgs.Add(img);
                    }
                }
                WorkOfTime = DateTime.Now - start;
            }
            return imgs;
        }
        public void DeleteItem(byte[] hash)
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "DELETE FROM cache WHERE hash = @hash";
                    command.Parameters.AddWithValue("hash", hash);
                    command.ExecuteNonQuery();
                }
            }
        }
        public void Clear()
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "DELETE FROM cache";
                    command.ExecuteNonQuery();
                }
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "DELETE FROM options";
                    command.ExecuteNonQuery();
                }
            }
        }
        public void SetDownloadDir(string dir)
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "DELETE FROM options";
                    command.ExecuteNonQuery();
                }
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "INSERT INTO options (download_dir) VALUES (@download_dir)";
                    command.Parameters.AddWithValue("download_dir", dir);
                    command.ExecuteNonQuery();
                }
            }
        }
        public string GetDownloadDir()
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "SELECT download_dir FROM options";
                    return System.Convert.ToString(command.ExecuteScalar());
                }
            }
        }
    }
}
