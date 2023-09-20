using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class VoiceGroupPersonaEarnestTribalTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "VoiceGroupPersonaEarnestTribalTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.GetPersona() == DefaultTraits.PersonaEarnest && ConversationTagHelper.TribalVoiceGroup(character);
		}

		public const string Id = "VoiceGroupPersonaEarnestTribalTag";
	}
}
