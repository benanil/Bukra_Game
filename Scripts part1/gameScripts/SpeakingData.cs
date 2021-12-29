using Subtegral.DialogueSystem.DataContainers;
using UnityEngine;

namespace Dialog
{
    [CreateAssetMenu(fileName = "SpeakingData", menuName = "SpeakingData")]
    public class SpeakingData : ScriptableObject
    {
        public DialogueContainer TurkishSpeak;
        public DialogueContainer EnglishSpeak;
    }
}