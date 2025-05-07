using UnityEngine;

namespace VirtualGPU
{
    public class Framebuffer
    {
        public int Width;
        public int Height;
        public Color[] ColorBuffer;
        public float[] DepthBuffer;

        public Framebuffer(int width, int height)
        {
            Width = width;
            Height = height;
            ColorBuffer = new Color[Width * Height];
            DepthBuffer = new float[Width * Height];
        }

        public void Clear(Color color)
        {
            for (int i = 0; i < ColorBuffer.Length; i++)
            {
                ColorBuffer[i] = color;
                DepthBuffer[i] = float.MaxValue;
            }
        }

        public void WriteColor(int x, int y, Color color)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height) return;
            ColorBuffer[y * Width + x] = color;
        }

        public void WriteDepth(int x, int y, float depth)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height) return;
            DepthBuffer[y * Width + x] = depth;
        }

        public float ReadDepth(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height) return float.MaxValue;
            return DepthBuffer[y * Width + x];
        }
    }
}
