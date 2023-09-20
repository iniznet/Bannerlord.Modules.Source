using System;
using NetworkMessages.FromServer;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200030F RID: 783
	public static class PeerExtensions
	{
		// Token: 0x06002A5D RID: 10845 RVA: 0x000A4AED File Offset: 0x000A2CED
		public static void SendExistingObjects(this NetworkCommunicator peer, Mission mission)
		{
			MBAPI.IMBPeer.SendExistingObjects(peer.Index, mission.Pointer);
		}

		// Token: 0x06002A5E RID: 10846 RVA: 0x000A4B05 File Offset: 0x000A2D05
		public static VirtualPlayer GetPeer(this PeerComponent peerComponent)
		{
			return peerComponent.Peer;
		}

		// Token: 0x06002A5F RID: 10847 RVA: 0x000A4B0D File Offset: 0x000A2D0D
		public static NetworkCommunicator GetNetworkPeer(this PeerComponent peerComponent)
		{
			return peerComponent.Peer.Communicator as NetworkCommunicator;
		}

		// Token: 0x06002A60 RID: 10848 RVA: 0x000A4B1F File Offset: 0x000A2D1F
		public static T GetComponent<T>(this NetworkCommunicator networkPeer) where T : PeerComponent
		{
			return networkPeer.VirtualPlayer.GetComponent<T>();
		}

		// Token: 0x06002A61 RID: 10849 RVA: 0x000A4B2C File Offset: 0x000A2D2C
		public static void RemoveComponent<T>(this NetworkCommunicator networkPeer, bool synched = true) where T : PeerComponent
		{
			networkPeer.VirtualPlayer.RemoveComponent<T>(true);
		}

		// Token: 0x06002A62 RID: 10850 RVA: 0x000A4B3A File Offset: 0x000A2D3A
		public static void RemoveComponent(this NetworkCommunicator networkPeer, PeerComponent component)
		{
			networkPeer.VirtualPlayer.RemoveComponent(component);
		}

		// Token: 0x06002A63 RID: 10851 RVA: 0x000A4B48 File Offset: 0x000A2D48
		public static PeerComponent GetComponent(this NetworkCommunicator networkPeer, uint componentId)
		{
			return networkPeer.VirtualPlayer.GetComponent(componentId);
		}

		// Token: 0x06002A64 RID: 10852 RVA: 0x000A4B56 File Offset: 0x000A2D56
		public static void AddComponent(this NetworkCommunicator networkPeer, Type peerComponentType)
		{
			networkPeer.VirtualPlayer.AddComponent(peerComponentType);
		}

		// Token: 0x06002A65 RID: 10853 RVA: 0x000A4B65 File Offset: 0x000A2D65
		public static void AddComponent(this NetworkCommunicator networkPeer, uint componentId)
		{
			networkPeer.VirtualPlayer.AddComponent(componentId);
		}

		// Token: 0x06002A66 RID: 10854 RVA: 0x000A4B74 File Offset: 0x000A2D74
		public static T AddComponent<T>(this NetworkCommunicator networkPeer) where T : PeerComponent, new()
		{
			if (networkPeer.GetComponent<T>() != null)
			{
				return networkPeer.TellClientToAddComponent<T>();
			}
			return networkPeer.VirtualPlayer.AddComponent<T>();
		}

		// Token: 0x06002A67 RID: 10855 RVA: 0x000A4B98 File Offset: 0x000A2D98
		public static T TellClientToAddComponent<T>(this NetworkCommunicator networkPeer) where T : PeerComponent, new()
		{
			T component = networkPeer.GetComponent<T>();
			GameNetwork.BeginModuleEventAsServer(networkPeer);
			GameNetwork.WriteMessage(new AddPeerComponent(networkPeer, component.TypeId));
			GameNetwork.EndModuleEventAsServer();
			return component;
		}
	}
}
