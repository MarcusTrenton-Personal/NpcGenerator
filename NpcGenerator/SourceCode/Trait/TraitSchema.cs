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
    public class TraitSchema
    {
        public void Add(TraitCategory traitCategory)
        {
            m_categories.Add(traitCategory);
        }

        public void Add(ReplacementSearch replacement)
        {
            m_replacements.Add(replacement);
        }

        public IReadOnlyList<TraitCategory> GetTraitCategories()
        {
            return m_categories;
        }

        public IReadOnlyList<ReplacementSearch> GetReplacementSearches()
        {
            return m_replacements;
        }

        public bool HasTrait(TraitId traitId)
        {
            if (traitId is null)
            {
                throw new ArgumentNullException(nameof(traitId));
            }

            TraitCategory category = m_categories.Find(category => category.Name == traitId.CategoryName);
            if (category is null)
            {
                return false;
            }

            bool hasTrait = category.HasTrait(traitId.TraitName);
            return hasTrait;
        }

        private readonly List<TraitCategory> m_categories = new List<TraitCategory>();
        private readonly List<ReplacementSearch> m_replacements = new List<ReplacementSearch>();
    }
}
