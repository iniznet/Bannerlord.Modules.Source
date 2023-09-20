using System;

namespace TaleWorlds.Localization
{
	// Token: 0x02000005 RID: 5
	public class LocalizationException : Exception
	{
		// Token: 0x0600003F RID: 63 RVA: 0x00002BE1 File Offset: 0x00000DE1
		public LocalizationException()
		{
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00002BE9 File Offset: 0x00000DE9
		public LocalizationException(string message)
			: base(message)
		{
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00002BF2 File Offset: 0x00000DF2
		public LocalizationException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
