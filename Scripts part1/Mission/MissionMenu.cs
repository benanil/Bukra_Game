using AnilTools;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dialog
{
    public class MissionMenu : Singleton<MissionMenu>,UILanguage
    {
        #region statics
        private readonly string CoinPath = "graphical/Sprites/Menu Sprites/Coin";
        private readonly string ExpPath = "graphical/Sprites/Menu Sprites/Exp Deneme";
        #endregion

        // goal
        [Header("Goal")]
        [SerializeField] private GameObject GoalPref;
        [SerializeField] private Transform goalParent;
        // revenue
        [Header("Revenue")]
        [SerializeField] private GameObject RevenuePref;
        [SerializeField] private Transform RevenueParent;
        [SerializeField] private Text RevenueHeader;
        [SerializeField] private Text RevenueMoney;
        [SerializeField] private Text RevenueExp;
        //other
        [Header("Other")]
        [SerializeField] private Image MissionImage;
        [SerializeField] private Text MissionHeader;
        [SerializeField] private Image MissionOnMapImage;
        [SerializeField] private Text DescriptionText;

        // tek tek tik atılacak sırayla
        private readonly Queue<MissionGoalSlot> GoalCodes = new Queue<MissionGoalSlot>();
        private readonly List<GameObject> GoalObjs = new List<GameObject>();

        private readonly List<RevenueInMenu> revenueCodes = new List<RevenueInMenu>();
        private readonly List<GameObject> RevenueObjs = new List<GameObject>();

        private readonly List<short> trashIds = new List<short>();

        private short CurrentLanguage => (short)GameMenu.instance.languages.currentLanguage;
        private Mission mission;
        private MissionLanguage missionLanguage => mission.missionLanguages[CurrentLanguage];
        
        /// <summary>
        /// mission menusunu doldurmaya yarar
        /// </summary>
        internal void ChangeMenu(string description, Mission mission)
        {
            Clear();

            this.mission = mission;
            MissionImage.sprite = mission.LevelIMG;
            MissionOnMapImage.sprite = mission.LevelOnMapIMG;
            DescriptionText.text = description;

            MissionHeader.text = missionLanguage.MissionName;

            for (int i = 0; i < missionLanguage.goals.Length; i++)
            {
                var goalObj = Instantiate(GoalPref,goalParent);
                var GoalCode = goalObj.GetComponent<MissionGoalSlot>();
                goalObj.transform.SetParent(goalParent);
                GoalCode.Initialize(missionLanguage.goals[i]);
                GoalCodes.Enqueue(GoalCode);
                GoalObjs.Add(goalObj);
            }

            // revenue
            for (int i = 0; i < mission.RevenueItems.Length; i++)
            {
                var RevenueObj = Instantiate(RevenuePref, RevenueParent);
                var revenueCode = RevenueObj.GetComponent<RevenueInMenu>();
                RevenueObj.transform.SetParent(RevenueParent);
                revenueCodes.Add(revenueCode);
                RevenueObjs.Add(RevenueObj);
                
                int x = 0;
                if (!trashIds.Contains(mission.RevenueItems[i].Id))
                {
                    for (int j = 0; j < mission.RevenueItems.Length; j++)
                        if (mission.RevenueItems[j].Id == mission.RevenueItems[i].Id)
                            x++;
                    
                    revenueCode.Initialize(mission.RevenueItems[i], x.ToString() + Language.X + mission.RevenueItems[i].Title);
                    trashIds.Add(mission.RevenueItems[i].Id);
                }
            }

            AddCoinAndEXP(mission.RevenueExp, mission.RevenueMoney);
            
            trashIds.Clear();
        }

        private void AddRevenue(Sprite sprite,string value)
        {
            var RevenueObj = Instantiate(RevenuePref, RevenueParent);
            var revenueCode = RevenueObj.GetComponent<RevenueInMenu>();
            RevenueObj.transform.SetParent(RevenueParent);
            revenueCodes.Add(revenueCode);
            RevenueObjs.Add(RevenueObj);
            revenueCode.Initialize(sprite, value);
        }

        private void AddCoinAndEXP(int exp,int money)
        {
            AddRevenue(Resources.Load<Sprite>(CoinPath), money.ToString());
            AddRevenue(Resources.Load<Sprite>(ExpPath), exp.ToString());
        }

        public void TikAt()
        {
            var goalLine = GoalCodes.Dequeue();
            goalLine.tik.color = Color.green;
            goalLine.GoalText.fontStyle = FontStyle.Bold;

            // mission goalini güncelleme
            MissionHandeller.instance.texts.MissionGoalTXT.text = goalLine.GoalText.text;
        }

        private void Clear()
        {
            GoalCodes.Clear();
            revenueCodes.Clear();
            for (int i = 0; i < GoalObjs.Count; i++){
                Destroy(GoalObjs[i]);
            }
            for (int i = 0; i < RevenueObjs.Count; i++){
                Destroy(RevenueObjs[i]);
            }
        }

        public void UpdateLanguage(int id)
        {
            RevenueHeader.text = GameMenu.instance.GetCurrentLanguage.Revenue;
        }
    }
}