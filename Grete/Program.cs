using System.Data.SQLite;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ErzaLib;
using Shipwreck.Phash;
using System.Drawing;
using Shipwreck.Phash.Bitmaps;

namespace Grete
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<ImageInfo> imgs = new List<ImageInfo>();
            SQLiteConnection connection = new SQLiteConnection(@"data source = C:\utils\data\erza.sqlite");
            connection.Open();
            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                command.CommandText = "SELECT images.image_id, images.file_path FROM images LEFT OUTER JOIN phashs on images.image_id = phashs.image_id WHERE phashs.phash IS NULL AND images.file_path IS NOT NULL;";
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ImageInfo image = new ImageInfo();
                        image.ImageID = (long)reader["image_id"];
                        object o = reader["file_path"];
                        if (o != DBNull.Value)
                        {
                            image.FilePath = (string)o;
                        }
                        imgs.Add(image);
                    }
                }
            }
            for (int i = 0; i < imgs.Count; i++)
            {
                //PHash
                var bitmap = (Bitmap)Image.FromFile(imgs[i].FilePath);
                var phash = ImagePhash.ComputeDigest(bitmap.ToLuminanceImage());
                using (SQLiteCommand insert_command = new SQLiteCommand(connection))
                {
                    insert_command.CommandText = "insert into phashs (image_id, phash) values (@image_id, @phash)";
                    insert_command.Parameters.AddWithValue("image_id", imgs[i].ImageID);
                    insert_command.Parameters.AddWithValue("phash", phash.Coefficients);
                    insert_command.ExecuteNonQuery();
                }
            }
            connection.Close();
        }
    }
}