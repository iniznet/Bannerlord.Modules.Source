using System;
using System.Collections.Generic;

namespace TaleWorlds.TwoDimension
{
	// Token: 0x02000024 RID: 36
	public class MaterialPool<T> where T : Material, new()
	{
		// Token: 0x0600014F RID: 335 RVA: 0x00006FA5 File Offset: 0x000051A5
		public MaterialPool(int initialBufferSize)
		{
			this._materialList = new List<T>(initialBufferSize);
		}

		// Token: 0x06000150 RID: 336 RVA: 0x00006FBC File Offset: 0x000051BC
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

		// Token: 0x06000151 RID: 337 RVA: 0x0000701D File Offset: 0x0000521D
		public void ResetAll()
		{
			this._nextAvailableIndex = 0;
		}

		// Token: 0x040000B9 RID: 185
		private List<T> _materialList;

		// Token: 0x040000BA RID: 186
		private int _nextAvailableIndex;
	}
}
