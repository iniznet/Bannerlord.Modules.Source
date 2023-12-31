﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultCombatSimulationModel : CombatSimulationModel
	{
		public override int SimulateHit(CharacterObject strikerTroop, CharacterObject struckTroop, PartyBase strikerParty, PartyBase struckParty, float strikerAdvantage, MapEvent battle)
		{
			MilitaryPowerModel militaryPowerModel = Campaign.Current.Models.MilitaryPowerModel;
			float troopPower = militaryPowerModel.GetTroopPower(strikerTroop, strikerParty.Side, strikerParty.MapEvent.SimulationContext, strikerParty.MapEventSide.LeaderSimulationModifier);
			float troopPower2 = militaryPowerModel.GetTroopPower(struckTroop, struckParty.Side, struckParty.MapEvent.SimulationContext, struckParty.MapEventSide.LeaderSimulationModifier);
			int num = (int)((0.5f + 0.5f * MBRandom.RandomFloat) * (40f * MathF.Pow(troopPower / troopPower2, 0.7f) * strikerAdvantage));
			ExplainedNumber explainedNumber = new ExplainedNumber((float)num, false, null);
			if (strikerParty.IsMobile && struckParty.IsMobile)
			{
				this.CalculateSimulationDamagePerkEffects(strikerTroop, struckTroop, strikerParty.MobileParty, struckParty.MobileParty, ref explainedNumber, battle);
			}
			return (int)explainedNumber.ResultNumber;
		}

		private void CalculateSimulationDamagePerkEffects(CharacterObject strikerTroop, CharacterObject struckTroop, MobileParty strikerParty, MobileParty struckParty, ref ExplainedNumber effectiveDamage, MapEvent battle)
		{
			if (strikerParty.HasPerk(DefaultPerks.Tactics.TightFormations, false) && strikerTroop.IsInfantry && struckTroop.IsMounted)
			{
				PerkHelper.AddPerkBonusForParty(DefaultPerks.Tactics.TightFormations, strikerParty, true, ref effectiveDamage);
			}
			if (struckParty.HasPerk(DefaultPerks.Tactics.LooseFormations, false) && struckTroop.IsInfantry && strikerTroop.IsRanged)
			{
				PerkHelper.AddPerkBonusForParty(DefaultPerks.Tactics.LooseFormations, struckParty, true, ref effectiveDamage);
			}
			TerrainType faceTerrainType = Campaign.Current.MapSceneWrapper.GetFaceTerrainType(strikerParty.CurrentNavigationFace);
			if (strikerParty.HasPerk(DefaultPerks.Tactics.ExtendedSkirmish, false) && (faceTerrainType == TerrainType.Snow || faceTerrainType == TerrainType.Forest))
			{
				PerkHelper.AddPerkBonusForParty(DefaultPerks.Tactics.ExtendedSkirmish, strikerParty, true, ref effectiveDamage);
			}
			if (strikerParty.HasPerk(DefaultPerks.Tactics.DecisiveBattle, false) && (faceTerrainType == TerrainType.Plain || faceTerrainType == TerrainType.Steppe || faceTerrainType == TerrainType.Desert))
			{
				PerkHelper.AddPerkBonusForParty(DefaultPerks.Tactics.DecisiveBattle, strikerParty, true, ref effectiveDamage);
			}
			if (!strikerParty.IsBandit && struckParty.IsBandit && strikerParty.HasPerk(DefaultPerks.Tactics.LawKeeper, false))
			{
				PerkHelper.AddPerkBonusForParty(DefaultPerks.Tactics.LawKeeper, strikerParty, true, ref effectiveDamage);
			}
			if (strikerParty.HasPerk(DefaultPerks.Tactics.Coaching, false))
			{
				PerkHelper.AddPerkBonusForParty(DefaultPerks.Tactics.Coaching, strikerParty, true, ref effectiveDamage);
			}
			if (struckParty.HasPerk(DefaultPerks.Tactics.EliteReserves, false) && struckTroop.Tier >= 3)
			{
				PerkHelper.AddPerkBonusForParty(DefaultPerks.Tactics.EliteReserves, struckParty, true, ref effectiveDamage);
			}
			if (strikerParty.HasPerk(DefaultPerks.Tactics.Encirclement, false) && strikerParty.MemberRoster.TotalHealthyCount > struckParty.MemberRoster.TotalHealthyCount)
			{
				PerkHelper.AddPerkBonusForParty(DefaultPerks.Tactics.Encirclement, strikerParty, true, ref effectiveDamage);
			}
			if (strikerParty.HasPerk(DefaultPerks.Tactics.Counteroffensive, true) && strikerParty.MemberRoster.TotalHealthyCount < struckParty.MemberRoster.TotalHealthyCount)
			{
				PerkHelper.AddPerkBonusForParty(DefaultPerks.Tactics.Counteroffensive, strikerParty, false, ref effectiveDamage);
			}
			bool flag = false;
			using (List<MapEventParty>.Enumerator enumerator = battle.PartiesOnSide(BattleSideEnum.Defender).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Party == struckParty.Party)
					{
						flag = true;
						break;
					}
				}
			}
			bool flag2 = !flag;
			bool flag3 = flag2;
			if (battle.IsSiegeAssault && flag2 && strikerParty.HasPerk(DefaultPerks.Tactics.Besieged, false))
			{
				PerkHelper.AddPerkBonusForParty(DefaultPerks.Tactics.Besieged, strikerParty, true, ref effectiveDamage);
			}
			if (flag && strikerParty.HasPerk(DefaultPerks.Scouting.Vanguard, false))
			{
				effectiveDamage.AddFactor(DefaultPerks.Scouting.Vanguard.PrimaryBonus, DefaultPerks.Scouting.Vanguard.Name);
			}
			if ((battle.IsSiegeOutside || battle.IsSallyOut) && flag3 && strikerParty.HasPerk(DefaultPerks.Scouting.Rearguard, false))
			{
				PerkHelper.AddPerkBonusForParty(DefaultPerks.Scouting.Rearguard, strikerParty, false, ref effectiveDamage);
			}
			if (battle.IsSallyOut && flag && strikerParty.HasPerk(DefaultPerks.Scouting.Vanguard, true))
			{
				effectiveDamage.AddFactor(DefaultPerks.Scouting.Vanguard.SecondaryBonus, DefaultPerks.Scouting.Vanguard.Name);
			}
			if (battle.IsFieldBattle && flag2 && strikerParty.HasPerk(DefaultPerks.Tactics.Counteroffensive, false))
			{
				PerkHelper.AddPerkBonusForParty(DefaultPerks.Tactics.Counteroffensive, strikerParty, true, ref effectiveDamage);
			}
			if (strikerParty.Army != null && strikerParty.LeaderHero != null && strikerParty.Army.LeaderParty == strikerParty && strikerParty.LeaderHero.GetPerkValue(DefaultPerks.Tactics.TacticalMastery))
			{
				PerkHelper.AddEpicPerkBonusForCharacter(DefaultPerks.Tactics.TacticalMastery, strikerParty.LeaderHero.CharacterObject, DefaultSkills.Tactics, true, ref effectiveDamage, Campaign.Current.Models.CharacterDevelopmentModel.MinSkillRequiredForEpicPerkBonus);
			}
		}

		public override float GetMaximumSiegeEquipmentProgress(Settlement settlement)
		{
			float num = 0f;
			if (settlement.SiegeEvent != null && settlement.IsFortification)
			{
				foreach (SiegeEvent.SiegeEngineConstructionProgress siegeEngineConstructionProgress in settlement.SiegeEvent.GetSiegeEventSide(BattleSideEnum.Attacker).SiegeEngines.AllSiegeEngines())
				{
					if (!siegeEngineConstructionProgress.IsConstructed && siegeEngineConstructionProgress.Progress > num)
					{
						num = siegeEngineConstructionProgress.Progress;
					}
				}
			}
			return num;
		}

		public override int GetNumberOfEquipmentsBuilt(Settlement settlement)
		{
			if (settlement.SiegeEvent != null && settlement.IsFortification)
			{
				settlement.Town.GetWallLevel();
				bool flag = false;
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				foreach (SiegeEvent.SiegeEngineConstructionProgress siegeEngineConstructionProgress in settlement.SiegeEvent.GetSiegeEventSide(BattleSideEnum.Attacker).SiegeEngines.AllSiegeEngines())
				{
					if (siegeEngineConstructionProgress.IsConstructed)
					{
						if (siegeEngineConstructionProgress.SiegeEngine == DefaultSiegeEngineTypes.Ram)
						{
							flag = true;
						}
						else if (siegeEngineConstructionProgress.SiegeEngine == DefaultSiegeEngineTypes.SiegeTower)
						{
							num++;
						}
						else if (siegeEngineConstructionProgress.SiegeEngine == DefaultSiegeEngineTypes.Trebuchet || siegeEngineConstructionProgress.SiegeEngine == DefaultSiegeEngineTypes.Onager || siegeEngineConstructionProgress.SiegeEngine == DefaultSiegeEngineTypes.Ballista)
						{
							num2++;
						}
						else if (siegeEngineConstructionProgress.SiegeEngine == DefaultSiegeEngineTypes.FireOnager || siegeEngineConstructionProgress.SiegeEngine == DefaultSiegeEngineTypes.FireBallista)
						{
							num3++;
						}
					}
				}
				return (flag ? 1 : 0) + num + num2 + num3;
			}
			return 0;
		}

		public override float GetSettlementAdvantage(Settlement settlement)
		{
			if (settlement.SiegeEvent != null && settlement.IsFortification)
			{
				int wallLevel = settlement.Town.GetWallLevel();
				bool flag = false;
				bool flag2 = false;
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				foreach (SiegeEvent.SiegeEngineConstructionProgress siegeEngineConstructionProgress in settlement.SiegeEvent.GetSiegeEventSide(BattleSideEnum.Attacker).SiegeEngines.AllSiegeEngines())
				{
					if (siegeEngineConstructionProgress.IsConstructed)
					{
						if (siegeEngineConstructionProgress.SiegeEngine == DefaultSiegeEngineTypes.Ram || siegeEngineConstructionProgress.SiegeEngine == DefaultSiegeEngineTypes.ImprovedRam)
						{
							if (siegeEngineConstructionProgress.SiegeEngine == DefaultSiegeEngineTypes.ImprovedRam)
							{
								flag2 = true;
							}
							flag = true;
						}
						else if (siegeEngineConstructionProgress.SiegeEngine == DefaultSiegeEngineTypes.SiegeTower)
						{
							num++;
						}
						else if (siegeEngineConstructionProgress.SiegeEngine == DefaultSiegeEngineTypes.Trebuchet || siegeEngineConstructionProgress.SiegeEngine == DefaultSiegeEngineTypes.Onager || siegeEngineConstructionProgress.SiegeEngine == DefaultSiegeEngineTypes.Ballista)
						{
							num2++;
						}
						else if (siegeEngineConstructionProgress.SiegeEngine == DefaultSiegeEngineTypes.FireOnager || siegeEngineConstructionProgress.SiegeEngine == DefaultSiegeEngineTypes.FireBallista)
						{
							num3++;
						}
					}
				}
				float num4 = 4f + (float)(wallLevel - 1);
				if (settlement.SettlementTotalWallHitPoints < 1E-05f)
				{
					num4 *= 0.25f;
				}
				float num5 = 1f + num4;
				float num6 = 1f + ((flag | (num > 0)) ? 0.12f : 0f) + (flag2 ? 0.24f : (flag ? 0.16f : 0f)) + ((num > 1) ? 0.24f : ((num == 1) ? 0.16f : 0f)) + (float)num2 * 0.08f + (float)num3 * 0.12f;
				float num7 = num5 / num6;
				ExplainedNumber explainedNumber = new ExplainedNumber(num7, false, null);
				ISiegeEventSide siegeEventSide = settlement.SiegeEvent.GetSiegeEventSide(BattleSideEnum.Attacker);
				this.CalculateSettlementAdvantagePerkEffects(settlement, ref explainedNumber, siegeEventSide);
				return explainedNumber.ResultNumber;
			}
			if (settlement.IsVillage)
			{
				return 1.25f;
			}
			return 1f;
		}

		private void CalculateSettlementAdvantagePerkEffects(Settlement settlement, ref ExplainedNumber effectiveAdvantage, ISiegeEventSide opposingSide)
		{
			if (opposingSide.GetInvolvedPartiesForEventType(MapEvent.BattleTypes.Siege).Any((PartyBase x) => x.MobileParty.HasPerk(DefaultPerks.Tactics.OnTheMarch, false)))
			{
				effectiveAdvantage.AddFactor(DefaultPerks.Tactics.OnTheMarch.PrimaryBonus, DefaultPerks.Tactics.OnTheMarch.Name);
			}
			if (PerkHelper.GetPerkValueForTown(DefaultPerks.Tactics.OnTheMarch, settlement.Town))
			{
				PerkHelper.AddPerkBonusForTown(DefaultPerks.Tactics.OnTheMarch, settlement.Town, ref effectiveAdvantage);
			}
		}

		[return: TupleElementNames(new string[] { "defenderRounds", "attackerRounds" })]
		public override ValueTuple<int, int> GetSimulationRoundsForBattle(MapEvent mapEvent, int numDefenders, int numAttackers)
		{
			if (mapEvent.IsInvulnerable)
			{
				return new ValueTuple<int, int>(0, 0);
			}
			int eventType = (int)mapEvent.EventType;
			Settlement mapEventSettlement = mapEvent.MapEventSettlement;
			if (eventType == 5)
			{
				float settlementAdvantage = this.GetSettlementAdvantage(mapEventSettlement);
				float num = 1f + MathF.Pow((float)numDefenders, 0.3f);
				float num2 = MathF.Max(num * settlementAdvantage, (float)((numDefenders + 1) / (numAttackers + 1)));
				if ((mapEventSettlement.IsTown && numDefenders > 100) || (mapEventSettlement.IsCastle && numDefenders > 30))
				{
					return new ValueTuple<int, int>(MathF.Round(0.5f + num2), MathF.Round(0.5f + num));
				}
			}
			int num3;
			int num4;
			if (numDefenders <= 10)
			{
				num3 = MBRandom.RoundRandomized(MathF.Min((float)numAttackers * 3f, (float)numDefenders * 0.3f));
				num4 = MBRandom.RoundRandomized(MathF.Min((float)numDefenders * 3f, (float)numAttackers * 0.3f));
			}
			else
			{
				num3 = MBRandom.RoundRandomized(MathF.Min((float)numAttackers * 2f, MathF.Pow((float)numDefenders, 0.6f)));
				num4 = MBRandom.RoundRandomized(MathF.Min((float)numDefenders * 2f, MathF.Pow((float)numAttackers, 0.6f)));
			}
			return new ValueTuple<int, int>(num3, num4);
		}

		[return: TupleElementNames(new string[] { "defenderAdvantage", "attackerAdvantage" })]
		public override ValueTuple<float, float> GetBattleAdvantage(PartyBase defenderParty, PartyBase attackerParty, MapEvent.BattleTypes mapEventType, Settlement settlement)
		{
			float num = 1f;
			float num2 = 1f;
			float num3 = num * this.PartyBattleAdvantage(defenderParty, attackerParty);
			num2 *= this.PartyBattleAdvantage(attackerParty, defenderParty);
			if (mapEventType == MapEvent.BattleTypes.Siege)
			{
				num2 *= 0.9f;
			}
			return new ValueTuple<float, float>(num3, num2);
		}

		private float PartyBattleAdvantage(PartyBase party, PartyBase opposingParty)
		{
			float num = 1f;
			if (party.LeaderHero != null)
			{
				int skillValue = party.LeaderHero.GetSkillValue(DefaultSkills.Tactics);
				float num2 = DefaultSkillEffects.TacticsAdvantage.PrimaryBonus * (float)skillValue * 0.01f;
				num += num2;
				if (party.LeaderHero.GetPerkValue(DefaultPerks.Scouting.Patrols) && opposingParty.Culture.IsBandit)
				{
					num += DefaultPerks.Scouting.Patrols.SecondaryBonus * num;
				}
			}
			if (party.IsMobile && opposingParty.IsMobile && party.LeaderHero != null && opposingParty.LeaderHero != null && party.MobileParty.HasPerk(DefaultPerks.Tactics.PreBattleManeuvers, true))
			{
				int num3 = party.LeaderHero.GetSkillValue(DefaultSkills.Tactics) - opposingParty.LeaderHero.GetSkillValue(DefaultSkills.Tactics);
				if (num3 > 0)
				{
					num += (float)num3 * 0.01f;
				}
			}
			return num;
		}
	}
}
