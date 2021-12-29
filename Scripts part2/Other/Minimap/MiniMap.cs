using AnilTools;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : Singleton<MiniMap>
{
    // [SerializeField]
    // [Tooltip("Playerin ayağının altında nereye gideceğini gösterir")]
    // private Transform Indexer;

    [SerializeField]
    [Tooltip("av yapılacak alan vs işaretlemek için")]
    private GameObject MapAreaIndexer;

    private NpcController2 indexedNpcController;

    [NonSerialized]
    public Vector3 TargetPosition;

    [SerializeField] private List<GameObject> myField;

    private void Start()
    {
        // Indexer.gameObject.SetActive(false);
        if(MapAreaIndexer != null)
        MapAreaIndexer.SetActive(true);
    }

    private void Update()
    {
        // if (TargetPosition != default)
        // {
        //     Indexer.transform.OnlyYRot(Quaternion.LookRotation(Indexer.transform.position - TargetPosition));
        // }
    }

    /// <summary>
    /// Mini maptaki Mavi nokta oyuncunun nereye gitmek istediğini belirler
    /// </summary>
    public void SetTargetPosition(in Predicate<NpcController2> predicate)
    {
        // if (Indexer == null) return;
        // Indexer.gameObject.SetActive(true);
        if (indexedNpcController)
            indexedNpcController.HeadActive(false);

        if(MapAreaIndexer != null)
        MapAreaIndexer.SetActive(false);

        indexedNpcController = NpcDatabase.npcControllers.Find(predicate);
        if (indexedNpcController)
        {
            indexedNpcController.HeadActive(true);
            TargetPosition = indexedNpcController.transform.position;
        }
        else
        {
            Debug.Log("person you want to indicete is not exist");
        }
    }

    /// <summary>
    /// Mini maptaki Mavi nokta oyuncunun nereye gitmek istediğini belirler
    /// </summary>
    public void SetTargetPosition(Vector3 WorldPosition)
    {
        // if (Indexer == null) return;
        if (indexedNpcController)
            indexedNpcController.HeadActive(false);

        // player indexer
        // Indexer.gameObject.SetActive(true);

        // area indexer
        MapAreaIndexer.SetActive(true);
        MapAreaIndexer.transform.position = WorldPosition;

        TargetPosition = WorldPosition;
    }

    public void SetIndexerRing(bool value)
    { 
        // Indexer.gameObject.SetActive(value);
    }

}