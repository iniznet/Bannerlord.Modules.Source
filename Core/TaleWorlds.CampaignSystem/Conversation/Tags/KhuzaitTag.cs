using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class KhuzaitTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "KhuzaitTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.Culture.StringId == "khuzait";
		}

		public const string Id = "KhuzaitTag";
	}
}
