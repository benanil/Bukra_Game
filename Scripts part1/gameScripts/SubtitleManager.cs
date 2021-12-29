
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using AnilTools;
using UnityEngine.Events;

namespace Assets.Scripts
{
    public class SubtitleManager : Singleton<SubtitleManager>
    {
        private string[] subtitles = new string[8]; // max 8

        public Text text;

        public WaitForSecondsRealtime WaitTime = new WaitForSecondsRealtime(2.5f);

        public void StartSubtitle(string[] subtitles, UnityEvent endAction = null)
        {
            this.subtitles = subtitles;

            StartCoroutine(RunSubtitle(endAction));
        }

        public void StartSubtitle(string subtitle, UnityEvent endAction = null)
        {
            this.subtitles[0] = subtitle;

            for (int i = 1; i < subtitles.Length; i++){
                subtitles[i] = string.Empty;
            }
            StartCoroutine(RunSubtitle(endAction));
        }

        public void ShowImmediate(string word, UnityEvent action)
        {
            StartCoroutine(_ShowImmediate(word, action)); 
        }

        public IEnumerator _ShowImmediate(string word, UnityEvent endAction){

            text.text = word;
            yield return new WaitForSecondsRealtime(.5f);
            text.text = string.Empty;

            if (endAction != null){
                endAction.Invoke();
            }
        }

        private IEnumerator RunSubtitle(UnityEvent endAction)
        {
            int i = 0;

            while (!string.IsNullOrEmpty(subtitles[i]))
            {
                i++;
                yield return text.TextLoadingAnim(subtitles[i]);
                yield return WaitTime;
            }

            text.text = string.Empty;

            if (endAction != null)
            {
                endAction.Invoke();
            }
        }
    }
}