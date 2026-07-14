using UnityEngine;

namespace VirtualGPU
{
    public struct Mat4
    {
        private float[,] elements;
        public float[,] Elements
        {
            get
            {
                if (elements == null) elements = new float[4, 4];
                return elements;
            }
        }

        public static Mat4 Identity()
        {
            Mat4 result = new Mat4();
            result.Elements[0, 0] = 1;
            result.Elements[1, 1] = 1;
            result.Elements[2, 2] = 1;
            result.Elements[3, 3] = 1;
            return result;
        }

        public static Mat4 Translate(Vec3 translation)
        {
            Mat4 result = Identity();
            result.Elements[0, 3] = translation.x;
            result.Elements[1, 3] = translation.y;
            result.Elements[2, 3] = translation.z;
            return result;
        }

        public static Mat4 Rotate(Vec3 rotation)
        {
            Mat4 result = Identity();
            float cosX = Mathf.Cos(rotation.x);
            float sinX = Mathf.Sin(rotation.x);
            float cosY = Mathf.Cos(rotation.y);
            float sinY = Mathf.Sin(rotation.y);
            float cosZ = Mathf.Cos(rotation.z);
            float sinZ = Mathf.Sin(rotation.z);

            result.Elements[0, 0] = cosZ * cosY - sinZ * sinX * sinY;
            result.Elements[0, 1] = -sinZ * cosX;
            result.Elements[0, 2] = cosZ * sinY + sinZ * sinX * cosY;
            result.Elements[0, 3] = 0;

            result.Elements[1, 0] = sinZ * cosY + cosZ * sinX * sinY;
            result.Elements[1, 1] = cosZ * cosX;
            result.Elements[1, 2] = sinZ * sinY - cosZ * sinX * cosY;
            result.Elements[1, 3] = 0;

            result.Elements[2, 0] = -cosX * sinY;
            result.Elements[2, 1] = sinX;
            result.Elements[2, 2] = cosX * cosY;
            result.Elements[2, 3] = 0;

            result.Elements[3, 0] = 0;
            result.Elements[3, 1] = 0;
            result.Elements[3, 2] = 0;
            result.Elements[3, 3] = 1;

            return result;
        }

        public static Mat4 Scale(Vec3 scale)
        {
            Mat4 result = Identity();
            result.Elements[0, 0] = scale.x;
            result.Elements[1, 1] = scale.y;
            result.Elements[2, 2] = scale.z;
            return result;
        }

        public static Mat4 operator *(Mat4 a, Mat4 b)
        {
            Mat4 result = new Mat4();
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    for (int k = 0; k < 4; k++)
                        result.Elements[i, j] += a.Elements[i, k] * b.Elements[k, j];
            return result;
        }

        public static Vec4 operator *(Mat4 mat, Vec4 vec)
        {
            float x = mat.Elements[0, 0] * vec.x + mat.Elements[0, 1] * vec.y + mat.Elements[0, 2] * vec.z + mat.Elements[0, 3] * vec.w;
            float y = mat.Elements[1, 0] * vec.x + mat.Elements[1, 1] * vec.y + mat.Elements[1, 2] * vec.z + mat.Elements[1, 3] * vec.w;
            float z = mat.Elements[2, 0] * vec.x + mat.Elements[2, 1] * vec.y + mat.Elements[2, 2] * vec.z + mat.Elements[2, 3] * vec.w;
            float w = mat.Elements[3, 0] * vec.x + mat.Elements[3, 1] * vec.y + mat.Elements[3, 2] * vec.z + mat.Elements[3, 3] * vec.w;
            return new Vec4(x, y, z, w);
        }

        public static Vec3 operator *(Mat4 mat, Vec3 vec)
        {
            float x = mat.Elements[0, 0] * vec.x + mat.Elements[0, 1] * vec.y + mat.Elements[0, 2] * vec.z + mat.Elements[0, 3];
            float y = mat.Elements[1, 0] * vec.x + mat.Elements[1, 1] * vec.y + mat.Elements[1, 2] * vec.z + mat.Elements[1, 3];
            float z = mat.Elements[2, 0] * vec.x + mat.Elements[2, 1] * vec.y + mat.Elements[2, 2] * vec.z + mat.Elements[2, 3];
            return new Vec3(x, y, z);
        }
    }
}
