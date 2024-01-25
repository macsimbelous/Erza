using System.Data.SQLite;
using System.Data;
using ErzaLib;
using Shipwreck.Phash;

namespace Lora
{
    internal class Program
    {
        static void Main(string[] args)
        {
            double similar = 0.95;
            List<ImgTags> img_tags = new List<ImgTags>();
            List<PhashCacheItem> phash_cache = new List<PhashCacheItem>();
            List<long> img_wo_tags = new List<long>();
            using (SQLiteConnection connection = new SQLiteConnection("data source=C:\\utils\\data\\erza.sqlite"))
            {
                connection.Open();
                long count_rows;
                //Определяем число записей
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.CommandText = "SELECT count(*) FROM images LEFT OUTER JOIN image_tags on images.image_id = image_tags.image_id WHERE images.is_deleted = 0 AND image_tags.image_id IS NULL;";
                    command.Connection = connection;
                    count_rows = System.Convert.ToInt64(command.ExecuteScalar());
                }
                //Считываем записи
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.CommandText = "SELECT images.image_id FROM images LEFT OUTER JOIN image_tags on images.image_id = image_tags.image_id WHERE images.is_deleted = 0 AND image_tags.image_id IS NULL;";
                    command.Connection = connection;
                    SQLiteDataReader reader = command.ExecuteReader();
                    int count = 0;
                    while (reader.Read())
                    {
                        long id = reader.GetInt64(0);
                        img_wo_tags.Add(id);
                        count++;
                        Console.Write("Считано: {0} из {1}\r", count, count_rows);
                    }
                }
                //Сравниваем
                for (int i = 0; i < img_wo_tags.Count; i++)
                {
                    long ImageID = img_wo_tags[i];
                    Console.Write($"[{i + 1}/{img_wo_tags.Count}] {ImageID}...");
                    byte[]? phash;
                    List<long> similars = new List<long>();
                    using (SQLiteCommand command = new SQLiteCommand("SELECT phash FROM phashs WHERE image_id = @image_id", connection))
                    {
                        command.Parameters.AddWithValue("image_id", ImageID);
                        object o = command.ExecuteScalar();
                        if (o == null)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Нет Phash");
                            Console.ResetColor();
                            continue;
                        }
                        else
                        {
                            phash = o as byte[];
                        }
                    }
                    if (phash_cache.Count > 0)
                    {
                        foreach (PhashCacheItem current_phash in phash_cache)
                        {
                            double result = ImagePhash.GetCrossCorrelation(phash, current_phash.Phash);
                            if (result >= similar)
                            {
                                similars.Add(current_phash.ImageID);
                            }
                        }
                    }
                    else
                    {
                        using (SQLiteCommand command = new SQLiteCommand("select image_id, phash from phashs;", connection))
                        {
                            SQLiteDataReader reader = command.ExecuteReader();
                            while (reader.Read())
                            {
                                long imageid = (long)reader["image_id"];
                                byte[] current_phash = (byte[])reader["phash"];
                                double result = ImagePhash.GetCrossCorrelation(phash, current_phash);
                                if (result >= similar)
                                {
                                    similars.Add(imageid);
                                }
                                phash_cache.Add(new PhashCacheItem(imageid, current_phash));
                            }
                            reader.Close();
                        }
                    }

                    List<long> tagids = new List<long>();
                    foreach (long imageid in similars)
                    {
                        tagids.AddRange(ErzaLib.ErzaDB.GetTagIDsFromImageTags(imageid, connection));
                    }
                    tagids = tagids.Distinct().ToList();
                    if (tagids.Count > 0)
                    {
                        //ErzaLib.ErzaDB.AddImageTags(ImageID, tagids, connection);
                        img_tags.Add(new ImgTags(ImageID, tagids));
                    }
                    Console.WriteLine($"Найдено тегов {tagids.Count}");
                }
            }
        }
    }
    class ImgTags
    {
        public ImgTags(long ImageID, List<long> TagIDs)
        {
            this.TagIDs = TagIDs;
            this.ImageID = ImageID;
        }
        public long ImageID;
        public List<long> TagIDs;
    }
    class PhashCacheItem
    {
        public PhashCacheItem(long ImageID, byte[] Phash)
        {
            this.Phash = Phash;
            this.ImageID = ImageID;
        }
        public long ImageID;
        public byte[] Phash;
    }
}
