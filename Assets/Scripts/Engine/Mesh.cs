using UnityEngine;
using UnityEngine.UIElements;

namespace VirtualGPU
{
    public class Mesh
    {
        public Vec3[] Vertices;
        public int[] Indices;
        public Vec3[] Normals;
        public Vec2[] UVs;
        public Color[] Colors;

        public float[] GetVertexBuffer()
        {
            int vertexCount = Vertices.Length;
            int vertexStride = 3 + 3 + 2 + 4;
            int bufferLength = vertexCount * vertexStride;
            float[] buffer = new float[bufferLength];

            for (int i = 0; i < vertexCount; i++)
            {
                int startPos = i * vertexStride;

                Vec3 vertex = Vertices[i];
                buffer[startPos] = vertex.x;
                buffer[startPos + 1] = vertex.y;
                buffer[startPos + 2] = vertex.z;

                Vec3 normal = Normals[i];
                buffer[startPos + 3] = normal.x;
                buffer[startPos + 4] = normal.y;
                buffer[startPos + 5] = normal.z;

                Vec2 uv = UVs[i];
                buffer[startPos + 6] = uv.x;
                buffer[startPos + 7] = uv.y;

                Color color = Colors[i];
                buffer[startPos + 8] = color.r;
                buffer[startPos + 9] = color.g;
                buffer[startPos + 10] = color.b;
                buffer[startPos + 11] = color.a;
            }

            return buffer;
        }

        public static readonly Mesh Quad = new Mesh()
        {
            Vertices = new Vec3[]
            {
                new Vec3(-0.5f, 0f, -0.5f), new Vec3(-0.5f, 0f, 0.5f), new Vec3(0.5f, 0f, 0.5f), new Vec3(0.5f, 0f, -0.5f)
            },
            Indices = new int[]
            {
                0, 1, 2,
                0, 2, 3
            },
            Normals = new Vec3[]
            {
                new Vec3(0, 1, 0), new Vec3(0, 1, 0), new Vec3(0, 1, 0), new Vec3(0, 1, 0)
            },
            UVs = new Vec2[]
            {
                new Vec2(0, 0), new Vec2(0, 1), new Vec2(1, 1), new Vec2(1, 0)
            },
            Colors = new Color[]
            {
                Color.white, Color.white, Color.white, Color.white
            }
        };

        public static readonly Mesh Pyramid = new Mesh()
        {
            Vertices = new Vec3[]
            {
                new Vec3(-0.5f, -0.5f, -0.5f), new Vec3(0f, 0.5f, 0f), new Vec3(0.5f, -0.5f, -0.5f), // Front
                new Vec3(-0.5f, -0.5f, 0.5f), new Vec3(0f, 0.5f, 0f), new Vec3(-0.5f, -0.5f, -0.5f), // Left
                new Vec3(0.5f, -0.5f, 0.5f), new Vec3(0f, 0.5f, 0f), new Vec3(-0.5f, -0.5f, 0.5f), // Back
                new Vec3(0.5f, -0.5f, -0.5f), new Vec3(0f, 0.5f, 0f), new Vec3(0.5f, -0.5f, 0.5f), // Right
                new Vec3(-0.5f, -0.5f, 0.5f), new Vec3(-0.5f, -0.5f, -0.5f), new Vec3(0.5f, -0.5f, -0.5f), new Vec3(0.5f, -0.5f, 0.5f) // Bottom
            },
            Indices = new int[]
            {
                0, 1, 2,
                3, 4, 5,
                6, 7, 8,
                9, 10, 11,
                12, 13, 14,
                12, 14, 15
            },
            Normals = new Vec3[]
            {
                new Vec3(0f, 0.4472f, -0.8944f), new Vec3(0f, 0.4472f, -0.8944f), new Vec3(0f, 0.4472f, -0.8944f),
                new Vec3(-0.8944f, 0.4472f, 0f), new Vec3(-0.8944f, 0.4472f, 0f), new Vec3(-0.8944f, 0.4472f, 0f),
                new Vec3(0f, 0.4472f, 0.8944f), new Vec3(0f, 0.4472f, 0.8944f), new Vec3(0f, 0.4472f, 0.8944f),
                new Vec3(0.8944f, 0.4472f, 0f), new Vec3(0.8944f, 0.4472f, 0f), new Vec3(0.8944f, 0.4472f, 0f),
                new Vec3(0f, -1f, 0f), new Vec3(0f, -1f, 0f), new Vec3(0f, -1f, 0f), new Vec3(0f, -1f, 0f)
            },
            UVs = new Vec2[]
            {
                new Vec2(0, 0), new Vec2(0.5f, 1), new Vec2(1, 0),
                new Vec2(0, 0), new Vec2(0.5f, 1), new Vec2(1, 0),
                new Vec2(0, 0), new Vec2(0.5f, 1), new Vec2(1, 0),
                new Vec2(0, 0), new Vec2(0.5f, 1), new Vec2(1, 0),
                new Vec2(0, 0), new Vec2(0, 1), new Vec2(1, 1), new Vec2(1, 0)
            },
            Colors = new Color[]
            {
                Color.white, Color.white, Color.white,
                Color.white, Color.white, Color.white,
                Color.white, Color.white, Color.white,
                Color.white, Color.white, Color.white,
                Color.white, Color.white, Color.white, Color.white
            }
        };
    }
}
