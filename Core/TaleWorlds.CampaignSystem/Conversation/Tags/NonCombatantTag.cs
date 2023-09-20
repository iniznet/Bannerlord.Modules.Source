using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class NonCombatantTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "NonCombatantTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.HeroObject.IsNoncombatant;
		}

		public const string Id = "NonCombatantTag";
	}
}
