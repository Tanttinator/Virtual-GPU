using UnityEngine;

namespace VirtualGPU
{
    public class Transform
    {
        public Vec3 Position { get; set; }
        public Vec3 Rotation { get; set; }
        public Vec3 Scale { get; set; }

        public Vec3 Forward => Mat4.Rotate(Rotation) * new Vec3(0, 0, -1);

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
