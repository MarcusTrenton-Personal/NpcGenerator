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
    public class ReplacementTests
    {
        [TestMethod]
        public void ConstructedSuccessfully()
        {
            Trait originalTrait = new Trait("Blue");
            const string REPLACEMENT_TRAIT = "Green";
            TraitCategory category = new TraitCategory("Colour");

            Replacement replacement = new Replacement(originalTrait, REPLACEMENT_TRAIT, category);

            Assert.AreEqual(originalTrait, replacement.OriginalTrait, "Wrong original trait stored");
            Assert.AreEqual(REPLACEMENT_TRAIT, replacement.ReplacementTraitName, "Wrong replacement trait name stored");
            Assert.AreEqual(category, replacement.Category, "Wrong category stored");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void NullOriginalTrait()
        {
            const string REPLACEMENT_TRAIT = "Green";
            TraitCategory category = new TraitCategory("Colour");

            new Replacement(originalTrait: null, REPLACEMENT_TRAIT, category);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void NullReplacementTraitName()
        {
            Trait originalTrait = new Trait("Blue");
            TraitCategory category = new TraitCategory("Colour");

            new Replacement(originalTrait, replacementTraitName: null, category);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void EmptyReplacementTraitName()
        {
            Trait originalTrait = new Trait("Blue");
            TraitCategory category = new TraitCategory("Colour");

            new Replacement(originalTrait, replacementTraitName: String.Empty, category);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void NullCategory()
        {
            Trait originalTrait = new Trait("Blue");
            const string REPLACEMENT_TRAIT = "Green";

            new Replacement(originalTrait, REPLACEMENT_TRAIT, category: null);
        }
    }
}
