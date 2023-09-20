using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000299 RID: 665
	public abstract class MissionMultiplayerGameModeBaseClient : MissionNetwork, ICameraModeLogic
	{
		// Token: 0x170006A7 RID: 1703
		// (get) Token: 0x060023F1 RID: 9201 RVA: 0x00085068 File Offset: 0x00083268
		// (set) Token: 0x060023F2 RID: 9202 RVA: 0x00085070 File Offset: 0x00083270
		public MissionLobbyComponent MissionLobbyComponent { get; private set; }

		// Token: 0x170006A8 RID: 1704
		// (get) Token: 0x060023F3 RID: 9203 RVA: 0x00085079 File Offset: 0x00083279
		// (set) Token: 0x060023F4 RID: 9204 RVA: 0x00085081 File Offset: 0x00083281
		public MissionNetworkComponent MissionNetworkComponent { get; private set; }

		// Token: 0x170006A9 RID: 1705
		// (get) Token: 0x060023F5 RID: 9205 RVA: 0x0008508A File Offset: 0x0008328A
		// (set) Token: 0x060023F6 RID: 9206 RVA: 0x00085092 File Offset: 0x00083292
		public MissionScoreboardComponent ScoreboardComponent { get; private set; }

		// Token: 0x170006AA RID: 1706
		// (get) Token: 0x060023F7 RID: 9207 RVA: 0x0008509B File Offset: 0x0008329B
		// (set) Token: 0x060023F8 RID: 9208 RVA: 0x000850A3 File Offset: 0x000832A3
		public MultiplayerGameNotificationsComponent NotificationsComponent { get; private set; }

		// Token: 0x170006AB RID: 1707
		// (get) Token: 0x060023F9 RID: 9209 RVA: 0x000850AC File Offset: 0x000832AC
		// (set) Token: 0x060023FA RID: 9210 RVA: 0x000850B4 File Offset: 0x000832B4
		public MultiplayerWarmupComponent WarmupComponent { get; private set; }

		// Token: 0x170006AC RID: 1708
		// (get) Token: 0x060023FB RID: 9211 RVA: 0x000850BD File Offset: 0x000832BD
		// (set) Token: 0x060023FC RID: 9212 RVA: 0x000850C5 File Offset: 0x000832C5
		public IRoundComponent RoundComponent { get; private set; }

		// Token: 0x170006AD RID: 1709
		// (get) Token: 0x060023FD RID: 9213 RVA: 0x000850CE File Offset: 0x000832CE
		// (set) Token: 0x060023FE RID: 9214 RVA: 0x000850D6 File Offset: 0x000832D6
		public MultiplayerTimerComponent TimerComponent { get; private set; }

		// Token: 0x170006AE RID: 1710
		// (get) Token: 0x060023FF RID: 9215
		public abstract bool IsGameModeUsingGold { get; }

		// Token: 0x170006AF RID: 1711
		// (get) Token: 0x06002400 RID: 9216
		public abstract bool IsGameModeTactical { get; }

		// Token: 0x170006B0 RID: 1712
		// (get) Token: 0x06002401 RID: 9217 RVA: 0x000850DF File Offset: 0x000832DF
		public virtual bool IsGameModeUsingCasualGold
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170006B1 RID: 1713
		// (get) Token: 0x06002402 RID: 9218
		public abstract bool IsGameModeUsingRoundCountdown { get; }

		// Token: 0x170006B2 RID: 1714
		// (get) Token: 0x06002403 RID: 9219 RVA: 0x000850E2 File Offset: 0x000832E2
		public virtual bool IsGameModeUsingAllowCultureChange
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170006B3 RID: 1715
		// (get) Token: 0x06002404 RID: 9220 RVA: 0x000850E5 File Offset: 0x000832E5
		public virtual bool IsGameModeUsingAllowTroopChange
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170006B4 RID: 1716
		// (get) Token: 0x06002405 RID: 9221
		public abstract MissionLobbyComponent.MultiplayerGameType GameType { get; }

		// Token: 0x06002406 RID: 9222
		public abstract int GetGoldAmount();

		// Token: 0x06002407 RID: 9223 RVA: 0x000850E8 File Offset: 0x000832E8
		public virtual SpectatorCameraTypes GetMissionCameraLockMode(bool lockedToMainPlayer)
		{
			return SpectatorCameraTypes.Invalid;
		}

		// Token: 0x170006B5 RID: 1717
		// (get) Token: 0x06002408 RID: 9224 RVA: 0x000850EB File Offset: 0x000832EB
		public bool IsRoundInProgress
		{
			get
			{
				IRoundComponent roundComponent = this.RoundComponent;
				return roundComponent != null && roundComponent.CurrentRoundState == MultiplayerRoundState.InProgress;
			}
		}

		// Token: 0x170006B6 RID: 1718
		// (get) Token: 0x06002409 RID: 9225 RVA: 0x00085101 File Offset: 0x00083301
		public bool IsInWarmup
		{
			get
			{
				return this.MissionLobbyComponent.IsInWarmup;
			}
		}

		// Token: 0x170006B7 RID: 1719
		// (get) Token: 0x0600240A RID: 9226 RVA: 0x0008510E File Offset: 0x0008330E
		public float RemainingTime
		{
			get
			{
				return this.TimerComponent.GetRemainingTime(GameNetwork.IsClientOrReplay);
			}
		}

		// Token: 0x0600240B RID: 9227 RVA: 0x00085120 File Offset: 0x00083320
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

		// Token: 0x0600240C RID: 9228 RVA: 0x000851AA File Offset: 0x000833AA
		public override void EarlyStart()
		{
			this.MissionLobbyComponent.MissionType = this.GameType;
		}

		// Token: 0x0600240D RID: 9229 RVA: 0x000851C0 File Offset: 0x000833C0
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

		// Token: 0x0600240E RID: 9230 RVA: 0x00085254 File Offset: 0x00083454
		protected virtual int GetWarningTimer()
		{
			return 0;
		}

		// Token: 0x0600240F RID: 9231
		public abstract void OnGoldAmountChangedForRepresentative(MissionRepresentativeBase representative, int goldAmount);

		// Token: 0x06002410 RID: 9232 RVA: 0x00085257 File Offset: 0x00083457
		public virtual bool CanRequestTroopChange()
		{
			return false;
		}

		// Token: 0x06002411 RID: 9233 RVA: 0x0008525A File Offset: 0x0008345A
		public virtual bool CanRequestCultureChange()
		{
			return false;
		}
	}
}
