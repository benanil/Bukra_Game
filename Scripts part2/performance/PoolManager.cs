using UnityEngine;
using System.Collections.Generic;
using AnilTools;
using Inventory;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PoolManager : Singleton<PoolManager>
{
	[SerializeField] private List<PoolData1> Items1;

	public Dictionary<PoolObject, Queue<GameObject>> poolDictionary ;

    private void Start()
    {
		poolDictionary = new Dictionary<PoolObject, Queue<GameObject>>();
        for (int i = 0; i < Items1.Count; i++)
        {
			var queue = new Queue<GameObject>();
            for (int j = 0; j < Items1[i].size; j++)
            {
				var go = Instantiate(Items1[i].prefab);
                go.transform.SetParent(transform, true);
                go.SetActive(false);
				queue.Enqueue(go);
            }
			poolDictionary.Add(Items1[i].poolObject, queue);
        }
		
    }

	public GameObject ReuseObject(PoolObject poolObject, Vector3 position, Quaternion rotation)
	{
		GameObject objectToReuse = default;

        Debug2.Log(Enum.GetName(typeof(PoolObject), poolObject), Color.cyan);

		if (poolDictionary.ContainsKey(poolObject))
		{
			objectToReuse = poolDictionary[poolObject].Dequeue();
			poolDictionary[poolObject].Enqueue(objectToReuse);

			objectToReuse.SetActive(true);
			objectToReuse.transform.SetPositionAndRotation(position, rotation);
			objectToReuse.transform.parent = null;

            Debug2.Log("reusing skelton", Color.cyan);
		}
        else
        {
			Debug2.Log("PoolManagere " + poolObject.ToString() + "ekle", Color.cyan);
        }

		return objectToReuse;
	}

	[Serializable]
	public struct PoolData1
	{
		public GameObject prefab;
		public PoolObject poolObject;
		public short size;
	}

}

#if UNITY_EDITOR
[CustomEditor(typeof(PoolManager))]
public class PoolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
	}
}
#endif
