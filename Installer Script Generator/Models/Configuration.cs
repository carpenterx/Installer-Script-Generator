using System.Globalization;
using System.Windows.Data;
using System;
using System.IO;

namespace Installer_Script_Generator.Models
{
    public class Configuration
    {
        public string ConfigurationPath { get; set; }
        public string ReleasePath { get; set; }
        public string Version { get; set; }
        public string FileExtension { get; set; }
        public string FileType { get; set; }

        public Configuration(string configurationPath, string releasePath, string version, string fileExtension = "", string fileType = "")
        {
            ConfigurationPath = configurationPath;
            ReleasePath = releasePath;
            Version = version;
            FileExtension = fileExtension;
            FileType = fileType;
        }
    }

    public class PathToFolderNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string folderPath = (string)value;
            return Path.GetFileName(folderPath);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
