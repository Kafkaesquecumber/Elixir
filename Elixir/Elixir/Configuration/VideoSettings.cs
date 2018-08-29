
using System;
using System.Reflection;
using Elixir.GameFramework;
using Elixir.Graphics;
using Elixir.Internal;

namespace Elixir.Configuration
{
    [Serializable]
    public class VideoSettings
    {
        private int _width = 800;
        /// <summary>
        /// The width of the window
        /// </summary>
        public int Width
        {
            get => _width;
            set
            {
                if (value != _width)
                {
                    _width = value;
                    if (Engine.Get.Initialized)
                    {
                        Engine.Get.Window.Size = new IntVector2(value, Height);
                    }
                }
            }
        }

        private int _height = 600;
        /// <summary>
        /// The height of the window
        /// </summary>
        public int Height
        {
            get => _height;
            set
            {
                if (_height != value)
                {
                    _height = value;
                    if (Engine.Get.Initialized)
                    {
                        Engine.Get.Window.Size = new IntVector2(Width, value);
                    }
                }
            }
        }

        private string _title;
        /// <summary>
        /// The title of the window
        /// </summary>
        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    if (Engine.Get.Initialized)
                    {
                        Engine.Get.Window.Title = value;
                    }
                }
            }
        }

        private bool _vsync;
        /// <summary>
        /// Whether or not the window should use vertical synchronization
        /// </summary>
        public bool VSync
        {
            get => _vsync;
            set
            {
                if (_vsync != value)
                {
                    _vsync = value;
                    if (Engine.Get.Initialized)
                    {
                        Engine.Get.Window.VSync = value;
                    }
                }
            }
        }
        
        public VideoSettings()
        {
            Title = Assembly.GetEntryAssembly().GetName().Name;
        }
    }
}
