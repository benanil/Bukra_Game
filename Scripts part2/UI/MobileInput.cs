

using AnilTools;

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class MobileInput : Singleton<MobileInput>
{
    public List<FixedTouchField> touchFields;
    public List<Joystick> joyisticks;
    public List<AnilButton> anlButtons;

    public FixedTouchField FindTouchField(string name)
    {
        return touchFields.Find(x => x.name.StartsWith(name));
    }

    public Joystick FindJoyistic(string name)
    {
        return joyisticks.Find(x => x.name.StartsWith(name));
    }

    public AnilButton FindButton(string name)
    {
        return anlButtons.Find(x => x.name.StartsWith(name));
    }
}
#if UNITY_EDITOR

[CustomEditor(typeof(MobileInput))]
public class MobileInputEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var _target = (MobileInput)target;

        if (GUILayout.Button("Refresh"))
        {
            _target.anlButtons = FindObjectsOfType<AnilButton>().ToList();
            _target.joyisticks = FindObjectsOfType<Joystick>().ToList();
            _target.touchFields = FindObjectsOfType<FixedTouchField>().ToList();
        }
    }
}
#endif