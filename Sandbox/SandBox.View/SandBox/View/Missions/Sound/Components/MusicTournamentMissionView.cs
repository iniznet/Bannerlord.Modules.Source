using System;
using psai.net;
using SandBox.Tournaments.MissionLogics;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace SandBox.View.Missions.Sound.Components
{
	public class MusicTournamentMissionView : MissionView, IMusicHandler
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
			this._startTimer = new Timer(Mission.Current.CurrentTime, 3f, true);
		}

		public override void EarlyStart()
		{
			this._allOneShotSoundEventsAreDisabled = false;
			this._tournamentBehavior = Mission.Current.GetMissionBehavior<TournamentBehavior>();
			this._currentMatch = null;
			this._lastMatch = null;
			this._arenaSoundEntity = base.Mission.Scene.FindEntityWithTag("arena_sound");
			SoundManager.SetGlobalParameter("ArenaIntensity", 0f);
		}

		public override void OnMissionScreenFinalize()
		{
			SoundManager.SetGlobalParameter("ArenaIntensity", 0f);
			MBMusicManager.Current.DeactivateBattleMode();
			MBMusicManager.Current.OnBattleMusicHandlerFinalize();
		}

		private void CheckIntensityFall()
		{
			PsaiInfo psaiInfo = PsaiCore.Instance.GetPsaiInfo();
			if (psaiInfo.effectiveThemeId >= 0)
			{
				if (float.IsNaN(psaiInfo.currentIntensity))
				{
					MBMusicManager.Current.ChangeCurrentThemeIntensity(MusicParameters.MinIntensity);
					return;
				}
				if (psaiInfo.currentIntensity < MusicParameters.MinIntensity)
				{
					MBMusicManager.Current.ChangeCurrentThemeIntensity(MusicParameters.MinIntensity - psaiInfo.currentIntensity);
				}
			}
		}

		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			if (this._fightStarted)
			{
				bool flag = affectedAgent.IsMine || (affectedAgent.RiderAgent != null && affectedAgent.RiderAgent.IsMine);
				Team team = affectedAgent.Team;
				BattleSideEnum battleSideEnum = ((team != null) ? team.Side : (-1));
				bool flag2;
				if (!flag)
				{
					if (battleSideEnum != -1)
					{
						Team playerTeam = Mission.Current.PlayerTeam;
						flag2 = ((playerTeam != null) ? playerTeam.Side : (-1)) == battleSideEnum;
					}
					else
					{
						flag2 = false;
					}
				}
				else
				{
					flag2 = true;
				}
				bool flag3 = flag2;
				if ((affectedAgent.IsHuman && affectedAgent.State != 2) || flag)
				{
					float num = (flag3 ? MusicParameters.FriendlyTroopDeadEffectOnIntensity : MusicParameters.EnemyTroopDeadEffectOnIntensity);
					if (flag)
					{
						num *= MusicParameters.PlayerTroopDeadEffectMultiplierOnIntensity;
					}
					MBMusicManager.Current.ChangeCurrentThemeIntensity(num);
				}
			}
			if (affectedAgent != null && affectorAgent != null && affectorAgent.IsMainAgent && (agentState == 4 || agentState == 3))
			{
				int num2 = 0;
				if (affectedAgent.Team == affectorAgent.Team)
				{
					if (affectedAgent.IsHuman)
					{
						num2 += -30;
					}
					else
					{
						num2 += -20;
					}
				}
				else if (affectedAgent.IsHuman)
				{
					num2++;
					if (affectedAgent.HasMount)
					{
						num2++;
					}
					if (killingBlow.OverrideKillInfo == null)
					{
						num2 += 3;
					}
					if (killingBlow.IsMissile)
					{
						num2++;
					}
					else
					{
						num2 += 2;
					}
				}
				else if (affectedAgent.RiderAgent != null)
				{
					num2 += 3;
				}
				this.UpdateAudienceIntensity(num2, false);
			}
		}

		void IMusicHandler.OnUpdated(float dt)
		{
			if (!this._fightStarted && Agent.Main != null && Agent.Main.IsActive() && this._startTimer.Check(Mission.Current.CurrentTime))
			{
				this._fightStarted = true;
				MBMusicManager.Current.StartTheme(12, 0.5f, false);
			}
			if (this._fightStarted)
			{
				this.CheckIntensityFall();
			}
		}

		public override void OnMissionTick(float dt)
		{
			if (this._tournamentBehavior != null)
			{
				if (this._currentMatch != this._tournamentBehavior.CurrentMatch)
				{
					TournamentMatch currentMatch = this._tournamentBehavior.CurrentMatch;
					if (currentMatch != null && currentMatch.IsPlayerParticipating())
					{
						Agent main = Agent.Main;
						if (main != null && main.IsActive())
						{
							this._currentMatch = this._tournamentBehavior.CurrentMatch;
							this.OnTournamentRoundBegin(this._tournamentBehavior.NextRound == null);
						}
					}
				}
				if (this._lastMatch != this._tournamentBehavior.LastMatch)
				{
					this._lastMatch = this._tournamentBehavior.LastMatch;
					if (this._tournamentBehavior.NextRound == null || this._tournamentBehavior.LastMatch.IsPlayerParticipating())
					{
						this.OnTournamentRoundEnd();
					}
				}
			}
		}

		public override void OnScoreHit(Agent affectedAgent, Agent affectorAgent, WeaponComponentData attackerWeapon, bool isBlocked, bool isSiegeEngineHit, in Blow blow, in AttackCollisionData collisionData, float damagedHp, float hitDistance, float shotDifficulty)
		{
			if (affectorAgent != null && affectedAgent != null && affectorAgent.IsMainAgent && affectedAgent.IsHuman && affectedAgent.Position.Distance(affectorAgent.Position) >= 15f && (blow.VictimBodyPart == null || blow.VictimBodyPart == 1))
			{
				this.UpdateAudienceIntensity(3, false);
			}
		}

		public override void OnMissileHit(Agent attacker, Agent victim, bool isCanceled, AttackCollisionData collisionData)
		{
			if (!isCanceled && attacker != null && victim != null && attacker.IsMainAgent && victim.IsHuman && collisionData.IsShieldBroken)
			{
				this.UpdateAudienceIntensity(2, false);
			}
		}

		public override void OnMeleeHit(Agent attacker, Agent victim, bool isCanceled, AttackCollisionData collisionData)
		{
			if (!isCanceled && attacker != null && victim != null && attacker.IsMainAgent && victim.IsHuman && collisionData.IsShieldBroken)
			{
				this.UpdateAudienceIntensity(2, false);
			}
		}

		private void UpdateAudienceIntensity(int intensityChangeAmount, bool isEnd = false)
		{
			MusicTournamentMissionView.ReactionType reactionType;
			if (!isEnd)
			{
				reactionType = ((intensityChangeAmount >= 0) ? MusicTournamentMissionView.ReactionType.Positive : MusicTournamentMissionView.ReactionType.Negative);
			}
			else
			{
				reactionType = MusicTournamentMissionView.ReactionType.End;
			}
			this._currentTournamentIntensity += intensityChangeAmount;
			bool flag = false;
			if (this._currentTournamentIntensity > 60)
			{
				flag = this._arenaIntensityLevel != MusicTournamentMissionView.ArenaIntensityLevel.High;
				this._arenaIntensityLevel = MusicTournamentMissionView.ArenaIntensityLevel.High;
			}
			else if (this._currentTournamentIntensity > 30)
			{
				flag = this._arenaIntensityLevel != MusicTournamentMissionView.ArenaIntensityLevel.Mid;
				this._arenaIntensityLevel = MusicTournamentMissionView.ArenaIntensityLevel.Mid;
			}
			else if (this._currentTournamentIntensity <= 30)
			{
				flag = this._arenaIntensityLevel != MusicTournamentMissionView.ArenaIntensityLevel.Low;
				this._arenaIntensityLevel = MusicTournamentMissionView.ArenaIntensityLevel.Low;
			}
			if (flag)
			{
				SoundManager.SetGlobalParameter("ArenaIntensity", (float)this._arenaIntensityLevel);
			}
			if (!this._allOneShotSoundEventsAreDisabled)
			{
				this.Cheer(reactionType);
			}
		}

		private void Cheer(MusicTournamentMissionView.ReactionType reactionType)
		{
			string text = null;
			switch (reactionType)
			{
			case MusicTournamentMissionView.ReactionType.Positive:
				text = "event:/mission/ambient/arena/reaction";
				break;
			case MusicTournamentMissionView.ReactionType.Negative:
				text = "event:/mission/ambient/arena/negative_reaction";
				break;
			case MusicTournamentMissionView.ReactionType.End:
				text = "event:/mission/ambient/arena/reaction";
				break;
			}
			if (text != null)
			{
				string text2 = text;
				Vec3 globalPosition = this._arenaSoundEntity.GlobalPosition;
				SoundManager.StartOneShotEvent(text2, ref globalPosition);
			}
		}

		public void OnTournamentRoundBegin(bool isFinalRound)
		{
			this._isFinalRound = isFinalRound;
			this.UpdateAudienceIntensity(0, false);
		}

		public void OnTournamentRoundEnd()
		{
			int num = 10;
			if (this._lastMatch.IsPlayerWinner())
			{
				num += 10;
			}
			this.UpdateAudienceIntensity(num, this._isFinalRound);
		}

		private const string ArenaSoundTag = "arena_sound";

		private const string ArenaIntensityParameterId = "ArenaIntensity";

		private const string ArenaPositiveReactionsSoundId = "event:/mission/ambient/arena/reaction";

		private const string ArenaNegativeReactionsSoundId = "event:/mission/ambient/arena/negative_reaction";

		private const string ArenaTournamentEndSoundId = "event:/mission/ambient/arena/reaction";

		private const int MainAgentKnocksDownAnOpponentBaseIntensityChange = 1;

		private const int MainAgentKnocksDownAnOpponentHeadShotIntensityChange = 3;

		private const int MainAgentKnocksDownAnOpponentMountedTargetIntensityChange = 1;

		private const int MainAgentKnocksDownAnOpponentRangedHitIntensityChange = 1;

		private const int MainAgentKnocksDownAnOpponentMeleeHitIntensityChange = 2;

		private const int MainAgentHeadShotFrom15MetersRangeIntensityChange = 3;

		private const int MainAgentDismountsAnOpponentIntensityChange = 3;

		private const int MainAgentBreaksAShieldIntensityChange = 2;

		private const int MainAgentWinsTournamentRoundIntensityChange = 10;

		private const int RoundEndIntensityChange = 10;

		private const int MainAgentKnocksDownATeamMateIntensityChange = -30;

		private const int MainAgentKnocksDownAFriendlyHorseIntensityChange = -20;

		private int _currentTournamentIntensity;

		private MusicTournamentMissionView.ArenaIntensityLevel _arenaIntensityLevel;

		private bool _allOneShotSoundEventsAreDisabled;

		private TournamentBehavior _tournamentBehavior;

		private TournamentMatch _currentMatch;

		private TournamentMatch _lastMatch;

		private GameEntity _arenaSoundEntity;

		private bool _isFinalRound;

		private bool _fightStarted;

		private Timer _startTimer;

		private enum ArenaIntensityLevel
		{
			None,
			Low,
			Mid,
			High
		}

		private enum ReactionType
		{
			Positive,
			Negative,
			End
		}
	}
}
