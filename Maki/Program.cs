/* Copyright © Macsim Belous 2012 */
/* This file is part of Erza.

    Foobar is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Foobar is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Foobar.  If not, see <https://www.gnu.org/licenses/>.*/
using System;
using System.Collections.Generic;
using ErzaLib2;
using System.IO;
using System.Data.SQLite;
using ImageMagick;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace Maki
{
    class Program
    {
        public static string PreviewPath = "D:\\previews";
        static void Main(string[] args)
        {
            const int PreviewWidth = 300;
            const int  PreviewHeight = 225;
            //string previews = "data source=C:\\utils\\data\\previews.sqlite";
            //string previews = "data source=E:\\previews.sqlite";
            List<string> bad_files = new List<string>();
            Regex rx = new Regex("^[a-f0-9]{32}$", RegexOptions.Compiled);
            //string previews = @"data source=C:\Users\maksim\Source\Repos\Erza\Ange\bin\Debug\Previews.sqlite";
            //SQLiteConnection conn = new SQLiteConnection(previews);
            //conn.Open();
            string[] files = Directory.GetFiles("F:\\AnimeArt", "*.*", SearchOption.AllDirectories);
            List<string> files_to_preview = new List<string>();
            for (int i=0;i< files.Length;i++)
            {
                if (!ImageInfo.IsImageFile(files[i])) { continue; }
                string hash = Path.GetFileNameWithoutExtension(files[i]);
                if (ExistPreview(hash))
                {
                    Console.WriteLine($"[{i + 1}/{files.Length}] {files[i]} Уже есть в БД");
                }
                else
                {
                    files_to_preview.Add(files[i]);
                }
            }
            Console.WriteLine("Добавляем превьюшки новых картинок.");
            //SQLiteTransaction transact = conn.BeginTransaction();
            var p_size = new MagickGeometry(PreviewWidth, PreviewHeight);
            CreateSubDirs(PreviewPath);
            for (int i=0; i< files_to_preview.Count;i++)
            {
                Console.Write($"[{i+1}/{files_to_preview.Count}] {files_to_preview[i]}...");
                try
                {
                    string hash = Path.GetFileNameWithoutExtension(files_to_preview[i]);
                    if (!IsMD5(hash, rx))
                    {
                       hash = ComputeMD5(files_to_preview[i]);
                    }
                    string dest_file = PreviewPath + "\\" + hash[0] + "\\" + hash[1] + "\\" + hash + ".jpg";

                    using (MagickImage img = new MagickImage(files_to_preview[i]))
                    {
                        img.Resize(p_size);
                        img.Format = MagickFormat.Jpeg;
                        img.SetCompression(CompressionMethod.JPEG);
                        img.Quality = 80;
                        img.Write(dest_file);
                    }
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Успех!");
                    Console.ResetColor();
                }
                catch (Exception)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    //Console.WriteLine(ex.ToString());
                    Console.WriteLine("Ошибка!");
                    Console.ResetColor();
                    bad_files.Add(files_to_preview[i]);
                }
            }
            //transact.Commit();
            List<string> hashs = ReadAllHashFromPrewiewsDB();
            Console.WriteLine();
            string erzadb = "data source=C:\\utils\\data\\erza.sqlite";
            using (SQLiteConnection erza_conn = new SQLiteConnection(erzadb))
            {
                erza_conn.Open();
                foreach (string hash in hashs)
                {
                    string dest_file = PreviewPath + "\\" + hash[0] + "\\" + hash[1] + "\\" + hash + ".jpg";
                    ImageInfo img = ErzaDB.GetImageWithOutTags(hash, erza_conn);
                    if (img != null)
                    {
                        if (img.Deleted)
                        {
                            //RomovePreviewFromDB(hash, conn);
                            File.Delete(dest_file);
                            Console.WriteLine("{0} Удалён!", hash);
                        }
                        else
                        {
                            Console.WriteLine("{0} Присутствует!", hash);
                        }
                    }
                    else
                    {
                        //RomovePreviewFromDB(hash, conn);
                        File.Delete(dest_file);
                        Console.WriteLine("{0} Удалён!", hash);
                    }
                }

            }
            foreach(string s in bad_files)
            {
                Console.WriteLine(s);
            }
            Console.WriteLine($"Ошибок {bad_files.Count}");
        }
        public static bool ExistPreview(string hash)
        {
            return File.Exists(PreviewPath + "\\" + hash[0] + "\\" + hash[1] + "\\" + hash + ".jpg");
        }
        public static List<string> ReadAllHashFromPrewiewsDB()
        {
            string[] files = Directory.GetFiles(PreviewPath, "*.*", SearchOption.AllDirectories);
            List<string> hashs = new List<string>();
            foreach(string file in files)
            {
                if (ImageInfo.IsImageFile(file))
                {
                    hashs.Add(Path.GetFileNameWithoutExtension(file));
                }
            }
            return hashs;
        }
        public static bool IsMD5(string Text, Regex rx)
        {
            Match match = rx.Match(Text);
            if (match.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static string ComputeMD5(string FilePath)
        {
            MD5 hash_enc = MD5.Create();
            FileStream fsData = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
            byte[] hash = hash_enc.ComputeHash(fsData);
            fsData.Close();
            return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
        }
        public static void CreateSubDirs(string Path)
        {
            string[] Hex = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f" };
            foreach (string first in Hex)
            {
                foreach (string second in Hex)
                {
                    string p = Path + "\\" + first + "\\" + second;
                    if (!Directory.Exists(p))
                    {
                        Directory.CreateDirectory(p);
                    }
                }
            }
        }
    }
}
