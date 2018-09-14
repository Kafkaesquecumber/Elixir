using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Glaives.Core;
using Glaives.Core.Graphics;

namespace ImGuiNET
{
    public static class ImGui
    {
        public static void NewFrame()
        {
            ImGuiNative.igNewFrame();
        }

        public static void Render()
        {
            ImGuiNative.igRender();
        }

        public static void Shutdown()
        {
            ImGuiNative.igShutdown();
        }

        public static unsafe IO GetIO() => new IO(ImGuiNative.igGetIO());

        public static unsafe Style GetStyle() => new Style(ImGuiNative.igGetStyle());

        public static void PushID(string id)
        {
            ImGuiNative.igPushIDStr(id);
        }

        public static void PushID(int id)
        {
            ImGuiNative.igPushIDInt(id);
        }

        public static void PushIDRange(string idBegin, string idEnd)
        {
            ImGuiNative.igPushIDStrRange(idBegin, idEnd);
        }

        public static void PushItemWidth(float width)
        {
            ImGuiNative.igPushItemWidth(width);
        }

        public static void PopItemWidth()
        {
            ImGuiNative.igPopItemWidth();
        }

        public static void PopID()
        {
            ImGuiNative.igPopID();
        }

        public static uint GetID(string id)
        {
            return ImGuiNative.igGetIDStr(id);
        }

        public static uint GetID(string idBegin, string idEnd)
        {
            return ImGuiNative.igGetIDStrRange(idBegin, idEnd);
        }

        public static void Text(string message)
        {
            ImGuiNative.igText(message);
        }

        public static void Text(string message, Color color)
        {
            ImGuiNative.igTextColored(color.ToNumericsVector4(), message);
        }

        public static void TextDisabled(string text)
        {
            ImGuiNative.igTextDisabled(text);
        }

        public static void TextWrapped(string text)
        {
            ImGuiNative.igTextWrapped(text);
        }

        public static unsafe void TextUnformatted(string message)
        {
            fixed (byte* bytes = System.Text.Encoding.UTF8.GetBytes(message))
            {
                ImGuiNative.igTextUnformatted(bytes, null);
            }
        }

        public static void LabelText(string label, string text)
        {
            ImGuiNative.igLabelText(label, text);
        }

        public static void Bullet()
        {
            ImGuiNative.igBullet();
        }

        public static void BulletText(string text)
        {
            ImGuiNative.igBulletText(text);
        }

        public static bool InvisibleButton(string id) => InvisibleButton(id, Vector2.Zero);

        public static bool InvisibleButton(string id, Vector2 size)
        {
            return ImGuiNative.igInvisibleButton(id, size.ToNumericsVector2());
        }

        public static void Image(IntPtr userTextureID, Vector2 size, Vector2 uv0, Vector2 uv1, Color tintColor, Color borderColor)
        {
            ImGuiNative.igImage(userTextureID, size.ToNumericsVector2(), 
                uv0.ToNumericsVector2(), uv1.ToNumericsVector2(), tintColor.ToNumericsVector4(), borderColor.ToNumericsVector4());
        }

        public static bool ImageButton(
            IntPtr userTextureID,
            Vector2 size,
            Vector2 uv0,
            Vector2 uv1,
            int framePadding,
            Color backgroundColor,
            Color tintColor)
        {
            return ImGuiNative.igImageButton(userTextureID, size.ToNumericsVector2(), uv0.ToNumericsVector2(), uv1.ToNumericsVector2(), 
                framePadding, backgroundColor.ToNumericsVector4(), tintColor.ToNumericsVector4());
        }

        //obsolete!
        public static bool CollapsingHeader(string label, string id, bool displayFrame, bool defaultOpen)
        {
            TreeNodeFlags default_open_flags = TreeNodeFlags.DefaultOpen;
            return ImGuiNative.igCollapsingHeader(label, (defaultOpen ? default_open_flags : 0));
        }


        public static bool CollapsingHeader(string label, TreeNodeFlags flags)
        {
            return ImGuiNative.igCollapsingHeader(label, flags);
        }

        public static bool Checkbox(string label, ref bool value)
        {
            return ImGuiNative.igCheckbox(label, ref value);
        }

        public static unsafe bool RadioButton(string label, ref int target, int buttonValue)
        {
            int targetCopy = target;
            bool result = ImGuiNative.igRadioButton(label, &targetCopy, buttonValue);
            target = targetCopy;
            return result;
        }

        public static bool RadioButtonBool(string label, bool active)
        {
            return ImGuiNative.igRadioButtonBool(label, active);
        }

        public static bool BeginCombo(string label, string previewValue, ComboFlags flags)
            => ImGuiNative.igBeginCombo(label, previewValue, flags);

        public static void EndCombo() => ImGuiNative.igEndCombo();

        public unsafe static bool Combo(string label, ref int current_item, string[] items)
        {
            return ImGuiNative.igCombo(label, ref current_item, items, items.Length, 5);
        }

        public unsafe static bool Combo(string label, ref int current_item, string[] items, int heightInItems)
        {
            return ImGuiNative.igCombo(label, ref current_item, items, items.Length, heightInItems);
        }

        public static bool ColorButton(string desc_id, Color color, ColorEditFlags flags, Vector2 size)
        {
            return ImGuiNative.igColorButton(desc_id, color.ToNumericsVector4(), flags, size.ToNumericsVector2());
        }

