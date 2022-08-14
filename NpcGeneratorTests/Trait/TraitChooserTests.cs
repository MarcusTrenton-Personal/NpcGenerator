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
using Services;
using System;
using System.Collections.Generic;

namespace Tests
{
    [TestClass]
    public class TraitChooserTests
    {
        const string HEADS = "Heads";
        const string TAILS = "Tails";

        [TestMethod]
        public void TraitSelectionReturnsAllValidValues()
        {
            List<Trait> traits = new List<Trait>
            {
                new Trait(HEADS, 1, isHidden: false),
                new Trait(TAILS, 1, isHidden: false)
            };

            int headCount = 0;
            int tailCount = 0;
            const int ROLL_COUNT = 100;

            for (int i = 0; i < ROLL_COUNT; ++i)
            {
                TraitChooser chooser = new TraitChooser(traits, new CryptoRandom());
                string[] choice = chooser.Choose(1, out _);
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
            }

            Assert.IsTrue(headCount > 0, HEADS + " never came up after " + ROLL_COUNT + " flips");
            Assert.IsTrue(tailCount > 0, TAILS + " never came up after " + ROLL_COUNT + " flips");
        }

        [TestMethod]
        public void TraitSelectionIsRandom()
        {
            List<Trait> traits = new List<Trait>
            {
                new Trait(HEADS, 2, isHidden: false),
                new Trait(TAILS, 1, isHidden: false)
            };

            int headCount = 0;
            int tailCount = 0;
            const int ROLL_COUNT = 100;

            for (int i = 0; i < ROLL_COUNT; ++i)
            {
                TraitChooser chooser = new TraitChooser(traits, new CryptoRandom());
                string[] choice = chooser.Choose(1, out _);
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
            }

            Assert.IsTrue(headCount >= tailCount, "Double weighted " + HEADS + " occured less often than " + TAILS +
                "after " + ROLL_COUNT + "flips, defying astronomical odds.");
            Assert.IsTrue(tailCount > 0, "Despite a 1/3 chance of being picked, " + TAILS + ", was never picked after " + ROLL_COUNT +
                "flips, defying astronomical odds.");
        }

        [TestMethod]
        public void TraitSelectionTwice()
        {
            const int SELECTION_COUNT = 2;

            List<Trait> traits = new List<Trait>
            {
                new Trait(HEADS, 1, isHidden: false),
                new Trait(TAILS, 1, isHidden: false)
            };

            TraitChooser chooser = new TraitChooser(traits, m_random);
            string[] selections = chooser.Choose(SELECTION_COUNT, out _);
            Assert.AreEqual(SELECTION_COUNT, selections.Length, "Wrong number of selections");
            Assert.AreNotEqual(selections[0], selections[1], "Did not select two different traits");
        }

        [TestMethod]
        public void NoWeightSingleSelection()
        {
            List<Trait> traits = new List<Trait>
            {
                new Trait(HEADS, 0, isHidden: false)
            };
            TraitChooser chooser = new TraitChooser(traits, m_random);

            bool threwException = false;
            try
            {
                string[] selections = chooser.Choose(1, out _);
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
            List<Trait> traits = new List<Trait>
            {
                new Trait(HEADS, 0, isHidden: false),
                new Trait(TAILS, 1, isHidden: false)
            };
            TraitChooser chooser = new TraitChooser(traits, m_random);

            bool threwException = false;
            try
            {
                string[] selections = chooser.Choose(2, out _);
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
            List<Trait> traits = new List<Trait>
            {
                new Trait(HEADS, 1, isHidden: false)
            };
            TraitChooser chooser = new TraitChooser(traits, m_random);

            bool threwException = false;
            try
            {
                string[] selections = chooser.Choose(2, out _);
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
            List<Trait> traits = new List<Trait>
            {
                new Trait(HEADS, 1, isHidden: true)
            };
            TraitChooser chooser = new TraitChooser(traits, m_random);

            string[] selections = chooser.Choose(1, out _);

            Assert.AreEqual(0, selections.Length, "Hidden selected trait is incorrectly present in output");
        }

        [TestMethod]
        public void HiddenTraitOnMultiSelection()
        {
            List<Trait> traits = new List<Trait>
            {
                new Trait(HEADS, 1, isHidden: true),
                new Trait(TAILS, 1, isHidden: false)
            };
            TraitChooser chooser = new TraitChooser(traits, m_random);

            string[] selections = chooser.Choose(2, out _);

            Assert.AreEqual(1, selections.Length, "Selected traits is not the correct number.");
            Assert.AreEqual(TAILS, selections[0], "Hidden selected trait is incorrectly present in output");
        }

        [TestMethod]
        public void EmptyBonusSelection()
        {
            List<Trait> traits = new List<Trait>
            {
                new Trait(HEADS, 1, isHidden: false)
            };
            TraitChooser chooser = new TraitChooser(traits, m_random);

            string[] selections = chooser.Choose(1, out List<BonusSelection> bonusSelection);

            Assert.AreEqual(0, bonusSelection.Count, "Returned a bonus selection where there should be none.");
        }

        [TestMethod]
        public void SingleBonusSelection()
        {
            List<Trait> traits = new List<Trait>();
            Trait trait = new Trait(HEADS, 1, isHidden: false);
            const string BONUS_CATEGORY = "Animal";
            TraitCategory category = new TraitCategory(BONUS_CATEGORY, 1);
            trait.BonusSelection = new BonusSelection(category, 1);
            traits.Add(trait);

            TraitChooser chooser = new TraitChooser(traits, m_random);

            string[] selections = chooser.Choose(1, out List<BonusSelection> bonusSelections);

            Assert.AreEqual(1, selections.Length, "Selected traits is not the correct number.");
            Assert.AreEqual(1, bonusSelections.Count, "Wrong number of bonusSelections");
            Assert.AreEqual(category, bonusSelections[0].TraitCategory, "Incorrect bonus category selected");
            Assert.AreEqual(1, bonusSelections[0].SelectionCount, "Incorrect bonus selection count");
        }

        [TestMethod]
        public void MultipleBonusSelections()
        {
            List<Trait> traits = new List<Trait>();
            Trait trait = new Trait(HEADS, 1, isHidden: false);
            const string BONUS_CATEGORY = "Animal";
            TraitCategory category = new TraitCategory(BONUS_CATEGORY, 1);
            trait.BonusSelection = new BonusSelection(category, 2);
            traits.Add(trait);

            TraitChooser chooser = new TraitChooser(traits, m_random);

            string[] selections = chooser.Choose(1, out List<BonusSelection> bonusSelections);

            Assert.AreEqual(1, selections.Length, "Selected traits is not the correct number.");
            Assert.AreEqual(1, bonusSelections.Count, "Wrong number of bonusSelections");
            Assert.AreEqual(category, bonusSelections[0].TraitCategory, "Incorrect bonus category selected");
            Assert.AreEqual(2, bonusSelections[0].SelectionCount, "Incorrect bonus selection count");
        }

        MockRandom m_random = new MockRandom();
    }
}
