// MIT License
// 
// Copyright(c) 2018 Glaives Game Engine.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.IO;
using System.Xml.Serialization;

namespace Glaives.Core.Configuration
{
    /// <summary>
    /// Root class for all engine settings
    /// </summary>
    [Serializable]
    public class Settings 
    {
        public static Settings Default => new Settings();

        public GeneralSettings General;
        public VideoSettings Video;
        public InputSettings Input;
        public ImGuiSettings ImGui;

        public Settings()
        {
            General = new GeneralSettings();
            Video = new VideoSettings();
            Input = new InputSettings();
            ImGui = new ImGuiSettings();
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