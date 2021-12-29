using AnilTools;
using System.Collections.Generic;
using UnityEngine;

public class PopUpMenu : Singleton<PopUpMenu>
{
    [SerializeField]
    private PopUpPref Prefab;
    private Queue<PopUpPref> PrefPool = new Queue<PopUpPref>();
    private readonly byte PrefCount = 10;

    private Transform pool;

    private PopUpPref prefTMP;

    private void Start()
    {
        pool = GameObject.Find("pool").transform;

        StartPool();
    }

    private void StartPool()
    {
        for (int i = 0; i < PrefCount; i++){

            PopUpPref Result = Instantiate(Prefab);

            Result.transform.localEulerAngles = Vector3.zero;
            Result.transform.localScale = Vector3.one;
            Result.transform.SetParent(pool);

            PrefPool.Enqueue(prefTMP);
        }
    }

    public void PopUp(string text , Color color)
    {
        GameMenu.instance.canvases.WarningCanvas.SetVisuality(true);
        var obj = PrefPool.Dequeue();
        obj.Spawn(text, color);
        obj.transform.SetParent(transform);
        PrefPool.Enqueue(obj);
    }

    public void PopUp(string text, Color color , Sprite icon)
    {
        GameMenu.instance.canvases.WarningCanvas.SetVisuality(true);
        var obj = PrefPool.Dequeue();
        obj.Spawn(text, color , icon);
        obj.transform.SetParent(transform); 
        PrefPool.Enqueue(obj);
    }
}
