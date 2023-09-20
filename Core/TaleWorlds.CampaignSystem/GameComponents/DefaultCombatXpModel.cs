using System;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x020000FE RID: 254
	public class DefaultCombatXpModel : CombatXpModel
	{
		// Token: 0x0600150F RID: 5391 RVA: 0x00060790 File Offset: 0x0005E990
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

		// Token: 0x06001510 RID: 5392 RVA: 0x000607BC File Offset: 0x0005E9BC
		public override void GetXpFromHit(CharacterObject attackerTroop, CharacterObject captain, CharacterObject attackedTroop, PartyBase party, int damage, bool isFatal, CombatXpModel.MissionTypeEnum missionType, out int xpAmount)
		{
			int num = attackedTroop.MaxHitPoints();
			float num2;
			float num3;
			if (((party != null) ? party.MapEvent : null) != null)
			{
				MilitaryPowerModel militaryPowerModel = Campaign.Current.Models.MilitaryPowerModel;
				MapEvent mapEvent = party.MapEvent;
				num2 = militaryPowerModel.GetTroopPowerBasedOnContext(attackedTroop, (mapEvent != null) ? mapEvent.EventType : MapEvent.BattleTypes.None, party.Side, missionType == CombatXpModel.MissionTypeEnum.SimulationBattle);
				MilitaryPowerModel militaryPowerModel2 = Campaign.Current.Models.MilitaryPowerModel;
				MapEvent mapEvent2 = party.MapEvent;
				num3 = militaryPowerModel2.GetTroopPowerBasedOnContext(attackerTroop, (mapEvent2 != null) ? mapEvent2.EventType : MapEvent.BattleTypes.None, party.Side, missionType == CombatXpModel.MissionTypeEnum.SimulationBattle);
			}
			else
			{
				num2 = Campaign.Current.Models.MilitaryPowerModel.GetTroopPowerBasedOnContext(attackedTroop, MapEvent.BattleTypes.None, BattleSideEnum.None, false);
				num3 = Campaign.Current.Models.MilitaryPowerModel.GetTroopPowerBasedOnContext(attackerTroop, MapEvent.BattleTypes.None, BattleSideEnum.None, false);
			}
			float num4 = 0.4f * (num3 + 0.5f) * (num2 + 0.5f) * (float)(MathF.Min(damage, num) + (isFatal ? num : 0));
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

		// Token: 0x06001511 RID: 5393 RVA: 0x00060951 File Offset: 0x0005EB51
		public override float GetXpMultiplierFromShotDifficulty(float shotDifficulty)
		{
			if (shotDifficulty > 14.4f)
			{
				shotDifficulty = 14.4f;
			}
			return MBMath.Lerp(0f, 2f, (shotDifficulty - 1f) / 13.4f, 1E-05f);
		}

		// Token: 0x170005DF RID: 1503
		// (get) Token: 0x06001512 RID: 5394 RVA: 0x00060983 File Offset: 0x0005EB83
		public override float CaptainRadius
		{
			get
			{
				return 10f;
			}
		}

		// Token: 0x06001513 RID: 5395 RVA: 0x0006098C File Offset: 0x0005EB8C
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
					Hero leaderHero = party.MobileParty.LeaderHero;
					if (leaderHero != null && leaderHero.GetPerkValue(DefaultPerks.Bow.BullsEye))
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
