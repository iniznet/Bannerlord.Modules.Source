using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Source.Missions.Handlers;
using TaleWorlds.MountAndBlade.ViewModelCollection;
using TaleWorlds.MountAndBlade.ViewModelCollection.Scoreboard;

namespace SandBox.ViewModelCollection
{
	// Token: 0x02000006 RID: 6
	public class SPScoreboardVM : ScoreboardBaseVM, IBattleObserver
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000023 RID: 35 RVA: 0x0000416C File Offset: 0x0000236C
		private bool _isPlayerDefendingSiege
		{
			get
			{
				Mission mission = Mission.Current;
				return mission != null && mission.IsSiegeBattle && Mission.Current.PlayerTeam.IsDefender;
			}
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00004192 File Offset: 0x00002392
		public SPScoreboardVM(BattleSimulation simulation)
		{
			this._battleSimulation = simulation;
			this.BattleResults = new MBBindingList<BattleResultVM>();
		}

		// Token: 0x06000025 RID: 37 RVA: 0x000041AC File Offset: 0x000023AC
		public override void RefreshValues()
		{
			base.RefreshValues();
			if (this._isPlayerDefendingSiege)
			{
				base.QuitText = GameTexts.FindText("str_surrender", null).ToString();
			}
		}

		// Token: 0x06000026 RID: 38 RVA: 0x000041D4 File Offset: 0x000023D4
		public override void Initialize(IMissionScreen missionScreen, Mission mission, Action releaseSimulationSources, Action<bool> onToggle)
		{
			base.Initialize(missionScreen, mission, releaseSimulationSources, onToggle);
			if (this._battleSimulation != null)
			{
				this._battleSimulation.BattleObserver = this;
				this.PlayerSide = (PlayerEncounter.PlayerIsAttacker ? 1 : 0);
				base.Defenders = new SPScoreboardSideVM(GameTexts.FindText("str_battle_result_army", "defender"), MobileParty.MainParty.MapEvent.DefenderSide.LeaderParty.Banner);
				base.Attackers = new SPScoreboardSideVM(GameTexts.FindText("str_battle_result_army", "attacker"), MobileParty.MainParty.MapEvent.AttackerSide.LeaderParty.Banner);
				base.IsSimulation = true;
				base.IsMainCharacterDead = true;
				base.ShowScoreboard = true;
				this._battleSimulation.ResetSimulation();
				base.PowerComparer.Update((double)base.Defenders.CurrentPower, (double)base.Attackers.CurrentPower, (double)base.Defenders.CurrentPower, (double)base.Attackers.CurrentPower);
			}
			else
			{
				base.IsSimulation = false;
				BattleObserverMissionLogic missionBehavior = this._mission.GetMissionBehavior<BattleObserverMissionLogic>();
				if (missionBehavior != null)
				{
					missionBehavior.SetObserver(this);
				}
				else
				{
					Debug.FailedAssert("SPScoreboard on CustomBattle", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox.ViewModelCollection\\SPScoreboardVM.cs", "Initialize", 79);
				}
				if (Campaign.Current != null)
				{
					if (PlayerEncounter.Battle != null)
					{
						base.Defenders = new SPScoreboardSideVM(GameTexts.FindText("str_battle_result_army", "defender"), MobileParty.MainParty.MapEvent.DefenderSide.LeaderParty.Banner);
						base.Attackers = new SPScoreboardSideVM(GameTexts.FindText("str_battle_result_army", "attacker"), MobileParty.MainParty.MapEvent.AttackerSide.LeaderParty.Banner);
						this.PlayerSide = (PlayerEncounter.PlayerIsAttacker ? 1 : 0);
					}
					else
					{
						base.Defenders = new SPScoreboardSideVM(GameTexts.FindText("str_battle_result_army", "defender"), Mission.Current.Teams.Defender.Banner);
						base.Attackers = new SPScoreboardSideVM(GameTexts.FindText("str_battle_result_army", "attacker"), Mission.Current.Teams.Attacker.Banner);
						this.PlayerSide = 0;
					}
				}
				else
				{
					Debug.FailedAssert("SPScoreboard on CustomBattle", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox.ViewModelCollection\\SPScoreboardVM.cs", "Initialize", 105);
				}
			}
			string text;
			string text2;
			if (MobileParty.MainParty.MapEvent != null)
			{
				PartyBase leaderParty = MobileParty.MainParty.MapEvent.DefenderSide.LeaderParty;
				if (((leaderParty != null) ? leaderParty.MapFaction : null) is Kingdom)
				{
					text = Color.FromUint(((Kingdom)MobileParty.MainParty.MapEvent.DefenderSide.LeaderParty.MapFaction).PrimaryBannerColor).ToString();
				}
				else
				{
					IFaction mapFaction = MobileParty.MainParty.MapEvent.DefenderSide.LeaderParty.MapFaction;
					text = Color.FromUint((mapFaction != null) ? mapFaction.Banner.GetPrimaryColor() : 0U).ToString();
				}
				PartyBase leaderParty2 = MobileParty.MainParty.MapEvent.AttackerSide.LeaderParty;
				if (((leaderParty2 != null) ? leaderParty2.MapFaction : null) is Kingdom)
				{
					text2 = Color.FromUint(((Kingdom)MobileParty.MainParty.MapEvent.AttackerSide.LeaderParty.MapFaction).PrimaryBannerColor).ToString();
				}
				else
				{
					IFaction mapFaction2 = MobileParty.MainParty.MapEvent.AttackerSide.LeaderParty.MapFaction;
					text2 = Color.FromUint((mapFaction2 != null) ? mapFaction2.Banner.GetPrimaryColor() : 0U).ToString();
				}
			}
			else
			{
				text2 = Color.FromUint(Mission.Current.Teams.Attacker.Color).ToString();
				text = Color.FromUint(Mission.Current.Teams.Defender.Color).ToString();
			}
			base.PowerComparer.SetColors(text, text2);
			base.MissionTimeInSeconds = -1;
		}

		// Token: 0x06000027 RID: 39 RVA: 0x000045C8 File Offset: 0x000027C8
		public override void Tick(float dt)
		{
			SallyOutEndLogic missionBehavior = Mission.Current.GetMissionBehavior<SallyOutEndLogic>();
			if (!base.IsOver)
			{
				if (!this._mission.IsMissionEnding)
				{
					BattleEndLogic battleEndLogic = this._battleEndLogic;
					if ((battleEndLogic == null || !battleEndLogic.IsEnemySideRetreating) && (missionBehavior == null || !missionBehavior.IsSallyOutOver))
					{
						goto IL_62;
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
			IL_62:
			base.PowerComparer.IsEnabled = Mission.Current != null && Mission.Current.Mode != 6;
			base.IsPowerComparerEnabled = base.PowerComparer.IsEnabled && !BannerlordConfig.HideBattleUI && !MBCommon.IsPaused;
			if (!base.IsSimulation && !base.IsOver)
			{
				base.MissionTimeInSeconds = (int)Mission.Current.CurrentTime;
			}
		}

		// Token: 0x06000028 RID: 40 RVA: 0x000046A2 File Offset: 0x000028A2
		public override void ExecutePlayAction()
		{
			if (base.IsSimulation)
			{
				this._battleSimulation.Play();
			}
		}

		// Token: 0x06000029 RID: 41 RVA: 0x000046B7 File Offset: 0x000028B7
		public override void ExecuteFastForwardAction()
		{
			if (!base.IsSimulation)
			{
				Mission.Current.SetFastForwardingFromUI(base.IsFastForwarding);
				return;
			}
			if (!base.IsFastForwarding)
			{
				this._battleSimulation.Play();
				return;
			}
			this._battleSimulation.FastForward();
		}

		// Token: 0x0600002A RID: 42 RVA: 0x000046F1 File Offset: 0x000028F1
		public override void ExecuteEndSimulationAction()
		{
			if (base.IsSimulation)
			{
				this._battleSimulation.Skip();
			}
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00004706 File Offset: 0x00002906
		public override void ExecuteQuitAction()
		{
			this.OnExitBattle();
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00004710 File Offset: 0x00002910
		private void GetBattleRewards(bool playerVictory)
		{
			this.BattleResults.Clear();
			if (playerVictory)
			{
				ExplainedNumber renownExplained = new ExplainedNumber(0f, true, null);
				ExplainedNumber influencExplained = new ExplainedNumber(0f, true, null);
				ExplainedNumber moraleExplained = new ExplainedNumber(0f, true, null);
				float num;
				float num2;
				float num3;
				float num4;
				float playerEarnedLootPercentage;
				PlayerEncounter.GetBattleRewards(ref num, ref num2, ref num3, ref num4, ref playerEarnedLootPercentage, ref renownExplained, ref influencExplained, ref moraleExplained);
				if (num > 0.1f)
				{
					this.BattleResults.Add(new BattleResultVM(SPScoreboardVM._renownStr.Format(num), () => SandBoxUIHelper.GetExplainedNumberTooltip(ref renownExplained), null));
				}
				if (num2 > 0.1f)
				{
					this.BattleResults.Add(new BattleResultVM(SPScoreboardVM._influenceStr.Format(num2), () => SandBoxUIHelper.GetExplainedNumberTooltip(ref influencExplained), null));
				}
				if (num3 > 0.1f || num3 < -0.1f)
				{
					this.BattleResults.Add(new BattleResultVM(SPScoreboardVM._moraleStr.Format(num3), () => SandBoxUIHelper.GetExplainedNumberTooltip(ref moraleExplained), null));
				}
				int num5 = ((this.PlayerSide == 1) ? base.Attackers.Parties.Count : base.Defenders.Parties.Count);
				if (playerEarnedLootPercentage > 0.1f && num5 > 1)
				{
					this.BattleResults.Add(new BattleResultVM(SPScoreboardVM._lootStr.Format(playerEarnedLootPercentage), () => SandBoxUIHelper.GetBattleLootAwardTooltip(playerEarnedLootPercentage), null));
				}
			}
			foreach (SPScoreboardPartyVM spscoreboardPartyVM in base.Defenders.Parties)
			{
				foreach (SPScoreboardUnitVM spscoreboardUnitVM in spscoreboardPartyVM.Members.Where((SPScoreboardUnitVM member) => member.IsHero && member.Score.Dead > 0))
				{
					this.BattleResults.Add(new BattleResultVM(SPScoreboardVM._deadLordStr.SetTextVariable("A0", spscoreboardUnitVM.Character.Name).ToString(), () => new List<TooltipProperty>(), SandBoxUIHelper.GetCharacterCode(spscoreboardUnitVM.Character as CharacterObject, false)));
				}
			}
			foreach (SPScoreboardPartyVM spscoreboardPartyVM2 in base.Attackers.Parties)
			{
				foreach (SPScoreboardUnitVM spscoreboardUnitVM2 in spscoreboardPartyVM2.Members.Where((SPScoreboardUnitVM member) => member.IsHero && member.Score.Dead > 0))
				{
					this.BattleResults.Add(new BattleResultVM(SPScoreboardVM._deadLordStr.SetTextVariable("A0", spscoreboardUnitVM2.Character.Name).ToString(), () => new List<TooltipProperty>(), SandBoxUIHelper.GetCharacterCode(spscoreboardUnitVM2.Character as CharacterObject, false)));
				}
			}
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00004AA0 File Offset: 0x00002CA0
		public void OnBattleOver()
		{
			ScoreboardBaseVM.BattleResultType battleResultType = -1;
			if (PlayerEncounter.IsActive && PlayerEncounter.Battle != null)
			{
				base.IsOver = true;
				if (PlayerEncounter.WinningSide == this.PlayerSide)
				{
					battleResultType = 1;
				}
				else if (PlayerEncounter.CampaignBattleResult != null && PlayerEncounter.CampaignBattleResult.EnemyPulledBack)
				{
					battleResultType = 2;
				}
				else
				{
					battleResultType = 0;
				}
				bool flag = PlayerEncounter.WinningSide == this.PlayerSide;
				this.GetBattleRewards(flag);
			}
			else
			{
				Mission mission = Mission.Current;
				if (mission != null && mission.MissionEnded)
				{
					base.IsOver = true;
					if ((Mission.Current.HasMissionBehavior<SallyOutEndLogic>() && !Mission.Current.MissionResult.BattleResolved) || Mission.Current.MissionResult.PlayerVictory)
					{
						battleResultType = 1;
					}
					else if (Mission.Current.MissionResult.BattleState == 3)
					{
						battleResultType = 2;
					}
					else
					{
						battleResultType = 0;
					}
				}
				else
				{
					BattleEndLogic battleEndLogic = this._battleEndLogic;
					if (battleEndLogic != null && battleEndLogic.IsEnemySideRetreating)
					{
						base.IsOver = true;
					}
				}
			}
			switch (battleResultType)
			{
			case -1:
				break;
			case 0:
				base.BattleResult = GameTexts.FindText("str_defeat", null).ToString();
				base.BattleResultIndex = battleResultType;
				return;
			case 1:
				base.BattleResult = GameTexts.FindText("str_victory", null).ToString();
				base.BattleResultIndex = battleResultType;
				return;
			case 2:
				base.BattleResult = GameTexts.FindText("str_battle_result_retreat", null).ToString();
				base.BattleResultIndex = battleResultType;
				break;
			default:
				return;
			}
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00004C04 File Offset: 0x00002E04
		public void OnExitBattle()
		{
			if (base.IsSimulation)
			{
				if (this._battleSimulation.IsSimulationFinished)
				{
					this._releaseSimulationSources();
					this._battleSimulation.OnReturn();
					return;
				}
				Game.Current.GameStateManager.RegisterActiveStateDisableRequest(this);
				InformationManager.ShowInquiry(new InquiryData(GameTexts.FindText("str_order_Retreat", null).ToString(), GameTexts.FindText("str_retreat_question", null).ToString(), true, true, GameTexts.FindText("str_ok", null).ToString(), GameTexts.FindText("str_cancel", null).ToString(), delegate
				{
					Game.Current.GameStateManager.UnregisterActiveStateDisableRequest(this);
					this._releaseSimulationSources();
					this._battleSimulation.OnReturn();
				}, delegate
				{
					Game.Current.GameStateManager.UnregisterActiveStateDisableRequest(this);
				}, "", 0f, null, null, null), false, false);
				return;
			}
			else
			{
				BattleEndLogic missionBehavior = this._mission.GetMissionBehavior<BattleEndLogic>();
				BasicMissionHandler missionBehavior2 = this._mission.GetMissionBehavior<BasicMissionHandler>();
				BattleEndLogic.ExitResult exitResult = ((missionBehavior != null) ? missionBehavior.TryExit() : (this._mission.MissionEnded ? 3 : 1));
				if (exitResult == 1 || exitResult == 2)
				{
					this.OnToggle(false);
					missionBehavior2.CreateWarningWidgetForResult(exitResult);
					return;
				}
				if (exitResult == null)
				{
					InformationManager.ShowInquiry(this._retreatInquiryData, false, false);
					return;
				}
				if (missionBehavior == null && exitResult == 3)
				{
					this._mission.EndMission();
				}
				return;
			}
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00004D38 File Offset: 0x00002F38
		public void TroopNumberChanged(BattleSideEnum side, IBattleCombatant battleCombatant, BasicCharacterObject character, int number = 0, int numberDead = 0, int numberWounded = 0, int numberRouted = 0, int numberKilled = 0, int numberReadyToUpgrade = 0)
		{
			PartyBase partyBase = battleCombatant as PartyBase;
			bool flag = ((partyBase != null) ? partyBase.Owner : null) == Hero.MainHero;
			base.GetSide(side).UpdateScores(battleCombatant, flag, character, number, numberDead, numberWounded, numberRouted, numberKilled, numberReadyToUpgrade);
			base.PowerComparer.Update((double)base.Defenders.CurrentPower, (double)base.Attackers.CurrentPower, (double)base.Defenders.InitialPower, (double)base.Attackers.InitialPower);
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00004DB8 File Offset: 0x00002FB8
		public void HeroSkillIncreased(BattleSideEnum side, IBattleCombatant battleCombatant, BasicCharacterObject heroCharacter, SkillObject upgradedSkill)
		{
			PartyBase partyBase = battleCombatant as PartyBase;
			bool flag = ((partyBase != null) ? partyBase.Owner : null) == Hero.MainHero;
			base.GetSide(side).UpdateHeroSkills(battleCombatant, flag, heroCharacter, upgradedSkill);
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00004DF0 File Offset: 0x00002FF0
		public void BattleResultsReady()
		{
			if (!base.IsOver)
			{
				this.OnBattleOver();
			}
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00004E00 File Offset: 0x00003000
		public void TroopSideChanged(BattleSideEnum prevSide, BattleSideEnum newSide, IBattleCombatant battleCombatant, BasicCharacterObject character)
		{
			SPScoreboardStatsVM spscoreboardStatsVM = base.GetSide(prevSide).RemoveTroop(battleCombatant, character);
			base.GetSide(newSide).GetPartyAddIfNotExists(battleCombatant, false);
			base.GetSide(newSide).AddTroop(battleCombatant, character, spscoreboardStatsVM);
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000033 RID: 51 RVA: 0x00004E3C File Offset: 0x0000303C
		// (set) Token: 0x06000034 RID: 52 RVA: 0x00004E44 File Offset: 0x00003044
		[DataSourceProperty]
		public override MBBindingList<BattleResultVM> BattleResults
		{
			get
			{
				return this._battleResults;
			}
			set
			{
				if (value != this._battleResults)
				{
					this._battleResults = value;
					base.OnPropertyChangedWithValue<MBBindingList<BattleResultVM>>(value, "BattleResults");
				}
			}
		}

		// Token: 0x04000007 RID: 7
		private readonly BattleSimulation _battleSimulation;

		// Token: 0x04000008 RID: 8
		private static readonly TextObject _renownStr = new TextObject("{=eiWQoW9j}You gained {A0} renown.", null);

		// Token: 0x04000009 RID: 9
		private static readonly TextObject _influenceStr = new TextObject("{=5zeL8sa9}You gained {A0} influence.", null);

		// Token: 0x0400000A RID: 10
		private static readonly TextObject _moraleStr = new TextObject("{=WAKz9xX8}You gained {A0} morale.", null);

		// Token: 0x0400000B RID: 11
		private static readonly TextObject _lootStr = new TextObject("{=xu5NA6AW}You earned {A0}% of the loot.", null);

		// Token: 0x0400000C RID: 12
		private static readonly TextObject _deadLordStr = new TextObject("{=gDKhs4lD}{A0} has died on the battlefield.", null);

		// Token: 0x0400000D RID: 13
		private float _missionEndScoreboardDelayTimer;

		// Token: 0x0400000E RID: 14
		private MBBindingList<BattleResultVM> _battleResults;
	}
}
