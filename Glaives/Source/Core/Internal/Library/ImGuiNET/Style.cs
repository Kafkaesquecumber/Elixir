
using Glaives.Core;
using Glaives.Core.Graphics;

namespace ImGuiNET
{
    public unsafe class Style
    {
        public readonly NativeStyle* NativePtr;

        public Style(NativeStyle* style)
        {
            NativePtr = style;
        }

        /// <summary>
        /// Global alpha applies to everything in ImGui.
        /// </summary>
        public float Alpha
        {
            get { return NativePtr->Alpha; }
            set { NativePtr->Alpha = value; }
        }

        /// <summary>
        /// Padding within a window.
        /// </summary>
        public Vector2 WindowPadding
        {
            get { return new Vector2(NativePtr->WindowPadding.X, NativePtr->WindowPadding.Y); }
            set { NativePtr->WindowPadding = value.ToNumericsVector2(); }
        }

        /// <summary>
        /// Minimum window size.
        /// </summary>
        public Vector2 WindowMinSize
        {
            get { return new Vector2(NativePtr->WindowMinSize.X, NativePtr->WindowMinSize.Y); }
            set { NativePtr->WindowMinSize = value.ToNumericsVector2(); }
        }

        /// <summary>
        /// Radius of window corners rounding. Set to 0.0f to have rectangular windows.
        /// </summary>
        public float WindowRounding
        {
            get { return NativePtr->WindowRounding; }
            set { NativePtr->WindowRounding = value; }
        }

        /// <summary>
        /// Alignment for title bar text.
        /// </summary>
        public Vector2 WindowTitleAlign
        {
            get { return new Vector2(NativePtr->WindowTitleAlign.X, NativePtr->WindowTitleAlign.Y); }
            set { NativePtr->WindowTitleAlign = value.ToNumericsVector2(); }
        }

        /// <summary>
        /// Radius of child window corners rounding. Set to 0.0f to have rectangular windows.
        /// </summary>
        public float ChildWindowRounding
        {
            get { return NativePtr->ChildRounding; }
            set { NativePtr->ChildRounding = value; }
        }

        /// <summary>
        /// Padding within a framed rectangle (used by most widgets).
        /// </summary>
        public Vector2 FramePadding
        {
            get { return new Vector2(NativePtr->FramePadding.X, NativePtr->FramePadding.Y); }
            set { NativePtr->FramePadding = value.ToNumericsVector2(); }
        }

        /// <summary>
        /// Radius of frame corners rounding. Set to 0.0f to have rectangular frame (used by most widgets). 
        /// </summary>
        public float FrameRounding
        {
            get { return NativePtr->FrameRounding; }
            set { NativePtr->FrameRounding = value; }
        }

        /// <summary>
        /// Horizontal and vertical spacing between widgets/lines.
        /// </summary>
        public Vector2 ItemSpacing
        {
            get { return new Vector2(NativePtr->ItemSpacing.X, NativePtr->ItemSpacing.Y); }
            set { NativePtr->ItemSpacing = value.ToNumericsVector2(); }
        }

        /// <summary>
        /// Horizontal and vertical spacing between within elements of a composed widget (e.g. a slider and its label).
        /// </summary>
        public Vector2 ItemInnerSpacing
        {
            get { return new Vector2(NativePtr->ItemInnerSpacing.X, NativePtr->ItemInnerSpacing.Y); }
            set { NativePtr->ItemInnerSpacing = value.ToNumericsVector2(); }
        }

        /// <summary>
        /// Expand reactive bounding box for touch-based system where touch position is not accurate enough. Unfortunately we don't sort widgets so priority on overlap will always be given to the first widget. So don't grow this too much!
        /// </summary>
        public Vector2 TouchExtraPadding
        {
            get { return new Vector2(NativePtr->TouchExtraPadding.X, NativePtr->TouchExtraPadding.Y); }
            set { NativePtr->TouchExtraPadding = value.ToNumericsVector2(); }
        }

        /// <summary>
        /// Horizontal indentation when e.g. entering a tree node
        /// </summary>
        public float IndentSpacing
        {
            get { return NativePtr->IndentSpacing; }
            set { NativePtr->IndentSpacing = value; }
        }

