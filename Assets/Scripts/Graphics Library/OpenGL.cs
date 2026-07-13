using UnityEngine;

namespace VirtualGPU
{
    public class OpenGL
    {
        GPU gpu;

        Color clearColor;

        public OpenGL(GPU gpu)
        {
            this.gpu = gpu;
        }

        public void ClearColor(Color color)
        {
            clearColor = color;
        }

        public void Clear()
        {
            gpu.Clear(clearColor);
        }

        public void BindVertexBuffer(Vertex[] buffer)
        {
            gpu.BindVertexBuffer(buffer);
        }

        public void BindIndexBuffer(int[] buffer)
        {
            gpu.BindIndexBuffer(buffer);
        }

        public void BindTexture(int id, Texture texture)
        {
            gpu.BindTexture(id, texture);
        }

        public void BindSampler(int id, Sampler sampler)
        {
            gpu.BindSampler(id, sampler);
        }

        public void Draw(Shader shader)
        {
            gpu.Draw(shader);
        }

        public void DrawWireframe(Shader shader)
        {
            gpu.DrawWireframe(shader);
        }

        public void SwapBuffers()
        {
            gpu.Present();
        }
    }
}
