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
            List<CImage> il = new List<CImage>();
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
                    if (IsImageFile(file))
                    {
                        count++;
                        Console.WriteLine("Подсчитываю хэш {0} [{1}/{2}]", Path.GetFileName(file), count, files_count);
                        CImage img = new CImage();
                        img.hash = CalculateHashFile(file);
                        img.hash_str = BitConverter.ToString(img.hash).Replace("-", string.Empty);
                        img.hash_str = img.hash_str.ToLower();
                        img.file = file;
                        il.Add(img);
                    }
                }
            }
            else
            {
                int count = 0;
                foreach (string file in files)
                {
                    if (IsImageFile(file))
                    {
                        count++;
                        if (files_in_db.BinarySearch(file) >= 0)
                        {
                            Console.WriteLine("Фаил {0} уже есть в БД. [{1}/{2}]", Path.GetFileName(file), count, files_count);
                            continue;
                        }
                        Console.WriteLine("Подсчитываю хэш {0} [{1}/{2}]", Path.GetFileName(file), count, files_count);
                        CImage img = new CImage();
                        img.hash = CalculateHashFile(file);
                        img.hash_str = BitConverter.ToString(img.hash).Replace("-", string.Empty);
                        img.hash_str = img.hash_str.ToLower();
                        img.file = file;
                        il.Add(img);
                    }
                }
            }
            Console.WriteLine("Вычислено хэшей: {0}", il.Count);
            ImageToDB(il);
            //Console.ReadKey();
        }
        static void ImageToDB(List<CImage> il)
        {
            Console.WriteLine("Добавляем хэши в базу данных SQLite");
            using (SQLiteConnection connection = new SQLiteConnection(connection_string))
            {
                DateTime start = DateTime.Now;
                connection.Open();
                SQLiteTransaction transact = connection.BeginTransaction();
                for (int i2 = 0; i2 < il.Count; i2++)
                {
                    Console.Write("Обрабатываю хэш {0} ({1}/{2})\r", il[i2].hash_str, (i2 + 1), il.Count);
                    using (SQLiteCommand command = new SQLiteCommand())
                    {
                        command.CommandText = "select * from hash_tags where hash = @hash";
                        //command.Parameters.Add("hash", DbType.Binary, 16).Value = il[i2].hash;
                        command.Parameters.AddWithValue("hash", il[i2].hash);
                        command.Connection = connection;
                        SQLiteDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            ulong id = System.Convert.ToUInt64(reader["id"]);
                            bool is_deleted = System.Convert.ToBoolean(reader["is_deleted"]);
                            reader.Close();
                            if (is_deleted)
                            {
                                il[i2].is_deleted = true;
                                continue;
                            }
                            using (SQLiteCommand update_command = new SQLiteCommand(connection))
                            {
                                update_command.CommandText = "UPDATE hash_tags SET file_name = @file_name WHERE id = @id";
                                update_command.Parameters.AddWithValue("id", id);
                                update_command.Parameters.AddWithValue("file_name", il[i2].file);
                                update_command.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            reader.Close();
                            using (SQLiteCommand insert_command = new SQLiteCommand(connection))
                            {
                                insert_command.CommandText = "insert into hash_tags (hash, file_name, is_new, is_deleted) values (@hash, @file_name, @is_new, @is_deleted)";
                                insert_command.Parameters.AddWithValue("hash", il[i2].hash);
                                insert_command.Parameters.AddWithValue("is_new", true);
                                insert_command.Parameters.AddWithValue("is_deleted", false);
                                insert_command.Parameters.AddWithValue("file_name", il[i2].file);
                                insert_command.ExecuteNonQuery();
                            }
                        }
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
        static bool IsImageFile(string s)
        {
            int t = s.LastIndexOf('.');
            if (t >= 0)
            {
                string ext = s.Substring(t).ToLower();
                switch (ext)
                {
                    case ".jpg":
                        return true;
                    //break;
                    case ".jpeg":
                        return true;
                    //break;
                    case ".png":
                        return true;
                    //break;
                    case ".bmp":
                        return true;
                    //break;
                    case ".gif":
                        return true;
                    //break;
                    case ".tif":
                        return true;
                    //break;
                    case ".tiff":
                        return true;
                    //break;

                }
            }
            return false;
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
                    command.CommandText = "select file_name from hash_tags where file_name IS NOT NULL";
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
    public class CImage
    {
        public bool is_new = true;
        public bool is_deleted = false;
        public long id;
        public byte[] hash;
        public string file = null;
        //public string file_url;
        public List<string> tags = new List<string>();
        public List<string> urls = new List<string>();
        public string hash_str;
        public string tags_string
        {
            get
            {
                string s = String.Empty;
                for (int i = 0; i < tags.Count; i++)
                {
                    if (i > 0)
                    {
                        s = s + " ";
                    }
                    s = s + tags[i];
                }
                return s;
            }
            set
            {
                string[] t = value.Split(' ');
                for (int i = 0; i < t.Length; i++)
                {
                    if (t[i].Length > 0)
                    {
                        tags.Add(t[i]);
                    }
                }
            }
        }
        public override string ToString()
        {
            if (this.file != String.Empty)
            {
                return file.Substring(file.LastIndexOf('\\') + 1);
            }
            else
            {
                return "No File!";
            }
        }
    }
}
