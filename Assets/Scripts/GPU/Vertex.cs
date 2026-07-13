using UnityEngine;

namespace VirtualGPU
{
    public class Vertex
    {
        public Vec3 Position { get; private set; }
        public Vec2 UV { get; private set; } = new Vec2(0, 0);
        public Vec3 Normal { get; private set; } = new Vec3(0, 0, 1);
        public Color Color { get; private set; } = new Color(1, 1, 1, 1);

        public Vertex(Vec3 position)
        {
            Position = position;
        }

        public Vertex(Vec3 position, Vec2 uv)
        {
            Position = position;
            UV = uv;
        }

        public Vertex(Vec3 position, Color color)
        {
            Position = position;
            Color = color;
        }

        public Vertex(Vec3 position, Vec2 uv, Color color)
        {
            Position = position;
            UV = uv;
            Color = color;
        }
    }
}
