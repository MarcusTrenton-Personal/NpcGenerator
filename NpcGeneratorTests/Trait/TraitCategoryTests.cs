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
using System;
using System.Collections.Generic;

namespace Tests
{
    [TestClass]
    public class TraitCategoryTests
    {
        const string HEADS = "Heads";
        const string TAILS = "Tails";

        [TestMethod]
        public void TraitSelectionReturnsAllValidValues()
        {
            TraitCategory category = new TraitCategory("Coin", selectionCount: 1);
            category.Add(new Trait(HEADS, 1, isHidden: false));
            category.Add(new Trait(TAILS, 1, isHidden: false));

            int headCount = 0;
            int tailCount = 0;
            const int ROLL_COUNT = 100;

            for (int i = 0; i < ROLL_COUNT; ++i)
            {
                string[] choice = category.Choose(category.DefaultSelectionCount, out _);
                switch (choice[0])
                {
                    case HEADS:
                        headCount++;
                        break;

                    case TAILS:
                        tailCount++;
                        break;

                    default:
                        Assert.Fail("Trait returned value " + choice + " that was not added");
                        break;
                }
                category.ResetChoices();
            }

            Assert.IsTrue(headCount > 0, HEADS + " never came up after " + ROLL_COUNT + " flips");
            Assert.IsTrue(tailCount > 0, TAILS + " never came up after " + ROLL_COUNT + " flips");
        }

        [TestMethod]
        public void TraitSelectionIsRandom()
        {
            TraitCategory category = new TraitCategory("Coin", selectionCount: 1);
            category.Add(new Trait(HEADS, 2, isHidden: false));
            category.Add(new Trait(TAILS, 1, isHidden: false));

            int headCount = 0;
            int tailCount = 0;
            const int ROLL_COUNT = 100;

            for (int i = 0; i < ROLL_COUNT; ++i)
            {
                string[] choice = category.Choose(category.DefaultSelectionCount, out _);
                switch (choice[0])
                {
                    case HEADS:
                        headCount++;
                        break;

                    case TAILS:
                        tailCount++;
                        break;

                    default:
                        Assert.Fail("Trait returned value " + choice + " that was not added");
                        break;
                }
                category.ResetChoices();
            }

            Assert.IsTrue(headCount >= tailCount, "Double weighted " + HEADS + " occured less often than " + TAILS +
                "after " + ROLL_COUNT + "flips, defying astronomical odds.");
        }

        [TestMethod]
        public void TraitSelectionTwice()
        {
            const int SELECTION_COUNT = 2;

            TraitCategory category = new TraitCategory("Coin", SELECTION_COUNT);
            category.Add(new Trait(HEADS, 1, isHidden: false));
            category.Add(new Trait(TAILS, 1, isHidden: false));

            string[] selections = category.Choose(category.DefaultSelectionCount, out _);
            Assert.AreEqual(SELECTION_COUNT, selections.Length, "Wrong number of selections");
            Assert.AreNotEqual(selections[0], selections[1], "Did not select two different traits");
        }

        [TestMethod]
        public void NoWeightSingleSelection()
        {
            TraitCategory category = new TraitCategory("Coin", selectionCount: 1);
            category.Add(new Trait(HEADS, 0, isHidden: false)); 

            bool threwException = false;
            try 
            {
                string[] selections = category.Choose(category.DefaultSelectionCount, out _);
            }
            catch (Exception)
            {
                threwException = true;
            }
            
            Assert.IsTrue(threwException, "Impossible selection of 0 weight options did not throw exception");
        }

        [TestMethod]
        public void NoWeightMultipleSelection()
        {
            TraitCategory category = new TraitCategory("Coin", selectionCount: 2);
            category.Add(new Trait(HEADS, 0, isHidden: false));
            category.Add(new Trait(TAILS, 1, isHidden: false));

            bool threwException = false;
            try
            {
                string[] selections = category.Choose(category.DefaultSelectionCount, out _);
            }
            catch (Exception)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Impossible selection of 0 weight options did not throw exception");
        }

        [TestMethod]
        public void MoreSelectionsThanOptions()
        {
            TraitCategory category = new TraitCategory("Coin", selectionCount: 2);
            category.Add(new Trait(HEADS, 0, isHidden: false));

            bool threwException = false;
            try
            {
                string[] selections = category.Choose(category.DefaultSelectionCount, out _);
            }
            catch (Exception)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Impossible selection of 0 weight options did not throw exception");
        }

        [TestMethod]
        public void HiddenTraitOnSingleSelection()
        {
            TraitCategory category = new TraitCategory("Coin", selectionCount: 1);
            category.Add(new Trait(HEADS, 1, isHidden: true));

            string[] selections = category.Choose(category.DefaultSelectionCount, out _);

            Assert.AreEqual(0, selections.Length, "Hidden selected trait is incorrectly present in output");
        }

        [TestMethod]
        public void HiddenTraitOnMultiSelection()
        {
            TraitCategory category = new TraitCategory("Coin", selectionCount: 2);
            category.Add(new Trait(HEADS, 1, isHidden: true));
            category.Add(new Trait(TAILS, 1, isHidden: false));

            string[] selections = category.Choose(category.DefaultSelectionCount, out _);

            Assert.AreEqual(1, selections.Length, "Selected traits is not the correct number.");
            Assert.AreEqual(TAILS, selections[0], "Hidden selected trait is incorrectly present in output");
        }

        [TestMethod]
        public void EmptyBonusSelection()
        {
            TraitCategory category = new TraitCategory("Coin", selectionCount: 1);
            category.Add(new Trait(HEADS, 1, isHidden: false));

            string[] selections = category.Choose(category.DefaultSelectionCount, out List<BonusSelection> bonusSelection);

            Assert.AreEqual(0, bonusSelection.Count, "Returned a bonus selection where there should be none.");
        }

        [TestMethod]
        public void ChooseMoreThanDefault()
        {
            TraitCategory category = new TraitCategory("Coin", selectionCount: 1);
            category.Add(new Trait(HEADS, 1, isHidden: false));
            category.Add(new Trait(TAILS, 1, isHidden: false));

            string[] selections = category.Choose(2, out _);

            Assert.AreEqual(2, selections.Length, "Selected traits is not the correct number.");
            Assert.IsTrue(selections[0] != selections[1], "Same trait was selected twice");
        }

        [TestMethod]
        public void ChooseLessThanDefault()
        {
            TraitCategory category = new TraitCategory("Coin", selectionCount: 2);
            category.Add(new Trait(HEADS, 1, isHidden: false));
            category.Add(new Trait(TAILS, 1, isHidden: false));

            string[] selections = category.Choose(1, out _);

            Assert.AreEqual(1, selections.Length, "Selected traits is not the correct number.");
            Assert.IsTrue(selections[0] == HEADS || selections[0] == TAILS, "Unknown trait selected");
        }
    }
}