        public static unsafe bool ColorEdit3(string label, ref float r, ref float g, ref float b, ColorEditFlags flags = ColorEditFlags.Default)
        {
            System.Numerics.Vector3 localColor = new System.Numerics.Vector3(r, g, b);
            bool result = ImGuiNative.igColorEdit3(label, &localColor, flags);
            if (result)
            {
                r = localColor.X;
                g = localColor.Y;
                b = localColor.Z;
            }

            return result;
        }

        public static unsafe bool ColorEdit3(string label, ref Color color, ColorEditFlags flags = ColorEditFlags.Default)
        {
            System.Numerics.Vector3 localColor = color.ToNumericsVector3();
            bool result = ImGuiNative.igColorEdit3(label, &localColor, flags);
            if (result)
            {
                color = new Color(localColor.X, localColor.Y, localColor.Z);
            }

            return result;
        }

        public static unsafe bool ColorEdit4(string label, ref float r, ref float g, ref float b, ref float a, ColorEditFlags flags = ColorEditFlags.Default)
        {
            System.Numerics.Vector4 localColor = new System.Numerics.Vector4(r, g, b, a);
            bool result = ImGuiNative.igColorEdit4(label, &localColor, flags);
            if (result)
            {
                r = localColor.X;
                g = localColor.Y;
                b = localColor.Z;
                a = localColor.W;
            }

            return result;
        }

        public static unsafe bool ColorEdit4(string label, ref Color color, ColorEditFlags flags = ColorEditFlags.Default)
        {
            System.Numerics.Vector4 localColor = color.ToNumericsVector4();
            bool result = ImGuiNative.igColorEdit4(label, &localColor, flags);
            if (result)
            {
                color = new Color(localColor.X, localColor.Y, localColor.Z, localColor.W);
            }

            return result;
        }

        public static unsafe bool ColorPicker3(string label, ref Color color, ColorEditFlags flags = ColorEditFlags.Default)
        {
            System.Numerics.Vector3 localColor = color.ToNumericsVector3();
            bool result = ImGuiNative.igColorPicker3(label, &localColor, flags);
            if (result)
            {
                color = new Color(localColor.X, localColor.Y, localColor.Z);
            }
            return result;
        }

        public static unsafe bool ColorPicker4(string label, ref Color color, ColorEditFlags flags = ColorEditFlags.Default)
        {
            System.Numerics.Vector4 localColor = color.ToNumericsVector4();
            bool result = ImGuiNative.igColorPicker4(label, &localColor, flags);
            if (result)
            {
                color = new Color(localColor.X, localColor.Y, localColor.Z, localColor.W);
            }
            return result;
        }

        public unsafe static void PlotLines(
            string label,
            float[] values,
            int valuesOffset,
            string overlayText,
            float scaleMin,
            float scaleMax,
            Vector2 graphSize,
            int stride)
        {
            fixed (float* valuesBasePtr = values)
            {
                ImGuiNative.igPlotLines(
                    label,
                    valuesBasePtr,
                    values.Length,
                    valuesOffset,
                    overlayText,
                    scaleMin,
                    scaleMax,
                    graphSize.ToNumericsVector2(),
                    stride);
            }
        }

        public static void ShowStyleSelector(string label)
        {
            ImGuiNative.igShowStyleSelector(label);
        }

        public static void ShowFontSelector(string label)
        {
            ImGuiNative.igShowFontSelector(label);
        }

        [Obsolete("This PlotHistogram overload is deprecated. Use the overload accepting a startIndex and count.")]
        public unsafe static void PlotHistogram(
            string label,
            float[] values,
            int valuesOffset,
            string overlayText,
            float scaleMin,
            float scaleMax,
            Vector2 graphSize,
            int stride)
        {
            fixed (float* valuesBasePtr = values)
            {
                ImGuiNative.igPlotHistogram(
                    label,
                    valuesBasePtr,
                    values.Length,
                    valuesOffset,
                    overlayText,
                    scaleMin,
                    scaleMax,
                    graphSize.ToNumericsVector2(),
                    stride);
            }
        }

        public unsafe static void PlotHistogram(
            string label,
            float[] values,
            int startIndex,
            int count,
            string overlayText = null,
            float scaleMin = float.MaxValue,
            float scaleMax = float.MaxValue,
            Vector2 graphSize = default(Vector2),
            int elementStride = 1)
        {
            fixed (float* valuesBasePtr = values)
            {
                ImGuiNative.igPlotHistogram(
                    label,
                    valuesBasePtr,
                    count,
                    startIndex,
                    overlayText,
                    scaleMin,
                    scaleMax,
                    graphSize.ToNumericsVector2(),
                    elementStride * sizeof(float));
            }
        }

        public static bool SliderFloat(string sliderLabel, ref float value, float min, float max, string displayText, float power)
        {
            return ImGuiNative.igSliderFloat(sliderLabel, ref value, min, max, displayText, power);
        }

        public static bool SliderVector2(string label, ref Vector2 value, float min, float max, string displayText, float power)
        {
            System.Numerics.Vector2 vec2 = value.ToNumericsVector2();
            bool result = ImGuiNative.igSliderFloat2(label, ref vec2, min, max, displayText, power);
            value = new Vector2(vec2.X, vec2.Y);
            return result;
        }

