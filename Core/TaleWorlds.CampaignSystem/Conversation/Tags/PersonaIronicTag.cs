using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class PersonaIronicTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "PersonaIronicTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.GetPersona() == DefaultTraits.PersonaIronic;
		}

		public const string Id = "PersonaIronicTag";
	}
}
