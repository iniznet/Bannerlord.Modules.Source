using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace TaleWorlds.Library
{
	public class TimedDictionaryCache<TKey, TValue>
	{
		public TimedDictionaryCache(long validMilliseconds)
		{
			this._dictionary = new Dictionary<TKey, ValueTuple<long, TValue>>();
			this._stopwatch = new Stopwatch();
			this._stopwatch.Start();
			this._validMilliseconds = validMilliseconds;
		}

		public TimedDictionaryCache(TimeSpan validTimeSpan)
			: this((long)validTimeSpan.TotalMilliseconds)
		{
		}

		private bool IsItemExpired(TKey key)
		{
			return this._stopwatch.ElapsedMilliseconds - this._dictionary[key].Item1 >= this._validMilliseconds;
		}

		private bool RemoveIfExpired(TKey key)
		{
			if (this.IsItemExpired(key))
			{
				this._dictionary.Remove(key);
				return true;
			}
			return false;
		}

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

		public void Clear()
		{
			this._dictionary.Clear();
		}

		public bool ContainsKey(TKey key)
		{
			return this._dictionary.ContainsKey(key) && !this.RemoveIfExpired(key);
		}

		public bool Remove(TKey key)
		{
			this.RemoveIfExpired(key);
			return this._dictionary.Remove(key);
		}

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

		[TupleElementNames(new string[] { "Timestamp", "Value" })]
		private readonly Dictionary<TKey, ValueTuple<long, TValue>> _dictionary;

		private readonly Stopwatch _stopwatch;

		private readonly long _validMilliseconds;
	}
}
