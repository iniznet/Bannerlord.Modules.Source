using System;
using System.Reflection;

namespace TaleWorlds.Library
{
	internal static class EnumHelper<T1>
	{
		public static bool Overlaps(sbyte p1, sbyte p2)
		{
			return (p1 & p2) != 0;
		}

		public static bool Overlaps(byte p1, byte p2)
		{
			return (p1 & p2) > 0;
		}

		public static bool Overlaps(short p1, short p2)
		{
			return (p1 & p2) != 0;
		}

		public static bool Overlaps(ushort p1, ushort p2)
		{
			return (p1 & p2) > 0;
		}

		public static bool Overlaps(int p1, int p2)
		{
			return (p1 & p2) != 0;
		}

		public static bool Overlaps(uint p1, uint p2)
		{
			return (p1 & p2) > 0U;
		}

		public static bool Overlaps(long p1, long p2)
		{
			return (p1 & p2) != 0L;
		}

		public static bool Overlaps(ulong p1, ulong p2)
		{
			return (p1 & p2) > 0UL;
		}

		public static bool ContainsAll(sbyte p1, sbyte p2)
		{
			return (p1 & p2) == p2;
		}

		public static bool ContainsAll(byte p1, byte p2)
		{
			return (p1 & p2) == p2;
		}

		public static bool ContainsAll(short p1, short p2)
		{
			return (p1 & p2) == p2;
		}

		public static bool ContainsAll(ushort p1, ushort p2)
		{
			return (p1 & p2) == p2;
		}

		public static bool ContainsAll(int p1, int p2)
		{
			return (p1 & p2) == p2;
		}

		public static bool ContainsAll(uint p1, uint p2)
		{
			return (p1 & p2) == p2;
		}

		public static bool ContainsAll(long p1, long p2)
		{
			return (p1 & p2) == p2;
		}

		public static bool ContainsAll(ulong p1, ulong p2)
		{
			return (p1 & p2) == p2;
		}

		public static bool initProc(T1 p1, T1 p2)
		{
			Type type = typeof(T1);
			if (type.IsEnum)
			{
				type = Enum.GetUnderlyingType(type);
			}
			Type[] array = new Type[] { type, type };
			MethodInfo methodInfo = typeof(EnumHelper<T1>).GetMethod("Overlaps", array);
			if (methodInfo == null)
			{
				methodInfo = typeof(T1).GetMethod("Overlaps", array);
			}
			if (methodInfo == null)
			{
				throw new MissingMethodException("Unknown type of enum");
			}
			EnumHelper<T1>.HasAnyFlag = (Func<T1, T1, bool>)Delegate.CreateDelegate(typeof(Func<T1, T1, bool>), methodInfo);
			return EnumHelper<T1>.HasAnyFlag(p1, p2);
		}

		public static bool initAllProc(T1 p1, T1 p2)
		{
			Type type = typeof(T1);
			if (type.IsEnum)
			{
				type = Enum.GetUnderlyingType(type);
			}
			Type[] array = new Type[] { type, type };
			MethodInfo methodInfo = typeof(EnumHelper<T1>).GetMethod("ContainsAll", array);
			if (methodInfo == null)
			{
				methodInfo = typeof(T1).GetMethod("ContainsAll", array);
			}
			if (methodInfo == null)
			{
				throw new MissingMethodException("Unknown type of enum");
			}
			EnumHelper<T1>.HasAllFlags = (Func<T1, T1, bool>)Delegate.CreateDelegate(typeof(Func<T1, T1, bool>), methodInfo);
			return EnumHelper<T1>.HasAllFlags(p1, p2);
		}

		public static Func<T1, T1, bool> HasAnyFlag = new Func<T1, T1, bool>(EnumHelper<T1>.initProc);

		public static Func<T1, T1, bool> HasAllFlags = new Func<T1, T1, bool>(EnumHelper<T1>.initAllProc);
	}
}
