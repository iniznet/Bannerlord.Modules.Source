using System;
using System.Collections.Generic;

namespace TaleWorlds.TwoDimension
{
	public class MaterialPool<T> where T : Material, new()
	{
		public MaterialPool(int initialBufferSize)
		{
			this._materialList = new List<T>(initialBufferSize);
		}

		public T New()
		{
			if (this._nextAvailableIndex < this._materialList.Count)
			{
				T t = this._materialList[this._nextAvailableIndex];
				this._nextAvailableIndex++;
				return t;
			}
			T t2 = new T();
			this._materialList.Add(t2);
			this._nextAvailableIndex++;
			return t2;
		}

		public void ResetAll()
		{
			this._nextAvailableIndex = 0;
		}

		private List<T> _materialList;

		private int _nextAvailableIndex;
	}
}
