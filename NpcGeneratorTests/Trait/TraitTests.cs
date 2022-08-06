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
    public class TraitTests
    {
        [TestMethod]
        public void ValueConstructor()
        {
            const string NAME = "Blue";
            const int WEIGHT = 1;
            const bool IS_HIDDEN = false;
            Trait trait = new Trait(NAME, WEIGHT, IS_HIDDEN);

            Assert.AreEqual(NAME, trait.Name, "Wrong name was stored");
            Assert.AreEqual(WEIGHT, trait.Weight, "Wrong weight was stored");
            Assert.AreEqual(IS_HIDDEN, trait.IsHidden, "Wrong IsHidden was stored");
            Assert.IsNull(trait.BonusSelection, "BonusSelection is not null despite not be specified");
        }

        [TestMethod]
        public void DeepCopyWithRename()
        {
            Trait original = new Trait("Blue", 1, isHidden: false);
            TraitCategory category = new TraitCategory("Animal", 1);
            original.BonusSelection = new BonusSelection(category, 1);

            const string COPY_NAME = "Green";
            Trait copy = original.DeepCopyWithRename(COPY_NAME);

            Assert.IsFalse(ReferenceEquals(original, copy), "Original and copy are the same object, which is not a copy");
            Assert.AreEqual(COPY_NAME, copy.Name, "Name was not renamed.");
            Assert.AreEqual(original.Weight, copy.Weight, "Weight was not copied correctly");
            Assert.AreEqual(original.IsHidden, copy.IsHidden, "IsHidden was not copied correctly");
            Assert.IsFalse(ReferenceEquals(original.BonusSelection, copy.BonusSelection), 
                "Original and copy BonusSelection are the same object, which is not a deep copy");
            Assert.AreEqual(original.BonusSelection.SelectionCount, copy.BonusSelection.SelectionCount,
                "BonusSelection SelectionCount was not copied");
            Assert.AreEqual(original.BonusSelection.TraitCategory, copy.BonusSelection.TraitCategory, 
                "BonusSelection TraitCategory was not copied");
        }
    }
}
