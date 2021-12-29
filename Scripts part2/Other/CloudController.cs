using AnilTools;
using UnityEngine;

public class CloudController : MonoBehaviour
{
    private const float Speed = 0.02f;
    private void Update()
    {
        transform.Rotate(SVector3.up * Speed, Space.World);
    }
}

