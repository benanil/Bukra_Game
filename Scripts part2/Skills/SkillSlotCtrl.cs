using AnilTools;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Skill
{
    public class SkillSlotCtrl : Singleton<SkillSlotCtrl>, UILanguage
    {
        public Skill[] AllSkills;
        public List<SkillSlot> skillSlots = new List<SkillSlot>();

        [SerializeField] private GameObject SkillPrefab;
        [SerializeField] private Transform SlotsParent;
        
        [SerializeField] private Text DamageMultiplerTXT;
                         public static string DamageMultipler;
        public Text description;

        public void Start()
        {
            for (short i = 0; i < AllSkills.Length; i++)
            {
                var slt = Instantiate(SkillPrefab);
                slt.transform.SetParent(SlotsParent);
                slt.transform.localScale = Vector3.one;
                var sklSlot = slt.GetComponent<SkillSlot>();
                sklSlot.Set(AllSkills[i]);
                skillSlots.Add(sklSlot);
            }
            
            AllSkills.GroupBy(x => (short)x.skillName);
        }

        public SkillSlot ReturnSlot(SkillName skillName)
        {
            return skillSlots.Find(x => x.Skill.skillName == skillName);
        }

        public Skill ReturnSkill(SkillName skillName)
        {
            for (int i = 0; i < AllSkills.Length; i++)
            {
                if (AllSkills[i].skillName == skillName)
                {
                    return AllSkills[i];
                }
            }
            return default;
        }

        public void UpdateLanguage(int id)
        {
            var skillLanguages = GameMenu.instance.languages.languages[id].skillLanguages;
            var skillMenuLanguages = GameMenu.instance.languages.languages[id].SkillMenuLang;
            
            DamageMultipler =  skillMenuLanguages.DamageMultipler;

            DamageMultiplerTXT.text = DamageMultipler + skillMenuLanguages.DamageMultipler;

            if (skillLanguages.Length != AllSkills.Length)
            {
                Debug2.Log("skill dil dosyası uyusmuyor", Color.red);
                return;
            }

            skillLanguages.GroupBy(x => (short)x.skillName);

            for (short i = 0; i < AllSkills.Length; i++)
            {
                AllSkills[i].SetLanguage(skillLanguages[i]);
                skillSlots[i].ChangeLanguage(skillLanguages[i]);
            }
        }
    }
}