using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class SallyOutMissionNotificationsHandler
	{
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

		public void OnAfterStart()
		{
			this._isPlayerBesieged = Mission.Current.PlayerTeam.Side == BattleSideEnum.Defender;
			this.SetNotificationTimerEnabled(false, true);
		}

		public void OnMissionEnd()
		{
			this._spawnLogic.OnReinforcementsSpawned -= this.OnReinforcementsSpawned;
			this._spawnLogic.OnInitialTroopsSpawned -= this.OnInitialTroopsSpawned;
		}

		public void OnDeploymentFinished()
		{
			this.SetNotificationTimerEnabled(true, true);
			this._besiegerSiegeEngines = this._sallyOutController.BesiegerSiegeEngines;
		}

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

		private void SetNotificationTimerEnabled(bool value, bool resetTimer = true)
		{
			this._notificationTimerEnabled = value;
			if (resetTimer)
			{
				this._notificationTimer.Reset();
			}
		}

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

		private void PlayNotificationSound(int soundId)
		{
			MatrixFrame cameraFrame = Mission.Current.GetCameraFrame();
			Vec3 vec = cameraFrame.origin + cameraFrame.rotation.u;
			MBSoundEvent.PlaySound(soundId, vec);
		}

		private void OnInitialTroopsSpawned(BattleSideEnum battleSide, int numberOfTroopsSpawned)
		{
			if (battleSide == BattleSideEnum.Attacker)
			{
				this._besiegerSpawnedTroopCount += numberOfTroopsSpawned;
			}
		}

		private void OnReinforcementsSpawned(BattleSideEnum battleSide, int numberOfTroopsSpawned)
		{
			if (battleSide == BattleSideEnum.Attacker)
			{
				this._besiegerSpawnedTroopCount += numberOfTroopsSpawned;
				this._notificationsQueue.Enqueue(SallyOutMissionNotificationsHandler.NotificationType.BesiegerSideReinforcementsSpawned);
			}
		}

		private bool IsSiegeEnginesDestroyed()
		{
			if (this._besiegerSiegeEngines != null)
			{
				return this._besiegerSiegeEngines.All((SiegeWeapon siegeEngine) => siegeEngine.DestructionComponent.IsDestroyed);
			}
			return false;
		}

		private const float NotificationCheckInterval = 5f;

		private MissionAgentSpawnLogic _spawnLogic;

		private SallyOutMissionController _sallyOutController;

		private bool _isPlayerBesieged;

		private MBReadOnlyList<SiegeWeapon> _besiegerSiegeEngines;

		private Queue<SallyOutMissionNotificationsHandler.NotificationType> _notificationsQueue;

		private BasicMissionTimer _notificationTimer;

		private bool _notificationTimerEnabled = true;

		private bool _objectiveMessageSent;

		private bool _siegeEnginesDestroyedMessageSent;

		private bool _besiegersStrengtheningMessageSent;

		private int _besiegerSpawnedTroopCount;

		private enum NotificationType
		{
			SallyOutObjective,
			BesiegerSideStrenghtening,
			BesiegerSideReinforcementsSpawned,
			BesiegedSideTacticalRetreat,
			BesiegedSidePlayerPullbackRequest,
			SiegeEnginesDestroyed
		}
	}
}
