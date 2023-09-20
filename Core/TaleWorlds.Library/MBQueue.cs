using System;
using System.Collections;
using System.Collections.Generic;

namespace TaleWorlds.Library
{
	// Token: 0x02000065 RID: 101
	public class MBQueue<T> : IMBCollection, ICollection, IEnumerable, IEnumerable<T>
	{
		// Token: 0x17000050 RID: 80
		// (get) Token: 0x06000379 RID: 889 RVA: 0x0000B1B8 File Offset: 0x000093B8
		public int Count
		{
			get
			{
				return this._data.Count;
			}
		}

		// Token: 0x0600037A RID: 890 RVA: 0x0000B1C5 File Offset: 0x000093C5
		public MBQueue()
		{
			this._data = new Queue<T>();
		}

		// Token: 0x0600037B RID: 891 RVA: 0x0000B1D8 File Offset: 0x000093D8
		public MBQueue(Queue<T> queue)
		{
			this._data = new Queue<T>(queue);
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x0600037C RID: 892 RVA: 0x0000B1EC File Offset: 0x000093EC
		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x0600037D RID: 893 RVA: 0x0000B1EF File Offset: 0x000093EF
		public object SyncRoot
		{
			get
			{
				return null;
			}
		}

		// Token: 0x0600037E RID: 894 RVA: 0x0000B1F2 File Offset: 0x000093F2
		public bool Contains(T item)
		{
			return this._data.Contains(item);
		}

		// Token: 0x0600037F RID: 895 RVA: 0x0000B200 File Offset: 0x00009400
		public IEnumerator<T> GetEnumerator()
		{
			return this._data.GetEnumerator();
		}

		// Token: 0x06000380 RID: 896 RVA: 0x0000B212 File Offset: 0x00009412
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x06000381 RID: 897 RVA: 0x0000B21A File Offset: 0x0000941A
		public void Clear()
		{
			this._data.Clear();
		}

		// Token: 0x06000382 RID: 898 RVA: 0x0000B227 File Offset: 0x00009427
		public void Enqueue(T item)
		{
			this._data.Enqueue(item);
		}

		// Token: 0x06000383 RID: 899 RVA: 0x0000B235 File Offset: 0x00009435
		public T Dequeue()
		{
			return this._data.Dequeue();
		}

		// Token: 0x06000384 RID: 900 RVA: 0x0000B242 File Offset: 0x00009442
		public void CopyTo(Array array, int index)
		{
			this._data.CopyTo((T[])array, index);
		}

		// Token: 0x04000111 RID: 273
		private readonly Queue<T> _data;
	}
}
