using System;
using System.Collections.Generic;

namespace TaleWorlds.Core
{
	// Token: 0x020000A7 RID: 167
	public static class MBNetwork
	{
		// Token: 0x170002A6 RID: 678
		// (get) Token: 0x0600082A RID: 2090 RVA: 0x0001BEEC File Offset: 0x0001A0EC
		// (set) Token: 0x0600082B RID: 2091 RVA: 0x0001BEF3 File Offset: 0x0001A0F3
		public static INetworkCommunication NetworkViewCommunication { get; private set; }

		// Token: 0x170002A7 RID: 679
		// (get) Token: 0x0600082C RID: 2092 RVA: 0x0001BEFB File Offset: 0x0001A0FB
		public static bool MultiplayerDisabled
		{
			get
			{
				return MBNetwork.NetworkViewCommunication == null || MBNetwork.NetworkViewCommunication.MultiplayerDisabled;
			}
		}

		// Token: 0x170002A8 RID: 680
		// (get) Token: 0x0600082D RID: 2093 RVA: 0x0001BF10 File Offset: 0x0001A110
		// (set) Token: 0x0600082E RID: 2094 RVA: 0x0001BF17 File Offset: 0x0001A117
		public static List<ICommunicator> NetworkPeers { get; private set; }

		// Token: 0x170002A9 RID: 681
		// (get) Token: 0x0600082F RID: 2095 RVA: 0x0001BF1F File Offset: 0x0001A11F
		// (set) Token: 0x06000830 RID: 2096 RVA: 0x0001BF26 File Offset: 0x0001A126
		public static List<ICommunicator> DisconnectedNetworkPeers { get; private set; }

		// Token: 0x170002AA RID: 682
		// (get) Token: 0x06000831 RID: 2097 RVA: 0x0001BF2E File Offset: 0x0001A12E
		public static bool LogDisabled
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170002AB RID: 683
		// (get) Token: 0x06000832 RID: 2098 RVA: 0x0001BF31 File Offset: 0x0001A131
		public static bool IsSessionActive
		{
			get
			{
				return MBNetwork.IsServer || MBNetwork.IsClient;
			}
		}

		// Token: 0x170002AC RID: 684
		// (get) Token: 0x06000833 RID: 2099 RVA: 0x0001BF41 File Offset: 0x0001A141
		public static bool IsServer
		{
			get
			{
				return MBNetwork.NetworkViewCommunication != null && MBNetwork.NetworkViewCommunication.IsServer;
			}
		}

		// Token: 0x170002AD RID: 685
		// (get) Token: 0x06000834 RID: 2100 RVA: 0x0001BF56 File Offset: 0x0001A156
		public static bool IsClient
		{
			get
			{
				return MBNetwork.NetworkViewCommunication != null && MBNetwork.NetworkViewCommunication.IsClient;
			}
		}

		// Token: 0x06000835 RID: 2101 RVA: 0x0001BF6B File Offset: 0x0001A16B
		public static void Initialize(INetworkCommunication networkCommunication)
		{
			MBNetwork.VirtualPlayers = new VirtualPlayer[1023];
			MBNetwork.NetworkPeers = new List<ICommunicator>();
			MBNetwork.DisconnectedNetworkPeers = new List<ICommunicator>();
			MBNetwork.NetworkViewCommunication = networkCommunication;
		}

		// Token: 0x170002AE RID: 686
		// (get) Token: 0x06000836 RID: 2102 RVA: 0x0001BF96 File Offset: 0x0001A196
		public static VirtualPlayer MyPeer
		{
			get
			{
				if (MBNetwork.NetworkViewCommunication != null)
				{
					return MBNetwork.NetworkViewCommunication.MyPeer;
				}
				return null;
			}
		}

		// Token: 0x040004A9 RID: 1193
		public const int MaxPlayerCount = 1023;

		// Token: 0x040004AB RID: 1195
		public static VirtualPlayer[] VirtualPlayers;
	}
}
