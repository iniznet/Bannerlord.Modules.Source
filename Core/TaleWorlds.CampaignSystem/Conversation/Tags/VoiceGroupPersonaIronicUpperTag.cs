using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class VoiceGroupPersonaIronicUpperTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "VoiceGroupPersonaIronicUpperTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.GetPersona() == DefaultTraits.PersonaIronic && ConversationTagHelper.UsesHighRegister(character);
		}

		public const string Id = "VoiceGroupPersonaIronicUpperTag";
	}
}
