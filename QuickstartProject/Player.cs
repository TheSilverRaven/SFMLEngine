using SFML.Graphics;
using SFML.System;
using SilverRaven.SFML.Tools;

namespace SilverRaven.SFML.Quickstart
{
    public class Player : Object
    {
        private CircleShape shape;
        private float speed;
        private Vector2f moveInput;

        public Player() : base()
        {
            shape = new () {
                Radius = 20f,
                Position = Program.WINDOW_SIZE / 2f,
                FillColor = Color.Cyan
            };
            speed = 200f;
        }

        protected override void HandleInput(InputSystem input)
        {
            moveInput = moveInput.Lerp(new Vector2f(input.GetAxis("Horizontal"), -input.GetAxis("Vertical")).ClampMagnitude(), 10f * Program.DELTA_TIME);
        }

        protected override void Update()
        {
            shape.Position += moveInput * speed * Program.DELTA_TIME;
        }

        protected override void Draw(RenderWindow window)
        {
            window.Draw(shape);
        }
    }
}