using System;

namespace TaleWorlds.Core
{
	// Token: 0x0200009A RID: 154
	public class MBUnderFlowException : MBException
	{
		// Token: 0x060007FC RID: 2044 RVA: 0x0001B79B File Offset: 0x0001999B
		public MBUnderFlowException()
			: base("The given value is less than the expected value.")
		{
		}

		// Token: 0x060007FD RID: 2045 RVA: 0x0001B7A8 File Offset: 0x000199A8
		public MBUnderFlowException(string parameterName)
			: base("The given value is less than the expected value : " + parameterName)
		{
		}
	}
}
