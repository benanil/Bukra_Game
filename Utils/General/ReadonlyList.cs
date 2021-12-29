
using System;
using System.Collections;
using System.Collections.Generic;

namespace AnilTools
{
    public class ReadonlyList<T> : ICollection, IEnumerable, IReadOnlyList<T>, IEnumerable<T>, IReadOnlyCollection<T>
	{
		public ReadonlyList(List<T> list)
		{
			this._list = list;
		}

        public int Count => this._list.Count;

        public bool IsSynchronized => false;

        public object SyncRoot => null;

        public T this[int index]
		{
			get
			{
				return this._list[index];
			}
		}

        public bool Contains(T item) => this._list.Contains(item);

        public List<T>.Enumerator GetEnumerator() => this._list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this._list.GetEnumerator();

        public int IndexOf(T item) => this._list.IndexOf(item);

        public void CopyTo(Array array, int index)
		{
            if (array is T[] array2)
            {
                this._list.CopyTo(array2, index);
                return;
            }
            array.GetType().GetElementType();
			object[] array3 = array as object[];
			int count = this._list.Count;
			try
			{
				for (int i = 0; i < count; i++)
				{
					array3[index++] = this._list[i];
				}
			}
			catch (ArrayTypeMismatchException)
			{

			}
		}

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
			return _list.GetEnumerator();
        }

        // Token: 0x040000C8 RID: 200
        private readonly List<T> _list = new List<T>();
	}
}