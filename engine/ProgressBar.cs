
using SFML.Graphics;
using SFML.System;
using static SilverRaven.SFML.Tools.MathTools;
using static SilverRaven.SFML.Engine;

namespace SilverRaven.SFML
{
    public class ProgressBar : Menu.MenuItem
    {
        private RectangleShape background;
        private RectangleShape fill;
        private float progress;
        private float smoothingAmount;

        public ProgressBar(Vector2f position, Vector2f size, float smoothingAmount = 5f, Color? backgroundColor = null, Color? fillColor = null, float progress = 0f)
        {
            background = new RectangleShape() {
                Position = position,
                Size = size,
                FillColor = backgroundColor ?? Color.Black,
                OutlineColor = Color.Black,
                OutlineThickness = 2f
            };
            fill = new RectangleShape() {
                Position = position,
                Size = size,
                FillColor = fillColor ?? Color.Red,
                Scale = new Vector2f(0f, 1f)
            };
            this.smoothingAmount = smoothingAmount;
            this.progress = progress;
        }

        public ProgressBar(RectangleShape background, RectangleShape fill, float smoothingAmount = 5f, float progress = 0f)
        {
            this.background = background;
            this.fill = fill;
            this.smoothingAmount = smoothingAmount;
            this.progress = progress;
        }

        public virtual void SetValue(float value) => progress = value;
        public virtual void SetValue(float value, float min, float max) => progress = InverseLerp(min, max, value);
        public override void Update() => fill.Scale = new (Lerp(fill.Scale.X, progress, DELTA_TIME * smoothingAmount), 1f);

        public override void Draw(RenderWindow window)
        {
            window.Draw(background);
            window.Draw(fill);
        }
    }
}