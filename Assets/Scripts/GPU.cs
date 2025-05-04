using UnityEngine;

public class GPU : MonoBehaviour
{
    [SerializeField] Screen screen;

    Vertex[] vertexBuffer;
    int[] indexBuffer;
    Framebuffer framebuffer;

    // INTERFACE
    public void Clear(Color color)
    {
        framebuffer.Clear(color);
    }

    public void SetVertexBuffer(Vertex[] vertices)
    {
        vertexBuffer = vertices;
    }

    public void SetIndexBuffer(int[] indices)
    {
        indexBuffer = indices;
    }

    public void Draw(Shader shader)
    {
        for (int i = 0; i < indexBuffer.Length; i += 3)
        {
            int index0 = indexBuffer[i];
            int index1 = indexBuffer[i + 1];
            int index2 = indexBuffer[i + 2];

            Vertex v0 = vertexBuffer[index0];
            Vertex v1 = vertexBuffer[index1];
            Vertex v2 = vertexBuffer[index2];

            DrawTriangle(v0, v1, v2, shader);
        }
    }

    public void DrawWireframe()
    {
        for (int i = 0; i < indexBuffer.Length; i += 3)
        {
            int index0 = indexBuffer[i];
            int index1 = indexBuffer[i + 1];
            int index2 = indexBuffer[i + 2];

            Vertex v0 = vertexBuffer[index0];
            Vertex v1 = vertexBuffer[index1];
            Vertex v2 = vertexBuffer[index2];

            DrawWireframeTriangle(v0.Position, v1.Position, v2.Position, Color.white);
        }
    }

    public void Present()
    {
        screen.Draw(framebuffer.ColorBuffer);
    }

    // INTERNAL
    void SetPixel(int x, int y, Color color)
    {
        framebuffer.SetPixel(x, y, color);
    }

    void DrawLine(int x0, int y0, int x1, int y1, Color color)
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

    void DrawWireframeTriangle(Vec2 v0, Vec2 v1, Vec2 v2, Color color)
    {
        DrawLine((int)v0.x, (int)v0.y, (int)v1.x, (int)v1.y, color);
        DrawLine((int)v1.x, (int)v1.y, (int)v2.x, (int)v2.y, color);
        DrawLine((int)v2.x, (int)v2.y, (int)v0.x, (int)v0.y, color);
    }

    void DrawTriangle(Vertex v0, Vertex v1, Vertex v2, Shader shader)
    {
        float minX = Mathf.Min(v0.Position.x, v1.Position.x, v2.Position.x);
        float maxX = Mathf.Max(v0.Position.x, v1.Position.x, v2.Position.x);
        float minY = Mathf.Min(v0.Position.y, v1.Position.y, v2.Position.y);
        float maxY = Mathf.Max(v0.Position.y, v1.Position.y, v2.Position.y);

        float area = SignedTriangleArea(v0.Position, v1.Position, v2.Position);

        for (int x = (int)minX; x <= (int)maxX; x++)
        {
            for (int y = (int)minY; y <= (int)maxY; y++)
            {
                Vec2 p = new Vec2(x, y);
                float alpha = SignedTriangleArea(p, v1.Position, v2.Position) / area;
                float beta = SignedTriangleArea(p, v2.Position, v0.Position) / area;
                float gamma = SignedTriangleArea(p, v0.Position, v1.Position) / area;
                if (alpha < 0 || beta < 0 || gamma < 0) continue;
                Vec2 uv = alpha * v0.UV + beta * v1.UV + gamma * v2.UV;
                SetPixel(x, y, shader.Fragment(uv));
            }
        }
    }

    float SignedTriangleArea(Vec2 a, Vec2 b, Vec2 c)
    {
        return SignedTriangleArea(a.x, a.y, b.x, b.y, c.x, c.y);
    }

    float SignedTriangleArea(float x0, float y0, float x1, float y1, float x2, float y2)
    {
        return (x0 * (y1 - y2) + x1 * (y2 - y0) + x2 * (y0 - y1)) / 2.0f;
    }

    private void Awake()
    {
        framebuffer = new Framebuffer(screen.Width, screen.Height);
    }
}
