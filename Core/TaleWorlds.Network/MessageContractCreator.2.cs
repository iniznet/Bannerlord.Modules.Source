using System;

namespace TaleWorlds.Network
{
	// Token: 0x02000019 RID: 25
	internal class MessageContractCreator<T> : MessageContractCreator where T : MessageContract, new()
	{
		// Token: 0x0600008C RID: 140 RVA: 0x0000320B File Offset: 0x0000140B
		public override MessageContract Invoke()
		{
			return new T();
		}
	}
}
