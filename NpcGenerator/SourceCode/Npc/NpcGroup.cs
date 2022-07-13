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
using System.Text;

namespace NpcGenerator
{
    public class NpcGroup
    {
        public NpcGroup(TraitSchema traitSchema, int npcCount)
        {
            if(traitSchema == null)
            {
                throw new ArgumentNullException(nameof(traitSchema));
            }

            for(int i = 0; i < traitSchema.TraitCategoryCount; ++i)
            {
                traitCategoryNames.Add(traitSchema.GetAtIndex(i).Name);
            }

            for(int i = 0; i < npcCount; ++i)
            {
                Npc npc = new Npc();
                for(int j = 0; j < traitSchema.TraitCategoryCount; ++j)
                {
                    string trait = traitSchema.GetAtIndex(j).Choose();
                    npc.AddTrait(trait);
                }
                npcs.Add(npc);
            }
        }

        public string ToCsv()
        {
            StringBuilder text = new StringBuilder();
            
            TraitCategoryNamesToCsv(text);
            text.Append('\n');

            for(int i = 0; i < npcs.Count; ++i)
            {
                npcs[i].ToCsvRow(text);
                if (i + 1 < npcs.Count)
                {
                    text.Append('\n');
                }
            }

            return text.ToString();
        }

        public string ToJson()
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw) { Formatting = Formatting.Indented })
            {
                writer.WriteStartObject();
                writer.WritePropertyName("npc_group");
                writer.WriteStartArray();

                foreach(Npc npc in npcs)
                {
                    npc.ToJsonObject(writer, traitCategoryNames);
                }

                writer.WriteEnd(); //End of array
                writer.WriteEndObject(); //End of json

            }
            string json = sw.ToString();

            return json;
        }

        private void TraitCategoryNamesToCsv(StringBuilder text)
        {
            for (int i = 0; i < traitCategoryNames.Count; ++i)
            {
                text.Append(traitCategoryNames[i]);
                if (i + 1 < traitCategoryNames.Count)
                {
                    text.Append(',');
                }
            }
        }

        public Npc GetNpcAtIndex(int index)
        {
            return npcs[index];
        }

        public string GetTraitCategoryNameAtIndex(int index)
        {
            return traitCategoryNames[index];
        }

        public int TraitCategoryCount { get { return traitCategoryNames.Count; } }
        public int NpcCount { get { return npcs.Count; } }

        private readonly List<string> traitCategoryNames = new List<string>();
        private readonly List<Npc> npcs = new List<Npc>();
    }
}
