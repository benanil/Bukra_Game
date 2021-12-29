using Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Skill
{
    public class SkillButton : AnilButton, IPointerClickHandler
    {   
        public Skill skill;

        public void SetSkill(Skill skill){
            this.skill = skill;
            Image.sprite = skill.icon;
            Sellected = true;
            SetActive(true);
        }

        public void RemoveSkill(){
            Image.sprite = null;
            skill = null;
            Sellected = false;
            SetActive(false);
        }

        internal void SetActive(bool value)
        {
            if (skill){
                Image.enabled = value;
                Image.color = value ? Color.white : Color.clear;
            }
            else{
                Image.enabled = false;
            }
        }

        public void OnPointerClick(PointerEventData eventData){
            if (skill){
                CombatControl.instance.Combo(skill.animationClip);
                SetActive(false);
            }
        }
    }
}