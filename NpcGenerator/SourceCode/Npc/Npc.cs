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
along with this program.If not, see<https://www.gnu.org/licenses/>.*/

using System;
using System.Collections.Generic;
using System.Text;

namespace NpcGenerator
{
    public class Npc
    {
        public void AddTrait(string name)
        {
            traits.Add(name);
        }

        public string GetTraitAtIndex(int index)
        {
            return traits[index];
        }

        public string[] GetTraits()
        {
            return traits.ToArray();
        }

        public void ToCsvRow(StringBuilder stringBuilder)
        {
            if (stringBuilder == null)
            {
                throw new ArgumentNullException(nameof(stringBuilder));
            }

            for (int i = 0; i < traits.Count; ++i)
            {
                stringBuilder.Append(traits[i]);
                if (i + 1 < traits.Count)
                {
                    stringBuilder.Append(',');
                }
            }
        }

        public int TraitCount { get { return traits.Count; } }

        private readonly List<string> traits = new List<string>();
    }
}