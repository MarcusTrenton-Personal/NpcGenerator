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
using System;
using System.Collections.Generic;

namespace NpcGenerator
{
    public class NpcGroup
    {
        public NpcGroup(IReadOnlyList<Category> categoryOrder)
        {
            ParamUtil.VerifyElementsAreNotNull(nameof(categoryOrder), categoryOrder);
            foreach (Category category in categoryOrder)
            {
                ParamUtil.VerifyHasContent("NpcGroup.Category.Name", category.Name);
            }

            VerifyCategoryHasNoDuplicates(categoryOrder);
            m_categoryOrder = categoryOrder;
            m_visibleCategoryOrder = GetVisibleCategoryOrder(m_categoryOrder);
        }

        private static void VerifyCategoryHasNoDuplicates(IReadOnlyList<Category> categoryOrder)
        {
            for (int i = 0; i < categoryOrder.Count; ++i)
            {
                for (int j = i + 1; j < categoryOrder.Count; ++j)
                {
                    if (categoryOrder[i].Name == categoryOrder[j].Name)
                    {
                        throw new ArgumentException("categoryOrder has duplicate element " + categoryOrder[i].Name, nameof(categoryOrder));
                    }
                }
            }
        }

        private static IReadOnlyList<string> GetVisibleCategoryOrder(IReadOnlyList<Category> categories)
        {
            List<string> visibleCategoryOrder = new List<string>();
            foreach (NpcGroup.Category category in categories)
            {
                if (!category.IsHidden)
                {
                    visibleCategoryOrder.Add(category.Name);
                }
            }

            return visibleCategoryOrder;
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
            return m_categoryOrder[index].Name;
        }

        public int NpcCount { get { return m_npcs.Count; } }
        public IReadOnlyList<Category> CategoryOrder { get => m_categoryOrder; }
        public IReadOnlyList<string> VisibleCategoryOrder { get => m_visibleCategoryOrder; }

        private readonly IReadOnlyList<Category> m_categoryOrder;
        private readonly IReadOnlyList<string> m_visibleCategoryOrder;
        private readonly List<Npc> m_npcs = new List<Npc>();

        public class Category : IEquatable<Category>
        {
            public Category(string name) : this(name, isHidden: false)
            {
            }

            public Category(string name, bool isHidden)
            {
                Name = name;
                IsHidden = isHidden;
            }

            //Taken from Microsoft example at
            //https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/how-to-define-value-equality-for-a-type
            public bool Equals(Category other)
            {
                if (other is null)
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                if (GetType() != other.GetType())
                {
                    return false;
                }

                // Return true if the fields match, other than OriginalName
                return (Name == other.Name) && (IsHidden == other.IsHidden);
            }

            public override bool Equals(object obj)
            {
                if (obj is Category other)
                {
                    return Equals(other);
                }
                return false;
            }

            public override int GetHashCode() => (Name, IsHidden).GetHashCode();

            public static bool operator ==(Category lhs, Category rhs)
            {
                if (lhs is null)
                {
                    if (rhs is null)
                    {
                        return true;
                    }

                    return false;
                }
                return lhs.Equals(rhs);
            }

            public static bool operator !=(Category lhs, Category rhs) => !(lhs == rhs);

            public string Name { get; private set; }
            public bool IsHidden { get; private set; }
        }
    }
}
