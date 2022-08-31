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

using Microsoft.Win32;
using Services;
using Services.Message;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Input;
using WpfServices;

namespace NpcGenerator
{
    public class NpcGeneratorModel : BaseModel, INpcGeneratorModel
    {
        public NpcGeneratorModel(
            IUserSettings userSettings,
            IMessager messager,
            ILocalFileIO fileIo,
            IConfigurationParser parser,
            Dictionary<string, INpcExport> npcExporters,
            ILocalization localization,
            IRandom random,
            bool showErrorMessages)
        {
            m_userSettings = userSettings;
            m_messager = messager;
            m_fileIo = fileIo;
            m_parser = parser;
            m_npcExporters = npcExporters;
            m_localization = localization;
            m_random = random;
            m_showErrorMessages = showErrorMessages;
            m_configurationHasError = false;

            CreateFileWatcher();

            //Deliberately do parse the initial schema, as there is no way to show an error message during boot-up.
            //Instead evaluate lazily.
        }

        ~NpcGeneratorModel()
        {
            TearDownFileWatcher();
        }

        private void CreateFileWatcher()
        {
            m_configurationFileWatcher = new FileSystemWatcher
            {
                NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastWrite,
                EnableRaisingEvents = false
            };
            m_configurationFileWatcher.Deleted += OnConfigurationChanged;
            m_configurationFileWatcher.Renamed += OnConfigurationRenamed;
            m_configurationFileWatcher.Created += OnConfigurationChanged;
            m_configurationFileWatcher.Changed += OnConfigurationChanged;

            SetFileWatcherPath();
        }

        private void TearDownFileWatcher()
        {
            m_configurationFileWatcher.Deleted -= OnConfigurationChanged;
            m_configurationFileWatcher.Renamed -= OnConfigurationRenamed;
            m_configurationFileWatcher.Created -= OnConfigurationChanged;
            m_configurationFileWatcher.Changed -= OnConfigurationChanged;
        }

        private void LazyParseTraitSchema()
        {
            if (m_traitSchema == null && !m_configurationHasError)
            {
                m_configurationHasError = !ParseTraitSchema(out m_traitSchema, out m_replacementSubModels);
            }
        }

        public ICommand ChooseConfiguration 
        { 
            get
            {
                return m_chooseConfigurationCommand ??= new CommandHandler(
                    (object parameter) => ExecuteChooseConfiguration(parameter),
                    (object parameter) => AlwaysTrue(parameter));
            }
        }

        public string ConfigurationPath 
        { 
            get
            {
                return m_userSettings.ConfigurationPath;
            }
        }

        public bool IsConfigurationValid 
        { 
            get
            {
                LazyParseTraitSchema();
                bool doesExist = File.Exists(m_userSettings.ConfigurationPath);
                return doesExist && !m_configurationHasError;
            }
        }

        public int NpcQuantity
        {
            get
            {
                return m_userSettings.NpcQuantity;
            }

            set
            {
                m_userSettings.NpcQuantity = value;
            }
        }

        public IReadOnlyList<ReplacementSubModel> Replacements 
        { 
            get
            {
                LazyParseTraitSchema();
                return m_replacementSubModels;
            }
        }

        public ICommand GenerateNpcs 
        { 
            get
            {
                return m_generateNpcsCommand ??= new CommandHandler(
                    (object parameter) => ExecuteGenerateNpcs(parameter),
                    (object parameter) => CanExecuteGenerateNpcs(parameter));
            }
        }

        public DataTable ResultNpcs 
        { 
            get
            {
                return m_table;
            }
        }

        public ICommand SaveNpcs 
        { 
            get
            {
                return m_saveNpcsCommand ??= new CommandHandler(
                    (object parameter) => ExecuteSaveNpcs(parameter),
                    (object parameter) => CanExecuteSaveNpcs(parameter));
            }
        }

        private static bool AlwaysTrue(object _)
        {
            return true;
        }

