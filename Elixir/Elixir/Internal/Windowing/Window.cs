using System;
using Elixir.GameFramework;
using Elixir.Graphics;
using Elixir.Internal.Interface;
using Elixir.Internal.Graphics;

namespace Elixir.Internal.Windowing
{
    internal sealed class Window
    {
        internal IntPtr Handle { get; private set; }
        
        private IntVector2 _size;
        internal IntVector2 Size
        {
            get => _size;
            set
            {
                Interface.ResizeWindow(value.X, value.Y);
                Interface.GetWindowSize(out int newWidth, out int newHeight);
                _size = new IntVector2(newWidth, newHeight);
            }
        }
        
        private string _title;
        internal string Title
        {
            get => _title;
            set
            {
                Interface.SetWindowTitle(value);
                _title = value;
            }
        }

        private bool _vSync;
        internal bool VSync
        {
            get => _vSync;
            set
            {
                if (_vSync != value)
                {
                    _vSync = value;
                    Interface.SetVSyncEnabled(value);
                }
            }
        }

        internal Viewport Viewport { get; private set; }
        
        internal string OpenGlVersion => Interface.GetCurrentGraphicsApiVersion();
        internal readonly WindowInterface Interface;
        
        internal Window(IntVector2 size, string title)
        {
            Interface = (WindowInterface)InterfaceManager.CreateInterface(InterfaceType.Window);
            _size = size;   // set private member on purpose
            _title = title; // set private member on purpose
            
            Viewport = new Viewport();

            Handle = Interface.OpenWindow(Size, title);
            Interface.SetVSyncEnabled(VSync);
        }
        
        internal void Center()
        {
            Interface.Center();
        }        
    }
}
