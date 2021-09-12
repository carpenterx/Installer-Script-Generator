using MahApps.Metro.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows;
using System.Windows.Controls;

namespace Installer_Script_Generator.Windows
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : MetroWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void DropFolder(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] folders = (string[])e.Data.GetData(DataFormats.FileDrop);

                (sender as Label).Content = folders[0];
            }
        }

        private void BrowseToReleases(object sender, RoutedEventArgs e)
        {
            BrowseToFolder("Releases", releasesLabel);
        }

        private void BrowseToScripts(object sender, RoutedEventArgs e)
        {
            BrowseToFolder("Scripts", scriptsLabel);
        }

        private void BrowseToFolder(string folderName, Label label)
        {
            CommonOpenFileDialog dialog = new()
            {
                IsFolderPicker = true,
                Title = $"Select {folderName} Folder"
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                label.Content = dialog.FileName;
            }
        }
    }
}
