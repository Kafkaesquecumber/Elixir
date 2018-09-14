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
using System.IO;
using System.Runtime.CompilerServices;
using Glaives.Core.Configuration;
using Glaives.Core.Coroutines;
using Glaives.Core.Diagnostics;
using Glaives.Core.Graphics;
using Glaives.Core.Internal.EditorGui;
using Glaives.Core.Input;
using Glaives.Core.Internal;
using Glaives.Core.Internal.Graphics;
using Glaives.Core.Internal.Input;
using Glaives.Core.Internal.LevelManagment;
using Glaives.Core.Internal.Timing;
using Glaives.Core.Internal.Windowing;
using ImGuiNET;

[assembly: InternalsVisibleTo("Glaives.Editor", AllInternalsVisible = true)]
namespace Glaives.Core
{
    /// <summary>
    /// The service provider for all engine features
    /// </summary>
    public sealed class Engine
    {
        private static Engine _engine;

        /// <summary>
        /// Get the engine singleton
        /// </summary>
        public static Engine Get => _engine ?? (_engine = new Engine());

        private Settings _settings;
        /// <summary>
        /// Contains all project-specific engine settings
        /// </summary>
        public Settings Settings
        {
            get { EnsureInit(); return _settings; }
            private set => _settings = value;
        }

        private Statistics _stats;
        /// <summary>
        /// Engine statistics
        /// </summary>
        public Statistics Stats
        {
            get { EnsureInit(); return _stats; }
            private set => _stats = value;
        }
        
        private GameInstance _gameInstance;
        /// <summary>
        /// The game instance
        /// </summary>
        public GameInstance GameInstance
        {
            get { EnsureInit(); return _gameInstance; }
            private set => _gameInstance = value;
        }

        private Debugger _debug;
        /// <summary>
        /// Debug utility
        /// </summary>
        public Debugger Debug
        {
            get { EnsureInit(); return _debug; }
            private set => _debug = value;
        }
        
        private ContentLoader _content;
        /// <summary>
        /// <para>Loads and caches content files</para>
        /// <para>When loading content, the content loader will first check if this file was loaded previously</para>
        /// <para>If it is present in the content cache it will return the cached content, otherwise it will load it and add it to the content cache</para>
        /// </summary>
        public ContentLoader Content
        {
            get { EnsureInit(); return _content; }
            private set => _content = value;
        }

        private GraphicsDevice _graphicsDevice;
        /// <summary>
        /// Contains rendering-related functionality
        /// </summary>
        public GraphicsDevice Graphics
        {
            get { EnsureInit(); return _graphicsDevice; }
            private set => _graphicsDevice = value;
        }

        /// <summary>
        /// The viewport of the window
        /// </summary>
        public Viewport Viewport
        {
            get { EnsureInit(); return Window.Viewport; }
        }

        /// <summary>
        /// The path to the content root directory
        /// </summary>
        public string ContentFolder { get; private set; }

        /// <summary>
        /// The handle to the native window
        /// </summary>
        public IntPtr WindowHandle => Window?.Handle ?? IntPtr.Zero;

        /// <summary>
        /// The currently loaded level
        /// </summary>
        public Level CurrentLevel => LevelManager.Level;

        /// <summary>
        /// Whether or not we should render ImGui stuff
        /// </summary>
        public bool RenderImGui { get; set; } = true;

        /// <summary>
        /// Whether or not we should show the default main menu bar and it's contents
        /// </summary>
        public bool ShowImGuiGlaivesMenuBar { get; set; } = true;

        internal Window Window { get; private set; }
        internal EngineTimer EngineTimer { get; private set; }
        internal CoroutineRunner GlobalCoroutineRunner { get; private set; }
        internal InputManager InputManager { get; private set; }
        internal LevelManager LevelManager { get; private set; }
        internal bool AllowGameInstanceInstantiation { get; private set; }
        internal  bool Initialized { get; private set; }
        
        private bool _initCalled;
        
        private Engine() { }

        /// <summary>
        /// Initializes all modules and loads the initial level
        /// </summary>
        /// <typeparam name="TGameInstanceType">The type of your custom game instance (create a class and inherit from GameInstance)</typeparam>
        /// <typeparam name="TInitialLevelType">The type of your custom level to load as the first level (create a class and inherit from Level)</typeparam>
        /// <param name="contentFolder">The relative path to your content root folder</param>
        public void Initialize<TGameInstanceType, TInitialLevelType>(string contentFolder) 
            where TGameInstanceType : GameInstance, new() 
            where TInitialLevelType : Level
        {
            Initialize(typeof(TGameInstanceType), typeof(TInitialLevelType), contentFolder);
        }

