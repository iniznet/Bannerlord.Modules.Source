using System;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class InHomeSettlementTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "InHomeSettlementTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return (character.IsHero && Settlement.CurrentSettlement != null && character.HeroObject.HomeSettlement == Settlement.CurrentSettlement) || (character.IsHero && Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.OwnerClan.Leader == character.HeroObject);
		}

		public const string Id = "InHomeSettlementTag";
	}
}
