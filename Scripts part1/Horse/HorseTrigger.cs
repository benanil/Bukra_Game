using AnilTools;
using AnilTools.Collision;
using UnityEngine;

namespace Horse
{
    public class HorseTrigger : SphereCollisionDetection
    {
        [SerializeField] private Sprite MountSprite;
        [SerializeField] private HorseAI at;

        private void Awake() 
        {
            at = transform.parent.GetComponent<HorseAI>();
            target = NpcController2.Player.transform; 
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void OnTriggerEnter()
        {
            if (!HorseAI.PlayerOnHorse)
                InputListenner.Set(KeyCode.E, at.BeginMount);
        }

        protected override void OnTriggerExit()
        {
            if (!HorseAI.PlayerOnHorse) Etkilesim.Disable();
        }
    }
}
