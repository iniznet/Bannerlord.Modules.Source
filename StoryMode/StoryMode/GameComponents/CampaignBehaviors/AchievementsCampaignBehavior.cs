using System;
using System.Collections.Generic;
using System.Linq;
using StoryMode.Quests.ThirdPhase;
using TaleWorlds.AchievementSystem;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace StoryMode.GameComponents.CampaignBehaviors
{
	// Token: 0x0200004A RID: 74
	public class AchievementsCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x060003CD RID: 973 RVA: 0x00017805 File Offset: 0x00015A05
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<bool>("_deactivateAchievements", ref this._deactivateAchievements);
			if (this._deactivateAchievements)
			{
				this.DeactivateAchievements(false);
				Debug.Print("Achievements were disabled in save file!", 0, 0, 17592186044416UL);
			}
		}

		// Token: 0x060003CE RID: 974 RVA: 0x00017840 File Offset: 0x00015A40
		public override void RegisterEvents()
		{
			CampaignEvents.OnCharacterCreationIsOverEvent.AddNonSerializedListener(this, new Action(this.CacheHighestSkillValue));
			CampaignEvents.OnWorkshopChangedEvent.AddNonSerializedListener(this, new Action<Workshop, Hero, WorkshopType>(this.ProgressOwnedWorkshopCount));
			CampaignEvents.MobilePartyCreated.AddNonSerializedListener(this, new Action<MobileParty>(this.ProgressOwnedCaravanCount));
			CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementOwnerChanged));
			CampaignEvents.KingdomCreatedEvent.AddNonSerializedListener(this, new Action<Kingdom>(this.ProgressCreatedKingdomCount));
			CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.OnHeroKilled));
			CampaignEvents.BeforeHeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.OnBeforeHeroKilled));
			CampaignEvents.ClanTierIncrease.AddNonSerializedListener(this, new Action<Clan, bool>(this.ProgressClanTier));
			CampaignEvents.OnHideoutBattleCompletedEvent.AddNonSerializedListener(this, new Action<BattleSideEnum, MapEvent>(this.OnHideoutBattleCompleted));
			CampaignEvents.HeroGainedSkill.AddNonSerializedListener(this, new Action<Hero, SkillObject, int, bool>(this.ProgressHeroSkillValue));
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
			CampaignEvents.PlayerInventoryExchangeEvent.AddNonSerializedListener(this, new Action<List<ValueTuple<ItemRosterElement, int>>, List<ValueTuple<ItemRosterElement, int>>, bool>(this.PlayerInventoryExchange));
			CampaignEvents.TournamentFinished.AddNonSerializedListener(this, new Action<CharacterObject, MBReadOnlyList<CharacterObject>, Town, ItemObject>(this.OnTournamentFinish));
			CampaignEvents.SiegeCompletedEvent.AddNonSerializedListener(this, new Action<Settlement, MobileParty, bool, MapEvent.BattleTypes>(this.OnSiegeCompleted));
			CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.OnMapEventEnded));
			CampaignEvents.OnQuestCompletedEvent.AddNonSerializedListener(this, new Action<QuestBase, QuestBase.QuestCompleteDetails>(this.OnQuestCompleted));
			CampaignEvents.OnBuildingLevelChangedEvent.AddNonSerializedListener(this, new Action<Town, Building, int>(this.OnBuildingLevelChanged));
			CampaignEvents.OnNewItemCraftedEvent.AddNonSerializedListener(this, new Action<ItemObject, Crafting.OverrideData, bool>(this.OnNewItemCrafted));
			CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.OnClanChangedKingdom));
			CampaignEvents.OnClanDestroyedEvent.AddNonSerializedListener(this, new Action<Clan>(this.OnClanDestroyed));
			CampaignEvents.OnPlayerTradeProfitEvent.AddNonSerializedListener(this, new Action<int>(this.ProgressTotalTradeProfit));
			CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.OnDailyTick));
			CampaignEvents.HeroesMarried.AddNonSerializedListener(this, new Action<Hero, Hero, bool>(this.CheckHeroMarriage));
			CampaignEvents.KingdomDecisionConcluded.AddNonSerializedListener(this, new Action<KingdomDecision, DecisionOutcome, bool>(this.CheckKingdomDecisionConcluded));
			CampaignEvents.OnMissionStartedEvent.AddNonSerializedListener(this, new Action<IMission>(this.OnMissionStarted));
			CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEnter));
			CampaignEvents.OnNewGameCreatedPartialFollowUpEndEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnNewGameCreatedPartialFollowUpEnd));
			CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, new Action(this.OnGameLoadFinished));
			CampaignEvents.HeroCreated.AddNonSerializedListener(this, new Action<Hero, bool>(this.OnHeroCreated));
			CampaignEvents.OnIssueUpdatedEvent.AddNonSerializedListener(this, new Action<IssueBase, IssueBase.IssueUpdateDetails, Hero>(this.OnIssueUpdated));
			CampaignEvents.RulingClanChanged.AddNonSerializedListener(this, new Action<Kingdom, Clan>(this.OnRulingClanChanged));
			StoryModeEvents.OnStoryModeTutorialEndedEvent.AddNonSerializedListener(this, new Action(this.CheckTutorialFinished));
			StoryModeEvents.OnBannerPieceCollectedEvent.AddNonSerializedListener(this, new Action(this.ProgressAssembledDragonBanner));
			StoryModeEvents.OnConfigChangedEvent.AddNonSerializedListener(this, new Action(this.OnConfigChanged));
		}

		// Token: 0x060003CF RID: 975 RVA: 0x00017B5B File Offset: 0x00015D5B
		private void OnRulingClanChanged(Kingdom kingdom, Clan newRulingCLan)
		{
			this.ProgressOwnedFortificationCount();
		}

		// Token: 0x060003D0 RID: 976 RVA: 0x00017B64 File Offset: 0x00015D64
		private void OnIssueUpdated(IssueBase issueBase, IssueBase.IssueUpdateDetails detail, Hero issueSolver)
		{
			if (issueSolver == Hero.MainHero && !issueBase.IsSolvingWithAlternative && detail == 5 && issueBase.IssueOwner.MapFaction != null && issueBase.IssueOwner.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction))
			{
				this.SetStatInternal("CompletedAnIssueInHostileTown", 1);
			}
		}

		// Token: 0x060003D1 RID: 977 RVA: 0x00017BBA File Offset: 0x00015DBA
		private void OnHideoutBattleCompleted(BattleSideEnum winnerSide, MapEvent mapEvent)
		{
			if (mapEvent.InvolvedParties.Contains(PartyBase.MainParty) && winnerSide == mapEvent.PlayerSide)
			{
				this.ProgressHideoutClearedCount();
			}
		}

		// Token: 0x060003D2 RID: 978 RVA: 0x00017BDD File Offset: 0x00015DDD
		private void OnBeforeHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification = true)
		{
			this.ProgressKingOrQueenKilledInBattle(victim, killer, detail);
		}

		// Token: 0x060003D3 RID: 979 RVA: 0x00017BE8 File Offset: 0x00015DE8
		private void OnConfigChanged()
		{
			if (!this.CheckAchievementSystemActivity())
			{
				this.DeactivateAchievements(true);
			}
		}

		// Token: 0x060003D4 RID: 980 RVA: 0x00017BF9 File Offset: 0x00015DF9
		private void OnHeroCreated(Hero hero, bool isBornNaturally)
		{
			if (isBornNaturally)
			{
				if (hero.Father == Hero.MainHero || hero.Mother == Hero.MainHero)
				{
					this.ProgressChildCount();
				}
				this.CheckGrandparent();
			}
		}

		// Token: 0x060003D5 RID: 981 RVA: 0x00017C24 File Offset: 0x00015E24
		private void OnGameLoadFinished()
		{
			if (this.CheckAchievementSystemActivity())
			{
				this.CacheAndInitializeAchievementVariables();
				this.CacheHighestSkillValue();
				return;
			}
			this.DeactivateAchievements(true);
		}

		// Token: 0x060003D6 RID: 982 RVA: 0x00017C44 File Offset: 0x00015E44
		private async void CacheAndInitializeAchievementVariables()
		{
			this._butter = MBObjectManager.Instance.GetObject<ItemObject>("butter");
			List<string> list = new List<string>
			{
				"CreatedKingdomCount", "ClearedHideoutCount", "RepelledSiegeAssaultCount", "KingOrQueenKilledInBattle", "SuccessfulSiegeCount", "WonTournamentCount", "SuccessfulBattlesAgainstArmyCount", "DefeatedArmyWhileAloneCount", "TotalTradeProfit", "MaxDailyIncome",
				"CapturedATownAloneCount", "DefeatedTroopCount", "FarthestHeadShot"
			};
			this._orderedSettlementList = (from x in Settlement.All
				where x.IsFortification
				orderby x.StringId descending
				select x).ToList<Settlement>();
			int neededIntegerCount = MathF.Ceiling((float)this._orderedSettlementList.Count / 30f);
			this._settlementIntegerSetList = new int[neededIntegerCount];
			for (int i = 0; i < neededIntegerCount; i++)
			{
				list.Add("SettlementSet" + i);
			}
			int[] array = await AchievementManager.GetStats(list.ToArray());
			if (array != null)
			{
				int num = 0;
				int[] array2 = array;
				num++;
				this._cachedCreatedKingdomCount = array2[num];
				int[] array3 = array;
				num++;
				this._cachedHideoutClearedCount = array3[num];
				int[] array4 = array;
				num++;
				this._cachedRepelledSiegeAssaultCount = array4[num];
				int[] array5 = array;
				num++;
				this._cachedKingOrQueenKilledInBattle = array5[num];
				int[] array6 = array;
				num++;
				this._cachedSuccessfulSiegeCount = array6[num];
				int[] array7 = array;
				num++;
				this._cachedWonTournamentCount = array7[num];
				int[] array8 = array;
				num++;
				this._cachedSuccessfulBattlesAgainstArmyCount = array8[num];
				int[] array9 = array;
				num++;
				this._cachedSuccessfulBattlesAgainstArmyAloneCount = array9[num];
				int[] array10 = array;
				num++;
				this._cachedTotalTradeProfit = array10[num];
				int[] array11 = array;
				num++;
				this._cachedMaxDailyIncome = array11[num];
				int[] array12 = array;
				num++;
				this._cachedCapturedTownAloneCount = array12[num];
				int[] array13 = array;
				num++;
				this._cachedDefeatedTroopCount = array13[num];
				int[] array14 = array;
				num++;
				this._cachedFarthestHeadShot = array14[num];
				for (int j = 0; j < neededIntegerCount; j++)
				{
					int num2 = array[num++];
					if (num2 == -1)
					{
						this._settlementIntegerSetList[j] = 0;
						this.SetStatInternal("SettlementSet" + j, 0);
					}
					else
					{
						this._settlementIntegerSetList[j] = num2;
					}
				}
			}
			else
			{
				this.DeactivateAchievements(true);
				Debug.Print("Achievements are disabled because current platform does not support achievements!", 0, 0, 17592186044416UL);
			}
		}

		// Token: 0x060003D7 RID: 983 RVA: 0x00017C7D File Offset: 0x00015E7D
		private void OnNewGameCreatedPartialFollowUpEnd(CampaignGameStarter starter)
		{
			if (this.CheckAchievementSystemActivity())
			{
				this.CacheAndInitializeAchievementVariables();
				return;
			}
			this.DeactivateAchievements(true);
		}

		// Token: 0x060003D8 RID: 984 RVA: 0x00017C95 File Offset: 0x00015E95
		private void OnDailyTick()
		{
			this.ProgressDailyTribute();
			this.ProgressDailyIncome();
		}

		// Token: 0x060003D9 RID: 985 RVA: 0x00017CA3 File Offset: 0x00015EA3
		private void OnClanDestroyed(Clan clan)
		{
			this.ProgressClansUnderKingdomCount();
		}

		// Token: 0x060003DA RID: 986 RVA: 0x00017CAB File Offset: 0x00015EAB
		private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification = true)
		{
			this.ProgressDailyIncome();
			this.ProgressClansUnderKingdomCount();
			this.ProgressOwnedFortificationCount();
		}

		// Token: 0x060003DB RID: 987 RVA: 0x00017CBF File Offset: 0x00015EBF
		private void OnNewItemCrafted(ItemObject itemObject, Crafting.OverrideData overrideData, bool isCraftingOrderItem)
		{
			this.ProgressHighestTierSwordCrafted(itemObject, overrideData);
		}

		// Token: 0x060003DC RID: 988 RVA: 0x00017CC9 File Offset: 0x00015EC9
		private void OnBuildingLevelChanged(Town town, Building building, int levelChange)
		{
			this.ProgressDailyIncome();
			this.CheckProjectsInSettlement(town);
		}

		// Token: 0x060003DD RID: 989 RVA: 0x00017CD8 File Offset: 0x00015ED8
		private void OnQuestCompleted(QuestBase quest, QuestBase.QuestCompleteDetails detail)
		{
			this.ProgressImperialBarbarianVictory(quest, detail);
		}

		// Token: 0x060003DE RID: 990 RVA: 0x00017CE2 File Offset: 0x00015EE2
		private void OnTournamentFinish(CharacterObject winner, MBReadOnlyList<CharacterObject> participants, Town town, ItemObject prize)
		{
			this.ProgressTournamentWonCount(winner);
			this.ProgressTournamentRank(winner);
		}

		// Token: 0x060003DF RID: 991 RVA: 0x00017CF2 File Offset: 0x00015EF2
		private void OnMapEventEnded(MapEvent mapEvent)
		{
			this.ProgressRepelSiegeAssaultCount(mapEvent);
			this.CheckDefeatedSuperiorForce(mapEvent);
			this.ProgressSuccessfulBattlesAgainstArmyCount(mapEvent);
			this.ProgressSuccessfulBattlesAgainstArmyAloneCount(mapEvent);
		}

		// Token: 0x060003E0 RID: 992 RVA: 0x00017D10 File Offset: 0x00015F10
		private void OnSiegeCompleted(Settlement siegeSettlement, MobileParty attackerParty, bool isWin, MapEvent.BattleTypes battleType)
		{
			this.ProgressRepelSiegeAssaultCount(siegeSettlement, isWin);
			this.ProgressSuccessfulSiegeCount(attackerParty, isWin);
			this.ProgressCapturedATownAlone(attackerParty, isWin);
		}

		// Token: 0x060003E1 RID: 993 RVA: 0x00017D2C File Offset: 0x00015F2C
		private void PlayerInventoryExchange(List<ValueTuple<ItemRosterElement, int>> purchasedItems, List<ValueTuple<ItemRosterElement, int>> soldItems, bool isTrading)
		{
			if (this._butter != null)
			{
				int num = 0;
				bool flag = false;
				foreach (ValueTuple<ItemRosterElement, int> valueTuple in purchasedItems)
				{
					ItemRosterElement item = valueTuple.Item1;
					if (item.EquipmentElement.Item == this._butter)
					{
						flag = true;
					}
				}
				if (flag)
				{
					int num2 = PartyBase.MainParty.ItemRoster.FindIndexOfItem(this._butter);
					if (num2 != -1)
					{
						num = PartyBase.MainParty.ItemRoster.GetElementNumber(num2);
					}
					this.SetStatInternal("ButtersInInventoryCount", num);
				}
			}
		}

		// Token: 0x060003E2 RID: 994 RVA: 0x00017DE4 File Offset: 0x00015FE4
		private void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
		{
			if (!this.CheckAchievementSystemActivity())
			{
				this.DeactivateAchievements(true);
			}
		}

		// Token: 0x060003E3 RID: 995 RVA: 0x00017DF8 File Offset: 0x00015FF8
		private bool CheckAchievementSystemActivity()
		{
			bool flag = this.CheckIfModulesAreDefault() && !Game.Current.CheatMode && !this._deactivateAchievements;
			if (!flag)
			{
				if (!this.CheckIfModulesAreDefault())
				{
					Debug.Print("Achievements were disabled because !CheckIfModulesAreDefault", 0, 0, 17592186044416UL);
				}
				if (Game.Current.CheatMode)
				{
					Debug.Print("Achievements were disabled because Game.Current.CheatMode", 0, 0, 17592186044416UL);
				}
				if (this._deactivateAchievements)
				{
					Debug.Print("Achievements were disabled because _deactivateAchievements was true", 0, 0, 17592186044416UL);
				}
			}
			return flag;
		}

		// Token: 0x060003E4 RID: 996 RVA: 0x00017E84 File Offset: 0x00016084
		private void OnSettlementEnter(MobileParty party, Settlement settlement, Hero hero)
		{
			if (party == MobileParty.MainParty && settlement.IsFortification)
			{
				int num = this._orderedSettlementList.IndexOf(settlement);
				int num2 = MathF.Floor((float)num / 30f);
				int num3 = this._settlementIntegerSetList[num2];
				int num4 = 1 << (int)(30f - ((float)num % 30f + 1f));
				int num5 = num3 | num4;
				this.SetStatInternal("SettlementSet" + num2, num5);
				if (this._settlementIntegerSetList[num2] != num5)
				{
					this._settlementIntegerSetList[num2] = num5;
					this.CheckEnteredEverySettlement();
				}
			}
		}

		// Token: 0x060003E5 RID: 997 RVA: 0x00017F14 File Offset: 0x00016114
		private void CheckEnteredEverySettlement()
		{
			int num = 0;
			for (int i = 0; i < this._settlementIntegerSetList.Length; i++)
			{
				for (int j = this._settlementIntegerSetList[i]; j > 0; j >>= 1)
				{
					if (j % 2 == 1)
					{
						num++;
					}
				}
			}
			if (num == this._orderedSettlementList.Count)
			{
				this.SetStatInternal("EnteredEverySettlement", 1);
			}
		}

		// Token: 0x060003E6 RID: 998 RVA: 0x00017F70 File Offset: 0x00016170
		private void CacheHighestSkillValue()
		{
			int num = 0;
			foreach (SkillObject skillObject in Skills.All)
			{
				int skillValue = Hero.MainHero.GetSkillValue(skillObject);
				if (skillValue > num)
				{
					num = skillValue;
				}
			}
			this._cachedHighestSkillValue = num;
		}

		// Token: 0x060003E7 RID: 999 RVA: 0x00017FD8 File Offset: 0x000161D8
		private void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification = true)
		{
			this.CheckExecutedLordRelation(victim, killer, detail);
			this.CheckBestServedCold(victim, killer, detail);
		}

		// Token: 0x060003E8 RID: 1000 RVA: 0x00017FEC File Offset: 0x000161EC
		private void OnSettlementOwnerChanged(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturerHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
		{
			this.ProgressDailyIncome();
			if (settlement.IsFortification)
			{
				this.ProgressOwnedFortificationCount();
			}
		}

		// Token: 0x060003E9 RID: 1001 RVA: 0x00018004 File Offset: 0x00016204
		private void OnMissionStarted(IMission obj)
		{
			AchievementsCampaignBehavior.AchievementMissionLogic achievementMissionLogic = new AchievementsCampaignBehavior.AchievementMissionLogic(new Action<Agent, Agent>(this.OnAgentRemoved), new Action<Agent, WeaponComponentData, BoneBodyPartType, int>(this.OnAgentHit));
			Mission.Current.AddMissionBehavior(achievementMissionLogic);
		}

		// Token: 0x060003EA RID: 1002 RVA: 0x0001803A File Offset: 0x0001623A
		private void OnAgentHit(Agent affectorAgent, WeaponComponentData attackerWeapon, BoneBodyPartType victimBoneBodyPartType, int hitDistance)
		{
			if (affectorAgent == Agent.Main && attackerWeapon != null && !attackerWeapon.IsMeleeWeapon && victimBoneBodyPartType == null && hitDistance > this._cachedFarthestHeadShot)
			{
				this.SetStatInternal("FarthestHeadShot", hitDistance);
				this._cachedFarthestHeadShot = hitDistance;
			}
		}

		// Token: 0x060003EB RID: 1003 RVA: 0x00018074 File Offset: 0x00016274
		private void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent)
		{
			if (affectorAgent == Agent.Main)
			{
				string text = "DefeatedTroopCount";
				int num = this._cachedDefeatedTroopCount + 1;
				this._cachedDefeatedTroopCount = num;
				this.SetStatInternal(text, num);
			}
		}

		// Token: 0x060003EC RID: 1004 RVA: 0x000180A8 File Offset: 0x000162A8
		private void ProgressChildCount()
		{
			int num = Hero.MainHero.Children.Count;
			using (List<LogEntry>.Enumerator enumerator = Campaign.Current.LogEntryHistory.GameActionLogs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PlayerCharacterChangedLogEntry playerCharacterChangedLogEntry;
					if ((playerCharacterChangedLogEntry = enumerator.Current as PlayerCharacterChangedLogEntry) != null)
					{
						num += playerCharacterChangedLogEntry.OldPlayerHero.Children.Count;
					}
				}
			}
			this.SetStatInternal("NumberOfChildrenBorn", num);
		}

		// Token: 0x060003ED RID: 1005 RVA: 0x00018134 File Offset: 0x00016334
		private void CheckGrandparent()
		{
			if (Hero.MainHero.Children.Any((Hero x) => x.Children.Any((Hero y) => y.Children.Any<Hero>())))
			{
				this.SetStatInternal("GreatGranny", 1);
			}
		}

		// Token: 0x060003EE RID: 1006 RVA: 0x00018172 File Offset: 0x00016372
		public void OnRadagosDuelWon()
		{
			this.SetStatInternal("RadagosDefeatedInDuel", 1);
		}

		// Token: 0x060003EF RID: 1007 RVA: 0x00018180 File Offset: 0x00016380
		private void CheckBestServedCold(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail)
		{
			if (killer == Hero.MainHero && detail == 6)
			{
				using (List<LogEntry>.Enumerator enumerator = Campaign.Current.LogEntryHistory.GameActionLogs.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						CharacterKilledLogEntry characterKilledLogEntry;
						if ((characterKilledLogEntry = enumerator.Current as CharacterKilledLogEntry) != null && characterKilledLogEntry.Killer == victim && characterKilledLogEntry.Victim.Clan == Clan.PlayerClan)
						{
							this.SetStatInternal("BestServedCold", 1);
							break;
						}
					}
				}
			}
		}

		// Token: 0x060003F0 RID: 1008 RVA: 0x00018214 File Offset: 0x00016414
		private void CheckProposedAndWonPolicy(KingdomDecision decision, DecisionOutcome chosenOutcome)
		{
			if (decision.ProposerClan == Clan.PlayerClan)
			{
				MBList<DecisionOutcome> mblist = new MBList<DecisionOutcome>();
				mblist.Add(chosenOutcome);
				if (decision.GetQueriedDecisionOutcome(mblist) != null)
				{
					this.SetStatInternal("ProposedAndWonAPolicy", 1);
				}
			}
		}

		// Token: 0x060003F1 RID: 1009 RVA: 0x00018246 File Offset: 0x00016446
		private void CheckKingdomDecisionConcluded(KingdomDecision decision, DecisionOutcome chosenOutcome, bool isPlayerInvolved)
		{
			this.CheckProposedAndWonPolicy(decision, chosenOutcome);
			this.ProgressOwnedFortificationCount();
			this.ProgressClansUnderKingdomCount();
		}

		// Token: 0x060003F2 RID: 1010 RVA: 0x0001825C File Offset: 0x0001645C
		private void CheckHeroMarriage(Hero hero1, Hero hero2, bool showNotification = true)
		{
			if (hero1 == Hero.MainHero || hero2 == Hero.MainHero)
			{
				Hero hero3 = ((hero1 == Hero.MainHero) ? hero2 : hero1);
				using (List<LogEntry>.Enumerator enumerator = Campaign.Current.LogEntryHistory.GameActionLogs.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						CharacterKilledLogEntry characterKilledLogEntry;
						if ((characterKilledLogEntry = enumerator.Current as CharacterKilledLogEntry) != null && characterKilledLogEntry.Killer == Hero.MainHero && hero3.ExSpouses.Contains(characterKilledLogEntry.Victim))
						{
							this.SetStatInternal("Hearthbreaker", 1);
						}
					}
				}
			}
		}

		// Token: 0x060003F3 RID: 1011 RVA: 0x00018304 File Offset: 0x00016504
		private void ProgressClansUnderKingdomCount()
		{
			if (Clan.PlayerClan.Kingdom != null && Clan.PlayerClan.Kingdom.Leader == Hero.MainHero)
			{
				this.SetStatInternal("ClansUnderPlayerKingdomCount", Clan.PlayerClan.Kingdom.Clans.Count);
			}
		}

		// Token: 0x060003F4 RID: 1012 RVA: 0x00018354 File Offset: 0x00016554
		private void ProgressSuccessfulBattlesAgainstArmyCount(MapEvent mapEvent)
		{
			if (mapEvent.IsPlayerMapEvent && mapEvent.Winner == mapEvent.GetMapEventSide(mapEvent.PlayerSide))
			{
				if (mapEvent.GetMapEventSide(mapEvent.DefeatedSide).Parties.Any((MapEventParty x) => x.Party.MobileParty != null && x.Party.MobileParty.Army != null))
				{
					string text = "SuccessfulBattlesAgainstArmyCount";
					int num = this._cachedSuccessfulBattlesAgainstArmyCount + 1;
					this._cachedSuccessfulBattlesAgainstArmyCount = num;
					this.SetStatInternal(text, num);
				}
			}
		}

		// Token: 0x060003F5 RID: 1013 RVA: 0x000183D0 File Offset: 0x000165D0
		private void ProgressSuccessfulBattlesAgainstArmyAloneCount(MapEvent mapEvent)
		{
			if (mapEvent.IsPlayerMapEvent && mapEvent.Winner == mapEvent.GetMapEventSide(mapEvent.PlayerSide))
			{
				if (mapEvent.GetMapEventSide(mapEvent.DefeatedSide).Parties.Any((MapEventParty x) => x.Party.MobileParty != null && x.Party.MobileParty.Army != null) && mapEvent.GetMapEventSide(mapEvent.PlayerSide).Parties.Count == 1)
				{
					string text = "DefeatedArmyWhileAloneCount";
					int num = this._cachedSuccessfulBattlesAgainstArmyAloneCount + 1;
					this._cachedSuccessfulBattlesAgainstArmyAloneCount = num;
					this.SetStatInternal(text, num);
				}
			}
		}

		// Token: 0x060003F6 RID: 1014 RVA: 0x00018468 File Offset: 0x00016668
		private void ProgressDailyTribute()
		{
			IFaction mapFaction = Clan.PlayerClan.MapFaction;
			float num = 1f;
			int num2 = 0;
			if (Clan.PlayerClan.Kingdom != null)
			{
				num = AchievementsCampaignBehavior.CalculateTributeShareFactor(Clan.PlayerClan);
			}
			foreach (StanceLink stanceLink in mapFaction.Stances)
			{
				if (stanceLink.IsNeutral && stanceLink.GetDailyTributePaid(mapFaction) < 0)
				{
					int num3 = (int)((float)stanceLink.GetDailyTributePaid(mapFaction) * num);
					num2 += num3;
				}
			}
			this.SetStatInternal("MaxDailyTributeGain", MathF.Abs(num2));
		}

		// Token: 0x060003F7 RID: 1015 RVA: 0x00018514 File Offset: 0x00016714
		private static float CalculateTributeShareFactor(Clan clan)
		{
			Kingdom kingdom = clan.Kingdom;
			int num = kingdom.Fiefs.Sum(delegate(Town x)
			{
				if (!x.IsCastle)
				{
					return 3;
				}
				return 1;
			}) + 1 + kingdom.Clans.Count;
			return (float)(clan.Fiefs.Sum(delegate(Town x)
			{
				if (!x.IsCastle)
				{
					return 3;
				}
				return 1;
			}) + ((clan == kingdom.RulingClan) ? 1 : 0) + 1) / (float)num;
		}

		// Token: 0x060003F8 RID: 1016 RVA: 0x000185A0 File Offset: 0x000167A0
		private void ProgressDailyIncome()
		{
			int num = (int)Campaign.Current.Models.ClanFinanceModel.CalculateClanIncome(Clan.PlayerClan, false, false, false).ResultNumber;
			if (num > this._cachedMaxDailyIncome)
			{
				this.SetStatInternal("MaxDailyIncome", num);
				this._cachedMaxDailyIncome = num;
			}
		}

		// Token: 0x060003F9 RID: 1017 RVA: 0x000185EF File Offset: 0x000167EF
		private void ProgressTotalTradeProfit(int profit)
		{
			this._cachedTotalTradeProfit += profit;
			this.SetStatInternal("TotalTradeProfit", this._cachedTotalTradeProfit);
		}

		// Token: 0x060003FA RID: 1018 RVA: 0x00018610 File Offset: 0x00016810
		private void CheckProjectsInSettlement(Town town)
		{
			if (town.OwnerClan == Clan.PlayerClan)
			{
				foreach (Settlement settlement in Clan.PlayerClan.Settlements.Where((Settlement x) => x.IsFortification))
				{
					bool flag = true;
					foreach (Building building in settlement.Town.Buildings)
					{
						if (building.CurrentLevel != 3 && !building.BuildingType.IsDefaultProject)
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						this.SetStatInternal("CompletedAllProjects", 1);
					}
				}
			}
		}

		// Token: 0x060003FB RID: 1019 RVA: 0x000186F8 File Offset: 0x000168F8
		private void ProgressHighestTierSwordCrafted(ItemObject itemObject, Crafting.OverrideData overrideData)
		{
			WeaponComponentData primaryWeapon = itemObject.WeaponComponent.PrimaryWeapon;
			if (primaryWeapon.WeaponClass == 2 || primaryWeapon.WeaponClass == 3)
			{
				this.SetStatInternal("HighestTierSwordCrafted", itemObject.Tier + 1);
			}
		}

		// Token: 0x060003FC RID: 1020 RVA: 0x00018736 File Offset: 0x00016936
		private void ProgressAssembledDragonBanner()
		{
			if (StoryModeManager.Current.MainStoryLine.FirstPhase != null && StoryModeManager.Current.MainStoryLine.FirstPhase.AllPiecesCollected)
			{
				this.SetStatInternal("AssembledDragonBanner", 1);
			}
		}

		// Token: 0x060003FD RID: 1021 RVA: 0x0001876C File Offset: 0x0001696C
		private void ProgressImperialBarbarianVictory(QuestBase quest, QuestBase.QuestCompleteDetails detail)
		{
			if (quest.IsSpecialQuest && quest.GetType() == typeof(DefeatTheConspiracyQuestBehavior.DefeatTheConspiracyQuest))
			{
				if (StoryModeManager.Current.MainStoryLine.MainStoryLineSide == MainStoryLineSide.CreateAntiImperialKingdom || StoryModeManager.Current.MainStoryLine.MainStoryLineSide == MainStoryLineSide.SupportAntiImperialKingdom)
				{
					this.SetStatInternal("BarbarianVictory", 1);
					return;
				}
				this.SetStatInternal("ImperialVictory", 1);
			}
		}

		// Token: 0x060003FE RID: 1022 RVA: 0x000187D8 File Offset: 0x000169D8
		private void CheckDefeatedSuperiorForce(MapEvent mapEvent)
		{
			if (mapEvent.IsPlayerMapEvent && mapEvent.Winner == mapEvent.GetMapEventSide(mapEvent.PlayerSide))
			{
				int num = mapEvent.GetMapEventSide(mapEvent.DefeatedSide).Parties.Sum((MapEventParty x) => x.HealthyManCountAtStart);
				int num2 = mapEvent.GetMapEventSide(mapEvent.WinningSide).Parties.Sum((MapEventParty x) => x.HealthyManCountAtStart);
				if (num - num2 >= 500)
				{
					this.SetStatInternal("DefeatedSuperiorForce", 1);
				}
			}
		}

		// Token: 0x060003FF RID: 1023 RVA: 0x00018887 File Offset: 0x00016A87
		private void CheckTutorialFinished()
		{
			if (!StoryModeManager.Current.MainStoryLine.TutorialPhase.IsSkipped)
			{
				this.SetStatInternal("FinishedTutorial", 1);
			}
		}

		// Token: 0x06000400 RID: 1024 RVA: 0x000188AC File Offset: 0x00016AAC
		private void ProgressSuccessfulSiegeCount(MobileParty attackerParty, bool isWin)
		{
			if (attackerParty == MobileParty.MainParty && isWin)
			{
				string text = "SuccessfulSiegeCount";
				int num = this._cachedSuccessfulSiegeCount + 1;
				this._cachedSuccessfulSiegeCount = num;
				this.SetStatInternal(text, num);
			}
		}

		// Token: 0x06000401 RID: 1025 RVA: 0x000188E4 File Offset: 0x00016AE4
		private void ProgressCapturedATownAlone(MobileParty attackerParty, bool isWin)
		{
			if (attackerParty == MobileParty.MainParty && isWin && attackerParty.Army == null)
			{
				string text = "CapturedATownAloneCount";
				int num = this._cachedCapturedTownAloneCount + 1;
				this._cachedCapturedTownAloneCount = num;
				this.SetStatInternal(text, num);
			}
		}

		// Token: 0x06000402 RID: 1026 RVA: 0x00018924 File Offset: 0x00016B24
		private void ProgressRepelSiegeAssaultCount(Settlement siegeSettlement, bool isWin)
		{
			if (siegeSettlement.OwnerClan == Clan.PlayerClan && !isWin)
			{
				string text = "RepelledSiegeAssaultCount";
				int num = this._cachedRepelledSiegeAssaultCount + 1;
				this._cachedRepelledSiegeAssaultCount = num;
				this.SetStatInternal(text, num);
			}
		}

		// Token: 0x06000403 RID: 1027 RVA: 0x00018960 File Offset: 0x00016B60
		private void ProgressRepelSiegeAssaultCount(MapEvent mapEvent)
		{
			if (mapEvent.MapEventSettlement != null && mapEvent.MapEventSettlement.OwnerClan == Clan.PlayerClan && mapEvent.EventType == 5 && mapEvent.BattleState == null && PlayerEncounter.Battle != null && PlayerEncounter.CampaignBattleResult != null && PlayerEncounter.CampaignBattleResult.PlayerVictory)
			{
				string text = "RepelledSiegeAssaultCount";
				int num = this._cachedRepelledSiegeAssaultCount + 1;
				this._cachedRepelledSiegeAssaultCount = num;
				this.SetStatInternal(text, num);
			}
		}

		// Token: 0x06000404 RID: 1028 RVA: 0x000189D0 File Offset: 0x00016BD0
		private void ProgressTournamentRank(CharacterObject winner)
		{
			if (winner == CharacterObject.PlayerCharacter && Campaign.Current.TournamentManager.GetLeaderboard()[0].Key == Hero.MainHero)
			{
				this.SetStatInternal("LeaderOfTournament", 1);
			}
		}

		// Token: 0x06000405 RID: 1029 RVA: 0x00018A18 File Offset: 0x00016C18
		private void ProgressHeroSkillValue(Hero hero, SkillObject skill, int change = 1, bool shouldNotify = true)
		{
			if (hero == Hero.MainHero && this._cachedHighestSkillValue > -1)
			{
				int skillValue = hero.GetSkillValue(skill);
				if (skillValue > this._cachedHighestSkillValue)
				{
					this.SetStatInternal("HighestSkillValue", skillValue);
					this._cachedHighestSkillValue = skillValue;
				}
			}
		}

		// Token: 0x06000406 RID: 1030 RVA: 0x00018A5C File Offset: 0x00016C5C
		private void ProgressHideoutClearedCount()
		{
			string text = "ClearedHideoutCount";
			int num = this._cachedHideoutClearedCount + 1;
			this._cachedHideoutClearedCount = num;
			this.SetStatInternal(text, num);
		}

		// Token: 0x06000407 RID: 1031 RVA: 0x00018A85 File Offset: 0x00016C85
		private void CheckExecutedLordRelation(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail)
		{
			if (killer == Hero.MainHero && detail == 6 && (int)victim.GetRelationWithPlayer() <= -100)
			{
				this.SetStatInternal("ExecutedLordRelation100", 1);
			}
		}

		// Token: 0x06000408 RID: 1032 RVA: 0x00018AAC File Offset: 0x00016CAC
		private void ProgressKingOrQueenKilledInBattle(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail)
		{
			if (killer == Hero.MainHero && victim.IsFactionLeader && detail == 4)
			{
				string text = "KingOrQueenKilledInBattle";
				int num = this._cachedKingOrQueenKilledInBattle + 1;
				this._cachedKingOrQueenKilledInBattle = num;
				this.SetStatInternal(text, num);
			}
		}

		// Token: 0x06000409 RID: 1033 RVA: 0x00018AEC File Offset: 0x00016CEC
		private void ProgressTournamentWonCount(CharacterObject winner)
		{
			if (winner == CharacterObject.PlayerCharacter)
			{
				string text = "WonTournamentCount";
				int num = this._cachedWonTournamentCount + 1;
				this._cachedWonTournamentCount = num;
				this.SetStatInternal(text, num);
			}
		}

		// Token: 0x0600040A RID: 1034 RVA: 0x00018B1D File Offset: 0x00016D1D
		private void ProgressOwnedWorkshopCount(Workshop workshop, Hero oldOwner, WorkshopType oldType)
		{
			if (workshop.Owner == Hero.MainHero)
			{
				this.ProgressHasOwnedCaravanAndWorkshop();
			}
		}

		// Token: 0x0600040B RID: 1035 RVA: 0x00018B32 File Offset: 0x00016D32
		private void ProgressOwnedCaravanCount(MobileParty party)
		{
			if (party.IsCaravan && party.MapFaction == Hero.MainHero.MapFaction)
			{
				this.ProgressHasOwnedCaravanAndWorkshop();
			}
		}

		// Token: 0x0600040C RID: 1036 RVA: 0x00018B54 File Offset: 0x00016D54
		private void ProgressHasOwnedCaravanAndWorkshop()
		{
			if (Hero.MainHero.OwnedWorkshops.Count > 0 && Hero.MainHero.OwnedCaravans.Count > 0)
			{
				this.SetStatInternal("HasOwnedCaravanAndWorkshop", 1);
			}
		}

		// Token: 0x0600040D RID: 1037 RVA: 0x00018B88 File Offset: 0x00016D88
		private void ProgressOwnedFortificationCount()
		{
			int num;
			if (Hero.MainHero.MapFaction.IsKingdomFaction && Hero.MainHero.IsFactionLeader)
			{
				num = Hero.MainHero.MapFaction.Fiefs.Count;
			}
			else
			{
				num = Hero.MainHero.Clan.Fiefs.Count;
			}
			this.SetStatInternal("OwnedFortificationCount", num);
		}

		// Token: 0x0600040E RID: 1038 RVA: 0x00018BEC File Offset: 0x00016DEC
		private void ProgressCreatedKingdomCount(Kingdom kingdom)
		{
			if (kingdom.Leader == Hero.MainHero)
			{
				string text = "CreatedKingdomCount";
				int num = this._cachedCreatedKingdomCount + 1;
				this._cachedCreatedKingdomCount = num;
				this.SetStatInternal(text, num);
			}
		}

		// Token: 0x0600040F RID: 1039 RVA: 0x00018C22 File Offset: 0x00016E22
		private void ProgressClanTier(Clan clan, bool shouldNotify)
		{
			if (clan == Clan.PlayerClan && clan.Tier == 6)
			{
				this.SetStatInternal("ReachedClanTierSix", 1);
			}
		}

		// Token: 0x06000410 RID: 1040 RVA: 0x00018C44 File Offset: 0x00016E44
		private bool CheckIfModulesAreDefault()
		{
			bool flag = Campaign.Current.PreviouslyUsedModules.All((string x) => x.Equals("Native", StringComparison.OrdinalIgnoreCase) || x.Equals("SandBoxCore", StringComparison.OrdinalIgnoreCase) || x.Equals("CustomBattle", StringComparison.OrdinalIgnoreCase) || x.Equals("SandBox", StringComparison.OrdinalIgnoreCase) || x.Equals("Multiplayer", StringComparison.OrdinalIgnoreCase) || x.Equals("BirthAndDeath", StringComparison.OrdinalIgnoreCase) || x.Equals("StoryMode", StringComparison.OrdinalIgnoreCase));
			if (!flag)
			{
				Debug.Print("Achievements are disabled! !CheckIfModulesAreDefault:", 0, 0, 17592186044416UL);
				foreach (string text in Campaign.Current.PreviouslyUsedModules)
				{
					Debug.Print(text, 0, 0, 17592186044416UL);
				}
			}
			return flag;
		}

		// Token: 0x06000411 RID: 1041 RVA: 0x00018CEC File Offset: 0x00016EEC
		private void DeactivateAchievements(bool showMessage = true)
		{
			this._deactivateAchievements = true;
			CampaignEventDispatcher.Instance.RemoveListeners(this);
			if (showMessage)
			{
				MBInformationManager.AddQuickInformation(new TextObject("{=Z9mcDuDi}Achievements are disabled!", null), 0, null, "");
			}
		}

		// Token: 0x06000412 RID: 1042 RVA: 0x00018D1A File Offset: 0x00016F1A
		private void SetStatInternal(string statId, int value)
		{
			if (!this._deactivateAchievements)
			{
				AchievementManager.SetStat(statId, value);
			}
		}

		// Token: 0x04000183 RID: 387
		private const float SettlementCountStoredInIntegerSet = 30f;

		// Token: 0x04000184 RID: 388
		private const string CreatedKingdomCountStatID = "CreatedKingdomCount";

		// Token: 0x04000185 RID: 389
		private const string ClearedHideoutCountStatID = "ClearedHideoutCount";

		// Token: 0x04000186 RID: 390
		private const string RepelledSiegeAssaultStatID = "RepelledSiegeAssaultCount";

		// Token: 0x04000187 RID: 391
		private const string KingOrQueenKilledInBattleStatID = "KingOrQueenKilledInBattle";

		// Token: 0x04000188 RID: 392
		private const string SuccessfulSiegeCountStatID = "SuccessfulSiegeCount";

		// Token: 0x04000189 RID: 393
		private const string WonTournamentCountStatID = "WonTournamentCount";

		// Token: 0x0400018A RID: 394
		private const string HighestTierSwordCraftedStatID = "HighestTierSwordCrafted";

		// Token: 0x0400018B RID: 395
		private const string SuccessfulBattlesAgainstArmyCountStatID = "SuccessfulBattlesAgainstArmyCount";

		// Token: 0x0400018C RID: 396
		private const string DefeatedArmyWhileAloneCountStatID = "DefeatedArmyWhileAloneCount";

		// Token: 0x0400018D RID: 397
		private const string TotalTradeProfitStatID = "TotalTradeProfit";

		// Token: 0x0400018E RID: 398
		private const string MaxDailyTributeGainStatID = "MaxDailyTributeGain";

		// Token: 0x0400018F RID: 399
		private const string MaxDailyIncomeStatID = "MaxDailyIncome";

		// Token: 0x04000190 RID: 400
		private const string CapturedATownAloneCountStatID = "CapturedATownAloneCount";

		// Token: 0x04000191 RID: 401
		private const string DefeatedTroopCountStatID = "DefeatedTroopCount";

		// Token: 0x04000192 RID: 402
		private const string FarthestHeadStatID = "FarthestHeadShot";

		// Token: 0x04000193 RID: 403
		private const string ButtersInInventoryStatID = "ButtersInInventoryCount";

		// Token: 0x04000194 RID: 404
		private const string ReachedClanTierSixStatID = "ReachedClanTierSix";

		// Token: 0x04000195 RID: 405
		private const string OwnedFortificationCountStatID = "OwnedFortificationCount";

		// Token: 0x04000196 RID: 406
		private const string HasOwnedCaravanAndWorkshopStatID = "HasOwnedCaravanAndWorkshop";

		// Token: 0x04000197 RID: 407
		private const string ExecutedLordWithMinus100RelationStatID = "ExecutedLordRelation100";

		// Token: 0x04000198 RID: 408
		private const string HighestSkillValueStatID = "HighestSkillValue";

		// Token: 0x04000199 RID: 409
		private const string LeaderOfTournamentStatID = "LeaderOfTournament";

		// Token: 0x0400019A RID: 410
		private const string FinishedTutorialStatID = "FinishedTutorial";

		// Token: 0x0400019B RID: 411
		private const string DefeatedSuperiorForceStatID = "DefeatedSuperiorForce";

		// Token: 0x0400019C RID: 412
		private const string BarbarianVictoryStatID = "BarbarianVictory";

		// Token: 0x0400019D RID: 413
		private const string ImperialVictoryStatID = "ImperialVictory";

		// Token: 0x0400019E RID: 414
		private const string AssembledDragonBannerStatID = "AssembledDragonBanner";

		// Token: 0x0400019F RID: 415
		private const string CompletedAllProjectsStatID = "CompletedAllProjects";

		// Token: 0x040001A0 RID: 416
		private const string ClansUnderPlayerKingdomCountStatID = "ClansUnderPlayerKingdomCount";

		// Token: 0x040001A1 RID: 417
		private const string HearthBreakerStatID = "Hearthbreaker";

		// Token: 0x040001A2 RID: 418
		private const string ProposedAndWonAPolicyStatID = "ProposedAndWonAPolicy";

		// Token: 0x040001A3 RID: 419
		private const string BestServedColdStatID = "BestServedCold";

		// Token: 0x040001A4 RID: 420
		private const string DefeatedRadagosInDUelStatID = "RadagosDefeatedInDuel";

		// Token: 0x040001A5 RID: 421
		private const string GreatGrannyStatID = "GreatGranny";

		// Token: 0x040001A6 RID: 422
		private const string NumberOfChildrenStatID = "NumberOfChildrenBorn";

		// Token: 0x040001A7 RID: 423
		private const string UndercoverStatID = "CompletedAnIssueInHostileTown";

		// Token: 0x040001A8 RID: 424
		private const string EnteredEverySettlemenStatID = "EnteredEverySettlement";

		// Token: 0x040001A9 RID: 425
		private bool _deactivateAchievements;

		// Token: 0x040001AA RID: 426
		private int _cachedCreatedKingdomCount;

		// Token: 0x040001AB RID: 427
		private int _cachedHideoutClearedCount;

		// Token: 0x040001AC RID: 428
		private int _cachedHighestSkillValue = -1;

		// Token: 0x040001AD RID: 429
		private int _cachedRepelledSiegeAssaultCount;

		// Token: 0x040001AE RID: 430
		private int _cachedCapturedTownAloneCount;

		// Token: 0x040001AF RID: 431
		private int _cachedKingOrQueenKilledInBattle;

		// Token: 0x040001B0 RID: 432
		private int _cachedSuccessfulSiegeCount;

		// Token: 0x040001B1 RID: 433
		private int _cachedWonTournamentCount;

		// Token: 0x040001B2 RID: 434
		private int _cachedSuccessfulBattlesAgainstArmyCount;

		// Token: 0x040001B3 RID: 435
		private int _cachedSuccessfulBattlesAgainstArmyAloneCount;

		// Token: 0x040001B4 RID: 436
		private int _cachedTotalTradeProfit;

		// Token: 0x040001B5 RID: 437
		private int _cachedMaxDailyIncome;

		// Token: 0x040001B6 RID: 438
		private int _cachedDefeatedTroopCount;

		// Token: 0x040001B7 RID: 439
		private int _cachedFarthestHeadShot;

		// Token: 0x040001B8 RID: 440
		private ItemObject _butter;

		// Token: 0x040001B9 RID: 441
		private List<Settlement> _orderedSettlementList = new List<Settlement>();

		// Token: 0x040001BA RID: 442
		private int[] _settlementIntegerSetList;

		// Token: 0x02000086 RID: 134
		private class AchievementMissionLogic : MissionLogic
		{
			// Token: 0x06000696 RID: 1686 RVA: 0x0002414C File Offset: 0x0002234C
			public AchievementMissionLogic(Action<Agent, Agent> onAgentRemoved, Action<Agent, WeaponComponentData, BoneBodyPartType, int> onAgentHitAction)
			{
				this.OnAgentRemovedAction = onAgentRemoved;
				this.OnAgentHitAction = onAgentHitAction;
			}

			// Token: 0x06000697 RID: 1687 RVA: 0x00024162 File Offset: 0x00022362
			public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
			{
				Action<Agent, Agent> onAgentRemovedAction = this.OnAgentRemovedAction;
				if (onAgentRemovedAction == null)
				{
					return;
				}
				onAgentRemovedAction(affectedAgent, affectorAgent);
			}

			// Token: 0x06000698 RID: 1688 RVA: 0x00024176 File Offset: 0x00022376
			public override void OnScoreHit(Agent affectedAgent, Agent affectorAgent, WeaponComponentData attackerWeapon, bool isBlocked, bool isSiegeEngineHit, in Blow blow, in AttackCollisionData collisionData, float damagedHp, float hitDistance, float shotDifficulty)
			{
				Action<Agent, WeaponComponentData, BoneBodyPartType, int> onAgentHitAction = this.OnAgentHitAction;
				if (onAgentHitAction == null)
				{
					return;
				}
				onAgentHitAction(affectorAgent, attackerWeapon, blow.VictimBodyPart, (int)hitDistance);
			}

			// Token: 0x0400027D RID: 637
			private Action<Agent, Agent> OnAgentRemovedAction;

			// Token: 0x0400027E RID: 638
			private Action<Agent, WeaponComponentData, BoneBodyPartType, int> OnAgentHitAction;
		}
	}
}
