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
using Services;
using System;
using System.Collections.Generic;

namespace Tests
{
    [TestClass]
    public class RequirementTests
    {
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void ConstructWithNullLogicalExpression()
        {
            new Requirement(logicalExpression: null, new NpcHolder());
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void ConstructWithNullNpcHolder()
        {
            new Requirement(new AlwaysTrue(), npcHolder: null);
        }

        [TestMethod]
        public void IsUnlockedForAlwaysAlwaysTrue()
        {
            Npc npc = new Npc();
            Requirement req = new Requirement(new AlwaysTrue(), new NpcHolder());

            bool isUnlocked = req.IsUnlockedFor(npc);

            Assert.IsTrue(isUnlocked, "Did not evaluate logical expresssion correctly.");
        }

        [TestMethod]
        public void IsUnlockedForAlwaysAlwaysFalse()
        {
            Npc npc = new Npc();
            Requirement req = new Requirement(new AlwaysFalse(), new NpcHolder());

            bool isUnlocked = req.IsUnlockedFor(npc);

            Assert.IsFalse(isUnlocked, "Did not evaluate logical expresssion correctly.");
        }

        [TestMethod]
        public void IsUnlockedForComplexExpression()
        {
            Npc npc = new Npc();
            LogicalAny any = new LogicalAny();
            any.Add(new AlwaysTrue());
            any.Add(new AlwaysFalse());
            Requirement req = new Requirement(any, new NpcHolder());

            bool isUnlocked = req.IsUnlockedFor(npc);

            Assert.IsTrue(isUnlocked, "Did not evaluate logical expresssion correctly.");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void IsUnlockedForNullNpc()
        {
            Requirement req = new Requirement(new AlwaysTrue(), new NpcHolder());

            req.IsUnlockedFor(null);
        }

        [TestMethod]
        public void GetNpcForSingleAlwaysTrue()
        {
            const string CATEGORY = "colour";
            const string TRAIT = "blue";

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT, CATEGORY) });

            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait npcHasTrait = new NpcHasTrait(new TraitId(CATEGORY, TRAIT), npcHolder);
            Requirement req = new Requirement(npcHasTrait, npcHolder);

            bool isUnlocked = req.IsUnlockedFor(npc);

            Assert.IsTrue(isUnlocked, "Did not evaluate logical expresssion correctly.");
        }

        [TestMethod]
        public void GetNpcForSingleAlwaysFalse()
        {
            const string CATEGORY = "colour";

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait("blue", CATEGORY) });

            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait npcHasTrait = new NpcHasTrait(new TraitId(CATEGORY, "red"), npcHolder);
            Requirement req = new Requirement(npcHasTrait, npcHolder);

            bool isUnlocked = req.IsUnlockedFor(npc);

            Assert.IsFalse(isUnlocked, "Did not evaluate logical expresssion correctly.");
        }

        [TestMethod]
        public void GetNpcForMultipleExpressions()
        {
            const string CATEGORY = "colour";
            const string TRAIT = "blue";

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT, CATEGORY) });

            NpcHolder npcHolder = new NpcHolder();
            LogicalAny any = new LogicalAny();
            any.Add(new NpcHasTrait(new TraitId(CATEGORY, TRAIT), npcHolder));
            any.Add(new NpcHasTrait(new TraitId(CATEGORY, "red"), npcHolder));
            Requirement req = new Requirement(any, npcHolder);

            bool isUnlocked = req.IsUnlockedFor(npc);

            Assert.IsTrue(isUnlocked, "Did not evaluate logical expresssion correctly.");
        }

        [TestMethod]
        public void SingleDependentCategory()
        {
            const string CATEGORY = "Animal";

            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait npcHasTrait = new NpcHasTrait(new TraitId(CATEGORY, "Bear"), npcHolder);
            Requirement req = new Requirement(npcHasTrait, npcHolder);

            HashSet<string> dependencies = req.DependentCategoryNames();

            Assert.AreEqual(1, dependencies.Count, "Wrong number of dependencies");
            foreach(string dep in dependencies)
            {
                Assert.AreEqual(CATEGORY, dep, "Wrong dependency");
            }
        }

        [TestMethod]
        public void MultipleIndpendentCategories()
        {
            const string CATEGORY0 = "Animal";
            const string CATEGORY1 = "Colour";

            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait npcHasTrait0 = new NpcHasTrait(new TraitId(CATEGORY0, "Bear"), npcHolder);
            NpcHasTrait npcHasTrait1 = new NpcHasTrait(new TraitId(CATEGORY1, "Blue"), npcHolder);
            
            LogicalNone logicalNone = new LogicalNone();
            logicalNone.Add(npcHasTrait0);
            logicalNone.Add(npcHasTrait1);

            Requirement req = new Requirement(logicalNone, npcHolder);

            HashSet<string> dependencies = req.DependentCategoryNames();

            Assert.AreEqual(2, dependencies.Count, "Wrong number of dependencies");

            SortedList<string, string> alphabeticalCategoryDependencies = new SortedList<string, string>();
            foreach (string dep in dependencies)
            {
                alphabeticalCategoryDependencies.Add(dep, dep);
            }

            Assert.AreEqual(CATEGORY0, alphabeticalCategoryDependencies.Values[0], "Wrong dependency");
            Assert.AreEqual(CATEGORY1, alphabeticalCategoryDependencies.Values[1], "Wrong dependency");
        }

        [TestMethod]
        public void MultipleOverlappingCategories()
        {
            const string CATEGORY0 = "Animal";
            const string CATEGORY1 = "Colour";

            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait npcHasTrait0 = new NpcHasTrait(new TraitId(CATEGORY0, "Bear"), npcHolder);
            NpcHasTrait npcHasTrait1 = new NpcHasTrait(new TraitId(CATEGORY0, "Rhino"), npcHolder);
            NpcHasTrait npcHasTrait2 = new NpcHasTrait(new TraitId(CATEGORY1, "Blue"), npcHolder);

            LogicalNone logicalNone = new LogicalNone();
            logicalNone.Add(npcHasTrait0);
            logicalNone.Add(npcHasTrait1);
            logicalNone.Add(npcHasTrait2);

            Requirement req = new Requirement(logicalNone, npcHolder);

            HashSet<string> dependencies = req.DependentCategoryNames();

            Assert.AreEqual(2, dependencies.Count, "Wrong number of dependencies");

            SortedList<string, string> alphabeticalCategoryDependencies = new SortedList<string, string>();
            foreach (string dep in dependencies)
            {
                alphabeticalCategoryDependencies.Add(dep, dep);
            }

            Assert.AreEqual(CATEGORY0, alphabeticalCategoryDependencies.Values[0], "Wrong dependency");
            Assert.AreEqual(CATEGORY1, alphabeticalCategoryDependencies.Values[1], "Wrong dependency");
        }
    }
}
