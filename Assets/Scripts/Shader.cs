using UnityEngine;

namespace VirtualGPU
{
    public abstract class Shader
    {
        public Uniform Uniforms { get; private set; } = new Uniform();

        public virtual Varyings Vertex(Vertex vertex)
        {
            Varyings varyings = new Varyings();
            varyings.ClipPos = Uniforms.MVPMatrix * new Vec4(vertex.Position, 1.0f);
            varyings.UV = vertex.UV;
            varyings.Normal = vertex.Normal;
            varyings.Color = vertex.Color;
            return varyings;
        }

        public virtual Color Fragment(Varyings varyings)
        {
            return new Color(1, 1, 1, 1);
        }
    }

    public class Varyings
    {
        public Vec4 ClipPos;
        public Vec2 UV;
        public Vec3 Normal;
        public Color Color;
    }

    public class Uniform
    {
        public Mat4 MVPMatrix;
    }

    public class FlatColorShader : Shader
    {
        private Color color;

        public FlatColorShader(Color color)
        {
            this.color = color;
        }

        public override Color Fragment(Varyings varyings)
        {
            return color;
        }
    }

    public class VertexColorShader : Shader
    {
        public override Color Fragment(Varyings varyings)
        {
            return varyings.Color;
        }
    }

    public class UVShader : Shader
    {
        public override Color Fragment(Varyings varyings)
        {
            Vec2 uv = varyings.UV;
            return new Color(uv.x, uv.y, 0.0f, 1.0f);
        }
    }
}