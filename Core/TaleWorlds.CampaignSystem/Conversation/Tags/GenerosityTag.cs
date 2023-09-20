using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class GenerosityTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "GenerosityTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.HeroObject.GetTraitLevel(DefaultTraits.Generosity) > 0;
		}

		public const string Id = "GenerosityTag";
	}
}
