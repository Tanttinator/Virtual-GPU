using UnityEngine;

namespace VirtualGPU
{
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

        public void BindVertexBuffer(Vertex[] vertices)
        {
            vertexBuffer = vertices;
        }

        public void BindIndexBuffer(int[] indices)
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

        public void DrawWireframe(Shader shader)
        {
            for (int i = 0; i < indexBuffer.Length; i += 3)
            {
                int index0 = indexBuffer[i];
                int index1 = indexBuffer[i + 1];
                int index2 = indexBuffer[i + 2];

                Vertex v0 = vertexBuffer[index0];
                Vertex v1 = vertexBuffer[index1];
                Vertex v2 = vertexBuffer[index2];

                DrawWireframeTriangle(v0, v1, v2, shader, Color.white);
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

        void DrawWireframeTriangle(Vertex v0, Vertex v1, Vertex v2, Shader shader, Color color)
        {
            Varyings[] varyings = new Varyings[]
            {
                shader.Vertex(v0),
                shader.Vertex(v1),
                shader.Vertex(v2)
            };

            Vec3[] ndc = new Vec3[]
            {
                varyings[0].ClipPos / varyings[0].ClipPos.w,
                varyings[1].ClipPos / varyings[0].ClipPos.w,
                varyings[2].ClipPos / varyings[0].ClipPos.w
            };

            Vec3[] screenPos = new Vec3[]
            {
                new Vec3((ndc[0].x * 0.5f + 0.5f) * screen.Width, (-ndc[0].y * 0.5f + 0.5f) * screen.Height, ndc[0].z * 0.5f + 0.5f),
                new Vec3((ndc[1].x * 0.5f + 0.5f) * screen.Width, (-ndc[1].y * 0.5f + 0.5f) * screen.Height, ndc[1].z * 0.5f + 0.5f),
                new Vec3((ndc[2].x * 0.5f + 0.5f) * screen.Width, (-ndc[2].y * 0.5f + 0.5f) * screen.Height, ndc[2].z * 0.5f + 0.5f)
            };

            DrawLine((int)screenPos[0].x, (int)screenPos[0].y, (int)screenPos[1].x, (int)screenPos[1].y, color);
            DrawLine((int)screenPos[1].x, (int)screenPos[1].y, (int)screenPos[2].x, (int)screenPos[2].y, color);
            DrawLine((int)screenPos[2].x, (int)screenPos[2].y, (int)screenPos[0].x, (int)screenPos[0].y, color);
        }

        void DrawTriangle(Vertex v0, Vertex v1, Vertex v2, Shader shader)
        {
            Varyings[] varyings = new Varyings[]
            {
                shader.Vertex(v0),
                shader.Vertex(v1),
                shader.Vertex(v2)
            };

            Vec3[] ndc = new Vec3[]
            {
                varyings[0].ClipPos / varyings[0].ClipPos.w,
                varyings[1].ClipPos / varyings[0].ClipPos.w,
                varyings[2].ClipPos / varyings[0].ClipPos.w
            };

            Vec3[] screenPos = new Vec3[]
            {
                new Vec3((ndc[0].x * 0.5f + 0.5f) * screen.Width, (-ndc[0].y * 0.5f + 0.5f) * screen.Height, ndc[0].z * 0.5f + 0.5f),
                new Vec3((ndc[1].x * 0.5f + 0.5f) * screen.Width, (-ndc[1].y * 0.5f + 0.5f) * screen.Height, ndc[1].z * 0.5f + 0.5f),
                new Vec3((ndc[2].x * 0.5f + 0.5f) * screen.Width, (-ndc[2].y * 0.5f + 0.5f) * screen.Height, ndc[2].z * 0.5f + 0.5f)
            };

            float minX = Mathf.Max(0f, Mathf.Min(screenPos[0].x, screenPos[1].x, screenPos[2].x));
            float maxX = Mathf.Min(screen.Width - 1, Mathf.Max(screenPos[0].x, screenPos[1].x, screenPos[2].x));
            float minY = Mathf.Max(0f, Mathf.Min(screenPos[0].y, screenPos[1].y, screenPos[2].y));
            float maxY = Mathf.Min(screen.Height - 1, Mathf.Max(screenPos[0].y, screenPos[0].y, screenPos[2].y));

            float area = SignedTriangleArea(screenPos[0], screenPos[1], screenPos[2]);

            for (int x = (int)minX; x <= (int)maxX; x++)
            {
                for (int y = (int)minY; y <= (int)maxY; y++)
                {
                    Vec2 p = new Vec2(x, y);
                    float alpha = SignedTriangleArea(p, screenPos[1], screenPos[2]) / area;
                    float beta = SignedTriangleArea(p, screenPos[2], screenPos[0]) / area;
                    float gamma = SignedTriangleArea(p, screenPos[0], screenPos[1]) / area;
                    if (alpha < 0 || beta < 0 || gamma < 0) continue;
                    Varyings v = new Varyings // TODO: Pass screen position instead of clip position
                    {
                        ClipPos = alpha * varyings[0].ClipPos + beta * varyings[1].ClipPos + gamma * varyings[2].ClipPos,
                        UV = alpha * varyings[0].UV + beta * varyings[1].UV + gamma * varyings[2].UV,
                        Normal = alpha * varyings[0].Normal + beta * varyings[1].Normal + gamma * varyings[2].Normal,
                        Color = alpha * varyings[0].Color + beta * varyings[1].Color + gamma * varyings[2].Color
                    };
                    SetPixel(x, y, shader.Fragment(v));
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
}
