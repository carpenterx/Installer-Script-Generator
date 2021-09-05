namespace Installer_Script_Generator.Models
{
    public class Configuration
    {
        public string ReleasePath { get; set; }
        public string Version { get; set; }
        public string FileExtension { get; set; }
        public string FileType { get; set; }

        public Configuration(string releasePath, string version, string fileExtension, string fileType)
        {
            ReleasePath = releasePath;
            Version = version;
            FileExtension = fileExtension;
            FileType = fileType;
        }
    }
}