        public static bool SliderVector3(string label, ref Color value, float min, float max, string displayText, float power)
        {
            System.Numerics.Vector3 vec3 = value.ToNumericsVector3();
            bool result = ImGuiNative.igSliderFloat3(label, ref vec3, min, max, displayText, power);
            value = new Color(vec3.X, vec3.Y, vec3.Z);
            return result;
        }

        public static bool SliderVector4(string label, ref Color value, float min, float max, string displayText, float power)
        {
            System.Numerics.Vector4 vec4 = value.ToNumericsVector4();
            bool result = ImGuiNative.igSliderFloat4(label, ref vec4, min, max, displayText, power);
            value = new Color(vec4.X, vec4.Y, vec4.Z, vec4.W);
            return result;
        }

        public static bool SliderAngle(string label, ref float radians, float minDegrees, float maxDegrees)
        {
            return ImGuiNative.igSliderAngle(label, ref radians, minDegrees, maxDegrees);
        }

        public static bool SliderInt(string sliderLabel, ref int value, int min, int max, string displayText)
        {
            return ImGuiNative.igSliderInt(sliderLabel, ref value, min, max, displayText);
        }

        public static bool SliderInt2(string label, ref Int2 value, int min, int max, string displayText)
        {
            return ImGuiNative.igSliderInt2(label, ref value, min, max, displayText);
        }

        public static bool SliderInt3(string label, ref Int3 value, int min, int max, string displayText)
        {
            return ImGuiNative.igSliderInt3(label, ref value, min, max, displayText);
        }

        public static bool SliderInt4(string label, ref Int4 value, int min, int max, string displayText)
        {
            return ImGuiNative.igSliderInt4(label, ref value, min, max, displayText);
        }

        public static bool DragFloat(string label, ref float value, float min, float max, float dragSpeed = 1f, string displayFormat = "%f", float dragPower = 1f)
        {
            return ImGuiNative.igDragFloat(label, ref value, dragSpeed, min, max, displayFormat, dragPower);
        }

        public static bool DragVector2(string label, ref Vector2 value, float min, float max, float dragSpeed = 1f, string displayFormat = "%f", float dragPower = 1f)
        {
            System.Numerics.Vector2 vec2 = value.ToNumericsVector2();
            bool result = ImGuiNative.igDragFloat2(label, ref vec2, dragSpeed, min, max, displayFormat, dragPower);
            value = new Vector2(vec2.X, vec2.Y);
            return result;
        }

        public static bool DragVector3(string label, ref Color value, float min, float max, float dragSpeed = 1f, string displayFormat = "%f", float dragPower = 1f)
        {
            System.Numerics.Vector3 vec3 = value.ToNumericsVector3();
            bool result = ImGuiNative.igDragFloat3(label, ref vec3, dragSpeed, min, max, displayFormat, dragPower);
            value = new Color(vec3.X, vec3.Y, vec3.Z);
            return result;
        }

        public static bool DragVector4(string label, ref Color value, float min, float max, float dragSpeed = 1f, string displayFormat = "%f", float dragPower = 1f)
        {
            System.Numerics.Vector4 vec4 = value.ToNumericsVector4();
            bool result = ImGuiNative.igDragFloat4(label, ref vec4, dragSpeed, min, max, displayFormat, dragPower);
            value = new Color(vec4.X, vec4.Y, vec4.Z, vec4.W);
            return result;
        }

        public static bool DragFloatRange2(
            string label,
            ref float currentMinValue,
            ref float currentMaxValue,
            float speed = 1.0f,
            float minValueLimit = 0.0f,
            float maxValueLimit = 0.0f,
            string displayFormat = "%.3f",
            string displayFormatMax = null,
            float power = 1.0f)
        {
            return ImGuiNative.igDragFloatRange2(label, ref currentMinValue, ref currentMaxValue, speed, minValueLimit, maxValueLimit, displayFormat, displayFormatMax, power);
        }

        public static bool DragInt(string label, ref int value, float speed, int minValue, int maxValue, string displayText)
        {
            return ImGuiNative.igDragInt(label, ref value, speed, minValue, maxValue, displayText);
        }

        public static bool DragInt2(string label, ref Int2 value, float speed, int minValue, int maxValue, string displayText)
        {
            return ImGuiNative.igDragInt2(label, ref value, speed, minValue, maxValue, displayText);
        }

        public static bool DragInt3(string label, ref Int3 value, float speed, int minValue, int maxValue, string displayText)
        {
            return ImGuiNative.igDragInt3(label, ref value, speed, minValue, maxValue, displayText);
        }

        public static bool DragInt4(string label, ref Int4 value, float speed, int minValue, int maxValue, string displayText)
        {
            return ImGuiNative.igDragInt4(label, ref value, speed, minValue, maxValue, displayText);
        }

        public static bool DragIntRange2(
            string label,
            ref int currentMinValue,
            ref int currentMaxValue,
            float speed = 1.0f,
            int minLimit = 0,
            int maxLimit = 0,
            string displayFormat = "%.0f",
            string displayFormatMax = null)
        {
            return ImGuiNative.igDragIntRange2(
                label,
                ref currentMinValue,
                ref currentMaxValue,
                speed,
                minLimit,
                maxLimit,
                displayFormat,
                displayFormatMax);
        }

        public static bool Button(string message)
        {
            return ImGuiNative.igButton(message, Vector2.Zero.ToNumericsVector2());
        }

