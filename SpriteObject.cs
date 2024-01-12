using SFML.Graphics;
using SFML.System;

using SilverRaven.SFML.Tools;

namespace SilverRaven.SFML
{
    public class SpriteObject : Object
    {
        private const string TEXTURE_PATH = "sprites.png";
        public const float PIXEL_SCALE = 4f;
        private static Texture tex;
        public static bool drawColliders;

        public Sprite sprite;
        public FloatRect collider;
        public bool drawCollider = false;

        private Vector2f position;
        public Vector2f Position {
            get => position;
            set {
                position = value;

                collider.SetPosition(position);
                sprite.Position = position;
            }
        }

        public SpriteObject(Vector2f position, IntRect textureRect, FloatRect collider) : base()
        {
            sprite = GetSprite(textureRect);

            collider.Width *= PIXEL_SCALE;
            collider.Height *= PIXEL_SCALE;
            collider.Top *= PIXEL_SCALE;
            collider.Left *= PIXEL_SCALE;
            this.collider = collider;

            Position = position;
        }

        /// <summary>
        /// Creates a Sprite object from the texture
        /// </summary>
        /// <param name="texRect">The Rectangle on the Texture</param>
        public static Sprite GetSprite(IntRect texRect)
        {
            tex ??= new (TEXTURE_PATH);

            Sprite sprite = new (tex) {
                TextureRect = texRect,
                Scale = new Vector2f(PIXEL_SCALE, PIXEL_SCALE)
            };
            return sprite;
        }

        public FloatRect GetCollider() => collider.Move(Position);

        /// <summary>
        /// Check whether this object collides with one specific other SpriteObject
        /// </summary>
        /// <param name="obj">The object this object may collide with</param>
        public bool CollidesWith(SpriteObject obj) => GetCollider().Intersects(obj.GetCollider());

        /// <summary>
        /// Check whether this object collides with any other of the given type
        /// </summary>
        /// <param name="t">The object this object collides with. Null when there's no collision.</param>
        /// <typeparam name="T">The type to filter collisions with</typeparam>
        /// <returns>Whether this object collides with any other of the given type</returns>
        public bool CollideAll<T>(out T t) where T : SpriteObject
        {
            FloatRect coll = GetCollider();
            foreach (Object obj in GetAllObjects())
            {
                if (obj == this) continue;
                if (obj is not T) continue;

                T objT = obj as T;
                if (coll.Intersects(objT.GetCollider()))
                {
                    t = objT;
                    return true;
                }
            }
            t = null;
            return false;
        }
        /// <summary>
        /// Check whether this object collides with any other SpriteObject
        /// </summary>
        /// <param name="t">The object this object collides with. Null when there's no collision.</param>
        /// <returns>Whether this object collides with any other SpriteObject</returns>
        public bool CollideAll(out SpriteObject o)
        {
            FloatRect coll = GetCollider();
            foreach (Object obj in GetAllObjects())
            {
                if (obj == this) continue;
                if (obj is not SpriteObject) continue;

                SpriteObject sObj = obj as SpriteObject;
                if (coll.Intersects(sObj.GetCollider()))
                {
                    o = sObj;
                    return true;
                }
            }
            o = null;
            return false;
        }

        /// <summary>
        /// Draw the Sprite to the RenderWindow. Use drawCollider or SpriteObject.drawColliders toggles to draw collider bounds.
        /// </summary>
        protected override void Draw(RenderWindow window)
        {
            window.Draw(sprite);

            if (drawCollider || drawColliders) DrawCollider();
            void DrawCollider()
            {
                FloatRect coll = GetCollider();
                window.Draw(new RectangleShape() {
                    FillColor = Color.Transparent,
                    OutlineColor = Color.Green,
                    OutlineThickness = 1f,
                    Position = coll.Position(),
                    Size = coll.Size()
                });
            }
        }
    }
}