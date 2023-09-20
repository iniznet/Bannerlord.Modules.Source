using System;

namespace TaleWorlds.Library
{
	public static class EnumHelper
	{
		public static ulong GetCombinedULongEnumFlagsValue(Type type)
		{
			ulong num = 0UL;
			foreach (object obj in Enum.GetValues(type))
			{
				num |= (ulong)obj;
			}
			return num;
		}

		public static uint GetCombinedUIntEnumFlagsValue(Type type)
		{
			uint num = 0U;
			foreach (object obj in Enum.GetValues(type))
			{
				num |= (uint)obj;
			}
			return num;
		}
	}
}
