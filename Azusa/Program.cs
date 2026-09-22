using ErzaLib2;
using ImageDimensions;
using ImageMagick;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Azusa
{
    class Program
    {
        static int bad_images = 0;
        static int taglib_count = 0;
        static SQLiteConnection connection;
        static List<img_info> imgs;
        static List<string> bad_files;
        static DateTime start;
        static void Main(string[] args)
        {
            start = DateTime.Now;
            connection = new SQLiteConnection("data source=C:\\utils\\data\\erza.sqlite");
            connection.Open();
            Console.WriteLine("Получаю список файлов из БД...");
            imgs = new List<img_info>();
            bad_files = new List<string>();
            Console.CancelKeyPress += new ConsoleCancelEventHandler(OnExit);
            using (SQLiteCommand command = new SQLiteCommand())
            {
                command.CommandText = "SELECT image_id, file_path FROM images WHERE file_path IS NOT NULL AND (width = 0 OR height = 0)";
                command.Connection = connection;
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        img_info img = new img_info();
                        img.ImageID = reader.GetInt64(0);
                        img.Path = reader.GetString(1);
                        imgs.Add(img);
                    }
                    reader.Close();
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Завершено");
            Console.ResetColor();
            for (int i = 0; i < imgs.Count; i++)
            {
                Console.Write($"Обрабатываю: {Path.GetFileName(imgs[i].Path)} [{i + 1}/{imgs.Count}]");
                try
                {
                    Size s = GetImageSize(imgs[i].Path);
                    imgs[i].Height = s.Height;
                    imgs[i].Width = s.Width;
                    //imgs[i].Ratio = Math.Round((double)imgs[i].Width / (double)imgs[i].Height, 2);
                    //ErzaDB.SetImageResolution(imgs[i].ImageID, s.Width, s.Height, connection);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(" Обработан");
                    Console.ResetColor();
                }
                catch (Exception)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(" Ошибка!");
                    Console.ResetColor();
                    bad_images++;
                    bad_files.Add(imgs[i].Path);
                }
            }
            ExitAzusa();
        }
        static Size GetImageSize(string Path)
        {
            try
            {
                using (var image = new MagickImage())
                {
                    // 2. Пингуем файл (читаются только заголовки и метаданные)
                    image.Ping(Path);

                    // Теперь доступны размеры без декодирования пикселей
                    //Console.WriteLine($"Ширина: {image.Width}, Высота: {image.Height}");
                    Size s = new Size();
                    s.Height = (int)image.Height;
                    s.Width = (int)image.Width;
                    taglib_count++;
                    return s;

                    // 3. Извлекаем EXIF-метаданные
                    /*var exifProfile = image.GetExifProfile();
                    if (exifProfile != null)
                    {
                        var dateTime = exifProfile.GetValue(ExifTag.DateTime);
                        if (dateTime != null)
                        {
                            Console.WriteLine($"Дата съемки: {dateTime.Value}");
                        }
                    }

                    // Также можно читать IPTC или XMP
                    var iptcProfile = image.GetIptcProfile();*/
                }
            }
            catch (Exception)
            {
            }
            try
            {
                return ImageHelper.GetDimensions(Path);
            }
            catch (Exception)
            {
            }
            throw new Exception();
        }
        protected static void OnExit(object sender, ConsoleCancelEventArgs args)
        {
            Thread.Sleep(500);
            ExitAzusa();
        }
        static void ExitAzusa()
        {
            int count_file = 0;
            int all_files = imgs.Count;
            SQLiteTransaction transact = connection.BeginTransaction();
            foreach (img_info img in imgs)
            {
                if (img.Width > 0 && img.Height > 0)
                {
                    count_file++;
                    ErzaDB.SetImageResolution(img.ImageID, img.Width, img.Height, connection);
                    Console.WriteLine("Фаил {0} добавлен. [{1}/{2}]", Path.GetFileName(img.Path), count_file, all_files);
                }
            }
            transact.Commit();
            DateTime finish = DateTime.Now;
            Console.WriteLine("\nФайлов обработано: {0} за: {1} секунд ({2} в секунду)", count_file, (finish - start).TotalSeconds.ToString("0.00"), (count_file / (finish - start).TotalSeconds));
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Не правильных файлов: {bad_images}");
            Console.ResetColor();
            foreach (string bad_file in bad_files) { Console.Write(bad_file); }
            connection.Close();
            Console.WriteLine("\nВыход");
            Environment.Exit(0);
        }
    }
    class img_info
    {
        public long ImageID;
        public string hash;
        public int Width = 0;
        public int Height = 0;
        public double Ratio;
        public string Format;
        public string Path;
    }
}
