using UnityEngine;

public abstract class Shader
{
    public abstract Color Fragment(Vec2 uv);
}

public class FlatColorShader : Shader
{
    private Color color;

    public FlatColorShader(Color color)
    {
        this.color = color;
    }

    public override Color Fragment(Vec2 uv)
    {
        return color;
    }
}

public class UVShader : Shader
{
    public override Color Fragment(Vec2 uv)
    {
        return new Color(uv.x, uv.y, 0.0f, 1.0f);
    }
}
