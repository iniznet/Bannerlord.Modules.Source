using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TaleWorlds.Library
{
	// Token: 0x02000060 RID: 96
	public class MBArrayList<T> : IMBCollection, ICollection, IEnumerable, IEnumerable<T>
	{
		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060002EC RID: 748 RVA: 0x00009A64 File Offset: 0x00007C64
		// (set) Token: 0x060002ED RID: 749 RVA: 0x00009A6C File Offset: 0x00007C6C
		public int Count { get; private set; }

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x060002EE RID: 750 RVA: 0x00009A75 File Offset: 0x00007C75
		public int Capacity
		{
			get
			{
				return this._data.Length;
			}
		}

		// Token: 0x060002EF RID: 751 RVA: 0x00009A7F File Offset: 0x00007C7F
		public MBArrayList()
		{
			this._data = new T[1];
			this.Count = 0;
		}

		// Token: 0x060002F0 RID: 752 RVA: 0x00009A9A File Offset: 0x00007C9A
		public MBArrayList(List<T> list)
		{
			this._data = list.ToArray();
			this.Count = this._data.Length;
		}

		// Token: 0x060002F1 RID: 753 RVA: 0x00009ABC File Offset: 0x00007CBC
		public MBArrayList(IEnumerable<T> list)
		{
			this._data = list.ToArray<T>();
			this.Count = this._data.Length;
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x060002F2 RID: 754 RVA: 0x00009ADE File Offset: 0x00007CDE
		public T[] RawArray
		{
			get
			{
				return this._data;
			}
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x060002F3 RID: 755 RVA: 0x00009AE6 File Offset: 0x00007CE6
		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x060002F4 RID: 756 RVA: 0x00009AE9 File Offset: 0x00007CE9
		public object SyncRoot
		{
			get
			{
				return null;
			}
		}

		// Token: 0x1700004F RID: 79
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

		// Token: 0x060002F7 RID: 759 RVA: 0x00009B0C File Offset: 0x00007D0C
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

		// Token: 0x060002F8 RID: 760 RVA: 0x00009B4C File Offset: 0x00007D4C
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

		// Token: 0x060002F9 RID: 761 RVA: 0x00009B88 File Offset: 0x00007D88
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

		// Token: 0x060002FA RID: 762 RVA: 0x00009B97 File Offset: 0x00007D97
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x060002FB RID: 763 RVA: 0x00009BA0 File Offset: 0x00007DA0
		public void Clear()
		{
			for (int i = 0; i < this.Count; i++)
			{
				this._data[i] = default(T);
			}
			this.Count = 0;
		}

		// Token: 0x060002FC RID: 764 RVA: 0x00009BDC File Offset: 0x00007DDC
		private void EnsureCapacity(int newMinimumCapacity)
		{
			if (newMinimumCapacity > this.Capacity)
			{
				T[] array = new T[MathF.Max(this.Capacity * 2, newMinimumCapacity)];
				this.CopyTo(array, 0);
				this._data = array;
			}
		}

		// Token: 0x060002FD RID: 765 RVA: 0x00009C18 File Offset: 0x00007E18
		public void Add(T item)
		{
			this.EnsureCapacity(this.Count + 1);
			this._data[this.Count] = item;
			int count = this.Count;
			this.Count = count + 1;
		}

		// Token: 0x060002FE RID: 766 RVA: 0x00009C58 File Offset: 0x00007E58
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

		// Token: 0x060002FF RID: 767 RVA: 0x00009CCC File Offset: 0x00007ECC
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

		// Token: 0x06000300 RID: 768 RVA: 0x00009D40 File Offset: 0x00007F40
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

		// Token: 0x040000FF RID: 255
		private T[] _data;
	}
}
