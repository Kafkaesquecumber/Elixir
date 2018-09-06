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
using System.Collections.Generic;
using System.Text;
using Glaives.Graphics;
using Glaives.Input;
using Glaives.Internal;

namespace Glaives.GameFramework
{
    /// <summary>
    /// A hierarchy of GameObjects
    /// </summary>
    public abstract class Level
    {
        /// <summary>
        /// A shorthand to Engine.Get
        /// </summary>
        protected Engine Engine => Engine.Get;

        /// <summary>
        /// The root actor of the level is immutable and will always exist in the level
        /// </summary>
        public Actor Root { get; private set; }

        /// <summary>
        /// The total amount of actors in the level (not including the level root)
        /// </summary>
        public int ActorCount { get; private set; }

        /// <summary>
        /// The default view 
        /// </summary>
        public View DefaultView { get; private set; }

        /// <summary>
        /// The currently active view
        /// </summary>
        public View CurrentView { get; set; }

        /// <summary>
        /// Actors to be destroyed at the end of the frame
        /// </summary>
        internal readonly List<Actor> PendingDestroyActors = new List<Actor>();

        internal void LoadLevelInternal()
        {
            Root = new Actor(immutable: true)
            {
                Name = new UniqueString(GetType().Name)
            };

            DefaultView = new View(immutable: true, size: new IntVector2(1280, 720))
            {
                Name = new UniqueString("DefaultView")
            };
            CurrentView = DefaultView;

            LoadLevel();
            ActorCount = Root.DoRecursive(actor => actor.InitializeInternal()) - 1;
        }

        internal void Tick(float deltaTime)
        {
            ActorCount = Root.DoRecursive(actor => actor.TickInternal(deltaTime)) - 1;
            Engine.Get.Graphics.DrawBatches();
        }

        internal void DestroyPendingActors()
        {
            int actorsDestroyed = 0;
            for (int i = PendingDestroyActors.Count - 1; i >= 0; i--)
            {
                // Remove from rendering
                if (PendingDestroyActors[i] is DrawableActor drawableActor)
                {
                    Engine.Get.Graphics.RemoveDrawableActor(drawableActor);
                }

                // Remove from level hierarchy
                RemoveActor(PendingDestroyActors[i]);
                PendingDestroyActors[i] = null;
                actorsDestroyed++;
            }

            PendingDestroyActors.Clear();
            if (actorsDestroyed > 0)
            {
                //GC.Collect();
            }
        }

        internal void AddActor(Actor actor)
        {
            if (!actor.Immutable)
            {
                actor.Parent = Root;
            }
        }

        internal void RemoveActor(Actor actor)
        {
            if (!actor.Immutable)
            {
                if (!actor.PendingDestruction)
                {
                    throw new GlaivesException("Attempting to remove an actor that is not PendingDestruction, this should not be possible");
                }
                // This is legal in this case because PendingDestruction is true
                actor.Parent = null;
            }
        }

        internal void OnInputActionEvent(KeyState keyState, Key key, int gamepadId)
        {
            Root.DoRecursive(x =>
            {
                if (x.InputEnabled)
                {
                    x.ReceiveInputActionInternal(keyState, key, gamepadId);
                }
            });
        }

        internal void OnInputAxisEvent(InputAxis axis, float value, int gamepadId)
        {
            Root.DoRecursive(x =>
            {
                if (x.InputEnabled)
                {
                    x.ReceiveInputAxisInternal(axis, value, gamepadId);
                }
            });
        }

        private void BuildHierarchyString(ref StringBuilder stringBuilder, Actor parent, int depth = 0)
        {
            string tabs = string.Empty;
            for (int i = 0; i < depth; i++)
            {
                tabs += " - ";
            }
            stringBuilder.AppendLine(tabs + "<" + parent + ">");
            foreach (Actor child in parent.Children)
            {
                BuildHierarchyString(ref stringBuilder, child, depth + 1);
            }
        }

        /// <summary>
        /// A string that represents the level hierarchy
        /// </summary>
        /// <returns></returns>
        public string BuildHierarchyString()
        {
            StringBuilder builder = new StringBuilder();
            BuildHierarchyString(ref builder, Root);
            return builder.ToString();
        }
        
        /// <summary>
        /// Method for loading actors into the level
        /// </summary>
        protected abstract void LoadLevel();
    }
}
