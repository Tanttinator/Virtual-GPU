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
        for (int y = 0; y < screen.Height; y++)
        {
            for (int x = 0; x < screen.Width; x++)
            {
                Color color = new Color((float)x / screen.Width, (float)y / screen.Height, 0.5f);
                gpu.SetPixel(x, y, color);
            }
        }
        gpu.Present();
    }

    private void Start()
    {
        RunProgram();
    }
}
