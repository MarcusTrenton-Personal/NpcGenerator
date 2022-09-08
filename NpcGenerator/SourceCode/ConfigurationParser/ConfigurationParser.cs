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
using System.IO;

namespace NpcGenerator
{
    public class ConfigurationParser : IConfigurationParser
    {
        public ConfigurationParser(IEnumerable<FormatParser> parsers)
        {
            IEnumerator<FormatParser> enumerator = parsers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                IFormatConfigurationParser parser = enumerator.Current.Parser;
                m_parsers[enumerator.Current.FileExtensionWithDot] = parser;
            }
        }

        public TraitSchema Parse(string path)
        {
            string fileType = Path.GetExtension(path);
            bool isFound = m_parsers.TryGetValue(fileType, out IFormatConfigurationParser parser);
            if (isFound)
            {
                string text = File.ReadAllText(path);
                bool isEmpty = string.IsNullOrWhiteSpace(text);
                if (isEmpty)
                {
                    string fileName = Path.GetFileName(path);
                    throw new EmptyFileException(fileName);
                }
                return parser.Parse(text);
            }

            string acceptedFileTypes = "";
            foreach(string fileExtension in m_parsers.Keys)
            {
                acceptedFileTypes += fileExtension + " ";
            }
            throw new ArgumentException("File " + path + " is not one of the accepted extensions: " + acceptedFileTypes);
        }

        private readonly Dictionary<string, IFormatConfigurationParser> m_parsers = 
            new Dictionary<string, IFormatConfigurationParser>();
    }

    public class FormatParser
    {
        public FormatParser(string fileExtensionWithDot, IFormatConfigurationParser parser)
        {
            FileExtensionWithDot = fileExtensionWithDot;
            Parser = parser;
        }

        public string FileExtensionWithDot { get; private set; }
        public IFormatConfigurationParser Parser { get; private set; }
    }
}
