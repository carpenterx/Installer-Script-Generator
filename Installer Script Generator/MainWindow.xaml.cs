using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.IO;
using System.Reflection;
using System.Windows;
using Path = System.IO.Path;

namespace Installer_Script_Generator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
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

        public MainWindow()
        {
            InitializeComponent();

            basicTemplate = ReadResource(TEMPLATE_NAME);
            fileAssociationTemplate = ReadResource(FILE_ASSOCIATION_TEMPLATE_NAME);
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
            CommonOpenFileDialog dialog = new()
            {
                IsFolderPicker = true,
                InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "Releases")
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                pathLabel.Content = dialog.FileName;
            }
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
            string directoryPath = pathLabel.Content.ToString();
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
                dlg.Title = "Save Iss Script";
                dlg.InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "Releases", "Installer Scripts");
                dlg.FileName = Path.GetFileName(directoryPath);
                dlg.DefaultExt = ".iss";
                dlg.Filter = "ISS Script (.iss)|*.iss";

                if (dlg.ShowDialog() == true)
                {
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
            //Clipboard.SetText(outputScript);

            return outputScript;
        }
    }
}
