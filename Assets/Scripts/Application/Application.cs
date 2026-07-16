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
        Engine engine;

        Shader litShader;
        Shader redShader;
        Shader vertexColorShader;
        Shader uvShader;

        Texture billCypherTexture;
        Sampler billCypherSampler;

        Scene scene;

        GameObject billCypher;

        PerspectiveCamera perspectiveCamera;
        OrthographicCamera orthographicCamera;

        public float FPS { get; private set; } = 0f;

        public Application(ApplicationSettings settings, Engine engine)
        {
            this.settings = settings;
            this.engine = engine;
        }

        public IEnumerator RunProgram()
        {
            engine.Setup(1920, 1080);

            litShader = new LitShader();
            redShader = new UnlitShader(Color.red);
            vertexColorShader = new VertexColorShader();
            uvShader = new UVShader();

            scene = new Scene();

            billCypher = new GameObject();

            billCypher.Transform.Rotation = new Vec3(0, Mathf.PI / 4, 0);
            billCypher.Transform.Scale = new Vec3(settings.Scale, settings.Scale, 1);

            billCypher.Mesh = Mesh.Pyramid;

            billCypherTexture = new Texture(256, 256);
            billCypherTexture.SetPixels(settings.BillCypher.GetPixels());
            billCypherSampler = new Sampler(Sampler.FilterMode.Point, Sampler.WrapMode.Repeat);

            billCypher.Material = new Material()
            {
                Shader = litShader,
                Texture = billCypherTexture,
                Sampler = billCypherSampler
            };

            scene.GameObjects = new GameObject[]
            {
                billCypher
            };

            perspectiveCamera = new PerspectiveCamera(16f / 9f);
            perspectiveCamera.FieldOfView = 90.0f;
            perspectiveCamera.Transform.Position = new Vec3(0, 0, 2.5f);

            orthographicCamera = new OrthographicCamera(16f / 9f);
            orthographicCamera.Size = 2.0f;
            orthographicCamera.Transform.Position = new Vec3(0, 0, 2.5f);

            scene.Camera = perspectiveCamera;

            scene.MainLight = new DirectionalLight(Color.white);
            scene.MainLight.Transform.Rotation = new Vec3(0, Mathf.PI, 0);

            scene.AmbientLight = new Color(0.1f, 0.1f, 0.1f, 1);

            while (true)
            {
                OnUpdate();
                OnRender();
                yield return new WaitForEndOfFrame();
            }
        }

        void OnUpdate()
        {
            billCypher.Transform.Rotation += new Vec3(0, 1f, 0) * Time.deltaTime;
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

            if (settings.CameraType == CameraType.Perspective)
                scene.Camera = perspectiveCamera;
            else
                scene.Camera = orthographicCamera;

            FPS = 1f / Time.deltaTime;
        }

        void OnRender()
        {
            engine.RenderScene(scene);
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
