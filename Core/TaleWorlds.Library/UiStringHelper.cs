using System;

namespace TaleWorlds.Library
{
	// Token: 0x0200002B RID: 43
	public static class UiStringHelper
	{
		// Token: 0x0600015B RID: 347 RVA: 0x00005D4D File Offset: 0x00003F4D
		public static bool IsStringNoneOrEmptyForUi(string str)
		{
			return string.IsNullOrEmpty(str) || str == "none";
		}
	}
}
