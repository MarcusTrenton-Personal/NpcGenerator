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
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Tests
{
    [TestClass]
    public class NpcGeneratorModelTests : FileCreatingTests
    {
        [TestMethod]
        public void ConfigurationPathReflectsUserSettings()
        {
            const string FILE_PATH1 = "...";
            const string FILE_PATH2 = "FakeFile.csv";

            StubUserSettings userSettings = new StubUserSettings
            {
                ConfigurationPath = FILE_PATH1
            };

            NpcGeneratorModel npcGeneratorModel = new NpcGeneratorModel(
                userSettings,
                new StubAppSettings(),
                new StubMessager(),
                new StubLocalFileIo(),
                new MockCsvConfigurationParser(),
                new Dictionary<string, INpcExport>(),
                new StubLocalization(),
                new MockRandom(), 
                showErrorMessages: false,
                forceFailNpcGeneration: false);
            
            Assert.AreEqual(FILE_PATH1, npcGeneratorModel.ConfigurationPath, 
                "Configuration path is not the one in UserSettings");
            userSettings.ConfigurationPath = FILE_PATH2;

            Assert.AreEqual(FILE_PATH2, npcGeneratorModel.ConfigurationPath, 
                "Configuration path is not the one in UserSettings");
        }

        [TestMethod]
        public void NpcQuantityReflectsUserSettings()
        {
            const int QUANTITY1 = 1;
            const int QUANTITY2 = 5;

            StubUserSettings userSettings = new StubUserSettings();
            NpcGeneratorModel npcGeneratorModel = new NpcGeneratorModel(
                userSettings,
                new StubAppSettings(),
                new StubMessager(),
                new StubLocalFileIo(),
                new MockCsvConfigurationParser(),
                new Dictionary<string, INpcExport>(),
                new StubLocalization(),
                new MockRandom(),
                showErrorMessages: false,
                forceFailNpcGeneration: false);

            userSettings.NpcQuantity = QUANTITY1;
            Assert.AreEqual(QUANTITY1, npcGeneratorModel.NpcQuantity, "NpcQuantity is not the one in UserSettings");

            npcGeneratorModel.NpcQuantity = QUANTITY2;
            Assert.AreEqual(QUANTITY2, userSettings.NpcQuantity, "NpcQuantity is not the one in UserSettings");
            Assert.AreEqual(QUANTITY2, npcGeneratorModel.NpcQuantity, "NpcQuantity is not the one in UserSettings");
        }

        [TestMethod]
        public void InvalidNpcQuantityBlocksNpcGeneration()
        {
            StubUserSettings userSettings = new StubUserSettings();
            NpcGeneratorModel npcGeneratorModel = new NpcGeneratorModel(
                userSettings,
                new StubAppSettings(),
                new StubMessager(),
                new StubLocalFileIo(),
                new MockCsvConfigurationParser(),
                new Dictionary<string, INpcExport>(),
                new StubLocalization(),
                new MockRandom(),
                showErrorMessages: false,
                forceFailNpcGeneration: false)
            {
                NpcQuantity = 0
            };
            bool canGenerateNpcs = npcGeneratorModel.GenerateNpcs.CanExecute(null);
            Assert.IsFalse(canGenerateNpcs, "Not blocking the generation of 0 npcs");
        }

        [TestMethod]
        public void InvalidConfigurationPathBlocksNpcGeneration()
        {
            StubUserSettings userSettings = new StubUserSettings();
            NpcGeneratorModel npcGeneratorModel = new NpcGeneratorModel(
                userSettings,
                new StubAppSettings(),
                new StubMessager(),
                new StubLocalFileIo(),
                new MockCsvConfigurationParser(),
                new Dictionary<string, INpcExport>(),
                new StubLocalization(),
                new MockRandom(),
                showErrorMessages: false,
                forceFailNpcGeneration: false);

            userSettings.ConfigurationPath = "Bad file";
            bool canGenerateNpcs = npcGeneratorModel.GenerateNpcs.CanExecute(null);
            Assert.IsFalse(canGenerateNpcs, "Not blocking the generation of npcs with invalid ConfigurationPath");
        }

        [TestMethod]
        public void GenerateNpcs()
        {
            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            StubUserSettings userSettings = UserSettingsWithFakeInputFile(method);

            static TraitSchema Callback(string path)
            {
                Trait green = new Trait("Green");
                Trait red = new Trait("Red");
                
                TraitCategory category = new TraitCategory("Colour");
                category.Add(green);
                category.Add(red);

                TraitSchema schema = new TraitSchema();
                schema.Add(category);

                return schema;
            }

            NpcGeneratorModel npcGeneratorModel = new NpcGeneratorModel(
                userSettings,
                new StubAppSettings(),
                new StubMessager(),
                new StubLocalFileIo(),
                new CallbackConfigurationParser(Callback),
                new Dictionary<string, INpcExport>(),
                new StubLocalization(),
                new MockRandom(),
                showErrorMessages: false,
                forceFailNpcGeneration: false); 

            const int QUANTITY = 5;
            npcGeneratorModel.NpcQuantity = 5;

            npcGeneratorModel.GenerateNpcs.Execute(null);
            DataTable result = npcGeneratorModel.ResultNpcs;

            Assert.AreEqual(QUANTITY, result.Rows.Count, "Generate the wrong number of npcs");
            bool canSave = npcGeneratorModel.SaveNpcs.CanExecute(null);
            Assert.IsTrue(canSave, "Have npcs but cannot save");

            File.Delete(userSettings.ConfigurationPath);
        }

        private StubUserSettings UserSettingsWithFakeInputFile(string fileNameWithoutDot)
        {
            StubUserSettings userSettings = new StubUserSettings();
            string path = Path.Combine(TestDirectory, fileNameWithoutDot + ".csv");
            string text = "Empty";
            File.WriteAllText(path, text);
            userSettings.ConfigurationPath = path;
            return userSettings;
        }

        [TestMethod]
        public void CannotSaveWithoutNpcs()
        {
            StubUserSettings userSettings = new StubUserSettings();
            NpcGeneratorModel npcGeneratorModel = new NpcGeneratorModel(
                userSettings,
                new StubAppSettings(),
                new StubMessager(),
                new StubLocalFileIo(),
                new MockCsvConfigurationParser(),
                new Dictionary<string, INpcExport>(),
                new StubLocalization(),
                new MockRandom(),
                showErrorMessages: false,
                forceFailNpcGeneration: false);

            bool canSave = npcGeneratorModel.SaveNpcs.CanExecute(null);
            Assert.IsFalse(canSave, "Can save even though there are no npcs");
        }

        [TestMethod]
        public void ReplacementsForNullSchema()
        {
            NpcGeneratorModel npcGeneratorModel = new NpcGeneratorModel(
                new StubUserSettings(),
                new StubAppSettings(),
                new StubMessager(),
                new StubLocalFileIo(),
                new MockCsvConfigurationParser(),
                new Dictionary<string, INpcExport>(),
                new StubLocalization(),
                new MockRandom(),
                showErrorMessages: false,
                forceFailNpcGeneration: false);

            IReadOnlyList<ReplacementSubModel> replacements = npcGeneratorModel.Replacements;

            Assert.AreEqual(0, replacements.Count, "Replacements somehow found for a null schema");
        }

        [TestMethod]
        public void ReplacementsForSchemaWithoutReplacements()
        {
            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            StubUserSettings userSettings = UserSettingsWithFakeInputFile(method);

            static TraitSchema Callback(string path)
            {
                Trait green = new Trait("Green");
                Trait red = new Trait("Red");

                TraitCategory category = new TraitCategory("Colour");
                category.Add(green);
                category.Add(red);

                TraitSchema schema = new TraitSchema();
                schema.Add(category);

                return schema;
            }

            NpcGeneratorModel npcGeneratorModel = new NpcGeneratorModel(
                userSettings,
                new StubAppSettings(),
                new StubMessager(),
                new StubLocalFileIo(),
                new CallbackConfigurationParser(Callback),
                new Dictionary<string, INpcExport>(),
                new StubLocalization(),
                new MockRandom(),
                showErrorMessages: false,
                forceFailNpcGeneration: false);

            IReadOnlyList<ReplacementSubModel> replacements = npcGeneratorModel.Replacements;

            Assert.AreEqual(0, replacements.Count, "Replacements somehow found for a replacement-less schema");

            File.Delete(userSettings.ConfigurationPath);
        }

        [TestMethod]
        public void ReplacementsForSchemaWithReplacements()
        {
            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            StubUserSettings userSettings = UserSettingsWithFakeInputFile(method);

            const string REPLACEMENT_CATEGORY = "Colour";
            const string ORIGINAL_TRAIT_NAME = "Green";
            const string REPLACEMENT_CANDIDATE_TRAIT_NAME = "Red";

            static TraitSchema Callback(string path)
            {
                Trait originalTrait = new Trait(ORIGINAL_TRAIT_NAME);
                Trait replacementCandidateTrait = new Trait(REPLACEMENT_CANDIDATE_TRAIT_NAME);

                TraitCategory category = new TraitCategory(REPLACEMENT_CATEGORY);
                category.Add(originalTrait);
                category.Add(replacementCandidateTrait);

                ReplacementSearch replacementSearch = new ReplacementSearch(originalTrait, category);

                TraitSchema schema = new TraitSchema();
                schema.Add(category);
                schema.Add(replacementSearch);

                return schema;
            }

            NpcGeneratorModel npcGeneratorModel = new NpcGeneratorModel(
                userSettings,
                new StubAppSettings(),
                new StubMessager(),
                new StubLocalFileIo(),
                new CallbackConfigurationParser(Callback),
                new Dictionary<string, INpcExport>(),
                new StubLocalization(),
                new MockRandom(),
                showErrorMessages: false,
                forceFailNpcGeneration: false);

            IReadOnlyList<ReplacementSubModel> replacements = npcGeneratorModel.Replacements;

            Assert.AreEqual(1, replacements.Count, "Wrong number of replacements found");
            Assert.AreEqual(REPLACEMENT_CATEGORY, replacements[0].Category, "Wrong replacement category");
            Assert.AreEqual(ORIGINAL_TRAIT_NAME, replacements[0].OriginalTrait, "Wrong original trait");
            string[] replacementCandidates = replacements[0].ReplacementTraits;

            Assert.AreEqual(2, replacementCandidates.Length, "Wrong number of replacement candidates");
            bool isOriginalTraitCandidate = Array.FindIndex(replacementCandidates, candidate => candidate == ORIGINAL_TRAIT_NAME) >= 0;
            Assert.IsTrue(isOriginalTraitCandidate, "Wrong replacement candidates. Should include original trait " + ORIGINAL_TRAIT_NAME);
            bool isReplacementTraitCandidate = Array.FindIndex(
                replacementCandidates, candidate => candidate == REPLACEMENT_CANDIDATE_TRAIT_NAME) >= 0;
            Assert.IsTrue(isOriginalTraitCandidate, "Wrong replacement candidates. Should include original trait " + 
                REPLACEMENT_CANDIDATE_TRAIT_NAME);

            File.Delete(userSettings.ConfigurationPath);
        }

        [TestMethod]
        public void GenerateCatchesIOException()
        {
            static IOException CreateCallback()
            {
                return new IOException();
            }

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            GenerateCatches(CreateCallback, method);
        }

        [TestMethod]
        public void GenerateCatchesEmptyFileException()
        {
            static EmptyFileException CreateCallback()
            {
                return new EmptyFileException("test.csv");
            }

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            GenerateCatches(CreateCallback, method);
        }

        [TestMethod]
        public void GenerateCatchesEmptyCategoryNameException()
        {
            static EmptyCategoryNameException CreateCallback()
            {
                return new EmptyCategoryNameException();
            }

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            GenerateCatches(CreateCallback, method);
        }

        [TestMethod]
        public void GenerateCatchesCategoryWeightMismatchException()
        {
            static CategoryWeightMismatchException CreateCallback()
            {
                return new CategoryWeightMismatchException();
            }

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            GenerateCatches(CreateCallback, method);
        }

        [TestMethod]
        public void GenerateCatchesTraitMissingCategoryException()
        {
            static TraitMissingCategoryException CreateCallback()
            {
                return new TraitMissingCategoryException("Blue");
            }

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            GenerateCatches(CreateCallback, method);
        }

        [TestMethod]
        public void GenerateCatchesMissingWeightException()
        {
            static MissingWeightException CreateCallback()
            {
                return new MissingWeightException(new TraitId("Animal", "Bear"));
            }

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            GenerateCatches(CreateCallback, method);
        }

        [TestMethod]
        public void GenerateCatchesWeightIsNotWholeNumberException()
        {
            static WeightIsNotWholeNumberException CreateCallback()
            {
                return new WeightIsNotWholeNumberException(new TraitId("Animal", "Bear"), "-1.5");
            }

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            GenerateCatches(CreateCallback, method);
        }

        [TestMethod]
        public void GenerateCatchesMismatchedBonusSelectionException()
        {
            static MismatchedBonusSelectionException CreateCallback()
            {
                return new MismatchedBonusSelectionException("Animal", new TraitId("Colour", "Blue"));
            }

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            GenerateCatches(CreateCallback, method);
        }

        [TestMethod]
        public void GenerateCatchesMismatchedReplacementTraitException()
        {
            static MissingReplacementTraitException CreateCallback()
            {
                return new MissingReplacementTraitException(new TraitId("Colour", "Blue"));
            }

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            GenerateCatches(CreateCallback, method);
        }

        [TestMethod]
        public void GenerateCatchesMismatchedReplacementCategoryException()
        {
            static MissingReplacementCategoryException CreateCallback()
            {
                return new MissingReplacementCategoryException(new TraitId("Colour", "Blue"));
            }

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            GenerateCatches(CreateCallback, method);
        }

        [TestMethod]
        public void GenerateCatchesDuplicateCategoryNameException()
        {
            static DuplicateCategoryNameException CreateCallback()
            {
                return new DuplicateCategoryNameException("Colour");
            }

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            GenerateCatches(CreateCallback, method);
        }

        [TestMethod]
        public void GenerateCatchesRequirementTraitIdNotFoundException()
        {
            static RequirementTraitIdNotFoundException CreateCallback()
            {
                return new RequirementTraitIdNotFoundException("Animal", new TraitId("Colour", "Blue"));
            }

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            GenerateCatches(CreateCallback, method);
        }

        [TestMethod]
        public void GenerateCatchesUnknownLogicalOperatorException()
        {
            static UnknownLogicalOperatorException CreateCallback()
            {
                return new UnknownLogicalOperatorException("Animal", "bitshift");
            }

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            GenerateCatches(CreateCallback, method);
        }

        [TestMethod]
        public void GenerateCatchesSelfRequiringCategoryException()
        {
            static SelfRequiringCategoryException CreateCallback()
            {
                return new SelfRequiringCategoryException("Animal");
            }

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            GenerateCatches(CreateCallback, method);
        }

        [TestMethod]
        public void GenerateCatchesTooFewTraitsInCategoryException()
        {
            static TooFewTraitsInCategoryException CreateCallback()
            {
                return new TooFewTraitsInCategoryException("Animal", 3, 1);
            }

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            GenerateCatches(CreateCallback, method);
        }

        [TestMethod]
        public void GenerateCatchesCircularRequirementsException()
        {
            static CircularRequirementsException CreateCallback()
            {
                return new CircularRequirementsException(new List<TraitSchema.Dependency>());
            }

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            GenerateCatches(CreateCallback, method);
        }

        private void GenerateCatches<T>(Func<T> createException, string fileName) where T : Exception
        {
            StubUserSettings userSettings = UserSettingsWithFakeInputFile(fileName);

            TraitSchema Callback(string path)
            {
                throw createException();
            }

            NpcGeneratorModel npcGeneratorModel = new NpcGeneratorModel(
                userSettings,
                new StubAppSettings(),
                new StubMessager(),
                new StubLocalFileIo(),
                new CallbackConfigurationParser(Callback),
                new Dictionary<string, INpcExport>(),
                new StubLocalization(),
                new MockRandom(),
                showErrorMessages: false,
                forceFailNpcGeneration: false)
            {
                NpcQuantity = 1
            };

            bool modelCaughtException = true;
            try
            {
                npcGeneratorModel.GenerateNpcs.Execute(null);
            }
            catch (T)
            {
                modelCaughtException = false;
            }

            Assert.IsTrue(modelCaughtException, "NpcGeneratorModel.GenerateNpcs() failed to catch " + typeof(T));

            File.Delete(userSettings.ConfigurationPath);
        }

        //Catching generic exceptions is bad, as it promotes laziness in exception handling.
        //Each exception should be caught individually and localized for the user,
        //not swallowed into a generic "Something went wrong" message.
        [TestMethod]
        public void GenerateDoesNotCatchAllExceptions()
        {
            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            StubUserSettings userSettings = UserSettingsWithFakeInputFile(method);

            static TraitSchema Callback(string path)
            {
                throw new Exception();
            }

            NpcGeneratorModel npcGeneratorModel = new NpcGeneratorModel(
                userSettings,
                new StubAppSettings(),
                new StubMessager(),
                new StubLocalFileIo(),
                new CallbackConfigurationParser(Callback),
                new Dictionary<string, INpcExport>(),
                new StubLocalization(),
                new MockRandom(),
                showErrorMessages: false,
                forceFailNpcGeneration: false)
            {
                NpcQuantity = 1
            };

            bool modelCaughtException = true;
            try
            {
                npcGeneratorModel.GenerateNpcs.Execute(null);
            }
            catch (Exception)
            {
                modelCaughtException = false;
            }

            Assert.IsFalse(modelCaughtException, "NpcGeneratorModel.GenerateNpcs() caught generic exception when it should not");

            File.Delete(userSettings.ConfigurationPath);
        }

        [TestMethod]
        public void GenerateCatchesTooFewTraitsInCategoryExceptionDueToBonusSelections()
        {
            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            StubUserSettings userSettings = UserSettingsWithFakeInputFile(method);

            static TraitSchema Callback(string path)
            {
                Trait trait = new Trait("Green");
                TraitCategory category = new TraitCategory("Colour", 1);
                trait.BonusSelection = new BonusSelection(category.Name, 10);
                category.Add(trait);

                TraitSchema schema = new TraitSchema();
                schema.Add(category);

                return schema;
            }

            NpcGeneratorModel npcGeneratorModel = new NpcGeneratorModel(
                userSettings,
                new StubAppSettings(),
                new StubMessager(),
                new StubLocalFileIo(),
                new CallbackConfigurationParser(Callback),
                new Dictionary<string, INpcExport>(),
                new StubLocalization(),
                new MockRandom(),
                showErrorMessages: false,
                forceFailNpcGeneration: false)
            {
                NpcQuantity = 1
            };

            bool modelCaughtException = true;
            try
            {
                npcGeneratorModel.GenerateNpcs.Execute(null);
            }
            catch (Exception)
            {
                modelCaughtException = false;
            }

            Assert.IsTrue(modelCaughtException, "NpcGeneratorModel.GenerateNpcs() failed to catch TooFewTraitsInCategoryException");

            File.Delete(userSettings.ConfigurationPath);
        }
    }

    internal class CallbackConfigurationParser : IConfigurationParser
    {
        public CallbackConfigurationParser(Func<string, TraitSchema> callback)
        {
            m_callback = callback;
        }

        public TraitSchema Parse(string path)
        {
            return m_callback(path);
        }

        private readonly Func<string, TraitSchema> m_callback;
    }
}
