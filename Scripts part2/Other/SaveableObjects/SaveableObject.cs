
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UrFairy;

namespace SaveableObjects
{
    public class SaveableObject : MonoBehaviour
    {
        public SaveableObjectData objectData;

        private List<Renderer> _meshRenderers;
        private List<Renderer> meshRenderers
        {
            get
            {
                if (_meshRenderers == null)
                {
                    var renderers = GetComponentsInChildren<Renderer>().ToList();
                    var MainRenderer = GetComponent<Renderer>();
                    if (MainRenderer)
                        renderers.Add(MainRenderer);
                    
                    _meshRenderers = renderers;
                }
                return _meshRenderers;
            }
        }

        /// <summary>
        /// destroys object constantly it doesnt apear next time till save file deleted
        /// </summary>
        public void Destroy()
        {
            meshRenderers.ForEach(x => x.enabled = false);
            objectData.activity = false;
        }

        public void Respawn()
        {
            objectData.activity = true;
            meshRenderers.ForEach(x => x.enabled = true);
        }

        [ContextMenu("set Id")]
        public void SetId()
        {
            objectData.instanceId = GetInstanceID();
        }
    }

    [System.Serializable]
    public class SaveableObjectData
    {
        public bool activity = true;
        public int instanceId;
    }

}