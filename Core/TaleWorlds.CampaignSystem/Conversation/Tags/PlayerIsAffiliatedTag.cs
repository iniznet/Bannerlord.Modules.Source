using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class PlayerIsAffiliatedTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "PlayerIsAffiliatedTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return Hero.MainHero.MapFaction.IsKingdomFaction;
		}

		public const string Id = "PlayerIsAffiliatedTag";
	}
}
