using AnilTools;
using Rock;
using UnityEngine;

namespace Skill
{
    public class SkillBox : AnilButton
    {

        #region Singleton
        private static SkillBox _instance;
        public static SkillBox instance
        {
            get
            {
                return _instance;
            }
        }
        #endregion

        public static Skill SellectedSkill;

        public static bool HasSkill => SellectedSkill != null;

        private void Start()
        {
            _instance = this;
        }

        private const float Anchor = 75f;

        private void Update()
        {
            if (Sellected == true)
            {
                transform.position = Input.mousePosition + SVector3.up * Anchor + SVector3.Left * Anchor;
                if (Input.GetMouseButtonUp(0))
                {
                    Desellect();
                }
            }
        }

        public override void Sellect()
        {
            Image.sprite = SkillMenuHandeller.CurrentSkill.icon;
            Image.color = Color.white;
            Sellected = true;
        }

        public override void Desellect()
        {
            Image.sprite = null;
            Sellected = false;
            Image.color = Color.clear;
            RockHandeller.instance.UpdateUI();
            SellectedSkill = null;
        }
    }
}