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
                IList<string> errorMessages;
                bool isValid = json.IsValid(m_schema, out errorMessages);
                if (!isValid)
                {
                    throw new ArgumentException(errorMessages.ToString());
                }
            }

            ProtoTraitSchema protoTraitSchema = json.ToObject<ProtoTraitSchema>();
            TraitSchema traitSchema = new TraitSchema();
            foreach (ProtoTraitCategory protoCategory in protoTraitSchema.trait_categories)
            {
                TraitCategory category = new TraitCategory(protoCategory.Name);
                foreach (ProtoTrait protoTrait in protoCategory.traits)
                {
                    Trait trait = new Trait(protoTrait.Name, protoTrait.Weight);
                    category.Add(trait);
                }
                traitSchema.Add(category);
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

        private readonly JSchema m_schema = null;
    }
}