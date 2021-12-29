using UnityEngine;

namespace Skill
{
    [CreateAssetMenu(menuName = "Language/Skill", fileName = "SkillTR")]
    public class SkillLanguage : ScriptableObject
    {
        public SkillName skillName;
        public string Name;
        public string Description;
        public string DamageMultipler;
    }
}