using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class PlayerIsFemaleTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "PlayerIsFemaleTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return Hero.MainHero.IsFemale;
		}

		public const string Id = "PlayerIsFemaleTag";
	}
}
