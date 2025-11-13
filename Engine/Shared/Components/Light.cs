using System.Drawing;

namespace Concrete;

public abstract class Light : Component
{
    [Show] public float brightness = 1;
    [Show] public Color color = Color.White;

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
    [Show] public float range = 10;
}

public class DirectionalLight : Light
{
    // no unique variables
}

public class SpotLight : Light
{
    [Show] public float range = 4;
    [Show] public float angle = 30;
    [Show] public float softness = 0.5f;
}