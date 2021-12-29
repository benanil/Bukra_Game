using UnityEngine;

public class MillController : MonoBehaviour
{
    private const float Speed = 0.2f;

    private void Update()
    {
        transform.Rotate(Vector3.forward * Speed, Space.World);
    }

}
