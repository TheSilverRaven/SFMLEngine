namespace SilverRaven.SFML.Tools
{
    /// <summary>
    /// Collection of math utility functions.
    /// </summary>
    public static class MathTools
    {
        /// <summary>
        /// Clamps a value between 0 and 1.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        public static float Clamp01(float value) => Clamp(value, 0f, 1f);

        /// <summary>
        /// Clamps a value between a minimum and maximum value.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>The clamped value.</returns>
        public static float Clamp(float value, float min, float max) => MathF.Max(MathF.Min(value, max), min);

        /// <summary>
        /// Performs linear interpolation between two values.
        /// </summary>
        /// <param name="a">The starting value.</param>
        /// <param name="b">The ending value.</param>
        /// <param name="t">The interpolation factor.</param>
        /// <returns>The interpolated value.</returns>
        public static float Lerp(float a, float b, float t) => a + (b - a) * t; // linear interpolation

        /// <summary>
        /// Calculates the inverse linear interpolation value for a given value between two specified values.
        /// </summary>
        /// <param name="a">The starting value.</param>
        /// <param name="b">The ending value.</param>
        /// <param name="v">The value to calculate the inverse linear interpolation for.</param>
        /// <returns>The inverse linear interpolation value.</returns>
        public static float InverseLerp(float a, float b, float v) => (a - v) / (a + b); // inverse linear interpolation (gets the t for v = Lerp(a, b, t))

        public static float SmoothStep(float a, float b, float t) => a + (b - a) * (3.0f - t * 2.0f) * t * t; // cubic interpolation

        public static float SmootherStep(float a, float b, float t) => a + (b - a) * ((t * (t * 6.0f - 15.0f) + 10.0f) * t * t * t); // quintuple interpolation

        /// <summary>
        /// Converts radians to degrees.
        /// </summary>
        /// <returns>The conversion factor from radians to degrees.</returns>
        public static float RadToDeg() => 360f / MathF.Tau;

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <returns>The conversion factor from degrees to radians.</returns>
        public static float DegToRad() => MathF.Tau / 360f;
        
        /// <summary>
        /// Repeats a value within a specified range.
        /// </summary>
        /// <param name="value">The value to repeat.</param>
        /// <param name="min">The minimum value of the range.</param>
        /// <param name="max">The maximum value of the range.</param>
        /// <returns>The repeated value within the specified range.</returns>
        public static float Repeat(float value, float min, float max)
        {
            float range = max - min;
            float result = (value - min) % range;
            if (result < 0) result += range;
            return result + min;
        }
    }
}