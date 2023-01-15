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

namespace NpcGenerator
{
    public class Trait : IGuardedByRequirement
    {
        public Trait(string name) : this(name, weight: 1, isHidden: false)
        {
        }

        public Trait(string name, int weight) : this(name, weight, isHidden: false)
        { 
        }

        public Trait(string name, int weight, bool isHidden)
        {
            Name = name;
            OriginalName = name;
            Weight = weight;
            IsHidden = isHidden;
        }

        public Trait DeepCopyWithRename(string newName)
        {
            Trait copy = (Trait) MemberwiseClone();
            copy.Name = newName;
            copy.BonusSelection = BonusSelection?.ShallowCopy();
            return copy;
        }

        public void Set(in Requirement requirement)
        {
            m_requirement = requirement;
        }

        public bool IsUnlockedFor(in Npc npc)
        {
            if (npc is null)
            {
                throw new ArgumentNullException(nameof(npc));
            }

            bool isUnlocked = m_requirement is null || m_requirement.IsUnlockedFor(npc);
            return isUnlocked;
        }

        public HashSet<string> DependentCategoryNames()
        {
            HashSet<string> categories = m_requirement == null ? new HashSet<string>() : m_requirement.DependentCategoryNames();
            return categories;
        }

        public string Name { get; private set; }
        public string OriginalName { get; private set; }
        public int Weight { get; private set; }
        public bool IsHidden { get; private set; }
        public BonusSelection BonusSelection { get; set; } = null;

        private Requirement m_requirement;
    }
}