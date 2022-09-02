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

namespace NpcGenerator
{
    public class NpcGroup
    {
        public NpcGroup(IReadOnlyList<string> categoryOrder)
        {
            VerifyCategoryOrderng(categoryOrder);
            m_categoryOrder = categoryOrder;
        }

        private static void VerifyCategoryOrderng(IReadOnlyList<string> categoryOrder)
        {
            VerifyCategoryNotNullOrEmpty(categoryOrder);
            VerifyCategoryHasNoDuplicates(categoryOrder);
        }

        private static void VerifyCategoryNotNullOrEmpty(IReadOnlyList<string> categoryOrder)
        {
            if (categoryOrder is null)
            {
                throw new ArgumentNullException(nameof(categoryOrder));
            }
            foreach (string category in categoryOrder)
            {
                if (string.IsNullOrEmpty(category))
                {
                    throw new ArgumentException("categoryOrder elements cannot be null or empty", nameof(categoryOrder));
                }
            }
        }

        private static void VerifyCategoryHasNoDuplicates(IReadOnlyList<string> categoryOrder)
        {
            for (int i = 0; i < categoryOrder.Count; ++i)
            {
                for (int j = i + 1; j < categoryOrder.Count; ++j)
                {
                    if (categoryOrder[i] == categoryOrder[j])
                    {
                        throw new ArgumentException("categoryOrder has duplicate element " + categoryOrder[i], nameof(categoryOrder));
                    }
                }
            }
        }

        public void Add(Npc npc)
        {
            m_npcs.Add(npc);
        }

        public Npc GetNpcAtIndex(int index)
        {
            return m_npcs[index];
        }

        public string GetTraitCategoryNameAtIndex(int index)
        {
            return m_categoryOrder[index];
        }

        public int NpcCount { get { return m_npcs.Count; } }
        public IReadOnlyList<string> CategoryOrder { get => m_categoryOrder; }

        private readonly IReadOnlyList<string> m_categoryOrder;
        private readonly List<Npc> m_npcs = new List<Npc>();
    }
}
