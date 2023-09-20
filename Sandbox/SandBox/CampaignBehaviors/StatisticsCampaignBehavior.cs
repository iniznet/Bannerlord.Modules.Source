using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.MountAndBlade;

namespace SandBox.CampaignBehaviors
{
	// Token: 0x020000A3 RID: 163
	public class StatisticsCampaignBehavior : CampaignBehaviorBase, IStatisticsCampaignBehavior, ICampaignBehavior
	{
		// Token: 0x06000978 RID: 2424 RVA: 0x0004F444 File Offset: 0x0004D644
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<int>("_highestTournamentRank", ref this._highestTournamentRank);
			dataStore.SyncData<int>("_numberOfTournamentWins", ref this._numberOfTournamentWins);
			dataStore.SyncData<int>("_numberOfChildrenBorn", ref this._numberOfChildrenBorn);
			dataStore.SyncData<int>("_numberOfPrisonersRecruited", ref this._numberOfPrisonersRecruited);
			dataStore.SyncData<int>("_numberOfTroopsRecruited", ref this._numberOfTroopsRecruited);
			dataStore.SyncData<int>("_numberOfClansDefected", ref this._numberOfClansDefected);
			dataStore.SyncData<int>("_numberOfIssuesSolved", ref this._numberOfIssuesSolved);
			dataStore.SyncData<int>("_totalInfluenceEarned", ref this._totalInfluenceEarned);
			dataStore.SyncData<int>("_totalCrimeRatingGained", ref this._totalCrimeRatingGained);
			dataStore.SyncData<ulong>("_totalTimePlayedInSeconds", ref this._totalTimePlayedInSeconds);
			dataStore.SyncData<int>("_numberOfbattlesWon", ref this._numberOfbattlesWon);
			dataStore.SyncData<int>("_numberOfbattlesLost", ref this._numberOfbattlesLost);
			dataStore.SyncData<int>("_largestBattleWonAsLeader", ref this._largestBattleWonAsLeader);
			dataStore.SyncData<int>("_largestArmyFormedByPlayer", ref this._largestArmyFormedByPlayer);
			dataStore.SyncData<int>("_numberOfEnemyClansDestroyed", ref this._numberOfEnemyClansDestroyed);
			dataStore.SyncData<int>("_numberOfHeroesKilledInBattle", ref this._numberOfHeroesKilledInBattle);
			dataStore.SyncData<int>("_numberOfTroopsKnockedOrKilledAsParty", ref this._numberOfTroopsKnockedOrKilledAsParty);
			dataStore.SyncData<int>("_numberOfTroopsKnockedOrKilledByPlayer", ref this._numberOfTroopsKnockedOrKilledByPlayer);
			dataStore.SyncData<int>("_numberOfHeroPrisonersTaken", ref this._numberOfHeroPrisonersTaken);
			dataStore.SyncData<int>("_numberOfTroopPrisonersTaken", ref this._numberOfTroopPrisonersTaken);
			dataStore.SyncData<int>("_numberOfTownsCaptured", ref this._numberOfTownsCaptured);
			dataStore.SyncData<int>("_numberOfHideoutsCleared", ref this._numberOfHideoutsCleared);
			dataStore.SyncData<int>("_numberOfCastlesCaptured", ref this._numberOfCastlesCaptured);
			dataStore.SyncData<int>("_numberOfVillagesRaided", ref this._numberOfVillagesRaided);
			dataStore.SyncData<CampaignTime>("_timeSpentAsPrisoner", ref this._timeSpentAsPrisoner);
			dataStore.SyncData<ulong>("_totalDenarsEarned", ref this._totalDenarsEarned);
			dataStore.SyncData<ulong>("_denarsEarnedFromCaravans", ref this._denarsEarnedFromCaravans);
			dataStore.SyncData<ulong>("_denarsEarnedFromWorkshops", ref this._denarsEarnedFromWorkshops);
			dataStore.SyncData<ulong>("_denarsEarnedFromRansoms", ref this._denarsEarnedFromRansoms);
			dataStore.SyncData<ulong>("_denarsEarnedFromTaxes", ref this._denarsEarnedFromTaxes);
			dataStore.SyncData<ulong>("_denarsEarnedFromTributes", ref this._denarsEarnedFromTributes);
			dataStore.SyncData<ulong>("_denarsPaidAsTributes", ref this._denarsPaidAsTributes);
			dataStore.SyncData<int>("_numberOfCraftingPartsUnlocked", ref this._numberOfCraftingPartsUnlocked);
			dataStore.SyncData<int>("_numberOfWeaponsCrafted", ref this._numberOfWeaponsCrafted);
			dataStore.SyncData<int>("_numberOfCraftingOrdersCompleted", ref this._numberOfCraftingOrdersCompleted);
			dataStore.SyncData<ValueTuple<string, int>>("_mostExpensiveItemCrafted", ref this._mostExpensiveItemCrafted);
			dataStore.SyncData<int>("_numberOfCompanionsHired", ref this._numberOfCompanionsHired);
			dataStore.SyncData<Dictionary<Hero, ValueTuple<int, int>>>("_companionData", ref this._companionData);
			dataStore.SyncData<int>("_lastPlayerBattleSize", ref this._lastPlayerBattleSize);
		}