        private void ExecuteChooseConfiguration(object _)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text CSV (*.csv)|*.csv|Text JSON (*.json)|*json|All files (*.*)|*.*"
            };
            bool? filePicked = openFileDialog.ShowDialog();
            if (filePicked == true)
            {
                m_userSettings.ConfigurationPath = openFileDialog.FileName;
                NotifyPropertyChanged("ConfigurationPath");
                NotifyPropertyChanged("IsConfigurationValid");
                SetFileWatcherPath();

                m_configurationHasError = !ParseTraitSchema(out m_traitSchema, out m_replacementSubModels);
                m_messager.Send(sender: this, message: new Message.SelectConfiguration());
            }
        }

        private bool CanExecuteGenerateNpcs(object _)
        {
            bool configurationFileExists = File.Exists(m_userSettings.ConfigurationPath);
            bool isNpcQuantityValid = m_userSettings.NpcQuantity > 0;
            bool canExecute = configurationFileExists && isNpcQuantityValid;
            return canExecute;
        }

        private void ExecuteGenerateNpcs(object _)
        {
            LazyParseTraitSchema();
            if (m_traitSchema == null)
            {
                return;
            }

            List<Replacement> replacements = GetReplacements(m_replacementSubModels, m_traitSchema);
            try
            {
                m_npcGroup = NpcFactory.Create(m_traitSchema, m_userSettings.NpcQuantity, replacements, m_random);

                DataTable table = new DataTable("Npc Table");
                for (int i = 0; i < m_npcGroup.CategoryOrder.Count; ++i)
                {
                    table.Columns.Add(m_npcGroup.GetTraitCategoryNameAtIndex(i));
                }
                for (int i = 0; i < m_npcGroup.NpcCount; ++i)
                {
                    string[] text = NpcToStringArray.Export(m_npcGroup.GetNpcAtIndex(i), m_npcGroup.CategoryOrder);
                    table.Rows.Add(text);
                }
                m_table = table;
                NotifyPropertyChanged("ResultNpcs");

                m_messager.Send(sender: this, message: new Message.GenerateNpcs(m_userSettings.NpcQuantity));
            }
            catch(TooFewTraitsInCategoryException exception)
            {
                ShowLocalizedErrorMessageIfAllowed(
                    "too_few_traits_in_category", exception.Requested, exception.Category, exception.Available);
            }
        }

        private static List<Replacement> GetReplacements(List<ReplacementSubModel> replacementSubModels, TraitSchema schema)
        {
            List<Replacement> replacements = new List<Replacement>();
            IReadOnlyList<ReplacementSearch> searches = schema.GetReplacementSearches();
            foreach (ReplacementSubModel subModel in replacementSubModels)
            {
                foreach (ReplacementSearch search in searches)
                {
                    if (search.Trait.Name == subModel.OriginalTrait && search.Category.Name == subModel.Category)
                    {
                        Replacement replacement = new Replacement(search.Trait, subModel.CurrentReplacementTrait, search.Category);
                        replacements.Add(replacement);
                        break;
                    }
                }
            }
            return replacements;
        }

        private bool CanExecuteSaveNpcs(object _)
        {
            return m_npcGroup != null;
        }

        private void ExecuteSaveNpcs(object _)
        {
            List<FileContentProvider> contentProviders = new List<FileContentProvider>(m_npcExporters.Count);
            foreach(KeyValuePair<string, INpcExport> exporter in m_npcExporters)
            {
                FileContentProvider fileContentProvider = new FileContentProvider()
                {
                    FileExtensionWithoutDot = exporter.Key,
                    GetContent = () => exporter.Value.Export(m_npcGroup)
                };
                contentProviders.Add(fileContentProvider);
            }

            try
            {
                bool success = m_fileIo.SaveToPickedFile(contentProviders, out string pickedFormat);
                if (success)
                {
                    m_messager.Send(sender: this, message: new Message.SaveNpcs(pickedFormat));
                }
            }
            catch(JsonExportFormatException exception)
            {
                if (m_showErrorMessages)
                {
                    MessageBox.Show(exception.Message);
                }   
            }
        }

        private void SetFileWatcherPath()
        {
            bool doesConfigurationFileExist = File.Exists(m_userSettings.ConfigurationPath);
            if (doesConfigurationFileExist)
            {
                string fileName = Path.GetFileName(ConfigurationPath);
                string path = Path.GetDirectoryName(ConfigurationPath);
                m_configurationFileWatcher.Path = path;
                m_configurationFileWatcher.Filter = fileName;
                m_configurationFileWatcher.EnableRaisingEvents = true;
            }
        }

        private void OnConfigurationChanged(object sender, FileSystemEventArgs e)
        {
            m_configurationHasError = !ParseTraitSchema(out m_traitSchema, out m_replacementSubModels);
            NotifyPropertyChanged("IsConfigurationValid");
        }   

        private void OnConfigurationRenamed(object sender, RenamedEventArgs e)
        {
            NotifyPropertyChanged("IsConfigurationValid");
        }

        private bool ParseTraitSchema(out TraitSchema traitSchema, out List<ReplacementSubModel> replacementSubModels)
        {
            bool isSuccess = false;
            traitSchema = null;
            bool doesConfigurationFileExist = File.Exists(m_userSettings.ConfigurationPath);
            if (doesConfigurationFileExist)
            {
                try
                {
                    string cachedConfigurationPath = m_fileIo.CacheFile(m_userSettings.ConfigurationPath);
                    traitSchema = m_parser.Parse(cachedConfigurationPath);
                    isSuccess = true;
                }
                catch (IOException exception)
                {
                    ShowMessageIfAllowed(exception);
                }
                catch (JsonFormatException exception)
                {
                    if (m_showErrorMessages)
                    {
                        string message = m_localization.GetText("configuration_file_invalid", exception.Path);
                        message += "\n" + exception.Message;

                        MessageBox.Show(message);
                    }
                }
                catch (MismatchedBonusSelectionException exception)
                {
                    ShowLocalizedErrorMessageIfAllowed(
                        "mismatched_bonus_selection", 
                        exception.SourceTraitId.CategoryName,
                        exception.SourceTraitId.TraitName,
                        exception.NotFoundCategoryName);
                }
                catch (MismatchedReplacementTraitException exception)
                {
                    ShowLocalizedErrorMessageIfAllowed(
                        "mismatched_replacement_trait", exception.TraitId.TraitName, exception.TraitId.CategoryName);
                }
                catch (MismatchedReplacementCategoryException exception)
                {
                    ShowLocalizedErrorMessageIfAllowed(
                        "mismatched_replacement_category", exception.TraitId.CategoryName, exception.TraitId.TraitName);
                }
                catch (DuplicateCategoryNameException exception)
                {
                    ShowLocalizedErrorMessageIfAllowed("duplicate_category_name", exception.Category);
                }
                catch (RequirementTraitIdNotFoundException exception)
                {
                    ShowLocalizedErrorMessageIfAllowed(
                        "requirement_trait_not_found", 
                        exception.RequirementCategory,
                        exception.TraitIdNotFound.CategoryName,
                        exception.TraitIdNotFound.TraitName);
                }
                catch (UnknownLogicalOperatorException exception)
                {
                    ShowLocalizedErrorMessageIfAllowed("unknown_operator", exception.RequirementCategory, exception.OperatorName);
                }
                catch (SelfRequiringCategoryException exception)
                {
                    ShowLocalizedErrorMessageIfAllowed("self_requiring_category", exception.Category);
                }
                catch (FormatException exception)
                {
                    ShowMessageIfAllowed(exception);
                }
                catch (ArithmeticException exception)
                {
                    ShowMessageIfAllowed(exception);
                }
            }

            replacementSubModels = MakeReplacementSubModels(m_traitSchema);
            NotifyPropertyChanged("Replacements");
            return isSuccess;
        }

        private void ShowLocalizedErrorMessageIfAllowed(string localizationId, params object[] formatParameters)
        {
            if (m_showErrorMessages)
            {
                string message = m_localization.GetText(localizationId, formatParameters);
                MessageBox.Show(message);
            }
        }

        private void ShowMessageIfAllowed(Exception exception)
        {
            if (m_showErrorMessages)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private static List<ReplacementSubModel> MakeReplacementSubModels(TraitSchema traitSchema)
        {
            List<ReplacementSubModel> replacements = new List<ReplacementSubModel>();
            if (traitSchema == null)
            {
                return replacements;
            }

            IReadOnlyList<ReplacementSearch> replacementSearches = traitSchema.GetReplacementSearches();
            foreach (ReplacementSearch replacementSearch in replacementSearches)
            {
                ReplacementSubModel replacementSubModel = new ReplacementSubModel()
                {
                    Category = replacementSearch.Category.Name,
                    OriginalTrait = replacementSearch.Trait.Name,
                    CurrentReplacementTrait = replacementSearch.Trait.Name,
                    ReplacementTraits = replacementSearch.Category.GetTraitNames()
                };
                replacements.Add(replacementSubModel);
            }

            return replacements;
        }

        private readonly IUserSettings m_userSettings;
        private readonly ILocalFileIO m_fileIo;
        private readonly IMessager m_messager;
        private readonly IConfigurationParser m_parser;
        private readonly ILocalization m_localization;
        private readonly IRandom m_random;
        private readonly Dictionary<string, INpcExport> m_npcExporters = new Dictionary<string, INpcExport>();
        private readonly bool m_showErrorMessages;

        private ICommand m_chooseConfigurationCommand;
        private ICommand m_generateNpcsCommand;
        private ICommand m_saveNpcsCommand;

        private List<ReplacementSubModel> m_replacementSubModels;
        private NpcGroup m_npcGroup;
        private DataTable m_table;
        private TraitSchema m_traitSchema = null;
        private FileSystemWatcher m_configurationFileWatcher;
        private bool m_configurationHasError = false;
    }
}
