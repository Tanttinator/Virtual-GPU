using System.Collections;
using UnityEngine;

namespace VirtualGPU
{
    public class Application : MonoBehaviour
    {
        public enum CameraType
        {
            Perspective,
            Orthographic
        }

        [Header("Program Parameters")]
        [SerializeField] bool wireframe = false;
        [SerializeField] float scale = 1.0f;
        [SerializeField] CameraType cameraType = CameraType.Perspective;

        [Header("Resources")]
        [SerializeField] Screen screen;
        [SerializeField] GPU gpu;

        Shader litShader;
        Shader redShader;
        Shader vertexColorShader;
        Shader uvShader;

        Mesh quad;
        Mesh billCypher;
        new Transform transform;

        PerspectiveCamera perspectiveCamera;
        OrthographicCamera orthographicCamera;

        Color ambientLight = new Color(0.1f, 0.1f, 0.1f, 1);
        DirectionalLight directionalLight;

        IEnumerator RunProgram()
        {
            litShader = new LitShader();
            redShader = new UnlitShader(Color.red);
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
                Indices = new int[] { 0, 2, 1, 0, 3, 2, 0, 4, 1, 1, 4, 2, 2, 4, 3, 3, 4, 0 }
            };

            transform = new Transform();
            transform.Rotation = new Vec3(0, Mathf.PI / 4, 0);
            transform.Scale = new Vec3(scale, scale, 1);

            perspectiveCamera = new PerspectiveCamera(screen.Width, screen.Height);
            perspectiveCamera.FieldOfView = 90.0f;
            perspectiveCamera.Transform.Position = new Vec3(0, 0, 2.5f);

            orthographicCamera = new OrthographicCamera(screen.Width, screen.Height);
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
        }

        void OnRender()
        {
            gpu.Clear(Color.black);

            Mesh mesh = billCypher;

            gpu.BindVertexBuffer(mesh.Vertices);
            gpu.BindIndexBuffer(mesh.Indices);

            Camera camera = null;
            if (cameraType == CameraType.Perspective)
            {
                camera = perspectiveCamera;
            }
            else if (cameraType == CameraType.Orthographic)
            {
                camera = orthographicCamera;
            }

            Shader shader = litShader;
            shader.Uniforms.ModelMatrix = transform.GetModelMatrix();
            shader.Uniforms.ViewMatrix = camera.GetViewMatrix();
            shader.Uniforms.ProjectionMatrix = camera.GetProjectionMatrix();
            shader.Uniforms.AmbientLight = ambientLight;
            shader.Uniforms.MainLight = directionalLight;

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
