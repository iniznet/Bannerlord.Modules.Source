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
	public class BaseNetworkComponent : UdpNetworkComponent
	{
		public MultiplayerIntermissionState ClientIntermissionState { get; private set; }

		public float CurrentIntermissionTimer { get; private set; }

		public int CurrentBattleIndex { get; private set; }

		protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
		{
			base.AddRemoveMessageHandlers(registerer);
			if (GameNetwork.IsClientOrReplay)
			{
				registerer.RegisterBaseHandler<AddPeerComponent>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventAddPeerComponent));
				registerer.RegisterBaseHandler<RemovePeerComponent>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventRemovePeerComponent));
				registerer.RegisterBaseHandler<SynchronizingDone>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventSynchronizingDone));
				registerer.RegisterBaseHandler<LoadMission>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventLoadMission));
				registerer.RegisterBaseHandler<UnloadMission>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventUnloadMission));
				registerer.RegisterBaseHandler<InitializeCustomGameMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventInitializeCustomGame));
				registerer.RegisterBaseHandler<MultiplayerOptionsInitial>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventMultiplayerOptionsInitial));
				registerer.RegisterBaseHandler<MultiplayerOptionsImmediate>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventMultiplayerOptionsImmediate));
				registerer.RegisterBaseHandler<MultiplayerIntermissionUpdate>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventMultiplayerIntermissionUpdate));
				registerer.RegisterBaseHandler<MultiplayerIntermissionMapItemAdded>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventIntermissionMapItemAdded));
				registerer.RegisterBaseHandler<MultiplayerIntermissionCultureItemAdded>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventIntermissionCultureItemAdded));
				registerer.RegisterBaseHandler<MultiplayerIntermissionMapItemVoteCountChanged>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventIntermissionMapItemVoteCountChanged));
				registerer.RegisterBaseHandler<MultiplayerIntermissionCultureItemVoteCountChanged>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventIntermissionCultureItemVoteCountChanged));
				return;
			}
			if (GameNetwork.IsServer)
			{
				registerer.RegisterBaseHandler<FinishedLoading>(new GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>(this.HandleClientEventFinishedLoading));
				registerer.RegisterBaseHandler<SyncRelevantGameOptionsToServer>(new GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>(this.HandleSyncRelevantGameOptionsToServer));
				registerer.RegisterBaseHandler<IntermissionVote>(new GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>(this.HandleIntermissionClientVote));
			}
		}

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

		public override void HandlePlayerDisconnect(NetworkCommunicator networkPeer)
		{
			MultiplayerIntermissionVotingManager.Instance.HandlePlayerDisconnect(networkPeer.VirtualPlayer.Id);
		}

		public void IntermissionCastVote(string itemID, int voteCount)
		{
			GameNetwork.BeginModuleEventAsClient();
			GameNetwork.WriteMessage(new IntermissionVote(itemID, voteCount));
			GameNetwork.EndModuleEventAsClient();
		}

		public override void HandleNewClientAfterSynchronized(NetworkCommunicator networkPeer)
		{
			Mission mission = Mission.Current;
			MissionNetworkComponent missionNetworkComponent = ((mission != null) ? mission.GetMissionBehavior<MissionNetworkComponent>() : null);
			if (missionNetworkComponent != null)
			{
				missionNetworkComponent.OnClientSynchronized(networkPeer);
			}
		}

		public void UpdateCurrentBattleIndex(int currentBattleIndex)
		{
			this.CurrentBattleIndex = currentBattleIndex;
		}

		public bool DisplayingWelcomeMessage { get; private set; }

		public event BaseNetworkComponent.WelcomeMessageReceivedDelegate WelcomeMessageReceived;

		public void SetDisplayingWelcomeMessage(bool displaying)
		{
			this.DisplayingWelcomeMessage = displaying;
		}

		private void HandleServerEventMultiplayerOptionsInitial(GameNetworkMessage baseMessage)
		{
			MultiplayerOptionsInitial multiplayerOptionsInitial = (MultiplayerOptionsInitial)baseMessage;
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
						multiplayerOptionsInitial.GetOption(optionType).GetValue(out flag);
						optionType.SetValue(flag, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
						break;
					}
					case MultiplayerOptions.OptionValueType.Integer:
					case MultiplayerOptions.OptionValueType.Enum:
					{
						int num;
						multiplayerOptionsInitial.GetOption(optionType).GetValue(out num);
						optionType.SetValue(num, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
						break;
					}
					case MultiplayerOptions.OptionValueType.String:
					{
						string text;
						multiplayerOptionsInitial.GetOption(optionType).GetValue(out text);
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

		private void HandleServerEventMultiplayerOptionsImmediate(GameNetworkMessage baseMessage)
		{
			MultiplayerOptionsImmediate multiplayerOptionsImmediate = (MultiplayerOptionsImmediate)baseMessage;
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
						multiplayerOptionsImmediate.GetOption(optionType).GetValue(out flag);
						optionType.SetValue(flag, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
						break;
					}
					case MultiplayerOptions.OptionValueType.Integer:
					case MultiplayerOptions.OptionValueType.Enum:
					{
						int num;
						multiplayerOptionsImmediate.GetOption(optionType).GetValue(out num);
						optionType.SetValue(num, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
						break;
					}
					case MultiplayerOptions.OptionValueType.String:
					{
						string text;
						multiplayerOptionsImmediate.GetOption(optionType).GetValue(out text);
						optionType.SetValue(text, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
						break;
					}
					default:
						throw new ArgumentOutOfRangeException();
					}
				}
			}
		}

		private void HandleServerEventMultiplayerIntermissionUpdate(GameNetworkMessage baseMessage)
		{
			MultiplayerIntermissionUpdate multiplayerIntermissionUpdate = (MultiplayerIntermissionUpdate)baseMessage;
			this.CurrentIntermissionTimer = multiplayerIntermissionUpdate.IntermissionTimer;
			this.ClientIntermissionState = multiplayerIntermissionUpdate.IntermissionState;
			Action onIntermissionStateUpdated = this.OnIntermissionStateUpdated;
			if (onIntermissionStateUpdated == null)
			{
				return;
			}
			onIntermissionStateUpdated();
		}

		private void HandleServerEventIntermissionMapItemAdded(GameNetworkMessage baseMessage)
		{
			MultiplayerIntermissionMapItemAdded multiplayerIntermissionMapItemAdded = (MultiplayerIntermissionMapItemAdded)baseMessage;
			MultiplayerIntermissionVotingManager.Instance.AddMapItem(multiplayerIntermissionMapItemAdded.MapId);
			Action onIntermissionStateUpdated = this.OnIntermissionStateUpdated;
			if (onIntermissionStateUpdated == null)
			{
				return;
			}
			onIntermissionStateUpdated();
		}

		private void HandleServerEventIntermissionCultureItemAdded(GameNetworkMessage baseMessage)
		{
			MultiplayerIntermissionCultureItemAdded multiplayerIntermissionCultureItemAdded = (MultiplayerIntermissionCultureItemAdded)baseMessage;
			MultiplayerIntermissionVotingManager.Instance.AddCultureItem(multiplayerIntermissionCultureItemAdded.CultureId);
			Action onIntermissionStateUpdated = this.OnIntermissionStateUpdated;
			if (onIntermissionStateUpdated == null)
			{
				return;
			}
			onIntermissionStateUpdated();
		}

		private void HandleServerEventIntermissionMapItemVoteCountChanged(GameNetworkMessage baseMessage)
		{
			MultiplayerIntermissionMapItemVoteCountChanged multiplayerIntermissionMapItemVoteCountChanged = (MultiplayerIntermissionMapItemVoteCountChanged)baseMessage;
			MultiplayerIntermissionVotingManager.Instance.SetVotesOfMap(multiplayerIntermissionMapItemVoteCountChanged.MapItemIndex, multiplayerIntermissionMapItemVoteCountChanged.VoteCount);
			Action onIntermissionStateUpdated = this.OnIntermissionStateUpdated;
			if (onIntermissionStateUpdated == null)
			{
				return;
			}
			onIntermissionStateUpdated();
		}

		private void HandleServerEventIntermissionCultureItemVoteCountChanged(GameNetworkMessage baseMessage)
		{
			MultiplayerIntermissionCultureItemVoteCountChanged multiplayerIntermissionCultureItemVoteCountChanged = (MultiplayerIntermissionCultureItemVoteCountChanged)baseMessage;
			MultiplayerIntermissionVotingManager.Instance.SetVotesOfCulture(multiplayerIntermissionCultureItemVoteCountChanged.CultureItemIndex, multiplayerIntermissionCultureItemVoteCountChanged.VoteCount);
			Action onIntermissionStateUpdated = this.OnIntermissionStateUpdated;
			if (onIntermissionStateUpdated == null)
			{
				return;
			}
			onIntermissionStateUpdated();
		}

		private void HandleServerEventAddPeerComponent(GameNetworkMessage baseMessage)
		{
			AddPeerComponent addPeerComponent = (AddPeerComponent)baseMessage;
			NetworkCommunicator peer = addPeerComponent.Peer;
			uint componentId = addPeerComponent.ComponentId;
			if (peer.GetComponent(componentId) == null)
			{
				peer.AddComponent(componentId);
			}
		}

		private void HandleServerEventRemovePeerComponent(GameNetworkMessage baseMessage)
		{
			RemovePeerComponent removePeerComponent = (RemovePeerComponent)baseMessage;
			NetworkCommunicator peer = removePeerComponent.Peer;
			uint componentId = removePeerComponent.ComponentId;
			PeerComponent component = peer.GetComponent(componentId);
			peer.RemoveComponent(component);
		}

		private void HandleServerEventSynchronizingDone(GameNetworkMessage baseMessage)
		{
			SynchronizingDone synchronizingDone = (SynchronizingDone)baseMessage;
			NetworkCommunicator peer = synchronizingDone.Peer;
			Mission mission = Mission.Current;
			MissionNetworkComponent missionNetworkComponent = ((mission != null) ? mission.GetMissionBehavior<MissionNetworkComponent>() : null);
			if (missionNetworkComponent != null && !peer.IsMine)
			{
				missionNetworkComponent.OnClientSynchronized(peer);
				return;
			}
			peer.IsSynchronized = synchronizingDone.Synchronized;
			if (missionNetworkComponent != null && synchronizingDone.Synchronized)
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

		private async void HandleServerEventLoadMission(GameNetworkMessage baseMessage)
		{
			LoadMission message = (LoadMission)baseMessage;
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
				Debug.FailedAssert("[DEBUG]Invalid multiplayer game type.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Components\\BaseNetworkComponent.cs", "HandleServerEventLoadMission", 370);
			}
		}

		private void HandleServerEventUnloadMission(GameNetworkMessage baseMessage)
		{
			UnloadMission unloadMission = (UnloadMission)baseMessage;
			this.HandleServerEventUnloadMissionAux(unloadMission);
		}

		private void HandleServerEventInitializeCustomGame(GameNetworkMessage baseMessage)
		{
			InitializeCustomGameMessage initializeCustomGameMessage = (InitializeCustomGameMessage)baseMessage;
			this.InitializeCustomGameAux(initializeCustomGameMessage);
		}

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
					Debug.FailedAssert("[DEBUG]Invalid multiplayer game type.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Components\\BaseNetworkComponent.cs", "InitializeCustomGameAux", 403);
				}
			}
			else
			{
				LoadingWindow.DisableGlobalLoadingWindow();
				GameNetwork.SyncRelevantGameOptionsToServer();
			}
		}

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

		private bool HandleClientEventFinishedLoading(NetworkCommunicator networkPeer, GameNetworkMessage baseMessage)
		{
			FinishedLoading finishedLoading = (FinishedLoading)baseMessage;
			this.HandleClientEventFinishedLoadingAux(networkPeer, finishedLoading);
			return true;
		}

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

		private bool HandleSyncRelevantGameOptionsToServer(NetworkCommunicator networkPeer, GameNetworkMessage baseMessage)
		{
			SyncRelevantGameOptionsToServer syncRelevantGameOptionsToServer = (SyncRelevantGameOptionsToServer)baseMessage;
			networkPeer.SetRelevantGameOptions(syncRelevantGameOptionsToServer.SendMeBloodEvents, syncRelevantGameOptionsToServer.SendMeSoundEvents);
			return true;
		}

		private bool HandleIntermissionClientVote(NetworkCommunicator networkPeer, GameNetworkMessage baseMessage)
		{
			IntermissionVote intermissionVote = (IntermissionVote)baseMessage;
			int voteCount = intermissionVote.VoteCount;
			if (voteCount == -1 || voteCount == 1)
			{
				if ((MultiplayerIntermissionVotingManager.Instance.CurrentVoteState == MultiplayerIntermissionState.CountingForMapVote && MultiplayerIntermissionVotingManager.Instance.IsMapItem(intermissionVote.ItemID)) || (MultiplayerIntermissionVotingManager.Instance.CurrentVoteState == MultiplayerIntermissionState.CountingForCultureVote && MultiplayerIntermissionVotingManager.Instance.IsCultureItem(intermissionVote.ItemID)))
				{
					MultiplayerIntermissionVotingManager.Instance.AddVote(networkPeer.VirtualPlayer.Id, intermissionVote.ItemID, intermissionVote.VoteCount);
				}
				return true;
			}
			return false;
		}

		public const float MaxIntermissionStateTime = 240f;

		public Action OnIntermissionStateUpdated;

		public delegate void WelcomeMessageReceivedDelegate(string messageText);
	}
}
