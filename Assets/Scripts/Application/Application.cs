using System;
using System.Collections;
using UnityEngine;

namespace VirtualGPU
{
    public class Application
    {
        public enum CameraType
        {
            Perspective,
            Orthographic
        }

        ApplicationSettings settings;
        OpenGL openGl;

        Shader litShader;
        Shader redShader;
        Shader vertexColorShader;
        Shader uvShader;

        Texture billCypherTexture;
        Sampler billCypherSampler;

        Mesh quad;
        Mesh billCypher;
        Transform transform;

        PerspectiveCamera perspectiveCamera;
        OrthographicCamera orthographicCamera;

        Color ambientLight = new Color(0.1f, 0.1f, 0.1f, 1);
        DirectionalLight directionalLight;

        public float FPS { get; private set; } = 0f;

        public Application(ApplicationSettings settings, OpenGL openGl)
        {
            this.settings = settings;
            this.openGl = openGl;
        }

        public IEnumerator RunProgram()
        {
            litShader = new LitShader();
            redShader = new UnlitShader(Color.red);
            vertexColorShader = new VertexColorShader();
            uvShader = new UVShader();

            billCypherTexture = new Texture(256, 256);
            billCypherTexture.SetPixels(settings.BillCypher.GetPixels());
            billCypherSampler = new Sampler(Sampler.FilterMode.Point, Sampler.WrapMode.Repeat);

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
                    new Vertex(new Vec3(0f, 0.5f, 0f), new Vec2(0.5f, 1f)), // Top

                    // Front face
                    new Vertex(new Vec3(-0.5f, -0.5f, 0.5f), new Vec2(0, 0)),
                    new Vertex(new Vec3(0.5f, -0.5f, 0.5f), new Vec2(1, 0)),

                    // Back face
                    new Vertex(new Vec3(0.5f, -0.5f, -0.5f), new Vec2(0, 0)),
                    new Vertex(new Vec3(-0.5f, -0.5f, -0.5f), new Vec2(1, 0)),

                    // Left face
                    new Vertex(new Vec3(-0.5f, -0.5f, -0.5f), new Vec2(0, 0)),
                    new Vertex(new Vec3(-0.5f, -0.5f, 0.5f), new Vec2(1, 0)),

                    // Right face
                    new Vertex(new Vec3(0.5f, -0.5f, 0.5f), new Vec2(0, 0)),
                    new Vertex(new Vec3(0.5f, -0.5f, -0.5f), new Vec2(1, 0)),

                    // Bottom face
                    new Vertex(new Vec3(-0.5f, -0.5f, -0.5f), new Vec2(0, 0)),
                    new Vertex(new Vec3(-0.5f, -0.5f, 0.5f), new Vec2(0, 1)),
                    new Vertex(new Vec3(0.5f, -0.5f, 0.5f), new Vec2(1, 1)),
                    new Vertex(new Vec3(0.5f, -0.5f, -0.5f), new Vec2(1, 0)),
                },
                Indices = new int[]
                {
                    // Front face
                    1, 0, 2,
                    // Back face
                    3, 0, 4,
                    // Left face
                    5, 0, 6,
                    // Right face
                    7, 0, 8,
                    // Bottom face
                    9, 10, 11,
                    9, 11, 12,
                }
            };

            transform = new Transform();
            transform.Rotation = new Vec3(0, Mathf.PI / 4, 0);
            transform.Scale = new Vec3(settings.Scale, settings.Scale, 1);

            perspectiveCamera = new PerspectiveCamera(16f / 9f);
            perspectiveCamera.FieldOfView = 90.0f;
            perspectiveCamera.Transform.Position = new Vec3(0, 0, 2.5f);

            orthographicCamera = new OrthographicCamera(16f / 9f);
            orthographicCamera.Size = 2.0f;
            orthographicCamera.Transform.Position = new Vec3(0, 0, 2.5f);

            directionalLight = new DirectionalLight(Color.white);
            directionalLight.Transform.Rotation = new Vec3(0, Mathf.PI, 0);

            while (true)
            {
                OnUpdate();
                OnRender();
                yield return new WaitForEndOfFrame();
            }
        }

        void OnUpdate()
        {
            transform.Rotation += new Vec3(0, 1f, 0) * Time.deltaTime;
            //directionalLight.Transform.Rotation += new Vec3(0, 1, 0) * Time.deltaTime;
            //perspectiveCamera.Transform.Rotation += new Vec3(1, 0, 0) * Time.deltaTime;

            if (Input.GetKey(KeyCode.W))
            {
                perspectiveCamera.Transform.Position += perspectiveCamera.Transform.Forward * Time.deltaTime;
                orthographicCamera.Transform.Position += perspectiveCamera.Transform.Forward * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.S))
            {
                perspectiveCamera.Transform.Position -= perspectiveCamera.Transform.Forward * Time.deltaTime;
                orthographicCamera.Transform.Position -= perspectiveCamera.Transform.Forward * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.A))
            {
                perspectiveCamera.Transform.Rotation += new Vec3(0, 1f, 0) * Time.deltaTime;
                orthographicCamera.Transform.Rotation += new Vec3(0, 1f, 0) * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.D))
            {
                perspectiveCamera.Transform.Rotation += new Vec3(0, -1f, 0) * Time.deltaTime;
                orthographicCamera.Transform.Rotation += new Vec3(0, -1f, 0) * Time.deltaTime;
            }

            FPS = 1f / Time.deltaTime;
        }

        void OnRender()
        {
            openGl.ClearColor(Color.black);
            openGl.Clear();

            Mesh mesh = billCypher;

            openGl.BindVertexBuffer(mesh.Vertices);
            openGl.BindIndexBuffer(mesh.Indices);

            Camera camera = null;
            if (settings.CameraType == CameraType.Perspective)
            {
                camera = perspectiveCamera;
            }
            else if (settings.CameraType == CameraType.Orthographic)
            {
                camera = orthographicCamera;
            }

            Shader shader = litShader;
            shader.Uniforms.ModelMatrix = transform.GetModelMatrix();
            shader.Uniforms.ViewMatrix = camera.GetViewMatrix();
            shader.Uniforms.ProjectionMatrix = camera.GetProjectionMatrix();
            shader.Uniforms.AmbientLight = ambientLight;
            shader.Uniforms.MainLight = directionalLight;

            openGl.BindTexture(0, billCypherTexture);
            openGl.BindSampler(0, billCypherSampler);

            if (settings.Wireframe)
                openGl.DrawWireframe(shader);
            else
                openGl.Draw(shader);

            openGl.SwapBuffers();
        }
    }

    [Serializable]
    public class ApplicationSettings
    {
        public bool Wireframe = false;
        public float Scale = 1f;
        public Application.CameraType CameraType;

        public Texture2D BillCypher;
    }
}
