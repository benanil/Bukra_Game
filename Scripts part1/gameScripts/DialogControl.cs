using Player;
using System.Linq;
using Inventory;
using UnityEngine;
using AnilTools;
using UnityEngine.UI;
using Subtegral.DialogueSystem.DataContainers;
using UnityEngine.Events;
using System;

namespace Dialog
{
    public partial class DialogControl : Singleton<DialogControl>
    {
        #region Commands
        private const string SellCommand = "Sell";
        private const string BuyCommand = "Buy";
        private const string GiveCommand = "Give";
        private const string EventCommand = "Event";
        private const string KillingCommand = "Kill";
        private const string ItemBringCommand = "Item";
        private const string FireCommand = "Fire";
        private const string HereCommand = "Here";
        private const string FreeCommand = "Free";
        private const string TradeCommand = "Trade";
        private const string CamCommand = "Cam";
        #endregion

        [Header("Dialog", order = 1)]
        [SerializeField] private Text Dialog; // npclerle kunuşmak için panel yazı alanı
        [SerializeField] private GameObject dialogPaneli;
        [SerializeField] private GameObject PlayerUI;
        [SerializeField] private GameObject InventoryCamera;

        private int money;
        public int Money
        {
            get
            {
                return money;
            }
            private set
            {
                money = value;
                moneyText.text.Replace(moneyText.text, money.ToString()); 
            }
        }

        [SerializeField] private Text moneyText;
        [SerializeField] private Text uyarı;
        [Tooltip("warning or celebrate")]
        [SerializeField] private Text diyalogUyarı;
        [SerializeField] private Text talkingNpcName;

        [SerializeField] private CanvasGroup DialogGroup;
        [SerializeField] private CanvasGroup EquipmentGroup;
        [SerializeField] private CanvasGroup silahcıMenu;
        [SerializeField] private CanvasGroup TüccarMenu;
        [SerializeField] private CanvasGroup HealerMenu;
        [SerializeField] private CanvasGroup InventoryCanvasGroup;

        private CanvasGroup CurrentTradeMenu;

        //Dil ayarlama için
        AvibleLanguages CurrentLanguage = AvibleLanguages.English;

        // görev ile alakalı
        [NonSerialized]
        public bool isMission = false;
        
        [NonSerialized]
        public short Section = 0; // not section: bölüm her yeni konuşma dizisidir

        // patrol ile alakalı
        public static NpcController2 npcController;
        public static bool OnConversation;

        [SerializeField] private GameObject sword;

        [Header("DialogueParser")]

        [SerializeField] private Text dialogueText;
        [SerializeField] private Button choicePrefab;
        [SerializeField] private Transform buttonContainer;
        [SerializeField] private DialogueContainer[] NpcDefaults;

        private DialogueContainer dialogue; // chace

        private Button ExtraButton;

        [SerializeField]
        private EventAndDescription[] DialogueEvents;
        
        private bool updatebölüm = true;

        private static bool _inEquipment;

        public static bool GetinEquipment() => _inEquipment;

        public static void SetinEquipment(bool value)
        {
            _inEquipment = value;
            if (instance.InventoryCamera) 
                instance.InventoryCamera.SetActive(value);
        }

        [Serializable]
        public struct EventAndDescription{
#if UNITY_EDITOR
            public string description;
#endif
            public UnityEvent Event;
        }

        private void Start(){
            moneyText.text = Money.ToString();
        }

        public void StartDialogue()
        {       
            ExtraButton = null;
            
            if (isMission){
                if (CurrentLanguage == 0){//TR
                    dialogue = Getmission().konusmaQueue.Dequeue().TurkishSpeak;
                }
                else {
                    dialogue = Getmission().konusmaQueue.Dequeue().EnglishSpeak;
                }
            }
            else{
                if (npcController.npcType == NpcType.Patrol){
                    ExtraButton = Instantiate(choicePrefab, buttonContainer);
                    ExtraButton.onClick = new Button.ButtonClickedEvent();

                    if (npcController.isHered){
                        dialogue = GameMenu.instance.GetCurrentLanguage.PatrolFire;
                        ExtraButton.onClick.AddListener(FirePatrol);
                        ExtraButton.GetComponentInChildren<Text>().text = GameMenu.instance.GetCurrentLanguage.fire;
                    }
                    else {
                        dialogue = GameMenu.instance.GetCurrentLanguage.PatrolHere;
                        ExtraButton.onClick.AddListener(HerePatrol);
                        ExtraButton.GetComponentInChildren<Text>().text = GameMenu.instance.GetCurrentLanguage.here;
                    }
				}
                else if (npcController.npcType == NpcType.Villager){
                    dialogue = NpcDefaults[RandomReal.Range(0, NpcDefaults.Length)];
                }
                else {
                    dialogue = npcController.GetDialogueContainer();
                    ExtraButton = Instantiate(choicePrefab, buttonContainer);
                    ExtraButton.onClick.AddListener(EnterTrade);
                    ExtraButton.GetComponentInChildren<Text>().text = GameMenu.instance.GetCurrentLanguage.trade;
                }
            }
            talkingNpcName.text = npcController.Name.ToString().Replace('_',' ');
            var narrativeData = dialogue.NodeLinks.First(); //Entrypoint node
            ProceedToNarrative(narrativeData.TargetNodeGUID);
        }

