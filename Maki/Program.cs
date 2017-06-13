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
            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 80L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            SQLiteTransaction transact = conn.BeginTransaction();
            foreach (string file in files)
            {
                if (!ImageInfo.IsImageFile(file)) { continue; }
                string hash = Path.GetFileNameWithoutExtension(file);
                if (ExistPreview(hash, conn))
                {
                    Console.WriteLine(file + " Уже есть в БД");
                }
                else
                {
                    Bitmap preview = CreateThumbnail(file, 200, 150);
                    using(MemoryStream stream = new MemoryStream())
                    {
                        preview.Save(stream, jpgEncoder, myEncoderParameters);
                        LoadPreviewToDB(hash, stream.ToArray(), conn);
                    }
                    Console.WriteLine(file + " Добавлен");
                }
            }
            transact.Commit();
            List<string> hashs = ReadAllHashFromPrewiewsDB(conn);
            Console.WriteLine();
            string erzadb = "data source=C:\\utils\\erza\\erza.sqlite";
            using (SQLiteConnection erza_conn = new SQLiteConnection(erzadb))
            {
                erza_conn.Open();
                transact = conn.BeginTransaction();
                foreach (string hash in hashs)
                {
                    ImageInfo img = ErzaDB.GetImageWithOutTags(hash, erza_conn);
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
                transact.Commit();
            }
            using (SQLiteCommand command = conn.CreateCommand())
            {
                command.CommandText = "vacuum;";
                command.ExecuteNonQuery();
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
