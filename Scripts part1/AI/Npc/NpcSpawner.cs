using AnilTools;
using UnityEngine;
using UnityEngine.AI;
using UrFairy;

namespace Assets.Resources.Scripts.AI.Npc
{
    public class NpcSpawner : MonoBehaviour
    {
        public Transform[] SpawnPositions;
        public GameObject[] npcs;

        private void Start()
        {
            StartSpawning();
        }

        [ContextMenu("try")]
        public void Spawn(){
            var pos = Mathmatic.FindSamplePos(SpawnPositions.GetRandom().position, (Vector3.one * 10).Y(2)) + Vector3.one * 1.5f;
            GameObject loadedObj = Instantiate(npcs.GetRandom(), pos,Quaternion.identity);
        
            loadedObj.GetComponent<NavMeshAgent>().Warp(pos);
            NpcDatabase.Add(loadedObj.GetComponent<NpcController2>());
        }

        private void StartSpawning()
        {
            for (short i = 0; i < npcs.Length; i++)
            {
                Vector3 pos = Mathmatic.FindSamplePos(SpawnPositions.GetRandom().position, (Vector3.one * 10).Y(2)) + Vector3.one * 1.5f;
                GameObject LoadedObj = Instantiate(npcs.GetRandom(), pos, Quaternion.identity);

                LoadedObj.GetComponent<NavMeshAgent>().Warp(pos);
                NpcDatabase.Add(LoadedObj.GetComponent<NpcController2>());
            }
        }
    }
}