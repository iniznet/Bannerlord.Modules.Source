using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class RogueSkillsTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "RogueSkillsTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.HeroObject.GetTraitLevel(DefaultTraits.RogueSkills) > 0;
		}

		public const string Id = "RogueSkillsTag";
	}
}
