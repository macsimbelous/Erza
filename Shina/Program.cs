/* Copyright © Macsim Belous 2013 */
/* This file is part of Shina.

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
using System.Data;
using System.Net;
using System.Xml;
using System.Threading;
using ErzaLib;
using System.Diagnostics;

namespace Shina
{
    class Program
    {
        static string ConnectionString = @"data source=C:\utils\Erza\erza.sqlite";
        static void Main(string[] args)
        {
            List<ImageInfo> il;
            Console.WriteLine("Считываем хэши из базы данных");
            ImagesDB erza = new ImagesDB(ConnectionString);
            erza.ProgressCallBack = new ImagesDB.ProgressCallBackT(ProgressSQLiteCallBack);
            var timer = Stopwatch.StartNew();
            il = erza.GetAllImagesWithOutTags();
            timer.Stop();
            Console.WriteLine("\nСчитано хэшей: {0} за: {1} секунд ({2} в секунду)", il.Count.ToString(), (timer.ElapsedMilliseconds / 1000).ToString("0.00"), (il.Count / (timer.ElapsedMilliseconds / 1000)).ToString("0.00"));

            int resolved_count = 0;
            Grabber grab = new Grabber();
            grab.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
            grab.LoginForDanbooru = "macsimbelous";
            grab.ApiKeyForDanbooru = "F5eC.ADAOFChEkOwVA1x.BUES0S9GaRqSoohs7wO";
            grab.UseDanbooru = true;
            grab.UseGelbooru = true;
            grab.UseKonachan = true;
            grab.UseSankaku = false;
            grab.UseYandere = true;
            Console.WriteLine("Проверяем хэши зерез Данбуру, Яндере, Коначан и Гелбуру");
            for (int i = 0; i < il.Count; i++)
            {
                Console.Write("[{0}/{1}] Проверяю {2}: ", i + 1, il.Count, il[i].GetHashString());
                DateTime start = DateTime.Now;
                ImageInfo img = grab.GetImageInfoFromImageBoards(il[i].GetHashString());
                if (img != null)
                {
                    il[i].AddTags(img.tags);
                    erza.UpdateTagsForImage(il[i]);
                    resolved_count++;
                    Console.WriteLine("Получено {0} тегов", il[i].tags.Count);
                }
                else
                {
                    Console.WriteLine("Хэш не найден!");
                }
                MyWait(start, 1500);
            }
            Console.WriteLine("Получены теги для {0} из {1} изображений", resolved_count, il.Count);
            //Console.ReadKey();
        }
        static void MyWait(DateTime start, int delay)
        {
            int current = (int)((DateTime.Now - start).TotalMilliseconds);
            //Console.Write("TIME {0}, ", current);
            if (current < delay)
            {
#if DEBUG
                Console.WriteLine("TIME {0}, WAIT {1}", current, delay - current);
#endif
                Thread.Sleep(delay - current);
                return;
            }
            else
            {
                return;
            }
        }
        static void ProgressSQLiteCallBack(string hash, int Count, int Total)
        {
            Console.Write("Считано: {0} из {1}\r", Count, Total);
        }
    }
}
