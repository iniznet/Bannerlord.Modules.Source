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
	public class MultiplayerPermissionHandler : UdpNetworkComponent
	{
		public event Action<PlayerId, bool> OnPlayerPlatformMuteChanged;

		public MultiplayerPermissionHandler()
		{
			this._chatBox = Game.Current.GetGameHandler<ChatBox>();
		}

		public override void OnUdpNetworkHandlerClose()
		{
			base.OnUdpNetworkHandlerClose();
			this.HandleClientDisconnect();
		}

		private void HandleClientDisconnect()
		{
			foreach (ValueTuple<PlayerId, Permission> valueTuple in this._registeredEvents.Keys)
			{
				PlatformServices.Instance.UnregisterPermissionChangeEvent(valueTuple.Item1, valueTuple.Item2, new PermissionChanged(this.VoicePermissionChanged));
				bool flag;
				this._registeredEvents.TryRemove(new ValueTuple<PlayerId, Permission>(valueTuple.Item1, valueTuple.Item2), out flag);
			}
		}

		protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
		{
			if (GameNetwork.IsClient)
			{
				registerer.RegisterBaseHandler<InitializeLobbyPeer>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventInitializeLobbyPeer));
			}
		}

		private void HandleServerEventInitializeLobbyPeer(GameNetworkMessage baseMessage)
		{
			InitializeLobbyPeer initializeLobbyPeer = (InitializeLobbyPeer)baseMessage;
			if (GameNetwork.MyPeer != null && initializeLobbyPeer.Peer != GameNetwork.MyPeer)
			{
				if (PlatformServices.Instance.RegisterPermissionChangeEvent(initializeLobbyPeer.ProvidedId, Permission.CommunicateUsingText, new PermissionChanged(this.TextPermissionChanged)))
				{
					this._registeredEvents[new ValueTuple<PlayerId, Permission>(initializeLobbyPeer.ProvidedId, Permission.CommunicateUsingText)] = true;
				}
				if (PlatformServices.Instance.RegisterPermissionChangeEvent(initializeLobbyPeer.ProvidedId, Permission.CommunicateUsingVoice, new PermissionChanged(this.VoicePermissionChanged)))
				{
					this._registeredEvents[new ValueTuple<PlayerId, Permission>(initializeLobbyPeer.ProvidedId, Permission.CommunicateUsingVoice)] = true;
				}
			}
		}

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

		private ChatBox _chatBox;

		[TupleElementNames(new string[] { "PlayerId", "Permission" })]
		private ConcurrentDictionary<ValueTuple<PlayerId, Permission>, bool> _registeredEvents = new ConcurrentDictionary<ValueTuple<PlayerId, Permission>, bool>();
	}
}
