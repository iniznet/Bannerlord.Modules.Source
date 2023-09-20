using System;
using System.Collections;
using System.Collections.Generic;

namespace TaleWorlds.Library
{
	public class PriorityQueue<TPriority, TValue> : ICollection<KeyValuePair<TPriority, TValue>>, IEnumerable<KeyValuePair<TPriority, TValue>>, IEnumerable
	{
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

		public PriorityQueue()
		{
			this._baseHeap = new List<KeyValuePair<TPriority, TValue>>();
		}

		public PriorityQueue(int capacity)
		{
			this._baseHeap = new List<KeyValuePair<TPriority, TValue>>(capacity);
		}

		public PriorityQueue(int capacity, IComparer<TPriority> comparer)
		{
			if (comparer == null)
			{
				throw new ArgumentNullException();
			}
			this._baseHeap = new List<KeyValuePair<TPriority, TValue>>(capacity);
			this._customComparer = comparer;
		}

		public PriorityQueue(IComparer<TPriority> comparer)
		{
			if (comparer == null)
			{
				throw new ArgumentNullException();
			}
			this._baseHeap = new List<KeyValuePair<TPriority, TValue>>();
			this._customComparer = comparer;
		}

		public PriorityQueue(IEnumerable<KeyValuePair<TPriority, TValue>> data)
			: this(data, Comparer<TPriority>.Default)
		{
		}

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

		public void Enqueue(TPriority priority, TValue value)
		{
			this.Insert(priority, value);
		}

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

		public TValue DequeueValue()
		{
			return this.Dequeue().Value;
		}

		public KeyValuePair<TPriority, TValue> Peek()
		{
			if (!this.IsEmpty)
			{
				return this._baseHeap[0];
			}
			throw new InvalidOperationException("Priority queue is empty");
		}

		public TValue PeekValue()
		{
			return this.Peek().Value;
		}

		public bool IsEmpty
		{
			get
			{
				return this._baseHeap.Count == 0;
			}
		}

		private void ExchangeElements(int pos1, int pos2)
		{
			KeyValuePair<TPriority, TValue> keyValuePair = this._baseHeap[pos1];
			this._baseHeap[pos1] = this._baseHeap[pos2];
			this._baseHeap[pos2] = keyValuePair;
		}

		private void Insert(TPriority priority, TValue value)
		{
			KeyValuePair<TPriority, TValue> keyValuePair = new KeyValuePair<TPriority, TValue>(priority, value);
			this._baseHeap.Add(keyValuePair);
			this.HeapifyFromEndToBeginning(this._baseHeap.Count - 1);
		}

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

		public void Add(KeyValuePair<TPriority, TValue> item)
		{
			this.Enqueue(item.Key, item.Value);
		}

		public void Clear()
		{
			this._baseHeap.Clear();
		}

		public bool Contains(KeyValuePair<TPriority, TValue> item)
		{
			return this._baseHeap.Contains(item);
		}

		public int Count
		{
			get
			{
				return this._baseHeap.Count;
			}
		}

		public void CopyTo(KeyValuePair<TPriority, TValue>[] array, int arrayIndex)
		{
			this._baseHeap.CopyTo(array, arrayIndex);
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

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

		public IEnumerator<KeyValuePair<TPriority, TValue>> GetEnumerator()
		{
			return this._baseHeap.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private readonly List<KeyValuePair<TPriority, TValue>> _baseHeap;

		private readonly IComparer<TPriority> _customComparer;
	}
}
