using SFML.Graphics;
using SFML.System;

namespace SilverRaven.SFML.Tools
{
    public static class SFMLExtensions
    {
        public static Text AlignText(this Text txt, float alignmentX, float alignmentY) => txt.AlignText(new Vector2f(alignmentX, alignmentY));
        public static Text AlignText(this Text txt, Vector2f alignment) 
        {
            txt.Origin = txt.GetLocalBounds().Size().Scale(alignment);
            return txt;
        }
        public static Vector2f Center(this FloatRect rect) => new (rect.Width / 2f + rect.Left, rect.Height / 2f + rect.Top);
        public static Vector2f Position(this FloatRect rect) => new (rect.Left, rect.Top);
        public static FloatRect SetComponent(this FloatRect rect, float? left = null, float? top = null, float? width = null, float? height = null)
        {
            rect.Left = left ?? rect.Left;
            rect.Top = top ?? rect.Top;
            rect.Width = width ?? rect.Width;
            rect.Height = height ?? rect.Height;
            return rect;
        }
        public static FloatRect SetCenter(this FloatRect rect, Vector2f center)
        {
            Vector2f dif = center - rect.Center();
            rect.Left += dif.X;
            rect.Top += dif.Y;
            return rect;
        }
        public static FloatRect SetPosition(this FloatRect rect, Vector2f position)
        {
            Vector2f dif = position - rect.Position();
            rect.Left += dif.X;
            rect.Top += dif.Y;
            return rect;
        }
        public static FloatRect Move(this FloatRect rect, Vector2f offset)
        {
            rect.Left += offset.X;
            rect.Top += offset.Y;
            return rect;
        }
        public static bool Contains(this FloatRect rect, Vector2f point) => rect.Contains(point.X, point.Y);
        public static bool Contains(this IntRect rect, Vector2i point) => rect.Contains(point.X, point.Y);
        public static Vector2f Size(this FloatRect bounds) => new (bounds.Width, bounds.Height);
        public static Vector2i Size(this IntRect bounds) => new (bounds.Width, bounds.Height);
        public static IntRect SetComponent(this IntRect rect, int? left = null, int? top = null, int? width = null, int? height = null)
        {
            rect.Left = left ?? rect.Left;
            rect.Top = top ?? rect.Top;
            rect.Width = width ?? rect.Width;
            rect.Height = height ?? rect.Height;
            return rect;
        }


        public static Color SetAlpha(this Color c, byte a) => new (c.R, c.G, c.B, a);
        public static Color SetAlpha(this Color c, float a) => new (c.R, c.G, c.B, (byte)(a * 255));

        public static Color Tint(this Color c, Color tint) => new (
            (byte)Math.Clamp(c.R / 255f * (tint.R / 255f) * 255f, 0, 255), // convert from byte to float (range 0-1), multiply, convert back to byte
            (byte)Math.Clamp(c.G / 255f * (tint.G / 255f) * 255f, 0, 255),
            (byte)Math.Clamp(c.B / 255f * (tint.B / 255f) * 255f, 0, 255),
            (byte)Math.Clamp(c.A / 255f * (tint.A / 255f) * 255f, 0, 255)
        );

        public static Color Lerp(this Color a, Color b, float t) => new (
            (byte)MathTools.Lerp(a.R, b.R, t), 
            (byte)MathTools.Lerp(a.G, b.G, t), 
            (byte)MathTools.Lerp(a.B, b.B, t), 
            (byte)MathTools.Lerp(a.A, b.A, t)
        );

        /// <summary>
        /// Gets the HSV Components of the given color
        /// </summary>
        /// <param name="hue">Hue in range 0-1</param>
        /// <param name="saturation">Saturation in range 0-1</param>
        /// <param name="value">Value/Brightness in range 0-1</param>
        public static void ColorToHSV(this Color color, out double hue, out double saturation, out double value)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            hue = GetHue(color.R, color.G, color.B) / 360f;
            saturation = (max == 0) ? 0 : 1 - ((double)min / max);
            value = max / 255d;

            float GetHue(byte R, byte G, byte B) // in degrees
            {
                if (R == G && G == B) return 0; // UNDEFINED
 
                byte max = Math.Max(R, Math.Max(G, B));
                byte min = Math.Min(R, Math.Min(G, B));

                float delta = (max - min) / 255f;
                float r = R / 255f;
                float g = G / 255f; 
                float b = B / 255f; 
    
                float hue = 0f;
                if (r == max) hue = (g - b) / delta; 
                else 
                if (g == max) hue = 2f + (b - r) / delta; 
                else 
                if (b == max) hue = 4f + (r - g) / delta;

                hue *= 60f;
    
                if (hue < 0f) hue += 360f;
                return hue;
            }
        }

        /// <summary>
        /// Set HSV values of a given color
        /// </summary>
        /// <param name="hue">Hue in range 0-1</param>
        /// <param name="saturation">Saturation in range 0-1</param>
        /// <param name="value">Value/Brightness in range 0-1</param>
        public static Color SetHSV(this Color color, double hue, double saturation, double value)
        {
            hue *= 360;
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value *= 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            return hi switch
            {
                0 => new Color((byte)v, (byte)t, (byte)p, 255),
                1 => new Color((byte)q, (byte)v, (byte)p, 255),
                2 => new Color((byte)p, (byte)v, (byte)t, 255),
                3 => new Color((byte)p, (byte)q, (byte)v, 255),
                4 => new Color((byte)t, (byte)p, (byte)v, 255),
                _ => new Color((byte)v, (byte)p, (byte)q, 255),
            };
        }
    }
}