
using System;
using System.Collections.Generic;

namespace AnilTools 
{
    public class ObjectPool<T>
    {
        public List<T> list;
        public int currentIndex;

        public ObjectPool(T[] elements)
        {
            list = new List<T>(elements);
        }

        public ObjectPool(List<T> elements)
        {
            list = new List<T>(elements);
        }

        public T Get()
        {
            if (++currentIndex == list.Count) currentIndex = 0;
            return list[currentIndex];
        }
    }
}
