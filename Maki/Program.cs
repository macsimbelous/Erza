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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using ErzaLib;
using System.IO;
using System.Data;
using System.Data.SQLite;
using ImageMagick;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using WebP.Net;

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
            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 80L);
            myEncoderParameters.Param[0] = myEncoderParameter;
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

                    //Directory.CreateDirectory(PreviewPath + "\\" + hash[0] + "\\" + hash[1]);
                    if (Path.GetExtension(files_to_preview[i]).ToLower() == ".webp")
                    {
                        using var webp = new WebPObject(File.ReadAllBytes(files_to_preview[i]));
                        var m = new MagickFactory();
                        Bitmap bitmap = new Bitmap(webp.GetImage());
                        MagickImage image = new MagickImage(m.Image.Create(bitmap));

                        image.Resize(p_size);
                        image.SetCompression(CompressionMethod.JPEG);
                        image.Quality = 80;
                        image.Write(dest_file);
                        
                    }
                    else
                    {
                        using (MagickImage img = new MagickImage(files_to_preview[i]))
                        {
                            img.Resize(p_size);
                            img.SetCompression(CompressionMethod.JPEG);
                            img.Quality = 80;
                            img.Write(dest_file);
                        }
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
                /*Bitmap preview = CreateThumbnail(files_to_preview[i], PreviewWidth, PreviewHeight);
                if (preview != null)
                {
                    string dest_file = PreviewPath + "\\" + hash[0] + "\\" + hash[1] + "\\" + hash + ".jpg";
                    Directory.CreateDirectory(PreviewPath + "\\" + hash[0] + "\\" + hash[1]);
                    try
                    {
                        using (FileStream bw = new FileStream(dest_file, FileMode.Create))
                        {
                            preview.Save(bw, jpgEncoder, myEncoderParameters);
                            bw.Close();
                        }
                        Console.WriteLine($"[{i + 1}/{files_to_preview.Count}] {files_to_preview[i]} Добавлен");
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(ex.Message);
                        Console.ResetColor();
                    }
                    
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{files_to_preview[i]} Ошибка!");
                    Console.ResetColor();
                    bad_files.Add(files_to_preview[i]);
                }*/
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
                        if (img.IsDeleted)
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
            /*List<string> exist_files = new List<string>();
            foreach (string file in files)
            {
                if (ImageInfo.IsImageFile(file))
                {
                    exist_files.Add(Path.GetFileNameWithoutExtension(file));
                }
            }
            exist_files.Sort();
            for(int i = 0; i < hashs.Count; i++)
            {
                string hash = hashs[i];
                int index = exist_files.BinarySearch(hash);
                if(index < 0)
                {
                    File.Delete(PreviewPath + "\\" + hash[0] + "\\" + hash[1] + "\\" + hash + ".jpg");
                    Console.WriteLine("{0} Удалён!", hash);
                }
                else
                {
                    Console.WriteLine("{0} Присутствует!", hash);
                }
            }*/
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
