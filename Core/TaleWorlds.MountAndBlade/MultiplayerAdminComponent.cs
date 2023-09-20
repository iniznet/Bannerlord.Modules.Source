using System;
using System.Collections.Generic;
using NetworkMessages.FromClient;
using NetworkMessages.FromServer;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000297 RID: 663
	public class MultiplayerAdminComponent : MissionNetwork
	{
		// Token: 0x14000046 RID: 70
		// (add) Token: 0x060023DA RID: 9178 RVA: 0x00084CC4 File Offset: 0x00082EC4
		// (remove) Token: 0x060023DB RID: 9179 RVA: 0x00084CFC File Offset: 0x00082EFC
		public event MultiplayerAdminComponent.OnSelectPlayerToKickDelegate OnSelectPlayerToKick;

		// Token: 0x14000047 RID: 71
		// (add) Token: 0x060023DC RID: 9180 RVA: 0x00084D34 File Offset: 0x00082F34
		// (remove) Token: 0x060023DD RID: 9181 RVA: 0x00084D6C File Offset: 0x00082F6C
		public event MultiplayerAdminComponent.OnShowAdminMenuDelegate OnShowAdminMenu;

		// Token: 0x060023DE RID: 9182 RVA: 0x00084DA1 File Offset: 0x00082FA1
		public void OnApplySettings()
		{
			MultiplayerOptions.Instance.InitializeOptionsFromUi();
			GameNetwork.BeginBroadcastModuleEvent();
			GameNetwork.WriteMessage(new MultiplayerOptionsImmediate());
			GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
		}

		// Token: 0x060023DF RID: 9183 RVA: 0x00084DC3 File Offset: 0x00082FC3
		public void KickPlayer(bool banPlayer)
		{
			if (this.OnSelectPlayerToKick != null)
			{
				this.OnSelectPlayerToKick(false);
			}
		}

		// Token: 0x060023E0 RID: 9184 RVA: 0x00084DD9 File Offset: 0x00082FD9
		public void ShowAdminMenu()
		{
			if (this.OnShowAdminMenu != null)
			{
				this.OnShowAdminMenu();
			}
		}

		// Token: 0x060023E1 RID: 9185 RVA: 0x00084DF0 File Offset: 0x00082FF0
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

		// Token: 0x060023E2 RID: 9186 RVA: 0x00084ECF File Offset: 0x000830CF
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			MultiplayerAdminComponent._multiplayerAdminComponent = this;
		}

		// Token: 0x060023E3 RID: 9187 RVA: 0x00084EDD File Offset: 0x000830DD
		protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
		{
			if (GameNetwork.IsServer)
			{
				registerer.Register<KickPlayer>(new GameNetworkMessage.ClientMessageHandlerDelegate<KickPlayer>(this.HandleClientEventKickPlayer));
			}
		}

		// Token: 0x060023E4 RID: 9188 RVA: 0x00084EF8 File Offset: 0x000830F8
		private bool HandleClientEventKickPlayer(NetworkCommunicator peer, KickPlayer message)
		{
			if (peer.IsAdmin)
			{
				this.KickPlayer(message.PlayerPeer, message.BanPlayer);
			}
			return true;
		}

		// Token: 0x060023E5 RID: 9189 RVA: 0x00084F15 File Offset: 0x00083115
		public override void OnRemoveBehavior()
		{
			MultiplayerAdminComponent._multiplayerAdminComponent = null;
			base.OnRemoveBehavior();
		}

		// Token: 0x060023E6 RID: 9190 RVA: 0x00084F23 File Offset: 0x00083123
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

		// Token: 0x060023E7 RID: 9191 RVA: 0x00084F58 File Offset: 0x00083158
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

		// Token: 0x04000D21 RID: 3361
		private static MultiplayerAdminComponent _multiplayerAdminComponent;

		// Token: 0x020005B0 RID: 1456
		// (Invoke) Token: 0x06003B7D RID: 15229
		public delegate void OnSelectPlayerToKickDelegate(bool banPlayer);

		// Token: 0x020005B1 RID: 1457
		// (Invoke) Token: 0x06003B81 RID: 15233
		public delegate void OnShowAdminMenuDelegate();
	}
}
