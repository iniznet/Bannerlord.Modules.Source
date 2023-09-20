using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class OldTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "OldTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.Age > 49f;
		}

		public const string Id = "OldTag";
	}
}
