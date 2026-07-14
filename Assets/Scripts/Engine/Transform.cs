using UnityEngine;

namespace VirtualGPU
{
    public class Transform
    {
        public Vec3 Position;
        Vec3 rotation;
        public Vec3 Rotation
        {
            get => rotation;
            set
            {
                rotation = value;
                Forward = (Mat4.Rotate(rotation) * new Vec3(0, 0, -1)).Normalize();
            }
        }
        public Vec3 Scale;

        public Vec3 Forward;

        public Transform()
        {
            Position = new Vec3(0, 0, 0);
            Rotation = new Vec3(0, 0, 0);
            Scale = new Vec3(1, 1, 1);
        }

        public Mat4 GetModelMatrix()
        {
            Mat4 translationMatrix = Mat4.Translate(Position);
            Mat4 rotationMatrix = Mat4.Rotate(Rotation);
            Mat4 scaleMatrix = Mat4.Scale(Scale);

            return translationMatrix * rotationMatrix * scaleMatrix;
        }

    }
}
