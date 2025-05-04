using UnityEngine;

namespace VirtualGPU
{
    public class Vec3
    {
        public readonly float x;
        public readonly float y;
        public readonly float z;

        public Vec3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static implicit operator Vec2(Vec3 v)
        {
            return new Vec2(v.x, v.y);
        }

        public static Vec3 operator +(Vec3 a, Vec3 b)
        {
            return new Vec3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vec3 operator -(Vec3 a, Vec3 b)
        {
            return new Vec3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vec3 operator *(Vec3 vec, float scalar)
        {
            return new Vec3(vec.x * scalar, vec.y * scalar, vec.z * scalar);
        }

        public static Vec3 operator *(float scalar, Vec3 vec)
        {
            return new Vec3(vec.x * scalar, vec.y * scalar, vec.z * scalar);
        }

        public static Vec3 operator -(Vec3 vec)
        {
            return new Vec3(-vec.x, -vec.y, -vec.z);
        }

        override public string ToString()
        {
            return $"({x}, {y}, {z})";
        }
    }
}
