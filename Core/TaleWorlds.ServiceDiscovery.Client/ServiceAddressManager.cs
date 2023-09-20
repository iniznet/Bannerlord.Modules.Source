using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TaleWorlds.Library;

namespace TaleWorlds.ServiceDiscovery.Client
{
	public static class ServiceAddressManager
	{
		private static string EnivornmentFilePath
		{
			get
			{
				return Path.Combine(BasePath.Name, "Parameters", "Environment");
			}
		}

		public static void Initalize()
		{
			ServiceAddressManager.LoadCache();
		}

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

		private static void SetServiceAddress(ServiceResolvedAddress resolvedAddress, ref string address, ref ushort port, ref bool isSecure)
		{
			if (resolvedAddress != null)
			{
				address = resolvedAddress.Address;
				port = (ushort)resolvedAddress.Port;
				isSecure = resolvedAddress.IsSecure;
			}
		}

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

		private static void LoadCache()
		{
		}

		private static void SaveCache()
		{
		}

		private const string ParametersDirectoryName = "Parameters";

		private const string EnvironmentFileName = "Environment";

		private const string CacheDirectoryName = "Data";

		private const string CachedServiceAddressesFileName = "ServiceAddresses.dat";

		private const int ResolveAddressTaskTimeoutDurationInSeconds = 30000;

		private const int ServiceAddressExpirationTimeInDays = 7;

		private static List<ServiceAddressManager.CachedServiceAddress> _serviceAddressCache = new List<ServiceAddressManager.CachedServiceAddress>();

		[Serializable]
		private class CachedServiceAddress
		{
			public string ServiceName { get; set; }

			public string EnvironmentId { get; set; }

			public ServiceResolvedAddress ResolvedAddress { get; set; }

			public DateTime SavedAt { get; set; }
		}
	}
}
