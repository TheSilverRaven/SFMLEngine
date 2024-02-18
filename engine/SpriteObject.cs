using SFML.Graphics;
using SFML.System;
using static SilverRaven.SFML.Engine;

namespace SilverRaven.SFML
{
    public class SpriteObject : CollidableObject
    {
        public Sprite sprite;

        public SpriteObject(IntRect textureRect, Vector2f colliderSize, Vector2f colliderOrigin, Texture texture, float? sortOrder = null) : base(colliderSize, colliderOrigin, sortOrder)
        {
            sprite = new (texture) {
                TextureRect = textureRect,
                Scale = new Vector2f(Settings.PIXEL_SCALE, Settings.PIXEL_SCALE)
            };
        }

        protected override void OnUpdatePosition()
        {
            base.OnUpdatePosition();
            sprite.Position = position;
        }

        protected override void Draw(RenderWindow window)
        {
            window.Draw(sprite);
            base.Draw(window);
        }
    }
}