using AnilTools;
using Dialog;
using System.Collections.Generic;
using UnityEngine;

public enum Path2
{
	village1, village2, Riverside, mountainSide , MushroomSide
}

[SelectionBase]
public class NpcDatabase : Singleton<NpcDatabase>
{
    public static readonly List<NpcController2> npcControllers = new List<NpcController2>();
    public static readonly List<NpcController2> Patrols = new List<NpcController2>();

    public Transform[] Homes;
    public Transform[] DigPositions;
    public Transform[] TrainingPositions;
    public Transform[] AmelePositions;

    private void Start()
    {
        npcControllers.AddRange(FindObjectsOfType<NpcController2>());
        Patrols.AddRange(npcControllers.FindAll(x => x.npcType == NpcType.Patrol));
    }

    private void OnDisable()
    {
        npcControllers.Clear();
        Patrols.Clear();
    }

    private void OnApplicationQuit()
    {
        npcControllers.Clear();
        Patrols.Clear();
    }

    public static void Add(NpcController2 npc)
    {
        if (npc.npcType == NpcType.Patrol) Patrols.Add(npc);
        else                                 npcControllers.Add(npc);
    }

}