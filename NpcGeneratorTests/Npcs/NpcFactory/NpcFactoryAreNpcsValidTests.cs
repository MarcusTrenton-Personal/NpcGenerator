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

namespace Tests.NpcFactoryTests.AreNpcsValid
{
    [TestClass]
    public class NpcFactoryAreNpcsValidTests
    {
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void AreNpcsValidNullNpcGroup()
        {
            TraitSchema schema = new TraitSchema();

            NpcFactory.AreNpcsValid(npcGroup: null, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> _);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void AreNpcsValidNullNpc()
        {
            NpcGroup npcGroup = new NpcGroup(new List<string>() { "Animal" });
            npcGroup.Add(null);
            TraitSchema schema = new TraitSchema();

            NpcFactory.AreNpcsValid(npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> _);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void AreNpcsValidNullSchema()
        {
            NpcGroup npcGroup = new NpcGroup(new List<string>() { "Animal" });
            npcGroup.Add(new Npc());

            NpcFactory.AreNpcsValid(npcGroup, schema: null, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> _);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void AreNpcsValidNullReplacements()
        {
            TraitSchema schema = new TraitSchema();
            NpcGroup npcGroup = new NpcGroup(new List<string>() { "Animal" });
            npcGroup.Add(new Npc());

            NpcFactory.AreNpcsValid(npcGroup, schema, replacements: null, out Dictionary<Npc, List<NpcSchemaViolation>> _);
        }

        [TestMethod]
        public void AreNpcsValidEmpty()
        {
            TraitSchema schema = new TraitSchema();
            NpcGroup npcGroup = new NpcGroup(new List<string>() { "Animal" });
            Npc npc = new Npc();
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsTrue(areValid, "Npc is incorrectly invalid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(0, violations.Count, "Wrong number of violations");
        }

        [TestMethod]
        public void AreNpcsValidEmptyViolationTooFewTraits()
        {
            const string CATEGORY = "Animal";
            const string TRAIT = "Bear";
            const int TRAIT_COUNT = 2;

            Trait trait = new Trait(TRAIT);
            Trait trait2 = new Trait("Velociraptor");
            TraitCategory category = new TraitCategory(CATEGORY, TRAIT_COUNT);
            category.Add(trait);
            category.Add(trait2);
            TraitSchema schema = new TraitSchema();
            schema.Add(category);

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT, CATEGORY) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY });
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsFalse(areValid, "Npc is incorrectly valid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(1, violations.Count, "Wrong number of violations");
            Assert.AreEqual(CATEGORY, violations[0].Category, "Wrong violation category");
            Assert.IsNull(violations[0].Trait, "Wrong violation trait");
            Assert.AreEqual(NpcSchemaViolation.Reason.TooFewTraitsInCategory, violations[0].Violation, "Wrong violation reason");
        }

        [TestMethod]
        public void AreNpcsValidWithSingleCategoryAndSingleTrait()
        {
            const string CATEGORY = "Animal";
            const string TRAIT = "Bear";

            Trait trait = new Trait(TRAIT);
            Trait trait2 = new Trait("Velociraptor");
            TraitCategory category = new TraitCategory(CATEGORY);
            category.Add(trait);
            category.Add(trait2);
            TraitSchema schema = new TraitSchema();
            schema.Add(category);

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT, CATEGORY) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY });
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsTrue(areValid, "Npc is incorrectly invalid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(0, violations.Count, "Wrong number of violations");
        }

