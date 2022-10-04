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
            Npc.Trait[] traits = chooser.Choose(selectionCount, out bonusSelections);
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

        public static bool AreNpcsValid(in NpcGroup npcGroup, in TraitSchema schema, IReadOnlyList<Replacement> replacements, 
            out Dictionary<Npc,List<NpcSchemaViolation>> violationsPerNpc)
        {
            if (npcGroup is null)
            {
                throw new ArgumentNullException(nameof(npcGroup));
            }
            for (int i = 0; i < npcGroup.NpcCount; ++i)
            {
                Npc npc = npcGroup.GetNpcAtIndex(i);
                if (npc is null)
                {
                    throw new ArgumentException("An npc in the " + nameof(npcGroup) + " is null");
                }
            }
            if (schema is null)
            {
                throw new ArgumentNullException(nameof(schema));
            }
            if (replacements is null)
            {
                throw new ArgumentNullException(nameof(replacements));
            }

            bool areValid = true;
            violationsPerNpc = new Dictionary<Npc, List<NpcSchemaViolation>>();
            for (int i = 0; i < npcGroup.NpcCount; ++i)
            {
                Npc npc = npcGroup.GetNpcAtIndex(i);
                areValid &= IsNpcValid(npc, schema, replacements, out List<NpcSchemaViolation> violations);
                violationsPerNpc[npc] = violations;
            }

            return areValid;
        }

        private static bool IsNpcValid(
            in Npc npc, in TraitSchema schema, IReadOnlyList<Replacement> replacements, out List<NpcSchemaViolation> violations)
        {
            violations = new List<NpcSchemaViolation>();

            AddUnknownTraitViolations(npc, schema, violations);
            AddUnusedReplacementViolations(npc, replacements, violations);
            AddLockedCategoryViolations(npc, schema, violations);
            AddIncorrectTraitCountViolations(npc, schema, violations);

            return violations.Count == 0;
        }

        private static void AddUnknownTraitViolations(in Npc npc, in TraitSchema schema, List<NpcSchemaViolation> violations)
        {
            IReadOnlyList<TraitCategory> schemaCategories = schema.GetTraitCategories();
            foreach (string npcCategory in npc.GetCategories())
            {
                TraitCategory schemaCategory = ListUtil.Find(schemaCategories, category => category.Name == npcCategory);
                if (schemaCategory is null)
                {
                    violations.Add(new NpcSchemaViolation(npcCategory, NpcSchemaViolation.Reason.CategoryNotFoundInSchema));
                    continue;
                }

                foreach (Npc.Trait npcTrait in npc.GetTraitsOfCategory(npcCategory))
                {
                    Trait schemaTrait = schemaCategory.GetTrait(npcTrait.OriginalName) ?? schemaCategory.GetTrait(npcTrait.Name);
                    if (schemaTrait is null)
                    {
                        violations.Add(new NpcSchemaViolation(npcCategory, npcTrait.Name, NpcSchemaViolation.Reason.TraitNotFoundInSchema));
                        continue;
                    }
                    if (!schemaTrait.IsHidden && npcTrait.IsHidden)
                    {
                        violations.Add(new NpcSchemaViolation(npcCategory, npcTrait.Name, NpcSchemaViolation.Reason.TraitIsIncorrectlyHidden));
                        continue;
                    }
                    if (schemaTrait.IsHidden && !npcTrait.IsHidden)
                    {
                        violations.Add(new NpcSchemaViolation(npcCategory, npcTrait.Name, NpcSchemaViolation.Reason.TraitIsIncorrectlyNotHidden));
                        continue;
                    }
                }
            }
        }

        private static void AddUnusedReplacementViolations(
            in Npc npc, in IReadOnlyList<Replacement> replacements, List<NpcSchemaViolation> violations)
        {
            IReadOnlyList<string> npcCategories = npc.GetCategories();
            foreach (Replacement replacement in replacements)
            {
                bool didNameChange = replacement.OriginalTrait.Name != replacement.ReplacementTraitName;
                if (didNameChange)
                {
                    string npcCategoryWithReplacement = ListUtil.Find(npcCategories, category => category == replacement.Category.Name);
                    if (!string.IsNullOrEmpty(npcCategoryWithReplacement))
                    {
                        Npc.Trait[] traits = npc.GetTraitsOfCategory(npcCategoryWithReplacement);
                        Npc.Trait unreplacedTrait = Array.Find(traits, trait => trait.Name == replacement.OriginalTrait.Name);
                        if (unreplacedTrait != null)
                        {
                            violations.Add(new NpcSchemaViolation(
                                npcCategoryWithReplacement,
                                replacement.OriginalTrait.Name,
                                NpcSchemaViolation.Reason.UnusedReplacement));
                        }
                    }
                }
            }
        }

        private static void AddLockedCategoryViolations(in Npc npc, in TraitSchema schema, List<NpcSchemaViolation> violations)
        {
            IReadOnlyList<TraitCategory> schemaCategories = schema.GetTraitCategories();
            IReadOnlyList<string> npcCategories = npc.GetCategories();

            foreach (string npcCategory in npcCategories)
            {
                TraitCategory category = ListUtil.Find(schemaCategories, category => category.Name == npcCategory);
                if (category != null)
                {
                    bool couldHaveTraitsFromCategory = category.IsUnlockedFor(npc);
                    if (!couldHaveTraitsFromCategory)
                    {
                        Npc.Trait[] traits = npc.GetTraitsOfCategory(npcCategory);
                        if (traits.Length > 0)
                        {
                            violations.Add(new NpcSchemaViolation(npcCategory, traits[0].Name, NpcSchemaViolation.Reason.HasTraitInLockedCategory));
                        }
                    }
                }
            }
        }

        private static void AddIncorrectTraitCountViolations(in Npc npc, in TraitSchema schema, List<NpcSchemaViolation> violations)
        {
            Dictionary<string, int> bonusSelectionIntoCount = BonusSelectionIntoCount(npc, schema);
            IReadOnlyList<TraitCategory> schemaCategories = schema.GetTraitCategories();
            foreach (string npcCategory in npc.GetCategories())
            {
                int foundTraitCount = npc.GetTraitsOfCategory(npcCategory).Length;

                TraitCategory schemaCategory = ListUtil.Find(schemaCategories, category => category.Name == npcCategory);
                if (schemaCategory != null)
                {
                    int justifiedTraitCount = schemaCategory.DefaultSelectionCount + bonusSelectionIntoCount[npcCategory];

                    if (foundTraitCount != justifiedTraitCount)
                    {
                        NpcSchemaViolation.Reason reason = foundTraitCount < justifiedTraitCount ?
                            NpcSchemaViolation.Reason.TooFewTraitsInCategory : NpcSchemaViolation.Reason.TooManyTraitsInCategory;
                        violations.Add(new NpcSchemaViolation(npcCategory, reason));
                    }
                }
            }
        }

        private static Dictionary<string, int> BonusSelectionIntoCount(in Npc npc, in TraitSchema schema)
        {
            Dictionary<string, int> bonusSelectionIntoCount = new Dictionary<string, int>();
            IReadOnlyList<TraitCategory> schemaCategories = schema.GetTraitCategories();
            foreach (string npcCategory in npc.GetCategories())
            {
                bonusSelectionIntoCount[npcCategory] = 0;
            }

            foreach (string npcCategory in npc.GetCategories())
            {
                TraitCategory schemaCategory = ListUtil.Find(schemaCategories, category => category.Name == npcCategory);
                if (schemaCategory != null)
                {
                    foreach (Npc.Trait npcTrait in npc.GetTraitsOfCategory(npcCategory))
                    {
                        Trait schemaTrait = schemaCategory.GetTrait(npcTrait.OriginalName) ?? schemaCategory.GetTrait(npcTrait.Name);
                        if (schemaTrait != null)
                        {
                            if (schemaTrait.BonusSelection != null)
                            {
                                BonusSelection bonus = schemaTrait.BonusSelection;
                                bonusSelectionIntoCount[bonus.CategoryName] += bonus.SelectionCount;
                            }
                        }
                    }
                }
            }
            return bonusSelectionIntoCount;
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

    public class NpcSchemaViolation
    {
        public NpcSchemaViolation(string category, Reason violation) : this(category, trait: null, violation)
        {
        } 

        public NpcSchemaViolation(string category, string trait, Reason violation)
        {
            Category = category;
            Trait = trait;
            Violation = violation;
        }

        public string Category { get; private set; }

        public string Trait { get; private set; }

        public Reason Violation { get; private set; }

        public enum Reason
        {
            HasTraitInLockedCategory,
            TooFewTraitsInCategory,
            TooManyTraitsInCategory,
            TraitNotFoundInSchema,
            CategoryNotFoundInSchema,
            TraitIsIncorrectlyHidden,
            TraitIsIncorrectlyNotHidden,
            UnusedReplacement
        }
    }
}
