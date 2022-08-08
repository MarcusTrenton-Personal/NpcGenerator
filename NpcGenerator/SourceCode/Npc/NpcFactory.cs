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
    public static class NpcFactory
    {
        public static NpcGroup Create(TraitSchema traitSchema, int npcCount, IReadOnlyList<Replacement> replacements)
        {
            if (traitSchema == null)
            {
                throw new ArgumentNullException(nameof(traitSchema));
            }
            if (replacements == null)
            {
                throw new ArgumentNullException(nameof(replacements));
            }

            List<TraitCategory> categoriesWithReplacements = GetReplacementCategories(traitSchema, replacements);
            List<string> traitCategoryNames = categoriesWithReplacements.ConvertAll(category => category.Name);
            NpcGroup group = new NpcGroup(traitCategoryNames);

            for (int i = 0; i < npcCount; ++i)
            {
                Npc npc = new Npc(categoriesWithReplacements);
                group.Add(npc);
            }

            return group;
        }

        private static List<TraitCategory> GetReplacementCategories(TraitSchema schema, IReadOnlyList<Replacement> replacements)
        {
            List<TraitCategory> replacementCategories = new List<TraitCategory>();
            IReadOnlyList<TraitCategory> originalCategories = schema.GetTraitCategories();
            for (int i = 0; i < originalCategories.Count; ++i)
            {
                TraitCategory originalCategory = originalCategories[i];
                TraitCategory replacementCategory = originalCategory.DeepCopyWithReplacements(replacements);
                replacementCategories.Add(replacementCategory);
            }

            //The copies made above have references to the old categories that must be updated.
            Dictionary<TraitCategory, TraitCategory> originalCategoriesToReplacements = new Dictionary<TraitCategory, TraitCategory>();
            for (int i = 0; i < originalCategories.Count; ++i)
            {
                TraitCategory originalCategory = originalCategories[i];
                TraitCategory replacementCategory = replacementCategories[i];
                originalCategoriesToReplacements[originalCategory] = replacementCategory;
            }
            foreach (TraitCategory category in replacementCategories)
            {
                category.ReplaceTraitReferences(originalCategoriesToReplacements);
            }

            return replacementCategories;
        }
    }
}