        public static bool Button(string message, Vector2 size)
        {
            return ImGuiNative.igButton(message, size.ToNumericsVector2());
        }

        public static unsafe void ProgressBar(float fraction, Vector2 size, string overlayText)
        {
            System.Numerics.Vector2 vec2 = size.ToNumericsVector2();
            ImGuiNative.igProgressBar(fraction, &vec2, overlayText);
        }

        public static void SetNextWindowSize(Vector2 size, Condition condition)
        {
            System.Numerics.Vector2 vec2 = size.ToNumericsVector2();
            ImGuiNative.igSetNextWindowSize(vec2, condition);
        }

        public static void SetNextWindowFocus()
        {
            ImGuiNative.igSetNextWindowFocus();
        }

        public static void SetNextWindowPos(Vector2 position, Condition condition)
        {
            ImGuiNative.igSetNextWindowPos(position.ToNumericsVector2(), condition, Vector2.Zero.ToNumericsVector2());
        }

        public static void SetNextWindowPos(Vector2 position, Condition condition, Vector2 pivot)
        {
            ImGuiNative.igSetNextWindowPos(position.ToNumericsVector2(), condition, pivot.ToNumericsVector2());
        }

        public static void AddInputCharacter(char keyChar)
        {
            ImGuiNative.ImGuiIO_AddInputCharacter(keyChar);
        }

        /// <summary>
        /// Helper to scale the ClipRect field of each ImDrawCmd.
        /// Use if your final output buffer is at a different scale than ImGui expects,
        /// or if there is a difference between your window resolution and framebuffer resolution.
        /// </summary>
        /// <param name="drawData">Pointer to the DrawData to scale.</param>
        /// <param name="scale">The scale to apply.</param>
        public static unsafe void ScaleClipRects(DrawData* drawData, Vector2 scale)
        {
            for (int i = 0; i < drawData->CmdListsCount; i++)
            {
                NativeDrawList* cmd_list = drawData->CmdLists[i];
                for (int cmd_i = 0; cmd_i < cmd_list->CmdBuffer.Size; cmd_i++)
                {
                    DrawCmd* drawCmdList = (DrawCmd*)cmd_list->CmdBuffer.Data;
                    DrawCmd* cmd = &drawCmdList[cmd_i];
                    cmd->ClipRect = new System.Numerics.Vector4(cmd->ClipRect.X * scale.X, cmd->ClipRect.Y * scale.Y, cmd->ClipRect.Z * scale.X, cmd->ClipRect.W * scale.Y);
                }
            }
        }

        public static float GetWindowHeight()
        {
            return ImGuiNative.igGetWindowHeight();
        }


        public static float GetWindowWidth()
        {
            return ImGuiNative.igGetWindowWidth();
        }

        public static Vector2 GetWindowSize()
        {
            System.Numerics.Vector2 size;
            ImGuiNative.igGetWindowSize(out size);
            return new Vector2(size.X, size.Y);
        }

        public static Vector2 GetWindowPosition()
        {
            System.Numerics.Vector2 pos;
            ImGuiNative.igGetWindowPos(out pos);
            return new Vector2(pos.X, pos.Y);
        }


        public static void SetWindowSize(Vector2 size, Condition cond = 0)
        {
            ImGuiNative.igSetWindowSize(size.ToNumericsVector2(), cond);
        }

        public static bool BeginWindow(string windowTitle) => BeginWindow(windowTitle, WindowFlags.Default);

        public static unsafe bool BeginWindow(string windowTitle, WindowFlags flags)
        {
            return ImGuiNative.igBegin(windowTitle, null, flags);
        }

        public static unsafe bool BeginWindow(string windowTitle, ref bool opened, WindowFlags flags)
        {
            byte openedLocal = opened ? (byte)1 : (byte)0;
            bool ret = ImGuiNative.igBegin(windowTitle, &openedLocal, flags);
            opened = openedLocal != 0;
            return ret;
        }

        public static unsafe bool BeginWindow(string windowTitle, ref bool opened, float backgroundAlpha, WindowFlags flags)
        {
            byte openedLocal = opened ? (byte)1 : (byte)0;
            bool ret = ImGuiNative.igBegin2(windowTitle, &openedLocal, new System.Numerics.Vector2(), backgroundAlpha, flags);
            opened = openedLocal != 0;
            return ret;
        }

        public static unsafe bool BeginWindow(string windowTitle, ref bool opened, Vector2 startingSize, WindowFlags flags)
        {
            byte openedLocal = opened ? (byte)1 : (byte)0;
            bool ret = ImGuiNative.igBegin2(windowTitle, &openedLocal, startingSize.ToNumericsVector2(), 1f, flags);
            opened = openedLocal != 0;
            return ret;
        }

        public static unsafe bool BeginWindow(string windowTitle, ref bool opened, Vector2 startingSize, float backgroundAlpha, WindowFlags flags)
        {
            byte openedLocal = opened ? (byte)1 : (byte)0;
            bool ret = ImGuiNative.igBegin2(windowTitle, &openedLocal, startingSize.ToNumericsVector2(), backgroundAlpha, flags);
            opened = openedLocal != 0;
            return ret;
        }

        public static bool BeginMenu(string label)
        {
            return ImGuiNative.igBeginMenu(label, true);
        }

        public static bool BeginMenu(string label, bool enabled)
        {
            return ImGuiNative.igBeginMenu(label, enabled);
        }

