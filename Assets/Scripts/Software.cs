using UnityEngine;

public class Software : MonoBehaviour
{
    [SerializeField] Screen screen;
    [SerializeField] GPU gpu;

    void RunProgram()
    {
        RenderFrame();
    }

    void RenderFrame()
    {
        gpu.Clear(Color.black);
        gpu.DrawTriangle(new Vec2(100, 100), new Vec2(200, 300), new Vec2(300, 100), Color.red);
        gpu.Present();
    }

    private void Start()
    {
        RunProgram();
    }
}
