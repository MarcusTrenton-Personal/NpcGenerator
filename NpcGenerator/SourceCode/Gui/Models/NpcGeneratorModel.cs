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

using Microsoft.Win32;
using Services;
using Services.Message;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using WpfServices;

namespace NpcGenerator
{
    public class NpcGeneratorModel : BaseModel, INpcGeneratorModel
    {
        public NpcGeneratorModel(
            IUserSettings userSettings,
            IAppSettings appSettings,
            IMessager messager,
            ILocalFileIO fileIo,
            IConfigurationParser parser,
            Dictionary<string, INpcExport> npcExporters,
            ILocalization localization,
            IRandom random,
            bool showErrorMessages,
            bool forceFailNpcGeneration)
        {
            m_userSettings = userSettings;
            m_appSettings = appSettings;
            m_messager = messager;
            m_fileIo = fileIo;
            m_parser = parser;
            m_npcExporters = npcExporters;
            m_localization = localization;
            m_random = random;
            m_showErrorMessages = showErrorMessages;
            m_forceFailNpcGeneration = forceFailNpcGeneration;
            m_configurationHasError = false;

            UpdateUserSettingsWithDefaults();

            CreateFileWatcher();

            //Deliberately do parse the initial schema, as there is no way to show an error message during boot-up.
            //Instead evaluate lazily.
        }

        ~NpcGeneratorModel()
        {
            TearDownFileWatcher();
        }

