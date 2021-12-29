using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    // todo async yap
    private void LateUpdate()
    {
        transform.position = Input.mousePosition;
    }
}


