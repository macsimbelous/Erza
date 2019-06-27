using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErzaLib
{
    public class PreviewDB
    {
        public static void ReCreatePreviewToDB(string Hash, string FilePath, int Width, int Height, SQLiteConnection Connection)
        {
            using (Bitmap preview = CreatePreview(FilePath, Width, Height))
            {
                if (preview != null)
                {
                    byte[] array = PreviewToJpeg(preview);
                    UpdatePreviewToDB(Hash, array, Connection);
                }
            }
        }
        public static byte[] PreviewToJpeg(Bitmap preview)
        {
            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 80L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            if (preview != null)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    preview.Save(stream, jpgEncoder, myEncoderParameters);
                    return stream.ToArray();
                }
            }
            else
            {
                return null;
            }
        }
        public static Bitmap CreatePreview(string lcFilename, int lnWidth, int lnHeight)
        {
            System.Drawing.Bitmap bmpOut = null;
            try
            {
                Bitmap loBMP = new Bitmap(lcFilename);
                ImageFormat loFormat = loBMP.RawFormat;

                int lnNewWidth = 0;
                int lnNewHeight = 0;

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
        public static void UpdatePreviewToDB(string Hash, byte[] Preview, SQLiteConnection Connection)
        {
            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = "UPDATE previews SET preview = @preview WHERE hash = @hash";
                command.Parameters.AddWithValue("hash", Hash);
                command.Parameters.AddWithValue("preview", Preview);
                command.ExecuteNonQuery();
            }
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
    }
}
