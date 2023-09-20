using System;

namespace TaleWorlds.Diamond
{
	// Token: 0x0200000C RID: 12
	public interface IClientSessionProvider<T> where T : Client<T>
	{
		// Token: 0x0600003E RID: 62
		IClientSession CreateSession(T session);
	}
}
