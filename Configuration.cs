using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;

namespace NpcGenerator
{
    static class Configuration
    {
        public static List<TraitGroup> Parse(string path)
        {
            IEnumerable<string> lines = File.ReadLines(path);
            IEnumerator<string> enumerator = lines.GetEnumerator();
            bool hasTitleRow = enumerator.MoveNext();
            if(!hasTitleRow)
            {
                throw new IOException("The file is empty: " + path);
            }

            List<TraitGroup> traitGroups = ParseTitles(enumerator.Current);

            while(enumerator.MoveNext())
            {
                ParseOptionsRow(traitGroups, enumerator.Current);
            }

            return traitGroups;
        }

        private static List<TraitGroup> ParseTitles(string titleRow)
        {
            string[] titleCells = titleRow.Split(',');
            bool hasMatchingTraitWeightPairs = titleCells.Length % 2 == 0;
            if (!hasMatchingTraitWeightPairs)
            {
                throw new IOException("The configuration file must have an equal number of trait and weight columns with no empty columns in between.");
            }

            List<TraitGroup> traitGroups = new List<TraitGroup>();
            for(int i = 0; i < titleCells.Length; i += 2)
            {
                traitGroups.Add(new TraitGroup(titleCells[i]));
            }
            return traitGroups;
        }

        private static void ParseOptionsRow(List<TraitGroup> traitGroups, string optionsRow)
        {
            string[] cells = optionsRow.Split(',');
            for(int i = 0; i*2 < cells.Length; ++i)
            {
                string traitName = cells[i * 2];
                if(!string.IsNullOrEmpty(traitName))
                {
                    if(i >= traitGroups.Count)
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
                            throw new ArithmeticException("Weight " + traitWeightString + " is not a whole number of at least 0");
                        }
                        traitGroups[i].Add(new Trait(traitName, weight));
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
