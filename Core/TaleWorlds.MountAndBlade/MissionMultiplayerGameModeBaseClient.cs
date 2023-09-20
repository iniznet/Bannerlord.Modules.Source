using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public abstract class MissionMultiplayerGameModeBaseClient : MissionNetwork, ICameraModeLogic
	{
		public MissionLobbyComponent MissionLobbyComponent { get; private set; }

		public MissionNetworkComponent MissionNetworkComponent { get; private set; }

		public MissionScoreboardComponent ScoreboardComponent { get; private set; }

		public MultiplayerGameNotificationsComponent NotificationsComponent { get; private set; }

		public MultiplayerWarmupComponent WarmupComponent { get; private set; }

		public IRoundComponent RoundComponent { get; private set; }

		public MultiplayerTimerComponent TimerComponent { get; private set; }

		public abstract bool IsGameModeUsingGold { get; }

		public abstract bool IsGameModeTactical { get; }

		public virtual bool IsGameModeUsingCasualGold
		{
			get
			{
				return true;
			}
		}

		public abstract bool IsGameModeUsingRoundCountdown { get; }

		public virtual bool IsGameModeUsingAllowCultureChange
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsGameModeUsingAllowTroopChange
		{
			get
			{
				return false;
			}
		}

		public abstract MultiplayerGameType GameType { get; }

		public abstract int GetGoldAmount();

		public virtual SpectatorCameraTypes GetMissionCameraLockMode(bool lockedToMainPlayer)
		{
			return SpectatorCameraTypes.Invalid;
		}

		public bool IsRoundInProgress
		{
			get
			{
				IRoundComponent roundComponent = this.RoundComponent;
				return roundComponent != null && roundComponent.CurrentRoundState == MultiplayerRoundState.InProgress;
			}
		}

		public bool IsInWarmup
		{
			get
			{
				return this.MissionLobbyComponent.IsInWarmup;
			}
		}

		public float RemainingTime
		{
			get
			{
				return this.TimerComponent.GetRemainingTime(GameNetwork.IsClientOrReplay);
			}
		}

		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this.MissionLobbyComponent = base.Mission.GetMissionBehavior<MissionLobbyComponent>();
			this.MissionNetworkComponent = base.Mission.GetMissionBehavior<MissionNetworkComponent>();
			this.ScoreboardComponent = base.Mission.GetMissionBehavior<MissionScoreboardComponent>();
			this.NotificationsComponent = base.Mission.GetMissionBehavior<MultiplayerGameNotificationsComponent>();
			this.WarmupComponent = base.Mission.GetMissionBehavior<MultiplayerWarmupComponent>();
			this.RoundComponent = base.Mission.GetMissionBehavior<IRoundComponent>();
			this.TimerComponent = base.Mission.GetMissionBehavior<MultiplayerTimerComponent>();
		}

		public override void EarlyStart()
		{
			this.MissionLobbyComponent.MissionType = this.GameType;
		}

		public bool CheckTimer(out int remainingTime, out int remainingWarningTime, bool forceUpdate = false)
		{
			bool flag = false;
			float num = 0f;
			if (this.WarmupComponent != null && this.MissionLobbyComponent.CurrentMultiplayerState == MissionLobbyComponent.MultiplayerGameState.WaitingFirstPlayers)
			{
				flag = !this.WarmupComponent.IsInWarmup;
			}
			else if (this.RoundComponent != null)
			{
				flag = !this.RoundComponent.CurrentRoundState.StateHasVisualTimer();
				num = this.RoundComponent.LastRoundEndRemainingTime;
			}
			if (forceUpdate || !flag)
			{
				if (flag)
				{
					remainingTime = MathF.Ceiling(num);
				}
				else
				{
					remainingTime = MathF.Ceiling(this.RemainingTime);
				}
				remainingWarningTime = this.GetWarningTimer();
				return true;
			}
			remainingTime = 0;
			remainingWarningTime = 0;
			return false;
		}

		protected virtual int GetWarningTimer()
		{
			return 0;
		}

		public abstract void OnGoldAmountChangedForRepresentative(MissionRepresentativeBase representative, int goldAmount);

		public virtual bool CanRequestTroopChange()
		{
			return false;
		}

		public virtual bool CanRequestCultureChange()
		{
			return false;
		}
	}
}
