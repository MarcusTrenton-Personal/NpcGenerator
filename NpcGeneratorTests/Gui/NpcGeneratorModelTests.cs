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
                new StubMessager(),
                new StubLocalFileIo(),
                new MockCsvConfigurationParser(),
                new Dictionary<string, INpcExport>(),
                new StubLocalization(),
                new MockRandom(), 
                showErrorMessages: false);
            
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
                new StubMessager(),
                new StubLocalFileIo(),
                new MockCsvConfigurationParser(),
                new Dictionary<string, INpcExport>(),
                new StubLocalization(),
                new MockRandom(),
                showErrorMessages: false);

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
                new StubMessager(),
                new StubLocalFileIo(),
                new MockCsvConfigurationParser(),
                new Dictionary<string, INpcExport>(),
                new StubLocalization(),
                new MockRandom(),
                showErrorMessages: false)
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
                new StubMessager(),
                new StubLocalFileIo(),
                new MockCsvConfigurationParser(),
                new Dictionary<string, INpcExport>(),
                new StubLocalization(),
                new MockRandom(),
                showErrorMessages: false);

            userSettings.ConfigurationPath = "Bad file";
            bool canGenerateNpcs = npcGeneratorModel.GenerateNpcs.CanExecute(null);
            Assert.IsFalse(canGenerateNpcs, "Not blocking the generation of npcs with invalid ConfigurationPath");
        }

        [TestMethod]
        public void GenerateNpcs()
        {
            StubUserSettings userSettings = new StubUserSettings();
            string path = Path.Combine(TestDirectory, "colour.csv");
            string text = "Colour,Weight\n" +
                "Green,1\n" +
                "Red,1";
            File.WriteAllText(path, text);
            userSettings.ConfigurationPath = path;

            NpcGeneratorModel npcGeneratorModel = new NpcGeneratorModel(
                userSettings,
                new StubMessager(),
                new StubLocalFileIo(),
                new MockCsvConfigurationParser(),
                new Dictionary<string, INpcExport>(),
                new StubLocalization(),
                new MockRandom(),
                showErrorMessages: false); 

            const int QUANTITY = 5;
            npcGeneratorModel.NpcQuantity = 5;

            npcGeneratorModel.GenerateNpcs.Execute(null);
            DataTable result = npcGeneratorModel.ResultNpcs;

            Assert.AreEqual(QUANTITY, result.Rows.Count, "Generate the wrong number of npcs");
            bool canSave = npcGeneratorModel.SaveNpcs.CanExecute(null);
            Assert.IsTrue(canSave, "Have npcs but cannot save");

            File.Delete(path);
        }

        [TestMethod]
        public void CannotSaveWithoutNpcs()
        {
            StubUserSettings userSettings = new StubUserSettings();
            NpcGeneratorModel npcGeneratorModel = new NpcGeneratorModel(
                userSettings,
                new StubMessager(),
                new StubLocalFileIo(),
                new MockCsvConfigurationParser(),
                new Dictionary<string, INpcExport>(),
                new StubLocalization(),
                new MockRandom(),
                showErrorMessages: false);

            bool canSave = npcGeneratorModel.SaveNpcs.CanExecute(null);
            Assert.IsFalse(canSave, "Can save even though there are no npcs");
        }

        [TestMethod]
        public void ReplacementsForNullSchema()
        {
            NpcGeneratorModel npcGeneratorModel = new NpcGeneratorModel(
                new StubUserSettings(),
                new StubMessager(),
                new StubLocalFileIo(),
                new MockCsvConfigurationParser(),
                new Dictionary<string, INpcExport>(),
                new StubLocalization(),
                new MockRandom(),
                showErrorMessages: false);

            IReadOnlyList<ReplacementSubModel> replacements = npcGeneratorModel.Replacements;

            Assert.AreEqual(0, replacements.Count, "Replacements somehow found for a null schema");
        }

        [TestMethod]
        public void ReplacementsForSchemaWithoutReplacements()
        {
            StubUserSettings userSettings = new StubUserSettings();
            string path = Path.Combine(TestDirectory, "colour2.csv");
            string text = "Colour,Weight\n" +
                "Green,1\n" +
                "Red,1";
            File.WriteAllText(path, text);
            userSettings.ConfigurationPath = path;

            NpcGeneratorModel npcGeneratorModel = new NpcGeneratorModel(
                userSettings,
                new StubMessager(),
                new StubLocalFileIo(),
                new MockCsvConfigurationParser(),
                new Dictionary<string, INpcExport>(),
                new StubLocalization(),
                new MockRandom(),
                showErrorMessages: false);

            IReadOnlyList<ReplacementSubModel> replacements = npcGeneratorModel.Replacements;

            Assert.AreEqual(0, replacements.Count, "Replacements somehow found for a replacement-less schema");

            File.Delete(path);
        }

        [TestMethod]
        public void ReplacementsForSchemaWithReplacements()
        {
            StubUserSettings userSettings = new StubUserSettings();
            string path = Path.Combine(TestDirectory, "replacements.json");
            string text = @"{
                'replacements' : [
                    {
                        'category_name' : 'Colour',
                        'trait_name' : 'Green'
                    }
                ],
                'trait_categories' : [
                    {
                        'name' : 'Colour',
                        'selections': 1,
                        'traits' : [
                            { 
                                'name' : 'Green', 
                                'weight' : 1
                            },
                            { 
                                'name' : 'Red', 
                                'weight' : 1
                            }
                        ]
                    }
                ]
            }";
            File.WriteAllText(path, text);
            userSettings.ConfigurationPath = path;

            NpcGeneratorModel npcGeneratorModel = new NpcGeneratorModel(
                userSettings,
                new StubMessager(),
                new StubLocalFileIo(),
                new MockJsonConfigurationParser(),
                new Dictionary<string, INpcExport>(),
                new StubLocalization(),
                new MockRandom(),
                showErrorMessages: false);

            IReadOnlyList<ReplacementSubModel> replacements = npcGeneratorModel.Replacements;

            Assert.AreEqual(1, replacements.Count, "Wrong number of replacements found.");

            File.Delete(path);
        }

        [TestMethod]
        public void GenerateCatchesIOException()
        {
            static IOException CreateCallback()
            {
                return new IOException();
            }

            GenerateCatches(CreateCallback);
        }

        [TestMethod]
        public void GenerateCatchesMismatchedBonusSelectionException()
        {
            static MismatchedBonusSelectionException CreateCallback()
            {
                return new MismatchedBonusSelectionException("Animal", new TraitId("Colour", "Blue"));
            }

            GenerateCatches(CreateCallback);
        }

        [TestMethod]
        public void GenerateCatchesMismatchedReplacementTraitException()
        {
            static MismatchedReplacementTraitException CreateCallback()
            {
                return new MismatchedReplacementTraitException(new TraitId("Colour", "Blue"));
            }

            GenerateCatches(CreateCallback);
        }

        [TestMethod]
        public void GenerateCatchesMismatchedReplacementCategoryException()
        {
            static MismatchedReplacementCategoryException CreateCallback()
            {
                return new MismatchedReplacementCategoryException(new TraitId("Colour", "Blue"));
            }

            GenerateCatches(CreateCallback);
        }

        [TestMethod]
        public void GenerateCatchesDuplicateCategoryNameException()
        {
            static DuplicateCategoryNameException CreateCallback()
            {
                return new DuplicateCategoryNameException("Colour");
            }

            GenerateCatches(CreateCallback);
        }

        [TestMethod]
        public void GenerateCatchesRequirementTraitIdNotFoundException()
        {
            static RequirementTraitIdNotFoundException CreateCallback()
            {
                return new RequirementTraitIdNotFoundException("Animal", new TraitId("Colour", "Blue"));
            }

            GenerateCatches(CreateCallback);
        }

        [TestMethod]
        public void GenerateCatchesUnknownLogicalOperatorException()
        {
            static UnknownLogicalOperatorException CreateCallback()
            {
                return new UnknownLogicalOperatorException("Animal", "bitshift");
            }

            GenerateCatches(CreateCallback);
        }

        [TestMethod]
        public void GenerateCatchesSelfRequiringCategoryException()
        {
            static SelfRequiringCategoryException CreateCallback()
            {
                return new SelfRequiringCategoryException("Animal");
            }

            GenerateCatches(CreateCallback);
        }

        [TestMethod]
        public void GenerateCatchesFormatException()
        {
            static FormatException CreateCallback()
            {
                return new FormatException();
            }

            GenerateCatches(CreateCallback);
        }

        [TestMethod]
        public void GenerateCatchesArithmeticException()
        {
            static ArithmeticException CreateCallback()
            {
                return new ArithmeticException();
            }

            GenerateCatches(CreateCallback);
        }

        [TestMethod]
        public void GenerateCatchesTooFewTraitsInCategoryException()
        {
            static TooFewTraitsInCategoryException CreateCallback()
            {
                return new TooFewTraitsInCategoryException("Animal", 3, 1);
            }

            GenerateCatches(CreateCallback);
        }

        private void GenerateCatches<T>(Func<T> createException) where T : Exception
        {
            StubUserSettings userSettings = new StubUserSettings();
            string path = Path.Combine(TestDirectory, "colour.csv");
            string text = "Colour,Weight\n" +
                "Green,1\n";
            File.WriteAllText(path, text);
            userSettings.ConfigurationPath = path;

            TraitSchema Callback(string path)
            {
                throw createException();
            }

            NpcGeneratorModel npcGeneratorModel = new NpcGeneratorModel(
                userSettings,
                new StubMessager(),
                new StubLocalFileIo(),
                new CallbackConfigurationParser(Callback),
                new Dictionary<string, INpcExport>(),
                new StubLocalization(),
                new MockRandom(),
                showErrorMessages: false)
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

            Assert.IsTrue(modelCaughtException, "NpcGeneratorModel.GenerateNpcs() failed to catch " + typeof(T));

            File.Delete(path);
        }

        //Catching generic exceptions is bad, as it promotes laziness in exception handling.
        //Each exception should be caught individually and localized for the user,
        //not swallowed into a generic "Something went wrong" message.
        [TestMethod]
        public void GenerateDoesNotCatchAllExceptions()
        {
            StubUserSettings userSettings = new StubUserSettings();
            string path = Path.Combine(TestDirectory, "colour.csv");
            string text = "Colour,Weight\n" +
                "Green,1\n";
            File.WriteAllText(path, text);
            userSettings.ConfigurationPath = path;

            TraitSchema Callback(string path)
            {
                throw new Exception();
            }

            NpcGeneratorModel npcGeneratorModel = new NpcGeneratorModel(
                userSettings,
                new StubMessager(),
                new StubLocalFileIo(),
                new CallbackConfigurationParser(Callback),
                new Dictionary<string, INpcExport>(),
                new StubLocalization(),
                new MockRandom(),
                showErrorMessages: false)
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

            File.Delete(path);
        }

        [TestMethod]
        public void GenerateCatchesTooFewTraitsInCategoryExceptionPostParsing()
        {
            StubUserSettings userSettings = new StubUserSettings();
            string path = Path.Combine(TestDirectory, "too_many_selections.json");
            string text = @"{
                'trait_categories' : [
                    {
                        'name' : 'Colour',
                        'selections': 10,
                        'traits' : [
                            { 
                                'name' : 'Green', 
                                'weight' : 1
                            },
                            { 
                                'name' : 'Red', 
                                'weight' : 1
                            }
                        ]
                    }
                ]
            }";
            File.WriteAllText(path, text);
            userSettings.ConfigurationPath = path;

            NpcGeneratorModel npcGeneratorModel = new NpcGeneratorModel(
                userSettings,
                new StubMessager(),
                new StubLocalFileIo(),
                new MockJsonConfigurationParser(),
                new Dictionary<string, INpcExport>(),
                new StubLocalization(),
                new MockRandom(),
                showErrorMessages: false);

            npcGeneratorModel.NpcQuantity = 1;

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

            File.Delete(path);
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
