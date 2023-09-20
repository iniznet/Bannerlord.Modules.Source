using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class ArtisanNotableTypeTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "ArtisanNotableTypeTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.Occupation == Occupation.Artisan;
		}

		public const string Id = "ArtisanNotableTypeTag";
	}
}
