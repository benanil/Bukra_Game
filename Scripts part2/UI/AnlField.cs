

using UnityEngine;

public abstract class AnlField : MonoBehaviour
{
    protected Vector2 StartPos;
    protected Vector2 position;
    protected bool Pressed;

    public float X
    {
        get
        {
            var result = StartPos.x - position.x;
            if (result < -1)
                result = -1;
            if (result > 1)
                result = 1;
            return result;
        }
    }

    public float Y
    {
        get
        {
            var result = StartPos.y - position.y;

            if (result < -1)
                result = -1;
            if (result > 1)
                result = 1;
            return result;
        }
    }
}

