using System;
using System.Collections.Generic;

namespace TaleWorlds.Library
{
	public static class Extensions
	{
		public static MBList<T> ToMBList<T>(this T[] source)
		{
			MBList<T> mblist = new MBList<T>(source.Length);
			mblist.AddRange(source);
			return mblist;
		}

		public static MBList<T> ToMBList<T>(this List<T> source)
		{
			MBList<T> mblist = new MBList<T>(source.Count);
			mblist.AddRange(source);
			return mblist;
		}

		public static MBList<T> ToMBList<T>(this IEnumerable<T> source)
		{
			T[] array;
			if ((array = source as T[]) != null)
			{
				return array.ToMBList<T>();
			}
			List<T> list;
			if ((list = source as List<T>) != null)
			{
				return list.ToMBList<T>();
			}
			MBList<T> mblist = new MBList<T>();
			mblist.AddRange(source);
			return mblist;
		}

		public static void AppendList<T>(this List<T> list1, List<T> list2)
		{
			if (list1.Count + list2.Count > list1.Capacity)
			{
				list1.Capacity = list1.Count + list2.Count;
			}
			for (int i = 0; i < list2.Count; i++)
			{
				list1.Add(list2[i]);
			}
		}

		public static MBReadOnlyDictionary<TKey, TValue> GetReadOnlyDictionary<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
		{
			return new MBReadOnlyDictionary<TKey, TValue>(dictionary);
		}

		public static bool HasAnyFlag<T>(this T p1, T p2) where T : struct
		{
			return EnumHelper<T>.HasAnyFlag(p1, p2);
		}

		public static bool HasAllFlags<T>(this T p1, T p2) where T : struct
		{
			return EnumHelper<T>.HasAllFlags(p1, p2);
		}

		public static int GetDeterministicHashCode(this string text)
		{
			int num = 5381;
			for (int i = 0; i < text.Length; i++)
			{
				num = (num << 5) + num + (int)text[i];
			}
			return num;
		}

		public static int IndexOfMin<TSource>(this IReadOnlyList<TSource> self, Func<TSource, int> func)
		{
			int num = int.MaxValue;
			int num2 = -1;
			for (int i = 0; i < self.Count; i++)
			{
				int num3 = func(self[i]);
				if (num3 < num)
				{
					num = num3;
					num2 = i;
				}
			}
			return num2;
		}

		public static int IndexOfMin<TSource>(this MBReadOnlyList<TSource> self, Func<TSource, int> func)
		{
			int num = int.MaxValue;
			int num2 = -1;
			for (int i = 0; i < self.Count; i++)
			{
				int num3 = func(self[i]);
				if (num3 < num)
				{
					num = num3;
					num2 = i;
				}
			}
			return num2;
		}

		public static int IndexOfMax<TSource>(this IReadOnlyList<TSource> self, Func<TSource, int> func)
		{
			int num = int.MinValue;
			int num2 = -1;
			for (int i = 0; i < self.Count; i++)
			{
				int num3 = func(self[i]);
				if (num3 > num)
				{
					num = num3;
					num2 = i;
				}
			}
			return num2;
		}

		public static int IndexOfMax<TSource>(this MBReadOnlyList<TSource> self, Func<TSource, int> func)
		{
			int num = int.MinValue;
			int num2 = -1;
			for (int i = 0; i < self.Count; i++)
			{
				int num3 = func(self[i]);
				if (num3 > num)
				{
					num = num3;
					num2 = i;
				}
			}
			return num2;
		}

		public static int IndexOf<TValue>(this TValue[] source, TValue item)
		{
			for (int i = 0; i < source.Length; i++)
			{
				if (source[i].Equals(item))
				{
					return i;
				}
			}
			return -1;
		}

		public static int FindIndex<TValue>(this IReadOnlyList<TValue> source, Func<TValue, bool> predicate)
		{
			for (int i = 0; i < source.Count; i++)
			{
				if (predicate(source[i]))
				{
					return i;
				}
			}
			return -1;
		}

		public static int FindIndex<TValue>(this MBReadOnlyList<TValue> source, Func<TValue, bool> predicate)
		{
			for (int i = 0; i < source.Count; i++)
			{
				if (predicate(source[i]))
				{
					return i;
				}
			}
			return -1;
		}

		public static int FindLastIndex<TValue>(this IReadOnlyList<TValue> source, Func<TValue, bool> predicate)
		{
			for (int i = source.Count - 1; i >= 0; i--)
			{
				if (predicate(source[i]))
				{
					return i;
				}
			}
			return -1;
		}

		public static int FindLastIndex<TValue>(this MBReadOnlyList<TValue> source, Func<TValue, bool> predicate)
		{
			for (int i = source.Count - 1; i >= 0; i--)
			{
				if (predicate(source[i]))
				{
					return i;
				}
			}
			return -1;
		}

		public static void Randomize<T>(this IList<T> array)
		{
			Random random = new Random();
			int i = array.Count;
			while (i > 1)
			{
				i--;
				int num = random.Next(0, i + 1);
				T t = array[num];
				array[num] = array[i];
				array[i] = t;
			}
		}
	}
}
