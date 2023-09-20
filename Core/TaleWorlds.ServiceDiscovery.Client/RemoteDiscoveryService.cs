using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TaleWorlds.Library;

namespace TaleWorlds.ServiceDiscovery.Client
{
	// Token: 0x02000003 RID: 3
	public class RemoteDiscoveryService : IDiscoveryService
	{
		// Token: 0x06000003 RID: 3 RVA: 0x00002048 File Offset: 0x00000248
		public RemoteDiscoveryService(string address)
		{
			this._address = address;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002058 File Offset: 0x00000258
		async Task<ServiceAddress[]> IDiscoveryService.ResolveService(string service, string tag)
		{
			ServiceAddress[] array = null;
			try
			{
				array = JsonConvert.DeserializeObject<ServiceAddress[]>(await HttpHelper.DownloadStringTaskAsync(string.Concat(new string[] { this._address, "Data/Resolve/", service, "/", tag })));
			}
			catch (Exception ex)
			{
				Debug.Print("Error on ResolveService: " + ex.Message, 0, Debug.DebugColor.White, 17592186044416UL);
				array = null;
			}
			return array;
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000020B0 File Offset: 0x000002B0
		async Task<ServiceAddress[]> IDiscoveryService.DiscoverServices()
		{
			ServiceAddress[] array = null;
			try
			{
				using (HttpClient client = new HttpClient())
				{
					HttpResponseMessage httpResponseMessage = await client.GetAsync(this._address + "Data/GetDiscoveredServices");
					httpResponseMessage.EnsureSuccessStatusCode();
					array = JsonConvert.DeserializeObject<ServiceAddress[]>(await httpResponseMessage.Content.ReadAsStringAsync());
				}
				HttpClient client = null;
			}
			catch (Exception ex)
			{
				Debug.Print("Error on DiscoverServices: " + ex.Message, 0, Debug.DebugColor.White, 17592186044416UL);
				array = null;
			}
			return array;
		}

		// Token: 0x04000001 RID: 1
		private readonly string _address;
	}
}
