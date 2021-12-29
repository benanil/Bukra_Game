using UnityEngine;

public class SlotInfo : MonoBehaviour
{
    [HideInInspector] public short EnemyCount;

    private void Start()
    {
        //InvokeRepeating(nameof(SpawnUpdate), 5, 23.4f);
    }

    public void SpawnUpdate()
    {
        if (EnemyCount == 0)
        {
            // eğer slottaki enemy bitti ise yapılacaklar
        }
    }

}

