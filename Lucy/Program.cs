using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;
using System.IO;

namespace Lucy
{
    class Program
    {
        static void Main(string[] args)
        {
            string ConnectionString = @"data source=C:\Users\macs\Dropbox\utils\Erza\erza.sqlite";
            string tag;
            if (args.Length <= 0)
            {
                Console.WriteLine("Укажите тег!");
                return; 
            }
            else
            {
                tag = args[0];
            }
            string prefix_path1 = "d:\\pictures\\animeart\\media\\";
            string prefix_path2 = "d:\\pictures\\animeart\\художники\\";
            string dest_path = "d:\\pictures\\animeart\\unsorted\\";
            List<string> imgs = new List<string>();
            imgs.AddRange(GetFilesFromImageDB(ConnectionString, tag, prefix_path1));
            imgs.AddRange(GetFilesFromImageDB(ConnectionString, tag, prefix_path2));
            foreach (string img in imgs)
            {
                Console.WriteLine("{0} >> {1}", img, dest_path + Path.GetFileName(img));
                File.Move(img, dest_path + Path.GetFileName(img));
            }
        }
        static List<string> GetFilesFromImageDB(string ConnectionString, string tag, string prefix_path) 
        {
            List<string> imgs = new List<string>();
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string sql = "select file_name from hash_tags where (tags like '% " + tag + " %' or tags like '" + tag + " %' or tags like '% " + tag + "' or tags = '" + tag + "') AND (file_name like '" + prefix_path + "%')";
                SQLiteCommand command = new SQLiteCommand(sql, connection);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (Convert.IsDBNull((object)reader["file_name"]))
                    {
                        continue;
                    }
                    else
                    {
                        imgs.Add((string)reader["file_name"]);
                    }
                }
                reader.Close();
            }
            return imgs;
        }
    }
}
