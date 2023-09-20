using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class AseraiTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "AseraiTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.Culture.StringId == "aserai";
		}

		public const string Id = "AseraiTag";
	}
}
