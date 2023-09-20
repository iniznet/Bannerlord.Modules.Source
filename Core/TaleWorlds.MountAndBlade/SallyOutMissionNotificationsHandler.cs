using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200027E RID: 638
	public class SallyOutMissionNotificationsHandler
	{
		// Token: 0x060021ED RID: 8685 RVA: 0x0007BFC8 File Offset: 0x0007A1C8
		public SallyOutMissionNotificationsHandler(MissionAgentSpawnLogic spawnLogic, SallyOutMissionController sallyOutController)
		{
			this._spawnLogic = spawnLogic;
			this._sallyOutController = sallyOutController;
			this._spawnLogic.OnReinforcementsSpawned += this.OnReinforcementsSpawned;
			this._spawnLogic.OnInitialTroopsSpawned += this.OnInitialTroopsSpawned;
			this._besiegerSpawnedTroopCount = 0;
			this._notificationTimer = new BasicMissionTimer();
			this._notificationsQueue = new Queue<SallyOutMissionNotificationsHandler.NotificationType>();
		}

		// Token: 0x060021EE RID: 8686 RVA: 0x0007C03C File Offset: 0x0007A23C
		public void OnBesiegedSideFallsbackToKeep()
		{
			if (this._isPlayerBesieged)
			{
				if (Mission.Current.PlayerTeam.FormationsIncludingEmpty.Any((Formation f) => f.IsAIControlled && f.CountOfUnits > 0))
				{
					this._notificationsQueue.Enqueue(SallyOutMissionNotificationsHandler.NotificationType.BesiegedSideTacticalRetreat);
					if (Mission.Current.MainAgent != null && Mission.Current.MainAgent.IsActive())
					{
						this._notificationsQueue.Enqueue(SallyOutMissionNotificationsHandler.NotificationType.BesiegedSidePlayerPullbackRequest);
						return;
					}
				}
			}
			else
			{
				this._notificationsQueue.Enqueue(SallyOutMissionNotificationsHandler.NotificationType.BesiegedSideTacticalRetreat);
			}
		}

		// Token: 0x060021EF RID: 8687 RVA: 0x0007C0C8 File Offset: 0x0007A2C8
		public void OnAfterStart()
		{
			this._isPlayerBesieged = Mission.Current.PlayerTeam.Side == BattleSideEnum.Defender;
			this.SetNotificationTimerEnabled(false, true);
		}

		// Token: 0x060021F0 RID: 8688 RVA: 0x0007C0EA File Offset: 0x0007A2EA
		public void OnMissionEnd()
		{
			this._spawnLogic.OnReinforcementsSpawned -= this.OnReinforcementsSpawned;
			this._spawnLogic.OnInitialTroopsSpawned -= this.OnInitialTroopsSpawned;
		}

		// Token: 0x060021F1 RID: 8689 RVA: 0x0007C11A File Offset: 0x0007A31A
		public void OnDeploymentFinished()
		{
			this.SetNotificationTimerEnabled(true, true);
			this._besiegerSiegeEngines = this._sallyOutController.BesiegerSiegeEngines;
		}

		// Token: 0x060021F2 RID: 8690 RVA: 0x0007C138 File Offset: 0x0007A338
		public void OnMissionTick(float dt)
		{
			if (this._notificationTimerEnabled && this._notificationTimer.ElapsedTime >= 5f)
			{
				this.CheckPeriodicNotifications();
				if (!this._notificationsQueue.IsEmpty<SallyOutMissionNotificationsHandler.NotificationType>())
				{
					SallyOutMissionNotificationsHandler.NotificationType notificationType = this._notificationsQueue.Dequeue();
					this.SendNotification(notificationType);
				}
				this._notificationTimer.Reset();
			}
		}

		// Token: 0x060021F3 RID: 8691 RVA: 0x0007C190 File Offset: 0x0007A390
		private void SetNotificationTimerEnabled(bool value, bool resetTimer = true)
		{
			this._notificationTimerEnabled = value;
			if (resetTimer)
			{
				this._notificationTimer.Reset();
			}
		}

		// Token: 0x060021F4 RID: 8692 RVA: 0x0007C1A8 File Offset: 0x0007A3A8
		private void CheckPeriodicNotifications()
		{
			if (!this._objectiveMessageSent)
			{
				this._notificationsQueue.Enqueue(SallyOutMissionNotificationsHandler.NotificationType.SallyOutObjective);
				this._objectiveMessageSent = true;
			}
			if (!this._siegeEnginesDestroyedMessageSent && this.IsSiegeEnginesDestroyed())
			{
				this._notificationsQueue.Enqueue(SallyOutMissionNotificationsHandler.NotificationType.SiegeEnginesDestroyed);
				this._siegeEnginesDestroyedMessageSent = true;
			}
			if (!this._besiegersStrengtheningMessageSent && this._spawnLogic.NumberOfRemainingDefenderTroops == 0 && this._besiegerSpawnedTroopCount >= this._spawnLogic.NumberOfActiveDefenderTroops)
			{
				this._notificationsQueue.Enqueue(SallyOutMissionNotificationsHandler.NotificationType.BesiegerSideStrenghtening);
				this._besiegersStrengtheningMessageSent = true;
			}
		}

		// Token: 0x060021F5 RID: 8693 RVA: 0x0007C230 File Offset: 0x0007A430
		private void SendNotification(SallyOutMissionNotificationsHandler.NotificationType type)
		{
			int num = -1;
			if (this._isPlayerBesieged)
			{
				switch (type)
				{
				case SallyOutMissionNotificationsHandler.NotificationType.SallyOutObjective:
					MBInformationManager.AddQuickInformation(GameTexts.FindText("str_sally_out_besieged_objective_message", null), 0, null, "");
					num = SoundEvent.GetEventIdFromString("event:/alerts/horns/move");
					break;
				case SallyOutMissionNotificationsHandler.NotificationType.BesiegerSideStrenghtening:
					MBInformationManager.AddQuickInformation(GameTexts.FindText("str_sally_out_enemy_becoming_strong_message", null), 0, null, "");
					num = SoundEvent.GetEventIdFromString("event:/alerts/horns/retreat");
					break;
				case SallyOutMissionNotificationsHandler.NotificationType.BesiegerSideReinforcementsSpawned:
					MBInformationManager.AddQuickInformation(GameTexts.FindText("str_enemy_reinforcements_arrived", null), 0, null, "");
					num = SoundEvent.GetEventIdFromString("event:/alerts/horns/reinforcements");
					break;
				case SallyOutMissionNotificationsHandler.NotificationType.BesiegedSideTacticalRetreat:
					MBInformationManager.AddQuickInformation(GameTexts.FindText("str_sally_out_allied_troops_tactical_retreat_message", null), 0, null, "");
					num = SoundEvent.GetEventIdFromString("event:/alerts/horns/retreat");
					break;
				case SallyOutMissionNotificationsHandler.NotificationType.BesiegedSidePlayerPullbackRequest:
					MBInformationManager.AddQuickInformation(GameTexts.FindText("str_sally_out_allied_pullback_or_take_command_message", null), 0, null, "");
					break;
				case SallyOutMissionNotificationsHandler.NotificationType.SiegeEnginesDestroyed:
					MBInformationManager.AddQuickInformation(GameTexts.FindText("str_sally_out_enemy_siege_engines_destroyed_message", null), 0, null, "");
					num = SoundEvent.GetEventIdFromString("event:/alerts/horns/move");
					break;
				}
			}
			else
			{
				switch (type)
				{
				case SallyOutMissionNotificationsHandler.NotificationType.SallyOutObjective:
					MBInformationManager.AddQuickInformation(GameTexts.FindText("str_sally_out_besieger_objective_message", null), 0, null, "");
					num = SoundEvent.GetEventIdFromString("event:/alerts/horns/move");
					break;
				case SallyOutMissionNotificationsHandler.NotificationType.BesiegerSideReinforcementsSpawned:
					MBInformationManager.AddQuickInformation(GameTexts.FindText("str_allied_reinforcements_arrived", null), 0, null, "");
					num = SoundEvent.GetEventIdFromString("event:/alerts/horns/reinforcements");
					break;
				case SallyOutMissionNotificationsHandler.NotificationType.BesiegedSideTacticalRetreat:
					MBInformationManager.AddQuickInformation(GameTexts.FindText("str_enemy_troops_fall_back_to_keep_message", null), 0, null, "");
					num = SoundEvent.GetEventIdFromString("event:/alerts/horns/move");
					break;
				case SallyOutMissionNotificationsHandler.NotificationType.SiegeEnginesDestroyed:
					MBInformationManager.AddQuickInformation(GameTexts.FindText("str_sally_out_allied_siege_engines_destroyed_message", null), 0, null, "");
					num = SoundEvent.GetEventIdFromString("event:/alerts/horns/move");
					break;
				}
			}
			if (num >= 0)
			{
				this.PlayNotificationSound(num);
			}
		}

		// Token: 0x060021F6 RID: 8694 RVA: 0x0007C408 File Offset: 0x0007A608
		private void PlayNotificationSound(int soundId)
		{
			MatrixFrame cameraFrame = Mission.Current.GetCameraFrame();
			Vec3 vec = cameraFrame.origin + cameraFrame.rotation.u;
			MBSoundEvent.PlaySound(soundId, vec);
		}

		// Token: 0x060021F7 RID: 8695 RVA: 0x0007C43F File Offset: 0x0007A63F
		private void OnInitialTroopsSpawned(BattleSideEnum battleSide, int numberOfTroopsSpawned)
		{
			if (battleSide == BattleSideEnum.Attacker)
			{
				this._besiegerSpawnedTroopCount += numberOfTroopsSpawned;
			}
		}

		// Token: 0x060021F8 RID: 8696 RVA: 0x0007C453 File Offset: 0x0007A653
		private void OnReinforcementsSpawned(BattleSideEnum battleSide, int numberOfTroopsSpawned)
		{
			if (battleSide == BattleSideEnum.Attacker)
			{
				this._besiegerSpawnedTroopCount += numberOfTroopsSpawned;
				this._notificationsQueue.Enqueue(SallyOutMissionNotificationsHandler.NotificationType.BesiegerSideReinforcementsSpawned);
			}
		}

		// Token: 0x060021F9 RID: 8697 RVA: 0x0007C473 File Offset: 0x0007A673
		private bool IsSiegeEnginesDestroyed()
		{
			if (this._besiegerSiegeEngines != null)
			{
				return this._besiegerSiegeEngines.All((SiegeWeapon siegeEngine) => siegeEngine.DestructionComponent.IsDestroyed);
			}
			return false;
		}

		// Token: 0x04000CB5 RID: 3253
		private const float NotificationCheckInterval = 5f;

		// Token: 0x04000CB6 RID: 3254
		private MissionAgentSpawnLogic _spawnLogic;

		// Token: 0x04000CB7 RID: 3255
		private SallyOutMissionController _sallyOutController;

		// Token: 0x04000CB8 RID: 3256
		private bool _isPlayerBesieged;

		// Token: 0x04000CB9 RID: 3257
		private MBReadOnlyList<SiegeWeapon> _besiegerSiegeEngines;

		// Token: 0x04000CBA RID: 3258
		private Queue<SallyOutMissionNotificationsHandler.NotificationType> _notificationsQueue;

		// Token: 0x04000CBB RID: 3259
		private BasicMissionTimer _notificationTimer;

		// Token: 0x04000CBC RID: 3260
		private bool _notificationTimerEnabled = true;

		// Token: 0x04000CBD RID: 3261
		private bool _objectiveMessageSent;

		// Token: 0x04000CBE RID: 3262
		private bool _siegeEnginesDestroyedMessageSent;

		// Token: 0x04000CBF RID: 3263
		private bool _besiegersStrengtheningMessageSent;

		// Token: 0x04000CC0 RID: 3264
		private int _besiegerSpawnedTroopCount;

		// Token: 0x0200058A RID: 1418
		private enum NotificationType
		{
			// Token: 0x04001D80 RID: 7552
			SallyOutObjective,
			// Token: 0x04001D81 RID: 7553
			BesiegerSideStrenghtening,
			// Token: 0x04001D82 RID: 7554
			BesiegerSideReinforcementsSpawned,
			// Token: 0x04001D83 RID: 7555
			BesiegedSideTacticalRetreat,
			// Token: 0x04001D84 RID: 7556
			BesiegedSidePlayerPullbackRequest,
			// Token: 0x04001D85 RID: 7557
			SiegeEnginesDestroyed
		}
	}
}
