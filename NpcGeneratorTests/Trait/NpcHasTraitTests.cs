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
    public class NpcHasTraitTests
    {
        public const string CategoryFound = "Animal";
        public const string CategoryNotFound = "Colour";
        public const string TraitFound = "Bear";
        public const string TraitNotFound = "Velociraptor";

        private readonly INpcProvider m_npcProvider;
        private readonly Npc m_npcWithTrait;
        private readonly TraitId m_foundTraitId = new TraitId(CategoryFound, TraitFound);

        private class StubNpcProvider : INpcProvider
        {
            public StubNpcProvider(Npc npc)
            {
                Npc = npc;
            }

            public Npc Npc { get; set; }
        }

        public NpcHasTraitTests()
        {
            m_npcWithTrait = new Npc();
            m_npcWithTrait.Add(CategoryFound, new Npc.Trait[] { new Npc.Trait(TraitFound, CategoryFound) });

            m_npcProvider = new StubNpcProvider(m_npcWithTrait);
        }

        [TestMethod]
        public void NullTraitId()
        {
            bool threwException = false;
            try
            {
                NpcHasTrait expression = new NpcHasTrait(traitId: null, npcProvider: m_npcProvider);
            }
            catch(ArgumentNullException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Creating a NpcHasTrait with a null TraitId should throw an ArgumentNullException");
        }

        [TestMethod]
        public void NullNpcProvider()
        {
            bool threwException = false;
            try
            {
                NpcHasTrait expression = new NpcHasTrait(traitId: m_foundTraitId, npcProvider: null);
            }
            catch (ArgumentNullException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Creating a NpcHasTrait with a null npcProvider should throw an ArgumentNullException");
        }

        [TestMethod]
        public void NullNpcProvidedDuringEvaluate()
        {
            StubNpcProvider npcProvider = new StubNpcProvider(npc: null);
            NpcHasTrait expression = new NpcHasTrait(traitId: m_foundTraitId, npcProvider: npcProvider);

            bool threwException = false;
            try
            {
                bool found = expression.Evaluate();
            }
            catch (InvalidOperationException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "NpcHasTrait with a null Npc during Evaluate did not throw an InvalidOperationException");
        }

        [TestMethod]
        public void NpcHasTrait()
        {
            NpcHasTrait expression = new NpcHasTrait(traitId: m_foundTraitId, npcProvider: m_npcProvider);

            bool found = expression.Evaluate();

            Assert.IsTrue(found, "Did not find the trait even though the Npc has it");
        }

        [TestMethod]
        public void NpcDoesNotHaveTrait()
        {
            TraitId traitNotFound = new TraitId(CategoryFound, TraitNotFound);
            NpcHasTrait expression = new NpcHasTrait(traitId: traitNotFound, npcProvider: m_npcProvider);

            bool found = expression.Evaluate();

            Assert.IsFalse(found, "Did find the trait even though the Npc did not have it");
        }

        [TestMethod]
        public void NpcDoesNotHaveCategory()
        {
            TraitId categoryNotFound = new TraitId(CategoryNotFound, TraitFound);
            NpcHasTrait expression = new NpcHasTrait(traitId: categoryNotFound, npcProvider: m_npcProvider);

            bool found = expression.Evaluate();

            Assert.IsFalse(found, "Did find the trait even though the Npc did not have it");
        }

        [TestMethod]
        public void NpcHasTraitAfterChangingNpcs()
        {
            Npc emptyNpc = new Npc();
            StubNpcProvider provider = new StubNpcProvider(emptyNpc);
            NpcHasTrait expression = new NpcHasTrait(traitId: m_foundTraitId, npcProvider: provider);

            bool initialIsFound = expression.Evaluate();
            Assert.IsFalse(initialIsFound, "Incorrectly found a trait in an empty npc");

            provider.Npc = m_npcWithTrait;
            bool followUpIsFound = expression.Evaluate();
            Assert.IsTrue(followUpIsFound, "Did not find the trait even though the Npc has it");
        }

        [TestMethod]
        public void NpcDoesNotHaveTraitAfterChangingNpcs()
        {
            StubNpcProvider provider = new StubNpcProvider(m_npcWithTrait);
            NpcHasTrait expression = new NpcHasTrait(traitId: m_foundTraitId, npcProvider: provider);

            bool initialIsFound = expression.Evaluate();
            Assert.IsTrue(initialIsFound, "Did not find the trait even though the Npc has it");
            
            Npc emptyNpc = new Npc();
            provider.Npc = emptyNpc;
            bool followUpIsFound = expression.Evaluate();
            Assert.IsFalse(followUpIsFound, "Incorrectly found a trait in an empty npc");
        }

        [TestMethod]
        public void GetTraitId()
        {
            StubNpcProvider provider = new StubNpcProvider(m_npcWithTrait);
            NpcHasTrait expression = new NpcHasTrait(traitId: m_foundTraitId, npcProvider: provider);

            Assert.AreEqual(m_foundTraitId, expression.TraitId, "Wrong TraitId returned");
        }
    }
}
