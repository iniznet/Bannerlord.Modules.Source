using System;

namespace TaleWorlds.Diamond
{
	// Token: 0x0200000E RID: 14
	public sealed class InnerProcessConnectionInformation : IConnectionInformation
	{
		// Token: 0x06000041 RID: 65 RVA: 0x00002664 File Offset: 0x00000864
		string IConnectionInformation.GetAddress()
		{
			return "InnerProcess";
		}
	}
}
