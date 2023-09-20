using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class PersonaCurtTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "PersonaCurtTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.GetPersona() == DefaultTraits.PersonaCurt;
		}

		public const string Id = "PersonaCurtTag";
	}
}