        private void ProceedToNarrative(string narrativeDataGUID)
        {
            if (!dialogue)
                return;
            var text = dialogue.DialogueNodeData.Find(x => string.CompareOrdinal(x.NodeGUID, narrativeDataGUID) == 0).DialogueText;
            var choices = dialogue.NodeLinks.Where(x => string.CompareOrdinal(x.BaseNodeGUID, narrativeDataGUID) == 0);
            var buttons = buttonContainer.GetComponentsInChildren<Button>();
            
            StartCoroutine(dialogueText.TextLoadingAnim(text));

            int startNum = ExtraButton ? 1 : 0;
 
            for (int i = startNum; i < buttons.Length; i++){
                Destroy(buttons[i].gameObject);
            }

            foreach (var choice in choices){
                var button = Instantiate(choicePrefab, buttonContainer);
                button.GetComponentInChildren<Text>().text = choice.PortName;
                button.onClick.AddListener(() => ProceedToNarrative(choice.TargetNodeGUID));
            }

            var choiceList = choices.ToList();

            // konuşma sonu
            if (choiceList.Count == 0){

                if (byte.TryParse(text.Substring(text.Length - 2), out byte lastTwo))
                {
                    if (text.StartsWith(SellCommand)){
                        SellItem(lastTwo);
                    }
                    else if (text.StartsWith(GiveCommand)||text.StartsWith(FreeCommand)){ 
                        ReciveItem(lastTwo);
                    }
                    else if (text.StartsWith(EventCommand)){
                        DialogueEvents[lastTwo].Event.Invoke();
                    }
                }
                else if (text.StartsWith(HereCommand)){
                    HerePatrol();
                }
                else if (text.StartsWith(FireCommand)) {
                    FirePatrol();
                }
                else if (text.StartsWith(TradeCommand)){
                    EnterTrade();
                    return;
                }
                Debug.Log(text);
                OnDialogFinish(); 
            }
        }

        private void OnDialogFinish()
        {
            Vector3 foundedPos = default;

            byte lastTwo = 0;

            // exposed property
            foreach (ExposedProperty property in dialogue.ExposedProperties){

                if (byte.TryParse(property.PropertyValue.Substring(property.PropertyValue.Length-2), out lastTwo))
                {
                    if (property.PropertyValue.StartsWith(SellCommand)){
                        SellItem(lastTwo);
                    }
                    else if (property.PropertyValue.StartsWith(BuyCommand)){
                        BuyItem(lastTwo);
                    }
                    else if (property.PropertyValue.StartsWith(FreeCommand)
                           ||property.PropertyValue.StartsWith(GiveCommand)){
                        ReciveItem(lastTwo);
                    }
                    else if (property.PropertyValue.StartsWith(EventCommand)){
                        DialogueEvents[lastTwo].Event.Invoke();
                    }
                    else if (property.PropertyValue.StartsWith(CamCommand)){
                        CameraEventController.instance.Play(lastTwo);
                    }
                    Debug.Log(property.PropertyName);
                }
                if (property.PropertyValue.StartsWith(ItemBringCommand) || property.PropertyValue.StartsWith(KillingCommand))
                {
                    foundedPos = SpawnController.instance.ObjectSpawnPositons.Find(x =>
                                                                           x.spawnPosition == Getmission().EnemySpawnPosition).Position.position;
                }
            }

            if (isMission){

                if (Getmission().konusmaQueue.Count == 0){ // Mission Finish
                    FinishMission();
                }
                else if (foundedPos == default){
                    Debug.Log("Bulunan Pos yok sonraki npc işaretleniyor");
                    MiniMap.instance.SetTargetPosition(x => x.Name == Getmission().npc[Section + 1]);
                }
                else {
                    Debug.Log("Bulunan Pos işaretleniyor");
                    MiniMap.instance.SetTargetPosition(foundedPos);
                }

                if (Getmission().hasTicks[Section] == true){
                    MissionMenu.instance.TikAt();
                }
            }
            OnExitDialog();
        }

