using Microsoft.VisualStudio.TestTools.UnitTesting;
using NpcGenerator;
using System;

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

            for(int i = 0; i < ROLL_COUNT; ++i)
            {
                string choice = category.Choose();
                switch(choice)
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

            Assert.IsTrue(headCount >= tailCount, "Double weighted " + HEADS + " occured less often than "+ TAILS + 
                "after " + ROLL_COUNT + "flips, defying astronomical odds.");
        }
    }
}
