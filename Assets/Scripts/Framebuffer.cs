using UnityEngine;

namespace VirtualGPU
{
    public class Framebuffer
    {
        public int Width;
        public int Height;
        public Color[] ColorBuffer;

        public Framebuffer(int width, int height)
        {
            Width = width;
            Height = height;
            ColorBuffer = new Color[Width * Height];
        }

        public void Clear(Color color)
        {
            for (int i = 0; i < ColorBuffer.Length; i++)
            {
                ColorBuffer[i] = color;
            }
        }

        public void SetPixel(int x, int y, Color color)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height) return;
            ColorBuffer[y * Width + x] = color;
        }
    }
}
