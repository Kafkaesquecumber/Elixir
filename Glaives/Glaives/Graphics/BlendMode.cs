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

using System;
using OpenTK.Graphics.OpenGL;

namespace Glaives.Graphics
{
    public struct BlendMode : IEquatable<BlendMode>
    {
        public enum Factor
        {
            /// <summary>(0, 0, 0, 0)</summary>
            Zero = BlendingFactorSrc.Zero,

            /// <summary>(1, 1, 1, 1)</summary>
            One = BlendingFactorSrc.One,

            /// <summary>(src.r, src.g, src.b, src.a)</summary>
            SrcColor = BlendingFactorSrc.SrcColor,

            /// <summary>(1, 1, 1, 1) - (src.r, src.g, src.b, src.a)</summary>
            OneMinusSrcColor = BlendingFactorSrc.OneMinusSrcColor,

            /// <summary>(dst.r, dst.g, dst.b, dst.a)</summary>
            DstColor = BlendingFactorSrc.DstColor,

            /// <summary>(1, 1, 1, 1) - (dst.r, dst.g, dst.b, dst.a)</summary>
            OneMinusDstColor = BlendingFactorSrc.OneMinusDstColor,

            /// <summary>(src.a, src.a, src.a, src.a)</summary>
            SrcAlpha = BlendingFactorSrc.SrcAlpha,

            /// <summary>(1, 1, 1, 1) - (src.a, src.a, src.a, src.a)</summary>
            OneMinusSrcAlpha = BlendingFactorSrc.OneMinusSrcAlpha,

            /// <summary>(dst.a, dst.a, dst.a, dst.a)</summary>
            DstAlpha = BlendingFactorSrc.DstAlpha,

            /// <summary>(1, 1, 1, 1) - (dst.a, dst.a, dst.a, dst.a)</summary>
            OneMinusDstAlpha = BlendingFactorSrc.OneMinusDstAlpha
        }

        ////////////////////////////////////////////////////////////
        /// <summary>
        /// Enumeration of the blending equations
        /// </summary>
        ////////////////////////////////////////////////////////////
        public enum Equation
        {
            /// <summary>Pixel = Src * SrcFactor + Dst * DstFactor</summary>
            Add = BlendEquationMode.FuncAdd,

            /// <summary>Pixel = Src * SrcFactor - Dst * DstFactor</summary>
            Subtract = BlendEquationMode.FuncSubtract
        }

        /// <summary>Blend source and dest according to dest alpha</summary>
        public static readonly BlendMode Alpha = new BlendMode(Factor.SrcAlpha, Factor.OneMinusSrcAlpha, Equation.Add,
                                                               Factor.One, Factor.OneMinusSrcAlpha, Equation.Add);

        /// <summary>Add source to dest</summary>
        public static readonly BlendMode Add = new BlendMode(Factor.SrcAlpha, Factor.One, Equation.Add,
                                                             Factor.One, Factor.One, Equation.Add);

        /// <summary>Multiply source and dest</summary>
        public static readonly BlendMode Multiply = new BlendMode(Factor.DstColor, Factor.Zero);

        /// <summary>Overwrite dest with source</summary>
        public static readonly BlendMode None = new BlendMode(Factor.One, Factor.Zero);

        /// <summary>Source blending factor for the color channels</summary>
        public readonly Factor ColorSrcFactor;

        /// <summary>Destination blending factor for the color channels</summary>
        public readonly Factor ColorDstFactor;

        /// <summary>Blending equation for the color channels</summary>
        public readonly Equation ColorEquation;

        /// <summary>Source blending factor for the alpha channel</summary>
        public readonly Factor AlphaSrcFactor;

        /// <summary>Destination blending factor for the alpha channel</summary>
        public readonly Factor AlphaDstFactor;

        /// <summary>Blending equation for the alpha channel</summary>
        public readonly Equation AlphaEquation;

