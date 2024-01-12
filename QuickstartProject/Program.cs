using SFML.System;
using SFML.Graphics;
using SFML.Window;

using SilverRaven.SFML.Tools;

namespace SilverRaven.SFML.Quickstart
{
    public static class Program
    {
        public const int WINDOW_WIDTH = 800;
        public const int WINDOW_HEIGHT = 600;
        public const uint TARGET_FPS = 60;
        public static Vector2f WINDOW_SIZE => new (WINDOW_WIDTH, WINDOW_HEIGHT);

        public static bool IS_PAUSED;
        public static float DELTA_TIME;
        public static float TIME;
        public static Random RANDOM;

        private static InputSystem input;

        static void Main()
        {
            InitializeObjects();
            InitializeWindow(out RenderWindow window);
            Clock gameClock = new ();

            while (true)
            {
                window.DispatchEvents();

                DELTA_TIME = gameClock.Restart().AsSeconds();
                TIME += DELTA_TIME;

                input.Update(window);
                HandleInput();

                ForceUpdate();
                if (!IS_PAUSED) Update();

                window.Clear(new Color(30, 30, 30));
                Draw(window);
                window.Display();
            }
        }

        /// <summary>
        /// Initialize Objects like sounds, fonts and other custom classes like Player or Level
        /// </summary>
        private static void InitializeObjects()
        {
            input = new InputSystem();
            RANDOM = new ();
            
            // ...
            Object.CreateInstance(new Player());
        }

        private static void InitializeWindow(out RenderWindow window)
        {
            window = new (new VideoMode(WINDOW_WIDTH, WINDOW_HEIGHT), "My Game");
            window.SetFramerateLimit(TARGET_FPS);
            TIME = 0f;
        }

        private static void HandleInput()
        {
            if (input.GetKeyDown(Keyboard.Key.Escape)) IS_PAUSED = !IS_PAUSED;
            if (IS_PAUSED && input.GetKeyDown(Keyboard.Key.Space)) Environment.Exit(0);

            Object.HandleInputAll(input);
        }

        private static void ForceUpdate()
        {
            Object.ForceUpdateAll();
        }

        private static void Update()
        {
            Object.UpdateAll();
        }

        private static void Draw(RenderWindow window)
        {
            Object.DrawAll(window);

            if (IS_PAUSED)
            {
                window.Draw(new RectangleShape() {
                    FillColor = new Color(0, 0, 0, 128),
                    Size = WINDOW_SIZE
                });
            }
        }
    }
}