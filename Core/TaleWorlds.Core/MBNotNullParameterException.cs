using System;

namespace TaleWorlds.Core
{
	// Token: 0x0200009F RID: 159
	public class MBNotNullParameterException : MBException
	{
		// Token: 0x06000802 RID: 2050 RVA: 0x0001B807 File Offset: 0x00019A07
		public MBNotNullParameterException(string parameterName)
			: base("The parameter must be null : " + parameterName)
		{
		}
	}
}
