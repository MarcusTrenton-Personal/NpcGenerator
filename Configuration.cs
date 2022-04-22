using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;

namespace NpcGenerator
{
    static class Configuration
    {
        public static TraitSchema Parse(string path)
        {
            IEnumerable<string> lines = File.ReadLines(path);
            IEnumerator<string> enumerator = lines.GetEnumerator();
            bool hasTitleRow = enumerator.MoveNext();
            if(!hasTitleRow)
            {
                throw new IOException("The file is empty: " + path);
            }

            TraitSchema traitSchema = ParseTitles(enumerator.Current);

            while(enumerator.MoveNext())
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
            for(int i = 0; i < titleCells.Length; i += 2)
            {
                traitSchema.Add(new TraitCategory(titleCells[i]));
            }
            return traitSchema;
        }

        private static void ParseOptionsRow(TraitSchema traitSchema, string optionsRow)
        {
            string[] cells = optionsRow.Split(',');
            for(int i = 0; i*2 < cells.Length; ++i)
            {
                string traitName = cells[i * 2];
                if(!string.IsNullOrEmpty(traitName))
                {
                    if(i >= traitSchema.TraitCategoryCount)
                    {
                        throw new FormatException("Trait " + traitName + " is missing a column title.");
                    }

                    string traitWeightString = cells[i * 2 + 1];
                    if(!string.IsNullOrEmpty(traitWeightString))
                    {
                        int weight;
                        bool isInteger = int.TryParse(traitWeightString, out weight);
                        if(!isInteger || weight < 0)
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
