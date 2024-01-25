using System.Data.SQLite;
using System.Data;

namespace Lora
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<string> img_wo_tags = new List<string>();
            using (SQLiteConnection connection = new SQLiteConnection("data source=C:\\utils\\data\\erza.sqlite"))
            {
                connection.Open();
                long count_rows;
                //Определяем число записей
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.CommandText = "SELECT count(*) FROM images LEFT OUTER JOIN image_tags on images.image_id = image_tags.image_id WHERE images.is_deleted = 0 AND image_tags.image_id IS NULL;";
                    command.Connection = connection;
                    count_rows = System.Convert.ToInt64(command.ExecuteScalar());
                }
                //Считываем записи
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.CommandText = "SELECT images.hash FROM images LEFT OUTER JOIN image_tags on images.image_id = image_tags.image_id WHERE images.is_deleted = 0 AND image_tags.image_id IS NULL;";
                    command.Connection = connection;
                    SQLiteDataReader reader = command.ExecuteReader();
                    int count = 0;
                    while (reader.Read())
                    {
                        string hash = (string)reader["hash"];
                        img_wo_tags.Add(hash);
                        count++;
                        Console.Write("Считано: {0} из {1}\r", count, count_rows);
                    }
                }
            }
        }
    }
}
