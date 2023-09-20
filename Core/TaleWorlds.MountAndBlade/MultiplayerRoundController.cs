using System;
using System.Linq;
using NetworkMessages.FromClient;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002AC RID: 684
	public class MultiplayerRoundController : MissionNetwork, IRoundComponent, IMissionBehavior
	{
		// Token: 0x14000060 RID: 96
		// (add) Token: 0x060025B2 RID: 9650 RVA: 0x0008EC08 File Offset: 0x0008CE08
		// (remove) Token: 0x060025B3 RID: 9651 RVA: 0x0008EC40 File Offset: 0x0008CE40
		public event Action OnRoundStarted;

		// Token: 0x14000061 RID: 97
		// (add) Token: 0x060025B4 RID: 9652 RVA: 0x0008EC78 File Offset: 0x0008CE78
		// (remove) Token: 0x060025B5 RID: 9653 RVA: 0x0008ECB0 File Offset: 0x0008CEB0
		public event Action OnPreparationEnded;

		// Token: 0x14000062 RID: 98
		// (add) Token: 0x060025B6 RID: 9654 RVA: 0x0008ECE8 File Offset: 0x0008CEE8
		// (remove) Token: 0x060025B7 RID: 9655 RVA: 0x0008ED20 File Offset: 0x0008CF20
		public event Action OnPreRoundEnding;

		// Token: 0x14000063 RID: 99
		// (add) Token: 0x060025B8 RID: 9656 RVA: 0x0008ED58 File Offset: 0x0008CF58
		// (remove) Token: 0x060025B9 RID: 9657 RVA: 0x0008ED90 File Offset: 0x0008CF90
		public event Action OnRoundEnding;

		// Token: 0x14000064 RID: 100
		// (add) Token: 0x060025BA RID: 9658 RVA: 0x0008EDC8 File Offset: 0x0008CFC8
		// (remove) Token: 0x060025BB RID: 9659 RVA: 0x0008EE00 File Offset: 0x0008D000
		public event Action OnPostRoundEnded;

		// Token: 0x14000065 RID: 101
		// (add) Token: 0x060025BC RID: 9660 RVA: 0x0008EE38 File Offset: 0x0008D038
		// (remove) Token: 0x060025BD RID: 9661 RVA: 0x0008EE70 File Offset: 0x0008D070
		public event Action OnCurrentRoundStateChanged;

		// Token: 0x170006EB RID: 1771
		// (get) Token: 0x060025BE RID: 9662 RVA: 0x0008EEA5 File Offset: 0x0008D0A5
		// (set) Token: 0x060025BF RID: 9663 RVA: 0x0008EEAD File Offset: 0x0008D0AD
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

		// Token: 0x170006EC RID: 1772
		// (get) Token: 0x060025C0 RID: 9664 RVA: 0x0008EEE2 File Offset: 0x0008D0E2
		// (set) Token: 0x060025C1 RID: 9665 RVA: 0x0008EEEA File Offset: 0x0008D0EA
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

		// Token: 0x170006ED RID: 1773
		// (get) Token: 0x060025C2 RID: 9666 RVA: 0x0008EF1A File Offset: 0x0008D11A
		// (set) Token: 0x060025C3 RID: 9667 RVA: 0x0008EF22 File Offset: 0x0008D122
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

		// Token: 0x170006EE RID: 1774
		// (get) Token: 0x060025C4 RID: 9668 RVA: 0x0008EF52 File Offset: 0x0008D152
		// (set) Token: 0x060025C5 RID: 9669 RVA: 0x0008EF5A File Offset: 0x0008D15A
		public bool IsMatchEnding { get; private set; }

		// Token: 0x170006EF RID: 1775
		// (get) Token: 0x060025C6 RID: 9670 RVA: 0x0008EF63 File Offset: 0x0008D163
		// (set) Token: 0x060025C7 RID: 9671 RVA: 0x0008EF6B File Offset: 0x0008D16B
		public float LastRoundEndRemainingTime { get; private set; }

		// Token: 0x170006F0 RID: 1776
		// (get) Token: 0x060025C8 RID: 9672 RVA: 0x0008EF74 File Offset: 0x0008D174
		public float RemainingRoundTime
		{
			get
			{
				return this._gameModeServer.TimerComponent.GetRemainingTime(false);
			}
		}

		// Token: 0x170006F1 RID: 1777
		// (get) Token: 0x060025C9 RID: 9673 RVA: 0x0008EF87 File Offset: 0x0008D187
		// (set) Token: 0x060025CA RID: 9674 RVA: 0x0008EF8F File Offset: 0x0008D18F
		public MultiplayerRoundState CurrentRoundState { get; private set; }

		// Token: 0x170006F2 RID: 1778
		// (get) Token: 0x060025CB RID: 9675 RVA: 0x0008EF98 File Offset: 0x0008D198
		public bool IsRoundInProgress
		{
			get
			{
				return this.CurrentRoundState == MultiplayerRoundState.InProgress;
			}
		}

		// Token: 0x060025CC RID: 9676 RVA: 0x0008EFA3 File Offset: 0x0008D1A3
		public void EnableEquipmentUpdate()
		{
			this._equipmentUpdateDisabled = false;
		}

		// Token: 0x060025CD RID: 9677 RVA: 0x0008EFAC File Offset: 0x0008D1AC
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

		// Token: 0x060025CE RID: 9678 RVA: 0x0008F004 File Offset: 0x0008D204
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

		// Token: 0x060025CF RID: 9679 RVA: 0x0008F05A File Offset: 0x0008D25A
		private bool CheckPostEndRound()
		{
			return this._gameModeServer.TimerComponent.CheckIfTimerPassed();
		}

		// Token: 0x060025D0 RID: 9680 RVA: 0x0008F06C File Offset: 0x0008D26C
		private bool CheckPostMatchEnd()
		{
			return this._gameModeServer.TimerComponent.CheckIfTimerPassed();
		}

		// Token: 0x060025D1 RID: 9681 RVA: 0x0008F080 File Offset: 0x0008D280
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

		// Token: 0x060025D2 RID: 9682 RVA: 0x0008F0E3 File Offset: 0x0008D2E3
		private void PostMatchEnd()
		{
			this._gameModeServer.TimerComponent.StartTimerAsServer(5f);
			this.ChangeRoundState(MultiplayerRoundState.MatchEnded);
			this._missionLobbyComponent.SetStateEndingAsServer();
		}

		// Token: 0x060025D3 RID: 9683 RVA: 0x0008F10C File Offset: 0x0008D30C
		public override void OnRemoveBehavior()
		{
			GameNetwork.RemoveNetworkHandler(this);
			base.OnRemoveBehavior();
		}

		// Token: 0x060025D4 RID: 9684 RVA: 0x0008F11C File Offset: 0x0008D31C
		private void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode mode)
		{
			GameNetwork.NetworkMessageHandlerRegisterer networkMessageHandlerRegisterer = new GameNetwork.NetworkMessageHandlerRegisterer(mode);
			if (!GameNetwork.IsClient && GameNetwork.IsServer)
			{
				networkMessageHandlerRegisterer.Register<CultureVoteClient>(new GameNetworkMessage.ClientMessageHandlerDelegate<CultureVoteClient>(this.HandleClientEventCultureSelect));
			}
		}

		// Token: 0x060025D5 RID: 9685 RVA: 0x0008F150 File Offset: 0x0008D350
		protected override void OnUdpNetworkHandlerClose()
		{
			this.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Remove);
		}

		// Token: 0x060025D6 RID: 9686 RVA: 0x0008F15C File Offset: 0x0008D35C
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

		// Token: 0x060025D7 RID: 9687 RVA: 0x0008F250 File Offset: 0x0008D450
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

		// Token: 0x060025D8 RID: 9688 RVA: 0x0008F2CB File Offset: 0x0008D4CB
		protected override void HandleLateNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
		{
		}

		// Token: 0x060025D9 RID: 9689 RVA: 0x0008F2CD File Offset: 0x0008D4CD
		public bool HandleClientEventCultureSelect(NetworkCommunicator peer, CultureVoteClient message)
		{
			peer.GetComponent<MissionPeer>().HandleVoteChange(message.VotedType, message.VotedCulture);
			return true;
		}

		// Token: 0x060025DA RID: 9690 RVA: 0x0008F2E8 File Offset: 0x0008D4E8
		private bool CheckForRoundEnd()
		{
			if (!this._roundTimeOver)
			{
				this._roundTimeOver = this._gameModeServer.TimerComponent.CheckIfTimerPassed();
			}
			return (!this._gameModeServer.CheckIfOvertime() && this._roundTimeOver) || this._gameModeServer.CheckForRoundEnd();
		}

		// Token: 0x060025DB RID: 9691 RVA: 0x0008F338 File Offset: 0x0008D538
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

		// Token: 0x060025DC RID: 9692 RVA: 0x0008F40C File Offset: 0x0008D60C
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

		// Token: 0x060025DD RID: 9693 RVA: 0x0008F490 File Offset: 0x0008D690
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

		// Token: 0x060025DE RID: 9694 RVA: 0x0008F51C File Offset: 0x0008D71C
		private bool CheckForPreparationEnd()
		{
			return this.CurrentRoundState == MultiplayerRoundState.Preparation && this._gameModeServer.TimerComponent.CheckIfTimerPassed();
		}

		// Token: 0x060025DF RID: 9695 RVA: 0x0008F539 File Offset: 0x0008D739
		private void EndPreparation()
		{
			if (this.OnPreparationEnded != null)
			{
				this.OnPreparationEnded();
			}
		}

		// Token: 0x060025E0 RID: 9696 RVA: 0x0008F54E File Offset: 0x0008D74E
		private void StartSpawning(bool disableEquipmentUpdate = true)
		{
			this._gameModeServer.TimerComponent.StartTimerAsServer((float)MultiplayerOptions.OptionType.RoundTimeLimit.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
			if (disableEquipmentUpdate)
			{
				this._gameModeServer.SpawnComponent.ToggleUpdatingSpawnEquipment(false);
			}
			this.ChangeRoundState(MultiplayerRoundState.InProgress);
		}

		// Token: 0x060025E1 RID: 9697 RVA: 0x0008F584 File Offset: 0x0008D784
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

		// Token: 0x060025E2 RID: 9698 RVA: 0x0008F5C8 File Offset: 0x0008D7C8
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

		// Token: 0x04000DFC RID: 3580
		private MissionMultiplayerGameModeBase _gameModeServer;

		// Token: 0x04000DFD RID: 3581
		private int _roundCount;

		// Token: 0x04000DFE RID: 3582
		private BattleSideEnum _roundWinner;

		// Token: 0x04000DFF RID: 3583
		private RoundEndReason _roundEndReason;

		// Token: 0x04000E00 RID: 3584
		private MissionLobbyComponent _missionLobbyComponent;

		// Token: 0x04000E02 RID: 3586
		private bool _roundTimeOver;

		// Token: 0x04000E04 RID: 3588
		private MissionTime _currentRoundStateStartTime;

		// Token: 0x04000E06 RID: 3590
		private bool _equipmentUpdateDisabled = true;
	}
}
