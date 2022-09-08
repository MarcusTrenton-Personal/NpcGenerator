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
using System.IO;

namespace NpcGenerator
{
    public class CsvConfigurationParser : IFormatConfigurationParser
    {
        public TraitSchema Parse(string text)
        {
            string[] lines = text.Split('\n');

            TraitSchema traitSchema = ParseCategories(lines[0]);

            for(int i = 1; i < lines.Length; ++i)
            {
                ParseOptionsRow(traitSchema, lines[i]);
            }

            DetectDuplicateTraitNamesInASingleCategory(traitSchema);

            return traitSchema;
        }

        private static TraitSchema ParseCategories(string titleRow)
        {
            string[] titleCells = titleRow.Split(',');
            bool hasMatchingTraitWeightPairs = titleCells.Length % 2 == 0;
            if (!hasMatchingTraitWeightPairs)
            {
                throw new CategoryWeightMismatchException();
            }

            TraitSchema traitSchema = new TraitSchema();
            for (int i = 0; i < titleCells.Length; i += 2)
            {
                string title = titleCells[i];
                bool isEmpty = string.IsNullOrEmpty(title);
                if (isEmpty)
                {
                    throw new EmptyCategoryNameException();
                }
                traitSchema.Add(new TraitCategory(title, selectionCount: 1));
            }

            DetectDuplicateCategoryNames(traitSchema);

            return traitSchema;
        }

        private static void DetectDuplicateCategoryNames(TraitSchema traitSchema)
        {
            List<string> names = new List<string>();
            foreach (TraitCategory category in traitSchema.GetTraitCategories())
            {
                string categoryName = category.Name;
                bool isDuplicateName = names.Contains(categoryName);
                if (isDuplicateName)
                {
                    throw new DuplicateCategoryNameException(categoryName);
                }
                else
                {
                    names.Add(categoryName);
                }
            }
        }

        private static void DetectDuplicateTraitNamesInASingleCategory(TraitSchema traitSchema)
        {
            foreach (TraitCategory category in traitSchema.GetTraitCategories())
            {
                string[] traitNames = category.GetTraitNames();
                for (int j = 0; j < traitNames.Length; j++)
                {
                    string[] matchingNames = Array.FindAll(traitNames, name => name == traitNames[j]);
                    bool duplicateFound = matchingNames.Length > 1;
                    if (duplicateFound)
                    {
                        throw new DuplicateTraitNameInCategoryException(new TraitId(category.Name, traitNames[j]));
                    }
                }
            }
        }

        private static void ParseOptionsRow(TraitSchema traitSchema, string optionsRow)
        {
            IReadOnlyList<TraitCategory> categories = traitSchema.GetTraitCategories();
            string[] cells = optionsRow.Split(',');
            for (int i = 0; i * 2 < cells.Length; ++i)
            {
                string traitName = cells[i * 2];
                if (!string.IsNullOrEmpty(traitName))
                {
                    if (i >= categories.Count)
                    {
                        throw new TraitMissingCategoryException(traitName);
                    }

                    int weightIndex = i * 2 + 1;
                    if (weightIndex >= cells.Length)
                    {
                        throw new MissingWeightException(new TraitId(categories[i].Name, traitName));
                    }

                    string traitWeightString = cells[weightIndex];
                    if (!string.IsNullOrEmpty(traitWeightString))
                    {
                        bool isInteger = int.TryParse(traitWeightString, out int weight);
                        if (!isInteger || weight < 0)
                        {
                            throw new WeightIsNotWholeNumberException(new TraitId(categories[i].Name, traitName), traitWeightString);
                        }
                        categories[i].Add(new Trait(traitName, weight, isHidden: false));
                    }
                    else
                    {
                        throw new MissingWeightException(new TraitId(categories[i].Name, traitName));
                    }
                }
            }
        }
    }
}