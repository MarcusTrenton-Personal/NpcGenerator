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
using NpcGenerator.Message;
using System;
using System.Collections.Generic;

namespace Tests
{
    [TestClass]
    public class InvalidNpcsTests
    {
        [TestMethod]
        public void ValidViolations()
        {
            NpcSchemaViolationCollection violations = new NpcSchemaViolationCollection
            {
                categoryViolations = new List<NpcSchemaViolation>
                {
                    new NpcSchemaViolation("Animal", NpcSchemaViolation.Reason.TooFewTraitsInCategory)
                },
                violationsByNpc = new Dictionary<Npc, List<NpcSchemaViolation>>
                {
                    [new Npc()] = new List<NpcSchemaViolation> { new NpcSchemaViolation("Animal", NpcSchemaViolation.Reason.HasLockedTrait) }
                }
            };

            InvalidNpcs message = new InvalidNpcs(violations);

            Assert.AreEqual(violations, message.Violations, "Wrong violations were store");
        }

        [TestMethod]
        public void EmptyViolations()
        {
            NpcSchemaViolationCollection violations = new NpcSchemaViolationCollection
            {
                categoryViolations = new List<NpcSchemaViolation>(),
                violationsByNpc = new Dictionary<Npc, List<NpcSchemaViolation>>()
            };

            InvalidNpcs message = new InvalidNpcs(violations);

            Assert.AreEqual(violations, message.Violations, "Wrong violations were store");
        }

        [TestMethod]
        public void EmptyNpcViolations()
        {
            NpcSchemaViolationCollection violations = new NpcSchemaViolationCollection
            {
                categoryViolations = new List<NpcSchemaViolation>(),
                violationsByNpc = new Dictionary<Npc, List<NpcSchemaViolation>>
                {
                    [new Npc()] = new List<NpcSchemaViolation> { new NpcSchemaViolation("Animal", NpcSchemaViolation.Reason.HasLockedTrait) }
                }
            };

            InvalidNpcs message = new InvalidNpcs(violations);

            Assert.AreEqual(violations, message.Violations, "Wrong violations were store");
        }

        [TestMethod]
        public void EmptyCategoryViolations()
        {
            NpcSchemaViolationCollection violations = new NpcSchemaViolationCollection
            {
                categoryViolations = new List<NpcSchemaViolation>
                {
                    new NpcSchemaViolation("Animal", NpcSchemaViolation.Reason.TooFewTraitsInCategory)
                },
                violationsByNpc = new Dictionary<Npc, List<NpcSchemaViolation>>()
            };

            InvalidNpcs message = new InvalidNpcs(violations);

            Assert.AreEqual(violations, message.Violations, "Wrong violations were store");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void NullViolations()
        {
            new InvalidNpcs(null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void NullCategoryViolation()
        {
            NpcSchemaViolationCollection violations = new NpcSchemaViolationCollection
            {
                categoryViolations = null,
                violationsByNpc = new Dictionary<Npc, List<NpcSchemaViolation>>
                {
                    [new Npc()] = new List<NpcSchemaViolation> { new NpcSchemaViolation("Animal", NpcSchemaViolation.Reason.HasLockedTrait) }
                }
            };

            new InvalidNpcs(violations);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void NullCategoryViolationElement()
        {
            NpcSchemaViolationCollection violations = new NpcSchemaViolationCollection
            {
                categoryViolations = new List<NpcSchemaViolation> { null },
                violationsByNpc = new Dictionary<Npc, List<NpcSchemaViolation>>
                {
                    [new Npc()] = new List<NpcSchemaViolation> { new NpcSchemaViolation("Animal", NpcSchemaViolation.Reason.HasLockedTrait) }
                }
            };

            new InvalidNpcs(violations);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void NullViolationsByNpc()
        {
            NpcSchemaViolationCollection violations = new NpcSchemaViolationCollection
            {
                categoryViolations = new List<NpcSchemaViolation>
                {
                    new NpcSchemaViolation("Animal", NpcSchemaViolation.Reason.TooFewTraitsInCategory)
                },
                violationsByNpc = null
            };

            new InvalidNpcs(violations);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void NullViolationsByNpcElement()
        {
            NpcSchemaViolationCollection violations = new NpcSchemaViolationCollection
            {
                categoryViolations = new List<NpcSchemaViolation>
                {
                    new NpcSchemaViolation("Animal", NpcSchemaViolation.Reason.TooFewTraitsInCategory)
                },
                violationsByNpc = new Dictionary<Npc, List<NpcSchemaViolation>>
                {
                    [new Npc()] = null
                }
            };

            new InvalidNpcs(violations);
        }
    }
}