        private void UpdateUserSettingsWithDefaults()
        {
            if (m_userSettings.ConfigurationPath == UserSettings.DEFAULT_CONFIGURATION_PATH && 
                m_appSettings.DefaultConfigurationRelativePath != null)
            {
                m_userSettings.ConfigurationPath = PathHelper.FullPathOf(m_appSettings.DefaultConfigurationRelativePath);
            }
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
            if (m_traitSchema is null && !m_configurationHasError)
            {
                ParseTraitSchema(out m_traitSchema, out m_replacementSubModels);
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

                ParseTraitSchema(out m_traitSchema, out m_replacementSubModels);
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
            if (m_traitSchema is null)
            {
                return;
            }

            List<Replacement> replacements = GetReplacements(m_replacementSubModels, m_traitSchema);
            try
            {
                m_npcGroup = NpcFactory.Create(m_traitSchema, m_userSettings.NpcQuantity, replacements, m_random);

                UpdateNpcTable();

                m_messager.Send(sender: this, message: new Message.GenerateNpcs(m_userSettings.NpcQuantity));

                ValidateNpcs(replacements);
            }
            catch(TooFewTraitsInCategoryException exception)
            {
                ShowLocalizedErrorMessageIfAllowed(
                    "too_few_traits_in_category", exception.Requested, exception.Category, exception.Available);
            }
        }

        private void UpdateNpcTable()
        {
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
        }

        private void ValidateNpcs(List<Replacement> replacements)
        {
            bool areValid = NpcFactory.AreNpcsValid(
                    m_npcGroup, m_traitSchema, replacements, out Dictionary<Npc, List<NpcSchemaViolation>> violations);
            if (m_forceFailNpcGeneration)
            {
                NpcSchemaViolation fakeViolation = new NpcSchemaViolation("Fake Failure Test", NpcSchemaViolation.Reason.CategoryNotFoundInSchema);
                violations[new Npc()] = new List<NpcSchemaViolation>() { fakeViolation };
                areValid = false;
            }
            if (!areValid)
            {
                StringBuilder message = new StringBuilder();
                
                message.AppendLine(m_localization.GetText("npcs_generated_incorrectly") + "\n");
                string errorBody = NpcErrorBody(violations);
                message.Append(errorBody);
                message.AppendLine();
                message.Append(m_localization.GetText("support_email_call_to_action", m_appSettings.SupportEmail));

                m_messager.Send(sender: this, message: new Message.InvalidNpcs(violations));

                if (m_showErrorMessages)
                {
                    string errorTitle = m_localization.GetText("error", m_appSettings.SupportEmail);
                    MessageBoxResult result = MessageBox.Show(message.ToString(), errorTitle, MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        SendErrorEmail(errorTitle, errorBody, replacements);
                    }                    
                }
            }
        }

        private string NpcErrorBody(Dictionary<Npc, List<NpcSchemaViolation>> violations)
        {
            StringBuilder errorBody = new StringBuilder();
            foreach (List<NpcSchemaViolation> violationList in violations.Values)
            {
                foreach (NpcSchemaViolation violation in violationList)
                {
                    string errorMessage = violation.Violation switch
                    {
                        NpcSchemaViolation.Reason.HasTraitInLockedCategory =>
                            m_localization.GetText("npc_error_trait_in_locked_category", violation.Category, violation.Trait),
                        NpcSchemaViolation.Reason.TooFewTraitsInCategory =>
                            m_localization.GetText("npc_error_too_few_traits_in_category", violation.Category),
                        NpcSchemaViolation.Reason.TooManyTraitsInCategory =>
                            m_localization.GetText("npc_error_too_many_traits_in_category", violation.Category),
                        NpcSchemaViolation.Reason.TraitNotFoundInSchema =>
                            m_localization.GetText("npc_error_trait_not_found_in_schema", violation.Trait, violation.Category),
                        NpcSchemaViolation.Reason.CategoryNotFoundInSchema =>
                            m_localization.GetText("npc_error_category_not_found_in_schema", violation.Category),
                        NpcSchemaViolation.Reason.TraitIsIncorrectlyHidden =>
                            m_localization.GetText("npc_error_trait_incorrectly_hidden", violation.Category, violation.Trait),
                        NpcSchemaViolation.Reason.TraitIsIncorrectlyNotHidden =>
                            m_localization.GetText("npc_error_trait_incorrectly_not_hidden", violation.Category, violation.Trait),
                        NpcSchemaViolation.Reason.UnusedReplacement =>
                            m_localization.GetText("npc_error_unused_replacement", violation.Category, violation.Trait),
                        _ => throw new ArgumentException("Unknown violation type " + violation.Violation.ToString()),
                    };
                    errorBody.AppendLine(errorMessage);
                }
            }
            return errorBody.ToString();
        }

        private void SendErrorEmail(string errorTitle, string errorBody, List<Replacement> replacements)
        {
            StringBuilder actionBuilder = new StringBuilder();
            actionBuilder.Append("mailto:" + m_appSettings.SupportEmail);
            actionBuilder.Append("?subject=" + m_localization.GetText("error_email_subject"));
            actionBuilder.Append("&body=" + m_localization.GetText("error_email_body_start"));

            actionBuilder.AppendLine(ErrorSectionTitle(errorTitle));
            actionBuilder.AppendLine(errorBody);
            actionBuilder.AppendLine();

            string npcsTitle = m_localization.GetText("npcs");
            actionBuilder.AppendLine(ErrorSectionTitle(npcsTitle));
            string npcsText = m_npcExporters[NpcToJson.FileExtensionWithoutDotStatic].Export(m_npcGroup);
            actionBuilder.AppendLine(npcsText);
            actionBuilder.AppendLine();

            string configurationTitle = m_localization.GetText("choose_configuration_file_label");
            actionBuilder.AppendLine(ErrorSectionTitle(configurationTitle));
            string configurationText = File.ReadAllText(m_userSettings.ConfigurationPath);
            actionBuilder.AppendLine(configurationText);
            actionBuilder.AppendLine();

            string replacementTitle = m_localization.GetText("trait_replacement");
            actionBuilder.AppendLine(ErrorSectionTitle(replacementTitle));
            foreach (Replacement replacement in replacements)
            {
                string replacementText = m_localization.GetText(
                    "trait_replacement_description",
                    replacement.OriginalTrait.Name,
                    replacement.ReplacementTraitName,
                    replacement.Category.Name);
                actionBuilder.AppendLine(replacementText);
            }
            actionBuilder.AppendLine();

            string npcNumberLabelText = m_localization.GetText("npc_quantity_label");
            actionBuilder.AppendLine(ErrorSectionTitle(npcNumberLabelText) + " " + NpcQuantity);

            UriHelper.StartEmail(new Uri(actionBuilder.ToString()));
        }

        private static string ErrorSectionTitle(string titleName)
        {
            const string SEPERATOR = "----------";
            string title = SEPERATOR + titleName + SEPERATOR;
            return title;
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
            ParseTraitSchema(out m_traitSchema, out m_replacementSubModels);
            NotifyPropertyChanged("IsConfigurationValid");
        }   

        private void OnConfigurationRenamed(object sender, RenamedEventArgs e)
        {
            NotifyPropertyChanged("IsConfigurationValid");
        }

        private void ParseTraitSchema(out TraitSchema traitSchema, out List<ReplacementSubModel> replacementSubModels)
        {
            bool originallyHadError = m_configurationHasError;
            m_configurationHasError = true;
            traitSchema = null;
            bool doesConfigurationFileExist = File.Exists(m_userSettings.ConfigurationPath);
            if (doesConfigurationFileExist)
            {
                try
                {
                    string cachedConfigurationPath = m_fileIo.CacheFile(m_userSettings.ConfigurationPath);
                    traitSchema = m_parser.Parse(cachedConfigurationPath);
                    m_configurationHasError = false;
                }
                catch (EmptyFileException exception)
                {
                    ShowLocalizedErrorMessageIfAllowed("empty_file", exception.FileName);
                }
                catch (CategoryWeightMismatchException)
                {
                    ShowLocalizedErrorMessageIfAllowed("category_weight_mismatch");
                }
                catch (EmptyCategoryNameException)
                {
                    ShowLocalizedErrorMessageIfAllowed("empty_category_name");
                }
                catch (TraitMissingCategoryException exception)
                {
                    ShowLocalizedErrorMessageIfAllowed("trait_has_no_category", exception.TraitName);
                }
                catch (MissingWeightException exception)
                {
                    ShowLocalizedErrorMessageIfAllowed("trait_has_no_weight", exception.TraitId.CategoryName, exception.TraitId.TraitName);
                }
                catch (WeightIsNotWholeNumberException exception)
                {
                    ShowLocalizedErrorMessageIfAllowed(
                        "trait_weight_is_not_whole_number", 
                        exception.TraitId.CategoryName, 
                        exception.TraitId.TraitName, 
                        exception.InvalidWeight);
                }
                catch (JsonFormatException exception)
                {
                    if (m_showErrorMessages)
                    {
                        string message = m_localization.GetText("configuration_file_invalid", m_userSettings.ConfigurationPath);
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
                catch (MissingReplacementTraitException exception)
                {
                    ShowLocalizedErrorMessageIfAllowed(
                        "mismatched_replacement_trait", exception.TraitId.TraitName, exception.TraitId.CategoryName);
                }
                catch (MissingReplacementCategoryException exception)
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
                catch (SelfRequiringTraitException exception)
                {
                    ShowLocalizedErrorMessageIfAllowed("self_requiring_trait", exception.TraitId.TraitName, exception.TraitId.CategoryName);
                }
                catch (TooFewTraitsInCategoryException exception)
                {
                    ShowLocalizedErrorMessageIfAllowed(
                        "too_few_traits_in_category", exception.Requested, exception.Category, exception.Available);
                }
                catch (CircularRequirementsException exception)
                {
                    if (m_showErrorMessages)
                    {
                        StringBuilder builder = new StringBuilder();
                        string header = m_localization.GetText("circular_requirements");
                        builder.Append(header);
                        foreach (TraitSchema.Dependency dependency in exception.Cycle)
                        {
                            builder.Append('\n');
                            string localizationId = dependency.DependencyType == TraitSchema.Dependency.Type.Requirement ?
                                "circular_requirement_link" : "circular_requirement_link_bonus_selection";
                            string link = m_localization.GetText(localizationId, dependency.OriginalCategory, dependency.DependentCategory);
                            builder.Append(link);
                        }

                        MessageBox.Show(builder.ToString());
                    }
                }
                catch (IOException exception)
                {
                    ShowMessageIfAllowed(exception);
                }
            }

            replacementSubModels = MakeReplacementSubModels(m_traitSchema);
            NotifyPropertyChanged("Replacements");
            if (originallyHadError != m_configurationHasError)
            {
                NotifyPropertyChanged("IsConfigurationValid");
            }
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
            if (traitSchema is null)
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
        private readonly IAppSettings m_appSettings;
        private readonly ILocalFileIO m_fileIo;
        private readonly IMessager m_messager;
        private readonly IConfigurationParser m_parser;
        private readonly ILocalization m_localization;
        private readonly IRandom m_random;
        private readonly Dictionary<string, INpcExport> m_npcExporters = new Dictionary<string, INpcExport>();
        private readonly bool m_showErrorMessages;
        private readonly bool m_forceFailNpcGeneration = false;

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
