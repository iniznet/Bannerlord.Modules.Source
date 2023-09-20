using System;

namespace TaleWorlds.ServiceDiscovery.Client
{
	public class ServiceAddress
	{
		public string Service { get; private set; }

		public ServiceResolvedAddress[] ResolvedAddresses { get; private set; }

		public ServiceAddress(string service, ServiceResolvedAddress[] resolvedAddresses)
		{
			this.ResolvedAddresses = resolvedAddresses;
			this.Service = service;
		}

		public static bool IsServiceAddress(string address)
		{
			if (!string.IsNullOrEmpty(address))
			{
				string text = address.ToLower();
				if (text.StartsWith("service://") && text.EndsWith('/'.ToString()))
				{
					return true;
				}
			}
			return false;
		}

		public static bool TryGetAddressName(string serviceAddress, out string addressName)
		{
			if (ServiceAddress.IsServiceAddress(serviceAddress))
			{
				addressName = serviceAddress.Substring("service://".Length).Trim(new char[] { '/' });
				return true;
			}
			addressName = null;
			return false;
		}

		private const string Prefix = "service://";

		private const char Suffix = '/';
	}
}
