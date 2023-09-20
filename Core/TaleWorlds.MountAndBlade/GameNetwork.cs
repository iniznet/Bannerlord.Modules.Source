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
	// Token: 0x020002EC RID: 748
	public static class GameNetwork
	{
		// Token: 0x1700074D RID: 1869
		// (get) Token: 0x0600287E RID: 10366 RVA: 0x0009D34B File Offset: 0x0009B54B
		public static bool IsServer
		{
			get
			{
				return MBCommon.CurrentGameType == MBCommon.GameType.MultiServer || MBCommon.CurrentGameType == MBCommon.GameType.MultiClientServer;
			}
		}

		// Token: 0x1700074E RID: 1870
		// (get) Token: 0x0600287F RID: 10367 RVA: 0x0009D35F File Offset: 0x0009B55F
		public static bool IsServerOrRecorder
		{
			get
			{
				return GameNetwork.IsServer || MBCommon.CurrentGameType == MBCommon.GameType.SingleRecord;
			}
		}

		// Token: 0x1700074F RID: 1871
		// (get) Token: 0x06002880 RID: 10368 RVA: 0x0009D372 File Offset: 0x0009B572
		public static bool IsClient
		{
			get
			{
				return MBCommon.CurrentGameType == MBCommon.GameType.MultiClient;
			}
		}

		// Token: 0x17000750 RID: 1872
		// (get) Token: 0x06002881 RID: 10369 RVA: 0x0009D37C File Offset: 0x0009B57C
		public static bool IsReplay
		{
			get
			{
				return MBCommon.CurrentGameType == MBCommon.GameType.SingleReplay;
			}
		}

		// Token: 0x17000751 RID: 1873
		// (get) Token: 0x06002882 RID: 10370 RVA: 0x0009D386 File Offset: 0x0009B586
		public static bool IsClientOrReplay
		{
			get
			{
				return GameNetwork.IsClient || GameNetwork.IsReplay;
			}
		}

		// Token: 0x17000752 RID: 1874
		// (get) Token: 0x06002883 RID: 10371 RVA: 0x0009D396 File Offset: 0x0009B596
		public static bool IsDedicatedServer
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000753 RID: 1875
		// (get) Token: 0x06002884 RID: 10372 RVA: 0x0009D399 File Offset: 0x0009B599
		public static bool MultiplayerDisabled
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000754 RID: 1876
		// (get) Token: 0x06002885 RID: 10373 RVA: 0x0009D39C File Offset: 0x0009B59C
		public static bool IsMultiplayer
		{
			get
			{
				return GameNetwork.IsServer || GameNetwork.IsClient;
			}
		}

		// Token: 0x17000755 RID: 1877
		// (get) Token: 0x06002886 RID: 10374 RVA: 0x0009D3AC File Offset: 0x0009B5AC
		public static bool IsSessionActive
		{
			get
			{
				return GameNetwork.IsServerOrRecorder || GameNetwork.IsClientOrReplay;
			}
		}

		// Token: 0x17000756 RID: 1878
		// (get) Token: 0x06002887 RID: 10375 RVA: 0x0009D3BC File Offset: 0x0009B5BC
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

		// Token: 0x17000757 RID: 1879
		// (get) Token: 0x06002888 RID: 10376 RVA: 0x0009D3C5 File Offset: 0x0009B5C5
		public static IEnumerable<NetworkCommunicator> NetworkPeers
		{
			get
			{
				return MBNetwork.NetworkPeers.OfType<NetworkCommunicator>();
			}
		}

		// Token: 0x17000758 RID: 1880
		// (get) Token: 0x06002889 RID: 10377 RVA: 0x0009D3D1 File Offset: 0x0009B5D1
		public static int NetworkPeerCount
		{
			get
			{
				return MBNetwork.NetworkPeers.Count;
			}
		}

		// Token: 0x17000759 RID: 1881
		// (get) Token: 0x0600288A RID: 10378 RVA: 0x0009D3DD File Offset: 0x0009B5DD
		public static bool NetworkPeersValid
		{
			get
			{
				return MBNetwork.NetworkPeers != null;
			}
		}

		// Token: 0x0600288B RID: 10379 RVA: 0x0009D3E7 File Offset: 0x0009B5E7
		private static void AddNetworkPeer(NetworkCommunicator networkPeer)
		{
			MBNetwork.NetworkPeers.Add(networkPeer);
			Debug.Print("> AddNetworkPeer: " + networkPeer.UserName, 0, Debug.DebugColor.White, 17179869184UL);
		}

		// Token: 0x0600288C RID: 10380 RVA: 0x0009D415 File Offset: 0x0009B615
		private static void RemoveNetworkPeer(NetworkCommunicator networkPeer)
		{
			Debug.Print("> RemoveNetworkPeer: " + networkPeer.UserName, 0, Debug.DebugColor.White, 17179869184UL);
			MBNetwork.NetworkPeers.Remove(networkPeer);
		}

		// Token: 0x0600288D RID: 10381 RVA: 0x0009D444 File Offset: 0x0009B644
		private static void AddToDisconnectedPeers(NetworkCommunicator networkPeer)
		{
			Debug.Print("> AddToDisconnectedPeers: " + networkPeer.UserName, 0, Debug.DebugColor.White, 17179869184UL);
			MBNetwork.DisconnectedNetworkPeers.Add(networkPeer);
		}

		// Token: 0x0600288E RID: 10382 RVA: 0x0009D474 File Offset: 0x0009B674
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

		// Token: 0x0600288F RID: 10383 RVA: 0x0009D4B8 File Offset: 0x0009B6B8
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

		// Token: 0x06002890 RID: 10384 RVA: 0x0009D510 File Offset: 0x0009B710
		public static void Initialize(IGameNetworkHandler handler)
		{
			GameNetwork._handler = handler;
			MBNetwork.Initialize(new NetworkCommunication());
			GameNetwork.NetworkComponents = new List<UdpNetworkComponent>();
			GameNetwork.NetworkHandlers = new List<IUdpNetworkHandler>();
			GameNetwork._handler.OnInitialize();
		}

		// Token: 0x06002891 RID: 10385 RVA: 0x0009D540 File Offset: 0x0009B740
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

		// Token: 0x06002892 RID: 10386 RVA: 0x0009D610 File Offset: 0x0009B810
		private static void StartMultiplayer()
		{
			VirtualPlayer.Reset();
			GameNetwork._handler.OnStartMultiplayer();
		}

		// Token: 0x06002893 RID: 10387 RVA: 0x0009D624 File Offset: 0x0009B824
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

		// Token: 0x06002894 RID: 10388 RVA: 0x0009D6FC File Offset: 0x0009B8FC
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

		// Token: 0x06002895 RID: 10389 RVA: 0x0009D780 File Offset: 0x0009B980
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

		// Token: 0x06002896 RID: 10390 RVA: 0x0009D8F8 File Offset: 0x0009BAF8
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

		// Token: 0x06002897 RID: 10391 RVA: 0x0009D958 File Offset: 0x0009BB58
		public static void StartReplay()
		{
			GameNetwork._handler.OnStartReplay();
		}

		// Token: 0x06002898 RID: 10392 RVA: 0x0009D964 File Offset: 0x0009BB64
		public static void EndReplay()
		{
			GameNetwork._handler.OnEndReplay();
		}

		// Token: 0x06002899 RID: 10393 RVA: 0x0009D970 File Offset: 0x0009BB70
		public static void PreStartMultiplayerOnServer()
		{
			MBCommon.CurrentGameType = (GameNetwork.IsDedicatedServer ? MBCommon.GameType.MultiServer : MBCommon.GameType.MultiClientServer);
			GameNetwork.ClientPeerIndex = -1;
		}

		// Token: 0x0600289A RID: 10394 RVA: 0x0009D988 File Offset: 0x0009BB88
		public static void StartMultiplayerOnServer(int port)
		{
			Debug.Print("StartMultiplayerOnServer", 0, Debug.DebugColor.White, 17592186044416UL);
			GameNetwork.PreStartMultiplayerOnServer();
			GameNetwork.InitializeServerSide(port);
			GameNetwork.StartMultiplayer();
		}

		// Token: 0x0600289B RID: 10395 RVA: 0x0009D9B0 File Offset: 0x0009BBB0
		[MBCallback]
		internal static bool HandleNetworkPacketAsServer(MBNetworkPeer networkPeer)
		{
			return GameNetwork.HandleNetworkPacketAsServer(networkPeer.NetworkPeer);
		}

		// Token: 0x0600289C RID: 10396 RVA: 0x0009D9C0 File Offset: 0x0009BBC0
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
							List<object> list;
							if (GameNetwork._fromClientMessageHandlers.TryGetValue(num, out list))
							{
								foreach (object obj in list)
								{
									Delegate @delegate = obj as Delegate;
									flag = flag && (bool)@delegate.DynamicInvokeWithLog(new object[] { networkPeer, gameNetworkMessage });
									if (!flag)
									{
										break;
									}
								}
								if (list.Count == 0)
								{
									Debug.Print("Handler not found for network message " + gameNetworkMessage, 0, Debug.DebugColor.White, 17179869184UL);
								}
							}
							else
							{
								Debug.FailedAssert("Unknown network messageId " + gameNetworkMessage, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\GameNetwork.cs", "HandleNetworkPacketAsServer", 626);
								flag = false;
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

		// Token: 0x0600289D RID: 10397 RVA: 0x0009DB70 File Offset: 0x0009BD70
		[MBCallback]
		public static void HandleConsoleCommand(string command)
		{
			if (GameNetwork._handler != null)
			{
				GameNetwork._handler.OnHandleConsoleCommand(command);
			}
		}

		// Token: 0x0600289E RID: 10398 RVA: 0x0009DB84 File Offset: 0x0009BD84
		private static void InitializeServerSide(int port)
		{
			MBAPI.IMBNetwork.InitializeServerSide(port);
		}

		// Token: 0x0600289F RID: 10399 RVA: 0x0009DB91 File Offset: 0x0009BD91
		private static void TerminateServerSide()
		{
			MBAPI.IMBNetwork.TerminateServerSide();
			if (!GameNetwork.IsDedicatedServer)
			{
				MBCommon.CurrentGameType = MBCommon.GameType.Single;
			}
		}

		// Token: 0x060028A0 RID: 10400 RVA: 0x0009DBAA File Offset: 0x0009BDAA
		private static void PrepareNewUdpSession(int peerIndex, int sessionKey)
		{
			MBAPI.IMBNetwork.PrepareNewUdpSession(peerIndex, sessionKey);
		}

		// Token: 0x060028A1 RID: 10401 RVA: 0x0009DBB8 File Offset: 0x0009BDB8
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

		// Token: 0x060028A2 RID: 10402 RVA: 0x0009DF04 File Offset: 0x0009C104
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

		// Token: 0x060028A3 RID: 10403 RVA: 0x0009DF68 File Offset: 0x0009C168
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

		// Token: 0x060028A4 RID: 10404 RVA: 0x0009E0BC File Offset: 0x0009C2BC
		public static void BeginModuleEventAsClient()
		{
			MBAPI.IMBNetwork.BeginModuleEventAsClient(true);
		}

		// Token: 0x060028A5 RID: 10405 RVA: 0x0009E0C9 File Offset: 0x0009C2C9
		public static void EndModuleEventAsClient()
		{
			MBAPI.IMBNetwork.EndModuleEventAsClient(true);
		}

		// Token: 0x060028A6 RID: 10406 RVA: 0x0009E0D6 File Offset: 0x0009C2D6
		public static void BeginModuleEventAsClientUnreliable()
		{
			MBAPI.IMBNetwork.BeginModuleEventAsClient(false);
		}

		// Token: 0x060028A7 RID: 10407 RVA: 0x0009E0E3 File Offset: 0x0009C2E3
		public static void EndModuleEventAsClientUnreliable()
		{
			MBAPI.IMBNetwork.EndModuleEventAsClient(false);
		}

		// Token: 0x060028A8 RID: 10408 RVA: 0x0009E0F0 File Offset: 0x0009C2F0
		public static void BeginModuleEventAsServer(NetworkCommunicator communicator)
		{
			GameNetwork.BeginModuleEventAsServer(communicator.VirtualPlayer);
		}

		// Token: 0x060028A9 RID: 10409 RVA: 0x0009E0FD File Offset: 0x0009C2FD
		public static void BeginModuleEventAsServerUnreliable(NetworkCommunicator communicator)
		{
			GameNetwork.BeginModuleEventAsServerUnreliable(communicator.VirtualPlayer);
		}

		// Token: 0x060028AA RID: 10410 RVA: 0x0009E10A File Offset: 0x0009C30A
		public static void BeginModuleEventAsServer(VirtualPlayer peer)
		{
			MBAPI.IMBPeer.BeginModuleEvent(peer.Index, true);
		}

		// Token: 0x060028AB RID: 10411 RVA: 0x0009E11D File Offset: 0x0009C31D
		public static void EndModuleEventAsServer()
		{
			MBAPI.IMBPeer.EndModuleEvent(true);
		}

		// Token: 0x060028AC RID: 10412 RVA: 0x0009E12A File Offset: 0x0009C32A
		public static void BeginModuleEventAsServerUnreliable(VirtualPlayer peer)
		{
			MBAPI.IMBPeer.BeginModuleEvent(peer.Index, false);
		}

		// Token: 0x060028AD RID: 10413 RVA: 0x0009E13D File Offset: 0x0009C33D
		public static void EndModuleEventAsServerUnreliable()
		{
			MBAPI.IMBPeer.EndModuleEvent(false);
		}

		// Token: 0x060028AE RID: 10414 RVA: 0x0009E14A File Offset: 0x0009C34A
		public static void BeginBroadcastModuleEvent()
		{
			MBAPI.IMBNetwork.BeginBroadcastModuleEvent();
		}

		// Token: 0x060028AF RID: 10415 RVA: 0x0009E158 File Offset: 0x0009C358
		public static void EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags broadcastFlags, NetworkCommunicator targetPlayer = null)
		{
			int num = ((targetPlayer != null) ? targetPlayer.Index : (-1));
			MBAPI.IMBNetwork.EndBroadcastModuleEvent((int)broadcastFlags, num, true);
		}

		// Token: 0x060028B0 RID: 10416 RVA: 0x0009E180 File Offset: 0x0009C380
		public static void EndBroadcastModuleEventUnreliable(GameNetwork.EventBroadcastFlags broadcastFlags, NetworkCommunicator targetPlayer = null)
		{
			int num = ((targetPlayer != null) ? targetPlayer.Index : (-1));
			MBAPI.IMBNetwork.EndBroadcastModuleEvent((int)broadcastFlags, num, false);
		}

		// Token: 0x060028B1 RID: 10417 RVA: 0x0009E1A8 File Offset: 0x0009C3A8
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

		// Token: 0x060028B2 RID: 10418 RVA: 0x0009E248 File Offset: 0x0009C448
		public static void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode mode)
		{
			GameNetwork.NetworkMessageHandlerRegisterer networkMessageHandlerRegisterer = new GameNetwork.NetworkMessageHandlerRegisterer(mode);
			networkMessageHandlerRegisterer.Register<CreatePlayer>(new GameNetworkMessage.ServerMessageHandlerDelegate<CreatePlayer>(GameNetwork.HandleServerEventCreatePlayer));
			networkMessageHandlerRegisterer.Register<DeletePlayer>(new GameNetworkMessage.ServerMessageHandlerDelegate<DeletePlayer>(GameNetwork.HandleServerEventDeletePlayer));
		}

		// Token: 0x060028B3 RID: 10419 RVA: 0x0009E273 File Offset: 0x0009C473
		public static void StartMultiplayerOnClient(string serverAddress, int port, int sessionKey, int playerIndex)
		{
			Debug.Print("StartMultiplayerOnClient", 0, Debug.DebugColor.White, 17592186044416UL);
			MBCommon.CurrentGameType = MBCommon.GameType.MultiClient;
			GameNetwork.ClientPeerIndex = playerIndex;
			GameNetwork.InitializeClientSide(serverAddress, port, sessionKey, playerIndex);
			GameNetwork.StartMultiplayer();
			GameNetwork.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add);
		}

		// Token: 0x060028B4 RID: 10420 RVA: 0x0009E2AC File Offset: 0x0009C4AC
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
						if ((gameNetworkMessage.GetLogFilter() & (MultiplayerMessageFilter)(-1)) != MultiplayerMessageFilter.None)
						{
							if (GameNetworkMessage.IsClientMissionOver)
							{
								Debug.Print("WARNING: Entering message processing while client mission is over", 0, Debug.DebugColor.White, 17592186044416UL);
							}
							Debug.Print("Processing message: " + gameNetworkMessage.GetType().Name + ": " + gameNetworkMessage.GetLogFormat(), 0, Debug.DebugColor.White, 17179869184UL);
						}
						List<object> list;
						if (GameNetwork._fromServerMessageHandlers.TryGetValue(num, out list))
						{
							foreach (object obj in list)
							{
								try
								{
									(obj as Delegate).DynamicInvokeWithLog(new object[] { gameNetworkMessage });
								}
								catch
								{
									Debug.Print("Exception in handler of " + num.ToString(), 0, Debug.DebugColor.White, 17179869184UL);
									Debug.Print("Exception in handler of " + gameNetworkMessage.GetType().Name, 0, Debug.DebugColor.Red, 17179869184UL);
									throw;
								}
							}
							if (list.Count == 0)
							{
								Debug.Print("No message handler found for " + gameNetworkMessage.GetType().Name, 0, Debug.DebugColor.Red, 17179869184UL);
							}
						}
						else
						{
							Debug.Print("Invalid messageId " + num.ToString(), 0, Debug.DebugColor.White, 17179869184UL);
							Debug.Print("Invalid messageId " + gameNetworkMessage.GetType().Name, 0, Debug.DebugColor.White, 17179869184UL);
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

		// Token: 0x060028B5 RID: 10421 RVA: 0x0009E554 File Offset: 0x0009C754
		private static int GetSessionKeyForPlayer()
		{
			return new Random(DateTime.Now.Millisecond).Next(1, 4001);
		}

		// Token: 0x060028B6 RID: 10422 RVA: 0x0009E580 File Offset: 0x0009C780
		public static NetworkCommunicator HandleNewClientConnect(PlayerConnectionInfo playerConnectionInfo, bool isAdmin)
		{
			NetworkCommunicator networkCommunicator = GameNetwork.AddNewPlayerOnServer(playerConnectionInfo, false, isAdmin) as NetworkCommunicator;
			GameNetwork._handler.OnNewPlayerConnect(playerConnectionInfo, networkCommunicator);
			return networkCommunicator;
		}

		// Token: 0x060028B7 RID: 10423 RVA: 0x0009E5A8 File Offset: 0x0009C7A8
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

		// Token: 0x060028B8 RID: 10424 RVA: 0x0009E5EC File Offset: 0x0009C7EC
		public static void AddNetworkPeerToDisconnectAsServer(NetworkCommunicator networkPeer)
		{
			Debug.Print("adding peer to disconnect index:" + networkPeer.Index, 0, Debug.DebugColor.White, 17179869184UL);
			GameNetwork.AddPeerToDisconnect(networkPeer);
			GameNetwork.BeginModuleEventAsServer(networkPeer);
			GameNetwork.WriteMessage(new DeletePlayer(networkPeer.Index, false));
			GameNetwork.EndModuleEventAsServer();
		}

		// Token: 0x060028B9 RID: 10425 RVA: 0x0009E644 File Offset: 0x0009C844
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

		// Token: 0x060028BA RID: 10426 RVA: 0x0009E718 File Offset: 0x0009C918
		private static void HandleServerEventDeletePlayer(DeletePlayer message)
		{
			NetworkCommunicator networkCommunicator = GameNetwork.NetworkPeers.FirstOrDefault((NetworkCommunicator networkPeer) => networkPeer.Index == message.PlayerIndex);
			if (networkCommunicator != null)
			{
				GameNetwork.HandleRemovePlayerInternal(networkCommunicator, message.AddToDisconnectList);
			}
		}

		// Token: 0x060028BB RID: 10427 RVA: 0x0009E75D File Offset: 0x0009C95D
		public static void InitializeClientSide(string serverAddress, int port, int sessionKey, int playerIndex)
		{
			MBAPI.IMBNetwork.InitializeClientSide(serverAddress, port, sessionKey, playerIndex);
		}

		// Token: 0x060028BC RID: 10428 RVA: 0x0009E76D File Offset: 0x0009C96D
		public static void TerminateClientSide()
		{
			MBAPI.IMBNetwork.TerminateClientSide();
			MBCommon.CurrentGameType = MBCommon.GameType.Single;
		}

		// Token: 0x060028BD RID: 10429 RVA: 0x0009E77F File Offset: 0x0009C97F
		public static void DestroyComponent(UdpNetworkComponent udpNetworkComponent)
		{
			GameNetwork.RemoveNetworkHandler(udpNetworkComponent);
			GameNetwork.NetworkComponents.Remove(udpNetworkComponent);
		}

		// Token: 0x060028BE RID: 10430 RVA: 0x0009E794 File Offset: 0x0009C994
		public static T AddNetworkComponent<T>() where T : UdpNetworkComponent
		{
			T t = (T)((object)Activator.CreateInstance(typeof(T), new object[0]));
			GameNetwork.NetworkComponents.Add(t);
			GameNetwork.NetworkHandlers.Add(t);
			return t;
		}

		// Token: 0x060028BF RID: 10431 RVA: 0x0009E7DD File Offset: 0x0009C9DD
		public static void AddNetworkHandler(IUdpNetworkHandler handler)
		{
			GameNetwork.NetworkHandlers.Add(handler);
		}

		// Token: 0x060028C0 RID: 10432 RVA: 0x0009E7EA File Offset: 0x0009C9EA
		public static void RemoveNetworkHandler(IUdpNetworkHandler handler)
		{
			handler.OnUdpNetworkHandlerClose();
			GameNetwork.NetworkHandlers.Remove(handler);
		}

		// Token: 0x060028C1 RID: 10433 RVA: 0x0009E800 File Offset: 0x0009CA00
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

		// Token: 0x1700075A RID: 1882
		// (get) Token: 0x060028C2 RID: 10434 RVA: 0x0009E86C File Offset: 0x0009CA6C
		// (set) Token: 0x060028C3 RID: 10435 RVA: 0x0009E873 File Offset: 0x0009CA73
		public static List<UdpNetworkComponent> NetworkComponents { get; private set; }

		// Token: 0x1700075B RID: 1883
		// (get) Token: 0x060028C4 RID: 10436 RVA: 0x0009E87B File Offset: 0x0009CA7B
		// (set) Token: 0x060028C5 RID: 10437 RVA: 0x0009E882 File Offset: 0x0009CA82
		public static List<IUdpNetworkHandler> NetworkHandlers { get; private set; }

		// Token: 0x060028C6 RID: 10438 RVA: 0x0009E88C File Offset: 0x0009CA8C
		public static void WriteMessage(GameNetworkMessage message)
		{
			Type type = message.GetType();
			message.MessageId = GameNetwork._gameNetworkMessageTypesAll[type];
			message.Write();
		}

		// Token: 0x060028C7 RID: 10439 RVA: 0x0009E8B8 File Offset: 0x0009CAB8
		private static void AddServerMessageHandler<T>(GameNetworkMessage.ServerMessageHandlerDelegate<T> handler) where T : GameNetworkMessage
		{
			int num = GameNetwork._gameNetworkMessageTypesFromServer[typeof(T)];
			GameNetwork._fromServerMessageHandlers[num].Add(handler);
		}

		// Token: 0x060028C8 RID: 10440 RVA: 0x0009E8EC File Offset: 0x0009CAEC
		private static void AddClientMessageHandler<T>(GameNetworkMessage.ClientMessageHandlerDelegate<T> handler) where T : GameNetworkMessage
		{
			int num = GameNetwork._gameNetworkMessageTypesFromClient[typeof(T)];
			GameNetwork._fromClientMessageHandlers[num].Add(handler);
		}

		// Token: 0x060028C9 RID: 10441 RVA: 0x0009E920 File Offset: 0x0009CB20
		private static void RemoveServerMessageHandler<T>(GameNetworkMessage.ServerMessageHandlerDelegate<T> handler) where T : GameNetworkMessage
		{
			int num = GameNetwork._gameNetworkMessageTypesFromServer[typeof(T)];
			GameNetwork._fromServerMessageHandlers[num].Remove(handler);
		}

		// Token: 0x060028CA RID: 10442 RVA: 0x0009E954 File Offset: 0x0009CB54
		private static void RemoveClientMessageHandler<T>(GameNetworkMessage.ClientMessageHandlerDelegate<T> handler) where T : GameNetworkMessage
		{
			int num = GameNetwork._gameNetworkMessageTypesFromClient[typeof(T)];
			GameNetwork._fromClientMessageHandlers[num].Remove(handler);
		}

		// Token: 0x060028CB RID: 10443 RVA: 0x0009E988 File Offset: 0x0009CB88
		internal static void FindGameNetworkMessages()
		{
			Debug.Print("Searching Game NetworkMessages Methods", 0, Debug.DebugColor.White, 17179869184UL);
			GameNetwork._fromClientMessageHandlers = new Dictionary<int, List<object>>();
			GameNetwork._fromServerMessageHandlers = new Dictionary<int, List<object>>();
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
			}
			GameNetwork._gameNetworkMessageIdsFromServer = new List<Type>(list2.Count);
			for (int k = 0; k < list2.Count; k++)
			{
				Type type2 = list2[k];
				GameNetwork._gameNetworkMessageIdsFromServer.Add(type2);
				GameNetwork._gameNetworkMessageTypesFromServer.Add(type2, k);
				GameNetwork._gameNetworkMessageTypesAll.Add(type2, k);
				GameNetwork._fromServerMessageHandlers.Add(k, new List<object>());
			}
			CompressionBasic.NetworkComponentEventTypeFromClientCompressionInfo = new CompressionInfo.Integer(0, list.Count - 1, true);
			CompressionBasic.NetworkComponentEventTypeFromServerCompressionInfo = new CompressionInfo.Integer(0, list2.Count - 1, true);
			Debug.Print("Found " + list.Count + " Client Game Network Messages", 0, Debug.DebugColor.White, 17179869184UL);
			Debug.Print("Found " + list2.Count + " Server Game Network Messages", 0, Debug.DebugColor.White, 17179869184UL);
		}

		// Token: 0x060028CC RID: 10444 RVA: 0x0009EBB4 File Offset: 0x0009CDB4
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

		// Token: 0x060028CD RID: 10445 RVA: 0x0009EC09 File Offset: 0x0009CE09
		public static void IncreaseTotalUploadLimit(int value)
		{
			MBAPI.IMBNetwork.IncreaseTotalUploadLimit(value);
		}

		// Token: 0x060028CE RID: 10446 RVA: 0x0009EC16 File Offset: 0x0009CE16
		public static void ResetDebugVariables()
		{
			MBAPI.IMBNetwork.ResetDebugVariables();
		}

		// Token: 0x060028CF RID: 10447 RVA: 0x0009EC22 File Offset: 0x0009CE22
		public static void PrintDebugStats()
		{
			MBAPI.IMBNetwork.PrintDebugStats();
		}

		// Token: 0x060028D0 RID: 10448 RVA: 0x0009EC2E File Offset: 0x0009CE2E
		public static float GetAveragePacketLossRatio()
		{
			return MBAPI.IMBNetwork.GetAveragePacketLossRatio();
		}

		// Token: 0x060028D1 RID: 10449 RVA: 0x0009EC3A File Offset: 0x0009CE3A
		public static void GetDebugUploadsInBits(ref GameNetwork.DebugNetworkPacketStatisticsStruct networkStatisticsStruct, ref GameNetwork.DebugNetworkPositionCompressionStatisticsStruct posStatisticsStruct)
		{
			MBAPI.IMBNetwork.GetDebugUploadsInBits(ref networkStatisticsStruct, ref posStatisticsStruct);
		}

		// Token: 0x060028D2 RID: 10450 RVA: 0x0009EC48 File Offset: 0x0009CE48
		public static void PrintReplicationTableStatistics()
		{
			MBAPI.IMBNetwork.PrintReplicationTableStatistics();
		}

		// Token: 0x060028D3 RID: 10451 RVA: 0x0009EC54 File Offset: 0x0009CE54
		public static void ClearReplicationTableStatistics()
		{
			MBAPI.IMBNetwork.ClearReplicationTableStatistics();
		}

		// Token: 0x060028D4 RID: 10452 RVA: 0x0009EC60 File Offset: 0x0009CE60
		public static void ResetDebugUploads()
		{
			MBAPI.IMBNetwork.ResetDebugUploads();
		}

		// Token: 0x060028D5 RID: 10453 RVA: 0x0009EC6C File Offset: 0x0009CE6C
		public static void ResetMissionData()
		{
			MBAPI.IMBNetwork.ResetMissionData();
		}

		// Token: 0x060028D6 RID: 10454 RVA: 0x0009EC78 File Offset: 0x0009CE78
		private static void AddPeerToDisconnect(NetworkCommunicator networkPeer)
		{
			MBAPI.IMBNetwork.AddPeerToDisconnect(networkPeer.Index);
		}

		// Token: 0x060028D7 RID: 10455 RVA: 0x0009EC8C File Offset: 0x0009CE8C
		public static void InitializeCompressionInfos()
		{
			CompressionBasic.ActionCodeCompressionInfo = new CompressionInfo.Integer(ActionIndexCache.act_none.Index, MBAnimation.GetNumActionCodes() - 1, true);
			CompressionBasic.AnimationIndexCompressionInfo = new CompressionInfo.Integer(0, MBAnimation.GetNumAnimations() - 1, true);
			CompressionBasic.CultureIndexCompressionInfo = new CompressionInfo.Integer(-1, MBObjectManager.Instance.GetObjectTypeList<BasicCultureObject>().Count - 1, true);
			CompressionBasic.SoundEventsCompressionInfo = new CompressionInfo.Integer(0, SoundEvent.GetTotalEventCount() - 1, true);
			CompressionMission.ActionSetCompressionInfo = new CompressionInfo.Integer(0, MBActionSet.GetNumberOfActionSets(), true);
			CompressionMission.MonsterUsageSetCompressionInfo = new CompressionInfo.Integer(0, MBActionSet.GetNumberOfMonsterUsageSets(), true);
		}

		// Token: 0x060028D8 RID: 10456 RVA: 0x0009ED1A File Offset: 0x0009CF1A
		[MBCallback]
		internal static void SyncRelevantGameOptionsToServer()
		{
			SyncRelevantGameOptionsToServer syncRelevantGameOptionsToServer = new SyncRelevantGameOptionsToServer();
			syncRelevantGameOptionsToServer.InitializeOptions();
			GameNetwork.BeginModuleEventAsClient();
			GameNetwork.WriteMessage(syncRelevantGameOptionsToServer);
			GameNetwork.EndModuleEventAsClient();
		}

		// Token: 0x060028D9 RID: 10457 RVA: 0x0009ED38 File Offset: 0x0009CF38
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

		// Token: 0x1700075C RID: 1884
		// (get) Token: 0x060028DA RID: 10458 RVA: 0x0009EE5F File Offset: 0x0009D05F
		// (set) Token: 0x060028DB RID: 10459 RVA: 0x0009EE66 File Offset: 0x0009D066
		public static NetworkCommunicator MyPeer { get; private set; }

		// Token: 0x1700075D RID: 1885
		// (get) Token: 0x060028DC RID: 10460 RVA: 0x0009EE6E File Offset: 0x0009D06E
		public static bool IsMyPeerReady
		{
			get
			{
				return GameNetwork.MyPeer != null && GameNetwork.MyPeer.IsSynchronized;
			}
		}

		// Token: 0x04000F48 RID: 3912
		public const int MaxAutomatedBattleIndex = 10;

		// Token: 0x04000F49 RID: 3913
		private static IGameNetworkHandler _handler;

		// Token: 0x04000F4A RID: 3914
		public static int ClientPeerIndex;

		// Token: 0x04000F4B RID: 3915
		private const MultiplayerMessageFilter MultiplayerLogging = MultiplayerMessageFilter.All;

		// Token: 0x04000F4E RID: 3918
		private static Dictionary<Type, int> _gameNetworkMessageTypesAll;

		// Token: 0x04000F4F RID: 3919
		private static Dictionary<Type, int> _gameNetworkMessageTypesFromClient;

		// Token: 0x04000F50 RID: 3920
		private static List<Type> _gameNetworkMessageIdsFromClient;

		// Token: 0x04000F51 RID: 3921
		private static Dictionary<Type, int> _gameNetworkMessageTypesFromServer;

		// Token: 0x04000F52 RID: 3922
		private static List<Type> _gameNetworkMessageIdsFromServer;

		// Token: 0x04000F53 RID: 3923
		private static Dictionary<int, List<object>> _fromClientMessageHandlers;

		// Token: 0x04000F54 RID: 3924
		private static Dictionary<int, List<object>> _fromServerMessageHandlers;

		// Token: 0x020005FD RID: 1533
		public class NetworkMessageHandlerRegisterer
		{
			// Token: 0x06003D0C RID: 15628 RVA: 0x000F1EFD File Offset: 0x000F00FD
			public NetworkMessageHandlerRegisterer(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode definitionMode)
			{
				this._registerMode = definitionMode;
			}

			// Token: 0x06003D0D RID: 15629 RVA: 0x000F1F0C File Offset: 0x000F010C
			public void Register<T>(GameNetworkMessage.ServerMessageHandlerDelegate<T> handler) where T : GameNetworkMessage
			{
				if (this._registerMode == GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add)
				{
					GameNetwork.AddServerMessageHandler<T>(handler);
					return;
				}
				GameNetwork.RemoveServerMessageHandler<T>(handler);
			}

			// Token: 0x06003D0E RID: 15630 RVA: 0x000F1F23 File Offset: 0x000F0123
			public void Register<T>(GameNetworkMessage.ClientMessageHandlerDelegate<T> handler) where T : GameNetworkMessage
			{
				if (this._registerMode == GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add)
				{
					GameNetwork.AddClientMessageHandler<T>(handler);
					return;
				}
				GameNetwork.RemoveClientMessageHandler<T>(handler);
			}

			// Token: 0x04001F33 RID: 7987
			private readonly GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode _registerMode;

			// Token: 0x02000702 RID: 1794
			public enum RegisterMode
			{
				// Token: 0x04002352 RID: 9042
				Add,
				// Token: 0x04002353 RID: 9043
				Remove
			}
		}

		// Token: 0x020005FE RID: 1534
		public class NetworkMessageHandlerRegistererContainer
		{
			// Token: 0x06003D0F RID: 15631 RVA: 0x000F1F3A File Offset: 0x000F013A
			public NetworkMessageHandlerRegistererContainer()
			{
				this._fromClientHandlers = new List<Delegate>();
				this._fromServerHandlers = new List<Delegate>();
			}

			// Token: 0x06003D10 RID: 15632 RVA: 0x000F1F58 File Offset: 0x000F0158
			public void Register<T>(GameNetworkMessage.ServerMessageHandlerDelegate<T> handler) where T : GameNetworkMessage
			{
				this._fromServerHandlers.Add(handler);
			}

			// Token: 0x06003D11 RID: 15633 RVA: 0x000F1F66 File Offset: 0x000F0166
			public void Register<T>(GameNetworkMessage.ClientMessageHandlerDelegate<T> handler) where T : GameNetworkMessage
			{
				this._fromClientHandlers.Add(handler);
			}

			// Token: 0x06003D12 RID: 15634 RVA: 0x000F1F74 File Offset: 0x000F0174
			public void RegisterMessages()
			{
				if (this._fromServerHandlers.Count > 0)
				{
					using (List<Delegate>.Enumerator enumerator = this._fromServerHandlers.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Delegate @delegate = enumerator.Current;
							Type type = @delegate.GetType().GenericTypeArguments[0];
							int num = GameNetwork._gameNetworkMessageTypesFromServer[type];
							GameNetwork._fromServerMessageHandlers[num].Add(@delegate);
						}
						return;
					}
				}
				foreach (Delegate delegate2 in this._fromClientHandlers)
				{
					Type type2 = delegate2.GetType().GenericTypeArguments[0];
					int num2 = GameNetwork._gameNetworkMessageTypesFromClient[type2];
					GameNetwork._fromClientMessageHandlers[num2].Add(delegate2);
				}
			}

			// Token: 0x06003D13 RID: 15635 RVA: 0x000F2068 File Offset: 0x000F0268
			public void UnregisterMessages()
			{
				if (this._fromServerHandlers.Count > 0)
				{
					using (List<Delegate>.Enumerator enumerator = this._fromServerHandlers.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Delegate @delegate = enumerator.Current;
							Type type = @delegate.GetType().GenericTypeArguments[0];
							int num = GameNetwork._gameNetworkMessageTypesFromServer[type];
							GameNetwork._fromServerMessageHandlers[num].Remove(@delegate);
						}
						return;
					}
				}
				foreach (Delegate delegate2 in this._fromClientHandlers)
				{
					Type type2 = delegate2.GetType().GenericTypeArguments[0];
					int num2 = GameNetwork._gameNetworkMessageTypesFromClient[type2];
					GameNetwork._fromClientMessageHandlers[num2].Remove(delegate2);
				}
			}

			// Token: 0x04001F34 RID: 7988
			private List<Delegate> _fromClientHandlers;

			// Token: 0x04001F35 RID: 7989
			private List<Delegate> _fromServerHandlers;
		}

		// Token: 0x020005FF RID: 1535
		[Flags]
		public enum EventBroadcastFlags
		{
			// Token: 0x04001F37 RID: 7991
			None = 0,
			// Token: 0x04001F38 RID: 7992
			ExcludeTargetPlayer = 1,
			// Token: 0x04001F39 RID: 7993
			ExcludeNoBloodStainsOption = 2,
			// Token: 0x04001F3A RID: 7994
			ExcludeNoParticlesOption = 4,
			// Token: 0x04001F3B RID: 7995
			ExcludeNoSoundOption = 8,
			// Token: 0x04001F3C RID: 7996
			AddToMissionRecord = 16,
			// Token: 0x04001F3D RID: 7997
			IncludeUnsynchronizedClients = 32,
			// Token: 0x04001F3E RID: 7998
			ExcludeOtherTeamPlayers = 64,
			// Token: 0x04001F3F RID: 7999
			ExcludePeerTeamPlayers = 128,
			// Token: 0x04001F40 RID: 8000
			DontSendToPeers = 256
		}

		// Token: 0x02000600 RID: 1536
		[EngineStruct("Debug_network_position_compression_statistics_struct")]
		public struct DebugNetworkPositionCompressionStatisticsStruct
		{
			// Token: 0x04001F41 RID: 8001
			public int totalPositionUpload;

			// Token: 0x04001F42 RID: 8002
			public int totalPositionPrecisionBitCount;

			// Token: 0x04001F43 RID: 8003
			public int totalPositionCoarseBitCountX;

			// Token: 0x04001F44 RID: 8004
			public int totalPositionCoarseBitCountY;

			// Token: 0x04001F45 RID: 8005
			public int totalPositionCoarseBitCountZ;
		}

		// Token: 0x02000601 RID: 1537
		[EngineStruct("Debug_network_packet_statistics_struct")]
		public struct DebugNetworkPacketStatisticsStruct
		{
			// Token: 0x04001F46 RID: 8006
			public int TotalPackets;

			// Token: 0x04001F47 RID: 8007
			public int TotalUpload;

			// Token: 0x04001F48 RID: 8008
			public int TotalConstantsUpload;

			// Token: 0x04001F49 RID: 8009
			public int TotalReliableEventUpload;

			// Token: 0x04001F4A RID: 8010
			public int TotalReplicationUpload;

			// Token: 0x04001F4B RID: 8011
			public int TotalUnreliableEventUpload;

			// Token: 0x04001F4C RID: 8012
			public int TotalReplicationTableAdderCount;

			// Token: 0x04001F4D RID: 8013
			public int debug_total_replication_table_adder_bit_count;

			// Token: 0x04001F4E RID: 8014
			public int debug_total_replication_table_adder;

			// Token: 0x04001F4F RID: 8015
			public double debug_total_cell_priority;

			// Token: 0x04001F50 RID: 8016
			public double debug_total_cell_agent_priority;

			// Token: 0x04001F51 RID: 8017
			public double debug_total_cell_cell_priority;

			// Token: 0x04001F52 RID: 8018
			public int debug_total_cell_priority_checks;

			// Token: 0x04001F53 RID: 8019
			public int debug_total_sent_cell_count;

			// Token: 0x04001F54 RID: 8020
			public int debug_total_not_sent_cell_count;

			// Token: 0x04001F55 RID: 8021
			public int debug_total_replication_write_count;

			// Token: 0x04001F56 RID: 8022
			public int debug_cur_max_packet_size_in_bytes;

			// Token: 0x04001F57 RID: 8023
			public double average_ping_time;

			// Token: 0x04001F58 RID: 8024
			public double debug_average_dt_to_send_packet;

			// Token: 0x04001F59 RID: 8025
			public double time_out_period;

			// Token: 0x04001F5A RID: 8026
			public double pacing_rate;

			// Token: 0x04001F5B RID: 8027
			public double delivery_rate;

			// Token: 0x04001F5C RID: 8028
			public double round_trip_time;

			// Token: 0x04001F5D RID: 8029
			public int inflight_bit_count;

			// Token: 0x04001F5E RID: 8030
			public int is_congested;

			// Token: 0x04001F5F RID: 8031
			public int probe_bw_phase_index;

			// Token: 0x04001F60 RID: 8032
			public double lost_percent;

			// Token: 0x04001F61 RID: 8033
			public int lost_count;

			// Token: 0x04001F62 RID: 8034
			public int total_count_on_lost_check;
		}

		// Token: 0x02000602 RID: 1538
		public struct AddPlayersResult
		{
			// Token: 0x04001F63 RID: 8035
			public bool Success;

			// Token: 0x04001F64 RID: 8036
			public NetworkCommunicator[] NetworkPeers;
		}
	}
}
