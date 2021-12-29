using System;
using UnityEngine;

namespace AnilTools
{
    // an object pool system for components
    public class ComponentPool<C> where C : Component
    {
        private readonly C[] components;
        private int index;

        // Pops a component in pool
        public C Get()
        {
            if ((++index) == components.Length) index = 0;
            return components[index];
        }

        public ComponentPool(in int size, Transform parent, Action<C> OnSpawn = null)
        { 
            this.components = new C[size];
            for (; index < size; index++)
            {
                components[index] = new GameObject(typeof(C).Name).AddComponent<C>();
                components[index].transform.parent = parent;
                OnSpawn?.Invoke(components[index]);
            }
            index = 0;
        }

        public ComponentPool(in int size, Action<C> OnSpawn = null) : this(size, new GameObject($"{typeof(C).Name} Pool").transform, OnSpawn) { }
        
        public ComponentPool(in int size, C prefab, Action<C> onSpawn = null)
        {
            this.components = new C[size];
            var parent = new GameObject($"{prefab.name} Pool").transform;
            for (; index < components.Length; index++)
            {
                components[index] = UnityEngine.Object.Instantiate(prefab, parent);
                onSpawn?.Invoke(components[index]);
            }
            index = 0;
        }
    }
}
