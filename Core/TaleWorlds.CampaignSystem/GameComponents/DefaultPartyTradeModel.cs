using System;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultPartyTradeModel : PartyTradeModel
	{
		public override int CaravanTransactionHighestValueItemCount
		{
			get
			{
				return 3;
			}
		}

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

		public override float GetTradePenaltyFactor(MobileParty party)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(1f, false, null);
			SkillHelper.AddSkillBonusForParty(DefaultSkills.Trade, DefaultSkillEffects.TradePenaltyReduction, party, ref explainedNumber);
			return 1f / explainedNumber.ResultNumber;
		}
	}
}
