using System;
using System.Linq;
using NetworkMessages.FromClient;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
	public class MultiplayerRoundController : MissionNetwork, IRoundComponent, IMissionBehavior
	{
		public event Action OnRoundStarted;

		public event Action OnPreparationEnded;

		public event Action OnPreRoundEnding;

		public event Action OnRoundEnding;

		public event Action OnPostRoundEnded;

		public event Action OnCurrentRoundStateChanged;

		public int RoundCount
		{
			get
			{
				return this._roundCount;
			}
			set
			{
				if (this._roundCount != value)
				{
					this._roundCount = value;
					if (GameNetwork.IsServer)
					{
						GameNetwork.BeginBroadcastModuleEvent();
						GameNetwork.WriteMessage(new RoundCountChange(this._roundCount));
						GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
					}
				}
			}
		}

		public BattleSideEnum RoundWinner
		{
			get
			{
				return this._roundWinner;
			}
			set
			{
				if (this._roundWinner != value)
				{
					this._roundWinner = value;
					if (GameNetwork.IsServer)
					{
						GameNetwork.BeginBroadcastModuleEvent();
						GameNetwork.WriteMessage(new RoundWinnerChange(value));
						GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
					}
				}
			}
		}

		public RoundEndReason RoundEndReason
		{
			get
			{
				return this._roundEndReason;
			}
			set
			{
				if (this._roundEndReason != value)
				{
					this._roundEndReason = value;
					if (GameNetwork.IsServer)
					{
						GameNetwork.BeginBroadcastModuleEvent();
						GameNetwork.WriteMessage(new RoundEndReasonChange(value));
						GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
					}
				}
			}
		}

		public bool IsMatchEnding { get; private set; }

		public float LastRoundEndRemainingTime { get; private set; }

		public float RemainingRoundTime
		{
			get
			{
				return this._gameModeServer.TimerComponent.GetRemainingTime(false);
			}
		}

		public MultiplayerRoundState CurrentRoundState { get; private set; }

		public bool IsRoundInProgress
		{
			get
			{
				return this.CurrentRoundState == MultiplayerRoundState.InProgress;
			}
		}

		public void EnableEquipmentUpdate()
		{
			this._equipmentUpdateDisabled = false;
		}

		public override void AfterStart()
		{
			this.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add);
			if (GameNetwork.IsServerOrRecorder)
			{
				this._gameModeServer = Mission.Current.GetMissionBehavior<MissionMultiplayerGameModeBase>();
			}
			this._missionLobbyComponent = Mission.Current.GetMissionBehavior<MissionLobbyComponent>();
			this._roundCount = 0;
			this._gameModeServer.TimerComponent.StartTimerAsServer(8f);
		}

		private void EndRound()
		{
			if (this.OnPreRoundEnding != null)
			{
				this.OnPreRoundEnding();
			}
			this.ChangeRoundState(MultiplayerRoundState.Ending);
			this._gameModeServer.TimerComponent.StartTimerAsServer(3f);
			this._roundTimeOver = false;
			if (this.OnRoundEnding != null)
			{
				this.OnRoundEnding();
			}
		}

		private bool CheckPostEndRound()
		{
			return this._gameModeServer.TimerComponent.CheckIfTimerPassed();
		}

		private bool CheckPostMatchEnd()
		{
			return this._gameModeServer.TimerComponent.CheckIfTimerPassed();
		}

		private void PostRoundEnd()
		{
			this._gameModeServer.TimerComponent.StartTimerAsServer(5f);
			this.ChangeRoundState(MultiplayerRoundState.Ended);
			if (this._roundCount == MultiplayerOptions.OptionType.RoundTotal.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) || this.CheckForMatchEndEarly() || !this.HasEnoughCharactersOnBothSides())
			{
				this.IsMatchEnding = true;
			}
			if (this.OnPostRoundEnded != null)
			{
				this.OnPostRoundEnded();
			}
		}

		private void PostMatchEnd()
		{
			this._gameModeServer.TimerComponent.StartTimerAsServer(5f);
			this.ChangeRoundState(MultiplayerRoundState.MatchEnded);
			this._missionLobbyComponent.SetStateEndingAsServer();
		}

		public override void OnRemoveBehavior()
		{
			GameNetwork.RemoveNetworkHandler(this);
			base.OnRemoveBehavior();
		}

		private void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode mode)
		{
			GameNetwork.NetworkMessageHandlerRegisterer networkMessageHandlerRegisterer = new GameNetwork.NetworkMessageHandlerRegisterer(mode);
			if (!GameNetwork.IsClient && GameNetwork.IsServer)
			{
				networkMessageHandlerRegisterer.Register<CultureVoteClient>(new GameNetworkMessage.ClientMessageHandlerDelegate<CultureVoteClient>(this.HandleClientEventCultureSelect));
			}
		}

		protected override void OnUdpNetworkHandlerClose()
		{
			this.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Remove);
		}

		public override void OnPreDisplayMissionTick(float dt)
		{
			if (GameNetwork.IsServer)
			{
				if (this._missionLobbyComponent.CurrentMultiplayerState != MissionLobbyComponent.MultiplayerGameState.WaitingFirstPlayers)
				{
					if (!this.IsMatchEnding && this._missionLobbyComponent.CurrentMultiplayerState != MissionLobbyComponent.MultiplayerGameState.Ending && (this.CurrentRoundState == MultiplayerRoundState.WaitingForPlayers || this.CurrentRoundState == MultiplayerRoundState.Ended))
					{
						if (this.CheckForNewRound())
						{
							this.BeginNewRound();
							return;
						}
						if (this.IsMatchEnding)
						{
							this.PostMatchEnd();
							return;
						}
					}
					else if (this.CurrentRoundState == MultiplayerRoundState.Preparation)
					{
						if (this.CheckForPreparationEnd())
						{
							this.EndPreparation();
							this.StartSpawning(this._equipmentUpdateDisabled);
							return;
						}
					}
					else if (this.CurrentRoundState == MultiplayerRoundState.InProgress)
					{
						if (this.CheckForRoundEnd())
						{
							this.EndRound();
							return;
						}
					}
					else if (this.CurrentRoundState == MultiplayerRoundState.Ending)
					{
						if (this.CheckPostEndRound())
						{
							this.PostRoundEnd();
							return;
						}
					}
					else if (this.CurrentRoundState == MultiplayerRoundState.Ended && this.IsMatchEnding && this.CheckPostMatchEnd())
					{
						this.PostMatchEnd();
						return;
					}
				}
			}
			else
			{
				this._gameModeServer.TimerComponent.CheckIfTimerPassed();
			}
		}

		private void ChangeRoundState(MultiplayerRoundState newRoundState)
		{
			if (this.CurrentRoundState != newRoundState)
			{
				if (this.CurrentRoundState == MultiplayerRoundState.InProgress)
				{
					this.LastRoundEndRemainingTime = this.RemainingRoundTime;
				}
				this.CurrentRoundState = newRoundState;
				this._currentRoundStateStartTime = MissionTime.Now;
				Action onCurrentRoundStateChanged = this.OnCurrentRoundStateChanged;
				if (onCurrentRoundStateChanged != null)
				{
					onCurrentRoundStateChanged();
				}
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new RoundStateChange(newRoundState, this._currentRoundStateStartTime.NumberOfTicks, MathF.Ceiling(this.LastRoundEndRemainingTime)));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
			}
		}

		protected override void HandleLateNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
		{
		}

		public bool HandleClientEventCultureSelect(NetworkCommunicator peer, CultureVoteClient message)
		{
			peer.GetComponent<MissionPeer>().HandleVoteChange(message.VotedType, message.VotedCulture);
			return true;
		}

		private bool CheckForRoundEnd()
		{
			if (!this._roundTimeOver)
			{
				this._roundTimeOver = this._gameModeServer.TimerComponent.CheckIfTimerPassed();
			}
			return (!this._gameModeServer.CheckIfOvertime() && this._roundTimeOver) || this._gameModeServer.CheckForRoundEnd();
		}

		private bool CheckForNewRound()
		{
			if (this.CurrentRoundState != MultiplayerRoundState.WaitingForPlayers && !this._gameModeServer.TimerComponent.CheckIfTimerPassed())
			{
				return false;
			}
			int[] array = new int[2];
			foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
			{
				MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
				if (networkCommunicator.IsSynchronized && ((component != null) ? component.Team : null) != null && component.Team.Side != BattleSideEnum.None)
				{
					array[(int)component.Team.Side]++;
				}
			}
			if (array.Sum() < MultiplayerOptions.OptionType.MinNumberOfPlayersForMatchStart.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) && this.RoundCount == 0)
			{
				this.IsMatchEnding = true;
				return false;
			}
			return true;
		}

		private bool HasEnoughCharactersOnBothSides()
		{
			bool flag;
			if (MultiplayerOptions.OptionType.NumberOfBotsTeam1.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) <= 0)
			{
				flag = GameNetwork.NetworkPeers.Count((NetworkCommunicator q) => q.GetComponent<MissionPeer>() != null && q.GetComponent<MissionPeer>().Team == Mission.Current.AttackerTeam) > 0;
			}
			else
			{
				flag = true;
			}
			bool flag2;
			if (MultiplayerOptions.OptionType.NumberOfBotsTeam2.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) <= 0)
			{
				flag2 = GameNetwork.NetworkPeers.Count((NetworkCommunicator q) => q.GetComponent<MissionPeer>() != null && q.GetComponent<MissionPeer>().Team == Mission.Current.DefenderTeam) > 0;
			}
			else
			{
				flag2 = true;
			}
			bool flag3 = flag2;
			return flag && flag3;
		}

		private void BeginNewRound()
		{
			if (this.CurrentRoundState == MultiplayerRoundState.WaitingForPlayers)
			{
				this._gameModeServer.ClearPeerCounts();
			}
			this.ChangeRoundState(MultiplayerRoundState.Preparation);
			int roundCount = this.RoundCount;
			this.RoundCount = roundCount + 1;
			Mission.Current.ResetMission();
			this._gameModeServer.MultiplayerTeamSelectComponent.BalanceTeams();
			this._gameModeServer.TimerComponent.StartTimerAsServer((float)MultiplayerOptions.OptionType.RoundPreparationTimeLimit.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
			Action onRoundStarted = this.OnRoundStarted;
			if (onRoundStarted != null)
			{
				onRoundStarted();
			}
			this._gameModeServer.SpawnComponent.ToggleUpdatingSpawnEquipment(true);
		}

		private bool CheckForPreparationEnd()
		{
			return this.CurrentRoundState == MultiplayerRoundState.Preparation && this._gameModeServer.TimerComponent.CheckIfTimerPassed();
		}

		private void EndPreparation()
		{
			if (this.OnPreparationEnded != null)
			{
				this.OnPreparationEnded();
			}
		}

		private void StartSpawning(bool disableEquipmentUpdate = true)
		{
			this._gameModeServer.TimerComponent.StartTimerAsServer((float)MultiplayerOptions.OptionType.RoundTimeLimit.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
			if (disableEquipmentUpdate)
			{
				this._gameModeServer.SpawnComponent.ToggleUpdatingSpawnEquipment(false);
			}
			this.ChangeRoundState(MultiplayerRoundState.InProgress);
		}

		private bool CheckForMatchEndEarly()
		{
			bool flag = false;
			MissionScoreboardComponent missionBehavior = Mission.Current.GetMissionBehavior<MissionScoreboardComponent>();
			if (missionBehavior != null)
			{
				for (int i = 0; i < 2; i++)
				{
					if (missionBehavior.GetRoundScore((BattleSideEnum)i) > MultiplayerOptions.OptionType.RoundTotal.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) / 2)
					{
						flag = true;
						break;
					}
				}
			}
			return flag;
		}

		protected override void HandleNewClientAfterSynchronized(NetworkCommunicator networkPeer)
		{
			if (!networkPeer.IsServerPeer)
			{
				GameNetwork.BeginModuleEventAsServer(networkPeer);
				GameNetwork.WriteMessage(new RoundStateChange(this.CurrentRoundState, this._currentRoundStateStartTime.NumberOfTicks, MathF.Ceiling(this.LastRoundEndRemainingTime)));
				GameNetwork.EndModuleEventAsServer();
				GameNetwork.BeginModuleEventAsServer(networkPeer);
				GameNetwork.WriteMessage(new RoundWinnerChange(this.RoundWinner));
				GameNetwork.EndModuleEventAsServer();
				GameNetwork.BeginModuleEventAsServer(networkPeer);
				GameNetwork.WriteMessage(new RoundCountChange(this.RoundCount));
				GameNetwork.EndModuleEventAsServer();
			}
		}

		private MissionMultiplayerGameModeBase _gameModeServer;

		private int _roundCount;

		private BattleSideEnum _roundWinner;

		private RoundEndReason _roundEndReason;

		private MissionLobbyComponent _missionLobbyComponent;

		private bool _roundTimeOver;

		private MissionTime _currentRoundStateStartTime;

		private bool _equipmentUpdateDisabled = true;
	}
}
