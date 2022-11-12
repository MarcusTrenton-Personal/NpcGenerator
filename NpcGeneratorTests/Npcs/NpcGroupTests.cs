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
along with this program. If not, see <https://www.gnu.org/licenses/>.*/

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
            List<NpcGroup.Category> categoryOrder = new List<NpcGroup.Category>() { new NpcGroup.Category("Colour") };
            NpcGroup group = new NpcGroup(categoryOrder);

            Assert.AreEqual(0, group.NpcCount, "Wrong number of Npcs stored");
        }

        [TestMethod]
        public void AddAndGetSingleNpc()
        {
            List<NpcGroup.Category> categoryOrder = new List<NpcGroup.Category>() { new NpcGroup.Category("Colour") };
            NpcGroup group = new NpcGroup(categoryOrder);

            Npc npc = new Npc();
            group.Add(npc);

            Assert.AreEqual(1, group.NpcCount, "Wrong number of Npcs stored");
            Assert.AreEqual(npc, group.GetNpcAtIndex(0), "Different Npc returned than was stored");
        }

        [TestMethod]
        public void DoubleAddNpc()
        {
            List<NpcGroup.Category> categoryOrder = new List<NpcGroup.Category>() { new NpcGroup.Category("Colour") };
            NpcGroup group = new NpcGroup(categoryOrder);

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
            List<NpcGroup.Category> categoryOrder = new List<NpcGroup.Category>() { new NpcGroup.Category("Colour") };
            NpcGroup group = new NpcGroup(categoryOrder);

            Npc npc0 = new Npc();
            group.Add(npc0);
            Npc npc1 = new Npc();
            group.Add(npc1);

            Assert.AreEqual(2, group.NpcCount, "Wrong number of Npcs stored");
            Assert.AreEqual(npc0, group.GetNpcAtIndex(0), "Different Npc returned than was stored");
            Assert.AreEqual(npc1, group.GetNpcAtIndex(1), "Different Npc returned than was stored");
        }

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetWithoutAdd()
        {
            List<NpcGroup.Category> categoryOrder = new List<NpcGroup.Category>() { new NpcGroup.Category("Colour") };
            NpcGroup group = new NpcGroup(categoryOrder);

            group.GetNpcAtIndex(0);
        }

        [TestMethod]
        public void EmptyTraitCategories()
        {
            List<NpcGroup.Category> categoryOrder = new List<NpcGroup.Category>();
            NpcGroup group = new NpcGroup(categoryOrder);

            Assert.AreEqual(0, group.CategoryOrder.Count, "Wrong number of Category names stored");
        }

        [TestMethod]
        public void SingleTraitCategory()
        {
            const string CATEGORY_NAME = "Colour";
            List<NpcGroup.Category> categoryOrder = new List<NpcGroup.Category>() { new NpcGroup.Category(CATEGORY_NAME) };
            NpcGroup group = new NpcGroup(categoryOrder);

            Assert.AreEqual(1, group.CategoryOrder.Count, "Wrong number of Category names stored");
            Assert.AreEqual(CATEGORY_NAME, group.CategoryOrder[0].Name, "Wrong Category name");
        }

        [TestMethod]
        public void MultipleTraitCategories()
        {
            const string CATEGORY_NAME0 = "Colour";
            const string CATEGORY_NAME1 = "Animal";
            List<NpcGroup.Category> categoryOrder = new List<NpcGroup.Category>() { 
                new NpcGroup.Category(CATEGORY_NAME0), new NpcGroup.Category(CATEGORY_NAME1) };
            NpcGroup group = new NpcGroup(categoryOrder);

            Assert.AreEqual(2, group.CategoryOrder.Count, "Wrong number of Trait names stored");
            Assert.AreEqual(CATEGORY_NAME0, group.CategoryOrder[0].Name, "Wrong Category name");
            Assert.AreEqual(CATEGORY_NAME1, group.CategoryOrder[1].Name, "Wrong Category name");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void NullCategoryList()
        {
            new NpcGroup(null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void NullCategoryInList()
        {
            new NpcGroup(new List<NpcGroup.Category> { null });
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void EmptyCategoryInList()
        {
            new NpcGroup(new List<NpcGroup.Category> { new NpcGroup.Category(string.Empty) });
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void DuplicateCategoryInList()
        {
            const string CATEGORY = "Animal";
            new NpcGroup(new List<NpcGroup.Category> { new NpcGroup.Category(CATEGORY), new NpcGroup.Category(CATEGORY) });
        }

        [TestMethod]
        public void VisibleCategoryOrderFromEmpty()
        {
            NpcGroup group = new NpcGroup(new List<NpcGroup.Category>());

            Assert.AreEqual(0, group.VisibleCategoryOrder.Count, "Wrong number of visible categories");
        }

        [TestMethod]
        public void VisibleCategoryOrderFromVisibleOriginals()
        {
            const string CATEGORY0 = "Animal";
            const string CATEGORY1 = "Building";
            const string CATEGORY2 = "Colour";

            NpcGroup group = new NpcGroup(new List<NpcGroup.Category>() {
                new NpcGroup.Category(CATEGORY0), new NpcGroup.Category(CATEGORY1), new NpcGroup.Category(CATEGORY2) 
            });

            Assert.AreEqual(3, group.VisibleCategoryOrder.Count, "Wrong number of visible categories");
            Assert.AreEqual(CATEGORY0, group.VisibleCategoryOrder[0], "Wrong ordered, visible category");
            Assert.AreEqual(CATEGORY1, group.VisibleCategoryOrder[1], "Wrong ordered, visible category");
            Assert.AreEqual(CATEGORY2, group.VisibleCategoryOrder[2], "Wrong ordered, visible category");
        }

        [TestMethod]
        public void VisibleCategoryOrderFromHiddenOriginals()
        {
            const string CATEGORY0 = "Animal";
            const string CATEGORY1 = "Building";
            const string CATEGORY2 = "Colour";

            NpcGroup group = new NpcGroup(new List<NpcGroup.Category>() {
                new NpcGroup.Category(CATEGORY0, isHidden: true), 
                new NpcGroup.Category(CATEGORY1, isHidden: true), 
                new NpcGroup.Category(CATEGORY2, isHidden: true)
            });

            Assert.AreEqual(0, group.VisibleCategoryOrder.Count, "Wrong number of visible categories");
        }

        [TestMethod]
        public void VisibleCategoryOrderFromMixedOriginals()
        {
            const string CATEGORY0 = "Animal";
            const string CATEGORY1 = "Building";
            const string CATEGORY2 = "Colour";
            const string CATEGORY3 = "Demeanor";

            NpcGroup group = new NpcGroup(new List<NpcGroup.Category>() {
                new NpcGroup.Category(CATEGORY0),
                new NpcGroup.Category(CATEGORY1, isHidden: true),
                new NpcGroup.Category(CATEGORY2),
                new NpcGroup.Category(CATEGORY3, isHidden: true)
            });

            Assert.AreEqual(2, group.VisibleCategoryOrder.Count, "Wrong number of visible categories");
            Assert.AreEqual(CATEGORY0, group.VisibleCategoryOrder[0], "Wrong ordered, visible category");
            Assert.AreEqual(CATEGORY2, group.VisibleCategoryOrder[1], "Wrong ordered, visible category");
        }

#pragma warning disable CS0253 // Possible unintended reference comparison. Intentional for testing equality.
#pragma warning disable CS1718 // Comparison made to the same variable. Intential for testing equality.

        [TestMethod]
        public void NpcCategoryEqualitySameValues()
        {
            const string CATEGORY = "Colour";
            const bool IS_HIDDEN = false;

            NpcGroup.Category a = new NpcGroup.Category(CATEGORY, IS_HIDDEN);
            NpcGroup.Category b = new NpcGroup.Category(CATEGORY, IS_HIDDEN);

            Assert.IsTrue(a.Equals(b), "Incorrectly unequal");
            Assert.IsTrue(a == b, "Incorrectly unequal");
            Assert.IsFalse(a != b, "Incorrectly unequal");
        }

        [TestMethod]
        public void NpcCategoryEqualityDifferentIsHidden()
        {
            const string CATEGORY = "Colour";

            NpcGroup.Category a = new NpcGroup.Category(CATEGORY, isHidden: true);
            NpcGroup.Category b = new NpcGroup.Category(CATEGORY, isHidden: false);

            Assert.IsFalse(a.Equals(b), "Incorrectly equal");
            Assert.IsFalse(a == b, "Incorrectly equal");
            Assert.IsTrue(a != b, "Incorrectly equal");
        }

        [TestMethod]
        public void NpcCategoryEqualityDifferentCategoryName()
        {
            const bool IS_HIDDEN = false;

            NpcGroup.Category a = new NpcGroup.Category(name: "Colour", IS_HIDDEN);
            NpcGroup.Category b = new NpcGroup.Category(name: "Shade", IS_HIDDEN);

            Assert.IsFalse(a.Equals(b), "Incorrectly equal");
            Assert.IsFalse(a == b, "Incorrectly equal");
            Assert.IsTrue(a != b, "Incorrectly equal");
        }

        [TestMethod]
        public void NpcCategoryEqualityOtherIsNull()
        {
            NpcGroup.Category a = new NpcGroup.Category(name: "Red", isHidden: false);
            NpcGroup.Category b = null;

            Assert.IsFalse(a.Equals(b), "Incorrectly equal");
            Assert.IsFalse(a == b, "Incorrectly equal");
            Assert.IsTrue(a != b, "Incorrectly equal");
        }

        [TestMethod]
        public void NpcCategoryEqualityAgainstDifferentClass()
        {
            NpcGroup.Category a = new NpcGroup.Category(name: "Red", isHidden: false);
            object b = new object();

            Assert.IsFalse(a.Equals(b), "Incorrectly equal");
            Assert.IsFalse(a == b, "Incorrectly equal");
            Assert.IsTrue(a != b, "Incorrectly equal");
        }

        [TestMethod]
        public void NpcCategoryEqualityVsSelf()
        {
            NpcGroup.Category a = new NpcGroup.Category(name: "Red", isHidden: false);

            Assert.IsTrue(a.Equals(a), "Incorrectly unequal");
            Assert.IsTrue(a == a, "Incorrectly unequal");
            Assert.IsFalse(a != a, "Incorrectly unequal");
        }

#pragma warning restore CS0253 // Possible unintended reference comparison. Intentional for testing equality.
#pragma warning restore CS1718 // Comparison made to the same variable. Intential for testing equality.
    }
}
