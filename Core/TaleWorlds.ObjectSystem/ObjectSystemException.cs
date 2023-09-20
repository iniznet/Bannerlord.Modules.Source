using System;

namespace TaleWorlds.ObjectSystem
{
	// Token: 0x02000010 RID: 16
	public class ObjectSystemException : Exception
	{
		// Token: 0x0600007D RID: 125 RVA: 0x00004427 File Offset: 0x00002627
		internal ObjectSystemException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00004431 File Offset: 0x00002631
		internal ObjectSystemException(string message)
			: base(message)
		{
		}

		// Token: 0x0600007F RID: 127 RVA: 0x0000443A File Offset: 0x0000263A
		internal ObjectSystemException()
		{
		}
	}
}
