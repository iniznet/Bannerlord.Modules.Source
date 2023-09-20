using System;
using System.Collections.Generic;

namespace TaleWorlds.Library
{
	// Token: 0x02000029 RID: 41
	public static class Extensions
	{
		// Token: 0x06000136 RID: 310 RVA: 0x000057FB File Offset: 0x000039FB
		public static MBList<T> ToMBList<T>(this T[] source)
		{
			MBList<T> mblist = new MBList<T>(source.Length);
			mblist.AddRange(source);
			return mblist;
		}

		// Token: 0x06000137 RID: 311 RVA: 0x0000580C File Offset: 0x00003A0C
		public static MBList<T> ToMBList<T>(this List<T> source)
		{
			MBList<T> mblist = new MBList<T>(source.Count);
			mblist.AddRange(source);
			return mblist;
		}

		// Token: 0x06000138 RID: 312 RVA: 0x00005820 File Offset: 0x00003A20
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

		// Token: 0x06000139 RID: 313 RVA: 0x0000585C File Offset: 0x00003A5C
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

		// Token: 0x0600013A RID: 314 RVA: 0x000058AF File Offset: 0x00003AAF
		public static MBReadOnlyDictionary<TKey, TValue> GetReadOnlyDictionary<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
		{
			return new MBReadOnlyDictionary<TKey, TValue>(dictionary);
		}

		// Token: 0x0600013B RID: 315 RVA: 0x000058B7 File Offset: 0x00003AB7
		public static bool HasAnyFlag<T>(this T p1, T p2) where T : struct
		{
			return EnumHelper<T>.HasAnyFlag(p1, p2);
		}

		// Token: 0x0600013C RID: 316 RVA: 0x000058C5 File Offset: 0x00003AC5
		public static bool HasAllFlags<T>(this T p1, T p2) where T : struct
		{
			return EnumHelper<T>.HasAllFlags(p1, p2);
		}

		// Token: 0x0600013D RID: 317 RVA: 0x000058D4 File Offset: 0x00003AD4
		public static int GetDeterministicHashCode(this string text)
		{
			int num = 5381;
			for (int i = 0; i < text.Length; i++)
			{
				num = (num << 5) + num + (int)text[i];
			}
			return num;
		}

		// Token: 0x0600013E RID: 318 RVA: 0x00005908 File Offset: 0x00003B08
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

		// Token: 0x0600013F RID: 319 RVA: 0x00005948 File Offset: 0x00003B48
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

		// Token: 0x06000140 RID: 320 RVA: 0x00005988 File Offset: 0x00003B88
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

		// Token: 0x06000141 RID: 321 RVA: 0x000059C8 File Offset: 0x00003BC8
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

		// Token: 0x06000142 RID: 322 RVA: 0x00005A08 File Offset: 0x00003C08
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

		// Token: 0x06000143 RID: 323 RVA: 0x00005A44 File Offset: 0x00003C44
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

		// Token: 0x06000144 RID: 324 RVA: 0x00005A74 File Offset: 0x00003C74
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

		// Token: 0x06000145 RID: 325 RVA: 0x00005AA4 File Offset: 0x00003CA4
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

		// Token: 0x06000146 RID: 326 RVA: 0x00005AD8 File Offset: 0x00003CD8
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

		// Token: 0x06000147 RID: 327 RVA: 0x00005B0C File Offset: 0x00003D0C
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
