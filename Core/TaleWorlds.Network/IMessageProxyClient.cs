using System;
using System.Threading.Tasks;

namespace TaleWorlds.Network
{
	// Token: 0x02000009 RID: 9
	public interface IMessageProxyClient
	{
		// Token: 0x06000034 RID: 52
		Task Disconnect();

		// Token: 0x06000035 RID: 53
		Task SystemReset();
	}
}
