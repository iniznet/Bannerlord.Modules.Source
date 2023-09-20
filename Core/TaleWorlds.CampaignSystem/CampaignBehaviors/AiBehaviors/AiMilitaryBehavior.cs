using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors.AiBehaviors
{
	public class AiMilitaryBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
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

		private void OnSettlementEntered(MobileParty mobileParty, Settlement settlement, Hero hero)
		{
			if (mobileParty != null && mobileParty.IsBandit && settlement.IsHideout && mobileParty.DefaultBehavior != AiBehavior.GoToSettlement)
			{
				mobileParty.Ai.SetMoveGoToSettlement(settlement);
			}
		}

		private void FindBestTargetAndItsValueForFaction(Army.ArmyTypes missionType, PartyThinkParams p, float ourStrength, float newArmyCreatingAdditionalConstant = 1f)
		{
			MobileParty mobilePartyOf = p.MobilePartyOf;
			IFaction mapFaction = mobilePartyOf.MapFaction;
			if (mobilePartyOf.Army != null && mobilePartyOf.Army.LeaderParty != mobilePartyOf)
			{
				return;
			}
			float num4;
			if (mobilePartyOf.Army != null)
			{
				float num = 0f;
				foreach (MobileParty mobileParty in mobilePartyOf.Army.Parties)
				{
					float num2 = (PartyBaseHelper.FindPartySizeNormalLimit(mobileParty) + 1f) * 0.5f;
					float num3 = mobileParty.PartySizeRatio / num2;
					num += num3;
				}
				num4 = num / (float)mobilePartyOf.Army.Parties.Count;
			}
			else if (newArmyCreatingAdditionalConstant <= 1.01f)
			{
				float num5 = (PartyBaseHelper.FindPartySizeNormalLimit(mobilePartyOf) + 1f) * 0.5f;
				num4 = mobilePartyOf.PartySizeRatio / num5;
			}
			else
			{
				num4 = 1f;
			}
			float num6 = MathF.Max(1f, MathF.Min((float)mobilePartyOf.MapFaction.Fiefs.Count / 5f, 2.5f));
			if (missionType == Army.ArmyTypes.Defender)
			{
				num6 = MathF.Pow(num6, 0.75f);
			}
			float num7 = MathF.Min(1f, MathF.Pow(num4, num6));
			AiBehavior aiBehavior = AiBehavior.Hold;
			switch (missionType)
			{
			case Army.ArmyTypes.Besieger:
				aiBehavior = AiBehavior.BesiegeSettlement;
				break;
			case Army.ArmyTypes.Raider:
				aiBehavior = AiBehavior.RaidSettlement;
				break;
			case Army.ArmyTypes.Defender:
				aiBehavior = AiBehavior.DefendSettlement;
				break;
			case Army.ArmyTypes.Patrolling:
				aiBehavior = AiBehavior.PatrolAroundPoint;
				break;
			}
			if (missionType == Army.ArmyTypes.Defender || missionType == Army.ArmyTypes.Patrolling)
			{
				this.CalculateMilitaryBehaviorForFactionSettlementsParallel(mapFaction, p, missionType, aiBehavior, ourStrength, num7, newArmyCreatingAdditionalConstant);
				return;
			}
			foreach (IFaction faction in FactionManager.GetEnemyFactions(mapFaction))
			{
				this.CalculateMilitaryBehaviorForFactionSettlementsParallel(faction, p, missionType, aiBehavior, ourStrength, num7, newArmyCreatingAdditionalConstant);
			}
		}

		private void CalculateMilitaryBehaviorForFactionSettlementsParallel(IFaction faction, PartyThinkParams p, Army.ArmyTypes missionType, AiBehavior aiBehavior, float ourStrength, float partySizeScore, float newArmyCreatingAdditionalConstant)
		{
			MobileParty mobilePartyOf = p.MobilePartyOf;
			int count = faction.Settlements.Count;
			float totalStrength = faction.TotalStrength;
			for (int i = 0; i < faction.Settlements.Count; i++)
			{
				Settlement settlement = faction.Settlements[i];
				if (this.CheckIfSettlementIsSuitableForMilitaryAction(settlement, mobilePartyOf, missionType))
				{
					this.CalculateMilitaryBehaviorForSettlement(settlement, missionType, aiBehavior, p, ourStrength, partySizeScore, count, totalStrength, newArmyCreatingAdditionalConstant);
				}
			}
		}

		private bool CheckIfSettlementIsSuitableForMilitaryAction(Settlement settlement, MobileParty mobileParty, Army.ArmyTypes missionType)
		{
			if (Game.Current.CheatMode && !CampaignCheats.MainPartyIsAttackable && settlement.Party.MapEvent != null && settlement.Party.MapEvent == MapEvent.PlayerMapEvent)
			{
				return false;
			}
			if (((mobileParty.DefaultBehavior == AiBehavior.BesiegeSettlement && missionType == Army.ArmyTypes.Besieger) || (mobileParty.DefaultBehavior == AiBehavior.RaidSettlement && missionType == Army.ArmyTypes.Raider) || (mobileParty.DefaultBehavior == AiBehavior.DefendSettlement && missionType == Army.ArmyTypes.Defender)) && mobileParty.TargetSettlement == settlement)
			{
				return false;
			}
			if (missionType == Army.ArmyTypes.Raider)
			{
				float num = MathF.Max(100f, MathF.Min(250f, Campaign.Current.Models.MapDistanceModel.GetDistance(mobileParty.MapFaction.FactionMidSettlement, settlement.MapFaction.FactionMidSettlement)));
				if (Campaign.Current.Models.MapDistanceModel.GetDistance(mobileParty, settlement) > num)
				{
					return false;
				}
			}
			return true;
		}

		private void CalculateMilitaryBehaviorForSettlement(Settlement settlement, Army.ArmyTypes missionType, AiBehavior aiBehavior, PartyThinkParams p, float ourStrength, float partySizeScore, int numberOfEnemyFactionSettlements, float totalEnemyMobilePartyStrength, float newArmyCreatingAdditionalConstant = 1f)
		{
			if ((missionType == Army.ArmyTypes.Defender && settlement.LastAttackerParty != null && settlement.LastAttackerParty.IsActive) || (missionType == Army.ArmyTypes.Raider && settlement.IsVillage && settlement.Village.VillageState == Village.VillageStates.Normal) || (missionType == Army.ArmyTypes.Besieger && settlement.IsFortification && (settlement.SiegeEvent == null || settlement.SiegeEvent.BesiegerCamp.LeaderParty.MapFaction == p.MobilePartyOf.MapFaction)) || (missionType == Army.ArmyTypes.Patrolling && !settlement.IsCastle && p.WillGatherAnArmy))
			{
				MobileParty mobilePartyOf = p.MobilePartyOf;
				IFaction mapFaction = mobilePartyOf.MapFaction;
				float num = mobilePartyOf.Food;
				float num2 = -mobilePartyOf.FoodChange;
				if (mobilePartyOf.Army != null && mobilePartyOf == mobilePartyOf.Army.LeaderParty)
				{
					foreach (MobileParty mobileParty in mobilePartyOf.Army.LeaderParty.AttachedParties)
					{
						num += mobileParty.Food;
						num2 += -mobileParty.FoodChange;
					}
				}
				float num3 = MathF.Max(0f, num) / num2;
				float num4 = ((num3 < 5f) ? (0.1f + 0.9f * (num3 / 5f)) : 1f);
				float num5 = ((missionType != Army.ArmyTypes.Patrolling) ? Campaign.Current.Models.TargetScoreCalculatingModel.GetTargetScoreForFaction(settlement, missionType, mobilePartyOf, ourStrength, numberOfEnemyFactionSettlements, totalEnemyMobilePartyStrength) : Campaign.Current.Models.TargetScoreCalculatingModel.CalculatePatrollingScoreForSettlement(settlement, mobilePartyOf));
				num5 *= partySizeScore * num4 * newArmyCreatingAdditionalConstant;
				if (mobilePartyOf.Objective == MobileParty.PartyObjective.Defensive)
				{
					if (aiBehavior == AiBehavior.DefendSettlement || (aiBehavior == AiBehavior.PatrolAroundPoint && settlement.MapFaction == mapFaction))
					{
						num5 *= 1.2f;
					}
					else
					{
						num5 *= 0.8f;
					}
				}
				else if (mobilePartyOf.Objective == MobileParty.PartyObjective.Aggressive)
				{
					if (aiBehavior == AiBehavior.BesiegeSettlement || aiBehavior == AiBehavior.RaidSettlement)
					{
						num5 *= 1.2f;
					}
					else
					{
						num5 *= 0.8f;
					}
				}
				if (!mobilePartyOf.IsDisbanding)
				{
					IDisbandPartyCampaignBehavior disbandPartyCampaignBehavior = this._disbandPartyCampaignBehavior;
					if (disbandPartyCampaignBehavior == null || !disbandPartyCampaignBehavior.IsPartyWaitingForDisband(mobilePartyOf))
					{
						goto IL_209;
					}
				}
				num5 *= 0.25f;
				IL_209:
				AIBehaviorTuple aibehaviorTuple = new AIBehaviorTuple(settlement, aiBehavior, p.WillGatherAnArmy);
				ValueTuple<AIBehaviorTuple, float> valueTuple = new ValueTuple<AIBehaviorTuple, float>(aibehaviorTuple, num5);
				p.AddBehaviorScore(valueTuple);
			}
		}

		private void AiHourlyTick(MobileParty mobileParty, PartyThinkParams p)
		{
			if (mobileParty.IsMilitia || mobileParty.IsCaravan || mobileParty.IsVillager || mobileParty.IsBandit || mobileParty.IsDisbanding || mobileParty.LeaderHero == null || (mobileParty.MapFaction != Clan.PlayerClan.MapFaction && !mobileParty.MapFaction.IsKingdomFaction))
			{
				return;
			}
			Settlement currentSettlement = mobileParty.CurrentSettlement;
			if (((currentSettlement != null) ? currentSettlement.SiegeEvent : null) != null)
			{
				return;
			}
			if (mobileParty.Army != null)
			{
				mobileParty.Ai.SetInitiative(0.33f, 0.33f, 24f);
				if (mobileParty.Army.LeaderParty == mobileParty && (mobileParty.Army.AIBehavior == Army.AIBehaviorFlags.Gathering || mobileParty.Army.AIBehavior == Army.AIBehaviorFlags.WaitingForArmyMembers))
				{
					mobileParty.Ai.SetInitiative(0.33f, 1f, 24f);
					p.DoNotChangeBehavior = true;
				}
				else if (mobileParty.Army.AIBehavior == Army.AIBehaviorFlags.Patrolling)
				{
					mobileParty.Ai.SetInitiative(1f, 1f, 24f);
				}
				else if (mobileParty.Army.AIBehavior == Army.AIBehaviorFlags.Defending && mobileParty.Army.LeaderParty == mobileParty && mobileParty.Army.AiBehaviorObject != null && mobileParty.Army.AiBehaviorObject is Settlement && ((Settlement)mobileParty.Army.AiBehaviorObject).GatePosition.DistanceSquared(mobileParty.Position2D) < 100f)
				{
					mobileParty.Ai.SetInitiative(1f, 1f, 24f);
				}
				if (mobileParty.Army.LeaderParty != mobileParty)
				{
					return;
				}
			}
			else if (mobileParty.DefaultBehavior == AiBehavior.DefendSettlement || mobileParty.Objective == MobileParty.PartyObjective.Defensive)
			{
				mobileParty.Ai.SetInitiative(0.33f, 1f, 2f);
			}
			float num4;
			if (mobileParty.Army != null)
			{
				float num = 0f;
				foreach (MobileParty mobileParty2 in mobileParty.Army.Parties)
				{
					float num2 = (PartyBaseHelper.FindPartySizeNormalLimit(mobileParty2) + 1f) * 0.5f;
					float num3 = mobileParty2.PartySizeRatio / num2;
					num += num3;
				}
				num4 = num / (float)mobileParty.Army.Parties.Count;
			}
			else
			{
				float num5 = (PartyBaseHelper.FindPartySizeNormalLimit(mobileParty) + 1f) * 0.5f;
				num4 = mobileParty.PartySizeRatio / num5;
			}
			float num6 = MathF.Max(1f, MathF.Min((float)mobileParty.MapFaction.Fiefs.Count / 5f, 2.5f));
			float num7 = MathF.Min(1f, MathF.Pow(num4, num6));
			float num8 = mobileParty.Food;
			float num9 = -mobileParty.FoodChange;
			int num10 = 1;
			if (mobileParty.Army != null && mobileParty == mobileParty.Army.LeaderParty)
			{
				foreach (MobileParty mobileParty3 in mobileParty.Army.LeaderParty.AttachedParties)
				{
					num8 += mobileParty3.Food;
					num9 += -mobileParty3.FoodChange;
					num10++;
				}
			}
			float num11 = MathF.Max(0f, num8) / num9;
			float num12 = ((num11 < 5f) ? (0.1f + 0.9f * (num11 / 5f)) : 1f);
			float totalStrengthWithFollowers = mobileParty.GetTotalStrengthWithFollowers(false);
			if ((mobileParty.DefaultBehavior == AiBehavior.BesiegeSettlement || mobileParty.DefaultBehavior == AiBehavior.RaidSettlement || mobileParty.DefaultBehavior == AiBehavior.DefendSettlement) && mobileParty.TargetSettlement != null)
			{
				float num13 = Campaign.Current.Models.TargetScoreCalculatingModel.CurrentObjectiveValue(mobileParty);
				num13 *= ((mobileParty.MapEvent == null || mobileParty.SiegeEvent == null) ? (num12 * num7) : 1f);
				if (mobileParty.SiegeEvent != null)
				{
					float num14 = 0f;
					foreach (PartyBase partyBase in ((mobileParty.DefaultBehavior == AiBehavior.BesiegeSettlement) ? mobileParty.SiegeEvent.BesiegedSettlement.GetInvolvedPartiesForEventType(MapEvent.BattleTypes.Siege) : mobileParty.SiegeEvent.BesiegerCamp.GetInvolvedPartiesForEventType(MapEvent.BattleTypes.Siege)))
					{
						num14 += partyBase.TotalStrength;
					}
					float num15 = totalStrengthWithFollowers / num14;
					float num16 = MathF.Max(1f, MathF.Pow(num15, 1.75f) * 0.15f);
					num13 *= num16;
				}
				if (!mobileParty.IsDisbanding)
				{
					IDisbandPartyCampaignBehavior disbandPartyCampaignBehavior = this._disbandPartyCampaignBehavior;
					if (disbandPartyCampaignBehavior == null || !disbandPartyCampaignBehavior.IsPartyWaitingForDisband(mobileParty))
					{
						goto IL_48D;
					}
				}
				num13 *= 0.25f;
				IL_48D:
				p.CurrentObjectiveValue = num13;
				AiBehavior defaultBehavior = mobileParty.DefaultBehavior;
				AIBehaviorTuple aibehaviorTuple = new AIBehaviorTuple(mobileParty.TargetSettlement, defaultBehavior, false);
				ValueTuple<AIBehaviorTuple, float> valueTuple = new ValueTuple<AIBehaviorTuple, float>(aibehaviorTuple, num13);
				p.AddBehaviorScore(valueTuple);
			}
			p.Initialization();
			bool flag = false;
			float num17 = 1f;
			float num18 = totalStrengthWithFollowers;
			if (mobileParty.LeaderHero != null && mobileParty.Army == null && mobileParty.LeaderHero.Clan != null && mobileParty.PartySizeRatio > 0.6f && (mobileParty.LeaderHero.Clan.Leader == mobileParty.LeaderHero || (mobileParty.LeaderHero.Clan.Leader.PartyBelongedTo == null && mobileParty.LeaderHero.Clan.WarPartyComponents != null && mobileParty.LeaderHero.Clan.WarPartyComponents.FirstOrDefault<WarPartyComponent>() == mobileParty.WarPartyComponent)))
			{
				int traitLevel = mobileParty.LeaderHero.GetTraitLevel(DefaultTraits.Calculating);
				IFaction mapFaction = mobileParty.MapFaction;
				Kingdom kingdom = (Kingdom)mapFaction;
				int count = ((Kingdom)mapFaction).Armies.Count;
				int num19 = 50 + count * count * 20 + mobileParty.LeaderHero.RandomInt(20) + traitLevel * 20;
				float num20 = 1f - (float)count * 0.2f;
				bool flag2;
				if (mobileParty.LeaderHero.Clan.Influence > (float)num19 && mobileParty.MapFaction.IsKingdomFaction && !mobileParty.LeaderHero.Clan.IsUnderMercenaryService)
				{
					flag2 = FactionManager.GetEnemyFactions(mobileParty.MapFaction as Kingdom).AnyQ((IFaction x) => x.Fiefs.Any<Town>());
				}
				else
				{
					flag2 = false;
				}
				flag = flag2;
				if (flag)
				{
					float num21 = ((kingdom.Armies.Count == 0) ? (1f + MathF.Sqrt((float)((int)CampaignTime.Now.ToDays - kingdom.LastArmyCreationDay)) * 0.15f) : 1f);
					float num22 = (10f + MathF.Sqrt(MathF.Min(900f, mobileParty.LeaderHero.Clan.Influence))) / 50f;
					float num23 = MathF.Sqrt(mobileParty.PartySizeRatio);
					num17 = num21 * num22 * num20 * num23;
					num18 = mobileParty.Party.TotalStrength;
					List<MobileParty> mobilePartiesToCallToArmy = Campaign.Current.Models.ArmyManagementCalculationModel.GetMobilePartiesToCallToArmy(mobileParty);
					if (mobilePartiesToCallToArmy.Count == 0)
					{
						flag = false;
					}
					else
					{
						foreach (MobileParty mobileParty4 in mobilePartiesToCallToArmy)
						{
							num18 += mobileParty4.Party.TotalStrength;
						}
					}
				}
			}
			for (int i = 0; i < 4; i++)
			{
				if (flag)
				{
					p.WillGatherAnArmy = true;
					this.FindBestTargetAndItsValueForFaction((Army.ArmyTypes)i, p, num18, num17);
				}
				p.WillGatherAnArmy = false;
				this.FindBestTargetAndItsValueForFaction((Army.ArmyTypes)i, p, totalStrengthWithFollowers, 1f);
			}
		}

		private const int MinimumInfluenceNeededToCreateArmy = 50;

		private IDisbandPartyCampaignBehavior _disbandPartyCampaignBehavior;
	}
}
