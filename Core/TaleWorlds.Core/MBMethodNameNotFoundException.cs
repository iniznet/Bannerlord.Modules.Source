using System;

namespace TaleWorlds.Core
{
	// Token: 0x0200009C RID: 156
	public class MBMethodNameNotFoundException : MBException
	{
		// Token: 0x060007FF RID: 2047 RVA: 0x0001B7CE File Offset: 0x000199CE
		public MBMethodNameNotFoundException(string methodName)
			: base("Unable to find method " + methodName)
		{
		}
	}
}
