using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.PlatformService;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002FC RID: 764
	public class MultiplayerPermissionHandler : UdpNetworkComponent
	{
		// Token: 0x14000080 RID: 128
		// (add) Token: 0x06002982 RID: 10626 RVA: 0x000A0A68 File Offset: 0x0009EC68
		// (remove) Token: 0x06002983 RID: 10627 RVA: 0x000A0AA0 File Offset: 0x0009ECA0
		public event Action<PlayerId, bool> OnPlayerPlatformMuteChanged;

		// Token: 0x06002984 RID: 10628 RVA: 0x000A0AD5 File Offset: 0x0009ECD5
		public MultiplayerPermissionHandler()
		{
			this._chatBox = Game.Current.GetGameHandler<ChatBox>();
		}

		// Token: 0x06002985 RID: 10629 RVA: 0x000A0AF8 File Offset: 0x0009ECF8
		public override void OnUdpNetworkHandlerClose()
		{
			base.OnUdpNetworkHandlerClose();
			this.HandleClientDisconnect();
		}

		// Token: 0x06002986 RID: 10630 RVA: 0x000A0B08 File Offset: 0x0009ED08
		private void HandleClientDisconnect()
		{
			foreach (ValueTuple<PlayerId, Permission> valueTuple in this._registeredEvents.Keys)
			{
				PlatformServices.Instance.UnregisterPermissionChangeEvent(valueTuple.Item1, valueTuple.Item2, new PermissionChanged(this.VoicePermissionChanged));
				bool flag;
				this._registeredEvents.TryRemove(new ValueTuple<PlayerId, Permission>(valueTuple.Item1, valueTuple.Item2), out flag);
			}
		}

		// Token: 0x06002987 RID: 10631 RVA: 0x000A0B98 File Offset: 0x0009ED98
		protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
		{
			if (GameNetwork.IsClient)
			{
				registerer.Register<InitializeLobbyPeer>(new GameNetworkMessage.ServerMessageHandlerDelegate<InitializeLobbyPeer>(this.HandleServerEventInitializeLobbyPeer));
			}
		}

		// Token: 0x06002988 RID: 10632 RVA: 0x000A0BB4 File Offset: 0x0009EDB4
		private void HandleServerEventInitializeLobbyPeer(InitializeLobbyPeer message)
		{
			if (GameNetwork.MyPeer != null && message.Peer != GameNetwork.MyPeer)
			{
				if (PlatformServices.Instance.RegisterPermissionChangeEvent(message.ProvidedId, Permission.CommunicateUsingText, new PermissionChanged(this.TextPermissionChanged)))
				{
					this._registeredEvents[new ValueTuple<PlayerId, Permission>(message.ProvidedId, Permission.CommunicateUsingText)] = true;
				}
				if (PlatformServices.Instance.RegisterPermissionChangeEvent(message.ProvidedId, Permission.CommunicateUsingVoice, new PermissionChanged(this.VoicePermissionChanged)))
				{
					this._registeredEvents[new ValueTuple<PlayerId, Permission>(message.ProvidedId, Permission.CommunicateUsingVoice)] = true;
				}
			}
		}

		// Token: 0x06002989 RID: 10633 RVA: 0x000A0C44 File Offset: 0x0009EE44
		public override void OnPlayerDisconnectedFromServer(NetworkCommunicator networkPeer)
		{
			base.OnPlayerDisconnectedFromServer(networkPeer);
			if (PlatformServices.Instance.UnregisterPermissionChangeEvent(networkPeer.VirtualPlayer.Id, Permission.CommunicateUsingText, new PermissionChanged(this.TextPermissionChanged)))
			{
				bool flag;
				this._registeredEvents.TryRemove(new ValueTuple<PlayerId, Permission>(networkPeer.VirtualPlayer.Id, Permission.CommunicateUsingText), out flag);
			}
			if (PlatformServices.Instance.UnregisterPermissionChangeEvent(networkPeer.VirtualPlayer.Id, Permission.CommunicateUsingVoice, new PermissionChanged(this.VoicePermissionChanged)))
			{
				bool flag;
				this._registeredEvents.TryRemove(new ValueTuple<PlayerId, Permission>(networkPeer.VirtualPlayer.Id, Permission.CommunicateUsingVoice), out flag);
			}
		}

		// Token: 0x0600298A RID: 10634 RVA: 0x000A0CE0 File Offset: 0x0009EEE0
		private void TextPermissionChanged(PlayerId targetPlayerId, Permission permission, bool hasPermission)
		{
			foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
			{
				if (!(targetPlayerId != networkCommunicator.VirtualPlayer.Id))
				{
					networkCommunicator.GetComponent<MissionPeer>();
					bool flag = !hasPermission;
					this._chatBox.SetPlayerMutedFromPlatform(targetPlayerId, flag);
					Action<PlayerId, bool> onPlayerPlatformMuteChanged = this.OnPlayerPlatformMuteChanged;
					if (onPlayerPlatformMuteChanged != null)
					{
						onPlayerPlatformMuteChanged(targetPlayerId, flag);
					}
				}
			}
		}

		// Token: 0x0600298B RID: 10635 RVA: 0x000A0D64 File Offset: 0x0009EF64
		private void VoicePermissionChanged(PlayerId targetPlayerId, Permission permission, bool hasPermission)
		{
			foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
			{
				if (!(targetPlayerId != networkCommunicator.VirtualPlayer.Id))
				{
					MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
					bool flag = !hasPermission;
					component.SetMutedFromPlatform(flag);
					Action<PlayerId, bool> onPlayerPlatformMuteChanged = this.OnPlayerPlatformMuteChanged;
					if (onPlayerPlatformMuteChanged != null)
					{
						onPlayerPlatformMuteChanged(targetPlayerId, flag);
					}
				}
			}
		}

		// Token: 0x04000F7E RID: 3966
		private ChatBox _chatBox;

		// Token: 0x04000F80 RID: 3968
		[TupleElementNames(new string[] { "PlayerId", "Permission" })]
		private ConcurrentDictionary<ValueTuple<PlayerId, Permission>, bool> _registeredEvents = new ConcurrentDictionary<ValueTuple<PlayerId, Permission>, bool>();
	}
}
