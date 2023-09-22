using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.MapEvents;
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
	public class SPScoreboardVM : ScoreboardBaseVM, IBattleObserver
	{
		private bool _isPlayerDefendingSiege
		{
			get
			{
				Mission mission = Mission.Current;
				return mission != null && mission.IsSiegeBattle && Mission.Current.PlayerTeam.IsDefender;
			}
		}

		public SPScoreboardVM(BattleSimulation simulation)
		{
			this._battleSimulation = simulation;
			this.BattleResults = new MBBindingList<BattleResultVM>();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			if (this._isPlayerDefendingSiege)
			{
				base.QuitText = GameTexts.FindText("str_surrender", null).ToString();
			}
		}

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

		public override void ExecutePlayAction()
		{
			if (base.IsSimulation)
			{
				this._battleSimulation.Play();
			}
		}

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

		public override void ExecuteEndSimulationAction()
		{
			if (base.IsSimulation)
			{
				this._battleSimulation.Skip();
			}
		}

		public override void ExecuteQuitAction()
		{
			this.OnExitBattle();
		}

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

		private void UpdateSimulationResult(bool playerVictory)
		{
			if (!base.IsSimulation)
			{
				this.SimulationResult = "NotSimulation";
				return;
			}
			if (!playerVictory)
			{
				this.SimulationResult = "SimulationDefeat";
				return;
			}
			if (PlayerEncounter.Battle.PartiesOnSide(this.PlayerSide).Sum((MapEventParty x) => x.Party.NumberOfHealthyMembers) < 70)
			{
				this.SimulationResult = "SimulationVictorySmall";
				return;
			}
			this.SimulationResult = "SimulationVictoryLarge";
		}

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
				this.UpdateSimulationResult(flag);
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

		public void TroopNumberChanged(BattleSideEnum side, IBattleCombatant battleCombatant, BasicCharacterObject character, int number = 0, int numberDead = 0, int numberWounded = 0, int numberRouted = 0, int numberKilled = 0, int numberReadyToUpgrade = 0)
		{
			PartyBase partyBase = battleCombatant as PartyBase;
			bool flag = ((partyBase != null) ? partyBase.Owner : null) == Hero.MainHero;
			base.GetSide(side).UpdateScores(battleCombatant, flag, character, number, numberDead, numberWounded, numberRouted, numberKilled, numberReadyToUpgrade);
			base.PowerComparer.Update((double)base.Defenders.CurrentPower, (double)base.Attackers.CurrentPower, (double)base.Defenders.InitialPower, (double)base.Attackers.InitialPower);
		}

		public void HeroSkillIncreased(BattleSideEnum side, IBattleCombatant battleCombatant, BasicCharacterObject heroCharacter, SkillObject upgradedSkill)
		{
			PartyBase partyBase = battleCombatant as PartyBase;
			bool flag = ((partyBase != null) ? partyBase.Owner : null) == Hero.MainHero;
			base.GetSide(side).UpdateHeroSkills(battleCombatant, flag, heroCharacter, upgradedSkill);
		}

		public void BattleResultsReady()
		{
			if (!base.IsOver)
			{
				this.OnBattleOver();
			}
		}

		public void TroopSideChanged(BattleSideEnum prevSide, BattleSideEnum newSide, IBattleCombatant battleCombatant, BasicCharacterObject character)
		{
			SPScoreboardStatsVM spscoreboardStatsVM = base.GetSide(prevSide).RemoveTroop(battleCombatant, character);
			SPScoreboardSideVM side = base.GetSide(newSide);
			PartyBase partyBase = battleCombatant as PartyBase;
			side.GetPartyAddIfNotExists(battleCombatant, ((partyBase != null) ? partyBase.Owner : null) == Hero.MainHero);
			base.GetSide(newSide).AddTroop(battleCombatant, character, spscoreboardStatsVM);
		}

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

		[DataSourceProperty]
		public string SimulationResult
		{
			get
			{
				return this._simulationResult;
			}
			set
			{
				if (value != this._simulationResult)
				{
					this._simulationResult = value;
					base.OnPropertyChangedWithValue<string>(value, "SimulationResult");
				}
			}
		}

		private readonly BattleSimulation _battleSimulation;

		private static readonly TextObject _renownStr = new TextObject("{=eiWQoW9j}You gained {A0} renown.", null);

		private static readonly TextObject _influenceStr = new TextObject("{=5zeL8sa9}You gained {A0} influence.", null);

		private static readonly TextObject _moraleStr = new TextObject("{=WAKz9xX8}You gained {A0} morale.", null);

		private static readonly TextObject _lootStr = new TextObject("{=xu5NA6AW}You earned {A0}% of the loot.", null);

		private static readonly TextObject _deadLordStr = new TextObject("{=gDKhs4lD}{A0} has died on the battlefield.", null);

		private float _missionEndScoreboardDelayTimer;

		private MBBindingList<BattleResultVM> _battleResults;

		private string _simulationResult;
	}
}
