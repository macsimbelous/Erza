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
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace ErzaLib
{
    public class ErzaDB
    {
        public static void LoadImageToErza(ImageInfo Image, SQLiteConnection Connection)
        {
            long image_id;
            List<long> tag_ids = new List<long>();
            ImageInfo temp_image = GetImageWithOutTags(Image.Hash, Connection);
            if (temp_image == null)
            {
                AddImage(Image.Hash, Image.IsDeleted, Image.FilePath,Image.Width, Image.Height, Connection);
                image_id = GetImageID(Image.Hash, Connection);
            }
            else
            {
                if (temp_image.IsDeleted == true)
                {
                    return;
                }
                else
                {
                    image_id = temp_image.ImageID;
                }
            }
            foreach (string tag in Image.Tags)
            {
                long t = GetTagID(tag, Connection);
                if (t >= 0)
                {
                    tag_ids.Add(t);
                }
                else
                {
                    AddTag(tag, Connection);
                    tag_ids.Add(GetTagID(tag, Connection));
                }
            }
            tag_ids = tag_ids.Except(GetTagIDsFromImageTags(image_id, Connection)).ToList();
            /*tag_ids.AddRange(GetTagIDsFromImageTags(image_id, Connection));
            tag_ids = tag_ids.Distinct().ToList();
            RemoveImageTags(image_id, Connection);*/
            if (tag_ids.Count > 0)
            {
                AddImageTags(image_id, tag_ids, Connection);
            }
        }
        public static void AddImage(string Hash, bool IsDeleted, string FilePath, int Width, int Height, SQLiteConnection Connection)
        {
            using (SQLiteCommand insert_command = new SQLiteCommand(Connection))
            {
                insert_command.CommandText = "insert into images (hash, is_deleted, file_path, width, height) values (@hash, @is_deleted, @file_path, @width, @height)";
                insert_command.Parameters.AddWithValue("hash", Hash);
                insert_command.Parameters.AddWithValue("is_deleted", IsDeleted);
                insert_command.Parameters.AddWithValue("width", Width);
                insert_command.Parameters.AddWithValue("height", Height);
                if (FilePath == null)
                {
                    insert_command.Parameters.AddWithValue("file_path", DBNull.Value);
                }
                else
                {
                    insert_command.Parameters.AddWithValue("file_path", FilePath);
                }
                insert_command.ExecuteNonQuery();
            }
        }
        public static void AddTagToImage(string Hash, string Tag, SQLiteConnection Connection)
        {
            string sql = "SELECT images.image_id FROM images inner join image_tags on images.image_id = image_tags.image_id inner join tags on image_tags.tag_id = tags.tag_id WHERE images.hash = @hash AND tags.tag = @tag";
            using (SQLiteCommand command = new SQLiteCommand(sql, Connection))
            {
                command.Parameters.AddWithValue("hash", Hash);
                command.Parameters.AddWithValue("tag", Tag);
                object o = command.ExecuteScalar();
                if (o == null)
                {
                    long t = GetTagID(Tag, Connection);
                    if (t < 0)
                    {
                        AddTag(Tag, Connection);
                        t = GetTagID(Tag, Connection);
                    }
                    long i = GetImageID(Hash, Connection);
                    AddImageTags(i, t, Connection);
                }
            }
        }
        public static long GetImageID(string Hash, SQLiteConnection Connection)
        {
            string sql = "SELECT image_id FROM images WHERE hash = @hash";
            using (SQLiteCommand command = new SQLiteCommand(sql, Connection))
            {
                command.Parameters.AddWithValue("hash", Hash);
                object o = command.ExecuteScalar();
                if (o == null)
                {
                    return -1;
                }
                else
                {
                    return Convert.ToInt64(o);
                }
            }
        }
        public static ImageInfo GetImageWithOutTags(string Hash, SQLiteConnection Connection)
        {
            string sql = "SELECT image_id, hash, is_deleted, file_path, width, height FROM images WHERE hash = @hash";
            using (SQLiteCommand command = new SQLiteCommand(sql, Connection))
            {
                command.Parameters.AddWithValue("hash", Hash);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    ImageInfo image = new ImageInfo();
                    image.ImageID = (long)reader["image_id"];
                    image.Hash = (string)reader["hash"];
                    image.IsDeleted = Convert.ToBoolean(reader["is_deleted"]);
                    image.Width = Convert.ToInt32(reader["width"]);
                    image.Height = Convert.ToInt32(reader["height"]);
                    object o = reader["file_path"];
                    if (o != DBNull.Value)
                    {
                        image.FilePath = (string)o;
                    }
                    reader.Close();
                    return image;
                }
                else
                {
                    reader.Close();
                    return null;
                }
            }
        }
        public static void UpdateImage(ImageInfo Image, SQLiteConnection Connection)
        {
            using (SQLiteCommand update_command = new SQLiteCommand(Connection))
            {
                update_command.CommandText = "UPDATE images SET is_deleted = @is_deleted, width = @width, height = @height, file_path = @file_path WHERE hash = @hash";
                update_command.Parameters.AddWithValue("hash", Image.Hash);
                update_command.Parameters.AddWithValue("width", Image.Width);
                update_command.Parameters.AddWithValue("height", Image.Height);
                update_command.Parameters.AddWithValue("file_path", Image.FilePath);
                update_command.Parameters.AddWithValue("is_deleted", Image.IsDeleted);
                update_command.ExecuteNonQuery();
            }
        }
        public static void SetImageResolution(string Hash, int Width, int Height, SQLiteConnection Connection)
        {
            using (SQLiteCommand update_command = new SQLiteCommand(Connection))
            {
                update_command.CommandText = "UPDATE images SET width = @width, height = @height WHERE hash = @hash";
                update_command.Parameters.AddWithValue("hash", Hash);
                update_command.Parameters.AddWithValue("width", Width);
                update_command.Parameters.AddWithValue("height", Height);
                update_command.ExecuteNonQuery();
            }
        }
        public static void SetImagePath(string Hash, string FilePath, SQLiteConnection Connection)
        {
            using (SQLiteCommand update_command = new SQLiteCommand(Connection))
            {
                update_command.CommandText = "UPDATE images SET file_path = @file_path WHERE hash = @hash";
                update_command.Parameters.AddWithValue("hash", Hash);
                update_command.Parameters.AddWithValue("file_path", FilePath);
                update_command.ExecuteNonQuery();
            }
        }
        public static void DeleteImage(string Hash, SQLiteConnection Connection)
        {
            long id = GetImageID(Hash, Connection);
            if (id < 0)
            {
                return;
            }
            else
            {
                DeleteImage(id, Connection);
                /*RemoveImageTags(id, Connection);
                using (SQLiteCommand update_command = new SQLiteCommand(Connection))
                {
                    update_command.CommandText = "UPDATE images SET is_deleted = @is_deleted, width = @width, height = @height, file_path = @file_path WHERE hash = @hash";
                    update_command.Parameters.AddWithValue("hash", Hash);
                    update_command.Parameters.AddWithValue("width", 0);
                    update_command.Parameters.AddWithValue("height", 0);
                    update_command.Parameters.AddWithValue("file_path", null);
                    update_command.Parameters.AddWithValue("is_deleted", true);
                    update_command.ExecuteNonQuery();
                }*/
            }
        }
        public static void DeleteImage(long ImageID, SQLiteConnection Connection)
        {
            RemoveImageTags(ImageID, Connection);
            using (SQLiteCommand update_command = new SQLiteCommand(Connection))
            {
                update_command.CommandText = "UPDATE images SET is_deleted = @is_deleted, width = @width, height = @height, file_path = @file_path WHERE image_id = @image_id";
                update_command.Parameters.AddWithValue("image_id", ImageID);
                update_command.Parameters.AddWithValue("width", 0);
                update_command.Parameters.AddWithValue("height", 0);
                update_command.Parameters.AddWithValue("file_path", null);
                update_command.Parameters.AddWithValue("is_deleted", true);
                update_command.ExecuteNonQuery();
            }
        }
        public static void VipeImage(string Hash, SQLiteConnection Connection)
        {
            long id = GetImageID(Hash, Connection);
            if (id < 0)
            {
                return;
            }
            else
            {
                VipeImage(id, Connection);
                /*RemoveImageTags(id, Connection);
                using (SQLiteCommand command = new SQLiteCommand(Connection))
                {
                    command.CommandText = "DELETE FROM images WHERE hash = @hash";
                    command.Parameters.AddWithValue("hash", Hash);
                    command.ExecuteNonQuery();
                }*/
            }
        }
        public static void VipeImage(long ImageID, SQLiteConnection Connection)
        {
            RemoveImageTags(ImageID, Connection);
            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = "DELETE FROM images WHERE image_id = @image_id";
                command.Parameters.AddWithValue("image_id", ImageID);
                command.ExecuteNonQuery();
            }
        }
        public static void AddTag(string Tag, SQLiteConnection Connection)
        {
            string sql = "INSERT INTO tags (tag) VALUES (@tag);";
            using (SQLiteCommand command = new SQLiteCommand(sql, Connection))
            {
                command.Parameters.AddWithValue("tag", Tag);
                command.ExecuteNonQuery();
            }
        }
        public static long GetTagID(string Tag, SQLiteConnection Connection)
        {
            string sql = "SELECT tag_id FROM tags WHERE tag = @tag";
            using (SQLiteCommand command = new SQLiteCommand(sql, Connection))
            {
                command.Parameters.AddWithValue("tag", Tag);
                object o = command.ExecuteScalar();
                if (o == null)
                {
                    return -1;
                }
                else
                {
                    return System.Convert.ToInt64(o);
                }
            }
        }
        public static void AddImageTags(long ImageID, List<long> TagIDs, SQLiteConnection Connection)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("INSERT INTO image_tags (image_id, tag_id) VALUES ");
            for (int i = 0; i < TagIDs.Count; i++)
            {
                if (i > 0) sql.Append(", ");
                sql.Append("(" + ImageID.ToString() + ", " + TagIDs[i].ToString() + ")");
            }
            sql.Append(";");
            using (SQLiteCommand ins_command = new SQLiteCommand(sql.ToString(), Connection))
            {
                ins_command.ExecuteNonQuery();
            }
        }
        public static void AddImageTags(long ImageID, long TagID, SQLiteConnection Connection)
        {
            using (SQLiteCommand ins_command = new SQLiteCommand(Connection))
            {
                ins_command.CommandText = "INSERT INTO image_tags (image_id, tag_id) VALUES (@image_id, @tag_id)";
                ins_command.Parameters.AddWithValue("image_id", ImageID);
                ins_command.Parameters.AddWithValue("tag_id", TagID);
                ins_command.ExecuteNonQuery();
            }
        }
        public static void RemoveImageTags(long ImageID, SQLiteConnection Connection)
        {
            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = "DELETE FROM image_tags WHERE image_id = @image_id";
                command.Parameters.AddWithValue("image_id", ImageID);
                command.ExecuteNonQuery();
            }
        }
        public static List<long> GetTagIDsFromImageTags(long ImageID, SQLiteConnection Connection)
        {
            List<long> ids = new List<long>();
            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = "SELECT tag_id FROM image_tags WHERE image_id = @image_id";
                command.Parameters.AddWithValue("image_id", ImageID);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ids.Add((long)reader[0]);
                    }
                }
            }
            return ids;
        }
        public static List<ImageInfo> GetImagesByTag(string Tag, SQLiteConnection Connection)
        {
            List<ImageInfo> imgs = new List<ImageInfo>();
            string sql = "select images.image_id, images.hash, images.is_deleted, images.width, images.height, images.file_path from tags inner join image_tags on tags.tag_id = image_tags.tag_id inner join images on images.image_id = image_tags.image_id where tags.tag = @tag AND images.is_deleted = 0;";
            using (SQLiteCommand command = new SQLiteCommand(sql, Connection))
            {
                command.Parameters.AddWithValue("tag", Tag);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ImageInfo image = new ImageInfo();
                    image.ImageID = (long)reader["image_id"];
                    image.Hash = (string)reader["hash"];
                    image.IsDeleted = Convert.ToBoolean(reader["is_deleted"]);
                    image.Width = Convert.ToInt32(reader["width"]);
                    image.Height = Convert.ToInt32(reader["height"]);
                    object o = reader["file_path"];
                    if (o != DBNull.Value)
                    {
                        image.FilePath = (string)o;
                    }
                    imgs.Add(image);
                }
                reader.Close();
                return imgs;
            }
        }
        public static List<ImageInfo> GetImagesByPartTag(string PartTag, SQLiteConnection Connection)
        {
            List<ImageInfo> imgs = new List<ImageInfo>();
            string sql = "select images.image_id, images.hash, images.is_deleted, images.width, images.height, images.file_path from tags inner join image_tags on tags.tag_id = image_tags.tag_id inner join images on images.image_id = image_tags.image_id where tags.tag like @tag AND images.is_deleted = 0 GROUP BY images.image_id";
            using (SQLiteCommand command = new SQLiteCommand(sql, Connection))
            {
                command.Parameters.AddWithValue("tag", '%' + PartTag + '%');
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ImageInfo image = new ImageInfo();
                    image.ImageID = (long)reader["image_id"];
                    image.Hash = (string)reader["hash"];
                    image.IsDeleted = Convert.ToBoolean(reader["is_deleted"]);
                    image.Width = Convert.ToInt32(reader["width"]);
                    image.Height = Convert.ToInt32(reader["height"]);
                    object o = reader["file_path"];
                    if (o != DBNull.Value)
                    {
                        image.FilePath = (string)o;
                    }
                    imgs.Add(image);
                }
                reader.Close();
                return imgs;
            }
        }
        public static List<ImageInfo> GetAllImages(SQLiteConnection Connection)
        {
            List<ImageInfo> imgs = new List<ImageInfo>();
            string sql = "select * from images where is_deleted = 0;";
            using (SQLiteCommand command = new SQLiteCommand(sql, Connection))
            {
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ImageInfo image = new ImageInfo();
                    image.ImageID = (long)reader["image_id"];
                    image.Hash = (string)reader["hash"];
                    image.IsDeleted = Convert.ToBoolean(reader["is_deleted"]);
                    image.Width = Convert.ToInt32(reader["width"]);
                    image.Height = Convert.ToInt32(reader["height"]);
                    object o = reader["file_path"];
                    if (o != DBNull.Value)
                    {
                        image.FilePath = (string)o;
                    }
                    imgs.Add(image);
                }
                reader.Close();
                return imgs;
            }
        }
        public static List<ImageInfo> GetImagesByTags(List<string> Tags, bool Or, SQLiteConnection Connection)
        {
            List<ImageInfo> imgs = new List<ImageInfo>();
            if (Or)
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select images.image_id, images.hash, images.is_deleted, images.width, images.height, images.file_path from tags inner join image_tags on tags.tag_id = image_tags.tag_id inner join images on images.image_id = image_tags.image_id where tags.tag in (");
                for (int i = 0; i < Tags.Count; i++)
                {
                    if (i > 0) sql.Append(", ");
                    sql.Append("'" + Tags[i] + "'");
                }
                sql.Append(") AND images.is_deleted = 0 group by images.image_id;");
                //string sql = "select images.image_id, images.hash, images.is_deleted, images.width, images.height, images.file_path, Count(images.image_id) as CountName from tags inner join image_tags on tags.tag_id = image_tags.tag_id inner join images on images.image_id = image_tags.image_id where tags.tag in ('bdsm', 'oral') group by images.image_id Having CountName=2;";
                using (SQLiteCommand command = new SQLiteCommand(sql.ToString(), Connection))
                {
                    //command.Parameters.AddWithValue("tag", Tag);
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        ImageInfo image = new ImageInfo();
                        image.ImageID = (long)reader["image_id"];
                        image.Hash = (string)reader["hash"];
                        image.IsDeleted = Convert.ToBoolean(reader["is_deleted"]);
                        image.Width = Convert.ToInt32(reader["width"]);
                        image.Height = Convert.ToInt32(reader["height"]);
                        object o = reader["file_path"];
                        if (o != DBNull.Value)
                        {
                            image.FilePath = (string)o;
                        }
                        imgs.Add(image);
                    }
                    reader.Close();
                }
            }
            else
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select images.image_id, images.hash, images.is_deleted, images.width, images.height, images.file_path, Count(images.image_id) as CountName from tags inner join image_tags on tags.tag_id = image_tags.tag_id inner join images on images.image_id = image_tags.image_id where tags.tag in (");
                for (int i = 0; i < Tags.Count; i++)
                {
                    if (i > 0) sql.Append(", ");
                    sql.Append("'" + Tags[i] + "'");
                }
                sql.Append(") group by images.image_id Having CountName=");
                sql.Append(Tags.Count);
                sql.Append(';');
                //string sql = "select images.image_id, images.hash, images.is_deleted, images.width, images.height, images.file_path, Count(images.image_id) as CountName from tags inner join image_tags on tags.tag_id = image_tags.tag_id inner join images on images.image_id = image_tags.image_id where tags.tag in ('bdsm', 'oral') group by images.image_id Having CountName=2;";
                using (SQLiteCommand command = new SQLiteCommand(sql.ToString(), Connection))
                {
                    //command.Parameters.AddWithValue("tag", Tag);
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        ImageInfo image = new ImageInfo();
                        image.ImageID = (long)reader["image_id"];
                        image.Hash = (string)reader["hash"];
                        image.IsDeleted = Convert.ToBoolean(reader["is_deleted"]);
                        image.Width = Convert.ToInt32(reader["width"]);
                        image.Height = Convert.ToInt32(reader["height"]);
                        object o = reader["file_path"];
                        if (o != DBNull.Value)
                        {
                            image.FilePath = (string)o;
                        }
                        imgs.Add(image);
                    }
                    reader.Close();
                }
            }
            return imgs;
        }
        public static List<string> GetTagsByImageIDToString(long ImageID, SQLiteConnection Connection)
        {
            List<string> tags = new List<string>();
            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = "select tags.tag from tags inner join image_tags on tags.tag_id = image_tags.tag_id where image_tags.image_id = @image_id";
                command.Parameters.AddWithValue("image_id", ImageID);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tags.Add(reader.GetString(0));
                    }
                }
            }
            return tags;
        }
        public static List<TagInfo> GetTagsByImageID(long ImageID, SQLiteConnection Connection)
        {
            List<TagInfo> tags = new List<TagInfo>();
            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = "select tags.tag, tags.count, tags.type, tags.localization, tags.description from tags inner join image_tags on tags.tag_id = image_tags.tag_id where image_tags.image_id = @image_id";
                command.Parameters.AddWithValue("image_id", ImageID);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TagInfo tag = new TagInfo();
                        tag.Tag = (string)reader["tag"];
                        tag.Count = (long)reader["count"];
                        object o = reader["localization"];
                        if (o != DBNull.Value)
                        {
                            tag.TagRus = (string)o;
                        }
                        o = reader["description"];
                        if (o != DBNull.Value)
                        {
                            tag.Description = (string)o;
                        }
                        tag.Type = (TagType)reader["type"];
                        tags.Add(tag);
                    }
                }
            }
            return tags;
        }
        public static List<string> GetAllTags(SQLiteConnection Connection)
        {
            List<string> tags = new List<string>();
            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = "SELECT tag FROM tags";
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tags.Add(reader.GetString(0));
                    }
                }
            }
            return tags;
        }
    }
}
