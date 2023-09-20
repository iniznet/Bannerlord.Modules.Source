using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class VlandianTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "VlandianTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.Culture.StringId == "vlandia";
		}

		public const string Id = "VlandianTag";
	}
}
