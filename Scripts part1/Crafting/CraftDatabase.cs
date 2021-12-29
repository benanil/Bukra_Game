
using AnilTools;
using System.Collections.Generic;
using UnityEngine;

namespace Crafting
{
    public class CraftDatabase : Singleton<CraftDatabase>
    {
        [SerializeField] private GameObject Prefab;
        public Craft[] AllCrafts;
        
        public Transform SlotPrefab;
        public Sprite plusPrefab;
        public Sprite EqualsPrefab;

        public List<CraftSlot> craftSlots;

        public void Start()
        {
            for (short i = 0; i < transform.childCount; i++){
                craftSlots.Add(transform.GetChild(i).GetComponent<CraftSlot>());
            }
        }

        public void UpdateCounts()
        {
            for (short c = 0; c < craftSlots.Count; c++){
                craftSlots[c].UpdateUI();
            }
        }
    }
}