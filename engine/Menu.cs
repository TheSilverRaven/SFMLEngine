using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SilverRaven.SFML.Tools;
using static SilverRaven.SFML.Engine;

namespace SilverRaven.SFML
{
    public class Menu : GameObject
    {
        public class MenuItem 
        {
            public virtual void HandleInput() { }
            public virtual void Draw(RenderWindow window) { }
            public virtual void Select() { }
            public virtual void Deselect() { }
            public virtual void Use() { }
            public virtual void PerformUse() { }
            public virtual void Update() { }
        }

        public bool enabled;
        protected List<MenuItem> items;
        protected MenuItem selectedItem;

        public Menu() : base(11f)
        {
            items = new();
            selectedItem = null;
        }

        public virtual void Enable() => enabled = true;
        public virtual void Disable() => enabled = false;

        protected override void HandleInput()
        {
            if (!enabled) return;
            foreach (var item in items)
                item.HandleInput();
            
            if (INPUT.GetKey(Keyboard.Key.Space)) 
                selectedItem?.Use(); // Important to be after item.HandleInput()
            if (INPUT.GetKeyUp(Keyboard.Key.Space)) 
            {
                selectedItem?.PerformUse();
                INPUT.DepleteInput(Keyboard.Key.Space);
            }
        }

        protected override void ForceUpdate()
        {
            if (!enabled) return;
            foreach (var item in items)
                item.Update();
        }

        public virtual void Select(MenuItem item)
        {
            selectedItem?.Deselect();
            selectedItem = item;
            selectedItem?.Select();
        }

        protected override void Draw(RenderWindow window)
        {
            if (!enabled) return;

            foreach (var item in items)
                item.Draw(window);
        }
    }

    public class Button : Menu.MenuItem
    {
        public Action onClick;

        protected Text label;
        protected RectangleShape shape;
        protected Menu menu;
        protected bool selected;
        protected bool pressed;
        protected bool highlighted;

        protected static Color normalColor = new Color(0, 0, 0, 64);
        protected static Color selectedColor = new Color(90, 90, 90, 96);
        protected static Color highlightedColor = new Color(255, 255, 255, 96);
        protected static Color pressedColor = new Color(128, 128, 128, 96);

        public Button(Vector2f position, Menu menu, Vector2f? size = null, string text = null)
        {
            this.menu = menu;
            Vector2f _size = size ?? new Vector2f(200f, 40f);
            shape = new ()
            {
                Size = _size,
                FillColor = Color.Black,
                Position = position,
                Origin = _size.Scale(new Vector2f(.5f, .5f))
            };
            label = new Text(text ?? "Button", GetFont(), 24) {Position = position};
            label.AlignText(.5f, 1f);
        }

        public override void HandleInput()
        {
            highlighted = shape.GetGlobalBounds().Contains((Vector2f)INPUT.MousePosition);
            pressed = highlighted && INPUT.GetMouseDown(Mouse.Button.Left);
            if (pressed && !selected) menu?.Select(this);
            if (highlighted && INPUT.GetMouseUp(Mouse.Button.Left)) PerformUse();
        }

        public override void Draw(RenderWindow window)
        {
            UpdateColors();
            window.Draw(shape);
            window.Draw(label);
        }

        protected virtual void UpdateColors()
        {
            shape.FillColor = normalColor;
            if (selected) shape.FillColor = selectedColor;
            if (highlighted) shape.FillColor = highlightedColor;
            if (pressed) shape.FillColor = pressedColor;
        }

        public void SetText(string text)
        {
            label.DisplayedString = text;
            label.AlignText(.5f, 1f);
        }

        public override void Select() => selected = true;
        public override void Deselect() => selected = false;
        public override void Use() => pressed = true;
        public override void PerformUse() => onClick?.Invoke();
    }
}