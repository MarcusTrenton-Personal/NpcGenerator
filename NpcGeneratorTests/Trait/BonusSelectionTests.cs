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
    [TestClass]
    public class BonusSelectionTests
    {
        [TestMethod]
        public void ValueConstructor()
        {
            TraitCategory category = new TraitCategory("Colour");
            const int SELECTION_COUNT = 1;
            BonusSelection bonusSelection = new BonusSelection(category.Name, SELECTION_COUNT);

            Assert.AreEqual(SELECTION_COUNT, bonusSelection.SelectionCount, "Wrong SelectionCount was stored");
            Assert.AreEqual(category.Name, bonusSelection.CategoryName, "Wrong TraitCategory was stored");
        }

        [TestMethod]
        public void ShallowCopy()
        {
            TraitCategory category = new TraitCategory("Colour");
            BonusSelection original = new BonusSelection(category.Name, 1);

            BonusSelection copy = original.ShallowCopy();

            Assert.IsFalse(ReferenceEquals(original, copy), "Original and copy are the same object, which is not a copy");
            Assert.AreEqual(original.SelectionCount, copy.SelectionCount, "SelectionCount was not copied correctly.");
            Assert.AreEqual(original.CategoryName, copy.CategoryName, "TraitCategory was not copied correctly");
        }
    }
}
