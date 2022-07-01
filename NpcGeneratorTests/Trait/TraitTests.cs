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
        const string HEADS = "Heads";
        const string TAILS = "Tails";

        [TestMethod]
        public void TraitSelectionReturnsAllValidValues()
        {
            TraitCategory category = new TraitCategory("Coin");
            category.Add(new Trait(HEADS, 1));
            category.Add(new Trait(TAILS, 1));

            int headCount = 0;
            int tailCount = 0;
            const int ROLL_COUNT = 100;

            for (int i = 0; i < ROLL_COUNT; ++i)
            {
                string choice = category.Choose();
                switch (choice)
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
            TraitCategory category = new TraitCategory("Coin");
            category.Add(new Trait(HEADS, 2));
            category.Add(new Trait(TAILS, 1));

            int headCount = 0;
            int tailCount = 0;
            const int ROLL_COUNT = 100;

            for (int i = 0; i < ROLL_COUNT; ++i)
            {
                string choice = category.Choose();
                switch (choice)
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
        }
    }
}
