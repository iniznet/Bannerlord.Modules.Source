using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class BattanianTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "BattanianTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.Culture.StringId == "battania";
		}

		public const string Id = "BattanianTag";
	}
}
