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

using System.Data.SQLite;
using System.IO;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace ErzaLib2
{
    public class ErzaDB
    {
        public static void LoadImageToErza(ImageInfo Image, SQLiteConnection Connection)
        {
            List<long> tag_ids = new List<long>();
            ImageInfo? temp_image = GetImageWithOutTags(Image.Hash, Connection);
            if (temp_image == null)
            {
                AddImage(Image, Connection);
            }
            else
            {
                if (temp_image.Deleted != true)
                {
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
                    tag_ids = tag_ids.Except(GetTagsByImageIDToTagIDs(temp_image.ImageID, Connection)).ToList();
                    if (tag_ids.Count > 0)
                    {
                        AddTagsToImage(temp_image.ImageID, tag_ids, Connection);
                    }
                }
            }
        }
        public static void AddImage(ImageInfo Image, SQLiteConnection Connection)
        {
            using (SQLiteCommand insert_command = new SQLiteCommand(Connection))
            {
                insert_command.CommandText = "insert into images (hash, phash, favorited, deleted, file_path, width, height, tags) values (@hash, @phash, @favorited, @deleted, @file_path, @width, @height, @tags)";
                insert_command.Parameters.AddWithValue("hash", Image.Hash);
                insert_command.Parameters.AddWithValue("deleted", Image.Deleted);
                insert_command.Parameters.AddWithValue("favorited", Image.Favorited);
                insert_command.Parameters.AddWithValue("width", Image.Width);
                insert_command.Parameters.AddWithValue("height", Image.Height);
                if (Image.FilePath == null)
                {
                    insert_command.Parameters.AddWithValue("file_path", DBNull.Value);
                }
                else
                {
                    insert_command.Parameters.AddWithValue("file_path", Image.FilePath);
                }
                if (Image.PHash == null)
                {
                    insert_command.Parameters.AddWithValue("phash", DBNull.Value);
                }
                else
                {
                    insert_command.Parameters.AddWithValue("phash", Image.PHash);
                }
                if (Image.Tags != null && Image.Tags.Count > 0)
                {
                    List<long> tagids = new List<long>();
                    foreach (string tag in Image.Tags)
                    {
                        long t = GetTagID(tag, Connection);
                        if (t >= 0)
                        {
                            tagids.Add(t);
                        }
                        else
                        {
                            AddTag(tag, Connection);
                            tagids.Add(GetTagID(tag, Connection));
                        }
                    }
                    insert_command.Parameters.AddWithValue("tags", GetStringOfTagIDs(tagids));
                }
                else
                {
                    insert_command.Parameters.AddWithValue("tags", DBNull.Value);
                }
                insert_command.ExecuteNonQuery();
            }
        }
        public static void AddTagToImage(long ImageID, long TagID, SQLiteConnection Connection)
        {
            List<long> tagids = new List<long>();
            tagids.Add(TagID);
            using (SQLiteCommand command = new SQLiteCommand("SELECT tags FROM images WHERE image_id = @image_id", Connection))
            {
                command.Parameters.AddWithValue("image_id", ImageID);
                object o = command.ExecuteScalar();
                if (o != null && o != DBNull.Value)
                {
                    tagids.AddRange(ParseStringOfTagIDs((string)o));
                }
            }
            tagids = tagids.Distinct().ToList();
            using (SQLiteCommand command = new SQLiteCommand("UPDATE images SET tags = @tags WHERE image_id = @image_id", Connection))
            {
                command.Parameters.AddWithValue("tags", GetStringOfTagIDs(tagids));
                command.Parameters.AddWithValue("image_id", ImageID);
                command.ExecuteNonQuery();
            }
        }
        public static void AddTagsToImage(long ImageID, IEnumerable<long> TagIDs, SQLiteConnection Connection)
        {
            List<long> tagids = new List<long>();
            foreach (long TagID in TagIDs) {
                tagids.Add(TagID);
            }
            using (SQLiteCommand command = new SQLiteCommand("SELECT tags FROM images WHERE image_id = @image_id", Connection))
            {
                command.Parameters.AddWithValue("image_id", ImageID);
                object o = command.ExecuteScalar();
                if (o != null && o != DBNull.Value)
                {
                    tagids.AddRange(ParseStringOfTagIDs((string)o));
                }
            }
            tagids = tagids.Distinct().ToList();
            using (SQLiteCommand command = new SQLiteCommand("UPDATE images SET tags = @tags WHERE image_id = @image_id", Connection))
            {
                command.Parameters.AddWithValue("tags", GetStringOfTagIDs(tagids));
                command.Parameters.AddWithValue("image_id", ImageID);
                command.ExecuteNonQuery();
            }
        }
        public static long GetImageID(string? Hash, SQLiteConnection Connection)
        {
            string sql = "SELECT image_id FROM images WHERE hash = @hash";
            using (SQLiteCommand command = new SQLiteCommand(sql, Connection))
            {
                command.Parameters.AddWithValue("hash", Hash);
                object o = command.ExecuteScalar();
                if (o == null || o == DBNull.Value)
                {
                    return -1;
                }
                else
                {
                    return Convert.ToInt64(o);
                }
            }
        }
        public static ImageInfo? GetImageWithOutTags(string? Hash, SQLiteConnection Connection)
        {
            string sql = "SELECT image_id, hash, deleted, file_path, width, height, favorited, phash FROM images WHERE hash = @hash";
            using (SQLiteCommand command = new SQLiteCommand(sql, Connection))
            {
                command.Parameters.AddWithValue("hash", Hash);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    ImageInfo image = new ImageInfo();
                    image.ImageID = (long)reader["image_id"];
                    image.Hash = (string)reader["hash"];
                    image.Deleted = Convert.ToBoolean(reader["deleted"]);
                    image.Favorited = Convert.ToBoolean(reader["favorited"]);
                    image.Width = Convert.ToInt32(reader["width"]);
                    image.Height = Convert.ToInt32(reader["height"]);
                    object o = reader["file_path"];
                    if (o != DBNull.Value)
                    {
                        image.FilePath = (string)o;
                    }
                    o = reader["phash"];
                    if (o != DBNull.Value)
                    {
                        image.PHash = (byte[])o;
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
        public static ImageInfo? GetImageWithOutTags(long ImageID, SQLiteConnection Connection)
        {
            string sql = "SELECT image_id, hash, deleted, file_path, width, height, favorited, phash FROM images WHERE image_id = @image_id";
            using (SQLiteCommand command = new SQLiteCommand(sql, Connection))
            {
                command.Parameters.AddWithValue("image_id", ImageID);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    ImageInfo image = new ImageInfo();
                    image.ImageID = (long)reader["image_id"];
                    image.Hash = (string)reader["hash"];
                    image.Deleted = Convert.ToBoolean(reader["deleted"]);
                    image.Favorited = Convert.ToBoolean(reader["favorited"]);
                    image.Width = Convert.ToInt32(reader["width"]);
                    image.Height = Convert.ToInt32(reader["height"]);
                    object o = reader["file_path"];
                    if (o != DBNull.Value)
                    {
                        image.FilePath = (string)o;
                    }
                    o = reader["phash"];
                    if (o != DBNull.Value)
                    {
                        image.PHash = (byte[])o;
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
                update_command.CommandText = "UPDATE images SET deleted = @deleted, width = @width, height = @height, file_path = @file_path, favorited = @favorited WHERE hash = @hash";
                update_command.Parameters.AddWithValue("hash", Image.Hash);
                update_command.Parameters.AddWithValue("width", Image.Width);
                update_command.Parameters.AddWithValue("height", Image.Height);
                update_command.Parameters.AddWithValue("file_path", Image.FilePath);
                update_command.Parameters.AddWithValue("deleted", Image.Deleted);
                update_command.Parameters.AddWithValue("favorited", Image.Favorited);
                update_command.ExecuteNonQuery();
            }
        }
        public static void SetImageResolution(long ImageID, int Width, int Height, SQLiteConnection Connection)
        {
            using (SQLiteCommand update_command = new SQLiteCommand(Connection))
            {
                update_command.CommandText = "UPDATE images SET width = @width, height = @height WHERE image_id = @image_id";
                update_command.Parameters.AddWithValue("image_id", ImageID);
                update_command.Parameters.AddWithValue("width", Width);
                update_command.Parameters.AddWithValue("height", Height);
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
        public static void SetImagePath(long ImageID, string FilePath, SQLiteConnection Connection)
        {
            using (SQLiteCommand update_command = new SQLiteCommand(Connection))
            {
                update_command.CommandText = "UPDATE images SET file_path = @file_path WHERE image_id = @image_id";
                update_command.Parameters.AddWithValue("image_id", ImageID);
                update_command.Parameters.AddWithValue("file_path", FilePath);
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
        public static void SetImagePhash(long ImageID, byte[] Phash, SQLiteConnection Connection)
        {
            using (SQLiteCommand update_command = new SQLiteCommand(Connection))
            {
                update_command.CommandText = "UPDATE images SET phash = @phash WHERE image_id = @image_id";
                update_command.Parameters.AddWithValue("image_id", ImageID);
                update_command.Parameters.AddWithValue("phash", Phash);
                update_command.ExecuteNonQuery();
            }
        }
        public static void SetImageFavorit(long ImageID, bool Favorited, SQLiteConnection Connection)
        {
            using (SQLiteCommand update_command = new SQLiteCommand(Connection))
            {
                update_command.CommandText = "UPDATE images SET favorited = @favorited WHERE image_id = @image_id";
                update_command.Parameters.AddWithValue("image_id", ImageID);
                update_command.Parameters.AddWithValue("favorited", Favorited);
                update_command.ExecuteNonQuery();
            }
        }
        public static void DeleteImage(long ImageID, SQLiteConnection Connection)
        {
            using (SQLiteCommand update_command = new SQLiteCommand(Connection))
            {
                update_command.CommandText = "UPDATE images SET deleted = @deleted, width = @width, height = @height, file_path = @file_path, favorited = @favorited, phash = @phash, tags = @tags WHERE image_id = @image_id";
                update_command.Parameters.AddWithValue("image_id", ImageID);
                update_command.Parameters.AddWithValue("width", 0);
                update_command.Parameters.AddWithValue("height", 0);
                update_command.Parameters.AddWithValue("file_path", null);
                update_command.Parameters.AddWithValue("deleted", true);
                update_command.Parameters.AddWithValue("favorited", false);
                update_command.Parameters.AddWithValue("phash", null);
                update_command.Parameters.AddWithValue("tags", null);
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
            }
        }
        public static void VipeImage(long ImageID, SQLiteConnection Connection)
        {
            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = "DELETE FROM images WHERE image_id = @image_id";
                command.Parameters.AddWithValue("image_id", ImageID);
                command.ExecuteNonQuery();
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
        public static void AddTag(string Tag, TagType Type, SQLiteConnection Connection)
        {
            string sql = "INSERT INTO tags (tag, type) VALUES (@tag, @type);";
            using (SQLiteCommand command = new SQLiteCommand(sql, Connection))
            {
                command.Parameters.AddWithValue("tag", Tag);
                command.Parameters.AddWithValue("type", Type);
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
                if (o == null || o == DBNull.Value)
                {
                    return -1;
                }
                else
                {
                    return System.Convert.ToInt64(o);
                }
            }
        }
        public static List<ImageInfo> GetImagesByTag(string Tag, SQLiteConnection Connection)
        {
            List<ImageInfo> imgs = new List<ImageInfo>();
            string sql = "SELECT image_id, favorited, deleted, width, height, hash, phash, file_path FROM images WHERE deleted = 0 AND file_path IS NOT NULL AND tags LIKE '%#' || (SELECT tag_id FROM tags WHERE tag = @tag) || '#%'";
            using (SQLiteCommand command = new SQLiteCommand(sql, Connection))
            {
                command.Parameters.AddWithValue("tag", Tag);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ImageInfo image = new ImageInfo();
                    image.ImageID = (long)reader["image_id"];
                    image.Hash = (string)reader["hash"];
                    image.Favorited = Convert.ToBoolean(reader["favorited"]);
                    image.Deleted = Convert.ToBoolean(reader["deleted"]);
                    image.Width = Convert.ToInt32(reader["width"]);
                    image.Height = Convert.ToInt32(reader["height"]);
                    object o = reader["file_path"];
                    if (o != DBNull.Value)
                    {
                        image.FilePath = (string)o;
                    }
                    o = reader["phash"];
                    if (o != DBNull.Value)
                    {
                        image.PHash = (byte[])o;
                    }
                    imgs.Add(image);
                }
                reader.Close();
                return imgs;
            }
        }
        public static List<ImageInfo> GetImagesByPartTag(string PartTag, SQLiteConnection Connection)
        {
            List<long> tagids = new List<long>();
            using (SQLiteCommand command = new SQLiteCommand("SELECT tag_id FROM tags WHERE tag LIKE '%' || @tag || '%'", Connection))
            {
                command.Parameters.AddWithValue("tag", PartTag);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    tagids.Add(reader.GetInt64(0));
                }
                reader.Close();
            }
            List<ImageInfo> imgs = new List<ImageInfo>();
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT image_id, favorited, deleted, width, height, hash, phash, file_path FROM images WHERE deleted = 0 AND file_path IS NOT NULL AND (");
            for(int i = 0; i < tagids.Count; i++)
            {
                if (i > 0) sql.Append(" OR ");
                sql.Append("tags LIKE '%#" + tagids[i].ToString() + "#%'");
            }
            sql.Append(')');
            using (SQLiteCommand command = new SQLiteCommand(sql.ToString(), Connection))
            {
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ImageInfo image = new ImageInfo();
                    image.ImageID = (long)reader["image_id"];
                    image.Hash = (string)reader["hash"];
                    image.Favorited = Convert.ToBoolean(reader["favorited"]);
                    image.Deleted = Convert.ToBoolean(reader["deleted"]);
                    image.Width = Convert.ToInt32(reader["width"]);
                    image.Height = Convert.ToInt32(reader["height"]);
                    object o = reader["file_path"];
                    if (o != DBNull.Value)
                    {
                        image.FilePath = (string)o;
                    }
                    o = reader["phash"];
                    if (o != DBNull.Value)
                    {
                        image.PHash = (byte[])o;
                    }
                    imgs.Add(image);
                }
                reader.Close();
                return imgs;
            }
        }
        public static List<ImageInfo> GetAllImages(bool WithOutFilePath, SQLiteConnection Connection)
        {
            List<ImageInfo> imgs = new List<ImageInfo>();
            string sql;
            if (WithOutFilePath) {
                sql = "SELECT image_id, favorited, deleted, width, height, hash, phash, file_path FROM images WHERE deleted = 0;";
            }
            else
            {
                sql = "SELECT image_id, favorited, deleted, width, height, hash, phash, file_path FROM images WHERE deleted = 0 AND file_path IS NOT NULL;";
            }
            using (SQLiteCommand command = new SQLiteCommand(sql, Connection))
            {
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ImageInfo image = new ImageInfo();
                    image.ImageID = (long)reader["image_id"];
                    image.Favorited = Convert.ToBoolean(reader["favorited"]);
                    image.Hash = (string)reader["hash"];
                    image.Deleted = Convert.ToBoolean(reader["deleted"]);
                    image.Width = Convert.ToInt32(reader["width"]);
                    image.Height = Convert.ToInt32(reader["height"]);
                    object o = reader["file_path"];
                    if (o != DBNull.Value)
                    {
                        image.FilePath = (string)o;
                    }
                    o = reader["phash"];
                    if (o != DBNull.Value)
                    {
                        image.PHash = (byte[])o;
                    }
                    imgs.Add(image);
                }
                reader.Close();
                return imgs;
            }
        }
        public static List<ImageInfo> GetImagesByTags(List<string> Tags, bool Or, SQLiteConnection Connection)
        {
            List<long> tagids = new List<long>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT tag_id FROM tags WHERE tag IN (");
            for (int i = 0; i < Tags.Count; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.Append("@tag" + i.ToString());
            }
            sb.Append(")");
            using (SQLiteCommand command = new SQLiteCommand(sb.ToString(), Connection))
            {
                for (int i = 0; i < Tags.Count; i++)
                {
                    command.Parameters.AddWithValue("tag" + i.ToString(), Tags[i]);
                }
                
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    tagids.Add(reader.GetInt64(0));
                }
                reader.Close();
            }
            List<ImageInfo> imgs = new List<ImageInfo>();
            if (Or)
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT image_id, favorited, deleted, width, height, hash, phash, file_path FROM images WHERE deleted = 0 AND file_path IS NOT NULL AND (");
                for (int i = 0; i < tagids.Count; i++)
                {
                    if (i > 0) sql.Append(" OR ");
                    sql.Append("tags LIKE '%#" + tagids[i].ToString() + "#%'");
                }
                sql.Append(')');
                using (SQLiteCommand command = new SQLiteCommand(sql.ToString(), Connection))
                {
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        ImageInfo image = new ImageInfo();
                        image.ImageID = (long)reader["image_id"];
                        image.Hash = (string)reader["hash"];
                        image.Favorited = Convert.ToBoolean(reader["favorited"]);
                        image.Deleted = Convert.ToBoolean(reader["deleted"]);
                        image.Width = Convert.ToInt32(reader["width"]);
                        image.Height = Convert.ToInt32(reader["height"]);
                        object o = reader["file_path"];
                        if (o != DBNull.Value)
                        {
                            image.FilePath = (string)o;
                        }
                        o = reader["phash"];
                        if (o != DBNull.Value)
                        {
                            image.PHash = (byte[])o;
                        }
                        imgs.Add(image);
                    }
                    reader.Close();
                }
            }
            else
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT image_id, favorited, deleted, width, height, hash, phash, file_path FROM images WHERE deleted = 0 AND file_path IS NOT NULL AND (");
                for (int i = 0; i < tagids.Count; i++)
                {
                    if (i > 0) sql.Append(" AND ");
                    sql.Append("tags LIKE '%#" + tagids[i].ToString() + "#%'");
                }
                sql.Append(')');
                using (SQLiteCommand command = new SQLiteCommand(sql.ToString(), Connection))
                {
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        ImageInfo image = new ImageInfo();
                        image.ImageID = (long)reader["image_id"];
                        image.Hash = (string)reader["hash"];
                        image.Favorited = Convert.ToBoolean(reader["favorited"]);
                        image.Deleted = Convert.ToBoolean(reader["deleted"]);
                        image.Width = Convert.ToInt32(reader["width"]);
                        image.Height = Convert.ToInt32(reader["height"]);
                        object o = reader["file_path"];
                        if (o != DBNull.Value)
                        {
                            image.FilePath = (string)o;
                        }
                        o = reader["phash"];
                        if (o != DBNull.Value)
                        {
                            image.PHash = (byte[])o;
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
            List<long> tagids = GetTagsByImageIDToTagIDs(ImageID, Connection);
            foreach (long tagid in tagids)
            {
                using (SQLiteCommand command = new SQLiteCommand(Connection))
                {
                    command.CommandText = "SELECT tag FROM tags WHERE tag_id = @tag_id";
                    command.Parameters.AddWithValue("tag_id", tagid);
                    object o = command.ExecuteScalar();
                    if (o != null && o != DBNull.Value)
                    {
                        tags.Add((string)o);
                    }
                }
            }
            return tags;
        }
        public static List<TagInfo> GetTagsByImageID(long ImageID, SQLiteConnection Connection)
        {
            List<TagInfo> tags = new List<TagInfo>();
            List<long> tagids = GetTagsByImageIDToTagIDs(ImageID, Connection);
            foreach (long tagid in tagids)
            {
                using (SQLiteCommand command = new SQLiteCommand(Connection))
                {
                    command.CommandText = "SELECT tag_id, count, type, tag, localization, description FROM tags WHERE tag_id = @tag_id";
                    command.Parameters.AddWithValue("tag_id", tagid);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TagInfo tag = new TagInfo();
                            tag.Tag = (string)reader["tag"];
                            tag.TagID = (long)reader["tag_id"];
                            tag.Count = (long)reader["count"];
                            object o2 = reader["localization"];
                            if (o2 != DBNull.Value)
                            {
                                tag.Localization = (string)o2;
                            }
                            o2 = reader["description"];
                            if (o2 != DBNull.Value)
                            {
                                tag.Description = (string)o2;
                            }
                            tag.Type = (TagType)reader["type"];
                            tags.Add(tag);
                        }
                    }
                }
            }
            return tags;
        }
        public static List<long> GetTagsByImageIDToTagIDs(long ImageID, SQLiteConnection Connection)
        {
            List<long> tags = new List<long>();
            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = "select tags from images where image_id = @image_id";
                command.Parameters.AddWithValue("image_id", ImageID);
                object o = command.ExecuteScalar();
                if (o != null && o != DBNull.Value)
                {
                    tags = ParseStringOfTagIDs((string)o);
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
        public static void DeleteTagFromImage(long TagID, long ImageID, SQLiteConnection Connection)
        {
            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = "UPDATE images SET tags = REPLACE(tags, '#' || @tag_id || '#', '#') WHERE image_id = @image_id;";
                command.Parameters.AddWithValue("tag_id", TagID);
                command.Parameters.AddWithValue("image_id", ImageID);
                command.ExecuteNonQuery();
            }
        }
        public static void DeleteTagFromImage(string Tag, long ImageID, SQLiteConnection Connection)
        {
            long tagid = GetTagID(Tag, Connection);
            DeleteTagFromImage(tagid, ImageID, Connection);
        }
        public static List<string> SearchTags(string Query, bool First, bool Sort, SQLiteConnection Connection)
        {
            List<string> tags = new List<string>();
            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                if (Sort)
                {
                    command.CommandText = "SELECT tag FROM tags WHERE tag LIKE @tag ORDER BY tag ASC";
                }
                else
                {
                    command.CommandText = "SELECT tag FROM tags WHERE tag LIKE @tag";
                }
                if (First)
                {
                    command.Parameters.AddWithValue("tag", Query + "%");
                }
                else
                {
                    command.Parameters.AddWithValue("tag", "%" + Query + "%");
                }
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
        public static long CountTag(string Tag, SQLiteConnection Connection)
        {
            using (SQLiteCommand command = new SQLiteCommand())
            {
                command.CommandText = "SELECT count(*) FROM images WHERE tags LIKE '%#' || (SELECT tag_id FROM tags WHERE tag = @tag) || '#%';";
                command.Parameters.AddWithValue("tag", Tag);
                command.Connection = Connection;
                object o = command.ExecuteScalar();
                if (o == null || o == DBNull.Value)
                {
                    return 0;
                }
                else
                {
                    return System.Convert.ToInt64(o);
                }
            }
        }
        public static List<TagInfo> CountTags(List<TagInfo> Tags, SQLiteConnection Connection)
        {
            List<TagInfo> tags = new List<TagInfo>();
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT tag, tag_id, type, localization, description, (SELECT COUNT(*) FROM images WHERE tags LIKE '%#' || tag_id || '#%') AS count_tag FROM tags WHERE tag IN (");
            for (int i = 0; i < Tags.Count; i++)
            {
                if (i > 0) sql.Append(", ");
                //sql.Append("'" + Tags[i].Tag + "'");
                sql.Append("@tag" + i.ToString());
            }
            sql.Append(")");
            using (SQLiteCommand command = new SQLiteCommand(sql.ToString(), Connection))
            {
                for (int i = 0; i < Tags.Count; i++)
                {
                    command.Parameters.AddWithValue("@tag" + i.ToString(), Tags[i].Tag);
                }
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TagInfo tag = new TagInfo();
                        tag.Tag = (string)reader[0];
                        tag.TagID = (long)reader[1];
                        tag.Type = (TagType)reader[2];
                        tag.Count = (long)reader[5];
                        object o = reader[3];
                        if (o != DBNull.Value)
                        {
                            tag.Localization = (string)o;
                        }
                        o = reader[4];
                        if (o != DBNull.Value)
                        {
                            tag.Description = (string)o;
                        }
                        tag.Type = (TagType)reader[2];
                        tags.Add(tag);
                    }
                    reader.Close();
                }
            }
            return tags;
        }
        public static string GetStringOfTagIDs(IEnumerable<long> tags)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("#");
            foreach (long tag_id in tags)
            {
                sb.Append(tag_id.ToString());
                sb.Append("#");
            }
            return sb.ToString();
        }
        public static List<long> ParseStringOfTagIDs(string StringTagIDs) 
        {
            List<long> tag_ids = new List<long>();
            string[] tags = StringTagIDs.Split('#');
            foreach (string tag_id in tags)
            {
                try
                {
                    tag_ids.Add(long.Parse(tag_id));
                }
                catch(Exception) { }
            }
            return tag_ids;
        }
    }
}
