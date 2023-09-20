using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class ImpulsiveTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "ImpulsiveTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.HeroObject.GetTraitLevel(DefaultTraits.Calculating) < 0;
		}

		public const string Id = "ImpulsiveTag";
	}
}
