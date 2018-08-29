using System;
using System.IO;
using System.Xml.Serialization;

namespace Elixir.Configuration
{
    [Serializable]
    public class Settings 
    {
        public static Settings Default => new Settings();

        public GeneralSettings General;
        public VideoSettings Video;
        public InputSettings Input;

        public Settings()
        {
            General = new GeneralSettings();
            Video = new VideoSettings();
            Input = new InputSettings();
        }
        
        /// <summary>
        /// Serialize the settings to an XML file
        /// </summary>
        /// <param name="settingsPath"></param>
        public void Save(string settingsPath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
            using (FileStream stream = new FileStream($"{settingsPath}/{nameof(Settings)}.xml", FileMode.Create))
            {
                serializer.Serialize(stream, this);
            }
        }

        /// <summary>
        /// Load the settings from an XML file
        /// </summary>
        /// <param name="settingsFile">The XML file which contains the settings</param>
        public static Settings FromFile(string settingsFile)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
            using (FileStream stream = new FileStream(settingsFile, FileMode.Open))
            {
                return (Settings)serializer.Deserialize(stream);
            }
        }
    }
}