using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Skill
{
    public class SkillYuva : AnilButton, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler,IPointerExitHandler, ISaveLoad<short>
    {
        public Skill skill;
        public Image SkillImg;

        [SerializeField] private SkillButton skillButton;

        public bool HasSkill => skill != null;

        private void Update(){
            if (Sellected){
                if (Input.GetMouseButtonUp(0)){
                    //Debug.Log("yuva pointer up ");
                    Sellect();
                }
            }
        }

        public override void Sellect(){
            if (SkillBox.HasSkill ){
                if (!HasSkill){
                    if (SkillMenuHandeller.instance.skillYuva.Save()  != SkillBox.SellectedSkill.id &&
                        SkillMenuHandeller.instance.skillYuva1.Save() != SkillBox.SellectedSkill.id &&
                        SkillMenuHandeller.instance.skillYuva2.Save() != SkillBox.SellectedSkill.id)
                    {
                        SkillImg.sprite = SkillBox.SellectedSkill.icon;
                        skill = SkillBox.SellectedSkill;
                        skillButton.SetSkill(skill);
                    }
                }
                else{
                    Desellect();
                    Sellect();
                }
            }
        }

        public override void Desellect()
        {
            SkillImg.sprite = null;
            skill = null;
            Sellected = false;

            skillButton.RemoveSkill();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!bırakılıyor){
                Sellect();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Desellect();
            StartCoroutine(nameof(Bırak));
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            Sellected = true;
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            Sellected = false;
        }
        /// <summary>
        /// save xml return SkillId
        /// </summary>
        public short Save()
        {
            if (skill){
                return (short)skill.skillName;
            }
            else{
                return -1;
            }
        }

        /// <summary>
        /// Load xml 
        /// </summary>
        public void Load(short loadData)
        { 
            var savedSkill = SkillSlotCtrl.instance.ReturnSkill((SkillName)loadData);
            if (savedSkill == default)
            {
                //Debug.Log("yuvaya birsey kaydedilmedi save edilmis skill yok");
                return;
            }

            SkillImg.sprite = savedSkill.icon;
            skill = savedSkill;

            SkillBox.SellectedSkill = savedSkill;
            Sellect();

            // combo sistemine eklenecek şeyler
            // 3 butondan açılıp kapalı olacaklar fln
        }

        private bool bırakılıyor;

        private IEnumerator Bırak()
        {
            bırakılıyor = true;
            yield return new WaitForSecondsRealtime(0.5f);
            bırakılıyor = false;
        }

        
    }

}