using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class VoiceGroupPersonaIronicLowerTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "VoiceGroupPersonaIronicLowerTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.GetPersona() == DefaultTraits.PersonaIronic && ConversationTagHelper.UsesLowRegister(character);
		}

		public const string Id = "VoiceGroupPersonaIronicLowerTag";
	}
}
