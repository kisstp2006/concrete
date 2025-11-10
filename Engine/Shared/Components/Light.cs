using System.Drawing;

namespace Concrete;

public abstract class Light : Component
{
    [SerializeMember] [Show] public float brightness = 1;
    [SerializeMember] [Show] public Color color = Color.White;

    public Light()
    {
        LightRegistry.registered.Add(this);
    }

    public override void Dispose()
    {
        LightRegistry.registered.Remove(this);
    }
}

public class PointLight : Light
{
    [SerializeMember] [Show] public float range = 10;
}

public class DirectionalLight : Light
{
    // no unique variables
}

public class SpotLight : Light
{
    [SerializeMember] [Show] public float range = 4;
    [SerializeMember] [Show] public float angle = 30;
    [SerializeMember] [Show] public float softness = 0.5f;
}