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
    public static class NpcFactory
    {
        public static NpcGroup Create(TraitSchema traitSchema, int npcCount, IReadOnlyList<Replacement> replacements, IRandom random)
        {
            if (traitSchema is null)
            {
                throw new ArgumentNullException(nameof(traitSchema));
            }
            if (replacements is null)
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

            IReadOnlyList<string> categoryNameOrder = traitSchema.CalculateTraversalOrder();
            IReadOnlyList<TraitCategory> categoryOrder = 
                ListUtil.ConvertAll(categoryNameOrder, name => categoriesWithReplacements.Find(category => category.Name == name));

            for (int i = 0; i < npcCount; ++i)
            {
                Npc npc = CreateNpc(categoryOrder, random);
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

            return replacementCategories;
        }

        private static Npc CreateNpc(IReadOnlyList<TraitCategory> categoryOrder, IRandom random)
        {
            Npc npc = new Npc();
            Dictionary<TraitCategory, TraitChooser> chooserForCategory = new Dictionary<TraitCategory, TraitChooser>();
            Dictionary<TraitCategory, int> selectionsPerCategory = new Dictionary<TraitCategory, int>();
            foreach (TraitCategory category in categoryOrder)
            {
                selectionsPerCategory[category] = category.DefaultSelectionCount;
                chooserForCategory[category] = category.CreateTraitChooser(random);
            }

            bool gainedNewTraitThisIteration;
            do
            {
                gainedNewTraitThisIteration = false;
                foreach(TraitCategory category in categoryOrder)
                {
                    TraitChooser chooser = chooserForCategory[category];
                    gainedNewTraitThisIteration = TryAddTraitFromCategory(
                        category,
                        categoryOrder,
                        npc,
                        chooser,
                        selectionsPerCategory);
                    if (gainedNewTraitThisIteration)
                    {
                        //Reset back to the first category in the order every time any trait is gained.
                        //Evaluation of a requirement must be deferred as long as possible.
                        //A requirement to not have a trait implies that all potential bonus selections into that category have been exhausted.
                        break; 
                    }
                }
            }
            while (gainedNewTraitThisIteration);

            return npc;
        }

        private static bool TryAddTraitFromCategory(
            TraitCategory category,
            IReadOnlyList<TraitCategory> categories,
            Npc npc,
            TraitChooser chooser,
            Dictionary<TraitCategory, int> selectionsPerCategory)
        {
            bool wasTraitAdded = false;

            int selectionCount = selectionsPerCategory[category];
            bool canUseCategory = category.IsUnlockedFor(npc);
            if (selectionCount > 0 && canUseCategory)
            {
                try
                {
                    wasTraitAdded = ChooseTraitsFromCategory(
                        chooser, selectionCount, npc, category.Name, out IReadOnlyList<BonusSelection> bonusSelections);

                    selectionsPerCategory[category] = 0;

                    AddBonusSelections(bonusSelections, categories, selectionsPerCategory);
                }
                catch (TooFewTraitsException exception)
                {
                    throw new TooFewTraitsInCategoryException(category.Name, requested: exception.Requested, available: exception.Available);
                }
            }

            return wasTraitAdded;
        }

        private static bool ChooseTraitsFromCategory(
            TraitChooser chooser, 
            int selectionCount, 
            Npc npc, 
            string outputCategoryName, 
            out IReadOnlyList<BonusSelection> bonusSelections)
        {
            string[] traits = chooser.Choose(selectionCount, out bonusSelections);
            npc.Add(category: outputCategoryName, traits: traits);
            bool wasTraitAdded = traits.Length > 0;
            return wasTraitAdded;
        }

        private static void AddBonusSelections(
            IReadOnlyList<BonusSelection> bonusSelections, 
            IReadOnlyList<TraitCategory> categories, 
            Dictionary<TraitCategory, int> selectionsPerCategory)
        {
            foreach (BonusSelection bonusSelection in bonusSelections)
            {
                TraitCategory cat = ListUtil.Find(categories, category => category.Name == bonusSelection.CategoryName);
                selectionsPerCategory[cat] = bonusSelection.SelectionCount;
            }
        }
    }

    public class TooFewTraitsInCategoryException : ArgumentException
    {
        public TooFewTraitsInCategoryException(string category, int requested, int available)
        {
            Category = category;
            Requested = requested;
            Available = available;
        }

        public string Category { get; private set; }
        public int Requested { get; private set; }
        public int Available { get; private set; }
    }
}
