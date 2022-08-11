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
using Newtonsoft.Json;
using NpcGenerator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Tests
{
    [TestClass]
    public class NpcTests
    {
        [TestMethod]
        public void AddSingle()
        {
            const string CATEGORY = "Colour";
            const string TRAIT = "Blue";

            Npc npc = new Npc();
            npc.Add(CATEGORY, new string[] { TRAIT });
            string[] traits = npc.GetTraitsOfCategory(CATEGORY);

            Assert.IsNotNull(traits, "Returned array should never be null. At worst it is empty.");
            Assert.AreEqual(1, traits.Length, "Wrong number of traits found.");
            Assert.AreEqual(TRAIT, traits[0], "Wrong trait returned.");
        }

        [TestMethod]
        public void AddEmptyTraitArray()
        {
            const string CATEGORY = "Colour";

            Npc npc = new Npc();
            npc.Add(CATEGORY, Array.Empty<string>());
            string[] traits = npc.GetTraitsOfCategory(CATEGORY);

            Assert.IsNotNull(traits, "Returned array should never be null. At worst it is empty.");
            Assert.AreEqual(0, traits.Length, "Wrong number of traits found.");
        }

        [TestMethod]
        public void AddMultipleTraits()
        {
            const string CATEGORY = "Colour";
            const string TRAIT0 = "Blue";
            const string TRAIT1 = "Green";

            Npc npc = new Npc();
            npc.Add(CATEGORY, new string[] { TRAIT0, TRAIT1 });
            string[] traits = npc.GetTraitsOfCategory(CATEGORY);

            Assert.IsNotNull(traits, "Returned array should never be null. At worst it is empty.");
            Assert.AreEqual(2, traits.Length, "Wrong number of traits found.");

            bool foundTrait0 = Array.FindIndex(traits, name => name == TRAIT0) > -1;
            Assert.AreEqual(TRAIT0, traits[0], "Wrong trait returned.");

            bool foundTrait1 = Array.FindIndex(traits, name => name == TRAIT1) > -1;
            Assert.AreEqual(TRAIT1, traits[1], "Wrong trait returned.");
        }

        [TestMethod]
        public void RepeatedAddTraits()
        {
            const string CATEGORY = "Colour";
            const string TRAIT0 = "Blue";
            const string TRAIT1 = "Green";

            Npc npc = new Npc();
            npc.Add(CATEGORY, new string[] { TRAIT0 });
            npc.Add(CATEGORY, new string[] { TRAIT1 });
            string[] traits = npc.GetTraitsOfCategory(CATEGORY);

            Assert.IsNotNull(traits, "Returned array should never be null. At worst it is empty.");
            Assert.AreEqual(2, traits.Length, "Wrong number of traits found.");

            bool foundTrait0 = Array.FindIndex(traits, name => name == TRAIT0) > -1;
            Assert.AreEqual(TRAIT0, traits[0], "Wrong trait returned.");

            bool foundTrait1 = Array.FindIndex(traits, name => name == TRAIT1) > -1;
            Assert.AreEqual(TRAIT1, traits[1], "Wrong trait returned.");
        }

        [TestMethod]
        public void GetCategoryWithoutTraits()
        {
            const string CATEGORY = "Colour";

            Npc npc = new Npc();
            string[] traits = npc.GetTraitsOfCategory(CATEGORY);

            Assert.IsNotNull(traits, "Returned array should never be null. At worst it is empty.");
            Assert.AreEqual(0, traits.Length, "Wrong number of traits found.");
        }

        [TestMethod]
        public void AddNullCategory()
        {
            const string TRAIT = "Blue";

            Npc npc = new Npc();

            bool threwException = false;
            try
            {
                npc.Add(null, new string[] { TRAIT });
            }
            catch (Exception)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException);
        }

        [TestMethod]
        public void AddEmptyCategory()
        {
            const string TRAIT = "Blue";

            Npc npc = new Npc();

            bool threwException = false;
            try
            {
                npc.Add(string.Empty, new string[] { TRAIT });
            }
            catch (Exception)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException);
        }

        [TestMethod]
        public void AddNullTraitArray()
        {
            const string CATEGORY = "Colour";

            Npc npc = new Npc();

            bool threwException = false;
            try
            {
                npc.Add(CATEGORY, null);
            }
            catch (Exception)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException);
        }

        [TestMethod]
        public void AddNullTrait()
        {
            const string CATEGORY = "Colour";

            Npc npc = new Npc();

            bool threwException = false;
            try
            {
                npc.Add(CATEGORY, new string[] { null });
            }
            catch (Exception)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException);
        }

        [TestMethod]
        public void AddEmptyTrait()
        {
            const string CATEGORY = "Colour";

            Npc npc = new Npc();

            bool threwException = false;
            try
            {
                npc.Add(CATEGORY, new string[] { string.Empty });
            }
            catch (Exception)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException);
        }
    }
}
