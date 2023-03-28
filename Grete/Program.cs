using System.Data.SQLite;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ErzaLib;
using Shipwreck.Phash;
using System.Drawing;
using Shipwreck.Phash.Bitmaps;
using System.Diagnostics.Metrics;
using System.Threading;

namespace Grete
{
    internal class Program
    {
        static Queue<ImageInfo> image_queue;
        static SQLiteConnection connection;
        static Thread[] threads;
        static bool abort = false;
        static int LIMIT_THREADS = Environment.ProcessorCount;
        static object locker = new object();
        static object locker2 = new object();
        static int count = 0;
        static void Main(string[] args)
        {
            //List<ImageInfo> imgs = new List<ImageInfo>();
            image_queue = new Queue<ImageInfo>();
            connection = new SQLiteConnection(@"data source = C:\utils\data\erza.sqlite");
            connection.Open();
            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                command.CommandText = "SELECT images.image_id, images.file_path FROM images LEFT OUTER JOIN phashs on images.image_id = phashs.image_id WHERE phashs.phash IS NULL AND images.file_path IS NOT NULL;";
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ImageInfo image = new ImageInfo();
                        image.ImageID = (long)reader["image_id"];
                        object o = reader["file_path"];
                        if (o != DBNull.Value)
                        {
                            image.FilePath = (string)o;
                        }
                        //imgs.Add(image);
                        image_queue.Enqueue(image);
                    }
                }
            }
            Console.WriteLine($"Не хэшированых изображений: {image_queue.Count}");
            //for (int i = 0; i < imgs.Count; i++)
            //{
            //    //PHash
            //    var bitmap = (Bitmap)Image.FromFile(imgs[i].FilePath);
            //    var phash = ImagePhash.ComputeDigest(bitmap.ToLuminanceImage());
            //    using (SQLiteCommand insert_command = new SQLiteCommand(connection))
            //    {
            //        insert_command.CommandText = "insert into phashs (image_id, phash) values (@image_id, @phash)";
            //        insert_command.Parameters.AddWithValue("image_id", imgs[i].ImageID);
            //        insert_command.Parameters.AddWithValue("phash", phash.Coefficients);
            //        insert_command.ExecuteNonQuery();
            //    }
            //    Console.WriteLine($"[{i+1}\\{imgs.Count}] ID: {imgs[i].ImageID}");
            //}
            int size_queue = image_queue.Count;
            Console.CancelKeyPress += new ConsoleCancelEventHandler(OnExit);
            threads = new Thread[LIMIT_THREADS];
            for (int i = 0; i < LIMIT_THREADS; i++)
            {
                threads[i] = new Thread(Calculate);
                threads[i].Name = "Поток " + i.ToString();
                threads[i].Start();
            }
            while (true)
            {
                Console.Write($"\r{count}\\{size_queue}");
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
            connection.Close();
        }
        public static void Calculate()
        {
            while (!abort)
            {
                ImageInfo file;
                lock (locker)
                {
                    try
                    {
                        file = image_queue.Dequeue();
                    }
                    catch (InvalidOperationException)
                    {
                        return;
                    }
                }
                try
                {
                    //PHash
                    var bitmap = (Bitmap)Image.FromFile(file.FilePath);
                    var phash = ImagePhash.ComputeDigest(bitmap.ToLuminanceImage());
                    bitmap.Dispose();
                    bitmap = null;
                    //ImageInfo img = new ImageInfo();
                    //img.pHash = BitConverter.ToString(hash.Coefficents).Replace("-", string.Empty).ToLower();
                    //img.MD5 = md5_enc(file);
                    //img.MD5 = file;
                    //string phash = BitConverter.ToString(hash.Coefficents).Replace("-", string.Empty).ToLower();
                    lock (locker2)
                    {
                        using (SQLiteCommand insert_command = new SQLiteCommand(connection))
                        {
                            insert_command.CommandText = "insert into phashs (image_id, phash) values (@image_id, @phash)";
                            insert_command.Parameters.AddWithValue("image_id", file.ImageID);
                            insert_command.Parameters.AddWithValue("phash", phash.Coefficients);
                            insert_command.ExecuteNonQuery();
                        }
                        count++;
                    }
                }
                catch (Exception)
                {

                }
            }
        }
        protected static void OnExit(object sender, ConsoleCancelEventArgs args)
        {
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
            connection.Close();
            Console.WriteLine("Exit");
            Environment.Exit(0);
        }
    }
}