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
using Newtonsoft.Json;
using NpcGenerator;
using Services;
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
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT, CATEGORY, isHidden: false) });
            Npc.Trait[] traits = npc.GetTraitsOfCategory(CATEGORY);

            Assert.IsNotNull(traits, "Returned array should never be null. At worst it is empty.");
            Assert.AreEqual(1, traits.Length, "Wrong number of traits found.");
            Assert.AreEqual(TRAIT, traits[0].Name, "Wrong trait returned.");
        }

        [TestMethod]
        public void AddEmptyTraitArray()
        {
            const string CATEGORY = "Colour";

            Npc npc = new Npc();
            npc.Add(CATEGORY, Array.Empty<Npc.Trait>());
            Npc.Trait[] traits = npc.GetTraitsOfCategory(CATEGORY);

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
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT0, CATEGORY), new Npc.Trait(TRAIT1, CATEGORY) });
            Npc.Trait[] traits = npc.GetTraitsOfCategory(CATEGORY);

            Assert.IsNotNull(traits, "Returned array should never be null. At worst it is empty.");
            Assert.AreEqual(2, traits.Length, "Wrong number of traits found.");

            bool foundTrait0 = Array.FindIndex(traits, trait => trait.Name == TRAIT0) > -1;
            Assert.AreEqual(TRAIT0, traits[0].Name, "Wrong trait returned.");

            bool foundTrait1 = Array.FindIndex(traits, trait => trait.Name == TRAIT1) > -1;
            Assert.AreEqual(TRAIT1, traits[1].Name, "Wrong trait returned.");
        }

        [TestMethod]
        public void RepeatedAddTraits()
        {
            const string CATEGORY = "Colour";
            const string TRAIT0 = "Blue";
            const string TRAIT1 = "Green";

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT0, CATEGORY) });
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT1, CATEGORY) });
            Npc.Trait[] traits = npc.GetTraitsOfCategory(CATEGORY);

            Assert.IsNotNull(traits, "Returned array should never be null. At worst it is empty.");
            Assert.AreEqual(2, traits.Length, "Wrong number of traits found.");

            bool foundTrait0 = Array.FindIndex(traits, trait => trait.Name == TRAIT0) > -1;
            Assert.AreEqual(TRAIT0, traits[0].Name, "Wrong trait returned.");

            bool foundTrait1 = Array.FindIndex(traits, trait => trait.Name == TRAIT1) > -1;
            Assert.AreEqual(TRAIT1, traits[1].Name, "Wrong trait returned.");
        }

        [TestMethod]
        public void RedundantAddTraits()
        {
            const string CATEGORY = "Colour";
            const string TRAIT = "Blue";
            Npc.Trait trait = new Npc.Trait(TRAIT, CATEGORY, isHidden: false);

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { trait, trait });
            Npc.Trait[] traits = npc.GetTraitsOfCategory(CATEGORY);

            Assert.IsNotNull(traits, "Returned array should never be null. At worst it is empty.");
            Assert.AreEqual(1, traits.Length, "Wrong number of traits found.");
            Assert.AreEqual(TRAIT, traits[0].Name, "Wrong trait returned.");
        }

        [TestMethod]
        public void GetCategoryWithoutTraits()
        {
            const string CATEGORY = "Colour";

            Npc npc = new Npc();
            Npc.Trait[] traits = npc.GetTraitsOfCategory(CATEGORY);

            Assert.IsNotNull(traits, "Returned array should never be null. At worst it is empty.");
            Assert.AreEqual(0, traits.Length, "Wrong number of traits found.");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void AddNullCategory()
        {
            Npc npc = new Npc();
            string category = null;
            npc.Add(category, new Npc.Trait[] { new Npc.Trait("Blue", category) });
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void AddEmptyCategory()
        {
            string CATEGORY = string.Empty;

            Npc npc = new Npc();
            
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait("Blue", CATEGORY) });
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void AddNullTraitArray()
        {
            Npc npc = new Npc();

            npc.Add("Colour", null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void AddNullTrait()
        {
            Npc npc = new Npc();

            npc.Add("Colour", new Npc.Trait[] { null });
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void AddNullTraitName()
        {
            const string CATEGORY = "Colour";

            Npc npc = new Npc();

            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(null, CATEGORY) });
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void AddEmptyTrait()
        {
            const string CATEGORY = "Colour";

            Npc npc = new Npc();

            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(string.Empty, CATEGORY) });
        }

        [TestMethod]
        public void HasTraitInEmptyNpc()
        {
            Npc npc = new Npc();

            bool hasTrait = npc.HasTrait(new TraitId("Animal", "Bear"));

            Assert.IsFalse(hasTrait, "Incorrectly found trait in empty Npc");
        }

        [TestMethod]
        public void HasTraitWhereCategoryDoesNotExist()
        {
            const string CATEGORY = "Colour";

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait("Blue", CATEGORY) });

            bool hasTrait = npc.HasTrait(new TraitId("Animal", "Bear"));

            Assert.IsFalse(hasTrait, "Incorrectly found trait that is not in Npc");
        }

        [TestMethod]
        public void HasTraitWhereTraitDoesNotExit()
        {
            const string CATEGORY = "Colour";

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait("Blue", CATEGORY) });

            bool hasTrait = npc.HasTrait(new TraitId(CATEGORY, "Red"));

            Assert.IsFalse(hasTrait, "Incorrectly found trait that is not in Npc");
        }

        [TestMethod]
        public void HasTraitWhereTraitNameIsInDifferentCategory()
        {
            const string CATEGORY0 = "Skin";
            const string CATEGORY1 = "Race";
            const string TRAIT = "Black";

            Npc npc = new Npc();
            npc.Add(CATEGORY0, new Npc.Trait[] { new Npc.Trait(TRAIT, CATEGORY0) });

            bool hasTrait = npc.HasTrait(new TraitId(CATEGORY1, TRAIT));

            Assert.IsFalse(hasTrait, "Incorrectly found trait that is not in Npc");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void HasTraitWithNullParameter()
        {
            Npc npc = new Npc();

            npc.HasTrait(null);
        }

        [TestMethod]
        public void HasTraitThatExists()
        {
            const string CATEGORY = "Animal";
            const string TRAIT = "Bear";

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT, CATEGORY) });

            bool hasTrait = npc.HasTrait(new TraitId(CATEGORY, TRAIT));

            Assert.IsTrue(hasTrait, "Failed to found trait that is in Npc");
        }

        [TestMethod]
        public void GetCategoriesEmpty()
        {
            Npc npc = new Npc();

            IReadOnlyList<string> categories = npc.GetCategories();

            Assert.AreEqual(0, categories.Count, "Wrong number of categories");
        }

        [TestMethod]
        public void GetCategoriesSingle()
        {
            const string CATEGORY = "Animal";

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait("Bear", CATEGORY) });

            IReadOnlyList<string> categories = npc.GetCategories();

            Assert.AreEqual(1, categories.Count, "Wrong number of categories");
            Assert.AreEqual(CATEGORY, categories[0], "Wrong category name");
        }

        [TestMethod]
        public void GetCategoriesMultiple()
        {
            const string CATEGORY0 = "Animal";
            const string CATEGORY1 = "Colour";

            Npc npc = new Npc();
            npc.Add(CATEGORY0, new Npc.Trait[] { new Npc.Trait("Bear", CATEGORY0) });
            npc.Add(CATEGORY1, new Npc.Trait[] { new Npc.Trait("Blue", CATEGORY1) });

            IReadOnlyList<string> categories = npc.GetCategories();

            Assert.AreEqual(2, categories.Count, "Wrong number of categories");
            string category0 = ListUtil.Find(categories, category => category == CATEGORY0);
            Assert.AreEqual(CATEGORY0, category0, "Wrong category name");
            string category1 = ListUtil.Find(categories, category => category == CATEGORY1);
            Assert.AreEqual(CATEGORY1, category1, "Wrong category name");
        }

        [TestMethod]
        public void AddSameTraitTwice()
        {
            const string CATEGORY = "Animal";
            const string TRAIT = "Bear";

            Npc npc = new Npc();
            npc.Add("Animal", new Npc.Trait[] { new Npc.Trait(TRAIT, CATEGORY), new Npc.Trait(TRAIT, CATEGORY) });

            IReadOnlyList<string> categories = npc.GetCategories();

            Assert.AreEqual(1, categories.Count, "Wrong number of categories");
            string category = ListUtil.Find(categories, category => category == CATEGORY);
            Assert.AreEqual(CATEGORY, category, "Wrong category name");

            Npc.Trait[] traits = npc.GetTraitsOfCategory(CATEGORY);
            Assert.AreEqual(1, traits.Length, "Duplicate Traits found");
            Assert.AreEqual(TRAIT, traits[0].Name, "Wrong trait name");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void HasCategoryNull()
        {
            Npc npc = new Npc();

            bool hasCategory = npc.HasCategory(null);

            Assert.IsFalse(hasCategory, "Wrong result from HasCategory");
        }

        [TestMethod]
        public void HasCategoryEmpty()
        {
            Npc npc = new Npc();

            bool hasCategory = npc.HasCategory(string.Empty);

            Assert.IsFalse(hasCategory, "Wrong result from HasCategory");
        }

        [TestMethod]
        public void HasCategoryTrue()
        {
            const string CATEGORY = "Animal";

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { });

            bool hasCategory = npc.HasCategory(CATEGORY);

            Assert.IsTrue(hasCategory, "Wrong result from HasCategory");
        }

        [TestMethod]
        public void HasCategoryFalse()
        {
            Npc npc = new Npc();
            npc.Add("Animal", new Npc.Trait[] { });

            bool hasCategory = npc.HasCategory("Colour");

            Assert.IsFalse(hasCategory, "Wrong result from HasCategory");
        }

