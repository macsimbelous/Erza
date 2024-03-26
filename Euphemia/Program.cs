/* Copyright © Macsim Belous 2012 */
/* This file is part of Euphemia.

    Foobar is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Foobar is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Foobar.  If not, see <http://www.gnu.org/licenses/>.*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Data;
using System.Data.SQLite;
using ErzaLib;

namespace Euphemia
{
    class Program
    {
        static public bool FullMode = false;
        //static string connection_string = "data source=\"C:\\Temp\\erza.sqlite\"";
        static string connection_string = "data source=C:\\utils\\data\\erza.sqlite";
        static string dir = null;
        static void Main(string[] args)
        {
            List<ImageInfo> deleted_imgs = new List<ImageInfo>();
            foreach (string option in args)
            {
                if (option == "-full")
                {
                    FullMode = true;
                }
                else
                {
                    dir = option;
                }
            }
            Console.WriteLine("Получаю список файлов из {0}", dir);
            List<ImageInfo> il = new List<ImageInfo>();
            List<string> files_in_dir = new List<string>(Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories));
            for (int i = 0; i < files_in_dir.Count;i++ )
            {
                files_in_dir[i] = files_in_dir[i].ToLower();
            }
            files_in_dir.Sort();
            using (SQLiteConnection connection = new SQLiteConnection(connection_string))
            {
                connection.Open();
                List<string> files_in_db = new List<string>();
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.CommandText = "select file_path from images where file_path IS NOT NULL";
                    command.Connection = connection;
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        files_in_db.Add(((string)reader[0]).ToLower());
                    }
                    reader.Close();
                }
                files_in_db.Sort();
                if (FullMode == true)
                {
                    int count = 0;
                    foreach (string file in files_in_dir)
                    {
                        if (ImageInfo.IsImageFile(file))
                        {
                            count++;
                            Console.WriteLine("Подсчитываю хэш {0} [{1}/{2}]", Path.GetFileName(file), count, files_in_dir.Count);
                            ImageInfo img = new ImageInfo();
                            byte[] temp = CalculateHashFile(file);
                            img.Hash = BitConverter.ToString(temp).Replace("-", string.Empty);
                            img.Hash = img.Hash.ToLower();
                            img.FilePath = file;
                            il.Add(img);
                        }
                    }
                }
                else
                {
                    int count = 0;
                    foreach (string file in files_in_dir)
                    {
                        if (ImageInfo.IsImageFile(file))
                        {
                            count++;
                            if (files_in_db.BinarySearch(file) >= 0)
                            {
                                Console.WriteLine("Фаил {0} уже есть в БД. [{1}/{2}]", Path.GetFileName(file), count, files_in_dir.Count);
                                continue;
                            }
                            Console.WriteLine("Подсчитываю хэш {0} [{1}/{2}]", Path.GetFileName(file), count, files_in_dir.Count);
                            ImageInfo img = new ImageInfo();
                            byte[] temp = CalculateHashFile(file);
                            img.Hash = BitConverter.ToString(temp).Replace("-", string.Empty);
                            img.Hash = img.Hash.ToLower();
                            img.FilePath = file;
                            il.Add(img);
                        }
                    }
                }
                Console.WriteLine("Вычислено хэшей: {0}", il.Count);
                Console.WriteLine("Добавляем хэши в базу данных SQLite");
                SQLiteTransaction transact = connection.BeginTransaction();
                for (int i2 = 0; i2 < il.Count; i2++)
                {
                    Console.Write("Обрабатываю хэш {0} ({1}/{2})\r", il[i2].Hash, (i2 + 1), il.Count);
                    ImageInfo temp = ErzaDB.GetImageWithOutTags(il[i2].Hash, connection);
                    if (temp != null)
                    {
                        if (temp.IsDeleted)
                        {
                            il[i2].IsDeleted = true;
                            deleted_imgs.Add(il[i2]);
                            continue;
                        }
                        ErzaDB.SetImagePath(il[i2].Hash, il[i2].FilePath, connection);
                    }
                    else
                    {
                        ErzaDB.AddImage(il[i2].Hash, false, il[i2].FilePath, 0, 0, connection);
                    }
                }
                transact.Commit();
                Console.WriteLine($"\nХэшей добавлено: {il.Count}");

                //Ищем отсутствующие файлы
                List <ImageInfo> img_list = new List<ImageInfo>();
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.CommandText = "SELECT image_id, file_path FROM images WHERE file_path IS NOT NULL ORDER BY file_path ASC";
                    command.Connection = connection;
                    SQLiteDataReader reader = command.ExecuteReader();
                    int count = 0;
                    while (reader.Read())
                    {
                        ImageInfo img = new ImageInfo();
                        img.ImageID = (long)reader["image_id"];
                        img.FilePath = (string)reader["file_path"];
                        img_list.Add(img);
                        count++;
                        Console.Write($"\rПолучаем список файлов из БД...{count.ToString("#######")}");
                    }
                    reader.Close();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("ОК");
                    Console.ResetColor();
                    //Console.WriteLine($"\rВсего: {count}");
                }
                if (img_list.Count <= 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Ничего нет!");
                    Console.ResetColor();
                    return;
                }
                int count_deleted = 0;
                int count_file = 0;
                int all_files = img_list.Count;
                SQLiteTransaction transact2 = connection.BeginTransaction();
                foreach (ImageInfo img in img_list)
                {
                    count_file++;
                    if (files_in_dir.BinarySearch(img.FilePath.ToLower()) >= 0)  //if (System.IO.File.Exists(img.FilePath))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Фаил {0} есть. [{1}/{2}]", Path.GetFileName(img.FilePath), count_file, all_files);
                        Console.ResetColor();
                    }
                    else
                    {
                        count_deleted++;
                        ErzaDB.DeleteImage(img.ImageID, connection);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Фаил {0} отсутствует. Помечен удалённым. [{1}/{2}]", Path.GetFileName(img.FilePath), count_file, all_files);
                        Console.ResetColor();
                    }
                }
                transact2.Commit();
                Console.WriteLine($"Файлов проверено: {img_list.Count} ");
                Console.WriteLine($"Файлов отсутствует: {count_deleted}");
                Console.WriteLine($"Присутствуют но помечены удалёнными: {deleted_imgs.Count}");
                if(deleted_imgs.Count > 0)
                {
                    string deleted_path = args[0] + "\\UnSorted\\deleted";
                    Console.Write($"Перемещаю помеченные удалёнными в {deleted_path}...");
                    Directory.CreateDirectory(deleted_path);
                    foreach (ImageInfo img in deleted_imgs)
                    {
                        File.Move(img.FilePath, deleted_path + "\\" + Path.GetFileName(img.FilePath));
                    }
                    Console.WriteLine("Закончено.");
                }
            }
        }
        static byte[] CalculateHashFile(string file)
        {
            MD5 hash_enc = MD5.Create();
            FileStream fsData = new FileStream(file, FileMode.Open, FileAccess.Read);
            byte[] hash = hash_enc.ComputeHash(fsData);
            fsData.Close();
            return hash;
        }
    }
}
