using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class VoiceGroupPersonaCurtTribalTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "VoiceGroupPersonaCurtTribalTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.GetPersona() == DefaultTraits.PersonaCurt && ConversationTagHelper.TribalVoiceGroup(character);
		}

		public const string Id = "VoiceGroupPersonaCurtTribalTag";
	}
}
