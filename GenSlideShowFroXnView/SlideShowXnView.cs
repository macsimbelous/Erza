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
using System.IO;

namespace GenSlideShowFroXnView
{
    class SlideShowXnView
    {
        private string head_string = "# Slide Show Sequence";
        public float Timer = 5;
        public int Loop = 1;
        public int FullScreen = 1;
        public int MouseNav = 1;
        public int WatchDir = 0;
        public int TitleBar = 0;
        public int Fit = 1;
        public int StrechVideo = 0;
        public int AudioLoop = 0;
        public int High = 1;
        public int CenterWindow = 1;
        public int OnTop = 0;
        public int Frame = 1;
        public int ReadErrors = 0;
        public int RandomOrder = 1;
        public int ShowText = 0;
        public string Text = "<Directory><Filename>";
        public int BackgroundColor = 0;
        public string FontName = "Courier";
        public int FontBold = 0;
        public int FontItalic = 0;
        public int FontHeight = -13;
        public int TextOpacity = 255;
        public string TextColor = "ffffff";
        public int TextPosition = 0;
        public int Effect = 2;
        public int EffectMask = 268435455;
        public List<string> images;
        public SlideShowXnView()
        {
            images = new List<string>();
        }
        public void GenerateSlideShow(string file)
        {
            using (StreamWriter outfile = new StreamWriter(file))
            {
                outfile.WriteLine(this.head_string);
                outfile.WriteLine("Timer = {0}", this.Timer);
                outfile.WriteLine("Loop = {0}", this.Loop);
                outfile.WriteLine("FullScreen = {0}", this.FullScreen);
                outfile.WriteLine("MouseNav = {0}", this.MouseNav);
                outfile.WriteLine("WatchDir = {0}", this.WatchDir);
                outfile.WriteLine("TitleBar = {0}", this.TitleBar);
                outfile.WriteLine("Fit = {0}", this.Fit);
                outfile.WriteLine("StrechVideo = {0}", this.StrechVideo);
                outfile.WriteLine("AudioLoop = {0}", this.AudioLoop);
                outfile.WriteLine("High = {0}", this.High);
                outfile.WriteLine("CenterWindow = {0}", this.CenterWindow);
                outfile.WriteLine("OnTop = {0}", this.OnTop);
                outfile.WriteLine("Frame = {0}", this.Frame);
                outfile.WriteLine("ReadErrors = {0}", this.ReadErrors);
                outfile.WriteLine("RandomOrder = {0}", this.RandomOrder);
                outfile.WriteLine("ShowText = {0}", this.ShowText);
                outfile.WriteLine("Text = {0}", this.Text);
                outfile.WriteLine("BackgroundColor = {0}", this.BackgroundColor);
                outfile.WriteLine("FontName = {0}", this.FontName);
                outfile.WriteLine("FontBold = {0}", this.FontBold);
                outfile.WriteLine("FontItalic = {0}", this.FontItalic);
                outfile.WriteLine("FontHeight = {0}", this.FontHeight);
                outfile.WriteLine("TextOpacity = {0}", this.TextOpacity);
                outfile.WriteLine("TextColor = {0}", this.TextColor);
                outfile.WriteLine("TextPosition = {0}", this.TextPosition);
                outfile.WriteLine("Effect = {0}", this.Effect);
                outfile.WriteLine("EffectMask = {0}", this.EffectMask);
                foreach (string s in images)
                {
                    outfile.WriteLine("\"" + s + "\"");
                }
            }
        }
    }
}