        /// <summary>
        /// Initializes all modules and loads the initial level
        /// </summary>
        /// <param name="gameInstanceType">The type of your custom game instance (create a class and inherit from GameInstance)</param>
        /// <param name="initialLevelType">The type of your custom level to load as the first level (create a class and inherit from Level)</param>
        /// <param name="contentFolder">The relative path to your content root folder</param>
        public void Initialize(Type gameInstanceType, Type initialLevelType, string contentFolder)
        {
            if (_initCalled)
            {
                throw new GlaivesException("Initialize was already called previously and may not be called more than once");
            }

            _initCalled = true;

            if (!Directory.Exists(contentFolder))
            {
                throw new GlaivesException($"Content folder '{contentFolder}' not found");
            }
            ContentFolder = contentFolder;

            if (gameInstanceType == null)
            {
                throw new GlaivesException($"{nameof(gameInstanceType)} may not be null");
            }

            if (!gameInstanceType.IsSubclassOf(typeof(GameInstance)))
            {
                throw new GlaivesException($"{nameof(gameInstanceType)} does not derive from {typeof(GameInstance).Name}");
            }

            if (initialLevelType == null)
            {
                throw new GlaivesException($"{nameof(initialLevelType)} may not be null");
            }

            if (!initialLevelType.IsSubclassOf(typeof(Level)))
            {
                throw new GlaivesException($"{nameof(initialLevelType)} does not derive from {nameof(Level)}");
            }

            AllowGameInstanceInstantiation = true;
            GameInstance = (GameInstance)Activator.CreateInstance(gameInstanceType);
            AllowGameInstanceInstantiation = false;

            Debug = new Debugger(GameInstance);
            GlobalCoroutineRunner = new CoroutineRunner();
            Settings = GameInstance.GetSettingsInternal();
            
            Window = new Window(new IntVector2(Settings.Video.Width, Settings.Video.Height), Settings.Video.Title);
            Window.InputActionEvent += OnInputActionEvent;
            Window.InputAxisEvent += OnInputAxisEvent;

            Debug.Info($"OpenGL version {Window.OpenGlVersion}");
            
            Stats = new Statistics();

            InputManager = new InputManager(Window, Settings.Input);
            Content = new ContentLoader();
            Graphics = new GraphicsDevice(Window);
            LevelManager = new LevelManager();

            Initialized = true;
            
            Run(initialLevelType); // Contains the program loop
        }
        
        private void OnInputActionEvent(KeyState keyState, Key key, int gamepadId)
        {
            LevelManager.OnInputActionEvent(keyState, key, gamepadId);
        }

        private void OnInputAxisEvent(InputAxis axis, float value, int gamepadId)
        {
            LevelManager.OnInputAxisEvent(axis, value, gamepadId);
        }

        /// <summary>
        /// Load a new level and unload the current one
        /// </summary>
        /// <typeparam name="TLevel">The level type</typeparam>
        public void LoadLevel<TLevel>() where TLevel : Level, new()
        {
            LevelManager.LoadLevel<TLevel>();
        }

        /// <summary>
        /// Load a new level and unload the current one
        /// </summary>
        /// <param name="levelType">The level type (must derive from Level)</param>
        public void LoadLevel(Type levelType)
        {
            LevelManager.LoadLevel(levelType);
        }

        /// <summary>
        /// Quit the game and close the window
        /// </summary>
        public void Quit()
        {
            EnsureInit();
            Window.CloseWindow();
        }

        /// <summary>
        /// Called in the actor constructor
        /// </summary>
        /// <param name="newActor"></param>
        internal void OnActorConstruction(Actor newActor)
        {
            EnsureInit();

            // Add to level hierarchy
            Level level = LevelManager.Level;
            if (level == null)
            {
                throw new GlaivesException("No level was loaded while creating the actor");
            }
            level.AddActor(newActor);

            // Add to graphics device for rendering
            if (newActor is DrawableActor drawableActor)
            {
                Graphics.AddDrawableActor(drawableActor);
            }
        }

        internal void Run(Type initialLevelType)
        {
            EngineTimer = new EngineTimer(Window);
            EngineTimer.Initialize();
            
            ImGuiHelper.Init(Window);
            ImGuiInternal.SetupImGuiStyle(Settings.ImGui.Theme, Settings.ImGui.Alpha);

            while (Window.IsOpen())
            {
                Window.PollEvents();
                
                EngineTimer.CaptureFrameStartTime();
                
                Window.Clear(Color.Black);

                ImGuiHelper.NewFrame(Window.Viewport.Size, Vector2.Unit, (float)EngineTimer.DeltaTime);
                if (ShowImGuiGlaivesMenuBar)
                {
                    ImGuiInternal.GlaivesMainMenuBar();
                }

                // Tick after first iteration so delta time is set
                if (!EngineTimer.FirstIteration) 
                {
                    Level level = LevelManager.Level;
                    GlobalCoroutineRunner.Update();  
                    
                    GameInstance.ImGuiInternal();
                    if (level != null)
                    {
                        level.TickInternal((float) EngineTimer.DeltaTime);  // TickInternal the level (with all its actors)
                        level.ImGuiInternal();                              // Create ImGui layout
                        level.DestroyPendingActors();                       // Destroy pending actors    
                    }
                }
                else
                {
                    GameInstance.InitializeInternal();          // Initialize game instance
                    LevelManager.LoadLevel(initialLevelType);   // Load the initial level
                }
                
                ImGuiHelper.Render(Window.Viewport.Size);
                
                InputManager.Flush();
                Window.Swap();
                EngineTimer.Sleep(); 
                EngineTimer.RefreshDeltaTime();
            }

            ImGui.Shutdown();
            Window.Terminate();
        }


        private void EnsureInit()
        {
            if (!_initCalled)
            {
                throw new GlaivesException("Engine has not been initialized, call initialize first");
            }
        }
    }
}
