using UnityEngine;

namespace VirtualGPU
{
    public class Light
    {

    }

    public class DirectionalLight : Light
    {
        public readonly Transform Transform = new Transform();
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
