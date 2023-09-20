using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TaleWorlds.Library;

namespace TaleWorlds.ServiceDiscovery.Client
{
	public class RemoteDiscoveryService : IDiscoveryService
	{
		public RemoteDiscoveryService(string address)
		{
			this._address = address;
		}

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

		private readonly string _address;
	}
}
