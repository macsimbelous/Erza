using System.Data.SQLite;
using System.Data;
using ErzaLib;
using Shipwreck.Phash;
using System.IO;
using System.Collections.Concurrent;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;

namespace Lora
{
    internal class Program
    {
        static double SIMILAR = 0.99;
        static int LIMIT_THREADS = Environment.ProcessorCount;
        static Thread[] threads = new Thread[LIMIT_THREADS];
        static ConcurrentQueue<PhashCacheItem>? img_wo_tags;
        static ConcurrentQueue<PhashCacheItem> img_finded_tags = new ConcurrentQueue<PhashCacheItem>();
        static PhashCacheItem[]? phash_cache;
        static object locker = new object();
        static object locker3 = new object();
        static bool abort = false;
        static bool on_exit_flag = false;
        static int c_count = 0;
        static int ce_count = 0;
        static SQLiteConnection connection = new SQLiteConnection("data source=C:\\utils\\data\\erza.sqlite");
        static void Main(string[] args)
        {
            //List<ImgTags> img_tags = new List<ImgTags>();
            connection.Open();
            if (CountItemOnCache() <= 0)
            {
                //Считываем записи без тегов
                img_wo_tags = ReadImgWOTags(connection);
            }
            else
            {
                img_wo_tags = GetImagesFromCache(connection);
            }

            int size_queue = img_wo_tags.Count;
            //Загружаем кэш
            Console.WriteLine("Загружаем кэш...");
            phash_cache = LoadCache(connection);
            Console.WriteLine("Готово");
            //Сравниваем
            Console.CancelKeyPress += new ConsoleCancelEventHandler(OnExit);
            for (int i = 0; i < LIMIT_THREADS; i++)
            {
                threads[i] = new Thread(Similaring);
                threads[i].Name = "Поток " + i.ToString();
                threads[i].Start();
            }
            while (true)
            {
                Console.Write($"\rC: {c_count}\\CE: {ce_count}\\T: {size_queue}");
                bool alive = false;
                foreach (Thread thread in threads)
                {
                    if (thread.IsAlive)
                    {
                        alive = true;
                        break;
                    }
                }
                if (alive)
                {
                    Thread.Sleep(0);
                }
                else
                {
                    break;
                }
            }
            while (on_exit_flag)
            {
                Thread.Sleep(0);
            }
            SQLiteTransaction transaction = connection.BeginTransaction();
            int index = 1;
            int count_img_find_tags = 0;
            while (true)
            {
                PhashCacheItem? item = null;
                if (img_finded_tags.TryDequeue(out item))
                {
                    index++;
                    Console.Write($"[{index}] {item.ImageID}...");
                    List<long> tagids = new List<long>();
                    foreach (long imageid in item.Similars)
                    {
                        tagids.AddRange(ErzaLib.ErzaDB.GetTagIDsFromImageTags(imageid, connection));
                    }
                    tagids = tagids.Distinct().ToList();
                    if (tagids.Count > 0)
                    {
                        ErzaLib.ErzaDB.AddImageTags(item.ImageID, tagids, connection);
                        count_img_find_tags++;
                    }
                    Console.WriteLine($"Найдено тегов {tagids.Count}");
                }
                else
                {
                    break;
                }
            }
            if (img_wo_tags.Count > 0)
            {
                Console.Write("Добавляем в Кэш...");
                int c = 0;
                foreach (PhashCacheItem img in img_wo_tags)
                {
                    using (SQLiteCommand command = new SQLiteCommand())
                    {
                        command.CommandText = "INSERT INTO lora_cache (image_id, phash) VALUES (@image_id, @phash);";
                        command.Parameters.AddWithValue("image_id", img.ImageID);
                        command.Parameters.AddWithValue("phash", img.Phash);
                        command.Connection = connection;
                        command.ExecuteNonQuery();
                    }
                    c++;
                }
                Console.WriteLine($"добавлено {c}");
            }
            transaction.Commit();
            connection.Close();
            Console.WriteLine($"Найдено изображений с тегами: {count_img_find_tags}");
        }
        static ConcurrentQueue<PhashCacheItem> ReadImgWOTags(SQLiteConnection Connection)
        {
            ConcurrentQueue<PhashCacheItem> imgs = new ConcurrentQueue<PhashCacheItem>();
            long count_rows;
            //Определяем число записей
            using (SQLiteCommand command = new SQLiteCommand())
            {
                command.CommandText = "SELECT count(*) FROM images LEFT OUTER JOIN image_tags on images.image_id = image_tags.image_id LEFT OUTER JOIN phashs ON images.image_id = phashs.image_id WHERE images.is_deleted = 0 AND image_tags.image_id IS NULL AND phashs.phash IS NOT NULL;";
                command.Connection = Connection;
                count_rows = System.Convert.ToInt64(command.ExecuteScalar());
            }
            //Считываем записи
            using (SQLiteCommand command = new SQLiteCommand())
            {
                command.CommandText = "SELECT images.image_id, phashs.phash FROM images LEFT OUTER JOIN image_tags on images.image_id = image_tags.image_id LEFT OUTER JOIN phashs ON images.image_id = phashs.image_id WHERE images.is_deleted = 0 AND image_tags.image_id IS NULL AND phashs.phash IS NOT NULL;";
                command.Connection = Connection;
                SQLiteDataReader reader = command.ExecuteReader();
                int count = 0;
                while (reader.Read())
                {
                    imgs.Enqueue(new PhashCacheItem((long)reader[0], (byte[])reader[1]));
                    count++;
                    Console.Write("Считано: {0} из {1}\r", count, count_rows);
                }
                Console.WriteLine();
            }
            return imgs;
        }
        static PhashCacheItem[] LoadCache(SQLiteConnection Connection)
        {
            List<PhashCacheItem> phash_cache = new List<PhashCacheItem>();
            using (SQLiteCommand command = new SQLiteCommand("SELECT phashs.image_id, phashs.phash FROM phashs LEFT OUTER JOIN image_tags on phashs.image_id = image_tags.image_id WHERE image_tags.image_id IS NOT NULL GROUP BY phashs.image_id;", Connection))
            {
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    phash_cache.Add(new PhashCacheItem((long)reader[0], (byte[])reader[1]));
                }
                reader.Close();
            }
            return phash_cache.ToArray();
        }
        static void Similaring()
        {
            while (!abort)
            {
                PhashCacheItem? item = null;
                if (img_wo_tags.TryDequeue(out item))
                {
                    try
                    {
                        List<long> similars = new List<long>();
                        foreach (PhashCacheItem current_phash in phash_cache)
                        {
                            double result = ImagePhash.GetCrossCorrelation(item.Phash, current_phash.Phash);
                            if (result >= SIMILAR)
                            {
                                similars.Add(current_phash.ImageID);
                            }
                        }
                        item.Similars = similars;
                        img_finded_tags.Enqueue(item);
                    }
                    catch (Exception)
                    {
                        lock (locker3)
                        {
                            ce_count++;
                        }
                    }
                    lock (locker)
                    {
                        c_count++;
                    }
                }
                else
                {
                    break;
                }
            }
        }
        protected static void OnExit(object sender, ConsoleCancelEventArgs args)
        {
            on_exit_flag = true;
            abort = true;
            while (true)
            {
                bool alive = false;
                foreach (Thread thread in threads)
                {
                    if (thread.IsAlive)
                    {
                        alive = true;
                        break;
                    }
                }
                if (alive)
                {
                    continue;
                }
                else
                {
                    break;
                }
            }
            SQLiteTransaction transaction = connection.BeginTransaction();
            int index = 1;
            while (true)
            {
                PhashCacheItem? item = null;
                if (img_finded_tags.TryDequeue(out item))
                {
                    index++;
                    Console.Write($"[{index}] {item.ImageID}...");
                    List<long> tagids = new List<long>();
                    foreach (long imageid in item.Similars)
                    {
                        tagids.AddRange(ErzaLib.ErzaDB.GetTagIDsFromImageTags(imageid, connection));
                    }
                    tagids = tagids.Distinct().ToList();
                    if (tagids.Count > 0)
                    {
                        ErzaLib.ErzaDB.AddImageTags(item.ImageID, tagids, connection);
                    }
                    Console.WriteLine($"Найдено тегов {tagids.Count}");
                }
                else
                {
                    break;
                }
            }

            Console.Write("Добавляем в Кэш...");
            int c = 0;
            foreach (PhashCacheItem img in img_wo_tags)
            {
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.CommandText = "INSERT INTO lora_cache (image_id, phash) VALUES (@image_id, @phash);";
                    command.Parameters.AddWithValue("image_id", img.ImageID);
                    command.Parameters.AddWithValue("phash", img.Phash);
                    command.Connection = connection;
                    command.ExecuteNonQuery();
                }
                c++;
            }
            Console.WriteLine($"добавлено {c}");

