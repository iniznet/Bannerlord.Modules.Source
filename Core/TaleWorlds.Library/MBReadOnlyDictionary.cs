using System;
using System.Collections;
using System.Collections.Generic;

namespace TaleWorlds.Library
{
	[Serializable]
	public class MBReadOnlyDictionary<TKey, TValue> : ICollection, IEnumerable, IReadOnlyDictionary<TKey, TValue>, IEnumerable<KeyValuePair<TKey, TValue>>, IReadOnlyCollection<KeyValuePair<TKey, TValue>>
	{
		public MBReadOnlyDictionary(Dictionary<TKey, TValue> dictionary)
		{
			this._dictionary = dictionary;
		}

		public int Count
		{
			get
			{
				return this._dictionary.Count;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public object SyncRoot
		{
			get
			{
				return null;
			}
		}

		public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
		{
			return this._dictionary.GetEnumerator();
		}

		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
		{
			return this._dictionary.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this._dictionary.GetEnumerator();
		}

		public bool ContainsKey(TKey key)
		{
			return this._dictionary.ContainsKey(key);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			return this._dictionary.TryGetValue(key, out value);
		}

		public TValue this[TKey key]
		{
			get
			{
				return this._dictionary[key];
			}
		}

		public IEnumerable<TKey> Keys
		{
			get
			{
				return this._dictionary.Keys;
			}
		}

		public IEnumerable<TValue> Values
		{
			get
			{
				return this._dictionary.Values;
			}
		}

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

		private Dictionary<TKey, TValue> _dictionary;
	}
}
