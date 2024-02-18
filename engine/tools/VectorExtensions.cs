using SFML.System;

namespace SilverRaven.SFML.Tools
{
    /// <summary>
    /// Provides extension methods for the <see cref="Vector2f"/> class.
    /// </summary>
    public static class VectorExtensions
    {
        /// <summary>
        /// Calculates the magnitude (length) of the vector.
        /// </summary>
        public static float Magnitude(this Vector2f v) => MathF.Sqrt(v.X * v.X + v.Y * v.Y);

        /// <summary>
        /// Scales the vector by the specified scalar values.
        /// </summary>
        /// <param name="xScalar">The scalar value for the X component (default is 1).</param>
        /// <param name="yScalar">The scalar value for the Y component (default is 1).</param>
        public static Vector2f Scale(this Vector2f v, float xScalar = 1f, float yScalar = 1f) => v.Scale(new Vector2f(xScalar, yScalar));

        /// <summary>
        /// Scales the vector by the specified scalar vector.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="scalar">The scalar vector.</param>
        public static Vector2f Scale(this Vector2f v, Vector2f scalar)
        {
            v.X *= scalar.X;
            v.Y *= scalar.Y;
            return v;
        }

        /// <summary>
        /// Normalizes the vector.
        /// </summary>
        public static Vector2f Normalize(this Vector2f v) => v / v.Magnitude();

        /// <summary>
        /// Moves the vector towards the specified position by the specified maximum distance.
        /// </summary>
        /// <param name="pos">The target position.</param>
        /// <param name="maxDistance">The maximum distance to move.</param>
        public static Vector2f MoveTowards(this Vector2f v, Vector2f pos, float maxDistance)
        {
            Vector2f dif = pos - v;
            if (dif.Magnitude() <= maxDistance) return pos;

            return v + dif.Normalize() * maxDistance;
        }

        /// <summary>
        /// Clamps the magnitude of the vector to the specified maximum magnitude.
        /// </summary>
        /// <param name="maxMagnitude">The maximum magnitude (default is 1).</param>
        public static Vector2f ClampMagnitude(this Vector2f v, float maxMagnitude = 1f) => v.Magnitude() > maxMagnitude ? v.Normalize() * maxMagnitude : v;

        /// <summary>
        /// Creates a vector with the specified component values.
        /// </summary>
        /// <param name="x">The X component value (default is null).</param>
        /// <param name="y">The Y component value (default is null).</param>
        public static Vector2f With(this Vector2f v, float? x = null, float? y = null) => new (x ?? v.X, y ?? v.Y);

        /// <summary>
        /// Performs a linear interpolation between two vectors.
        /// </summary>
        /// <param name="target">The target vector.</param>
        /// <param name="t">The interpolation parameter.</param>
        /// <returns>The interpolated vector.</returns>
        public static Vector2f Lerp(this Vector2f v, Vector2f target, float t) => new (MathTools.Lerp(v.X, target.X, t), MathTools.Lerp(v.Y, target.Y, t));

        /// <summary>
        /// Flips the vector horizontally.
        /// </summary>
        public static Vector2f FlipX(this Vector2f v) => new (-v.X, v.Y);

        /// <summary>
        /// Flips the vector vertically.
        /// </summary>
        public static Vector2f FlipY(this Vector2f v) => new (v.X, -v.Y);

        /// <summary>
        /// Calculates the distance between two vectors.
        /// </summary>
        /// <param name="other">The other vector.</param>
        public static float Distance(this Vector2f v, Vector2f other) => (other - v).Magnitude();

        /// <summary>
        /// Calculates the absolute values of the vector components.
        /// </summary>
        public static Vector2f Abs(this Vector2f v) => new (MathF.Abs(v.X), MathF.Abs(v.Y));

        /// <summary>
        /// Rounds the vector components to the nearest integer values.
        /// </summary>
        public static Vector2f Round(this Vector2f v) => new (MathF.Round(v.X), MathF.Round(v.Y));

        /// <summary>
        /// Rounds the vector components down to the nearest integer values.
        /// </summary>
        public static Vector2f Floor(this Vector2f v) => new (MathF.Floor(v.X), MathF.Floor(v.Y));

        /// <summary>
        /// Rounds the vector components up to the nearest integer values.
        /// </summary>
        public static Vector2f Ceil(this Vector2f v) => new (MathF.Ceiling(v.X), MathF.Ceiling(v.Y));

        /// <summary>
        /// Calculates the dot product of two vectors.
        /// </summary>
        /// <param name="other">The other vector.</param>
        public static float Dot(this Vector2f v, Vector2f other) => v.X * other.X + v.Y * other.Y;

        /// <summary>
        /// Calculates the angle of the vector (in radians).
        /// </summary>
        public static float Angle(this Vector2f v) => MathF.Atan2(v.Y, v.X);

        /// <summary>
        /// Rotates the vector by the specified angle.
        /// </summary>
        /// <param name="angle">The rotation angle (in radians).</param>
        public static Vector2f Rotate(this Vector2f v, float angle)
        {
            float cos = MathF.Cos(angle);
            float sin = MathF.Sin(angle);
            return new Vector2f(v.X * cos - v.Y * sin, v.X * sin + v.Y * cos);
        }
    }
}