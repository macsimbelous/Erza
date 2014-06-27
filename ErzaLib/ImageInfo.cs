/* Copyright © Macsim Belous 2013 */
/* This file is part of ErzaLib.

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
using System.Text;
using System.Globalization;

namespace ErzaLib
{
    public class ImageInfo
    {
        public bool is_new = true;
        public bool is_deleted = false;
        public ulong db_id = 0;
        public int sankaku_post_id = 0;
        public int danbooru_post_id = 0;
        public int gelbooru_post_id = 0;
        public int konachan_post_id = 0;
        public int yandere_post_id = 0;
        public byte[] hash = null;
        public string file = null;
        public string sankaku_url = null;
        public string danbooru_url = null;
        public string gelbooru_url = null;
        public string konachan_url = null;
        public string yandere_url = null;
        public List<string> tags = new List<string>();
        public List<string> urls = new List<string>();
        public string GetHashString()
        {
            return BitConverter.ToString(this.hash).Replace("-", string.Empty).ToLower();
        }
        public void SetHashString(string hash_string)
        {
            if (hash_string == null)
            {
                throw new ArgumentNullException("hexString");
            }
            if ((hash_string.Length & 1) != 0)
            {
                throw new ArgumentOutOfRangeException("hexString", hash_string, "hexString must contain an even number of characters.");
            }
            byte[] result = new byte[hash_string.Length / 2];
            for (int i = 0; i < hash_string.Length; i += 2)
            {
                result[i / 2] = byte.Parse(hash_string.Substring(i, 2), NumberStyles.HexNumber);
            }
            this.hash = result;
        }
        public string GetStringOfTags()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < this.tags.Count; i++)
            {
                if (i == 0)
                {
                    sb.Append(this.tags[i]);
                }
                else
                {
                    sb.Append(' ');
                    sb.Append(this.tags[i]);
                }
            }
            return sb.ToString();
        }
        public void AddTag(string tag)
        {
            if ((tag != null) && (tag != String.Empty))
            {
                if (this.tags.LastIndexOf(tag) < 0)
                {
                    this.tags.Add(tag);
                }
            }
        }
        public void AddTags(string[] tags_array)
        {
            foreach (string tag in tags_array)
            {
                if ((tag != null) && (tag != String.Empty))
                {
                    if (this.tags.LastIndexOf(tag) < 0)
                    {
                        this.tags.Add(tag);
                    }
                }
            }
        }
        public void AddTags(List<string> tags_array)
        {
            foreach (string tag in tags_array)
            {
                if ((tag != null) && (tag != String.Empty))
                {
                    if (this.tags.LastIndexOf(tag) < 0)
                    {
                        this.tags.Add(tag);
                    }
                }
            }
        }
        public void AddStringOfTags(string tags_string)
        {
            string[] tags_array = tags_string.Split(' ');
            foreach (string tag in tags_array)
            {
                if ((tag != null) && (tag != String.Empty))
                {
                    if (this.tags.LastIndexOf(tag) < 0)
                    {
                        this.tags.Add(tag);
                    }
                }
            }
        }
        public override string ToString()
        {
            if (this.file != String.Empty)
            {
                return file.Substring(file.LastIndexOf('\\') + 1);
            }
            else
            {
                return "No File!";
            }
        }
    }
}
