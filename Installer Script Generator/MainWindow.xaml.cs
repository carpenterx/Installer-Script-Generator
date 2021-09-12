using Installer_Script_Generator.Models;
using Installer_Script_Generator.Properties;
using Installer_Script_Generator.Windows;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace Installer_Script_Generator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private const string APP_RESOURCES_NAMESPACE = "Installer_Script_Generator.Resources.";
        private const string TEMPLATE_NAME = "Script Template.iss";
        private const string FILE_ASSOCIATION_TEMPLATE_NAME = "Template with file association.iss";
        private string basicTemplate;
        private string fileAssociationTemplate;

        private const string APP_NAME_STRING = "APP_NAME";
        private const string RELEASE_PATH_STRING = "RELEASE_PATH";
        private const string APP_VERSION_STRING = "APP_VERSION";
        private const string EXTENSION_STRING = "[EXTENSION]";
        private const string FILE_TYPE_STRING = "[FILE_TYPE]";

        private const string CONFIG_FILE_NAME = "configurations.json";
        private const string APPLICATION_FOLDER = "Installer Script Generator";

        private ObservableCollection<Configuration> configurations = new();

        public MainWindow()
        {
            InitializeComponent();

            basicTemplate = ReadResource(TEMPLATE_NAME);
            fileAssociationTemplate = ReadResource(FILE_ASSOCIATION_TEMPLATE_NAME);

            LoadConfigurationsHistory();

            configurationsListView.ItemsSource = configurations;
        }

        private void LoadConfigurationsHistory()
        {
            string configurationsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), APPLICATION_FOLDER, CONFIG_FILE_NAME);
            
            if (File.Exists(configurationsPath))
            {
                configurations = JsonConvert.DeserializeObject<ObservableCollection<Configuration>>(File.ReadAllText(configurationsPath));
            }
        }

        private string ReadResource(string fileName)
        {
            string resourceData = "";
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(APP_RESOURCES_NAMESPACE + fileName))
            {
                TextReader tr = new StreamReader(stream);
                resourceData = tr.ReadToEnd();
            }
            return resourceData;
        }

        private void BrowseToReleaseClick(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new();
            dialog.IsFolderPicker = true;
            dialog.Title = "Select Release Folder";
            if (Directory.Exists(Settings.Default.ReleasesFolder))
            {
                dialog.InitialDirectory = Settings.Default.ReleasesFolder;
            }
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                pathLabel.Content = dialog.FileName;
            }

            TraversalRequest request = new(FocusNavigationDirection.Next);
            request.Wrapped = true;
            (sender as Button).MoveFocus(request);
        }

        private void DropFolder(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] folders = (string[])e.Data.GetData(DataFormats.FileDrop);

                pathLabel.Content = folders[0];
            }
        }

        private void SaveScriptClick(object sender, RoutedEventArgs e)
        {
            string directoryPath = pathLabel.Content?.ToString();
            if (Directory.Exists(directoryPath))
            {
                string outputScript;
                if (extensionTxt.Text != "")
                {
                    outputScript = GenerateInstallerScript(fileAssociationTemplate, directoryPath);
                }
                else
                {
                    outputScript = GenerateInstallerScript(basicTemplate, directoryPath);
                }

                SaveFileDialog dlg = new();
                dlg.Title = $"Save {Settings.Default.ScriptFile}";
                if (Directory.Exists(Settings.Default.ScriptsFolder))
                {
                    //dlg.InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "Releases", "Installer Scripts");

                    dlg.InitialDirectory = Settings.Default.ScriptsFolder;
                }
                dlg.FileName = $"{Path.GetFileName(directoryPath)} {versionTxt.Text}";
                dlg.DefaultExt = Settings.Default.ScriptExtension;
                dlg.Filter = $"{Settings.Default.ScriptFile} ({Settings.Default.ScriptExtension})|*{Settings.Default.ScriptExtension}";

                if (dlg.ShowDialog() == true)
                {
                    Configuration configuration = new(pathLabel.Content.ToString(), versionTxt.Text, extensionTxt.Text, fileTypeTxt.Text);

                    configurations.Add(configuration);

                    File.WriteAllText(dlg.FileName, outputScript);
                }
            }
        }

        private string GenerateInstallerScript(string template, string directoryPath)
        {
            string directoryName = Path.GetFileName(directoryPath);
            string outputScript = template.Replace(RELEASE_PATH_STRING, directoryPath);
            outputScript = outputScript.Replace(APP_NAME_STRING, directoryName);
            outputScript = outputScript.Replace(APP_VERSION_STRING, versionTxt.Text);
            outputScript = outputScript.Replace(EXTENSION_STRING, extensionTxt.Text);
            outputScript = outputScript.Replace(FILE_TYPE_STRING, fileTypeTxt.Text);

            return outputScript;
        }

        private void AddConfiguration(object sender, RoutedEventArgs e)
        {
            if (pathLabel.Content != null)
            {
                Configuration configuration = new(pathLabel.Content.ToString(), versionTxt.Text, extensionTxt.Text, fileTypeTxt.Text);

                configurations.Add(configuration);
            }
        }

        private void SaveData(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveConfigurationsHistory();
            Settings.Default.Save();
        }

        private void SaveConfigurationsHistory()
        {
            string applicationDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), APPLICATION_FOLDER);
            if (!Directory.Exists(applicationDirectory))
            {
                Directory.CreateDirectory(applicationDirectory);
            }
            string json = JsonConvert.SerializeObject(configurations, Formatting.Indented);
            string configurationsPath = Path.Combine(applicationDirectory, CONFIG_FILE_NAME);
            File.WriteAllText(configurationsPath, json);
        }

        private void LoadConfiguration(object sender, RoutedEventArgs e)
        {
            if (configurationsListView.SelectedItem is Configuration selectedConfiguration)
            {
                pathLabel.Content = selectedConfiguration.ReleasePath;
                versionTxt.Text = selectedConfiguration.Version;
                extensionTxt.Text = selectedConfiguration.FileExtension;
                fileTypeTxt.Text = selectedConfiguration.FileType;
            }
        }

        private void RemoveConfiguration(object sender, RoutedEventArgs e)
        {
            if (configurationsListView.SelectedItem is Configuration selectedConfiguration)
            {
                configurations.Remove(selectedConfiguration);
            }
        }

        private void OpenScriptsFolder(object sender, RoutedEventArgs e)
        {
            //Process.Start("explorer.exe", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "Releases", "Installer Scripts"));
            if(Directory.Exists(Settings.Default.ScriptsFolder))
            {
                Process.Start("explorer.exe", Settings.Default.ScriptsFolder);
            }

            TraversalRequest request = new(FocusNavigationDirection.Previous);
            request.Wrapped = true;
            (sender as Button).MoveFocus(request);
        }

        private void SettingsClick(object sender, RoutedEventArgs e)
        {
            ShowSettings();
        }

        private void ShowSettingsKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                ShowSettings();
            }
        }

        private void ShowSettings()
        {
            SettingsWindow settingsWindow = new();
            settingsWindow.Owner = this;
            if (settingsWindow.ShowDialog() == true)
            {

            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            TestSettings();
        }

        private void TestSettings()
        {
            if (!Directory.Exists(Settings.Default.ReleasesFolder) || !Directory.Exists(Settings.Default.ScriptsFolder))
            {
                ShowSettings();
            }
        }
    }
}
