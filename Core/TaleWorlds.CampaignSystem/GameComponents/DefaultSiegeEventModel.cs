using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultSiegeEventModel : SiegeEventModel
	{
		public override string GetSiegeEngineMapPrefabName(SiegeEngineType type, int wallLevel, BattleSideEnum side)
		{
			string text = null;
			if (type == DefaultSiegeEngineTypes.Onager)
			{
				text = "mangonel_a_mapicon";
			}
			else if (type == DefaultSiegeEngineTypes.Catapult)
			{
				text = "mangonel_b_mapicon";
			}
			else if (type == DefaultSiegeEngineTypes.FireOnager)
			{
				text = "mangonel_a_fire_mapicon";
			}
			else if (type == DefaultSiegeEngineTypes.FireCatapult)
			{
				text = "mangonel_b_fire_mapicon";
			}
			else if (type == DefaultSiegeEngineTypes.Ballista)
			{
				text = ((side == BattleSideEnum.Attacker) ? "ballista_a_mapicon" : "ballista_b_mapicon");
			}
			else if (type == DefaultSiegeEngineTypes.FireBallista)
			{
				text = ((side == BattleSideEnum.Attacker) ? "ballista_a_fire_mapicon" : "ballista_b_fire_mapicon");
			}
			else if (type == DefaultSiegeEngineTypes.Trebuchet)
			{
				text = "trebuchet_a_mapicon";
			}
			else if (type == DefaultSiegeEngineTypes.Bricole)
			{
				text = "trebuchet_b_mapicon";
			}
			else if (type == DefaultSiegeEngineTypes.Ram)
			{
				text = "batteringram_a_mapicon";
			}
			else if (type == DefaultSiegeEngineTypes.SiegeTower)
			{
				switch (wallLevel)
				{
				case 1:
					text = "siegetower_5m_mapicon";
					break;
				case 2:
					text = "siegetower_9m_mapicon";
					break;
				case 3:
					text = "siegetower_12m_mapicon";
					break;
				}
			}
			return text;
		}

		public override string GetSiegeEngineMapProjectilePrefabName(SiegeEngineType type)
		{
			string text = null;
			if (type == DefaultSiegeEngineTypes.Onager || type == DefaultSiegeEngineTypes.Catapult || type == DefaultSiegeEngineTypes.Trebuchet || type == DefaultSiegeEngineTypes.Bricole)
			{
				text = "mangonel_mapicon_projectile";
			}
			else if (type == DefaultSiegeEngineTypes.FireOnager || type == DefaultSiegeEngineTypes.FireCatapult)
			{
				text = "mangonel_fire_mapicon_projectile";
			}
			else if (type == DefaultSiegeEngineTypes.Ballista)
			{
				text = "ballista_mapicon_projectile";
			}
			else if (type == DefaultSiegeEngineTypes.FireBallista)
			{
				text = "ballista_fire_mapicon_projectile";
			}
			return text;
		}

		public override string GetSiegeEngineMapReloadAnimationName(SiegeEngineType type, BattleSideEnum side)
		{
			string text = null;
			if (type == DefaultSiegeEngineTypes.Onager)
			{
				text = "mangonel_a_mapicon_reload";
			}
			else if (type == DefaultSiegeEngineTypes.Catapult)
			{
				text = "mangonel_b_mapicon_reload";
			}
			else if (type == DefaultSiegeEngineTypes.FireOnager)
			{
				text = "mangonel_a_fire_mapicon_reload";
			}
			else if (type == DefaultSiegeEngineTypes.FireCatapult)
			{
				text = "mangonel_b_fire_mapicon_reload";
			}
			else if (type == DefaultSiegeEngineTypes.Ballista)
			{
				text = ((side == BattleSideEnum.Attacker) ? "ballista_a_mapicon_reload" : "ballista_b_mapicon_reload");
			}
			else if (type == DefaultSiegeEngineTypes.FireBallista)
			{
				text = ((side == BattleSideEnum.Attacker) ? "ballista_a_fire_mapicon_reload" : "ballista_b_fire_mapicon_reload");
			}
			else if (type == DefaultSiegeEngineTypes.Trebuchet)
			{
				text = "trebuchet_a_mapicon_reload";
			}
			else if (type == DefaultSiegeEngineTypes.Bricole)
			{
				text = "trebuchet_b_mapicon_reload";
			}
			return text;
		}

		public override string GetSiegeEngineMapFireAnimationName(SiegeEngineType type, BattleSideEnum side)
		{
			string text = null;
			if (type == DefaultSiegeEngineTypes.Onager)
			{
				text = "mangonel_a_mapicon_fire";
			}
			else if (type == DefaultSiegeEngineTypes.Catapult)
			{
				text = "mangonel_b_mapicon_fire";
			}
			else if (type == DefaultSiegeEngineTypes.FireOnager)
			{
				text = "mangonel_a_fire_mapicon_fire";
			}
			else if (type == DefaultSiegeEngineTypes.FireCatapult)
			{
				text = "mangonel_b_fire_mapicon_fire";
			}
			else if (type == DefaultSiegeEngineTypes.Ballista)
			{
				text = ((side == BattleSideEnum.Attacker) ? "ballista_a_mapicon_fire" : "ballista_b_mapicon_fire");
			}
			else if (type == DefaultSiegeEngineTypes.FireBallista)
			{
				text = ((side == BattleSideEnum.Attacker) ? "ballista_a_fire_mapicon_fire" : "ballista_b_fire_mapicon_fire");
			}
			else if (type == DefaultSiegeEngineTypes.Trebuchet)
			{
				text = "trebuchet_a_mapicon_fire";
			}
			else if (type == DefaultSiegeEngineTypes.Bricole)
			{
				text = "trebuchet_b_mapicon_fire";
			}
			return text;
		}

		public override sbyte GetSiegeEngineMapProjectileBoneIndex(SiegeEngineType type, BattleSideEnum side)
		{
			if (type == DefaultSiegeEngineTypes.Onager || type == DefaultSiegeEngineTypes.FireOnager)
			{
				return 2;
			}
			if (type == DefaultSiegeEngineTypes.Catapult || type == DefaultSiegeEngineTypes.FireCatapult)
			{
				return 2;
			}
			if (type == DefaultSiegeEngineTypes.Ballista || type == DefaultSiegeEngineTypes.FireBallista)
			{
				return 7;
			}
			if (type == DefaultSiegeEngineTypes.Trebuchet)
			{
				return 4;
			}
			if (type == DefaultSiegeEngineTypes.Bricole)
			{
				return 20;
			}
			return -1;
		}

		public override MobileParty GetEffectiveSiegePartyForSide(SiegeEvent siegeEvent, BattleSideEnum battleSide)
		{
			MobileParty mobileParty = null;
			if (battleSide == BattleSideEnum.Attacker)
			{
				mobileParty = siegeEvent.BesiegerCamp.BesiegerParty;
			}
			else
			{
				int num = 0;
				int num2 = -1;
				for (PartyBase partyBase = siegeEvent.BesiegedSettlement.GetNextInvolvedPartyForEventType(ref num2, MapEvent.BattleTypes.Siege); partyBase != null; partyBase = siegeEvent.BesiegedSettlement.GetNextInvolvedPartyForEventType(ref num2, MapEvent.BattleTypes.Siege))
				{
					if (partyBase.LeaderHero != null)
					{
						Hero effectiveEngineer = partyBase.MobileParty.EffectiveEngineer;
						int num3 = ((effectiveEngineer != null) ? effectiveEngineer.GetSkillValue(DefaultSkills.Engineering) : 0);
						if (num3 > num)
						{
							num = num3;
							mobileParty = partyBase.MobileParty;
						}
					}
				}
			}
			return mobileParty;
		}

		public override float GetCasualtyChance(MobileParty siegeParty, SiegeEvent siegeEvent, BattleSideEnum side)
		{
			float num = 1f;
			if (siegeParty != null && siegeParty.HasPerk(DefaultPerks.Engineering.CampBuilding, true))
			{
				num += DefaultPerks.Engineering.CampBuilding.SecondaryBonus;
			}
			if (siegeParty != null && siegeParty.HasPerk(DefaultPerks.Medicine.SiegeMedic, true))
			{
				num += DefaultPerks.Medicine.SiegeMedic.SecondaryBonus;
			}
			if (side == BattleSideEnum.Defender)
			{
				Town town = siegeEvent.BesiegedSettlement.Town;
				if (((town != null) ? town.Governor : null) != null && siegeEvent.BesiegedSettlement.Town.Governor.GetPerkValue(DefaultPerks.Medicine.BattleHardened))
				{
					num += DefaultPerks.Medicine.BattleHardened.SecondaryBonus;
				}
			}
			return num;
		}

		public override int GetSiegeEngineDestructionCasualties(SiegeEvent siegeEvent, BattleSideEnum side, SiegeEngineType destroyedSiegeEngine)
		{
			return 2;
		}

		public override int GetColleteralDamageCasualties(SiegeEngineType siegeEngineType, MobileParty party)
		{
			int num = 1;
			if (party != null && party.HasPerk(DefaultPerks.Crossbow.Terror, false) && MBRandom.RandomFloat < DefaultPerks.Crossbow.Terror.PrimaryBonus)
			{
				num++;
			}
			return num;
		}

		public override float GetSiegeEngineHitChance(SiegeEngineType siegeEngineType, BattleSideEnum battleSide, SiegeBombardTargets target, Town town)
		{
			float num;
			if (target - SiegeBombardTargets.Wall > 1)
			{
				if (target != SiegeBombardTargets.People)
				{
					throw new ArgumentOutOfRangeException("target", target, null);
				}
				num = siegeEngineType.AntiPersonnelHitChance;
			}
			else
			{
				num = siegeEngineType.HitChance;
			}
			ExplainedNumber explainedNumber = new ExplainedNumber(num, false, null);
			if (battleSide == BattleSideEnum.Attacker && target == SiegeBombardTargets.RangedEngines)
			{
				float num2 = 0f;
				switch (town.GetWallLevel())
				{
				case 1:
					num2 = 0.05f;
					break;
				case 2:
					num2 = 0.1f;
					break;
				case 3:
					num2 = 0.15f;
					break;
				}
				explainedNumber.Add(-num2, new TextObject("{=b9NaTqyr}Extra Defender Defense", null), null);
			}
			if (battleSide == BattleSideEnum.Defender)
			{
				if (target == SiegeBombardTargets.RangedEngines && town.Governor != null && town.Governor.GetPerkValue(DefaultPerks.Engineering.DreadfulSieger))
				{
					explainedNumber.AddFactor(DefaultPerks.Engineering.DreadfulSieger.PrimaryBonus, DefaultPerks.Engineering.DreadfulSieger.Name);
				}
				if (siegeEngineType == DefaultSiegeEngineTypes.Ballista)
				{
					PerkHelper.AddPerkBonusForTown(DefaultPerks.Crossbow.Pavise, town, ref explainedNumber);
				}
			}
			SiegeEvent siegeEvent = town.Settlement.SiegeEvent;
			MobileParty effectiveSiegePartyForSide = this.GetEffectiveSiegePartyForSide(siegeEvent, battleSide);
			MobileParty effectiveSiegePartyForSide2 = this.GetEffectiveSiegePartyForSide(siegeEvent, battleSide.GetOppositeSide());
			if (effectiveSiegePartyForSide != null)
			{
				if ((siegeEngineType == DefaultSiegeEngineTypes.Trebuchet || siegeEngineType == DefaultSiegeEngineTypes.Onager || siegeEngineType == DefaultSiegeEngineTypes.FireOnager) && effectiveSiegePartyForSide.HasPerk(DefaultPerks.Engineering.Foreman, false))
				{
					explainedNumber.AddFactor(DefaultPerks.Engineering.Foreman.PrimaryBonus, DefaultPerks.Engineering.Foreman.Name);
				}
				if ((siegeEngineType == DefaultSiegeEngineTypes.Ballista || siegeEngineType == DefaultSiegeEngineTypes.FireBallista) && effectiveSiegePartyForSide.HasPerk(DefaultPerks.Engineering.Salvager, false))
				{
					explainedNumber.AddFactor(DefaultPerks.Engineering.Salvager.PrimaryBonus, DefaultPerks.Engineering.Salvager.Name);
				}
			}
			if (battleSide == BattleSideEnum.Defender && effectiveSiegePartyForSide2 != null && target == SiegeBombardTargets.RangedEngines && effectiveSiegePartyForSide2.HasPerk(DefaultPerks.Engineering.DungeonArchitect, false))
			{
				explainedNumber.AddFactor(DefaultPerks.Engineering.DungeonArchitect.PrimaryBonus, DefaultPerks.Engineering.DungeonArchitect.Name);
			}
			if (explainedNumber.ResultNumber < 0f)
			{
				explainedNumber = new ExplainedNumber(0f, false, null);
			}
			return explainedNumber.ResultNumber;
		}

		public override float GetSiegeStrategyScore(SiegeEvent siege, BattleSideEnum side, SiegeStrategy strategy)
		{
			if (strategy == DefaultSiegeStrategies.PreserveStrength)
			{
				return -9000f;
			}
			if (strategy != DefaultSiegeStrategies.Custom)
			{
				return MBRandom.RandomFloat;
			}
			if (siege == PlayerSiege.PlayerSiegeEvent && side == PlayerSiege.PlayerSide && siege.BesiegerCamp != null && siege.BesiegerCamp.BesiegerParty == MobileParty.MainParty)
			{
				return 9000f;
			}
			return -100f;
		}

		public override float GetConstructionProgressPerHour(SiegeEngineType type, SiegeEvent siegeEvent, ISiegeEventSide side)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(0f, false, null);
			float availableManDayPower = this.GetAvailableManDayPower(side);
			float num = (float)type.ManDayCost;
			explainedNumber.Add(1f / (num / availableManDayPower * 24f), this._baseConstructionSpeedText, null);
			MobileParty effectiveSiegePartyForSide = this.GetEffectiveSiegePartyForSide(siegeEvent, side.BattleSide);
			if (effectiveSiegePartyForSide != null)
			{
				int? num2;
				if (effectiveSiegePartyForSide == null)
				{
					num2 = null;
				}
				else
				{
					Hero effectiveEngineer = effectiveSiegePartyForSide.EffectiveEngineer;
					num2 = ((effectiveEngineer != null) ? new int?(effectiveEngineer.GetSkillValue(DefaultSkills.Engineering)) : null);
				}
				if ((num2 ?? 0) > 0)
				{
					SkillHelper.AddSkillBonusForParty(DefaultSkills.Engineering, DefaultSkillEffects.SiegeEngineProductionBonus, effectiveSiegePartyForSide, ref explainedNumber);
				}
			}
			float num3 = 0f;
			if (side.BattleSide == BattleSideEnum.Defender)
			{
				foreach (Building building in siegeEvent.BesiegedSettlement.Town.Buildings)
				{
					num3 += building.GetBuildingEffectAmount(BuildingEffectEnum.SiegeEngineSpeed);
				}
				explainedNumber.AddFactor(num3 * 0.01f, this._constructionSpeedProjectBonusText);
				Hero governor = siegeEvent.BesiegedSettlement.Town.Governor;
				if (((governor != null) ? governor.CurrentSettlement : null) != null && governor.CurrentSettlement == siegeEvent.BesiegedSettlement)
				{
					SkillHelper.AddSkillBonusForCharacter(DefaultSkills.Engineering, DefaultSkillEffects.SiegeEngineProductionBonus, governor.CharacterObject, ref explainedNumber, -1, true, 0);
				}
			}
			if (((siegeEvent != null) ? siegeEvent.BesiegerCamp.BesiegerParty : null) != null && siegeEvent.BesiegerCamp.BesiegerParty.HasPerk(DefaultPerks.Steward.Sweatshops, true))
			{
				explainedNumber.AddFactor(DefaultPerks.Steward.Sweatshops.SecondaryBonus, null);
			}
			if (effectiveSiegePartyForSide != null)
			{
				SiegeEvent.SiegeEngineConstructionProgress siegePreparations = side.SiegeEngines.SiegePreparations;
				if (siegePreparations != null && !siegePreparations.IsConstructed && effectiveSiegePartyForSide.HasPerk(DefaultPerks.Engineering.ImprovedTools, false))
				{
					explainedNumber.AddFactor(DefaultPerks.Engineering.ImprovedTools.PrimaryBonus, DefaultPerks.Engineering.ImprovedTools.Name);
				}
				else
				{
					PerkObject perkObject = (type.IsRanged ? DefaultPerks.Engineering.TorsionEngines : DefaultPerks.Engineering.Scaffolds);
					if (effectiveSiegePartyForSide.HasPerk(perkObject, false))
					{
						explainedNumber.AddFactor(perkObject.PrimaryBonus, perkObject.Name);
					}
				}
			}
			if (side.BattleSide == BattleSideEnum.Defender)
			{
				Settlement besiegedSettlement = siegeEvent.BesiegedSettlement;
				PerkObject salvager = DefaultPerks.Engineering.Salvager;
				if (PerkHelper.GetPerkValueForTown(salvager, besiegedSettlement.Town))
				{
					explainedNumber.AddFactor(salvager.SecondaryBonus * besiegedSettlement.Militia, salvager.Name);
				}
			}
			return explainedNumber.ResultNumber;
		}

		public override float GetAvailableManDayPower(ISiegeEventSide side)
		{
			int num = -1;
			PartyBase partyBase = side.GetNextInvolvedPartyForEventType(ref num, MapEvent.BattleTypes.Siege);
			int num2 = 0;
			while (partyBase != null)
			{
				num2 += partyBase.NumberOfHealthyMembers;
				partyBase = side.GetNextInvolvedPartyForEventType(ref num, MapEvent.BattleTypes.Siege);
			}
			return MathF.Sqrt((float)num2);
		}

		public override IEnumerable<SiegeEngineType> GetPrebuiltSiegeEnginesOfSettlement(Settlement settlement)
		{
			List<SiegeEngineType> list = new List<SiegeEngineType>();
			if (settlement.IsFortification)
			{
				Town town = settlement.Town;
				Building building2 = town.Buildings.Find((Building building) => building.BuildingType == DefaultBuildingTypes.SettlementSiegeWorkshop);
				if (building2 != null)
				{
					switch (building2.CurrentLevel)
					{
					case 0:
						goto IL_B7;
					case 1:
						goto IL_91;
					case 2:
						break;
					case 3:
						list.Add(DefaultSiegeEngineTypes.Ballista);
						list.Add(DefaultSiegeEngineTypes.Catapult);
						break;
					default:
						Debug.FailedAssert("Invalid building level", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\GameComponents\\DefaultSiegeEventCalculationModel.cs", "GetPrebuiltSiegeEnginesOfSettlement", 567);
						goto IL_B7;
					}
					list.Add(DefaultSiegeEngineTypes.Ballista);
					list.Add(DefaultSiegeEngineTypes.Catapult);
					IL_91:
					list.Add(DefaultSiegeEngineTypes.Ballista);
				}
				IL_B7:
				if (town.Governor != null && town.Governor.GetPerkValue(DefaultPerks.Engineering.SiegeWorks))
				{
					list.Add(DefaultSiegeEngineTypes.Catapult);
				}
			}
			return list;
		}

		public override IEnumerable<SiegeEngineType> GetPrebuiltSiegeEnginesOfSiegeCamp(BesiegerCamp besiegerCamp)
		{
			List<SiegeEngineType> list = new List<SiegeEngineType>();
			if (besiegerCamp.BesiegerParty.HasPerk(DefaultPerks.Engineering.Battlements, false))
			{
				list.Add(DefaultSiegeEngineTypes.Ballista);
			}
			return list;
		}

		public override float GetSiegeEngineHitPoints(SiegeEvent siegeEvent, SiegeEngineType siegeEngine, BattleSideEnum battleSide)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber((float)siegeEngine.BaseHitPoints, false, null);
			Settlement besiegedSettlement = siegeEvent.BesiegedSettlement;
			MobileParty effectiveSiegePartyForSide = this.GetEffectiveSiegePartyForSide(siegeEvent, battleSide);
			if (battleSide == BattleSideEnum.Defender && besiegedSettlement.Town.Governor != null && besiegedSettlement.Town.Governor.GetPerkValue(DefaultPerks.Engineering.SiegeEngineer))
			{
				explainedNumber.AddFactor(DefaultPerks.Engineering.SiegeEngineer.PrimaryBonus, DefaultPerks.Engineering.SiegeEngineer.Name);
			}
			if (siegeEngine.IsRanged)
			{
				if (effectiveSiegePartyForSide != null && effectiveSiegePartyForSide.HasPerk(DefaultPerks.Engineering.SiegeWorks, false))
				{
					explainedNumber.AddFactor(DefaultPerks.Engineering.SiegeWorks.PrimaryBonus, DefaultPerks.Engineering.SiegeWorks.Name);
				}
			}
			else if (battleSide == BattleSideEnum.Attacker && effectiveSiegePartyForSide != null && effectiveSiegePartyForSide.HasPerk(DefaultPerks.Engineering.Carpenters, false))
			{
				explainedNumber.AddFactor(DefaultPerks.Engineering.Carpenters.PrimaryBonus, DefaultPerks.Engineering.Carpenters.Name);
			}
			return explainedNumber.ResultNumber;
		}

		public override float GetSiegeEngineDamage(SiegeEvent siegeEvent, BattleSideEnum battleSide, SiegeEngineType siegeEngine, SiegeBombardTargets target)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber((float)siegeEngine.Damage, false, null);
			MobileParty effectiveSiegePartyForSide = this.GetEffectiveSiegePartyForSide(siegeEvent, battleSide);
			if (effectiveSiegePartyForSide != null)
			{
				if (battleSide == BattleSideEnum.Attacker)
				{
					if (target == SiegeBombardTargets.Wall && effectiveSiegePartyForSide.HasPerk(DefaultPerks.Engineering.WallBreaker, false))
					{
						explainedNumber.AddFactor(DefaultPerks.Engineering.WallBreaker.PrimaryBonus, DefaultPerks.Engineering.WallBreaker.Name);
					}
					if (target == SiegeBombardTargets.RangedEngines && effectiveSiegePartyForSide.HasPerk(DefaultPerks.Tactics.MakeThemPay, false))
					{
						explainedNumber.AddFactor(DefaultPerks.Tactics.MakeThemPay.PrimaryBonus, DefaultPerks.Tactics.MakeThemPay.Name);
					}
				}
				if ((target == SiegeBombardTargets.RangedEngines || target == SiegeBombardTargets.Wall) && effectiveSiegePartyForSide.HasPerk(DefaultPerks.Engineering.Masterwork, false))
				{
					int num = effectiveSiegePartyForSide.LeaderHero.GetSkillValue(DefaultSkills.Engineering) - 250;
					if (num > 0)
					{
						float num2 = (float)num * DefaultPerks.Engineering.Masterwork.PrimaryBonus;
						explainedNumber.AddFactor(num2, DefaultPerks.Engineering.Masterwork.Name);
					}
				}
			}
			if (battleSide == BattleSideEnum.Defender && target == SiegeBombardTargets.RangedEngines)
			{
				Hero governor = siegeEvent.BesiegedSettlement.Town.Governor;
				if (governor != null && governor.GetPerkValue(DefaultPerks.Tactics.MakeThemPay))
				{
					explainedNumber.AddFactor(DefaultPerks.Tactics.MakeThemPay.SecondaryBonus, DefaultPerks.Tactics.MakeThemPay.Name);
				}
			}
			return explainedNumber.ResultNumber;
		}

		public override int GetRangedSiegeEngineReloadTime(SiegeEvent siegeEvent, BattleSideEnum side, SiegeEngineType siegeEngine)
		{
			float campaignRateOfFirePerDay = siegeEngine.CampaignRateOfFirePerDay;
			float num = 1440f / campaignRateOfFirePerDay;
			ExplainedNumber explainedNumber = new ExplainedNumber(num, false, null);
			MobileParty effectiveSiegePartyForSide = this.GetEffectiveSiegePartyForSide(siegeEvent, side);
			if (effectiveSiegePartyForSide != null)
			{
				if ((siegeEngine == DefaultSiegeEngineTypes.Ballista || siegeEngine == DefaultSiegeEngineTypes.FireBallista) && effectiveSiegePartyForSide.HasPerk(DefaultPerks.Engineering.Clockwork, false))
				{
					explainedNumber.AddFactor(DefaultPerks.Engineering.Clockwork.PrimaryBonus, DefaultPerks.Engineering.Clockwork.Name);
				}
				else if ((siegeEngine == DefaultSiegeEngineTypes.Onager || siegeEngine == DefaultSiegeEngineTypes.Trebuchet || siegeEngine == DefaultSiegeEngineTypes.FireOnager) && effectiveSiegePartyForSide.HasPerk(DefaultPerks.Engineering.ArchitecturalCommisions, false))
				{
					explainedNumber.AddFactor(DefaultPerks.Engineering.ArchitecturalCommisions.PrimaryBonus, DefaultPerks.Engineering.ArchitecturalCommisions.Name);
				}
			}
			return MathF.Round(explainedNumber.ResultNumber);
		}

		public override IEnumerable<SiegeEngineType> GetAvailableAttackerRangedSiegeEngines(PartyBase party)
		{
			bool hasFirePerks = party.MobileParty.HasPerk(DefaultPerks.Engineering.Stonecutters, true) || party.MobileParty.HasPerk(DefaultPerks.Engineering.SiegeEngineer, true);
			yield return DefaultSiegeEngineTypes.Ballista;
			if (hasFirePerks)
			{
				yield return DefaultSiegeEngineTypes.FireBallista;
			}
			yield return DefaultSiegeEngineTypes.Onager;
			if (hasFirePerks)
			{
				yield return DefaultSiegeEngineTypes.FireOnager;
			}
			yield return DefaultSiegeEngineTypes.Trebuchet;
			yield break;
		}

		public override IEnumerable<SiegeEngineType> GetAvailableDefenderSiegeEngines(PartyBase party)
		{
			bool hasFirePerks = party.MobileParty.HasPerk(DefaultPerks.Engineering.Stonecutters, true) || party.MobileParty.HasPerk(DefaultPerks.Engineering.SiegeEngineer, true);
			yield return DefaultSiegeEngineTypes.Ballista;
			if (hasFirePerks)
			{
				yield return DefaultSiegeEngineTypes.FireBallista;
			}
			yield return DefaultSiegeEngineTypes.Bricole;
			yield return DefaultSiegeEngineTypes.Catapult;
			if (hasFirePerks)
			{
				yield return DefaultSiegeEngineTypes.FireCatapult;
			}
			yield break;
		}

		public override IEnumerable<SiegeEngineType> GetAvailableAttackerRamSiegeEngines(PartyBase party)
		{
			yield return DefaultSiegeEngineTypes.Ram;
			yield break;
		}

		public override IEnumerable<SiegeEngineType> GetAvailableAttackerTowerSiegeEngines(PartyBase party)
		{
			yield return DefaultSiegeEngineTypes.SiegeTower;
			yield break;
		}

		public override FlattenedTroopRoster GetPriorityTroopsForSallyOutAmbush()
		{
			FlattenedTroopRoster flattenedTroopRoster = new FlattenedTroopRoster(4);
			foreach (TroopRosterElement troopRosterElement in MobileParty.MainParty.MemberRoster.GetTroopRoster())
			{
				if (this.IsPriorityTroopForSallyOutAmbush(troopRosterElement))
				{
					flattenedTroopRoster.Add(troopRosterElement);
				}
			}
			SiegeEvent playerSiegeEvent = PlayerSiege.PlayerSiegeEvent;
			if (playerSiegeEvent.BesiegedSettlement.OwnerClan == Clan.PlayerClan && playerSiegeEvent.BesiegedSettlement.Town.GarrisonParty != null && playerSiegeEvent.BesiegedSettlement.Town.GarrisonParty.MemberRoster.Count > 0)
			{
				foreach (TroopRosterElement troopRosterElement2 in playerSiegeEvent.BesiegedSettlement.Town.GarrisonParty.MemberRoster.GetTroopRoster())
				{
					if (this.IsPriorityTroopForSallyOutAmbush(troopRosterElement2))
					{
						flattenedTroopRoster.Add(troopRosterElement2);
					}
				}
			}
			if (MobileParty.MainParty.Army != null && MobileParty.MainParty.Army.LeaderParty == MobileParty.MainParty)
			{
				foreach (PartyBase partyBase in playerSiegeEvent.GetSiegeEventSide(BattleSideEnum.Defender).GetInvolvedPartiesForEventType(MapEvent.BattleTypes.Siege))
				{
					if (partyBase != PartyBase.MainParty)
					{
						foreach (TroopRosterElement troopRosterElement3 in partyBase.MemberRoster.GetTroopRoster())
						{
							if (this.IsPriorityTroopForSallyOutAmbush(troopRosterElement3))
							{
								flattenedTroopRoster.Add(troopRosterElement3);
							}
						}
					}
				}
			}
			return flattenedTroopRoster;
		}

		private bool IsPriorityTroopForSallyOutAmbush(TroopRosterElement troop)
		{
			CharacterObject character = troop.Character;
			return character.IsHero || character.HasMount();
		}

		private readonly TextObject _baseConstructionSpeedText = new TextObject("{=MhGbcXJ4}Base construction speed", null);

		private readonly TextObject _constructionSpeedProjectBonusText = new TextObject("{=xoTWC8Sm}Project Bonus", null);
	}
}
