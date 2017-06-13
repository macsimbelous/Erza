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
            string previews = "data source=C:\\utils\\erza\\previews.sqlite";
            //string previews = @"data source=C:\Users\maksim\Source\Repos\Erza\Ange\bin\Debug\Previews.sqlite";
            SQLiteConnection conn = new SQLiteConnection(previews);
            conn.Open();
            string[] files = Directory.GetFiles("I:\\AnimeArt", "*.*", SearchOption.AllDirectories);
            foreach(string file in files)
            {
                if(ExistPreview(Path.GetFileNameWithoutExtension(file), conn))
                {
                    Console.WriteLine(file + " Уже есть в БД");
                }
                else
                {
                    Bitmap preview = CreateThumbnail(file, 200, 150);
                    
                }
            }
            conn.Close();
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

                decimal lnRatio;
                int lnNewWidth = 0;
                int lnNewHeight = 0;

                //*** If the image is smaller than a thumbnail just return it
                if (loBMP.Width < lnWidth && loBMP.Height < lnHeight)
                    return loBMP;

                if (loBMP.Width > loBMP.Height)
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
                }
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
    }
}
