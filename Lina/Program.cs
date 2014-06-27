/* Copyright © Macsim Belous 2012 */
/* This file is part of Lina.

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
using System.IO;

namespace Lina
{
    class Program
    {
        static void Main(string[] args)
        {
            int images_count = 0;
            if (args.Length < 1) { Console.WriteLine("Не задан каталог!"); }
            Console.WriteLine("Получаю список файлов из {0}", args[0]);
            string[] files = Directory.GetFiles(args[0], "*.*", SearchOption.AllDirectories);
            Console.WriteLine("Генерируем галерею");
            using (StreamWriter outfile = new StreamWriter(args[0] + "\\index.html"))
            {
                outfile.WriteLine("<!DOCTYPE HTML><html><head><meta charset=\"utf-8\"><title>Тег IMG</title></head><body>");
                foreach (string file in files)
                {
                    if (IsImageFile(file))
                    {
                        outfile.WriteLine("<p><img width=\"100%\" src=\"{0}\"></p>", System.IO.Path.GetFileName(file));
                        images_count++;
                    }
                }
                outfile.WriteLine("</body></html>");
            }
            Console.WriteLine("Генерация галереи завершена\nВ галерею вошло {0} изображений", args[0]);
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
