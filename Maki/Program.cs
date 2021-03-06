﻿/* Copyright © Macsim Belous 2012 */
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using ErzaLib;
using System.IO;
using System.Data;
using System.Data.SQLite;

namespace Maki
{
    class Program
    {
        static void Main(string[] args)
        {
            const int PreviewWidth = 300;
            const int  PreviewHeight = 225;
            //string previews = "data source=C:\\utils\\data\\previews.sqlite";
            string previews = "data source=E:\\previews.sqlite";
            List<string> bad_files = new List<string>();
            //string previews = @"data source=C:\Users\maksim\Source\Repos\Erza\Ange\bin\Debug\Previews.sqlite";
            SQLiteConnection conn = new SQLiteConnection(previews);
            conn.Open();
            string[] files = Directory.GetFiles("F:\\AnimeArt", "*.*", SearchOption.AllDirectories);
            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 80L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            List<string> files_to_preview = new List<string>();
            for (int i=0;i< files.Length;i++)
            {
                if (!ImageInfo.IsImageFile(files[i])) { continue; }
                string hash = Path.GetFileNameWithoutExtension(files[i]);
                if (ExistPreview(hash, conn))
                {
                    Console.WriteLine($"[{i + 1}/{files.Length}] {files[i]} Уже есть в БД");
                }
                else
                {
                    files_to_preview.Add(files[i]);
                }
            }
            Console.WriteLine("Добавляем превьюшки новых картинок.");
            SQLiteTransaction transact = conn.BeginTransaction();
            for (int i=0; i< files_to_preview.Count;i++)
            {
                string hash = Path.GetFileNameWithoutExtension(files_to_preview[i]);
                Bitmap preview = CreateThumbnail(files_to_preview[i], PreviewWidth, PreviewHeight);
                if (preview != null)
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        preview.Save(stream, jpgEncoder, myEncoderParameters);
                        try
                        {
                            LoadPreviewToDB(hash, stream.ToArray(), conn);
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(ex.Message);
                            Console.ResetColor();
                        }
                    }
                    Console.WriteLine($"[{i+1}/{files_to_preview.Count}] {files_to_preview[i]} Добавлен");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{files_to_preview[i]} Ошибка!");
                    Console.ResetColor();
                    bad_files.Add(files_to_preview[i]);
                }
            }
            transact.Commit();
            List<string> hashs = ReadAllHashFromPrewiewsDB(conn);
            Console.WriteLine();
            string erzadb = "data source=C:\\utils\\data\\erza.sqlite";
            using (SQLiteConnection erza_conn = new SQLiteConnection(erzadb))
            {
                erza_conn.Open();
                transact = conn.BeginTransaction();
                foreach (string hash in hashs)
                {
                    ImageInfo img = ErzaDB.GetImageWithOutTags(hash, erza_conn);
                    if (img != null)
                    {
                        if (img.IsDeleted)
                        {
                            RomovePreviewFromDB(hash, conn);
                            Console.WriteLine("{0} Удалён!", hash);
                        }
                        else
                        {
                            Console.WriteLine("{0} Присутствует!", hash);
                        }
                    }
                    else
                    {
                        RomovePreviewFromDB(hash, conn);
                        Console.WriteLine("{0} Удалён!", hash);
                    }
                }
                transact.Commit();
            }
            if ((args.Length > 0) && (args[0] == "--vacuum"))
            {
                Console.WriteLine("Сжимаем БД.");
                using (SQLiteCommand command = conn.CreateCommand())
                {
                    command.CommandText = "vacuum;";
                    command.ExecuteNonQuery();
                }
            }
            conn.Close();
            foreach(string s in bad_files)
            {
                Console.WriteLine(s);
            }
            Console.WriteLine($"Ошибок {bad_files.Count}");
        }
        public static bool ExistPreview(string hash, SQLiteConnection conn)
        {
            using(SQLiteCommand command = new SQLiteCommand(conn))
            {
                command.CommandText = "SELECT hash FROM previews WHERE hash = @hash";
                command.Parameters.AddWithValue("hash", hash);
                object o = command.ExecuteScalar();
                if(o == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        public static Bitmap CreateThumbnail(string lcFilename, int lnWidth, int lnHeight)
        {
            System.Drawing.Bitmap bmpOut = null;
            try
            {
                Bitmap loBMP = new Bitmap(lcFilename);
                ImageFormat loFormat = loBMP.RawFormat;

                //decimal lnRatio;
                int lnNewWidth = 0;
                int lnNewHeight = 0;

                //*** If the image is smaller than a thumbnail just return it
                if (loBMP.Width < lnWidth && loBMP.Height < lnHeight)
                    return loBMP;

                float temp = (float)loBMP.Width / (float)lnWidth;
                if ((int)((float)loBMP.Height / temp) > lnHeight)
                {
                    temp = (float)loBMP.Height / (float)lnHeight;
                    lnNewWidth = (int)((float)loBMP.Width / temp);
                    lnNewHeight = lnHeight;
                }
                else
                {
                    lnNewWidth = lnWidth;
                    lnNewHeight = (int)((float)loBMP.Height / temp);
                }
                /*if (loBMP.Width > loBMP.Height)
                {
                    lnRatio = (decimal)lnWidth / loBMP.Width;
                    lnNewWidth = lnWidth;
                    decimal lnTemp = loBMP.Height * lnRatio;
                    lnNewHeight = (int)lnTemp;
                }
                else
                {
                    lnRatio = (decimal)lnHeight / loBMP.Height;
                    lnNewHeight = lnHeight;
                    decimal lnTemp = loBMP.Width * lnRatio;
                    lnNewWidth = (int)lnTemp;
                }*/
                bmpOut = new Bitmap(lnNewWidth, lnNewHeight);
                Graphics g = Graphics.FromImage(bmpOut);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.FillRectangle(Brushes.White, 0, 0, lnNewWidth, lnNewHeight);
                g.DrawImage(loBMP, 0, 0, lnNewWidth, lnNewHeight);

                loBMP.Dispose();
            }
            catch
            {
                return null;
            }

            return bmpOut;
        }
        public static ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
        public static void LoadPreviewToDB(string Hash, byte[] Preview, SQLiteConnection Connection)
        {
            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = "INSERT INTO previews(hash, preview) VALUES(@hash, @preview);";
                command.Parameters.AddWithValue("hash", Hash);
                command.Parameters.AddWithValue("preview", Preview);
                command.ExecuteNonQuery();
            }
        }
        public static List<string> ReadAllHashFromPrewiewsDB(SQLiteConnection Connection)
        {
            List<string> hashs = new List<string>();
            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = "SELECT hash FROM previews";
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    int count = 0;
                    while (reader.Read())
                    {
                        string hash = (string)reader["hash"];
                        hashs.Add(hash);
                        count++;
                        Console.Write("Считано: {0:##########}\r", count);
                    }
                }
            }
            return hashs;
        }
        public static void RomovePreviewFromDB(string Hash, SQLiteConnection Connection)
        {
            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = "DELETE FROM previews WHERE hash = @hash";
                command.Parameters.AddWithValue("hash", Hash);
                command.ExecuteNonQuery();
            }
        }
    }
}
