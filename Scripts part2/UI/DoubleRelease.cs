using UnityEngine.Events;
using UnityEngine;
using UnityEngine.EventSystems;
using AnilTools;

namespace Assets.Resources.Scripts.UI
{
    public class DoubleRelease : MonoBehaviour  , IPointerEnterHandler
    {
        public UnityEvent ReleaseEvent;

        private enum State
        { 
            firstRelease,
            secondRelease,
        }

        [SerializeField]
        private float defaultTime;
        [SerializeField]
        private Timer timer;
        [SerializeField]
        private State state;


        [SerializeField]
        private Timer CanReleaseTimer;

        private void Start()
        {
            CanReleaseTimer = new Timer(1);
            timer = new Timer(defaultTime);
            timer.Start();
            CanReleaseTimer.Start();
        }

#if UNITY_EDITOR || UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        [SerializeField]
        private KeyCode keyCode;
        
        private void Update()
        {
            if (Input.GetKeyDown(keyCode))
            {
                OnPointerEnter(null);
            }
        }
#endif
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (CanReleaseTimer.TimeHasCome())
            if (!timer.TimeHasCome())
            {
                switch (state)
                {
                    case State.firstRelease: state += 1; break;
                    case State.secondRelease:
                        state = 0;
                        ReleaseEvent.Invoke();
                        timer.Reset();
                        CanReleaseTimer.Reset();
                        Debug2.Log("double release");
                        break;
                }
            }
            else
            {
                state = 0;
                timer.Reset();
            }
        }
    }
}