		// Token: 0x06000979 RID: 2425 RVA: 0x0004F710 File Offset: 0x0004D910
		public override void RegisterEvents()
		{
			CampaignEvents.HeroCreated.AddNonSerializedListener(this, new Action<Hero, bool>(this.OnHeroCreated));
			CampaignEvents.OnIssueUpdatedEvent.AddNonSerializedListener(this, new Action<IssueBase, IssueBase.IssueUpdateDetails, Hero>(this.OnIssueUpdated));
			CampaignEvents.TournamentFinished.AddNonSerializedListener(this, new Action<CharacterObject, MBReadOnlyList<CharacterObject>, Town, ItemObject>(this.OnTournamentFinished));
			CampaignEvents.OnClanInfluenceChangedEvent.AddNonSerializedListener(this, new Action<Clan, float>(this.OnClanInfluenceChanged));
			CampaignEvents.OnAfterSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnAfterSessionLaunched));
			CampaignEvents.CrimeRatingChanged.AddNonSerializedListener(this, new Action<IFaction, float>(this.OnCrimeRatingChanged));
			CampaignEvents.OnMainPartyPrisonerRecruitedEvent.AddNonSerializedListener(this, new Action<FlattenedTroopRoster>(this.OnMainPartyPrisonerRecruited));
			CampaignEvents.OnUnitRecruitedEvent.AddNonSerializedListener(this, new Action<CharacterObject, int>(this.OnUnitRecruited));
			CampaignEvents.OnBeforeSaveEvent.AddNonSerializedListener(this, new Action(this.OnBeforeSave));
			CampaignEvents.CraftingPartUnlockedEvent.AddNonSerializedListener(this, new Action<CraftingPiece>(this.OnCraftingPartUnlocked));
			CampaignEvents.OnNewItemCraftedEvent.AddNonSerializedListener(this, new Action<ItemObject, Crafting.OverrideData, bool>(this.OnNewItemCrafted));
			CampaignEvents.NewCompanionAdded.AddNonSerializedListener(this, new Action<Hero>(this.OnNewCompanionAdded));
			CampaignEvents.HeroOrPartyTradedGold.AddNonSerializedListener(this, new Action<ValueTuple<Hero, PartyBase>, ValueTuple<Hero, PartyBase>, ValueTuple<int, string>, bool>(this.OnHeroOrPartyTradedGold));
			CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.OnMapEventEnd));
			CampaignEvents.OnClanDestroyedEvent.AddNonSerializedListener(this, new Action<Clan>(this.OnClanDestroyed));
			CampaignEvents.PartyAttachedAnotherParty.AddNonSerializedListener(this, new Action<MobileParty>(this.OnPartyAttachedAnotherParty));
			CampaignEvents.ArmyCreated.AddNonSerializedListener(this, new Action<Army>(this.OnArmyCreated));
			CampaignEvents.HeroPrisonerTaken.AddNonSerializedListener(this, new Action<PartyBase, Hero>(this.OnHeroPrisonerTaken));
			CampaignEvents.OnPrisonerTakenEvent.AddNonSerializedListener(this, new Action<FlattenedTroopRoster>(this.OnPrisonersTaken));
			CampaignEvents.RaidCompletedEvent.AddNonSerializedListener(this, new Action<BattleSideEnum, RaidEventComponent>(this.OnRaidCompleted));
			CampaignEvents.OnHideoutBattleCompletedEvent.AddNonSerializedListener(this, new Action<BattleSideEnum, MapEvent>(this.OnHideoutBattleCompleted));
			CampaignEvents.MapEventStarted.AddNonSerializedListener(this, new Action<MapEvent, PartyBase, PartyBase>(this.OnMapEventStarted));
			CampaignEvents.HeroPrisonerReleased.AddNonSerializedListener(this, new Action<Hero, PartyBase, IFaction, EndCaptivityDetail>(this.OnHeroPrisonerReleased));
			CampaignEvents.OnPlayerPartyKnockedOrKilledTroopEvent.AddNonSerializedListener(this, new Action<CharacterObject>(this.OnPlayerPartyKnockedOrKilledTroop));
			CampaignEvents.OnMissionStartedEvent.AddNonSerializedListener(this, new Action<IMission>(this.OnMissionStarted));
			CampaignEvents.OnPlayerEarnedGoldFromAssetEvent.AddNonSerializedListener(this, new Action<DefaultClanFinanceModel.AssetIncomeType, int>(this.OnPlayerEarnedGoldFromAsset));
			CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.OnHeroKilled));
		}

		// Token: 0x0600097A RID: 2426 RVA: 0x0004F98A File Offset: 0x0004DB8A
		private void OnBeforeSave()
		{
			this.UpdateTotalTimePlayedInSeconds();
		}

		// Token: 0x0600097B RID: 2427 RVA: 0x0004F992 File Offset: 0x0004DB92
		private void OnAfterSessionLaunched(CampaignGameStarter starter)
		{
			this._lastGameplayTimeCheck = DateTime.Now;
			if (this._highestTournamentRank == 0)
			{
				this._highestTournamentRank = Campaign.Current.TournamentManager.GetLeaderBoardRank(Hero.MainHero);
			}
		}

		// Token: 0x0600097C RID: 2428 RVA: 0x0004F9C1 File Offset: 0x0004DBC1
		public void OnDefectionPersuasionSucess()
		{
			this._numberOfClansDefected++;
		}

		// Token: 0x0600097D RID: 2429 RVA: 0x0004F9D1 File Offset: 0x0004DBD1
		private void OnUnitRecruited(CharacterObject character, int amount)
		{
			this._numberOfTroopsRecruited += amount;
		}

		// Token: 0x0600097E RID: 2430 RVA: 0x0004F9E1 File Offset: 0x0004DBE1
		private void OnMainPartyPrisonerRecruited(FlattenedTroopRoster flattenedTroopRoster)
		{
			this._numberOfPrisonersRecruited += LinQuick.CountQ<FlattenedTroopRosterElement>(flattenedTroopRoster);
		}

		// Token: 0x0600097F RID: 2431 RVA: 0x0004F9F6 File Offset: 0x0004DBF6
		private void OnCrimeRatingChanged(IFaction kingdom, float deltaCrimeAmount)
		{
			if (deltaCrimeAmount > 0f)
			{
				this._totalCrimeRatingGained += (int)deltaCrimeAmount;
			}
		}

		// Token: 0x06000980 RID: 2432 RVA: 0x0004FA0F File Offset: 0x0004DC0F
		private void OnClanInfluenceChanged(Clan clan, float change)
		{
			if (change > 0f && clan == Clan.PlayerClan)
			{
				this._totalInfluenceEarned += (int)change;
			}
		}

		// Token: 0x06000981 RID: 2433 RVA: 0x0004FA30 File Offset: 0x0004DC30
		private void OnTournamentFinished(CharacterObject winner, MBReadOnlyList<CharacterObject> participants, Town town, ItemObject prize)
		{
			if (winner.HeroObject == Hero.MainHero)
			{
				this._numberOfTournamentWins++;
				int leaderBoardRank = Campaign.Current.TournamentManager.GetLeaderBoardRank(Hero.MainHero);
				if (leaderBoardRank < this._highestTournamentRank)
				{
					this._highestTournamentRank = leaderBoardRank;
				}
			}
		}

		// Token: 0x06000982 RID: 2434 RVA: 0x0004FA80 File Offset: 0x0004DC80
		private void OnIssueUpdated(IssueBase issue, IssueBase.IssueUpdateDetails details, Hero issueSolver = null)
		{
			if (details == 5 || details == 3 || details == 6)
			{
				this._numberOfIssuesSolved++;
				if (issueSolver != null && issueSolver.IsPlayerCompanion)
				{
					if (this._companionData.ContainsKey(issueSolver))
					{
						this._companionData[issueSolver] = new ValueTuple<int, int>(this._companionData[issueSolver].Item1 + 1, this._companionData[issueSolver].Item2);
						return;
					}
					this._companionData.Add(issueSolver, new ValueTuple<int, int>(1, 0));
				}
			}
		}

		// Token: 0x06000983 RID: 2435 RVA: 0x0004FB09 File Offset: 0x0004DD09
		private void OnHeroCreated(Hero hero, bool isBornNaturally = false)
		{
			if (hero.Mother == Hero.MainHero || hero.Father == Hero.MainHero)
			{
				this._numberOfChildrenBorn++;
			}
		}

		// Token: 0x06000984 RID: 2436 RVA: 0x0004FB33 File Offset: 0x0004DD33
		private void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification = true)
		{
			if (killer != null && killer.PartyBelongedTo == MobileParty.MainParty && detail == 4)
			{
				this._numberOfHeroesKilledInBattle++;
			}
		}

		// Token: 0x06000985 RID: 2437 RVA: 0x0004FB58 File Offset: 0x0004DD58
		private void OnMissionStarted(IMission mission)
		{
			StatisticsCampaignBehavior.StatisticsMissionLogic statisticsMissionLogic = new StatisticsCampaignBehavior.StatisticsMissionLogic();
			Mission.Current.AddMissionBehavior(statisticsMissionLogic);
		}

		// Token: 0x06000986 RID: 2438 RVA: 0x0004FB78 File Offset: 0x0004DD78
		private void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent)
		{
			if (affectorAgent != null)
			{
				if (affectorAgent == Agent.Main)
				{
					this._numberOfTroopsKnockedOrKilledByPlayer++;
				}
				else if (affectorAgent.IsPlayerTroop)
				{
					this._numberOfTroopsKnockedOrKilledAsParty++;
				}
				else if (affectorAgent.IsHero)
				{
					Hero heroObject = (affectorAgent.Character as CharacterObject).HeroObject;
					if (heroObject.IsPlayerCompanion)
					{
						if (this._companionData.ContainsKey(heroObject))
						{
							this._companionData[heroObject] = new ValueTuple<int, int>(this._companionData[heroObject].Item1, this._companionData[heroObject].Item2 + 1);
						}
						else
						{
							this._companionData.Add(heroObject, new ValueTuple<int, int>(0, 1));
						}
					}
				}
				if (affectedAgent.IsHero && affectedAgent.State == 4)
				{
					this._numberOfHeroesKilledInBattle++;
				}
			}
		}

		// Token: 0x06000987 RID: 2439 RVA: 0x0004FC56 File Offset: 0x0004DE56
		private void OnPlayerPartyKnockedOrKilledTroop(CharacterObject troop)
		{
			this._numberOfTroopsKnockedOrKilledAsParty++;
		}

		// Token: 0x06000988 RID: 2440 RVA: 0x0004FC66 File Offset: 0x0004DE66
		private void OnHeroPrisonerReleased(Hero prisoner, PartyBase party, IFaction capturerFaction, EndCaptivityDetail detail)
		{
			if (prisoner == Hero.MainHero)
			{
				this._timeSpentAsPrisoner += CampaignTime.Now - PlayerCaptivity.CaptivityStartTime;
			}
		}

		// Token: 0x06000989 RID: 2441 RVA: 0x0004FC90 File Offset: 0x0004DE90
		private void OnMapEventStarted(MapEvent mapEvent, PartyBase attackerParty, PartyBase defenderParty)
		{
			if (mapEvent.IsPlayerMapEvent)
			{
				this._lastPlayerBattleSize = mapEvent.AttackerSide.TroopCount + mapEvent.DefenderSide.TroopCount;
			}
		}

		// Token: 0x0600098A RID: 2442 RVA: 0x0004FCB7 File Offset: 0x0004DEB7
		private void OnHideoutBattleCompleted(BattleSideEnum winnerSide, MapEvent mapEvent)
		{
			if (mapEvent.PlayerSide == winnerSide)
			{
				this._numberOfHideoutsCleared++;
			}
		}

		// Token: 0x0600098B RID: 2443 RVA: 0x0004FCD0 File Offset: 0x0004DED0
		private void OnRaidCompleted(BattleSideEnum winnerSide, RaidEventComponent raidEvent)
		{
			if (raidEvent.MapEvent.PlayerSide == winnerSide)
			{
				this._numberOfVillagesRaided++;
			}
		}

		// Token: 0x0600098C RID: 2444 RVA: 0x0004FCEE File Offset: 0x0004DEEE
		private void OnPrisonersTaken(FlattenedTroopRoster troopRoster)
		{
			this._numberOfTroopPrisonersTaken += LinQuick.CountQ<FlattenedTroopRosterElement>(troopRoster);
		}

		// Token: 0x0600098D RID: 2445 RVA: 0x0004FD03 File Offset: 0x0004DF03
		private void OnHeroPrisonerTaken(PartyBase capturer, Hero prisoner)
		{
			if (capturer == PartyBase.MainParty)
			{
				this._numberOfHeroPrisonersTaken++;
			}
		}

		// Token: 0x0600098E RID: 2446 RVA: 0x0004FD1B File Offset: 0x0004DF1B
		private void OnArmyCreated(Army army)
		{
			if (army.LeaderParty == MobileParty.MainParty && this._largestArmyFormedByPlayer < army.TotalManCount)
			{
				this._largestArmyFormedByPlayer = army.TotalManCount;
			}
		}

		// Token: 0x0600098F RID: 2447 RVA: 0x0004FD44 File Offset: 0x0004DF44
		private void OnPartyAttachedAnotherParty(MobileParty mobileParty)
		{
			if (mobileParty.Army == MobileParty.MainParty.Army && MobileParty.MainParty.Army.LeaderParty == MobileParty.MainParty && this._largestArmyFormedByPlayer < MobileParty.MainParty.Army.TotalManCount)
			{
				this._largestArmyFormedByPlayer = MobileParty.MainParty.Army.TotalManCount;
			}
		}

		// Token: 0x06000990 RID: 2448 RVA: 0x0004FDA5 File Offset: 0x0004DFA5
		private void OnClanDestroyed(Clan clan)
		{
			if (clan.IsAtWarWith(Clan.PlayerClan))
			{
				this._numberOfEnemyClansDestroyed++;
			}
		}

		// Token: 0x06000991 RID: 2449 RVA: 0x0004FDC4 File Offset: 0x0004DFC4
		private void OnMapEventEnd(MapEvent mapEvent)
		{
			if (mapEvent.IsPlayerMapEvent)
			{
				if (mapEvent.WinningSide == mapEvent.PlayerSide)
				{
					this._numberOfbattlesWon++;
					if (mapEvent.IsSiegeAssault && !mapEvent.IsPlayerSergeant() && mapEvent.MapEventSettlement != null)
					{
						if (mapEvent.MapEventSettlement.IsTown)
						{
							this._numberOfTownsCaptured++;
						}
						else if (mapEvent.MapEventSettlement.IsCastle)
						{
							this._numberOfCastlesCaptured++;
						}
					}
					if (this._largestBattleWonAsLeader < this._lastPlayerBattleSize && !mapEvent.IsPlayerSergeant())
					{
						this._largestBattleWonAsLeader = this._lastPlayerBattleSize;
						return;
					}
				}
				else if (mapEvent.HasWinner)
				{
					this._numberOfbattlesLost++;
				}
			}
		}

		// Token: 0x06000992 RID: 2450 RVA: 0x0004FE84 File Offset: 0x0004E084
		private void OnHeroOrPartyTradedGold(ValueTuple<Hero, PartyBase> giver, ValueTuple<Hero, PartyBase> recipient, ValueTuple<int, string> goldAmount, bool showNotification)
		{
			if (recipient.Item1 == Hero.MainHero || recipient.Item2 == PartyBase.MainParty)
			{
				this._totalDenarsEarned += (ulong)((long)goldAmount.Item1);
			}
		}

		// Token: 0x06000993 RID: 2451 RVA: 0x0004FEB4 File Offset: 0x0004E0B4
		public void OnPlayerAcceptedRansomOffer(int ransomPrice)
		{
			this._denarsEarnedFromRansoms += (ulong)((long)ransomPrice);
		}

		// Token: 0x06000994 RID: 2452 RVA: 0x0004FEC8 File Offset: 0x0004E0C8
		private void OnPlayerEarnedGoldFromAsset(DefaultClanFinanceModel.AssetIncomeType assetType, int amount)
		{
			switch (assetType)
			{
			case 0:
				this._denarsEarnedFromWorkshops += (ulong)((long)amount);
				return;
			case 1:
				this._denarsEarnedFromCaravans += (ulong)((long)amount);
				return;
			case 2:
				this._denarsEarnedFromTaxes += (ulong)((long)amount);
				return;
			case 3:
				this._denarsEarnedFromTributes += (ulong)((long)amount);
				return;
			case 4:
				this._denarsPaidAsTributes += (ulong)((long)amount);
				return;
			default:
				return;
			}
		}

		// Token: 0x06000995 RID: 2453 RVA: 0x0004FF3F File Offset: 0x0004E13F
		private void OnNewCompanionAdded(Hero hero)
		{
			this._numberOfCompanionsHired++;
		}

		// Token: 0x06000996 RID: 2454 RVA: 0x0004FF50 File Offset: 0x0004E150
		private void OnNewItemCrafted(ItemObject itemObject, Crafting.OverrideData overrideData, bool isCraftingOrderItem)
		{
			this._numberOfWeaponsCrafted++;
			if (isCraftingOrderItem)
			{
				this._numberOfCraftingOrdersCompleted++;
			}
			if (this._mostExpensiveItemCrafted.Item2 == 0 || this._mostExpensiveItemCrafted.Item2 < itemObject.Value)
			{
				this._mostExpensiveItemCrafted.Item1 = itemObject.Name.ToString();
				this._mostExpensiveItemCrafted.Item2 = itemObject.Value;
			}
		}

		// Token: 0x06000997 RID: 2455 RVA: 0x0004FFC3 File Offset: 0x0004E1C3
		private void OnCraftingPartUnlocked(CraftingPiece craftingPiece)
		{
			this._numberOfCraftingPartsUnlocked++;
		}

		// Token: 0x06000998 RID: 2456 RVA: 0x0004FFD4 File Offset: 0x0004E1D4
		[return: TupleElementNames(new string[] { "name", "value" })]
		public ValueTuple<string, int> GetCompanionWithMostKills()
		{
			if (Extensions.IsEmpty<KeyValuePair<Hero, ValueTuple<int, int>>>(this._companionData))
			{
				return new ValueTuple<string, int>(null, 0);
			}
			KeyValuePair<Hero, ValueTuple<int, int>> keyValuePair = Extensions.MaxBy<KeyValuePair<Hero, ValueTuple<int, int>>, int>(this._companionData, (KeyValuePair<Hero, ValueTuple<int, int>> kvp) => kvp.Value.Item2);
			return new ValueTuple<string, int>(keyValuePair.Key.Name.ToString(), keyValuePair.Value.Item2);
		}

		// Token: 0x06000999 RID: 2457 RVA: 0x00050044 File Offset: 0x0004E244
		[return: TupleElementNames(new string[] { "name", "value" })]
		public ValueTuple<string, int> GetCompanionWithMostIssuesSolved()
		{
			if (Extensions.IsEmpty<KeyValuePair<Hero, ValueTuple<int, int>>>(this._companionData))
			{
				return new ValueTuple<string, int>(null, 0);
			}
			KeyValuePair<Hero, ValueTuple<int, int>> keyValuePair = Extensions.MaxBy<KeyValuePair<Hero, ValueTuple<int, int>>, int>(this._companionData, (KeyValuePair<Hero, ValueTuple<int, int>> kvp) => kvp.Value.Item1);
			return new ValueTuple<string, int>(keyValuePair.Key.Name.ToString(), keyValuePair.Value.Item1);
		}

		// Token: 0x0600099A RID: 2458 RVA: 0x000500B3 File Offset: 0x0004E2B3
		public int GetHighestTournamentRank()
		{
			return this._highestTournamentRank;
		}

		// Token: 0x0600099B RID: 2459 RVA: 0x000500BB File Offset: 0x0004E2BB
		public int GetNumberOfTournamentWins()
		{
			return this._numberOfTournamentWins;
		}

		// Token: 0x0600099C RID: 2460 RVA: 0x000500C3 File Offset: 0x0004E2C3
		public int GetNumberOfChildrenBorn()
		{
			return this._numberOfChildrenBorn;
		}

		// Token: 0x0600099D RID: 2461 RVA: 0x000500CB File Offset: 0x0004E2CB
		public int GetNumberOfPrisonersRecruited()
		{
			return this._numberOfPrisonersRecruited;
		}

		// Token: 0x0600099E RID: 2462 RVA: 0x000500D3 File Offset: 0x0004E2D3
		public int GetNumberOfTroopsRecruited()
		{
			return this._numberOfTroopsRecruited;
		}

		// Token: 0x0600099F RID: 2463 RVA: 0x000500DB File Offset: 0x0004E2DB
		public int GetNumberOfClansDefected()
		{
			return this._numberOfClansDefected;
		}

		// Token: 0x060009A0 RID: 2464 RVA: 0x000500E3 File Offset: 0x0004E2E3
		public int GetNumberOfIssuesSolved()
		{
			return this._numberOfIssuesSolved;
		}

		// Token: 0x060009A1 RID: 2465 RVA: 0x000500EB File Offset: 0x0004E2EB
		public int GetTotalInfluenceEarned()
		{
			return this._totalInfluenceEarned;
		}

		// Token: 0x060009A2 RID: 2466 RVA: 0x000500F3 File Offset: 0x0004E2F3
		public int GetTotalCrimeRatingGained()
		{
			return this._totalCrimeRatingGained;
		}

		// Token: 0x060009A3 RID: 2467 RVA: 0x000500FB File Offset: 0x0004E2FB
		public int GetNumberOfBattlesWon()
		{
			return this._numberOfbattlesWon;
		}

		// Token: 0x060009A4 RID: 2468 RVA: 0x00050103 File Offset: 0x0004E303
		public int GetNumberOfBattlesLost()
		{
			return this._numberOfbattlesLost;
		}

		// Token: 0x060009A5 RID: 2469 RVA: 0x0005010B File Offset: 0x0004E30B
		public int GetLargestBattleWonAsLeader()
		{
			return this._largestBattleWonAsLeader;
		}

		// Token: 0x060009A6 RID: 2470 RVA: 0x00050113 File Offset: 0x0004E313
		public int GetLargestArmyFormedByPlayer()
		{
			return this._largestArmyFormedByPlayer;
		}

		// Token: 0x060009A7 RID: 2471 RVA: 0x0005011B File Offset: 0x0004E31B
		public int GetNumberOfEnemyClansDestroyed()
		{
			return this._numberOfEnemyClansDestroyed;
		}

		// Token: 0x060009A8 RID: 2472 RVA: 0x00050123 File Offset: 0x0004E323
		public int GetNumberOfHeroesKilledInBattle()
		{
			return this._numberOfHeroesKilledInBattle;
		}

		// Token: 0x060009A9 RID: 2473 RVA: 0x0005012B File Offset: 0x0004E32B
		public int GetNumberOfTroopsKnockedOrKilledAsParty()
		{
			return this._numberOfTroopsKnockedOrKilledAsParty;
		}

		// Token: 0x060009AA RID: 2474 RVA: 0x00050133 File Offset: 0x0004E333
		public int GetNumberOfTroopsKnockedOrKilledByPlayer()
		{
			return this._numberOfTroopsKnockedOrKilledByPlayer;
		}

		// Token: 0x060009AB RID: 2475 RVA: 0x0005013B File Offset: 0x0004E33B
		public int GetNumberOfHeroPrisonersTaken()
		{
			return this._numberOfHeroPrisonersTaken;
		}

		// Token: 0x060009AC RID: 2476 RVA: 0x00050143 File Offset: 0x0004E343
		public int GetNumberOfTroopPrisonersTaken()
		{
			return this._numberOfTroopPrisonersTaken;
		}

		// Token: 0x060009AD RID: 2477 RVA: 0x0005014B File Offset: 0x0004E34B
		public int GetNumberOfTownsCaptured()
		{
			return this._numberOfTownsCaptured;
		}

		// Token: 0x060009AE RID: 2478 RVA: 0x00050153 File Offset: 0x0004E353
		public int GetNumberOfHideoutsCleared()
		{
			return this._numberOfHideoutsCleared;
		}

		// Token: 0x060009AF RID: 2479 RVA: 0x0005015B File Offset: 0x0004E35B
		public int GetNumberOfCastlesCaptured()
		{
			return this._numberOfCastlesCaptured;
		}

		// Token: 0x060009B0 RID: 2480 RVA: 0x00050163 File Offset: 0x0004E363
		public int GetNumberOfVillagesRaided()
		{
			return this._numberOfVillagesRaided;
		}

		// Token: 0x060009B1 RID: 2481 RVA: 0x0005016B File Offset: 0x0004E36B
		public int GetNumberOfCraftingPartsUnlocked()
		{
			return this._numberOfCraftingPartsUnlocked;
		}

		// Token: 0x060009B2 RID: 2482 RVA: 0x00050173 File Offset: 0x0004E373
		public int GetNumberOfWeaponsCrafted()
		{
			return this._numberOfWeaponsCrafted;
		}

		// Token: 0x060009B3 RID: 2483 RVA: 0x0005017B File Offset: 0x0004E37B
		public int GetNumberOfCraftingOrdersCompleted()
		{
			return this._numberOfCraftingOrdersCompleted;
		}

		// Token: 0x060009B4 RID: 2484 RVA: 0x00050183 File Offset: 0x0004E383
		public int GetNumberOfCompanionsHired()
		{
			return this._numberOfCompanionsHired;
		}

		// Token: 0x060009B5 RID: 2485 RVA: 0x0005018B File Offset: 0x0004E38B
		public CampaignTime GetTimeSpentAsPrisoner()
		{
			return this._timeSpentAsPrisoner;
		}

		// Token: 0x060009B6 RID: 2486 RVA: 0x00050193 File Offset: 0x0004E393
		public ulong GetTotalTimePlayedInSeconds()
		{
			this.UpdateTotalTimePlayedInSeconds();
			return this._totalTimePlayedInSeconds;
		}

		// Token: 0x060009B7 RID: 2487 RVA: 0x000501A1 File Offset: 0x0004E3A1
		public ulong GetTotalDenarsEarned()
		{
			return this._totalDenarsEarned;
		}

		// Token: 0x060009B8 RID: 2488 RVA: 0x000501A9 File Offset: 0x0004E3A9
		public ulong GetDenarsEarnedFromCaravans()
		{
			return this._denarsEarnedFromCaravans;
		}

		// Token: 0x060009B9 RID: 2489 RVA: 0x000501B1 File Offset: 0x0004E3B1
		public ulong GetDenarsEarnedFromWorkshops()
		{
			return this._denarsEarnedFromWorkshops;
		}

		// Token: 0x060009BA RID: 2490 RVA: 0x000501B9 File Offset: 0x0004E3B9
		public ulong GetDenarsEarnedFromRansoms()
		{
			return this._denarsEarnedFromRansoms;
		}

		// Token: 0x060009BB RID: 2491 RVA: 0x000501C1 File Offset: 0x0004E3C1
		public ulong GetDenarsEarnedFromTaxes()
		{
			return this._denarsEarnedFromTaxes;
		}

		// Token: 0x060009BC RID: 2492 RVA: 0x000501C9 File Offset: 0x0004E3C9
		public ulong GetDenarsEarnedFromTributes()
		{
			return this._denarsEarnedFromTributes;
		}

		// Token: 0x060009BD RID: 2493 RVA: 0x000501D1 File Offset: 0x0004E3D1
		public ulong GetDenarsPaidAsTributes()
		{
			return this._denarsPaidAsTributes;
		}

		// Token: 0x060009BE RID: 2494 RVA: 0x000501D9 File Offset: 0x0004E3D9
		public CampaignTime GetTotalTimePlayed()
		{
			return CampaignTime.Now - Campaign.Current.CampaignStartTime;
		}

		// Token: 0x060009BF RID: 2495 RVA: 0x000501EF File Offset: 0x0004E3EF
		public ValueTuple<string, int> GetMostExpensiveItemCrafted()
		{
			return this._mostExpensiveItemCrafted;
		}

		// Token: 0x060009C0 RID: 2496 RVA: 0x000501F8 File Offset: 0x0004E3F8
		[CommandLineFunctionality.CommandLineArgumentFunction("print_gameplay_statistics", "campaign")]
		public static string PrintGameplayStatistics(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"statistics.print_gameplay_statistics\".";
			}
			IStatisticsCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<IStatisticsCampaignBehavior>();
			string text = "";
			text += "---------------------------GENERAL---------------------------\n";
			text = string.Concat(new object[]
			{
				text,
				"Played Time in Campaign Time(Days): ",
				campaignBehavior.GetTotalTimePlayed().ToDays,
				"\n"
			});
			text = string.Concat(new object[]
			{
				text,
				"Played Time in Real Time: ",
				campaignBehavior.GetTotalTimePlayedInSeconds(),
				"\n"
			});
			text = string.Concat(new object[]
			{
				text,
				"Number of children born: ",
				campaignBehavior.GetNumberOfChildrenBorn(),
				"\n"
			});
			text = string.Concat(new object[]
			{
				text,
				"Total influence earned: ",
				campaignBehavior.GetTotalInfluenceEarned(),
				"\n"
			});
			text = string.Concat(new object[]
			{
				text,
				"Number of issues solved: ",
				campaignBehavior.GetNumberOfIssuesSolved(),
				"\n"
			});
			text = string.Concat(new object[]
			{
				text,
				"Number of tournaments won: ",
				campaignBehavior.GetNumberOfTournamentWins(),
				"\n"
			});
			text = string.Concat(new object[]
			{
				text,
				"Best tournament rank: ",
				campaignBehavior.GetHighestTournamentRank(),
				"\n"
			});
			text = string.Concat(new object[]
			{
				text,
				"Number of prisoners recruited: ",
				campaignBehavior.GetNumberOfPrisonersRecruited(),
				"\n"
			});
			text = string.Concat(new object[]
			{
				text,
				"Number of troops recruited: ",
				campaignBehavior.GetNumberOfTroopsRecruited(),
				"\n"
			});
			text = string.Concat(new object[]
			{
				text,
				"Number of enemy clans defected: ",
				campaignBehavior.GetNumberOfClansDefected(),
				"\n"
			});
			text = string.Concat(new object[]
			{
				text,
				"Total crime rating gained: ",
				campaignBehavior.GetTotalCrimeRatingGained(),
				"\n"
			});
			text += "---------------------------BATTLE---------------------------\n";
			text = string.Concat(new object[]
			{
				text,
				"Battles Won / Lost: ",
				campaignBehavior.GetNumberOfBattlesWon(),
				" / ",
				campaignBehavior.GetNumberOfBattlesLost(),
				"\n"
			});
			text = string.Concat(new object[]
			{
				text,
				"Largest battle won as the leader: ",
				campaignBehavior.GetLargestBattleWonAsLeader(),
				"\n"
			});
			text = string.Concat(new object[]
			{
				text,
				"Largest army formed by the player: ",
				campaignBehavior.GetLargestArmyFormedByPlayer(),
				"\n"
			});
			text = string.Concat(new object[]
			{
				text,
				"Number of enemy clans destroyed: ",
				campaignBehavior.GetNumberOfEnemyClansDestroyed(),
				"\n"
			});
			text = string.Concat(new object[]
			{
				text,
				"Heroes killed in battle: ",
				campaignBehavior.GetNumberOfHeroesKilledInBattle(),
				"\n"
			});
			text = string.Concat(new object[]
			{
				text,
				"Troops killed or knocked out in person: ",
				campaignBehavior.GetNumberOfTroopsKnockedOrKilledByPlayer(),
				"\n"
			});
			text = string.Concat(new object[]
			{
				text,
				"Troops killed or knocked out by player party: ",
				campaignBehavior.GetNumberOfTroopsKnockedOrKilledAsParty(),
				"\n"
			});
			text = string.Concat(new object[]
			{
				text,
				"Number of hero prisoners taken: ",
				campaignBehavior.GetNumberOfHeroPrisonersTaken(),
				"\n"
			});
			text = string.Concat(new object[]
			{
				text,
				"Number of troop prisoners taken: ",
				campaignBehavior.GetNumberOfTroopPrisonersTaken(),
				"\n"
			});
			text = string.Concat(new object[]
			{
				text,
				"Number of captured towns: ",
				campaignBehavior.GetNumberOfTownsCaptured(),
				"\n"
			});
			text = string.Concat(new object[]
			{
				text,
				"Number of captured castles: ",
				campaignBehavior.GetNumberOfCastlesCaptured(),
				"\n"
			});
			text = string.Concat(new object[]
			{
				text,
				"Number of cleared hideouts: ",
				campaignBehavior.GetNumberOfHideoutsCleared(),
				"\n"
			});
			text = string.Concat(new object[]
			{
				text,
				"Number of raided villages: ",
				campaignBehavior.GetNumberOfVillagesRaided(),
				"\n"
			});
			text = string.Concat(new object[]
			{
				text,
				"Number of days spent as prisoner: ",
				campaignBehavior.GetTimeSpentAsPrisoner().ToDays,
				"\n"
			});
			text += "---------------------------FINANCES---------------------------\n";
			text = string.Concat(new object[]
			{
				text,
				"Total denars earned: ",
				campaignBehavior.GetTotalDenarsEarned(),
				"\n"
			});
			text = string.Concat(new object[]
			{
				text,
				"Total denars earned from caravans: ",
				campaignBehavior.GetDenarsEarnedFromCaravans(),
				"\n"
			});
			text = string.Concat(new object[]
			{
				text,
				"Total denars earned from workshops: ",
				campaignBehavior.GetDenarsEarnedFromWorkshops(),
				"\n"
			});
			text = string.Concat(new object[]
			{
				text,
				"Total denars earned from ransoms: ",
				campaignBehavior.GetDenarsEarnedFromRansoms(),
				"\n"
			});
			text = string.Concat(new object[]
			{
				text,
				"Total denars earned from taxes: ",
				campaignBehavior.GetDenarsEarnedFromTaxes(),
				"\n"
			});
			text = string.Concat(new object[]
			{
				text,
				"Total denars earned from tributes: ",
				campaignBehavior.GetDenarsEarnedFromTributes(),
				"\n"
			});
			text = string.Concat(new object[]
			{
				text,
				"Total denars paid in tributes: ",
				campaignBehavior.GetDenarsPaidAsTributes(),
				"\n"
			});
			text += "---------------------------CRAFTING---------------------------\n";
			text = string.Concat(new object[]
			{
				text,
				"Number of weapons crafted: ",
				campaignBehavior.GetNumberOfWeaponsCrafted(),
				"\n"
			});
			text = string.Concat(new object[]
			{
				text,
				"Most expensive weapon crafted: ",
				campaignBehavior.GetMostExpensiveItemCrafted().Item1,
				" - ",
				campaignBehavior.GetMostExpensiveItemCrafted().Item2,
				"\n"
			});
			text = string.Concat(new object[]
			{
				text,
				"Numbere of crafting parts unlocked: ",
				campaignBehavior.GetNumberOfCraftingPartsUnlocked(),
				"\n"
			});
			text = string.Concat(new object[]
			{
				text,
				"Number of crafting orders completed: ",
				campaignBehavior.GetNumberOfCraftingOrdersCompleted(),
				"\n"
			});
			text += "---------------------------COMPANIONS---------------------------\n";
			text = string.Concat(new object[]
			{
				text,
				"Number of hired companions: ",
				campaignBehavior.GetNumberOfCompanionsHired(),
				"\n"
			});
			text = string.Concat(new object[]
			{
				text,
				"Companion with most issues solved: ",
				campaignBehavior.GetCompanionWithMostIssuesSolved().Item1,
				" - ",
				campaignBehavior.GetCompanionWithMostIssuesSolved().Item2,
				"\n"
			});
			return string.Concat(new object[]
			{
				text,
				"Companion with most kills: ",
				campaignBehavior.GetCompanionWithMostKills().Item1,
				" - ",
				campaignBehavior.GetCompanionWithMostKills().Item2,
				"\n"
			});
		}

		// Token: 0x060009C1 RID: 2497 RVA: 0x000509EC File Offset: 0x0004EBEC
		private void UpdateTotalTimePlayedInSeconds()
		{
			ulong num = (ulong)((long)(DateTime.Now - this._lastGameplayTimeCheck).Seconds);
			if (num > 0UL)
			{
				this._totalTimePlayedInSeconds += num;
				this._lastGameplayTimeCheck.AddSeconds(num);
			}
		}

		// Token: 0x04000337 RID: 823
		private int _highestTournamentRank;

		// Token: 0x04000338 RID: 824
		private int _numberOfTournamentWins;

		// Token: 0x04000339 RID: 825
		private int _numberOfChildrenBorn;

		// Token: 0x0400033A RID: 826
		private int _numberOfPrisonersRecruited;

		// Token: 0x0400033B RID: 827
		private int _numberOfTroopsRecruited;

		// Token: 0x0400033C RID: 828
		private int _numberOfClansDefected;

		// Token: 0x0400033D RID: 829
		private int _numberOfIssuesSolved;

		// Token: 0x0400033E RID: 830
		private int _totalInfluenceEarned;

		// Token: 0x0400033F RID: 831
		private int _totalCrimeRatingGained;

		// Token: 0x04000340 RID: 832
		private ulong _totalTimePlayedInSeconds;

		// Token: 0x04000341 RID: 833
		private int _numberOfbattlesWon;

		// Token: 0x04000342 RID: 834
		private int _numberOfbattlesLost;

		// Token: 0x04000343 RID: 835
		private int _largestBattleWonAsLeader;

		// Token: 0x04000344 RID: 836
		private int _largestArmyFormedByPlayer;

		// Token: 0x04000345 RID: 837
		private int _numberOfEnemyClansDestroyed;

		// Token: 0x04000346 RID: 838
		private int _numberOfHeroesKilledInBattle;

		// Token: 0x04000347 RID: 839
		private int _numberOfTroopsKnockedOrKilledAsParty;

		// Token: 0x04000348 RID: 840
		private int _numberOfTroopsKnockedOrKilledByPlayer;

		// Token: 0x04000349 RID: 841
		private int _numberOfHeroPrisonersTaken;

		// Token: 0x0400034A RID: 842
		private int _numberOfTroopPrisonersTaken;

		// Token: 0x0400034B RID: 843
		private int _numberOfTownsCaptured;

		// Token: 0x0400034C RID: 844
		private int _numberOfHideoutsCleared;

		// Token: 0x0400034D RID: 845
		private int _numberOfCastlesCaptured;

		// Token: 0x0400034E RID: 846
		private int _numberOfVillagesRaided;

		// Token: 0x0400034F RID: 847
		private CampaignTime _timeSpentAsPrisoner = CampaignTime.Zero;

		// Token: 0x04000350 RID: 848
		private ulong _totalDenarsEarned;

		// Token: 0x04000351 RID: 849
		private ulong _denarsEarnedFromCaravans;

		// Token: 0x04000352 RID: 850
		private ulong _denarsEarnedFromWorkshops;

		// Token: 0x04000353 RID: 851
		private ulong _denarsEarnedFromRansoms;

		// Token: 0x04000354 RID: 852
		private ulong _denarsEarnedFromTaxes;

		// Token: 0x04000355 RID: 853
		private ulong _denarsEarnedFromTributes;

		// Token: 0x04000356 RID: 854
		private ulong _denarsPaidAsTributes;

		// Token: 0x04000357 RID: 855
		private int _numberOfCraftingPartsUnlocked;

		// Token: 0x04000358 RID: 856
		private int _numberOfWeaponsCrafted;

		// Token: 0x04000359 RID: 857
		private int _numberOfCraftingOrdersCompleted;

		// Token: 0x0400035A RID: 858
		private ValueTuple<string, int> _mostExpensiveItemCrafted = new ValueTuple<string, int>(null, 0);

		// Token: 0x0400035B RID: 859
		private int _numberOfCompanionsHired;

		// Token: 0x0400035C RID: 860
		private Dictionary<Hero, ValueTuple<int, int>> _companionData = new Dictionary<Hero, ValueTuple<int, int>>();

		// Token: 0x0400035D RID: 861
		private int _lastPlayerBattleSize;

		// Token: 0x0400035E RID: 862
		private DateTime _lastGameplayTimeCheck;

		// Token: 0x02000195 RID: 405
		private class StatisticsMissionLogic : MissionLogic
		{
			// Token: 0x06001267 RID: 4711 RVA: 0x000745A1 File Offset: 0x000727A1
			public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
			{
				if (this.behavior != null)
				{
					this.behavior.OnAgentRemoved(affectedAgent, affectorAgent);
				}
			}

			// Token: 0x040007BC RID: 1980
			private readonly StatisticsCampaignBehavior behavior = Campaign.Current.CampaignBehaviorManager.GetBehavior<StatisticsCampaignBehavior>();
		}
	}
}
