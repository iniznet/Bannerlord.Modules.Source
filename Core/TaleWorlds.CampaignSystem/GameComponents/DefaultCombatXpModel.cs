using System;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultCombatXpModel : CombatXpModel
	{
		public override SkillObject GetSkillForWeapon(WeaponComponentData weapon, bool isSiegeEngineHit)
		{
			SkillObject skillObject = DefaultSkills.Athletics;
			if (isSiegeEngineHit)
			{
				skillObject = DefaultSkills.Engineering;
			}
			else if (weapon != null)
			{
				skillObject = weapon.RelevantSkill;
			}
			return skillObject;
		}

		public override void GetXpFromHit(CharacterObject attackerTroop, CharacterObject captain, CharacterObject attackedTroop, PartyBase party, int damage, bool isFatal, CombatXpModel.MissionTypeEnum missionType, out int xpAmount)
		{
			int num = attackedTroop.MaxHitPoints();
			MilitaryPowerModel militaryPowerModel = Campaign.Current.Models.MilitaryPowerModel;
			float defaultTroopPower = militaryPowerModel.GetDefaultTroopPower(attackedTroop);
			float defaultTroopPower2 = militaryPowerModel.GetDefaultTroopPower(attackerTroop);
			float num2 = 0f;
			float num3 = 0f;
			if (((party != null) ? party.MapEvent : null) != null)
			{
				num3 = militaryPowerModel.GetContextModifier(attackedTroop, party.Side, party.MapEvent.SimulationContext);
				num2 = party.MapEventSide.LeaderSimulationModifier;
			}
			float troopPower = militaryPowerModel.GetTroopPower(defaultTroopPower, num2, num3);
			float troopPower2 = militaryPowerModel.GetTroopPower(defaultTroopPower2, num2, num3);
			float num4 = 0.4f * (troopPower2 + 0.5f) * (troopPower + 0.5f) * (float)(MathF.Min(damage, num) + (isFatal ? num : 0));
			num4 *= ((missionType == CombatXpModel.MissionTypeEnum.NoXp) ? 0f : ((missionType == CombatXpModel.MissionTypeEnum.PracticeFight) ? 0.0625f : ((missionType == CombatXpModel.MissionTypeEnum.Tournament) ? 0.33f : ((missionType == CombatXpModel.MissionTypeEnum.SimulationBattle) ? 0.9f : ((missionType == CombatXpModel.MissionTypeEnum.Battle) ? 1f : 1f)))));
			ExplainedNumber explainedNumber = new ExplainedNumber(num4, false, null);
			if (party != null)
			{
				this.GetBattleXpBonusFromPerks(party, ref explainedNumber, attackerTroop);
			}
			if (captain != null && captain.IsHero && captain.GetPerkValue(DefaultPerks.Leadership.InspiringLeader))
			{
				explainedNumber.AddFactor(DefaultPerks.Leadership.InspiringLeader.SecondaryBonus, DefaultPerks.Leadership.InspiringLeader.Name);
			}
			xpAmount = MathF.Round(explainedNumber.ResultNumber);
		}

		public override float GetXpMultiplierFromShotDifficulty(float shotDifficulty)
		{
			if (shotDifficulty > 14.4f)
			{
				shotDifficulty = 14.4f;
			}
			return MBMath.Lerp(0f, 2f, (shotDifficulty - 1f) / 13.4f, 1E-05f);
		}

		public override float CaptainRadius
		{
			get
			{
				return 10f;
			}
		}

		private void GetBattleXpBonusFromPerks(PartyBase party, ref ExplainedNumber xpToGain, CharacterObject troop)
		{
			if (party.IsMobile && party.MobileParty.LeaderHero != null)
			{
				if (!troop.IsRanged && party.MobileParty.HasPerk(DefaultPerks.OneHanded.Trainer, true))
				{
					xpToGain.AddFactor(DefaultPerks.OneHanded.Trainer.SecondaryBonus, DefaultPerks.OneHanded.Trainer.Name);
				}
				if (troop.HasThrowingWeapon() && party.MobileParty.HasPerk(DefaultPerks.Throwing.Resourceful, true))
				{
					xpToGain.AddFactor(DefaultPerks.Throwing.Resourceful.SecondaryBonus, DefaultPerks.Throwing.Resourceful.Name);
				}
				if (troop.IsInfantry)
				{
					if (party.MobileParty.HasPerk(DefaultPerks.OneHanded.CorpsACorps, false))
					{
						xpToGain.AddFactor(DefaultPerks.OneHanded.CorpsACorps.PrimaryBonus, DefaultPerks.OneHanded.CorpsACorps.Name);
					}
					if (party.MobileParty.HasPerk(DefaultPerks.TwoHanded.BaptisedInBlood, true))
					{
						xpToGain.AddFactor(DefaultPerks.TwoHanded.BaptisedInBlood.SecondaryBonus, DefaultPerks.TwoHanded.BaptisedInBlood.Name);
					}
				}
				if (party.MobileParty.HasPerk(DefaultPerks.OneHanded.LeadByExample, false))
				{
					xpToGain.AddFactor(DefaultPerks.OneHanded.LeadByExample.PrimaryBonus, DefaultPerks.OneHanded.LeadByExample.Name);
				}
				if (troop.IsRanged)
				{
					if (party.MobileParty.HasPerk(DefaultPerks.Crossbow.MountedCrossbowman, true))
					{
						xpToGain.AddFactor(DefaultPerks.Crossbow.MountedCrossbowman.SecondaryBonus, DefaultPerks.Crossbow.MountedCrossbowman.Name);
					}
					if (party.MobileParty.HasPerk(DefaultPerks.Bow.BullsEye, false))
					{
						xpToGain.AddFactor(DefaultPerks.Bow.BullsEye.PrimaryBonus, DefaultPerks.Bow.BullsEye.Name);
					}
				}
				if (troop.Culture.IsBandit && party.MobileParty.HasPerk(DefaultPerks.Roguery.NoRestForTheWicked, false))
				{
					xpToGain.AddFactor(DefaultPerks.Roguery.NoRestForTheWicked.PrimaryBonus, DefaultPerks.Roguery.NoRestForTheWicked.Name);
				}
			}
			if (party.IsMobile && party.MobileParty.IsGarrison)
			{
				Settlement currentSettlement = party.MobileParty.CurrentSettlement;
				if (((currentSettlement != null) ? currentSettlement.Town.Governor : null) != null)
				{
					PerkHelper.AddPerkBonusForTown(DefaultPerks.TwoHanded.ProjectileDeflection, party.MobileParty.CurrentSettlement.Town, ref xpToGain);
					if (troop.IsMounted)
					{
						PerkHelper.AddPerkBonusForTown(DefaultPerks.Polearm.Guards, party.MobileParty.CurrentSettlement.Town, ref xpToGain);
					}
				}
			}
		}
	}
}
