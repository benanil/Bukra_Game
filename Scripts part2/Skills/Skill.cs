using UnityEngine;
using UnityEngine.Video;

namespace Skill
{
    [CreateAssetMenu(fileName = "Skils", menuName = "Skil")]
    public class Skill : ScriptableObject
    {
        public short id 
        {
            get
            {
                return (short)skillName;
            }
        }
        public SkillName skillName;
        public string Name;
        public float SkillTime;
        public int damageMultipler;
        public int RequiredLevel;
        public Sprite icon;
        public VideoClip videoClip;
        public AnimationClip animationClip;
        [Multiline]
        public string Description;

        public void SetLanguage(SkillLanguage skillLanguage)
        {
            Description = skillLanguage.Description;
            name = skillLanguage.name;
        }

    }

    public enum SkillName
    { 
        None, RapidSword , JumpAttack 
    }

}