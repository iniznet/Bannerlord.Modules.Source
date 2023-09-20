using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class FirstMeetingTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "FirstMeetingTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return Campaign.Current.ConversationManager.CurrentConversationIsFirst;
		}

		public const string Id = "FirstMeetingTag";
	}
}
