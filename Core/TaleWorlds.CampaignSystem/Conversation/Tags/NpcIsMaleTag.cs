using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class NpcIsMaleTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "NpcIsMaleTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return !character.IsFemale;
		}

		public const string Id = "NpcIsMaleTag";
	}
}
