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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NpcGenerator;
using System;
using System.Collections.Generic;

namespace Tests
{
    [TestClass]
    public class NpcGroupTests
    {
        [TestMethod]
        public void InitialGroupIsEmpty()
        {
            List<string> categoryNames = new List<string>() { "Colour" };
            NpcGroup group = new NpcGroup(categoryNames);

            Assert.AreEqual(0, group.NpcCount, "Wrong number of Npcs stored");
        }

        [TestMethod]
        public void AddAndGetSingleNpc()
        {
            List<string> categoryNames = new List<string>() { "Colour" }; 
            NpcGroup group = new NpcGroup(categoryNames);

            Npc npc = new Npc();
            group.Add(npc);

            Assert.AreEqual(1, group.NpcCount, "Wrong number of Npcs stored");
            Assert.AreEqual(npc, group.GetNpcAtIndex(0), "Different Npc returned than was stored");
        }

        [TestMethod]
        public void DoubleAddNpc()
        {
            List<string> categoryNames = new List<string>() { "Colour" };
            NpcGroup group = new NpcGroup(categoryNames);

            Npc npc = new Npc();
            group.Add(npc);
            group.Add(npc);

            Assert.AreEqual(2, group.NpcCount, "Wrong number of Npcs stored");
            Assert.AreEqual(npc, group.GetNpcAtIndex(0), "Different Npc returned than was stored");
            Assert.AreEqual(npc, group.GetNpcAtIndex(1), "Different Npc returned than was stored");
        }

        [TestMethod]
        public void AddMultipleNpcs()
        {
            List<string> categoryNames = new List<string>() { "Colour" };
            NpcGroup group = new NpcGroup(categoryNames);

            Npc npc0 = new Npc();
            group.Add(npc0);
            Npc npc1 = new Npc();
            group.Add(npc1);

            Assert.AreEqual(2, group.NpcCount, "Wrong number of Npcs stored");
            Assert.AreEqual(npc0, group.GetNpcAtIndex(0), "Different Npc returned than was stored");
            Assert.AreEqual(npc1, group.GetNpcAtIndex(1), "Different Npc returned than was stored");
        }

        [TestMethod]
        public void GetWithoutAdd()
        {
            List<string> categoryNames = new List<string>() { "Colour" };
            NpcGroup group = new NpcGroup(categoryNames);

            bool threwException = false;
            try
            {
                Npc npc = group.GetNpcAtIndex(0);
            }
            catch(Exception)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Did not throw an exception for fetch an out of bounds index");
        }

        [TestMethod]
        public void EmptyTraitCategories()
        {
            List<string> categoryNames = new List<string>();
            NpcGroup group = new NpcGroup(categoryNames);

            Assert.AreEqual(0, group.CategoryOrder.Count, "Wrong number of Category names stored");
        }

        [TestMethod]
        public void SingleTraitCategory()
        {
            const string CATEGORY_NAME = "Colour";
            List<string> categoryNames = new List<string>() { CATEGORY_NAME };
            NpcGroup group = new NpcGroup(categoryNames);

            Assert.AreEqual(1, group.CategoryOrder.Count, "Wrong number of Category names stored");
            Assert.AreEqual(CATEGORY_NAME, group.CategoryOrder[0], "Wrong Category name");
        }

        [TestMethod]
        public void MultipleTraitCategories()
        {
            const string CATEGORY_NAME0 = "Colour";
            const string CATEGORY_NAME1 = "Animal";
            List<string> categoryNames = new List<string>() { CATEGORY_NAME0, CATEGORY_NAME1 };
            NpcGroup group = new NpcGroup(categoryNames);

            Assert.AreEqual(2, group.CategoryOrder.Count, "Wrong number of Trait names stored");
            Assert.AreEqual(CATEGORY_NAME0, group.CategoryOrder[0], "Wrong Category name");
            Assert.AreEqual(CATEGORY_NAME1, group.CategoryOrder[1], "Wrong Category name");
        }
    }
}
