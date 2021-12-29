using UnityEngine;

namespace AnilTools.Assets.Resources.Utils.heleper
{
    public class Note : MonoBehaviour{
#if UNITY_EDITOR
        [Multiline(3)]
        public string note;
#endif
    }
}