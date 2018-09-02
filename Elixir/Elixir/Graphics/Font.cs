using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Elixir.GameFramework;
using OpenTK.Graphics.OpenGL;
using Bitmap = System.Drawing.Bitmap;
using Color = Elixir.GameFramework.Color;
using SharpFont;

namespace Elixir.Graphics
{
    public class Font : LoadableContent, IDisposable
    {
        internal override bool IsDisposed => FontFace == null; 
        
        internal FontCreateOptions CreateOptions;

        public FontFace FontFace { get; private set; }
        
        internal Font(string file, FontCreateOptions createOptions)
        {
            CreateOptions = createOptions;
            
            FontFace = new FontFace(File.OpenRead(file));
            
        }
        
        public void Dispose()
        {
            FontFace = null;
        }
    }
}