        public static bool BeginMenuBar()
        {
            return ImGuiNative.igBeginMenuBar();
        }

        public static void CloseCurrentPopup()
        {
            ImGuiNative.igCloseCurrentPopup();
        }

        public static void EndMenuBar()
        {
            ImGuiNative.igEndMenuBar();
        }

        public static void EndMenu()
        {
            ImGuiNative.igEndMenu();
        }

        public static void Separator()
        {
            ImGuiNative.igSeparator();
        }

        public static bool MenuItem(string label)
        {
            return MenuItem(label, string.Empty, false, true);
        }

        public static bool MenuItem(string label, string shortcut)
        {
            return MenuItem(label, shortcut, false, true);
        }

        public static bool MenuItem(string label, bool enabled)
        {
            return MenuItem(label, string.Empty, false, enabled);
        }

        public static bool MenuItem(string label, string shortcut, bool selected, bool enabled)
        {
            return ImGuiNative.igMenuItem(label, shortcut, selected, enabled);
        }

        public static unsafe bool InputText(string label, byte[] textBuffer, uint bufferSize, InputTextFlags flags, TextEditCallback textEditCallback)
        {
            return InputText(label, textBuffer, bufferSize, flags, textEditCallback, IntPtr.Zero);
        }

        public static unsafe bool InputText(string label, byte[] textBuffer, uint bufferSize, InputTextFlags flags, TextEditCallback textEditCallback, IntPtr userData)
        {
            Debug.Assert(bufferSize <= textBuffer.Length);
            fixed (byte* ptrBuf = textBuffer)
            {
                return InputText(label, new IntPtr(ptrBuf), bufferSize, flags, textEditCallback, userData);
            }
        }

        public static unsafe bool InputText(string label, IntPtr textBuffer, uint bufferSize, InputTextFlags flags, TextEditCallback textEditCallback)
        {
            return InputText(label, textBuffer, bufferSize, flags, textEditCallback, IntPtr.Zero);
        }

        public static unsafe bool InputText(string label, IntPtr textBuffer, uint bufferSize, InputTextFlags flags, TextEditCallback textEditCallback, IntPtr userData)
        {
            return ImGuiNative.igInputText(label, textBuffer, bufferSize, flags, textEditCallback, userData.ToPointer());
        }

        public static void EndWindow()
        {
            ImGuiNative.igEnd();
        }

        public static void PushStyleColor(ColorTarget target, Color color)
        {
            ImGuiNative.igPushStyleColor(target, color.ToNumericsVector4());
        }

        public static void PopStyleColor()
        {
            PopStyleColor(1);
        }

        public static void PopStyleColor(int numStyles)
        {
            ImGuiNative.igPopStyleColor(numStyles);
        }

        public static void PushStyleVar(StyleVar var, float value) => ImGuiNative.igPushStyleVar(var, value);
        public static void PushStyleVar(StyleVar var, Vector2 value) => ImGuiNative.igPushStyleVarVec(var, value.ToNumericsVector2());

        public static void PopStyleVar() => ImGuiNative.igPopStyleVar(1);
        public static void PopStyleVar(int count) => ImGuiNative.igPopStyleVar(count);

        public static unsafe void InputTextMultiline(string label, IntPtr textBuffer, uint bufferSize, Vector2 size, InputTextFlags flags, TextEditCallback callback)
        {
            ImGuiNative.igInputTextMultiline(label, textBuffer, bufferSize, size.ToNumericsVector2(), flags, callback, null);
        }

        public static unsafe DrawData* GetDrawData()
        {
            return ImGuiNative.igGetDrawData();
        }

        public static unsafe void InputTextMultiline(string label, IntPtr textBuffer, uint bufferSize, Vector2 size, InputTextFlags flags, TextEditCallback callback, IntPtr userData)
        {
            ImGuiNative.igInputTextMultiline(label, textBuffer, bufferSize, size.ToNumericsVector2(), flags, callback, userData.ToPointer());
        }

        public static bool BeginChildFrame(uint id, Vector2 size, WindowFlags flags)
        {
            return ImGuiNative.igBeginChildFrame(id, size.ToNumericsVector2(), flags);
        }

        public static void EndChildFrame()
        {
            ImGuiNative.igEndChildFrame();
        }

        public static unsafe void ColorConvertRGBToHSV(float r, float g, float b, out float h, out float s, out float v)
        {
            float h2, s2, v2;
            ImGuiNative.igColorConvertRGBtoHSV(r, g, b, &h2, &s2, &v2);
            h = h2;
            s = s2;
            v = v2;
        }

        public static unsafe void ColorConvertHSVToRGB(float h, float s, float v, out float r, out float g, out float b)
        {
            float r2, g2, b2;
            ImGuiNative.igColorConvertHSVtoRGB(h, s, v, &r2, &g2, &b2);
            r = r2;
            g = g2;
            b = b2;
        }


        public static int GetKeyIndex(GuiKey key)
        {
            //TODO this got exported by later version of cimgui, call ImGuiNative after upgrading
            IO io = GetIO();
            return io.KeyMap[key];
        }

        public static bool IsKeyDown(int keyIndex)
        {
            return ImGuiNative.igIsKeyDown(keyIndex);
        }

        public static bool IsKeyPressed(int keyIndex, bool repeat = true)
        {
            return ImGuiNative.igIsKeyPressed(keyIndex, repeat);
        }

