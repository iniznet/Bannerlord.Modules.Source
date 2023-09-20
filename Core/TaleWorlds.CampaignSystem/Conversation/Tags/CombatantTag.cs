using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class CombatantTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "CombatantTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return !character.IsHero || !character.HeroObject.IsNoncombatant;
		}

		public const string Id = "CombatantTag";
	}
}
