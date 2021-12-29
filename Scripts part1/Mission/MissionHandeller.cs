using AnilTools;
using Inventory;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dialog
{
    public enum MainMisions
    {
        CollectMushrooms, FeedJacob , ImmediateWood , HelpSmith , ClearMine
    }

    public class MissionHandeller : Singleton<MissionHandeller>
    {
        private const string missionComplated = "Mission Complated";
        private const string missionFinished = " Mission finished";

        public List<Mission> MissionDatabase; // find metodu ile görevi bul
        
        public MainMisions CurrentMision;

        [System.Serializable]
        public struct Texts
        {
            public Text Kill_Item_Count; // sol üstte bulunur KillCount & ItemCount diye
            public Text CurrentMissionTXT;
            public Text RevenueExp;
            public Text MissionGoalTXT;
        }
        public Texts texts;

        public static bool OnMission = false;
        private string KillCountTxt = "Kill Count : ", ItemCountTxt = "Item Count : ";

        [SerializeField] XMLmanager Savegame;
        public Mission RealMission;

        // storydatabase
        public static short CurrentStage;
        [SerializeField]
        StoryDatabase storyDatabase;
        StoryMission storyMission;
        bool firstStageDefined;

        [SerializeField]
        private AudioClip WinAudioClip;
        
        public short KillCount;
        [SerializeField]
        private short ItemCount;
        
        private short CurrentLanguage => (short)GameMenu.instance.languages.currentLanguage;
        
        public void OnJsonLoaded()
        {
            // ana görevi ekranın sol üst köşesine yazar
            UpdateMission(CurrentMision);
            texts.CurrentMissionTXT.text = CurrentMision.ToString();

            MissionMenu.instance.ChangeMenu(RealMission.missionLanguages[CurrentLanguage].Description, RealMission);
            
            MiniMap.instance.SetTargetPosition(x => x.Name == RealMission.npc[0]);
        }

        public void UpdateMission(MainMisions Value)
        {
            CurrentMision = Value;
            RealMission = ReturnCurrentMission();
            UpdateUI();
        }

        public void AddItem()
        {
            ItemCount++;
            texts.Kill_Item_Count.text = ItemCountTxt + ItemCount.ToString();
            if (ItemCount == RealMission.RequiredItemCount){
                MissionMenu.instance.TikAt();
                Warning.instance.Warn(RealMission.missionLanguages[CurrentLanguage].TurnBackText, Language.Alright);
                if (RealMission.ReturningNpc != NpcName.none)
                MiniMap.instance.SetTargetPosition(x => x.Name == RealMission.npc[RealMission.npc.Length - 1]);
                else
                MiniMap.instance.SetTargetPosition(x => x.Name == RealMission.ReturningNpc);
            }
        }

        public void Addkill()
        {
            KillCount++;
            texts.Kill_Item_Count.text = KillCountTxt + KillCount.ToString();
            if (KillCount == RealMission.RequiredKillCount){
                MissionMenu.instance.TikAt();
                Warning.instance.Warn(RealMission.missionLanguages[CurrentLanguage].TurnBackText, Language.Alright);
                if (RealMission.ReturningNpc != NpcName.none)
                MiniMap.instance.SetTargetPosition(x => x.Name == RealMission.npc[RealMission.npc.Length-1]);
                else
                MiniMap.instance.SetTargetPosition(x => x.Name == RealMission.ReturningNpc);
            }
        }

        public void SetLanguage(string killcountMenu , string ItemCountMenu)
        {
            KillCountTxt = killcountMenu;
            this.ItemCountTxt = ItemCountMenu;
        }

        public void OnStartMission()
        {
            OnMission = true;
            MiniMap.instance.SetIndexerRing(true);

            if (RealMission.missionType == MissionType.ItemBringing){
                // eğer item getirme görevi ise yapılacaklar
            }
            else if (RealMission.missionType == MissionType.killing){
                SpawnController.instance.SpawnEnemies(RealMission.EnemySpawnPosition, RealMission.RequiredKillCount, RealMission.Enemy);
            }
            else if (RealMission.missionType == MissionType.story)
            {
                OnStartStory();
            }

            MissionMenu.instance.ChangeMenu(RealMission.missionLanguages[CurrentLanguage].Description, RealMission);
        }

        public void OnEndMission()
        {
            NotificationManager.SendNotificulation(missionComplated, CurrentMision.ToString() + missionFinished);

            GameManager.instance.audioSource.PlayOneShot(WinAudioClip);

            if (RealMission.missionType == MissionType.ItemBringing){
                for (int i = 0; i < RealMission.RequiredItemCount; i++){
                    CharacterInventory.instance.RemoveItem(RealMission.requiredItem.Id);
                }
            }

            string reciveStr = GameMenu.instance.languages.currentLanguage == 0 ? " Alındı" : "Recived";

            for (int i = 0; i < RealMission.RevenueItems.Length; i++){
                var item = RealMission.RevenueItems[i];
                CharacterInventory.instance.AddItem(item.Id);
                PopUpMenu.instance.PopUp(item.Title + reciveStr, Color.white);
            }

            ItemCount = 0;
            KillCount = 0;
            CurrentStage = 0;
            CurrentMision += 1;
            firstStageDefined = false;

            PlayerInfo.instance.AddExp(RealMission.RevenueExp);
            CanvasManager.ShowTxt(texts.RevenueExp, RealMission.RevenueExp.ToString(), Color.yellow);

            RealMission = ReturnCurrentMission();

            Debug2.Log("sonraki missiona geçtin");
            
            Savegame.SaveVeriables();

            MiniMap.instance.SetTargetPosition(x => x.Name == RealMission.npc[0]);

            MissionMenu.instance.ChangeMenu(RealMission.missionLanguages[CurrentLanguage].Description, RealMission);

            UpdateUI();
        }

        private NpcController2 CurrentNpc;

        internal void UpdateUI()
        {
            texts.Kill_Item_Count.text = string.Empty;
            texts.CurrentMissionTXT.text = CurrentMision.ToString();
            
            if (CurrentStage == 0 &&  !firstStageDefined){
                if (!RealMission)
                    return;

                firstStageDefined = true;
                
                texts.MissionGoalTXT.text = RealMission.CurrentLanguage.goals[0].ToString();

                if (RealMission.missionType.Equals(MissionType.story) && storyMission.Equals(null)){
                    storyMission = storyDatabase.GetCurrentstory((int)CurrentMision);
                    OnStartStory();
                }

                if (RealMission.missionType != MissionType.story){
                    // npc kafasındaki ok açılsın
                    if (CurrentNpc != null)
                        CurrentNpc.HeadActive(false);

                    NpcDatabase.npcControllers.Find(x => x.Name == RealMission.npc[0])?.HeadActive(true);
                }
            }
            
            if (CurrentStage > 0){
                texts.MissionGoalTXT.text = RealMission.missionLanguages[(short)GameMenu.instance.languages.currentLanguage].goals[CurrentStage];
            }
        }

        internal Mission ReturnCurrentMission()
        {
            return MissionDatabase[(int)CurrentMision];
        }

        // story

        internal void OnStartStory()
        {
            storyMission = storyDatabase.GetCurrentstory((int)CurrentMision);

            for (int i = 0; i < storyMission.stages.Length; i++){ 
                storyMission.stages[i].id = i;
            }
            storyMission.stages[0].OnEnabled();
            UpdateUI();
        }

        public void OnPlayerNextStage()
        {
            Debug2.Log("sonraki stageye geçtin güncellendi");

            if (CurrentStage == storyMission.stages.Length - 1){
                OnEndMission();
                return;
            }
            else{
                CurrentStage++;

                if (RealMission.missionType == MissionType.story){
                    storyMission.stages[CurrentStage].OnEnabled();
                }
                
                UpdateUI();
            }
        }
    }
}
