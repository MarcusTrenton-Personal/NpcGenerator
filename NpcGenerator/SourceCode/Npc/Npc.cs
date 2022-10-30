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

using Newtonsoft.Json;
using Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace NpcGenerator
{
    public class Npc
    {
        public void Add(string category, Trait[] traits)
        {
            if (category is null)
            {
                throw new ArgumentNullException(nameof(category));
            }
            if (category.Length == 0)
            {
                throw new ArgumentException("Array must not be empty", nameof(category));
            }
            if (traits is null)
            {
                throw new ArgumentNullException(nameof(traits));
            }
            foreach (Trait trait in traits)
            {
                if (trait is null)
                {
                    throw new ArgumentException(nameof(traits) + " has a null element");
                }

                if (string.IsNullOrEmpty(trait.Name))
                {
                    throw new ArgumentException("Array elements must not be null or have empty name");
                }
            }

            bool categoryExists = m_traitsByCategory.ContainsKey(category);
            HashSet<Trait> traitSet;
            if (categoryExists)
            {
                traitSet = m_traitsByCategory[category];
            }
            else
            {
                m_traitsByCategory[category] = new HashSet<Trait>();
                traitSet = m_traitsByCategory[category];
            }

            foreach (Trait trait in traits)
            {
                traitSet.Add(trait);
            }
        }

        public Trait[] GetTraitsOfCategory(string category)
        {
            bool success = m_traitsByCategory.TryGetValue(category, out HashSet<Trait> traits);
            if (success)
            {
                Trait[] traitArray = new Trait[traits.Count];
                traits.CopyTo(traitArray);
                return traitArray;
            }
            else
            {
                return Array.Empty<Trait>();
            }
        }

        public bool HasTrait(TraitId traitId)
        {
            if (traitId is null)
            {
                throw new ArgumentNullException(nameof(traitId));
            }

            bool hasCategoryOfTrait = m_traitsByCategory.TryGetValue(traitId.CategoryName, out HashSet<Trait> traits);
            if (!hasCategoryOfTrait)
            {
                return false;
            }

            Trait trait = HashSetUtil.Find(traits, trait => trait.Name == traitId.TraitName);
            return trait != null;
        }

        public IReadOnlyList<string> GetCategories()
        {
            return new List<string>(m_traitsByCategory.Keys);
        }

        public class Trait : IEquatable<Trait>
        {
            public Trait(string name) : this(name, isHidden: false, name)
            {
            }

            public Trait(string name, bool isHidden) : this(name, isHidden, name)
            {
            }

            public Trait(string name, bool isHidden, string originalName)
            {
                Name = name;
                IsHidden = isHidden;
                OriginalName = originalName;
            }

            //Taken from Microsoft example at
            //https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/how-to-define-value-equality-for-a-type
            public bool Equals(Trait other)
            {
                if (other is null)
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                if (GetType() != other.GetType())
                {
                    return false;
                }

                // Return true if the fields match, other than OriginalName
                return (Name == other.Name) && (IsHidden == other.IsHidden);
            }

            public override bool Equals(object obj)
            {
                if (obj is Trait other)
                {
                    return Equals(other);
                }
                return false;
            }

            public override int GetHashCode() => (Name, IsHidden).GetHashCode();

            public static bool operator ==(Trait lhs, Trait rhs)
            {
                if (lhs is null)
                {
                    if (rhs is null)
                    {
                        return true;
                    }

                    return false;
                }
                return lhs.Equals(rhs);
            }

            public static bool operator !=(Trait lhs, Trait rhs) => !(lhs == rhs);

            public string Name { get; private set; }
            public bool IsHidden { get; private set; }
            public string OriginalName { get; private set; }
        }

        private readonly Dictionary<string,HashSet<Trait>> m_traitsByCategory = new Dictionary<string, HashSet<Trait>>();
    }
}