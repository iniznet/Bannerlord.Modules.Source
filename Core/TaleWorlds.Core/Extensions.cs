using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TaleWorlds.Library;

namespace TaleWorlds.Core
{
	public static class Extensions
	{
		public static string ToHexadecimalString(this uint number)
		{
			return string.Format("{0:X}", number);
		}

		public static string Description(this Enum value)
		{
			object[] customAttributes = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
			if (customAttributes.Length != 0)
			{
				return ((DescriptionAttribute)customAttributes[0]).Description;
			}
			return value.ToString();
		}

		public static float NextFloat(this Random random)
		{
			return (float)random.NextDouble();
		}

		public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
		{
			TKey tkey;
			return source.MaxBy(selector, Comparer<TKey>.Default, out tkey);
		}

		public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, out TKey maxKey)
		{
			return source.MaxBy(selector, Comparer<TKey>.Default, out maxKey);
		}

		public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer, out TKey maxKey)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (selector == null)
			{
				throw new ArgumentNullException("selector");
			}
			if (comparer == null)
			{
				throw new ArgumentNullException("comparer");
			}
			TSource tsource3;
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw new InvalidOperationException("Sequence contains no elements");
				}
				TSource tsource = enumerator.Current;
				maxKey = selector(tsource);
				while (enumerator.MoveNext())
				{
					TSource tsource2 = enumerator.Current;
					TKey tkey = selector(tsource2);
					if (comparer.Compare(tkey, maxKey) > 0)
					{
						tsource = tsource2;
						maxKey = tkey;
					}
				}
				tsource3 = tsource;
			}
			return tsource3;
		}

		public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
		{
			return source.MinBy(selector, Comparer<TKey>.Default);
		}

		public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (selector == null)
			{
				throw new ArgumentNullException("selector");
			}
			if (comparer == null)
			{
				throw new ArgumentNullException("comparer");
			}
			TSource tsource3;
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw new InvalidOperationException("Sequence was empty");
				}
				TSource tsource = enumerator.Current;
				TKey tkey = selector(tsource);
				while (enumerator.MoveNext())
				{
					TSource tsource2 = enumerator.Current;
					TKey tkey2 = selector(tsource2);
					if (comparer.Compare(tkey2, tkey) < 0)
					{
						tsource = tsource2;
						tkey = tkey2;
					}
				}
				tsource3 = tsource;
			}
			return tsource3;
		}

		public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			return source.DistinctBy(keySelector, null);
		}

		public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (keySelector == null)
			{
				throw new ArgumentNullException("keySelector");
			}
			return Extensions.DistinctByImpl<TSource, TKey>(source, keySelector, comparer);
		}

		private static IEnumerable<TSource> DistinctByImpl<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
			return from g in source.GroupBy(keySelector, comparer)
				select g.First<TSource>();
		}

		public static string Add(this string str, string appendant, bool newLine = true)
		{
			if (str == null)
			{
				str = "";
			}
			str += appendant;
			if (newLine)
			{
				str += "\n";
			}
			return str;
		}

		public static IEnumerable<string> Split(this string str, int maxChunkSize)
		{
			for (int i = 0; i < str.Length; i += maxChunkSize)
			{
				yield return str.Substring(i, MathF.Min(maxChunkSize, str.Length - i));
			}
			yield break;
		}

		public static BattleSideEnum GetOppositeSide(this BattleSideEnum side)
		{
			if (side == BattleSideEnum.Attacker)
			{
				return BattleSideEnum.Defender;
			}
			if (side != BattleSideEnum.Defender)
			{
				return side;
			}
			return BattleSideEnum.Attacker;
		}

		public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> source, int splitItemCount)
		{
			if (splitItemCount <= 0)
			{
				throw new ArgumentException();
			}
			int i = 0;
			return source.GroupBy(delegate(T x)
			{
				int j = i;
				i = j + 1;
				return j % splitItemCount;
			});
		}

		public static bool IsEmpty<T>(this IEnumerable<T> source)
		{
			ICollection<T> collection = source as ICollection<T>;
			if (collection != null)
			{
				return collection.Count == 0;
			}
			ICollection collection2 = source as ICollection;
			if (collection2 != null)
			{
				return collection2.Count == 0;
			}
			return !source.Any<T>();
		}

		public static void Shuffle<T>(this IList<T> list)
		{
			int i = list.Count;
			while (i > 1)
			{
				i--;
				int num = MBRandom.RandomInt(i + 1);
				T t = list[num];
				list[num] = list[i];
				list[i] = t;
			}
		}

		public static T GetRandomElement<T>(this IReadOnlyList<T> e)
		{
			if (e.Count == 0)
			{
				return default(T);
			}
			return e[MBRandom.RandomInt(e.Count)];
		}

		public static T GetRandomElement<T>(this MBReadOnlyList<T> e)
		{
			if (e.Count == 0)
			{
				return default(T);
			}
			return e[MBRandom.RandomInt(e.Count)];
		}

		public static T GetRandomElement<T>(this MBList<T> e)
		{
			if (e.Count == 0)
			{
				return default(T);
			}
			return e[MBRandom.RandomInt(e.Count)];
		}

		public static T GetRandomElement<T>(this T[] e)
		{
			if (e.Length == 0)
			{
				return default(T);
			}
			return e[MBRandom.RandomInt(e.Length)];
		}

		public static T GetRandomElementInefficiently<T>(this IEnumerable<T> e)
		{
			if (e.IsEmpty<T>())
			{
				return default(T);
			}
			return e.ElementAt(MBRandom.RandomInt(e.Count<T>()));
		}

		public static T GetRandomElementWithPredicate<T>(this T[] e, Func<T, bool> predicate)
		{
			if (e.Length == 0)
			{
				return default(T);
			}
			int num = 0;
			for (int i = 0; i < e.Length; i++)
			{
				if (predicate(e[i]))
				{
					num++;
				}
			}
			if (num == 0)
			{
				return default(T);
			}
			int num2 = MBRandom.RandomInt(num);
			for (int j = 0; j < e.Length; j++)
			{
				if (predicate(e[j]))
				{
					num2--;
					if (num2 < 0)
					{
						return e[j];
					}
				}
			}
			Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.Core\\Extensions.cs", "GetRandomElementWithPredicate", 442);
			return default(T);
		}

		public static T GetRandomElementWithPredicate<T>(this MBReadOnlyList<T> e, Func<T, bool> predicate)
		{
			if (e.Count == 0)
			{
				return default(T);
			}
			int num = 0;
			for (int i = 0; i < e.Count; i++)
			{
				if (predicate(e[i]))
				{
					num++;
				}
			}
			if (num == 0)
			{
				return default(T);
			}
			int num2 = MBRandom.RandomInt(num);
			for (int j = 0; j < e.Count; j++)
			{
				if (predicate(e[j]))
				{
					num2--;
					if (num2 < 0)
					{
						return e[j];
					}
				}
			}
			Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.Core\\Extensions.cs", "GetRandomElementWithPredicate", 485);
			return default(T);
		}

		public static T GetRandomElementWithPredicate<T>(this MBList<T> e, Func<T, bool> predicate)
		{
			return e.GetRandomElementWithPredicate(predicate);
		}

		public static T GetRandomElementWithPredicate<T>(this IReadOnlyList<T> e, Func<T, bool> predicate)
		{
			if (e.Count == 0)
			{
				return default(T);
			}
			int num = 0;
			for (int i = 0; i < e.Count; i++)
			{
				if (predicate(e[i]))
				{
					num++;
				}
			}
			if (num == 0)
			{
				return default(T);
			}
			int num2 = MBRandom.RandomInt(num);
			for (int j = 0; j < e.Count; j++)
			{
				if (predicate(e[j]))
				{
					num2--;
					if (num2 < 0)
					{
						return e[j];
					}
				}
			}
			Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.Core\\Extensions.cs", "GetRandomElementWithPredicate", 533);
			return default(T);
		}

		public static List<Tuple<T1, T2>> CombineWith<T1, T2>(this IEnumerable<T1> list1, IEnumerable<T2> list2)
		{
			List<Tuple<T1, T2>> list3 = new List<Tuple<T1, T2>>();
			foreach (T1 t in list1)
			{
				foreach (T2 t2 in list2)
				{
					list3.Add(new Tuple<T1, T2>(t, t2));
				}
			}
			return list3;
		}
	}
}
