using System;

namespace TaleWorlds.Core
{
	// Token: 0x0200009E RID: 158
	public class MBNullParameterException : MBException
	{
		// Token: 0x06000801 RID: 2049 RVA: 0x0001B7F4 File Offset: 0x000199F4
		public MBNullParameterException(string parameterName)
			: base("The parameter cannot be null : " + parameterName)
		{
		}
	}
}
