using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors.AiBehaviors
{
	public class AiVisitSettlementBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.AiHourlyTickEvent.AddNonSerializedListener(this, new Action<MobileParty, PartyThinkParams>(this.AiHourlyTick));
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
		}

		private void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
		{
			this._disbandPartyCampaignBehavior = Campaign.Current.GetCampaignBehavior<IDisbandPartyCampaignBehavior>();
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

		private void AiHourlyTick(MobileParty mobileParty, PartyThinkParams p)
		{
			Settlement currentSettlement = mobileParty.CurrentSettlement;
			if (((currentSettlement != null) ? currentSettlement.SiegeEvent : null) != null)
			{
				return;
			}
			Settlement currentSettlementOfMobilePartyForAICalculation = MobilePartyHelper.GetCurrentSettlementOfMobilePartyForAICalculation(mobileParty);
			if (mobileParty.IsBandit)
			{
				this.CalculateVisitHideoutScoresForBanditParty(mobileParty, currentSettlementOfMobilePartyForAICalculation, p);
				return;
			}
			IFaction mapFaction = mobileParty.MapFaction;
			if (mobileParty.IsMilitia || mobileParty.IsCaravan || mobileParty.IsVillager || (!mapFaction.IsMinorFaction && !mapFaction.IsKingdomFaction && (mobileParty.LeaderHero == null || !mobileParty.LeaderHero.IsLord)))
			{
				return;
			}
			if (mobileParty.Army == null || mobileParty.Army.LeaderParty == mobileParty || mobileParty.Army.Cohesion < (float)mobileParty.Army.CohesionThresholdForDispersion)
			{
				Hero leaderHero = mobileParty.LeaderHero;
				ValueTuple<float, float, int, int> valueTuple = this.CalculatePartyParameters(mobileParty);
				float item = valueTuple.Item1;
				float item2 = valueTuple.Item2;
				int item3 = valueTuple.Item3;
				int item4 = valueTuple.Item4;
				float num = item2 / Math.Min(1f, Math.Max(0.1f, item));
				float num2 = ((num >= 1f) ? 0.33f : ((MathF.Max(1f, MathF.Min(2f, num)) - 0.5f) / 1.5f));
				float num3 = mobileParty.Food;
				float num4 = -mobileParty.FoodChange;
				int num5 = ((leaderHero != null) ? leaderHero.Gold : 0);
				if (mobileParty.Army != null && mobileParty == mobileParty.Army.LeaderParty)
				{
					foreach (MobileParty mobileParty2 in mobileParty.Army.LeaderParty.AttachedParties)
					{
						num3 += mobileParty2.Food;
						num4 += -mobileParty2.FoodChange;
						int num6 = num5;
						Hero leaderHero2 = mobileParty2.LeaderHero;
						num5 = num6 + ((leaderHero2 != null) ? leaderHero2.Gold : 0);
					}
				}
				float num7 = 1f;
				if (leaderHero != null && mobileParty.IsLordParty)
				{
					num7 = this.CalculateSellItemScore(mobileParty);
				}
				int num8 = mobileParty.Party.PrisonerSizeLimit;
				if (mobileParty.Army != null)
				{
					foreach (MobileParty mobileParty3 in mobileParty.Army.LeaderParty.AttachedParties)
					{
						num8 += mobileParty3.Party.PrisonerSizeLimit;
					}
				}
				SortedList<ValueTuple<float, int>, Settlement> sortedList = this.FindSettlementsToVisitWithDistances(mobileParty);
				float num9 = PartyBaseHelper.FindPartySizeNormalLimit(mobileParty);
				float num10 = Campaign.MapDiagonalSquared;
				if (num3 - num4 < 0f)
				{
					foreach (KeyValuePair<ValueTuple<float, int>, Settlement> keyValuePair in sortedList)
					{
						float item5 = keyValuePair.Key.Item1;
						Settlement value = keyValuePair.Value;
						if (item5 < 250f && item5 < num10 && (float)value.ItemRoster.TotalFood > num4 * 2f)
						{
							num10 = item5;
						}
					}
				}
				float num11 = 2000f;
				float num12 = 2000f;
				if (leaderHero != null)
				{
					num11 = HeroHelper.StartRecruitingMoneyLimitForClanLeader(leaderHero);
					num12 = HeroHelper.StartRecruitingMoneyLimit(leaderHero);
				}
				float num13 = Campaign.AverageDistanceBetweenTwoFortifications * 0.4f;
				float num14 = (84f + Campaign.AverageDistanceBetweenTwoFortifications * 1.5f) * 0.5f;
				float num15 = (424f + 7.57f * Campaign.AverageDistanceBetweenTwoFortifications) * 0.5f;
				foreach (KeyValuePair<ValueTuple<float, int>, Settlement> keyValuePair2 in sortedList)
				{
					Settlement value2 = keyValuePair2.Value;
					float item6 = keyValuePair2.Key.Item1;
					float num16 = 1.6f;
					if (mobileParty.IsDisbanding)
					{
						goto IL_37E;
					}
					IDisbandPartyCampaignBehavior disbandPartyCampaignBehavior = this._disbandPartyCampaignBehavior;
					if (disbandPartyCampaignBehavior != null && disbandPartyCampaignBehavior.IsPartyWaitingForDisband(mobileParty))
					{
						goto IL_37E;
					}
					if (leaderHero == null)
					{
						this.AddBehaviorTupleWithScore(p, value2, this.CalculateMergeScoreForLeaderlessParty(mobileParty, value2, item6));
					}
					else
					{
						float num17 = item6;
						if (num17 >= 250f)
						{
							this.AddBehaviorTupleWithScore(p, value2, 0.025f);
							continue;
						}
						float num18 = num17;
						num17 = MathF.Max(num13, num17);
						float num19 = MathF.Max(0.1f, MathF.Min(1f, num14 / (num14 - num13 + num17)));
						float num20 = num19;
						if (item < 0.6f)
						{
							num20 = MathF.Pow(num19, MathF.Pow(0.6f / MathF.Max(0.15f, item), 0.3f));
						}
						int? num21 = ((currentSettlementOfMobilePartyForAICalculation != null) ? new int?(currentSettlementOfMobilePartyForAICalculation.ItemRoster.TotalFood) : null);
						int num22 = item4 / Campaign.Current.Models.MobilePartyFoodConsumptionModel.NumberOfMenOnMapToEatOneFood * 3;
						bool flag = ((num21.GetValueOrDefault() > num22) & (num21 != null)) || num3 > (float)(item4 / Campaign.Current.Models.MobilePartyFoodConsumptionModel.NumberOfMenOnMapToEatOneFood);
						float num23 = (float)item3 / (float)item4;
						float num24 = 1f + ((item4 > 0) ? (num23 * MathF.Max(0.25f, num19 * num19) * MathF.Pow((float)item3, 0.25f) * ((mobileParty.Army != null) ? 4f : 3f) * ((value2.IsFortification && flag) ? 18f : 0f)) : 0f);
						if (mobileParty.MapEvent != null || mobileParty.SiegeEvent != null)
						{
							num24 = MathF.Sqrt(num24);
						}
						float num25 = 1f;
						if ((value2 == currentSettlementOfMobilePartyForAICalculation && currentSettlementOfMobilePartyForAICalculation.IsFortification) || (currentSettlementOfMobilePartyForAICalculation == null && value2 == mobileParty.TargetSettlement))
						{
							num25 = 1.2f;
						}
						else if (currentSettlementOfMobilePartyForAICalculation == null && value2 == mobileParty.LastVisitedSettlement)
						{
							num25 = 0.8f;
						}
						float num26 = 0.16f;
						float num27 = Math.Max(0f, num3) / num4;
						if (num4 > 0f && (mobileParty.BesiegedSettlement == null || num27 <= 1f) && num5 > 100 && (value2.IsTown || value2.IsVillage) && num27 < 4f)
						{
							float num28 = (float)((int)(num4 * ((num27 < 1f && value2.IsVillage) ? Campaign.Current.Models.PartyFoodBuyingModel.MinimumDaysFoodToLastWhileBuyingFoodFromVillage : Campaign.Current.Models.PartyFoodBuyingModel.MinimumDaysFoodToLastWhileBuyingFoodFromTown)) + 1);
							float num29 = 3f - Math.Min(3f, Math.Max(0f, num27 - 1f));
							float num30 = num28 + 20f * (float)(value2.IsTown ? 2 : 1) * ((num18 > 100f) ? 1f : (num18 / 100f));
							int num31 = (int)((float)(num5 - 100) / Campaign.Current.Models.PartyFoodBuyingModel.LowCostFoodPriceAverage);
							num26 += num29 * num29 * 0.093f * ((num27 < 2f) ? (1f + 0.5f * (2f - num27)) : 1f) * (float)Math.Pow((double)(Math.Min(num30, (float)Math.Min(num31, value2.ItemRoster.TotalFood)) / num30), 0.5);
						}
						float num32 = 0f;
						int num33 = 0;
						int num34 = 0;
						if (item < 1f && mobileParty.CanPayMoreWage())
						{
							num33 = value2.NumberOfLordPartiesAt;
							num34 = value2.NumberOfLordPartiesTargeting;
							if (currentSettlementOfMobilePartyForAICalculation == value2)
							{
								int num35 = num33;
								Army army = mobileParty.Army;
								num33 = num35 - ((army != null) ? army.LeaderPartyAndAttachedPartiesCount : 1);
								if (num33 < 0)
								{
									num33 = 0;
								}
							}
							if (mobileParty.TargetSettlement == value2 || (mobileParty.Army != null && mobileParty.Army.LeaderParty.TargetSettlement == value2))
							{
								int num36 = num34;
								Army army2 = mobileParty.Army;
								num34 = num36 - ((army2 != null) ? army2.LeaderPartyAndAttachedPartiesCount : 1);
								if (num34 < 0)
								{
									num34 = 0;
								}
							}
							if (!value2.IsCastle && !mobileParty.Party.IsStarving && (float)leaderHero.Gold > num12 && (leaderHero.Clan.Leader == leaderHero || (float)leaderHero.Clan.Gold > num11) && num9 > mobileParty.PartySizeRatio)
							{
								num32 = (float)this.ApproximateNumberOfVolunteersCanBeRecruitedFromSettlement(leaderHero, value2);
								num32 = ((num32 > (float)((int)((num9 - mobileParty.PartySizeRatio) * 100f))) ? ((float)((int)((num9 - mobileParty.PartySizeRatio) * 100f))) : num32);
							}
						}
						float num37 = num32 * num19 / MathF.Sqrt((float)(1 + num33 + num34));
						float num38 = ((num37 < 1f) ? num37 : ((float)Math.Pow((double)num37, (double)num2)));
						float num39 = Math.Max(Math.Min(1f, num26), Math.Max((mapFaction == value2.MapFaction) ? 0.25f : 0.16f, num * Math.Max(1f, Math.Min(2f, num)) * num38 * (1f - 0.9f * num23) * (1f - 0.9f * num23)));
						if (mobileParty.Army != null)
						{
							num39 /= (float)mobileParty.Army.LeaderPartyAndAttachedPartiesCount;
						}
						num16 *= num39 * num24 * num26 * num20;
						if (num16 >= 2.5f)
						{
							this.AddBehaviorTupleWithScore(p, value2, num16);
							break;
						}
						float num40 = 1f;
						if (num32 > 0f)
						{
							num40 = 1f + ((mobileParty.DefaultBehavior == AiBehavior.GoToSettlement && value2 != currentSettlementOfMobilePartyForAICalculation && num17 < num13) ? (0.1f * MathF.Min(5f, num32) - 0.1f * MathF.Min(5f, num32) * (num17 / num13) * (num17 / num13)) : 0f);
						}
						float num41 = (value2.IsCastle ? 1.4f : 1f);
						num16 *= (value2.IsTown ? num7 : 1f) * num40 * num41;
						if (num16 >= 2.5f)
						{
							this.AddBehaviorTupleWithScore(p, value2, num16);
							break;
						}
						int num42 = mobileParty.PrisonRoster.TotalManCount + mobileParty.PrisonRoster.TotalHeroes * 5;
						float num43 = 1f;
						float num44 = 1f;
						if (mobileParty.Army != null)
						{
							if (mobileParty.Army.LeaderParty != mobileParty)
							{
								num43 = ((float)mobileParty.Army.CohesionThresholdForDispersion - mobileParty.Army.Cohesion) / (float)mobileParty.Army.CohesionThresholdForDispersion;
							}
							num44 = ((MobileParty.MainParty != null && mobileParty.Army == MobileParty.MainParty.Army) ? 0.6f : 0.8f);
							foreach (MobileParty mobileParty4 in mobileParty.Army.LeaderParty.AttachedParties)
							{
								num42 += mobileParty4.PrisonRoster.TotalManCount + mobileParty4.PrisonRoster.TotalHeroes * 5;
							}
						}
						float num45 = (value2.IsFortification ? (1f + 2f * (float)(num42 / num8)) : 1f);
						float num46 = 1f;
						if (mobileParty.Ai.NumberOfRecentFleeingFromAParty > 0)
						{
							Vec2 vec = value2.Position2D - mobileParty.Position2D;
							vec.Normalize();
							float num47 = mobileParty.AverageFleeTargetDirection.Distance(vec);
							num46 = 1f - Math.Max(2f - num47, 0f) * 0.25f * (Math.Min((float)mobileParty.Ai.NumberOfRecentFleeingFromAParty, 10f) * 0.2f);
						}
						float num48 = 1f;
						float num49 = 1f;
						float num50 = 1f;
						float num51 = 1f;
						float num52 = 1f;
						if (num26 <= 0.5f)
						{
							ValueTuple<float, float, float, float> valueTuple2 = this.CalculateBeingSettlementOwnerScores(mobileParty, value2, currentSettlementOfMobilePartyForAICalculation, -1f, num19, item);
							num48 = valueTuple2.Item1;
							num49 = valueTuple2.Item2;
							num50 = valueTuple2.Item3;
							num51 = valueTuple2.Item4;
						}
						else
						{
							float num53 = MathF.Sqrt(num10);
							num52 = ((num53 > num15) ? (1f + 7f * MathF.Min(1f, num26 - 0.5f)) : (1f + 7f * (num53 / num15) * MathF.Min(1f, num26 - 0.5f)));
						}
						num16 *= num46 * num52 * num25 * num43 * num45 * num44 * num48 * num50 * num49 * num51;
					}
					IL_BEE:
					if (num16 > 0.025f)
					{
						this.AddBehaviorTupleWithScore(p, value2, num16);
						continue;
					}
					continue;
					IL_37E:
					this.AddBehaviorTupleWithScore(p, value2, this.CalculateMergeScoreForDisbandingParty(mobileParty, value2, item6));
					goto IL_BEE;
				}
			}
		}

		private int ApproximateNumberOfVolunteersCanBeRecruitedFromSettlement(Hero hero, Settlement settlement)
		{
			int num = 4;
			if (hero.MapFaction != settlement.MapFaction)
			{
				num = 2;
			}
			int num2 = 0;
			foreach (Hero hero2 in settlement.Notables)
			{
				if (hero2.IsAlive)
				{
					for (int i = 0; i < num; i++)
					{
						if (hero2.VolunteerTypes[i] != null)
						{
							num2++;
						}
					}
				}
			}
			return num2;
		}

		private float CalculateSellItemScore(MobileParty mobileParty)
		{
			float num = 0f;
			float num2 = 0f;
			for (int i = 0; i < mobileParty.ItemRoster.Count; i++)
			{
				ItemRosterElement itemRosterElement = mobileParty.ItemRoster[i];
				if (itemRosterElement.EquipmentElement.Item.IsMountable)
				{
					num2 += (float)(itemRosterElement.Amount * itemRosterElement.EquipmentElement.Item.Value);
				}
				else if (!itemRosterElement.EquipmentElement.Item.IsFood)
				{
					num += (float)(itemRosterElement.Amount * itemRosterElement.EquipmentElement.Item.Value);
				}
			}
			float num3 = ((num2 > (float)mobileParty.LeaderHero.Gold * 0.1f) ? MathF.Min(3f, MathF.Pow((num2 + 1000f) / ((float)mobileParty.LeaderHero.Gold * 0.1f + 1000f), 0.33f)) : 1f);
			float num4 = 1f + MathF.Min(3f, MathF.Pow(num / (((float)mobileParty.MemberRoster.TotalManCount + 5f) * 100f), 0.33f));
			float num5 = num3 * num4;
			if (mobileParty.Army != null)
			{
				num5 = MathF.Sqrt(num5);
			}
			return num5;
		}

		private ValueTuple<float, float, int, int> CalculatePartyParameters(MobileParty mobileParty)
		{
			float num = 0f;
			int num2 = 0;
			int num3 = 0;
			float num6;
			if (mobileParty.Army != null)
			{
				float num4 = 0f;
				foreach (MobileParty mobileParty2 in mobileParty.Army.Parties)
				{
					float partySizeRatio = mobileParty2.PartySizeRatio;
					num4 += partySizeRatio;
					num2 += mobileParty2.MemberRoster.TotalWounded;
					num3 += mobileParty2.MemberRoster.TotalManCount;
					float num5 = PartyBaseHelper.FindPartySizeNormalLimit(mobileParty2);
					num += num5;
				}
				num6 = num4 / (float)mobileParty.Army.Parties.Count;
				num /= (float)mobileParty.Army.Parties.Count;
			}
			else
			{
				num6 = mobileParty.PartySizeRatio;
				num2 += mobileParty.MemberRoster.TotalWounded;
				num3 += mobileParty.MemberRoster.TotalManCount;
				num += PartyBaseHelper.FindPartySizeNormalLimit(mobileParty);
			}
			return new ValueTuple<float, float, int, int>(num6, num, num2, num3);
		}

		private void CalculateVisitHideoutScoresForBanditParty(MobileParty mobileParty, Settlement currentSettlement, PartyThinkParams p)
		{
			if (!mobileParty.MapFaction.Culture.CanHaveSettlement)
			{
				return;
			}
			if (currentSettlement != null && currentSettlement.IsHideout)
			{
				return;
			}
			int num = 0;
			for (int i = 0; i < mobileParty.ItemRoster.Count; i++)
			{
				ItemRosterElement itemRosterElement = mobileParty.ItemRoster[i];
				num += itemRosterElement.Amount * itemRosterElement.EquipmentElement.Item.Value;
			}
			float num2 = 1f + 4f * Math.Min((float)num, 1000f) / 1000f;
			int num3 = 0;
			MBReadOnlyList<Hideout> allHideouts = Campaign.Current.AllHideouts;
			foreach (Hideout hideout in allHideouts)
			{
				if (hideout.Settlement.Culture == mobileParty.Party.Culture && hideout.IsInfested)
				{
					num3++;
				}
			}
			float num4 = 1f + 4f * (float)Math.Sqrt((double)(mobileParty.PrisonRoster.TotalManCount / mobileParty.Party.PrisonerSizeLimit));
			int numberOfMinimumBanditPartiesInAHideoutToInfestIt = Campaign.Current.Models.BanditDensityModel.NumberOfMinimumBanditPartiesInAHideoutToInfestIt;
			int numberOfMaximumBanditPartiesInEachHideout = Campaign.Current.Models.BanditDensityModel.NumberOfMaximumBanditPartiesInEachHideout;
			int numberOfMaximumHideoutsAtEachBanditFaction = Campaign.Current.Models.BanditDensityModel.NumberOfMaximumHideoutsAtEachBanditFaction;
			float num5 = (424f + 7.57f * Campaign.AverageDistanceBetweenTwoFortifications) / 2f;
			foreach (Hideout hideout2 in allHideouts)
			{
				Settlement settlement = hideout2.Settlement;
				if (settlement.Party.MapEvent == null && settlement.Culture == mobileParty.Party.Culture)
				{
					float num6 = Campaign.Current.Models.MapDistanceModel.GetDistance(mobileParty, settlement);
					num6 = Math.Max(10f, num6);
					float num7 = num5 / (num5 + num6);
					int num8 = 0;
					foreach (MobileParty mobileParty2 in settlement.Parties)
					{
						if (mobileParty2.IsBandit && !mobileParty2.IsBanditBossParty)
						{
							num8++;
						}
					}
					float num10;
					if (num8 < numberOfMinimumBanditPartiesInAHideoutToInfestIt)
					{
						float num9 = (float)(numberOfMaximumHideoutsAtEachBanditFaction - num3) / (float)numberOfMaximumHideoutsAtEachBanditFaction;
						num10 = ((num3 < numberOfMaximumHideoutsAtEachBanditFaction) ? (0.25f + 0.75f * num9) : 0f);
					}
					else
					{
						num10 = Math.Max(0f, 1f * (1f - (float)(Math.Min(numberOfMaximumBanditPartiesInEachHideout, num8) - numberOfMinimumBanditPartiesInAHideoutToInfestIt) / (float)(numberOfMaximumBanditPartiesInEachHideout - numberOfMinimumBanditPartiesInAHideoutToInfestIt)));
					}
					float num11 = ((mobileParty.DefaultBehavior == AiBehavior.GoToSettlement && mobileParty.TargetSettlement == settlement) ? 1f : (MBRandom.RandomFloat * MBRandom.RandomFloat * MBRandom.RandomFloat * MBRandom.RandomFloat * MBRandom.RandomFloat * MBRandom.RandomFloat * MBRandom.RandomFloat * MBRandom.RandomFloat));
					float num12 = num7 * num10 * num2 * num11 * num4;
					this.AddBehaviorTupleWithScore(p, hideout2.Settlement, num12);
				}
			}
		}

		private ValueTuple<float, float, float, float> CalculateBeingSettlementOwnerScores(MobileParty mobileParty, Settlement settlement, Settlement currentSettlement, float idealGarrisonStrengthPerWalledCenter, float distanceScorePure, float averagePartySizeRatioToMaximumSize)
		{
			float num = 1f;
			float num2 = 1f;
			float num3 = 1f;
			float num4 = 1f;
			Hero leaderHero = mobileParty.LeaderHero;
			IFaction mapFaction = mobileParty.MapFaction;
			if (currentSettlement != settlement && (mobileParty.Army == null || mobileParty.Army.LeaderParty != mobileParty))
			{
				if (settlement.OwnerClan.Leader == leaderHero)
				{
					float currentTime = Campaign.CurrentTime;
					float lastVisitTimeOfOwner = settlement.LastVisitTimeOfOwner;
					float num5 = ((currentTime - lastVisitTimeOfOwner > 24f) ? (currentTime - lastVisitTimeOfOwner) : ((24f - (currentTime - lastVisitTimeOfOwner)) * 15f)) / 360f;
					num += num5;
				}
				if (MBRandom.RandomFloat < 0.1f && settlement.IsFortification && leaderHero.Clan != Clan.PlayerClan && (settlement.OwnerClan.Leader == leaderHero || settlement.OwnerClan == leaderHero.Clan))
				{
					if (idealGarrisonStrengthPerWalledCenter == -1f)
					{
						idealGarrisonStrengthPerWalledCenter = FactionHelper.FindIdealGarrisonStrengthPerWalledCenter(mapFaction as Kingdom, null);
					}
					int num6 = Campaign.Current.Models.SettlementGarrisonModel.FindNumberOfTroopsToTakeFromGarrison(mobileParty, settlement, idealGarrisonStrengthPerWalledCenter);
					if (num6 > 0)
					{
						num2 = 1f + MathF.Pow((float)num6, 0.67f);
						if (mobileParty.Army != null && mobileParty.Army.LeaderParty == mobileParty)
						{
							num2 = 1f + (num2 - 1f) / MathF.Sqrt((float)mobileParty.Army.Parties.Count);
						}
					}
				}
			}
			if (settlement == leaderHero.HomeSettlement && mobileParty.Army == null)
			{
				float num7 = (leaderHero.HomeSettlement.IsCastle ? 1.5f : 1f);
				if (currentSettlement == settlement)
				{
					num3 += 3000f * num7 / (250f + leaderHero.PassedTimeAtHomeSettlement * leaderHero.PassedTimeAtHomeSettlement);
				}
				else
				{
					num3 += 1000f * num7 / (250f + leaderHero.PassedTimeAtHomeSettlement * leaderHero.PassedTimeAtHomeSettlement);
				}
			}
			if (settlement != currentSettlement)
			{
				float num8 = 1f;
				if (mobileParty.LastVisitedSettlement == settlement)
				{
					num8 = 0.25f;
				}
				if (settlement.IsFortification && settlement.MapFaction == mapFaction && settlement.OwnerClan != Clan.PlayerClan)
				{
					float num9 = ((settlement.Town.GarrisonParty != null) ? settlement.Town.GarrisonParty.Party.TotalStrength : 0f);
					float num10 = FactionHelper.OwnerClanEconomyEffectOnGarrisonSizeConstant(settlement.OwnerClan);
					float num11 = FactionHelper.SettlementProsperityEffectOnGarrisonSizeConstant(settlement.Town);
					float num12 = FactionHelper.SettlementFoodPotentialEffectOnGarrisonSizeConstant(settlement);
					if (idealGarrisonStrengthPerWalledCenter == -1f)
					{
						idealGarrisonStrengthPerWalledCenter = FactionHelper.FindIdealGarrisonStrengthPerWalledCenter(mapFaction as Kingdom, null);
					}
					float num13 = idealGarrisonStrengthPerWalledCenter;
					if (settlement.Town.GarrisonParty != null && settlement.Town.GarrisonParty.HasLimitedWage())
					{
						num13 = (float)settlement.Town.GarrisonParty.PaymentLimit / Campaign.Current.AverageWage;
					}
					else
					{
						if (mobileParty.Army != null)
						{
							num13 *= 0.75f;
						}
						num13 *= num10 * num11 * num12;
					}
					float num14 = num13;
					if (num9 < num14)
					{
						float num15 = ((settlement.OwnerClan == leaderHero.Clan) ? 149f : 99f);
						if (settlement.OwnerClan == Clan.PlayerClan)
						{
							num15 *= 0.5f;
						}
						float num16 = 1f - num9 / num14;
						num4 = 1f + num15 * distanceScorePure * distanceScorePure * averagePartySizeRatioToMaximumSize * num16 * num16 * num16 * num8;
					}
				}
			}
			return new ValueTuple<float, float, float, float>(num, num2, num3, num4);
		}

		private float CalculateMergeScoreForDisbandingParty(MobileParty disbandParty, Settlement settlement, float distance)
		{
			float num = MathF.Pow(3.5f - 0.95f * (Math.Min(Campaign.MapDiagonal, distance) / Campaign.MapDiagonal), 3f);
			Hero owner = disbandParty.Party.Owner;
			float num2;
			if (((owner != null) ? owner.Clan : null) != settlement.OwnerClan)
			{
				Hero owner2 = disbandParty.Party.Owner;
				num2 = ((((owner2 != null) ? owner2.MapFaction : null) == settlement.MapFaction) ? 0.35f : 0.025f);
			}
			else
			{
				num2 = 1f;
			}
			float num3 = num2;
			float num4 = ((disbandParty.DefaultBehavior == AiBehavior.GoToSettlement && disbandParty.TargetSettlement == settlement) ? 1f : 0.3f);
			float num5 = (settlement.IsFortification ? 3f : 1f);
			float num6 = num * num3 * num4 * num5;
			if (num6 < 0.025f)
			{
				num6 = 0.035f;
			}
			return num6;
		}

		private float CalculateMergeScoreForLeaderlessParty(MobileParty leaderlessParty, Settlement settlement, float distance)
		{
			if (settlement.IsVillage)
			{
				return -1.6f;
			}
			float num = MathF.Pow(3.5f - 0.95f * (Math.Min(Campaign.MapDiagonal, distance) / Campaign.MapDiagonal), 3f);
			float num2;
			if (leaderlessParty.ActualClan != settlement.OwnerClan)
			{
				Clan actualClan = leaderlessParty.ActualClan;
				num2 = ((((actualClan != null) ? actualClan.MapFaction : null) == settlement.MapFaction) ? 0.35f : 0f);
			}
			else
			{
				num2 = 2f;
			}
			float num3 = num2;
			float num4 = ((leaderlessParty.DefaultBehavior == AiBehavior.GoToSettlement && leaderlessParty.TargetSettlement == settlement) ? 1f : 0.3f);
			float num5 = (settlement.IsFortification ? 3f : 0.5f);
			return num * num3 * num4 * num5;
		}

		private SortedList<ValueTuple<float, int>, Settlement> FindSettlementsToVisitWithDistances(MobileParty mobileParty)
		{
			SortedList<ValueTuple<float, int>, Settlement> sortedList = new SortedList<ValueTuple<float, int>, Settlement>();
			MapDistanceModel mapDistanceModel = Campaign.Current.Models.MapDistanceModel;
			if (mobileParty.LeaderHero != null && mobileParty.LeaderHero.MapFaction.IsKingdomFaction)
			{
				if (mobileParty.Army == null || mobileParty.Army.LeaderParty == mobileParty)
				{
					LocatableSearchData<Settlement> locatableSearchData = Settlement.StartFindingLocatablesAroundPosition(mobileParty.Position2D, 30f);
					for (Settlement settlement = Settlement.FindNextLocatable(ref locatableSearchData); settlement != null; settlement = Settlement.FindNextLocatable(ref locatableSearchData))
					{
						if (!settlement.IsCastle && this.IsSettlementSuitableForVisitingCondition(mobileParty, settlement))
						{
							float distance = mapDistanceModel.GetDistance(mobileParty, settlement);
							if (distance < 350f)
							{
								sortedList.Add(new ValueTuple<float, int>(distance, settlement.GetHashCode()), settlement);
							}
						}
					}
				}
				using (List<Settlement>.Enumerator enumerator = mobileParty.MapFaction.Settlements.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Settlement settlement2 = enumerator.Current;
						if (this.IsSettlementSuitableForVisitingCondition(mobileParty, settlement2))
						{
							float distance2 = mapDistanceModel.GetDistance(mobileParty, settlement2);
							if (distance2 < 350f && distance2 > 900f)
							{
								sortedList.Add(new ValueTuple<float, int>(distance2, settlement2.GetHashCode()), settlement2);
							}
						}
					}
					return sortedList;
				}
			}
			LocatableSearchData<Settlement> locatableSearchData2 = Settlement.StartFindingLocatablesAroundPosition(mobileParty.Position2D, 50f);
			for (Settlement settlement3 = Settlement.FindNextLocatable(ref locatableSearchData2); settlement3 != null; settlement3 = Settlement.FindNextLocatable(ref locatableSearchData2))
			{
				if (this.IsSettlementSuitableForVisitingCondition(mobileParty, settlement3))
				{
					float distance3 = mapDistanceModel.GetDistance(mobileParty, settlement3);
					if (distance3 < 350f)
					{
						sortedList.Add(new ValueTuple<float, int>(distance3, settlement3.GetHashCode()), settlement3);
					}
				}
			}
			return sortedList;
		}

		private void AddBehaviorTupleWithScore(PartyThinkParams p, Settlement settlement, float visitingNearbySettlementScore)
		{
			AIBehaviorTuple aibehaviorTuple = new AIBehaviorTuple(settlement, AiBehavior.GoToSettlement, false);
			float num;
			if (p.TryGetBehaviorScore(aibehaviorTuple, out num))
			{
				p.SetBehaviorScore(aibehaviorTuple, num + visitingNearbySettlementScore);
				return;
			}
			ValueTuple<AIBehaviorTuple, float> valueTuple = new ValueTuple<AIBehaviorTuple, float>(aibehaviorTuple, visitingNearbySettlementScore);
			p.AddBehaviorScore(valueTuple);
		}

		private bool IsSettlementSuitableForVisitingCondition(MobileParty mobileParty, Settlement settlement)
		{
			return settlement.Party.MapEvent == null && settlement.Party.SiegeEvent == null && (!mobileParty.Party.Owner.MapFaction.IsAtWarWith(settlement.MapFaction) || (mobileParty.Party.Owner.MapFaction.IsMinorFaction && settlement.IsVillage)) && (settlement.IsVillage || settlement.IsFortification) && (!settlement.IsVillage || settlement.Village.VillageState == Village.VillageStates.Normal);
		}

		private const float NumberOfHoursAtDay = 24f;

		private const float IdealTimePeriodForVisitingOwnedSettlement = 360f;

		private const float DefaultMoneyLimitForRecruiting = 2000f;

		private const float MaximumMeaningfulDistance = 250f;

		private const float MaximumFilteredDistance = 350f;

		private const float MeaningfulScoreThreshold = 0.025f;

		private const float GoodEnoughScore = 2.5f;

		private const float BaseVisitScore = 1.6f;

		private IDisbandPartyCampaignBehavior _disbandPartyCampaignBehavior;
	}
}
