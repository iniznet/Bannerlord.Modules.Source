using System;
using System.Collections.Generic;
using System.Linq;

namespace TaleWorlds.LinQuick
{
	public static class LinQuick
	{
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

		public static bool AnyQ<T>(this T[] source, Func<T, bool> predicate)
		{
			return source.FindIndexQ(predicate) != -1;
		}

		public static bool AnyQ<T>(this List<T> source)
		{
			return source.Count > 0;
		}

		public static bool AnyQ<T>(this List<T> source, Func<T, bool> predicate)
		{
			return source.FindIndexQ(predicate) != -1;
		}

		public static bool AnyQ<T>(this IReadOnlyList<T> source)
		{
			return source.Count > 0;
		}

		public static bool AnyQ<T>(this IReadOnlyList<T> source, Func<T, bool> predicate)
		{
			return source.FindIndexQ(predicate) != -1;
		}

		public static bool AnyQ<T>(this IEnumerable<T> source)
		{
			IReadOnlyList<T> readOnlyList;
			if ((readOnlyList = source as IReadOnlyList<T>) != null)
			{
				return readOnlyList.AnyQ<T>();
			}
			return source.GetEnumerator().MoveNext();
		}

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

		public static bool ContainsQ<T>(this T[] source, T value)
		{
			return source.FindIndexQ(value) != -1;
		}

		public static bool ContainsQ<T>(this List<T> source, T value)
		{
			return source.FindIndexQ(value) != -1;
		}

		public static bool ContainsQ<T>(this IReadOnlyList<T> source, T value)
		{
			return source.FindIndexQ(value) != -1;
		}

		public static bool ContainsQ<T>(this IEnumerable<T> source, T value)
		{
			return source.FindIndexQ(value) != -1;
		}

		public static bool ContainsQ<T>(this T[] source, Func<T, bool> predicate)
		{
			return source.FindIndexQ(predicate) != -1;
		}

		public static bool ContainsQ<T>(this List<T> source, Func<T, bool> predicate)
		{
			return source.FindIndexQ(predicate) != -1;
		}

		public static bool ContainsQ<T>(this IReadOnlyList<T> source, Func<T, bool> predicate)
		{
			return source.FindIndexQ(predicate) != -1;
		}

		public static bool ContainsQ<T>(this IEnumerable<T> source, Func<T, bool> predicate)
		{
			return source.FindIndexQ(predicate) != -1;
		}

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

		public static T FirstOrDefaultQ<T>(this T[] source, Func<T, bool> predicate)
		{
			int num = source.FindIndexQ(predicate);
			if (num == -1)
			{
				return default(T);
			}
			return source[num];
		}

		public static T FirstOrDefaultQ<T>(this List<T> source, Func<T, bool> predicate)
		{
			int num = source.FindIndexQ(predicate);
			if (num == -1)
			{
				return default(T);
			}
			return source[num];
		}

		public static T FirstOrDefaultQ<T>(this IReadOnlyList<T> source, Func<T, bool> predicate)
		{
			int num = source.FindIndexQ(predicate);
			if (num == -1)
			{
				return default(T);
			}
			return source[num];
		}

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

		public static IOrderedEnumerable<T> OrderByQ<T, S>(this IEnumerable<T> source, Func<T, S> selector)
		{
			return source.OrderBy(selector);
		}

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

		public static T[] ToArrayQ<T>(this List<T> source)
		{
			return source.ToArray();
		}

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

		public static List<T> ToListQ<T>(this T[] source)
		{
			List<T> list = new List<T>(source.Length);
			list.AddRange(source);
			return list;
		}

		public static List<T> ToListQ<T>(this List<T> source)
		{
			List<T> list = new List<T>(source.Count);
			list.AddRange(source);
			return list;
		}

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

		public static IEnumerable<T> WhereQ<T>(this IReadOnlyList<T> source, Func<T, bool> predicate)
		{
			List<T> list;
			if ((list = source as List<T>) != null)
			{
				return list.WhereQ(predicate);
			}
			return LinQuick.WhereQImp<T>(source, predicate);
		}

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

		public static IEnumerable<T> WhereQ<T>(this IEnumerable<T> source, Func<T, bool> predicate)
		{
			IReadOnlyList<T> readOnlyList;
			if ((readOnlyList = source as IReadOnlyList<T>) != null)
			{
				return readOnlyList.WhereQ(predicate);
			}
			return LinQuick.WhereQImp<T>(source, predicate);
		}

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
