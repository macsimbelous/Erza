﻿using System.Data.SQLite;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ErzaLib;
using Shipwreck.Phash;
using System.Drawing;
using Shipwreck.Phash.Bitmaps;
using System.Diagnostics.Metrics;
using System.Threading;
using System.Collections.Concurrent;
using System.IO;

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
        static int count = 0;
        static ConcurrentQueue<PhashInfo> WriteBuffer = new ConcurrentQueue<PhashInfo>();
        static void Main(string[] args)
        {
            //List<ImageInfo> imgs = new List<ImageInfo>();
            //connection = new SQLiteConnection(@"data source = C:\utils\data\erza.sqlite");
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
                    var bitmap = (Bitmap)Image.FromFile(file.FilePath);
                    img.pHash = ImagePhash.ComputeDigest(bitmap.ToLuminanceImage()).Coefficients;
                    bitmap.Dispose();
                    bitmap = null;
                    //ImageInfo img = new ImageInfo();
                    //img.pHash = BitConverter.ToString(hash.Coefficents).Replace("-", string.Empty).ToLower();
                    //img.MD5 = md5_enc(file);
                    //img.MD5 = file;
                    //string phash = BitConverter.ToString(hash.Coefficents).Replace("-", string.Empty).ToLower();
                    

                    WriteBuffer.Enqueue(img);
                    lock (locker2)
                    {
                        count++;
                    }
                }
                catch (Exception)
                {

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
                    PhashInfo img;
                    int i = 0;
                    while (WriteBuffer.TryDequeue(out img))
                    {
                        images.Add(img);
                        i++;
                        if (i >= 10) { break; }
                    }
                    SQLiteTransaction transaction = connection.BeginTransaction();
                    foreach (PhashInfo temp in images)
                    {
                        using (SQLiteCommand insert_command = new SQLiteCommand(connection))
                        {
                            insert_command.CommandText = "insert into phashs (image_id, phash) values (@image_id, @phash)";
                            insert_command.Parameters.AddWithValue("image_id", temp.ImageID);
                            insert_command.Parameters.AddWithValue("phash", temp.pHash);
                            insert_command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                    images.Clear();
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