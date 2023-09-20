using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace SandBox.View.Missions.Sound.Components
{
	// Token: 0x02000027 RID: 39
	public class MusicArenaPracticeMissionView : MissionView, IMusicHandler
	{
		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000142 RID: 322 RVA: 0x0001099E File Offset: 0x0000EB9E
		bool IMusicHandler.IsPausable
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06000143 RID: 323 RVA: 0x000109A1 File Offset: 0x0000EBA1
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			MBMusicManager.Current.DeactivateCurrentMode();
			MBMusicManager.Current.ActivateBattleMode();
			MBMusicManager.Current.OnBattleMusicHandlerInit(this);
		}

		// Token: 0x06000144 RID: 324 RVA: 0x000109C8 File Offset: 0x0000EBC8
		public override void EarlyStart()
		{
			this._allOneShotSoundEventsAreDisabled = false;
			this._arenaSoundEntity = base.Mission.Scene.FindEntityWithTag("arena_sound");
			SoundManager.SetGlobalParameter("ArenaIntensity", 0f);
		}

		// Token: 0x06000145 RID: 325 RVA: 0x000109FB File Offset: 0x0000EBFB
		public override void OnMissionScreenFinalize()
		{
			SoundManager.SetGlobalParameter("ArenaIntensity", 0f);
			MBMusicManager.Current.DeactivateBattleMode();
			MBMusicManager.Current.OnBattleMusicHandlerFinalize();
		}

		// Token: 0x06000146 RID: 326 RVA: 0x00010A20 File Offset: 0x0000EC20
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			if (affectedAgent != null && affectorAgent != null && affectorAgent.IsMainAgent && (agentState == 4 || agentState == 3))
			{
				if (affectedAgent.Team != affectorAgent.Team)
				{
					if (affectedAgent.IsHuman)
					{
						this._currentTournamentIntensity++;
						if (affectedAgent.HasMount)
						{
							this._currentTournamentIntensity++;
						}
						if (killingBlow.OverrideKillInfo == null)
						{
							this._currentTournamentIntensity += 3;
						}
						if (killingBlow.IsMissile)
						{
							this._currentTournamentIntensity++;
						}
						else
						{
							this._currentTournamentIntensity += 2;
						}
					}
					else if (affectedAgent.RiderAgent != null)
					{
						this._currentTournamentIntensity += 3;
					}
				}
				this.UpdateAudienceIntensity();
			}
		}

		// Token: 0x06000147 RID: 327 RVA: 0x00010AE8 File Offset: 0x0000ECE8
		public override void OnMissionTick(float dt)
		{
		}

		// Token: 0x06000148 RID: 328 RVA: 0x00010AEC File Offset: 0x0000ECEC
		public override void OnScoreHit(Agent affectedAgent, Agent affectorAgent, WeaponComponentData attackerWeapon, bool isBlocked, bool isSiegeEngineHit, in Blow blow, in AttackCollisionData collisionData, float damagedHp, float hitDistance, float shotDifficulty)
		{
			if (affectorAgent != null && affectedAgent != null && affectorAgent.IsMainAgent && affectedAgent.IsHuman && affectedAgent.Position.Distance(affectorAgent.Position) >= 15f && (blow.VictimBodyPart == null || blow.VictimBodyPart == 1))
			{
				this._currentTournamentIntensity += 3;
				this.UpdateAudienceIntensity();
			}
		}

		// Token: 0x06000149 RID: 329 RVA: 0x00010B51 File Offset: 0x0000ED51
		public override void OnMissileHit(Agent attacker, Agent victim, bool isCanceled, AttackCollisionData collisionData)
		{
			if (!isCanceled && attacker != null && victim != null && attacker.IsMainAgent && victim.IsHuman && collisionData.IsShieldBroken)
			{
				this._currentTournamentIntensity += 2;
				this.UpdateAudienceIntensity();
			}
		}

		// Token: 0x0600014A RID: 330 RVA: 0x00010B89 File Offset: 0x0000ED89
		public override void OnMeleeHit(Agent attacker, Agent victim, bool isCanceled, AttackCollisionData collisionData)
		{
			if (!isCanceled && attacker != null && victim != null && attacker.IsMainAgent && victim.IsHuman && collisionData.IsShieldBroken)
			{
				this._currentTournamentIntensity += 2;
				this.UpdateAudienceIntensity();
			}
		}

		// Token: 0x0600014B RID: 331 RVA: 0x00010BC4 File Offset: 0x0000EDC4
		private void UpdateAudienceIntensity()
		{
			bool flag = false;
			if (this._currentTournamentIntensity > 60)
			{
				flag = this._currentArenaIntensityLevel != MusicArenaPracticeMissionView.ArenaIntensityLevel.High;
				this._currentArenaIntensityLevel = MusicArenaPracticeMissionView.ArenaIntensityLevel.High;
			}
			else if (this._currentTournamentIntensity > 30)
			{
				flag = this._currentArenaIntensityLevel != MusicArenaPracticeMissionView.ArenaIntensityLevel.Mid;
				this._currentArenaIntensityLevel = MusicArenaPracticeMissionView.ArenaIntensityLevel.Mid;
			}
			else if (this._currentTournamentIntensity <= 30)
			{
				flag = this._currentArenaIntensityLevel != MusicArenaPracticeMissionView.ArenaIntensityLevel.Low;
				this._currentArenaIntensityLevel = MusicArenaPracticeMissionView.ArenaIntensityLevel.Low;
			}
			if (flag)
			{
				SoundManager.SetGlobalParameter("ArenaIntensity", (float)this._currentArenaIntensityLevel);
			}
			if (!this._allOneShotSoundEventsAreDisabled)
			{
				this.Cheer();
			}
		}

		// Token: 0x0600014C RID: 332 RVA: 0x00010C54 File Offset: 0x0000EE54
		private void Cheer()
		{
			string text = "event:/mission/ambient/arena/reaction";
			Vec3 globalPosition = this._arenaSoundEntity.GlobalPosition;
			SoundManager.StartOneShotEvent(text, ref globalPosition);
		}

		// Token: 0x0600014D RID: 333 RVA: 0x00010C7A File Offset: 0x0000EE7A
		public void OnUpdated(float dt)
		{
		}

		// Token: 0x040000BA RID: 186
		private const string ArenaSoundTag = "arena_sound";

		// Token: 0x040000BB RID: 187
		private const string ArenaIntensityParameterId = "ArenaIntensity";

		// Token: 0x040000BC RID: 188
		private const string ArenaPositiveReactionsSoundId = "event:/mission/ambient/arena/reaction";

		// Token: 0x040000BD RID: 189
		private const int MainAgentKnocksDownAnOpponentBaseIntensityChange = 1;

		// Token: 0x040000BE RID: 190
		private const int MainAgentKnocksDownAnOpponentHeadShotIntensityChange = 3;

		// Token: 0x040000BF RID: 191
		private const int MainAgentKnocksDownAnOpponentMountedTargetIntensityChange = 1;

		// Token: 0x040000C0 RID: 192
		private const int MainAgentKnocksDownAnOpponentRangedHitIntensityChange = 1;

		// Token: 0x040000C1 RID: 193
		private const int MainAgentKnocksDownAnOpponentMeleeHitIntensityChange = 2;

		// Token: 0x040000C2 RID: 194
		private const int MainAgentHeadShotFrom15MetersRangeIntensityChange = 3;

		// Token: 0x040000C3 RID: 195
		private const int MainAgentDismountsAnOpponentIntensityChange = 3;

		// Token: 0x040000C4 RID: 196
		private const int MainAgentBreaksAShieldIntensityChange = 2;

		// Token: 0x040000C5 RID: 197
		private int _currentTournamentIntensity;

		// Token: 0x040000C6 RID: 198
		private MusicArenaPracticeMissionView.ArenaIntensityLevel _currentArenaIntensityLevel;

		// Token: 0x040000C7 RID: 199
		private bool _allOneShotSoundEventsAreDisabled;

		// Token: 0x040000C8 RID: 200
		private GameEntity _arenaSoundEntity;

		// Token: 0x02000073 RID: 115
		private enum ArenaIntensityLevel
		{
			// Token: 0x0400028B RID: 651
			None,
			// Token: 0x0400028C RID: 652
			Low,
			// Token: 0x0400028D RID: 653
			Mid,
			// Token: 0x0400028E RID: 654
			High
		}
	}
}
