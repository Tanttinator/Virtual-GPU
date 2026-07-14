using UnityEngine;

namespace VirtualGPU
{
    public struct Vec2
    {
        public readonly float x;
        public readonly float y;

        public Vec2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vec2 operator +(Vec2 a, Vec2 b)
        {
            return new Vec2(a.x + b.x, a.y + b.y);
        }

        public static Vec2 operator *(Vec2 vec, float scalar)
        {
            return new Vec2(vec.x * scalar, vec.y * scalar);
        }

        public static Vec2 operator *(float scalar, Vec2 vec)
        {
            return new Vec2(vec.x * scalar, vec.y * scalar);
        }

        override public string ToString()
        {
            return $"({x}, {y})";
        }
    }
}
