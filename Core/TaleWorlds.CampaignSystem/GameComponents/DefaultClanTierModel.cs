using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x020000FC RID: 252
	public class DefaultClanTierModel : ClanTierModel
	{
		// Token: 0x170005D7 RID: 1495
		// (get) Token: 0x060014F2 RID: 5362 RVA: 0x0005F8ED File Offset: 0x0005DAED
		public override int MinClanTier
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x170005D8 RID: 1496
		// (get) Token: 0x060014F3 RID: 5363 RVA: 0x0005F8F0 File Offset: 0x0005DAF0
		public override int MaxClanTier
		{
			get
			{
				return 6;
			}
		}

		// Token: 0x170005D9 RID: 1497
		// (get) Token: 0x060014F4 RID: 5364 RVA: 0x0005F8F3 File Offset: 0x0005DAF3
		public override int MercenaryEligibleTier
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x170005DA RID: 1498
		// (get) Token: 0x060014F5 RID: 5365 RVA: 0x0005F8F6 File Offset: 0x0005DAF6
		public override int VassalEligibleTier
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x170005DB RID: 1499
		// (get) Token: 0x060014F6 RID: 5366 RVA: 0x0005F8F9 File Offset: 0x0005DAF9
		public override int BannerEligibleTier
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x170005DC RID: 1500
		// (get) Token: 0x060014F7 RID: 5367 RVA: 0x0005F8FC File Offset: 0x0005DAFC
		public override int RebelClanStartingTier
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x170005DD RID: 1501
		// (get) Token: 0x060014F8 RID: 5368 RVA: 0x0005F8FF File Offset: 0x0005DAFF
		public override int CompanionToLordClanStartingTier
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x170005DE RID: 1502
		// (get) Token: 0x060014F9 RID: 5369 RVA: 0x0005F902 File Offset: 0x0005DB02
		private int KingdomEligibleTier
		{
			get
			{
				return Campaign.Current.Models.KingdomCreationModel.MinimumClanTierToCreateKingdom;
			}
		}

		// Token: 0x060014FA RID: 5370 RVA: 0x0005F918 File Offset: 0x0005DB18
		public override int CalculateInitialRenown(Clan clan)
		{
			int num = DefaultClanTierModel.TierLowerRenownLimits[clan.Tier];
			int num2 = ((clan.Tier >= this.MaxClanTier) ? (DefaultClanTierModel.TierLowerRenownLimits[this.MaxClanTier] + 1500) : DefaultClanTierModel.TierLowerRenownLimits[clan.Tier + 1]);
			int num3 = (int)((float)num2 - (float)(num2 - num) * 0.4f);
			return MBRandom.RandomInt(num, num3);
		}

		// Token: 0x060014FB RID: 5371 RVA: 0x0005F979 File Offset: 0x0005DB79
		public override int CalculateInitialInfluence(Clan clan)
		{
			return (int)(150f + (float)MBRandom.RandomInt((int)((float)this.CalculateInitialRenown(clan) / 15f)) + (float)MBRandom.RandomInt(MBRandom.RandomInt(MBRandom.RandomInt(400))));
		}

		// Token: 0x060014FC RID: 5372 RVA: 0x0005F9B0 File Offset: 0x0005DBB0
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

		// Token: 0x060014FD RID: 5373 RVA: 0x0005F9F0 File Offset: 0x0005DBF0
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
				int num4 = Campaign.Current.Models.WorkshopModel.GetMaxWorkshopCountForTier(clan.Tier + 1) - Campaign.Current.Models.WorkshopModel.GetMaxWorkshopCountForTier(clan.Tier);
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

		// Token: 0x060014FE RID: 5374 RVA: 0x0005FB78 File Offset: 0x0005DD78
		public override int GetRequiredRenownForTier(int tier)
		{
			return DefaultClanTierModel.TierLowerRenownLimits[tier];
		}

		// Token: 0x060014FF RID: 5375 RVA: 0x0005FB84 File Offset: 0x0005DD84
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

		// Token: 0x06001500 RID: 5376 RVA: 0x0005FC0E File Offset: 0x0005DE0E
		private void AddPartyLimitPerkEffects(Clan clan, ref ExplainedNumber result)
		{
			if (clan.Leader != null && clan.Leader.GetPerkValue(DefaultPerks.Leadership.TalentMagnet))
			{
				result.Add(DefaultPerks.Leadership.TalentMagnet.SecondaryBonus, DefaultPerks.Leadership.TalentMagnet.Name, null);
			}
		}

		// Token: 0x06001501 RID: 5377 RVA: 0x0005FC48 File Offset: 0x0005DE48
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

		// Token: 0x06001502 RID: 5378 RVA: 0x0005FCA3 File Offset: 0x0005DEA3
		private int GetCompanionLimitFromTier(int clanTier)
		{
			return clanTier + 3;
		}

		// Token: 0x04000779 RID: 1913
		private static readonly int[] TierLowerRenownLimits = new int[] { 0, 50, 150, 350, 900, 2350, 6150 };

		// Token: 0x0400077A RID: 1914
		private readonly TextObject _partyLimitBonusText = GameTexts.FindText("str_clan_tier_party_limit_bonus", null);

		// Token: 0x0400077B RID: 1915
		private readonly TextObject _companionLimitBonusText = GameTexts.FindText("str_clan_tier_companion_limit_bonus", null);

		// Token: 0x0400077C RID: 1916
		private readonly TextObject _mercenaryEligibleText = GameTexts.FindText("str_clan_tier_mercenary_eligible", null);

		// Token: 0x0400077D RID: 1917
		private readonly TextObject _vassalEligibleText = GameTexts.FindText("str_clan_tier_vassal_eligible", null);

		// Token: 0x0400077E RID: 1918
		private readonly TextObject _additionalCurrentPartySizeBonus = GameTexts.FindText("str_clan_tier_party_size_bonus", null);

		// Token: 0x0400077F RID: 1919
		private readonly TextObject _additionalWorkshopCountBonus = GameTexts.FindText("str_clan_tier_workshop_count_bonus", null);

		// Token: 0x04000780 RID: 1920
		private readonly TextObject _kingdomEligibleText = GameTexts.FindText("str_clan_tier_kingdom_eligible", null);
	}
}
