using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class VoiceGroupPersonaCurtUpperTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "VoiceGroupPersonaCurtUpperTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.GetPersona() == DefaultTraits.PersonaCurt && ConversationTagHelper.UsesHighRegister(character);
		}

		public const string Id = "VoiceGroupPersonaCurtUpperTag";
	}
}
