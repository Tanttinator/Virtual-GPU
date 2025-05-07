using UnityEngine;

namespace VirtualGPU
{
    public class Light
    {

    }

    public class DirectionalLight : Light
    {
        public Transform Transform { get; private set; } = new Transform();
        public Color Color { get; private set; }

        public DirectionalLight(Color color)
        {
            Color = color;
        }

        public Vec3 GetLightDirection()
        {
            return Transform.Forward;
        }
    }
}
