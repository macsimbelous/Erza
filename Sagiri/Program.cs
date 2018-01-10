/* Copyright © Macsim Belous 2017 */
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
using System.IO;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace Sagiri
{
    class Program
    {
        static Config config = null;
        static bool AllComputeHash = false;
        static bool Recurse = false;
        static void Main(string[] args)
        {
            LoadSettings();
            string dest_path = config.TargetPath;
            string[] Hex = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f" };
            List<string> error_files = new List<string>();
            Regex rx = new Regex("^[a-f0-9]{32}$", RegexOptions.Compiled);

            ParseArgs(args);
            foreach (string first in Hex)
            {
                foreach (string second in Hex)
                {
                    string path = dest_path + "\\" + first + "\\" + second;
                    if (Directory.Exists(path))
                    {
                        continue;
                    }
                    else
                    {
                        Directory.CreateDirectory(path);
                    }
                }
            }
            int error = 0;
            string[] files;
            //if (args.Length > 0 && String.Compare(args[0], "-r", false) == 0)
            if (Program.Recurse)
            {
                files = Directory.GetFiles(config.SourcePath, "*.*", SearchOption.AllDirectories);
            }
            else
            {
                files = Directory.GetFiles(config.SourcePath, "*.*", SearchOption.TopDirectoryOnly);
            }
            //string[] files = Directory.GetFiles("I:\\AnimeArt\\UnSorted", "*.*", SearchOption.AllDirectories);
            for(int i = 0; i < files.Length; i++)
            {
                if (IsImageFile(files[i]))
                {
                    string dest_file;
                    if (Program.AllComputeHash || !IsMD5(Path.GetFileNameWithoutExtension(files[i]), rx))
                    {
                        string hash = ComputeMD5(files[i]);
                        string sub_dir = "\\" + hash[0] + "\\" + hash[1];
                        string ext = Path.GetExtension(files[i]);
                        if (ext.CompareTo(".jpeg") == 0)
                        {
                            dest_file = dest_path + sub_dir + "\\" + hash + ".jpg";
                        }
                        else
                        {
                            dest_file = dest_path + sub_dir + "\\" + hash + ext;
                        }
                    }
                    else
                    {
                        string sub_dir = "\\" + Path.GetFileName(files[i])[0] + "\\" + Path.GetFileName(files[i])[1];
                        dest_file = dest_path + sub_dir + "\\" + Path.GetFileName(files[i]);
                    }
                    //string sub_dir = "\\" + Path.GetFileName(files[i])[0] + "\\" + Path.GetFileName(files[i])[1];
                    //string dest_file = dest_path + sub_dir + "\\" + Path.GetFileName(files[i]);
                    if (File.Exists(dest_file))
                    {
                        FileInfo dest_file_info = new FileInfo(dest_file);
                        FileInfo sourc_file = new FileInfo(files[i]);
                        if (sourc_file.Length == dest_file_info.Length)
                        {
                            File.Delete(dest_file);
                            File.Move(files[i], dest_file);
                            Console.WriteLine("{0} >> {1}", files[i], dest_file);
                        }
                        else
                        {
                            Console.WriteLine("ФАЙЛЫ НЕ РАВНЫ\n{0}\n{1}", files[i], dest_file);
                            error++;
                            error_files.Add(files[i]);
                        }
                    }
                    else
                    {
                        File.Move(files[i], dest_file);
                        Console.WriteLine("{0} >> {1}", files[i], dest_file);
                    }
                }
                else
                {
                    Console.WriteLine("{0} >> {1}", files[i], "Не изображение, пропуск!");
                }
            }
            Console.WriteLine("Ошибок {0}", error);
            foreach(string s in error_files)
            {
                Console.WriteLine(s);
            }
        }
        public static bool IsImageFile(string FilePath)
        {
            string ext = Path.GetExtension(FilePath);
            switch (ext.ToLower())
            {
                case ".jpg":
                    return true;
                case ".jpeg":
                    return true;
                case ".png":
                    return true;
                case ".bmp":
                    return true;
                case ".gif":
                    return true;
                case ".tif":
                    return true;
                case ".tiff":
                    return true;
            }
            return false;
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
        public static void ParseArgs(string[] args)
        {
            List<string> temp = new List<string>();
            foreach(string s in args)
            {
                if(String.Compare(s, "-r", false) == 0)
                {
                    Program.Recurse = true;
                    continue;
                }
                if (String.Compare(s, "-a", false) == 0)
                {
                    Program.AllComputeHash = true;
                    continue;
                }
                temp.Add(s);
            }
            switch (temp.Count)
            {
                case 0:
                    break;
                case 1:
                    Program.config.SourcePath = temp[0];
                    break;
                case 2:
                    Program.config.SourcePath = temp[0];
                    Program.config.TargetPath = temp[1];
                    break;
                default:
                    Program.config.SourcePath = temp[0];
                    Program.config.TargetPath = temp[1];
                    break;
            }
        }
        static void LoadSettings()
        {
            Program.config = new Config();
            //Параметры по умолчанию
            Program.config.SourcePath = @"I:\AnimeArt\Unsorted";
            Program.config.TargetPath = @"I:\AnimeArt";
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(Config));
            if (File.Exists("C:\\utils\\Erza\\Sagiri.json"))
            {
                using (FileStream fs = new FileStream("C:\\utils\\Erza\\Sagiri.json", FileMode.Open))
                {
                    Program.config = (Config)jsonFormatter.ReadObject(fs);
                }
                return;
            }
            Console.WriteLine("Конфигурационный файл не найден!\nЗагружены настройки по умолчанью.");
        }
    }
    [DataContract]
    public class Config
    {
        [DataMember]
        public string SourcePath;
        [DataMember]
        public string TargetPath;
        public Config()
        {
        }
    }
}
