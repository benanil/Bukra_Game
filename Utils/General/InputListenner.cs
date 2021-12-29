
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AnilTools
{
    public class InputListenner : MonoBehaviour
    {
        private static InputListenner Instance;

        public static Dictionary<KeyCode, List<Action>> keyValuePairs;

        private void OnEnable()
        {
            keyValuePairs = new Dictionary<KeyCode, List<Action>>();
        }

        private void Update()
        {
            int oldKeyCount = keyValuePairs.Count;
            foreach (var ( key, actions ) in keyValuePairs)
            {
                int oldActionCount = actions.Count;
                for (int i = 0; i < actions.Count; i++)
                {
                    if (Input.GetKeyDown(key))
                    {
                        actions[i].Invoke();
                        if (oldActionCount != actions.Count) break;
                        if (oldKeyCount    != keyValuePairs.Count) break;
                    }
                }
            }
        }

        public static void Set(KeyCode keyCode, Action _event) {
            Clear(keyCode);
            Add(keyCode, _event);
        }

        public static void Add(KeyCode keyCode, Action _event)
        {
            CheckInstance();

            if (keyValuePairs.TryGetValue(keyCode, out var value))
            {
                keyValuePairs[keyCode].Add(_event);
            }
            else 
            {
                keyValuePairs.Add(keyCode, new List<Action>() { _event });
            }
        }

        public static void Remove(KeyCode keyCode, Action _event)
        {
            CheckInstance();

            if (keyValuePairs.TryGetValue(keyCode, out var list))
            {
                for (int i = 0; i < list.Count; i++) 
                {
                    if (list[i].Method.Name == _event.Method.Name) 
                    {
                        list.RemoveAt(i);
                        return;
                    }
                }
            }
            else
            {
                Debug.Log("kaldırilmaya çalışılan key dictionary de yok");
                return;
            }
        }

        public static void Clear(KeyCode keyCode) 
        {
            CheckInstance();
            if (keyValuePairs.ContainsKey(keyCode))
            {
                keyValuePairs[keyCode].Clear();
            }
        }

        private static void CheckInstance() 
        {
            if (!Instance) Instance = new GameObject("Inputlistenner").AddComponent<InputListenner>();
        }

    }
    
}