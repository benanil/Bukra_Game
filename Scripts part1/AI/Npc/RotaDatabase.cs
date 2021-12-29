using AnilTools;
using MiddleGames;
using System;
using UnityEngine;

public class RotaDatabase : Singleton<RotaDatabase>
{
    public Transform[] AllPoints;
    [Tooltip("for willage 2")]
    public Transform[] AllPoints1;

    [SerializeField] private WillageCenter[] WillageCenters;


    public static Transform GetWaypoint(in int willage, in int index) => instance._GetWaypoint(willage, index);
    private Transform _GetWaypoint(in int willage, in int index)
    {
        if (willage == 0) return AllPoints[index];
        if (willage == 1) return AllPoints1[index];
        throw new Exception("ebeen nikahı burda 2 koy var");
    }

    public static Vector3 GetWaypointPos(in int willage, in int index) => instance._GetWaypointPos(willage, index);
    private Vector3 _GetWaypointPos(in int willage, in int index)
    {
        if (willage == 0) return AllPoints[index].position;
        if (willage == 1) return AllPoints1[index].position;
        throw new Exception("ebeen nikahı burda 2 koy var");
    }
    public static int GetWillageLength(in int willageIndex) => instance._GetWillageLength(willageIndex);
    private int _GetWillageLength(int willageIndex)
    {
        return willageIndex == 0 ? AllPoints.Length : AllPoints1.Length;
    }

    public static Vector3 ChoseRandPosNearToVillage(in int willageIndex)
    {
        return instance._ChoseRandPosNearToVillage(willageIndex);
    }

    private Vector3 _ChoseRandPosNearToVillage(in int willageIndex)
    {
        Vector3 result = default;

        while (result == default)
        {
            Vector3 randPos = _GetWaypointPos(willageIndex, RandomReal.Range(0, _GetWillageLength(willageIndex) - 1));
            if (WillageCenters[willageIndex].bounds.Contains(randPos))
            {
                result = randPos;
            }
        }
        return result;
    }

    private void OnDrawGizmos() {}
}

