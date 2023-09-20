using System;
using System.Threading.Tasks;

namespace TaleWorlds.ServiceDiscovery.Client
{
	// Token: 0x02000002 RID: 2
	public interface IDiscoveryService
	{
		// Token: 0x06000001 RID: 1
		Task<ServiceAddress[]> DiscoverServices();

		// Token: 0x06000002 RID: 2
		Task<ServiceAddress[]> ResolveService(string service, string tag = "");
	}
}
