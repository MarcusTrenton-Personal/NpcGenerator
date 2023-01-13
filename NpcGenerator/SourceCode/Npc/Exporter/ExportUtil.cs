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
    public static class ExportUtil
    {
        public const string MULTI_TRAIT_SEPARATOR = " & ";

        public static void CombineTraits(in Npc.Trait[] traits, in StringBuilder stringBuilder)
        {
            ParamUtil.VerifyElementsAreNotNull(nameof(traits), traits);
            ParamUtil.VerifyNotNull(nameof(stringBuilder), stringBuilder);

            HashSet<string> visibleDistinctTraits = VisibleDistinctTraits(traits);

            bool isFirstVisibleTrait = true;
            foreach (string trait in visibleDistinctTraits)
            {
                string separator = isFirstVisibleTrait ? string.Empty : MULTI_TRAIT_SEPARATOR;
                stringBuilder.Append(separator);
                stringBuilder.Append(trait);
                isFirstVisibleTrait = false;
            }
        }

        public static HashSet<string> VisibleDistinctTraits(Npc.Trait[] traits)
        {
            ParamUtil.VerifyElementsAreNotNull(nameof(traits), traits);

            HashSet<string> visibleDistinctTraits = new HashSet<string>();
            foreach (Npc.Trait trait in traits)
            {
                if (!trait.IsHidden)
                {
                    visibleDistinctTraits.Add(trait.Name);
                }
            }
            return visibleDistinctTraits;
        }
    }
}