        [TestMethod]
        public void AreNpcsValidWithSingleCategoryAndMultipleTraits()
        {
            const string CATEGORY = "Animal";
            const string TRAIT0 = "Bear";
            const string TRAIT1 = "Velociraptor";

            Trait trait0 = new Trait(TRAIT0);
            Trait trait1 = new Trait(TRAIT1);
            Trait trait2 = new Trait("Baby Shark");
            TraitCategory category = new TraitCategory(CATEGORY, 2);
            category.Add(trait0);
            category.Add(trait1);
            category.Add(trait2);
            TraitSchema schema = new TraitSchema();
            schema.Add(category);

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT0, CATEGORY), new Npc.Trait(TRAIT1, CATEGORY) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY });
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsTrue(areValid, "Npc is incorrectly invalid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(0, violations.Count, "Wrong number of violations");
        }

        [TestMethod]
        public void AreNpcsValidWithMulipleCategoriesAndMultipleTraits()
        {
            const string CATEGORY0 = "Animal";
            const string C0T0 = "Bear";
            const string C0T1 = "Velociraptor";

            const string CATEGORY1 = "Colour";
            const string C1T0 = "Blue";

            Trait c0t0 = new Trait(C0T0);
            Trait c0t1 = new Trait(C0T1);
            Trait c0t2 = new Trait("Baby Shark");
            TraitCategory category0 = new TraitCategory(CATEGORY0, 2);
            category0.Add(c0t0);
            category0.Add(c0t1);
            category0.Add(c0t2);

            Trait c1t0 = new Trait(C1T0);
            Trait c1t1 = new Trait("Red");
            TraitCategory category1 = new TraitCategory(CATEGORY1);
            category1.Add(c1t0);
            category1.Add(c1t1);

            TraitSchema schema = new TraitSchema();
            schema.Add(category0);
            schema.Add(category1);

            Npc npc = new Npc();
            npc.Add(CATEGORY0, new Npc.Trait[] { new Npc.Trait(C0T0, CATEGORY0), new Npc.Trait(C0T1, CATEGORY0) });
            npc.Add(CATEGORY1, new Npc.Trait[] { new Npc.Trait(C1T0, CATEGORY1) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY0, CATEGORY1 });
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsTrue(areValid, "Npc is incorrectly invalid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(0, violations.Count, "Wrong number of violations");
        }

        [TestMethod]
        public void AreNpcsValidWith0SelectionCategory()
        {
            const string CATEGORY0 = "Animal";
            const string C0T0 = "Bear";
            const string C0T1 = "Velociraptor";

            Trait c0t0 = new Trait(C0T0);
            Trait c0t1 = new Trait(C0T1);
            Trait c0t2 = new Trait("Baby Shark");
            TraitCategory category0 = new TraitCategory(CATEGORY0, 2);
            category0.Add(c0t0);
            category0.Add(c0t1);
            category0.Add(c0t2);

            Trait c1t0 = new Trait("Blue");
            Trait c1t1 = new Trait("Red");
            const string CATEGORY1 = "Colour";
            TraitCategory category1 = new TraitCategory(CATEGORY1, 0);
            category1.Add(c1t0);
            category1.Add(c1t1);

            TraitSchema schema = new TraitSchema();
            schema.Add(category0);
            schema.Add(category1);

            Npc npc = new Npc();
            npc.Add(CATEGORY0, new Npc.Trait[] { new Npc.Trait(C0T0, CATEGORY0), new Npc.Trait(C0T1, CATEGORY0) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY0 });
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsTrue(areValid, "Npc is incorrectly invalid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(0, violations.Count, "Wrong number of violations");
        }

        [TestMethod]
        public void AreNpcsValidViolationTooManyAndTooFewTraits()
        {
            const string CATEGORY0 = "Animal";
            const string C0T0 = "Bear";
            const string C0T1 = "Velociraptor";

            Trait c0t0 = new Trait(C0T0);
            Trait c0t1 = new Trait(C0T1);
            Trait c0t2 = new Trait("Baby Shark");
            TraitCategory category0 = new TraitCategory(CATEGORY0, 3);
            category0.Add(c0t0);
            category0.Add(c0t1);
            category0.Add(c0t2);

            const string CATEGORY1 = "Colour";
            const string C1T0 = "Blue";

            Trait c1t0 = new Trait(C1T0);
            Trait c1t1 = new Trait("Red");
            TraitCategory category1 = new TraitCategory(CATEGORY1, 0);
            category1.Add(c1t0);
            category1.Add(c1t1);

            TraitSchema schema = new TraitSchema();
            schema.Add(category0);
            schema.Add(category1);

            Npc npc = new Npc();
            npc.Add(CATEGORY0, new Npc.Trait[] { new Npc.Trait(C0T0, CATEGORY0), new Npc.Trait(C0T1, CATEGORY0) });
            npc.Add(CATEGORY1, new Npc.Trait[] { new Npc.Trait(C1T0, CATEGORY1) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY0, CATEGORY1 });
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsFalse(areValid, "Npc is incorrectly invalid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(2, violations.Count, "Wrong number of violations");

            NpcSchemaViolation tooFewTraitsViolation = violations.Find(
                violation => violation.Category == CATEGORY0 &&
                violation.Trait is null &&
                violation.Violation == NpcSchemaViolation.Reason.TooFewTraitsInCategory);
            Assert.IsNotNull(tooFewTraitsViolation, "TooFewTraitsInCategory violation not detected");

            NpcSchemaViolation tooManyTraitsViolation = violations.Find(
                violation => violation.Category == CATEGORY1 &&
                violation.Trait is null &&
                violation.Violation == NpcSchemaViolation.Reason.TooManyTraitsInCategory);
            Assert.IsNotNull(tooManyTraitsViolation, "TooManyTraitsInCategory violation not detected");
        }

        [TestMethod]
        public void AreNpcsValidMultipleViolationsMultipleNpcs()
        {
            const string CATEGORY0 = "Animal";
            const string C0T0 = "Bear";
            const string C0T1 = "Velociraptor";
            const string C0T2 = "Baby Shark";

            Trait c0t0 = new Trait(C0T0);
            Trait c0t1 = new Trait(C0T1);
            Trait c0t2 = new Trait(C0T2);
            TraitCategory category0 = new TraitCategory(CATEGORY0, 3);
            category0.Add(c0t0);
            category0.Add(c0t1);
            category0.Add(c0t2);

            const string CATEGORY1 = "Colour";
            const string C1T0 = "Blue";

            Trait c1t0 = new Trait(C1T0);
            Trait c1t1 = new Trait("Red");
            TraitCategory category1 = new TraitCategory(CATEGORY1, 0);
            category1.Add(c1t0);
            category1.Add(c1t1);

            TraitSchema schema = new TraitSchema();
            schema.Add(category0);
            schema.Add(category1);

            Npc npc0 = new Npc();
            npc0.Add(CATEGORY0, new Npc.Trait[] { new Npc.Trait(C0T0, CATEGORY0), new Npc.Trait(C0T1, CATEGORY0) });
            npc0.Add(CATEGORY1, new Npc.Trait[] { new Npc.Trait(C1T0, CATEGORY1) });

            const string TRAIT_NOT_FOUND = "Kraken";
            Npc npc1 = new Npc();
            Npc.Trait[] traits = new Npc.Trait[] {
                new Npc.Trait(C0T0, CATEGORY0), new Npc.Trait(C0T1, CATEGORY0), new Npc.Trait(TRAIT_NOT_FOUND, CATEGORY0) };
            npc1.Add(CATEGORY0, traits);

            Npc npc2 = new Npc();
            npc2.Add(CATEGORY0, new Npc.Trait[] { new Npc.Trait(C0T0, CATEGORY0), new Npc.Trait(C0T1, CATEGORY0), new Npc.Trait(C0T2, CATEGORY0) });

            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY0, CATEGORY1 });
            npcGroup.Add(npc0);
            npcGroup.Add(npc1);
            npcGroup.Add(npc2);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsFalse(areValid, "Npcs are incorrectly valid");

            List<NpcSchemaViolation> violations0 = violationsPerNpc[npc0];
            Assert.AreEqual(2, violations0.Count, "Wrong number of violations");

            NpcSchemaViolation tooFewTraitsViolation = violations0.Find(
                violation => violation.Category == CATEGORY0 &&
                violation.Trait is null &&
                violation.Violation == NpcSchemaViolation.Reason.TooFewTraitsInCategory);
            Assert.IsNotNull(tooFewTraitsViolation, "TooFewTraitsInCategory violation not detected");

            NpcSchemaViolation tooManyTraitsViolation = violations0.Find(
                violation => violation.Category == CATEGORY1 &&
                violation.Trait is null &&
                violation.Violation == NpcSchemaViolation.Reason.TooManyTraitsInCategory);
            Assert.IsNotNull(tooManyTraitsViolation, "TooManyTraitsInCategory violation not detected");

            List<NpcSchemaViolation> violations1 = violationsPerNpc[npc1];
            Assert.AreEqual(1, violations1.Count, "Wrong number of violations");

            NpcSchemaViolation traiNotFoundViolation = violations1.Find(
                violation => violation.Category == CATEGORY0 &&
                violation.Trait == TRAIT_NOT_FOUND &&
                violation.Violation == NpcSchemaViolation.Reason.TraitNotFoundInSchema);
            Assert.IsNotNull(traiNotFoundViolation, "TraitNotFoundInSchema violation not detected");

            List<NpcSchemaViolation> violations2 = violationsPerNpc[npc2];
            Assert.AreEqual(0, violations2.Count, "Wrong number of violations");
        }

        [TestMethod]
        public void AreNpcsValidViolationCategoryNotFound()
        {
            const string CATEGORY0 = "Animal";
            const string C0T0 = "Bear";
            const string C0T1 = "Velociraptor";

            const string CATEGORY_NOT_FOUND = "Hair Dye";
            const string TRAIT_NOT_FOUND = "Blonde";

            Trait c0t0 = new Trait(C0T0);
            Trait c0t1 = new Trait(C0T1);
            Trait c0t2 = new Trait("Baby Shark");
            TraitCategory category0 = new TraitCategory(CATEGORY0, 3);
            category0.Add(c0t0);
            category0.Add(c0t1);
            category0.Add(c0t2);

            TraitSchema schema = new TraitSchema();
            schema.Add(category0);

            Npc npc = new Npc();
            npc.Add(CATEGORY_NOT_FOUND, new Npc.Trait[] { new Npc.Trait(TRAIT_NOT_FOUND, CATEGORY_NOT_FOUND) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY0, CATEGORY_NOT_FOUND });
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsFalse(areValid, "Npc is incorrectly valid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(1, violations.Count, "Wrong number of violations");

            NpcSchemaViolation categoryNotFoundViolation = violations.Find(
                violation => violation.Category == CATEGORY_NOT_FOUND &&
                violation.Trait is null &&
                violation.Violation == NpcSchemaViolation.Reason.CategoryNotFoundInSchema);
            Assert.IsNotNull(categoryNotFoundViolation, "CategoryNotFoundInSchema violation not detected");
        }

        [TestMethod]
        public void AreNpcsValidViolationTraitNotFound()
        {
            const string CATEGORY = "Hair Dye";
            const string TRAIT_NOT_FOUND = "Blonde";

            Trait c0t0 = new Trait("Purple");
            TraitCategory category = new TraitCategory(CATEGORY, 1);
            category.Add(c0t0);

            TraitSchema schema = new TraitSchema();
            schema.Add(category);

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT_NOT_FOUND, CATEGORY) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY });
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsFalse(areValid, "Npc is incorrectly valid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(1, violations.Count, "Wrong number of violations");

            NpcSchemaViolation categoryNotFoundViolation = violations.Find(
                violation => violation.Category == CATEGORY &&
                violation.Trait == TRAIT_NOT_FOUND &&
                violation.Violation == NpcSchemaViolation.Reason.TraitNotFoundInSchema);
            Assert.IsNotNull(categoryNotFoundViolation, "TraitNotFoundInSchema violation not detected");
        }

        [TestMethod]
        public void AreNpcsValidWithHiddenTraits()
        {
            const string CATEGORY = "Hair Dye";
            const string TRAIT = "Blonde";

            Trait c0t0 = new Trait(TRAIT, 1, isHidden: true);
            TraitCategory category = new TraitCategory(CATEGORY);
            category.Add(c0t0);

            TraitSchema schema = new TraitSchema();
            schema.Add(category);

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT, CATEGORY, isHidden: true) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY });
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsTrue(areValid, "Npc is incorrectly invalid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(0, violations.Count, "Wrong number of violations");
        }

        [TestMethod]
        public void AreNpcsValidViolationIsHiddenOnlyInNpc()
        {
            const string CATEGORY = "Hair Dye";
            const string TRAIT = "Blonde";

            Trait c0t0 = new Trait(TRAIT, 1, isHidden: false);
            TraitCategory category = new TraitCategory(CATEGORY);
            category.Add(c0t0);

            TraitSchema schema = new TraitSchema();
            schema.Add(category);

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT, CATEGORY, isHidden: true) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY });
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsFalse(areValid, "Npc is incorrectly valid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(1, violations.Count, "Wrong number of violations");

            NpcSchemaViolation categoryNotFoundViolation = violations.Find(
                violation => violation.Category == CATEGORY &&
                violation.Trait == TRAIT &&
                violation.Violation == NpcSchemaViolation.Reason.TraitIsIncorrectlyHidden);
            Assert.IsNotNull(categoryNotFoundViolation, "TraitIsHiddenMismatch violation not detected");
        }

        [TestMethod]
        public void AreNpcsValidViolationIsHiddenOnlyInSchema()
        {
            const string CATEGORY = "Hair Dye";
            const string TRAIT = "Blonde";

            Trait c0t0 = new Trait(TRAIT, 1, isHidden: true);
            TraitCategory category = new TraitCategory(CATEGORY);
            category.Add(c0t0);

            TraitSchema schema = new TraitSchema();
            schema.Add(category);

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT, CATEGORY, isHidden: false) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY });
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsFalse(areValid, "Npc is incorrectly valid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(1, violations.Count, "Wrong number of violations");

            NpcSchemaViolation categoryNotFoundViolation = violations.Find(
                violation => violation.Category == CATEGORY &&
                violation.Trait == TRAIT &&
                violation.Violation == NpcSchemaViolation.Reason.TraitIsIncorrectlyNotHidden);
            Assert.IsNotNull(categoryNotFoundViolation, "TraitIsHiddenMismatch violation not detected");
        }

        [TestMethod]
        public void AreNpcsValidWithIntraCategoryBonusSelection()
        {
            const string CATEGORY = "Race";
            const string TRAIT0 = "Caucasian";
            const string TRAIT1 = "African";
            const string TRAIT2_WITH_BONUS_SELECTION = "Biracial";

            Trait trait0 = new Trait(TRAIT0);
            Trait trait1 = new Trait(TRAIT1);
            Trait trait2 = new Trait(TRAIT2_WITH_BONUS_SELECTION)
            {
                BonusSelection = new BonusSelection(CATEGORY, 2)
            };
            Trait trait3 = new Trait("Hispanic");
            Trait trait4 = new Trait("Asian");
            TraitCategory category = new TraitCategory(CATEGORY, 1);
            category.Add(trait0);
            category.Add(trait1);
            category.Add(trait2);
            category.Add(trait3);
            category.Add(trait4);
            TraitSchema schema = new TraitSchema();
            schema.Add(category);

            Npc npc = new Npc();
            Npc.Trait[] traits = new Npc.Trait[] {
                new Npc.Trait(TRAIT0, CATEGORY), new Npc.Trait(TRAIT1, CATEGORY), new Npc.Trait(TRAIT2_WITH_BONUS_SELECTION, CATEGORY) };
            npc.Add(CATEGORY, traits);
            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY });
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsTrue(areValid, "Npc is incorrectly invalid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(0, violations.Count, "Wrong number of violations");
        }

        [TestMethod]
        public void AreNpcsValidWithInterCategoryBonusSelection()
        {
            const string CATEGORY0 = "Animal";
            const string C0T0 = "Bear";
            const string C0T1 = "Velociraptor";
            const string C0T2 = "Baby Shark";

            const string CATEGORY1 = "Colour";
            const string C1T0 = "Blue";
            const string C1T1 = "Red";

            Trait c0t0 = new Trait(C0T0)
            {
                BonusSelection = new BonusSelection(CATEGORY1, 1)
            };
            Trait c0t1 = new Trait(C0T1);
            Trait c0t2 = new Trait(C0T2);
            TraitCategory category0 = new TraitCategory(CATEGORY0, 2);
            category0.Add(c0t0);
            category0.Add(c0t1);
            category0.Add(c0t2);

            Trait c1t0 = new Trait(C1T0)
            {
                BonusSelection = new BonusSelection(CATEGORY0, 1)
            };
            Trait c1t1 = new Trait(C1T1);
            TraitCategory category1 = new TraitCategory(CATEGORY1);
            category1.Add(c1t0);
            category1.Add(c1t1);

            TraitSchema schema = new TraitSchema();
            schema.Add(category0);
            schema.Add(category1);

            Npc npc = new Npc();
            npc.Add(CATEGORY0, new Npc.Trait[] { new Npc.Trait(C0T0, CATEGORY0), new Npc.Trait(C0T1, CATEGORY0), new Npc.Trait(C0T2, CATEGORY0) });
            npc.Add(CATEGORY1, new Npc.Trait[] { new Npc.Trait(C1T0, CATEGORY1), new Npc.Trait(C1T1, CATEGORY1) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY0, CATEGORY1 });
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsTrue(areValid, "Npc is incorrectly invalid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(0, violations.Count, "Wrong number of violations");
        }

        [TestMethod]
        public void AreNpcsValidViolationTooFewTraitsWithBonusSelection()
        {
            const string CATEGORY = "Race";
            const string TRAIT0 = "Caucasian";
            const string TRAIT1 = "African";
            const string TRAIT2_WITH_BONUS_SELECTION = "Biracial";

            Trait trait0 = new Trait(TRAIT0);
            Trait trait1 = new Trait(TRAIT1);
            Trait trait2 = new Trait(TRAIT2_WITH_BONUS_SELECTION)
            {
                BonusSelection = new BonusSelection(CATEGORY, 2)
            };
            Trait trait3 = new Trait("Hispanic");
            Trait trait4 = new Trait("Asian");
            TraitCategory category = new TraitCategory(CATEGORY, 1);
            category.Add(trait0);
            category.Add(trait1);
            category.Add(trait2);
            category.Add(trait3);
            category.Add(trait4);
            TraitSchema schema = new TraitSchema();
            schema.Add(category);

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT0, CATEGORY), new Npc.Trait(TRAIT2_WITH_BONUS_SELECTION, CATEGORY) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY });
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsFalse(areValid, "Npc is incorrectly valid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(1, violations.Count, "Wrong number of violations");

            NpcSchemaViolation categoryNotFoundViolation = violations.Find(
                violation => violation.Category == CATEGORY &&
                violation.Trait is null &&
                violation.Violation == NpcSchemaViolation.Reason.TooFewTraitsInCategory);
            Assert.IsNotNull(categoryNotFoundViolation, "TooFewTraitsInCategory violation not detected");
        }

        [TestMethod]
        public void AreNpcsValidWithUnusedReplacement()
        {
            const string CATEGORY = "Animal";
            const string TRAIT = "Bear";
            const string TRAIT_WITH_REPLACEMENT = "Velociraptor";

            Trait trait = new Trait(TRAIT);
            Trait traitWithReplacement = new Trait(TRAIT_WITH_REPLACEMENT);
            TraitCategory category = new TraitCategory(CATEGORY);
            category.Add(trait);
            category.Add(traitWithReplacement);
            TraitSchema schema = new TraitSchema();
            schema.Add(category);

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT, CATEGORY) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY });
            npcGroup.Add(npc);

            Replacement replacement = new Replacement(traitWithReplacement, "Tyrannosaurus Rex", category);
            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>() { replacement }, out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsTrue(areValid, "Npc is incorrectly invalid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(0, violations.Count, "Wrong number of violations");
        }

        [TestMethod]
        public void AreNpcsValidWithReplacement()
        {
            const string CATEGORY = "Animal";
            const string TRAIT = "Bear";
            const string TRAIT_ORIGINAL_NAME = "Velociraptor";
            const string REPLACEMENT_NAME = "Tyrannosaurus Rex";

            Trait trait = new Trait(TRAIT);
            Trait traitWithReplacement = new Trait(TRAIT_ORIGINAL_NAME);
            TraitCategory category = new TraitCategory(CATEGORY);
            category.Add(trait);
            category.Add(traitWithReplacement);
            TraitSchema schema = new TraitSchema();
            schema.Add(category);

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(REPLACEMENT_NAME, CATEGORY, isHidden: false, originalName: TRAIT_ORIGINAL_NAME) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY });
            npcGroup.Add(npc);

            Replacement replacement = new Replacement(traitWithReplacement, REPLACEMENT_NAME, category);
            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>() { replacement }, out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsTrue(areValid, "Npc is incorrectly invalid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(0, violations.Count, "Wrong number of violations");
        }

        [TestMethod]
        public void AreNpcsValidWithIdenticalReplacement()
        {
            const string CATEGORY = "Animal";
            const string TRAIT = "Bear";
            const string TRAIT_NAME = "Velociraptor";

            Trait trait = new Trait(TRAIT);
            Trait traitWithReplacement = new Trait(TRAIT_NAME);
            TraitCategory category = new TraitCategory(CATEGORY);
            category.Add(trait);
            category.Add(traitWithReplacement);
            TraitSchema schema = new TraitSchema();
            schema.Add(category);

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT_NAME, CATEGORY, isHidden: false, originalName: TRAIT_NAME) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY });
            npcGroup.Add(npc);

            Replacement replacement = new Replacement(traitWithReplacement, TRAIT_NAME, category);
            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>() { replacement }, out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsTrue(areValid, "Npc is incorrectly invalid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(0, violations.Count, "Wrong number of violations");
        }

        [TestMethod]
        public void AreNpcsValidViolationUnusedReplacement()
        {
            const string CATEGORY = "Animal";
            const string TRAIT = "Bear";
            const string TRAIT_ORIGINAL_NAME = "Velociraptor";
            const string REPLACEMENT_NAME = "Tyrannosaurus Rex";

            Trait trait = new Trait(TRAIT);
            Trait traitWithReplacement = new Trait(TRAIT_ORIGINAL_NAME);
            TraitCategory category = new TraitCategory(CATEGORY);
            category.Add(trait);
            category.Add(traitWithReplacement);
            TraitSchema schema = new TraitSchema();
            schema.Add(category);

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT_ORIGINAL_NAME, CATEGORY) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY });
            npcGroup.Add(npc);

            Replacement replacement = new Replacement(traitWithReplacement, REPLACEMENT_NAME, category);
            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>() { replacement }, out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsFalse(areValid, "Npc is incorrectly valid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(1, violations.Count, "Wrong number of violations");

            NpcSchemaViolation categoryNotFoundViolation = violations.Find(violation =>
                violation.Category == CATEGORY &&
                violation.Trait == TRAIT_ORIGINAL_NAME &&
                violation.Violation == NpcSchemaViolation.Reason.UnusedReplacement);
            Assert.IsNotNull(categoryNotFoundViolation, "UnusedReplacement violation not detected");
        }

        [TestMethod]
        public void AreNpcsValidWithLockedCategory()
        {
            const string REQUIRED_CATEGORY = "Animal";
            const string REQUIRED_TRAIT = "Bear";
            const string ALTERNATIVE_TO_REQUIRED_TRAIT = "Rhino";

            Trait c0t0 = new Trait(REQUIRED_TRAIT);
            Trait c0t1 = new Trait(ALTERNATIVE_TO_REQUIRED_TRAIT);
            TraitCategory category = new TraitCategory(REQUIRED_CATEGORY);
            category.Add(c0t0);
            category.Add(c0t1);

            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait hasTrait = new NpcHasTrait(new TraitId(categoryName: REQUIRED_CATEGORY, traitName: REQUIRED_TRAIT), npcHolder);
            Requirement requirement = new Requirement(hasTrait, npcHolder);

            Trait c1t0 = new Trait("Blue");
            const string LOCKED_CATEGORY = "Colour";
            TraitCategory lockedCategory = new TraitCategory(LOCKED_CATEGORY);
            lockedCategory.Add(c1t0);
            lockedCategory.Set(requirement);

            TraitSchema schema = new TraitSchema();
            schema.Add(category);
            schema.Add(lockedCategory);

            Npc npc = new Npc();
            npc.Add(REQUIRED_CATEGORY, new Npc.Trait[] { new Npc.Trait(ALTERNATIVE_TO_REQUIRED_TRAIT, REQUIRED_CATEGORY) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { REQUIRED_CATEGORY, LOCKED_CATEGORY });
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsTrue(areValid, "Npc is incorrectly invalid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(0, violations.Count, "Wrong number of violations");
        }

        [TestMethod]
        public void AreNpcsValidViolationTraitFromLockedCategory()
        {
            const string REQUIRED_CATEGORY = "Animal";
            const string REQUIRED_TRAIT = "Bear";
            const string ALTERNATIVE_TO_REQUIRED_TRAIT = "Rhino";

            const string LOCKED_CATEGORY = "Colour";
            const string LOCKED_TRAIT = "Blue";

            Trait c0t0 = new Trait(REQUIRED_TRAIT);
            Trait c0t1 = new Trait(ALTERNATIVE_TO_REQUIRED_TRAIT);
            TraitCategory category = new TraitCategory(REQUIRED_CATEGORY);
            category.Add(c0t0);
            category.Add(c0t1);

            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait hasTrait = new NpcHasTrait(new TraitId(categoryName: REQUIRED_CATEGORY, traitName: REQUIRED_TRAIT), npcHolder);
            Requirement requirement = new Requirement(hasTrait, npcHolder);

            Trait c1t0 = new Trait(LOCKED_TRAIT);
            TraitCategory lockedCategory = new TraitCategory(LOCKED_CATEGORY);
            lockedCategory.Add(c1t0);
            lockedCategory.Set(requirement);

            TraitSchema schema = new TraitSchema();
            schema.Add(category);
            schema.Add(lockedCategory);

            Npc npc = new Npc();
            npc.Add(REQUIRED_CATEGORY, new Npc.Trait[] { new Npc.Trait(ALTERNATIVE_TO_REQUIRED_TRAIT, REQUIRED_CATEGORY) });
            npc.Add(LOCKED_CATEGORY, new Npc.Trait[] { new Npc.Trait(LOCKED_TRAIT, LOCKED_CATEGORY) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { REQUIRED_CATEGORY, LOCKED_CATEGORY });
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsFalse(areValid, "Npc is incorrectly valid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(1, violations.Count, "Wrong number of violations");

            NpcSchemaViolation categoryNotFoundViolation = violations.Find(
                violation => violation.Category == LOCKED_CATEGORY &&
                violation.Trait == LOCKED_TRAIT &&
                violation.Violation == NpcSchemaViolation.Reason.HasTraitInLockedCategory);
            Assert.IsNotNull(categoryNotFoundViolation, "HasTraitInLockedCategory violation not detected");
        }

        [TestMethod]
        public void AreNpcsValidWithUnlockedCategory()
        {
            const string REQUIRED_CATEGORY = "Animal";
            const string REQUIRED_TRAIT = "Bear";
            const string ALTERNATIVE_TO_REQUIRED_TRAIT = "Rhino";

            const string LOCKED_CATEGORY = "Colour";
            const string LOCKED_TRAIT = "Blue";

            Trait c0t0 = new Trait(REQUIRED_TRAIT);
            Trait c0t1 = new Trait(ALTERNATIVE_TO_REQUIRED_TRAIT);
            TraitCategory category = new TraitCategory(REQUIRED_CATEGORY);
            category.Add(c0t0);
            category.Add(c0t1);

            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait hasTrait = new NpcHasTrait(new TraitId(categoryName: REQUIRED_CATEGORY, traitName: REQUIRED_TRAIT), npcHolder);
            Requirement requirement = new Requirement(hasTrait, npcHolder);

            Trait c1t0 = new Trait(LOCKED_TRAIT);
            TraitCategory lockedCategory = new TraitCategory(LOCKED_CATEGORY);
            lockedCategory.Add(c1t0);
            lockedCategory.Set(requirement);

            TraitSchema schema = new TraitSchema();
            schema.Add(category);
            schema.Add(lockedCategory);

            Npc npc = new Npc();
            npc.Add(REQUIRED_CATEGORY, new Npc.Trait[] { new Npc.Trait(REQUIRED_TRAIT, REQUIRED_CATEGORY) });
            npc.Add(LOCKED_CATEGORY, new Npc.Trait[] { new Npc.Trait(LOCKED_TRAIT, LOCKED_CATEGORY) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { REQUIRED_CATEGORY, LOCKED_CATEGORY });
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsTrue(areValid, "Npc is incorrectly invalid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(0, violations.Count, "Wrong number of violations");
        }

        [TestMethod]
        public void AreNpcsValidWithReplacementUnlockedCategory()
        {
            const string REQUIRED_CATEGORY = "Animal";
            const string TRAIT_ORIGINAL_NAME = "Velociraptor";
            const string REPLACEMENT_NAME = "Tyrannosaurus Rex";

            const string LOCKED_CATEGORY = "Colour";
            const string LOCKED_TRAIT = "Blue";

            Trait traitWithReplacement = new Trait(TRAIT_ORIGINAL_NAME);
            Trait c0t1 = new Trait("Rhino");
            Trait c0t2 = new Trait(REPLACEMENT_NAME);
            TraitCategory category = new TraitCategory(REQUIRED_CATEGORY);
            category.Add(traitWithReplacement);
            category.Add(c0t1);
            category.Add(c0t2);

            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait hasTrait = new NpcHasTrait(new TraitId(categoryName: REQUIRED_CATEGORY, traitName: REPLACEMENT_NAME), npcHolder);
            Requirement requirement = new Requirement(hasTrait, npcHolder);

            Trait c1t0 = new Trait(LOCKED_TRAIT);
            TraitCategory lockedCategory = new TraitCategory(LOCKED_CATEGORY);
            lockedCategory.Add(c1t0);
            lockedCategory.Set(requirement);

            TraitSchema schema = new TraitSchema();
            schema.Add(category);
            schema.Add(lockedCategory);

            Npc npc = new Npc();
            npc.Add(REQUIRED_CATEGORY, new Npc.Trait[] { new Npc.Trait(
                name: REPLACEMENT_NAME,
                originalCategory: REQUIRED_CATEGORY,
                isHidden: false,
                originalName: TRAIT_ORIGINAL_NAME) });
            npc.Add(LOCKED_CATEGORY, new Npc.Trait[] { new Npc.Trait(LOCKED_TRAIT, LOCKED_CATEGORY) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { REQUIRED_CATEGORY, LOCKED_CATEGORY });
            npcGroup.Add(npc);

            Replacement replacement = new Replacement(traitWithReplacement, REPLACEMENT_NAME, category);
            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>() { replacement }, out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsTrue(areValid, "Npc is incorrectly invalid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(0, violations.Count, "Wrong number of violations");
        }

        [TestMethod]
        public void AreNpcsValidWithOutputCategory()
        {
            const string ORIGINAL_CATEGORY = "Young Fame";
            const string OUTPUT_CATEGORY = "Fame";
            const string TRAIT = "Social Media";

            Trait trait = new Trait(TRAIT);
            TraitCategory category = new TraitCategory(ORIGINAL_CATEGORY, OUTPUT_CATEGORY, 1);
            category.Add(trait);

            TraitSchema schema = new TraitSchema();
            schema.Add(category);

            Npc npc = new Npc();
            npc.Add(OUTPUT_CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT, ORIGINAL_CATEGORY) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { OUTPUT_CATEGORY });
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsTrue(areValid, "Npc is incorrectly invalid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(0, violations.Count, "Wrong number of violations");
        }

        [TestMethod]
        public void AreNpcsValidWithOutputCategoryFromTwoSources()
        {
            const string OUTPUT_CATEGORY = "Fame";

            const string ORIGINAL_CATEGORY0 = "Young Fame";
            const string C0T0 = "Social Media";
            const string ORIGINAL_CATEGORY1 = "Old Fame";
            const string C1T0 = "Radio";

            Trait trait0 = new Trait(C0T0);
            TraitCategory category0 = new TraitCategory(ORIGINAL_CATEGORY0, OUTPUT_CATEGORY, 1);
            category0.Add(trait0);

            Trait trait1 = new Trait(C1T0);
            TraitCategory category1 = new TraitCategory(ORIGINAL_CATEGORY1, OUTPUT_CATEGORY, 1);
            category1.Add(trait1);

            TraitSchema schema = new TraitSchema();
            schema.Add(category0);
            schema.Add(category1);

            Npc npc = new Npc();
            npc.Add(OUTPUT_CATEGORY, new Npc.Trait[] { new Npc.Trait(C0T0, ORIGINAL_CATEGORY0), new Npc.Trait(C1T0, ORIGINAL_CATEGORY1) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { OUTPUT_CATEGORY });
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsTrue(areValid, "Npc is incorrectly invalid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(0, violations.Count, "Wrong number of violations");
        }

        [TestMethod]
        public void AreNpcsValidWithOutputCategoryNotFound()
        {
            const string OUTPUT_CATEGORY = "Fame";
            const string OUTPUT_CATEGORY_NOT_FOUND = "Infamous";

            const string ORIGINAL_CATEGORY0 = "Young Fame";
            const string C0T0 = "Social Media";
            const string ORIGINAL_CATEGORY1 = "Old Fame";
            const string C1T0 = "Radio";

            Trait trait0 = new Trait(C0T0);
            TraitCategory category0 = new TraitCategory(ORIGINAL_CATEGORY0, OUTPUT_CATEGORY, 1);
            category0.Add(trait0);

            Trait trait1 = new Trait(C1T0);
            TraitCategory category1 = new TraitCategory(ORIGINAL_CATEGORY1, OUTPUT_CATEGORY, 1);
            category1.Add(trait1);

            TraitSchema schema = new TraitSchema();
            schema.Add(category0);
            schema.Add(category1);

            Npc npc = new Npc();
            npc.Add(OUTPUT_CATEGORY_NOT_FOUND, new Npc.Trait[] { 
                new Npc.Trait(C0T0, ORIGINAL_CATEGORY0), new Npc.Trait(C1T0, ORIGINAL_CATEGORY1) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { OUTPUT_CATEGORY_NOT_FOUND });
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsFalse(areValid, "Npc is incorrectly valid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(2, violations.Count, "Wrong number of violations");
            Assert.AreEqual(NpcSchemaViolation.Reason.CategoryNotFoundInSchema, violations[0].Violation, "Wrong violation type");
            Assert.AreEqual(OUTPUT_CATEGORY_NOT_FOUND, violations[0].Category, "Wrong category not found");
            Assert.AreEqual(NpcSchemaViolation.Reason.CategoryNotFoundInSchema, violations[1].Violation, "Wrong violation type");
            Assert.AreEqual(OUTPUT_CATEGORY_NOT_FOUND, violations[1].Category, "Wrong category not found");
        }

        [TestMethod]
        public void AreNpcsValidWithUnlockedTrait()
        {
            const string REQUIRED_CATEGORY = "Animal";
            const string REQUIRED_TRAIT = "Bear";

            const string GUARDED_CATEGORY = "Colour";
            const string GUARDED_TRAIT = "Blue";

            TraitCategory requiredCategory = new TraitCategory(REQUIRED_CATEGORY);
            Trait requiredTrait = new Trait(REQUIRED_TRAIT);
            requiredCategory.Add(requiredTrait);

            TraitCategory guardedCategory = new TraitCategory(GUARDED_CATEGORY);
            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait npcHasTrait = new NpcHasTrait(new TraitId(REQUIRED_CATEGORY, REQUIRED_TRAIT), npcHolder);
            Requirement req = new Requirement(npcHasTrait, npcHolder);
            Trait guardedTrait = new Trait(GUARDED_TRAIT);
            guardedTrait.Set(req);
            guardedCategory.Add(guardedTrait);

            TraitSchema schema = new TraitSchema();
            schema.Add(requiredCategory);
            schema.Add(guardedCategory);

            Npc npc = new Npc();
            npc.Add(REQUIRED_CATEGORY, new Npc.Trait[] { new Npc.Trait(REQUIRED_TRAIT, REQUIRED_CATEGORY) });
            npc.Add(GUARDED_CATEGORY, new Npc.Trait[] { new Npc.Trait(GUARDED_TRAIT, GUARDED_CATEGORY) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { REQUIRED_CATEGORY, GUARDED_CATEGORY });
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsTrue(areValid, "Npc is incorrectly invalid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(0, violations.Count, "Wrong number of violations");
        }

        [TestMethod]
        public void AreNpcsValidViolationTraitLocked()
        {
            const string REQUIRED_CATEGORY = "Animal";
            const string REQUIRED_TRAIT = "Bear";
            const string NON_REQUIRED_TRAIT = "Rhino";

            const string GUARDED_CATEGORY = "Colour";
            const string GUARDED_TRAIT = "Blue";

            TraitCategory requiredCategory = new TraitCategory(REQUIRED_CATEGORY);
            Trait requiredTrait = new Trait(REQUIRED_TRAIT);
            requiredCategory.Add(requiredTrait);

            requiredCategory.Add(new Trait(NON_REQUIRED_TRAIT));

            TraitCategory guardedCategory = new TraitCategory(GUARDED_CATEGORY);
            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait npcHasTrait = new NpcHasTrait(new TraitId(REQUIRED_CATEGORY, REQUIRED_TRAIT), npcHolder);
            Requirement req = new Requirement(npcHasTrait, npcHolder);
            Trait guardedTrait = new Trait(GUARDED_TRAIT);
            guardedTrait.Set(req);
            guardedCategory.Add(guardedTrait);

            TraitSchema schema = new TraitSchema();
            schema.Add(requiredCategory);
            schema.Add(guardedCategory);

            Npc npc = new Npc();
            npc.Add(REQUIRED_CATEGORY, new Npc.Trait[] { new Npc.Trait(NON_REQUIRED_TRAIT, REQUIRED_CATEGORY) });
            npc.Add(GUARDED_CATEGORY, new Npc.Trait[] { new Npc.Trait(GUARDED_TRAIT, GUARDED_CATEGORY) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { REQUIRED_CATEGORY, GUARDED_CATEGORY });
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsFalse(areValid, "Npc is incorrectly valid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(1, violations.Count, "Wrong number of violations");
            Assert.AreEqual(NpcSchemaViolation.Reason.HasLockedTrait, violations[0].Violation, "Wrong violation reason");
            Assert.AreEqual(GUARDED_CATEGORY, violations[0].Category, "Wrong violation category");
            Assert.AreEqual(GUARDED_TRAIT, violations[0].Trait, "Wrong violation trait");
        }

        [TestMethod]
        public void AreNpcsValidWithOneOfTheMutuallyExclusiveTraits()
        {
            const string CATEGORY = "Animal";
            const string MUTUALLY_EXCLUSIVE_TRAIT0 = "Bear";
            const string MUTUALLY_EXCLUSIVE_TRAIT1 = "Rhino";
            const string NON_EXCLUSIVE_TRAIT = "Velociraptor";

            TraitCategory category = new TraitCategory(CATEGORY, 2);

            //Deliberately include the locked trait first to show that it is skipped over in favour of the unlocked trait.
            NpcHolder npcHolder0 = new NpcHolder();
            NpcHasTrait npcHasMutuallyExclusiveTrait1 = new NpcHasTrait(new TraitId(CATEGORY, MUTUALLY_EXCLUSIVE_TRAIT1), npcHolder0);
            LogicalNone none0 = new LogicalNone();
            none0.Add(npcHasMutuallyExclusiveTrait1);
            Requirement req0 = new Requirement(none0, npcHolder0);
            Trait mutuallyExclusiveTrait0 = new Trait(MUTUALLY_EXCLUSIVE_TRAIT0);
            mutuallyExclusiveTrait0.Set(req0);
            category.Add(mutuallyExclusiveTrait0);

            NpcHolder npcHolder1 = new NpcHolder();
            NpcHasTrait npcHasMutuallyExclusiveTrait0 = new NpcHasTrait(new TraitId(CATEGORY, MUTUALLY_EXCLUSIVE_TRAIT0), npcHolder1);
            LogicalNone none1 = new LogicalNone();
            none1.Add(npcHasMutuallyExclusiveTrait0);
            Requirement req1 = new Requirement(none1, npcHolder1);
            Trait mutuallyExclusiveTrait1 = new Trait(MUTUALLY_EXCLUSIVE_TRAIT1);
            mutuallyExclusiveTrait1.Set(req1);
            category.Add(mutuallyExclusiveTrait1);

            category.Add(new Trait(NON_EXCLUSIVE_TRAIT));

            TraitSchema schema = new TraitSchema();
            schema.Add(category);

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { 
                new Npc.Trait(MUTUALLY_EXCLUSIVE_TRAIT0, CATEGORY), 
                new Npc.Trait(NON_EXCLUSIVE_TRAIT, CATEGORY) 
            });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY });
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsTrue(areValid, "Npc is incorrectly invalid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(0, violations.Count, "Wrong number of violations");
        }

        [TestMethod]
        public void AreNpcsValidViolationWithBothMutuallyExclusiveTraits()
        {
            const string CATEGORY = "Animal";
            const string MUTUALLY_EXCLUSIVE_TRAIT0 = "Bear";
            const string MUTUALLY_EXCLUSIVE_TRAIT1 = "Rhino";

            TraitCategory category = new TraitCategory(CATEGORY, 2);

            //Deliberately include the locked trait first to show that it is skipped over in favour of the unlocked trait.
            NpcHolder npcHolder0 = new NpcHolder();
            NpcHasTrait npcHasMutuallyExclusiveTrait1 = new NpcHasTrait(new TraitId(CATEGORY, MUTUALLY_EXCLUSIVE_TRAIT1), npcHolder0);
            LogicalNone none0 = new LogicalNone();
            none0.Add(npcHasMutuallyExclusiveTrait1);
            Requirement req0 = new Requirement(none0, npcHolder0);
            Trait mutuallyExclusiveTrait0 = new Trait(MUTUALLY_EXCLUSIVE_TRAIT0);
            mutuallyExclusiveTrait0.Set(req0);
            category.Add(mutuallyExclusiveTrait0);

            NpcHolder npcHolder1 = new NpcHolder();
            NpcHasTrait npcHasMutuallyExclusiveTrait0 = new NpcHasTrait(new TraitId(CATEGORY, MUTUALLY_EXCLUSIVE_TRAIT0), npcHolder1);
            LogicalNone none1 = new LogicalNone();
            none1.Add(npcHasMutuallyExclusiveTrait0);
            Requirement req1 = new Requirement(none1, npcHolder1);
            Trait mutuallyExclusiveTrait1 = new Trait(MUTUALLY_EXCLUSIVE_TRAIT1);
            mutuallyExclusiveTrait1.Set(req1);
            category.Add(mutuallyExclusiveTrait1);

            TraitSchema schema = new TraitSchema();
            schema.Add(category);

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { 
                new Npc.Trait(MUTUALLY_EXCLUSIVE_TRAIT0, CATEGORY), 
                new Npc.Trait(MUTUALLY_EXCLUSIVE_TRAIT1, CATEGORY) 
            });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY });
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsFalse(areValid, "Npc is incorrectly valid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(2, violations.Count, "Wrong number of violations");

            NpcSchemaViolation lockedTrait0Violation = violations.Find(
                violation => violation.Category == CATEGORY &&
                violation.Trait == MUTUALLY_EXCLUSIVE_TRAIT0 &&
                violation.Violation == NpcSchemaViolation.Reason.HasLockedTrait);
            Assert.IsNotNull(lockedTrait0Violation, "HasLockedTrait violation not detected");

            NpcSchemaViolation lockedTrait1Violation = violations.Find(
                violation => violation.Category == CATEGORY &&
                violation.Trait == MUTUALLY_EXCLUSIVE_TRAIT1 &&
                violation.Violation == NpcSchemaViolation.Reason.HasLockedTrait);
            Assert.IsNotNull(lockedTrait1Violation, "HasLockedTrait violation not detected");
        }
    }
}
