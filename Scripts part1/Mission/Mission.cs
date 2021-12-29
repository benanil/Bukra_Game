using UnityEngine;
using Inventory;
using System.Collections.Generic;

namespace Dialog
{
    [CreateAssetMenu(fileName = "Mission", menuName = "MissionData")]
    public class Mission : ScriptableObject
    {
        [Header("Görevin Adı",order=1)]
        public MainMisions mission;
        public MissionType missionType;

        public NpcName[] npc;
        [Tooltip("npc sayısı ile aynı olmalıdır, bu kişiyle konuştuktan sonra mission menüsüne tik atılıp atılmayacağını ayarlar")]
        public bool[] hasTicks;
        [Tooltip("item toplandıktan sonra dönülecek kişi")]
        public NpcName ReturningNpc = NpcName.none;
        [SerializeField]
        private SpeakingData[] konusma;

        private Queue<SpeakingData> _konusmaQueue;
        public Queue<SpeakingData> konusmaQueue
        {
            get{
                if (_konusmaQueue == null)
                {
                    _konusmaQueue = new Queue<SpeakingData>(konusma);
                }
                return _konusmaQueue;
            }
        }

        public ItemSC requiredItem;
        
        public byte RequiredItemCount;

        public PoolObject Enemy;
        
        public byte RequiredKillCount;

        public SpawnController.SpawnPosition EnemySpawnPosition;

        // para ve item ekle **
        public int RevenueExp;
        public int RevenueMoney;
        public ItemSC[] RevenueItems;

        public Sprite LevelIMG;
        public Sprite LevelOnMapIMG;

        public MissionLanguage[] missionLanguages;
        public MissionLanguage CurrentLanguage => missionLanguages[(byte)GameMenu.instance.languages.currentLanguage];

#if UNITY_EDITOR
        public bool ItemBringing => missionType == Dialog.MissionType.ItemBringing;
        public bool Killing => missionType == Dialog.MissionType.killing;
#endif
    }
}

public enum MissionType
{
    killing, ItemBringing, story
}