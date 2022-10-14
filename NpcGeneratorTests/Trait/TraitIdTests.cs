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

namespace Tests
{
#pragma warning disable CS0253 // Possible unintended reference comparison. Intentional for testing equality.
#pragma warning disable CS1718 // Comparison made to the same variable. Intential for testing equality.

    [TestClass]
    public class TraitIdTests
    {
        [TestMethod]
        public void EqualsNull()
        {
            TraitId traitId = new TraitId("Colour", "Blue");

            Assert.IsFalse(traitId.Equals(null), "Equals() incorrect");
            Assert.IsFalse(traitId == null, "== incorrect");
            Assert.IsTrue(traitId != null, "!= incorrect");
        }

        [TestMethod]
        public void EqualsObject()
        {
            TraitId traitId = new TraitId("Colour", "Blue");
            object empty = new object();

            Assert.IsFalse(traitId.Equals(empty), "Equals() incorrect");
            Assert.IsFalse(traitId == empty, "== incorrect");
            Assert.IsTrue(traitId != empty, "!= incorrect");
        }

        [TestMethod]
        public void EqualsMismatchedTrait()
        {
            TraitId a = new TraitId("Colour", "Blue");
            TraitId b = new TraitId("Colour", "Red");

            Assert.IsFalse(a.Equals(b), "Equals() incorrect");
            Assert.IsFalse(a == b, "== incorrect");
            Assert.IsTrue(a != b, "!= incorrect");
        }

        [TestMethod]
        public void EqualsMismatchedCategory()
        {
            TraitId a = new TraitId("Colour", "Blue");
            TraitId b = new TraitId("Hair", "Blue");

            Assert.IsFalse(a.Equals(b), "Equals() incorrect");
            Assert.IsFalse(a == b, "== incorrect");
            Assert.IsTrue(a != b, "!= incorrect");
        }

        [TestMethod]
        public void EqualsMismatchedEverything()
        {
            TraitId a = new TraitId("Colour", "Blue");
            TraitId b = new TraitId("Hair", "Red");

            Assert.IsFalse(a.Equals(b), "Equals() incorrect");
            Assert.IsFalse(a == b, "== incorrect");
            Assert.IsTrue(a != b, "!= incorrect");
        }

        [TestMethod]
        public void EqualsDifferentCaseTrait()
        {
            TraitId a = new TraitId("Colour", "Blue");
            TraitId b = new TraitId("Colour", "blue");

            Assert.IsFalse(a.Equals(b), "Equals() incorrect");
            Assert.IsFalse(a == b, "== incorrect");
            Assert.IsTrue(a != b, "!= incorrect");
        }

        [TestMethod]
        public void EqualsDifferentCaseCategory()
        {
            TraitId a = new TraitId("Colour", "Blue");
            TraitId b = new TraitId("colour", "Blue");

            Assert.IsFalse(a.Equals(b), "Equals() incorrect");
            Assert.IsFalse(a == b, "== incorrect");
            Assert.IsTrue(a != b, "!= incorrect");
        }

        [TestMethod]
        public void EqualsWithBothNullTraits()
        {
            TraitId a = new TraitId("Colour", null);
            TraitId b = new TraitId("Colour", null);

            Assert.IsTrue(a.Equals(b), "Equals() incorrect");
            Assert.IsTrue(a == b, "== incorrect");
            Assert.IsFalse(a != b, "!= incorrect");
        }

        [TestMethod]
        public void EqualsWithOneNullTrait()
        {
            TraitId a = new TraitId("Colour", "Blue");
            TraitId b = new TraitId("Colour", null);

            Assert.IsFalse(a.Equals(b), "Equals() incorrect");
            Assert.IsFalse(a == b, "== incorrect");
            Assert.IsTrue(a != b, "!= incorrect");
        }

        [TestMethod]
        public void EqualsWithBothNullCategories()
        {
            TraitId a = new TraitId(null, "Blue");
            TraitId b = new TraitId(null, "Blue");

            Assert.IsTrue(a.Equals(b), "Equals() incorrect");
            Assert.IsTrue(a == b, "== incorrect");
            Assert.IsFalse(a != b, "!= incorrect");
        }

        [TestMethod]
        public void EqualsWithOneNullCategory()
        {
            TraitId a = new TraitId("Colour", "Blue");
            TraitId b = new TraitId(null, "Blue");

            Assert.IsFalse(a.Equals(b), "Equals() incorrect");
            Assert.IsFalse(a == b, "== incorrect");
            Assert.IsTrue(a != b, "!= incorrect");
        }

        [TestMethod]
        public void EqualsWithEqualValues()
        {
            TraitId a = new TraitId("Colour", "Blue");
            TraitId b = new TraitId("Colour", "Blue");

            Assert.IsTrue(a.Equals(b), "Equals() incorrect");
            Assert.IsTrue(a == b, "== incorrect");
            Assert.IsFalse(a != b, "!= incorrect");
        }

        [TestMethod]
        public void EqualsThis()
        {
            TraitId a = new TraitId("Colour", "Blue");

            Assert.IsTrue(a.Equals(a), "Equals() incorrect");
            Assert.IsTrue(a == a, "== incorrect");
            Assert.IsFalse(a != a, "!= incorrect");
        }

        [TestMethod]
        public void GetHashCodeIsStable()
        {
            TraitId traitId = new TraitId("Colour", "Blue");
            int hashCode0 = traitId.GetHashCode();
            int hashCode1 = traitId.GetHashCode();

            Assert.AreEqual(hashCode0, hashCode1, "Hashcode calculation is not stable");
        }

        [TestMethod]
        public void GetHashCodeForDifferentCategories()
        {
            TraitId traitId0 = new TraitId("Colour", "Blue");
            TraitId traitId1 = new TraitId("Hair", "Blue");

            int hashCode0 = traitId0.GetHashCode();
            int hashCode1 = traitId1.GetHashCode();

            Assert.AreNotEqual(hashCode0, hashCode1, "Hashcode ignores category");
        }

        [TestMethod]
        public void GetHashCodeForDifferentTraits()
        {
            TraitId traitId0 = new TraitId("Colour", "Blue");
            TraitId traitId1 = new TraitId("Colour", "Red");

            int hashCode0 = traitId0.GetHashCode();
            int hashCode1 = traitId1.GetHashCode();

            Assert.AreNotEqual(hashCode0, hashCode1, "Hashcode ignores trait");
        }

        [TestMethod]
        public void GetHashCodeForSwappedTraitAndCategory()
        {
            TraitId traitId0 = new TraitId("Colour", "Blue");
            TraitId traitId1 = new TraitId("Blue", "Colour");

            int hashCode0 = traitId0.GetHashCode();
            int hashCode1 = traitId1.GetHashCode();

            Assert.AreNotEqual(hashCode0, hashCode1, "Hashcode is identical for mirrored tuple. It must be asymetrical.");
        }

        [TestMethod]
        public void CategoryNameGet()
        {
            TraitId traitId = new TraitId("Colour", "Blue");
            Assert.AreEqual("Colour", traitId.CategoryName, "Wrong category name stored");
        }

        [TestMethod]
        public void TraitNameGet()
        {
            TraitId traitId = new TraitId("Colour", "Blue");
            Assert.AreEqual("Blue", traitId.TraitName, "Wrong trait name stored");
        }
    }

#pragma warning restore CS0253 // Possible unintended reference comparison. Intentional for testing equality.
#pragma warning restore CS1718 // Comparison made to the same variable. Intential for testing equality.
}
