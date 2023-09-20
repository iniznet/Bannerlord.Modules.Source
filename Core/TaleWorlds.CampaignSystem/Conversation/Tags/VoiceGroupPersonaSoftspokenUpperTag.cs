using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class VoiceGroupPersonaSoftspokenUpperTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "VoiceGroupPersonaSoftspokenUpperTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.GetPersona() == DefaultTraits.PersonaSoftspoken && ConversationTagHelper.UsesHighRegister(character);
		}

		public const string Id = "VoiceGroupPersonaSoftspokenUpperTag";
	}
}
