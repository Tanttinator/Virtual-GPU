using UnityEngine;

public class GPU : MonoBehaviour
{
    [SerializeField] Screen screen;

    Framebuffer framebuffer;

    public void Clear(Color color)
    {
        framebuffer.Clear(color);
    }

    public void SetPixel(int x, int y, Color color)
    {
        framebuffer.SetPixel(x, y, color);
    }

    public void Present()
    {
        screen.Draw(framebuffer.ColorBuffer);
    }

    private void Awake()
    {
        framebuffer = new Framebuffer(screen.Width, screen.Height);
    }
}
