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
        static int c_count = 0;
        static int ce_count = 0;
        static void Main(string[] args)
        {
            List<ImgTags> img_tags = new List<ImgTags>();


            using (SQLiteConnection connection = new SQLiteConnection("data source=C:\\utils\\data\\erza.sqlite"))
            {
                connection.Open();
                //Считываем записи без тегов
                img_wo_tags = ReadImgWOTags(connection);
                int size_queue = img_wo_tags.Count;
                //Загружаем кэш
                Console.WriteLine("Загружаем кэш...");
                phash_cache = LoadCache(connection);
                Console.WriteLine("Готово");
                //Сравниваем
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
                /*
                for (int i = 0; i < img_wo_tags.Count; i++)
                {
                    long ImageID = img_wo_tags[i];
                    Console.Write($"[{i + 1}/{img_wo_tags.Count}] {ImageID}...");
                    byte[]? phash;


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
                }*/
            }
        }
        static ConcurrentQueue<PhashCacheItem> ReadImgWOTags(SQLiteConnection connection)
        {
            ConcurrentQueue<PhashCacheItem> imgs = new ConcurrentQueue<PhashCacheItem>();
            long count_rows;
            //Определяем число записей
            using (SQLiteCommand command = new SQLiteCommand())
            {
                command.CommandText = "SELECT count(*) FROM images LEFT OUTER JOIN image_tags on images.image_id = image_tags.image_id LEFT OUTER JOIN phashs ON images.image_id = phashs.image_id WHERE images.is_deleted = 0 AND image_tags.image_id IS NULL AND phashs.phash IS NOT NULL;";
                command.Connection = connection;
                count_rows = System.Convert.ToInt64(command.ExecuteScalar());
            }
            //Считываем записи
            using (SQLiteCommand command = new SQLiteCommand())
            {
                command.CommandText = "SELECT images.image_id, phashs.phash FROM images LEFT OUTER JOIN image_tags on images.image_id = image_tags.image_id LEFT OUTER JOIN phashs ON images.image_id = phashs.image_id WHERE images.is_deleted = 0 AND image_tags.image_id IS NULL AND phashs.phash IS NOT NULL;";
                command.Connection = connection;
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
        static PhashCacheItem[] LoadCache(SQLiteConnection connection)
        {
            List<PhashCacheItem> phash_cache = new List<PhashCacheItem>();
            using (SQLiteCommand command = new SQLiteCommand("SELECT phashs.image_id, phashs.phash FROM phashs LEFT OUTER JOIN image_tags on phashs.image_id = image_tags.image_id WHERE image_tags.image_id IS NOT NULL GROUP BY phashs.image_id;", connection))
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
