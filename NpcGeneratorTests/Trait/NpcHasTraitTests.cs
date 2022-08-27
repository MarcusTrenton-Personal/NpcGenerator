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
using System;
using System.Collections.Generic;

namespace NpcGeneratorTests.Trait
{
    [TestClass]
    public class NpcHasTraitTests
    {
        public const string CategoryFound = "Animal";
        public const string CategoryNotFound = "Colour";
        public const string TraitFound = "Bear";
        public const string TraitNotFound = "Velociraptor";

        private readonly INpcProvider m_npcProvider;
        public TraitId m_foundTraitId = new TraitId(CategoryFound, TraitFound);

        private class NpcProviderStub : INpcProvider
        {
            public NpcProviderStub(Npc npc)
            {
                m_npc = npc;
            }

            public Npc GetNpc()
            {
                return m_npc;
            }

            private readonly Npc m_npc;
        }

        public NpcHasTraitTests()
        {
            Npc npc = new Npc();
            npc.Add(CategoryFound, new string[] { TraitFound });

            m_npcProvider = new NpcProviderStub(npc);
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
            NpcProviderStub npcProvider = new NpcProviderStub(npc: null);
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
    }
}
