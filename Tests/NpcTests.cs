using Microsoft.VisualStudio.TestTools.UnitTesting;
using NpcGenerator;
using System.Text;

namespace Tests
{
    [TestClass]
    public class NpcTests
    {
        [TestMethod]
        public void NpcGeneratesCsv()
        {
            Npc npc = new Npc();
            npc.AddTrait("Blue");
            npc.AddTrait("Bear");

            StringBuilder textBuilder = new StringBuilder();
            npc.ToCsvRow(textBuilder);
            string csvRow = textBuilder.ToString();

            Assert.AreEqual("Blue,Bear", csvRow, "Npc did not generate expected CSV row");
        }

        [TestMethod]
        public void NpcGroupGeneratesCsv()
        {
            TraitCategory colourCategory = new TraitCategory("Colour");
            colourCategory.Add(new Trait("Blue", 1));

            TraitCategory animalCategory = new TraitCategory("Animal");
            animalCategory.Add(new Trait("Bear", 1));

            TraitSchema schema = new TraitSchema();
            schema.Add(colourCategory);
            schema.Add(animalCategory);

            NpcGroup npcGroup = new NpcGroup(schema, 1);
            string csv = npcGroup.ToCsv();

            Assert.AreEqual("Colour,Animal\nBlue,Bear", csv, "NpcGroup did not generate expected CSV text");
        }
    }
}
