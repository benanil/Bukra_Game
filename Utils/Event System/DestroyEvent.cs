

using UnityEngine;

namespace AnilTools.EventSystem
{

    public class DestroyEvent : MonoBehaviour
    {
        public Component[] components;
    
        [ContextMenu("Destroy")]
        public void Destroy()
        {
            for (int i = 0; i < components.Length; i++)
            {
                DestroyImmediate(components[i]);
            }
            DestroyImmediate(this);
        }
    }

}