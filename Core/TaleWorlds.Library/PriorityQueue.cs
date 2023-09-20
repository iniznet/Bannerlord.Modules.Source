using System;
using System.Collections;
using System.Collections.Generic;

namespace TaleWorlds.Library
{
	// Token: 0x02000078 RID: 120
	public class PriorityQueue<TPriority, TValue> : ICollection<KeyValuePair<TPriority, TValue>>, IEnumerable<KeyValuePair<TPriority, TValue>>, IEnumerable
	{
		// Token: 0x17000069 RID: 105
		// (get) Token: 0x060003FE RID: 1022 RVA: 0x0000C932 File Offset: 0x0000AB32
		private IComparer<TPriority> Comparer
		{
			get
			{
				if (this._customComparer == null)
				{
					return Comparer<TPriority>.Default;
				}
				return this._customComparer;
			}
		}

		// Token: 0x060003FF RID: 1023 RVA: 0x0000C948 File Offset: 0x0000AB48
		public PriorityQueue()
		{
			this._baseHeap = new List<KeyValuePair<TPriority, TValue>>();
		}

		// Token: 0x06000400 RID: 1024 RVA: 0x0000C95B File Offset: 0x0000AB5B
		public PriorityQueue(int capacity)
		{
			this._baseHeap = new List<KeyValuePair<TPriority, TValue>>(capacity);
		}

		// Token: 0x06000401 RID: 1025 RVA: 0x0000C96F File Offset: 0x0000AB6F
		public PriorityQueue(int capacity, IComparer<TPriority> comparer)
		{
			if (comparer == null)
			{
				throw new ArgumentNullException();
			}
			this._baseHeap = new List<KeyValuePair<TPriority, TValue>>(capacity);
			this._customComparer = comparer;
		}

		// Token: 0x06000402 RID: 1026 RVA: 0x0000C993 File Offset: 0x0000AB93
		public PriorityQueue(IComparer<TPriority> comparer)
		{
			if (comparer == null)
			{
				throw new ArgumentNullException();
			}
			this._baseHeap = new List<KeyValuePair<TPriority, TValue>>();
			this._customComparer = comparer;
		}

		// Token: 0x06000403 RID: 1027 RVA: 0x0000C9B6 File Offset: 0x0000ABB6
		public PriorityQueue(IEnumerable<KeyValuePair<TPriority, TValue>> data)
			: this(data, Comparer<TPriority>.Default)
		{
		}

		// Token: 0x06000404 RID: 1028 RVA: 0x0000C9C4 File Offset: 0x0000ABC4
		public PriorityQueue(IEnumerable<KeyValuePair<TPriority, TValue>> data, IComparer<TPriority> comparer)
		{
			if (data == null || comparer == null)
			{
				throw new ArgumentNullException();
			}
			this._customComparer = comparer;
			this._baseHeap = new List<KeyValuePair<TPriority, TValue>>(data);
			for (int i = this._baseHeap.Count / 2 - 1; i >= 0; i--)
			{
				this.HeapifyFromBeginningToEnd(i);
			}
		}

		// Token: 0x06000405 RID: 1029 RVA: 0x0000CA17 File Offset: 0x0000AC17
		public static PriorityQueue<TPriority, TValue> MergeQueues(PriorityQueue<TPriority, TValue> pq1, PriorityQueue<TPriority, TValue> pq2)
		{
			if (pq1 == null || pq2 == null)
			{
				throw new ArgumentNullException();
			}
			if (pq1.Comparer != pq2.Comparer)
			{
				throw new InvalidOperationException("Priority queues to be merged must have equal comparers");
			}
			return PriorityQueue<TPriority, TValue>.MergeQueues(pq1, pq2, pq1.Comparer);
		}

