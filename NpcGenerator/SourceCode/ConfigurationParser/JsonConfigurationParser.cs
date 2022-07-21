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

using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.IO;

namespace NpcGenerator
{
    public class JsonConfigurationParser : IFormatConfigurationParser
    {
        public JsonConfigurationParser(string schemaPath)
        {
            bool hasSchemaPath = !string.IsNullOrEmpty(schemaPath);
            if (hasSchemaPath)
            {
                string schemaText = File.ReadAllText(schemaPath);
                m_schema = JSchema.Parse(schemaText);
            }
        }

        public string SupportedFileExtension { get; } = ".json";

        public TraitSchema Parse(string path)
        {
            string text = File.ReadAllText(path);
            JToken json = JToken.Parse(text);
            
            if(m_schema != null)
            {
                //Validation schema means that no in-code validation is needed.
                bool isValid = json.IsValid(m_schema, out IList<string> errorMessages);
                if (!isValid)
                {
                    string message = "";
                    foreach(var error in errorMessages)
                    {
                        message += error + "\n";
                    }
                    throw new JsonFormatException(message, path);
                }
            }

            ProtoTraitSchema protoTraitSchema = json.ToObject<ProtoTraitSchema>();
            TraitSchema traitSchema = new TraitSchema();
            List<DeferredBonusSelection> deferredBonusSelections = new List<DeferredBonusSelection>();
            List<TraitCategory> categories = new List<TraitCategory>();
            foreach (ProtoTraitCategory protoCategory in protoTraitSchema.trait_categories)
            {
                TraitCategory category = new TraitCategory(protoCategory.Name, protoCategory.Selections);
                foreach (ProtoTrait protoTrait in protoCategory.traits)
                {
                    Trait trait = new Trait(protoTrait.Name, protoTrait.Weight, protoTrait.Hidden);
                    category.Add(trait);

                    if (protoTrait.bonus_selection != null)
                    {
                        deferredBonusSelections.Add(new DeferredBonusSelection()
                        {
                            m_trait = trait,
                            m_categoryName = protoTrait.bonus_selection.trait_category_name,
                            m_selections = protoTrait.bonus_selection.Selections
                        });
                    }
                }
                traitSchema.Add(category);
                categories.Add(category);
            }
            foreach (DeferredBonusSelection deferredBonusSelection in deferredBonusSelections)
            {
                TraitCategory category = categories.Find(category => category.Name == deferredBonusSelection.m_categoryName);
                if (category == null)
                {
                    throw new MismatchedBonusSelectionException(notFoundCategoryName: deferredBonusSelection.m_categoryName,
                        trait: deferredBonusSelection.m_trait.Name);
                }
                BonusSelection bonusSelection = new BonusSelection(category, deferredBonusSelection.m_selections);
                deferredBonusSelection.m_trait.BonusSelection = bonusSelection;
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
            public int Selections { get; set; }
            public List<ProtoTrait> traits;
        }

        private class ProtoTrait
        {
            public string Name { get; set; }
            public int Weight { get; set; }
            public bool Hidden { get; set; } = false;
            //Deliberately breaking with the normal naming scheme.
            //The variables must be named exactly like json varaibles (ignoring case) for the convenient deserialization.
            public ProtoBonusSelection bonus_selection { get; set; }
        }

        private class ProtoBonusSelection
        {
            //Deliberately breaking with the normal naming scheme.
            //The variables must be named exactly like json varaibles (ignoring case) for the convenient deserialization.
            public string trait_category_name;
            public int Selections { get; set; }
        }
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value null

        private class DeferredBonusSelection
        {
            public Trait m_trait;
            public string m_categoryName;
            public int m_selections;
        }

        private readonly JSchema m_schema = null;
    }

    public class JsonFormatException : FormatException
    {
        public JsonFormatException(string message, string path) : base(message)
        {
            Path = path;
        }
        public string Path { get; private set; }
    }

    public class MismatchedBonusSelectionException : FormatException
    {
        public MismatchedBonusSelectionException(string notFoundCategoryName, string trait)
        {
            NotFoundCategoryName = notFoundCategoryName;
            Trait = trait;
        }

        public string Trait { get; private set; }
        public string NotFoundCategoryName { get; private set; }
    }
}