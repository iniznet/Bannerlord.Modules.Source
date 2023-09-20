using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000329 RID: 809
	public class NetworkCommunication : INetworkCommunication
	{
		// Token: 0x170007DD RID: 2013
		// (get) Token: 0x06002BDC RID: 11228 RVA: 0x000AA761 File Offset: 0x000A8961
		VirtualPlayer INetworkCommunication.MyPeer
		{
			get
			{
				NetworkCommunicator myPeer = GameNetwork.MyPeer;
				if (myPeer == null)
				{
					return null;
				}
				return myPeer.VirtualPlayer;
			}
		}

		// Token: 0x170007DE RID: 2014
		// (get) Token: 0x06002BDD RID: 11229 RVA: 0x000AA773 File Offset: 0x000A8973
		bool INetworkCommunication.IsServer
		{
			get
			{
				return GameNetwork.IsServer;
			}
		}

		// Token: 0x170007DF RID: 2015
		// (get) Token: 0x06002BDE RID: 11230 RVA: 0x000AA77A File Offset: 0x000A897A
		bool INetworkCommunication.IsClient
		{
			get
			{
				return GameNetwork.IsClient;
			}
		}

		// Token: 0x170007E0 RID: 2016
		// (get) Token: 0x06002BDF RID: 11231 RVA: 0x000AA781 File Offset: 0x000A8981
		bool INetworkCommunication.MultiplayerDisabled
		{
			get
			{
				return GameNetwork.MultiplayerDisabled;
			}
		}
	}
}
