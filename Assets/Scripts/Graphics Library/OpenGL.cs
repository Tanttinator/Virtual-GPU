using System;
using UnityEngine;

namespace VirtualGPU
{
    public class OpenGL
    {
        GPU gpu;

        Window currentWindow;
        (int startX, int startY, int width, int height) viewport;
        Color clearColor;
        Vertex[] vertexBuffer;
        int[] indexBuffer;

        public OpenGL(GPU gpu)
        {
            this.gpu = gpu;
        }

        public Window CreateWindow(int width, int height)
        {
            Window window = new Window()
            {
                Width = width,
                Height = height
            };

            return window;
        }

        public void MakeContextCurrent(Window window)
        {
            currentWindow = window;
        }

        public void Viewport(int startX, int startY, int width, int height)
        {
            viewport = (startX, startY, width, height);
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
            vertexBuffer = buffer;
        }

        public void BindIndexBuffer(int[] buffer)
        {
            indexBuffer = buffer;
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
            gpu.Draw(vertexBuffer, indexBuffer, shader);
        }

        public void SwapBuffers()
        {
            gpu.Present();
        }
    }

    public class Window
    {
        public int Width;
        public int Height;
        public (int x, int y) Position = (0, 0);
    }
}
