using SFML.Graphics;
using SFML.System;
using SilverRaven.SFML.Tools;

namespace SilverRaven.SFML
{
    public class CollidableObject : GameObject
    {
        public static bool DRAW_COLLIDERS { get; set; }
        
        public FloatRect Collider { get; protected set; }
        private Vector2f origin;
        public bool drawCollider = false;

        public CollidableObject(Vector2f? collider = null, Vector2f? origin = null, float? sortOrder = null) : base(sortOrder)
        {
            Collider = new FloatRect(new Vector2f(), collider ?? new Vector2f());
            this.origin = origin ?? new Vector2f();
        }

        protected override void OnUpdatePosition()
        {
            base.OnUpdatePosition();
            Collider = Collider.SetPosition(position - origin);
        }

        /// <summary>
        /// Check whether this object collides with one specific other CollidableObject
        /// </summary>
        /// <param name="obj">The object this object may collide with</param>
        public bool CollidesWith(CollidableObject obj) => Collider.Intersects(obj.Collider);

        /// <summary>
        /// Check whether this object collides with any other CollidableObject
        /// </summary>
        /// <param name="o">The object this object collides with. Null when there's no collision.</param>
        /// <returns>Whether this object collides with any other CollidableObject</returns>
        public bool CollideAll(out CollidableObject o)
        {
            foreach (CollidableObject obj in GetAllObjects<CollidableObject>())
            {
                if (obj == this) continue;
                if (Collider.Intersects(obj.Collider))
                {
                    o = obj;
                    return true;
                }
            }
            o = null;
            return false;
        }
        /// <summary>
        /// Check whether this object collides with any other of the given type
        /// </summary>
        /// <param name="t">The object this object collides with. Null when there's no collision.</param>
        /// <typeparam name="T">The type to filter collisions with</typeparam>
        /// <returns>Whether this object collides with any other of the given type</returns>
        public bool CollideAll<T>(out T t) where T : CollidableObject
        {
            foreach (T obj in GetAllObjects<T>())
            {
                if (obj == this) continue;

                if (Collider.Intersects(obj.Collider))
                {
                    t = obj;
                    return true;
                }
            }
            t = null;
            return false;
        }
        /// <summary>
        /// Check whether this object collides with any other of the given type
        /// </summary>
        /// <typeparam name="T">The type to filter collisions with</typeparam>
        /// <returns>Whether this object collides with any other CollidableObject of the given typeT</returns>
        public bool CollideAll<T>() where T : CollidableObject
        {
            foreach (T obj in GetAllObjects<T>())
            {
                if (obj == this) continue;
                if (Collider.Intersects(obj.Collider)) return true;
            }
            return false;
        }

        /// <summary>
        /// Draw the Sprite to the RenderWindow. Use drawCollider or CollidableObject.drawColliders toggles to draw collider bounds.
        /// </summary>
        protected override void Draw(RenderWindow window)
        {
            base.Draw(window);
            
            if (drawCollider || DRAW_COLLIDERS) DrawCollider();

            void DrawCollider()
            {
                window.Draw(new RectangleShape() {
                    FillColor = Color.Transparent,
                    OutlineColor = Color.Green,
                    OutlineThickness = 1f,
                    Position = Collider.Position(),
                    Size = Collider.Size()
                });
            }
        }
    }
}