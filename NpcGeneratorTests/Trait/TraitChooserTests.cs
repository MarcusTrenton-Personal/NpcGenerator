﻿/*Copyright(C) 2022 Marcus Trenton, marcus.trenton@gmail.com

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
        const string CATEGORY = "Coin";

        [TestMethod]
        public void TraitSelectionReturnsAllValidValues()
        {
            List<Trait> traits = new List<Trait>
            {
                new Trait(HEADS, 1),
                new Trait(TAILS, 1)
            };

            int headCount = 0;
            int tailCount = 0;
            const int ROLL_COUNT = 100;

            for (int i = 0; i < ROLL_COUNT; ++i)
            {
                TraitChooser chooser = new TraitChooser(traits, CATEGORY, new CryptoRandom(), new Npc());
                Npc.Trait[] choice = chooser.Choose(1, out _);
                switch (choice[0].Name)
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
                new Trait(HEADS, 2),
                new Trait(TAILS, 1)
            };

            int headCount = 0;
            int tailCount = 0;
            const int ROLL_COUNT = 1000;

            for (int i = 0; i < ROLL_COUNT; ++i)
            {
                TraitChooser chooser = new TraitChooser(traits, CATEGORY, new CryptoRandom(), new Npc());
                Npc.Trait[] choice = chooser.Choose(1, out _);
                switch (choice[0].Name)
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
                new Trait(HEADS, 1),
                new Trait(TAILS, 1)
            };

            TraitChooser chooser = new TraitChooser(traits, CATEGORY, m_random, new Npc());
            Npc.Trait[] selections = chooser.Choose(SELECTION_COUNT, out _);
            Assert.AreEqual(SELECTION_COUNT, selections.Length, "Wrong number of selections");
            Assert.AreNotEqual(selections[0].Name, selections[1].Name, "Did not select two different traits");
        }

        [TestMethod, ExpectedException(typeof(NoRemainingWeightException))]
        public void NoWeightSingleSelection()
        {
            List<Trait> traits = new List<Trait>
            {
                new Trait(HEADS, 0)
            };
            TraitChooser chooser = new TraitChooser(traits, CATEGORY, m_random, new Npc());

            chooser.Choose(1, out _);
        }

        [TestMethod, ExpectedException(typeof(NoRemainingWeightException))]
        public void NoWeightMultipleSelection()
        {
            List<Trait> traits = new List<Trait>
            {
                new Trait(HEADS, 0),
                new Trait(TAILS, 1)
            };
            TraitChooser chooser = new TraitChooser(traits, CATEGORY, m_random, new Npc());

            chooser.Choose(2, out _);
        }

        [TestMethod, ExpectedException(typeof(TooFewTraitsException))]
        public void MoreSelectionsThanOptions()
        {
            List<Trait> traits = new List<Trait>
            {
                new Trait(HEADS, 1)
            };
            TraitChooser chooser = new TraitChooser(traits, CATEGORY, m_random, new Npc());

            chooser.Choose(2, out _);
        }

        [TestMethod]
        public void HiddenTraitOnSingleSelection()
        {
            List<Trait> traits = new List<Trait>
            {
                new Trait(HEADS, 1, isHidden: true)
            };
            TraitChooser chooser = new TraitChooser(traits, CATEGORY, m_random, new Npc());

            Npc.Trait[] selections = chooser.Choose(1, out _);

            Assert.AreEqual(1, selections.Length, "Selected trait count is wrong.");
            Assert.IsTrue(selections[0].IsHidden, "Trait was incorrectly returned as not hidden");
        }

        [TestMethod]
        public void HiddenTraitOnMultiSelection()
        {
            List<Trait> traits = new List<Trait>
            {
                new Trait(HEADS, 1, isHidden: true),
                new Trait(TAILS, 1, isHidden: false)
            };
            TraitChooser chooser = new TraitChooser(traits, CATEGORY, m_random, new Npc());

            Npc.Trait[] selections = chooser.Choose(2, out _);

            Assert.AreEqual(2, selections.Length, "Selected trait count is wrong.");
            Npc.Trait heads = Array.Find(selections, trait => trait.Name == HEADS);
            Assert.IsTrue(heads.IsHidden, "Trait was incorrectly returned as not hidden");
            Npc.Trait tails = Array.Find(selections, trait => trait.Name == TAILS);
            Assert.IsFalse(tails.IsHidden, "Trait was incorrectly returned as hidden");
        }

        [TestMethod]
        public void EmptyBonusSelection()
        {
            List<Trait> traits = new List<Trait>
            {
                new Trait(HEADS, 1)
            };
            TraitChooser chooser = new TraitChooser(traits, CATEGORY, m_random, new Npc());

            Npc.Trait[] selections = chooser.Choose(1, out IReadOnlyList<BonusSelection> bonusSelection);

            Assert.AreEqual(0, bonusSelection.Count, "Returned a bonus selection where there should be none.");
            Assert.AreEqual(1, selections.Length, "Wrong number of selections");
        }

        [TestMethod]
        public void SingleBonusSelection()
        {
            List<Trait> traits = new List<Trait>();
            Trait trait = new Trait(HEADS);
            const string BONUS_CATEGORY = "Animal";
            TraitCategory category = new TraitCategory(BONUS_CATEGORY, 1);
            trait.BonusSelection = new BonusSelection(category.Name, 1);
            traits.Add(trait);

            TraitChooser chooser = new TraitChooser(traits, CATEGORY, m_random, new Npc());

            Npc.Trait[] selections = chooser.Choose(1, out IReadOnlyList<BonusSelection> bonusSelections);

            Assert.AreEqual(1, selections.Length, "Selected traits is not the correct number.");
            Assert.AreEqual(1, bonusSelections.Count, "Wrong number of bonusSelections");
            Assert.AreEqual(category.Name, bonusSelections[0].CategoryName, "Incorrect bonus category selected");
            Assert.AreEqual(1, bonusSelections[0].SelectionCount, "Incorrect bonus selection count");
        }

        [TestMethod]
        public void MultipleBonusSelections()
        {
            List<Trait> traits = new List<Trait>();
            Trait trait = new Trait(HEADS);
            const string BONUS_CATEGORY = "Animal";
            TraitCategory category = new TraitCategory(BONUS_CATEGORY, 1);
            trait.BonusSelection = new BonusSelection(category.Name, 2);
            traits.Add(trait);

            TraitChooser chooser = new TraitChooser(traits, CATEGORY, m_random, new Npc());

            Npc.Trait[] selections = chooser.Choose(1, out IReadOnlyList<BonusSelection> bonusSelections);

            Assert.AreEqual(1, selections.Length, "Selected traits is not the correct number.");
            Assert.AreEqual(1, bonusSelections.Count, "Wrong number of bonusSelections");
            Assert.AreEqual(category.Name, bonusSelections[0].CategoryName, "Incorrect bonus category selected");
            Assert.AreEqual(2, bonusSelections[0].SelectionCount, "Incorrect bonus selection count");
        }

        [TestMethod]
        public void UnlockedTrait()
        {
            const string REQUIRED_CATEGORY = "Colour";
            const string REQUIRED_TRAIT = "Blue";

            const string GUARDED_CATEGORY = "Animal";
            const string GUARDED_TRAIT = "Bear";

            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait npcHasTrait = new NpcHasTrait(new TraitId(REQUIRED_CATEGORY, REQUIRED_TRAIT), npcHolder);
            Requirement req = new Requirement(npcHasTrait, npcHolder);
            Trait trait = new Trait(GUARDED_TRAIT);
            trait.Set(req);

            List<Trait> traits = new List<Trait>() { trait };

            Npc npc = new Npc();
            npc.Add(REQUIRED_CATEGORY, new Npc.Trait[] { new Npc.Trait(REQUIRED_TRAIT, REQUIRED_CATEGORY) });

            TraitChooser chooser = new TraitChooser(traits, GUARDED_CATEGORY, m_random, npc);

            Npc.Trait[] selections = chooser.Choose(1, out IReadOnlyList<BonusSelection> bonusSelections);

            Assert.AreEqual(1, selections.Length, "Selected traits is not the correct number.");
            Assert.AreEqual(GUARDED_TRAIT, selections[0].Name, "Selected trait name is wrong");
            Assert.AreEqual(0, bonusSelections.Count, "Wrong number of bonusSelections");
        }

        [TestMethod]
        public void LockedTrait()
        {
            const string REQUIRED_CATEGORY = "Colour";
            const string REQUIRED_TRAIT = "Blue";

            const string GUARDED_CATEGORY = "Animal";
            const string GUARDED_TRAIT = "Bear";
            const string UNGUARDED_TRAIT = "Rhino";

            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait npcHasTrait = new NpcHasTrait(new TraitId(REQUIRED_CATEGORY, REQUIRED_TRAIT), npcHolder);
            Requirement req = new Requirement(npcHasTrait, npcHolder);
            Trait guardTrait = new Trait(GUARDED_TRAIT);
            guardTrait.Set(req);

            List<Trait> traits = new List<Trait>() { guardTrait, new Trait(UNGUARDED_TRAIT) };

            TraitChooser chooser = new TraitChooser(traits, GUARDED_CATEGORY, m_random, new Npc());

            Npc.Trait[] selections = chooser.Choose(1, out IReadOnlyList<BonusSelection> bonusSelections);

            Assert.AreEqual(1, selections.Length, "Selected traits is not the correct number.");
            Assert.AreEqual(UNGUARDED_TRAIT, selections[0].Name, "Selected trait name is wrong");
            Assert.AreEqual(0, bonusSelections.Count, "Wrong number of bonusSelections");
        }

        [TestMethod]
        public void TooFewTraitsDueToRequirements()
        {
            const string REQUIRED_CATEGORY = "Colour";
            const string REQUIRED_TRAIT = "Blue";

            const string GUARDED_CATEGORY = "Animal";
            const string GUARDED_TRAIT = "Bear";

            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait npcHasTrait = new NpcHasTrait(new TraitId(REQUIRED_CATEGORY, REQUIRED_TRAIT), npcHolder);
            Requirement req = new Requirement(npcHasTrait, npcHolder);
            Trait trait = new Trait(GUARDED_TRAIT);
            trait.Set(req);

            List<Trait> traits = new List<Trait>() { trait };

            TraitChooser chooser = new TraitChooser(traits, GUARDED_CATEGORY, m_random, new Npc());

            bool threwException = false;
            try
            {
                chooser.Choose(1, out IReadOnlyList<BonusSelection> bonusSelections);
            }
            catch (TooFewTraitsPassRequirementsException exception)
            {
                threwException = true;

                Assert.AreEqual(1, exception.Requested, "Wrong number of Requested traits");
                Assert.AreEqual(0, exception.Available, "Wrong number of Available traits");
            }

            Assert.IsTrue(threwException, "Failed to throw TooFewTraitsPassRequirementsException");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void NullTraitList()
        {
            new TraitChooser(null, "Animal", m_random, new Npc());
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void NullElementInTraitList()
        {
            new TraitChooser(new List<Trait>() { null }, "Animal", m_random, new Npc());
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void NullRandom()
        {
            new TraitChooser(new List<Trait>(), "Animal", null, new Npc());
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void EmptyCategory()
        {
            new TraitChooser(new List<Trait>(), string.Empty, m_random, new Npc());
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void NullCategory()
        {
            new TraitChooser(new List<Trait>(), null, m_random, new Npc());
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void NullNpc()
        {
            new TraitChooser(new List<Trait>(), "Animal", m_random, null);
        }

        [TestMethod]
        public void ZeroCount()
        {
            List<Trait> traits = new List<Trait>
            {
                new Trait(HEADS, 1)
            };
            TraitChooser chooser = new TraitChooser(traits, CATEGORY, m_random, new Npc());

            Npc.Trait[] selections = chooser.Choose(0, out IReadOnlyList<BonusSelection> bonusSelections);

            Assert.AreEqual(0, selections.Length, "Wrong number of selections");
            Assert.AreEqual(0, bonusSelections.Count, "Wrong number of bonusSelections");
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void NegativeCount()
        {
            List<Trait> traits = new List<Trait>
            {
                new Trait(HEADS, 1)
            };
            TraitChooser chooser = new TraitChooser(traits, CATEGORY, m_random, new Npc());

            chooser.Choose(-1, out _);
        }

        private readonly MockRandom m_random = new MockRandom();
    }
}
