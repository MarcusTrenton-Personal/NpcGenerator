using System.Collections.Generic;
using System.Text;

namespace NpcGenerator
{
    public class Npc
    {
        public void AddTrait(string name)
        {
            traits.Add(name);
        }

        public string GetTraitAtIndex(int index)
        {
            return traits[index];
        }

        public string[] GetTraits()
        {
            return traits.ToArray();
        }

        public void ToCsvRow(StringBuilder stringBuilder)
        {
            for(int i = 0; i < traits.Count; ++i)
            {
                stringBuilder.Append(traits[i]);
                if (i + 1 < traits.Count)
                {
                    stringBuilder.Append(", ");
                }
            }
        }

        public int TraitCount { get { return traits.Count; } }

        private List<string> traits = new List<string>();
    }

    public class NpcGroup
    {
        public NpcGroup(TraitSchema traitSchema, int npcCount)
        {
            for(int i = 0; i < traitSchema.TraitCategoryCount; ++i)
            {
                traitGroupNames.Add(traitSchema.GetAtIndex(i).Name);
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
            
            TraitGroupNamesToCsv(text);
            text.Append("\n");

            for(int i = 0; i < npcs.Count; ++i)
            {
                npcs[i].ToCsvRow(text);
                if (i + 1 < npcs.Count)
                {
                    text.Append("\n");
                }
            }

            return text.ToString();
        }

        private void TraitGroupNamesToCsv(StringBuilder text)
        {
            for (int i = 0; i < traitGroupNames.Count; ++i)
            {
                text.Append(traitGroupNames[i]);
                if (i + 1 < traitGroupNames.Count)
                {
                    text.Append(", ");
                }
            }
        }

        public Npc GetNpcAtIndex(int index)
        {
            return npcs[index];
        }

        public string GetTraitGroupNameAtIndex(int index)
        {
            return traitGroupNames[index];
        }

        public int TraitGroupCount { get { return traitGroupNames.Count; } }
        public int NpcCount { get { return npcs.Count; } }

        private List<string> traitGroupNames = new List<string>();
        private List<Npc> npcs = new List<Npc>();
    }
}
