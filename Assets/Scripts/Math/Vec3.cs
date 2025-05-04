using UnityEngine;

public class Vec3
{
    public readonly float x;
    public readonly float y;
    public readonly float z;

    public Vec3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static implicit operator Vec2(Vec3 v)
    {
        return new Vec2(v.x, v.y);
    }
}
