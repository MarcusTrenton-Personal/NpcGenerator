using System;
using System.Collections.Generic;
using System.Text;

namespace NpcGenerator
{
    readonly public struct Trait
    {
        public Trait(string name, int weight)
        {
            Name = name;
            Weight = weight;
        }

        public string Name { get; }
        public int Weight { get; }
    }

    public class TraitGroup
    {
        public TraitGroup(string name)
        {
            Name = name;
        }

        public void Add(Trait trait)
        {
            traits.Add(trait);
            m_totalWeight += trait.Weight;
        }

        public string Choose()
        {
            if(traits.Count == 0)
            {
                throw new InvalidOperationException("Cannot choose trait from empty Trait Group " + Name);
            }

            int randomSelection = m_random.Next(0, m_totalWeight) + 1;
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
        private int m_totalWeight = 0;
        private Random m_random = new Random();
    }
}
