﻿/* Copyright © Macsim Belous 2012 */
/* This file is part of Misaka.

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
using System.Data.SQLite;
using System.Data;
using System.IO;
using ErzaLib;

namespace Misaka
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length <= 0) 
            { 
                Console.WriteLine("Не задан режим работы!");
                return;
            }
            if (!((args[0] == "-delete") | (args[0] == "-vipe")))
            { 
                Console.WriteLine("Параметр не опознан!");
                return;
            }
            //string connection_string = "data source=\"C:\\Temp\\erza.sqlite\"";
            string connection_string = "data source=C:\\utils\\Erza\\erza.sqlite";
            List<ImageInfo> img_list = new List<ImageInfo>();
            using (SQLiteConnection connection = new SQLiteConnection(connection_string))
            {
                connection.Open();
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
                        Console.Write("\r" + count.ToString("#######"));
                    }
                    reader.Close();
                    Console.WriteLine("\rВсего: " + (count++).ToString());
                }
            }
            if (img_list.Count <= 0)
            {
                Console.WriteLine("Ничего нет!");
                //Console.ReadKey();
                return;
            }
            using (SQLiteConnection connection = new SQLiteConnection(connection_string))
            {
                int count_deleted = 0;
                int count_file = 0;
                int all_files = img_list.Count;
                DateTime start = DateTime.Now;
                connection.Open();
                SQLiteTransaction transact = connection.BeginTransaction();
                foreach (ImageInfo img in img_list)
                {
                    count_file++;
                    if (System.IO.File.Exists(img.FilePath))
                    {
                        Console.WriteLine("Фаил {0} есть. [{1}/{2}]", Path.GetFileName(img.FilePath), count_file, all_files);
                    }
                    else
                    {
                        count_deleted++;
                        switch (args[0])
                        {
                            case "-delete":
                                //DeleteImageDB(img.id, connection);
                                ErzaDB.DeleteImage(img.ImageID, connection);
                                Console.WriteLine("Фаил {0} отсутствует. Помечен удалённым. [{1}/{2}]", Path.GetFileName(img.FilePath), count_file, all_files);
                                break;
                            case "-vipe":
                                //VipeImageDB(img.id, connection);
                                ErzaDB.VipeImage(img.ImageID, connection);
                                Console.WriteLine("Фаил {0} отсутствует. Данные о файле уничтожены! [{1}/{2}]", Path.GetFileName(img.FilePath), count_file, all_files);
                                break;
                        }
                    }
                }
                transact.Commit();
                DateTime finish = DateTime.Now;
                Console.WriteLine("Файлов проверено: {0} за: {1} секунд ({2} в секунду)", img_list.Count, (finish - start).TotalSeconds.ToString("0.00"), (img_list.Count / (finish - start).TotalSeconds));
                Console.WriteLine("Файлов отсутствует: {0}", count_deleted);
            }
            //Console.ReadKey();
        }
        static void DeleteImageDB(long id, SQLiteConnection connection)
        {
            using (SQLiteCommand update_command = new SQLiteCommand(connection))
            {
                update_command.CommandText = "UPDATE hash_tags SET file_name = @file_name, tags = @tags, is_new = @is_new, is_deleted = @is_deleted WHERE id = @id";
                update_command.Parameters.AddWithValue("is_new", false);
                update_command.Parameters.AddWithValue("is_deleted", true);
                update_command.Parameters.AddWithValue("id", id);
                update_command.Parameters.AddWithValue("tags", null);
                update_command.Parameters.AddWithValue("file_name", null);
                update_command.ExecuteNonQuery();
            }
        }
        static void VipeImageDB(long id, SQLiteConnection connection)
        {
            using (SQLiteCommand update_command = new SQLiteCommand(connection))
            {
                update_command.CommandText = "DELETE FROM hash_tags WHERE id = @id";
                update_command.Parameters.AddWithValue("id", id);
                update_command.ExecuteNonQuery();
            }
        }
    }
}
