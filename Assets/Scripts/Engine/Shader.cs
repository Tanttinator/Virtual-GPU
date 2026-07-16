using UnityEngine;

namespace VirtualGPU
{
    public abstract class Shader
    {
        public Uniform Uniforms { get; private set; } = new Uniform();

        public virtual Vertex Vertex(Vertex vertex)
        {
            vertex.WorldPos = Uniforms.ModelMatrix * vertex.ModelPos;
            vertex.ClipPos = Uniforms.ProjectionMatrix * Uniforms.ViewMatrix * new Vec4(vertex.WorldPos, 1f);
            return vertex;
        }

        public virtual Color Fragment(Fragment fragment, Texture[] textures, Sampler[] samplers)
        {
            return Color.white;
        }
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

        public override Color Fragment(Fragment fragment, Texture[] textures, Sampler[] samplers)
        {
            Texture mainTex = textures[0];
            Sampler mainTexSampler = samplers[0];

            Color albedo = mainTexSampler.Sample(mainTex, fragment.UV) * Color * fragment.VertexColor;

            Vec3 normal = fragment.Normal;
            Vec3 lightDir = Uniforms.MainLight.GetLightDirection();
            float intensity = Mathf.Max(0, Vec3.Dot(normal, -lightDir));

            Color ambient = albedo * Uniforms.AmbientLight;
            Color diffuse = albedo * intensity;
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

        public override Color Fragment(Fragment fragment, Texture[] textures, Sampler[] samplers)
        {
            return color;
        }
    }

    public class VertexColorShader : Shader
    {
        public override Color Fragment(Fragment fragment, Texture[] textures, Sampler[] samplers)
        {
            return fragment.VertexColor;
        }
    }

    public class UVShader : Shader
    {
        public override Color Fragment(Fragment fragment, Texture[] textures, Sampler[] samplers)
        {
            Vec2 uv = fragment.UV;
            return new Color(uv.x, uv.y, 0.0f, 1.0f);
        }
    }

    public class NormalsShader : Shader
    {
        public override Color Fragment(Fragment fragment, Texture[] textures, Sampler[] samplers)
        {
            Vec3 normal = fragment.Normal;
            return new Color(normal.x, normal.y, normal.z, 1.0f);
        }
    }
}