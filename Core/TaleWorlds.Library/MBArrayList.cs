using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TaleWorlds.Library
{
	public class MBArrayList<T> : IMBCollection, ICollection, IEnumerable, IEnumerable<T>
	{
		public int Count { get; private set; }

		public int Capacity
		{
			get
			{
				return this._data.Length;
			}
		}

		public MBArrayList()
		{
			this._data = new T[1];
			this.Count = 0;
		}

		public MBArrayList(List<T> list)
		{
			this._data = list.ToArray();
			this.Count = this._data.Length;
		}

		public MBArrayList(IEnumerable<T> list)
		{
			this._data = list.ToArray<T>();
			this.Count = this._data.Length;
		}

		public T[] RawArray
		{
			get
			{
				return this._data;
			}
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

		public T this[int index]
		{
			get
			{
				return this._data[index];
			}
			set
			{
				this._data[index] = value;
			}
		}

		public int IndexOf(T item)
		{
			int num = -1;
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			for (int i = 0; i < this.Count; i++)
			{
				if (@default.Equals(this._data[i], item))
				{
					num = i;
					break;
				}
			}
			return num;
		}

		public bool Contains(T item)
		{
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			for (int i = 0; i < this.Count; i++)
			{
				if (@default.Equals(this._data[i], item))
				{
					return true;
				}
			}
			return false;
		}

		public IEnumerator<T> GetEnumerator()
		{
			int num;
			for (int i = 0; i < this.Count; i = num + 1)
			{
				yield return this._data[i];
				num = i;
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public void Clear()
		{
			for (int i = 0; i < this.Count; i++)
			{
				this._data[i] = default(T);
			}
			this.Count = 0;
		}

		private void EnsureCapacity(int newMinimumCapacity)
		{
			if (newMinimumCapacity > this.Capacity)
			{
				T[] array = new T[MathF.Max(this.Capacity * 2, newMinimumCapacity)];
				this.CopyTo(array, 0);
				this._data = array;
			}
		}

		public void Add(T item)
		{
			this.EnsureCapacity(this.Count + 1);
			this._data[this.Count] = item;
			int count = this.Count;
			this.Count = count + 1;
		}

		public void AddRange(IEnumerable<T> list)
		{
			foreach (T t in list)
			{
				this.EnsureCapacity(this.Count + 1);
				this._data[this.Count] = t;
				int count = this.Count;
				this.Count = count + 1;
			}
		}

		public bool Remove(T item)
		{
			int num = this.IndexOf(item);
			if (num >= 0)
			{
				for (int i = num; i < this.Count - 1; i++)
				{
					this._data[num] = this._data[num + 1];
				}
				int count = this.Count;
				this.Count = count - 1;
				this._data[this.Count] = default(T);
				return true;
			}
			return false;
		}

		public void CopyTo(Array array, int index)
		{
			T[] array2;
			if ((array2 = array as T[]) != null)
			{
				for (int i = 0; i < this.Count; i++)
				{
					array2[i + index] = this._data[i];
				}
				return;
			}
			array.GetType().GetElementType();
			object[] array3 = array as object[];
			try
			{
				for (int j = 0; j < this.Count; j++)
				{
					array3[index++] = this._data[j];
				}
			}
			catch (ArrayTypeMismatchException)
			{
				Debug.FailedAssert("Invalid array type", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.Library\\MBArrayList.cs", "CopyTo", 210);
			}
		}

		private T[] _data;
	}
}