            transaction.Commit();
            connection.Close();
            Console.WriteLine("\nExit");
            Environment.Exit(0);
        }
        static int CountItemOnCache()
        {
            using (SQLiteCommand command = new SQLiteCommand())
            {
                command.CommandText = "SELECT count(*) FROM lora_cache;";
                command.Connection = connection;
                int count = Convert.ToInt32(command.ExecuteScalar());
                return count;
            }
        }
        static ConcurrentQueue<PhashCacheItem> GetImagesFromCache(SQLiteConnection Connection)
        {
            ConcurrentQueue<PhashCacheItem> imgs = new ConcurrentQueue<PhashCacheItem>();
            using (SQLiteCommand command = new SQLiteCommand())
            {
                command.CommandText = "SELECT image_id, phash FROM lora_cache;";
                command.Connection = Connection;
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    PhashCacheItem img = new PhashCacheItem((long)reader[0], (byte[])reader[1]);
                    imgs.Enqueue(img);
                }
                reader.Close();
            }
            using (SQLiteCommand command = new SQLiteCommand())
            {
                command.CommandText = "DELETE FROM lora_cache;";
                command.Connection = Connection;
                command.ExecuteNonQuery();
            }
            return imgs;
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
        public PhashCacheItem(long ImageID, byte[]? Phash)
        {
            this.Phash = Phash;
            this.ImageID = ImageID;
        }
        public long ImageID;
        public byte[]? Phash;
        public List<long>? Similars;
    }
}
