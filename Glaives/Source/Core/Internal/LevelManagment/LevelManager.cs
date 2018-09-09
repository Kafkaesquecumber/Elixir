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
using System.ComponentModel;
using Glaives.Core.Input;

namespace Glaives.Core.Internal.LevelManagment
{
    /// <summary>
    /// Manages loading and unloading of levels
    /// </summary>
    internal sealed class LevelManager
    {
        /// <summary>
        /// The level currently loaded 
        /// </summary>
        internal Level Level { get; private set; }

        internal LevelManager()
        {
        }

        /// <summary>
        /// Unload the current level and load the new level
        /// </summary>
        /// <typeparam name="T">The type of the level to load</typeparam>
        internal void LoadLevel<T>() where T : Level, new()
        {
            LoadLevel(typeof(T));
        }

        /// <summary>
        /// Unload the current level and load the new level
        /// </summary>
        /// <param name="levelType">The type of the level to load</param>
        internal void LoadLevel(Type levelType)
        {
            UnloadLevel();
            if (!levelType.IsSubclassOf(typeof(Level)))
            {
                throw new GlaivesException($"Failed to load level: {levelType.Name} does not derive from { typeof(Level).Name }");
            }

            Level = (Level)Activator.CreateInstance(levelType);
            Level.LoadLevelInternal();
        }

        internal void UnloadLevel()
        {
            if(Level != null)
            {
                Level.Root.ForceDestroy();
                Level.DestroyPendingActors();
                Level = null;
            }

            // Unload all cached content
            Engine.Get.Content.DisposeAllContent();
            GC.Collect();
        }

        internal void OnInputActionEvent(KeyState keyState, Key key, int gamepadId)
        {
            Level?.OnInputActionEvent(keyState, key, gamepadId);
        }

        internal void OnInputAxisEvent(InputAxis axis, float value, int gamepadId)
        {
            Level?.OnInputAxisEvent(axis, value, gamepadId);
        }
    }
}
