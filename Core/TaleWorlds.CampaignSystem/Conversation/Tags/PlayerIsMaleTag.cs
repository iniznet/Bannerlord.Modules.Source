using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class PlayerIsMaleTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "PlayerIsMaleTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return !Hero.MainHero.IsFemale;
		}

		public const string Id = "PlayerIsMaleTag";
	}
}
