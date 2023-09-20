using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class SturgianTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "SturgianTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.Culture.StringId == "sturgia";
		}

		public const string Id = "SturgianTag";
	}
}
