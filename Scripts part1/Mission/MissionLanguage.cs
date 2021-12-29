
using UnityEngine;

[CreateAssetMenu(menuName = "Language/MissionLanguage",fileName = "MushroomLanguage")]
public class MissionLanguage : ScriptableObject
{
    public string MissionName;
    [Multiline]
    public string TurnBackText;
    [TextArea]
    public string Description;

    [Multiline]
    public string[] goals;
}
