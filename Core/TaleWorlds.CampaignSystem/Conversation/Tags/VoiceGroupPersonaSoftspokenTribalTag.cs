using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class VoiceGroupPersonaSoftspokenTribalTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "VoiceGroupPersonaSoftspokenTribalTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.GetPersona() == DefaultTraits.PersonaSoftspoken && ConversationTagHelper.TribalVoiceGroup(character);
		}

		public const string Id = "VoiceGroupPersonaSoftspokenTribalTag";
	}
}
