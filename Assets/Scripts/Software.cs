using UnityEngine;

public class Software : MonoBehaviour
{
    [SerializeField] Screen screen;
    [SerializeField] GPU gpu;

    Shader redShader;
    Shader uvShader;

    void RunProgram()
    {
        redShader = new FlatColorShader(Color.red);
        uvShader = new UVShader();
        RenderFrame();
    }

    void RenderFrame()
    {
        gpu.Clear(Color.black);
        Vertex v0 = new Vertex(new Vec3(100, 100, 0), new Vec2(0, 0));
        Vertex v1 = new Vertex(new Vec3(200, 300, 0), new Vec2(1, 0));
        Vertex v2 = new Vertex(new Vec3(300, 100, 0), new Vec2(0.5f, 1));
        gpu.DrawTriangle(v0, v1, v2, uvShader);
        gpu.Present();
    }

    private void Start()
    {
        RunProgram();
    }
}
