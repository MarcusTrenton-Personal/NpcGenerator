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

using Services;
using System.Collections.Generic;
using System.Text;

namespace NpcGenerator
{
    public class NpcToStringArray
    {
        public const string MULTI_TRAIT_SEPARATOR = ExportUtil.MULTI_TRAIT_SEPARATOR;

        public static string[] Export(in Npc npc, in IReadOnlyList<string> categoryOrder)
        {
            ParamUtil.VerifyNotNull(nameof(npc), npc);
            ParamUtil.VerifyElementsAreNotNull(nameof(categoryOrder), categoryOrder);

            StringBuilder stringBuilder = new StringBuilder();
            List<string> traitsPerCategory = new List<string>();
            for (int i = 0; i < categoryOrder.Count; ++i)
            {
                Npc.Trait[] traits = npc.GetTraitsOfCategory(categoryOrder[i]);
                bool found = traits.Length > 0;
                if (found)
                {
                    ExportUtil.CombineTraits(traits, stringBuilder);
                    traitsPerCategory.Add(stringBuilder.ToString());
                    stringBuilder.Clear();
                }
                else
                {
                    traitsPerCategory.Add(string.Empty);
                }
            }
            return traitsPerCategory.ToArray();
        }
    }
}
