using System.Data.SQLite;
using System.Text;
using System.Text.RegularExpressions;

namespace Rias
{
    internal class Program
    {
        static int LIMIT = 50000;
        static string UNSORTED_PATH = @"F:\AnimeArt\UnSorted\";
        //static string TAGS_PATH = UNSORTED_PATH + @"tags\";
        static string TAGS_PATH = UNSORTED_PATH;
        static void Main(string[] args)
        {
            if (args.Length == 0) Console.WriteLine("Не заданы параметры");
            if (args[0] == "start")
            {
                Start();
            }
            else if(args[0] == "end")
            {
                End();
            }
            else
            {
                Console.WriteLine("Не корректнозаданны параметры");
            }
        }
        static void Start()
        {
            
            List<string> imgs = new List<string>();
            
            //Считываем записи
            using (SQLiteConnection Connection = new SQLiteConnection("data source=C:\\utils\\data\\erza.sqlite"))
            {
                Connection.Open();
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.CommandText = "SELECT file_path FROM images WHERE deleted = 0 AND tags IS NULL" + " LIMIT " + LIMIT.ToString();
                    command.Connection = Connection;
                    SQLiteDataReader reader = command.ExecuteReader();
                    int count = 0;
                    while (reader.Read())
                    {
                        imgs.Add(reader.GetString(0));
                        count++;
                        Console.Write("Считано: {0} из {1}\r", count, LIMIT);
                    }
                    Console.WriteLine();
                }
            }
            //Проверяем количество
            if (imgs.Count <= 0)
            {
                Console.WriteLine("Нет изображений без тегов.");
                return;
            }
            //Перемещаем в unsorted
            for (int i = 0; i < imgs.Count; i++)
            {
                string dest_file = UNSORTED_PATH + Path.GetFileName(imgs[i]);
                Console.Write($"{i+1}/{imgs.Count} {imgs[i]} >> {dest_file}");
                File.Move(imgs[i], dest_file);
                Console.WriteLine("...OK");
            }
        }
        static void End()
        {
            Regex rx = new Regex("^[a-f0-9]{32}$", RegexOptions.Compiled);
            //Загружаем файлы стегами
            string[] temp = Directory.GetFiles(TAGS_PATH, "*.txt", SearchOption.TopDirectoryOnly);
            List<string> files = new List<string>();
            //проверяем имена файлов на соответствие MD5 хэшу
            foreach (var item in temp)
            {
                if (IsMD5(Path.GetFileNameWithoutExtension(item), rx))
                {
                    files.Add(item);
                }
            }
            if (files.Count <= 0)
            {
                Console.WriteLine("Нет файлов с тегами");
                return;
            }
            //Добавляем теги в БД
            using (SQLiteConnection Connection = new SQLiteConnection("data source=C:\\utils\\data\\erza.sqlite"))
            {
                Connection.Open();
                SQLiteTransaction transact = Connection.BeginTransaction();
                for (int i = 0; i < files.Count; i++)
                {
                    string hash = Path.GetFileNameWithoutExtension(files[i]);
                    Console.Write($"{i + 1}/{files.Count} {hash} ");
                    string text = File.ReadAllText(files[i]);
                    string[] spit_srt = text.Split(',');
                    List<string> tags = new List<string>();
                    foreach (var item in spit_srt)
                    {
                        tags.Add(item.Trim().Replace(' ', '_'));
                    }
                    Console.Write($" {tags.Count}");

                    List<long> tag_ids = new List<long>();
                    foreach (var tag in tags)
                    {
                        long id = GetTagID(tag, Connection);
                        if (id < 0)
                        {
                            AddTag(tag, Connection);
                            id = GetTagID(tag, Connection);
                        }
                        tag_ids.Add(id);
                    }
                    AddTagsToImage(GetImageID(hash, Connection), tag_ids, Connection);

                    Console.WriteLine($"...OK");
                }
                transact.Commit();
            }
            foreach (var item in files)
            {
                File.Delete(item);
            }
        }
        static bool IsMD5(string Text, Regex rx)
        {
            Match match = rx.Match(Text);
            if (match.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        static void AddTagsToImage(long ImageID, IEnumerable<long> TagIDs, SQLiteConnection Connection)
        {
            List<long> tagids = new List<long>();
            foreach (long TagID in TagIDs)
            {
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
        static string GetStringOfTagIDs(IEnumerable<long> tags)
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
        static List<long> ParseStringOfTagIDs(string StringTagIDs)
        {
            List<long> tag_ids = new List<long>();
            string[] tags = StringTagIDs.Split('#');
            foreach (string tag_id in tags)
            {
                try
                {
                    tag_ids.Add(long.Parse(tag_id));
                }
                catch (Exception) { }
            }
            return tag_ids;
        }
        static long GetImageID(string? Hash, SQLiteConnection Connection)
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
        static long GetTagID(string Tag, SQLiteConnection Connection)
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
        static void AddTag(string Tag, SQLiteConnection Connection)
        {
            string sql = "INSERT INTO tags (tag) VALUES (@tag);";
            using (SQLiteCommand command = new SQLiteCommand(sql, Connection))
            {
                command.Parameters.AddWithValue("tag", Tag);
                command.ExecuteNonQuery();
            }
        }
    }
}