        /// <summary>
        /// menüyü kapatır
        /// </summary>
        public void OnExitDialog()
        {
            // Anims
            sword.SetActive(true);
            CharacterMove.instance.OnChangeDialogAnim();
            npcController.OnDialogAnimation();

            // paneller kapatılır
            dialogPaneli.SetActive(false);
            GameMenu.instance.canvases.BaseCanvas.SetVisuality(true);
            StopAllCoroutines();
            GameMenu.SetCanvasGroupFade( DialogGroup, false);
            DialogGroup.interactable = false;
            DialogGroup.blocksRaycasts = false;
            PlayerUI.SetActive(true);
            EquipmentGroup.SetVisible(true);
            // konusma ile ilgili
            Dialog.text = string.Empty;
            OnConversation = false;
            
            // görev ile ilgili

            if (isMission){
                isMission = false;
                if (updatebölüm){
                    Section++;
                }
                else{
                    updatebölüm = true;
                }
                MissionHandeller.instance.UpdateUI();
            }
        }

        // --- mission ---
        public void FinishMission()
        {
            // görevi bitirme kodlarını gir
            MiniMap.instance.SetIndexerRing(false);
            CharacterMove.instance.OnChangeDialogAnim();

            MissionHandeller.OnMission = false; // mission handeller on Missionu deaktif eder
            MissionHandeller.instance.OnEndMission();
            CanvasManager.ShowTxt(diyalogUyarı, words.MissionComplated, Color.green);

            Section = 0;
            isMission = false;

            OnExitDialog();
        }

        private Mission Getmission(){
            return MissionHandeller.instance.ReturnCurrentMission();
        }
        // --- Konuşma ---
        public void Konus()
        {
            // Anims
            sword.SetActive(false);
            CharacterMove.instance.OnChangeDialogAnim();
            npcController.OnDialogAnimation();
            dialogPaneli.SetActive(true);
            DialogGroup.SetVisible(true);
            PlayerUI.SetActive(false);
            OnConversation = true;

            // görev içindeyse bool olustur
            if (!MissionHandeller.OnMission){
                if (Getmission().npc[Section] == npcController.Name) {
                    isMission = true;
                    MissionHandeller.instance.OnStartMission(); // missionHandeller on missionu aktif eder
                    MissionMenu.instance.TikAt();               // mission menü üzerinde görev sahibi ile konuşulduğuna dair tik atar
                    Debug2.Log("görev başladı" + Getmission().ToString());
                }
            }
            else if (Getmission().npc[Section] == npcController.Name){
                isMission = true;
                //buranın amacı missionu bitirmeye çalışıp yeteri kadar kile yada eşyaya sahip değilse uyarı vermeye yarar
                if (Getmission().missionType == MissionType.ItemBringing)
                {
                    var itemCount = CharacterInventory.GetCount(Getmission().requiredItem);

                    if (itemCount <= Getmission().RequiredItemCount - 1 && Getmission().konusmaQueue.Count == 0){
                        CanvasManager.ShowTxt(diyalogUyarı, words.ItemWarning, Color.red);
                        updatebölüm = false;
                        return;
                    }
                }
                else if (Getmission().missionType == MissionType.killing)
                {
                    if (MissionHandeller.instance.KillCount <= Getmission().RequiredKillCount - 1 && Getmission().konusmaQueue.Count == 0){ 
                        CanvasManager.ShowTxt(diyalogUyarı, words.killingWarning, Color.red);
                        updatebölüm = false;
                        return;
                    }
                }
            }
            else{
                isMission = false;
            }
            
            StartDialogue();
        }
        
        // --- Patrol ---
        public void HerePatrol()
        {
            PopUpMenu.instance.PopUp(Language.CurrentLanguage().PatrolHered, Color.white);
            npcController.HerePatrol();
            Money -= 200;
            OnExitDialog();
        }

        public void FirePatrol()
        {
            PopUpMenu.instance.PopUp(Language.CurrentLanguage().PatrolFired, Color.white);
            npcController.FirePatrol();
            OnExitDialog();
        }

        // trigger
        public void TriggerEnter(NpcController2 npcController)
        {
            DialogControl.npcController = npcController;
            Konus();
        }

        // --- Language ---
        public void ChangeLanguage(DialogWords words, AvibleLanguages language)
        {
            this.words = words;
            moneyText.text = Money.ToString();
            CurrentLanguage = language;
        }
        [Serializable]
        public struct DialogWords
        {
            public static DialogWords Default = new DialogWords()
            {
                alındı = "Taken",
                satıldı = "Selled", 
                yetersiz = "NotEnough",
                ItemWarning = "Items NotEnough",
                killingWarning = "Kills Are Not Enough",
                MissionComplated = "Mission Complated"
            };

            public string alındı;
            public string satıldı;
            public string yetersiz;
            public string ItemWarning;
            public string killingWarning;
            public string MissionComplated;
        }

        private DialogWords words = DialogWords.Default;

    }
}