using AnilTools;
using UnityEngine;

public class PlayerInventoryCamera : MonoBehaviour
{
    [SerializeField] private float lookSpeed = 4;
    [SerializeField] private float distFromCameraTarget = 5;
    [SerializeField] private float cameraHeight = 1.5f;
    
    private FixedTouchField TouchField;
    private Transform PlayerTransform;
    
    public static bool canLook;
    private float yaw;
    private float pitch;

    Vector3 targetRotation;

    private void Start()
    {
        TouchField = MobileInput.instance.FindTouchField("Equipment");
        yaw = transform.eulerAngles.x;
        PlayerTransform = NpcController2.Player;
    }

    private void Update()
    {
        if (canLook)
        {
            pitch += TouchField.TouchDist.x * lookSpeed;

            targetRotation = new Vector3(yaw, pitch);

            transform.eulerAngles = targetRotation;
            transform.position = PlayerTransform.position - transform.forward * distFromCameraTarget + SVector3.up * cameraHeight;
        }
    }
}
