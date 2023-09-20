using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000324 RID: 804
	internal class MBNetworkPeer : DotNetObject
	{
		// Token: 0x170007C3 RID: 1987
		// (get) Token: 0x06002B84 RID: 11140 RVA: 0x000A9B36 File Offset: 0x000A7D36
		public NetworkCommunicator NetworkPeer { get; }

		// Token: 0x06002B85 RID: 11141 RVA: 0x000A9B3E File Offset: 0x000A7D3E
		internal MBNetworkPeer(NetworkCommunicator networkPeer)
		{
			this.NetworkPeer = networkPeer;
		}
	}
}
