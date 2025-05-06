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

        public float Magnitude()
        {
            return Mathf.Sqrt(x * x + y * y + z * z);
        }

        public Vec3 Normalize()
        {
            float magnitude = Magnitude();
            if (magnitude == 0) return new Vec3(0, 0, 0);
            return new Vec3(x / magnitude, y / magnitude, z / magnitude);
        }

        public static float Dot(Vec3 a, Vec3 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public static Vec3 Cross(Vec3 a, Vec3 b)
        {
            return new Vec3(
                a.y * b.z - a.z * b.y,
                a.z * b.x - a.x * b.z,
                a.x * b.y - a.y * b.x
            );
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
