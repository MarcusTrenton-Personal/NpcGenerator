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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NpcGenerator;

namespace Tests
{
    [TestClass]
    public class NpcGroupTests
    {
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