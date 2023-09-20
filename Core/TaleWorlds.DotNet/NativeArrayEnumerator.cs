using System;
using System.Collections;
using System.Collections.Generic;

namespace TaleWorlds.DotNet
{
	public sealed class NativeArrayEnumerator<T> : IReadOnlyList<T>, IEnumerable<T>, IEnumerable, IReadOnlyCollection<T> where T : struct
	{
		public NativeArrayEnumerator(NativeArray nativeArray)
		{
			this._nativeArray = nativeArray;
		}

		public T this[int index]
		{
			get
			{
				return this._nativeArray.GetElementAt<T>(index);
			}
		}

		public int Count
		{
			get
			{
				return this._nativeArray.GetLength<T>();
			}
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return this._nativeArray.GetEnumerator<T>().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this._nativeArray.GetEnumerator<T>().GetEnumerator();
		}

		private readonly NativeArray _nativeArray;
	}
}
