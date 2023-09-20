using System;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.MissionRepresentatives;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200029D RID: 669
	public class MissionMultiplayerTeamDeathmatchClient : MissionMultiplayerGameModeBaseClient
	{
		// Token: 0x14000053 RID: 83
		// (add) Token: 0x0600247D RID: 9341 RVA: 0x000870D4 File Offset: 0x000852D4
		// (remove) Token: 0x0600247E RID: 9342 RVA: 0x0008710C File Offset: 0x0008530C
		public event Action<GoldGain> OnGoldGainEvent;

		// Token: 0x170006CD RID: 1741
		// (get) Token: 0x0600247F RID: 9343 RVA: 0x00087141 File Offset: 0x00085341
		public override bool IsGameModeUsingGold
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170006CE RID: 1742
		// (get) Token: 0x06002480 RID: 9344 RVA: 0x00087144 File Offset: 0x00085344
		public override bool IsGameModeTactical
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170006CF RID: 1743
		// (get) Token: 0x06002481 RID: 9345 RVA: 0x00087147 File Offset: 0x00085347
		public override bool IsGameModeUsingRoundCountdown
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170006D0 RID: 1744
		// (get) Token: 0x06002482 RID: 9346 RVA: 0x0008714A File Offset: 0x0008534A
		public override MissionLobbyComponent.MultiplayerGameType GameType
		{
			get
			{
				return MissionLobbyComponent.MultiplayerGameType.TeamDeathmatch;
			}
		}

		// Token: 0x06002483 RID: 9347 RVA: 0x0008714D File Offset: 0x0008534D
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			base.MissionNetworkComponent.OnMyClientSynchronized += this.OnMyClientSynchronized;
			base.ScoreboardComponent.OnRoundPropertiesChanged += this.OnTeamScoresChanged;
		}

		// Token: 0x06002484 RID: 9348 RVA: 0x00087183 File Offset: 0x00085383
		public override void OnGoldAmountChangedForRepresentative(MissionRepresentativeBase representative, int goldAmount)
		{
			if (representative != null && base.MissionLobbyComponent.CurrentMultiplayerState != MissionLobbyComponent.MultiplayerGameState.Ending)
			{
				representative.UpdateGold(goldAmount);
				base.ScoreboardComponent.PlayerPropertiesChanged(representative.MissionPeer);
			}
		}

		// Token: 0x06002485 RID: 9349 RVA: 0x000871AE File Offset: 0x000853AE
		public override void AfterStart()
		{
			base.Mission.SetMissionMode(MissionMode.Battle, true);
		}

		// Token: 0x06002486 RID: 9350 RVA: 0x000871BD File Offset: 0x000853BD
		protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
		{
			if (GameNetwork.IsClient)
			{
				registerer.Register<SyncGoldsForSkirmish>(new GameNetworkMessage.ServerMessageHandlerDelegate<SyncGoldsForSkirmish>(this.HandleServerEventUpdateGold));
				registerer.Register<GoldGain>(new GameNetworkMessage.ServerMessageHandlerDelegate<GoldGain>(this.HandleServerEventTDMGoldGain));
			}
		}

		// Token: 0x06002487 RID: 9351 RVA: 0x000871EA File Offset: 0x000853EA
		private void OnMyClientSynchronized()
		{
			this._myRepresentative = GameNetwork.MyPeer.GetComponent<TeamDeathmatchMissionRepresentative>();
		}

		// Token: 0x06002488 RID: 9352 RVA: 0x000871FC File Offset: 0x000853FC
		private void HandleServerEventUpdateGold(SyncGoldsForSkirmish message)
		{
			MissionRepresentativeBase component = message.VirtualPlayer.GetComponent<MissionRepresentativeBase>();
			this.OnGoldAmountChangedForRepresentative(component, message.GoldAmount);
		}

		// Token: 0x06002489 RID: 9353 RVA: 0x00087222 File Offset: 0x00085422
		private void HandleServerEventTDMGoldGain(GoldGain message)
		{
			Action<GoldGain> onGoldGainEvent = this.OnGoldGainEvent;
			if (onGoldGainEvent == null)
			{
				return;
			}
			onGoldGainEvent(message);
		}

		// Token: 0x0600248A RID: 9354 RVA: 0x00087235 File Offset: 0x00085435
		public override int GetGoldAmount()
		{
			return this._myRepresentative.Gold;
		}

		// Token: 0x0600248B RID: 9355 RVA: 0x00087242 File Offset: 0x00085442
		public override void OnRemoveBehavior()
		{
			base.MissionNetworkComponent.OnMyClientSynchronized -= this.OnMyClientSynchronized;
			base.ScoreboardComponent.OnRoundPropertiesChanged -= this.OnTeamScoresChanged;
			base.OnRemoveBehavior();
		}

		// Token: 0x0600248C RID: 9356 RVA: 0x00087278 File Offset: 0x00085478
		private void OnTeamScoresChanged()
		{
			if (!GameNetwork.IsDedicatedServer && !this._battleEndingNotificationGiven && this._myRepresentative.MissionPeer.Team != null && this._myRepresentative.MissionPeer.Team.Side != BattleSideEnum.None)
			{
				int intValue = MultiplayerOptions.OptionType.MinScoreToWinMatch.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
				float num = (float)(intValue - base.ScoreboardComponent.GetRoundScore(this._myRepresentative.MissionPeer.Team.Side)) / (float)intValue;
				float num2 = (float)(intValue - base.ScoreboardComponent.GetRoundScore(this._myRepresentative.MissionPeer.Team.Side.GetOppositeSide())) / (float)intValue;
				MatrixFrame cameraFrame = Mission.Current.GetCameraFrame();
				Vec3 vec = cameraFrame.origin + cameraFrame.rotation.u;
				if (num <= 0.1f && num2 > 0.1f)
				{
					MBSoundEvent.PlaySound(SoundEvent.GetEventIdFromString("event:/alerts/report/battle_winning"), vec);
					this._battleEndingNotificationGiven = true;
				}
				if (num2 <= 0.1f && num > 0.1f)
				{
					MBSoundEvent.PlaySound(SoundEvent.GetEventIdFromString("event:/alerts/report/battle_losing"), vec);
					this._battleEndingNotificationGiven = true;
				}
			}
		}

		// Token: 0x04000D54 RID: 3412
		private const string BattleWinningSoundEventString = "event:/alerts/report/battle_winning";

		// Token: 0x04000D55 RID: 3413
		private const string BattleLosingSoundEventString = "event:/alerts/report/battle_losing";

		// Token: 0x04000D56 RID: 3414
		private const float BattleWinLoseAlertThreshold = 0.1f;

		// Token: 0x04000D58 RID: 3416
		private TeamDeathmatchMissionRepresentative _myRepresentative;

		// Token: 0x04000D59 RID: 3417
		private bool _battleEndingNotificationGiven;
	}
}
