using System;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class OnTheRoadTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "OnTheRoadTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return Settlement.CurrentSettlement == null;
		}

		public const string Id = "OnTheRoadTag";
	}
}
