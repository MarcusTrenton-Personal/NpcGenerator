﻿/*Copyright(C) 2022 Marcus Trenton, marcus.trenton@gmail.com

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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NpcGenerator
{
    public class NpcToJson : INpcExport
    {
        public NpcToJson(string schemaPath)
        {
            if (schemaPath != null)
            {
                ParamUtil.VerifyMatchesPattern(nameof(schemaPath), schemaPath, RegexUtil.ANY_FILE_PATH, schemaPath + " is not a filepath.");

                string schemaText = File.ReadAllText(schemaPath, Constants.TEXT_ENCODING);
                m_schema = JSchema.Parse(schemaText);
            }
        }

        public const string FileExtensionWithoutDotStatic = "json";
        public string FileExtensionWithoutDot { get; } = FileExtensionWithoutDotStatic;

        public string Export(in NpcGroup group)
        {
            ParamUtil.VerifyNotNull(nameof(group), group);

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw) { Formatting = Formatting.Indented })
            {
                writer.WriteStartObject();
                writer.WritePropertyName("npc_group");
                writer.WriteStartArray();

                for (int i = 0; i < group.NpcCount; ++i)
                {
                    ToJsonObject(group.GetNpcAtIndex(i), writer, group.VisibleCategoryOrder);
                }

                writer.WriteEndArray();
                writer.WriteEndObject();

            }
            string json = sw.ToString();
            
            ValidateJson(json);

            return json;
        }

        private void ValidateJson(string text)
        {
            if (m_schema != null)
            {
                JToken json = JToken.Parse(text);
                //Validation schema means that less in-code validation is needed.
                bool isValid = json.IsValid(m_schema, out IList<string> errorMessages);
                if (!isValid)
                {
                    string message = "";
                    foreach (var error in errorMessages)
                    {
                        message += error + "\n";
                    }
                    throw new JsonExportFormatException(message);
                }
            }
        }

        public static void ToJsonObject(Npc npc, JsonWriter writer, IReadOnlyList<string> categoryOrder)
        {
            writer.WriteStartObject();

            for (int i = 0; i < categoryOrder.Count; ++i)
            {
                Npc.Trait[] traits = npc.GetTraitsOfCategory(categoryOrder[i]);
                HashSet<string> distinctVisibleTraits = ExportUtil.VisibleDistinctTraits(traits);
                bool found = traits.Length > 0;
                if (found)
                {
                    writer.WritePropertyName(categoryOrder[i]);
                    writer.WriteStartArray();
                    foreach (string trait in distinctVisibleTraits)
                    {
                        writer.WriteValue(trait);
                    }
                    writer.WriteEndArray();
                }
            }

            writer.WriteEndObject();
        }

        private readonly JSchema m_schema = null;
    }

    public class JsonExportFormatException : FormatException
    {
        public JsonExportFormatException(string message) : base(message)
        {
        }
    }
}
