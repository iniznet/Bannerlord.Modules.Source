using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TaleWorlds.Library;

namespace TaleWorlds.ServiceDiscovery.Client
{
	// Token: 0x02000005 RID: 5
	public static class ServiceAddressManager
	{
		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000D RID: 13 RVA: 0x000021A0 File Offset: 0x000003A0
		private static string EnivornmentFilePath
		{
			get
			{
				return Path.Combine(BasePath.Name, "Parameters", "Environment");
			}
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000021B6 File Offset: 0x000003B6
		public static void Initalize()
		{
			ServiceAddressManager.LoadCache();
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000021C0 File Offset: 0x000003C0
		public static void ResolveAddress(string serviceDiscoveryAddress, string serviceAddress, ref string address, ref ushort port, ref bool isSecure)
		{
			string text;
			if (ServiceAddress.TryGetAddressName(serviceAddress, out text))
			{
				string fileContent = VirtualFolders.GetFileContent(ServiceAddressManager.EnivornmentFilePath);
				ServiceResolvedAddress serviceResolvedAddress;
				if (ServiceAddressManager.TryGetCachedServiceAddress(text, fileContent, out serviceResolvedAddress))
				{
					ServiceAddressManager.SetServiceAddress(serviceResolvedAddress, ref address, ref port, ref isSecure);
					return;
				}
				ServiceResolvedAddress serviceResolvedAddress2;
				if (ServiceAddressManager.TryGetRemoteServiceAddress(serviceDiscoveryAddress, text, fileContent, out serviceResolvedAddress2))
				{
					ServiceAddressManager.SetServiceAddress(serviceResolvedAddress2, ref address, ref port, ref isSecure);
					ServiceAddressManager.CacheServiceAddress(text, fileContent, serviceResolvedAddress2);
				}
			}
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002218 File Offset: 0x00000418
		private static bool TryGetRemoteServiceAddress(string remoteServiceDiscoveryAddress, string serviceName, string environmentId, out ServiceResolvedAddress resolvedAddress)
		{
			IDiscoveryService discoveryService = new RemoteDiscoveryService(remoteServiceDiscoveryAddress);
			Task<ServiceAddress[]> task = Task.Run<ServiceAddress[]>(() => discoveryService.ResolveService(serviceName, environmentId));
			task.Wait(30000);
			if (task.IsCompleted)
			{
				ServiceAddress[] result = task.Result;
				ServiceResolvedAddress serviceResolvedAddress;
				if (result == null)
				{
					serviceResolvedAddress = null;
				}
				else
				{
					ServiceAddress serviceAddress = result.FirstOrDefault<ServiceAddress>();
					if (serviceAddress == null)
					{
						serviceResolvedAddress = null;
					}
					else
					{
						ServiceResolvedAddress[] resolvedAddresses = serviceAddress.ResolvedAddresses;
						serviceResolvedAddress = ((resolvedAddresses != null) ? resolvedAddresses.FirstOrDefault<ServiceResolvedAddress>() : null);
					}
				}
				resolvedAddress = serviceResolvedAddress;
				return resolvedAddress != null;
			}
			resolvedAddress = null;
			return false;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000022A0 File Offset: 0x000004A0
		private static bool TryGetCachedServiceAddress(string serviceName, string environmentId, out ServiceResolvedAddress resolvedAddress)
		{
			ServiceAddressManager.CachedServiceAddress cachedServiceAddress = ServiceAddressManager._serviceAddressCache.FirstOrDefault((ServiceAddressManager.CachedServiceAddress address) => address.ServiceName == serviceName && address.EnvironmentId == environmentId);
			if (cachedServiceAddress != null)
			{
				if (DateTime.UtcNow - cachedServiceAddress.SavedAt < TimeSpan.FromDays(7.0))
				{
					resolvedAddress = cachedServiceAddress.ResolvedAddress;
					return true;
				}
				ServiceAddressManager._serviceAddressCache.Remove(cachedServiceAddress);
			}
			resolvedAddress = null;
			return false;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x0000231A File Offset: 0x0000051A
		private static void SetServiceAddress(ServiceResolvedAddress resolvedAddress, ref string address, ref ushort port, ref bool isSecure)
		{
			if (resolvedAddress != null)
			{
				address = resolvedAddress.Address;
				port = (ushort)resolvedAddress.Port;
				isSecure = resolvedAddress.IsSecure;
			}
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002338 File Offset: 0x00000538
		private static void CacheServiceAddress(string serviceAddress, string environmentId, ServiceResolvedAddress resolvedAddress)
		{
			if (resolvedAddress != null)
			{
				ServiceAddressManager._serviceAddressCache.Add(new ServiceAddressManager.CachedServiceAddress
				{
					ServiceName = serviceAddress,
					EnvironmentId = environmentId,
					ResolvedAddress = resolvedAddress,
					SavedAt = DateTime.UtcNow
				});
				ServiceAddressManager.SaveCache();
			}
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002371 File Offset: 0x00000571
		private static void LoadCache()
		{
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002373 File Offset: 0x00000573
		private static void SaveCache()
		{
		}

		// Token: 0x04000006 RID: 6
		private const string ParametersDirectoryName = "Parameters";

		// Token: 0x04000007 RID: 7
		private const string EnvironmentFileName = "Environment";

		// Token: 0x04000008 RID: 8
		private const string CacheDirectoryName = "Data";

		// Token: 0x04000009 RID: 9
		private const string CachedServiceAddressesFileName = "ServiceAddresses.dat";

		// Token: 0x0400000A RID: 10
		private const int ResolveAddressTaskTimeoutDurationInSeconds = 30000;

		// Token: 0x0400000B RID: 11
		private const int ServiceAddressExpirationTimeInDays = 7;

		// Token: 0x0400000C RID: 12
		private static List<ServiceAddressManager.CachedServiceAddress> _serviceAddressCache = new List<ServiceAddressManager.CachedServiceAddress>();

		// Token: 0x02000009 RID: 9
		[Serializable]
		private class CachedServiceAddress
		{
			// Token: 0x17000008 RID: 8
			// (get) Token: 0x06000024 RID: 36 RVA: 0x00002726 File Offset: 0x00000926
			// (set) Token: 0x06000025 RID: 37 RVA: 0x0000272E File Offset: 0x0000092E
			public string ServiceName { get; set; }

			// Token: 0x17000009 RID: 9
			// (get) Token: 0x06000026 RID: 38 RVA: 0x00002737 File Offset: 0x00000937
			// (set) Token: 0x06000027 RID: 39 RVA: 0x0000273F File Offset: 0x0000093F
			public string EnvironmentId { get; set; }

			// Token: 0x1700000A RID: 10
			// (get) Token: 0x06000028 RID: 40 RVA: 0x00002748 File Offset: 0x00000948
			// (set) Token: 0x06000029 RID: 41 RVA: 0x00002750 File Offset: 0x00000950
			public ServiceResolvedAddress ResolvedAddress { get; set; }

			// Token: 0x1700000B RID: 11
			// (get) Token: 0x0600002A RID: 42 RVA: 0x00002759 File Offset: 0x00000959
			// (set) Token: 0x0600002B RID: 43 RVA: 0x00002761 File Offset: 0x00000961
			public DateTime SavedAt { get; set; }
		}
	}
}
