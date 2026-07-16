using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

namespace VirtualGPU
{
    public class GPU
    {
        Screen screen;

        Texture[] textures = new Texture[8];
        Sampler[] samplers = new Sampler[8];

        // Memory
        List<Buffer<Color>> colorBuffers = new List<Buffer<Color>>();
        List<Buffer<float>> depthBuffers = new List<Buffer<float>>();

        List<Fragment> fragmentsCache = new List<Fragment>();

        public GPU(Screen screen)
        {
            this.screen = screen;
        }

        // INTERFACE
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

        public int CreateColorBuffer(int width, int height)
        {
            colorBuffers.Add(new Buffer<Color>(width, height));
            return colorBuffers.Count - 1;
        }

        public int CreateDepthBuffer(int width, int height)
        {
            depthBuffers.Add(new Buffer<float>(width, height));
            return depthBuffers.Count - 1;
        }

        public void ClearColor(int bufferId, Color color)
        {
            Buffer<Color> buffer = colorBuffers[bufferId];
            for (int x = 0; x < buffer.Width; x++)
            {
                for (int y = 0; y < buffer.Height; y++)
                {
                    buffer[x, y] = color;
                }
            }
        }

        public void ClearDepth(int bufferId)
        {
            Buffer<float> buffer = depthBuffers[bufferId];
            for (int x = 0; x < buffer.Width; x++)
            {
                for (int y = 0; y < buffer.Height; y++)
                {
                    buffer[x, y] = float.MaxValue;
                }
            }
        }

        public void Execute(Pipeline pipeline)
        {
            Vertex[] vertices = InputAssembler(pipeline.VertexBuffer);
            Vertex[] transformedVertices = VertexShader(vertices, pipeline.Program);
            Triangle[] primitives = PrimitiveAssembler(transformedVertices, pipeline.IndexBuffer);

            for (int i = 0; i < primitives.Length; i++)
            {
                Triangle triangle = primitives[i];
                if (Clipping(triangle)) continue;

                Vec3[] ndcPos = new Vec3[]
                {
                    PerspectiveDivide(triangle.Vertex0.ClipPos),
                    PerspectiveDivide(triangle.Vertex1.ClipPos),
                    PerspectiveDivide(triangle.Vertex2.ClipPos)
                };

                triangle.Vertex0.ScreenPos = ViewportMapping(ndcPos[0], pipeline.Viewport);
                triangle.Vertex1.ScreenPos = ViewportMapping(ndcPos[1], pipeline.Viewport);
                triangle.Vertex2.ScreenPos = ViewportMapping(ndcPos[2], pipeline.Viewport);

                if (BackfaceCulling(triangle)) continue;

                fragmentsCache.Capacity = 10000;
                Rasterizer(triangle, pipeline.Viewport, depthBuffers[pipeline.DepthTarget], fragmentsCache);
                Fragment[] fragments = FragmentShader(fragmentsCache, pipeline.Program);
                Blending(fragments, colorBuffers[pipeline.ColorTarget]);
                fragmentsCache.Clear();
            }
        }

        public void Present(int buffer)
        {
            screen.Draw(colorBuffers[buffer].Data);
        }

        // PIPELINE STAGES
        Vertex[] InputAssembler(float[] vertexBuffer)
        {
            int vertexStride = 3 + 3 + 2 + 4;
            int vertexCount = vertexBuffer.Length / vertexStride;
            Vertex[] vertices = new Vertex[vertexCount];

            for (int i = 0; i < vertexCount; i++)
            {
                int startPos = i * vertexStride;
                Vertex vertex = new Vertex()
                {
                    ModelPos = new Vec3(vertexBuffer[startPos], vertexBuffer[startPos + 1], vertexBuffer[startPos + 2]),
                    Normal = new Vec3(vertexBuffer[startPos + 3], vertexBuffer[startPos + 4], vertexBuffer[startPos + 5]),
                    UV = new Vec2(vertexBuffer[startPos + 6], vertexBuffer[startPos + 7]),
                    Color = new Color(vertexBuffer[startPos + 8], vertexBuffer[startPos + 9], vertexBuffer[startPos + 10], vertexBuffer[startPos + 11])
                };
                vertices[i] = vertex;
            }

            return vertices;
        }

        Vertex[] VertexShader(Vertex[] vertices, Shader shader)
        {
            Vertex[] transformedVertices = new Vertex[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                transformedVertices[i] = shader.Vertex(vertices[i]);
            }
            return transformedVertices;
        }

        Triangle[] PrimitiveAssembler(Vertex[] vertices, int[] indexBuffer)
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
                    Vertex0 = vertices[index0],
                    Vertex1 = vertices[index1],
                    Vertex2 = vertices[index2]
                };

