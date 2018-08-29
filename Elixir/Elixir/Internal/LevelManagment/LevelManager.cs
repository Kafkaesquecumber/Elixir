
using System;
using Elixir.GameFramework;

namespace Elixir.Internal.LevelManagment
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
                throw new ElixirException($"Failed to load level: {levelType.Name} does not derive from { typeof(Level).Name }");
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
        }
    }
}
