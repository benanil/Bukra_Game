using AnilTools.Save;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Skill
{
    public class SkillMenuHandeller : MonoBehaviour
    {
        #region singleton
        private static SkillMenuHandeller _instance;
        public static SkillMenuHandeller instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<SkillMenuHandeller>();
                }
                return _instance;
            }
        }
        #endregion

        public const string SkillPath = "Skill/";
        public const string _SkillName = "Skills.json";

        public static Skill CurrentSkill;
        public VideoPlayer videoPlayer;

        public SkillYuva skillYuva;
        public SkillYuva skillYuva1;
        public SkillYuva skillYuva2;

        [Header("Texts")]
        [SerializeField] private Text SkillName;
        [SerializeField] private Text DescriptionTxt;
        [SerializeField] private Text DamageMultiplerTXT;
                         public static string DamageMultipler;

        /// <summary>
        /// game menu üzerinden aç kapa
        /// </summary>
        public bool InSkillMenu = false; // false yap

        public void Load()
        {
            SkillYuvaSave loadData = JsonManager.Load<SkillYuvaSave>(SkillPath, _SkillName);
            skillYuva.Load(loadData.Yuva1);
            skillYuva1.Load(loadData.Yuva2);
            skillYuva2.Load(loadData.Yuva3);
        }

        //. xml save yap skilleri
        
        public void Save()
        {
            JsonManager.Save(new SkillYuvaSave(skillYuva.Save(), skillYuva1.Save(), skillYuva2.Save()), SkillPath,_SkillName);
        }

        public void UpdateSkillMenu(Skill skill)
        {
            if (!skill){
                Debug2.Log("SkillYok");
            }
            
            CurrentSkill = skill;
            videoPlayer.clip = skill.videoClip;
            DescriptionTxt.text = skill.Description;
            DamageMultiplerTXT.text = $"{DamageMultipler} {skill.damageMultipler}";
            SkillName.text = skill.name;
        }

        public void UpdateLanguage()
        {
            // attack multipler vs
            DamageMultipler = GameMenu.instance.GetCurrentLanguage.SkillMenuLang.DamageMultipler;
        }
    }

    [Serializable]
    public readonly struct SkillYuvaSave
    {
        public short Yuva1 { get; }
        public short Yuva2 { get; }
        public short Yuva3 { get; }

        public SkillYuvaSave(short yuva1, short yuva2, short yuva3)
        {
            Yuva1 = yuva1;
            Yuva2 = yuva2;
            Yuva3 = yuva3;
        }
    }

}