using System;
using System.Collections.Generic;
using System.Linq;
using SandBox.CampaignBehaviors;
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
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace StoryMode.GameComponents.CampaignBehaviors
{
	public class AchievementsCampaignBehavior : CampaignBehaviorBase
	{
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<bool>("_deactivateAchievements", ref this._deactivateAchievements);
		}

		public override void RegisterEvents()
		{
			CampaignEvents.OnCharacterCreationIsOverEvent.AddNonSerializedListener(this, new Action(this.CacheHighestSkillValue));
			CampaignEvents.WorkshopOwnerChangedEvent.AddNonSerializedListener(this, new Action<Workshop, Hero>(this.ProgressOwnedWorkshopCount));
			CampaignEvents.MobilePartyCreated.AddNonSerializedListener(this, new Action<MobileParty>(this.ProgressOwnedCaravanCount));
			CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementOwnerChanged));
			CampaignEvents.KingdomCreatedEvent.AddNonSerializedListener(this, new Action<Kingdom>(this.ProgressCreatedKingdomCount));
			CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.OnHeroKilled));
			CampaignEvents.BeforeHeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.OnBeforeHeroKilled));
			CampaignEvents.ClanTierIncrease.AddNonSerializedListener(this, new Action<Clan, bool>(this.ProgressClanTier));
			CampaignEvents.OnHideoutBattleCompletedEvent.AddNonSerializedListener(this, new Action<BattleSideEnum, HideoutEventComponent>(this.OnHideoutBattleCompleted));
			CampaignEvents.HeroGainedSkill.AddNonSerializedListener(this, new Action<Hero, SkillObject, int, bool>(this.ProgressHeroSkillValue));
			CampaignEvents.PlayerInventoryExchangeEvent.AddNonSerializedListener(this, new Action<List<ValueTuple<ItemRosterElement, int>>, List<ValueTuple<ItemRosterElement, int>>, bool>(this.PlayerInventoryExchange));
			CampaignEvents.TournamentFinished.AddNonSerializedListener(this, new Action<CharacterObject, MBReadOnlyList<CharacterObject>, Town, ItemObject>(this.OnTournamentFinish));
			CampaignEvents.SiegeCompletedEvent.AddNonSerializedListener(this, new Action<Settlement, MobileParty, bool, MapEvent.BattleTypes>(this.OnSiegeCompleted));
			CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.OnMapEventEnded));
			CampaignEvents.OnQuestCompletedEvent.AddNonSerializedListener(this, new Action<QuestBase, QuestBase.QuestCompleteDetails>(this.OnQuestCompleted));
			CampaignEvents.OnBuildingLevelChangedEvent.AddNonSerializedListener(this, new Action<Town, Building, int>(this.OnBuildingLevelChanged));
			CampaignEvents.OnNewItemCraftedEvent.AddNonSerializedListener(this, new Action<ItemObject, ItemModifier, bool>(this.OnNewItemCrafted));
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
			CampaignEvents.OnConfigChangedEvent.AddNonSerializedListener(this, new Action(this.OnConfigChanged));
			StoryModeEvents.OnStoryModeTutorialEndedEvent.AddNonSerializedListener(this, new Action(this.CheckTutorialFinished));
			StoryModeEvents.OnBannerPieceCollectedEvent.AddNonSerializedListener(this, new Action(this.ProgressAssembledDragonBanner));
		}

		private void OnRulingClanChanged(Kingdom kingdom, Clan newRulingCLan)
		{
			this.ProgressOwnedFortificationCount();
		}

		private void OnIssueUpdated(IssueBase issueBase, IssueBase.IssueUpdateDetails detail, Hero issueSolver)
		{
			if (issueSolver == Hero.MainHero && !issueBase.IsSolvingWithAlternative && detail == 5 && issueBase.IssueOwner.MapFaction != null && issueBase.IssueOwner.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction))
			{
				this.SetStatInternal("CompletedAnIssueInHostileTown", 1);
			}
		}

		private void OnHideoutBattleCompleted(BattleSideEnum winnerSide, HideoutEventComponent hideoutEventComponent)
		{
			if (hideoutEventComponent.MapEvent.InvolvedParties.Contains(PartyBase.MainParty) && winnerSide == hideoutEventComponent.MapEvent.PlayerSide)
			{
				this.ProgressHideoutClearedCount();
			}
		}

		private void OnBeforeHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification = true)
		{
			this.ProgressKingOrQueenKilledInBattle(victim, killer, detail);
		}

		private void OnConfigChanged()
		{
			TextObject textObject;
			if (!this.CheckAchievementSystemActivity(out textObject))
			{
				this.DeactivateAchievements(textObject, true, false);
			}
		}

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

		private void OnGameLoadFinished()
		{
			TextObject textObject;
			if (this.CheckAchievementSystemActivity(out textObject))
			{
				this.CacheAndInitializeAchievementVariables();
				this.CacheHighestSkillValue();
				return;
			}
			this.DeactivateAchievements(textObject, true, false);
		}

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
				this.DeactivateAchievements(new TextObject("{=4wS8eYYe}Achievements are disabled temporarily for this session due to service disconnection.", null), true, true);
				Debug.Print("Achievements are disabled because current platform does not support achievements!", 0, 0, 17592186044416UL);
			}
		}

		private void OnNewGameCreatedPartialFollowUpEnd(CampaignGameStarter starter)
		{
			TextObject textObject;
			if (this.CheckAchievementSystemActivity(out textObject))
			{
				this.CacheAndInitializeAchievementVariables();
				return;
			}
			this.DeactivateAchievements(textObject, true, false);
		}

		private void OnDailyTick()
		{
			this.ProgressDailyTribute();
			this.ProgressDailyIncome();
		}

		private void OnClanDestroyed(Clan clan)
		{
			this.ProgressClansUnderKingdomCount();
		}

		private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification = true)
		{
			this.ProgressDailyIncome();
			this.ProgressClansUnderKingdomCount();
			this.ProgressOwnedFortificationCount();
		}

		private void OnNewItemCrafted(ItemObject itemObject, ItemModifier overriddenItemModifier, bool isCraftingOrderItem)
		{
			this.ProgressHighestTierSwordCrafted(itemObject);
		}

		private void OnBuildingLevelChanged(Town town, Building building, int levelChange)
		{
			this.ProgressDailyIncome();
			this.CheckProjectsInSettlement(town);
		}

		private void OnQuestCompleted(QuestBase quest, QuestBase.QuestCompleteDetails detail)
		{
			this.ProgressImperialBarbarianVictory(quest, detail);
		}

		private void OnTournamentFinish(CharacterObject winner, MBReadOnlyList<CharacterObject> participants, Town town, ItemObject prize)
		{
			this.ProgressTournamentWonCount(winner);
			this.ProgressTournamentRank(winner);
		}

		private void OnMapEventEnded(MapEvent mapEvent)
		{
			this.ProgressRepelSiegeAssaultCount(mapEvent);
			this.CheckDefeatedSuperiorForce(mapEvent);
			this.ProgressSuccessfulBattlesAgainstArmyCount(mapEvent);
			this.ProgressSuccessfulBattlesAgainstArmyAloneCount(mapEvent);
		}

		private void OnSiegeCompleted(Settlement siegeSettlement, MobileParty attackerParty, bool isWin, MapEvent.BattleTypes battleType)
		{
			this.ProgressRepelSiegeAssaultCount(siegeSettlement, isWin);
			this.ProgressSuccessfulSiegeCount(attackerParty, isWin);
			this.ProgressCapturedATownAlone(attackerParty, isWin);
		}

		private void PlayerInventoryExchange(List<ValueTuple<ItemRosterElement, int>> purchasedItems, List<ValueTuple<ItemRosterElement, int>> soldItems, bool isTrading)
		{
			if (this._butter != null)
			{
				int itemNumber = PartyBase.MainParty.ItemRoster.GetItemNumber(this._butter);
				if (itemNumber > 0)
				{
					this.SetStatInternal("ButtersInInventoryCount", itemNumber);
				}
			}
		}

		public bool CheckAchievementSystemActivity(out TextObject reason)
		{
			reason = TextObject.Empty;
			DumpIntegrityCampaignBehavior behavior = Campaign.Current.CampaignBehaviorManager.GetBehavior<DumpIntegrityCampaignBehavior>();
			return (!this._deactivateAchievements && behavior != null && DumpIntegrityCampaignBehavior.IsGameIntegrityAchieved(ref reason)) || MBDebug.IsTestMode();
		}

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

		private void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification = true)
		{
			this.CheckExecutedLordRelation(victim, killer, detail);
			this.CheckBestServedCold(victim, killer, detail);
		}

		private void OnSettlementOwnerChanged(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturerHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
		{
			this.ProgressDailyIncome();
			if (settlement.IsFortification)
			{
				this.ProgressOwnedFortificationCount();
			}
		}

		private void OnMissionStarted(IMission obj)
		{
			AchievementsCampaignBehavior.AchievementMissionLogic achievementMissionLogic = new AchievementsCampaignBehavior.AchievementMissionLogic(new Action<Agent, Agent>(this.OnAgentRemoved), new Action<Agent, WeaponComponentData, BoneBodyPartType, int>(this.OnAgentHit));
			Mission.Current.AddMissionBehavior(achievementMissionLogic);
		}

		private void OnAgentHit(Agent affectorAgent, WeaponComponentData attackerWeapon, BoneBodyPartType victimBoneBodyPartType, int hitDistance)
		{
			if (affectorAgent != null && affectorAgent == Agent.Main && attackerWeapon != null && !attackerWeapon.IsMeleeWeapon && victimBoneBodyPartType == null && hitDistance > this._cachedFarthestHeadShot)
			{
				this.SetStatInternal("FarthestHeadShot", hitDistance);
				this._cachedFarthestHeadShot = hitDistance;
			}
		}

		private void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent)
		{
			if (affectorAgent != null && affectorAgent == Agent.Main && affectedAgent.IsHuman)
			{
				string text = "DefeatedTroopCount";
				int num = this._cachedDefeatedTroopCount + 1;
				this._cachedDefeatedTroopCount = num;
				this.SetStatInternal(text, num);
			}
		}

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

		private void CheckGrandparent()
		{
			if (Hero.MainHero.Children.Any((Hero x) => x.Children.Any((Hero y) => y.Children.Any<Hero>())))
			{
				this.SetStatInternal("GreatGranny", 1);
			}
		}

		public void OnRadagosDuelWon()
		{
			this.SetStatInternal("RadagosDefeatedInDuel", 1);
		}

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

		private void CheckKingdomDecisionConcluded(KingdomDecision decision, DecisionOutcome chosenOutcome, bool isPlayerInvolved)
		{
			this.CheckProposedAndWonPolicy(decision, chosenOutcome);
			this.ProgressOwnedFortificationCount();
			this.ProgressClansUnderKingdomCount();
		}

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

		private void ProgressClansUnderKingdomCount()
		{
			if (Clan.PlayerClan.Kingdom != null && Clan.PlayerClan.Kingdom.Leader == Hero.MainHero)
			{
				this.SetStatInternal("ClansUnderPlayerKingdomCount", Clan.PlayerClan.Kingdom.Clans.Count);
			}
		}

		private void ProgressSuccessfulBattlesAgainstArmyCount(MapEvent mapEvent)
		{
			if (mapEvent.IsPlayerMapEvent && mapEvent.Winner == mapEvent.GetMapEventSide(mapEvent.PlayerSide))
			{
				if (mapEvent.GetMapEventSide(mapEvent.DefeatedSide).Parties.Any((MapEventParty x) => x.Party.MobileParty != null && x.Party.MobileParty.AttachedTo != null))
				{
					string text = "SuccessfulBattlesAgainstArmyCount";
					int num = this._cachedSuccessfulBattlesAgainstArmyCount + 1;
					this._cachedSuccessfulBattlesAgainstArmyCount = num;
					this.SetStatInternal(text, num);
				}
			}
		}

		private void ProgressSuccessfulBattlesAgainstArmyAloneCount(MapEvent mapEvent)
		{
			if (mapEvent.IsPlayerMapEvent && mapEvent.Winner == mapEvent.GetMapEventSide(mapEvent.PlayerSide))
			{
				if (mapEvent.GetMapEventSide(mapEvent.DefeatedSide).Parties.Any((MapEventParty x) => x.Party.MobileParty != null && x.Party.MobileParty.AttachedTo != null) && mapEvent.GetMapEventSide(mapEvent.PlayerSide).Parties.Count == 1)
				{
					string text = "DefeatedArmyWhileAloneCount";
					int num = this._cachedSuccessfulBattlesAgainstArmyAloneCount + 1;
					this._cachedSuccessfulBattlesAgainstArmyAloneCount = num;
					this.SetStatInternal(text, num);
				}
			}
		}

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

		private void ProgressDailyIncome()
		{
			int num = (int)Campaign.Current.Models.ClanFinanceModel.CalculateClanIncome(Clan.PlayerClan, false, false, false).ResultNumber;
			if (num > this._cachedMaxDailyIncome)
			{
				this.SetStatInternal("MaxDailyIncome", num);
				this._cachedMaxDailyIncome = num;
			}
		}

		private void ProgressTotalTradeProfit(int profit)
		{
			this._cachedTotalTradeProfit += profit;
			this.SetStatInternal("TotalTradeProfit", this._cachedTotalTradeProfit);
		}

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

		private void ProgressHighestTierSwordCrafted(ItemObject itemObject)
		{
			WeaponComponentData primaryWeapon = itemObject.WeaponComponent.PrimaryWeapon;
			if (primaryWeapon.WeaponClass == 2 || primaryWeapon.WeaponClass == 3)
			{
				this.SetStatInternal("HighestTierSwordCrafted", itemObject.Tier + 1);
			}
		}

		private void ProgressAssembledDragonBanner()
		{
			if (StoryModeManager.Current.MainStoryLine.FirstPhase != null && StoryModeManager.Current.MainStoryLine.FirstPhase.AllPiecesCollected)
			{
				this.SetStatInternal("AssembledDragonBanner", 1);
			}
		}

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

		private void CheckTutorialFinished()
		{
			if (!StoryModeManager.Current.MainStoryLine.TutorialPhase.IsSkipped)
			{
				this.SetStatInternal("FinishedTutorial", 1);
			}
		}

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

		private void ProgressTournamentRank(CharacterObject winner)
		{
			if (winner == CharacterObject.PlayerCharacter && Campaign.Current.TournamentManager.GetLeaderboard()[0].Key == Hero.MainHero)
			{
				this.SetStatInternal("LeaderOfTournament", 1);
			}
		}

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

		private void ProgressHideoutClearedCount()
		{
			string text = "ClearedHideoutCount";
			int num = this._cachedHideoutClearedCount + 1;
			this._cachedHideoutClearedCount = num;
			this.SetStatInternal(text, num);
		}

		private void CheckExecutedLordRelation(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail)
		{
			if (killer == Hero.MainHero && detail == 6 && (int)victim.GetRelationWithPlayer() <= -100)
			{
				this.SetStatInternal("ExecutedLordRelation100", 1);
			}
		}

		private void ProgressKingOrQueenKilledInBattle(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail)
		{
			if (killer == Hero.MainHero && victim.IsKingdomLeader && detail == 4)
			{
				string text = "KingOrQueenKilledInBattle";
				int num = this._cachedKingOrQueenKilledInBattle + 1;
				this._cachedKingOrQueenKilledInBattle = num;
				this.SetStatInternal(text, num);
			}
		}

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

		private void ProgressOwnedWorkshopCount(Workshop workshop, Hero oldOwner)
		{
			if (workshop.Owner == Hero.MainHero)
			{
				this.ProgressHasOwnedCaravanAndWorkshop();
			}
		}

		private void ProgressOwnedCaravanCount(MobileParty party)
		{
			if (party.IsCaravan && party.MapFaction == Hero.MainHero.MapFaction)
			{
				this.ProgressHasOwnedCaravanAndWorkshop();
			}
		}

		private void ProgressHasOwnedCaravanAndWorkshop()
		{
			if (Hero.MainHero.OwnedWorkshops.Count > 0 && Hero.MainHero.OwnedCaravans.Count > 0)
			{
				this.SetStatInternal("HasOwnedCaravanAndWorkshop", 1);
			}
		}

		private void ProgressOwnedFortificationCount()
		{
			int num;
			if (Hero.MainHero.IsKingdomLeader)
			{
				num = Hero.MainHero.MapFaction.Fiefs.Count;
			}
			else
			{
				num = Hero.MainHero.Clan.Fiefs.Count;
			}
			this.SetStatInternal("OwnedFortificationCount", num);
		}

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

		private void ProgressClanTier(Clan clan, bool shouldNotify)
		{
			if (clan == Clan.PlayerClan && clan.Tier == 6)
			{
				this.SetStatInternal("ReachedClanTierSix", 1);
			}
		}

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

		public void DeactivateAchievements(TextObject reason = null, bool showMessage = true, bool temporarily = false)
		{
			this._deactivateAchievements = !temporarily || this._deactivateAchievements;
			CampaignEventDispatcher.Instance.RemoveListeners(this);
			if (showMessage)
			{
				if (reason == null || reason == TextObject.Empty)
				{
					reason = new TextObject("{=Z9mcDuDi}Achievements are disabled!", null);
				}
				MBInformationManager.AddQuickInformation(reason, 4000, null, "");
			}
		}

		private void SetStatInternal(string statId, int value)
		{
			if (!this._deactivateAchievements)
			{
				AchievementManager.SetStat(statId, value);
			}
		}

		private const float SettlementCountStoredInIntegerSet = 30f;

		private const string CreatedKingdomCountStatID = "CreatedKingdomCount";

		private const string ClearedHideoutCountStatID = "ClearedHideoutCount";

		private const string RepelledSiegeAssaultStatID = "RepelledSiegeAssaultCount";

		private const string KingOrQueenKilledInBattleStatID = "KingOrQueenKilledInBattle";

		private const string SuccessfulSiegeCountStatID = "SuccessfulSiegeCount";

		private const string WonTournamentCountStatID = "WonTournamentCount";

		private const string HighestTierSwordCraftedStatID = "HighestTierSwordCrafted";

		private const string SuccessfulBattlesAgainstArmyCountStatID = "SuccessfulBattlesAgainstArmyCount";

		private const string DefeatedArmyWhileAloneCountStatID = "DefeatedArmyWhileAloneCount";

		private const string TotalTradeProfitStatID = "TotalTradeProfit";

		private const string MaxDailyTributeGainStatID = "MaxDailyTributeGain";

		private const string MaxDailyIncomeStatID = "MaxDailyIncome";

		private const string CapturedATownAloneCountStatID = "CapturedATownAloneCount";

		private const string DefeatedTroopCountStatID = "DefeatedTroopCount";

		private const string FarthestHeadStatID = "FarthestHeadShot";

		private const string ButtersInInventoryStatID = "ButtersInInventoryCount";

		private const string ReachedClanTierSixStatID = "ReachedClanTierSix";

		private const string OwnedFortificationCountStatID = "OwnedFortificationCount";

		private const string HasOwnedCaravanAndWorkshopStatID = "HasOwnedCaravanAndWorkshop";

		private const string ExecutedLordWithMinus100RelationStatID = "ExecutedLordRelation100";

		private const string HighestSkillValueStatID = "HighestSkillValue";

		private const string LeaderOfTournamentStatID = "LeaderOfTournament";

		private const string FinishedTutorialStatID = "FinishedTutorial";

		private const string DefeatedSuperiorForceStatID = "DefeatedSuperiorForce";

		private const string BarbarianVictoryStatID = "BarbarianVictory";

		private const string ImperialVictoryStatID = "ImperialVictory";

		private const string AssembledDragonBannerStatID = "AssembledDragonBanner";

		private const string CompletedAllProjectsStatID = "CompletedAllProjects";

		private const string ClansUnderPlayerKingdomCountStatID = "ClansUnderPlayerKingdomCount";

		private const string HearthBreakerStatID = "Hearthbreaker";

		private const string ProposedAndWonAPolicyStatID = "ProposedAndWonAPolicy";

		private const string BestServedColdStatID = "BestServedCold";

		private const string DefeatedRadagosInDUelStatID = "RadagosDefeatedInDuel";

		private const string GreatGrannyStatID = "GreatGranny";

		private const string NumberOfChildrenStatID = "NumberOfChildrenBorn";

		private const string UndercoverStatID = "CompletedAnIssueInHostileTown";

		private const string EnteredEverySettlemenStatID = "EnteredEverySettlement";

		private bool _deactivateAchievements;

		private int _cachedCreatedKingdomCount;

		private int _cachedHideoutClearedCount;

		private int _cachedHighestSkillValue = -1;

		private int _cachedRepelledSiegeAssaultCount;

		private int _cachedCapturedTownAloneCount;

		private int _cachedKingOrQueenKilledInBattle;

		private int _cachedSuccessfulSiegeCount;

		private int _cachedWonTournamentCount;

		private int _cachedSuccessfulBattlesAgainstArmyCount;

		private int _cachedSuccessfulBattlesAgainstArmyAloneCount;

		private int _cachedTotalTradeProfit;

		private int _cachedMaxDailyIncome;

		private int _cachedDefeatedTroopCount;

		private int _cachedFarthestHeadShot;

		private ItemObject _butter;

		private List<Settlement> _orderedSettlementList = new List<Settlement>();

		private int[] _settlementIntegerSetList;

		private class AchievementMissionLogic : MissionLogic
		{
			public AchievementMissionLogic(Action<Agent, Agent> onAgentRemoved, Action<Agent, WeaponComponentData, BoneBodyPartType, int> onAgentHitAction)
			{
				this.OnAgentRemovedAction = onAgentRemoved;
				this.OnAgentHitAction = onAgentHitAction;
			}

			public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
			{
				Action<Agent, Agent> onAgentRemovedAction = this.OnAgentRemovedAction;
				if (onAgentRemovedAction == null)
				{
					return;
				}
				onAgentRemovedAction(affectedAgent, affectorAgent);
			}

			public override void OnScoreHit(Agent affectedAgent, Agent affectorAgent, WeaponComponentData attackerWeapon, bool isBlocked, bool isSiegeEngineHit, in Blow blow, in AttackCollisionData collisionData, float damagedHp, float hitDistance, float shotDifficulty)
			{
				Action<Agent, WeaponComponentData, BoneBodyPartType, int> onAgentHitAction = this.OnAgentHitAction;
				if (onAgentHitAction == null)
				{
					return;
				}
				onAgentHitAction(affectorAgent, attackerWeapon, blow.VictimBodyPart, (int)hitDistance);
			}

			private Action<Agent, Agent> OnAgentRemovedAction;

			private Action<Agent, WeaponComponentData, BoneBodyPartType, int> OnAgentHitAction;
		}
	}
}
