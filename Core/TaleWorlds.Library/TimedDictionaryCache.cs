using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace TaleWorlds.Library
{
	// Token: 0x0200008A RID: 138
	public class TimedDictionaryCache<TKey, TValue>
	{
		// Token: 0x060004BF RID: 1215 RVA: 0x0000F605 File Offset: 0x0000D805
		public TimedDictionaryCache(long validMilliseconds)
		{
			this._dictionary = new Dictionary<TKey, ValueTuple<long, TValue>>();
			this._stopwatch = new Stopwatch();
			this._stopwatch.Start();
			this._validMilliseconds = validMilliseconds;
		}

		// Token: 0x060004C0 RID: 1216 RVA: 0x0000F635 File Offset: 0x0000D835
		public TimedDictionaryCache(TimeSpan validTimeSpan)
			: this((long)validTimeSpan.TotalMilliseconds)
		{
		}

		// Token: 0x060004C1 RID: 1217 RVA: 0x0000F645 File Offset: 0x0000D845
		private bool IsItemExpired(TKey key)
		{
			return this._stopwatch.ElapsedMilliseconds - this._dictionary[key].Item1 >= this._validMilliseconds;
		}

		// Token: 0x060004C2 RID: 1218 RVA: 0x0000F66F File Offset: 0x0000D86F
		private bool RemoveIfExpired(TKey key)
		{
			if (this.IsItemExpired(key))
			{
				this._dictionary.Remove(key);
				return true;
			}
			return false;
		}

		// Token: 0x060004C3 RID: 1219 RVA: 0x0000F68C File Offset: 0x0000D88C
		public void PruneExpiredItems()
		{
			List<TKey> list = new List<TKey>();
			foreach (KeyValuePair<TKey, ValueTuple<long, TValue>> keyValuePair in this._dictionary)
			{
				if (this.IsItemExpired(keyValuePair.Key))
				{
					list.Add(keyValuePair.Key);
				}
			}
			foreach (TKey tkey in list)
			{
				this._dictionary.Remove(tkey);
			}
		}

		// Token: 0x060004C4 RID: 1220 RVA: 0x0000F740 File Offset: 0x0000D940
		public void Clear()
		{
			this._dictionary.Clear();
		}

		// Token: 0x060004C5 RID: 1221 RVA: 0x0000F74D File Offset: 0x0000D94D
		public bool ContainsKey(TKey key)
		{
			return this._dictionary.ContainsKey(key) && !this.RemoveIfExpired(key);
		}

		// Token: 0x060004C6 RID: 1222 RVA: 0x0000F769 File Offset: 0x0000D969
		public bool Remove(TKey key)
		{
			this.RemoveIfExpired(key);
			return this._dictionary.Remove(key);
		}

		// Token: 0x060004C7 RID: 1223 RVA: 0x0000F77F File Offset: 0x0000D97F
		public bool TryGetValue(TKey key, out TValue value)
		{
			if (this.ContainsKey(key))
			{
				value = this._dictionary[key].Item2;
				return true;
			}
			value = default(TValue);
			return false;
		}

		// Token: 0x17000085 RID: 133
		public TValue this[TKey key]
		{
			get
			{
				this.RemoveIfExpired(key);
				return this._dictionary[key].Item2;
			}
			set
			{
				this._dictionary[key] = new ValueTuple<long, TValue>(this._stopwatch.ElapsedMilliseconds, value);
			}
		}

		// Token: 0x060004CA RID: 1226 RVA: 0x0000F7E8 File Offset: 0x0000D9E8
		public MBReadOnlyDictionary<TKey, TValue> AsReadOnlyDictionary()
		{
			this.PruneExpiredItems();
			Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
			foreach (KeyValuePair<TKey, ValueTuple<long, TValue>> keyValuePair in this._dictionary)
			{
				dictionary[keyValuePair.Key] = keyValuePair.Value.Item2;
			}
			return dictionary.GetReadOnlyDictionary<TKey, TValue>();
		}

		// Token: 0x04000171 RID: 369
		[TupleElementNames(new string[] { "Timestamp", "Value" })]
		private readonly Dictionary<TKey, ValueTuple<long, TValue>> _dictionary;

		// Token: 0x04000172 RID: 370
		private readonly Stopwatch _stopwatch;

		// Token: 0x04000173 RID: 371
		private readonly long _validMilliseconds;
	}
}
