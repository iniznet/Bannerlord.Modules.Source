using System;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultPartyTrainingModel : PartyTrainingModel
	{
		public override int GetXpReward(CharacterObject character)
		{
			int num = character.Level + 6;
			return num * num / 3;
		}

		public override ExplainedNumber GetEffectiveDailyExperience(MobileParty mobileParty, TroopRosterElement troop)
		{
			ExplainedNumber explainedNumber = default(ExplainedNumber);
			if (mobileParty.IsLordParty && !troop.Character.IsHero && (mobileParty.Army == null || mobileParty.Army.LeaderParty != MobileParty.MainParty) && mobileParty.MapEvent == null && (mobileParty.Party.Owner == null || mobileParty.Party.Owner.Clan != Clan.PlayerClan))
			{
				if (mobileParty.LeaderHero != null && mobileParty.LeaderHero == mobileParty.ActualClan.Leader)
				{
					explainedNumber.Add(15f + (float)troop.Character.Tier * 3f, null, null);
				}
				else
				{
					explainedNumber.Add(10f + (float)troop.Character.Tier * 2f, null, null);
				}
			}
			if (mobileParty.IsActive && mobileParty.HasPerk(DefaultPerks.Leadership.CombatTips, false))
			{
				explainedNumber.Add((float)this.GetPerkExperiencesForTroops(DefaultPerks.Leadership.CombatTips), null, null);
			}
			if (mobileParty.IsActive && mobileParty.HasPerk(DefaultPerks.Leadership.RaiseTheMeek, false) && troop.Character.Tier < 3)
			{
				explainedNumber.Add((float)this.GetPerkExperiencesForTroops(DefaultPerks.Leadership.RaiseTheMeek), null, null);
			}
			if (mobileParty.IsGarrison)
			{
				Settlement currentSettlement = mobileParty.CurrentSettlement;
				if (((currentSettlement != null) ? currentSettlement.Town.Governor : null) != null && mobileParty.CurrentSettlement.Town.Governor.GetPerkValue(DefaultPerks.Bow.BullsEye))
				{
					explainedNumber.Add((float)this.GetPerkExperiencesForTroops(DefaultPerks.Bow.BullsEye), null, null);
				}
			}
			if (mobileParty.IsActive && mobileParty.HasPerk(DefaultPerks.Polearm.Drills, true))
			{
				explainedNumber.Add((float)this.GetPerkExperiencesForTroops(DefaultPerks.Polearm.Drills), null, null);
			}
			if (mobileParty.IsActive && mobileParty.HasPerk(DefaultPerks.OneHanded.MilitaryTradition, false) && troop.Character.IsInfantry)
			{
				explainedNumber.Add((float)this.GetPerkExperiencesForTroops(DefaultPerks.OneHanded.MilitaryTradition), null, null);
			}
			if (mobileParty.IsActive && mobileParty.HasPerk(DefaultPerks.Athletics.WalkItOff, true) && !troop.Character.IsMounted && mobileParty.IsMoving)
			{
				explainedNumber.Add((float)this.GetPerkExperiencesForTroops(DefaultPerks.Athletics.WalkItOff), null, null);
			}
			if (mobileParty.IsActive && mobileParty.HasPerk(DefaultPerks.Throwing.Saddlebags, true) && troop.Character.IsInfantry)
			{
				explainedNumber.Add((float)this.GetPerkExperiencesForTroops(DefaultPerks.Throwing.Saddlebags), null, null);
			}
			if (mobileParty.IsActive && mobileParty.HasPerk(DefaultPerks.Athletics.AGoodDaysRest, true) && !troop.Character.IsMounted && !mobileParty.IsMoving && mobileParty.CurrentSettlement != null)
			{
				explainedNumber.Add((float)this.GetPerkExperiencesForTroops(DefaultPerks.Athletics.AGoodDaysRest), null, null);
			}
			if (mobileParty.IsActive && mobileParty.HasPerk(DefaultPerks.Bow.Trainer, true) && troop.Character.IsRanged)
			{
				explainedNumber.Add((float)this.GetPerkExperiencesForTroops(DefaultPerks.Bow.Trainer), null, null);
			}
			if (mobileParty.IsActive && mobileParty.HasPerk(DefaultPerks.Crossbow.RenownMarksmen, false) && troop.Character.IsRanged)
			{
				explainedNumber.Add((float)this.GetPerkExperiencesForTroops(DefaultPerks.Crossbow.RenownMarksmen), null, null);
			}
			if (mobileParty.IsActive && mobileParty.IsMoving)
			{
				if (mobileParty.Morale > 75f)
				{
					PerkHelper.AddPerkBonusForParty(DefaultPerks.Scouting.ForcedMarch, mobileParty, false, ref explainedNumber);
				}
				if (mobileParty.ItemRoster.TotalWeight > (float)mobileParty.InventoryCapacity)
				{
					PerkHelper.AddPerkBonusForParty(DefaultPerks.Scouting.Unburdened, mobileParty, false, ref explainedNumber);
				}
			}
			if (mobileParty.IsActive && mobileParty.HasPerk(DefaultPerks.Steward.SevenVeterans, false) && troop.Character.Tier >= 4)
			{
				explainedNumber.Add((float)this.GetPerkExperiencesForTroops(DefaultPerks.Steward.SevenVeterans), null, null);
			}
			if (mobileParty.IsActive && mobileParty.HasPerk(DefaultPerks.Steward.DrillSergant, false))
			{
				explainedNumber.Add((float)this.GetPerkExperiencesForTroops(DefaultPerks.Steward.DrillSergant), null, null);
			}
			if (troop.Character.Culture.IsBandit)
			{
				PerkHelper.AddPerkBonusForParty(DefaultPerks.Roguery.NoRestForTheWicked, mobileParty, true, ref explainedNumber);
			}
			return explainedNumber;
		}

		private int GetPerkExperiencesForTroops(PerkObject perk)
		{
			if (perk == DefaultPerks.Leadership.CombatTips || perk == DefaultPerks.Leadership.RaiseTheMeek || perk == DefaultPerks.OneHanded.MilitaryTradition || perk == DefaultPerks.Crossbow.RenownMarksmen || perk == DefaultPerks.Steward.SevenVeterans || perk == DefaultPerks.Steward.DrillSergant)
			{
				return MathF.Round(perk.PrimaryBonus);
			}
			if (perk == DefaultPerks.Polearm.Drills || perk == DefaultPerks.Athletics.WalkItOff || perk == DefaultPerks.Athletics.AGoodDaysRest || perk == DefaultPerks.Bow.Trainer || perk == DefaultPerks.Bow.BullsEye || perk == DefaultPerks.Throwing.Saddlebags)
			{
				return MathF.Round(perk.SecondaryBonus);
			}
			return 0;
		}

		public override int GenerateSharedXp(CharacterObject troop, int xp, MobileParty mobileParty)
		{
			float num = (float)xp * 0.05f;
			if (troop.IsHero && !mobileParty.HasPerk(DefaultPerks.Leadership.LeaderOfMasses, true))
			{
				return 0;
			}
			if (troop.IsRanged && troop.IsRegular && mobileParty.HasPerk(DefaultPerks.Leadership.MakeADifference, true))
			{
				num += num * DefaultPerks.Leadership.MakeADifference.SecondaryBonus;
			}
			if (troop.IsMounted && troop.IsRegular && mobileParty.HasPerk(DefaultPerks.Leadership.LeadByExample, true))
			{
				num += num * DefaultPerks.Leadership.LeadByExample.SecondaryBonus;
			}
			return (int)num;
		}

		public override int CalculateXpGainFromBattles(FlattenedTroopRosterElement troopRosterElement, PartyBase party)
		{
			int num = troopRosterElement.XpGained;
			if ((party.MapEvent.IsPlayerSimulation || !party.MapEvent.IsPlayerMapEvent) && party.MobileParty.HasPerk(DefaultPerks.Leadership.TrustedCommander, true))
			{
				num += MathF.Round((float)num * DefaultPerks.Leadership.TrustedCommander.SecondaryBonus);
			}
			return num;
		}
	}
}
