using System;
using System.Collections;
using System.Collections.Generic;

namespace TaleWorlds.DotNet
{
	// Token: 0x02000024 RID: 36
	public sealed class NativeArrayEnumerator<T> : IReadOnlyList<T>, IEnumerable<T>, IEnumerable, IReadOnlyCollection<T> where T : struct
	{
		// Token: 0x060000D5 RID: 213 RVA: 0x000049CD File Offset: 0x00002BCD
		public NativeArrayEnumerator(NativeArray nativeArray)
		{
			this._nativeArray = nativeArray;
		}

		// Token: 0x17000017 RID: 23
		public T this[int index]
		{
			get
			{
				return this._nativeArray.GetElementAt<T>(index);
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x060000D7 RID: 215 RVA: 0x000049EA File Offset: 0x00002BEA
		public int Count
		{
			get
			{
				return this._nativeArray.GetLength<T>();
			}
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x000049F7 File Offset: 0x00002BF7
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return this._nativeArray.GetEnumerator<T>().GetEnumerator();
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x00004A09 File Offset: 0x00002C09
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this._nativeArray.GetEnumerator<T>().GetEnumerator();
		}

		// Token: 0x04000054 RID: 84
		private readonly NativeArray _nativeArray;
	}
}
