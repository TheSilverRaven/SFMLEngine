using SFML.Graphics;
using SFML.System;

namespace SilverRaven.SFML.Tools
{    
    public static class RandomExtensions
    {
        public static float NextFloat(this Random rand) => (float)rand.NextDouble();
        public static float Range(this Random rand, float min, float max) => min + rand.NextFloat() * (max - min);
        public static Vector2f OnUnitCircle(this Random rand)
        {
            float a = rand.NextFloat() * (2f * MathF.PI) - MathF.PI;
            return new Vector2f(MathF.Cos(a), MathF.Sin(a));
        }
        public static Vector2f InUnitCircle(this Random rand) => rand.OnUnitCircle() * MathF.Sqrt(rand.NextFloat());
    }

    public static class VectorExtensions
    {
        public static float Magnitude(this Vector2f v) => MathF.Sqrt(v.X * v.X + v.Y * v.Y);
        public static Vector2f Scale(this Vector2f v, Vector2f scalar)
        {
            v.X *= scalar.X;
            v.Y *= scalar.Y;
            return v;
        }
        public static Vector2f Normalized(this Vector2f v) => v / v.Magnitude();
        public static Vector2f MoveTowards(this Vector2f v, Vector2f pos, float maxDistance)
        {
            Vector2f dif = pos - v;
            if (dif.Magnitude() <= maxDistance) return pos;

            return v + dif.Normalized() * maxDistance;
        }
        public static Vector2f ClampMagnitude(this Vector2f v) => v.Magnitude() > 1f ? v / v.Magnitude() : v;
        public static Vector2f With(this Vector2f v, float? x = null, float? y = null) => new (x ?? v.X, y ?? v.Y);
        public static Vector2f Lerp(this Vector2f v, Vector2f target, float t) => new (MathTools.Lerp(v.X, target.X, t), MathTools.Lerp(v.Y, target.Y, t));
    }

    public static class MathTools
    {
        public static float Clamp01(float value) => Clamp(value, 0f, 1f);
        public static float Clamp(float value, float min, float max) => MathF.Max(MathF.Min(value, max), min);
        public static float Lerp(float a, float b, float t) => a + (b - a) * t; // linear interpolation
        public static float InverseLerp(float a, float b, float v) => (a - v) / (a + b); // inverse linear interpolation (gets the t for v = Lerp(a, b, t))
        public static float SmoothStep(float a, float b, float t) => a + (b - a) * (3.0f - t * 2.0f) * t * t; // cubic interpolation
        public static float SmootherStep(float a, float b, float t) => a + (b - a) * ((t * (t * 6.0f - 15.0f) + 10.0f) * t * t * t); // quintuple interpolation
        public static float RadToDeg() => 360f / MathF.Tau;
        public static float DegToRad() => MathF.Tau / 360f;
    }

    public static class ColorExtensions
    {
        public static Color SetAlpha(this Color c, byte a)
        {
            c.A = a;
            return c;
        }

        public static Color SetAlpha(this Color c, float a)
        {
            c.A = (byte)(a * 255);
            return c;
        }

        public static Color Tint(this Color c, Color tint)
        {
            c.R = (byte)Math.Clamp(c.R / 255f * (tint.R / 255f) * 255f, 0, 255);
            c.G = (byte)Math.Clamp(c.G / 255f * (tint.G / 255f) * 255f, 0, 255);
            c.B = (byte)Math.Clamp(c.B / 255f * (tint.B / 255f) * 255f, 0, 255);
            c.A = (byte)Math.Clamp(c.A / 255f * (tint.A / 255f) * 255f, 0, 255);
            return c;
        }

        public static Color Lerp(this Color a, Color b, float t) => new ((byte)MathTools.Lerp(a.R, b.R, t), (byte)MathTools.Lerp(a.G, b.G, t), (byte)MathTools.Lerp(a.B, b.B, t), (byte)MathTools.Lerp(a.A, b.A, t));

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

    public static class ShapeExtensions
    {
        public static void AlignText(this Text txt, float alignmentX, float alignmentY) => txt.AlignText(new Vector2f(alignmentX, alignmentY));
        public static void AlignText(this Text txt, Vector2f alignment) => txt.Origin = txt.GetLocalBounds().Size().Scale(alignment);
        public static Vector2f Center(this FloatRect bounds) => new (bounds.Width / 2f + bounds.Left, bounds.Height / 2f + bounds.Top);
        public static Vector2f TopLeft(this FloatRect bounds) => new (bounds.Left, bounds.Top);
        public static Vector2f Size(this FloatRect bounds) => new (bounds.Width, bounds.Height);
    }
}