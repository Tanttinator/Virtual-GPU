using UnityEngine;

namespace VirtualGPU
{
    public abstract class Camera
    {
        public Transform Transform { get; private set; } = new Transform();
        public float NearPlane { get; set; } = 0.1f;
        public float FarPlane { get; set; } = 100.0f;

        protected float aspectRatio;

        public Camera(float aspectRatio)
        {
            this.aspectRatio = aspectRatio;
        }

        public Mat4 GetViewMatrix()
        {
            Mat4 translationMatrix = Mat4.Translate(-Transform.Position);
            Mat4 rotationMatrix = Mat4.Rotate(-Transform.Rotation);

            return rotationMatrix * translationMatrix;
        }

        public abstract Mat4 GetProjectionMatrix();
    }

    public class PerspectiveCamera : Camera
    {
        public float FieldOfView { get; set; } = 45.0f;

        public PerspectiveCamera(float aspectRatio) : base(aspectRatio)
        {

        }

        override public Mat4 GetProjectionMatrix()
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

    public class OrthographicCamera : Camera
    {
        public float Size { get; set; } = 1.0f;

        public OrthographicCamera(float aspectRatio) : base(aspectRatio)
        {

        }

        override public Mat4 GetProjectionMatrix()
        {
            float left = Size * 0.5f * -aspectRatio;
            float right = Size * 0.5f * aspectRatio;
            float bottom = Size * -0.5f;
            float top = Size * 0.5f;

            Mat4 projectionMatrix = new Mat4();
            projectionMatrix.Elements[0, 0] = 2.0f / (right - left);
            projectionMatrix.Elements[1, 1] = 2.0f / (top - bottom);
            projectionMatrix.Elements[2, 2] = -2.0f / (FarPlane - NearPlane);
            projectionMatrix.Elements[3, 3] = 1.0f;
            projectionMatrix.Elements[3, 0] = -(right + left) / (right - left);
            projectionMatrix.Elements[3, 1] = -(top + bottom) / (top - bottom);
            projectionMatrix.Elements[3, 2] = -(FarPlane + NearPlane) / (FarPlane - NearPlane);

            return projectionMatrix;
        }
    }
}
