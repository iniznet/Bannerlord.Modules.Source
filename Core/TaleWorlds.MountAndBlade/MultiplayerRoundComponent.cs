using System;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002A9 RID: 681
	public class MultiplayerRoundComponent : MissionNetwork, IRoundComponent, IMissionBehavior
	{
		// Token: 0x1400005A RID: 90
		// (add) Token: 0x06002593 RID: 9619 RVA: 0x0008E6E0 File Offset: 0x0008C8E0
		// (remove) Token: 0x06002594 RID: 9620 RVA: 0x0008E718 File Offset: 0x0008C918
		public event Action OnRoundStarted;

		// Token: 0x1400005B RID: 91
		// (add) Token: 0x06002595 RID: 9621 RVA: 0x0008E750 File Offset: 0x0008C950
		// (remove) Token: 0x06002596 RID: 9622 RVA: 0x0008E788 File Offset: 0x0008C988
		public event Action OnPreparationEnded;

		// Token: 0x1400005C RID: 92
		// (add) Token: 0x06002597 RID: 9623 RVA: 0x0008E7C0 File Offset: 0x0008C9C0
		// (remove) Token: 0x06002598 RID: 9624 RVA: 0x0008E7F8 File Offset: 0x0008C9F8
		public event Action OnPreRoundEnding;

		// Token: 0x1400005D RID: 93
		// (add) Token: 0x06002599 RID: 9625 RVA: 0x0008E830 File Offset: 0x0008CA30
		// (remove) Token: 0x0600259A RID: 9626 RVA: 0x0008E868 File Offset: 0x0008CA68
		public event Action OnRoundEnding;

		// Token: 0x1400005E RID: 94
		// (add) Token: 0x0600259B RID: 9627 RVA: 0x0008E8A0 File Offset: 0x0008CAA0
		// (remove) Token: 0x0600259C RID: 9628 RVA: 0x0008E8D8 File Offset: 0x0008CAD8
		public event Action OnPostRoundEnded;

		// Token: 0x1400005F RID: 95
		// (add) Token: 0x0600259D RID: 9629 RVA: 0x0008E910 File Offset: 0x0008CB10
		// (remove) Token: 0x0600259E RID: 9630 RVA: 0x0008E948 File Offset: 0x0008CB48
		public event Action OnCurrentRoundStateChanged;

		// Token: 0x170006E5 RID: 1765
		// (get) Token: 0x0600259F RID: 9631 RVA: 0x0008E97D File Offset: 0x0008CB7D
		public float RemainingRoundTime
		{
			get
			{
				return this._gameModeClient.TimerComponent.GetRemainingTime(true);
			}
		}

		// Token: 0x170006E6 RID: 1766
		// (get) Token: 0x060025A0 RID: 9632 RVA: 0x0008E990 File Offset: 0x0008CB90
		// (set) Token: 0x060025A1 RID: 9633 RVA: 0x0008E998 File Offset: 0x0008CB98
		public float LastRoundEndRemainingTime { get; private set; }

		// Token: 0x170006E7 RID: 1767
		// (get) Token: 0x060025A2 RID: 9634 RVA: 0x0008E9A1 File Offset: 0x0008CBA1
		// (set) Token: 0x060025A3 RID: 9635 RVA: 0x0008E9A9 File Offset: 0x0008CBA9
		public MultiplayerRoundState CurrentRoundState { get; private set; }

		// Token: 0x170006E8 RID: 1768
		// (get) Token: 0x060025A4 RID: 9636 RVA: 0x0008E9B2 File Offset: 0x0008CBB2
		// (set) Token: 0x060025A5 RID: 9637 RVA: 0x0008E9BA File Offset: 0x0008CBBA
		public int RoundCount { get; private set; }

		// Token: 0x170006E9 RID: 1769
		// (get) Token: 0x060025A6 RID: 9638 RVA: 0x0008E9C3 File Offset: 0x0008CBC3
		// (set) Token: 0x060025A7 RID: 9639 RVA: 0x0008E9CB File Offset: 0x0008CBCB
		public BattleSideEnum RoundWinner { get; private set; }

		// Token: 0x170006EA RID: 1770
		// (get) Token: 0x060025A8 RID: 9640 RVA: 0x0008E9D4 File Offset: 0x0008CBD4
		// (set) Token: 0x060025A9 RID: 9641 RVA: 0x0008E9DC File Offset: 0x0008CBDC
		public RoundEndReason RoundEndReason { get; private set; }

		// Token: 0x060025AA RID: 9642 RVA: 0x0008E9E5 File Offset: 0x0008CBE5
		public override void AfterStart()
		{
			this.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add);
			this._gameModeClient = Mission.Current.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>();
		}

		// Token: 0x060025AB RID: 9643 RVA: 0x0008E9FE File Offset: 0x0008CBFE
		protected override void OnUdpNetworkHandlerClose()
		{
			this.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Remove);
		}

		// Token: 0x060025AC RID: 9644 RVA: 0x0008EA08 File Offset: 0x0008CC08
		private void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode mode)
		{
			GameNetwork.NetworkMessageHandlerRegisterer networkMessageHandlerRegisterer = new GameNetwork.NetworkMessageHandlerRegisterer(mode);
			if (GameNetwork.IsClient)
			{
				networkMessageHandlerRegisterer.Register<RoundStateChange>(new GameNetworkMessage.ServerMessageHandlerDelegate<RoundStateChange>(this.HandleServerEventChangeRoundState));
				networkMessageHandlerRegisterer.Register<RoundCountChange>(new GameNetworkMessage.ServerMessageHandlerDelegate<RoundCountChange>(this.HandleServerEventRoundCountChange));
				networkMessageHandlerRegisterer.Register<RoundWinnerChange>(new GameNetworkMessage.ServerMessageHandlerDelegate<RoundWinnerChange>(this.HandleServerEventRoundWinnerChange));
				networkMessageHandlerRegisterer.Register<RoundEndReasonChange>(new GameNetworkMessage.ServerMessageHandlerDelegate<RoundEndReasonChange>(this.HandleServerEventRoundEndReasonChange));
			}
		}

		// Token: 0x060025AD RID: 9645 RVA: 0x0008EA6C File Offset: 0x0008CC6C
		private void HandleServerEventChangeRoundState(RoundStateChange message)
		{
			if (this.CurrentRoundState == MultiplayerRoundState.InProgress)
			{
				this.LastRoundEndRemainingTime = (float)message.RemainingTimeOnPreviousState;
			}
			this.CurrentRoundState = message.RoundState;
			switch (this.CurrentRoundState)
			{
			case MultiplayerRoundState.Preparation:
				this._gameModeClient.TimerComponent.StartTimerAsClient(message.StateStartTimeInSeconds, (float)MultiplayerOptions.OptionType.RoundPreparationTimeLimit.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
				if (this.OnRoundStarted != null)
				{
					this.OnRoundStarted();
				}
				break;
			case MultiplayerRoundState.InProgress:
				this._gameModeClient.TimerComponent.StartTimerAsClient(message.StateStartTimeInSeconds, (float)MultiplayerOptions.OptionType.RoundTimeLimit.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
				if (this.OnPreparationEnded != null)
				{
					this.OnPreparationEnded();
				}
				break;
			case MultiplayerRoundState.Ending:
				this._gameModeClient.TimerComponent.StartTimerAsClient(message.StateStartTimeInSeconds, 3f);
				if (this.OnPreRoundEnding != null)
				{
					this.OnPreRoundEnding();
				}
				if (this.OnRoundEnding != null)
				{
					this.OnRoundEnding();
				}
				break;
			case MultiplayerRoundState.Ended:
				this._gameModeClient.TimerComponent.StartTimerAsClient(message.StateStartTimeInSeconds, 5f);
				if (this.OnPostRoundEnded != null)
				{
					this.OnPostRoundEnded();
				}
				break;
			case MultiplayerRoundState.MatchEnded:
				this._gameModeClient.TimerComponent.StartTimerAsClient(message.StateStartTimeInSeconds, 5f);
				break;
			}
			Action onCurrentRoundStateChanged = this.OnCurrentRoundStateChanged;
			if (onCurrentRoundStateChanged == null)
			{
				return;
			}
			onCurrentRoundStateChanged();
		}

		// Token: 0x060025AE RID: 9646 RVA: 0x0008EBD5 File Offset: 0x0008CDD5
		private void HandleServerEventRoundCountChange(RoundCountChange message)
		{
			this.RoundCount = message.RoundCount;
		}

		// Token: 0x060025AF RID: 9647 RVA: 0x0008EBE3 File Offset: 0x0008CDE3
		private void HandleServerEventRoundWinnerChange(RoundWinnerChange message)
		{
			this.RoundWinner = message.RoundWinner;
		}

		// Token: 0x060025B0 RID: 9648 RVA: 0x0008EBF1 File Offset: 0x0008CDF1
		private void HandleServerEventRoundEndReasonChange(RoundEndReasonChange message)
		{
			this.RoundEndReason = message.RoundEndReason;
		}

		// Token: 0x04000DDA RID: 3546
		public const int RoundEndDelayTime = 3;

		// Token: 0x04000DDB RID: 3547
		public const int RoundEndWaitTime = 8;

		// Token: 0x04000DDC RID: 3548
		public const int MatchEndWaitTime = 5;

		// Token: 0x04000DDD RID: 3549
		public const int WarmupEndWaitTime = 30;

		// Token: 0x04000DE4 RID: 3556
		private MissionMultiplayerGameModeBaseClient _gameModeClient;
	}
}