        public static bool IsKeyReleased(int keyIndex)
        {
            return ImGuiNative.igIsKeyReleased(keyIndex);
        }

        public static int GetKeyPressedAmount(int keyIndex, float repeatDelay, float rate)
        {
            return ImGuiNative.igGetKeyPressedAmount(keyIndex, repeatDelay, rate);
        }

        public static bool IsMouseDown(int button)
        {
            return ImGuiNative.igIsMouseDown(button);
        }

        public static bool IsMouseClicked(int button, bool repeat = false)
        {
            return ImGuiNative.igIsMouseClicked(button, repeat);
        }

        public static bool IsMouseDoubleClicked(int button)
        {
            return ImGuiNative.igIsMouseDoubleClicked(button);
        }

        public static bool IsMouseReleased(int button)
        {
            return ImGuiNative.igIsMouseReleased(button);
        }

        public static bool IsAnyWindowHovered()
        {
            return ImGuiNative.igIsAnyWindowHovered();
        }

        public static bool IsWindowFocused(FocusedFlags flags)
        {
            return ImGuiNative.igIsWindowFocused(flags);
        }

        public static bool IsWindowHovered(HoveredFlags flags)
        {
            return ImGuiNative.igIsWindowHovered(flags);
        }

        public static bool IsMouseHoveringRect(Vector2 minPosition, Vector2 maxPosition, bool clip)
        {
            return ImGuiNative.igIsMouseHoveringRect(minPosition.ToNumericsVector2(), maxPosition.ToNumericsVector2(), clip);
        }

        public static unsafe bool IsMousePosValid()
        {
            return ImGuiNative.igIsMousePosValid(null);
        }

        public static unsafe bool IsMousePosValid(Vector2 mousePos)
        {
            System.Numerics.Vector2 vec2 = mousePos.ToNumericsVector2();
            return ImGuiNative.igIsMousePosValid(&vec2);
        }

        public static bool IsMouseDragging(int button, float lockThreshold)
        {
            return ImGuiNative.igIsMouseDragging(button, lockThreshold);
        }

        public static Vector2 GetMousePos()
        {
            System.Numerics.Vector2 retVal;
            ImGuiNative.igGetMousePos(out retVal);
            return new Vector2(retVal.X, retVal.Y);
        }

        public static Vector2 GetMousePosOnOpeningCurrentPopup()
        {
            System.Numerics.Vector2 retVal;
            ImGuiNative.igGetMousePosOnOpeningCurrentPopup(out retVal);
            return new Vector2(retVal.X, retVal.Y);
        }

        public static Vector2 GetMouseDragDelta(int button, float lockThreshold)
        {
            System.Numerics.Vector2 retVal;
            ImGuiNative.igGetMouseDragDelta(out retVal, button, lockThreshold);
            return new Vector2(retVal.X, retVal.Y);
        }

        public static void ResetMouseDragDelta(int button)
        {
            ImGuiNative.igResetMouseDragDelta(button);
        }

        public static MouseCursorKind MouseCursor
        {
            get
            {
                return ImGuiNative.igGetMouseCursor();
            }
            set
            {
                ImGuiNative.igSetMouseCursor(value);
            }
        }

        public static Vector2 GetCursorStartPos()
        {
            System.Numerics.Vector2 retVal;
            ImGuiNative.igGetCursorStartPos(out retVal);
            return new Vector2(retVal.X, retVal.Y);
        }

        public static unsafe Vector2 GetCursorScreenPos()
        {
            System.Numerics.Vector2 retVal;
            ImGuiNative.igGetCursorScreenPos(&retVal);
            return new Vector2(retVal.X, retVal.Y);
        }

        public static void SetCursorScreenPos(Vector2 pos)
        {
            ImGuiNative.igSetCursorScreenPos(pos.ToNumericsVector2());
        }

        public static float GetFrameHeightWithSpacing()
        {
            return ImGuiNative.igGetFrameHeightWithSpacing();
        }

        public static void AlignTextToFramePadding()
        {
            ImGuiNative.igAlignTextToFramePadding();
        }


        public static bool BeginChild(string id, bool border = false, WindowFlags flags = 0)
        {
            return BeginChild(id, new Vector2(0, 0), border, flags);
        }

        public static bool BeginChild(string id, Vector2 size, bool border, WindowFlags flags)
        {
            return ImGuiNative.igBeginChild(id, size.ToNumericsVector2(), border, flags);
        }

        public static bool BeginChild(uint id, Vector2 size, bool border, WindowFlags flags)
        {
            return ImGuiNative.igBeginChildEx(id, size.ToNumericsVector2(), border, flags);
        }

        public static void EndChild()
        {
            ImGuiNative.igEndChild();
        }

        public static Vector2 GetContentRegionMax()
        {
            System.Numerics.Vector2 value;
            ImGuiNative.igGetContentRegionMax(out value);
            return new Vector2(value.X, value.Y);
        }

        public static Vector2 GetContentRegionAvailable()
        {
            System.Numerics.Vector2 value;
            ImGuiNative.igGetContentRegionAvail(out value);
            return new Vector2(value.X, value.Y);
        }

        public static float GetContentRegionAvailableWidth()
        {
            return ImGuiNative.igGetContentRegionAvailWidth();
        }

