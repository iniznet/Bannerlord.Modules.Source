using System;
using System.Threading.Tasks;
using NetworkMessages.FromClient;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002EF RID: 751
	public class BaseNetworkComponent : UdpNetworkComponent
	{
		// Token: 0x1700075E RID: 1886
		// (get) Token: 0x060028F2 RID: 10482 RVA: 0x0009F01C File Offset: 0x0009D21C
		// (set) Token: 0x060028F3 RID: 10483 RVA: 0x0009F024 File Offset: 0x0009D224
		public MultiplayerIntermissionState ClientIntermissionState { get; private set; }

		// Token: 0x1700075F RID: 1887
		// (get) Token: 0x060028F4 RID: 10484 RVA: 0x0009F02D File Offset: 0x0009D22D
		// (set) Token: 0x060028F5 RID: 10485 RVA: 0x0009F035 File Offset: 0x0009D235
		public float CurrentIntermissionTimer { get; private set; }

		// Token: 0x17000760 RID: 1888
		// (get) Token: 0x060028F6 RID: 10486 RVA: 0x0009F03E File Offset: 0x0009D23E
		// (set) Token: 0x060028F7 RID: 10487 RVA: 0x0009F046 File Offset: 0x0009D246
		public int CurrentBattleIndex { get; private set; }

		// Token: 0x060028F8 RID: 10488 RVA: 0x0009F050 File Offset: 0x0009D250
		protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
		{
			base.AddRemoveMessageHandlers(registerer);
			if (GameNetwork.IsClientOrReplay)
			{
				registerer.Register<AddPeerComponent>(new GameNetworkMessage.ServerMessageHandlerDelegate<AddPeerComponent>(this.HandleServerEventAddPeerComponent));
				registerer.Register<RemovePeerComponent>(new GameNetworkMessage.ServerMessageHandlerDelegate<RemovePeerComponent>(this.HandleServerEventRemovePeerComponent));
				registerer.Register<SynchronizingDone>(new GameNetworkMessage.ServerMessageHandlerDelegate<SynchronizingDone>(this.HandleServerEventSynchronizingDone));
				registerer.Register<LoadMission>(new GameNetworkMessage.ServerMessageHandlerDelegate<LoadMission>(this.HandleServerEventLoadMission));
				registerer.Register<UnloadMission>(new GameNetworkMessage.ServerMessageHandlerDelegate<UnloadMission>(this.HandleServerEventUnloadMission));
				registerer.Register<InitializeCustomGameMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<InitializeCustomGameMessage>(this.HandleServerEventInitializeCustomGame));
				registerer.Register<MultiplayerOptionsInitial>(new GameNetworkMessage.ServerMessageHandlerDelegate<MultiplayerOptionsInitial>(this.HandleServerEventMultiplayerOptionsInitial));
				registerer.Register<MultiplayerOptionsImmediate>(new GameNetworkMessage.ServerMessageHandlerDelegate<MultiplayerOptionsImmediate>(this.HandleServerEventMultiplayerOptionsImmediate));
				registerer.Register<MultiplayerIntermissionUpdate>(new GameNetworkMessage.ServerMessageHandlerDelegate<MultiplayerIntermissionUpdate>(this.HandleServerEventMultiplayerIntermissionUpdate));
				registerer.Register<MultiplayerIntermissionMapItemAdded>(new GameNetworkMessage.ServerMessageHandlerDelegate<MultiplayerIntermissionMapItemAdded>(this.HandleServerEventIntermissionMapItemAdded));
				registerer.Register<MultiplayerIntermissionCultureItemAdded>(new GameNetworkMessage.ServerMessageHandlerDelegate<MultiplayerIntermissionCultureItemAdded>(this.HandleServerEventIntermissionCultureItemAdded));
				registerer.Register<MultiplayerIntermissionMapItemVoteCountChanged>(new GameNetworkMessage.ServerMessageHandlerDelegate<MultiplayerIntermissionMapItemVoteCountChanged>(this.HandleServerEventIntermissionMapItemVoteCountChanged));
				registerer.Register<MultiplayerIntermissionCultureItemVoteCountChanged>(new GameNetworkMessage.ServerMessageHandlerDelegate<MultiplayerIntermissionCultureItemVoteCountChanged>(this.HandleServerEventIntermissionCultureItemVoteCountChanged));
				return;
			}
			if (GameNetwork.IsServer)
			{
				registerer.Register<FinishedLoading>(new GameNetworkMessage.ClientMessageHandlerDelegate<FinishedLoading>(this.HandleClientEventFinishedLoading));
				registerer.Register<SyncRelevantGameOptionsToServer>(new GameNetworkMessage.ClientMessageHandlerDelegate<SyncRelevantGameOptionsToServer>(this.HandleSyncRelevantGameOptionsToServer));
				registerer.Register<IntermissionVote>(new GameNetworkMessage.ClientMessageHandlerDelegate<IntermissionVote>(this.HandleIntermissionClientVote));
			}
		}

		// Token: 0x060028F9 RID: 10489 RVA: 0x0009F198 File Offset: 0x0009D398
		public override void OnUdpNetworkHandlerTick(float dt)
		{
			base.OnUdpNetworkHandlerTick(dt);
			if (GameNetwork.IsClientOrReplay && (this.ClientIntermissionState == MultiplayerIntermissionState.CountingForMission || this.ClientIntermissionState == MultiplayerIntermissionState.CountingForEnd || this.ClientIntermissionState == MultiplayerIntermissionState.CountingForMapVote || this.ClientIntermissionState == MultiplayerIntermissionState.CountingForCultureVote))
			{
				this.CurrentIntermissionTimer -= dt;
				if (this.CurrentIntermissionTimer <= 0f)
				{
					this.CurrentIntermissionTimer = 0f;
				}
			}
		}

		// Token: 0x060028FA RID: 10490 RVA: 0x0009F200 File Offset: 0x0009D400
		public override void HandleNewClientConnect(PlayerConnectionInfo playerConnectionInfo)
		{
			NetworkCommunicator networkPeer = playerConnectionInfo.NetworkPeer;
			if (!networkPeer.IsServerPeer)
			{
				GameNetwork.BeginModuleEventAsServer(networkPeer);
				GameNetwork.WriteMessage(new MultiplayerOptionsInitial());
				GameNetwork.EndModuleEventAsServer();
				GameNetwork.BeginModuleEventAsServer(networkPeer);
				GameNetwork.WriteMessage(new MultiplayerOptionsImmediate());
				GameNetwork.EndModuleEventAsServer();
				foreach (IntermissionVoteItem intermissionVoteItem in MultiplayerIntermissionVotingManager.Instance.MapVoteItems)
				{
					GameNetwork.BeginModuleEventAsServer(networkPeer);
					GameNetwork.WriteMessage(new MultiplayerIntermissionMapItemAdded(intermissionVoteItem.Id));
					GameNetwork.EndModuleEventAsServer();
					GameNetwork.BeginModuleEventAsServer(networkPeer);
					GameNetwork.WriteMessage(new MultiplayerIntermissionMapItemVoteCountChanged(intermissionVoteItem.Index, intermissionVoteItem.VoteCount));
					GameNetwork.EndModuleEventAsServer();
				}
				foreach (IntermissionVoteItem intermissionVoteItem2 in MultiplayerIntermissionVotingManager.Instance.CultureVoteItems)
				{
					GameNetwork.BeginModuleEventAsServer(networkPeer);
					GameNetwork.WriteMessage(new MultiplayerIntermissionCultureItemAdded(intermissionVoteItem2.Id));
					GameNetwork.EndModuleEventAsServer();
					GameNetwork.BeginModuleEventAsServer(networkPeer);
					GameNetwork.WriteMessage(new MultiplayerIntermissionCultureItemVoteCountChanged(intermissionVoteItem2.Index, intermissionVoteItem2.VoteCount));
					GameNetwork.EndModuleEventAsServer();
				}
				if (BannerlordNetwork.LobbyMissionType == LobbyMissionType.Custom)
				{
					bool flag = false;
					string text = "";
					string text2 = "";
					if ((GameNetwork.IsDedicatedServer && Mission.Current != null) || !GameNetwork.IsDedicatedServer)
					{
						flag = true;
						MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.Map, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions).GetValue(out text);
						MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.GameType, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions).GetValue(out text2);
					}
					GameNetwork.BeginModuleEventAsServer(networkPeer);
					GameNetwork.WriteMessage(new InitializeCustomGameMessage(flag, text2, text, this.CurrentBattleIndex));
					GameNetwork.EndModuleEventAsServer();
				}
			}
		}

		// Token: 0x060028FB RID: 10491 RVA: 0x0009F3C0 File Offset: 0x0009D5C0
		public override void HandlePlayerDisconnect(NetworkCommunicator networkPeer)
		{
			MultiplayerIntermissionVotingManager.Instance.HandlePlayerDisconnect(networkPeer.VirtualPlayer.Id);
		}

		// Token: 0x060028FC RID: 10492 RVA: 0x0009F3D7 File Offset: 0x0009D5D7
		public void IntermissionCastVote(string itemID, int voteCount)
		{
			GameNetwork.BeginModuleEventAsClient();
			GameNetwork.WriteMessage(new IntermissionVote(itemID, voteCount));
			GameNetwork.EndModuleEventAsClient();
		}

		// Token: 0x060028FD RID: 10493 RVA: 0x0009F3F0 File Offset: 0x0009D5F0
		public override void HandleNewClientAfterSynchronized(NetworkCommunicator networkPeer)
		{
			Mission mission = Mission.Current;
			MissionNetworkComponent missionNetworkComponent = ((mission != null) ? mission.GetMissionBehavior<MissionNetworkComponent>() : null);
			if (missionNetworkComponent != null)
			{
				missionNetworkComponent.OnClientSynchronized(networkPeer);
			}
		}

		// Token: 0x060028FE RID: 10494 RVA: 0x0009F419 File Offset: 0x0009D619
		public void UpdateCurrentBattleIndex(int currentBattleIndex)
		{
			this.CurrentBattleIndex = currentBattleIndex;
		}

		// Token: 0x17000761 RID: 1889
		// (get) Token: 0x060028FF RID: 10495 RVA: 0x0009F422 File Offset: 0x0009D622
		// (set) Token: 0x06002900 RID: 10496 RVA: 0x0009F42A File Offset: 0x0009D62A
		public bool DisplayingWelcomeMessage { get; private set; }

		// Token: 0x14000079 RID: 121
		// (add) Token: 0x06002901 RID: 10497 RVA: 0x0009F434 File Offset: 0x0009D634
		// (remove) Token: 0x06002902 RID: 10498 RVA: 0x0009F46C File Offset: 0x0009D66C
		public event BaseNetworkComponent.WelcomeMessageReceivedDelegate WelcomeMessageReceived;

		// Token: 0x06002903 RID: 10499 RVA: 0x0009F4A1 File Offset: 0x0009D6A1
		public void SetDisplayingWelcomeMessage(bool displaying)
		{
			this.DisplayingWelcomeMessage = displaying;
		}

		// Token: 0x06002904 RID: 10500 RVA: 0x0009F4AC File Offset: 0x0009D6AC
		private void HandleServerEventMultiplayerOptionsInitial(MultiplayerOptionsInitial message)
		{
			for (MultiplayerOptions.OptionType optionType = MultiplayerOptions.OptionType.ServerName; optionType < MultiplayerOptions.OptionType.NumOfSlots; optionType++)
			{
				MultiplayerOptionsProperty optionProperty = optionType.GetOptionProperty();
				if (optionProperty.Replication == MultiplayerOptionsProperty.ReplicationOccurrence.AtMapLoad)
				{
					switch (optionProperty.OptionValueType)
					{
					case MultiplayerOptions.OptionValueType.Bool:
					{
						bool flag;
						message.GetOption(optionType).GetValue(out flag);
						optionType.SetValue(flag, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
						break;
					}
					case MultiplayerOptions.OptionValueType.Integer:
					case MultiplayerOptions.OptionValueType.Enum:
					{
						int num;
						message.GetOption(optionType).GetValue(out num);
						optionType.SetValue(num, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
						break;
					}
					case MultiplayerOptions.OptionValueType.String:
					{
						string text;
						message.GetOption(optionType).GetValue(out text);
						optionType.SetValue(text, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
						break;
					}
					default:
						throw new ArgumentOutOfRangeException();
					}
				}
			}
			string strValue = MultiplayerOptions.OptionType.WelcomeMessage.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
			if (!string.IsNullOrEmpty(strValue))
			{
				BaseNetworkComponent.WelcomeMessageReceivedDelegate welcomeMessageReceived = this.WelcomeMessageReceived;
				if (welcomeMessageReceived == null)
				{
					return;
				}
				welcomeMessageReceived(strValue);
			}
		}

		// Token: 0x06002905 RID: 10501 RVA: 0x0009F570 File Offset: 0x0009D770
		private void HandleServerEventMultiplayerOptionsImmediate(MultiplayerOptionsImmediate message)
		{
			for (MultiplayerOptions.OptionType optionType = MultiplayerOptions.OptionType.ServerName; optionType < MultiplayerOptions.OptionType.NumOfSlots; optionType++)
			{
				MultiplayerOptionsProperty optionProperty = optionType.GetOptionProperty();
				if (optionProperty.Replication == MultiplayerOptionsProperty.ReplicationOccurrence.Immediately)
				{
					switch (optionProperty.OptionValueType)
					{
					case MultiplayerOptions.OptionValueType.Bool:
					{
						bool flag;
						message.GetOption(optionType).GetValue(out flag);
						optionType.SetValue(flag, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
						break;
					}
					case MultiplayerOptions.OptionValueType.Integer:
					case MultiplayerOptions.OptionValueType.Enum:
					{
						int num;
						message.GetOption(optionType).GetValue(out num);
						optionType.SetValue(num, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
						break;
					}
					case MultiplayerOptions.OptionValueType.String:
					{
						string text;
						message.GetOption(optionType).GetValue(out text);
						optionType.SetValue(text, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
						break;
					}
					default:
						throw new ArgumentOutOfRangeException();
					}
				}
			}
		}

		// Token: 0x06002906 RID: 10502 RVA: 0x0009F60F File Offset: 0x0009D80F
		private void HandleServerEventMultiplayerIntermissionUpdate(MultiplayerIntermissionUpdate message)
		{
			this.CurrentIntermissionTimer = message.IntermissionTimer;
			this.ClientIntermissionState = message.IntermissionState;
			Action onIntermissionStateUpdated = this.OnIntermissionStateUpdated;
			if (onIntermissionStateUpdated == null)
			{
				return;
			}
			onIntermissionStateUpdated();
		}

		// Token: 0x06002907 RID: 10503 RVA: 0x0009F639 File Offset: 0x0009D839
		private void HandleServerEventIntermissionMapItemAdded(MultiplayerIntermissionMapItemAdded message)
		{
			MultiplayerIntermissionVotingManager.Instance.AddMapItem(message.MapId);
			Action onIntermissionStateUpdated = this.OnIntermissionStateUpdated;
			if (onIntermissionStateUpdated == null)
			{
				return;
			}
			onIntermissionStateUpdated();
		}

		// Token: 0x06002908 RID: 10504 RVA: 0x0009F65B File Offset: 0x0009D85B
		private void HandleServerEventIntermissionCultureItemAdded(MultiplayerIntermissionCultureItemAdded message)
		{
			MultiplayerIntermissionVotingManager.Instance.AddCultureItem(message.CultureId);
			Action onIntermissionStateUpdated = this.OnIntermissionStateUpdated;
			if (onIntermissionStateUpdated == null)
			{
				return;
			}
			onIntermissionStateUpdated();
		}

		// Token: 0x06002909 RID: 10505 RVA: 0x0009F67D File Offset: 0x0009D87D
		private void HandleServerEventIntermissionMapItemVoteCountChanged(MultiplayerIntermissionMapItemVoteCountChanged message)
		{
			MultiplayerIntermissionVotingManager.Instance.SetVotesOfMap(message.MapItemIndex, message.VoteCount);
			Action onIntermissionStateUpdated = this.OnIntermissionStateUpdated;
			if (onIntermissionStateUpdated == null)
			{
				return;
			}
			onIntermissionStateUpdated();
		}

		// Token: 0x0600290A RID: 10506 RVA: 0x0009F6A5 File Offset: 0x0009D8A5
		private void HandleServerEventIntermissionCultureItemVoteCountChanged(MultiplayerIntermissionCultureItemVoteCountChanged message)
		{
			MultiplayerIntermissionVotingManager.Instance.SetVotesOfCulture(message.CultureItemIndex, message.VoteCount);
			Action onIntermissionStateUpdated = this.OnIntermissionStateUpdated;
			if (onIntermissionStateUpdated == null)
			{
				return;
			}
			onIntermissionStateUpdated();
		}

		// Token: 0x0600290B RID: 10507 RVA: 0x0009F6D0 File Offset: 0x0009D8D0
		private void HandleServerEventAddPeerComponent(AddPeerComponent message)
		{
			NetworkCommunicator peer = message.Peer;
			uint componentId = message.ComponentId;
			if (peer.GetComponent(componentId) == null)
			{
				peer.AddComponent(componentId);
			}
		}

		// Token: 0x0600290C RID: 10508 RVA: 0x0009F6FC File Offset: 0x0009D8FC
		private void HandleServerEventRemovePeerComponent(RemovePeerComponent message)
		{
			NetworkCommunicator peer = message.Peer;
			uint componentId = message.ComponentId;
			PeerComponent component = peer.GetComponent(componentId);
			peer.RemoveComponent(component);
		}

		// Token: 0x0600290D RID: 10509 RVA: 0x0009F724 File Offset: 0x0009D924
		private void HandleServerEventSynchronizingDone(SynchronizingDone message)
		{
			NetworkCommunicator peer = message.Peer;
			Mission mission = Mission.Current;
			MissionNetworkComponent missionNetworkComponent = ((mission != null) ? mission.GetMissionBehavior<MissionNetworkComponent>() : null);
			if (missionNetworkComponent != null && !peer.IsMine)
			{
				missionNetworkComponent.OnClientSynchronized(peer);
				return;
			}
			peer.IsSynchronized = message.Synchronized;
			if (missionNetworkComponent != null && message.Synchronized)
			{
				if (peer.GetComponent<MissionPeer>() == null)
				{
					LobbyClient gameClient = NetworkMain.GameClient;
					if (gameClient.CurrentState == LobbyClient.State.InCustomGame)
					{
						gameClient.QuitFromCustomGame();
						return;
					}
					if (gameClient.CurrentState == LobbyClient.State.HostingCustomGame)
					{
						gameClient.EndCustomGame();
						return;
					}
					gameClient.QuitFromMatchmakerGame();
					return;
				}
				else
				{
					missionNetworkComponent.OnClientSynchronized(peer);
				}
			}
		}

		// Token: 0x0600290E RID: 10510 RVA: 0x0009F7B4 File Offset: 0x0009D9B4
		private async void HandleServerEventLoadMission(LoadMission message)
		{
			while (GameStateManager.Current.ActiveState is MissionState)
			{
				await Task.Delay(1);
			}
			if (GameNetwork.MyPeer != null)
			{
				GameNetwork.MyPeer.IsSynchronized = false;
			}
			this.CurrentIntermissionTimer = 0f;
			this.ClientIntermissionState = MultiplayerIntermissionState.Idle;
			this.UpdateCurrentBattleIndex(message.BattleIndex);
			if (!Module.CurrentModule.StartMultiplayerGame(message.GameType, message.Map))
			{
				Debug.FailedAssert("[DEBUG]Invalid multiplayer game type.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Components\\BaseNetworkComponent.cs", "HandleServerEventLoadMission", 359);
			}
		}

		// Token: 0x0600290F RID: 10511 RVA: 0x0009F7F5 File Offset: 0x0009D9F5
		private void HandleServerEventUnloadMission(UnloadMission message)
		{
			this.HandleServerEventUnloadMissionAux(message);
		}

		// Token: 0x06002910 RID: 10512 RVA: 0x0009F7FE File Offset: 0x0009D9FE
		private void HandleServerEventInitializeCustomGame(InitializeCustomGameMessage message)
		{
			this.InitializeCustomGameAux(message);
		}

		// Token: 0x06002911 RID: 10513 RVA: 0x0009F808 File Offset: 0x0009DA08
		private async void InitializeCustomGameAux(InitializeCustomGameMessage message)
		{
			await Task.Delay(200);
			while (!(GameStateManager.Current.ActiveState is LobbyGameStateCustomGameClient))
			{
				await Task.Delay(1);
			}
			if (message.InMission)
			{
				MBDebug.Print(string.Concat(new string[] { "Client: I have received InitializeCustomGameMessage with mission ", message.GameType, " ", message.Map, ". Loading it..." }), 0, Debug.DebugColor.White, 17179869184UL);
				this.UpdateCurrentBattleIndex(message.BattleIndex);
				if (!Module.CurrentModule.StartMultiplayerGame(message.GameType, message.Map))
				{
					Debug.FailedAssert("[DEBUG]Invalid multiplayer game type.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Components\\BaseNetworkComponent.cs", "InitializeCustomGameAux", 390);
				}
			}
			else
			{
				LoadingWindow.DisableGlobalLoadingWindow();
				GameNetwork.SyncRelevantGameOptionsToServer();
			}
		}

		// Token: 0x06002912 RID: 10514 RVA: 0x0009F84C File Offset: 0x0009DA4C
		private async void HandleServerEventUnloadMissionAux(UnloadMission message)
		{
			GameNetwork.MyPeer.IsSynchronized = false;
			this.CurrentIntermissionTimer = 0f;
			this.ClientIntermissionState = MultiplayerIntermissionState.Idle;
			if (Mission.Current != null)
			{
				MissionCustomGameClientComponent missionBehavior = Mission.Current.GetMissionBehavior<MissionCustomGameClientComponent>();
				if (missionBehavior != null)
				{
					missionBehavior.SetServerEndingBeforeClientLoaded(message.UnloadingForBattleIndexMismatch);
				}
			}
			BannerlordNetwork.EndMultiplayerLobbyMission();
			Game.Current.GetGameHandler<ChatBox>().ResetMuteList();
			while (Mission.Current != null)
			{
				await Task.Delay(1);
			}
			LoadingWindow.DisableGlobalLoadingWindow();
		}

		// Token: 0x06002913 RID: 10515 RVA: 0x0009F88D File Offset: 0x0009DA8D
		private bool HandleClientEventFinishedLoading(NetworkCommunicator networkPeer, FinishedLoading message)
		{
			this.HandleClientEventFinishedLoadingAux(networkPeer, message);
			return true;
		}

		// Token: 0x06002914 RID: 10516 RVA: 0x0009F898 File Offset: 0x0009DA98
		private async void HandleClientEventFinishedLoadingAux(NetworkCommunicator networkPeer, FinishedLoading message)
		{
			while (Mission.Current != null && Mission.Current.CurrentState != Mission.State.Continuing)
			{
				await Task.Delay(1);
			}
			if (!networkPeer.IsServerPeer)
			{
				MBDebug.Print("Server: " + networkPeer.UserName + " has finished loading. From now on, I will include him in the broadcasted messages", 0, Debug.DebugColor.White, 17179869184UL);
				if (Mission.Current == null || this.CurrentBattleIndex != message.BattleIndex)
				{
					GameNetwork.BeginModuleEventAsServer(networkPeer);
					GameNetwork.WriteMessage(new UnloadMission(true));
					GameNetwork.EndModuleEventAsServer();
				}
				else
				{
					GameNetwork.ClientFinishedLoading(networkPeer);
				}
			}
		}

		// Token: 0x06002915 RID: 10517 RVA: 0x0009F8E1 File Offset: 0x0009DAE1
		private bool HandleSyncRelevantGameOptionsToServer(NetworkCommunicator networkPeer, SyncRelevantGameOptionsToServer message)
		{
			networkPeer.SetRelevantGameOptions(message.SendMeBloodEvents, message.SendMeSoundEvents);
			return true;
		}

		// Token: 0x06002916 RID: 10518 RVA: 0x0009F8F8 File Offset: 0x0009DAF8
		private bool HandleIntermissionClientVote(NetworkCommunicator networkPeer, IntermissionVote message)
		{
			int voteCount = message.VoteCount;
			if (voteCount == -1 || voteCount == 1)
			{
				if ((MultiplayerIntermissionVotingManager.Instance.CurrentVoteState == MultiplayerIntermissionState.CountingForMapVote && MultiplayerIntermissionVotingManager.Instance.IsMapItem(message.ItemID)) || (MultiplayerIntermissionVotingManager.Instance.CurrentVoteState == MultiplayerIntermissionState.CountingForCultureVote && MultiplayerIntermissionVotingManager.Instance.IsCultureItem(message.ItemID)))
				{
					MultiplayerIntermissionVotingManager.Instance.AddVote(networkPeer.VirtualPlayer.Id, message.ItemID, message.VoteCount);
				}
				return true;
			}
			return false;
		}

		// Token: 0x04000F56 RID: 3926
		public const float MaxIntermissionStateTime = 240f;

		// Token: 0x04000F59 RID: 3929
		public Action OnIntermissionStateUpdated;

		// Token: 0x02000606 RID: 1542
		// (Invoke) Token: 0x06003D24 RID: 15652
		public delegate void WelcomeMessageReceivedDelegate(string messageText);
	}
}
