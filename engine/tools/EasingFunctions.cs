namespace SilverRaven.SFML.Tools
{
    public static class EasingFunctions // Written by GH Copilot, for easing functions, see https://easings.net
    {
        // Quad
        public static float EaseInQuad(this float t) => t * t;
        public static float EaseOutQuad(this float t) => t * (2 - t);
        public static float EaseInOutQuad(this float t) => t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t;

        // Sine
        public static float EaseInSine(this float t) => 1 - (float)Math.Cos((t * Math.PI) / 2);
        public static float EaseOutSine(this float t) => (float)Math.Sin((t * Math.PI) / 2);
        public static float EaseInOutSine(this float t) => -(float)Math.Cos(Math.PI * t) / 2 + 0.5f;

        // Cubic
        public static float EaseInCubic(this float t) => t * t * t;
        public static float EaseOutCubic(this float t) => 1 - (float)Math.Pow(1 - t, 3);
        public static float EaseInOutCubic(this float t) => t < 0.5f ? 4 * t * t * t : 1 - (float)Math.Pow(-2 * t + 2, 3) / 2;

        // Quart
        public static float EaseInQuart(this float t) => t * t * t * t;
        public static float EaseOutQuart(this float t) => 1 - (float)Math.Pow(1 - t, 4);
        public static float EaseInOutQuart(this float t) => t < 0.5f ? 8 * t * t * t * t : 1 - (float)Math.Pow(-2 * t + 2, 4) / 2;

        // Quint
        public static float EaseInQuint(this float t) => t * t * t * t * t;
        public static float EaseOutQuint(this float t) => 1 - (float)Math.Pow(1 - t, 5);
        public static float EaseInOutQuint(this float t) => t < 0.5f ? 16 * t * t * t * t * t : 1 - (float)Math.Pow(-2 * t + 2, 5) / 2;

        // Expo
        public static float EaseInExpo(this float t) => (float)(t == 0 ? 0 : Math.Pow(2, 10 * t - 10));
        public static float EaseOutExpo(this float t) => (float)(t == 1 ? 1 : 1 - Math.Pow(2, -10 * t));
        public static float EaseInOutExpo(this float t) => (float)(t == 0 ? 0 : t == 1 ? 1 : t < 0.5 ? Math.Pow(2, 20 * t - 10) / 2 : (2 - Math.Pow(2, -20 * t + 10)) / 2);

        // Circ
        public static float EaseInCirc(this float t) => 1 - (float)Math.Sqrt(1 - Math.Pow(t, 2));
        public static float EaseOutCirc(this float t) => (float)Math.Sqrt(1 - Math.Pow(t - 1, 2));
        public static float EaseInOutCirc(this float t) => (float)(t < 0.5 ? (1 - Math.Sqrt(1 - Math.Pow(2 * t, 2))) / 2 : (Math.Sqrt(1 - Math.Pow(-2 * t + 2, 2)) + 1) / 2);

        // Back
        public static float EaseInBack(this float t)
        {
            float c1 = 1.70158f;
            float c3 = c1 + 1;

            return c3 * t * t * t - c1 * t * t;
        }
        public static float EaseOutBack(this float t)
        {
            float c1 = 1.70158f;
            float c3 = c1 + 1;

            return 1 + c3 * (float)Math.Pow(t - 1, 3) + c1 * (float)Math.Pow(t - 1, 2);
        }
        public static float EaseInOutBack(this float t)
        {
            float c1 = 1.70158f;
            float c2 = c1 * 1.525f;

            return (float)(t < 0.5 ? (Math.Pow(2 * t, 2) * ((c2 + 1) * 2 * t - c2)) / 2 : (Math.Pow(2 * t - 2, 2) * ((c2 + 1) * (t * 2 - 2) + c2) + 2) / 2);
        }

        // Elastic
        public static float EaseInElastic(this float t) => t == 0 ? 0 : t == 1 ? 1 : -(float)Math.Pow(2, 10 * t - 10) * (float)Math.Sin((t * 10 - 10.75) * ((2 * Math.PI) / 3));
        public static float EaseOutElastic(this float t) => t == 0 ? 0 : t == 1 ? 1 : (float)Math.Pow(2, -10 * t) * (float)Math.Sin((t * 10 - 0.75) * ((2 * Math.PI) / 3)) + 1;
        public static float EaseInOutElastic(this float t) => t == 0 ? 0 : t == 1 ? 1 : t < 0.5 ? -(float)Math.Pow(2, 20 * t - 10) * (float)Math.Sin((20 * t - 11.125) * ((2 * Math.PI) / 4.5)) / 2 : (float)Math.Pow(2, -20 * t + 10) * (float)Math.Sin((20 * t - 11.125) * ((2 * Math.PI) / 4.5)) / 2 + 1;

        // Bounce
        public static float EaseInBounce(this float t) => 1 - EaseOutBounce(1 - t);
        public static float EaseOutBounce(this float t) => t < 1 / 2.75f ? (float)(7.5625f * t * t) : t < 2 / 2.75f ? (float)(7.5625f * (t -= 1.5f / 2.75f) * t + .75) : t < 2.5f / 2.75f ? (float)(7.5625 * (t -= 2.25f / 2.75f) * t + .9375) : (float)(7.5625 * (t -= 2.625f / 2.75f) * t + .984375);
        public static float EaseInOutBounce(this float t) => t < 0.5 ? (1 - EaseOutBounce(1 - 2 * t)) / 2 : (1 + EaseOutBounce(2 * t - 1)) / 2;
    }
}