#pragma warning disable IDE0051 // animation event yüzünden 

using Player;
using UnityEngine;
using UnityEngine.Events;

namespace AnilTools.Assets.Resources.Scripts.Utils.heleper
{
    public class AnimatorEventListener : MonoBehaviour
    {
        [SerializeField] private UnityEvent[] events;

        private void AttackFinish()
        {
            CombatControl.instance.AttackFinish();
        }

        private void Event()
        {
            events[0].Invoke();
        }

        private void Event1()
        {
            events[1].Invoke();
        }

        private void Event2()
        {
            events[2].Invoke();
        }

        private void Event3()
        {
            events[3].Invoke();
        }

    }
}