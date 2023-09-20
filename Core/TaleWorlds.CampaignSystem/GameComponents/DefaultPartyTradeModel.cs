using System;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x02000127 RID: 295
	public class DefaultPartyTradeModel : PartyTradeModel
	{
		// Token: 0x1700060E RID: 1550
		// (get) Token: 0x060016A2 RID: 5794 RVA: 0x0006E1B9 File Offset: 0x0006C3B9
		public override int CaravanTransactionHighestValueItemCount
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x1700060F RID: 1551
		// (get) Token: 0x060016A3 RID: 5795 RVA: 0x0006E1BC File Offset: 0x0006C3BC
		public override int SmallCaravanFormingCostForPlayer
		{
			get
			{
				if (CharacterObject.PlayerCharacter.Culture.HasFeat(DefaultCulturalFeats.AseraiTraderFeat))
				{
					return MathF.Round(15000f * DefaultCulturalFeats.AseraiTraderFeat.EffectBonus);
				}
				return 15000;
			}
		}

		// Token: 0x17000610 RID: 1552
		// (get) Token: 0x060016A4 RID: 5796 RVA: 0x0006E1EF File Offset: 0x0006C3EF
		public override int LargeCaravanFormingCostForPlayer
		{
			get
			{
				if (CharacterObject.PlayerCharacter.Culture.HasFeat(DefaultCulturalFeats.AseraiTraderFeat))
				{
					return MathF.Round(22500f * DefaultCulturalFeats.AseraiTraderFeat.EffectBonus);
				}
				return 22500;
			}
		}

		// Token: 0x060016A5 RID: 5797 RVA: 0x0006E224 File Offset: 0x0006C424
		public override float GetTradePenaltyFactor(MobileParty party)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(1f, false, null);
			SkillHelper.AddSkillBonusForParty(DefaultSkills.Trade, DefaultSkillEffects.TradePenaltyReduction, party, ref explainedNumber);
			return 1f / explainedNumber.ResultNumber;
		}
	}
}
