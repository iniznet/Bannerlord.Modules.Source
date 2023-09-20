using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace SandBox.View.Missions.Sound.Components
{
	public class MusicArenaPracticeMissionView : MissionView, IMusicHandler
	{
		bool IMusicHandler.IsPausable
		{
			get
			{
				return false;
			}
		}

		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			MBMusicManager.Current.DeactivateCurrentMode();
			MBMusicManager.Current.ActivateBattleMode();
			MBMusicManager.Current.OnBattleMusicHandlerInit(this);
		}

		public override void EarlyStart()
		{
			this._allOneShotSoundEventsAreDisabled = false;
			this._arenaSoundEntity = base.Mission.Scene.FindEntityWithTag("arena_sound");
			SoundManager.SetGlobalParameter("ArenaIntensity", 0f);
		}

		public override void OnMissionScreenFinalize()
		{
			SoundManager.SetGlobalParameter("ArenaIntensity", 0f);
			MBMusicManager.Current.DeactivateBattleMode();
			MBMusicManager.Current.OnBattleMusicHandlerFinalize();
		}

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

		public override void OnMissionTick(float dt)
		{
		}

		public override void OnScoreHit(Agent affectedAgent, Agent affectorAgent, WeaponComponentData attackerWeapon, bool isBlocked, bool isSiegeEngineHit, in Blow blow, in AttackCollisionData collisionData, float damagedHp, float hitDistance, float shotDifficulty)
		{
			if (affectorAgent != null && affectedAgent != null && affectorAgent.IsMainAgent && affectedAgent.IsHuman && affectedAgent.Position.Distance(affectorAgent.Position) >= 15f && (blow.VictimBodyPart == null || blow.VictimBodyPart == 1))
			{
				this._currentTournamentIntensity += 3;
				this.UpdateAudienceIntensity();
			}
		}

		public override void OnMissileHit(Agent attacker, Agent victim, bool isCanceled, AttackCollisionData collisionData)
		{
			if (!isCanceled && attacker != null && victim != null && attacker.IsMainAgent && victim.IsHuman && collisionData.IsShieldBroken)
			{
				this._currentTournamentIntensity += 2;
				this.UpdateAudienceIntensity();
			}
		}

		public override void OnMeleeHit(Agent attacker, Agent victim, bool isCanceled, AttackCollisionData collisionData)
		{
			if (!isCanceled && attacker != null && victim != null && attacker.IsMainAgent && victim.IsHuman && collisionData.IsShieldBroken)
			{
				this._currentTournamentIntensity += 2;
				this.UpdateAudienceIntensity();
			}
		}

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

		private void Cheer()
		{
			string text = "event:/mission/ambient/arena/reaction";
			Vec3 globalPosition = this._arenaSoundEntity.GlobalPosition;
			SoundManager.StartOneShotEvent(text, ref globalPosition);
		}

		public void OnUpdated(float dt)
		{
		}

		private const string ArenaSoundTag = "arena_sound";

		private const string ArenaIntensityParameterId = "ArenaIntensity";

		private const string ArenaPositiveReactionsSoundId = "event:/mission/ambient/arena/reaction";

		private const int MainAgentKnocksDownAnOpponentBaseIntensityChange = 1;

		private const int MainAgentKnocksDownAnOpponentHeadShotIntensityChange = 3;

		private const int MainAgentKnocksDownAnOpponentMountedTargetIntensityChange = 1;

		private const int MainAgentKnocksDownAnOpponentRangedHitIntensityChange = 1;

		private const int MainAgentKnocksDownAnOpponentMeleeHitIntensityChange = 2;

		private const int MainAgentHeadShotFrom15MetersRangeIntensityChange = 3;

		private const int MainAgentDismountsAnOpponentIntensityChange = 3;

		private const int MainAgentBreaksAShieldIntensityChange = 2;

		private int _currentTournamentIntensity;

		private MusicArenaPracticeMissionView.ArenaIntensityLevel _currentArenaIntensityLevel;

		private bool _allOneShotSoundEventsAreDisabled;

		private GameEntity _arenaSoundEntity;

		private enum ArenaIntensityLevel
		{
			None,
			Low,
			Mid,
			High
		}
	}
}
