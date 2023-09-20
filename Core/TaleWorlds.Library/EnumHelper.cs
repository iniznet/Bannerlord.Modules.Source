using System;
using System.Reflection;

namespace TaleWorlds.Library
{
	// Token: 0x0200002A RID: 42
	internal static class EnumHelper<T1>
	{
		// Token: 0x06000148 RID: 328 RVA: 0x00005B59 File Offset: 0x00003D59
		public static bool Overlaps(sbyte p1, sbyte p2)
		{
			return (p1 & p2) != 0;
		}

		// Token: 0x06000149 RID: 329 RVA: 0x00005B61 File Offset: 0x00003D61
		public static bool Overlaps(byte p1, byte p2)
		{
			return (p1 & p2) > 0;
		}

		// Token: 0x0600014A RID: 330 RVA: 0x00005B69 File Offset: 0x00003D69
		public static bool Overlaps(short p1, short p2)
		{
			return (p1 & p2) != 0;
		}

		// Token: 0x0600014B RID: 331 RVA: 0x00005B71 File Offset: 0x00003D71
		public static bool Overlaps(ushort p1, ushort p2)
		{
			return (p1 & p2) > 0;
		}

		// Token: 0x0600014C RID: 332 RVA: 0x00005B79 File Offset: 0x00003D79
		public static bool Overlaps(int p1, int p2)
		{
			return (p1 & p2) != 0;
		}

		// Token: 0x0600014D RID: 333 RVA: 0x00005B81 File Offset: 0x00003D81
		public static bool Overlaps(uint p1, uint p2)
		{
			return (p1 & p2) > 0U;
		}

		// Token: 0x0600014E RID: 334 RVA: 0x00005B89 File Offset: 0x00003D89
		public static bool Overlaps(long p1, long p2)
		{
			return (p1 & p2) != 0L;
		}

		// Token: 0x0600014F RID: 335 RVA: 0x00005B92 File Offset: 0x00003D92
		public static bool Overlaps(ulong p1, ulong p2)
		{
			return (p1 & p2) > 0UL;
		}

		// Token: 0x06000150 RID: 336 RVA: 0x00005B9B File Offset: 0x00003D9B
		public static bool ContainsAll(sbyte p1, sbyte p2)
		{
			return (p1 & p2) == p2;
		}

		// Token: 0x06000151 RID: 337 RVA: 0x00005BA3 File Offset: 0x00003DA3
		public static bool ContainsAll(byte p1, byte p2)
		{
			return (p1 & p2) == p2;
		}

		// Token: 0x06000152 RID: 338 RVA: 0x00005BAB File Offset: 0x00003DAB
		public static bool ContainsAll(short p1, short p2)
		{
			return (p1 & p2) == p2;
		}

		// Token: 0x06000153 RID: 339 RVA: 0x00005BB3 File Offset: 0x00003DB3
		public static bool ContainsAll(ushort p1, ushort p2)
		{
			return (p1 & p2) == p2;
		}

		// Token: 0x06000154 RID: 340 RVA: 0x00005BBB File Offset: 0x00003DBB
		public static bool ContainsAll(int p1, int p2)
		{
			return (p1 & p2) == p2;
		}

		// Token: 0x06000155 RID: 341 RVA: 0x00005BC3 File Offset: 0x00003DC3
		public static bool ContainsAll(uint p1, uint p2)
		{
			return (p1 & p2) == p2;
		}

		// Token: 0x06000156 RID: 342 RVA: 0x00005BCB File Offset: 0x00003DCB
		public static bool ContainsAll(long p1, long p2)
		{
			return (p1 & p2) == p2;
		}

		// Token: 0x06000157 RID: 343 RVA: 0x00005BD3 File Offset: 0x00003DD3
		public static bool ContainsAll(ulong p1, ulong p2)
		{
			return (p1 & p2) == p2;
		}

		// Token: 0x06000158 RID: 344 RVA: 0x00005BDC File Offset: 0x00003DDC
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

		// Token: 0x06000159 RID: 345 RVA: 0x00005C84 File Offset: 0x00003E84
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

		// Token: 0x04000082 RID: 130
		public static Func<T1, T1, bool> HasAnyFlag = new Func<T1, T1, bool>(EnumHelper<T1>.initProc);

		// Token: 0x04000083 RID: 131
		public static Func<T1, T1, bool> HasAllFlags = new Func<T1, T1, bool>(EnumHelper<T1>.initAllProc);
	}
}
