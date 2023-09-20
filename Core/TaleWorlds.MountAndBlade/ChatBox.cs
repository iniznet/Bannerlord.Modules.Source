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
	public class ChatBox : GameHandler
	{
		public bool IsContentRestricted { get; private set; }

		public bool NetworkReady
		{
			get
			{
				return GameNetwork.IsClient || GameNetwork.IsServer || (NetworkMain.GameClient != null && NetworkMain.GameClient.Connected);
			}
		}

		protected override void OnGameStart()
		{
			ChatBox._chatBox = this;
		}

		public override void OnBeforeSave()
		{
		}

		public override void OnAfterSave()
		{
		}

		protected override void OnGameEnd()
		{
			ChatBox._chatBox = null;
		}

		public void SendMessageToAll(string message)
		{
			this.SendMessageToAll(message, null);
		}

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

		public void SendMessageToTeam(string message)
		{
			this.SendMessageToTeam(message, null);
		}

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

		private void OnServerMessage(string message)
		{
			if (this.ServerMessage != null)
			{
				this.ServerMessage(message);
			}
		}

		protected override void OnGameNetworkBegin()
		{
			ChatBox._queuedTeamMessages = new List<ChatBox.QueuedMessageInfo>();
			ChatBox._queuedEveryoneMessages = new List<ChatBox.QueuedMessageInfo>();
			this._isNetworkInitialized = true;
			this.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add);
		}

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

		protected override void OnGameNetworkEnd()
		{
			base.OnGameNetworkEnd();
			this.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Remove);
		}

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

		private void HandleServerEventServerMessage(ServerMessage message)
		{
			this.OnServerMessage(message.IsMessageTextId ? GameTexts.FindText(message.Message, null).ToString() : message.Message);
		}

		private bool HandleClientEventPlayerMessageAll(NetworkCommunicator networkPeer, NetworkMessages.FromClient.PlayerMessageAll message)
		{
			return this.ServerPrepareAndSendMessage(networkPeer, false, message.Message, message.ReceiverList);
		}

		private bool HandleClientEventPlayerMessageTeam(NetworkCommunicator networkPeer, NetworkMessages.FromClient.PlayerMessageTeam message)
		{
			return this.ServerPrepareAndSendMessage(networkPeer, true, message.Message, message.ReceiverList);
		}

		public static void ServerSendServerMessageToEveryone(string message)
		{
			ChatBox._chatBox.OnServerMessage(message);
			GameNetwork.BeginBroadcastModuleEvent();
			GameNetwork.WriteMessage(new ServerMessage(message, false));
			GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
		}

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

		public void ResetMuteList()
		{
			this._mutedPlayers.Clear();
		}

		public static void AddWhisperMessage(string fromUserName, string messageBody)
		{
			ChatBox._chatBox.OnWhisperMessageReceived(fromUserName, messageBody);
		}

		public static void AddErrorWhisperMessage(string toUserName)
		{
			ChatBox._chatBox.OnErrorWhisperMessageReceived(toUserName);
		}

		private void OnWhisperMessageReceived(string fromUserName, string messageBody)
		{
			if (this.WhisperMessageReceived != null)
			{
				this.WhisperMessageReceived(fromUserName, messageBody);
			}
		}

		private void OnErrorWhisperMessageReceived(string toUserName)
		{
			if (this.ErrorWhisperMessageReceived != null)
			{
				this.ErrorWhisperMessageReceived(toUserName);
			}
		}

		private void OnPlayerMessageReceived(NetworkCommunicator networkPeer, string message, bool toTeamOnly)
		{
			if (this.PlayerMessageReceived != null)
			{
				this.PlayerMessageReceived(networkPeer, message, toTeamOnly);
			}
		}

		public void SetPlayerMuted(PlayerId playerID, bool isMuted)
		{
			if (isMuted)
			{
				this.OnPlayerMuted(playerID);
				return;
			}
			this.OnPlayerUnmuted(playerID);
		}

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

		public bool IsPlayerMuted(PlayerId player)
		{
			return this.IsPlayerMutedFromGame(player) || this.IsPlayerMutedFromPlatform(player);
		}

		public bool IsPlayerMutedFromPlatform(PlayerId player)
		{
			return this._platformMutedPlayers.Contains(player);
		}

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

		public void SetChatFilterLists(string[] profanityList, string[] allowList)
		{
			this._profanityChecker = new ProfanityChecker(profanityList, allowList);
		}

		public void InitializeForMultiplayer()
		{
			PlatformServices.Instance.CheckPrivilege(Privilege.Chat, true, delegate(bool result)
			{
				this.IsContentRestricted = !result;
			});
		}

		public void InitializeForSinglePlayer()
		{
			this.IsContentRestricted = false;
		}

		public void OnLogin()
		{
			PlatformServices.Instance.CheckPrivilege(Privilege.Chat, false, delegate(bool chatPrivilegeResult)
			{
				this.IsContentRestricted = !chatPrivilegeResult;
			});
		}

		public event PlayerMessageReceivedDelegate PlayerMessageReceived;

		public event WhisperMessageSentDelegate WhisperMessageSent;

		public event WhisperMessageReceivedDelegate WhisperMessageReceived;

		public event ErrorWhisperMessageReceivedDelegate ErrorWhisperMessageReceived;

		public event ServerMessageDelegate ServerMessage;

		public event PlayerMutedDelegate OnPlayerMuteChanged;

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

		private static ChatBox _chatBox;

		private bool _isNetworkInitialized;

		private List<PlayerId> _mutedPlayers = new List<PlayerId>();

		private List<PlayerId> _platformMutedPlayers = new List<PlayerId>();

		private ProfanityChecker _profanityChecker;

		private static List<ChatBox.QueuedMessageInfo> _queuedTeamMessages;

		private static List<ChatBox.QueuedMessageInfo> _queuedEveryoneMessages;

		public Action<NetworkCommunicator, string> OnMessageReceivedAtDedicatedServer;

		private class QueuedMessageInfo
		{
			public bool IsExpired
			{
				get
				{
					return (DateTime.Now - this._creationTime).TotalSeconds >= 3.0;
				}
			}

			public QueuedMessageInfo(NetworkCommunicator sourcePeer, string message, List<VirtualPlayer> receiverList)
			{
				this.SourcePeer = sourcePeer;
				this.Message = message;
				this._creationTime = DateTime.Now;
				this.ReceiverList = receiverList;
			}

			public readonly NetworkCommunicator SourcePeer;

			public readonly string Message;

			public readonly List<VirtualPlayer> ReceiverList;

			private const float _timeOutDuration = 3f;

			private DateTime _creationTime;
		}
	}
}