		// Token: 0x06000406 RID: 1030 RVA: 0x0000CA4C File Offset: 0x0000AC4C
		public static PriorityQueue<TPriority, TValue> MergeQueues(PriorityQueue<TPriority, TValue> pq1, PriorityQueue<TPriority, TValue> pq2, IComparer<TPriority> comparer)
		{
			if (pq1 == null || pq2 == null || comparer == null)
			{
				throw new ArgumentNullException();
			}
			PriorityQueue<TPriority, TValue> priorityQueue = new PriorityQueue<TPriority, TValue>(pq1.Count + pq2.Count, comparer);
			priorityQueue._baseHeap.AddRange(pq1._baseHeap);
			priorityQueue._baseHeap.AddRange(pq2._baseHeap);
			for (int i = priorityQueue._baseHeap.Count / 2 - 1; i >= 0; i--)
			{
				priorityQueue.HeapifyFromBeginningToEnd(i);
			}
			return priorityQueue;
		}

		// Token: 0x06000407 RID: 1031 RVA: 0x0000CAC0 File Offset: 0x0000ACC0
		public void Enqueue(TPriority priority, TValue value)
		{
			this.Insert(priority, value);
		}

		// Token: 0x06000408 RID: 1032 RVA: 0x0000CACA File Offset: 0x0000ACCA
		public KeyValuePair<TPriority, TValue> Dequeue()
		{
			if (!this.IsEmpty)
			{
				KeyValuePair<TPriority, TValue> keyValuePair = this._baseHeap[0];
				this.DeleteRoot();
				return keyValuePair;
			}
			throw new InvalidOperationException("Priority queue is empty");
		}

		// Token: 0x06000409 RID: 1033 RVA: 0x0000CAF4 File Offset: 0x0000ACF4
		public TValue DequeueValue()
		{
			return this.Dequeue().Value;
		}

		// Token: 0x0600040A RID: 1034 RVA: 0x0000CB0F File Offset: 0x0000AD0F
		public KeyValuePair<TPriority, TValue> Peek()
		{
			if (!this.IsEmpty)
			{
				return this._baseHeap[0];
			}
			throw new InvalidOperationException("Priority queue is empty");
		}

		// Token: 0x0600040B RID: 1035 RVA: 0x0000CB30 File Offset: 0x0000AD30
		public TValue PeekValue()
		{
			return this.Peek().Value;
		}

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x0600040C RID: 1036 RVA: 0x0000CB4B File Offset: 0x0000AD4B
		public bool IsEmpty
		{
			get
			{
				return this._baseHeap.Count == 0;
			}
		}

		// Token: 0x0600040D RID: 1037 RVA: 0x0000CB5C File Offset: 0x0000AD5C
		private void ExchangeElements(int pos1, int pos2)
		{
			KeyValuePair<TPriority, TValue> keyValuePair = this._baseHeap[pos1];
			this._baseHeap[pos1] = this._baseHeap[pos2];
			this._baseHeap[pos2] = keyValuePair;
		}

		// Token: 0x0600040E RID: 1038 RVA: 0x0000CB9C File Offset: 0x0000AD9C
		private void Insert(TPriority priority, TValue value)
		{
			KeyValuePair<TPriority, TValue> keyValuePair = new KeyValuePair<TPriority, TValue>(priority, value);
			this._baseHeap.Add(keyValuePair);
			this.HeapifyFromEndToBeginning(this._baseHeap.Count - 1);
		}

		// Token: 0x0600040F RID: 1039 RVA: 0x0000CBD4 File Offset: 0x0000ADD4
		private int HeapifyFromEndToBeginning(int pos)
		{
			if (pos >= this._baseHeap.Count)
			{
				return -1;
			}
			IComparer<TPriority> comparer = this.Comparer;
			while (pos > 0)
			{
				int num = (pos - 1) / 2;
				if (comparer.Compare(this._baseHeap[num].Key, this._baseHeap[pos].Key) >= 0)
				{
					break;
				}
				this.ExchangeElements(num, pos);
				pos = num;
			}
			return pos;
		}

