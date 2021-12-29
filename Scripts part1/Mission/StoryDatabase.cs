using UnityEngine;

public class StoryDatabase : MonoBehaviour
{
    public StoryMission[] StoryDatas;

    public StoryMission GetCurrentstory(int id)
    {
        return StoryDatas[id];
    }
}

[System.Serializable]
public struct StoryMission
{
    public int id;
    public StageTrigger[] stages;
}

