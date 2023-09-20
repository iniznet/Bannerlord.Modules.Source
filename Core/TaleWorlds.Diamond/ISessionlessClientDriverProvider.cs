using System;

namespace TaleWorlds.Diamond
{
	// Token: 0x02000013 RID: 19
	public interface ISessionlessClientDriverProvider<T> where T : SessionlessClient<T>
	{
		// Token: 0x06000053 RID: 83
		ISessionlessClientDriver CreateDriver(T client);
	}
}