        public static Vector2 GetWindowContentRegionMin()
        {
            System.Numerics.Vector2 value;
            ImGuiNative.igGetWindowContentRegionMin(out value);
            return new Vector2(value.X, value.Y);
        }

        public static Vector2 GetWindowContentRegionMax()
        {
            System.Numerics.Vector2 value;
            ImGuiNative.igGetWindowContentRegionMax(out value);
            return new Vector2(value.X, value.Y);
        }

        public static float GetWindowContentRegionWidth()
        {
            return ImGuiNative.igGetWindowContentRegionWidth();
        }

        public static bool Selectable(string label)
        {
            return Selectable(label, false);
        }

        public static bool Selectable(string label, bool isSelected)
        {
            return Selectable(label, isSelected, SelectableFlags.Default);
        }

        public static bool BeginMainMenuBar()
        {
            return ImGuiNative.igBeginMainMenuBar();
        }

        public static bool OpenPopupOnItemClick(string id, int mouseButton)
        {
            return ImGuiNative.igOpenPopupOnItemClick(id, mouseButton);
        }

        public static bool BeginPopup(string id)
        {
            return ImGuiNative.igBeginPopup(id);
        }

        public static void EndMainMenuBar()
        {
            ImGuiNative.igEndMainMenuBar();
        }

        public static bool SmallButton(string label)
        {
            return ImGuiNative.igSmallButton(label);
        }

        public static bool BeginPopupModal(string name)
        {
            return BeginPopupModal(name, WindowFlags.Default);
        }

        public static bool BeginPopupModal(string name, ref bool opened)
        {
            return BeginPopupModal(name, ref opened, WindowFlags.Default);
        }

        public static unsafe bool BeginPopupModal(string name, WindowFlags extra_flags)
        {
            return ImGuiNative.igBeginPopupModal(name, null, extra_flags);
        }

        public static unsafe bool BeginPopupModal(string name, ref bool p_opened, WindowFlags extra_flags)
        {
            byte value = p_opened ? (byte)1 : (byte)0;
            bool result = ImGuiNative.igBeginPopupModal(name, &value, extra_flags);

            p_opened = value == 1 ? true : false;
            return result;
        }

        public static bool Selectable(string label, bool isSelected, SelectableFlags flags)
        {
            return Selectable(label, isSelected, flags, new Vector2());
        }

        public static bool Selectable(string label, bool isSelected, SelectableFlags flags, Vector2 size)
        {
            return ImGuiNative.igSelectable(label, isSelected, flags, size.ToNumericsVector2());
        }

        public static bool SelectableEx(string label, ref bool isSelected)
        {
            return ImGuiNative.igSelectableEx(label, ref isSelected, SelectableFlags.Default, new Vector2().ToNumericsVector2());
        }

        public static bool SelectableEx(string label, ref bool isSelected, SelectableFlags flags, Vector2 size)
        {
            return ImGuiNative.igSelectableEx(label, ref isSelected, flags, size.ToNumericsVector2());
        }

        public static unsafe Vector2 GetTextSize(string text, float wrapWidth = Int32.MaxValue)
        {
            System.Numerics.Vector2 result;
            IntPtr buffer = Marshal.StringToHGlobalAnsi(text);
            byte* textStart = (byte*)buffer.ToPointer();
            byte* textEnd = textStart + text.Length;
            ImGuiNative.igCalcTextSize(out result, (char*)textStart, (char*)textEnd, false, wrapWidth);
            return new Vector2(result.X, result.Y);
        }

        public static bool BeginPopupContextItem(string id)
        {
            return BeginPopupContextItem(id, 1);
        }

        public static bool BeginPopupContextItem(string id, int mouseButton)
        {
            return ImGuiNative.igBeginPopupContextItem(id, mouseButton);
        }

        public static unsafe void Dummy(float width, float height)
        {
            Dummy(new Vector2(width, height));
        }

        public static void EndPopup()
        {
            ImGuiNative.igEndPopup();
        }

        public static bool IsPopupOpen(string id)
        {
            return ImGuiNative.igIsPopupOpen(id);
        }

        public static unsafe void Dummy(Vector2 size)
        {
            System.Numerics.Vector2 vec2 = size.ToNumericsVector2();
            ImGuiNative.igDummy(&vec2);
        }

        public static void Spacing()
        {
            ImGuiNative.igSpacing();
        }

        public static float GetFrameHeight() => ImGuiNative.igGetFrameHeight();

        public static void Columns(int count, string id, bool border)
        {
            ImGuiNative.igColumns(count, id, border);
        }

        public static void NextColumn()
        {
            ImGuiNative.igNextColumn();
        }

        public static int GetColumnIndex()
        {
            return ImGuiNative.igGetColumnIndex();
        }

        public static float GetColumnOffset(int columnIndex)
        {
            return ImGuiNative.igGetColumnOffset(columnIndex);
        }

        public static void SetColumnOffset(int columnIndex, float offsetX)
        {
            ImGuiNative.igSetColumnOffset(columnIndex, offsetX);
        }

        public static float GetColumnWidth(int columnIndex)
        {
            return ImGuiNative.igGetColumnWidth(columnIndex);
        }

        public static void SetColumnWidth(int columnIndex, float width)
        {
            ImGuiNative.igSetColumnWidth(columnIndex, width);
        }

        public static int GetColumnsCount()
        {
            return ImGuiNative.igGetColumnsCount();
        }

