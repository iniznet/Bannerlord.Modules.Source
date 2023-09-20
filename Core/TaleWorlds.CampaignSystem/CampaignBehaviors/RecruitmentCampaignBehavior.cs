using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public class RecruitmentCampaignBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
			CampaignEvents.LocationCharactersAreReadyToSpawnEvent.AddNonSerializedListener(this, new Action<Dictionary<string, int>>(this.LocationCharactersAreReadyToSpawn));
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
			CampaignEvents.DailyTickTownEvent.AddNonSerializedListener(this, new Action<Town>(this.DailyTickTown));
			CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, new Action<Settlement>(this.DailyTickSettlement));
			CampaignEvents.HourlyTickPartyEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.HourlyTickParty));
			CampaignEvents.MercenaryNumberChangedInTown.AddNonSerializedListener(this, new Action<Town, int, int>(this.OnMercenaryNumberChanged));
			CampaignEvents.MercenaryTroopChangedInTown.AddNonSerializedListener(this, new Action<Town, CharacterObject, CharacterObject>(this.OnMercenaryTroopChanged));
			CampaignEvents.OnNewGameCreatedPartialFollowUpEndEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnNewGameCreatedPartialFollowUpEnd));
			CampaignEvents.OnUnitRecruitedEvent.AddNonSerializedListener(this, new Action<CharacterObject, int>(this.OnUnitRecruited));
			CampaignEvents.OnTroopRecruitedEvent.AddNonSerializedListener(this, new Action<Hero, Settlement, Hero, CharacterObject, int>(this.OnTroopRecruited));
		}

		private void DailyTickSettlement(Settlement settlement)
		{
			this.UpdateVolunteersOfNotablesInSettlement(settlement);
		}

		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<CharacterObject>("_selectedTroop", ref this._selectedTroop);
			dataStore.SyncData<Dictionary<Town, RecruitmentCampaignBehavior.TownMercenaryData>>("_townMercenaryData", ref this._townMercenaryData);
		}

		private RecruitmentCampaignBehavior.TownMercenaryData GetMercenaryData(Town town)
		{
			RecruitmentCampaignBehavior.TownMercenaryData townMercenaryData;
			if (!this._townMercenaryData.TryGetValue(town, out townMercenaryData))
			{
				townMercenaryData = new RecruitmentCampaignBehavior.TownMercenaryData(town);
				this._townMercenaryData.Add(town, townMercenaryData);
			}
			return townMercenaryData;
		}

		private void OnNewGameCreatedPartialFollowUpEnd(CampaignGameStarter starter)
		{
			foreach (Town town in Town.AllTowns)
			{
				this.UpdateCurrentMercenaryTroopAndCount(town, true);
			}
			foreach (Settlement settlement in Settlement.All)
			{
				this.UpdateVolunteersOfNotablesInSettlement(settlement);
			}
		}

		private void OnTroopRecruited(Hero recruiter, Settlement settlement, Hero recruitmentSource, CharacterObject troop, int count)
		{
			if (recruiter != null && recruiter.PartyBelongedTo != null && recruiter.GetPerkValue(DefaultPerks.Leadership.FamousCommander))
			{
				recruiter.PartyBelongedTo.MemberRoster.AddXpToTroop((int)DefaultPerks.Leadership.FamousCommander.SecondaryBonus * count, troop);
			}
			SkillLevelingManager.OnTroopRecruited(recruiter, count, troop.Tier);
			if (recruiter != null && recruiter.PartyBelongedTo != null && troop.Occupation == Occupation.Bandit)
			{
				SkillLevelingManager.OnBanditsRecruited(recruiter.PartyBelongedTo, troop, count);
			}
		}

		private void OnUnitRecruited(CharacterObject troop, int count)
		{
			if (Hero.MainHero.GetPerkValue(DefaultPerks.Leadership.FamousCommander))
			{
				MobileParty.MainParty.MemberRoster.AddXpToTroop((int)DefaultPerks.Leadership.FamousCommander.SecondaryBonus * count, troop);
			}
			SkillLevelingManager.OnTroopRecruited(Hero.MainHero, count, troop.Tier);
			if (troop.Occupation == Occupation.Bandit)
			{
				SkillLevelingManager.OnBanditsRecruited(MobileParty.MainParty, troop, count);
			}
		}

		private void DailyTickTown(Town town)
		{
			this.UpdateCurrentMercenaryTroopAndCount(town, (int)CampaignTime.Now.ToDays % 2 == 0);
		}

		private void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
		{
			this.AddGameMenus(campaignGameStarter);
			this.AddDialogs(campaignGameStarter);
		}

		private void OnMercenaryNumberChanged(Town town, int oldNumber, int newNumber)
		{
			this.CheckIfMercenaryCharacterNeedsToRefresh(town.Owner.Settlement, this.GetMercenaryData(town).TroopType);
		}

		private void OnMercenaryTroopChanged(Town town, CharacterObject oldTroopType, CharacterObject newTroopType)
		{
			this.CheckIfMercenaryCharacterNeedsToRefresh(town.Owner.Settlement, oldTroopType);
		}

		private void UpdateVolunteersOfNotablesInSettlement(Settlement settlement)
		{
			if ((settlement.IsTown && !settlement.Town.InRebelliousState) || (settlement.IsVillage && !settlement.Village.Bound.Town.InRebelliousState))
			{
				foreach (Hero hero in settlement.Notables)
				{
					if (hero.CanHaveRecruits && hero.IsAlive)
					{
						bool flag = false;
						CharacterObject basicVolunteer = Campaign.Current.Models.VolunteerModel.GetBasicVolunteer(hero);
						for (int i = 0; i < 6; i++)
						{
							if (MBRandom.RandomFloat < Campaign.Current.Models.VolunteerModel.GetDailyVolunteerProductionProbability(hero, i, settlement))
							{
								CharacterObject characterObject = hero.VolunteerTypes[i];
								if (characterObject == null)
								{
									hero.VolunteerTypes[i] = basicVolunteer;
									flag = true;
								}
								else if (characterObject.UpgradeTargets.Length != 0 && characterObject.Tier < Campaign.Current.Models.VolunteerModel.MaxVolunteerTier)
								{
									float num = MathF.Log(hero.Power / (float)characterObject.Tier, 2f) * 0.01f;
									if (MBRandom.RandomFloat < num)
									{
										hero.VolunteerTypes[i] = characterObject.UpgradeTargets[MBRandom.RandomInt(characterObject.UpgradeTargets.Length)];
										flag = true;
									}
								}
							}
						}
						if (flag)
						{
							CharacterObject[] volunteerTypes = hero.VolunteerTypes;
							for (int j = 1; j < 6; j++)
							{
								CharacterObject characterObject2 = volunteerTypes[j];
								if (characterObject2 != null)
								{
									int num2 = 0;
									int num3 = j - 1;
									CharacterObject characterObject3 = volunteerTypes[num3];
									while (num3 >= 0 && (characterObject3 == null || (float)characterObject2.Level + (characterObject2.IsMounted ? 0.5f : 0f) < (float)characterObject3.Level + (characterObject3.IsMounted ? 0.5f : 0f)))
									{
										if (characterObject3 == null)
										{
											num3--;
											num2++;
											if (num3 >= 0)
											{
												characterObject3 = volunteerTypes[num3];
											}
										}
										else
										{
											volunteerTypes[num3 + 1 + num2] = characterObject3;
											num3--;
											num2 = 0;
											if (num3 >= 0)
											{
												characterObject3 = volunteerTypes[num3];
											}
										}
									}
									volunteerTypes[num3 + 1 + num2] = characterObject2;
								}
							}
						}
					}
				}
			}
		}

		public void HourlyTickParty(MobileParty mobileParty)
		{
			if ((mobileParty.IsCaravan || mobileParty.IsLordParty) && mobileParty.MapEvent == null && mobileParty != MobileParty.MainParty)
			{
				Settlement currentSettlementOfMobilePartyForAICalculation = MobilePartyHelper.GetCurrentSettlementOfMobilePartyForAICalculation(mobileParty);
				if (currentSettlementOfMobilePartyForAICalculation != null)
				{
					if ((currentSettlementOfMobilePartyForAICalculation.IsVillage && !currentSettlementOfMobilePartyForAICalculation.IsRaided && !currentSettlementOfMobilePartyForAICalculation.IsUnderRaid) || (currentSettlementOfMobilePartyForAICalculation.IsTown && !currentSettlementOfMobilePartyForAICalculation.IsUnderSiege))
					{
						this.CheckRecruiting(mobileParty, currentSettlementOfMobilePartyForAICalculation);
						return;
					}
				}
				else if (MBRandom.RandomFloat < 0.05f && mobileParty.LeaderHero != null && mobileParty.ActualClan != Clan.PlayerClan && !mobileParty.IsCaravan)
				{
					IFaction mapFaction = mobileParty.MapFaction;
					if (mapFaction != null && mapFaction.IsMinorFaction && MobileParty.MainParty.Position2D.DistanceSquared(mobileParty.Position2D) > (MobileParty.MainParty.SeeingRange + 5f) * (MobileParty.MainParty.SeeingRange + 5f))
					{
						int partySizeLimit = mobileParty.Party.PartySizeLimit;
						float num = (float)mobileParty.Party.NumberOfAllMembers / (float)partySizeLimit;
						float num2 = (((double)num < 0.2) ? 1000f : (((double)num < 0.3) ? 2000f : (((double)num < 0.4) ? 3000f : (((double)num < 0.55) ? 4000f : (((double)num < 0.7) ? 5000f : 7000f)))));
						float num3 = (((float)mobileParty.LeaderHero.Gold > num2) ? 1f : MathF.Sqrt((float)mobileParty.LeaderHero.Gold / num2));
						if (MBRandom.RandomFloat < (1f - num) * num3)
						{
							CharacterObject basicTroop = mobileParty.ActualClan.BasicTroop;
							int num4 = MBRandom.RandomInt(3, 8);
							if (num4 + mobileParty.Party.NumberOfAllMembers > partySizeLimit)
							{
								num4 = partySizeLimit - mobileParty.Party.NumberOfAllMembers;
							}
							int troopRecruitmentCost = Campaign.Current.Models.PartyWageModel.GetTroopRecruitmentCost(basicTroop, mobileParty.LeaderHero, false);
							if (num4 * troopRecruitmentCost > mobileParty.LeaderHero.Gold)
							{
								num4 = mobileParty.LeaderHero.Gold / troopRecruitmentCost;
							}
							if (num4 > 0)
							{
								this.GetRecruitVolunteerFromMap(mobileParty, basicTroop, num4);
							}
						}
					}
				}
			}
		}

		private void UpdateCurrentMercenaryTroopAndCount(Town town, bool forceUpdate = false)
		{
			RecruitmentCampaignBehavior.TownMercenaryData mercenaryData = this.GetMercenaryData(town);
			if (!forceUpdate && mercenaryData.HasAvailableMercenary(Occupation.NotAssigned))
			{
				int num = this.FindNumberOfMercenariesWillBeAdded(mercenaryData.TroopType, true);
				mercenaryData.ChangeMercenaryCount(num);
				return;
			}
			if (MBRandom.RandomFloat < Campaign.Current.Models.TavernMercenaryTroopsModel.RegularMercenariesSpawnChance)
			{
				CharacterObject randomElementInefficiently = town.Culture.BasicMercenaryTroops.GetRandomElementInefficiently<CharacterObject>();
				this._selectedTroop = null;
				float num2 = this.FindTotalMercenaryProbability(randomElementInefficiently, 1f);
				float num3 = MBRandom.RandomFloat * num2;
				this.FindRandomMercenaryTroop(randomElementInefficiently, 1f, num3);
				int num4 = this.FindNumberOfMercenariesWillBeAdded(this._selectedTroop, false);
				mercenaryData.ChangeMercenaryType(this._selectedTroop, num4);
				return;
			}
			CharacterObject caravanGuard = town.Culture.CaravanGuard;
			if (caravanGuard != null)
			{
				this._selectedTroop = null;
				float num5 = this.FindTotalMercenaryProbability(caravanGuard, 1f);
				float num6 = MBRandom.RandomFloat * num5;
				this.FindRandomMercenaryTroop(caravanGuard, 1f, num6);
				int num7 = this.FindNumberOfMercenariesWillBeAdded(this._selectedTroop, false);
				mercenaryData.ChangeMercenaryType(this._selectedTroop, num7);
			}
		}

		private float FindTotalMercenaryProbability(CharacterObject mercenaryTroop, float probabilityOfTroop)
		{
			float num = probabilityOfTroop;
			foreach (CharacterObject characterObject in mercenaryTroop.UpgradeTargets)
			{
				num += this.FindTotalMercenaryProbability(characterObject, probabilityOfTroop / 1.5f);
			}
			return num;
		}

		private float FindRandomMercenaryTroop(CharacterObject mercenaryTroop, float probabilityOfTroop, float randomValueRemaining)
		{
			randomValueRemaining -= probabilityOfTroop;
			if (randomValueRemaining <= 1E-05f && this._selectedTroop == null)
			{
				this._selectedTroop = mercenaryTroop;
				return 1f;
			}
			float num = probabilityOfTroop;
			foreach (CharacterObject characterObject in mercenaryTroop.UpgradeTargets)
			{
				float num2 = this.FindRandomMercenaryTroop(characterObject, probabilityOfTroop / 1.5f, randomValueRemaining);
				randomValueRemaining -= num2;
				num += num2;
			}
			return num;
		}

		private int FindNumberOfMercenariesWillBeAdded(CharacterObject character, bool dailyUpdate = false)
		{
			int tier = Campaign.Current.Models.CharacterStatsModel.GetTier(character);
			int maxCharacterTier = Campaign.Current.Models.CharacterStatsModel.MaxCharacterTier;
			int num = (maxCharacterTier - tier) * 2;
			int num2 = (maxCharacterTier - tier) * 5;
			float randomFloat = MBRandom.RandomFloat;
			float randomFloat2 = MBRandom.RandomFloat;
			return MBRandom.RoundRandomized(MBMath.ClampFloat((randomFloat * randomFloat2 * (float)(num2 - num) + (float)num) * (dailyUpdate ? 0.1f : 1f), 1f, (float)num2));
		}

		private void CheckIfMercenaryCharacterNeedsToRefresh(Settlement settlement, CharacterObject oldTroopType)
		{
			if (settlement.IsTown && settlement == Settlement.CurrentSettlement && PlayerEncounter.LocationEncounter != null && settlement.LocationComplex != null && (CampaignMission.Current == null || GameStateManager.Current.ActiveState != CampaignMission.Current.State))
			{
				if (oldTroopType != null)
				{
					Settlement.CurrentSettlement.LocationComplex.GetLocationWithId("tavern").RemoveAllCharacters((LocationCharacter x) => x.Character.Occupation == oldTroopType.Occupation);
				}
				this.AddMercenaryCharacterToTavern(settlement);
			}
		}

		private void AddMercenaryCharacterToTavern(Settlement settlement)
		{
			if (settlement.LocationComplex != null && settlement.IsTown && this.GetMercenaryData(settlement.Town).HasAvailableMercenary(Occupation.NotAssigned))
			{
				Location locationWithId = Settlement.CurrentSettlement.LocationComplex.GetLocationWithId("tavern");
				if (locationWithId != null)
				{
					locationWithId.AddLocationCharacters(new CreateLocationCharacterDelegate(this.CreateMercenary), settlement.Culture, LocationCharacter.CharacterRelations.Neutral, 1);
				}
			}
		}

		private void CheckRecruiting(MobileParty mobileParty, Settlement settlement)
		{
			if (settlement.IsTown && mobileParty.IsCaravan)
			{
				RecruitmentCampaignBehavior.TownMercenaryData mercenaryData = this.GetMercenaryData(settlement.Town);
				if (mercenaryData.HasAvailableMercenary(Occupation.CaravanGuard) || mercenaryData.HasAvailableMercenary(Occupation.Mercenary))
				{
					int partySizeLimit = mobileParty.Party.PartySizeLimit;
					if (mobileParty.Party.NumberOfAllMembers < partySizeLimit)
					{
						CharacterObject troopType = mercenaryData.TroopType;
						int troopRecruitmentCost = Campaign.Current.Models.PartyWageModel.GetTroopRecruitmentCost(troopType, mobileParty.LeaderHero, false);
						int num = (mobileParty.IsCaravan ? 2000 : 0);
						if (mobileParty.PartyTradeGold > troopRecruitmentCost + num)
						{
							bool flag = true;
							double num2 = 0.0;
							for (int i = 0; i < mercenaryData.Number; i++)
							{
								if (flag)
								{
									int num3 = mobileParty.PartyTradeGold - (troopRecruitmentCost + num);
									double num4 = (double)MathF.Min(1f, MathF.Sqrt((float)num3 / (100f * (float)troopRecruitmentCost)));
									float num5 = (float)mobileParty.Party.NumberOfAllMembers / (float)partySizeLimit;
									float num6 = (MathF.Min(10f, 1f / num5) * MathF.Min(10f, 1f / num5) - 1f) * ((mobileParty.IsCaravan && mobileParty.Party.Owner == Hero.MainHero) ? 0.4f : 0.1f);
									num2 = num4 * (double)num6;
								}
								if ((double)MBRandom.RandomFloat < num2)
								{
									this.ApplyRecruitMercenary(mobileParty, settlement, troopType, 1);
									flag = true;
								}
								else
								{
									flag = false;
								}
							}
							return;
						}
					}
				}
			}
			else if (mobileParty.IsLordParty && !mobileParty.IsDisbanding && mobileParty.LeaderHero != null && !mobileParty.Party.IsStarving && (float)mobileParty.LeaderHero.Gold > HeroHelper.StartRecruitingMoneyLimit(mobileParty.LeaderHero) && (mobileParty.LeaderHero == mobileParty.LeaderHero.Clan.Leader || (float)mobileParty.LeaderHero.Clan.Gold > HeroHelper.StartRecruitingMoneyLimitForClanLeader(mobileParty.LeaderHero)) && ((float)mobileParty.Party.NumberOfAllMembers + 0.5f) / (float)mobileParty.LimitedPartySize <= 1f)
			{
				if (settlement.IsTown && this.GetMercenaryData(settlement.Town).HasAvailableMercenary(Occupation.Mercenary))
				{
					float num7 = (float)mobileParty.Party.NumberOfAllMembers / (float)mobileParty.LimitedPartySize;
					CharacterObject troopType2 = this.GetMercenaryData(settlement.Town).TroopType;
					if (troopType2 != null)
					{
						int troopRecruitmentCost2 = Campaign.Current.Models.PartyWageModel.GetTroopRecruitmentCost(troopType2, mobileParty.LeaderHero, false);
						if (troopRecruitmentCost2 < 5000)
						{
							float num8 = MathF.Min(1f, (float)mobileParty.LeaderHero.Gold / ((troopRecruitmentCost2 <= 100) ? 100000f : ((float)((troopRecruitmentCost2 <= 200) ? 125000 : ((troopRecruitmentCost2 <= 400) ? 150000 : ((troopRecruitmentCost2 <= 700) ? 175000 : ((troopRecruitmentCost2 <= 1100) ? 200000 : ((troopRecruitmentCost2 <= 1600) ? 250000 : ((troopRecruitmentCost2 <= 2200) ? 300000 : 400000)))))))));
							float num9 = num8 * num8;
							float num10 = MathF.Max(1f, MathF.Min(10f, 1f / num7)) - 1f;
							float num11 = num9 * num10 * 0.25f;
							int number = this.GetMercenaryData(settlement.Town).Number;
							int num12 = 0;
							for (int j = 0; j < number; j++)
							{
								if (MBRandom.RandomFloat < num11)
								{
									num12++;
								}
							}
							num12 = MathF.Min(num12, mobileParty.LimitedPartySize - mobileParty.Party.NumberOfAllMembers);
							num12 = (((double)troopRecruitmentCost2 <= 0.1) ? num12 : MathF.Min(mobileParty.LeaderHero.Gold / troopRecruitmentCost2, num12));
							if (num12 > 0)
							{
								this.ApplyRecruitMercenary(mobileParty, settlement, troopType2, num12);
							}
						}
					}
				}
				if (mobileParty.Party.NumberOfAllMembers < mobileParty.LimitedPartySize && mobileParty.CanPayMoreWage())
				{
					this.RecruitVolunteersFromNotable(mobileParty, settlement);
				}
			}
		}

		private void RecruitVolunteersFromNotable(MobileParty mobileParty, Settlement settlement)
		{
			if (((float)mobileParty.Party.NumberOfAllMembers + 0.5f) / (float)mobileParty.LimitedPartySize <= 1f)
			{
				foreach (Hero hero in settlement.Notables)
				{
					if (hero.IsAlive)
					{
						if (mobileParty.IsWageLimitExceeded())
						{
							break;
						}
						int num = MBRandom.RandomInt(6);
						int num2 = Campaign.Current.Models.VolunteerModel.MaximumIndexHeroCanRecruitFromHero(mobileParty.IsGarrison ? mobileParty.Party.Owner : mobileParty.LeaderHero, hero, -101);
						for (int i = num; i < num + 6; i++)
						{
							int num3 = i % 6;
							if (num3 >= num2)
							{
								break;
							}
							int num4 = ((mobileParty.LeaderHero != null) ? ((int)MathF.Sqrt((float)mobileParty.LeaderHero.Gold / 10000f)) : 0);
							float num5 = MBRandom.RandomFloat;
							for (int j = 0; j < num4; j++)
							{
								float randomFloat = MBRandom.RandomFloat;
								if (randomFloat > num5)
								{
									num5 = randomFloat;
								}
							}
							if (mobileParty.Army != null)
							{
								float num6 = ((mobileParty.Army.LeaderParty == mobileParty) ? 0.5f : 0.67f);
								num5 = MathF.Pow(num5, num6);
							}
							float num7 = (float)mobileParty.Party.NumberOfAllMembers / (float)mobileParty.LimitedPartySize;
							if (num5 > num7 - 0.1f)
							{
								CharacterObject characterObject = hero.VolunteerTypes[num3];
								if (characterObject != null && mobileParty.LeaderHero.Gold > Campaign.Current.Models.PartyWageModel.GetTroopRecruitmentCost(characterObject, mobileParty.LeaderHero, false) && mobileParty.PaymentLimit >= mobileParty.TotalWage + Campaign.Current.Models.PartyWageModel.GetCharacterWage(characterObject))
								{
									this.GetRecruitVolunteerFromIndividual(mobileParty, characterObject, hero, num3);
									break;
								}
							}
						}
					}
				}
			}
		}

		public void OnSettlementEntered(MobileParty mobileParty, Settlement settlement, Hero hero)
		{
			if (mobileParty != null && mobileParty.MapEvent == null)
			{
				if (!settlement.IsVillage)
				{
					Clan ownerClan = settlement.OwnerClan;
					if (ownerClan == null || ownerClan.IsAtWarWith(mobileParty.MapFaction))
					{
						return;
					}
				}
				if (!settlement.IsRaided && !settlement.IsUnderRaid)
				{
					int num = (mobileParty.IsCaravan ? 1 : ((mobileParty.Army != null && mobileParty.Army == MobileParty.MainParty.Army) ? ((MobileParty.MainParty.PartySizeRatio < 0.6f) ? 1 : ((MobileParty.MainParty.PartySizeRatio < 0.9f) ? 2 : 3)) : 7));
					List<MobileParty> list = new List<MobileParty>();
					if (mobileParty.Army != null && mobileParty.Army.LeaderParty == mobileParty)
					{
						using (List<MobileParty>.Enumerator enumerator = mobileParty.Army.Parties.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								MobileParty mobileParty2 = enumerator.Current;
								if ((mobileParty2 == mobileParty.Army.LeaderParty || mobileParty2.AttachedTo == mobileParty.Army.LeaderParty) && mobileParty2 != MobileParty.MainParty)
								{
									list.Add(mobileParty2);
								}
							}
							goto IL_138;
						}
					}
					if (mobileParty.AttachedTo == null && mobileParty != MobileParty.MainParty)
					{
						list.Add(mobileParty);
					}
					IL_138:
					for (int i = 0; i < num; i++)
					{
						foreach (MobileParty mobileParty3 in list)
						{
							this.CheckRecruiting(mobileParty3, settlement);
						}
					}
				}
			}
		}

		private void ApplyInternal(MobileParty side1Party, Settlement settlement, Hero individual, CharacterObject troop, int number, int bitCode, RecruitmentCampaignBehavior.RecruitingDetail detail)
		{
			int troopRecruitmentCost = Campaign.Current.Models.PartyWageModel.GetTroopRecruitmentCost(troop, side1Party.LeaderHero, false);
			if (detail == RecruitmentCampaignBehavior.RecruitingDetail.MercenaryFromTavern)
			{
				if (side1Party.IsCaravan)
				{
					side1Party.PartyTradeGold -= number * troopRecruitmentCost;
					this.GetMercenaryData(settlement.Town).ChangeMercenaryCount(-number);
				}
				else
				{
					GiveGoldAction.ApplyBetweenCharacters(side1Party.LeaderHero, null, number * troopRecruitmentCost, true);
					this.GetMercenaryData(settlement.Town).ChangeMercenaryCount(-number);
				}
				side1Party.AddElementToMemberRoster(troop, number, false);
			}
			else if (detail == RecruitmentCampaignBehavior.RecruitingDetail.VolunteerFromIndividual)
			{
				GiveGoldAction.ApplyBetweenCharacters(side1Party.LeaderHero, null, troopRecruitmentCost, true);
				individual.VolunteerTypes[bitCode] = null;
				side1Party.AddElementToMemberRoster(troop, 1, false);
			}
			else if (detail == RecruitmentCampaignBehavior.RecruitingDetail.VolunteerFromMap)
			{
				GiveGoldAction.ApplyBetweenCharacters(side1Party.LeaderHero, null, number * troopRecruitmentCost, true);
				side1Party.AddElementToMemberRoster(troop, number, false);
			}
			else if (detail == RecruitmentCampaignBehavior.RecruitingDetail.VolunteerFromIndividualToGarrison)
			{
				individual.VolunteerTypes[bitCode] = null;
				side1Party.AddElementToMemberRoster(troop, 1, false);
			}
			CampaignEventDispatcher.Instance.OnTroopRecruited(side1Party.LeaderHero, settlement, individual, troop, number);
		}

		private void ApplyRecruitMercenary(MobileParty side1Party, Settlement side2Party, CharacterObject subject, int number)
		{
			this.ApplyInternal(side1Party, side2Party, null, subject, number, -1, RecruitmentCampaignBehavior.RecruitingDetail.MercenaryFromTavern);
		}

		private void GetRecruitVolunteerFromMap(MobileParty side1Party, CharacterObject subject, int number)
		{
			this.ApplyInternal(side1Party, null, null, subject, number, -1, RecruitmentCampaignBehavior.RecruitingDetail.VolunteerFromMap);
		}

		private void GetRecruitVolunteerFromIndividual(MobileParty side1Party, CharacterObject subject, Hero individual, int bitCode)
		{
			this.ApplyInternal(side1Party, individual.CurrentSettlement, individual, subject, 1, bitCode, RecruitmentCampaignBehavior.RecruitingDetail.VolunteerFromIndividual);
		}

		private void LocationCharactersAreReadyToSpawn(Dictionary<string, int> unusedUsablePointCount)
		{
			Settlement settlement = PlayerEncounter.LocationEncounter.Settlement;
			Location locationWithId = settlement.LocationComplex.GetLocationWithId("tavern");
			if (CampaignMission.Current.Location == locationWithId)
			{
				this.AddMercenaryCharacterToTavern(settlement);
			}
		}

		private LocationCharacter CreateMercenary(CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			CharacterObject troopType = this.GetMercenaryData(PlayerEncounter.EncounterSettlement.Town).TroopType;
			Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(troopType.Race, "_settlement");
			return new LocationCharacter(new AgentData(new SimpleAgentOrigin(troopType, -1, null, default(UniqueTroopDescriptor))).Monster(monsterWithSuffix).NoHorses(true), new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddOutdoorWandererBehaviors), "spawnpoint_mercenary", true, relation, null, false, false, null, false, false, true);
		}

		protected void AddGameMenus(CampaignGameStarter campaignGameSystemStarter)
		{
			campaignGameSystemStarter.AddGameMenuOption("town_backstreet", "recruit_mercenaries", "{=NwO0CVzn}Recruit {MEN_COUNT} {MERCENARY_NAME} ({TOTAL_AMOUNT}{GOLD_ICON})", new GameMenuOption.OnConditionDelegate(this.buy_mercenaries_condition), delegate(MenuCallbackArgs x)
			{
				this.buy_mercenaries_on_consequence();
			}, false, 2, false, null);
		}

		protected void AddDialogs(CampaignGameStarter campaignGameStarter)
		{
			campaignGameStarter.AddDialogLine("mercenary_recruit_start", "start", "mercenary_tavern_talk", "{=I0StkXlK}Do you have a need for fighters, {?PLAYER.GENDER}madam{?}sir{\\?}? Me and {?PLURAL}{MERCENARY_COUNT} of my mates{?}one of my mates{\\?} are looking for a master. You might call us mercenaries, like. We'll join you for {GOLD_AMOUNT}{GOLD_ICON}", new ConversationSentence.OnConditionDelegate(this.conversation_mercenary_recruit_plural_start_on_condition), null, 100, null);
			campaignGameStarter.AddDialogLine("mercenary_recruit_start_single", "start", "mercenary_tavern_talk", "{=rJwExPKb}Do you have a need for fighters, {?PLAYER.GENDER}madam{?}sir{\\?}? I am looking for a master. I'll join you for {GOLD_AMOUNT}{GOLD_ICON}", new ConversationSentence.OnConditionDelegate(this.conversation_mercenary_recruit_single_start_on_condition), null, 100, null);
			campaignGameStarter.AddPlayerLine("mercenary_recruit_accept", "mercenary_tavern_talk", "mercenary_tavern_talk_hire", "{=PDLDvUfH}All right. I will hire {?PLURAL}all of you{?}you{\\?}. Here is {GOLD_AMOUNT}{GOLD_ICON}", new ConversationSentence.OnConditionDelegate(this.conversation_mercenary_recruit_accept_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_mercenary_recruit_accept_on_consequence), 100, null, null);
			campaignGameStarter.AddPlayerLine("mercenary_recruit_accept_some", "mercenary_tavern_talk", "mercenary_tavern_talk_hire", "{=aTPc7AkY}All right. But I can only hire {MERCENARY_COUNT} of you. Here is {GOLD_AMOUNT}{GOLD_ICON}", new ConversationSentence.OnConditionDelegate(this.conversation_mercenary_recruit_accept_some_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_mercenary_recruit_accept_some_on_consequence), 100, null, null);
			campaignGameStarter.AddPlayerLine("mercenary_recruit_reject_gold", "mercenary_tavern_talk", "close_window", "{=n5BGNLrc}That sounds good. But I can't afford any more men right now.", new ConversationSentence.OnConditionDelegate(this.conversation_mercenary_recruit_reject_gold_on_condition), null, 100, null, null);
			campaignGameStarter.AddPlayerLine("mercenary_recruit_reject", "mercenary_tavern_talk", "close_window", "{=I2thb8VU}Sorry. I don't need any other men right now.", new ConversationSentence.OnConditionDelegate(this.conversation_mercenary_recruit_dont_need_men_on_condition), null, 100, null, null);
			campaignGameStarter.AddDialogLine("mercenary_recruit_end", "mercenary_tavern_talk_hire", "close_window", "{=vbxQoyN3}{RANDOM_HIRE_SENTENCE}", new ConversationSentence.OnConditionDelegate(this.conversation_mercenary_recruit_end_on_condition), null, 100, null);
			campaignGameStarter.AddDialogLine("mercenary_recruit_start_2", "start", "close_window", "{=Jhj437BV}Don't worry, I'll be ready. Just having a last drink for the road.", new ConversationSentence.OnConditionDelegate(this.conversation_mercenary_recruited_on_condition), null, 100, null);
		}

		private bool buy_mercenaries_condition(MenuCallbackArgs args)
		{
			if (MobileParty.MainParty.CurrentSettlement != null && MobileParty.MainParty.CurrentSettlement.IsTown && this.GetMercenaryData(MobileParty.MainParty.CurrentSettlement.Town).Number > 0)
			{
				RecruitmentCampaignBehavior.TownMercenaryData mercenaryData = this.GetMercenaryData(MobileParty.MainParty.CurrentSettlement.Town);
				int troopRecruitmentCost = Campaign.Current.Models.PartyWageModel.GetTroopRecruitmentCost(mercenaryData.TroopType, Hero.MainHero, false);
				if (Hero.MainHero.Gold >= troopRecruitmentCost)
				{
					int num = MathF.Min(mercenaryData.Number, Hero.MainHero.Gold / troopRecruitmentCost);
					MBTextManager.SetTextVariable("MEN_COUNT", num);
					MBTextManager.SetTextVariable("MERCENARY_NAME", mercenaryData.TroopType.Name, false);
					MBTextManager.SetTextVariable("TOTAL_AMOUNT", num * troopRecruitmentCost);
				}
				else
				{
					args.Tooltip = GameTexts.FindText("str_decision_not_enough_gold", null);
					args.IsEnabled = false;
					int number = mercenaryData.Number;
					MBTextManager.SetTextVariable("MEN_COUNT", number);
					MBTextManager.SetTextVariable("MERCENARY_NAME", mercenaryData.TroopType.Name, false);
					MBTextManager.SetTextVariable("TOTAL_AMOUNT", number * troopRecruitmentCost);
				}
				args.optionLeaveType = GameMenuOption.LeaveType.Bribe;
				return true;
			}
			return false;
		}

		private void buy_mercenaries_on_consequence()
		{
			if (MobileParty.MainParty.CurrentSettlement != null && MobileParty.MainParty.CurrentSettlement.IsTown && this.GetMercenaryData(MobileParty.MainParty.CurrentSettlement.Town).Number > 0)
			{
				RecruitmentCampaignBehavior.TownMercenaryData mercenaryData = this.GetMercenaryData(MobileParty.MainParty.CurrentSettlement.Town);
				int troopRecruitmentCost = Campaign.Current.Models.PartyWageModel.GetTroopRecruitmentCost(mercenaryData.TroopType, Hero.MainHero, false);
				if (Hero.MainHero.Gold >= troopRecruitmentCost)
				{
					int num = MathF.Min(mercenaryData.Number, Hero.MainHero.Gold / troopRecruitmentCost);
					MobileParty.MainParty.MemberRoster.AddToCounts(mercenaryData.TroopType, num, false, 0, 0, true, -1);
					GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, -(num * troopRecruitmentCost), false);
					mercenaryData.ChangeMercenaryCount(-num);
					GameMenu.SwitchToMenu("town_backstreet");
				}
			}
		}

		private bool conversation_mercenary_recruit_plural_start_on_condition()
		{
			if (PlayerEncounter.EncounterSettlement == null || !PlayerEncounter.EncounterSettlement.IsTown)
			{
				return false;
			}
			RecruitmentCampaignBehavior.TownMercenaryData mercenaryData = this.GetMercenaryData(PlayerEncounter.EncounterSettlement.Town);
			bool flag = (CharacterObject.OneToOneConversationCharacter.Occupation == Occupation.Mercenary || CharacterObject.OneToOneConversationCharacter.Occupation == Occupation.CaravanGuard || CharacterObject.OneToOneConversationCharacter.Occupation == Occupation.Gangster) && PlayerEncounter.EncounterSettlement != null && PlayerEncounter.EncounterSettlement.IsTown && mercenaryData.Number > 1;
			if (flag)
			{
				int troopRecruitmentCost = Campaign.Current.Models.PartyWageModel.GetTroopRecruitmentCost(mercenaryData.TroopType, Hero.MainHero, false);
				MBTextManager.SetTextVariable("PLURAL", (mercenaryData.Number - 1 > 1) ? 1 : 0);
				MBTextManager.SetTextVariable("MERCENARY_COUNT", mercenaryData.Number - 1);
				MBTextManager.SetTextVariable("GOLD_AMOUNT", troopRecruitmentCost * mercenaryData.Number);
			}
			return flag;
		}

		private bool conversation_mercenary_recruit_single_start_on_condition()
		{
			if (PlayerEncounter.EncounterSettlement == null || !PlayerEncounter.EncounterSettlement.IsTown)
			{
				return false;
			}
			RecruitmentCampaignBehavior.TownMercenaryData mercenaryData = this.GetMercenaryData(PlayerEncounter.EncounterSettlement.Town);
			bool flag = (CharacterObject.OneToOneConversationCharacter.Occupation == Occupation.Mercenary || CharacterObject.OneToOneConversationCharacter.Occupation == Occupation.CaravanGuard || CharacterObject.OneToOneConversationCharacter.Occupation == Occupation.Gangster) && PlayerEncounter.EncounterSettlement != null && PlayerEncounter.EncounterSettlement.IsTown && mercenaryData.Number == 1;
			if (flag)
			{
				int troopRecruitmentCost = Campaign.Current.Models.PartyWageModel.GetTroopRecruitmentCost(mercenaryData.TroopType, Hero.MainHero, false);
				MBTextManager.SetTextVariable("GOLD_AMOUNT", mercenaryData.Number * troopRecruitmentCost);
			}
			return flag;
		}

		private bool conversation_mercenary_recruit_accept_on_condition()
		{
			RecruitmentCampaignBehavior.TownMercenaryData mercenaryData = this.GetMercenaryData(PlayerEncounter.EncounterSettlement.Town);
			int troopRecruitmentCost = Campaign.Current.Models.PartyWageModel.GetTroopRecruitmentCost(mercenaryData.TroopType, Hero.MainHero, false);
			MBTextManager.SetTextVariable("PLURAL", (mercenaryData.Number > 1) ? 1 : 0);
			return Hero.MainHero.Gold >= mercenaryData.Number * troopRecruitmentCost;
		}

		private bool conversation_mercenary_recruited_on_condition()
		{
			return (CharacterObject.OneToOneConversationCharacter.Occupation == Occupation.Mercenary || CharacterObject.OneToOneConversationCharacter.Occupation == Occupation.CaravanGuard || CharacterObject.OneToOneConversationCharacter.Occupation == Occupation.Gangster) && PlayerEncounter.EncounterSettlement != null;
		}

		private void BuyMercenaries()
		{
			this.GetMercenaryData(PlayerEncounter.EncounterSettlement.Town).ChangeMercenaryCount(-this._selectedMercenaryCount);
			int troopRecruitmentCost = Campaign.Current.Models.PartyWageModel.GetTroopRecruitmentCost(this.GetMercenaryData(PlayerEncounter.EncounterSettlement.Town).TroopType, Hero.MainHero, false);
			MobileParty.MainParty.AddElementToMemberRoster(CharacterObject.OneToOneConversationCharacter, this._selectedMercenaryCount, false);
			int num = this._selectedMercenaryCount * troopRecruitmentCost;
			GiveGoldAction.ApplyBetweenCharacters(Hero.MainHero, null, num, false);
			CampaignEventDispatcher.Instance.OnUnitRecruited(CharacterObject.OneToOneConversationCharacter, this._selectedMercenaryCount);
		}

		private void conversation_mercenary_recruit_accept_on_consequence()
		{
			this._selectedMercenaryCount = this.GetMercenaryData(PlayerEncounter.EncounterSettlement.Town).Number;
			this.BuyMercenaries();
		}

		private bool conversation_mercenary_recruit_accept_some_on_condition()
		{
			int troopRecruitmentCost = Campaign.Current.Models.PartyWageModel.GetTroopRecruitmentCost(this.GetMercenaryData(PlayerEncounter.EncounterSettlement.Town).TroopType, Hero.MainHero, false);
			if (Hero.MainHero.Gold >= troopRecruitmentCost && Hero.MainHero.Gold < this.GetMercenaryData(PlayerEncounter.EncounterSettlement.Town).Number * troopRecruitmentCost)
			{
				this._selectedMercenaryCount = 0;
				while (Hero.MainHero.Gold >= troopRecruitmentCost * (this._selectedMercenaryCount + 1))
				{
					this._selectedMercenaryCount++;
				}
				MBTextManager.SetTextVariable("MERCENARY_COUNT", this._selectedMercenaryCount);
				MBTextManager.SetTextVariable("GOLD_AMOUNT", troopRecruitmentCost * this._selectedMercenaryCount);
				return true;
			}
			return false;
		}

		private void conversation_mercenary_recruit_accept_some_on_consequence()
		{
			this.BuyMercenaries();
		}

		private bool conversation_mercenary_recruit_reject_gold_on_condition()
		{
			int troopRecruitmentCost = Campaign.Current.Models.PartyWageModel.GetTroopRecruitmentCost(this.GetMercenaryData(PlayerEncounter.EncounterSettlement.Town).TroopType, Hero.MainHero, false);
			return Hero.MainHero.Gold < troopRecruitmentCost;
		}

		private bool conversation_mercenary_recruit_dont_need_men_on_condition()
		{
			int troopRecruitmentCost = Campaign.Current.Models.PartyWageModel.GetTroopRecruitmentCost(this.GetMercenaryData(PlayerEncounter.EncounterSettlement.Town).TroopType, Hero.MainHero, false);
			return Hero.MainHero.Gold >= troopRecruitmentCost;
		}

		private bool conversation_mercenary_recruit_end_on_condition()
		{
			MBTextManager.SetTextVariable("RANDOM_HIRE_SENTENCE", GameTexts.FindText("str_mercenary_tavern_talk_hire", MBRandom.RandomInt(4).ToString()), false);
			return true;
		}

		private Dictionary<Town, RecruitmentCampaignBehavior.TownMercenaryData> _townMercenaryData = new Dictionary<Town, RecruitmentCampaignBehavior.TownMercenaryData>();

		private int _selectedMercenaryCount;

		private CharacterObject _selectedTroop;

		public class RecruitmentCampaignBehaviorTypeDefiner : SaveableTypeDefiner
		{
			public RecruitmentCampaignBehaviorTypeDefiner()
				: base(881200)
			{
			}

			protected override void DefineClassTypes()
			{
				base.AddClassDefinition(typeof(RecruitmentCampaignBehavior.TownMercenaryData), 1, null);
			}

			protected override void DefineContainerDefinitions()
			{
				base.ConstructContainerDefinition(typeof(Dictionary<Town, RecruitmentCampaignBehavior.TownMercenaryData>));
			}
		}

		internal class TownMercenaryData
		{
			[SaveableProperty(202)]
			public CharacterObject TroopType { get; private set; }

			[SaveableProperty(203)]
			public int Number { get; private set; }

			public TownMercenaryData(Town currentTown)
			{
				this._currentTown = currentTown;
			}

			public void ChangeMercenaryType(CharacterObject troopType, int number)
			{
				if (troopType != this.TroopType)
				{
					CharacterObject troopType2 = this.TroopType;
					this.TroopType = troopType;
					this.Number = number;
					CampaignEventDispatcher.Instance.OnMercenaryTroopChangedInTown(this._currentTown, troopType2, this.TroopType);
					return;
				}
				if (this.Number != number)
				{
					int num = number - this.Number;
					this.ChangeMercenaryCount(num);
				}
			}

			public void ChangeMercenaryCount(int difference)
			{
				if (difference != 0)
				{
					int number = this.Number;
					this.Number += difference;
					CampaignEventDispatcher.Instance.OnMercenaryNumberChangedInTown(this._currentTown, number, this.Number);
				}
			}

			public bool HasAvailableMercenary(Occupation occupation = Occupation.NotAssigned)
			{
				return this.TroopType != null && this.Number > 0 && (occupation == Occupation.NotAssigned || this.TroopType.Occupation == occupation);
			}

			internal static void AutoGeneratedStaticCollectObjectsTownMercenaryData(object o, List<object> collectedObjects)
			{
				((RecruitmentCampaignBehavior.TownMercenaryData)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				collectedObjects.Add(this._currentTown);
				collectedObjects.Add(this.TroopType);
			}

			internal static object AutoGeneratedGetMemberValueTroopType(object o)
			{
				return ((RecruitmentCampaignBehavior.TownMercenaryData)o).TroopType;
			}

			internal static object AutoGeneratedGetMemberValueNumber(object o)
			{
				return ((RecruitmentCampaignBehavior.TownMercenaryData)o).Number;
			}

			internal static object AutoGeneratedGetMemberValue_currentTown(object o)
			{
				return ((RecruitmentCampaignBehavior.TownMercenaryData)o)._currentTown;
			}

			[SaveableField(204)]
			private readonly Town _currentTown;
		}

		public enum RecruitingDetail
		{
			MercenaryFromTavern,
			VolunteerFromIndividual,
			VolunteerFromIndividualToGarrison,
			VolunteerFromMap
		}
	}
}
