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

namespace Tests
{
    [TestClass]
    public class ReplacementSearchTests
    {
        [TestMethod]
        public void ConstructSuccessfully()
        {
            Trait trait = new Trait("Rhino");
            TraitCategory category = new TraitCategory("Animal");
            const Sort SORT = Sort.Given;
            ReplacementSearch search = new ReplacementSearch(trait, category, SORT);

            Assert.AreEqual(trait, search.Trait, "Wrong stored trait");
            Assert.AreEqual(category, search.Category, "Wrong stored category");
            Assert.AreEqual(SORT, search.SortCriteria, "Wrong stored sort criteria");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void NullTrait()
        {
            TraitCategory category = new TraitCategory("Animal");
            const Sort SORT = Sort.Given;
            new ReplacementSearch(trait: null, category, SORT);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void NullCategory()
        {
            Trait trait = new Trait("Rhino");
            const Sort SORT = Sort.Given;
            new ReplacementSearch(trait, category: null, SORT);
        }
    }
}
