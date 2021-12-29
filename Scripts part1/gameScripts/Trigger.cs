using UnityEngine;

namespace Dialog
{
    public class Trigger : MonoBehaviour
    {
        public void Speak()
        {
            if (!GameMenu.PlayerOnMenu && !DialogControl.OnConversation)
            {
                var npcController = GetComponentInParent<NpcController2>();
                DialogControl.instance.TriggerEnter(npcController);
            }
        }
    }
}