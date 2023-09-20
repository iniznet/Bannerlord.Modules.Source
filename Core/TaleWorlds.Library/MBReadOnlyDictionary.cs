using System;
using System.Collections;
using System.Collections.Generic;

namespace TaleWorlds.Library
{
	// Token: 0x02000066 RID: 102
	[Serializable]
	public class MBReadOnlyDictionary<TKey, TValue> : ICollection, IEnumerable, IReadOnlyDictionary<TKey, TValue>, IEnumerable<KeyValuePair<TKey, TValue>>, IReadOnlyCollection<KeyValuePair<TKey, TValue>>
	{
		// Token: 0x06000385 RID: 901 RVA: 0x0000B256 File Offset: 0x00009456
		public MBReadOnlyDictionary(Dictionary<TKey, TValue> dictionary)
		{
			this._dictionary = dictionary;
		}

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x06000386 RID: 902 RVA: 0x0000B265 File Offset: 0x00009465
		public int Count
		{
			get
			{
				return this._dictionary.Count;
			}
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x06000387 RID: 903 RVA: 0x0000B272 File Offset: 0x00009472
		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x06000388 RID: 904 RVA: 0x0000B275 File Offset: 0x00009475
		public object SyncRoot
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06000389 RID: 905 RVA: 0x0000B278 File Offset: 0x00009478
		public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
		{
			return this._dictionary.GetEnumerator();
		}

		// Token: 0x0600038A RID: 906 RVA: 0x0000B285 File Offset: 0x00009485
		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
		{
			return this._dictionary.GetEnumerator();
		}

		// Token: 0x0600038B RID: 907 RVA: 0x0000B297 File Offset: 0x00009497
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this._dictionary.GetEnumerator();
		}

		// Token: 0x0600038C RID: 908 RVA: 0x0000B2A9 File Offset: 0x000094A9
		public bool ContainsKey(TKey key)
		{
			return this._dictionary.ContainsKey(key);
		}

		// Token: 0x0600038D RID: 909 RVA: 0x0000B2B7 File Offset: 0x000094B7
		public bool TryGetValue(TKey key, out TValue value)
		{
			return this._dictionary.TryGetValue(key, out value);
		}

		// Token: 0x17000056 RID: 86
		public TValue this[TKey key]
		{
			get
			{
				return this._dictionary[key];
			}
		}

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x0600038F RID: 911 RVA: 0x0000B2D4 File Offset: 0x000094D4
		public IEnumerable<TKey> Keys
		{
			get
			{
				return this._dictionary.Keys;
			}
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x06000390 RID: 912 RVA: 0x0000B2E1 File Offset: 0x000094E1
		public IEnumerable<TValue> Values
		{
			get
			{
				return this._dictionary.Values;
			}
		}

		// Token: 0x06000391 RID: 913 RVA: 0x0000B2F0 File Offset: 0x000094F0
		public void CopyTo(Array array, int index)
		{
			KeyValuePair<TKey, TValue>[] array2 = array as KeyValuePair<TKey, TValue>[];
			if (array2 != null)
			{
				((ICollection)this._dictionary).CopyTo(array2, index);
				return;
			}
			DictionaryEntry[] array3 = array as DictionaryEntry[];
			if (array3 != null)
			{
				using (Dictionary<TKey, TValue>.Enumerator enumerator = this._dictionary.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<TKey, TValue> keyValuePair = enumerator.Current;
						array3[index++] = new DictionaryEntry(keyValuePair.Key, keyValuePair.Value);
					}
					return;
				}
			}
			object[] array4 = array as object[];
			try
			{
				foreach (KeyValuePair<TKey, TValue> keyValuePair2 in this._dictionary)
				{
					array4[index++] = new KeyValuePair<TKey, TValue>(keyValuePair2.Key, keyValuePair2.Value);
				}
			}
			catch (ArrayTypeMismatchException)
			{
				Debug.FailedAssert("Invalid array type", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.Library\\MBReadOnlyDictionary.cs", "CopyTo", 96);
			}
		}

		// Token: 0x04000112 RID: 274
		private Dictionary<TKey, TValue> _dictionary;
	}
}
