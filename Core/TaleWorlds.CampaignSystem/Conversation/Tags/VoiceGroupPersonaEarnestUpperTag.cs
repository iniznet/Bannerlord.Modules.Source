using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class VoiceGroupPersonaEarnestUpperTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "VoiceGroupPersonaEarnestUpperTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.GetPersona() == DefaultTraits.PersonaEarnest && ConversationTagHelper.UsesHighRegister(character);
		}

		public const string Id = "VoiceGroupPersonaEarnestUpperTag";
	}
}
