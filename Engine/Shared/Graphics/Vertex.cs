using System.Numerics;

namespace Concrete;

public struct Vertex
{
    public Vector3 position;
    public Vector3 normal;
    public Vector2 uv;
    public Vector4 joints;
    public Vector4 weights;
}