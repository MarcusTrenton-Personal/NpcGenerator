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
using System.Text;

namespace Tests
{
    [TestClass]
    public class TraitSchemaTests
    {
        [TestMethod]
        public void EmptyTraitCategories()
        {
            TraitSchema traitSchema = new TraitSchema();
            IReadOnlyList<TraitCategory> categories = traitSchema.GetTraitCategories();

            Assert.IsNotNull(categories, "Categories are somehow null.");
            Assert.AreEqual(0, categories.Count, "Categories are not empty");
        }

        [TestMethod]
        public void SingleTraitCategory()
        {
            TraitSchema traitSchema = new TraitSchema();
            TraitCategory category = new TraitCategory("Colour", 1);
            traitSchema.Add(category);

            IReadOnlyList<TraitCategory> categories = traitSchema.GetTraitCategories();

            Assert.IsNotNull(categories, "Categories are somehow null.");
            Assert.AreEqual(1, categories.Count, "Categories count is incorrect");
            Assert.AreEqual(category, categories[0], "Wrong category stored");
        }

        [TestMethod]
        public void MultipleTraitCategories()
        {
            TraitSchema traitSchema = new TraitSchema();
            TraitCategory category0 = new TraitCategory("Colour", 1);
            traitSchema.Add(category0);
            TraitCategory category1 = new TraitCategory("Animal", 1);
            traitSchema.Add(category1);

            IReadOnlyList<TraitCategory> categories = traitSchema.GetTraitCategories();

            Assert.IsNotNull(categories, "Categories are somehow null.");
            Assert.AreEqual(2, categories.Count, "Categories count is incorrect");
            Assert.AreEqual(category0, categories[0], "Wrong category stored");
            Assert.AreEqual(category1, categories[1], "Wrong category stored");
        }

        [TestMethod]
        public void EmptyReplacements()
        {
            TraitSchema traitSchema = new TraitSchema();
            IReadOnlyList<ReplacementSearch> replacements = traitSchema.GetReplacementSearches();

            Assert.IsNotNull(replacements, "Replacements are somehow null.");
            Assert.AreEqual(0, replacements.Count, "Replacements are not empty");
        }

        [TestMethod]
        public void SingleReplacement()
        {
            TraitSchema traitSchema = new TraitSchema();
            TraitCategory category = new TraitCategory("Colour", 1);
            Trait trait = new Trait("Red", 1, isHidden: false);
            ReplacementSearch replacement = new ReplacementSearch(trait, category);
            traitSchema.Add(replacement);

            IReadOnlyList<ReplacementSearch> replacements = traitSchema.GetReplacementSearches();

            Assert.IsNotNull(replacements, "Replacements are somehow null.");
            Assert.AreEqual(1, replacements.Count, "Replacements are not empty");
            Assert.AreEqual(replacement, replacements[0], "Altered ReplacementSearch was stored");
        }

        [TestMethod]
        public void MultipleReplacements()
        {
            TraitSchema traitSchema = new TraitSchema();
            TraitCategory category = new TraitCategory("Colour", 1);
            Trait trait0 = new Trait("Green", 1, isHidden: false);
            Trait trait1 = new Trait("Red", 1, isHidden: false);

            ReplacementSearch replacement0 = new ReplacementSearch(trait0, category);
            traitSchema.Add(replacement0);
            ReplacementSearch replacement1 = new ReplacementSearch(trait1, category);
            traitSchema.Add(replacement1);

            IReadOnlyList<ReplacementSearch> replacements = traitSchema.GetReplacementSearches();

            Assert.IsNotNull(replacements, "Replacements are somehow null.");
            Assert.AreEqual(2, replacements.Count, "Replacements are not empty");
            Assert.AreEqual(replacement0, replacements[0], "Altered ReplacementSearch was stored");
            Assert.AreEqual(replacement1, replacements[1], "Altered ReplacementSearch was stored");
        }
    }
}
