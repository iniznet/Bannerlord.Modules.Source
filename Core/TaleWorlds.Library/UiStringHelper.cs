using System;

namespace TaleWorlds.Library
{
	public static class UiStringHelper
	{
		public static bool IsStringNoneOrEmptyForUi(string str)
		{
			return string.IsNullOrEmpty(str) || str == "none";
		}
	}
}
