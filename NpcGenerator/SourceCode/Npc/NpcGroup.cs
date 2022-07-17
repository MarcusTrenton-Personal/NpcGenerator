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
                m_traitCategoryNames.Add(traitSchema.GetAtIndex(i).Name);
            }

            for(int i = 0; i < npcCount; ++i)
            {
                Npc npc = new Npc();
                for(int j = 0; j < traitSchema.TraitCategoryCount; ++j)
                {
                    TraitCategory traitCategory = traitSchema.GetAtIndex(j);
                    string category = traitCategory.Name;
                    string[] traits = traitCategory.Choose();
                    npc.AddTrait(category: category, traits: traits);
                }
                m_npcs.Add(npc);
            }
        }

        public string ToCsv()
        {
            StringBuilder text = new StringBuilder();
            
            TraitCategoryNamesToCsv(text);
            text.Append('\n');

            for(int i = 0; i < m_npcs.Count; ++i)
            {
                m_npcs[i].ToCsvRow(text, m_traitCategoryNames);
                if (i + 1 < m_npcs.Count)
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

                foreach(Npc npc in m_npcs)
                {
                    npc.ToJsonObject(writer, m_traitCategoryNames);
                }

                writer.WriteEndArray();
                writer.WriteEndObject(); //End of json

            }
            string json = sw.ToString();

            return json;
        }

        private void TraitCategoryNamesToCsv(StringBuilder text)
        {
            for (int i = 0; i < m_traitCategoryNames.Count; ++i)
            {
                text.Append(m_traitCategoryNames[i]);
                if (i + 1 < m_traitCategoryNames.Count)
                {
                    text.Append(',');
                }
            }
        }

        public Npc GetNpcAtIndex(int index)
        {
            return m_npcs[index];
        }

        public string GetTraitCategoryNameAtIndex(int index)
        {
            return m_traitCategoryNames[index];
        }

        public int TraitCategoryCount { get { return m_traitCategoryNames.Count; } }
        public int NpcCount { get { return m_npcs.Count; } }
        public IList<string> TraitCategories { get => m_traitCategoryNames; }

        private readonly List<string> m_traitCategoryNames = new List<string>();
        private readonly List<Npc> m_npcs = new List<Npc>();
    }
}
