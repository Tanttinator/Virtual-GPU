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
        Vertex v1 = new Vertex(new Vec3(400, 100, 0), new Vec2(1, 0));
        Vertex v2 = new Vertex(new Vec3(100, 400, 0), new Vec2(0, 1));
        Vertex v3 = new Vertex(new Vec3(400, 400, 0), new Vec2(1, 1));
        Vertex[] vertices = { v0, v1, v2, v3 };
        int[] indices = { 0, 1, 2, 1, 3, 2 };

        gpu.SetVertexBuffer(vertices);
        gpu.SetIndexBuffer(indices);
        //gpu.Draw(uvShader);
        gpu.DrawWireframe();
        gpu.Present();
    }

    private void Start()
    {
        RunProgram();
    }
}
