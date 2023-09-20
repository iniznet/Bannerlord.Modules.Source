using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class VoiceGroupPersonaEarnestLowerTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "VoiceGroupPersonaEarnestLowerTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.GetPersona() == DefaultTraits.PersonaEarnest && ConversationTagHelper.UsesLowRegister(character);
		}

		public const string Id = "VoiceGroupPersonaEarnestLowerTag";
	}
}
