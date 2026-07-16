using UnityEngine;

namespace VirtualGPU
{
    public class Engine
    {
        OpenGL openGl;
        Window window;

        public Engine(OpenGL openGl)
        {
            this.openGl = openGl;
        }

        public void Setup(int screenWidth, int screenHeight)
        {
            window = openGl.CreateWindow(screenWidth, screenHeight);
            openGl.MakeContextCurrent(window);
            openGl.Viewport(0, 0, screenWidth, screenHeight);
        }

        public void RenderScene(Scene scene)
        {
            openGl.ClearColor(scene.Camera.ClearColor);
            openGl.Clear();

            foreach (GameObject gameObject in scene.GameObjects)
            {
                Mesh mesh = gameObject.Mesh;
                openGl.BindVertexBuffer(mesh.GetVertexBuffer());
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
