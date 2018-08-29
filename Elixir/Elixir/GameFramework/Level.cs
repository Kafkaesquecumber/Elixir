﻿using System;
using System.Collections.Generic;
using System.Text;
using Elixir.Graphics;
using Elixir.Internal;

namespace Elixir.GameFramework
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
            Root.InitializeRecursive();
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
                GC.Collect();
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
                    throw new ElixirException("Attempting to remove an actor that is not PendingDestruction, this should not be possible");
                }
                // This is legal in this case because PendingDestruction is true
                actor.Parent = null;
            }
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
        /// Method for loading objects into the level
        /// </summary>
        protected abstract void LoadLevel();
    }
}
