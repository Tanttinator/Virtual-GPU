using UnityEngine;

namespace VirtualGPU
{
    public class Sampler
    {
        public enum FilterMode
        {
            Point,
            Bilinear
        }

        public enum WrapMode
        {
            Clamp,
            Repeat
        }

        public FilterMode Filter { get; private set; } = FilterMode.Point;
        public WrapMode Wrap { get; private set; } = WrapMode.Clamp;

        public Sampler(FilterMode filter, WrapMode wrap)
        {
            Filter = filter;
            Wrap = wrap;
        }

        public Color Sample(Texture texture, Vec2 uv)
        {
            int width = texture.Width;
            int height = texture.Height;

            int x = Mathf.FloorToInt(uv.x * width);
            int y = Mathf.FloorToInt(uv.y * height);

            if (Wrap == WrapMode.Repeat)
            {
                x = (x + width) % width;
                y = (y + height) % height;
            }
            else // Wrap == WrapMode.Clamp
            {
                x = Mathf.Clamp(x, 0, width - 1);
                y = Mathf.Clamp(y, 0, height - 1);
            }

            return texture.Pixels[y * width + x];
        }
    }
}
