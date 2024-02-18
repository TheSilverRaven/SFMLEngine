using SFML.Graphics;
using SFML.System;

namespace SilverRaven.SFML
{
    public class Camera
    {
        private View cameraView;
        private float? clippingStart;
        private float? clippingEnd;

        public Vector2f Position {
            get => cameraView.Center;
            set => cameraView.Center = value;
        }

        public float Rotation {
            get => cameraView.Rotation;
            set => cameraView.Rotation = value;
        }

        public Camera(Vector2f center, Vector2f size, float? clippingStart = null, float? clippingEnd = null)
        {
            this.clippingStart = clippingStart;
            this.clippingEnd = clippingEnd;
            cameraView = new View(center, size);
        }

        public void Draw(RenderWindow window)
        {
            window.SetView(cameraView);
            GameObject.DrawLayer(window, clippingStart, clippingEnd);
        }
    }
}