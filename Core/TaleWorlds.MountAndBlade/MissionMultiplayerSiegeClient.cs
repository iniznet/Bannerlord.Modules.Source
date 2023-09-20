using System;
using System.Collections.Generic;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.MissionRepresentatives;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.MountAndBlade.Objects;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200029C RID: 668
	public class MissionMultiplayerSiegeClient : MissionMultiplayerGameModeBaseClient, ICommanderInfo, IMissionBehavior
	{
		// Token: 0x170006C7 RID: 1735
		// (get) Token: 0x06002459 RID: 9305 RVA: 0x0008656A File Offset: 0x0008476A
		public override bool IsGameModeUsingGold
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170006C8 RID: 1736
		// (get) Token: 0x0600245A RID: 9306 RVA: 0x0008656D File Offset: 0x0008476D
		public override bool IsGameModeTactical
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170006C9 RID: 1737
		// (get) Token: 0x0600245B RID: 9307 RVA: 0x00086570 File Offset: 0x00084770
		public override bool IsGameModeUsingRoundCountdown
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170006CA RID: 1738
		// (get) Token: 0x0600245C RID: 9308 RVA: 0x00086573 File Offset: 0x00084773
		public override MissionLobbyComponent.MultiplayerGameType GameType
		{
			get
			{
				return MissionLobbyComponent.MultiplayerGameType.Siege;
			}
		}

		// Token: 0x1400004E RID: 78
		// (add) Token: 0x0600245D RID: 9309 RVA: 0x00086578 File Offset: 0x00084778
		// (remove) Token: 0x0600245E RID: 9310 RVA: 0x000865B0 File Offset: 0x000847B0
		public event Action<BattleSideEnum, float> OnMoraleChangedEvent;

		// Token: 0x1400004F RID: 79
		// (add) Token: 0x0600245F RID: 9311 RVA: 0x000865E8 File Offset: 0x000847E8
		// (remove) Token: 0x06002460 RID: 9312 RVA: 0x00086620 File Offset: 0x00084820
		public event Action OnFlagNumberChangedEvent;

		// Token: 0x14000050 RID: 80
		// (add) Token: 0x06002461 RID: 9313 RVA: 0x00086658 File Offset: 0x00084858
		// (remove) Token: 0x06002462 RID: 9314 RVA: 0x00086690 File Offset: 0x00084890
		public event Action<FlagCapturePoint, Team> OnCapturePointOwnerChangedEvent;

		// Token: 0x14000051 RID: 81
		// (add) Token: 0x06002463 RID: 9315 RVA: 0x000866C8 File Offset: 0x000848C8
		// (remove) Token: 0x06002464 RID: 9316 RVA: 0x00086700 File Offset: 0x00084900
		public event Action<GoldGain> OnGoldGainEvent;

		// Token: 0x14000052 RID: 82
		// (add) Token: 0x06002465 RID: 9317 RVA: 0x00086738 File Offset: 0x00084938
		// (remove) Token: 0x06002466 RID: 9318 RVA: 0x00086770 File Offset: 0x00084970
		public event Action<int[]> OnCapturePointRemainingMoraleGainsChangedEvent;

		// Token: 0x170006CB RID: 1739
		// (get) Token: 0x06002467 RID: 9319 RVA: 0x000867A5 File Offset: 0x000849A5
		public bool AreMoralesIndependent
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170006CC RID: 1740
		// (get) Token: 0x06002468 RID: 9320 RVA: 0x000867A8 File Offset: 0x000849A8
		// (set) Token: 0x06002469 RID: 9321 RVA: 0x000867B0 File Offset: 0x000849B0
		public IEnumerable<FlagCapturePoint> AllCapturePoints { get; private set; }

		// Token: 0x0600246A RID: 9322 RVA: 0x000867BC File Offset: 0x000849BC
		protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
		{
			if (GameNetwork.IsClient)
			{
				registerer.Register<SiegeMoraleChangeMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<SiegeMoraleChangeMessage>(this.HandleMoraleChangedMessage));
				registerer.Register<SyncGoldsForSkirmish>(new GameNetworkMessage.ServerMessageHandlerDelegate<SyncGoldsForSkirmish>(this.HandleServerEventUpdateGold));
				registerer.Register<FlagDominationFlagsRemovedMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<FlagDominationFlagsRemovedMessage>(this.HandleFlagsRemovedMessage));
				registerer.Register<FlagDominationCapturePointMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<FlagDominationCapturePointMessage>(this.HandleServerEventPointCapturedMessage));
				registerer.Register<GoldGain>(new GameNetworkMessage.ServerMessageHandlerDelegate<GoldGain>(this.HandleServerEventTDMGoldGain));
			}
		}

		// Token: 0x0600246B RID: 9323 RVA: 0x0008682A File Offset: 0x00084A2A
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			base.MissionNetworkComponent.OnMyClientSynchronized += this.OnMyClientSynchronized;
			this._capturePointOwners = new Team[7];
			this.AllCapturePoints = Mission.Current.MissionObjects.FindAllWithType<FlagCapturePoint>();
		}

		// Token: 0x0600246C RID: 9324 RVA: 0x0008686C File Offset: 0x00084A6C
		public override void AfterStart()
		{
			base.Mission.SetMissionMode(MissionMode.Battle, true);
			foreach (FlagCapturePoint flagCapturePoint in this.AllCapturePoints)
			{
				if (flagCapturePoint.GameEntity.HasTag("keep_capture_point"))
				{
					this._masterFlag = flagCapturePoint;
				}
				else if (flagCapturePoint.FlagIndex == 0)
				{
					MatrixFrame globalFrame = flagCapturePoint.GameEntity.GetGlobalFrame();
					this._retreatHornPosition = globalFrame.origin + globalFrame.rotation.u * 3f;
				}
			}
		}

		// Token: 0x0600246D RID: 9325 RVA: 0x00086914 File Offset: 0x00084B14
		private void OnMyClientSynchronized()
		{
			this._myRepresentative = GameNetwork.MyPeer.GetComponent<SiegeMissionRepresentative>();
		}

		// Token: 0x0600246E RID: 9326 RVA: 0x00086926 File Offset: 0x00084B26
		public override int GetGoldAmount()
		{
			return this._myRepresentative.Gold;
		}

		// Token: 0x0600246F RID: 9327 RVA: 0x00086933 File Offset: 0x00084B33
		public override void OnGoldAmountChangedForRepresentative(MissionRepresentativeBase representative, int goldAmount)
		{
			if (representative != null && base.MissionLobbyComponent.CurrentMultiplayerState != MissionLobbyComponent.MultiplayerGameState.Ending)
			{
				representative.UpdateGold(goldAmount);
				base.ScoreboardComponent.PlayerPropertiesChanged(representative.MissionPeer);
			}
		}

		// Token: 0x06002470 RID: 9328 RVA: 0x00086960 File Offset: 0x00084B60
		public void OnNumberOfFlagsChanged()
		{
			Action onFlagNumberChangedEvent = this.OnFlagNumberChangedEvent;
			if (onFlagNumberChangedEvent != null)
			{
				onFlagNumberChangedEvent();
			}
			SiegeMissionRepresentative myRepresentative = this._myRepresentative;
			bool flag;
			if (myRepresentative == null)
			{
				flag = false;
			}
			else
			{
				Team team = myRepresentative.MissionPeer.Team;
				BattleSideEnum? battleSideEnum = ((team != null) ? new BattleSideEnum?(team.Side) : null);
				BattleSideEnum battleSideEnum2 = BattleSideEnum.Attacker;
				flag = (battleSideEnum.GetValueOrDefault() == battleSideEnum2) & (battleSideEnum != null);
			}
			if (flag)
			{
				Action<GoldGain> onGoldGainEvent = this.OnGoldGainEvent;
				if (onGoldGainEvent == null)
				{
					return;
				}
				onGoldGainEvent(new GoldGain(new List<KeyValuePair<ushort, int>>
				{
					new KeyValuePair<ushort, int>(512, 35)
				}));
			}
		}

		// Token: 0x06002471 RID: 9329 RVA: 0x000869F4 File Offset: 0x00084BF4
		public void OnCapturePointOwnerChanged(FlagCapturePoint flagCapturePoint, Team ownerTeam)
		{
			this._capturePointOwners[flagCapturePoint.FlagIndex] = ownerTeam;
			Action<FlagCapturePoint, Team> onCapturePointOwnerChangedEvent = this.OnCapturePointOwnerChangedEvent;
			if (onCapturePointOwnerChangedEvent != null)
			{
				onCapturePointOwnerChangedEvent(flagCapturePoint, ownerTeam);
			}
			if (ownerTeam != null && ownerTeam.Side == BattleSideEnum.Defender && this._remainingTimeForBellSoundToStop > 8f && flagCapturePoint == this._masterFlag)
			{
				this._bellSoundEvent.Stop();
				this._bellSoundEvent = null;
				this._remainingTimeForBellSoundToStop = float.MinValue;
				this._lastBellSoundPercentage += 0.2f;
			}
			if (this._myRepresentative != null && this._myRepresentative.MissionPeer.Team != null)
			{
				MatrixFrame cameraFrame = Mission.Current.GetCameraFrame();
				Vec3 vec = cameraFrame.origin + cameraFrame.rotation.u;
				if (this._myRepresentative.MissionPeer.Team == ownerTeam)
				{
					MBSoundEvent.PlaySound(SoundEvent.GetEventIdFromString("event:/alerts/report/flag_captured"), vec);
					return;
				}
				MBSoundEvent.PlaySound(SoundEvent.GetEventIdFromString("event:/alerts/report/flag_lost"), vec);
			}
		}

		// Token: 0x06002472 RID: 9330 RVA: 0x00086AE4 File Offset: 0x00084CE4
		public void OnMoraleChanged(int attackerMorale, int defenderMorale, int[] capturePointRemainingMoraleGains)
		{
			float num = (float)attackerMorale / 360f;
			float num2 = (float)defenderMorale / 360f;
			SiegeMissionRepresentative myRepresentative = this._myRepresentative;
			if (((myRepresentative != null) ? myRepresentative.MissionPeer.Team : null) != null && this._myRepresentative.MissionPeer.Team.Side != BattleSideEnum.None)
			{
				if ((this._capturePointOwners[this._masterFlag.FlagIndex] == null || this._capturePointOwners[this._masterFlag.FlagIndex].Side != BattleSideEnum.Defender) && this._remainingTimeForBellSoundToStop < 0f)
				{
					if (num2 > this._lastBellSoundPercentage)
					{
						this._lastBellSoundPercentage += 0.2f;
					}
					if (num2 <= 0.4f)
					{
						if (this._lastBellSoundPercentage > 0.4f)
						{
							this._remainingTimeForBellSoundToStop = float.MaxValue;
							this._lastBellSoundPercentage = 0.4f;
						}
					}
					else if (num2 <= 0.6f)
					{
						if (this._lastBellSoundPercentage > 0.6f)
						{
							this._remainingTimeForBellSoundToStop = 8f;
							this._lastBellSoundPercentage = 0.6f;
						}
					}
					else if (num2 <= 0.8f && this._lastBellSoundPercentage > 0.8f)
					{
						this._remainingTimeForBellSoundToStop = 4f;
						this._lastBellSoundPercentage = 0.8f;
					}
					if (this._remainingTimeForBellSoundToStop > 0f)
					{
						BattleSideEnum side = this._myRepresentative.MissionPeer.Team.Side;
						if (side != BattleSideEnum.Defender)
						{
							if (side == BattleSideEnum.Attacker)
							{
								this._bellSoundEvent = SoundEvent.CreateEventFromString("event:/multiplayer/warning_bells_attacker", base.Mission.Scene);
							}
						}
						else
						{
							this._bellSoundEvent = SoundEvent.CreateEventFromString("event:/multiplayer/warning_bells_defender", base.Mission.Scene);
						}
						MatrixFrame globalFrame = this._masterFlag.GameEntity.GetGlobalFrame();
						this._bellSoundEvent.PlayInPosition(globalFrame.origin + globalFrame.rotation.u * 3f);
					}
				}
				if (!this._battleEndingNotificationGiven || !this._battleEndingLateNotificationGiven)
				{
					float num3 = ((!this._battleEndingNotificationGiven) ? 0.25f : 0.15f);
					MatrixFrame cameraFrame = Mission.Current.GetCameraFrame();
					Vec3 vec = cameraFrame.origin + cameraFrame.rotation.u;
					if (num <= num3 && num2 > num3)
					{
						MBSoundEvent.PlaySound(SoundEvent.GetEventIdFromString((this._myRepresentative.MissionPeer.Team.Side == BattleSideEnum.Attacker) ? "event:/alerts/report/battle_losing" : "event:/alerts/report/battle_winning"), vec);
						if (this._myRepresentative.MissionPeer.Team.Side == BattleSideEnum.Attacker)
						{
							MBSoundEvent.PlaySound(SoundEvent.GetEventIdFromString("event:/multiplayer/retreat_horn_attacker"), this._retreatHornPosition);
						}
						else if (this._myRepresentative.MissionPeer.Team.Side == BattleSideEnum.Defender)
						{
							MBSoundEvent.PlaySound(SoundEvent.GetEventIdFromString("event:/multiplayer/retreat_horn_defender"), this._retreatHornPosition);
						}
						if (this._battleEndingNotificationGiven)
						{
							this._battleEndingLateNotificationGiven = true;
						}
						this._battleEndingNotificationGiven = true;
					}
					if (num2 <= num3 && num > num3)
					{
						MBSoundEvent.PlaySound(SoundEvent.GetEventIdFromString((this._myRepresentative.MissionPeer.Team.Side == BattleSideEnum.Defender) ? "event:/alerts/report/battle_losing" : "event:/alerts/report/battle_winning"), vec);
						if (this._battleEndingNotificationGiven)
						{
							this._battleEndingLateNotificationGiven = true;
						}
						this._battleEndingNotificationGiven = true;
					}
				}
			}
			Action<BattleSideEnum, float> onMoraleChangedEvent = this.OnMoraleChangedEvent;
			if (onMoraleChangedEvent != null)
			{
				onMoraleChangedEvent(BattleSideEnum.Attacker, num);
			}
			Action<BattleSideEnum, float> onMoraleChangedEvent2 = this.OnMoraleChangedEvent;
			if (onMoraleChangedEvent2 != null)
			{
				onMoraleChangedEvent2(BattleSideEnum.Defender, num2);
			}
			Action<int[]> onCapturePointRemainingMoraleGainsChangedEvent = this.OnCapturePointRemainingMoraleGainsChangedEvent;
			if (onCapturePointRemainingMoraleGainsChangedEvent == null)
			{
				return;
			}
			onCapturePointRemainingMoraleGainsChangedEvent(capturePointRemainingMoraleGains);
		}

		// Token: 0x06002473 RID: 9331 RVA: 0x00086E48 File Offset: 0x00085048
		public Team GetFlagOwner(FlagCapturePoint flag)
		{
			return this._capturePointOwners[flag.FlagIndex];
		}

		// Token: 0x06002474 RID: 9332 RVA: 0x00086E57 File Offset: 0x00085057
		public override void OnRemoveBehavior()
		{
			base.MissionNetworkComponent.OnMyClientSynchronized -= this.OnMyClientSynchronized;
			base.OnRemoveBehavior();
		}

		// Token: 0x06002475 RID: 9333 RVA: 0x00086E78 File Offset: 0x00085078
		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			if (this._remainingTimeForBellSoundToStop > 0f)
			{
				this._remainingTimeForBellSoundToStop -= dt;
				if (this._remainingTimeForBellSoundToStop <= 0f || base.MissionLobbyComponent.CurrentMultiplayerState != MissionLobbyComponent.MultiplayerGameState.Playing)
				{
					this._remainingTimeForBellSoundToStop = float.MinValue;
					this._bellSoundEvent.Stop();
					this._bellSoundEvent = null;
				}
			}
		}

		// Token: 0x06002476 RID: 9334 RVA: 0x00086EE0 File Offset: 0x000850E0
		public List<ItemObject> GetSiegeMissiles()
		{
			List<ItemObject> list = new List<ItemObject>();
			ItemObject @object = MBObjectManager.Instance.GetObject<ItemObject>("grapeshot_fire_projectile");
			list.Add(@object);
			foreach (GameEntity gameEntity in Mission.Current.GetActiveEntitiesWithScriptComponentOfType<RangedSiegeWeapon>())
			{
				RangedSiegeWeapon firstScriptOfType = gameEntity.GetFirstScriptOfType<RangedSiegeWeapon>();
				if (!string.IsNullOrEmpty(firstScriptOfType.MissileItemID))
				{
					ItemObject object2 = MBObjectManager.Instance.GetObject<ItemObject>(firstScriptOfType.MissileItemID);
					if (!list.Contains(object2))
					{
						list.Add(object2);
					}
				}
			}
			foreach (GameEntity gameEntity2 in Mission.Current.GetActiveEntitiesWithScriptComponentOfType<StonePile>())
			{
				StonePile firstScriptOfType2 = gameEntity2.GetFirstScriptOfType<StonePile>();
				if (!string.IsNullOrEmpty(firstScriptOfType2.GivenItemID))
				{
					ItemObject object3 = MBObjectManager.Instance.GetObject<ItemObject>(firstScriptOfType2.GivenItemID);
					if (!list.Contains(object3))
					{
						list.Add(object3);
					}
				}
			}
			return list;
		}

		// Token: 0x06002477 RID: 9335 RVA: 0x00086FF0 File Offset: 0x000851F0
		private void HandleMoraleChangedMessage(SiegeMoraleChangeMessage message)
		{
			this.OnMoraleChanged(message.AttackerMorale, message.DefenderMorale, message.CapturePointRemainingMoraleGains);
		}

		// Token: 0x06002478 RID: 9336 RVA: 0x0008700C File Offset: 0x0008520C
		private void HandleServerEventUpdateGold(SyncGoldsForSkirmish message)
		{
			SiegeMissionRepresentative component = message.VirtualPlayer.GetComponent<SiegeMissionRepresentative>();
			this.OnGoldAmountChangedForRepresentative(component, message.GoldAmount);
		}

		// Token: 0x06002479 RID: 9337 RVA: 0x00087032 File Offset: 0x00085232
		private void HandleFlagsRemovedMessage(FlagDominationFlagsRemovedMessage message)
		{
			this.OnNumberOfFlagsChanged();
		}

		// Token: 0x0600247A RID: 9338 RVA: 0x0008703C File Offset: 0x0008523C
		private void HandleServerEventPointCapturedMessage(FlagDominationCapturePointMessage message)
		{
			foreach (FlagCapturePoint flagCapturePoint in this.AllCapturePoints)
			{
				if (flagCapturePoint.FlagIndex == message.FlagIndex)
				{
					this.OnCapturePointOwnerChanged(flagCapturePoint, message.OwnerTeam);
					break;
				}
			}
		}

		// Token: 0x0600247B RID: 9339 RVA: 0x000870A0 File Offset: 0x000852A0
		private void HandleServerEventTDMGoldGain(GoldGain message)
		{
			Action<GoldGain> onGoldGainEvent = this.OnGoldGainEvent;
			if (onGoldGainEvent == null)
			{
				return;
			}
			onGoldGainEvent(message);
		}

		// Token: 0x04000D3F RID: 3391
		private const float DefenderMoraleDropThresholdIncrement = 0.2f;

		// Token: 0x04000D40 RID: 3392
		private const float DefenderMoraleDropThresholdLow = 0.4f;

		// Token: 0x04000D41 RID: 3393
		private const float DefenderMoraleDropThresholdMedium = 0.6f;

		// Token: 0x04000D42 RID: 3394
		private const float DefenderMoraleDropThresholdHigh = 0.8f;

		// Token: 0x04000D43 RID: 3395
		private const float DefenderMoraleDropMediumDuration = 8f;

		// Token: 0x04000D44 RID: 3396
		private const float DefenderMoraleDropHighDuration = 4f;

		// Token: 0x04000D45 RID: 3397
		private const float BattleWinLoseAlertThreshold = 0.25f;

		// Token: 0x04000D46 RID: 3398
		private const float BattleWinLoseLateAlertThreshold = 0.15f;

		// Token: 0x04000D47 RID: 3399
		private const string BattleWinningSoundEventString = "event:/alerts/report/battle_winning";

		// Token: 0x04000D48 RID: 3400
		private const string BattleLosingSoundEventString = "event:/alerts/report/battle_losing";

		// Token: 0x04000D49 RID: 3401
		private const float IndefiniteDurationThreshold = 8f;

		// Token: 0x04000D4A RID: 3402
		private Team[] _capturePointOwners;

		// Token: 0x04000D4C RID: 3404
		private FlagCapturePoint _masterFlag;

		// Token: 0x04000D4D RID: 3405
		private SiegeMissionRepresentative _myRepresentative;

		// Token: 0x04000D4E RID: 3406
		private SoundEvent _bellSoundEvent;

		// Token: 0x04000D4F RID: 3407
		private float _remainingTimeForBellSoundToStop = float.MinValue;

		// Token: 0x04000D50 RID: 3408
		private float _lastBellSoundPercentage = 1f;

		// Token: 0x04000D51 RID: 3409
		private bool _battleEndingNotificationGiven;

		// Token: 0x04000D52 RID: 3410
		private bool _battleEndingLateNotificationGiven;

		// Token: 0x04000D53 RID: 3411
		private Vec3 _retreatHornPosition;
	}
}
