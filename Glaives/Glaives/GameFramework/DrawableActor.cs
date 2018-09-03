// MIT License
// 
// Copyright(c) 2018 
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

using Glaives.Graphics;
using Glaives.Internal;
using Glaives.Internal.Graphics;

namespace Glaives.GameFramework
{
    /// <inheritdoc />
    /// <summary>
    /// An actor that contains geometry that can be drawn to a render target
    /// </summary>
    public abstract class DrawableActor : Actor
    {
        /// <summary>
        /// How transparent this actor is (between 0 and 1)
        /// </summary>
        public float Opacity
        {
            get => Color.A;
            set
            {
                if (Color.A != value)
                {
                    value = Utils.MathEx.Clamp(value, 0.0f, 1.0f);
                    Color = new Color(Color.R, Color.G, Color.B, value);
                }
            }
        }

        /// <summary>
        /// The RGBA color of this actor
        /// </summary>
        public Color Color { get; set; } = Color.White;

        /// <summary>
        /// Whether or not to flip the actor horizontally
        /// </summary>
        public bool FlipX { get; set; }

        /// <summary>
        /// Whether or not to flip the actor vertically
        /// </summary>
        public bool FlipY { get; set; }
    
        /// <summary>
        /// <para>The bounds of this drawable actor in local space</para>
        /// <para>Ignores the transformations (translation, rotation ,scale) that are applied</para>
        /// </summary>
        public abstract FloatRect LocalBounds { get; }

        private Vector2 _origin = new Vector2(0.5f, 0.5f);
        /// <summary>
        /// The normalized origin of the actor (between (0, 0) and (1, 1))
        /// </summary>
        public Vector2 Origin
        {
            get => _origin;
            set
            {
                if (value.X < 0.0f || value.X > 1.0f || value.Y < 0.0f || value.Y > 1.0f)
                {
                    throw new GlaivesException("Origin values must be between 0.0f and 1.0f");
                }

                if (_origin != value)
                {
                    _origin = value;
                    WorldMatrixIsDirty = true;
                    InverseWorldMatrixIsDirty = true;
                }
            }
        }
        
        /// <summary>
        /// The blending modes to use for this drawable actor
        /// </summary>
        public BlendMode BlendMode
        {
            get => RenderProgram.BlendMode;
            set
            {
                if (RenderProgram.BlendMode != value)
                {
                    RenderProgram newRenderProgram = new RenderProgram(value, RenderProgram.Texture, RenderProgram.Shader, RenderProgram.DrawLayer);
                    RenderProgram = newRenderProgram;
                    RenderProgramIsDirty = true;
                }
            }
        }

        /// <summary>
        /// The shader to use for this drawable actor
        /// </summary>
        public Shader Shader
        {
            get => RenderProgram.Shader;
            set
            {
                if (RenderProgram.Shader != value) 
                {
                    RenderProgram newRenderProgram = new RenderProgram(RenderProgram.BlendMode, RenderProgram.Texture, value, RenderProgram.DrawLayer);
                    RenderProgram = newRenderProgram;
                    RenderProgramIsDirty = true;
                }
            }
        }

        /// <summary>
        /// The texture of this drawable actor
        /// </summary>
        public Texture Texture
        {
            get => RenderProgram.Texture;
            set
            {
                if (RenderProgram.Texture != value) 
                {
                    RenderProgram newRenderProgram = new RenderProgram(RenderProgram.BlendMode, value, RenderProgram.Shader, RenderProgram.DrawLayer);
                    RenderProgram = newRenderProgram;
                    RenderProgramIsDirty = true;
                }
            }
        }

        /// <summary>
        /// The draw layers manage the order at which the drawable actors are drawn (higher layers will overlap lower layers)
        /// </summary>
        public int DrawLayer
        {
            get => RenderProgram.DrawLayer;
            set
            {
                if (RenderProgram.DrawLayer != value) 
                {
                    RenderProgram newRenderProgram = new RenderProgram(RenderProgram.BlendMode, RenderProgram.Texture, RenderProgram.Shader, value);
                    RenderProgram = newRenderProgram;
                    RenderProgramIsDirty = true;
                }
            }
        }
        
        /// <summary>
        /// <para>The global bounds of this drawable actor</para>
        /// <para>Takes into account the transformations (translation, rotation, scale) that are applied</para>
        /// </summary>
        public FloatRect GlobalBounds => WorldMatrix.TransformRect(LocalBounds);
        
        /// <summary>
        /// Internal use for the Actor base class only
        /// </summary>
        protected internal override FloatRect LocalBoundsInternal => LocalBounds;

        /// <summary>
        /// Internal use for the Actor base class only
        /// </summary>
        protected internal override Vector2 OriginInternal => Origin;
        
        internal RenderProgram RenderProgram = RenderProgram.Default;
        internal GeometryBatch Batch;
        internal bool RenderProgramIsDirty = true;
        
        /// <summary>
        /// <para>Defines the geometry of the drawable actor</para>
        /// </summary>
        /// <returns></returns>
        protected internal abstract Vertex[] GetVertices();
    }
}
