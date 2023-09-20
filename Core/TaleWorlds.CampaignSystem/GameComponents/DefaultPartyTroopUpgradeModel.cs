using System;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x02000129 RID: 297
	public class DefaultPartyTroopUpgradeModel : PartyTroopUpgradeModel
	{
		// Token: 0x060016AD RID: 5805 RVA: 0x0006E7E0 File Offset: 0x0006C9E0
		public override bool CanPartyUpgradeTroopToTarget(PartyBase upgradingParty, CharacterObject upgradeableCharacter, CharacterObject upgradeTarget)
		{
			bool flag = this.DoesPartyHaveRequiredItemsForUpgrade(upgradingParty, upgradeTarget);
			PerkObject perkObject;
			bool flag2 = this.DoesPartyHaveRequiredPerksForUpgrade(upgradingParty, upgradeableCharacter, upgradeTarget, out perkObject);
			return this.IsTroopUpgradeable(upgradingParty, upgradeableCharacter) && upgradeableCharacter.UpgradeTargets.Contains(upgradeTarget) && flag2 && flag;
		}

		// Token: 0x060016AE RID: 5806 RVA: 0x0006E81F File Offset: 0x0006CA1F
		public override bool IsTroopUpgradeable(PartyBase party, CharacterObject character)
		{
			return !character.IsHero && character.UpgradeTargets.Length != 0;
		}

		// Token: 0x060016AF RID: 5807 RVA: 0x0006E838 File Offset: 0x0006CA38
		public override int GetXpCostForUpgrade(PartyBase party, CharacterObject characterObject, CharacterObject upgradeTarget)
		{
			if (upgradeTarget != null && characterObject.UpgradeTargets.Contains(upgradeTarget))
			{
				int tier = upgradeTarget.Tier;
				int num = 0;
				for (int i = characterObject.Tier + 1; i <= tier; i++)
				{
					if (i <= 1)
					{
						num += 100;
					}
					else if (i == 2)
					{
						num += 300;
					}
					else if (i == 3)
					{
						num += 550;
					}
					else if (i == 4)
					{
						num += 900;
					}
					else if (i == 5)
					{
						num += 1300;
					}
					else if (i == 6)
					{
						num += 1700;
					}
					else if (i == 7)
					{
						num += 2100;
					}
					else
					{
						int num2 = upgradeTarget.Level + 4;
						num += (int)(1.333f * (float)num2 * (float)num2);
					}
				}
				return num;
			}
			return 100000000;
		}

		// Token: 0x060016B0 RID: 5808 RVA: 0x0006E8F8 File Offset: 0x0006CAF8
		public override int GetGoldCostForUpgrade(PartyBase party, CharacterObject characterObject, CharacterObject upgradeTarget)
		{
			PartyWageModel partyWageModel = Campaign.Current.Models.PartyWageModel;
			int troopRecruitmentCost = partyWageModel.GetTroopRecruitmentCost(upgradeTarget, null, true);
			int troopRecruitmentCost2 = partyWageModel.GetTroopRecruitmentCost(characterObject, null, true);
			bool flag = characterObject.Occupation == Occupation.Mercenary || characterObject.Occupation == Occupation.Gangster;
			ExplainedNumber explainedNumber = new ExplainedNumber((float)(troopRecruitmentCost - troopRecruitmentCost2) / ((!flag) ? 2f : 3f), false, null);
			if (party.IsMobile && party.LeaderHero != null)
			{
				if (party.MobileParty.HasPerk(DefaultPerks.Bow.RenownedArcher, true))
				{
					PerkHelper.AddPerkBonusForParty(DefaultPerks.Bow.RenownedArcher, party.MobileParty, false, ref explainedNumber);
				}
				if (party.IsMobile && party.LeaderHero != null && party.MobileParty.HasPerk(DefaultPerks.Steward.SoundReserves, false))
				{
					PerkHelper.AddPerkBonusForParty(DefaultPerks.Steward.SoundReserves, party.MobileParty, true, ref explainedNumber);
				}
			}
			if (characterObject.IsInfantry && party.MobileParty.HasPerk(DefaultPerks.Throwing.ThrowingCompetitions, false))
			{
				PerkHelper.AddPerkBonusForParty(DefaultPerks.Throwing.ThrowingCompetitions, party.MobileParty, true, ref explainedNumber);
			}
			if (characterObject.IsMounted && PartyBaseHelper.HasFeat(party, DefaultCulturalFeats.KhuzaitRecruitUpgradeFeat))
			{
				explainedNumber.AddFactor(DefaultCulturalFeats.KhuzaitRecruitUpgradeFeat.EffectBonus, GameTexts.FindText("str_culture", null));
			}
			else if (characterObject.IsInfantry && PartyBaseHelper.HasFeat(party, DefaultCulturalFeats.SturgianRecruitUpgradeFeat))
			{
				explainedNumber.AddFactor(DefaultCulturalFeats.SturgianRecruitUpgradeFeat.EffectBonus, GameTexts.FindText("str_culture", null));
			}
			return (int)explainedNumber.ResultNumber;
		}

		// Token: 0x060016B1 RID: 5809 RVA: 0x0006EA61 File Offset: 0x0006CC61
		public override int GetSkillXpFromUpgradingTroops(PartyBase party, CharacterObject troop, int numberOfTroops)
		{
			return (troop.Level + 10) * numberOfTroops;
		}

		// Token: 0x060016B2 RID: 5810 RVA: 0x0006EA70 File Offset: 0x0006CC70
		public override bool DoesPartyHaveRequiredItemsForUpgrade(PartyBase party, CharacterObject upgradeTarget)
		{
			ItemCategory upgradeRequiresItemFromCategory = upgradeTarget.UpgradeRequiresItemFromCategory;
			if (upgradeRequiresItemFromCategory != null)
			{
				int num = 0;
				for (int i = 0; i < party.ItemRoster.Count; i++)
				{
					ItemRosterElement itemRosterElement = party.ItemRoster[i];
					if (itemRosterElement.EquipmentElement.Item.ItemCategory == upgradeRequiresItemFromCategory)
					{
						num += itemRosterElement.Amount;
					}
				}
				return num > 0;
			}
			return true;
		}

		// Token: 0x060016B3 RID: 5811 RVA: 0x0006EAD4 File Offset: 0x0006CCD4
		public override bool DoesPartyHaveRequiredPerksForUpgrade(PartyBase party, CharacterObject character, CharacterObject upgradeTarget, out PerkObject requiredPerk)
		{
			requiredPerk = null;
			if (character.Culture.IsBandit && !upgradeTarget.Culture.IsBandit)
			{
				requiredPerk = DefaultPerks.Leadership.VeteransRespect;
				return party.MobileParty.HasPerk(requiredPerk, true);
			}
			return true;
		}

		// Token: 0x060016B4 RID: 5812 RVA: 0x0006EB10 File Offset: 0x0006CD10
		public override bool CanTroopGainXp(PartyBase owner, CharacterObject character)
		{
			for (int i = 0; i < character.UpgradeTargets.Length; i++)
			{
				CharacterObject characterObject = character.UpgradeTargets[i];
				int num = owner.MemberRoster.FindIndexOfTroop(character);
				int elementNumber = owner.MemberRoster.GetElementNumber(num);
				int elementXp = owner.MemberRoster.GetElementXp(num);
				int upgradeXpCost = character.GetUpgradeXpCost(owner, i);
				bool flag = elementXp >= upgradeXpCost * elementNumber;
				PerkObject perkObject;
				if (this.DoesPartyHaveRequiredPerksForUpgrade(owner, character, characterObject, out perkObject) && !flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060016B5 RID: 5813 RVA: 0x0006EB88 File Offset: 0x0006CD88
		public override float GetUpgradeChanceForTroopUpgrade(PartyBase party, CharacterObject troop, int upgradeTargetIndex)
		{
			float num = 1f;
			int num2 = troop.UpgradeTargets.Length;
			if (num2 > 1 && upgradeTargetIndex >= 0 && upgradeTargetIndex < num2)
			{
				if (party.LeaderHero != null && party.LeaderHero.PreferredUpgradeFormation != FormationClass.NumberOfAllFormations)
				{
					FormationClass preferredUpgradeFormation = party.LeaderHero.PreferredUpgradeFormation;
					if (CharacterHelper.SearchForFormationInTroopTree(troop.UpgradeTargets[upgradeTargetIndex], preferredUpgradeFormation))
					{
						num = 9999f;
					}
				}
				else
				{
					Hero leaderHero = party.LeaderHero;
					int num3 = ((leaderHero != null) ? leaderHero.RandomValue : party.Id.GetHashCode());
					int deterministicHashCode = troop.StringId.GetDeterministicHashCode();
					uint num4 = (uint)((num3 >> ((troop.Tier * 3) & 31)) ^ deterministicHashCode);
					if ((long)upgradeTargetIndex == (long)((ulong)num4 % (ulong)((long)num2)))
					{
						num = 9999f;
					}
				}
			}
			return num;
		}
	}
}
