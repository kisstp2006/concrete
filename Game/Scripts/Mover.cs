using Concrete;
using System.Numerics;

public class Mover : Component
{
    [Show("move")] public bool move;

    public override void Update(float deltaTime)
    {
        if (move) gameObject.transform.localPosition += new Vector3(0, 0, deltaTime * -1);
    }
}