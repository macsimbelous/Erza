/* Copyright © Macsim Belous 2012 */
/* This file is part of MoveToStoreImages.

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
using System.Security.Cryptography;
using System.IO;
using System.Data;
using System.Data.SQLite;

namespace Shina
{
    class Program
    {
        static void Main(string[] args)
        {
            int dup = 0;
            int complit = 0;
            List<String> files = new List<string>();
            files.AddRange(Directory.GetFiles(args[0], "*.*", SearchOption.AllDirectories));
            foreach (string file in files)
            {
                if (IsImageFile(file))
                {
                    if (MoveImageToStore(file, args[1]) == false)
                    {
                        dup++;
                    }
                    else
                    {
                        complit++;
                    }
                }
            }
            Console.WriteLine("Обработано: {0} Дубликатов: {1}", complit, dup);
            //Console.ReadKey();
        }
        static bool MoveImageToStore(string file, string StorePath)
        {
            MD5 hash_enc = MD5.Create();
            FileStream fsData = new FileStream(file, FileMode.Open, FileAccess.Read);
            byte[] hash = hash_enc.ComputeHash(fsData);
            fsData.Close();
            string t = BitConverter.ToString(hash).Replace("-", string.Empty);
            string ext = file.Substring(file.LastIndexOf('.'));
            if (ext.ToLower() == ".jpeg") { ext = ".jpg"; }
            string DestFile = StorePath + "\\" + t.Substring(0, 2) + "\\" + t.Substring(2, 2);
            Directory.CreateDirectory(DestFile.ToLower());
            DestFile = DestFile + "\\" + t + ext;
            try
            {
                System.IO.File.Move(file, DestFile.ToLower());
                System.Console.WriteLine(file.Substring(file.LastIndexOf('\\') + 1) + " -> " + t.Substring(file.LastIndexOf('\\') + 1).ToLower());
            }
            catch (IOException)
            {
                System.Console.WriteLine("ДУБЛИКАТ!!! " + file);
                return false;
            }
            return true;
        }
        
        static bool IsImageFile(string s)
        {
            int t = s.LastIndexOf('.');
            if (t >= 0)
            {
                string ext = s.Substring(t).ToLower();
                switch (ext)
                {
                    case ".jpg":
                        return true;
                    //break;
                    case ".jpeg":
                        return true;
                    //break;
                    case ".png":
                        return true;
                    //break;
                    case ".bmp":
                        return true;
                    //break;
                    case ".gif":
                        return true;
                    //break;
                    case ".tif":
                        return true;
                    //break;
                    case ".tiff":
                        return true;
                    //break;

                }
            }
            return false;
        }
    }
}
