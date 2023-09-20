using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class CalculatingTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "CalculatingTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.HeroObject.GetTraitLevel(DefaultTraits.Calculating) > 0;
		}

		public const string Id = "CalculatingTag";
	}
}
