using AnilTools;
using Assets.Resources.Scripts.AI.Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Inventory
{
    public enum PoolObject
    {
          Treelog, FirstArmor, StoneSword, Mushroom, mushroom, Axe, none, TwoSword, iron
        , stone, ekmek, elma, skeletonC, skeletonCDead, WoodPart, RockPart, RedPotion
        , Amele_Org, Org_Gigant, pickAxe, defanceRock, AttackRock, IronArmor, arrow, bow, RabitSkin, BoarSkin
        , CoinPouch, PopUp, PigMeat, RabitMeat, bone, BackPack, Rabit, boar, skeltonHıtParticle, skeltonHıtParticle1
    }

    public class MyPoolManager: Singleton<MyPoolManager>
    {
        [SerializeField] private List<Pool> Enemys;
        [SerializeField] private List<Pool> Items;

        private readonly Dictionary<PoolObject, Queue<GameObject>> EnemyDictionary = new Dictionary<PoolObject,Queue<GameObject>>();
        private readonly Dictionary<string, Queue<GameObject>> ItemDictionary = new Dictionary<string, Queue<GameObject>>();

        private void Start()
        {
            for (short i = 0; i < Enemys.Count; i++)
            {
                var objectPool = new Queue<GameObject>();

                for (short x = 0; x < Enemys[i].Size; x++)
                {
                    var obj = Instantiate(Enemys[i].prefab);
                    objectPool.Enqueue(obj);
                    obj.transform.SetParent(transform);
                    obj.SetActive(false);

                    if (Enemys[i].poolObject == PoolObject.skeletonC)
                    {
                        obj.GetComponent<CanavarControl>().enabled = false;
                        obj.GetComponent<NavMeshAgent>().enabled = false;
                    }
                    
                }
                EnemyDictionary.Add(Enemys[i].poolObject, objectPool);
            }

            for (short i = 0; i < Items.Count; i++)
            {
                var ıtemPool = new Queue<GameObject>();

                for (short x = 0; x < Items[i].Size; x++)
                {
                    GameObject obj = Instantiate(Items[i].prefab);
                    ıtemPool.Enqueue(obj);
                    obj.transform.SetParent(transform);
                    obj.SetActive(false);
                }

                ItemDictionary.Add(Items[i].poolObject.ToString(), ıtemPool);
            }

        }

        public GameObject SpawnFromPool(PoolObject name, Vector3 position, Quaternion rotation)
        {
            if (!EnemyDictionary.ContainsKey(name))
            {
                Debug.Log("tag doesnt exist");
                return null;
            }

            var objectToSpawn = EnemyDictionary[name].Dequeue();

            bool IsDestroy = SearchDestroy(name);

            objectToSpawn.SetActive(true);
            objectToSpawn.transform.SetPositionAndRotation(position, rotation);

            if (IsDestroy) { StartCoroutine(RePool(objectToSpawn)); }  // 10 sn sonra RePool yapar

            EnemyDictionary[name].Enqueue(objectToSpawn);
            return objectToSpawn;
        }

        public GameObject SpawnFromPool(string name, Vector3 position, Quaternion rotation)
        {
            if (!ItemDictionary.ContainsKey(name))
            {
                Debug.Log("tag doesnt exist");
                return null;
            }

            var objectToSpawn = ItemDictionary[name].Dequeue();

            objectToSpawn.SetActive(true);
            objectToSpawn.transform.SetPositionAndRotation(position, rotation);

            ItemDictionary[name].Enqueue(objectToSpawn);
            return objectToSpawn;
        }

        private bool SearchDestroy(PoolObject obj)
        {
            //objenin bir süre sonra yok edilip edilmeyeceğini kontrol eder
            return obj == PoolObject.skeletonCDead;
        }

        private IEnumerator RePool(GameObject DestroyObj)
        {
            yield return new WaitForSecondsRealtime(10);
            
            DestroyObj.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

            try
            {
                DestroyObj.GetComponent<CanavarControl>().enabled = false;
                DestroyObj.GetComponent<NavMeshAgent>().enabled = false;
                DestroyObj.SetActive(false);
            }catch{}
        }
    }

    [System.Serializable]
    public struct Pool
    {
        public PoolObject poolObject;
        public GameObject prefab;
        public short Size;
    }

}
