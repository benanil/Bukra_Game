using UnityEngine;
using UnityEngine.UI;

namespace Dialog
{
    public class MissionGoalSlot : MonoBehaviour
    {
        public Image tik;
        public Text GoalText;

        internal void Initialize(string text)
        {
            GoalText.text = text;
        }

    }
}