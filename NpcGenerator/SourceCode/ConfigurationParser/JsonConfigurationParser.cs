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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace NpcGenerator
{
    public static class JsonConfigurationParser
    {
        public static TraitSchema Parse(string path)
        {
            string filename = Path.GetFileName(path);
            string text = File.ReadAllText(path);
            ProtoTraitSchema protoTraitSchema = JsonConvert.DeserializeObject<ProtoTraitSchema>(text);
            if (protoTraitSchema == null)
            {
                throw new ArgumentException(filename + " has an unrecognizable json format");
            }
            if (protoTraitSchema.trait_categories == null)
            {
                throw new ArgumentException(filename + " has no trait_categories element");
            }

            TraitSchema traitSchema = new TraitSchema();
            int categoryCount = 0;
            foreach (ProtoTraitCategory protoCategory in protoTraitSchema.trait_categories)
            {
                if (protoCategory.traits == null)
                {
                    throw new ArgumentException(filename + " trait category " + protoCategory.Name + " has no traits");
                }
                if (string.IsNullOrEmpty(protoCategory.Name))
                {
                    throw new ArgumentException(filename + " has a trait category without a name");
                }

                TraitCategory category = new TraitCategory(protoCategory.Name);

                int traitCount = 0;
                foreach (ProtoTrait protoTrait in protoCategory.traits)
                {
                    if (string.IsNullOrEmpty(protoTrait.Name))
                    {
                        throw new ArgumentException(
                            filename + " trait category " + protoCategory.Name + " has a trait without a name");
                    }
                    if (protoTrait.Weight < 0)
                    {
                        throw new ArgumentException(filename + " trait " + protoTrait.Name + " a negative Weight");
                    }

                    Trait trait = new Trait(protoTrait.Name, protoTrait.Weight);
                    category.Add(trait);
                    traitCount++;
                }

                if (traitCount == 0)
                {
                    throw new ArgumentException(filename + " trait category " + protoCategory.Name + " has no traits");
                }

                traitSchema.Add(category);
                categoryCount++;
            }

            if (categoryCount == 0)
            {
                throw new ArgumentException(filename + " has no elements in the trait_categories array.");
            }

            return traitSchema;
        }

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value null
        //These values will be assigned through the magic of Newtonsoft's JsonConvert.DeserializeObject();
        private class ProtoTraitSchema
        {
            //Deliberately breaking with the normal naming scheme.
            //The variables must be named exactly like json varaibles (ignoring case) for the convenient deserialization.
            public List<ProtoTraitCategory> trait_categories;
        }

        private class ProtoTraitCategory
        {
            public string Name { get; set; }
            public List<ProtoTrait> traits;
        }

        private class ProtoTrait
        {
            public string Name { get; set; }
            public int Weight { get; set; }
        }
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value null
    }
}