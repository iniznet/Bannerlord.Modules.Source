using System;

namespace TaleWorlds.ObjectSystem
{
	// Token: 0x02000006 RID: 6
	public class MBInvalidReferenceException : ObjectSystemException
	{
		// Token: 0x06000016 RID: 22 RVA: 0x00002203 File Offset: 0x00000403
		internal MBInvalidReferenceException(string exceptionString)
			: base("Reference structure is not valid. " + exceptionString)
		{
		}
	}
}
