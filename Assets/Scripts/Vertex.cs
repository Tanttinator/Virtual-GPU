using UnityEngine;

public class Vertex
{
    public Vec3 Position { get; private set; }
    public Vec2 UV { get; private set; }

    public Vertex(Vec3 position, Vec2 uv)
    {
        Position = position;
        UV = uv;
    }
}
