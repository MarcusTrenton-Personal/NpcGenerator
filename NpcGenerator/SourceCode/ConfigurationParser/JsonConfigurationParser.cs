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
along with this program. If not, see<https://www.gnu.org/licenses/>.*/

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
            ValidateJson(json, m_schema, path);

            ProtoTraitSchema protoTraitSchema = json.ToObject<ProtoTraitSchema>();
            DetectDuplicateCategoryNames(protoTraitSchema.trait_categories);
            DetectDuplicateTraitNamesInASingleCategory(protoTraitSchema.trait_categories);
            TraitSchema traitSchema = ParseInternal(protoTraitSchema);
            return traitSchema;
        }

        private static void ValidateJson(JToken json, JSchema m_schema, string path)
        {
            if (m_schema != null)
            {
                //Validation schema means that less in-code validation is needed.
                bool isValid = json.IsValid(m_schema, out IList<string> errorMessages);
                if (!isValid)
                {
                    string message = "";
                    foreach (var error in errorMessages)
                    {
                        message += error + "\n";
                    }
                    throw new JsonFormatException(message, path);
                }
            }
        }

        private void DetectDuplicateCategoryNames(List<ProtoTraitCategory> trait_categories)
        {
            List<string> categoryNames = new List<string>();
            foreach (ProtoTraitCategory protoCategory in trait_categories)
            {
                bool isDuplicateName = categoryNames.Contains(protoCategory.Name);
                if (isDuplicateName)
                {
                    throw new DuplicateCategoryNameException(protoCategory.Name);
                }
                else
                {
                    categoryNames.Add(protoCategory.Name);
                }
            }
        }

        private void DetectDuplicateTraitNamesInASingleCategory(List<ProtoTraitCategory> trait_categories)
        {
            foreach (ProtoTraitCategory protoCategory in trait_categories)
            {
                List<string> traitNames = new List<string>();
                foreach (ProtoTrait protoTrait in protoCategory.traits)
                {
                    bool isDuplicateName = traitNames.Contains(protoTrait.Name);
                    if (isDuplicateName)
                    {
                        throw new DuplicateTraitNamesInCategoryException(category: protoCategory.Name, trait: protoTrait.Name);
                    }
                    else
                    {
                        traitNames.Add(protoTrait.Name);
                    }
                }
            }
        }

        private TraitSchema ParseInternal(ProtoTraitSchema protoTraitSchema)
        {
            ParseCategories(
                protoTraitSchema,
                out TraitSchema traitSchema,
                out List<DeferredBonusSelection> deferredBonusSelections,
                out List<TraitCategory> categories);
            ParseDeferredBonusSelections(deferredBonusSelections, categories);
            ParseReplacements(protoTraitSchema.replacements, traitSchema, categories);

            return traitSchema;
        }

        private static void ParseCategories(
            ProtoTraitSchema protoTraitSchema, 
            out TraitSchema traitSchema, 
            out List<DeferredBonusSelection> deferredBonusSelections, 
            out List<TraitCategory> categories)
        {
            traitSchema = new TraitSchema();
            deferredBonusSelections = new List<DeferredBonusSelection>();
            categories = new List<TraitCategory>();

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
        }

        private static void ParseDeferredBonusSelections(List<DeferredBonusSelection> deferredBonusSelections, List<TraitCategory> categories)
        {
            foreach (DeferredBonusSelection deferredBonusSelection in deferredBonusSelections)
            {
                TraitCategory category = categories.Find(category => category.Name == deferredBonusSelection.m_categoryName);
                if (category == null)
                {
                    throw new MismatchedBonusSelectionException(notFoundCategory: deferredBonusSelection.m_categoryName,
                        trait: deferredBonusSelection.m_trait.Name);
                }
                BonusSelection bonusSelection = new BonusSelection(category, deferredBonusSelection.m_selections);
                deferredBonusSelection.m_trait.BonusSelection = bonusSelection;
            }
        }

        private static void ParseReplacements(
            List<TraitId> protoReplacements, TraitSchema traitSchema, List<TraitCategory> categories)
        {
            if (protoReplacements != null)
            {
                foreach (TraitId protoReplacement in protoReplacements)
                {
                    TraitCategory category = categories.Find(category => category.Name == protoReplacement.category_name);
                    if (category == null)
                    {
                        throw new MismatchedReplacementCategoryException(category: protoReplacement.category_name,
                            trait: protoReplacement.trait_name);
                    }
                    Trait trait = category.GetTrait(protoReplacement.trait_name);
                    if (trait == null)
                    {
                        throw new MismatchedReplacementTraitException(category: protoReplacement.category_name,
                            trait: protoReplacement.trait_name);
                    }

                    ReplacementSearch replacement = new ReplacementSearch(trait, category);
                    traitSchema.Add(replacement);
                }
            }
        }

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value null
        //These values will be assigned through the magic of Newtonsoft's JsonConvert.DeserializeObject();
        private class ProtoTraitSchema
        {
            //Deliberately breaking with the normal naming scheme.
            //The variables must be named exactly like json varaibles (ignoring case) for the convenient deserialization.
            public List<TraitId> replacements;
            public List<ProtoTraitCategory> trait_categories;
        }

        private class ProtoTraitCategory
        {
            public string Name { get; set; }
            public int Selections { get; set; }
            public ProtoLogicalExpression requirements;
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

        private class DeferredBonusSelection
        {
            public Trait m_trait;
            public string m_categoryName;
            public int m_selections;
        }

        private class TraitId
        {
            public string trait_name;
            public string category_name;
        }

        private class ProtoLogicalExpression
        {
            //In the schema, the expression is either a trait_id with trait_name and category_name or
            //it is a logical_operation with an operator and operands.
            //Both are combined into a single class here so they can be in together in a list. A limitation of the C# language.
            //It is not valid for a single object to have non-null member from both sets.

            //trait_id
            public string trait_name = null;
            public string category_name = null;

            //logical_operation
            public string @operator = null;
            public List<ProtoLogicalExpression> operands;
        }
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value null

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
}