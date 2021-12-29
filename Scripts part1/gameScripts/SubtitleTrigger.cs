using Assets.Scripts;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Resources.Scripts.gameScripts
{
    public class SubtitleTrigger : MonoBehaviour
    {
        public string[] subtitles;
        public UnityEvent EndEvent;

        private void OnTriggerEnter(Collider other)
        {
            SubtitleManager.instance.StartSubtitle(subtitles, EndEvent);
        }
    }
}