#pragma warning disable CS0253 // Possible unintended reference comparison. Intentional for testing equality.
#pragma warning disable CS1718 // Comparison made to the same variable. Intential for testing equality.

        [TestMethod]
        public void NpcTraitEqualitySameValues()
        {
            const string NAME = "Blue";
            const string CATEGORY = "Colour";
            const bool IS_HIDDEN = false;

            Npc.Trait a = new Npc.Trait(NAME, CATEGORY, IS_HIDDEN);
            Npc.Trait b = new Npc.Trait(NAME, CATEGORY, IS_HIDDEN);

            Assert.IsTrue(a.Equals(b), "Incorrectly unequal");
            Assert.IsTrue(a == b, "Incorrectly unequal");
            Assert.IsFalse(a != b, "Incorrectly unequal");
        }

        [TestMethod]
        public void NpcTraitEqualityDifferentIsHidden()
        {
            const string NAME = "Blue";
            const string CATEGORY = "Colour";

            Npc.Trait a = new Npc.Trait(NAME, CATEGORY, isHidden: true);
            Npc.Trait b = new Npc.Trait(NAME, CATEGORY, isHidden: false);

            Assert.IsFalse(a.Equals(b), "Incorrectly equal");
            Assert.IsFalse(a == b, "Incorrectly equal");
            Assert.IsTrue(a != b, "Incorrectly equal");
        }

        [TestMethod]
        public void NpcTraitEqualityDifferentName()
        {
            const bool IS_HIDDEN = false;
            const string CATEGORY = "Colour";

            Npc.Trait a = new Npc.Trait(name: "Red", originalCategory: CATEGORY, isHidden: IS_HIDDEN);
            Npc.Trait b = new Npc.Trait(name: "Blue", originalCategory: CATEGORY, isHidden: IS_HIDDEN);

            Assert.IsFalse(a.Equals(b), "Incorrectly equal");
            Assert.IsFalse(a == b, "Incorrectly equal");
            Assert.IsTrue(a != b, "Incorrectly equal");
        }

        [TestMethod]
        public void NpcTraitEqualityDifferentOriginalName()
        {
            const string NAME = "Blue";
            const string CATEGORY = "Colour";
            const bool IS_HIDDEN = false;

            Npc.Trait a = new Npc.Trait(NAME, CATEGORY, IS_HIDDEN, originalName: "Black");
            Npc.Trait b = new Npc.Trait(NAME, CATEGORY, IS_HIDDEN, originalName: "White");

            Assert.IsFalse(a.Equals(b), "Incorrectly equal");
            Assert.IsFalse(a == b, "Incorrectly equal");
            Assert.IsTrue(a != b, "Incorrectly equal");
        }

        [TestMethod]
        public void NpcTraitEqualityDifferentOriginalCategory()
        {
            const string NAME = "Blue";
            const bool IS_HIDDEN = false;
            const string ORIGINAL_NAME = "Red";

            Npc.Trait a = new Npc.Trait(NAME, originalCategory: "Colour", IS_HIDDEN, originalName: ORIGINAL_NAME);
            Npc.Trait b = new Npc.Trait(NAME, originalCategory: "Shade", IS_HIDDEN, originalName: ORIGINAL_NAME);

            Assert.IsFalse(a.Equals(b), "Incorrectly equal");
            Assert.IsFalse(a == b, "Incorrectly equal");
            Assert.IsTrue(a != b, "Incorrectly equal");
        }

        [TestMethod]
        public void NpcTraitEqualityOtherIsNull()
        {
            Npc.Trait a = new Npc.Trait(name: "Red", originalCategory: "Colour", isHidden: false);
            Npc.Trait b = null;

            Assert.IsFalse(a.Equals(b), "Incorrectly equal");
            Assert.IsFalse(a == b, "Incorrectly equal");
            Assert.IsTrue(a != b, "Incorrectly equal");
        }

        [TestMethod]
        public void NpcTraitEqualityAgainstDifferentClass()
        {
            Npc.Trait a = new Npc.Trait(name: "Red", originalCategory: "Colour", isHidden: false);
            object b = new object();

            Assert.IsFalse(a.Equals(b), "Incorrectly equal");
            Assert.IsFalse(a == b, "Incorrectly equal");
            Assert.IsTrue(a != b, "Incorrectly equal");
        }

        [TestMethod]
        public void NpcTraitEqualityVsSelf()
        {
            Npc.Trait a = new Npc.Trait(name: "Red", originalCategory: "Colour", isHidden: false);

            Assert.IsTrue(a.Equals(a), "Incorrectly unequal");
            Assert.IsTrue(a == a, "Incorrectly unequal");
            Assert.IsFalse(a != a, "Incorrectly unequal");
        }

#pragma warning restore CS0253 // Possible unintended reference comparison. Intentional for testing equality.
#pragma warning restore CS1718 // Comparison made to the same variable. Intential for testing equality.
    }
}
