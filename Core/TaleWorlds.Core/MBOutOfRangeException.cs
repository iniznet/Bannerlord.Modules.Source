using System;

namespace TaleWorlds.Core
{
	// Token: 0x0200009B RID: 155
	public class MBOutOfRangeException : MBException
	{
		// Token: 0x060007FE RID: 2046 RVA: 0x0001B7BB File Offset: 0x000199BB
		public MBOutOfRangeException(string parameterName)
			: base("The given value is out of range : " + parameterName)
		{
		}
	}
}
