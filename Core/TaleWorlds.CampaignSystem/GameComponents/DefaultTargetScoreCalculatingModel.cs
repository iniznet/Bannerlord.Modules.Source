using System;
using System.Linq;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultTargetScoreCalculatingModel : TargetScoreCalculatingModel
	{
		public override float TravelingToAssignmentFactor
		{
			get
			{
				return 1.33f;
			}
		}

		public override float BesiegingFactor
		{
			get
			{
				return 1.67f;
			}
		}

		public override float AssaultingTownFactor
		{
			get
			{
				return 2f;
			}
		}

		public override float RaidingFactor
		{
			get
			{
				return 1.67f;
			}
		}

		public override float DefendingFactor
		{
			get
			{
				return 1.67f;
			}
		}

		private float ReasonableDistanceForBesiegingTown
		{
			get
			{
				return (127f + 2.27f * Campaign.AverageDistanceBetweenTwoFortifications) / 2f;
			}
		}

		private float ReasonableDistanceForBesiegingCastle
		{
			get
			{
				return (106f + 1.89f * Campaign.AverageDistanceBetweenTwoFortifications) / 2f;
			}
		}

		private float ReasonableDistanceForRaiding
		{
			get
			{
				return (106f + 1.89f * Campaign.AverageDistanceBetweenTwoFortifications) / 2f;
			}
		}

		private float ReasonableDistanceForDefendingTownOrCastle
		{
			get
			{
				return (160f + 2.84f * Campaign.AverageDistanceBetweenTwoFortifications) / 2f;
			}
		}

		private float ReasonableDistanceForDefendingVillage
		{
			get
			{
				return (80f + 1.42f * Campaign.AverageDistanceBetweenTwoFortifications) / 2f;
			}
		}

		private float DistanceOfMobilePartyDivider
		{
			get
			{
				return (254f + 4.54f * Campaign.AverageDistanceBetweenTwoFortifications) / 2f;
			}
		}

		private float RaidDistanceLimit
		{
			get
			{
				return (318f + 5.68f * Campaign.AverageDistanceBetweenTwoFortifications) / 2f;
			}
		}

		private float GiveUpDistanceLimit
		{
			get
			{
				return (127f + 2.27f * Campaign.AverageDistanceBetweenTwoFortifications) / 2f;
			}
		}

		public override float CurrentObjectiveValue(MobileParty mobileParty)
		{
			float totalStrengthWithFollowers = mobileParty.GetTotalStrengthWithFollowers(false);
			float num = 0f;
			if (mobileParty.TargetSettlement == null)
			{
				return 0f;
			}
			if (mobileParty.DefaultBehavior != AiBehavior.BesiegeSettlement && mobileParty.DefaultBehavior != AiBehavior.RaidSettlement && mobileParty.DefaultBehavior != AiBehavior.DefendSettlement)
			{
				return num;
			}
			int count = mobileParty.TargetSettlement.MapFaction.Settlements.Count;
			float totalStrength = mobileParty.TargetSettlement.MapFaction.TotalStrength;
			num = this.GetTargetScoreForFaction(mobileParty.TargetSettlement, (mobileParty.DefaultBehavior == AiBehavior.BesiegeSettlement) ? Army.ArmyTypes.Besieger : ((mobileParty.DefaultBehavior == AiBehavior.RaidSettlement) ? Army.ArmyTypes.Raider : Army.ArmyTypes.Defender), mobileParty, totalStrengthWithFollowers, count, totalStrength);
			AiBehavior defaultBehavior = mobileParty.DefaultBehavior;
			if (defaultBehavior != AiBehavior.RaidSettlement)
			{
				if (defaultBehavior != AiBehavior.BesiegeSettlement)
				{
					if (defaultBehavior == AiBehavior.DefendSettlement)
					{
						num *= ((mobileParty.Party.MapEvent != null && mobileParty.MapEvent.MapEventSettlement == mobileParty.TargetSettlement) ? this.DefendingFactor : this.TravelingToAssignmentFactor);
					}
				}
				else
				{
					num *= ((mobileParty.Party.MapEvent == null && mobileParty.TargetSettlement.SiegeEvent != null && mobileParty.TargetSettlement.SiegeEvent.BesiegerCamp.HasInvolvedPartyForEventType(mobileParty.Party, MapEvent.BattleTypes.Siege)) ? this.BesiegingFactor : ((mobileParty.Party.MapEvent != null && mobileParty.Party.MapEvent.MapEventSettlement == mobileParty.TargetSettlement) ? this.AssaultingTownFactor : this.TravelingToAssignmentFactor));
				}
			}
			else
			{
				num *= ((mobileParty.Party.MapEvent != null && mobileParty.MapEvent.MapEventSettlement == mobileParty.TargetSettlement) ? this.RaidingFactor : this.TravelingToAssignmentFactor);
			}
			return num;
		}

		public override float CalculatePatrollingScoreForSettlement(Settlement settlement, MobileParty mobileParty)
		{
			bool flag = mobileParty.Army != null && mobileParty.Army.LeaderParty == mobileParty && mobileParty.Army.AIBehavior != Army.AIBehaviorFlags.Gathering && mobileParty.Army.AIBehavior != Army.AIBehaviorFlags.WaitingForArmyMembers;
			if (mobileParty.Army != null && !flag && mobileParty.Army.Cohesion > (float)mobileParty.Army.CohesionThresholdForDispersion)
			{
				return 0f;
			}
			float num = ((mobileParty.LeaderHero != null && mobileParty.LeaderHero.Clan != null && mobileParty.LeaderHero.Clan.Fiefs.Count > 0) ? (mobileParty.LeaderHero.RandomFloat(0.2f, 0.4f) + (float)MathF.Min(4, mobileParty.LeaderHero.Clan.Fiefs.Count - 1) * 0.05f + mobileParty.LeaderHero.RandomFloatWithSeed((uint)CampaignTime.Now.ToHours, 0.2f)) : 0.5f);
			float num2 = 1f - num;
			Hero leaderHero = mobileParty.LeaderHero;
			float num3 = num2 + ((((leaderHero != null) ? leaderHero.Clan : null) != null && (settlement.OwnerClan == mobileParty.LeaderHero.Clan || mobileParty.LeaderHero.Clan.Settlements.Count == 0)) ? num : 0f);
			float length = (settlement.Position2D - mobileParty.Position2D).Length;
			float num4 = 1f - (length + 50f) / Campaign.MapDiagonal;
			num4 = (mobileParty.IsBandit ? (num4 * num4 * num4 * num4 * num4 * num4) : (num4 * num4 * num4 * num4));
			float num5 = 1f;
			if (settlement.MapFaction == mobileParty.MapFaction)
			{
				float numberOfEnemiesSpottedAround = settlement.NumberOfEnemiesSpottedAround;
				float numberOfAlliesSpottedAround = settlement.NumberOfAlliesSpottedAround;
				float num6 = MathF.Max(0f, numberOfEnemiesSpottedAround - numberOfAlliesSpottedAround * 0.25f);
				if (num6 > 1f)
				{
					int num7 = 0;
					foreach (WarPartyComponent warPartyComponent in mobileParty.MapFaction.WarPartyComponents)
					{
						MobileParty mobileParty2 = warPartyComponent.MobileParty;
						if (mobileParty2 != mobileParty && (mobileParty2.Army == null || mobileParty2.Army != mobileParty.Army) && (mobileParty2.Army == null || mobileParty2.Army.LeaderParty == mobileParty) && mobileParty2.DefaultBehavior == AiBehavior.PatrolAroundPoint && mobileParty2.TargetSettlement == settlement)
						{
							num7++;
						}
					}
					num5 += MathF.Pow(MathF.Min(10f, num6), 0.25f) - (float)num7;
				}
				else
				{
					num5 += num6;
				}
			}
			float num8 = ((mobileParty.Army != null && mobileParty.Army.LeaderParty != mobileParty) ? (((float)mobileParty.Army.CohesionThresholdForDispersion - mobileParty.Army.Cohesion) / (float)mobileParty.Army.CohesionThresholdForDispersion) : 1f);
			return (mobileParty.MapFaction.IsMinorFaction ? 1.5f : 1f) * num5 * num3 * num4 * num8 * 0.36f;
		}

		public override float GetTargetScoreForFaction(Settlement targetSettlement, Army.ArmyTypes missionType, MobileParty mobileParty, float ourStrength, int numberOfEnemyFactionSettlements = -1, float totalEnemyMobilePartyStrength = -1f)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			return this.GetTargetScoreForFaction(targetSettlement, missionType, mobileParty, ourStrength, out num, out num2, out num3, numberOfEnemyFactionSettlements, totalEnemyMobilePartyStrength);
		}

		private float GetTargetScoreForFaction(Settlement targetSettlement, Army.ArmyTypes missionType, MobileParty mobileParty, float ourStrength, out float powerScore, out float distanceScore, out float settlementImportanceScore, int numberOfEnemyFactionSettlements = -1, float totalEnemyMobilePartyStrength = -1f)
		{
			IFaction mapFaction = mobileParty.MapFaction;
			if (((missionType == Army.ArmyTypes.Besieger || missionType == Army.ArmyTypes.Raider) && !FactionManager.IsAtWarAgainstFaction(targetSettlement.MapFaction, mapFaction)) || (missionType == Army.ArmyTypes.Raider && (targetSettlement.Village.VillageState != Village.VillageStates.Normal || targetSettlement.Party.MapEvent != null) && (mobileParty.MapEvent == null || mobileParty.MapEvent.MapEventSettlement != targetSettlement)) || (missionType == Army.ArmyTypes.Besieger && (targetSettlement.Party.MapEvent != null || targetSettlement.SiegeEvent != null) && (targetSettlement.SiegeEvent == null || targetSettlement.SiegeEvent.BesiegerCamp.LeaderParty.MapFaction != mobileParty.MapFaction) && (mobileParty.MapEvent == null || mobileParty.MapEvent.MapEventSettlement != targetSettlement)) || (missionType == Army.ArmyTypes.Defender && (targetSettlement.LastAttackerParty == null || !targetSettlement.LastAttackerParty.IsActive || targetSettlement.LastAttackerParty.MapFaction == mobileParty.MapFaction || targetSettlement.MapFaction != mobileParty.MapFaction)))
			{
				powerScore = 0f;
				distanceScore = 0f;
				settlementImportanceScore = 0f;
				return 0f;
			}
			if (mobileParty.Objective == MobileParty.PartyObjective.Defensive && (missionType == Army.ArmyTypes.Besieger || missionType == Army.ArmyTypes.Raider))
			{
				powerScore = 0f;
				distanceScore = 0f;
				settlementImportanceScore = 0f;
				return 0f;
			}
			if (mobileParty.Objective == MobileParty.PartyObjective.Aggressive && (missionType == Army.ArmyTypes.Defender || missionType == Army.ArmyTypes.Patrolling))
			{
				powerScore = 0f;
				distanceScore = 0f;
				settlementImportanceScore = 0f;
				return 0f;
			}
			if (missionType == Army.ArmyTypes.Defender)
			{
				MobileParty lastAttackerParty = targetSettlement.LastAttackerParty;
				if (lastAttackerParty == null || !mobileParty.MapFaction.IsAtWarWith(lastAttackerParty.MapFaction))
				{
					powerScore = 0f;
					distanceScore = 0f;
					settlementImportanceScore = 0f;
					return 0f;
				}
			}
			if (mobileParty.Army == null && missionType == Army.ArmyTypes.Besieger && ((targetSettlement.Party.MapEvent != null && targetSettlement.Party.MapEvent.AttackerSide.LeaderParty != mobileParty.Party) || (targetSettlement.Party.SiegeEvent != null && mobileParty.BesiegedSettlement != targetSettlement)))
			{
				powerScore = 0f;
				distanceScore = 0f;
				settlementImportanceScore = 0f;
				return 0f;
			}
			float distance = Campaign.Current.Models.MapDistanceModel.GetDistance(mapFaction.FactionMidSettlement, targetSettlement);
			float distance2 = Campaign.Current.Models.MapDistanceModel.GetDistance(mobileParty, targetSettlement);
			float num = Campaign.MapDiagonalSquared;
			float num2 = Campaign.MapDiagonalSquared;
			int num3 = 0;
			int num4 = 0;
			Settlement settlement = null;
			Settlement settlement2 = null;
			foreach (Settlement settlement3 in mobileParty.MapFaction.Settlements)
			{
				if (settlement3.IsTown)
				{
					float num5 = settlement3.Position2D.DistanceSquared(targetSettlement.Position2D);
					if (num > num5)
					{
						num = num5;
						settlement = settlement3;
					}
					if (num2 > num5)
					{
						num2 = num5;
						settlement2 = settlement3;
					}
					num3++;
					num4++;
				}
				else if (settlement3.IsCastle)
				{
					float num6 = settlement3.Position2D.DistanceSquared(targetSettlement.Position2D);
					if (num2 > num6)
					{
						num2 = num6;
						settlement2 = settlement3;
					}
					num4++;
				}
			}
			if (settlement2 != null)
			{
				num2 = Campaign.Current.Models.MapDistanceModel.GetDistance(targetSettlement, settlement2);
			}
			if (settlement == settlement2)
			{
				num = num2;
			}
			else if (settlement != null)
			{
				num = Campaign.Current.Models.MapDistanceModel.GetDistance(targetSettlement, settlement);
			}
			float num7 = 1f;
			float num8 = MathF.Min(2f, MathF.Sqrt((float)num4)) / 2f;
			float num9 = MathF.Min(2f, MathF.Sqrt((float)num3)) / 2f;
			if (num8 > 0f && num9 < 1f)
			{
				num8 += 1f - num9;
			}
			num7 += 0.5f * (2f - (num8 + num9));
			float num10 = ((missionType == Army.ArmyTypes.Raider) ? (MathF.Max(0f, distance - Campaign.AverageDistanceBetweenTwoFortifications) * 0.15f + distance2 * 0.5f * num7 + num * 0.2f * num9 + num2 * 0.15f * num8) : ((missionType == Army.ArmyTypes.Besieger) ? (MathF.Max(0f, distance - Campaign.AverageDistanceBetweenTwoFortifications) * 0.15f + distance2 * 0.15f * num7 + num * 0.5f * num9 + num2 * 0.2f * num8) : (MathF.Max(0f, distance - Campaign.AverageDistanceBetweenTwoFortifications) * 0.15f + distance2 * 0.5f * num7 + num * 0.25f * num9 + num2 * 0.1f * num8)));
			float num11 = ((missionType == Army.ArmyTypes.Defender) ? (targetSettlement.IsVillage ? this.ReasonableDistanceForDefendingVillage : this.ReasonableDistanceForDefendingTownOrCastle) : ((missionType == Army.ArmyTypes.Besieger) ? (targetSettlement.IsTown ? this.ReasonableDistanceForBesiegingTown : this.ReasonableDistanceForBesiegingCastle) : this.ReasonableDistanceForRaiding));
			distanceScore = ((num10 < num11) ? (1f + (1f - num10 / num11) * 0.5f) : (num11 / num10 * (num11 / num10) * ((missionType != Army.ArmyTypes.Defender) ? (num11 / num10) : 1f)));
			if (distanceScore < 0.1f)
			{
				powerScore = 0f;
				distanceScore = 0f;
				settlementImportanceScore = 0f;
				return 0f;
			}
			float num12 = 1f;
			if (mobileParty.Army != null && mobileParty.Army.Cohesion < 40f)
			{
				num12 *= mobileParty.Army.Cohesion / 40f;
			}
			if (num12 < 0.25f)
			{
				powerScore = 0f;
				distanceScore = 0f;
				settlementImportanceScore = 0f;
				return 0f;
			}
			if (missionType == Army.ArmyTypes.Defender)
			{
				float num13 = 0f;
				float num14 = 0f;
				foreach (WarPartyComponent warPartyComponent in mapFaction.WarPartyComponents)
				{
					MobileParty mobileParty2 = warPartyComponent.MobileParty;
					if (mobileParty2 != mobileParty && (mobileParty2.Army == null || mobileParty2.Army != mobileParty.Army) && mobileParty2.AttachedTo == null)
					{
						if (mobileParty2.Army != null)
						{
							Army army = mobileParty2.Army;
							if (((army.AIBehavior == Army.AIBehaviorFlags.Gathering || army.AIBehavior == Army.AIBehaviorFlags.WaitingForArmyMembers) && army.AiBehaviorObject == targetSettlement) || (army.AIBehavior != Army.AIBehaviorFlags.Gathering && army.AIBehavior != Army.AIBehaviorFlags.WaitingForArmyMembers && army.AiBehaviorObject == targetSettlement) || (army.LeaderParty.TargetParty != null && (army.LeaderParty.TargetParty == targetSettlement.LastAttackerParty || (army.LeaderParty.TargetParty.MapEvent != null && army.LeaderParty.TargetParty.MapEvent == targetSettlement.LastAttackerParty.MapEvent) || (army.LeaderParty.TargetParty.BesiegedSettlement != null && army.LeaderParty.TargetParty.BesiegedSettlement == targetSettlement.LastAttackerParty.BesiegedSettlement))))
							{
								num14 += army.TotalStrength;
							}
						}
						else if ((mobileParty2.DefaultBehavior == AiBehavior.DefendSettlement && mobileParty2.TargetSettlement == targetSettlement) || (mobileParty2.TargetParty != null && (mobileParty2.TargetParty == targetSettlement.LastAttackerParty || (mobileParty2.TargetParty.MapEvent != null && mobileParty2.TargetParty.MapEvent == targetSettlement.LastAttackerParty.MapEvent) || (mobileParty2.TargetParty.BesiegedSettlement != null && mobileParty2.TargetParty.BesiegedSettlement == targetSettlement.LastAttackerParty.BesiegedSettlement))))
						{
							num14 += mobileParty2.Party.TotalStrength;
						}
					}
				}
				MobileParty lastAttackerParty2 = targetSettlement.LastAttackerParty;
				if ((targetSettlement.LastAttackerParty.MapEvent != null && targetSettlement.LastAttackerParty.MapEvent.MapEventSettlement == targetSettlement) || targetSettlement.LastAttackerParty.BesiegedSettlement == targetSettlement)
				{
					LocatableSearchData<MobileParty> locatableSearchData = MobileParty.StartFindingLocatablesAroundPosition(targetSettlement.GatePosition, 6f);
					for (MobileParty mobileParty3 = MobileParty.FindNextLocatable(ref locatableSearchData); mobileParty3 != null; mobileParty3 = MobileParty.FindNextLocatable(ref locatableSearchData))
					{
						if (mobileParty3.Aggressiveness > 0f && mobileParty3.MapFaction == lastAttackerParty2.MapFaction)
						{
							num13 += ((mobileParty3.Aggressiveness > 0.5f) ? 1f : (mobileParty3.Aggressiveness * 2f)) * mobileParty3.Party.TotalStrength;
						}
					}
				}
				else
				{
					Army army2 = lastAttackerParty2.Army;
					num13 = ((army2 != null) ? army2.TotalStrength : lastAttackerParty2.Party.TotalStrength);
				}
				float num15 = ourStrength + num14;
				float num16 = MathF.Max(100f, num13) * 1.1f;
				float num17 = num16 * 2.5f;
				powerScore = ((num15 >= num17) ? (num17 / num15 * (num17 / num15)) : MathF.Min(1f, num15 / num16 * (num15 / num16)));
				if (num15 < num16)
				{
					powerScore *= 0.9f;
				}
				if (ourStrength < num13)
				{
					powerScore *= MathF.Pow(ourStrength / num13, 0.25f);
				}
			}
			else
			{
				float num18 = targetSettlement.Party.TotalStrength;
				float num19 = 0f;
				bool flag = Hero.MainHero.CurrentSettlement == targetSettlement;
				foreach (MobileParty mobileParty4 in targetSettlement.Parties)
				{
					if (mobileParty4.Aggressiveness > 0.01f || mobileParty4.IsGarrison || mobileParty4.IsMilitia)
					{
						float num20 = ((mobileParty4 == MobileParty.MainParty) ? 0.5f : ((mobileParty4.Army != null && mobileParty4.Army.LeaderParty == MobileParty.MainParty) ? 0.8f : 1f));
						float num21 = (flag ? 0.8f : 1f);
						num18 += num20 * num21 * mobileParty4.Party.TotalStrength;
						if (!mobileParty4.IsGarrison && !mobileParty4.IsMilitia && mobileParty4.LeaderHero != null)
						{
							num19 += num20 * num21 * mobileParty4.Party.TotalStrength;
						}
					}
				}
				float num22 = 0f;
				float num23 = ((missionType == Army.ArmyTypes.Besieger && mobileParty.BesiegedSettlement != targetSettlement) ? (targetSettlement.IsTown ? 4f : 3f) : 1f);
				float num24 = MathF.Min(1f, distance2 / this.DistanceOfMobilePartyDivider);
				num23 *= 1f - 0.6f * (1f - num24) * (1f - num24);
				if (num18 < 100f && missionType == Army.ArmyTypes.Besieger)
				{
					num23 *= 0.5f + 0.5f * (num18 / 100f);
				}
				if ((mobileParty.MapEvent == null || mobileParty.MapEvent.MapEventSettlement != targetSettlement) && targetSettlement.MapFaction.IsKingdomFaction)
				{
					if (numberOfEnemyFactionSettlements < 0)
					{
						numberOfEnemyFactionSettlements = targetSettlement.MapFaction.Settlements.Count;
					}
					if (totalEnemyMobilePartyStrength < 0f)
					{
						totalEnemyMobilePartyStrength = targetSettlement.MapFaction.TotalStrength;
					}
					totalEnemyMobilePartyStrength *= 0.5f;
					float num25 = (totalEnemyMobilePartyStrength - num19) / ((float)numberOfEnemyFactionSettlements + 10f);
					num22 = MathF.Max(0f, num25) * num23;
				}
				float num26 = ((missionType == Army.ArmyTypes.Besieger) ? (1.25f + 0.25f * (float)targetSettlement.Town.GetWallLevel()) : 1f);
				if (missionType == Army.ArmyTypes.Besieger && targetSettlement.Town.FoodStocks < 100f)
				{
					num26 -= 0.5f * (num26 - 1f) * ((100f - targetSettlement.Town.FoodStocks) / 100f);
				}
				float num27 = ((missionType == Army.ArmyTypes.Besieger && mobileParty.LeaderHero != null) ? (mobileParty.LeaderHero.RandomFloat(0.1f) + (MathF.Max(MathF.Min(1.2f, mobileParty.Aggressiveness), 0.8f) - 0.8f) * 0.5f) : 0f);
				float num28 = num18 * (num26 - num27) + num22 + 0.1f;
				if (ourStrength < num28 * ((missionType == Army.ArmyTypes.Besieger) ? 1f : 0.6f))
				{
					powerScore = 0f;
					settlementImportanceScore = 1f;
					return 0f;
				}
				float num29 = 0f;
				if ((missionType == Army.ArmyTypes.Besieger && distance2 < this.RaidDistanceLimit) || (missionType == Army.ArmyTypes.Raider && targetSettlement.Party.MapEvent != null))
				{
					LocatableSearchData<MobileParty> locatableSearchData2 = MobileParty.StartFindingLocatablesAroundPosition((mobileParty.SiegeEvent != null && mobileParty.SiegeEvent.BesiegedSettlement == targetSettlement) ? mobileParty.Position2D : targetSettlement.GatePosition, 9f);
					for (MobileParty mobileParty5 = MobileParty.FindNextLocatable(ref locatableSearchData2); mobileParty5 != null; mobileParty5 = MobileParty.FindNextLocatable(ref locatableSearchData2))
					{
						if (mobileParty5.CurrentSettlement != targetSettlement && mobileParty5.Aggressiveness > 0.01f && mobileParty5.MapFaction == targetSettlement.Party.MapFaction)
						{
							float num30 = ((mobileParty5 == MobileParty.MainParty || (mobileParty5.Army != null && mobileParty5.Army.LeaderParty == MobileParty.MainParty)) ? 0.5f : 1f);
							float num31 = 1f;
							if (mobileParty.MapEvent != null && mobileParty.MapEvent.MapEventSettlement == targetSettlement)
							{
								float num32 = mobileParty5.Position2D.Distance(mobileParty.Position2D);
								num31 = 1f - num32 / 16f;
							}
							num29 += num31 * mobileParty5.Party.TotalStrength * num30;
						}
					}
					if (num29 < ourStrength)
					{
						num29 = MathF.Max(0f, num29 - ourStrength * 0.33f);
					}
					num28 += num29;
					num28 -= num22;
					if (targetSettlement.MapFaction.IsKingdomFaction)
					{
						if (numberOfEnemyFactionSettlements < 0)
						{
							numberOfEnemyFactionSettlements = targetSettlement.MapFaction.Settlements.Count;
						}
						if (totalEnemyMobilePartyStrength < 0f)
						{
							totalEnemyMobilePartyStrength = targetSettlement.MapFaction.TotalStrength;
						}
						totalEnemyMobilePartyStrength *= 0.5f;
						float num33 = (totalEnemyMobilePartyStrength - (num19 + num29)) / ((float)numberOfEnemyFactionSettlements + 10f);
						num22 = MathF.Max(0f, num33) * num23;
					}
					num28 += num22;
				}
				float num34 = ((missionType == Army.ArmyTypes.Raider) ? 0.6f : 0.4f);
				float num35 = ((missionType == Army.ArmyTypes.Raider) ? 0.9f : 0.8f);
				float num36 = ((missionType == Army.ArmyTypes.Raider) ? 2.5f : 3f);
				float num37 = ourStrength / num28;
				powerScore = ((ourStrength > num28 * num36) ? 1f : ((num37 > 2f) ? (num35 + (1f - num35) * ((num37 - 2f) / (num36 - 2f))) : ((num37 > 1f) ? (num34 + (num35 - num34) * ((num37 - 1f) / 1f)) : (num34 * 0.9f * num37 * num37))));
			}
			powerScore = ((powerScore > 1f) ? 1f : powerScore);
			float num38 = ((missionType == Army.ArmyTypes.Raider) ? targetSettlement.GetSettlementValueForEnemyHero(mobileParty.LeaderHero) : targetSettlement.GetSettlementValueForFaction(mapFaction));
			float num39 = (targetSettlement.IsVillage ? 0.5f : 0.33f);
			settlementImportanceScore = MathF.Pow(num38 / 50000f, num39);
			float num40 = 1f;
			if (missionType == Army.ArmyTypes.Raider)
			{
				if (targetSettlement.Village.Bound.Town.FoodStocks < 100f)
				{
					settlementImportanceScore *= 1f + 0.3f * ((100f - targetSettlement.Village.Bound.Town.FoodStocks) / 100f);
				}
				settlementImportanceScore *= 1.5f;
				num40 += ((mobileParty.Army != null) ? 0.5f : 1f) * ((mobileParty.LeaderHero != null && mobileParty.LeaderHero.Clan != null && mobileParty.LeaderHero.Clan.Gold < 10000) ? ((10000f - (float)mobileParty.LeaderHero.Clan.Gold) / 20000f) : 0f);
			}
			float num41 = ((missionType == Army.ArmyTypes.Defender) ? (targetSettlement.IsVillage ? 1.28f : 1.28f) : ((missionType == Army.ArmyTypes.Besieger) ? 0.8f : (0.28f * (1f + (1f - targetSettlement.SettlementHitPoints)))));
			if (missionType == Army.ArmyTypes.Defender && ((targetSettlement.IsFortification && targetSettlement.LastAttackerParty.BesiegedSettlement != targetSettlement) || (!targetSettlement.IsFortification && targetSettlement.LastAttackerParty.MapEvent == null)))
			{
				MobileParty lastAttackerParty3 = targetSettlement.LastAttackerParty;
				float distance3 = Campaign.Current.Models.MapDistanceModel.GetDistance(lastAttackerParty3, targetSettlement);
				float num42 = MathF.Min(this.GiveUpDistanceLimit, distance3) / this.GiveUpDistanceLimit;
				num41 = num42 * 0.8f + (1f - num42) * num41;
			}
			float num43 = 1f;
			if ((missionType == Army.ArmyTypes.Raider || missionType == Army.ArmyTypes.Besieger) && targetSettlement.OwnerClan != null && mobileParty.LeaderHero != null)
			{
				int relationWithClan = mobileParty.LeaderHero.Clan.GetRelationWithClan(targetSettlement.OwnerClan);
				if (relationWithClan > 0)
				{
					num43 = 1f - ((missionType == Army.ArmyTypes.Besieger) ? 0.4f : 0.8f) * (MathF.Sqrt((float)relationWithClan) / 10f);
				}
				else if (relationWithClan < 0)
				{
					num43 = 1f + ((missionType == Army.ArmyTypes.Besieger) ? 0.1f : 0.05f) * (MathF.Sqrt((float)(-(float)relationWithClan)) / 10f);
				}
			}
			float num44 = 1f;
			if (mobileParty.MapFaction != null && mobileParty.MapFaction.IsKingdomFaction && mobileParty.MapFaction.Leader == Hero.MainHero && (missionType != Army.ArmyTypes.Defender || (targetSettlement.LastAttackerParty != null && targetSettlement.LastAttackerParty.MapFaction != Hero.MainHero.MapFaction)))
			{
				StanceLink stanceLink = ((missionType != Army.ArmyTypes.Defender) ? Hero.MainHero.MapFaction.GetStanceWith(targetSettlement.MapFaction) : Hero.MainHero.MapFaction.GetStanceWith(targetSettlement.LastAttackerParty.MapFaction));
				if (stanceLink != null)
				{
					if (stanceLink.BehaviorPriority == 1)
					{
						if (missionType == Army.ArmyTypes.Besieger || missionType == Army.ArmyTypes.Raider)
						{
							num44 = 0.65f;
						}
						else if (missionType == Army.ArmyTypes.Defender)
						{
							num44 = 1.1f;
						}
					}
					else if (stanceLink.BehaviorPriority == 2 && (missionType == Army.ArmyTypes.Besieger || missionType == Army.ArmyTypes.Raider))
					{
						num44 = 1.3f;
					}
				}
			}
			float num45 = 1f;
			if (mobileParty.SiegeEvent != null && mobileParty.SiegeEvent.BesiegedSettlement == targetSettlement)
			{
				num45 = 4f;
			}
			float num46 = 1f;
			if (missionType == Army.ArmyTypes.Raider && mobileParty.MapEvent != null && mobileParty.MapEvent.IsRaid)
			{
				num46 = ((mobileParty.MapEvent.MapEventSettlement == targetSettlement) ? 1.3f : 0.3f);
			}
			float num47 = 1f;
			if (targetSettlement.SiegeEvent != null && targetSettlement.SiegeEvent.BesiegerCamp.LeaderParty.MapFaction == mobileParty.MapFaction)
			{
				float num48 = targetSettlement.SiegeEvent.BesiegerCamp.GetInvolvedPartiesForEventType(MapEvent.BattleTypes.Siege).Sum((PartyBase x) => x.TotalStrength) / targetSettlement.GetInvolvedPartiesForEventType(MapEvent.BattleTypes.Siege).Sum((PartyBase x) => x.TotalStrength);
				num47 += Math.Max(0f, 3f - num48);
			}
			float num49 = num43 * distanceScore * powerScore * settlementImportanceScore * num40 * num41 * num44 * num12 * num45 * num46 * num47;
			if (mobileParty.Objective == MobileParty.PartyObjective.Defensive && missionType == Army.ArmyTypes.Defender)
			{
				num49 *= 1.2f;
			}
			else if (mobileParty.Objective == MobileParty.PartyObjective.Aggressive && (missionType == Army.ArmyTypes.Besieger || missionType == Army.ArmyTypes.Raider))
			{
				num49 *= 1.2f;
			}
			return (num49 < 0f) ? 0f : num49;
		}

		private const float SiegeBaseValueFactor = 0.8f;

		private const float RaidBaseValueFactor = 0.28f;

		private const float DefenceBaseValueFactor = 1.28f;

		private const float DefenceVillageBaseValueFactor = 1.28f;

		private const float DefenceFollowEnemyBaseValueFactor = 0.8f;

		private const float MeaningfulCohesionThresholdForArmy = 40f;
	}
}
