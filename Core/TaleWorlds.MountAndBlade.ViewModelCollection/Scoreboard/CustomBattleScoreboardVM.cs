using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Source.Missions.Handlers;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Scoreboard
{
	public class CustomBattleScoreboardVM : ScoreboardBaseVM, IBattleObserver
	{
		public override void Initialize(IMissionScreen missionScreen, Mission mission, Action releaseSimulationSources, Action<bool> onToggle)
		{
			base.Initialize(missionScreen, mission, releaseSimulationSources, onToggle);
			base.IsSimulation = false;
			BattleObserverMissionLogic missionBehavior = Mission.Current.GetMissionBehavior<BattleObserverMissionLogic>();
			if (missionBehavior != null)
			{
				missionBehavior.SetObserver(this);
			}
			this._sallyOutEndLogic = Mission.Current.GetMissionBehavior<SallyOutEndLogic>();
			this._missionCombatantsLogic = this._mission.GetMissionBehavior<MissionCombatantsLogic>();
			if (this._missionCombatantsLogic != null)
			{
				this.PlayerSide = this._missionCombatantsLogic.PlayerSide;
				base.Defenders = new SPScoreboardSideVM(GameTexts.FindText("str_battle_result_army", "defender"), this._missionCombatantsLogic.GetBannerForSide(BattleSideEnum.Defender));
				base.Attackers = new SPScoreboardSideVM(GameTexts.FindText("str_battle_result_army", "attacker"), this._missionCombatantsLogic.GetBannerForSide(BattleSideEnum.Attacker));
			}
			this.PlayerSide = Mission.Current.PlayerTeam.Side;
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			SPScoreboardSideVM defenders = base.Defenders;
			if (defenders != null)
			{
				defenders.RefreshValues();
			}
			SPScoreboardSideVM attackers = base.Attackers;
			if (attackers == null)
			{
				return;
			}
			attackers.RefreshValues();
		}

		public override void Tick(float dt)
		{
			if (!base.IsOver)
			{
				if (!this._mission.IsMissionEnding)
				{
					BattleEndLogic battleEndLogic = this._battleEndLogic;
					if ((battleEndLogic == null || !battleEndLogic.IsEnemySideRetreating) && (this._missionCombatantsLogic == null || this._battleEndLogic == null || (!this._battleEndLogic.PlayerVictory && !this._battleEndLogic.EnemyVictory)))
					{
						SallyOutEndLogic sallyOutEndLogic = this._sallyOutEndLogic;
						if (sallyOutEndLogic == null || !sallyOutEndLogic.IsSallyOutOver)
						{
							goto IL_8D;
						}
					}
				}
				if (this._missionEndScoreboardDelayTimer < 1.5f)
				{
					this._missionEndScoreboardDelayTimer += dt;
				}
				else
				{
					this.OnBattleOver();
				}
			}
			IL_8D:
			base.PowerComparer.IsEnabled = Mission.Current != null && Mission.Current.Mode != MissionMode.Deployment;
			base.IsPowerComparerEnabled = base.PowerComparer.IsEnabled && !MBCommon.IsPaused && !BannerlordConfig.HideBattleUI;
			if (!base.IsSimulation && !base.IsOver)
			{
				base.MissionTimeInSeconds = (int)this._mission.CurrentTime;
			}
		}

		public override void ExecuteFastForwardAction()
		{
			if (base.IsMainCharacterDead)
			{
				Mission.Current.SetFastForwardingFromUI(base.IsFastForwarding);
			}
		}

		public override void ExecuteQuitAction()
		{
			this.OnExitBattle();
		}

		public void OnBattleOver()
		{
			Mission mission = Mission.Current;
			if (mission == null || !mission.MissionEnded)
			{
				BattleEndLogic battleEndLogic = this._battleEndLogic;
				if (battleEndLogic != null && battleEndLogic.IsEnemySideRetreating)
				{
					base.IsOver = true;
				}
				return;
			}
			base.IsOver = true;
			if (Mission.Current.GetMissionBehavior<SallyOutEndLogic>() == null || Mission.Current.MissionResult.BattleResolved)
			{
				MissionResult missionResult = Mission.Current.MissionResult;
				bool flag = missionResult != null && missionResult.PlayerVictory;
				base.BattleResultIndex = (flag ? 1 : 0);
				base.BattleResult = (flag ? GameTexts.FindText("str_victory", null).ToString() : GameTexts.FindText("str_defeat", null).ToString());
				return;
			}
			if (Mission.Current.MissionResult.BattleState == BattleState.DefenderPullBack)
			{
				base.BattleResultIndex = 2;
				base.BattleResult = GameTexts.FindText("str_battle_result_retreat", null).ToString();
				return;
			}
			if (Mission.Current.MissionResult.PlayerVictory)
			{
				base.BattleResultIndex = 1;
				base.BattleResult = GameTexts.FindText("str_finished", null).ToString();
				return;
			}
			base.BattleResultIndex = 0;
			base.BattleResult = GameTexts.FindText("str_defeat", null).ToString();
		}

		public void OnExitBattle()
		{
			BasicMissionHandler missionBehavior = this._mission.GetMissionBehavior<BasicMissionHandler>();
			BattleEndLogic.ExitResult exitResult = (this._mission.MissionEnded ? BattleEndLogic.ExitResult.True : BattleEndLogic.ExitResult.NeedsPlayerConfirmation);
			if (exitResult == BattleEndLogic.ExitResult.NeedsPlayerConfirmation)
			{
				this.OnToggle(false);
				missionBehavior.CreateWarningWidgetForResult(exitResult);
				return;
			}
			this._mission.EndMission();
		}

		public void TroopNumberChanged(BattleSideEnum side, IBattleCombatant battleCombatant, BasicCharacterObject character, int number = 0, int numberDead = 0, int numberWounded = 0, int numberRouted = 0, int numberKilled = 0, int numberReadyToUpgrade = 0)
		{
			base.GetSide(side).UpdateScores(battleCombatant, false, character, number, numberDead, numberWounded, numberRouted, numberKilled, numberReadyToUpgrade);
			base.PowerComparer.Update((double)base.Defenders.CurrentPower, (double)base.Attackers.CurrentPower, (double)base.Defenders.InitialPower, (double)base.Attackers.InitialPower);
		}

		public void HeroSkillIncreased(BattleSideEnum side, IBattleCombatant battleCombatant, BasicCharacterObject heroCharacter, SkillObject upgradedSkill)
		{
			base.GetSide(side).UpdateHeroSkills(battleCombatant, false, heroCharacter, upgradedSkill);
		}

		public void BattleResultsReady()
		{
		}

		public void TroopSideChanged(BattleSideEnum prevSide, BattleSideEnum newSide, IBattleCombatant battleCombatant, BasicCharacterObject character)
		{
			SPScoreboardStatsVM spscoreboardStatsVM = base.GetSide(prevSide).RemoveTroop(battleCombatant, character);
			base.GetSide(newSide).GetPartyAddIfNotExists(battleCombatant, false);
			base.GetSide(newSide).AddTroop(battleCombatant, character, spscoreboardStatsVM);
		}

		private SallyOutEndLogic _sallyOutEndLogic;

		private MissionCombatantsLogic _missionCombatantsLogic;

		private float _missionEndScoreboardDelayTimer;
	}
}
