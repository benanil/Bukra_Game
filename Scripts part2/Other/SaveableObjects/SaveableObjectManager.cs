using AnilTools;
using AnilTools.Save;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace SaveableObjects
{
    public class SaveableObjectManager : Singleton<SaveableObjectManager>
    {
        public const string savePath = "SaveableObjects/";
        public const string saveName = "SaveObject.json";

        public List<SaveableObjectData> saveableObjects;

        public void Load()
        {
#if UNITY_EDITOR

            var objs = JsonManager.LoadList<SaveableObjectData>(savePath, saveName) as List<SaveableObjectData>;

            try
            {
                if (objs != default)
                {
                    for (int i = 0; i < objs.Count; i++)
                    {
                        var objInScene = UnityShortCuts.FindObjectFromInstanceID(objs[i].instanceId) as SaveableObject;

                        if (objs.Count == i)
                        {
                            Debug2.Log("returned from saveable obl manager");
                            return;
                        }

                        objInScene.objectData = objs[i];

                        if (objs[i].activity == true)
                            objInScene.Respawn();
                        else
                            objInScene.Destroy();
                    }
                }
            }
            catch 
            {
                Refresh();
                if (objs != default)
                {
                    for (int i = 0; i < objs.Count; i++)
                    {
                        var objInScene = UnityShortCuts.FindObjectFromInstanceID(objs[i].instanceId) as SaveableObject;

                        if (objs.Count == i)
                        {
                            Debug2.Log("returned from saveable obl manager");
                            return;
                        }

                        objInScene.objectData = objs[i];

                        if (objs[i].activity == true)
                            objInScene.Respawn();
                        else
                            objInScene.Destroy();
                    }
                }
            }
#endif
        }

#if UNITY_EDITOR
        [ContextMenu("save saveables")]
#endif
        public void Save()
        {
            JsonManager.SaveList(savePath, saveName , saveableObjects);
        }

        public void Refresh()
        {
            List<SaveableObjectData> objectDatas = new List<SaveableObjectData>();

            FindObjectsOfType<SaveableObject>().Foreach(x =>
            {
                x.SetId();
                objectDatas.Add(x.objectData);
            });

            saveableObjects = objectDatas;
        }

        private void OnApplicationQuit()
        {
            Save();
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(SaveableObjectManager))]
    public class SaveObjectManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Refresh"))
            {
                ((SaveableObjectManager)target).Refresh();
            }
        }
    }
#endif

}