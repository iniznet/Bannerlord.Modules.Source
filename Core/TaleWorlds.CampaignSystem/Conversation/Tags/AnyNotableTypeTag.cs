using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class AnyNotableTypeTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "AnyNotableTypeTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.HeroObject.IsNotable;
		}

		public const string Id = "AnyNotableTypeTag";
	}
}