        /// <summary>
        /// Construct the blend mode given the factors and equation
        /// </summary>
        /// <param name="sourceFactor">Specifies how to compute the source factor for the color and alpha channels.</param>
        /// <param name="destinationFactor">Specifies how to compute the destination factor for the color and alpha channels.</param>
        public BlendMode(Factor sourceFactor, Factor destinationFactor)
            : this(sourceFactor, destinationFactor, Equation.Add)
        {
        }

        /// <summary>
        /// Construct the blend mode given the factors and equation
        /// </summary>
        /// <param name="sourceFactor">Specifies how to compute the source factor for the color and alpha channels.</param>
        /// <param name="destinationFactor">Specifies how to compute the destination factor for the color and alpha channels.</param>
        /// <param name="blendEquation">Specifies how to combine the source and destination colors and alpha.</param>
        public BlendMode(Factor sourceFactor, Factor destinationFactor, Equation blendEquation)
            : this(sourceFactor, destinationFactor, blendEquation, sourceFactor, destinationFactor, blendEquation)
        {
        }

        /// <summary>
        /// Construct the blend mode given the factors and equation
        /// </summary>
        /// <param name="colorSourceFactor">Specifies how to compute the source factor for the color channels.</param>
        /// <param name="colorDestinationFactor">Specifies how to compute the destination factor for the color channels.</param>
        /// <param name="colorBlendEquation">Specifies how to combine the source and destination colors.</param>
        /// <param name="alphaSourceFactor">Specifies how to compute the source factor.</param>
        /// <param name="alphaDestinationFactor">Specifies how to compute the destination factor.</param>
        /// <param name="alphaBlendEquation">Specifies how to combine the source and destination alphas.</param>
        public BlendMode(Factor colorSourceFactor, Factor colorDestinationFactor, Equation colorBlendEquation, Factor alphaSourceFactor, Factor alphaDestinationFactor, Equation alphaBlendEquation)
        {
            ColorSrcFactor = colorSourceFactor;
            ColorDstFactor = colorDestinationFactor;
            ColorEquation = colorBlendEquation;
            AlphaSrcFactor = alphaSourceFactor;
            AlphaDstFactor = alphaDestinationFactor;
            AlphaEquation = alphaBlendEquation;
        }

        /// <summary>
        /// Compare two blend modes and checks if they are equal
        /// </summary>
        /// <returns>Blend Modes are equal</returns>
        public static bool operator ==(BlendMode left, BlendMode right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compare two blend modes and checks if they are not equal
        /// </summary>
        /// <returns>Blend Modes are not equal</returns>
        public static bool operator !=(BlendMode left, BlendMode right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Compare blend mode and object and checks if they are equal
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>Object and blend mode are equal</returns>
        public override bool Equals(object obj)
        {
            return (obj is BlendMode) && obj.Equals(this);
        }

        /// <summary>
        /// Compare two blend modes and checks if they are equal
        /// </summary>
        /// <param name="other">Blend Mode to check</param>
        /// <returns>blend modes are equal</returns>
        public bool Equals(BlendMode other)
        {
            return (ColorSrcFactor == other.ColorSrcFactor) &&
                   (ColorDstFactor == other.ColorDstFactor) &&
                   (ColorEquation == other.ColorEquation) &&
                   (AlphaSrcFactor == other.AlphaSrcFactor) &&
                   (AlphaDstFactor == other.AlphaDstFactor) &&
                   (AlphaEquation == other.AlphaEquation);
        }

        /// <summary>
        /// Provide a integer describing the object
        /// </summary>
        /// <returns>Integer description of the object</returns>
        public override int GetHashCode()
        {
            return ColorSrcFactor.GetHashCode() ^
                   ColorDstFactor.GetHashCode() ^
                   ColorEquation.GetHashCode() ^
                   AlphaSrcFactor.GetHashCode() ^
                   AlphaDstFactor.GetHashCode() ^
                   AlphaEquation.GetHashCode();
        }
    }
}
