using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class EmpireTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "EmpireTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.Culture.StringId == "empire";
		}

		public const string Id = "EmpireTag";
	}
}
