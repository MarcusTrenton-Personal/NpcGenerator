/*Copyright(C) 2022 Marcus Trenton, marcus.trenton@gmail.com

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program. If not, see <https://www.gnu.org/licenses/>.*/

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Services
{
    public static class RegexUtil
    {
        //Based on https://www.codeproject.com/Tips/216238/Regular-Expression-to-Validate-File-Path-and-Exten
        //Thank you https://regexlib.com/
        private static string ABSOLUTE_FILE_PATH_PATTERN = 
            @"^([a-zA-Z]\:|\\\\[^\/\\:*?"" <>|]+\\[^\/\\:*?""<>|]+)(\\[^\/\\:*?""<>|]+)+(\.[^\/\\:*?""<>|]+)$";
        public static readonly Regex ABSOLUTE_FILE_PATH = new Regex(ABSOLUTE_FILE_PATH_PATTERN);
        private static string RELATIVE_FILE_PATH_PATTERN = @"^((\.\./|[a-zA-Z0-9_/\-\\])*\.[a-zA-Z0-9]+)$";
        public static readonly Regex RELATIVE_FILE_PATH = new Regex(RELATIVE_FILE_PATH_PATTERN);
        public static readonly Regex ANY_FILE_PATH = new Regex(ABSOLUTE_FILE_PATH_PATTERN + "|" + RELATIVE_FILE_PATH_PATTERN);
        public static readonly Regex FILE_EXTENSION_WITH_DOT = new Regex(@"^\.[A-Za-z0-9]+$");
        public static readonly Regex FILE_EXTENSION_WITHOUT_DOT = new Regex(@"^[A-Za-z0-9]+$");
        public static readonly Regex LANGUAGE_CODE = new Regex(@"^[A-Za-z]{2}-[A-Za-z]{2}$");
    }
}
