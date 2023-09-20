using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultClanTierModel : ClanTierModel
	{
		public override int MinClanTier
		{
			get
			{
				return 0;
			}
		}

		public override int MaxClanTier
		{
			get
			{
				return 6;
			}
		}

		public override int MercenaryEligibleTier
		{
			get
			{
				return 1;
			}
		}

		public override int VassalEligibleTier
		{
			get
			{
				return 2;
			}
		}

		public override int BannerEligibleTier
		{
			get
			{
				return 0;
			}
		}

		public override int RebelClanStartingTier
		{
			get
			{
				return 3;
			}
		}

		public override int CompanionToLordClanStartingTier
		{
			get
			{
				return 2;
			}
		}

		private int KingdomEligibleTier
		{
			get
			{
				return Campaign.Current.Models.KingdomCreationModel.MinimumClanTierToCreateKingdom;
			}
		}

		public override int CalculateInitialRenown(Clan clan)
		{
			int num = DefaultClanTierModel.TierLowerRenownLimits[clan.Tier];
			int num2 = ((clan.Tier >= this.MaxClanTier) ? (DefaultClanTierModel.TierLowerRenownLimits[this.MaxClanTier] + 1500) : DefaultClanTierModel.TierLowerRenownLimits[clan.Tier + 1]);
			int num3 = (int)((float)num2 - (float)(num2 - num) * 0.4f);
			return MBRandom.RandomInt(num, num3);
		}

		public override int CalculateInitialInfluence(Clan clan)
		{
			return (int)(150f + (float)MBRandom.RandomInt((int)((float)this.CalculateInitialRenown(clan) / 15f)) + (float)MBRandom.RandomInt(MBRandom.RandomInt(MBRandom.RandomInt(400))));
		}

		public override int CalculateTier(Clan clan)
		{
			int num = this.MinClanTier;
			for (int i = this.MinClanTier + 1; i <= this.MaxClanTier; i++)
			{
				if (clan.Renown >= (float)DefaultClanTierModel.TierLowerRenownLimits[i])
				{
					num = i;
				}
			}
			return num;
		}

		public override ValueTuple<ExplainedNumber, bool> HasUpcomingTier(Clan clan, out TextObject extraExplanation, bool includeDescriptions = false)
		{
			bool flag = clan.Tier < this.MaxClanTier;
			ExplainedNumber explainedNumber = new ExplainedNumber(0f, includeDescriptions, null);
			extraExplanation = TextObject.Empty;
			if (flag)
			{
				int num = this.GetPartyLimitForTier(clan, clan.Tier + 1) - this.GetPartyLimitForTier(clan, clan.Tier);
				if (num != 0)
				{
					explainedNumber.Add((float)num, this._partyLimitBonusText, null);
				}
				int num2 = this.GetCompanionLimitFromTier(clan.Tier + 1) - this.GetCompanionLimitFromTier(clan.Tier);
				if (num2 != 0)
				{
					explainedNumber.Add((float)num2, this._companionLimitBonusText, null);
				}
				int num3 = Campaign.Current.Models.PartySizeLimitModel.GetTierPartySizeEffect(clan.Tier + 1) - Campaign.Current.Models.PartySizeLimitModel.GetTierPartySizeEffect(clan.Tier);
				if (num3 > 0)
				{
					explainedNumber.Add((float)num3, this._additionalCurrentPartySizeBonus, null);
				}
				int num4 = Campaign.Current.Models.WorkshopModel.GetMaxWorkshopCountForClanTier(clan.Tier + 1) - Campaign.Current.Models.WorkshopModel.GetMaxWorkshopCountForClanTier(clan.Tier);
				if (num4 > 0)
				{
					explainedNumber.Add((float)num4, this._additionalWorkshopCountBonus, null);
				}
				if (clan.Tier + 1 == this.MercenaryEligibleTier)
				{
					extraExplanation = this._mercenaryEligibleText;
				}
				else if (clan.Tier + 1 == this.VassalEligibleTier)
				{
					extraExplanation = this._vassalEligibleText;
				}
				else if (clan.Tier + 1 == this.KingdomEligibleTier)
				{
					extraExplanation = this._kingdomEligibleText;
				}
			}
			return new ValueTuple<ExplainedNumber, bool>(explainedNumber, flag);
		}

		public override int GetRequiredRenownForTier(int tier)
		{
			return DefaultClanTierModel.TierLowerRenownLimits[tier];
		}

		public override int GetPartyLimitForTier(Clan clan, int clanTierToCheck)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(0f, false, null);
			if (!clan.IsMinorFaction)
			{
				if (clanTierToCheck < 3)
				{
					explainedNumber.Add(1f, null, null);
				}
				else if (clanTierToCheck < 5)
				{
					explainedNumber.Add(2f, null, null);
				}
				else
				{
					explainedNumber.Add(3f, null, null);
				}
			}
			else
			{
				explainedNumber.Add(MathF.Clamp((float)clanTierToCheck, 1f, 4f), null, null);
			}
			this.AddPartyLimitPerkEffects(clan, ref explainedNumber);
			return MathF.Round(explainedNumber.ResultNumber);
		}

		private void AddPartyLimitPerkEffects(Clan clan, ref ExplainedNumber result)
		{
			if (clan.Leader != null && clan.Leader.GetPerkValue(DefaultPerks.Leadership.TalentMagnet))
			{
				result.Add(DefaultPerks.Leadership.TalentMagnet.SecondaryBonus, DefaultPerks.Leadership.TalentMagnet.Name, null);
			}
		}

		public override int GetCompanionLimit(Clan clan)
		{
			int num = this.GetCompanionLimitFromTier(clan.Tier);
			if (clan.Leader.GetPerkValue(DefaultPerks.Leadership.WePledgeOurSwords))
			{
				num += (int)DefaultPerks.Leadership.WePledgeOurSwords.PrimaryBonus;
			}
			if (clan.Leader.GetPerkValue(DefaultPerks.Charm.Camaraderie))
			{
				num += (int)DefaultPerks.Charm.Camaraderie.SecondaryBonus;
			}
			return num;
		}

		private int GetCompanionLimitFromTier(int clanTier)
		{
			return clanTier + 3;
		}

		private static readonly int[] TierLowerRenownLimits = new int[] { 0, 50, 150, 350, 900, 2350, 6150 };

		private readonly TextObject _partyLimitBonusText = GameTexts.FindText("str_clan_tier_party_limit_bonus", null);

		private readonly TextObject _companionLimitBonusText = GameTexts.FindText("str_clan_tier_companion_limit_bonus", null);

		private readonly TextObject _mercenaryEligibleText = GameTexts.FindText("str_clan_tier_mercenary_eligible", null);

		private readonly TextObject _vassalEligibleText = GameTexts.FindText("str_clan_tier_vassal_eligible", null);

		private readonly TextObject _additionalCurrentPartySizeBonus = GameTexts.FindText("str_clan_tier_party_size_bonus", null);

		private readonly TextObject _additionalWorkshopCountBonus = GameTexts.FindText("str_clan_tier_workshop_count_bonus", null);

		private readonly TextObject _kingdomEligibleText = GameTexts.FindText("str_clan_tier_kingdom_eligible", null);
	}
}
