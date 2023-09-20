using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class PersonaSoftspokenTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "PersonaSoftspokenTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.GetPersona() == DefaultTraits.PersonaSoftspoken;
		}

		public const string Id = "PersonaSoftspokenTag";
	}
}
