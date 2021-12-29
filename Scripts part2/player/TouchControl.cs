using AnilTools;
using Dialog;
using Inventory;
using UnityEngine;
using Serializable = System.SerializableAttribute;
using static AnilTools.Mathmatic;
using static GameConstants;

namespace Player
{
    public class TouchControl : MonoBehaviour
    {
        [Serializable]
        private struct Sesler
        {
            [Header("Sesler", order = 1)]
            public AudioClip pick;
            public AudioClip CoinSound;
        }
        [SerializeField] Sesler sesler;

        [Serializable]
        private struct Parçacıklar
        {
            [Header("parçacıklar", order = 1)]
            public GameObject rockPart;
            public GameObject bodyPart;
            public GameObject woodPart;
        }

        [SerializeField] Parçacıklar parçacıklar; 

        [Space]
        [SerializeField]
        private Camera TpsCam;
        [SerializeField]
        private LayerMask layerMask;

        Vector3 contact;
        Quaternion rot;
        Transform sellection;

       // private MissionHandeller MissionHandeller;

        //depolanan veriler
        itemInfo _itemInfo;

        Sprite CoinSprite;

        private void Start()
        {
            CoinSprite = Resources.Load<Sprite>("graphical/Sprites/Menu Sprites/Coin") as Sprite;
        }

        private void Update()
        {
            if (GameMenu.PlayerOnMenu) return;
        
            if (InputPogo.MouseDown){
                if (RaycastFromCamera(out RaycastHit hit,150,layerMask)){
                    sellection = hit.transform;
                    contact = hit.point;
                    rot = Quaternion.FromToRotation(SVector3.up, hit.normal);
                    
                    if (sellection.CompareTag(Tags.log))// log = pickable
                    {
                        _itemInfo = sellection.GetComponent<itemInfo>();
                        
                        if (_itemInfo.pickitem is CoinItem coinItem){
                            _itemInfo.AddCoin(coinItem.Coin);
                            GameManager.instance.audioSource.PlayOneShot(sesler.CoinSound);
                            PopUpMenu.instance.PopUp(coinItem.Coin.ToString() + Language.C, Color.yellow, CoinSprite);
                        }
                        else if (UIInventory.instance.SlotSayısı > UIInventory.instance.FullSlots -1){// fix it
                            GameManager.instance.audioSource.PlayOneShot(sesler.pick);
                            _itemInfo.Pick();
                            PopUpMenu.instance.PopUp(((ItemSC)_itemInfo.pickitem).Title,Color.white);
                            Instantiate(parçacıklar.rockPart, contact, rot);
                        }
                        else{
                            Warning.instance.Warn(Language.InventoryWarning, Language.Alright);
                        }
                        // eger görevde ise
                        if (MissionHandeller.OnMission){
                            Dialog.MissionType missionType = MissionHandeller.instance.RealMission.missionType;

                            if (missionType == Dialog.MissionType.killing || missionType == Dialog.MissionType.story)
                                return;

                            // item uyuşuyorsa ekle
                            if (MissionHandeller.instance.RealMission.requiredItem.Id == _itemInfo.id){
                                MissionHandeller.instance.AddItem();
                            }
                        }
                        
                    }
                    else if (sellection.CompareTag(Tags.trigger)){
                        sellection.GetComponent<Trigger>().Speak();
                    }
                    else if (sellection.TryGetComponent(out PickableHealth healthPick))
                    {
                        CharacterHealth.instance.AddHealth(healthPick.Health);
                    }
                }
            }
        }
    }
}