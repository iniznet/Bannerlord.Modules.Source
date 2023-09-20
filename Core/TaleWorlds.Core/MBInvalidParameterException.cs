using System;

namespace TaleWorlds.Core
{
	// Token: 0x0200009D RID: 157
	public class MBInvalidParameterException : MBException
	{
		// Token: 0x06000800 RID: 2048 RVA: 0x0001B7E1 File Offset: 0x000199E1
		public MBInvalidParameterException(string parameterName)
			: base("The parameter must be valid : " + parameterName)
		{
		}
	}
}
