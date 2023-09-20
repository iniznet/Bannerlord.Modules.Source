using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class NonviolentProfessionTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "NonviolentProfessionTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && (character.Occupation == Occupation.Artisan || character.Occupation == Occupation.Merchant || character.Occupation == Occupation.Headman);
		}

		public const string Id = "NonviolentProfessionTag";
	}
}
