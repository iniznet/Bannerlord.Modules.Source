using System;

namespace TaleWorlds.ObjectSystem
{
	// Token: 0x0200000D RID: 13
	public class MBTypeMismatchException : ObjectSystemException
	{
		// Token: 0x06000079 RID: 121 RVA: 0x0000436F File Offset: 0x0000256F
		internal MBTypeMismatchException(string exceptionString)
			: base("Type Does not match with the expected one. " + exceptionString)
		{
		}
	}
}