                triangles[i] = triangle;
            }
            return triangles;
        }

        bool Clipping(Triangle triangle)
        {
            return triangle.Vertex0.ClipPos.w <= 0 && triangle.Vertex1.ClipPos.w <= 0 && triangle.Vertex2.ClipPos.w <= 0;
        }

        Vec3 PerspectiveDivide(Vec4 clipPos)
        {
            return clipPos / clipPos.w;
        }

        Vec3 ViewportMapping(Vec3 ndcPos, (int minX, int minY, int maxX, int maxY) viewport)
        {
            int width = viewport.maxX - viewport.minX + 1;
            int height = viewport.maxY - viewport.minY + 1;

            float x = viewport.minX + (ndcPos.x * 0.5f + 0.5f) * width;
            float y = viewport.minY + (-ndcPos.y * 0.5f + 0.5f) * height;
            float z = ndcPos.z * 0.5f + 0.5f;

            return new Vec3(x, y, z);
        }

        bool BackfaceCulling(Triangle triangle)
        {
            return SignedTriangleArea(triangle.Vertex0.ScreenPos, triangle.Vertex1.ScreenPos, triangle.Vertex2.ScreenPos) < 1;
        }

        void Rasterizer(Triangle triangle, (int minX, int minY, int maxX, int maxY) viewport, Buffer<float> depthTarget, List<Fragment> fragmentsOut)
        {
            Vertex v0 = triangle.Vertex0;
            Vertex v1 = triangle.Vertex1;
            Vertex v2 = triangle.Vertex2;

            // TODO: Normals per vertex
            Vec3 normal = Vec3.Cross(v1.WorldPos - v0.WorldPos, v2.WorldPos - v0.WorldPos);

            int minX = (int)Mathf.Max(Mathf.Min(v0.ScreenPos.x, v1.ScreenPos.x, v2.ScreenPos.x), viewport.minX);
            int maxX = (int)Mathf.Min(Mathf.Max(v0.ScreenPos.x, v1.ScreenPos.x, v2.ScreenPos.x), viewport.maxX);
            int minY = (int)Mathf.Max(Mathf.Min(v0.ScreenPos.y, v1.ScreenPos.y, v2.ScreenPos.y), viewport.minY);
            int maxY = (int)Mathf.Min(Mathf.Max(v0.ScreenPos.y, v1.ScreenPos.y, v2.ScreenPos.y), viewport.maxY);

            float area = SignedTriangleArea(v0.ScreenPos, v1.ScreenPos, v2.ScreenPos);
            float invArea = 1f / area;

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    Vec3 p = new Vec3(x, y, 0);
                    float alpha = SignedTriangleArea(p, v1.ScreenPos, v2.ScreenPos) * invArea;
                    float beta = SignedTriangleArea(p, v2.ScreenPos, v0.ScreenPos) * invArea;
                    float gamma = SignedTriangleArea(p, v0.ScreenPos, v1.ScreenPos) * invArea;
                    if (alpha < 0 || beta < 0 || gamma < 0) continue;

                    float z = alpha * v0.ScreenPos.z + beta * v1.ScreenPos.z + gamma * v2.ScreenPos.z;
                    if (z >= depthTarget[x, y]) continue;

                    depthTarget[x, y] = z;
                    p.z = z;

                    Fragment fragment = new Fragment
                    {
                        ScreenPos = p,
                        UV = alpha * v0.UV + beta * v1.UV + gamma * v2.UV,
                        Normal = normal,  //alpha * v0.Normal + beta * v1.Normal + gamma * v2.Normal,
                        VertexColor = alpha * v0.Color + beta * v1.Color + gamma * v2.Color
                    };
                    fragmentsOut.Add(fragment);
                }
            }
        }

        /*void DepthTest(List<FragmentInput> fragments)
        {
            for (int i = fragments.Count - 1; i >= 0; i--)
            {
                FragmentInput fragment = fragments[i];
                Vec3 position = fragment.ScreenPos;
                if (framebuffer.ReadDepth((int)position.x, (int)position.y) <= position.z)
                    fragments.RemoveAt(i);
            }
        }*/

        Fragment[] FragmentShader(List<Fragment> inputs, Shader shader)
        {
            Fragment[] fragments = new Fragment[inputs.Count];
            for (int i = 0; i < inputs.Count; i++)
            {
                Fragment fragment = inputs[i];
                fragment.Color = shader.Fragment(fragment, textures, samplers);
                fragments[i] = fragment;
            }
            return fragments;
        }

        void Blending(Fragment[] fragments, Buffer<Color> colorTarget)
        {
            foreach (Fragment fragment in fragments)
            {
                colorTarget[(int)fragment.ScreenPos.x, (int)fragment.ScreenPos.y] = fragment.Color;
            }
        }

        // INTERNAL
        void DrawLine(int x0, int y0, int x1, int y1, Color color)
        {
            int dx = Mathf.Abs(x1 - x0);
            int dy = -Mathf.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx + dy;

            while (true)
            {
                //SetPixel(x0, y0, color);

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

        float SignedTriangleArea(Vec3 a, Vec3 b, Vec3 c)
        {
            return (a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y)) / 2.0f;
        }
    }

    public struct Vertex
    {
        public Vec3 ModelPos;
        public Vec3 WorldPos;
        public Vec4 ClipPos;
        public Vec3 ScreenPos;
        public Vec2 UV;
        public Vec3 Normal;
        public Color Color;
    }

    struct Triangle
    {
        public Vertex Vertex0;
        public Vertex Vertex1;
        public Vertex Vertex2;
    }

    public struct Fragment
    {
        public Vec3 ScreenPos;
        public Vec2 UV;
        public Vec3 Normal;
        public Color VertexColor;
        public Color Color;
    }
}
