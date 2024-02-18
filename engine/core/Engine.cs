using SFML.System;
using SFML.Graphics;
using SFML.Window;

using SilverRaven.SFML.Tools;

namespace SilverRaven.SFML
{
    public static class Engine
    {
        /// <summary>
        /// Engine Settings
        /// </summary>
        public static class Settings
        {
            /// <summary>
            /// Title of the window.
            /// </summary>
            public static string WINDOW_TITLE = "My Game";
            /// <summary>
            /// Framerate limit of the render window.
            /// </summary>
            public static uint TARGET_FPS = 60;
            /// <summary>
            /// The width of the render window. (Does not update the window size when changed)
            /// </summary>
            public static uint WINDOW_WIDTH = 800;
            /// <summary>
            /// The height of the render window. (Does not update the window size when changed)
            /// </summary>
            public static uint WINDOW_HEIGHT = 600;
            /// <summary>
            /// Vector2f representation of WindowWidth and WindowHeight. (Does not update the window size when changed)
            /// </summary>
            public static Vector2f WINDOW_SIZE => new (WINDOW_WIDTH, WINDOW_HEIGHT);
            /// <summary>
            /// Background color of the window.
            /// </summary>
            public static Color BACKGROUND_COLOR = new (30, 30, 30);
            /// <summary>
            /// The default directory for assets. Fonts, Textures and Sounds are loaded from this path.
            /// </summary>
            public static string ASSET_PATH = "";
            /// <summary>
            /// Image scalar used by SpriteObjects.
            /// </summary>
            public static float PIXEL_SCALE = 4f;
            /// <summary>
            /// Set to true if a custom Pause Menu is implemented. Draws default pause menu when false.
            /// </summary>
            public static bool CUSTOM_PAUSE_MENU = false;
            /// <summary>
            /// The starting position of the camera.
            /// </summary>
            public static Vector2f CAMERA_START_POSITION = WINDOW_SIZE / 2f;
        }

#region static properties

        /// <summary>
        /// Toggle to pause the game. Pauses GAME_TIME and stops Update() from being executed
        /// </summary>
        public static bool IS_PAUSED = false;
        /// <summary>
        /// Whether the game window is focused.
        /// </summary>
        public static bool IS_FOCUSED = true;
        /// <summary>
        /// Main camera that renders the game world.
        /// </summary>
        public static Camera CAMERA { get; private set; }
        /// <summary>
        /// Camera that renders the games user interface.
        /// </summary>
        /// <value></value>
        public static Camera UI_CAMERA { get; private set; }
        /// <summary>
        /// global System.Random object
        /// </summary>
        public static Random RANDOM { get; private set; } = new ();
        /// <summary>
        /// InputSystem for getting window input
        /// </summary>
        public static InputSystem INPUT { get; private set; } = new ();
        /// <summary>
        /// Time it took to process and render the last frame. Multiply by DELTA_TIME to get framerate-independent motion.
        /// </summary>
        public static float DELTA_TIME { get; private set; } = 0f;
        /// <summary>
        /// Total time of the program running, in seconds.
        /// </summary>
        public static float TIME { get; private set; } = 0f;
        /// <summary>
        /// Total time of the game running, in seconds. Stops when pausing the game.
        /// </summary>
        public static float GAME_TIME { get; private set; } = 0f;

#endregion

        private static RenderWindow Window;
        private static Dictionary<string, Font> Fonts = new ();

        public static Action<bool> ON_CHANGE_FOCUS;

        private static float RemainingFixedTime;
        public static float FIXED_TIME_STEP = 1f / 60f;

