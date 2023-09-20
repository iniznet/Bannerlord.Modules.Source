using System;
using System.Collections.Generic;
using System.Linq;
using NetworkMessages.FromClient;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.PlatformService;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002F2 RID: 754
	public class ChatBox : GameHandler
	{
		// Token: 0x17000762 RID: 1890
		// (get) Token: 0x06002918 RID: 10520 RVA: 0x0009F97E File Offset: 0x0009DB7E
		// (set) Token: 0x06002919 RID: 10521 RVA: 0x0009F986 File Offset: 0x0009DB86
		public bool IsContentRestricted { get; private set; }

		// Token: 0x17000763 RID: 1891
		// (get) Token: 0x0600291A RID: 10522 RVA: 0x0009F98F File Offset: 0x0009DB8F
		public bool NetworkReady
		{
			get
			{
				return GameNetwork.IsClient || GameNetwork.IsServer || (NetworkMain.GameClient != null && NetworkMain.GameClient.Connected);
			}
		}

		// Token: 0x0600291B RID: 10523 RVA: 0x0009F9B4 File Offset: 0x0009DBB4
		protected override void OnGameStart()
		{
			ChatBox._chatBox = this;
		}

		// Token: 0x0600291C RID: 10524 RVA: 0x0009F9BC File Offset: 0x0009DBBC
		public override void OnBeforeSave()
		{
		}

		// Token: 0x0600291D RID: 10525 RVA: 0x0009F9BE File Offset: 0x0009DBBE
		public override void OnAfterSave()
		{
		}

		// Token: 0x0600291E RID: 10526 RVA: 0x0009F9C0 File Offset: 0x0009DBC0
		protected override void OnGameEnd()
		{
			ChatBox._chatBox = null;
		}

		// Token: 0x0600291F RID: 10527 RVA: 0x0009F9C8 File Offset: 0x0009DBC8
		public void SendMessageToAll(string message)
		{
			this.SendMessageToAll(message, null);
		}

		// Token: 0x06002920 RID: 10528 RVA: 0x0009F9D2 File Offset: 0x0009DBD2
		public void SendMessageToAll(string message, List<VirtualPlayer> receiverList)
		{
			if (GameNetwork.IsClient && !this.IsContentRestricted)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new NetworkMessages.FromClient.PlayerMessageAll(message));
				GameNetwork.EndModuleEventAsClient();
				return;
			}
			if (GameNetwork.IsServer)
			{
				this.ServerPrepareAndSendMessage(GameNetwork.MyPeer, false, message, receiverList);
			}
		}

		// Token: 0x06002921 RID: 10529 RVA: 0x0009FA0F File Offset: 0x0009DC0F
		public void SendMessageToTeam(string message)
		{
			this.SendMessageToTeam(message, null);
		}

		// Token: 0x06002922 RID: 10530 RVA: 0x0009FA19 File Offset: 0x0009DC19
		public void SendMessageToTeam(string message, List<VirtualPlayer> receiverList)
		{
			if (GameNetwork.IsClient && !this.IsContentRestricted)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new NetworkMessages.FromClient.PlayerMessageTeam(message));
				GameNetwork.EndModuleEventAsClient();
				return;
			}
			if (GameNetwork.IsServer)
			{
				this.ServerPrepareAndSendMessage(GameNetwork.MyPeer, true, message, receiverList);
			}
		}

		// Token: 0x06002923 RID: 10531 RVA: 0x0009FA56 File Offset: 0x0009DC56
		public void SendMessageToWhisperTarget(string message, string platformName, string whisperTarget)
		{
			if (NetworkMain.GameClient != null && NetworkMain.GameClient.Connected)
			{
				NetworkMain.GameClient.SendWhisper(whisperTarget, message);
				if (this.WhisperMessageSent != null)
				{
					this.WhisperMessageSent(message, whisperTarget);
				}
			}
		}

		// Token: 0x06002924 RID: 10532 RVA: 0x0009FA8C File Offset: 0x0009DC8C
		private void OnServerMessage(string message)
		{
			if (this.ServerMessage != null)
			{
				this.ServerMessage(message);
			}
		}

		// Token: 0x06002925 RID: 10533 RVA: 0x0009FAA2 File Offset: 0x0009DCA2
		protected override void OnGameNetworkBegin()
		{
			ChatBox._queuedTeamMessages = new List<ChatBox.QueuedMessageInfo>();
			ChatBox._queuedEveryoneMessages = new List<ChatBox.QueuedMessageInfo>();
			this._isNetworkInitialized = true;
			this.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add);
		}

		// Token: 0x06002926 RID: 10534 RVA: 0x0009FAC8 File Offset: 0x0009DCC8
		private void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode mode)
		{
			GameNetwork.NetworkMessageHandlerRegisterer networkMessageHandlerRegisterer = new GameNetwork.NetworkMessageHandlerRegisterer(mode);
			if (GameNetwork.IsClient)
			{
				networkMessageHandlerRegisterer.Register<NetworkMessages.FromServer.PlayerMessageTeam>(new GameNetworkMessage.ServerMessageHandlerDelegate<NetworkMessages.FromServer.PlayerMessageTeam>(this.HandleServerEventPlayerMessageTeam));
				networkMessageHandlerRegisterer.Register<NetworkMessages.FromServer.PlayerMessageAll>(new GameNetworkMessage.ServerMessageHandlerDelegate<NetworkMessages.FromServer.PlayerMessageAll>(this.HandleServerEventPlayerMessageAll));
				networkMessageHandlerRegisterer.Register<ServerMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<ServerMessage>(this.HandleServerEventServerMessage));
				return;
			}
			if (GameNetwork.IsServer)
			{
				networkMessageHandlerRegisterer.Register<NetworkMessages.FromClient.PlayerMessageAll>(new GameNetworkMessage.ClientMessageHandlerDelegate<NetworkMessages.FromClient.PlayerMessageAll>(this.HandleClientEventPlayerMessageAll));
				networkMessageHandlerRegisterer.Register<NetworkMessages.FromClient.PlayerMessageTeam>(new GameNetworkMessage.ClientMessageHandlerDelegate<NetworkMessages.FromClient.PlayerMessageTeam>(this.HandleClientEventPlayerMessageTeam));
			}
		}

		// Token: 0x06002927 RID: 10535 RVA: 0x0009FB45 File Offset: 0x0009DD45
		protected override void OnGameNetworkEnd()
		{
			base.OnGameNetworkEnd();
			this.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Remove);
		}

		// Token: 0x06002928 RID: 10536 RVA: 0x0009FB54 File Offset: 0x0009DD54
		private void HandleServerEventPlayerMessageAll(NetworkMessages.FromServer.PlayerMessageAll message)
		{
			if (!this.IsContentRestricted)
			{
				this.ShouldShowPlayersMessage(message.Player.VirtualPlayer.Id, delegate(bool result)
				{
					if (result)
					{
						this.OnPlayerMessageReceived(message.Player, message.Message, false);
					}
				});
			}
		}

		// Token: 0x06002929 RID: 10537 RVA: 0x0009FBA4 File Offset: 0x0009DDA4
		private void HandleServerEventPlayerMessageTeam(NetworkMessages.FromServer.PlayerMessageTeam message)
		{
			if (!this.IsContentRestricted)
			{
				this.ShouldShowPlayersMessage(message.Player.VirtualPlayer.Id, delegate(bool result)
				{
					if (result)
					{
						this.OnPlayerMessageReceived(message.Player, message.Message, true);
					}
				});
			}
		}

		// Token: 0x0600292A RID: 10538 RVA: 0x0009FBF4 File Offset: 0x0009DDF4
		private void HandleServerEventServerMessage(ServerMessage message)
		{
			this.OnServerMessage(message.IsMessageTextId ? GameTexts.FindText(message.Message, null).ToString() : message.Message);
		}

		// Token: 0x0600292B RID: 10539 RVA: 0x0009FC1D File Offset: 0x0009DE1D
		private bool HandleClientEventPlayerMessageAll(NetworkCommunicator networkPeer, NetworkMessages.FromClient.PlayerMessageAll message)
		{
			return this.ServerPrepareAndSendMessage(networkPeer, false, message.Message, message.ReceiverList);
		}

		// Token: 0x0600292C RID: 10540 RVA: 0x0009FC33 File Offset: 0x0009DE33
		private bool HandleClientEventPlayerMessageTeam(NetworkCommunicator networkPeer, NetworkMessages.FromClient.PlayerMessageTeam message)
		{
			return this.ServerPrepareAndSendMessage(networkPeer, true, message.Message, message.ReceiverList);
		}

		// Token: 0x0600292D RID: 10541 RVA: 0x0009FC49 File Offset: 0x0009DE49
		public static void ServerSendServerMessageToEveryone(string message)
		{
			ChatBox._chatBox.OnServerMessage(message);
			GameNetwork.BeginBroadcastModuleEvent();
			GameNetwork.WriteMessage(new ServerMessage(message, false));
			GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
		}

		// Token: 0x0600292E RID: 10542 RVA: 0x0009FC70 File Offset: 0x0009DE70
		private bool ServerPrepareAndSendMessage(NetworkCommunicator fromPeer, bool toTeamOnly, string message, List<VirtualPlayer> receiverList)
		{
			if (GameNetwork.IsDedicatedServer)
			{
				Action<NetworkCommunicator, string> onMessageReceivedAtDedicatedServer = this.OnMessageReceivedAtDedicatedServer;
				if (onMessageReceivedAtDedicatedServer != null)
				{
					onMessageReceivedAtDedicatedServer(fromPeer, message);
				}
			}
			if (fromPeer.IsMuted)
			{
				GameNetwork.BeginModuleEventAsServer(fromPeer);
				GameNetwork.WriteMessage(new ServerMessage("str_multiplayer_muted_message", true));
				GameNetwork.EndModuleEventAsServer();
				return true;
			}
			if (this._profanityChecker != null)
			{
				message = this._profanityChecker.CensorText(message);
			}
			if (!GameNetwork.IsDedicatedServer && fromPeer != GameNetwork.MyPeer && !this._mutedPlayers.Contains(fromPeer.VirtualPlayer.Id) && !PermaMuteList.IsPlayerMuted(fromPeer.VirtualPlayer.Id))
			{
				MissionPeer component = GameNetwork.MyPeer.GetComponent<MissionPeer>();
				if (component == null)
				{
					return false;
				}
				bool flag;
				if (toTeamOnly)
				{
					if (component == null)
					{
						return false;
					}
					MissionPeer component2 = fromPeer.GetComponent<MissionPeer>();
					if (component2 == null)
					{
						return false;
					}
					flag = component.Team == component2.Team;
				}
				else
				{
					flag = true;
				}
				if (flag)
				{
					this.OnPlayerMessageReceived(fromPeer, message, toTeamOnly);
				}
			}
			if (toTeamOnly)
			{
				ChatBox.ServerSendMessageToTeam(fromPeer, message, receiverList);
			}
			else
			{
				ChatBox.ServerSendMessageToEveryone(fromPeer, message, receiverList);
			}
			return true;
		}

		// Token: 0x0600292F RID: 10543 RVA: 0x0009FD68 File Offset: 0x0009DF68
		private static void ServerSendMessageToTeam(NetworkCommunicator networkPeer, string message, List<VirtualPlayer> receiverList)
		{
			if (!networkPeer.IsSynchronized)
			{
				ChatBox._queuedTeamMessages.Add(new ChatBox.QueuedMessageInfo(networkPeer, message, receiverList));
				return;
			}
			MissionPeer missionPeer = networkPeer.GetComponent<MissionPeer>();
			MissionPeer missionPeer2 = missionPeer;
			if (((missionPeer2 != null) ? missionPeer2.Team : null) != null)
			{
				using (IEnumerator<NetworkCommunicator> enumerator = GameNetwork.NetworkPeers.Where((NetworkCommunicator x) => !x.IsServerPeer && x.IsSynchronized && x.GetComponent<MissionPeer>().Team == missionPeer.Team).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						NetworkCommunicator networkCommunicator = enumerator.Current;
						if (receiverList == null || receiverList.Contains(networkCommunicator.VirtualPlayer))
						{
							GameNetwork.BeginModuleEventAsServer(networkCommunicator);
							GameNetwork.WriteMessage(new NetworkMessages.FromServer.PlayerMessageTeam(networkPeer, message));
							GameNetwork.EndModuleEventAsServer();
						}
					}
					return;
				}
			}
			ChatBox.ServerSendMessageToEveryone(networkPeer, message, receiverList);
		}

		// Token: 0x06002930 RID: 10544 RVA: 0x0009FE30 File Offset: 0x0009E030
		private static void ServerSendMessageToEveryone(NetworkCommunicator networkPeer, string message, List<VirtualPlayer> receiverList)
		{
			if (!networkPeer.IsSynchronized)
			{
				ChatBox._queuedEveryoneMessages.Add(new ChatBox.QueuedMessageInfo(networkPeer, message, receiverList));
				return;
			}
			foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers.Where((NetworkCommunicator x) => !x.IsServerPeer && x.IsSynchronized))
			{
				if (receiverList == null || receiverList.Contains(networkCommunicator.VirtualPlayer))
				{
					GameNetwork.BeginModuleEventAsServer(networkCommunicator);
					GameNetwork.WriteMessage(new NetworkMessages.FromServer.PlayerMessageAll(networkPeer, message));
					GameNetwork.EndModuleEventAsServer();
				}
			}
		}

		// Token: 0x06002931 RID: 10545 RVA: 0x0009FEDC File Offset: 0x0009E0DC
		public void ResetMuteList()
		{
			this._mutedPlayers.Clear();
		}

		// Token: 0x06002932 RID: 10546 RVA: 0x0009FEE9 File Offset: 0x0009E0E9
		public static void AddWhisperMessage(string fromUserName, string messageBody)
		{
			ChatBox._chatBox.OnWhisperMessageReceived(fromUserName, messageBody);
		}

		// Token: 0x06002933 RID: 10547 RVA: 0x0009FEF7 File Offset: 0x0009E0F7
		public static void AddErrorWhisperMessage(string toUserName)
		{
			ChatBox._chatBox.OnErrorWhisperMessageReceived(toUserName);
		}

		// Token: 0x06002934 RID: 10548 RVA: 0x0009FF04 File Offset: 0x0009E104
		private void OnWhisperMessageReceived(string fromUserName, string messageBody)
		{
			if (this.WhisperMessageReceived != null)
			{
				this.WhisperMessageReceived(fromUserName, messageBody);
			}
		}

		// Token: 0x06002935 RID: 10549 RVA: 0x0009FF1B File Offset: 0x0009E11B
		private void OnErrorWhisperMessageReceived(string toUserName)
		{
			if (this.ErrorWhisperMessageReceived != null)
			{
				this.ErrorWhisperMessageReceived(toUserName);
			}
		}

		// Token: 0x06002936 RID: 10550 RVA: 0x0009FF31 File Offset: 0x0009E131
		private void OnPlayerMessageReceived(NetworkCommunicator networkPeer, string message, bool toTeamOnly)
		{
			if (this.PlayerMessageReceived != null)
			{
				this.PlayerMessageReceived(networkPeer, message, toTeamOnly);
			}
		}

		// Token: 0x06002937 RID: 10551 RVA: 0x0009FF49 File Offset: 0x0009E149
		public void SetPlayerMuted(PlayerId playerID, bool isMuted)
		{
			if (isMuted)
			{
				this.OnPlayerMuted(playerID);
				return;
			}
			this.OnPlayerUnmuted(playerID);
		}

		// Token: 0x06002938 RID: 10552 RVA: 0x0009FF5D File Offset: 0x0009E15D
		public void SetPlayerMutedFromPlatform(PlayerId playerID, bool isMuted)
		{
			if (isMuted && !this._platformMutedPlayers.Contains(playerID))
			{
				this._platformMutedPlayers.Add(playerID);
				return;
			}
			if (!isMuted && this._platformMutedPlayers.Contains(playerID))
			{
				this._platformMutedPlayers.Remove(playerID);
			}
		}

		// Token: 0x06002939 RID: 10553 RVA: 0x0009FF9B File Offset: 0x0009E19B
		private void OnPlayerMuted(PlayerId mutedPlayer)
		{
			if (!this._mutedPlayers.Contains(mutedPlayer))
			{
				this._mutedPlayers.Add(mutedPlayer);
				PlayerMutedDelegate onPlayerMuteChanged = this.OnPlayerMuteChanged;
				if (onPlayerMuteChanged == null)
				{
					return;
				}
				onPlayerMuteChanged(mutedPlayer, true);
			}
		}

		// Token: 0x0600293A RID: 10554 RVA: 0x0009FFC9 File Offset: 0x0009E1C9
		private void OnPlayerUnmuted(PlayerId unmutedPlayer)
		{
			if (this._mutedPlayers.Contains(unmutedPlayer))
			{
				this._mutedPlayers.Remove(unmutedPlayer);
				PlayerMutedDelegate onPlayerMuteChanged = this.OnPlayerMuteChanged;
				if (onPlayerMuteChanged == null)
				{
					return;
				}
				onPlayerMuteChanged(unmutedPlayer, false);
			}
		}

		// Token: 0x0600293B RID: 10555 RVA: 0x0009FFF8 File Offset: 0x0009E1F8
		public bool IsPlayerMuted(PlayerId player)
		{
			return this.IsPlayerMutedFromGame(player) || this.IsPlayerMutedFromPlatform(player);
		}

		// Token: 0x0600293C RID: 10556 RVA: 0x000A000C File Offset: 0x0009E20C
		public bool IsPlayerMutedFromPlatform(PlayerId player)
		{
			return this._platformMutedPlayers.Contains(player);
		}

		// Token: 0x0600293D RID: 10557 RVA: 0x000A001C File Offset: 0x0009E21C
		public bool IsPlayerMutedFromGame(PlayerId player)
		{
			if (GameNetwork.IsDedicatedServer)
			{
				return this._mutedPlayers.Contains(player);
			}
			PlatformServices.Instance.CheckPrivilege(Privilege.Chat, false, delegate(bool result)
			{
				this.IsContentRestricted = !result;
			});
			return this._mutedPlayers.Contains(player) || PermaMuteList.IsPlayerMuted(player);
		}

		// Token: 0x0600293E RID: 10558 RVA: 0x000A006C File Offset: 0x0009E26C
		private void ShouldShowPlayersMessage(PlayerId player, Action<bool> result)
		{
			if (this.IsPlayerMuted(player) || !NetworkMain.GameClient.SupportedFeatures.SupportsFeatures(Features.TextChat))
			{
				result(false);
				return;
			}
			PlayerIdProvidedTypes providedType = player.ProvidedType;
			LobbyClient gameClient = NetworkMain.GameClient;
			PlayerIdProvidedTypes? playerIdProvidedTypes = ((gameClient != null) ? new PlayerIdProvidedTypes?(gameClient.PlayerID.ProvidedType) : null);
			if (!((providedType == playerIdProvidedTypes.GetValueOrDefault()) & (playerIdProvidedTypes != null)))
			{
				result(true);
				return;
			}
			PlatformServices.Instance.CheckPermissionWithUser(Permission.CommunicateUsingText, player, delegate(bool res)
			{
				result(res);
			});
		}

		// Token: 0x0600293F RID: 10559 RVA: 0x000A0115 File Offset: 0x0009E315
		public void SetChatFilterLists(string[] profanityList, string[] allowList)
		{
			this._profanityChecker = new ProfanityChecker(profanityList, allowList);
		}

		// Token: 0x06002940 RID: 10560 RVA: 0x000A0124 File Offset: 0x0009E324
		public void InitializeForMultiplayer()
		{
			PlatformServices.Instance.CheckPrivilege(Privilege.Chat, true, delegate(bool result)
			{
				this.IsContentRestricted = !result;
			});
		}

		// Token: 0x06002941 RID: 10561 RVA: 0x000A013E File Offset: 0x0009E33E
		public void InitializeForSinglePlayer()
		{
			this.IsContentRestricted = false;
		}

		// Token: 0x06002942 RID: 10562 RVA: 0x000A0147 File Offset: 0x0009E347
		public void OnLogin()
		{
			PlatformServices.Instance.CheckPrivilege(Privilege.Chat, false, delegate(bool chatPrivilegeResult)
			{
				this.IsContentRestricted = !chatPrivilegeResult;
			});
		}

		// Token: 0x1400007A RID: 122
		// (add) Token: 0x06002943 RID: 10563 RVA: 0x000A0164 File Offset: 0x0009E364
		// (remove) Token: 0x06002944 RID: 10564 RVA: 0x000A019C File Offset: 0x0009E39C
		public event PlayerMessageReceivedDelegate PlayerMessageReceived;

		// Token: 0x1400007B RID: 123
		// (add) Token: 0x06002945 RID: 10565 RVA: 0x000A01D4 File Offset: 0x0009E3D4
		// (remove) Token: 0x06002946 RID: 10566 RVA: 0x000A020C File Offset: 0x0009E40C
		public event WhisperMessageSentDelegate WhisperMessageSent;

		// Token: 0x1400007C RID: 124
		// (add) Token: 0x06002947 RID: 10567 RVA: 0x000A0244 File Offset: 0x0009E444
		// (remove) Token: 0x06002948 RID: 10568 RVA: 0x000A027C File Offset: 0x0009E47C
		public event WhisperMessageReceivedDelegate WhisperMessageReceived;

		// Token: 0x1400007D RID: 125
		// (add) Token: 0x06002949 RID: 10569 RVA: 0x000A02B4 File Offset: 0x0009E4B4
		// (remove) Token: 0x0600294A RID: 10570 RVA: 0x000A02EC File Offset: 0x0009E4EC
		public event ErrorWhisperMessageReceivedDelegate ErrorWhisperMessageReceived;

		// Token: 0x1400007E RID: 126
		// (add) Token: 0x0600294B RID: 10571 RVA: 0x000A0324 File Offset: 0x0009E524
		// (remove) Token: 0x0600294C RID: 10572 RVA: 0x000A035C File Offset: 0x0009E55C
		public event ServerMessageDelegate ServerMessage;

		// Token: 0x1400007F RID: 127
		// (add) Token: 0x0600294D RID: 10573 RVA: 0x000A0394 File Offset: 0x0009E594
		// (remove) Token: 0x0600294E RID: 10574 RVA: 0x000A03CC File Offset: 0x0009E5CC
		public event PlayerMutedDelegate OnPlayerMuteChanged;

		// Token: 0x0600294F RID: 10575 RVA: 0x000A0404 File Offset: 0x0009E604
		protected override void OnTick(float dt)
		{
			if (GameNetwork.IsServer && this._isNetworkInitialized)
			{
				for (int i = ChatBox._queuedTeamMessages.Count - 1; i >= 0; i--)
				{
					ChatBox.QueuedMessageInfo queuedMessageInfo = ChatBox._queuedTeamMessages[i];
					if (queuedMessageInfo.SourcePeer.IsSynchronized)
					{
						ChatBox.ServerSendMessageToTeam(queuedMessageInfo.SourcePeer, queuedMessageInfo.Message, queuedMessageInfo.ReceiverList);
						ChatBox._queuedTeamMessages.RemoveAt(i);
					}
					else if (queuedMessageInfo.IsExpired)
					{
						ChatBox._queuedTeamMessages.RemoveAt(i);
					}
				}
				for (int j = ChatBox._queuedEveryoneMessages.Count - 1; j >= 0; j--)
				{
					ChatBox.QueuedMessageInfo queuedMessageInfo2 = ChatBox._queuedEveryoneMessages[j];
					if (queuedMessageInfo2.SourcePeer.IsSynchronized)
					{
						ChatBox.ServerSendMessageToEveryone(queuedMessageInfo2.SourcePeer, queuedMessageInfo2.Message, queuedMessageInfo2.ReceiverList);
						ChatBox._queuedEveryoneMessages.RemoveAt(j);
					}
					else if (queuedMessageInfo2.IsExpired)
					{
						ChatBox._queuedEveryoneMessages.RemoveAt(j);
					}
				}
			}
		}

		// Token: 0x04000F69 RID: 3945
		private static ChatBox _chatBox;

		// Token: 0x04000F6B RID: 3947
		private bool _isNetworkInitialized;

		// Token: 0x04000F6C RID: 3948
		private List<PlayerId> _mutedPlayers = new List<PlayerId>();

		// Token: 0x04000F6D RID: 3949
		private List<PlayerId> _platformMutedPlayers = new List<PlayerId>();

		// Token: 0x04000F6E RID: 3950
		private ProfanityChecker _profanityChecker;

		// Token: 0x04000F6F RID: 3951
		private static List<ChatBox.QueuedMessageInfo> _queuedTeamMessages;

		// Token: 0x04000F70 RID: 3952
		private static List<ChatBox.QueuedMessageInfo> _queuedEveryoneMessages;

		// Token: 0x04000F71 RID: 3953
		public Action<NetworkCommunicator, string> OnMessageReceivedAtDedicatedServer;

		// Token: 0x0200060B RID: 1547
		private class QueuedMessageInfo
		{
			// Token: 0x170009C0 RID: 2496
			// (get) Token: 0x06003D2F RID: 15663 RVA: 0x000F2954 File Offset: 0x000F0B54
			public bool IsExpired
			{
				get
				{
					return (DateTime.Now - this._creationTime).TotalSeconds >= 3.0;
				}
			}

			// Token: 0x06003D30 RID: 15664 RVA: 0x000F2987 File Offset: 0x000F0B87
			public QueuedMessageInfo(NetworkCommunicator sourcePeer, string message, List<VirtualPlayer> receiverList)
			{
				this.SourcePeer = sourcePeer;
				this.Message = message;
				this._creationTime = DateTime.Now;
				this.ReceiverList = receiverList;
			}

			// Token: 0x04001F83 RID: 8067
			public readonly NetworkCommunicator SourcePeer;

			// Token: 0x04001F84 RID: 8068
			public readonly string Message;

			// Token: 0x04001F85 RID: 8069
			public readonly List<VirtualPlayer> ReceiverList;

			// Token: 0x04001F86 RID: 8070
			private const float _timeOutDuration = 3f;

			// Token: 0x04001F87 RID: 8071
			private DateTime _creationTime;
		}
	}
}
