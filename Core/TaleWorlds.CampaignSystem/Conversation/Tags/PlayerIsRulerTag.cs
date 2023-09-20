using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class PlayerIsRulerTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "PlayerIsRulerTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return Hero.MainHero.Clan.Leader == Hero.MainHero;
		}

		public const string Id = "PlayerIsRulerTag";
	}
}