        /// <summary>
        /// Opens the game window. For custom settings: change Engine.Settings class. Run all initialisation code before this. This method won't exit until the window closes.
        /// </summary>
        public static void Start()
        {
            // Initialize Window
            Window = new (new VideoMode(Settings.WINDOW_WIDTH, Settings.WINDOW_HEIGHT), Settings.WINDOW_TITLE, Styles.Titlebar | Styles.Close);
            Window.SetFramerateLimit(Settings.TARGET_FPS);
            Clock gameClock = new ();
            RegisterEvents();
            if (!Window.HasFocus()) 
            {
                IS_FOCUSED = false;
                ON_CHANGE_FOCUS?.Invoke(IS_FOCUSED);
            }

            // Initialize Objects
            CAMERA = new (Settings.CAMERA_START_POSITION, Settings.WINDOW_SIZE, null, 10f);
            UI_CAMERA = new (Settings.WINDOW_SIZE / 2f, Settings.WINDOW_SIZE, 10f, null);

            while (Window.IsOpen)
            {
                Window.DispatchEvents();

                DELTA_TIME = gameClock.Restart().AsSeconds();
                TIME += DELTA_TIME;
                if (!IS_PAUSED) 
                    GAME_TIME += DELTA_TIME;

                HandleInput();
                Update();
                FixedUpdate();
                ForceUpdate();

                Window.Clear(Settings.BACKGROUND_COLOR);
                Draw(Window);
                DrawUI(Window);
                Window.Display();
            }
        }

        private static void RegisterEvents()
        {
            Window.Closed += (sender, e) =>
            {
                Window.Close();
            };
            Window.GainedFocus += (sender, e) =>
            {
                IS_FOCUSED = true;
                ON_CHANGE_FOCUS?.Invoke(IS_FOCUSED);
            };
            Window.LostFocus += (sender, e) =>
            {
                IS_FOCUSED = false;
                ON_CHANGE_FOCUS?.Invoke(IS_FOCUSED);
            };

            Window.KeyPressed += (sender, e) => 
            {
                INPUT.SignalAnyKeyPressed(e.Code);
            };

            Window.KeyReleased += (sender, e) => 
            {
                INPUT.SignalAnyKeyReleased(e.Code);
            };
        }

        #region runtime calls

        private static void HandleInput()
        {
            if (!IS_FOCUSED) return;

            INPUT.Update(Window);
            HandleDefaultInput();
            GameObject.HandleInputAll();

            static void HandleDefaultInput()
            {
                if (!Settings.CUSTOM_PAUSE_MENU)
                {
                    if (INPUT.GetKeyDown(Keyboard.Key.Escape)) IS_PAUSED = !IS_PAUSED;
                    if (IS_PAUSED && INPUT.GetKeyDown(Keyboard.Key.Space)) Window.Close();
                }

                CollidableObject.DRAW_COLLIDERS = INPUT.GetKey(Keyboard.Key.LAlt);
            }
        }

        private static void ForceUpdate() => GameObject.ForceUpdateAll();
        
        private static void Update()
        {
            if (IS_PAUSED) return;
            GameObject.UpdateAll();
        }

        private static void FixedUpdate() 
        {
            RemainingFixedTime += DELTA_TIME;

            while (RemainingFixedTime > 0f)
            {
                RemainingFixedTime -= FIXED_TIME_STEP;

                GameObject.FixedUpdateAll();
            }
        }

        private static void Draw(RenderWindow window) => CAMERA.Draw(window);

        private static void DrawUI(RenderWindow window)
        {
            UI_CAMERA.Draw(window);

            if (IS_PAUSED && !Settings.CUSTOM_PAUSE_MENU)
            {
                window.Draw(new RectangleShape() {
                    FillColor = new Color(0, 0, 0, 128),
                    Size = Settings.WINDOW_SIZE
                });
                Font font = GetFont();
                if (font != null)
                    window.Draw(new Text("Paused", font, 32) { Position = new (20f, 20f) });
            }
        }

#endregion

        /// <summary>
        /// Retrieve a stored font by name.
        /// </summary>
        public static Font GetFont(string name)
        {
            if (Fonts.ContainsKey(name)) return Fonts[name];
            return GetFont();
        }
        /// <summary>
        /// Retrieve a stored font
        /// </summary>
        public static Font GetFont()
        {
            if (Fonts.Count <= 0) return null;
            return Fonts.Values.FirstOrDefault();
        }
        /// <summary>
        /// Stores a font by name to use later.
        /// </summary>
        public static void AddFont(string fileName, string name) => Fonts.Add(name, new Font(Settings.ASSET_PATH + fileName));

        public static void Quit()
        {
            Window.Close();
        }

    }
}