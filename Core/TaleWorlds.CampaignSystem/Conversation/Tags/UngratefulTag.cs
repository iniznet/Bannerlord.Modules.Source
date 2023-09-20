using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class UngratefulTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "UngratefulTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.HeroObject.GetTraitLevel(DefaultTraits.Generosity) < 0;
		}

		public const string Id = "UngratefulTag";
	}
}
