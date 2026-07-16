using UnityEngine;

namespace VirtualGPU
{
    public struct Pipeline
    {
        public int ColorTarget;
        public int DepthTarget;
        public (int, int, int, int) Viewport;
        public Shader Program;
        public float[] VertexBuffer;
        public int[] IndexBuffer;
    }
}
