using System.Data.SQLite;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ErzaLib2;
using Shipwreck.Phash;
using System.Drawing;
using Shipwreck.Phash.Bitmaps;
using System.Diagnostics.Metrics;
using System.Threading;
using System.Collections.Concurrent;
using System.IO;
using System.Data;
using static System.Net.Mime.MediaTypeNames;

namespace Grete
{
    internal class Program
    {
        static Queue<ImageInfo> image_queue = new Queue<ImageInfo>();
        static SQLiteConnection connection = new SQLiteConnection(@"data source = C:\utils\data\erza.sqlite");
        static int LIMIT_THREADS = Environment.ProcessorCount;
        static Thread[] threads = new Thread[LIMIT_THREADS];
        static Thread WriterThread = new Thread(Writer);
        static bool abort = false;
        static object locker = new object();
        static object locker2 = new object();
        static object locker3 = new object();
        static int c_count = 0;
        static int ce_count = 0;
        static int w_count = 0;
        static int we_count = 0;
        static int size_queue = 0;
        static ConcurrentQueue<PhashInfo> WriteBuffer = new ConcurrentQueue<PhashInfo>();
        static void Main(string[] args)
        {
            //List<ImageInfo> imgs = new List<ImageInfo>();
            //connection = new SQLiteConnection(@"data source = C:\utils\data\erza.sqlite");
            connection.Open();
            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                command.CommandText = "SELECT image_id, file_path FROM images WHERE phash IS NULL AND file_path IS NOT NULL;";
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
            size_queue = image_queue.Count;
            Console.CancelKeyPress += new ConsoleCancelEventHandler(OnExit);
            //threads = new Thread[LIMIT_THREADS];
            for (int i = 0; i < LIMIT_THREADS; i++)
            {
                threads[i] = new Thread(Calculate);
                threads[i].Name = "Поток " + i.ToString();
                threads[i].Start();
            }
            //WriterThread = new Thread(Writer);
            WriterThread.Name = "Writer";
            WriterThread.Start();
            while (true)
            {
                Console.Write($"\rC: {c_count}\\CE: {ce_count}\\W: {w_count}\\WE: {we_count}\\T: {size_queue}");
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
            while (WriterThread.IsAlive)
            {
                Thread.Sleep(0);
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
                        c_count++;
                    }
                    catch (InvalidOperationException)
                    {
                        return;
                    }
                }
                try
                {
                    PhashInfo img = new PhashInfo();
                    img.ImageID = file.ImageID;
                    //PHash
                    var bitmap = (Bitmap)System.Drawing.Image.FromFile(file.FilePath);
                    img.pHash = ImagePhash.ComputeDigest(bitmap.ToLuminanceImage()).Coefficients;
                    bitmap.Dispose();
                    bitmap = null;
                    //ImageInfo img = new ImageInfo();
                    //img.pHash = BitConverter.ToString(hash.Coefficents).Replace("-", string.Empty).ToLower();
                    //img.MD5 = md5_enc(file);
                    //img.MD5 = file;
                    //string phash = BitConverter.ToString(hash.Coefficents).Replace("-", string.Empty).ToLower();


                    WriteBuffer.Enqueue(img);
                }
                catch (Exception)
                {
                    lock (locker3)
                    {
                        ce_count++;
                    }
                }
            }
        }
        public static void Writer()
        {
            List<PhashInfo> images = new List<PhashInfo>();
            while (!abort)
            {
                try
                {
                    PhashInfo? img;
                    int i = 0;
                    while (WriteBuffer.TryDequeue(out img))
                    {
                        w_count++;
                        images.Add(img);
                        i++;
                        if (i >= 10) { break; }
                    }
                    if (images.Count > 0)
                    {
                        SQLiteTransaction transaction = connection.BeginTransaction();
                        foreach (PhashInfo temp in images)
                        {
                            using (SQLiteCommand insert_command = new SQLiteCommand(connection))
                            {
                                insert_command.CommandText = "UPDATE images SET phash = @phash WHERE image_id = @image_id";
                                insert_command.Parameters.AddWithValue("image_id", temp.ImageID);
                                insert_command.Parameters.AddWithValue("phash", temp.pHash);
                                insert_command.ExecuteNonQuery();
                            }
                        }
                        transaction.Commit();
                    }
                    images.Clear();
                }
                catch (Exception)
                {
                    we_count++;
                }
                if (w_count >= size_queue - ce_count)
                {
                    return;
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
            while (WriterThread.IsAlive)
            {
                Thread.Sleep(0);
            }
            connection.Close();
            Console.WriteLine("\nExit");
            Environment.Exit(0);
        }
    }
    class PhashInfo
    {
        public long ImageID = -1;
        public byte[]? pHash;
    }
}