
using AnilTools;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Inventory;
using System;
using Random = UnityEngine.Random;
using System.Collections;
using Animal;
using Assets.Resources.Scripts.AI.Enemy;

public class SpawnController : Singleton<SpawnController>
{
    private readonly string BoarPath = "Boar";
    private readonly string RabitPath = "Rabit";

    public enum SpawnPosition
    { 
        village , village1 , valley, mountain , lakeside , ForestMine , mushroom , Hunt
    }

    public List<SpawnData> ObjectSpawnPositons;
    [Space]
    [SerializeField] private PoolObject[] EnemyTypes;

    private Vector3 spawnPosChace;
    private readonly BVector3 randomPos = new BVector3(RandomDistance, 2, RandomDistance);

    private const byte RandomDistance = 10;

    // mem alloc
    CanavarControl enemyCode;
    GameObject enemy;
    NavMeshAgent enemyNav;

    public readonly List<CanavarControl> canavars = new List<CanavarControl>();

    private void Start()
    {
        this.canavars.AddRange(FindObjectsOfType<CanavarControl>().ToList());
    }

    /// <summary>
    /// haritaya belirlenen yaratığı spawnlar
    /// </summary>
    /// <param name="position">bu pozisyon ve çevresine spawnlar</param>
    /// <param name="random">random bir yere mi yoksa belirlenen noktayamı</param>
    /// <param name="EnemyType">düşman tipi</param>
    public void SpawnEnemies(SpawnPosition position , byte enemyCount , PoolObject EnemyType, bool random = false)
    {
        Transform spawnMainPos = random ? ObjectSpawnPositons[Random.Range(0, ObjectSpawnPositons.Count)].Position 
                               : ObjectSpawnPositons.Find(x => x.spawnPosition == position).Position;

        PoolObject enemyType = random ? EnemyTypes[Random.Range(0, EnemyTypes.Length)] : EnemyType;
        int EnemyCount = random ? Random.Range(3, 5) : enemyCount;

        for (short i = 0; i < EnemyCount + 1; i++)
        {
            spawnPosChace = Mathmatic.FindSamplePos(spawnMainPos.position, randomPos);
            enemy = PoolManager.instance.ReuseObject(enemyType, Vector3.zero, Quaternion.identity);
            enemyCode = enemy.GetComponent<CanavarControl>();
            enemyNav = enemy.GetComponent<NavMeshAgent>();

            canavars.Add(enemyCode);

            enemyCode.enabled = false;

            if (!enemyNav.Warp(spawnPosChace)) { 
                Debug2.LogError("enemy spawn edilirken warp edilemedi");
                return;
            }

            enemyCode.enabled = true;
            
            spawnPosChace = Vector3.zero;
        }
    }

    /// <summary>
    /// Heyvan spawnlamak içindir
    /// </summary>
    // public IEnumerator SpawnAnimalAsync(PoolObject poolObject, Predicate<SpawnData> spawnData)
    // {
    //     var SpawnData = ObjectSpawnPositons.Find(spawnData);
    //     string loadedPath;
    //     
    //     var spawnPos = Mathmatic.FindSamplePos(SpawnData.Position.position , randomPos);
    //     
    //     switch (poolObject){
    //         case PoolObject.Rabit: loadedPath = RabitPath; break;
    //         case PoolObject.boar : loadedPath = BoarPath; break;
    //         default: Debug.Log("heyvan gir"); yield break;
    //     }
    //     
    //     var loadRequest = Addressables.InstantiateAsync(loadedPath);
    //     
    //     yield return loadRequest;
    //     
    //     if (loadRequest.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
    //     {
    //         var loadedAsset = loadRequest.Result;
    //     
    //         loadedAsset.transform.position = spawnPos;
    //         loadedAsset.GetComponent<AnimalBase>().enabled = true;
    //         loadedAsset.name = "dela çük";
    //         Debug2.Log("Heyvan loaded", Color.cyan);
    //     }
    //     else
    //     {
    //         Debug2.Log("Heyvan yüklenemedi", Color.red);
    //     }
    // 
    // }

    [Serializable]
    public struct SpawnData
    {
        public SpawnPosition spawnPosition;
        public Transform Position;
    }

}
