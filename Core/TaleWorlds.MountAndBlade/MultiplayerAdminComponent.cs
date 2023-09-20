using System;
using System.Collections.Generic;
using NetworkMessages.FromClient;
using NetworkMessages.FromServer;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
	public class MultiplayerAdminComponent : MissionNetwork
	{
		public event MultiplayerAdminComponent.OnSelectPlayerToKickDelegate OnSelectPlayerToKick;

		public event MultiplayerAdminComponent.OnShowAdminMenuDelegate OnShowAdminMenu;

		public void OnApplySettings()
		{
			MultiplayerOptions.Instance.InitializeOptionsFromUi();
			GameNetwork.BeginBroadcastModuleEvent();
			GameNetwork.WriteMessage(new MultiplayerOptionsImmediate());
			GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
		}

		public void KickPlayer(bool banPlayer)
		{
			if (this.OnSelectPlayerToKick != null)
			{
				this.OnSelectPlayerToKick(false);
			}
		}

		public void ShowAdminMenu()
		{
			if (this.OnShowAdminMenu != null)
			{
				this.OnShowAdminMenu();
			}
		}

		public void KickPlayer(NetworkCommunicator peerToKick, bool banPlayer)
		{
			if (GameNetwork.IsServer || GameNetwork.IsDedicatedServer)
			{
				if (!peerToKick.IsMine)
				{
					MissionPeer component = peerToKick.GetComponent<MissionPeer>();
					if (component != null)
					{
						if (banPlayer)
						{
							if (GameNetwork.IsClient)
							{
								BannedPlayerManagerCustomGameClient.AddBannedPlayer(component.Peer.Id, GameNetwork.IsDedicatedServer ? (-1) : (Environment.TickCount + 600000));
							}
							else if (GameNetwork.IsDedicatedServer)
							{
								BannedPlayerManagerCustomGameServer.AddBannedPlayer(component.Peer.Id, component.GetNetworkPeer().UserName, GameNetwork.IsDedicatedServer ? (-1) : (Environment.TickCount + 600000));
							}
						}
						if (GameNetwork.IsDedicatedServer)
						{
							throw new NotImplementedException();
						}
						NetworkMain.GameClient.KickPlayer(component.Peer.Id, banPlayer);
						return;
					}
				}
			}
			else if (GameNetwork.IsClient)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new KickPlayer(peerToKick, banPlayer));
				GameNetwork.EndModuleEventAsClient();
			}
		}

		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			MultiplayerAdminComponent._multiplayerAdminComponent = this;
		}

		protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
		{
			if (GameNetwork.IsServer)
			{
				registerer.Register<KickPlayer>(new GameNetworkMessage.ClientMessageHandlerDelegate<KickPlayer>(this.HandleClientEventKickPlayer));
			}
		}

		private bool HandleClientEventKickPlayer(NetworkCommunicator peer, KickPlayer message)
		{
			if (peer.IsAdmin)
			{
				this.KickPlayer(message.PlayerPeer, message.BanPlayer);
			}
			return true;
		}

		public override void OnRemoveBehavior()
		{
			MultiplayerAdminComponent._multiplayerAdminComponent = null;
			base.OnRemoveBehavior();
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("help", "mp_host")]
		public static string MPHostHelp(List<string> strings)
		{
			if (MultiplayerAdminComponent._multiplayerAdminComponent == null)
			{
				return "Failed: MultiplayerAdminComponent has not been created.";
			}
			if (!GameNetwork.IsServerOrRecorder)
			{
				return "Failed: Only the host can use mp_host commands.";
			}
			return "" + "mp_host.restart_game : Restarts the game.\n" + "mp_host.kick_player : Kicks the given player.\n";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("kick_player", "mp_host")]
		public static string MPHostKickPlayer(List<string> strings)
		{
			if (MultiplayerAdminComponent._multiplayerAdminComponent == null)
			{
				return "Failed: MultiplayerAdminComponent has not been created.";
			}
			if (!GameNetwork.IsServerOrRecorder)
			{
				return "Failed: Only the host can use mp_host commands.";
			}
			if (strings.Count != 1)
			{
				return "Failed: Input is incorrect.";
			}
			string text = strings[0];
			foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
			{
				if (networkCommunicator.UserName == text)
				{
					DisconnectInfo disconnectInfo = networkCommunicator.PlayerConnectionInfo.GetParameter<DisconnectInfo>("DisconnectInfo") ?? new DisconnectInfo();
					disconnectInfo.Type = DisconnectType.KickedByHost;
					networkCommunicator.PlayerConnectionInfo.AddParameter("DisconnectInfo", disconnectInfo);
					GameNetwork.AddNetworkPeerToDisconnectAsServer(networkCommunicator);
					return "Player " + text + " has been kicked from the server.";
				}
			}
			return text + " could not be found.";
		}

		private static MultiplayerAdminComponent _multiplayerAdminComponent;

		public delegate void OnSelectPlayerToKickDelegate(bool banPlayer);

		public delegate void OnShowAdminMenuDelegate();
	}
}
