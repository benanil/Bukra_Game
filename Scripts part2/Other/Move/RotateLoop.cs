
using AnilTools;
using UnityEngine;
using UrFairy;

public class RotateLoop : MonoBehaviour
{
    public enum Axis
    {
        x , y , z
    }

    public Axis axis;
    public float angle;
    public float Speed = 1;
    
    [Range(-1,1f)]
    public float startSinValue;

    Quaternion rightTarget;
    Quaternion leftTarget;

    private void OnValidate()
    {
        Start();
    }

    private void Start()
    {
        switch (axis)
        {
            case Axis.x:
                rightTarget = Quaternion.Euler(transform.localEulerAngles.XPlus(angle));
                leftTarget = Quaternion.Euler(transform.localEulerAngles.XPlus(-angle));
                break;
            case Axis.y:
                rightTarget = Quaternion.Euler(transform.localEulerAngles.YPlus(angle));
                leftTarget = Quaternion.Euler(transform.localEulerAngles.YPlus(-angle));
                break;
            case Axis.z:
                rightTarget = Quaternion.Euler(transform.localEulerAngles.ZPlus(angle));
                leftTarget = Quaternion.Euler(transform.localEulerAngles.ZPlus(-angle));
                break;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        transform.localRotation = Quaternion.Slerp(leftTarget, rightTarget , Mathmatic.Remap(Mathf.Sin(startSinValue + Time.time * Speed))); 
    }
}
