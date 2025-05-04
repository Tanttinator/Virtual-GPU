using UnityEngine;

namespace VirtualGPU
{
    public class Vec4
    {
        public readonly float x;
        public readonly float y;
        public readonly float z;
        public readonly float w;

        public Vec4(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public Vec4(Vec3 v3, float w)
        {
            x = v3.x;
            y = v3.y;
            z = v3.z;
            this.w = w;
        }

        public static implicit operator Vec3(Vec4 v)
        {
            return new Vec3(v.x, v.y, v.z);
        }

        public static implicit operator Vec2(Vec4 v)
        {
            return new Vec2(v.x, v.y);
        }

        public static Vec4 operator +(Vec4 a, Vec4 b)
        {
            return new Vec4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
        }

        public static Vec4 operator -(Vec4 a, Vec4 b)
        {
            return new Vec4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
        }

        public static Vec4 operator *(Vec4 vec, float scalar)
        {
            return new Vec4(vec.x * scalar, vec.y * scalar, vec.z * scalar, vec.w * scalar);
        }

        public static Vec4 operator *(float scalar, Vec4 vec)
        {
            return new Vec4(vec.x * scalar, vec.y * scalar, vec.z * scalar, vec.w * scalar);
        }

        public static Vec4 operator /(Vec4 vec, float scalar)
        {
            return new Vec4(vec.x / scalar, vec.y / scalar, vec.z / scalar, vec.w / scalar);
        }

        public static Vec4 operator /(float scalar, Vec4 vec)
        {
            return new Vec4(scalar / vec.x, scalar / vec.y, scalar / vec.z, scalar / vec.w);
        }

        override public string ToString()
        {
            return $"({x}, {y}, {z}, {w})";
        }
    }
}
