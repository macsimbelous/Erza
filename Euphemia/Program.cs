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
        static string connection_string = "data source=C:\\utils\\Erza\\erza.sqlite";
        static string dir = null;
        static void Main(string[] args)
        {
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
            string[] files = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length;i++ )
            {
                files[i] = files[i].ToLower();
            }
            List<string> files_in_db = GetFilesFromImageDB();
            files_in_db.Sort();
            int files_count = files.Length;
            if (FullMode == true)
            {
                int count = 0;
                foreach (string file in files)
                {
                    if (ImageInfo.IsImageFile(file))
                    {
                        count++;
                        Console.WriteLine("Подсчитываю хэш {0} [{1}/{2}]", Path.GetFileName(file), count, files_count);
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
                foreach (string file in files)
                {
                    if (ImageInfo.IsImageFile(file))
                    {
                        count++;
                        if (files_in_db.BinarySearch(file) >= 0)
                        {
                            Console.WriteLine("Фаил {0} уже есть в БД. [{1}/{2}]", Path.GetFileName(file), count, files_count);
                            continue;
                        }
                        Console.WriteLine("Подсчитываю хэш {0} [{1}/{2}]", Path.GetFileName(file), count, files_count);
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
            ImageToDB(il);
            //Console.ReadKey();
        }
        static void ImageToDB(List<ImageInfo> il)
        {
            Console.WriteLine("Добавляем хэши в базу данных SQLite");
            using (SQLiteConnection connection = new SQLiteConnection(connection_string))
            {
                DateTime start = DateTime.Now;
                connection.Open();
                SQLiteTransaction transact = connection.BeginTransaction();
                for (int i2 = 0; i2 < il.Count; i2++)
                {
                    Console.Write("Обрабатываю хэш {0} ({1}/{2})\r", il[i2].Hash, (i2 + 1), il.Count);
                    ImageInfo temp = ErzaDB.GetImageWithOutTags(il[i2].Hash, connection);
                    if(temp != null)
                    {
                        if (temp.IsDeleted)
                        {
                            il[i2].IsDeleted = true;
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
                DateTime finish = DateTime.Now;
                //Console.WriteLine(finish.ToString());
                Console.WriteLine("\nХэшей добавлено: " + il.Count.ToString() + " за: " + (finish - start).TotalSeconds.ToString("0.00") + " секунд (" + (il.Count / (finish - start).TotalSeconds) + " в секунду)");
            }
        }
        static bool MoveImageToStore(string file, string StorePath)
        {
            MD5 hash_enc = MD5.Create();
            FileStream fsData = new FileStream(file, FileMode.Open, FileAccess.Read);
            byte[] hash = hash_enc.ComputeHash(fsData);
            fsData.Close();
            string t = BitConverter.ToString(hash).Replace("-", string.Empty);
            string ext = file.Substring(file.LastIndexOf('.'));
            if (ext.ToLower() == ".jpeg") { ext = ".jpg"; }
            string DestFile = StorePath + "\\" + t.Substring(0, 2) + "\\" + t.Substring(2, 2);
            Directory.CreateDirectory(DestFile.ToLower());
            DestFile = DestFile + "\\" + t + ext;
            try
            {
                System.IO.File.Move(file, DestFile.ToLower());
                System.Console.WriteLine(file.Substring(file.LastIndexOf('\\') + 1) + " -> " + t.Substring(file.LastIndexOf('\\') + 1).ToLower());
            }
            catch (IOException)
            {
                System.Console.WriteLine("ДУБЛИКАТ!!! " + file);
                return false;
            }
            return true;
        }
        static byte[] CalculateHashFile(string file)
        {
            MD5 hash_enc = MD5.Create();
            FileStream fsData = new FileStream(file, FileMode.Open, FileAccess.Read);
            byte[] hash = hash_enc.ComputeHash(fsData);
            fsData.Close();
            return hash;
        }
        static bool ExistFileToImageDB(string file)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connection_string))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.CommandText = "select * from hash_tags where file_name = @file_name";
                    //command.Parameters.Add("hash", DbType.Binary, 16).Value = il[i2].hash;
                    command.Parameters.AddWithValue("file_name", file);
                    command.Connection = connection;
                    SQLiteDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        reader.Close();
                        return true;
                    }
                    else
                    {
                        reader.Close();
                        return false;
                    }
                }
            }
        }
        static List<string> GetFilesFromImageDB()
        {
            List<string> temp = new List<string>();
            using (SQLiteConnection connection = new SQLiteConnection(connection_string))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.CommandText = "select file_path from images where file_path IS NOT NULL";
                    command.Connection = connection;
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        temp.Add(((string)reader[0]).ToLower());
                    }
                    reader.Close();
                }
            }
            return temp;
        }
    }
}
