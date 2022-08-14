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

using Services;
using System;
using System.Collections.Generic;

namespace NpcGenerator
{
    public static class NpcFactory
    {
        public static NpcGroup Create(TraitSchema traitSchema, int npcCount, IReadOnlyList<Replacement> replacements, IRandom random)
        {
            if (traitSchema == null)
            {
                throw new ArgumentNullException(nameof(traitSchema));
            }
            if (replacements == null)
            {
                throw new ArgumentNullException(nameof(replacements));
            }
            if (npcCount < 0)
            {
                throw new ArgumentException("npcCount must be greater than or equal to 0.", nameof(npcCount));
            }

            List<TraitCategory> categoriesWithReplacements = GetReplacementCategories(traitSchema, replacements);
            List<string> traitCategoryNames = categoriesWithReplacements.ConvertAll(category => category.Name);
            NpcGroup group = new NpcGroup(traitCategoryNames);

            for (int i = 0; i < npcCount; ++i)
            {
                Npc npc = CreateNpc(categoriesWithReplacements, random);
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

        private static Npc CreateNpc(IReadOnlyList<TraitCategory> categories, IRandom random)
        {
            Npc npc = new Npc();
            Dictionary<TraitCategory, TraitChooser> chooserForCategory = new Dictionary<TraitCategory, TraitChooser>();
            Dictionary<TraitCategory, int> selectionsPerCategory = new Dictionary<TraitCategory, int>();
            foreach (TraitCategory category in categories)
            {
                selectionsPerCategory[category] = category.DefaultSelectionCount;
                chooserForCategory[category] = category.CreateTraitChooser(random);
            }

            while (selectionsPerCategory.Count > 0)
            {
                GetElementOf(selectionsPerCategory, out TraitCategory category, out int count);
                TraitChooser chooser = chooserForCategory[category];
                string[] traits = chooser.Choose(count, out List<BonusSelection> bonusSelections);
                npc.Add(category: category.Name, traits: traits);

                selectionsPerCategory.Remove(category);

                foreach (BonusSelection bonusSelection in bonusSelections)
                {
                    selectionsPerCategory[bonusSelection.TraitCategory] = bonusSelection.SelectionCount;
                }
            }

            return npc;
        }

        private static void GetElementOf(Dictionary<TraitCategory, int> selectionsPerCategory, out TraitCategory category, out int count)
        {
            Dictionary<TraitCategory, int>.Enumerator enumerator = selectionsPerCategory.GetEnumerator();
            enumerator.MoveNext();
            KeyValuePair<TraitCategory, int> current = enumerator.Current;
            category = current.Key;
            count = current.Value;
        }
    }
}
