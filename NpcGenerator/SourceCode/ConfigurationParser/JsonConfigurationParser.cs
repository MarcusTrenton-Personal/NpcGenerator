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

using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Services;
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

        public TraitSchema Parse(string text)
        {
            JToken json = JToken.Parse(text);
            ValidateJson(json, m_schema);

            ProtoTraitSchema protoTraitSchema = json.ToObject<ProtoTraitSchema>();
            DetectDuplicateCategoryNames(protoTraitSchema.trait_categories);
            DetectDuplicateTraitNamesInASingleCategory(protoTraitSchema.trait_categories);
            DetectTooFewTraitsInCategory(protoTraitSchema.trait_categories);
            TraitSchema traitSchema = ParseInternal(protoTraitSchema);
            return traitSchema;
        }

        private static void ValidateJson(JToken json, JSchema m_schema)
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
                    throw new JsonFormatException(message);
                }
            }
        }

        private static void DetectDuplicateCategoryNames(List<ProtoTraitCategory> trait_categories)
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

        private static void DetectDuplicateTraitNamesInASingleCategory(List<ProtoTraitCategory> trait_categories)
        {
            foreach (ProtoTraitCategory protoCategory in trait_categories)
            {
                List<string> traitNames = new List<string>();
                foreach (ProtoTrait protoTrait in protoCategory.traits)
                {
                    bool isDuplicateName = traitNames.Contains(protoTrait.Name);
                    if (isDuplicateName)
                    {
                        throw new DuplicateTraitNameInCategoryException(new TraitId(protoCategory.Name, protoTrait.Name));
                    }
                    else
                    {
                        traitNames.Add(protoTrait.Name);
                    }
                }
            }
        }

        private static void DetectTooFewTraitsInCategory(List<ProtoTraitCategory> categories)
        {
            foreach (ProtoTraitCategory category in categories)
            {
                int availableTraits = category.traits.Count;
                int requiredTraits = category.Selections;
                if (requiredTraits > availableTraits)
                {
                    throw new TooFewTraitsInCategoryException(category.Name, requiredTraits, availableTraits);
                }
            }
        }

        private static TraitSchema ParseInternal(ProtoTraitSchema protoTraitSchema)
        {
            TraitSchema traitSchema = ParseSimplifiedSchema(protoTraitSchema);
            ParseBonusSelections(protoTraitSchema, traitSchema);
            ParseRequirements(protoTraitSchema, traitSchema);
            ParseReplacements(protoTraitSchema.replacements, traitSchema);
            return traitSchema;
        }

        private static TraitSchema ParseSimplifiedSchema(
            ProtoTraitSchema protoTraitSchema)
        {
            TraitSchema traitSchema = new TraitSchema();
            foreach (ProtoTraitCategory protoCategory in protoTraitSchema.trait_categories)
            {
                TraitCategory category = new TraitCategory(protoCategory.Name, protoCategory.Selections);
                foreach (ProtoTrait protoTrait in protoCategory.traits)
                {
                    Trait trait = new Trait(protoTrait.Name, protoTrait.Weight, protoTrait.Hidden);
                    category.Add(trait);
                }
                traitSchema.Add(category);
            }
            return traitSchema;
        }

        private static void ParseBonusSelections(ProtoTraitSchema protoTraitSchema, TraitSchema schema)
        {
            IReadOnlyList<TraitCategory> categories = schema.GetTraitCategories();
            foreach (ProtoTraitCategory protoCategory in protoTraitSchema.trait_categories)
            {
                foreach (ProtoTrait protoTrait in protoCategory.traits)
                {
                    ProtoBonusSelection protoBonusSelection = protoTrait.bonus_selection;
                    if (protoBonusSelection != null)
                    {
                        TraitCategory originalCategory = ListUtil.Find(categories, category => category.Name == protoCategory.Name);
                        Trait trait = originalCategory.GetTrait(protoTrait.Name);

                        TraitCategory targetCategory = ListUtil.Find(categories, category => category.Name == protoBonusSelection.trait_category_name);
                        if (targetCategory is null)
                        {
                            throw new MismatchedBonusSelectionException(notFoundCategory: protoBonusSelection.trait_category_name,
                                new TraitId(originalCategory.Name, trait.Name));
                        }

                        BonusSelection bonusSelection = new BonusSelection(targetCategory, protoBonusSelection.Selections);
                        trait.BonusSelection = bonusSelection;
                    }
                }
            }
        }

        private static void ParseRequirements(ProtoTraitSchema protoTraitSchema, TraitSchema schema)
        {
            foreach (ProtoTraitCategory protoCategory in protoTraitSchema.trait_categories)
            {
                TraitCategory category = ListUtil.Find(schema.GetTraitCategories(), category => category.Name == protoCategory.Name);

                bool selfRequiringCategory = RequiresCategory(protoCategory.requirements, category);
                if (selfRequiringCategory)
                {
                    throw new SelfRequiringCategoryException(category.Name);
                }

                Requirement requirement = ParseRequirement(protoCategory.requirements, category, schema);
                category.Set(requirement);
            }
        }

        private static bool RequiresCategory(ProtoLogicalExpression protoExpression, TraitCategory category)
        {
            if (protoExpression is null || category is null)
            {
                return false;
            }
            if (protoExpression.category_name == category.Name)
            {
                return true;
            }
            if (protoExpression.operands != null)
            {
                foreach (ProtoLogicalExpression subExpression in protoExpression.operands)
                {
                    bool doeSubExpressionRequireCategory = RequiresCategory(subExpression, category);
                    if (doeSubExpressionRequireCategory)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static Requirement ParseRequirement(
            ProtoLogicalExpression protoLogicalExpression, 
            TraitCategory requirementCategory,
            TraitSchema schema)
        {
            if (protoLogicalExpression != null)
            {
                NpcHolder npcHolder = new NpcHolder();
                ILogicalExpression logicalExpression = ParseLogicalExpression(
                    protoLogicalExpression,
                    requirementCategory,
                    schema,
                    npcHolder);
                return new Requirement(logicalExpression, npcHolder);
            }
            return null;
        }

        private static ILogicalExpression ParseLogicalExpression
            (ProtoLogicalExpression protoLogicalExpression,
            TraitCategory requirementCategory,
            TraitSchema schema,
            INpcProvider npcProvider)
        {
            ILogicalExpression requirementExpression;

            bool isTraitExpression = !string.IsNullOrEmpty(protoLogicalExpression.trait_name);
            if (isTraitExpression)
            {
                requirementExpression = ParseTraitRequirement(
                    protoLogicalExpression.category_name,
                    protoLogicalExpression.trait_name,
                    requirementCategory,
                    npcProvider,
                    schema);
            }
            else
            {
                requirementExpression = ParseLogicalOperator(
                    protoLogicalExpression.@operator,
                    protoLogicalExpression.operands,
                    requirementCategory,
                    npcProvider,
                    schema);
            }

            return requirementExpression;
        }

        private static NpcHasTrait ParseTraitRequirement(
            string categoryName, 
            string traitName,
            TraitCategory requirementCategory,
            INpcProvider npcProvider,
            TraitSchema schema)
        {
            TraitId traitId = new TraitId(categoryName, traitName);
            bool hasTrait = schema.HasTrait(traitId);
            if (!hasTrait)
            {
                throw new RequirementTraitIdNotFoundException(requirementCategory.Name, traitId);
            }

            return new NpcHasTrait(traitId, npcProvider: npcProvider);
        }

        private static ILogicalOperator ParseLogicalOperator(
            string operatorName, 
            List<ProtoLogicalExpression> operands,
            TraitCategory requirementCategory,
            INpcProvider npcProvider,
            TraitSchema schema)
        {
            ILogicalOperator logicalOperator = operatorName switch
            {
                "All" => new LogicalAll(),
                "Any" => new LogicalAny(),
                "None" => new LogicalNone(),
                _ => throw new UnknownLogicalOperatorException(requirementCategory.Name, operatorName),
            };

            foreach (ProtoLogicalExpression protoSubExpression in operands)
            {
                ILogicalExpression subExpression = ParseLogicalExpression(protoSubExpression, requirementCategory, schema, npcProvider);
                logicalOperator.Add(subExpression);
            }

            return logicalOperator;
        }

        private static void ParseReplacements(List<ProtoTraitId> protoReplacements, TraitSchema traitSchema)
        {
            if (protoReplacements != null)
            {
                foreach (ProtoTraitId protoReplacement in protoReplacements)
                {
                    IReadOnlyList<TraitCategory> categories = traitSchema.GetTraitCategories();
                    TraitCategory category = ListUtil.Find(categories, category => category.Name == protoReplacement.category_name);
                    if (category is null)
                    {
                        throw new MissingReplacementCategoryException(
                            new TraitId(protoReplacement.category_name, protoReplacement.trait_name));
                    }
                    Trait trait = category.GetTrait(protoReplacement.trait_name);
                    if (trait is null)
                    {
                        throw new MissingReplacementTraitException(
                            new TraitId(protoReplacement.category_name, protoReplacement.trait_name));
                    }

                    ReplacementSearch replacement = new ReplacementSearch(trait, category);
                    traitSchema.Add(replacement);
                }
            }
        }

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value null
#pragma warning disable IDE1006 // Naming conventions
        //These values will be assigned through the magic of Newtonsoft's JsonConvert.DeserializeObject();
        private class ProtoTraitSchema
        {
            //Deliberately breaking with the normal naming scheme.
            //The variables must be named exactly like json varaibles (ignoring case) for the convenient deserialization.
            public List<ProtoTraitId> replacements;
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

        private class ProtoTraitId
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
#pragma warning restore IDE1006 // Naming conventions

        private readonly JSchema m_schema = null;
    }

    public class JsonFormatException : FormatException
    {
        public JsonFormatException(string message) : base(message)
        {
        }
    }
}