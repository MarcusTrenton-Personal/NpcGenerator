/*Copyright(C) 2022  Marcus Trenton, marcus.trenton@gmail.com

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
using System.Security.Cryptography;

namespace NpcGenerator
{
    public class Trait
    {
        public Trait(string name, int weight)
        {
            Name = name;
            Weight = weight;
        }

        public string Name { get; }
        public int Weight { get; }
    }

    public class TraitCategory
    {
        public TraitCategory(string name)
        {
            Name = name;
        }

        public void Add(Trait trait)
        {
            if(trait == null)
            {
                throw new ArgumentNullException(nameof(trait));
            }

            traits.Add(trait);
            m_totalWeight += trait.Weight;
        }

        public string Choose()
        {
            if(traits.Count == 0)
            {
                throw new InvalidOperationException("Cannot choose trait from empty Trait Group " + Name);
            }

            int randomSelection = RandomNumberGenerator.GetInt32(0, m_totalWeight) + 1;
            int selectedIndex = -1;
            int weightCount = 0;
            for(int i = 0; i < traits.Count; ++i)
            {
                weightCount += traits[i].Weight;
                if(randomSelection <= weightCount)
                {
                    selectedIndex = i;
                    break;
                }
            }
            //If invalid index, let it crash.
            return traits[selectedIndex].Name;
        }

        public string Name
        {
            get;
            private set;
        }

        private List<Trait> traits = new List<Trait>();
        private int m_totalWeight;
    }

    public class TraitSchema
    {
        public int TraitCategoryCount 
        { 
            get 
            {
                return m_categories.Count;
            } 
        }

        public void Add(TraitCategory traitCategory)
        {
            m_categories.Add(traitCategory);
        }

        public TraitCategory GetAtIndex(int index)
        {
            return m_categories[index];
        }

        private List<TraitCategory> m_categories = new List<TraitCategory>();
    }
}
