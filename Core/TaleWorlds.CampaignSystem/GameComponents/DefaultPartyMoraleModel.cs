using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultPartyMoraleModel : PartyMoraleModel
	{
		public override float HighMoraleValue
		{
			get
			{
				return 70f;
			}
		}

		public override int GetDailyStarvationMoralePenalty(PartyBase party)
		{
			return -5;
		}

		public override int GetDailyNoWageMoralePenalty(MobileParty party)
		{
			return -3;
		}

		private int GetStarvationMoralePenalty(MobileParty party)
		{
			return -30;
		}

		private int GetNoWageMoralePenalty(MobileParty party)
		{
			return -20;
		}

		public override float GetStandardBaseMorale(PartyBase party)
		{
			return 50f;
		}

		public override float GetVictoryMoraleChange(PartyBase party)
		{
			return 20f;
		}

		public override float GetDefeatMoraleChange(PartyBase party)
		{
			return -20f;
		}

		private void CalculateFoodVarietyMoraleBonus(MobileParty party, ref ExplainedNumber result)
		{
			if (!party.Party.IsStarving)
			{
				float num;
				switch (party.ItemRoster.FoodVariety)
				{
				case 0:
				case 1:
					num = -2f;
					break;
				case 2:
					num = -1f;
					break;
				case 3:
					num = 0f;
					break;
				case 4:
					num = 1f;
					break;
				case 5:
					num = 2f;
					break;
				case 6:
					num = 3f;
					break;
				case 7:
					num = 5f;
					break;
				case 8:
					num = 6f;
					break;
				case 9:
					num = 7f;
					break;
				case 10:
					num = 8f;
					break;
				case 11:
					num = 9f;
					break;
				case 12:
					num = 10f;
					break;
				default:
					num = 10f;
					break;
				}
				if (num < 0f && party.LeaderHero != null && party.LeaderHero.GetPerkValue(DefaultPerks.Steward.WarriorsDiet))
				{
					num = 0f;
				}
				if (num != 0f)
				{
					result.Add(num, this._foodBonusMoraleText, null);
					if (num > 0f && party.HasPerk(DefaultPerks.Steward.Gourmet, false))
					{
						result.Add(num * DefaultPerks.Steward.Gourmet.PrimaryBonus, DefaultPerks.Steward.Gourmet.Name, null);
					}
				}
			}
		}

		private void GetPartySizeMoraleEffect(MobileParty mobileParty, ref ExplainedNumber result)
		{
			if (!mobileParty.IsMilitia && !mobileParty.IsVillager)
			{
				int num = mobileParty.Party.NumberOfAllMembers - mobileParty.Party.PartySizeLimit;
				if (num > 0)
				{
					result.Add(-1f * MathF.Sqrt((float)num), this._partySizeMoraleText, null);
				}
			}
		}

		private static void CheckPerkEffectOnPartyMorale(MobileParty party, PerkObject perk, bool isInfoNeeded, TextObject newInfo, int perkEffect, out TextObject outNewInfo, out int outPerkEffect)
		{
			outNewInfo = newInfo;
			outPerkEffect = perkEffect;
			if (party.LeaderHero != null && party.LeaderHero.GetPerkValue(perk))
			{
				if (isInfoNeeded)
				{
					MBTextManager.SetTextVariable("EFFECT_NAME", perk.Name, false);
					MBTextManager.SetTextVariable("NUM", 10);
					MBTextManager.SetTextVariable("STR1", newInfo, false);
					MBTextManager.SetTextVariable("STR2", GameTexts.FindText("str_party_effect", null), false);
					outNewInfo = GameTexts.FindText("str_new_item_line", null);
				}
				outPerkEffect += 10;
			}
		}

		private void GetMoraleEffectsFromPerks(MobileParty party, ref ExplainedNumber bonus)
		{
			if (party.HasPerk(DefaultPerks.Crossbow.PeasantLeader, false))
			{
				float num = this.CalculateTroopTierRatio(party);
				bonus.AddFactor(DefaultPerks.Crossbow.PeasantLeader.PrimaryBonus * num, DefaultPerks.Crossbow.PeasantLeader.Name);
			}
			Settlement currentSettlement = party.CurrentSettlement;
			if (((currentSettlement != null) ? currentSettlement.SiegeEvent : null) != null && party.HasPerk(DefaultPerks.Charm.SelfPromoter, true))
			{
				bonus.Add(DefaultPerks.Charm.SelfPromoter.SecondaryBonus, DefaultPerks.Charm.SelfPromoter.Name, null);
			}
			if (party.HasPerk(DefaultPerks.Steward.Logistician, false))
			{
				int num2 = 0;
				for (int i = 0; i < party.MemberRoster.Count; i++)
				{
					TroopRosterElement elementCopyAtIndex = party.MemberRoster.GetElementCopyAtIndex(i);
					if (elementCopyAtIndex.Character.IsMounted)
					{
						num2 += elementCopyAtIndex.Number;
					}
				}
				if (party.Party.NumberOfMounts > party.MemberRoster.TotalManCount - num2)
				{
					bonus.Add(DefaultPerks.Steward.Logistician.PrimaryBonus, DefaultPerks.Steward.Logistician.Name, null);
				}
			}
		}

		private float CalculateTroopTierRatio(MobileParty party)
		{
			int totalManCount = party.MemberRoster.TotalManCount;
			float num = 0f;
			foreach (TroopRosterElement troopRosterElement in party.MemberRoster.GetTroopRoster())
			{
				if (troopRosterElement.Character.Tier <= 3)
				{
					num += (float)troopRosterElement.Number;
				}
			}
			return num / (float)totalManCount;
		}

		private void GetMoraleEffectsFromSkill(MobileParty party, ref ExplainedNumber bonus)
		{
			CharacterObject effectivePartyLeaderForSkill = SkillHelper.GetEffectivePartyLeaderForSkill(party.Party);
			if (effectivePartyLeaderForSkill != null && effectivePartyLeaderForSkill.GetSkillValue(DefaultSkills.Leadership) > 0)
			{
				SkillHelper.AddSkillBonusForCharacter(DefaultSkills.Leadership, DefaultSkillEffects.LeadershipMoraleBonus, effectivePartyLeaderForSkill, ref bonus, -1, true, 0);
			}
		}

		public override ExplainedNumber GetEffectivePartyMorale(MobileParty mobileParty, bool includeDescription = false)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(50f, includeDescription, null);
			explainedNumber.Add(mobileParty.RecentEventsMorale, this._recentEventsText, null);
			this.GetMoraleEffectsFromSkill(mobileParty, ref explainedNumber);
			if (mobileParty.IsMilitia || mobileParty.IsGarrison)
			{
				if (mobileParty.IsMilitia)
				{
					if (mobileParty.HomeSettlement.IsStarving)
					{
						explainedNumber.Add((float)this.GetStarvationMoralePenalty(mobileParty), this._starvationMoraleText, null);
					}
				}
				else if (SettlementHelper.IsGarrisonStarving(mobileParty.CurrentSettlement))
				{
					explainedNumber.Add((float)this.GetStarvationMoralePenalty(mobileParty), this._starvationMoraleText, null);
				}
			}
			else if (mobileParty.Party.IsStarving)
			{
				explainedNumber.Add((float)this.GetStarvationMoralePenalty(mobileParty), this._starvationMoraleText, null);
			}
			if (mobileParty.HasUnpaidWages > 0f)
			{
				explainedNumber.Add(mobileParty.HasUnpaidWages * (float)this.GetNoWageMoralePenalty(mobileParty), this._noWageMoraleText, null);
			}
			this.GetMoraleEffectsFromPerks(mobileParty, ref explainedNumber);
			this.CalculateFoodVarietyMoraleBonus(mobileParty, ref explainedNumber);
			this.GetPartySizeMoraleEffect(mobileParty, ref explainedNumber);
			return explainedNumber;
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("show_party_morale_detail", "campaign")]
		public static string ShowPartyMoraleDetail(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.show_party_morale_detail [PartyName]\".";
			}
			string text = CampaignCheats.ConcatenateString(strings);
			foreach (MobileParty mobileParty in MobileParty.All)
			{
				if (string.Equals(text, mobileParty.Name.ToString(), StringComparison.OrdinalIgnoreCase))
				{
					return mobileParty.MoraleExplained.ToString();
				}
			}
			return "Couldn't find the party: " + text;
		}

		private const float BaseMoraleValue = 50f;

		private readonly TextObject _recentEventsText = GameTexts.FindText("str_recent_events", null);

		private readonly TextObject _starvationMoraleText = GameTexts.FindText("str_starvation_morale", null);

		private readonly TextObject _noWageMoraleText = GameTexts.FindText("str_no_wage_morale", null);

		private readonly TextObject _foodBonusMoraleText = GameTexts.FindText("str_food_bonus_morale", null);

		private readonly TextObject _partySizeMoraleText = GameTexts.FindText("str_party_size_morale", null);
	}
}