        /// <summary>
        /// Minimum horizontal spacing between two columns
        /// </summary>
        public float ColumnsMinSpacing
        {
            get { return NativePtr->ColumnsMinSpacing; }
            set { NativePtr->ColumnsMinSpacing = value; }
        }

        /// <summary>
        /// Width of the vertical scrollbar, Height of the horizontal scrollbar
        /// </summary>
        public float ScrollbarSize
        {
            get { return NativePtr->ScrollbarSize; }
            set { NativePtr->ScrollbarSize = value; }
        }

        /// <summary>
        /// Radius of grab corners for scrollbar
        /// </summary>
        public float ScrollbarRounding
        {
            get { return NativePtr->ScrollbarRounding; }
            set { NativePtr->ScrollbarRounding = value; }
        }

        /// <summary>
        /// Minimum width/height of a grab box for slider/scrollbar
        /// </summary>
        public float GrabMinSize
        {
            get { return NativePtr->GrabMinSize; }
            set { NativePtr->GrabMinSize = value; }
        }

        /// <summary>
        /// Radius of grabs corners rounding. Set to 0.0f to have rectangular slider grabs.
        /// </summary>
        public float GrabRounding
        {
            get { return NativePtr->GrabRounding; }
            set { NativePtr->GrabRounding = value; }
        }

        /// <summary>
        /// Window positions are clamped to be visible within the display area by at least this amount. Only covers regular windows.
        /// </summary>
        public Vector2 DisplayWindowPadding
        {
            get { return new Vector2(NativePtr->DisplayWindowPadding.X, NativePtr->DisplayWindowPadding.Y); }
            set { NativePtr->DisplayWindowPadding = value.ToNumericsVector2(); }
        }

        /// <summary>
        /// If you cannot see the edge of your screen (e.g. on a TV) increase the safe area padding. Covers popups/tooltips as well regular windows.
        /// </summary>
        public Vector2 DisplaySafeAreaPadding
        {
            get { return new Vector2(NativePtr->DisplaySafeAreaPadding.X, NativePtr->DisplaySafeAreaPadding.Y); }
            set { NativePtr->DisplaySafeAreaPadding = value.ToNumericsVector2(); }
        }

        /// <summary>
        /// Enable anti-aliasing on lines/borders. Disable if you are really tight on CPU/GPU.
        /// </summary>
        public bool AntiAliasedLines
        {
            get { return NativePtr->AntiAliasedLines == 1; }
            set { NativePtr->AntiAliasedLines = value ? (byte)1 : (byte)0; }
        }

        /// <summary>
        /// Enable anti-aliasing on filled shapes (rounded rectangles, circles, etc.)
        /// </summary>
        public bool AntiAliasedFill
        {
            get { return NativePtr->AntiAliasedFill == 1; }
            set { NativePtr->AntiAliasedFill = value ? (byte)1 : (byte)0; }
        }

        /// <summary>
        /// Tessellation tolerance. Decrease for highly tessellated curves (higher quality, more polygons), increase to reduce quality.
        /// </summary>
        public float CurveTessellationTolerance
        {
            get { return NativePtr->CurveTessellationTol; }
            set { NativePtr->CurveTessellationTol = value; }
        }

        /// <summary>
        /// Gets the current style color for the given UI element type.
        /// </summary>
        /// <param name="target">The type of UI element.</param>
        /// <returns>The element's color as currently configured.</returns>
        public Color GetColor(ColorTarget target)
        {
            var vec4 = *(System.Numerics.Vector4*)&NativePtr->Colors[(int)target * 4];
            return new Color(vec4.X, vec4.Y, vec4.Z, vec4.W);
        } 

        /// <summary>
        /// Sets the style color for a particular UI element type.
        /// </summary>
        /// <param name="target">The type of UI element.</param>
        /// <param name="value">The new color.</param>
        public void SetColor(ColorTarget target, Color value)
        {
            NativePtr->Colors[(int)target * 4 + 0] = value.R;
            NativePtr->Colors[(int)target * 4 + 1] = value.G;
            NativePtr->Colors[(int)target * 4 + 2] = value.B;
            NativePtr->Colors[(int)target * 4 + 3] = value.A;
        }
    }
}
