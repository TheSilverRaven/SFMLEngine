using SFML.System;

namespace SilverRaven.SFML.Tools
{
    /// <summary>
    /// Provides extension methods for the <see cref="Random"/> class.
    /// </summary>
    public static class RandomExtensions
    {
        /// <summary>
        /// Generates a random float value between 0.0 and 1.0.
        /// </summary>
        public static float NextFloat(this Random rand) => (float)rand.NextDouble();

        /// <summary>
        /// Generates a random float value within the specified range.
        /// </summary>
        /// <param name="min">The minimum value of the range.</param>
        /// <param name="max">The maximum value of the range.</param>
        public static float Range(this Random rand, float min, float max) => min + rand.NextFloat() * (max - min);

        /// <summary>
        /// Generates a random point on the unit circle.
        /// </summary>
        public static Vector2f OnUnitCircle(this Random rand)
        {
            float a = rand.NextFloat() * (2f * MathF.PI) - MathF.PI;
            return new Vector2f(MathF.Cos(a), MathF.Sin(a));
        }

        /// <summary>
        /// Generates a random point within the unit circle.
        /// </summary>
        public static Vector2f InUnitCircle(this Random rand) => rand.OnUnitCircle() * MathF.Sqrt(rand.NextFloat());
    }
}