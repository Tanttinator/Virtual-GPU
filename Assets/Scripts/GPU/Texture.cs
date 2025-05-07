using UnityEngine;

namespace VirtualGPU
{
    public class Texture
    {
        public Color[] Pixels { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Texture(int width, int height)
        {
            Width = width;
            Height = height;
            Pixels = new Color[width * height];
        }

        public void SetPixels(Color[] pixels)
        {
            if (pixels.Length != Pixels.Length)
            {
                throw new System.ArgumentException("Pixel array size does not match texture size.");
            }

            Pixels = pixels;
        }
    }
}
