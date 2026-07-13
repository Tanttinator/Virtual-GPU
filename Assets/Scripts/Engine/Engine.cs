using UnityEngine;

namespace VirtualGPU
{
    public class Engine
    {
        OpenGL openGl;

        public Engine(OpenGL openGl)
        {
            this.openGl = openGl;
        }

        public void RenderScene(Scene scene)
        {
            openGl.ClearColor(scene.Camera.ClearColor);
            openGl.Clear();

            foreach (GameObject gameObject in scene.GameObjects)
            {
                Mesh mesh = gameObject.Mesh;
                openGl.BindVertexBuffer(mesh.Vertices);
                openGl.BindIndexBuffer(mesh.Indices);

                Transform transform = gameObject.Transform;
                Material material = gameObject.Material;

                Shader shader = material.Shader;
                shader.Uniforms.ModelMatrix = transform.GetModelMatrix();
                shader.Uniforms.ViewMatrix = scene.Camera.GetViewMatrix();
                shader.Uniforms.ProjectionMatrix = scene.Camera.GetProjectionMatrix();
                shader.Uniforms.AmbientLight = scene.AmbientLight;
                shader.Uniforms.MainLight = scene.MainLight;

                openGl.BindTexture(0, material.Texture);
                openGl.BindSampler(0, material.Sampler);

                openGl.Draw(shader);
            }

            openGl.SwapBuffers();
        }
    }
}
