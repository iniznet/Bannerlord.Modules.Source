using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class RomanticallyInvolvedTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "RomanticallyInvolvedTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && Romance.GetRomanticLevel(character.HeroObject, CharacterObject.PlayerCharacter.HeroObject) >= Romance.RomanceLevelEnum.CourtshipStarted;
		}

		public const string Id = "RomanticallyInvolvedTag";
	}
}
