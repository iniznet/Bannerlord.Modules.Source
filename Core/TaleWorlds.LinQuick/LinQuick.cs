using System;
using System.Collections.Generic;
using System.Linq;

namespace TaleWorlds.LinQuick
{
	// Token: 0x02000002 RID: 2
	public static class LinQuick
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002048 File Offset: 0x00000248
		public static bool AllQ<T>(this T[] source, Func<T, bool> predicate)
		{
			int num = source.Length;
			for (int i = 0; i < num; i++)
			{
				if (!predicate(source[i]))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002078 File Offset: 0x00000278
		public static bool AllQ<T>(this List<T> source, Func<T, bool> predicate)
		{
			int count = source.Count;
			for (int i = 0; i < count; i++)
			{
				if (!predicate(source[i]))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000020AC File Offset: 0x000002AC
		public static bool AllQ<T>(this IReadOnlyList<T> source, Func<T, bool> predicate)
		{
			List<T> list = source as List<T>;
			if (list != null)
			{
				return list.AllQ(predicate);
			}
			T[] array = source as T[];
			if (array != null)
			{
				return array.AllQ(predicate);
			}
			int count = source.Count;
			for (int i = 0; i < count; i++)
			{
				if (!predicate(source[i]))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002104 File Offset: 0x00000304
		public static bool AllQ<T>(this IEnumerable<T> source, Func<T, bool> predicate)
		{
			IReadOnlyList<T> readOnlyList;
			if ((readOnlyList = source as IReadOnlyList<T>) != null)
			{
				return readOnlyList.AllQ(predicate);
			}
			foreach (T t in source)
			{
				if (!predicate(t))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002168 File Offset: 0x00000368
		public static bool AnyQ<T>(this T[] source, Func<T, bool> predicate)
		{
			return source.FindIndexQ(predicate) != -1;
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002177 File Offset: 0x00000377
		public static bool AnyQ<T>(this List<T> source)
		{
			return source.Count > 0;
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002182 File Offset: 0x00000382
		public static bool AnyQ<T>(this List<T> source, Func<T, bool> predicate)
		{
			return source.FindIndexQ(predicate) != -1;
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002191 File Offset: 0x00000391
		public static bool AnyQ<T>(this IReadOnlyList<T> source)
		{
			return source.Count > 0;
		}

		// Token: 0x06000009 RID: 9 RVA: 0x0000219C File Offset: 0x0000039C
		public static bool AnyQ<T>(this IReadOnlyList<T> source, Func<T, bool> predicate)
		{
			return source.FindIndexQ(predicate) != -1;
		}

		// Token: 0x0600000A RID: 10 RVA: 0x000021AC File Offset: 0x000003AC
		public static bool AnyQ<T>(this IEnumerable<T> source)
		{
			IReadOnlyList<T> readOnlyList;
			if ((readOnlyList = source as IReadOnlyList<T>) != null)
			{
				return readOnlyList.AnyQ<T>();
			}
			return source.GetEnumerator().MoveNext();
		}

		// Token: 0x0600000B RID: 11 RVA: 0x000021D8 File Offset: 0x000003D8
		public static bool AnyQ<T>(this IEnumerable<T> source, Func<T, bool> predicate)
		{
			IReadOnlyList<T> readOnlyList;
			if ((readOnlyList = source as IReadOnlyList<T>) != null)
			{
				return readOnlyList.AnyQ(predicate);
			}
			foreach (T t in source)
			{
				if (predicate(t))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600000C RID: 12 RVA: 0x0000223C File Offset: 0x0000043C
		public static float AverageQ(this float[] source)
		{
			if (source.Length == 0)
			{
				throw Error.NoElements();
			}
			float num = 0f;
			int num2 = source.Length;
			for (int i = 0; i < num2; i++)
			{
				num += source[i];
			}
			return num / (float)source.Length;
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002278 File Offset: 0x00000478
		public static float AverageQ(this IEnumerable<float> source)
		{
			float num = 0f;
			int num2 = 0;
			foreach (float num3 in source)
			{
				num += num3;
				num2++;
			}
			if (num2 == 0)
			{
				throw Error.NoElements();
			}
			return num / (float)num2;
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000022D8 File Offset: 0x000004D8
		public static float AverageQ<T>(this T[] source, Func<T, float> selector)
		{
			if (source.Length == 0)
			{
				throw Error.NoElements();
			}
			float num = 0f;
			int num2 = source.Length;
			for (int i = 0; i < num2; i++)
			{
				num += selector(source[i]);
			}
			return num / (float)source.Length;
		}

		// Token: 0x0600000F RID: 15 RVA: 0x0000231C File Offset: 0x0000051C
		public static float AverageQ<T>(this List<T> source, Func<T, float> selector)
		{
			int count = source.Count;
			if (count == 0)
			{
				throw Error.NoElements();
			}
			float num = 0f;
			for (int i = 0; i < count; i++)
			{
				num += selector(source[i]);
			}
			return num / (float)count;
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002360 File Offset: 0x00000560
		public static float AverageQ<T>(this IReadOnlyList<T> source, Func<T, float> selector)
		{
			List<T> list;
			if ((list = source as List<T>) != null)
			{
				return list.AverageQ(selector);
			}
			T[] array;
			if ((array = source as T[]) != null)
			{
				return array.AverageQ(selector);
			}
			int count = source.Count;
			if (count == 0)
			{
				throw Error.NoElements();
			}
			float num = 0f;
			for (int i = 0; i < count; i++)
			{
				num += selector(source[i]);
			}
			return num / (float)count;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000023CC File Offset: 0x000005CC
		public static float AverageQ<T>(this IEnumerable<T> source, Func<T, float> selector)
		{
			IReadOnlyList<T> readOnlyList;
			if ((readOnlyList = source as IReadOnlyList<T>) != null)
			{
				return readOnlyList.AverageQ(selector);
			}
			float num = 0f;
			int num2 = 0;
			foreach (T t in source)
			{
				float num3 = selector(t);
				num += num3;
				num2++;
			}
			if (num2 == 0)
			{
				throw Error.NoElements();
			}
			return num / (float)num2;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002448 File Offset: 0x00000648
		public static bool ContainsQ<T>(this T[] source, T value)
		{
			return source.FindIndexQ(value) != -1;
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002457 File Offset: 0x00000657
		public static bool ContainsQ<T>(this List<T> source, T value)
		{
			return source.FindIndexQ(value) != -1;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002466 File Offset: 0x00000666
		public static bool ContainsQ<T>(this IReadOnlyList<T> source, T value)
		{
			return source.FindIndexQ(value) != -1;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002475 File Offset: 0x00000675
		public static bool ContainsQ<T>(this IEnumerable<T> source, T value)
		{
			return source.FindIndexQ(value) != -1;
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00002484 File Offset: 0x00000684
		public static bool ContainsQ<T>(this T[] source, Func<T, bool> predicate)
		{
			return source.FindIndexQ(predicate) != -1;
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002493 File Offset: 0x00000693
		public static bool ContainsQ<T>(this List<T> source, Func<T, bool> predicate)
		{
			return source.FindIndexQ(predicate) != -1;
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000024A2 File Offset: 0x000006A2
		public static bool ContainsQ<T>(this IReadOnlyList<T> source, Func<T, bool> predicate)
		{
			return source.FindIndexQ(predicate) != -1;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x000024B1 File Offset: 0x000006B1
		public static bool ContainsQ<T>(this IEnumerable<T> source, Func<T, bool> predicate)
		{
			return source.FindIndexQ(predicate) != -1;
		}

		// Token: 0x0600001A RID: 26 RVA: 0x000024C0 File Offset: 0x000006C0
		public static int CountQ<T>(this T[] source, T value)
		{
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			int num = 0;
			int num2 = source.Length;
			for (int i = 0; i < num2; i++)
			{
				T t = source[i];
				if (@default.Equals(t, value))
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002500 File Offset: 0x00000700
		public static int CountQ<T>(this List<T> source, T value)
		{
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			int num = 0;
			int count = source.Count;
			for (int i = 0; i < count; i++)
			{
				if (@default.Equals(source[i], value))
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002540 File Offset: 0x00000740
		public static int CountQ<T>(this IReadOnlyList<T> source, T value)
		{
			List<T> list;
			if ((list = source as List<T>) != null)
			{
				return list.CountQ(value);
			}
			T[] array = source as T[];
			if (array != null)
			{
				return array.CountQ(value);
			}
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			int num = 0;
			int count = source.Count;
			for (int i = 0; i < count; i++)
			{
				if (@default.Equals(source[i], value))
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x0600001D RID: 29 RVA: 0x000025A8 File Offset: 0x000007A8
		public static int CountQ<T>(this T[] source, Func<T, bool> predicate)
		{
			int num = 0;
			int num2 = source.Length;
			for (int i = 0; i < num2; i++)
			{
				if (predicate(source[i]))
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x0600001E RID: 30 RVA: 0x000025DC File Offset: 0x000007DC
		public static int CountQ<T>(this List<T> source, Func<T, bool> predicate)
		{
			int num = 0;
			int count = source.Count;
			for (int i = 0; i < count; i++)
			{
				if (predicate(source[i]))
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002614 File Offset: 0x00000814
		public static int CountQ<T>(this IReadOnlyList<T> source, Func<T, bool> predicate)
		{
			List<T> list;
			if ((list = source as List<T>) != null)
			{
				return list.CountQ(predicate);
			}
			int num = 0;
			int count = source.Count;
			for (int i = 0; i < count; i++)
			{
				if (predicate(source[i]))
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x06000020 RID: 32 RVA: 0x0000265C File Offset: 0x0000085C
		public static int CountQ<T>(this IEnumerable<T> source, Func<T, bool> predicate)
		{
			IReadOnlyList<T> readOnlyList;
			if ((readOnlyList = source as IReadOnlyList<T>) != null)
			{
				return readOnlyList.CountQ(predicate);
			}
			int num = 0;
			foreach (T t in source)
			{
				if (predicate(t))
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x06000021 RID: 33 RVA: 0x000026C0 File Offset: 0x000008C0
		public static int CountQ<T>(this IEnumerable<T> source)
		{
			IReadOnlyList<T> readOnlyList;
			if ((readOnlyList = source as IReadOnlyList<T>) != null)
			{
				return readOnlyList.Count;
			}
			int num = 0;
			using (IEnumerator<T> enumerator = source.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002714 File Offset: 0x00000914
		public static int FindIndexQ<T>(this T[] source, T value)
		{
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			int num = -1;
			int num2 = source.Length;
			for (int i = 0; i < num2; i++)
			{
				if (@default.Equals(source[i], value))
				{
					num = i;
					break;
				}
			}
			return num;
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002750 File Offset: 0x00000950
		public static int FindIndexQ<T>(this List<T> source, T value)
		{
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			int num = -1;
			int count = source.Count;
			for (int i = 0; i < count; i++)
			{
				if (@default.Equals(source[i], value))
				{
					num = i;
					break;
				}
			}
			return num;
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002790 File Offset: 0x00000990
		public static int FindIndexQ<T>(this IReadOnlyList<T> source, T value)
		{
			List<T> list = source as List<T>;
			if (list != null)
			{
				return list.FindIndexQ(value);
			}
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			int num = -1;
			int count = source.Count;
			for (int i = 0; i < count; i++)
			{
				if (@default.Equals(source[i], value))
				{
					num = i;
					break;
				}
			}
			return num;
		}

		// Token: 0x06000025 RID: 37 RVA: 0x000027E8 File Offset: 0x000009E8
		public static int FindIndexQ<T>(this IEnumerable<T> source, T value)
		{
			IReadOnlyList<T> readOnlyList;
			if ((readOnlyList = source as IReadOnlyList<T>) != null)
			{
				return readOnlyList.FindIndexQ(value);
			}
			if (value != null && value is IComparable)
			{
				return source.FindIndexComparableQ(value);
			}
			return source.FindIndexNonComparableQ(value);
		}

		// Token: 0x06000026 RID: 38 RVA: 0x0000282C File Offset: 0x00000A2C
		public static int FindIndexQ<T>(this T[] source, Func<T, bool> predicate)
		{
			int num = -1;
			int num2 = source.Length;
			for (int i = 0; i < num2; i++)
			{
				if (predicate(source[i]))
				{
					num = i;
					break;
				}
			}
			return num;
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002860 File Offset: 0x00000A60
		public static int FindIndexQ<T>(this List<T> source, Func<T, bool> predicate)
		{
			int num = -1;
			int count = source.Count;
			for (int i = 0; i < count; i++)
			{
				if (predicate(source[i]))
				{
					num = i;
					break;
				}
			}
			return num;
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002898 File Offset: 0x00000A98
		public static int FindIndexQ<T>(this IReadOnlyList<T> source, Func<T, bool> predicate)
		{
			List<T> list = source as List<T>;
			if (list != null)
			{
				return list.FindIndexQ(predicate);
			}
			T[] array = source as T[];
			if (array != null)
			{
				return array.FindIndexQ(predicate);
			}
			int num = -1;
			int count = source.Count;
			for (int i = 0; i < count; i++)
			{
				if (predicate(source[i]))
				{
					num = i;
					break;
				}
			}
			return num;
		}

		// Token: 0x06000029 RID: 41 RVA: 0x000028F8 File Offset: 0x00000AF8
		public static int FindIndexQ<T>(this IEnumerable<T> source, Func<T, bool> predicate)
		{
			IReadOnlyList<T> readOnlyList;
			if ((readOnlyList = source as IReadOnlyList<T>) != null)
			{
				return readOnlyList.FindIndexQ(predicate);
			}
			using (IEnumerator<T> enumerator = source.GetEnumerator())
			{
				int num = 0;
				while (enumerator.MoveNext())
				{
					T t = enumerator.Current;
					if (predicate(t))
					{
						return num;
					}
					num++;
				}
			}
			return -1;
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002964 File Offset: 0x00000B64
		private static int FindIndexComparableQ<T>(this IEnumerable<T> source, T value)
		{
			Comparer<T> @default = Comparer<T>.Default;
			using (IEnumerator<T> enumerator = source.GetEnumerator())
			{
				int num = 0;
				while (enumerator.MoveNext())
				{
					T t = enumerator.Current;
					if (@default.Compare(t, value) == 0)
					{
						return num;
					}
					num++;
				}
			}
			return -1;
		}

		// Token: 0x0600002B RID: 43 RVA: 0x000029C4 File Offset: 0x00000BC4
		private static int FindIndexNonComparableQ<T>(this IEnumerable<T> source, T value)
		{
			using (IEnumerator<T> enumerator = source.GetEnumerator())
			{
				int num = 0;
				while (enumerator.MoveNext())
				{
					T t = enumerator.Current;
					if (t.Equals(value))
					{
						return num;
					}
					num++;
				}
			}
			return -1;
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002A28 File Offset: 0x00000C28
		public static T FirstOrDefaultQ<T>(this T[] source, Func<T, bool> predicate)
		{
			int num = source.FindIndexQ(predicate);
			if (num == -1)
			{
				return default(T);
			}
			return source[num];
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002A54 File Offset: 0x00000C54
		public static T FirstOrDefaultQ<T>(this List<T> source, Func<T, bool> predicate)
		{
			int num = source.FindIndexQ(predicate);
			if (num == -1)
			{
				return default(T);
			}
			return source[num];
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00002A80 File Offset: 0x00000C80
		public static T FirstOrDefaultQ<T>(this IReadOnlyList<T> source, Func<T, bool> predicate)
		{
			int num = source.FindIndexQ(predicate);
			if (num == -1)
			{
				return default(T);
			}
			return source[num];
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00002AAC File Offset: 0x00000CAC
		public static T FirstOrDefaultQ<T>(this IEnumerable<T> source, Func<T, bool> predicate)
		{
			IReadOnlyList<T> readOnlyList;
			if ((readOnlyList = source as IReadOnlyList<T>) != null)
			{
				return readOnlyList.FirstOrDefaultQ(predicate);
			}
			foreach (T t in source)
			{
				if (predicate(t))
				{
					return t;
				}
			}
			return default(T);
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00002B18 File Offset: 0x00000D18
		public static int MaxQ(this int[] source)
		{
			int num = source[0];
			for (int i = 0; i < source.Length; i++)
			{
				if (source[i] > num)
				{
					num = source[i];
				}
			}
			return num;
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00002B44 File Offset: 0x00000D44
		public static int MaxQ(this List<int> source)
		{
			int num = source[0];
			int count = source.Count;
			for (int i = 0; i < count; i++)
			{
				if (source[i] > num)
				{
					num = source[i];
				}
			}
			return num;
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00002B80 File Offset: 0x00000D80
		public static T MaxQ<T>(this T[] source) where T : IComparable<T>
		{
			if (source.Length == 0)
			{
				throw Error.NoElements();
			}
			T t = source[0];
			for (int i = 0; i < source.Length; i++)
			{
				if (source[i].CompareTo(t) > 0)
				{
					t = source[i];
				}
			}
			return t;
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00002BD0 File Offset: 0x00000DD0
		public static T MaxQ<T>(this List<T> source) where T : IComparable<T>
		{
			if (source.Count == 0)
			{
				throw Error.NoElements();
			}
			T t = source[0];
			int count = source.Count;
			for (int i = 0; i < count; i++)
			{
				T t2 = source[i];
				if (t2.CompareTo(t) > 0)
				{
					t = source[i];
				}
			}
			return t;
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00002C28 File Offset: 0x00000E28
		public static int MaxQ(this IReadOnlyList<int> source)
		{
			if (source.Count == 0)
			{
				throw Error.NoElements();
			}
			int num = source[0];
			List<int> list = source as List<int>;
			if (list != null)
			{
				return list.MaxQ();
			}
			int count = source.Count;
			for (int i = 0; i < count; i++)
			{
				if (source[i] > num)
				{
					num = source[i];
				}
			}
			return num;
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00002C84 File Offset: 0x00000E84
		public static T MaxQ<T>(this IReadOnlyList<T> source) where T : IComparable<T>
		{
			if (source.Count == 0)
			{
				throw Error.NoElements();
			}
			List<T> list = source as List<T>;
			if (list != null)
			{
				return list.MaxQ<T>();
			}
			T[] array = source as T[];
			if (array != null)
			{
				return array.MaxQ<T>();
			}
			T t = source[0];
			for (int i = 0; i < source.Count; i++)
			{
				T t2 = source[i];
				if (t2.CompareTo(t) > 0)
				{
					t = source[i];
				}
			}
			return t;
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00002D00 File Offset: 0x00000F00
		public static float MaxQ<T>(this T[] source, Func<T, float> selector)
		{
			if (source.Length == 0)
			{
				throw Error.NoElements();
			}
			float num = selector(source[0]);
			for (int i = 0; i < source.Length; i++)
			{
				float num2 = selector(source[i]);
				if (num2 > num)
				{
					num = num2;
				}
			}
			return num;
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00002D48 File Offset: 0x00000F48
		public static int MaxQ<T>(this T[] source, Func<T, int> selector)
		{
			if (source.Length == 0)
			{
				throw Error.NoElements();
			}
			int num = selector(source[0]);
			for (int i = 0; i < source.Length; i++)
			{
				int num2 = selector(source[i]);
				if (num2 > num)
				{
					num = num2;
				}
			}
			return num;
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00002D90 File Offset: 0x00000F90
		public static float MaxQ<T>(this List<T> source, Func<T, float> selector)
		{
			if (source.Count == 0)
			{
				throw Error.NoElements();
			}
			float num = selector(source[0]);
			for (int i = 0; i < source.Count; i++)
			{
				float num2 = selector(source[i]);
				if (num2 > num)
				{
					num = num2;
				}
			}
			return num;
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00002DE0 File Offset: 0x00000FE0
		public static int MaxQ<T>(this List<T> source, Func<T, int> selector)
		{
			if (source.Count == 0)
			{
				throw Error.NoElements();
			}
			int num = selector(source[0]);
			for (int i = 0; i < source.Count; i++)
			{
				int num2 = selector(source[i]);
				if (num2 > num)
				{
					num = num2;
				}
			}
			return num;
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00002E30 File Offset: 0x00001030
		public static float MaxQ<T>(this IReadOnlyList<T> source, Func<T, float> selector)
		{
			List<T> list;
			if ((list = source as List<T>) != null)
			{
				return list.MaxQ(selector);
			}
			T[] array;
			if ((array = source as T[]) != null)
			{
				return array.MaxQ(selector);
			}
			if (source.Count == 0)
			{
				throw Error.NoElements();
			}
			float num = selector(source[0]);
			for (int i = 0; i < source.Count; i++)
			{
				float num2 = selector(source[i]);
				if (num2 > num)
				{
					num = num2;
				}
			}
			return num;
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00002EA8 File Offset: 0x000010A8
		public static int MaxQ<T>(this IReadOnlyList<T> source, Func<T, int> selector)
		{
			List<T> list;
			if ((list = source as List<T>) != null)
			{
				return list.MaxQ(selector);
			}
			T[] array;
			if ((array = source as T[]) != null)
			{
				return array.MaxQ(selector);
			}
			if (source.Count == 0)
			{
				throw Error.NoElements();
			}
			int num = selector(source[0]);
			for (int i = 0; i < source.Count; i++)
			{
				int num2 = selector(source[i]);
				if (num2 > num)
				{
					num = num2;
				}
			}
			return num;
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00002F20 File Offset: 0x00001120
		public static float MaxQ<T>(this IEnumerable<T> source, Func<T, float> selector)
		{
			IReadOnlyList<T> readOnlyList;
			if ((readOnlyList = source as IReadOnlyList<T>) != null)
			{
				return readOnlyList.MaxQ(selector);
			}
			float num = 0f;
			bool flag = false;
			foreach (T t in source)
			{
				float num2 = selector(t);
				if (!flag)
				{
					num = num2;
					flag = true;
				}
				else if (num2 > num)
				{
					num = num2;
				}
			}
			if (!flag)
			{
				Error.NoElements();
			}
			return num;
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00002FA0 File Offset: 0x000011A0
		public static int MaxQ<T>(this IEnumerable<T> source, Func<T, int> selector)
		{
			IReadOnlyList<T> readOnlyList;
			if ((readOnlyList = source as IReadOnlyList<T>) != null)
			{
				return readOnlyList.MaxQ(selector);
			}
			int num = 0;
			bool flag = false;
			foreach (T t in source)
			{
				int num2 = selector(t);
				if (!flag)
				{
					num = num2;
					flag = true;
				}
				else if (num2 > num)
				{
					num = num2;
				}
			}
			if (!flag)
			{
				Error.NoElements();
			}
			return num;
		}

		// Token: 0x0600003E RID: 62 RVA: 0x0000301C File Offset: 0x0000121C
		public static ValueTuple<T, T, T> MaxElements3<T>(this IEnumerable<T> collection, Func<T, float> func)
		{
			float num = float.MinValue;
			float num2 = float.MinValue;
			float num3 = float.MinValue;
			T t = default(T);
			T t2 = default(T);
			T t3 = default(T);
			foreach (T t4 in collection)
			{
				float num4 = func(t4);
				if (num4 > num3)
				{
					if (num4 > num2)
					{
						num3 = num2;
						t3 = t2;
						if (num4 > num)
						{
							num2 = num;
							t2 = t;
							num = num4;
							t = t4;
						}
						else
						{
							num2 = num4;
							t2 = t4;
						}
					}
					else
					{
						num3 = num4;
						t3 = t4;
					}
				}
			}
			return new ValueTuple<T, T, T>(t, t2, t3);
		}

		// Token: 0x0600003F RID: 63 RVA: 0x000030D4 File Offset: 0x000012D4
		public static IOrderedEnumerable<T> OrderByQ<T, S>(this IEnumerable<T> source, Func<T, S> selector)
		{
			return source.OrderBy(selector);
		}

		// Token: 0x06000040 RID: 64 RVA: 0x000030E0 File Offset: 0x000012E0
		public static T[] OrderByQ<T, TKey>(this T[] source, Func<T, TKey> selector)
		{
			Comparer<TKey> @default = Comparer<TKey>.Default;
			TKey[] array = new TKey[source.Length];
			for (int i = 0; i < source.Length; i++)
			{
				array[i] = selector(source[i]);
			}
			T[] array2 = new T[source.Length];
			for (int j = 0; j < source.Length; j++)
			{
				array2[j] = source[j];
			}
			Array.Sort<TKey, T>(array, array2, @default);
			return array2;
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00003154 File Offset: 0x00001354
		public static T[] OrderByQ<T, TKey>(this List<T> source, Func<T, TKey> selector)
		{
			Comparer<TKey> @default = Comparer<TKey>.Default;
			int count = source.Count;
			TKey[] array = new TKey[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = selector(source[i]);
			}
			T[] array2 = new T[count];
			for (int j = 0; j < count; j++)
			{
				array2[j] = source[j];
			}
			Array.Sort<TKey, T>(array, array2, @default);
			return array2;
		}

		// Token: 0x06000042 RID: 66 RVA: 0x000031CC File Offset: 0x000013CC
		public static T[] OrderByQ<T, TKey>(this IReadOnlyList<T> source, Func<T, TKey> selector)
		{
			Comparer<TKey> @default = Comparer<TKey>.Default;
			int count = source.Count;
			TKey[] array = new TKey[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = selector(source[i]);
			}
			T[] array2 = new T[count];
			for (int j = 0; j < count; j++)
			{
				array2[j] = source[j];
			}
			Array.Sort<TKey, T>(array, array2, @default);
			return array2;
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00003243 File Offset: 0x00001443
		public static IEnumerable<R> SelectQ<T, R>(this T[] source, Func<T, R> selector)
		{
			int len = source.Length;
			int num;
			for (int i = 0; i < len; i = num)
			{
				R r = selector(source[i]);
				yield return r;
				num = i + 1;
			}
			yield break;
		}

		// Token: 0x06000044 RID: 68 RVA: 0x0000325A File Offset: 0x0000145A
		public static IEnumerable<R> SelectQ<T, R>(this List<T> source, Func<T, R> selector)
		{
			int len = source.Count;
			int num;
			for (int i = 0; i < len; i = num)
			{
				R r = selector(source[i]);
				yield return r;
				num = i + 1;
			}
			yield break;
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00003271 File Offset: 0x00001471
		public static IEnumerable<R> SelectQ<T, R>(this IReadOnlyList<T> source, Func<T, R> selector)
		{
			int len = source.Count;
			int num;
			for (int i = 0; i < len; i = num)
			{
				R r = selector(source[i]);
				yield return r;
				num = i + 1;
			}
			yield break;
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00003288 File Offset: 0x00001488
		public static IEnumerable<R> SelectQ<T, R>(this IEnumerable<T> source, Func<T, R> selector)
		{
			foreach (T t in source)
			{
				R r = selector(t);
				yield return r;
			}
			IEnumerator<T> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x06000047 RID: 71 RVA: 0x000032A0 File Offset: 0x000014A0
		public static int SumQ<T>(this T[] source, Func<T, int> func)
		{
			int num = 0;
			int num2 = source.Length;
			for (int i = 0; i < num2; i++)
			{
				num += func(source[i]);
			}
			return num;
		}

		// Token: 0x06000048 RID: 72 RVA: 0x000032D0 File Offset: 0x000014D0
		public static float SumQ<T>(this T[] source, Func<T, float> func)
		{
			float num = 0f;
			int num2 = source.Length;
			for (int i = 0; i < num2; i++)
			{
				num += func(source[i]);
			}
			return num;
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00003304 File Offset: 0x00001504
		public static int SumQ<T>(this List<T> source, Func<T, int> func)
		{
			int num = 0;
			int count = source.Count;
			for (int i = 0; i < count; i++)
			{
				num += func(source[i]);
			}
			return num;
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00003338 File Offset: 0x00001538
		public static float SumQ<T>(this List<T> source, Func<T, float> func)
		{
			float num = 0f;
			int count = source.Count;
			for (int i = 0; i < count; i++)
			{
				num += func(source[i]);
			}
			return num;
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00003370 File Offset: 0x00001570
		public static int SumQ<T>(this IReadOnlyList<T> source, Func<T, int> func)
		{
			List<T> list = source as List<T>;
			if (list != null)
			{
				return list.SumQ(func);
			}
			int num = 0;
			int count = source.Count;
			for (int i = 0; i < count; i++)
			{
				num += func(source[i]);
			}
			return num;
		}

		// Token: 0x0600004C RID: 76 RVA: 0x000033B8 File Offset: 0x000015B8
		public static float SumQ<T>(this IReadOnlyList<T> source, Func<T, float> func)
		{
			List<T> list = source as List<T>;
			if (list != null)
			{
				return list.SumQ(func);
			}
			float num = 0f;
			int count = source.Count;
			for (int i = 0; i < count; i++)
			{
				num += func(source[i]);
			}
			return num;
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00003404 File Offset: 0x00001604
		public static float SumQ<T>(this IEnumerable<T> source, Func<T, float> func)
		{
			IReadOnlyList<T> readOnlyList;
			if ((readOnlyList = source as IReadOnlyList<T>) != null)
			{
				return readOnlyList.SumQ(func);
			}
			float num = 0f;
			foreach (T t in source)
			{
				float num2 = func(t);
				num += num2;
			}
			return num;
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00003468 File Offset: 0x00001668
		public static int SumQ<T>(this IEnumerable<T> source, Func<T, int> func)
		{
			IReadOnlyList<T> readOnlyList;
			if ((readOnlyList = source as IReadOnlyList<T>) != null)
			{
				return readOnlyList.SumQ(func);
			}
			int num = 0;
			foreach (T t in source)
			{
				int num2 = func(t);
				num += num2;
			}
			return num;
		}

		// Token: 0x0600004F RID: 79 RVA: 0x000034C8 File Offset: 0x000016C8
		public static T[] ToArrayQ<T>(this T[] source)
		{
			int num = source.Length;
			T[] array = new T[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = source[i];
			}
			return array;
		}

		// Token: 0x06000050 RID: 80 RVA: 0x000034FB File Offset: 0x000016FB
		public static T[] ToArrayQ<T>(this List<T> source)
		{
			return source.ToArray();
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00003504 File Offset: 0x00001704
		public static T[] ToArrayQ<T>(this IReadOnlyList<T> source)
		{
			List<T> list;
			if ((list = source as List<T>) != null)
			{
				return list.ToArray();
			}
			T[] array;
			if ((array = source as T[]) != null)
			{
				return array.ToArrayQ<T>();
			}
			int count = source.Count;
			T[] array2 = new T[count];
			for (int i = 0; i < count; i++)
			{
				array2[i] = source[i];
			}
			return array2;
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00003564 File Offset: 0x00001764
		public static T[] ToArrayQ<T>(this IEnumerable<T> source)
		{
			IReadOnlyList<T> readOnlyList;
			if ((readOnlyList = source as IReadOnlyList<T>) != null)
			{
				return readOnlyList.ToArrayQ<T>();
			}
			List<T> list = new List<T>();
			foreach (T t in source)
			{
				list.Add(t);
			}
			return list.ToArray();
		}

		// Token: 0x06000053 RID: 83 RVA: 0x000035CC File Offset: 0x000017CC
		public static List<T> ToListQ<T>(this T[] source)
		{
			List<T> list = new List<T>(source.Length);
			list.AddRange(source);
			return list;
		}

		// Token: 0x06000054 RID: 84 RVA: 0x000035DD File Offset: 0x000017DD
		public static List<T> ToListQ<T>(this List<T> source)
		{
			List<T> list = new List<T>(source.Count);
			list.AddRange(source);
			return list;
		}

		// Token: 0x06000055 RID: 85 RVA: 0x000035F4 File Offset: 0x000017F4
		public static List<T> ToListQ<T>(this IReadOnlyList<T> source)
		{
			List<T> list;
			if ((list = source as List<T>) != null)
			{
				return list.ToListQ<T>();
			}
			T[] array;
			if ((array = source as T[]) != null)
			{
				return array.ToListQ<T>();
			}
			List<T> list2 = new List<T>(source.Count);
			list2.AddRange(source);
			return list2;
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00003638 File Offset: 0x00001838
		public static List<T> ToListQ<T>(this IEnumerable<T> source)
		{
			IReadOnlyList<T> readOnlyList;
			if ((readOnlyList = source as IReadOnlyList<T>) != null)
			{
				return readOnlyList.ToListQ<T>();
			}
			List<T> list = new List<T>();
			list.AddRange(source);
			return list;
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00003662 File Offset: 0x00001862
		public static IEnumerable<T> WhereQ<T>(this T[] source, Func<T, bool> predicate)
		{
			int length = source.Length;
			int num;
			for (int i = 0; i < length; i = num)
			{
				T t = source[i];
				if (predicate(t))
				{
					yield return t;
				}
				num = i + 1;
			}
			yield break;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00003679 File Offset: 0x00001879
		public static IEnumerable<T> WhereQ<T>(this List<T> source, Func<T, bool> predicate)
		{
			int length = source.Count;
			int num;
			for (int i = 0; i < length; i = num)
			{
				T t = source[i];
				if (predicate(t))
				{
					yield return t;
				}
				num = i + 1;
			}
			yield break;
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00003690 File Offset: 0x00001890
		public static IEnumerable<T> WhereQ<T>(this IReadOnlyList<T> source, Func<T, bool> predicate)
		{
			List<T> list;
			if ((list = source as List<T>) != null)
			{
				return list.WhereQ(predicate);
			}
			return LinQuick.WhereQImp<T>(source, predicate);
		}

		// Token: 0x0600005A RID: 90 RVA: 0x000036B6 File Offset: 0x000018B6
		private static IEnumerable<T> WhereQImp<T>(IReadOnlyList<T> source, Func<T, bool> predicate)
		{
			int length = source.Count;
			int num;
			for (int i = 0; i < length; i = num)
			{
				T t = source[i];
				if (predicate(t))
				{
					yield return t;
				}
				num = i + 1;
			}
			yield break;
		}

		// Token: 0x0600005B RID: 91 RVA: 0x000036D0 File Offset: 0x000018D0
		public static IEnumerable<T> WhereQ<T>(this IEnumerable<T> source, Func<T, bool> predicate)
		{
			IReadOnlyList<T> readOnlyList;
			if ((readOnlyList = source as IReadOnlyList<T>) != null)
			{
				return readOnlyList.WhereQ(predicate);
			}
			return LinQuick.WhereQImp<T>(source, predicate);
		}

		// Token: 0x0600005C RID: 92 RVA: 0x000036F6 File Offset: 0x000018F6
		private static IEnumerable<T> WhereQImp<T>(IEnumerable<T> source, Func<T, bool> predicate)
		{
			foreach (T t in source)
			{
				if (predicate(t))
				{
					yield return t;
				}
			}
			IEnumerator<T> enumerator = null;
			yield break;
			yield break;
		}
	}
}
