using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class VoiceGroupPersonaCurtLowerTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "VoiceGroupPersonaCurtLowerTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.GetPersona() == DefaultTraits.PersonaCurt && ConversationTagHelper.UsesLowRegister(character);
		}

		public const string Id = "VoiceGroupPersonaCurtLowerTag";
	}
}
