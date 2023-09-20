using System;

namespace TaleWorlds.Library
{
	// Token: 0x0200005F RID: 95
	public class MBList2D<T> : IMBCollection
	{
		// Token: 0x17000045 RID: 69
		// (get) Token: 0x060002DF RID: 735 RVA: 0x000098AE File Offset: 0x00007AAE
		// (set) Token: 0x060002E0 RID: 736 RVA: 0x000098B6 File Offset: 0x00007AB6
		public int Count1 { get; private set; }

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060002E1 RID: 737 RVA: 0x000098BF File Offset: 0x00007ABF
		// (set) Token: 0x060002E2 RID: 738 RVA: 0x000098C7 File Offset: 0x00007AC7
		public int Count2 { get; private set; }

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060002E3 RID: 739 RVA: 0x000098D0 File Offset: 0x00007AD0
		private int Capacity
		{
			get
			{
				return this._data.Length;
			}
		}

		// Token: 0x060002E4 RID: 740 RVA: 0x000098DA File Offset: 0x00007ADA
		public MBList2D(int count1, int count2)
		{
			this._data = new T[count1 * count2];
			this.Count1 = count1;
			this.Count2 = count2;
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060002E5 RID: 741 RVA: 0x000098FE File Offset: 0x00007AFE
		public T[] RawArray
		{
			get
			{
				return this._data;
			}
		}

		// Token: 0x17000049 RID: 73
		public T this[int index1, int index2]
		{
			get
			{
				return this._data[index1 * this.Count2 + index2];
			}
			set
			{
				this._data[index1 * this.Count2 + index2] = value;
			}
		}

		// Token: 0x060002E8 RID: 744 RVA: 0x00009938 File Offset: 0x00007B38
		public bool Contains(T item)
		{
			for (int i = 0; i < this.Count1; i++)
			{
				for (int j = 0; j < this.Count2; j++)
				{
					if (this._data[i * this.Count2 + j].Equals(item))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x060002E9 RID: 745 RVA: 0x00009994 File Offset: 0x00007B94
		public void Clear()
		{
			for (int i = 0; i < this.Count1 * this.Count2; i++)
			{
				this._data[i] = default(T);
			}
		}

		// Token: 0x060002EA RID: 746 RVA: 0x000099D0 File Offset: 0x00007BD0
		public void ResetWithNewCount(int newCount1, int newCount2)
		{
			if (this.Count1 == newCount1 && this.Count2 == newCount2)
			{
				this.Clear();
				return;
			}
			this.Count1 = newCount1;
			this.Count2 = newCount2;
			if (this.Capacity < newCount1 * newCount2)
			{
				this._data = new T[MathF.Max(this.Capacity * 2, newCount1 * newCount2)];
				return;
			}
			this.Clear();
		}

		// Token: 0x060002EB RID: 747 RVA: 0x00009A34 File Offset: 0x00007C34
		public void CopyRowTo(int sourceIndex1, int sourceIndex2, MBList2D<T> destination, int destinationIndex1, int destinationIndex2, int copyCount)
		{
			for (int i = 0; i < copyCount; i++)
			{
				destination[destinationIndex1, destinationIndex2 + i] = this[sourceIndex1, sourceIndex2 + i];
			}
		}

		// Token: 0x040000FC RID: 252
		private T[] _data;
	}
}
