using Unity.VisualScripting;
using UnityEngine;

namespace VirtualGPU
{
    public class Buffer<T>
    {
        T[] data;
        public int Width;
        public int Height;

        public T this[int x, int y]
        {
            get => data[x + y * Width];
            set => data[x + y * Width] = value;
        }

        public T[] Data => data;

        public Buffer(int width, int height)
        {
            Width = width;
            Height = height;
            data = new T[width * height];
        }
    }
}
