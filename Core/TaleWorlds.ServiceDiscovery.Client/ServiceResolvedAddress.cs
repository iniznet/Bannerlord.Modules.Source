using System;

namespace TaleWorlds.ServiceDiscovery.Client
{
	// Token: 0x02000006 RID: 6
	[Serializable]
	public class ServiceResolvedAddress
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000017 RID: 23 RVA: 0x00002381 File Offset: 0x00000581
		// (set) Token: 0x06000018 RID: 24 RVA: 0x00002389 File Offset: 0x00000589
		public string Address { get; private set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000019 RID: 25 RVA: 0x00002392 File Offset: 0x00000592
		// (set) Token: 0x0600001A RID: 26 RVA: 0x0000239A File Offset: 0x0000059A
		public int Port { get; private set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600001B RID: 27 RVA: 0x000023A3 File Offset: 0x000005A3
		// (set) Token: 0x0600001C RID: 28 RVA: 0x000023AB File Offset: 0x000005AB
		public bool IsSecure { get; private set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600001D RID: 29 RVA: 0x000023B4 File Offset: 0x000005B4
		// (set) Token: 0x0600001E RID: 30 RVA: 0x000023BC File Offset: 0x000005BC
		public string[] Tags { get; private set; }

		// Token: 0x0600001F RID: 31 RVA: 0x000023C5 File Offset: 0x000005C5
		public ServiceResolvedAddress(string address, int port, bool isSecure, string[] tags)
		{
			this.Address = address;
			this.Port = port;
			this.IsSecure = isSecure;
			this.Tags = tags;
		}
	}
}
