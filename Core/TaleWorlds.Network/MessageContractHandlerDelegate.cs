using System;

namespace TaleWorlds.Network
{
	// Token: 0x0200001B RID: 27
	// (Invoke) Token: 0x06000099 RID: 153
	public delegate void MessageContractHandlerDelegate<T>(T message) where T : MessageContract;
}
