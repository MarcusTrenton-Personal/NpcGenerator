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
along with this program. If not, see<https://www.gnu.org/licenses/>.*/

using System;
using System.Collections.Generic;
using System.Text;

namespace NpcGenerator
{
    public class NpcToCsv : INpcExport
    {
        public string FileExtensionWithoutDot { get; } = "csv";

        private const char SEPARATOR = ','; 

        public string Export(NpcGroup group)
        {
            StringBuilder text = new StringBuilder();

            TraitCategoryNamesToCsv(text, group.CategoryOrder);
            text.Append('\n');

            for (int i = 0; i < group.NpcCount; ++i)
            {
                ToCsvRow(group.GetNpcAtIndex(i), text, group.CategoryOrder);
                if (i + 1 < group.NpcCount)
                {
                    text.Append('\n');
                }
            }

            return text.ToString();
        }

        private void TraitCategoryNamesToCsv(StringBuilder text, IReadOnlyList<string> categoryOrder)
        {
            for (int i = 0; i < categoryOrder.Count; ++i)
            {
                text.Append(categoryOrder[i]);
                if (i + 1 < categoryOrder.Count)
                {
                    text.Append(SEPARATOR);
                }
            }
        }

        private void ToCsvRow(Npc npc, StringBuilder stringBuilder, IReadOnlyList<string> categoryOrder)
        {
            for (int i = 0; i < categoryOrder.Count; ++i)
            {
                string[] traits = npc.GetTraitsOfCategory(categoryOrder[i]);
                bool found = traits.Length > 0;
                if (found)
                {
                    ExportUtil.CombineTraits(traits, stringBuilder);
                }
                if (i + 1 < categoryOrder.Count)
                {
                    stringBuilder.Append(SEPARATOR);
                }
            }
        }
    }
}
