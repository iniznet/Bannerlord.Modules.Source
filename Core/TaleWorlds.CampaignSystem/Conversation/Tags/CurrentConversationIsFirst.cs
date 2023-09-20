using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class CurrentConversationIsFirst : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "CurrentConversationIsFirst";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return Campaign.Current.ConversationManager.CurrentConversationIsFirst;
		}

		public const string Id = "CurrentConversationIsFirst";
	}
}
