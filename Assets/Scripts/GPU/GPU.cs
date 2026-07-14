using System.Collections.Generic;
using UnityEngine;

namespace VirtualGPU
{
    public class GPU
    {
        Screen screen;
        Texture[] textures = new Texture[8];
        Sampler[] samplers = new Sampler[8];
        Framebuffer framebuffer;

        List<FragmentInput> fragmentsCache = new List<FragmentInput>();

        public GPU(Screen screen)
        {
            this.screen = screen;
            framebuffer = new Framebuffer(screen.Width, screen.Height);
        }

        // INTERFACE
        public void Clear(Color color)
        {
            framebuffer.Clear(color);
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

        public void Draw(Vertex[] vertexBuffer, int[] indexBuffer, Shader shader)
        {
            Vertex[] vertices = InputAssembler(vertexBuffer);
            Varyings[] varyings = VertexShader(vertices, shader);
            Triangle[] primitives = PrimitiveAssembler(varyings, indexBuffer);

            foreach (Triangle triangle in primitives)
            {
                if (Clipping(triangle)) continue;

                Vec3[] worldPos = new Vec3[]
                {
                    ClipToScreenPos(triangle.Vertex0.ClipPos),
                    ClipToScreenPos(triangle.Vertex1.ClipPos),
                    ClipToScreenPos(triangle.Vertex2.ClipPos)
                };

                fragmentsCache.Capacity = 10000;
                Rasterizer(triangle, worldPos, fragmentsCache);
                //DepthTest(fragmentsCache);
                Fragment[] fragments = FragmentShader(fragmentsCache, shader);
                Blending(fragments);
                fragmentsCache.Clear();
            }
        }

        public void Present()
        {
            screen.Draw(framebuffer.ColorBuffer);
        }

        // PIPELINE STAGES
        Vertex[] InputAssembler(Vertex[] vertexBuffer)
        {
            return vertexBuffer;
        }

        Varyings[] VertexShader(Vertex[] vertices, Shader shader)
        {
            Varyings[] varyings = new Varyings[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                varyings[i] = shader.Vertex(vertices[i]);
            }
            return varyings;
        }

        Triangle[] PrimitiveAssembler(Varyings[] varyings, int[] indexBuffer)
        {
            int triangleCount = indexBuffer.Length / 3;
            Triangle[] triangles = new Triangle[triangleCount];
            for (int i = 0; i < triangleCount; i++)
            {
                int index0 = indexBuffer[i * 3];
                int index1 = indexBuffer[i * 3 + 1];
                int index2 = indexBuffer[i * 3 + 2];

                Triangle triangle = new Triangle()
                {
                    Vertex0 = varyings[index0],
                    Vertex1 = varyings[index1],
                    Vertex2 = varyings[index2]
                };

                triangles[i] = triangle;
            }
            return triangles;
        }

        bool Clipping(Triangle triangle)
        {
            return triangle.Vertex0.ClipPos.w <= 0 && triangle.Vertex1.ClipPos.w <= 0 && triangle.Vertex2.ClipPos.w <= 0;
        }

        void Rasterizer(Triangle triangle, Vec3[] screenPos, List<FragmentInput> fragments)
        {
            Vec3 normal = Vec3.Cross(triangle.Vertex1.WorldPos - triangle.Vertex0.WorldPos, triangle.Vertex2.WorldPos - triangle.Vertex0.WorldPos).Normalize();

            int minX = (int)Mathf.Max(0f, Mathf.Min(screenPos[0].x, screenPos[1].x, screenPos[2].x));
            int maxX = (int)Mathf.Min(screen.Width - 1, Mathf.Max(screenPos[0].x, screenPos[1].x, screenPos[2].x));
            int minY = (int)Mathf.Max(0f, Mathf.Min(screenPos[0].y, screenPos[1].y, screenPos[2].y));
            int maxY = (int)Mathf.Min(screen.Height - 1, Mathf.Max(screenPos[0].y, screenPos[1].y, screenPos[2].y));

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
                    if (z >= framebuffer.ReadDepth(x, y)) continue;

                    p.z = z;

                    FragmentInput data = new FragmentInput
                    {
                        ScreenPos = p,
                        UV = alpha * triangle.Vertex0.UV + beta * triangle.Vertex1.UV + gamma * triangle.Vertex2.UV,
                        Normal = normal,
                        VertexColor = alpha * triangle.Vertex0.Color + beta * triangle.Vertex1.Color + gamma * triangle.Vertex2.Color
                    };
                    fragments.Add(data);
                }
            }
        }

        void DepthTest(List<FragmentInput> fragments)
        {
            for (int i = fragments.Count - 1; i >= 0; i--)
            {
                FragmentInput fragment = fragments[i];
                Vec3 position = fragment.ScreenPos;
                if (framebuffer.ReadDepth((int)position.x, (int)position.y) <= position.z)
                    fragments.RemoveAt(i);
            }
        }

        Fragment[] FragmentShader(List<FragmentInput> inputs, Shader shader)
        {
            Fragment[] fragments = new Fragment[inputs.Count];
            for (int i = 0; i < inputs.Count; i++)
            {
                FragmentInput input = inputs[i];
                Fragment fragment = new Fragment()
                {
                    ScreenX = (int)input.ScreenPos.x,
                    ScreenY = (int)input.ScreenPos.y,
                    Color = shader.Fragment(input, textures, samplers),
                    Depth = input.ScreenPos.z
                };
                fragments[i] = fragment;
            }
            return fragments;
        }

        void Blending(Fragment[] fragments)
        {
            foreach (Fragment fragment in fragments)
            {
                framebuffer.WriteDepth(fragment.ScreenX, fragment.ScreenY, fragment.Depth);
                framebuffer.WriteColor(fragment.ScreenX, fragment.ScreenY, fragment.Color);
            }
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

        struct Triangle
        {
            public Varyings Vertex0;
            public Varyings Vertex1;
            public Varyings Vertex2;
        }

        struct Fragment
        {
            public int ScreenX;
            public int ScreenY;
            public Color Color;
            public float Depth;
        }
    }
}
