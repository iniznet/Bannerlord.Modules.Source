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
	// Token: 0x0200002D RID: 45
	public class MusicTournamentMissionView : MissionView, IMusicHandler
	{
		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600016A RID: 362 RVA: 0x000110AA File Offset: 0x0000F2AA
		bool IMusicHandler.IsPausable
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600016B RID: 363 RVA: 0x000110B0 File Offset: 0x0000F2B0
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			MBMusicManager.Current.DeactivateCurrentMode();
			MBMusicManager.Current.ActivateBattleMode();
			MBMusicManager.Current.OnBattleMusicHandlerInit(this);
			this._startTimer = new Timer(Mission.Current.CurrentTime, 3f, true);
		}

		// Token: 0x0600016C RID: 364 RVA: 0x00011100 File Offset: 0x0000F300
		public override void EarlyStart()
		{
			this._allOneShotSoundEventsAreDisabled = false;
			this._tournamentBehavior = Mission.Current.GetMissionBehavior<TournamentBehavior>();
			this._currentMatch = null;
			this._lastMatch = null;
			this._arenaSoundEntity = base.Mission.Scene.FindEntityWithTag("arena_sound");
			SoundManager.SetGlobalParameter("ArenaIntensity", 0f);
		}

		// Token: 0x0600016D RID: 365 RVA: 0x0001115C File Offset: 0x0000F35C
		public override void OnMissionScreenFinalize()
		{
			SoundManager.SetGlobalParameter("ArenaIntensity", 0f);
			MBMusicManager.Current.DeactivateBattleMode();
			MBMusicManager.Current.OnBattleMusicHandlerFinalize();
		}

		// Token: 0x0600016E RID: 366 RVA: 0x00011184 File Offset: 0x0000F384
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

		// Token: 0x0600016F RID: 367 RVA: 0x000111EC File Offset: 0x0000F3EC
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

		// Token: 0x06000170 RID: 368 RVA: 0x0001134C File Offset: 0x0000F54C
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

		// Token: 0x06000171 RID: 369 RVA: 0x000113B4 File Offset: 0x0000F5B4
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

		// Token: 0x06000172 RID: 370 RVA: 0x0001147C File Offset: 0x0000F67C
		public override void OnScoreHit(Agent affectedAgent, Agent affectorAgent, WeaponComponentData attackerWeapon, bool isBlocked, bool isSiegeEngineHit, in Blow blow, in AttackCollisionData collisionData, float damagedHp, float hitDistance, float shotDifficulty)
		{
			if (affectorAgent != null && affectedAgent != null && affectorAgent.IsMainAgent && affectedAgent.IsHuman && affectedAgent.Position.Distance(affectorAgent.Position) >= 15f && (blow.VictimBodyPart == null || blow.VictimBodyPart == 1))
			{
				this.UpdateAudienceIntensity(3, false);
			}
		}

		// Token: 0x06000173 RID: 371 RVA: 0x000114D5 File Offset: 0x0000F6D5
		public override void OnMissileHit(Agent attacker, Agent victim, bool isCanceled, AttackCollisionData collisionData)
		{
			if (!isCanceled && attacker != null && victim != null && attacker.IsMainAgent && victim.IsHuman && collisionData.IsShieldBroken)
			{
				this.UpdateAudienceIntensity(2, false);
			}
		}

		// Token: 0x06000174 RID: 372 RVA: 0x00011501 File Offset: 0x0000F701
		public override void OnMeleeHit(Agent attacker, Agent victim, bool isCanceled, AttackCollisionData collisionData)
		{
			if (!isCanceled && attacker != null && victim != null && attacker.IsMainAgent && victim.IsHuman && collisionData.IsShieldBroken)
			{
				this.UpdateAudienceIntensity(2, false);
			}
		}

		// Token: 0x06000175 RID: 373 RVA: 0x00011530 File Offset: 0x0000F730
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

		// Token: 0x06000176 RID: 374 RVA: 0x000115E0 File Offset: 0x0000F7E0
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

		// Token: 0x06000177 RID: 375 RVA: 0x00011631 File Offset: 0x0000F831
		public void OnTournamentRoundBegin(bool isFinalRound)
		{
			this._isFinalRound = isFinalRound;
			this.UpdateAudienceIntensity(0, false);
		}

		// Token: 0x06000178 RID: 376 RVA: 0x00011644 File Offset: 0x0000F844
		public void OnTournamentRoundEnd()
		{
			int num = 10;
			if (this._lastMatch.IsPlayerWinner())
			{
				num += 10;
			}
			this.UpdateAudienceIntensity(num, this._isFinalRound);
		}

		// Token: 0x040000CD RID: 205
		private const string ArenaSoundTag = "arena_sound";

		// Token: 0x040000CE RID: 206
		private const string ArenaIntensityParameterId = "ArenaIntensity";

		// Token: 0x040000CF RID: 207
		private const string ArenaPositiveReactionsSoundId = "event:/mission/ambient/arena/reaction";

		// Token: 0x040000D0 RID: 208
		private const string ArenaNegativeReactionsSoundId = "event:/mission/ambient/arena/negative_reaction";

		// Token: 0x040000D1 RID: 209
		private const string ArenaTournamentEndSoundId = "event:/mission/ambient/arena/reaction";

		// Token: 0x040000D2 RID: 210
		private const int MainAgentKnocksDownAnOpponentBaseIntensityChange = 1;

		// Token: 0x040000D3 RID: 211
		private const int MainAgentKnocksDownAnOpponentHeadShotIntensityChange = 3;

		// Token: 0x040000D4 RID: 212
		private const int MainAgentKnocksDownAnOpponentMountedTargetIntensityChange = 1;

		// Token: 0x040000D5 RID: 213
		private const int MainAgentKnocksDownAnOpponentRangedHitIntensityChange = 1;

		// Token: 0x040000D6 RID: 214
		private const int MainAgentKnocksDownAnOpponentMeleeHitIntensityChange = 2;

		// Token: 0x040000D7 RID: 215
		private const int MainAgentHeadShotFrom15MetersRangeIntensityChange = 3;

		// Token: 0x040000D8 RID: 216
		private const int MainAgentDismountsAnOpponentIntensityChange = 3;

		// Token: 0x040000D9 RID: 217
		private const int MainAgentBreaksAShieldIntensityChange = 2;

		// Token: 0x040000DA RID: 218
		private const int MainAgentWinsTournamentRoundIntensityChange = 10;

		// Token: 0x040000DB RID: 219
		private const int RoundEndIntensityChange = 10;

		// Token: 0x040000DC RID: 220
		private const int MainAgentKnocksDownATeamMateIntensityChange = -30;

		// Token: 0x040000DD RID: 221
		private const int MainAgentKnocksDownAFriendlyHorseIntensityChange = -20;

		// Token: 0x040000DE RID: 222
		private int _currentTournamentIntensity;

		// Token: 0x040000DF RID: 223
		private MusicTournamentMissionView.ArenaIntensityLevel _arenaIntensityLevel;

		// Token: 0x040000E0 RID: 224
		private bool _allOneShotSoundEventsAreDisabled;

		// Token: 0x040000E1 RID: 225
		private TournamentBehavior _tournamentBehavior;

		// Token: 0x040000E2 RID: 226
		private TournamentMatch _currentMatch;

		// Token: 0x040000E3 RID: 227
		private TournamentMatch _lastMatch;

		// Token: 0x040000E4 RID: 228
		private GameEntity _arenaSoundEntity;

		// Token: 0x040000E5 RID: 229
		private bool _isFinalRound;

		// Token: 0x040000E6 RID: 230
		private bool _fightStarted;

		// Token: 0x040000E7 RID: 231
		private Timer _startTimer;

		// Token: 0x02000074 RID: 116
		private enum ArenaIntensityLevel
		{
			// Token: 0x04000290 RID: 656
			None,
			// Token: 0x04000291 RID: 657
			Low,
			// Token: 0x04000292 RID: 658
			Mid,
			// Token: 0x04000293 RID: 659
			High
		}

		// Token: 0x02000075 RID: 117
		private enum ReactionType
		{
			// Token: 0x04000295 RID: 661
			Positive,
			// Token: 0x04000296 RID: 662
			Negative,
			// Token: 0x04000297 RID: 663
			End
		}
	}
}