		// Token: 0x06000410 RID: 1040 RVA: 0x0000CC44 File Offset: 0x0000AE44
		private void DeleteRoot()
		{
			if (this._baseHeap.Count <= 1)
			{
				this._baseHeap.Clear();
				return;
			}
			this._baseHeap[0] = this._baseHeap[this._baseHeap.Count - 1];
			this._baseHeap.RemoveAt(this._baseHeap.Count - 1);
			this.HeapifyFromBeginningToEnd(0);
		}

		// Token: 0x06000411 RID: 1041 RVA: 0x0000CCB0 File Offset: 0x0000AEB0
		private void HeapifyFromBeginningToEnd(int pos)
		{
			if (pos >= this._baseHeap.Count)
			{
				return;
			}
			IComparer<TPriority> comparer = this.Comparer;
			for (;;)
			{
				int num = pos;
				int num2 = 2 * pos + 1;
				int num3 = 2 * pos + 2;
				if (num2 < this._baseHeap.Count && comparer.Compare(this._baseHeap[num].Key, this._baseHeap[num2].Key) < 0)
				{
					num = num2;
				}
				if (num3 < this._baseHeap.Count && comparer.Compare(this._baseHeap[num].Key, this._baseHeap[num3].Key) < 0)
				{
					num = num3;
				}
				if (num == pos)
				{
					break;
				}
				this.ExchangeElements(num, pos);
				pos = num;
			}
		}

		// Token: 0x06000412 RID: 1042 RVA: 0x0000CD7B File Offset: 0x0000AF7B
		public void Add(KeyValuePair<TPriority, TValue> item)
		{
			this.Enqueue(item.Key, item.Value);
		}

		// Token: 0x06000413 RID: 1043 RVA: 0x0000CD91 File Offset: 0x0000AF91
		public void Clear()
		{
			this._baseHeap.Clear();
		}

		// Token: 0x06000414 RID: 1044 RVA: 0x0000CD9E File Offset: 0x0000AF9E
		public bool Contains(KeyValuePair<TPriority, TValue> item)
		{
			return this._baseHeap.Contains(item);
		}

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x06000415 RID: 1045 RVA: 0x0000CDAC File Offset: 0x0000AFAC
		public int Count
		{
			get
			{
				return this._baseHeap.Count;
			}
		}

		// Token: 0x06000416 RID: 1046 RVA: 0x0000CDB9 File Offset: 0x0000AFB9
		public void CopyTo(KeyValuePair<TPriority, TValue>[] array, int arrayIndex)
		{
			this._baseHeap.CopyTo(array, arrayIndex);
		}

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x06000417 RID: 1047 RVA: 0x0000CDC8 File Offset: 0x0000AFC8
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06000418 RID: 1048 RVA: 0x0000CDCC File Offset: 0x0000AFCC
		public bool Remove(KeyValuePair<TPriority, TValue> item)
		{
			int num = this._baseHeap.IndexOf(item);
			if (num < 0)
			{
				return false;
			}
			this._baseHeap[num] = this._baseHeap[this._baseHeap.Count - 1];
			this._baseHeap.RemoveAt(this._baseHeap.Count - 1);
			if (this.HeapifyFromEndToBeginning(num) == num)
			{
				this.HeapifyFromBeginningToEnd(num);
			}
			return true;
		}

		// Token: 0x06000419 RID: 1049 RVA: 0x0000CE3A File Offset: 0x0000B03A
		public IEnumerator<KeyValuePair<TPriority, TValue>> GetEnumerator()
		{
			return this._baseHeap.GetEnumerator();
		}

		// Token: 0x0600041A RID: 1050 RVA: 0x0000CE4C File Offset: 0x0000B04C
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x04000138 RID: 312
		private readonly List<KeyValuePair<TPriority, TValue>> _baseHeap;

		// Token: 0x04000139 RID: 313
		private readonly IComparer<TPriority> _customComparer;
	}
}
