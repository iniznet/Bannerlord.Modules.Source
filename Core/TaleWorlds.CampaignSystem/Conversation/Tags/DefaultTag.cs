using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class DefaultTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "DefaultTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return true;
		}

		public const string Id = "DefaultTag";
	}
}
