using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class CautiousTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "CautiousTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.HeroObject.GetTraitLevel(DefaultTraits.Valor) < 0;
		}

		public const string Id = "CautiousTag";
	}
}
