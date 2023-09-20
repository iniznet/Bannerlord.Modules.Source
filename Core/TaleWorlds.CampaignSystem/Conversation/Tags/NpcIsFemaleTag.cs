using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class NpcIsFemaleTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "NpcIsFemaleTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsFemale;
		}

		public const string Id = "NpcIsFemaleTag";
	}
}
