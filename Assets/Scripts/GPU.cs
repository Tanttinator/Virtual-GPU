using UnityEngine;

public class GPU : MonoBehaviour
{
    [SerializeField] Screen screen;

    Framebuffer framebuffer;

    public void Clear(Color color)
    {
        framebuffer.Clear(color);
    }

    public void SetPixel(int x, int y, Color color)
    {
        framebuffer.SetPixel(x, y, color);
    }

    public void DrawLine(int x0, int y0, int x1, int y1, Color color)
    {
        int dx = Mathf.Abs(x1 - x0);
        int dy = -Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx + dy;

        while (true)
        {
            SetPixel(x0, y0, color);

            int e2 = 2 * err;
            if (e2 >= dy)
            {
                if (x0 == x1) break;
                err += dy;
                x0 += sx;
            }
            if (e2 <= dx)
            {
                if (y0 == y1) break;
                err += dx;
                y0 += sy;
            }
        }
    }

    public void DrawWireframeTriangle(Vec2 v0, Vec2 v1, Vec2 v2, Color color)
    {
        DrawLine((int)v0.x, (int)v0.y, (int)v1.x, (int)v1.y, color);
        DrawLine((int)v1.x, (int)v1.y, (int)v2.x, (int)v2.y, color);
        DrawLine((int)v2.x, (int)v2.y, (int)v0.x, (int)v0.y, color);
    }

    public void DrawTriangle(Vec2 v0, Vec2 v1, Vec2 v2, Color color)
    {
        float minX = Mathf.Min(v0.x, v1.x, v2.x);
        float maxX = Mathf.Max(v0.x, v1.x, v2.x);
        float minY = Mathf.Min(v0.y, v1.y, v2.y);
        float maxY = Mathf.Max(v0.y, v1.y, v2.y);

        for (int x = (int)minX; x <= (int)maxX; x++)
        {
            for (int y = (int)minY; y <= (int)maxY; y++)
            {
                Vec2 p = new Vec2(x, y);
                if (IsPointInTriangle(p, v0, v1, v2))
                {
                    SetPixel(x, y, color);
                }
            }
        }
    }

    public void Present()
    {
        screen.Draw(framebuffer.ColorBuffer);
    }

    bool IsPointInTriangle(Vec2 p, Vec2 v0, Vec2 v1, Vec2 v2)
    {
        float area = 0.5f * (-v1.y * v2.x + v0.y * (-v1.x + v2.x) + v0.x * (v1.y - v2.y) + v1.x * v2.y);
        float s = 1f / (2f * area) * (v0.y * v2.x - v0.x * v2.y + (v2.y - v0.y) * p.x + (v0.x - v2.x) * p.y);
        float t = 1f / (2f * area) * (v0.x * v1.y - v0.y * v1.x + (v0.y - v1.y) * p.x + (v1.x - v0.x) * p.y);

        return s >= 0 && t >= 0 && (s + t) <= 1;
    }

    private void Awake()
    {
        framebuffer = new Framebuffer(screen.Width, screen.Height);
    }
}
