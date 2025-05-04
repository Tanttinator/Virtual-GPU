using UnityEngine;

namespace VirtualGPU
{
    public class Camera
    {
        public Transform Transform { get; private set; } = new Transform();
        public float FieldOfView { get; set; } = 45.0f;
        public float NearPlane { get; set; } = 0.1f;
        public float FarPlane { get; set; } = 100.0f;

        float aspectRatio;

        public Camera(int screenWidth, int screenHeight)
        {
            aspectRatio = (float)screenWidth / (float)screenHeight;
        }

        public Mat4 GetViewMatrix()
        {
            Mat4 translationMatrix = Mat4.Translate(-Transform.Position);
            Mat4 rotationMatrix = Mat4.Rotate(-Transform.Rotation);

            return rotationMatrix * translationMatrix;
        }

        public Mat4 GetProjectionMatrix()
        {
            float t = 1f / Mathf.Tan(FieldOfView * 0.5f * Mathf.Deg2Rad);
            float rangeInv = 1.0f / (NearPlane - FarPlane);

            Mat4 projectionMatrix = new Mat4();
            projectionMatrix.Elements[0, 0] = t / aspectRatio;
            projectionMatrix.Elements[1, 1] = t;
            projectionMatrix.Elements[2, 2] = (NearPlane + FarPlane) * rangeInv;
            projectionMatrix.Elements[2, 3] = 2.0f * NearPlane * FarPlane * rangeInv;
            projectionMatrix.Elements[3, 2] = -1.0f;

            return projectionMatrix;
        }
    }
}
