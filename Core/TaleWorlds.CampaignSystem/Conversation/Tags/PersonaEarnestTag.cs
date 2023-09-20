using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class PersonaEarnestTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "PersonaEarnestTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.GetPersona() == DefaultTraits.PersonaEarnest;
		}

		public const string Id = "PersonaEarnestTag";
	}
}
