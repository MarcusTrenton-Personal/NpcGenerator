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
using System.IO;

namespace NpcGenerator
{
    public static class CsvConfigurationParser
    {
        public static TraitSchema Parse(string path)
        {
            IEnumerable<string> lines = File.ReadAllLines(path);
            IEnumerator<string> enumerator = lines.GetEnumerator();
            bool hasTitleRow = enumerator.MoveNext();
            if (!hasTitleRow)
            {
                throw new IOException("The file is empty: " + path);
            }

            TraitSchema traitSchema = ParseTitles(enumerator.Current);

            while (enumerator.MoveNext())
            {
                ParseOptionsRow(traitSchema, enumerator.Current);
            }

            return traitSchema;
        }

        private static TraitSchema ParseTitles(string titleRow)
        {
            string[] titleCells = titleRow.Split(',');
            bool hasMatchingTraitWeightPairs = titleCells.Length % 2 == 0;
            if (!hasMatchingTraitWeightPairs)
            {
                throw new IOException("The configuration file must have an equal number of trait and weight columns with no empty columns in between.");
            }

            TraitSchema traitSchema = new TraitSchema();
            for (int i = 0; i < titleCells.Length; i += 2)
            {
                string title = titleCells[i];
                bool isEmpty = string.IsNullOrEmpty(title);
                if (isEmpty)
                {
                    throw new FormatException("Missing title for trait column");
                }
                traitSchema.Add(new TraitCategory(title));
            }
            return traitSchema;
        }

        private static void ParseOptionsRow(TraitSchema traitSchema, string optionsRow)
        {
            string[] cells = optionsRow.Split(',');
            for (int i = 0; i * 2 < cells.Length; ++i)
            {
                string traitName = cells[i * 2];
                if (!string.IsNullOrEmpty(traitName))
                {
                    if (i >= traitSchema.TraitCategoryCount)
                    {
                        throw new FormatException("Trait " + traitName + " is missing a column title.");
                    }

                    string traitWeightString = cells[i * 2 + 1];
                    if (!string.IsNullOrEmpty(traitWeightString))
                    {
                        bool isInteger = int.TryParse(traitWeightString, out int weight);
                        if (!isInteger || weight < 0)
                        {
                            throw new ArithmeticException("Weight for " + traitWeightString + " is not a whole number of at least 0");
                        }
                        traitSchema.GetAtIndex(i).Add(new Trait(traitName, weight));
                    }
                    else
                    {
                        throw new FormatException("Trait " + traitName + " is missing a weight");
                    }
                }
            }
        }
    }
}