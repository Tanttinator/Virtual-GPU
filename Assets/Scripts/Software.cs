using System.Collections;
using UnityEngine;

namespace VirtualGPU
{
    public class Software : MonoBehaviour
    {
        [Header("Program Parameters")]
        [SerializeField] bool wireframe = false;
        [SerializeField] float scale = 1.0f;

        [Header("Resources")]
        [SerializeField] Screen screen;
        [SerializeField] GPU gpu;

        Shader redShader;
        Shader vertexColorShader;
        Shader uvShader;

        Mesh quad;
        Mesh billCypher;
        new Transform transform;

        new Camera camera;

        IEnumerator RunProgram()
        {
            redShader = new FlatColorShader(Color.red);
            vertexColorShader = new VertexColorShader();
            uvShader = new UVShader();

            quad = new Mesh()
            {
                Vertices = new Vertex[]
                {
                    new Vertex(new Vec3(-0.5f, -0.5f, 0), new Vec2(0, 0), new Color(1, 1, 1, 1)),
                    new Vertex(new Vec3(-0.5f, 0.5f, 0), new Vec2(0, 1), new Color(1, 0, 0, 1)),
                    new Vertex(new Vec3(0.5f, 0.5f, 0), new Vec2(1, 1), new Color(0, 1, 0, 1)),
                    new Vertex(new Vec3(0.5f, -0.5f, 0), new Vec2(1, 0), new Color(0, 0, 1, 1))
                },
                Indices = new int[] { 0, 1, 2, 0, 2, 3 }
            };

            billCypher = new Mesh()
            {
                Vertices = new Vertex[]
                {
                    new Vertex(new Vec3(-0.5f, -0.5f, -0.5f), new Vec2(0, 0)),
                    new Vertex(new Vec3(-0.5f, -0.5f, 0.5f), new Vec2(0, 1)),
                    new Vertex(new Vec3(0.5f, -0.5f, 0.5f), new Vec2(1, 1)),
                    new Vertex(new Vec3(0.5f, -0.5f, -0.5f), new Vec2(1, 0)),
                    new Vertex(new Vec3(0f, 0.5f, 0f), new Vec2(0, 0)),
                },
                Indices = new int[] { 0, 1, 2, 0, 2, 3, 0, 4, 1, 1, 4, 2, 2, 4, 3, 3, 4, 0 }
            };

            transform = new Transform();
            transform.Scale = new Vec3(scale, scale, 1);

            camera = new Camera(screen.Width, screen.Height);
            camera.Transform.Position = new Vec3(0, 0, 10);

            while (true)
            {
                OnUpdate();
                RenderFrame();
                yield return new WaitForEndOfFrame();
            }
        }

        void OnUpdate()
        {
            transform.Rotation += new Vec3(0, 1f, 0) * Time.deltaTime;
        }

        void RenderFrame()
        {
            gpu.Clear(Color.black);

            Mesh mesh = billCypher;

            gpu.BindVertexBuffer(mesh.Vertices);
            gpu.BindIndexBuffer(mesh.Indices);

            Shader shader = vertexColorShader;
            shader.Uniforms.MVPMatrix = camera.GetProjectionMatrix() * camera.GetViewMatrix() * transform.GetModelMatrix();

            if (wireframe)
                gpu.DrawWireframe(shader);
            else
                gpu.Draw(shader);

            gpu.Present();
        }

        private void Start()
        {
            StartCoroutine(RunProgram());
        }
    }
}
