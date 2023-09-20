using System;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002B0 RID: 688
	public class MultiplayerWarmupComponent : MissionNetwork
	{
		// Token: 0x170006F5 RID: 1781
		// (get) Token: 0x06002608 RID: 9736 RVA: 0x00090373 File Offset: 0x0008E573
		public static float TotalWarmupDuration
		{
			get
			{
				return (float)(MultiplayerOptions.OptionType.WarmupTimeLimit.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) * 60);
			}
		}

		// Token: 0x1400006A RID: 106
		// (add) Token: 0x06002609 RID: 9737 RVA: 0x00090384 File Offset: 0x0008E584
		// (remove) Token: 0x0600260A RID: 9738 RVA: 0x000903BC File Offset: 0x0008E5BC
		public event Action OnWarmupEnding;

		// Token: 0x1400006B RID: 107
		// (add) Token: 0x0600260B RID: 9739 RVA: 0x000903F4 File Offset: 0x0008E5F4
		// (remove) Token: 0x0600260C RID: 9740 RVA: 0x0009042C File Offset: 0x0008E62C
		public event Action OnWarmupEnded;

		// Token: 0x170006F6 RID: 1782
		// (get) Token: 0x0600260D RID: 9741 RVA: 0x00090461 File Offset: 0x0008E661
		public bool IsInWarmup
		{
			get
			{
				return this.WarmupState != MultiplayerWarmupComponent.WarmupStates.Ended;
			}
		}

		// Token: 0x170006F7 RID: 1783
		// (get) Token: 0x0600260E RID: 9742 RVA: 0x0009046F File Offset: 0x0008E66F
		// (set) Token: 0x0600260F RID: 9743 RVA: 0x00090478 File Offset: 0x0008E678
		private MultiplayerWarmupComponent.WarmupStates WarmupState
		{
			get
			{
				return this._warmupState;
			}
			set
			{
				this._warmupState = value;
				if (GameNetwork.IsServer)
				{
					this._currentStateStartTime = MissionTime.Now;
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new WarmupStateChange(this._warmupState, this._currentStateStartTime.NumberOfTicks));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
				}
			}
		}

		// Token: 0x06002610 RID: 9744 RVA: 0x000904C5 File Offset: 0x0008E6C5
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._gameMode = base.Mission.GetMissionBehavior<MissionMultiplayerGameModeBase>();
			this._timerComponent = base.Mission.GetMissionBehavior<MultiplayerTimerComponent>();
			this._lobbyComponent = base.Mission.GetMissionBehavior<MissionLobbyComponent>();
		}

		// Token: 0x06002611 RID: 9745 RVA: 0x00090500 File Offset: 0x0008E700
		public override void AfterStart()
		{
			base.AfterStart();
			this.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add);
		}

		// Token: 0x06002612 RID: 9746 RVA: 0x0009050F File Offset: 0x0008E70F
		protected override void OnUdpNetworkHandlerClose()
		{
			this.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Remove);
		}

		// Token: 0x06002613 RID: 9747 RVA: 0x00090518 File Offset: 0x0008E718
		private void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode mode)
		{
			GameNetwork.NetworkMessageHandlerRegisterer networkMessageHandlerRegisterer = new GameNetwork.NetworkMessageHandlerRegisterer(mode);
			if (GameNetwork.IsClient)
			{
				networkMessageHandlerRegisterer.Register<WarmupStateChange>(new GameNetworkMessage.ServerMessageHandlerDelegate<WarmupStateChange>(this.HandleServerEventWarmupStateChange));
			}
		}

		// Token: 0x06002614 RID: 9748 RVA: 0x00090545 File Offset: 0x0008E745
		public bool CheckForWarmupProgressEnd()
		{
			return this._gameMode.CheckForWarmupEnd() || this._timerComponent.GetRemainingTime(false) <= 30f;
		}

		// Token: 0x06002615 RID: 9749 RVA: 0x0009056C File Offset: 0x0008E76C
		public override void OnPreDisplayMissionTick(float dt)
		{
			if (GameNetwork.IsServer)
			{
				switch (this.WarmupState)
				{
				case MultiplayerWarmupComponent.WarmupStates.WaitingForPlayers:
					this.BeginWarmup();
					return;
				case MultiplayerWarmupComponent.WarmupStates.InProgress:
					if (this.CheckForWarmupProgressEnd())
					{
						this.EndWarmupProgress();
						return;
					}
					break;
				case MultiplayerWarmupComponent.WarmupStates.Ending:
					if (this._timerComponent.CheckIfTimerPassed())
					{
						this.EndWarmup();
						return;
					}
					break;
				case MultiplayerWarmupComponent.WarmupStates.Ended:
					if (this._timerComponent.CheckIfTimerPassed())
					{
						base.Mission.RemoveMissionBehavior(this);
						return;
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
		}

		// Token: 0x06002616 RID: 9750 RVA: 0x000905EC File Offset: 0x0008E7EC
		private void BeginWarmup()
		{
			this.WarmupState = MultiplayerWarmupComponent.WarmupStates.InProgress;
			Mission.Current.ResetMission();
			this._gameMode.MultiplayerTeamSelectComponent.BalanceTeams();
			this._timerComponent.StartTimerAsServer(MultiplayerWarmupComponent.TotalWarmupDuration);
			this._gameMode.SpawnComponent.SpawningBehavior.Clear();
			SpawnComponent.SetWarmupSpawningBehavior();
		}

		// Token: 0x06002617 RID: 9751 RVA: 0x00090644 File Offset: 0x0008E844
		private void EndWarmupProgress()
		{
			this.WarmupState = MultiplayerWarmupComponent.WarmupStates.Ending;
			this._timerComponent.StartTimerAsServer(30f);
			Action onWarmupEnding = this.OnWarmupEnding;
			if (onWarmupEnding == null)
			{
				return;
			}
			onWarmupEnding();
		}

		// Token: 0x06002618 RID: 9752 RVA: 0x00090670 File Offset: 0x0008E870
		private void EndWarmup()
		{
			this.WarmupState = MultiplayerWarmupComponent.WarmupStates.Ended;
			this._timerComponent.StartTimerAsServer(3f);
			Action onWarmupEnded = this.OnWarmupEnded;
			if (onWarmupEnded != null)
			{
				onWarmupEnded();
			}
			if (!GameNetwork.IsDedicatedServer)
			{
				this.PlayBattleStartingSound();
			}
			Mission.Current.ResetMission();
			this._gameMode.MultiplayerTeamSelectComponent.BalanceTeams();
			this._gameMode.SpawnComponent.SpawningBehavior.Clear();
			SpawnComponent.SetSpawningBehaviorForCurrentGameType(this._gameMode.GetMissionType());
			if (!this.CanMatchStartAfterWarmup())
			{
				this._lobbyComponent.SetStateEndingAsServer();
			}
		}

		// Token: 0x06002619 RID: 9753 RVA: 0x00090704 File Offset: 0x0008E904
		public bool CanMatchStartAfterWarmup()
		{
			bool[] array = new bool[2];
			foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
			{
				MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
				if (((component != null) ? component.Team : null) != null && component.Team.Side != BattleSideEnum.None)
				{
					array[(int)component.Team.Side] = true;
				}
				if (array[1] && array[0])
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600261A RID: 9754 RVA: 0x00090790 File Offset: 0x0008E990
		public override void OnRemoveBehavior()
		{
			base.OnRemoveBehavior();
			this.OnWarmupEnding = null;
			this.OnWarmupEnded = null;
			if (GameNetwork.IsServer && !this._gameMode.UseRoundController() && this._lobbyComponent.CurrentMultiplayerState != MissionLobbyComponent.MultiplayerGameState.Ending)
			{
				this._gameMode.SpawnComponent.SpawningBehavior.RequestStartSpawnSession();
			}
		}

		// Token: 0x0600261B RID: 9755 RVA: 0x000907E8 File Offset: 0x0008E9E8
		protected override void HandleNewClientAfterSynchronized(NetworkCommunicator networkPeer)
		{
			if (this.IsInWarmup && !networkPeer.IsServerPeer)
			{
				GameNetwork.BeginModuleEventAsServer(networkPeer);
				GameNetwork.WriteMessage(new WarmupStateChange(this._warmupState, this._currentStateStartTime.NumberOfTicks));
				GameNetwork.EndModuleEventAsServer();
			}
		}

		// Token: 0x0600261C RID: 9756 RVA: 0x00090820 File Offset: 0x0008EA20
		private void HandleServerEventWarmupStateChange(WarmupStateChange message)
		{
			this.WarmupState = message.WarmupState;
			switch (this.WarmupState)
			{
			case MultiplayerWarmupComponent.WarmupStates.InProgress:
				this._timerComponent.StartTimerAsClient(message.StateStartTimeInSeconds, MultiplayerWarmupComponent.TotalWarmupDuration);
				return;
			case MultiplayerWarmupComponent.WarmupStates.Ending:
			{
				this._timerComponent.StartTimerAsClient(message.StateStartTimeInSeconds, 30f);
				Action onWarmupEnding = this.OnWarmupEnding;
				if (onWarmupEnding == null)
				{
					return;
				}
				onWarmupEnding();
				return;
			}
			case MultiplayerWarmupComponent.WarmupStates.Ended:
			{
				this._timerComponent.StartTimerAsClient(message.StateStartTimeInSeconds, 3f);
				Action onWarmupEnded = this.OnWarmupEnded;
				if (onWarmupEnded != null)
				{
					onWarmupEnded();
				}
				this.PlayBattleStartingSound();
				return;
			}
			default:
				return;
			}
		}

		// Token: 0x0600261D RID: 9757 RVA: 0x000908C0 File Offset: 0x0008EAC0
		private void PlayBattleStartingSound()
		{
			MatrixFrame cameraFrame = Mission.Current.GetCameraFrame();
			Vec3 vec = cameraFrame.origin + cameraFrame.rotation.u;
			NetworkCommunicator myPeer = GameNetwork.MyPeer;
			MissionPeer missionPeer = ((myPeer != null) ? myPeer.GetComponent<MissionPeer>() : null);
			if (((missionPeer != null) ? missionPeer.Team : null) != null)
			{
				string text = ((missionPeer.Team.Side == BattleSideEnum.Attacker) ? MultiplayerOptions.OptionType.CultureTeam1.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) : MultiplayerOptions.OptionType.CultureTeam2.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
				MBSoundEvent.PlaySound(SoundEvent.GetEventIdFromString("event:/alerts/rally/" + text.ToLower()), vec);
				return;
			}
			MBSoundEvent.PlaySound(SoundEvent.GetEventIdFromString("event:/alerts/rally/generic"), vec);
		}

		// Token: 0x04000E1A RID: 3610
		public const int RespawnPeriodInWarmup = 3;

		// Token: 0x04000E1B RID: 3611
		public const int WarmupEndWaitTime = 30;

		// Token: 0x04000E1E RID: 3614
		private MissionMultiplayerGameModeBase _gameMode;

		// Token: 0x04000E1F RID: 3615
		private MultiplayerTimerComponent _timerComponent;

		// Token: 0x04000E20 RID: 3616
		private MissionLobbyComponent _lobbyComponent;

		// Token: 0x04000E21 RID: 3617
		private MissionTime _currentStateStartTime;

		// Token: 0x04000E22 RID: 3618
		private MultiplayerWarmupComponent.WarmupStates _warmupState;

		// Token: 0x020005D3 RID: 1491
		public enum WarmupStates
		{
			// Token: 0x04001E4B RID: 7755
			WaitingForPlayers,
			// Token: 0x04001E4C RID: 7756
			InProgress,
			// Token: 0x04001E4D RID: 7757
			Ending,
			// Token: 0x04001E4E RID: 7758
			Ended
		}
	}
}
