using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class OutlawSympathyTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "OutlawSympathyTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.HeroObject.IsWanderer && character.HeroObject.GetTraitLevel(DefaultTraits.RogueSkills) > 0;
		}

		public const string Id = "OutlawSympathyTag";
	}
}
