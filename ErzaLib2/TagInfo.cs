/* Copyright © Maksim Belous 2018 */
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

namespace ErzaLib2
{
    public class TagInfo
    {
        public long TagID;
        public long Count;
        public TagType Type;
        public string? Tag;
        public string? Localization;
        public string? Description;
        public TagInfo()
        {
            TagID = -1;
            Count = 0;
            Type = TagType.General;
        }
    }
    public enum TagType: long
    {
        General = 0,
        Artist = 1,
        Studio = 2,
        Copyright = 3,
        Character = 4,
        Circle = 5,
        Faults = 6,
        Medium = 8,
        Meta = 9
    }
}
