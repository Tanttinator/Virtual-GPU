using UnityEngine;

namespace VirtualGPU
{
    public class BoundingBox
    {
        public Vec2 Min { get; private set; }
        public Vec2 Max { get; private set; }

        public BoundingBox(Vec2 min, Vec2 max)
        {
            Min = min;
            Max = max;
        }

        public BoundingBox(float minX, float minY, float maxX, float maxY)
        {
            Min = new Vec2(minX, minY);
            Max = new Vec2(maxX, maxY);
        }
    }
}
