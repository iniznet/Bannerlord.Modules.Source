using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class MetBeforeTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "MetBeforeTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return !Campaign.Current.ConversationManager.CurrentConversationIsFirst;
		}

		public const string Id = "MetBeforeTag";
	}
}
