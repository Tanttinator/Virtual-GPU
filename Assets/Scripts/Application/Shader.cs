using UnityEngine;

namespace VirtualGPU
{
    public abstract class Shader
    {
        public Uniform Uniforms { get; private set; } = new Uniform();

        public virtual Varyings Vertex(Vertex vertex)
        {
            Varyings varyings = new Varyings();
            varyings.WorldPos = Uniforms.ModelMatrix * vertex.Position;
            varyings.ClipPos = Uniforms.ProjectionMatrix * Uniforms.ViewMatrix * new Vec4(varyings.WorldPos, 1f);
            varyings.UV = vertex.UV;
            varyings.Normal = vertex.Normal;
            varyings.Color = vertex.Color;
            return varyings;
        }

        public virtual Color Fragment(FragmentData data)
        {
            return new Color(1, 1, 1, 1);
        }
    }

    public class Varyings
    {
        public Vec3 WorldPos;
        public Vec4 ClipPos;
        public Vec2 UV;
        public Vec3 Normal;
        public Color Color;
    }

    public class FragmentData
    {
        public Vec3 ScreenPos;
        public Vec2 UV;
        public Vec3 Normal;
        public Color VertexColor;
    }

    public class Uniform
    {
        public Mat4 ModelMatrix;
        public Mat4 ViewMatrix;
        public Mat4 ProjectionMatrix;
        public Color AmbientLight;
        public DirectionalLight MainLight;
    }

    public class LitShader : Shader
    {
        public Color Color = Color.white;

        public override Color Fragment(FragmentData data)
        {
            Vec3 normal = data.Normal;
            Vec3 lightDir = Uniforms.MainLight.GetLightDirection();
            float intensity = Mathf.Max(0, Vec3.Dot(normal, -lightDir));

            Color ambient = Uniforms.AmbientLight;
            Color diffuse = Color * intensity;
            return ambient + diffuse;
        }
    }

    public class UnlitShader : Shader
    {
        private Color color;

        public UnlitShader(Color color)
        {
            this.color = color;
        }

        public override Color Fragment(FragmentData data)
        {
            return color;
        }
    }

    public class VertexColorShader : Shader
    {
        public override Color Fragment(FragmentData data)
        {
            return data.VertexColor;
        }
    }

    public class UVShader : Shader
    {
        public override Color Fragment(FragmentData data)
        {
            Vec2 uv = data.UV;
            return new Color(uv.x, uv.y, 0.0f, 1.0f);
        }
    }
}