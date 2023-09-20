using System;

namespace TaleWorlds.Core
{
	// Token: 0x02000099 RID: 153
	public class MBTypeMismatchException : MBException
	{
		// Token: 0x060007FB RID: 2043 RVA: 0x0001B788 File Offset: 0x00019988
		public MBTypeMismatchException(string exceptionString)
			: base("Type Does not match with the expected one. " + exceptionString)
		{
		}
	}
}
