using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class HeadmanNotableTypeTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "HeadmanNotableTypeTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.Occupation == Occupation.Headman;
		}

		public const string Id = "HeadmanNotableTypeTag";
	}
}
