using System.Numerics;

namespace Concrete;

public class Material
{
    public Vector4 color = Vector4.One;
    public uint? albedoTexture = null;
    public uint? roughnessTexture = null;
}