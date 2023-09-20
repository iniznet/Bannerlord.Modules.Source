using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class VoiceGroupPersonaSoftspokenLowerTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "VoiceGroupPersonaSoftspokenLowerTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.GetPersona() == DefaultTraits.PersonaSoftspoken && ConversationTagHelper.UsesLowRegister(character);
		}

		public const string Id = "VoiceGroupPersonaSoftspokenLowerTag";
	}
}
