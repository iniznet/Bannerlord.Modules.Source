using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NetworkMessages.FromClient;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
	public static class GameNetwork
	{
		public static bool IsServer
		{
			get
			{
				return MBCommon.CurrentGameType == MBCommon.GameType.MultiServer || MBCommon.CurrentGameType == MBCommon.GameType.MultiClientServer;
			}
		}

		public static bool IsServerOrRecorder
		{
			get
			{
				return GameNetwork.IsServer || MBCommon.CurrentGameType == MBCommon.GameType.SingleRecord;
			}
		}

		public static bool IsClient
		{
			get
			{
				return MBCommon.CurrentGameType == MBCommon.GameType.MultiClient;
			}
		}

		public static bool IsReplay
		{
			get
			{
				return MBCommon.CurrentGameType == MBCommon.GameType.SingleReplay;
			}
		}

		public static bool IsClientOrReplay
		{
			get
			{
				return GameNetwork.IsClient || GameNetwork.IsReplay;
			}
		}

		public static bool IsDedicatedServer
		{
			get
			{
				return false;
			}
		}

		public static bool MultiplayerDisabled
		{
			get
			{
				return false;
			}
		}

		public static bool IsMultiplayer
		{
			get
			{
				return GameNetwork.IsServer || GameNetwork.IsClient;
			}
		}

		public static bool IsSessionActive
		{
			get
			{
				return GameNetwork.IsServerOrRecorder || GameNetwork.IsClientOrReplay;
			}
		}

		public static IEnumerable<NetworkCommunicator> NetworkPeersIncludingDisconnectedPeers
		{
			get
			{
				foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
				{
					yield return networkCommunicator;
				}
				IEnumerator<NetworkCommunicator> enumerator = null;
				int num;
				for (int i = 0; i < MBNetwork.DisconnectedNetworkPeers.Count; i = num + 1)
				{
					yield return MBNetwork.DisconnectedNetworkPeers[i] as NetworkCommunicator;
					num = i;
				}
				yield break;
				yield break;
			}
		}

		public static IEnumerable<NetworkCommunicator> NetworkPeers
		{
			get
			{
				return MBNetwork.NetworkPeers.OfType<NetworkCommunicator>();
			}
		}

		public static int NetworkPeerCount
		{
			get
			{
				return MBNetwork.NetworkPeers.Count;
			}
		}

		public static bool NetworkPeersValid
		{
			get
			{
				return MBNetwork.NetworkPeers != null;
			}
		}

		private static void AddNetworkPeer(NetworkCommunicator networkPeer)
		{
			MBNetwork.NetworkPeers.Add(networkPeer);
			Debug.Print("> AddNetworkPeer: " + networkPeer.UserName, 0, Debug.DebugColor.White, 17179869184UL);
		}

		private static void RemoveNetworkPeer(NetworkCommunicator networkPeer)
		{
			Debug.Print("> RemoveNetworkPeer: " + networkPeer.UserName, 0, Debug.DebugColor.White, 17179869184UL);
			MBNetwork.NetworkPeers.Remove(networkPeer);
		}

		private static void AddToDisconnectedPeers(NetworkCommunicator networkPeer)
		{
			Debug.Print("> AddToDisconnectedPeers: " + networkPeer.UserName, 0, Debug.DebugColor.White, 17179869184UL);
			MBNetwork.DisconnectedNetworkPeers.Add(networkPeer);
		}

		public static void ClearAllPeers()
		{
			if (MBNetwork.VirtualPlayers != null)
			{
				for (int i = 0; i < MBNetwork.VirtualPlayers.Length; i++)
				{
					MBNetwork.VirtualPlayers[i] = null;
				}
				MBNetwork.NetworkPeers.Clear();
				MBNetwork.DisconnectedNetworkPeers.Clear();
			}
		}

		public static NetworkCommunicator FindNetworkPeer(int index)
		{
			foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
			{
				if (networkCommunicator.Index == index)
				{
					return networkCommunicator;
				}
			}
			return null;
		}

		public static void Initialize(IGameNetworkHandler handler)
		{
			GameNetwork._handler = handler;
			MBNetwork.Initialize(new NetworkCommunication());
			GameNetwork.NetworkComponents = new List<UdpNetworkComponent>();
			GameNetwork.NetworkHandlers = new List<IUdpNetworkHandler>();
			GameNetwork._handler.OnInitialize();
		}

		internal static void Tick(float dt)
		{
			int i = 0;
			try
			{
				for (i = 0; i < GameNetwork.NetworkHandlers.Count; i++)
				{
					GameNetwork.NetworkHandlers[i].OnUdpNetworkHandlerTick(dt);
				}
			}
			catch (Exception ex)
			{
				if (GameNetwork.NetworkHandlers.Count > 0 && i < GameNetwork.NetworkHandlers.Count && GameNetwork.NetworkHandlers[i] != null)
				{
					string text = GameNetwork.NetworkHandlers[i].ToString();
					Debug.Print("Exception On Network Component: " + text, 0, Debug.DebugColor.White, 17592186044416UL);
				}
				Debug.Print(ex.StackTrace, 0, Debug.DebugColor.White, 17592186044416UL);
				Debug.Print(ex.Message, 0, Debug.DebugColor.White, 17592186044416UL);
			}
		}

		private static void StartMultiplayer()
		{
			VirtualPlayer.Reset();
			GameNetwork._handler.OnStartMultiplayer();
		}

		public static void EndMultiplayer()
		{
			GameNetwork._handler.OnEndMultiplayer();
			for (int i = GameNetwork.NetworkComponents.Count - 1; i >= 0; i--)
			{
				GameNetwork.DestroyComponent(GameNetwork.NetworkComponents[i]);
			}
			for (int j = GameNetwork.NetworkHandlers.Count - 1; j >= 0; j--)
			{
				GameNetwork.RemoveNetworkHandler(GameNetwork.NetworkHandlers[j]);
			}
			if (GameNetwork.IsServer)
			{
				GameNetwork.TerminateServerSide();
			}
			if (GameNetwork.IsClientOrReplay)
			{
				GameNetwork.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Remove);
			}
			if (GameNetwork.IsClient)
			{
				GameNetwork.TerminateClientSide();
			}
			Debug.Print("Clearing peers list with count " + GameNetwork.NetworkPeerCount, 0, Debug.DebugColor.White, 17592186044416UL);
			GameNetwork.ClearAllPeers();
			VirtualPlayer.Reset();
			GameNetwork.MyPeer = null;
			Debug.Print("NetworkManager::HandleMultiplayerEnd", 0, Debug.DebugColor.White, 17592186044416UL);
		}

		[MBCallback]
		internal static void HandleRemovePlayer(MBNetworkPeer peer, bool isTimedOut)
		{
			DisconnectInfo disconnectInfo;
			if ((disconnectInfo = peer.NetworkPeer.PlayerConnectionInfo.GetParameter<DisconnectInfo>("DisconnectInfo")) == null)
			{
				(disconnectInfo = new DisconnectInfo()).Type = DisconnectType.QuitFromGame;
			}
			DisconnectInfo disconnectInfo2 = disconnectInfo;
			disconnectInfo2.Type = (isTimedOut ? DisconnectType.TimedOut : disconnectInfo2.Type);
			peer.NetworkPeer.PlayerConnectionInfo.AddParameter("DisconnectInfo", disconnectInfo2);
			GameNetwork.HandleRemovePlayerInternal(peer.NetworkPeer, peer.NetworkPeer.IsSynchronized && MultiplayerIntermissionVotingManager.Instance.CurrentVoteState == MultiplayerIntermissionState.Idle);
		}

		internal static void HandleRemovePlayerInternal(NetworkCommunicator networkPeer, bool isDisconnected)
		{
			if (GameNetwork.IsClient && networkPeer.IsMine)
			{
				GameNetwork.HandleDisconnect();
				return;
			}
			GameNetwork._handler.OnPlayerDisconnectedFromServer(networkPeer);
			if (GameNetwork.IsServer)
			{
				foreach (IUdpNetworkHandler udpNetworkHandler in GameNetwork.NetworkHandlers)
				{
					udpNetworkHandler.HandleEarlyPlayerDisconnect(networkPeer);
				}
				foreach (IUdpNetworkHandler udpNetworkHandler2 in GameNetwork.NetworkHandlers)
				{
					udpNetworkHandler2.HandlePlayerDisconnect(networkPeer);
				}
			}
			foreach (IUdpNetworkHandler udpNetworkHandler3 in GameNetwork.NetworkHandlers)
			{
				udpNetworkHandler3.OnPlayerDisconnectedFromServer(networkPeer);
			}
			GameNetwork.RemoveNetworkPeer(networkPeer);
			if (isDisconnected)
			{
				GameNetwork.AddToDisconnectedPeers(networkPeer);
			}
			MBNetwork.VirtualPlayers[networkPeer.VirtualPlayer.Index] = null;
			if (GameNetwork.IsServer)
			{
				foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
				{
					if (!networkCommunicator.IsServerPeer)
					{
						GameNetwork.BeginModuleEventAsServer(networkCommunicator);
						GameNetwork.WriteMessage(new DeletePlayer(networkPeer.Index, isDisconnected));
						GameNetwork.EndModuleEventAsServer();
					}
				}
			}
		}

		[MBCallback]
		internal static void HandleDisconnect()
		{
			GameNetwork._handler.OnDisconnectedFromServer();
			foreach (IUdpNetworkHandler udpNetworkHandler in GameNetwork.NetworkHandlers)
			{
				udpNetworkHandler.OnDisconnectedFromServer();
			}
			GameNetwork.MyPeer = null;
		}

		public static void StartReplay()
		{
			GameNetwork._handler.OnStartReplay();
		}

		public static void EndReplay()
		{
			GameNetwork._handler.OnEndReplay();
		}

		public static void PreStartMultiplayerOnServer()
		{
			MBCommon.CurrentGameType = (GameNetwork.IsDedicatedServer ? MBCommon.GameType.MultiServer : MBCommon.GameType.MultiClientServer);
			GameNetwork.ClientPeerIndex = -1;
		}

		public static void StartMultiplayerOnServer(int port)
		{
			Debug.Print("StartMultiplayerOnServer", 0, Debug.DebugColor.White, 17592186044416UL);
			GameNetwork.PreStartMultiplayerOnServer();
			GameNetwork.InitializeServerSide(port);
			GameNetwork.StartMultiplayer();
		}

		[MBCallback]
		internal static bool HandleNetworkPacketAsServer(MBNetworkPeer networkPeer)
		{
			return GameNetwork.HandleNetworkPacketAsServer(networkPeer.NetworkPeer);
		}

		internal static bool HandleNetworkPacketAsServer(NetworkCommunicator networkPeer)
		{
			if (networkPeer == null)
			{
				Debug.Print("networkPeer == null", 0, Debug.DebugColor.White, 17592186044416UL);
				return false;
			}
			bool flag = true;
			try
			{
				int num = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.NetworkComponentEventTypeFromClientCompressionInfo, ref flag);
				if (flag)
				{
					if (num >= 0 && num < GameNetwork._gameNetworkMessageIdsFromClient.Count)
					{
						GameNetworkMessage gameNetworkMessage = Activator.CreateInstance(GameNetwork._gameNetworkMessageIdsFromClient[num]) as GameNetworkMessage;
						gameNetworkMessage.MessageId = num;
						flag = gameNetworkMessage.Read();
						if (flag)
						{
							bool flag2 = false;
							bool flag3 = true;
							List<GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>> list;
							if (GameNetwork._fromClientBaseMessageHandlers.TryGetValue(num, out list))
							{
								foreach (GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage> clientMessageHandlerDelegate in list)
								{
									flag = flag && clientMessageHandlerDelegate(networkPeer, gameNetworkMessage);
									if (!flag)
									{
										break;
									}
								}
								flag3 = false;
								flag2 = list.Count != 0;
							}
							List<object> list2;
							if (GameNetwork._fromClientMessageHandlers.TryGetValue(num, out list2))
							{
								foreach (object obj in list2)
								{
									Delegate @delegate = obj as Delegate;
									flag = flag && (bool)@delegate.DynamicInvokeWithLog(new object[] { networkPeer, gameNetworkMessage });
									if (!flag)
									{
										break;
									}
								}
								flag3 = false;
								flag2 = flag2 || list2.Count != 0;
							}
							if (flag3)
							{
								Debug.FailedAssert("Unknown network messageId " + gameNetworkMessage, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\GameNetwork.cs", "HandleNetworkPacketAsServer", 714);
								flag = false;
							}
							else if (!flag2)
							{
								Debug.Print("Handler not found for network message " + gameNetworkMessage, 0, Debug.DebugColor.White, 17179869184UL);
							}
						}
					}
					else
					{
						Debug.Print("Handler not found for network message " + num.ToString(), 0, Debug.DebugColor.White, 17179869184UL);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.Print("error " + ex.Message, 0, Debug.DebugColor.White, 17592186044416UL);
				return false;
			}
			return flag;
		}

		[MBCallback]
		public static void HandleConsoleCommand(string command)
		{
			if (GameNetwork._handler != null)
			{
				GameNetwork._handler.OnHandleConsoleCommand(command);
			}
		}

		private static void InitializeServerSide(int port)
		{
			MBAPI.IMBNetwork.InitializeServerSide(port);
		}

		private static void TerminateServerSide()
		{
			MBAPI.IMBNetwork.TerminateServerSide();
			if (!GameNetwork.IsDedicatedServer)
			{
				MBCommon.CurrentGameType = MBCommon.GameType.Single;
			}
		}

		private static void PrepareNewUdpSession(int peerIndex, int sessionKey)
		{
			MBAPI.IMBNetwork.PrepareNewUdpSession(peerIndex, sessionKey);
		}

		public static ICommunicator AddNewPlayerOnServer(PlayerConnectionInfo playerConnectionInfo, bool serverPeer, bool isAdmin)
		{
			bool flag = playerConnectionInfo == null;
			int num = (flag ? MBAPI.IMBNetwork.AddNewBotOnServer() : MBAPI.IMBNetwork.AddNewPlayerOnServer(serverPeer));
			Debug.Print(string.Concat(new object[] { ">>> AddNewPlayerOnServer: ", playerConnectionInfo.Name, " index: ", num }), 0, Debug.DebugColor.White, 17179869184UL);
			if (num >= 0)
			{
				int num2 = 0;
				if (!serverPeer)
				{
					num2 = GameNetwork.GetSessionKeyForPlayer();
				}
				int num3 = -1;
				ICommunicator communicator = null;
				if (flag)
				{
					communicator = DummyCommunicator.CreateAsServer(num, "");
				}
				else
				{
					for (int i = 0; i < MBNetwork.DisconnectedNetworkPeers.Count; i++)
					{
						PlayerData parameter = playerConnectionInfo.GetParameter<PlayerData>("PlayerData");
						if (parameter != null && MBNetwork.DisconnectedNetworkPeers[i].VirtualPlayer.Id == parameter.PlayerId)
						{
							num3 = i;
							communicator = MBNetwork.DisconnectedNetworkPeers[i];
							NetworkCommunicator networkCommunicator = communicator as NetworkCommunicator;
							networkCommunicator.UpdateIndexForReconnectingPlayer(num);
							networkCommunicator.UpdateConnectionInfoForReconnect(playerConnectionInfo, isAdmin);
							MBAPI.IMBPeer.SetUserData(num, new MBNetworkPeer(networkCommunicator));
							Debug.Print("> RemoveFromDisconnectedPeers: " + networkCommunicator.UserName, 0, Debug.DebugColor.White, 17179869184UL);
							MBNetwork.DisconnectedNetworkPeers.RemoveAt(i);
							break;
						}
					}
					if (communicator == null)
					{
						communicator = NetworkCommunicator.CreateAsServer(playerConnectionInfo, num, isAdmin);
					}
				}
				MBNetwork.VirtualPlayers[communicator.VirtualPlayer.Index] = communicator.VirtualPlayer;
				if (!flag)
				{
					NetworkCommunicator networkCommunicator2 = communicator as NetworkCommunicator;
					if (serverPeer && GameNetwork.IsServer)
					{
						GameNetwork.ClientPeerIndex = num;
						GameNetwork.MyPeer = networkCommunicator2;
					}
					networkCommunicator2.SessionKey = num2;
					networkCommunicator2.SetServerPeer(serverPeer);
					GameNetwork.AddNetworkPeer(networkCommunicator2);
					playerConnectionInfo.NetworkPeer = networkCommunicator2;
					if (!serverPeer)
					{
						GameNetwork.PrepareNewUdpSession(num, num2);
					}
					if (num3 < 0)
					{
						GameNetwork.BeginBroadcastModuleEvent();
						GameNetwork.WriteMessage(new CreatePlayer(networkCommunicator2.Index, playerConnectionInfo.Name, num3, false, false));
						GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord | GameNetwork.EventBroadcastFlags.DontSendToPeers, null);
					}
					foreach (NetworkCommunicator networkCommunicator3 in GameNetwork.NetworkPeers)
					{
						if (networkCommunicator3 != networkCommunicator2 && networkCommunicator3 != GameNetwork.MyPeer)
						{
							GameNetwork.BeginModuleEventAsServer(networkCommunicator3);
							GameNetwork.WriteMessage(new CreatePlayer(networkCommunicator2.Index, playerConnectionInfo.Name, num3, false, false));
							GameNetwork.EndModuleEventAsServer();
						}
						if (!serverPeer)
						{
							bool flag2 = networkCommunicator3 == networkCommunicator2;
							GameNetwork.BeginModuleEventAsServer(networkCommunicator2);
							GameNetwork.WriteMessage(new CreatePlayer(networkCommunicator3.Index, networkCommunicator3.UserName, -1, false, flag2));
							GameNetwork.EndModuleEventAsServer();
						}
					}
					for (int j = 0; j < MBNetwork.DisconnectedNetworkPeers.Count; j++)
					{
						NetworkCommunicator networkCommunicator4 = MBNetwork.DisconnectedNetworkPeers[j] as NetworkCommunicator;
						GameNetwork.BeginModuleEventAsServer(networkCommunicator2);
						GameNetwork.WriteMessage(new CreatePlayer(networkCommunicator4.Index, networkCommunicator4.UserName, j, true, false));
						GameNetwork.EndModuleEventAsServer();
					}
					foreach (IUdpNetworkHandler udpNetworkHandler in GameNetwork.NetworkHandlers)
					{
						udpNetworkHandler.HandleNewClientConnect(playerConnectionInfo);
					}
					GameNetwork._handler.OnPlayerConnectedToServer(networkCommunicator2);
				}
				return communicator;
			}
			return null;
		}

		public static GameNetwork.AddPlayersResult AddNewPlayersOnServer(PlayerConnectionInfo[] playerConnectionInfos, bool serverPeer)
		{
			bool flag = MBAPI.IMBNetwork.CanAddNewPlayersOnServer(playerConnectionInfos.Length);
			NetworkCommunicator[] array = new NetworkCommunicator[playerConnectionInfos.Length];
			if (flag)
			{
				for (int i = 0; i < array.Length; i++)
				{
					ICommunicator communicator = GameNetwork.AddNewPlayerOnServer(playerConnectionInfos[i], serverPeer, false);
					array[i] = communicator as NetworkCommunicator;
				}
			}
			return new GameNetwork.AddPlayersResult
			{
				NetworkPeers = array,
				Success = flag
			};
		}

		public static void ClientFinishedLoading(NetworkCommunicator networkPeer)
		{
			foreach (IUdpNetworkHandler udpNetworkHandler in GameNetwork.NetworkHandlers)
			{
				udpNetworkHandler.HandleEarlyNewClientAfterLoadingFinished(networkPeer);
			}
			foreach (IUdpNetworkHandler udpNetworkHandler2 in GameNetwork.NetworkHandlers)
			{
				udpNetworkHandler2.HandleNewClientAfterLoadingFinished(networkPeer);
			}
			foreach (IUdpNetworkHandler udpNetworkHandler3 in GameNetwork.NetworkHandlers)
			{
				udpNetworkHandler3.HandleLateNewClientAfterLoadingFinished(networkPeer);
			}
			networkPeer.IsSynchronized = true;
			foreach (IUdpNetworkHandler udpNetworkHandler4 in GameNetwork.NetworkHandlers)
			{
				udpNetworkHandler4.HandleNewClientAfterSynchronized(networkPeer);
			}
			foreach (IUdpNetworkHandler udpNetworkHandler5 in GameNetwork.NetworkHandlers)
			{
				udpNetworkHandler5.HandleLateNewClientAfterSynchronized(networkPeer);
			}
		}

		public static void BeginModuleEventAsClient()
		{
			MBAPI.IMBNetwork.BeginModuleEventAsClient(true);
		}

		public static void EndModuleEventAsClient()
		{
			MBAPI.IMBNetwork.EndModuleEventAsClient(true);
		}

		public static void BeginModuleEventAsClientUnreliable()
		{
			MBAPI.IMBNetwork.BeginModuleEventAsClient(false);
		}

		public static void EndModuleEventAsClientUnreliable()
		{
			MBAPI.IMBNetwork.EndModuleEventAsClient(false);
		}

		public static void BeginModuleEventAsServer(NetworkCommunicator communicator)
		{
			GameNetwork.BeginModuleEventAsServer(communicator.VirtualPlayer);
		}

		public static void BeginModuleEventAsServerUnreliable(NetworkCommunicator communicator)
		{
			GameNetwork.BeginModuleEventAsServerUnreliable(communicator.VirtualPlayer);
		}

		public static void BeginModuleEventAsServer(VirtualPlayer peer)
		{
			MBAPI.IMBPeer.BeginModuleEvent(peer.Index, true);
		}

		public static void EndModuleEventAsServer()
		{
			MBAPI.IMBPeer.EndModuleEvent(true);
		}

		public static void BeginModuleEventAsServerUnreliable(VirtualPlayer peer)
		{
			MBAPI.IMBPeer.BeginModuleEvent(peer.Index, false);
		}

		public static void EndModuleEventAsServerUnreliable()
		{
			MBAPI.IMBPeer.EndModuleEvent(false);
		}

		public static void BeginBroadcastModuleEvent()
		{
			MBAPI.IMBNetwork.BeginBroadcastModuleEvent();
		}

		public static void EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags broadcastFlags, NetworkCommunicator targetPlayer = null)
		{
			int num = ((targetPlayer != null) ? targetPlayer.Index : (-1));
			MBAPI.IMBNetwork.EndBroadcastModuleEvent((int)broadcastFlags, num, true);
		}

		public static void EndBroadcastModuleEventUnreliable(GameNetwork.EventBroadcastFlags broadcastFlags, NetworkCommunicator targetPlayer = null)
		{
			int num = ((targetPlayer != null) ? targetPlayer.Index : (-1));
			MBAPI.IMBNetwork.EndBroadcastModuleEvent((int)broadcastFlags, num, false);
		}

		public static void UnSynchronizeEveryone()
		{
			Debug.Print("UnSynchronizeEveryone is called!", 0, Debug.DebugColor.White, 17179869184UL);
			foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
			{
				networkCommunicator.IsSynchronized = false;
			}
			foreach (IUdpNetworkHandler udpNetworkHandler in GameNetwork.NetworkHandlers)
			{
				udpNetworkHandler.OnEveryoneUnSynchronized();
			}
		}

		public static void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode mode)
		{
			GameNetwork.NetworkMessageHandlerRegisterer networkMessageHandlerRegisterer = new GameNetwork.NetworkMessageHandlerRegisterer(mode);
			networkMessageHandlerRegisterer.Register<CreatePlayer>(new GameNetworkMessage.ServerMessageHandlerDelegate<CreatePlayer>(GameNetwork.HandleServerEventCreatePlayer));
			networkMessageHandlerRegisterer.Register<DeletePlayer>(new GameNetworkMessage.ServerMessageHandlerDelegate<DeletePlayer>(GameNetwork.HandleServerEventDeletePlayer));
		}

		public static void StartMultiplayerOnClient(string serverAddress, int port, int sessionKey, int playerIndex)
		{
			Debug.Print("StartMultiplayerOnClient", 0, Debug.DebugColor.White, 17592186044416UL);
			MBCommon.CurrentGameType = MBCommon.GameType.MultiClient;
			GameNetwork.ClientPeerIndex = playerIndex;
			GameNetwork.InitializeClientSide(serverAddress, port, sessionKey, playerIndex);
			GameNetwork.StartMultiplayer();
			GameNetwork.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add);
		}

		[MBCallback]
		internal static bool HandleNetworkPacketAsClient()
		{
			bool flag = true;
			int num = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.NetworkComponentEventTypeFromServerCompressionInfo, ref flag);
			if (flag && num >= 0 && num < GameNetwork._gameNetworkMessageIdsFromServer.Count)
			{
				GameNetworkMessage gameNetworkMessage = Activator.CreateInstance(GameNetwork._gameNetworkMessageIdsFromServer[num]) as GameNetworkMessage;
				gameNetworkMessage.MessageId = num;
				Debug.Print("Reading message: " + gameNetworkMessage.GetType().Name, 0, Debug.DebugColor.White, 17179869184UL);
				flag = gameNetworkMessage.Read();
				if (flag)
				{
					if (!NetworkMain.GameClient.IsInGame && !GameNetwork.IsReplay)
					{
						Debug.Print("ignoring post mission message: " + gameNetworkMessage.GetType().Name, 0, Debug.DebugColor.White, 17179869184UL);
					}
					else
					{
						bool flag2 = false;
						bool flag3 = true;
						if ((gameNetworkMessage.GetLogFilter() & (MultiplayerMessageFilter)(-1)) != MultiplayerMessageFilter.None)
						{
							if (GameNetworkMessage.IsClientMissionOver)
							{
								Debug.Print("WARNING: Entering message processing while client mission is over", 0, Debug.DebugColor.White, 17592186044416UL);
							}
							Debug.Print("Processing message: " + gameNetworkMessage.GetType().Name + ": " + gameNetworkMessage.GetLogFormat(), 0, Debug.DebugColor.White, 17179869184UL);
						}
						List<GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>> list;
						if (GameNetwork._fromServerBaseMessageHandlers.TryGetValue(num, out list))
						{
							foreach (GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage> serverMessageHandlerDelegate in list)
							{
								serverMessageHandlerDelegate(gameNetworkMessage);
							}
							flag3 = false;
							flag2 = list.Count != 0;
						}
						List<object> list2;
						if (GameNetwork._fromServerMessageHandlers.TryGetValue(num, out list2))
						{
							foreach (object obj in list2)
							{
								(obj as Delegate).DynamicInvokeWithLog(new object[] { gameNetworkMessage });
							}
							flag3 = false;
							flag2 = flag2 || list2.Count != 0;
						}
						if (flag3)
						{
							Debug.Print("Invalid messageId " + num.ToString(), 0, Debug.DebugColor.White, 17179869184UL);
							Debug.Print("Invalid messageId " + gameNetworkMessage.GetType().Name, 0, Debug.DebugColor.White, 17179869184UL);
						}
						else if (!flag2)
						{
							Debug.Print("No message handler found for " + gameNetworkMessage.GetType().Name, 0, Debug.DebugColor.Red, 17179869184UL);
						}
					}
				}
				else
				{
					Debug.Print("Invalid message read for: " + gameNetworkMessage.GetType().Name, 0, Debug.DebugColor.White, 17179869184UL);
				}
			}
			else
			{
				Debug.Print("Invalid message id read: " + num, 0, Debug.DebugColor.White, 17179869184UL);
			}
			return flag;
		}

		private static int GetSessionKeyForPlayer()
		{
			return new Random(DateTime.Now.Millisecond).Next(1, 4001);
		}

		public static NetworkCommunicator HandleNewClientConnect(PlayerConnectionInfo playerConnectionInfo, bool isAdmin)
		{
			NetworkCommunicator networkCommunicator = GameNetwork.AddNewPlayerOnServer(playerConnectionInfo, false, isAdmin) as NetworkCommunicator;
			GameNetwork._handler.OnNewPlayerConnect(playerConnectionInfo, networkCommunicator);
			return networkCommunicator;
		}

		public static GameNetwork.AddPlayersResult HandleNewClientsConnect(PlayerConnectionInfo[] playerConnectionInfos, bool isAdmin)
		{
			GameNetwork.AddPlayersResult addPlayersResult = GameNetwork.AddNewPlayersOnServer(playerConnectionInfos, isAdmin);
			if (addPlayersResult.Success)
			{
				for (int i = 0; i < playerConnectionInfos.Length; i++)
				{
					GameNetwork._handler.OnNewPlayerConnect(playerConnectionInfos[i], addPlayersResult.NetworkPeers[i]);
				}
			}
			return addPlayersResult;
		}

		public static void AddNetworkPeerToDisconnectAsServer(NetworkCommunicator networkPeer)
		{
			Debug.Print("adding peer to disconnect index:" + networkPeer.Index, 0, Debug.DebugColor.White, 17179869184UL);
			GameNetwork.AddPeerToDisconnect(networkPeer);
			GameNetwork.BeginModuleEventAsServer(networkPeer);
			GameNetwork.WriteMessage(new DeletePlayer(networkPeer.Index, false));
			GameNetwork.EndModuleEventAsServer();
		}

		private static void HandleServerEventCreatePlayer(CreatePlayer message)
		{
			int playerIndex = message.PlayerIndex;
			string playerName = message.PlayerName;
			bool isReceiverPeer = message.IsReceiverPeer;
			NetworkCommunicator networkCommunicator;
			if (isReceiverPeer || message.IsNonExistingDisconnectedPeer || message.DisconnectedPeerIndex < 0)
			{
				networkCommunicator = NetworkCommunicator.CreateAsClient(playerName, playerIndex);
			}
			else
			{
				networkCommunicator = MBNetwork.DisconnectedNetworkPeers[message.DisconnectedPeerIndex] as NetworkCommunicator;
				networkCommunicator.UpdateIndexForReconnectingPlayer(message.PlayerIndex);
				Debug.Print("> RemoveFromDisconnectedPeers: " + networkCommunicator.UserName, 0, Debug.DebugColor.White, 17179869184UL);
				MBNetwork.DisconnectedNetworkPeers.RemoveAt(message.DisconnectedPeerIndex);
			}
			if (isReceiverPeer)
			{
				GameNetwork.MyPeer = networkCommunicator;
			}
			if (message.IsNonExistingDisconnectedPeer)
			{
				GameNetwork.AddToDisconnectedPeers(networkCommunicator);
			}
			else
			{
				MBNetwork.VirtualPlayers[networkCommunicator.VirtualPlayer.Index] = networkCommunicator.VirtualPlayer;
				GameNetwork.AddNetworkPeer(networkCommunicator);
			}
			GameNetwork._handler.OnPlayerConnectedToServer(networkCommunicator);
		}

		private static void HandleServerEventDeletePlayer(DeletePlayer message)
		{
			NetworkCommunicator networkCommunicator = GameNetwork.NetworkPeers.FirstOrDefault((NetworkCommunicator networkPeer) => networkPeer.Index == message.PlayerIndex);
			if (networkCommunicator != null)
			{
				GameNetwork.HandleRemovePlayerInternal(networkCommunicator, message.AddToDisconnectList);
			}
		}

		public static void InitializeClientSide(string serverAddress, int port, int sessionKey, int playerIndex)
		{
			MBAPI.IMBNetwork.InitializeClientSide(serverAddress, port, sessionKey, playerIndex);
		}

		public static void TerminateClientSide()
		{
			MBAPI.IMBNetwork.TerminateClientSide();
			MBCommon.CurrentGameType = MBCommon.GameType.Single;
		}

		public static void DestroyComponent(UdpNetworkComponent udpNetworkComponent)
		{
			GameNetwork.RemoveNetworkHandler(udpNetworkComponent);
			GameNetwork.NetworkComponents.Remove(udpNetworkComponent);
		}

		public static T AddNetworkComponent<T>() where T : UdpNetworkComponent
		{
			T t = (T)((object)Activator.CreateInstance(typeof(T), new object[0]));
			GameNetwork.NetworkComponents.Add(t);
			GameNetwork.NetworkHandlers.Add(t);
			return t;
		}

		public static void AddNetworkHandler(IUdpNetworkHandler handler)
		{
			GameNetwork.NetworkHandlers.Add(handler);
		}

		public static void RemoveNetworkHandler(IUdpNetworkHandler handler)
		{
			handler.OnUdpNetworkHandlerClose();
			GameNetwork.NetworkHandlers.Remove(handler);
		}

		public static T GetNetworkComponent<T>() where T : UdpNetworkComponent
		{
			using (List<UdpNetworkComponent>.Enumerator enumerator = GameNetwork.NetworkComponents.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					T t;
					if ((t = enumerator.Current as T) != null)
					{
						return t;
					}
				}
			}
			return default(T);
		}

		public static List<UdpNetworkComponent> NetworkComponents { get; private set; }

		public static List<IUdpNetworkHandler> NetworkHandlers { get; private set; }

		public static void WriteMessage(GameNetworkMessage message)
		{
			Type type = message.GetType();
			message.MessageId = GameNetwork._gameNetworkMessageTypesAll[type];
			message.Write();
		}

		private static void AddServerMessageHandler<T>(GameNetworkMessage.ServerMessageHandlerDelegate<T> handler) where T : GameNetworkMessage
		{
			int num = GameNetwork._gameNetworkMessageTypesFromServer[typeof(T)];
			GameNetwork._fromServerMessageHandlers[num].Add(handler);
		}

		private static void AddServerBaseMessageHandler(GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage> handler, Type messageType)
		{
			int num = GameNetwork._gameNetworkMessageTypesFromServer[messageType];
			GameNetwork._fromServerBaseMessageHandlers[num].Add(handler);
		}

		private static void AddClientMessageHandler<T>(GameNetworkMessage.ClientMessageHandlerDelegate<T> handler) where T : GameNetworkMessage
		{
			int num = GameNetwork._gameNetworkMessageTypesFromClient[typeof(T)];
			GameNetwork._fromClientMessageHandlers[num].Add(handler);
		}

		private static void AddClientBaseMessageHandler(GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage> handler, Type messageType)
		{
			int num = GameNetwork._gameNetworkMessageTypesFromClient[messageType];
			GameNetwork._fromClientBaseMessageHandlers[num].Add(handler);
		}

		private static void RemoveServerMessageHandler<T>(GameNetworkMessage.ServerMessageHandlerDelegate<T> handler) where T : GameNetworkMessage
		{
			int num = GameNetwork._gameNetworkMessageTypesFromServer[typeof(T)];
			GameNetwork._fromServerMessageHandlers[num].Remove(handler);
		}

		private static void RemoveServerBaseMessageHandler(GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage> handler, Type messageType)
		{
			int num = GameNetwork._gameNetworkMessageTypesFromServer[messageType];
			GameNetwork._fromServerBaseMessageHandlers[num].Remove(handler);
		}

		private static void RemoveClientMessageHandler<T>(GameNetworkMessage.ClientMessageHandlerDelegate<T> handler) where T : GameNetworkMessage
		{
			int num = GameNetwork._gameNetworkMessageTypesFromClient[typeof(T)];
			GameNetwork._fromClientMessageHandlers[num].Remove(handler);
		}

		private static void RemoveClientBaseMessageHandler(GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage> handler, Type messageType)
		{
			int num = GameNetwork._gameNetworkMessageTypesFromClient[messageType];
			GameNetwork._fromClientBaseMessageHandlers[num].Remove(handler);
		}

		internal static void FindGameNetworkMessages()
		{
			Debug.Print("Searching Game NetworkMessages Methods", 0, Debug.DebugColor.White, 17179869184UL);
			GameNetwork._fromClientMessageHandlers = new Dictionary<int, List<object>>();
			GameNetwork._fromServerMessageHandlers = new Dictionary<int, List<object>>();
			GameNetwork._fromClientBaseMessageHandlers = new Dictionary<int, List<GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>>>();
			GameNetwork._fromServerBaseMessageHandlers = new Dictionary<int, List<GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>>>();
			GameNetwork._gameNetworkMessageTypesAll = new Dictionary<Type, int>();
			GameNetwork._gameNetworkMessageTypesFromClient = new Dictionary<Type, int>();
			GameNetwork._gameNetworkMessageTypesFromServer = new Dictionary<Type, int>();
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			List<Type> list = new List<Type>();
			List<Type> list2 = new List<Type>();
			foreach (Assembly assembly in assemblies)
			{
				if (GameNetwork.CheckAssemblyForNetworkMessage(assembly))
				{
					GameNetwork.CollectGameNetworkMessagesFromAssembly(assembly, list, list2);
				}
			}
			list.Sort((Type s1, Type s2) => s1.FullName.CompareTo(s2.FullName));
			list2.Sort((Type s1, Type s2) => s1.FullName.CompareTo(s2.FullName));
			GameNetwork._gameNetworkMessageIdsFromClient = new List<Type>(list.Count);
			for (int j = 0; j < list.Count; j++)
			{
				Type type = list[j];
				GameNetwork._gameNetworkMessageIdsFromClient.Add(type);
				GameNetwork._gameNetworkMessageTypesFromClient.Add(type, j);
				GameNetwork._gameNetworkMessageTypesAll.Add(type, j);
				GameNetwork._fromClientMessageHandlers.Add(j, new List<object>());
				GameNetwork._fromClientBaseMessageHandlers.Add(j, new List<GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>>());
			}
			GameNetwork._gameNetworkMessageIdsFromServer = new List<Type>(list2.Count);
			for (int k = 0; k < list2.Count; k++)
			{
				Type type2 = list2[k];
				GameNetwork._gameNetworkMessageIdsFromServer.Add(type2);
				GameNetwork._gameNetworkMessageTypesFromServer.Add(type2, k);
				GameNetwork._gameNetworkMessageTypesAll.Add(type2, k);
				GameNetwork._fromServerMessageHandlers.Add(k, new List<object>());
				GameNetwork._fromServerBaseMessageHandlers.Add(k, new List<GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>>());
			}
			CompressionBasic.NetworkComponentEventTypeFromClientCompressionInfo = new CompressionInfo.Integer(0, list.Count - 1, true);
			CompressionBasic.NetworkComponentEventTypeFromServerCompressionInfo = new CompressionInfo.Integer(0, list2.Count - 1, true);
			Debug.Print("Found " + list.Count + " Client Game Network Messages", 0, Debug.DebugColor.White, 17179869184UL);
			Debug.Print("Found " + list2.Count + " Server Game Network Messages", 0, Debug.DebugColor.White, 17179869184UL);
		}

		private static bool CheckAssemblyForNetworkMessage(Assembly assembly)
		{
			Assembly assembly2 = Assembly.GetAssembly(typeof(GameNetworkMessage));
			if (assembly == assembly2)
			{
				return true;
			}
			AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies();
			for (int i = 0; i < referencedAssemblies.Length; i++)
			{
				if (referencedAssemblies[i].FullName == assembly2.FullName)
				{
					return true;
				}
			}
			return false;
		}

		public static void IncreaseTotalUploadLimit(int value)
		{
			MBAPI.IMBNetwork.IncreaseTotalUploadLimit(value);
		}

		public static void ResetDebugVariables()
		{
			MBAPI.IMBNetwork.ResetDebugVariables();
		}

		public static void PrintDebugStats()
		{
			MBAPI.IMBNetwork.PrintDebugStats();
		}

		public static float GetAveragePacketLossRatio()
		{
			return MBAPI.IMBNetwork.GetAveragePacketLossRatio();
		}

		public static void GetDebugUploadsInBits(ref GameNetwork.DebugNetworkPacketStatisticsStruct networkStatisticsStruct, ref GameNetwork.DebugNetworkPositionCompressionStatisticsStruct posStatisticsStruct)
		{
			MBAPI.IMBNetwork.GetDebugUploadsInBits(ref networkStatisticsStruct, ref posStatisticsStruct);
		}

		public static void PrintReplicationTableStatistics()
		{
			MBAPI.IMBNetwork.PrintReplicationTableStatistics();
		}

		public static void ClearReplicationTableStatistics()
		{
			MBAPI.IMBNetwork.ClearReplicationTableStatistics();
		}

		public static void ResetDebugUploads()
		{
			MBAPI.IMBNetwork.ResetDebugUploads();
		}

		public static void ResetMissionData()
		{
			MBAPI.IMBNetwork.ResetMissionData();
		}

		private static void AddPeerToDisconnect(NetworkCommunicator networkPeer)
		{
			MBAPI.IMBNetwork.AddPeerToDisconnect(networkPeer.Index);
		}

		public static void InitializeCompressionInfos()
		{
			CompressionBasic.ActionCodeCompressionInfo = new CompressionInfo.Integer(ActionIndexCache.act_none.Index, MBAnimation.GetNumActionCodes() - 1, true);
			CompressionBasic.AnimationIndexCompressionInfo = new CompressionInfo.Integer(0, MBAnimation.GetNumAnimations() - 1, true);
			CompressionBasic.CultureIndexCompressionInfo = new CompressionInfo.Integer(-1, MBObjectManager.Instance.GetObjectTypeList<BasicCultureObject>().Count - 1, true);
			CompressionBasic.SoundEventsCompressionInfo = new CompressionInfo.Integer(0, SoundEvent.GetTotalEventCount() - 1, true);
			CompressionMission.ActionSetCompressionInfo = new CompressionInfo.Integer(0, MBActionSet.GetNumberOfActionSets(), true);
			CompressionMission.MonsterUsageSetCompressionInfo = new CompressionInfo.Integer(0, MBActionSet.GetNumberOfMonsterUsageSets(), true);
		}

		[MBCallback]
		internal static void SyncRelevantGameOptionsToServer()
		{
			SyncRelevantGameOptionsToServer syncRelevantGameOptionsToServer = new SyncRelevantGameOptionsToServer();
			syncRelevantGameOptionsToServer.InitializeOptions();
			GameNetwork.BeginModuleEventAsClient();
			GameNetwork.WriteMessage(syncRelevantGameOptionsToServer);
			GameNetwork.EndModuleEventAsClient();
		}

		private static void CollectGameNetworkMessagesFromAssembly(Assembly assembly, List<Type> gameNetworkMessagesFromClient, List<Type> gameNetworkMessagesFromServer)
		{
			Type typeFromHandle = typeof(GameNetworkMessage);
			bool? flag = null;
			foreach (Type type in assembly.GetTypes())
			{
				if (typeFromHandle.IsAssignableFrom(type) && type != typeFromHandle && type.IsSealed && !(type.GetConstructor(Type.EmptyTypes) == null))
				{
					DefineGameNetworkMessageType customAttribute = type.GetCustomAttribute<DefineGameNetworkMessageType>();
					if (customAttribute != null)
					{
						if (flag == null || !flag.Value)
						{
							flag = new bool?(false);
							GameNetworkMessageSendType sendType = customAttribute.SendType;
							if (sendType != GameNetworkMessageSendType.FromClient)
							{
								if (sendType - GameNetworkMessageSendType.FromServer <= 1)
								{
									gameNetworkMessagesFromServer.Add(type);
								}
							}
							else
							{
								gameNetworkMessagesFromClient.Add(type);
							}
						}
					}
					else
					{
						DefineGameNetworkMessageTypeForMod customAttribute2 = type.GetCustomAttribute<DefineGameNetworkMessageTypeForMod>();
						if (customAttribute2 != null && (flag == null || flag.Value))
						{
							flag = new bool?(true);
							GameNetworkMessageSendType sendType2 = customAttribute2.SendType;
							if (sendType2 != GameNetworkMessageSendType.FromClient)
							{
								if (sendType2 - GameNetworkMessageSendType.FromServer <= 1)
								{
									gameNetworkMessagesFromServer.Add(type);
								}
							}
							else
							{
								gameNetworkMessagesFromClient.Add(type);
							}
						}
					}
				}
			}
		}

		public static NetworkCommunicator MyPeer { get; private set; }

		public static bool IsMyPeerReady
		{
			get
			{
				return GameNetwork.MyPeer != null && GameNetwork.MyPeer.IsSynchronized;
			}
		}

		public const int MaxAutomatedBattleIndex = 10;

		private static IGameNetworkHandler _handler;

		public static int ClientPeerIndex;

		private const MultiplayerMessageFilter MultiplayerLogging = MultiplayerMessageFilter.All;

		private static Dictionary<Type, int> _gameNetworkMessageTypesAll;

		private static Dictionary<Type, int> _gameNetworkMessageTypesFromClient;

		private static List<Type> _gameNetworkMessageIdsFromClient;

		private static Dictionary<Type, int> _gameNetworkMessageTypesFromServer;

		private static List<Type> _gameNetworkMessageIdsFromServer;

		private static Dictionary<int, List<object>> _fromClientMessageHandlers;

		private static Dictionary<int, List<object>> _fromServerMessageHandlers;

		private static Dictionary<int, List<GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>>> _fromClientBaseMessageHandlers;

		private static Dictionary<int, List<GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>>> _fromServerBaseMessageHandlers;

		public class NetworkMessageHandlerRegisterer
		{
			public NetworkMessageHandlerRegisterer(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode definitionMode)
			{
				this._registerMode = definitionMode;
			}

			public void Register<T>(GameNetworkMessage.ServerMessageHandlerDelegate<T> handler) where T : GameNetworkMessage
			{
				if (this._registerMode == GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add)
				{
					GameNetwork.AddServerMessageHandler<T>(handler);
					return;
				}
				GameNetwork.RemoveServerMessageHandler<T>(handler);
			}

			public void RegisterBaseHandler<T>(GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage> handler) where T : GameNetworkMessage
			{
				if (this._registerMode == GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add)
				{
					GameNetwork.AddServerBaseMessageHandler(handler, typeof(T));
					return;
				}
				GameNetwork.RemoveServerBaseMessageHandler(handler, typeof(T));
			}

			public void Register<T>(GameNetworkMessage.ClientMessageHandlerDelegate<T> handler) where T : GameNetworkMessage
			{
				if (this._registerMode == GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add)
				{
					GameNetwork.AddClientMessageHandler<T>(handler);
					return;
				}
				GameNetwork.RemoveClientMessageHandler<T>(handler);
			}

			public void RegisterBaseHandler<T>(GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage> handler) where T : GameNetworkMessage
			{
				if (this._registerMode == GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add)
				{
					GameNetwork.AddClientBaseMessageHandler(handler, typeof(T));
					return;
				}
				GameNetwork.RemoveClientBaseMessageHandler(handler, typeof(T));
			}

			private readonly GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode _registerMode;

			public enum RegisterMode
			{
				Add,
				Remove
			}
		}

		public class NetworkMessageHandlerRegistererContainer
		{
			public NetworkMessageHandlerRegistererContainer()
			{
				this._fromClientHandlers = new List<Delegate>();
				this._fromServerHandlers = new List<Delegate>();
				this._fromServerBaseHandlers = new List<Tuple<GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>, Type>>();
				this._fromClientBaseHandlers = new List<Tuple<GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>, Type>>();
			}

			public void RegisterBaseHandler<T>(GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage> handler) where T : GameNetworkMessage
			{
				this._fromServerBaseHandlers.Add(new Tuple<GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>, Type>(handler, typeof(T)));
			}

			public void Register<T>(GameNetworkMessage.ServerMessageHandlerDelegate<T> handler) where T : GameNetworkMessage
			{
				this._fromServerHandlers.Add(handler);
			}

			public void RegisterBaseHandler<T>(GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage> handler)
			{
				this._fromClientBaseHandlers.Add(new Tuple<GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>, Type>(handler, typeof(T)));
			}

			public void Register<T>(GameNetworkMessage.ClientMessageHandlerDelegate<T> handler) where T : GameNetworkMessage
			{
				this._fromClientHandlers.Add(handler);
			}

			public void RegisterMessages()
			{
				if (this._fromServerHandlers.Count > 0 || this._fromServerBaseHandlers.Count > 0)
				{
					foreach (Delegate @delegate in this._fromServerHandlers)
					{
						Type type = @delegate.GetType().GenericTypeArguments[0];
						int num = GameNetwork._gameNetworkMessageTypesFromServer[type];
						GameNetwork._fromServerMessageHandlers[num].Add(@delegate);
					}
					using (List<Tuple<GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>, Type>>.Enumerator enumerator2 = this._fromServerBaseHandlers.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							Tuple<GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>, Type> tuple = enumerator2.Current;
							int num2 = GameNetwork._gameNetworkMessageTypesFromServer[tuple.Item2];
							GameNetwork._fromServerBaseMessageHandlers[num2].Add(tuple.Item1);
						}
						return;
					}
				}
				foreach (Delegate delegate2 in this._fromClientHandlers)
				{
					Type type2 = delegate2.GetType().GenericTypeArguments[0];
					int num3 = GameNetwork._gameNetworkMessageTypesFromClient[type2];
					GameNetwork._fromClientMessageHandlers[num3].Add(delegate2);
				}
				foreach (Tuple<GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>, Type> tuple2 in this._fromClientBaseHandlers)
				{
					int num4 = GameNetwork._gameNetworkMessageTypesFromClient[tuple2.Item2];
					GameNetwork._fromClientBaseMessageHandlers[num4].Add(tuple2.Item1);
				}
			}

			public void UnregisterMessages()
			{
				if (this._fromServerHandlers.Count > 0 || this._fromServerBaseHandlers.Count > 0)
				{
					foreach (Delegate @delegate in this._fromServerHandlers)
					{
						Type type = @delegate.GetType().GenericTypeArguments[0];
						int num = GameNetwork._gameNetworkMessageTypesFromServer[type];
						GameNetwork._fromServerMessageHandlers[num].Remove(@delegate);
					}
					using (List<Tuple<GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>, Type>>.Enumerator enumerator2 = this._fromServerBaseHandlers.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							Tuple<GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>, Type> tuple = enumerator2.Current;
							int num2 = GameNetwork._gameNetworkMessageTypesFromServer[tuple.Item2];
							GameNetwork._fromServerBaseMessageHandlers[num2].Remove(tuple.Item1);
						}
						return;
					}
				}
				foreach (Delegate delegate2 in this._fromClientHandlers)
				{
					Type type2 = delegate2.GetType().GenericTypeArguments[0];
					int num3 = GameNetwork._gameNetworkMessageTypesFromClient[type2];
					GameNetwork._fromClientMessageHandlers[num3].Remove(delegate2);
				}
				foreach (Tuple<GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>, Type> tuple2 in this._fromClientBaseHandlers)
				{
					int num4 = GameNetwork._gameNetworkMessageTypesFromClient[tuple2.Item2];
					GameNetwork._fromClientBaseMessageHandlers[num4].Remove(tuple2.Item1);
				}
			}

			private List<Delegate> _fromClientHandlers;

			private List<Delegate> _fromServerHandlers;

			private List<Tuple<GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>, Type>> _fromServerBaseHandlers;

			private List<Tuple<GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>, Type>> _fromClientBaseHandlers;
		}

		[Flags]
		public enum EventBroadcastFlags
		{
			None = 0,
			ExcludeTargetPlayer = 1,
			ExcludeNoBloodStainsOption = 2,
			ExcludeNoParticlesOption = 4,
			ExcludeNoSoundOption = 8,
			AddToMissionRecord = 16,
			IncludeUnsynchronizedClients = 32,
			ExcludeOtherTeamPlayers = 64,
			ExcludePeerTeamPlayers = 128,
			DontSendToPeers = 256
		}

		[EngineStruct("Debug_network_position_compression_statistics_struct")]
		public struct DebugNetworkPositionCompressionStatisticsStruct
		{
			public int totalPositionUpload;

			public int totalPositionPrecisionBitCount;

			public int totalPositionCoarseBitCountX;

			public int totalPositionCoarseBitCountY;

			public int totalPositionCoarseBitCountZ;
		}

		[EngineStruct("Debug_network_packet_statistics_struct")]
		public struct DebugNetworkPacketStatisticsStruct
		{
			public int TotalPackets;

			public int TotalUpload;

			public int TotalConstantsUpload;

			public int TotalReliableEventUpload;

			public int TotalReplicationUpload;

			public int TotalUnreliableEventUpload;

			public int TotalReplicationTableAdderCount;

			public int debug_total_replication_table_adder_bit_count;

			public int debug_total_replication_table_adder;

			public double debug_total_cell_priority;

			public double debug_total_cell_agent_priority;

			public double debug_total_cell_cell_priority;

			public int debug_total_cell_priority_checks;

			public int debug_total_sent_cell_count;

			public int debug_total_not_sent_cell_count;

			public int debug_total_replication_write_count;

			public int debug_cur_max_packet_size_in_bytes;

			public double average_ping_time;

			public double debug_average_dt_to_send_packet;

			public double time_out_period;

			public double pacing_rate;

			public double delivery_rate;

			public double round_trip_time;

			public int inflight_bit_count;

			public int is_congested;

			public int probe_bw_phase_index;

			public double lost_percent;

			public int lost_count;

			public int total_count_on_lost_check;
		}

		public struct AddPlayersResult
		{
			public bool Success;

			public NetworkCommunicator[] NetworkPeers;
		}
	}
}
