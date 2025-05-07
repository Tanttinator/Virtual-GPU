using System.Collections.Generic;
using UnityEngine;

namespace VirtualGPU
{
    public class GPU : MonoBehaviour
    {
        [SerializeField] Screen screen;

        Vertex[] vertexBuffer;
        int[] indexBuffer;
        Texture[] textures = new Texture[8];
        Sampler[] samplers = new Sampler[8];
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

        public void BindTexture(int slot, Texture texture)
        {
            if (slot < 0 || slot >= textures.Length) return;
            textures[slot] = texture;
        }

        public void BindSampler(int slot, Sampler sampler)
        {
            if (slot < 0 || slot >= samplers.Length) return;
            samplers[slot] = sampler;
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
            framebuffer.WriteColor(x, y, color);
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
                varyings[1].ClipPos / varyings[1].ClipPos.w,
                varyings[2].ClipPos / varyings[2].ClipPos.w
            };

            Vec3[] screenPos = new Vec3[]
            {
                new Vec3((ndc[0].x * 0.5f + 0.5f) * screen.Width, (-ndc[0].y * 0.5f + 0.5f) * screen.Height, ndc[0].z * 0.5f + 0.5f),
                new Vec3((ndc[1].x * 0.5f + 0.5f) * screen.Width, (-ndc[1].y * 0.5f + 0.5f) * screen.Height, ndc[1].z * 0.5f + 0.5f),
                new Vec3((ndc[2].x * 0.5f + 0.5f) * screen.Width, (-ndc[2].y * 0.5f + 0.5f) * screen.Height, ndc[2].z * 0.5f + 0.5f)
            };

            int x0 = Mathf.RoundToInt(screenPos[0].x);
            int y0 = Mathf.RoundToInt(screenPos[0].y);
            int x1 = Mathf.RoundToInt(screenPos[1].x);
            int y1 = Mathf.RoundToInt(screenPos[1].y);
            int x2 = Mathf.RoundToInt(screenPos[2].x);
            int y2 = Mathf.RoundToInt(screenPos[2].y);

            DrawLine(x0, y0, x1, y1, color);
            DrawLine(x1, y1, x2, y2, color);
            DrawLine(x2, y2, x0, y0, color);
        }

        void DrawTriangle(Vertex v0, Vertex v1, Vertex v2, Shader shader)
        {
            Varyings[] varyings = new Varyings[]
            {
                shader.Vertex(v0),
                shader.Vertex(v1),
                shader.Vertex(v2)
            };

            bool behind = varyings[0].ClipPos.w <= 0 && varyings[1].ClipPos.w <= 0 && varyings[2].ClipPos.w <= 0;
            if (behind) return;

            Vec3[] screenPos = new Vec3[]
            {
                ClipToScreenPos(varyings[0].ClipPos),
                ClipToScreenPos(varyings[1].ClipPos),
                ClipToScreenPos(varyings[2].ClipPos)
            };

            Vec3 normal = Vec3.Cross(varyings[1].WorldPos - varyings[0].WorldPos, varyings[2].WorldPos - varyings[0].WorldPos).Normalize();

            int minX = Mathf.RoundToInt(Mathf.Max(0f, Mathf.Min(screenPos[0].x, screenPos[1].x, screenPos[2].x)));
            int maxX = Mathf.RoundToInt(Mathf.Min(screen.Width - 1, Mathf.Max(screenPos[0].x, screenPos[1].x, screenPos[2].x)));
            int minY = Mathf.RoundToInt(Mathf.Max(0f, Mathf.Min(screenPos[0].y, screenPos[1].y, screenPos[2].y)));
            int maxY = Mathf.RoundToInt(Mathf.Min(screen.Height - 1, Mathf.Max(screenPos[0].y, screenPos[1].y, screenPos[2].y)));

            float area = SignedTriangleArea(screenPos[0], screenPos[1], screenPos[2]);

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    Vec3 p = new Vec3(x, y, 0);
                    float alpha = SignedTriangleArea(p, screenPos[1], screenPos[2]) / area;
                    float beta = SignedTriangleArea(p, screenPos[2], screenPos[0]) / area;
                    float gamma = SignedTriangleArea(p, screenPos[0], screenPos[1]) / area;
                    if (alpha < 0 || beta < 0 || gamma < 0) continue;

                    float z = alpha * screenPos[0].z + beta * screenPos[1].z + gamma * screenPos[2].z;
                    if (framebuffer.ReadDepth(x, y) < z) continue;

                    FragmentData data = new FragmentData
                    {
                        ScreenPos = new Vec3(x, y, z),
                        UV = alpha * varyings[0].UV + beta * varyings[1].UV + gamma * varyings[2].UV,
                        Normal = normal,
                        VertexColor = alpha * varyings[0].Color + beta * varyings[1].Color + gamma * varyings[2].Color
                    };

                    Color color = shader.Fragment(data, textures, samplers);

                    framebuffer.WriteDepth(x, y, z);
                    SetPixel(x, y, color);
                }
            }
        }

        Vec3 ClipToScreenPos(Vec4 clipPos)
        {
            Vec3 ndcPos = clipPos / clipPos.w;
            Vec3 screenPos = new Vec3((ndcPos.x * 0.5f + 0.5f) * screen.Width, (-ndcPos.y * 0.5f + 0.5f) * screen.Height, ndcPos.z * 0.5f + 0.5f);
            return screenPos;
        }

        float SignedTriangleArea(Vec3 a, Vec3 b, Vec3 c)
        {
            return (a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y)) / 2.0f;
        }

        float SignedTriangleArea(Vec2 a, Vec2 b, Vec2 c)
        {
            return (a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y)) / 2.0f;
        }

        private void Awake()
        {
            framebuffer = new Framebuffer(screen.Width, screen.Height);
        }
    }
}
