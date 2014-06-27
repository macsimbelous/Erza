/* Copyright © Macsim Belous 2012 */
/* This file is part of GenSlideShowFroXnView.

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
using System.IO;
using System.Diagnostics;

namespace GenSlideShowFroXnView
{
    class Program
    {
        static void Main(string[] args)
        {
            SlideShowXnView ss = new SlideShowXnView();
            ss.images = find_sqlite(args);
            string temp_file = Path.GetTempFileName();
            ss.GenerateSlideShow(temp_file);
            //Запускаем слайдшоу
            Process myProcess = new Process();
            myProcess.StartInfo.FileName = Properties.Settings1.Default.XnViewPath;
            myProcess.StartInfo.Arguments = temp_file;
            myProcess.EnableRaisingEvents = true;
            myProcess.Start();
            myProcess.WaitForExit();
            System.IO.File.Delete(temp_file);
        }
        static List<string> find_sqlite(string[] tags)
        {
            List<long> tag_ids = new List<long>();
            List<long> image_ids = new List<long>();

            //Получаем ИД тегов
            using (SQLiteConnection connection = new SQLiteConnection(Properties.Settings1.Default.ConnectionString))
            {
                connection.Open();
                /*string sql = "SELECT * FROM hash_tags WHERE";
                for (int i = 0; i < tags.Length; i++)
                {
                    if (i != 0)
                    {
                        sql = sql + " OR";
                    }
                    sql = sql + " (tags like '% " + tags[i].Replace("'", "''") + " %' or tags like '" + tags[i].Replace("'", "''") + " %' or tags like '% " + tags[i].Replace("'", "''") + "' or tags = '" + tags[i].Replace("'", "''") + "')";
                }*/
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT * FROM hash_tags WHERE");
                for (int i = 0; i < tags.Length; i++)
                {
                    if (i != 0)
                    {
                        sql.Append(" OR");
                    }
                    sql.Append(" (tags like '% ");
                    sql.Append(tags[i].Replace("'", "''"));
                    sql.Append(" %' or tags like '");
                    sql.Append(tags[i].Replace("'", "''"));
                    sql.Append(" %' or tags like '% ");
                    sql.Append(tags[i].Replace("'", "''"));
                    sql.Append("' or tags = '");
                    sql.Append(tags[i].Replace("'", "''"));
                    sql.Append("')");
                }
                SQLiteCommand command = new SQLiteCommand(sql.ToString(), connection);
                SQLiteDataReader reader = command.ExecuteReader();
                List<string> imgs = new List<string>();
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
                return imgs;
            }
        }
    }
}
