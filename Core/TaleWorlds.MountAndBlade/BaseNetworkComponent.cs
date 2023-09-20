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

		private void HandleServerEventAddPeerComponent(AddPeerComponent message)
		{
			NetworkCommunicator peer = message.Peer;
			uint componentId = message.ComponentId;
			if (peer.GetComponent(componentId) == null)
			{
				peer.AddComponent(componentId);
			}
		}

		private void HandleServerEventRemovePeerComponent(RemovePeerComponent message)
		{
			NetworkCommunicator peer = message.Peer;
			uint componentId = message.ComponentId;
			PeerComponent component = peer.GetComponent(componentId);
			peer.RemoveComponent(component);
		}

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

		private void HandleServerEventUnloadMission(UnloadMission message)
		{
			this.HandleServerEventUnloadMissionAux(message);
		}

		private void HandleServerEventInitializeCustomGame(InitializeCustomGameMessage message)
		{
			this.InitializeCustomGameAux(message);
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
					Debug.FailedAssert("[DEBUG]Invalid multiplayer game type.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Components\\BaseNetworkComponent.cs", "InitializeCustomGameAux", 390);
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

		private bool HandleClientEventFinishedLoading(NetworkCommunicator networkPeer, FinishedLoading message)
		{
			this.HandleClientEventFinishedLoadingAux(networkPeer, message);
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

		private bool HandleSyncRelevantGameOptionsToServer(NetworkCommunicator networkPeer, SyncRelevantGameOptionsToServer message)
		{
			networkPeer.SetRelevantGameOptions(message.SendMeBloodEvents, message.SendMeSoundEvents);
			return true;
		}

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

		public const float MaxIntermissionStateTime = 240f;

		public Action OnIntermissionStateUpdated;

		public delegate void WelcomeMessageReceivedDelegate(string messageText);
	}
}
