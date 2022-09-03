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

namespace Tests
{
    [TestClass]
    public class RequirementTests
    {
        [TestMethod]
        public void InitializeNullLogicalExpression()
        {
            Requirement req = new Requirement();
            bool threwException = false;
            try
            {
                req.Initialize(null);
            }
            catch(ArgumentNullException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Did not throw ArgumentNullException with a null initialization.");
        }

        [TestMethod]
        public void IsUnlockedWithoutIntialize()
        {
            Npc npc = new Npc();
            Requirement req = new Requirement();

            bool threwException = false;
            try
            {
                bool isUnlocked = req.IsUnlockedFor(npc);
            }
            catch (InvalidOperationException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Did not throw ArgumentNullException with a null initialization.");
        }

        [TestMethod]
        public void IsUnlockedForAlwaysTrueExpression()
        {
            Npc npc = new Npc();
            Requirement req = new Requirement();
            req.Initialize(new AlwaysTrue());

            bool isUnlocked = req.IsUnlockedFor(npc);

            Assert.IsTrue(isUnlocked, "Did not evaluate logical expresssion correctly.");
        }

        [TestMethod]
        public void IsUnlockedForAlwaysFalseExpression()
        {
            Npc npc = new Npc();
            Requirement req = new Requirement();
            req.Initialize(new AlwaysFalse());

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
            Requirement req = new Requirement();
            req.Initialize(any);

            bool isUnlocked = req.IsUnlockedFor(npc);

            Assert.IsTrue(isUnlocked, "Did not evaluate logical expresssion correctly.");
        }

        [TestMethod]
        public void MultipleInitialize()
        {
            Npc npc = new Npc();
            Requirement req = new Requirement();
            req.Initialize(new AlwaysTrue());
            req.Initialize(new AlwaysFalse());

            bool isUnlocked = req.IsUnlockedFor(npc);

            Assert.IsFalse(isUnlocked, "Did not evaluate logical expresssion correctly.");
        }

        [TestMethod]
        public void IsUnlockedForNullNpc()
        {
            Requirement req = new Requirement();
            req.Initialize(new AlwaysTrue());

            bool threwException = false;
            try
            {
                bool isUnlocked = req.IsUnlockedFor(null);
            }
            catch(ArgumentNullException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Did not throw ArgumentNullException for null npc is IsUnlockedFor.");
        }

        [TestMethod]
        public void GetNpcForSingleTrueExpression()
        {
            const string CATEGORY = "colour";
            const string TRAIT = "blue";

            Npc npc = new Npc();
            npc.Add(CATEGORY, new string[] { TRAIT });

            Requirement req = new Requirement();
            req.Initialize(new NpcHasTrait(new TraitId(CATEGORY, TRAIT), req));

            bool isUnlocked = req.IsUnlockedFor(npc);

            Assert.IsTrue(isUnlocked, "Did not evaluate logical expresssion correctly.");
        }

        [TestMethod]
        public void GetNpcForSingleFalseExpression()
        {
            const string CATEGORY = "colour";

            Npc npc = new Npc();
            npc.Add(CATEGORY, new string[] { "blue" });

            Requirement req = new Requirement();
            req.Initialize(new NpcHasTrait(new TraitId(CATEGORY, "red"), req));
            bool isUnlocked = req.IsUnlockedFor(npc);

            Assert.IsFalse(isUnlocked, "Did not evaluate logical expresssion correctly.");
        }

        [TestMethod]
        public void GetNpcForMultipleExpressions()
        {
            const string CATEGORY = "colour";
            const string TRAIT = "blue";

            Npc npc = new Npc();
            npc.Add(CATEGORY, new string[] { TRAIT });

            Requirement req = new Requirement();
            LogicalAny any = new LogicalAny();
            any.Add(new NpcHasTrait(new TraitId(CATEGORY, TRAIT), req));
            any.Add(new NpcHasTrait(new TraitId(CATEGORY, "red"), req));
            req.Initialize(any);

            bool isUnlocked = req.IsUnlockedFor(npc);

            Assert.IsTrue(isUnlocked, "Did not evaluate logical expresssion correctly.");
        }

        private class AlwaysTrue : ILogicalExpression
        {
            public bool Evaluate()
            {
                return true;
            }
        }

        private class AlwaysFalse : ILogicalExpression
        {
            public bool Evaluate()
            {
                return false;
            }
        }
    }
}
