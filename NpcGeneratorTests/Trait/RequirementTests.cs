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
        public void ConstructWithNullLogicalExpression()
        {
            
            bool threwException = false;
            try
            {
                Requirement req = new Requirement(logicalExpression: null, new NpcHolder());
            }
            catch(ArgumentNullException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Did not throw ArgumentNullException for null LogicalExpression in constructor.");
        }

        [TestMethod]
        public void ConstructWithNullNpcHolder()
        {

            bool threwException = false;
            try
            {
                Requirement req = new Requirement(new AlwaysTrue(), npcHolder: null);
            }
            catch (ArgumentNullException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Did not throw ArgumentNullException for null NpcHolder in constructor.");
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

        [TestMethod]
        public void IsUnlockedForNullNpc()
        {
            Requirement req = new Requirement(new AlwaysTrue(), new NpcHolder());

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
        public void GetNpcForSingleAlwaysTrue()
        {
            const string CATEGORY = "colour";
            const string TRAIT = "blue";

            Npc npc = new Npc();
            npc.Add(CATEGORY, new string[] { TRAIT });

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
            npc.Add(CATEGORY, new string[] { "blue" });

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
            npc.Add(CATEGORY, new string[] { TRAIT });

            NpcHolder npcHolder = new NpcHolder();
            LogicalAny any = new LogicalAny();
            any.Add(new NpcHasTrait(new TraitId(CATEGORY, TRAIT), npcHolder));
            any.Add(new NpcHasTrait(new TraitId(CATEGORY, "red"), npcHolder));
            Requirement req = new Requirement(any, npcHolder);

            bool isUnlocked = req.IsUnlockedFor(npc);

            Assert.IsTrue(isUnlocked, "Did not evaluate logical expresssion correctly.");
        }
    }
}
