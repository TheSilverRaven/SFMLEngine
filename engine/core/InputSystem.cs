using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace SilverRaven.SFML.Tools
{
    public class InputSystem
    {
        private class KeyObserver
        {
            private readonly Keyboard.Key key;

            public KeyObserver(Keyboard.Key key)
            {
                this.key = key;
            }

            private bool wasPressed;
            private bool isPressed;
            public bool KeyPressed() => isPressed;
            public bool KeyDown() => wasPressed == false && isPressed == true;
            public bool KeyUp() => wasPressed == true && isPressed == false;

            public void Update()
            {
                wasPressed = isPressed;
                isPressed = Keyboard.IsKeyPressed(key);
            }
        }

        private class MouseObserver
        {
            private readonly Mouse.Button button;

            public MouseObserver(Mouse.Button button)
            {
                this.button = button;
            }

            private bool wasPressed;
            private bool isPressed;
            public bool KeyPressed() => isPressed;
            public bool KeyDown() => wasPressed == false && isPressed == true;
            public bool KeyUp() => wasPressed == true && isPressed == false;

            public void Update()
            {
                wasPressed = isPressed;
                isPressed = Mouse.IsButtonPressed(button);
            }
        }

        private Dictionary<Keyboard.Key, KeyObserver> keyObservers;
        private Dictionary<Mouse.Button, MouseObserver> mouseObservers;

        private Keyboard.Key? anyPressedKey;
        private Keyboard.Key? anyReleasedKey;
        private bool anyPressedKeyUpdate;
        private bool anyReleasedKeyUpdate;

        /// <summary>
        /// The current mouse Position, relative to the current window. Updated in InputSystem.Update()
        /// </summary>
        public Vector2i MousePosition { get; private set; }

        public InputSystem()
        {
            keyObservers = new();
            mouseObservers = new();
        }

        /// <summary>
        /// Returns input of an axis as integer, ranging from -1 to 1. Works with both WASD- and Arrow-Keys
        /// </summary>
        /// <param name="axis">The axis to get input from. Either "Horizontal" or "Vertical"</param>
        /// <returns>-1 if negative key is pressed, +1 if positive key is pressed, 0 if neither are pressed</returns>
        public int GetAxis(string axis)
        {
            int input = 0;

            if (axis == "Horizontal")
            {
                if (GetKey(Keyboard.Key.D) || GetKey(Keyboard.Key.Right)) input = 1;
                if (GetKey(Keyboard.Key.A) || GetKey(Keyboard.Key.Left)) input = -1;
            }

            if (axis == "Vertical")
            {
                if (GetKey(Keyboard.Key.W) || GetKey(Keyboard.Key.Up)) input = 1;
                if (GetKey(Keyboard.Key.S) || GetKey(Keyboard.Key.Down)) input = -1;
            }

            return input;
        }

        /// <summary>
        /// Updates all observer objects as well as the mousePosition property.
        /// </summary>
        /// <param name="window">RenderWindow relative to which the mousePosition is measured</param>
        public void Update(RenderWindow window)
        {
            foreach (var obs in keyObservers.Values)
                obs.Update();
            foreach (var obs in mouseObservers.Values)
                obs.Update();
            MousePosition = Mouse.GetPosition(window);

            if (anyPressedKeyUpdate) anyPressedKey = null;
            if (anyReleasedKeyUpdate) anyReleasedKey = null;
            anyPressedKeyUpdate = true;
            anyReleasedKeyUpdate = true;
        }

        /// <summary>
        /// Returns a KeyCode of the Key that was pressed this frame
        /// </summary>
        /// <returns>KeyCode of the pressed Key, null if none was pressed</returns>
        public bool GetAnyKeyDown(out Keyboard.Key? key)
        {
            key = anyPressedKey;
            return anyPressedKey != null;
        }

        /// <summary>
        /// Returns a KeyCode of the Key that was released this frame
        /// </summary>
        /// <returns>KeyCode of the released Key, null if none was released</returns>
        public bool GetAnyKeyUp(out Keyboard.Key? key)
        {
            key = anyReleasedKey;
            return anyReleasedKey != null;
        }

        /// <summary>
        /// INTERNAL method for updating values for GetAnyKeyDown
        /// </summary>
        public void SignalAnyKeyPressed(Keyboard.Key key)
        {
            anyPressedKey = key;
            anyPressedKeyUpdate = false;
        }

        /// <summary>
        /// INTERNAL method for updating values for GetAnyKeyUp
        /// </summary>
        public void SignalAnyKeyReleased(Keyboard.Key key)
        {
            anyReleasedKey = key;
            anyReleasedKeyUpdate = false;
        }

        /// <summary>
        /// Returns true when the key was pressed down this frame
        /// </summary>
        /// <param name="key">Key to be looking for</param>
        /// <param name="depleteInput">Deactivates this bool afterwards</param>
        /// <returns></returns>
        public bool GetKeyDown(Keyboard.Key key, bool depleteInput = false) 
        {
            bool result = GetKeyDown(key);
            if (depleteInput) DepleteInput(key);
            return result;
        }
        /// <summary>
        /// Returns true when the key was pressed down this frame
        /// </summary>
        /// <param name="key">Key to be looking for</param>
        /// <returns></returns>
        public bool GetKeyDown(Keyboard.Key key) 
        {
            keyObservers.TryGetValue(key, out KeyObserver obs);
            if (obs == null)
            {
                obs = new KeyObserver(key);
                keyObservers.Add(key, obs);
                obs.Update();
            }
            return obs.KeyDown();
        }

        /// <summary>
        /// Returns true when the key is pressed down
        /// </summary>
        /// <param name="key">Key to be looking for</param>
        /// <returns></returns>
        public bool GetKey(Keyboard.Key key)
        {
            keyObservers.TryGetValue(key, out KeyObserver obs);
            if (obs == null)
            {
                obs = new KeyObserver(key);
                keyObservers.Add(key, obs);
                obs.Update();
            }
            return obs.KeyPressed();
        }

        /// <summary>
        /// Returns true when the key was released this frame
        /// </summary>
        /// <param name="key">Key to be looking for</param>
        /// <param name="depleteInput">Deactivates this bool afterwards</param>
        /// <returns></returns>
        public bool GetKeyUp(Keyboard.Key key, bool depleteInput = false) 
        {
            bool result = GetKeyUp(key);
            if (depleteInput) DepleteInput(key);
            return result;
        }
        /// <summary>
        /// Returns true when the key was released this frame
        /// </summary>
        /// <param name="key">Key to be looking for</param>
        /// <returns></returns>
        public bool GetKeyUp(Keyboard.Key key)
        {
            keyObservers.TryGetValue(key, out KeyObserver obs);
            if (obs == null)
            {
                obs = new KeyObserver(key);
                keyObservers.Add(key, obs);
                obs.Update();
            }
            return obs.KeyUp();
        }

        /// <summary>
        /// Deactivates single frame inputs like GetKeyDown() for the rest of the frame.
        /// </summary>
        /// <param name="key">Key to be looking for</param>
        public void DepleteInput(Keyboard.Key key)
        {
            keyObservers.TryGetValue(key, out KeyObserver obs);
            if (obs == null) return;
            obs.Update();
        }

        /// <summary>
        /// Returns true when the button was pressed down this frame
        /// </summary>
        /// <param name="button">Mouse button to be looking for</param>
        /// <param name="depleteInput">Deactivates this bool afterwards</param>
        /// <returns></returns>
        public bool GetMouseDown(Mouse.Button button, bool depleteInput = false) 
        {
            bool result = GetMouseDown(button);
            if (depleteInput) DepleteInput(button);
            return result;
        }
        /// <summary>
        /// Returns true when the button was pressed down this frame
        /// </summary>
        /// <param name="button">Mouse button to be looking for</param>
        /// <returns></returns>
        public bool GetMouseDown(Mouse.Button button)
        {
            mouseObservers.TryGetValue(button, out MouseObserver obs);
            if (obs == null)
            {
                obs = new MouseObserver(button);
                mouseObservers.Add(button, obs);
                obs.Update();
            }
            return obs.KeyDown();
        }

        /// <summary>
        /// Returns true when the button is pressed this frame
        /// </summary>
        /// <param name="button">Mouse button to be looking for</param>
        /// <returns></returns>
        public bool GetMouse(Mouse.Button button)
        {
            mouseObservers.TryGetValue(button, out MouseObserver obs);
            if (obs == null)
            {
                obs = new MouseObserver(button);
                mouseObservers.Add(button, obs);
                obs.Update();
            }
            return obs.KeyPressed();
        }

        /// <summary>
        /// Returns true when the button was released this frame
        /// </summary>
        /// <param name="button">Mouse button to be looking for</param>
        /// <param name="depleteInput">Deactivates this bool afterwards</param>
        /// <returns></returns>
        public bool GetMouseUp(Mouse.Button button, bool depleteInput = false) 
        {
            bool result = GetMouseUp(button);
            if (depleteInput) DepleteInput(button);
            return result;
        }
        /// <summary>
        /// Returns true when the button was released this frame
        /// </summary>
        /// <param name="button">Mouse button to be looking for</param>
        /// <returns></returns>
        public bool GetMouseUp(Mouse.Button button)
        {
            mouseObservers.TryGetValue(button, out MouseObserver obs);
            if (obs == null)
            {
                obs = new MouseObserver(button);
                mouseObservers.Add(button, obs);
                obs.Update();
            }
            return obs.KeyUp();
        }


        /// <summary>
        /// Deactivates single frame inputs like GetMouseDown() for the rest of the frame.
        /// </summary>
        /// <param name="key">Key to be looking for</param>
        public void DepleteInput(Mouse.Button button)
        {
            mouseObservers.TryGetValue(button, out MouseObserver obs);
            if (obs == null) return;
            obs.Update();
        }
    }
}
