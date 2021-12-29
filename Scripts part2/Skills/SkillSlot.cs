
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

namespace Skill
{
    [RequireComponent(typeof(Image))]
    public class SkillSlot : AnilButton, IPointerDownHandler, IPointerUpHandler, IDragHandler,IPointerClickHandler
    {

        private const string LVL = " LVL";
        
        //xtodo: characterin leveline göre açık yada kapalı renkli olacak

        public Skill Skill;
        
        [SerializeField] private Text Name;
        [SerializeField] private Text RequiredLvl;
        [SerializeField] private Image icon;
        [SerializeField] private Image Border;

        private const float DragDelay = 0.5f;
        [SerializeField] private float DragTime;

        public bool Active => PlayerInfo.Level >= Skill.RequiredLevel;

        public bool Draging { get; private set; }
        public bool Down { get; private set; }
        public bool Up { get; private set; }
        
        private void Start()
        {
            DragTime = DragDelay;
            Border = GetComponent<Image>();
        }

        private void Update()
        {
            if (GameMenu.PlayerOnSkill)
            {
                if (Sellected && Active)
                {
                    DragTime -= 1;
                    if (DragTime <= 0)
                    {
                        SkillBox.SellectedSkill = Skill;
                        SkillBox.instance.Sellect();
                        DragTime = DragDelay;
                    }
                }

                Border.color = Sellected ? Color.white : Color.grey;
                Image.color = Active ? Color.white : Color.grey;
            }
        }

        public void Set(Skill skill)
        {
            if (!skill)
            {
                Debug2.Log("skill null");
                return;
            }
            Skill = skill;
            Name.text = skill.Name;
            icon.sprite = skill.icon;
            SkillMenuHandeller.instance.UpdateLanguage();
            RequiredLvl.text = skill.RequiredLevel + LVL; 
        }

        public void ChangeLanguage(SkillLanguage skillLanguage)
        {
            Name.text = skillLanguage.name;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Draging = true;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Down = true;
            Sellected = true;
            Sellect();
            SkillMenuHandeller.instance.UpdateSkillMenu(Skill);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Up = true;
            Sellected = false;
            Desellect();
            DragTime = DragDelay;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            SkillMenuHandeller.instance.UpdateSkillMenu(Skill);
        }

    }
}