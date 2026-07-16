using System;
using JetBrains.Annotations;
using UnityEngine;

namespace VirtualGPU
{
    public class OpenGL
    {
        GPU gpu;

        Window currentWindow;
        (int startX, int startY, int width, int height) viewport;
        Color clearColor;
        float[] vertexBuffer;
        int[] indexBuffer;

        (int color, int depth) framebufferA;
        (int color, int depth) framebufferB;
        bool swapped = false;

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

            framebufferA = (gpu.CreateColorBuffer(width, height), gpu.CreateDepthBuffer(width, height));
            framebufferB = (gpu.CreateColorBuffer(width, height), gpu.CreateDepthBuffer(width, height));

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
            (int color, int depth) = swapped ? framebufferB : framebufferA;
            gpu.ClearColor(color, clearColor);
            gpu.ClearDepth(depth);
        }

        public void BindVertexBuffer(float[] buffer)
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
            Pipeline pipeline = new Pipeline()
            {
                ColorTarget = swapped ? framebufferB.color : framebufferA.color,
                DepthTarget = swapped ? framebufferB.depth : framebufferA.depth,
                Viewport = viewport,
                Program = shader,
                VertexBuffer = vertexBuffer,
                IndexBuffer = indexBuffer
            };

            gpu.Execute(pipeline);
        }

        public void SwapBuffers()
        {
            //gpu.Present();
            swapped = !swapped;
            gpu.Present(swapped ? framebufferA.color : framebufferB.color);
        }
    }

    public class Window
    {
        public int Width;
        public int Height;
        public (int x, int y) Position = (0, 0);
    }
}
