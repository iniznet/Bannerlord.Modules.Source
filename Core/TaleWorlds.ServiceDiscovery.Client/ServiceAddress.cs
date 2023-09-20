using System;

namespace TaleWorlds.ServiceDiscovery.Client
{
	// Token: 0x02000004 RID: 4
	public class ServiceAddress
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000006 RID: 6 RVA: 0x000020F5 File Offset: 0x000002F5
		// (set) Token: 0x06000007 RID: 7 RVA: 0x000020FD File Offset: 0x000002FD
		public string Service { get; private set; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000008 RID: 8 RVA: 0x00002106 File Offset: 0x00000306
		// (set) Token: 0x06000009 RID: 9 RVA: 0x0000210E File Offset: 0x0000030E
		public ServiceResolvedAddress[] ResolvedAddresses { get; private set; }

		// Token: 0x0600000A RID: 10 RVA: 0x00002117 File Offset: 0x00000317
		public ServiceAddress(string service, ServiceResolvedAddress[] resolvedAddresses)
		{
			this.ResolvedAddresses = resolvedAddresses;
			this.Service = service;
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002130 File Offset: 0x00000330
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

		// Token: 0x0600000C RID: 12 RVA: 0x0000216E File Offset: 0x0000036E
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

		// Token: 0x04000002 RID: 2
		private const string Prefix = "service://";

		// Token: 0x04000003 RID: 3
		private const char Suffix = '/';
	}
}