        public static void OpenPopup(string id)
        {
            ImGuiNative.igOpenPopup(id);
        }

        public static void SameLine(float localPositionX = 0, float spacingW = -1.0f)
        {
            ImGuiNative.igSameLine(localPositionX, spacingW);
        }

        public static bool BeginDragDropSource(DragDropFlags flags, int mouseButton)
            => ImGuiNative.igBeginDragDropSource(flags, mouseButton);

        public static unsafe bool SetDragDropPayload(string type, IntPtr data, uint size, Condition cond)
            => ImGuiNative.igSetDragDropPayload(type, data.ToPointer(), size, cond);

        public static void EndDragDropSource() => ImGuiNative.igEndDragDropSource();

        public static bool BeginDragDropTarget() => ImGuiNative.igBeginDragDropTarget();

        public static unsafe Payload AcceptDragDropPayload(string type, DragDropFlags flags)
            => new Payload(ImGuiNative.igAcceptDragDropPayload(type, flags));

        public static void EndDragDropTarget() => ImGuiNative.igEndDragDropTarget();

        public static void PushClipRect(Vector2 min, Vector2 max, bool intersectWithCurrentCliRect)
        {
            ImGuiNative.igPushClipRect(min.ToNumericsVector2(), max.ToNumericsVector2(), intersectWithCurrentCliRect ? (byte)1 : (byte)0);
        }

        public static void PopClipRect()
        {
            ImGuiNative.igPopClipRect();
        }

        public static unsafe void StyleColorsClassic(Style style)
        {
            ImGuiNative.igStyleColorsClassic(style.NativePtr);
        }

        public static unsafe void StyleColorsDark(Style style)
        {
            ImGuiNative.igStyleColorsDark(style.NativePtr);
        }

        public static unsafe void StyleColorsLight(Style style)
        {
            ImGuiNative.igStyleColorsLight(style.NativePtr);
        }

        public static bool IsItemHovered(HoveredFlags flags)
        {
            return ImGuiNative.igIsItemHovered(flags);
        }

        public static bool IsLastItemActive()
        {
            return ImGuiNative.igIsItemActive();
        }

        public static bool IsLastItemVisible()
        {
            return ImGuiNative.igIsItemVisible();
        }

        public static bool IsAnyItemHovered()
        {
            return ImGuiNative.igIsAnyItemHovered();
        }

        public static bool IsAnyItemActive()
        {
            return ImGuiNative.igIsAnyItemActive();
        }

        public static unsafe DrawList GetOverlayDrawList() => new DrawList(ImGuiNative.igGetOverlayDrawList());

        public static void SetTooltip(string text)
        {
            ImGuiNative.igSetTooltip(text);
        }

        public static void SetNextTreeNodeOpen(bool opened)
        {
            ImGuiNative.igSetNextTreeNodeOpen(opened, Condition.Always);
        }

        public static void SetNextTreeNodeOpen(bool opened, Condition setCondition)
        {
            ImGuiNative.igSetNextTreeNodeOpen(opened, setCondition);
        }

        public static bool TreeNode(string label)
        {
            return ImGuiNative.igTreeNode(label);
        }

        public static bool TreeNodeEx(string label, TreeNodeFlags flags = 0)
        {
            return ImGuiNative.igTreeNodeEx(label, flags);
        }

        public static void TreePop()
        {
            ImGuiNative.igTreePop();
        }

        public static Vector2 GetLastItemRectSize()
        {
            System.Numerics.Vector2 result;
            ImGuiNative.igGetItemRectSize(out result);
            return new Vector2(result.X, result.Y);
        }

        public static Vector2 GetLastItemRectMin()
        {
            System.Numerics.Vector2 result;
            ImGuiNative.igGetItemRectMin(out result);
            return new Vector2(result.X, result.Y);
        }

        public static Vector2 GetLastItemRectMax()
        {
            System.Numerics.Vector2 result;
            ImGuiNative.igGetItemRectMax(out result);
            return new Vector2(result.X, result.Y);
        }

        public static bool IsWindowAppearing()
        {
            return ImGuiNative.igIsWindowAppearing();
        }

        public static void SetWindowFontScale(float scale)
        {
            ImGuiNative.igSetWindowFontScale(scale);
        }

        public static void SetScrollHere()
        {
            ImGuiNative.igSetScrollHere();
        }

        public static void SetItemDefaultFocus()
        {
            ImGuiNative.igSetItemDefaultFocus();
        }

        public static void SetScrollHere(float centerYRatio)
        {
            ImGuiNative.igSetScrollHere(centerYRatio);
        }

        public static unsafe void PushFont(Font font)
        {
            ImGuiNative.igPushFont(font.NativeFont);
        }

        public static void PopFont()
        {
            ImGuiNative.igPopFont();
        }

        public static void SetKeyboardFocusHere()
        {
            ImGuiNative.igSetKeyboardFocusHere(0);
        }

        public static void SetKeyboardFocusHere(int offset)
        {
            ImGuiNative.igSetKeyboardFocusHere(offset);
        }

        public static void CalcListClipping(int itemsCount, float itemsHeight, ref int outItemsDisplayStart, ref int outItemsDisplayEnd)
        {
            ImGuiNative.igCalcListClipping(itemsCount, itemsHeight, ref outItemsDisplayStart, ref outItemsDisplayEnd);
        }
    }
}
