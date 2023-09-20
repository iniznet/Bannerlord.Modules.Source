using System;

namespace TaleWorlds.Library
{
	public class MBList2D<T> : IMBCollection
	{
		public int Count1 { get; private set; }

		public int Count2 { get; private set; }

		private int Capacity
		{
			get
			{
				return this._data.Length;
			}
		}

		public MBList2D(int count1, int count2)
		{
			this._data = new T[count1 * count2];
			this.Count1 = count1;
			this.Count2 = count2;
		}

		public T[] RawArray
		{
			get
			{
				return this._data;
			}
		}

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

		public void Clear()
		{
			for (int i = 0; i < this.Count1 * this.Count2; i++)
			{
				this._data[i] = default(T);
			}
		}

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

		public void CopyRowTo(int sourceIndex1, int sourceIndex2, MBList2D<T> destination, int destinationIndex1, int destinationIndex2, int copyCount)
		{
			for (int i = 0; i < copyCount; i++)
			{
				destination[destinationIndex1, destinationIndex2 + i] = this[sourceIndex1, sourceIndex2 + i];
			}
		}

		private T[] _data;
	}
}
