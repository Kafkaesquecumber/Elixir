using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using Elixir.GameFramework;
using Bitmap = System.Drawing.Bitmap;
using Color = Elixir.GameFramework.Color;

namespace Elixir.Graphics
{
    public class Font : LoadableContent, IDisposable
    {
        // TODO: Add support for chinese/japanese/greek/arabic/hebrew/thai and others
        private const string LatinGlyphs = @"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890-=`,'./\;[]|!@#$%^&*()_+{}:""""|<>?~";
        private const string CyrilicGlyhps = @"абвгдеёжзийклмнопрстуфхцчшщъыьэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";

        private static PrivateFontCollection PrivateFontCollection = new PrivateFontCollection();
        private static InstalledFontCollection SystemFontCollection = new InstalledFontCollection();

        internal override bool IsDisposed => NativeFont == null;
        
        private Dictionary<char, FloatRect> _glyphSourceRects = new Dictionary<char, FloatRect>();

        internal System.Drawing.Font NativeFont;
        internal Texture GlyphTexture;
        internal float SpaceWidth;
        internal FontCreateOptions CreateOptions;

        private System.Drawing.Graphics _gfx;
        
        internal Font(string file, FontCreateOptions createOptions)
        {
            CreateOptions = createOptions;
            
            FontFamily family = CreateFontFamily(file);
            FontStyle style = (FontStyle)createOptions.StyleFlags;

            // Remove italic, it is done using vertex skewing in the Text (do keep it in the create options as we need it later)
            style &= ~FontStyle.Italic; 
            NativeFont = new System.Drawing.Font(family, createOptions.Size, style, GraphicsUnit.Millimeter);

            StringFormat stringFormat = StringFormat.GenericTypographic;
            
            using (Bitmap temp = new Bitmap(1, 1))
            {
                _gfx = System.Drawing.Graphics.FromImage(temp);
                _gfx.TextRenderingHint = TextRenderingHint.AntiAlias;
            }
            
            SizeF glyphStringSize = _gfx.MeasureString(LatinGlyphs, NativeFont, SizeF.Empty, stringFormat);
            SpaceWidth =  _gfx.MeasureString(" ", NativeFont).Width;

            double sizeSqrt = Math.Ceiling(Math.Sqrt(glyphStringSize.Width * glyphStringSize.Height)) * 1.2f; // 1.2f = HACK (glyphs don't fit on the texture without it)
            RectangleF layout = new RectangleF(0, 0, (int)sizeSqrt, (int)sizeSqrt);
            
            TextureCreateOptions textureOptions = new TextureCreateOptions(createOptions.FilterMode, TextureWrapMode.ClampToEdge);

            GlyphTexture = new Texture(
                (int) layout.Width,
                (int) layout.Height, Color.Transparant, textureOptions);

            _gfx.Dispose();
            _gfx = System.Drawing.Graphics.FromImage(GlyphTexture.NativeTexture);
            _gfx.TextRenderingHint = TextRenderingHint.AntiAlias;
            
            _gfx.DrawString(LatinGlyphs, NativeFont, Brushes.White, layout, stringFormat);
            
            float offsetX = 0.0f;
            float offsetY = 0.0f;
            int textureWidth = GlyphTexture.NativeTexture.Width;
            
            foreach (char glyphChar in LatinGlyphs)
            {
                SizeF glyphSize = _gfx.MeasureString(glyphChar.ToString(), NativeFont, SizeF.Empty, stringFormat);
                
                if (offsetX >= textureWidth - glyphSize.Width)
                {
                    offsetX = 0;
                    offsetY += NativeFont.Height;
                }

                _glyphSourceRects[glyphChar] = new FloatRect(offsetX, offsetY, glyphSize.Width, glyphSize.Height);

                offsetX += glyphSize.Width;
            }

            GlyphTexture.NativeTexture.Save("Glyphs.png");
            GlyphTexture.LoadTextureGL(textureOptions);
        }

        private FontFamily CreateFontFamily(string file)
        {
            using (PrivateFontCollection tempCollection = new PrivateFontCollection())
            {
                tempCollection.AddFontFile(file);
                using (FontFamily tempFamility = tempCollection.Families[0])
                {
                    // Check if the font is a system font
                    foreach (FontFamily systemFamily in SystemFontCollection.Families)
                    {
                        if (systemFamily.Name == tempFamility.Name)
                        {
                            return systemFamily;
                        }
                    }

                    // Check if the font is loaded previously
                    foreach (FontFamily fontFamily in PrivateFontCollection.Families)
                    {
                        if (fontFamily.Name == tempFamility.Name)
                        {
                            return fontFamily;
                        }
                    }

                    PrivateFontCollection.AddFontFile(file);
                    return PrivateFontCollection.Families[PrivateFontCollection.Families.Length - 1];
                }
            }
        }

        internal FloatRect GetGlyphSourceRect(char glyph)
        {
            if (glyph == ' ')
            {
                return new FloatRect(0, 0, SpaceWidth, 0);
            }
            else if (glyph == '\t')
            {
                return new FloatRect(0, 0, SpaceWidth * 4, 0);
            }

            if (!_glyphSourceRects.ContainsKey(glyph))
            {
                Engine.Get.Debug.Warning($"Character '{glyph}' is not yet supported for text rendering");
                return _glyphSourceRects['?'];
            }
            return _glyphSourceRects[glyph];
        }

        public void Dispose()
        {
            NativeFont?.Dispose();
            GlyphTexture?.Dispose();
            _gfx?.Dispose();
            NativeFont = null;
        }
    }
}
