using SFML.Graphics;
using SFML.System;
using SilverRaven.SFML.Tools;

namespace SilverRaven.SFML.Shapes
{
    public class GradientShape : Transformable, Drawable
    {
        public enum GradientDirection
        { Up, Down, Left, Right, TopLeft, TopRight, LowerLeft, LowerRight }

        private VertexArray vertices;
        private Color color1;
        private Color color2;
        private Color color3;
        private Color color4;

        private Vector2f size;
        public Vector2f Size {
            get => size;
            set {
                size = value;
                Update();
            }
        }

        public GradientShape()
        {
            vertices = new VertexArray(PrimitiveType.TriangleStrip, 4);
            SetColors(Color.Transparent);
        }
        public GradientShape(Color color)
        {
            vertices = new VertexArray(PrimitiveType.TriangleStrip, 4);
            SetColors(color);
        }
        public GradientShape(Color colorA, Color colorB, GradientDirection direction)
        {
            vertices = new VertexArray(PrimitiveType.TriangleStrip, 4);
            SetColors(colorA, colorB, direction);
        }
        public GradientShape(Color colorA, Color colorB, Color colorC, Color colorD)
        {
            vertices = new VertexArray(PrimitiveType.TriangleStrip, 4);
            SetColors(colorA, colorB, colorC, colorD);
        }

        private void Update()
        {
            // Color order:
            //              1  /3
            //              | / |
            //              2/  4

            vertices[0] = new (new Vector2f(0f, 0f), color1);
            vertices[1] = new (new Vector2f(0f, size.Y), color2);
            vertices[2] = new (new Vector2f(size.X, 0f), color3);
            vertices[3] = new (size, color4);
        }

        /// <summary>
        /// Set the Gradient to one solid color.
        /// </summary>
        public void SetColors(Color color)
        {
            color1 = color;
            color2 = color;
            color3 = color;
            color4 = color;
            Update();
        }

        /// <summary>
        /// Set the Colors to a linear gradient from colorA to colorB
        /// </summary>
        /// <param name="colorA">Starting color of the gradient</param>
        /// <param name="colorB">Ending color of the gradient</param>
        /// <param name="direction">Direction of the gradient to point towards</param> <summary>
        public void SetColors(Color colorA, Color colorB, GradientDirection direction)
        {
            // Color order:
            //              1  /3
            //              | / |
            //              2/  4

            Color mid = colorA.Lerp(colorB, .5f);
            switch (direction)
            {
                case GradientDirection.Up: 
                    color1 = colorB;
                    color2 = colorA;
                    color3 = colorB;
                    color4 = colorA;
                    break;
                case GradientDirection.Down: 
                    color1 = colorA;
                    color2 = colorB;
                    color3 = colorA;
                    color4 = colorB;
                    break;
                case GradientDirection.Left: 
                    color1 = colorB;
                    color2 = colorB;
                    color3 = colorA;
                    color4 = colorA;
                    break;
                default: case GradientDirection.Right: 
                    color1 = colorA;
                    color2 = colorA;
                    color3 = colorB;
                    color4 = colorB;
                    break;
                case GradientDirection.TopLeft: 
                    color1 = colorB;
                    color2 = mid;
                    color3 = mid;
                    color4 = colorA;
                    break;
                case GradientDirection.TopRight: 
                    color1 = mid;
                    color2 = colorA;
                    color3 = colorB;
                    color4 = mid;
                    break;
                case GradientDirection.LowerLeft: 
                    color1 = mid;
                    color2 = colorB;
                    color3 = colorA;
                    color4 = mid;
                    break;
                case GradientDirection.LowerRight: 
                    color1 = colorA;
                    color2 = mid;
                    color3 = mid;
                    color4 = colorB;
                    break;
            }
            Update();
        }

        /// <summary>
        /// Set the exact colors of all four courners of the gradient
        /// </summary>
        /// <param name="colorA">Top left color</param>
        /// <param name="colorB">Bottom left color</param>
        /// <param name="colorC">Top right color</param>
        /// <param name="colorD">Bottom right color</param>
        public void SetColors(Color colorA, Color colorB, Color colorC, Color colorD)
        {
            color1 = colorA;
            color2 = colorB;
            color3 = colorC;
            color4 = colorD;
            Update();
        }

        /// <summary>
        /// Set the exact colors of one or more courners of the gradient
        /// </summary>
        /// <param name="colorA">Top left color</param>
        /// <param name="colorB">Bottom left color</param>
        /// <param name="colorC">Top right color</param>
        /// <param name="colorD">Bottom right color</param>
        public void SetColors(Color? colorA = null, Color? colorB = null, Color? colorC = null, Color? colorD = null)
        {
            color1 = colorA ?? color1;
            color2 = colorB ?? color2;
            color3 = colorC ?? color3;
            color4 = colorD ?? color4;
            Update();
        }

        /// <summary>
        /// Draw the Gradient Object to the given RenderTarget
        /// </summary>
        public void Draw(RenderTarget target) => Draw(target, RenderStates.Default);
        /// <summary>
        /// Draw the Gradient Object to the given RenderTarget using the RenderStates
        /// </summary>
        public void Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;
            target.Draw(vertices, states);
        }
    }
}