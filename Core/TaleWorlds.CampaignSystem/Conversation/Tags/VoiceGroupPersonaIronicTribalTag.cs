using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class VoiceGroupPersonaIronicTribalTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "VoiceGroupPersonaIronicTribalTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.GetPersona() == DefaultTraits.PersonaIronic && ConversationTagHelper.TribalVoiceGroup(character);
		}

		public const string Id = "VoiceGroupPersonaIronicTribalTag";
	}
}
