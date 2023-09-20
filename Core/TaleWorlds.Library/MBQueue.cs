using System;
using System.Collections;
using System.Collections.Generic;

namespace TaleWorlds.Library
{
	public class MBQueue<T> : IMBCollection, ICollection, IEnumerable, IEnumerable<T>
	{
		public int Count
		{
			get
			{
				return this._data.Count;
			}
		}

		public MBQueue()
		{
			this._data = new Queue<T>();
		}

		public MBQueue(Queue<T> queue)
		{
			this._data = new Queue<T>(queue);
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public object SyncRoot
		{
			get
			{
				return null;
			}
		}

		public bool Contains(T item)
		{
			return this._data.Contains(item);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this._data.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public void Clear()
		{
			this._data.Clear();
		}

		public void Enqueue(T item)
		{
			this._data.Enqueue(item);
		}

		public T Dequeue()
		{
			return this._data.Dequeue();
		}

		public void CopyTo(Array array, int index)
		{
			this._data.CopyTo((T[])array, index);
		}

		private readonly Queue<T> _data;
	}
}
