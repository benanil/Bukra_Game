
using UnityEngine;
using UnityEngine.AI;
using AnilTools;

namespace Animal
{
    public class AnimalHealth : HealthBase
    {
        [SerializeField] private Inventory.PoolObject DropItem;
        [SerializeField] private Inventory.PoolObject Meat;
        [SerializeField] private Inventory.PoolObject RespawnName;
        [SerializeField] private SpawnController.SpawnPosition RespawnPosition;
        [SerializeField] private byte boneCount = 2;

        public override void Die()
        {
            GetComponent<Animator>().SetTrigger(Dead);
            Destroy(GetComponent<AnimalBase>());
            Destroy(GetComponent<NavMeshAgent>());
            PoolManager.instance.ReuseObject(DropItem, transform.position + Vector3.up, Quaternion.identity); // deri
            PoolManager.instance.ReuseObject(Meat, transform.position - transform.right, Quaternion.identity); // et
            for (byte i = 0; i < boneCount; i++)
            {
                PoolManager.instance.ReuseObject(Inventory.PoolObject.bone, transform.position + transform.right,Quaternion.identity);
            }

            this.Delay(20f, () =>
            {
                //StartCoroutine(SpawnController.instance.SpawnAnimalAsync(RespawnName, x => x.spawnPosition == RespawnPosition));
                Destroy(this);
            });          
        }
    }
}