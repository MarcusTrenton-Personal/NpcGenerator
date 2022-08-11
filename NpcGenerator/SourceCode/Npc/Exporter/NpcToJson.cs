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

using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NpcGenerator
{
    public class NpcToJson : INpcExport
    {
        public string FileExtensionWithoutDot { get; } = "json";

        public string Export(NpcGroup group)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw) { Formatting = Formatting.Indented })
            {
                writer.WriteStartObject();
                writer.WritePropertyName("npc_group");
                writer.WriteStartArray();

                for (int i = 0; i < group.NpcCount; ++i)
                {
                    ToJsonObject(group.GetNpcAtIndex(i), writer, group.CategoryOrder);
                }

                writer.WriteEndArray();
                writer.WriteEndObject();

            }
            string json = sw.ToString();

            return json;
        }

        public void ToJsonObject(Npc npc, JsonWriter writer, IReadOnlyList<string> categoryOrder)
        {
            writer.WriteStartObject();

            for (int i = 0; i < categoryOrder.Count; ++i)
            {
                string[] traits = npc.GetTraitsOfCategory(categoryOrder[i]);
                bool found = traits.Length > 0;
                if (found)
                {
                    writer.WritePropertyName(categoryOrder[i]);
                    writer.WriteStartArray();
                    for (int j = 0; j < traits.Length; ++j)
                    {
                        writer.WriteValue(traits[j]);
                    }
                    writer.WriteEndArray();
                }
            }

            writer.WriteEndObject();
        }
    }
}
