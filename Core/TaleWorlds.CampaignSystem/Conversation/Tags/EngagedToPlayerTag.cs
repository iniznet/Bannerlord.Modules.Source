using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class EngagedToPlayerTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "EngagedToPlayerTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && Romance.GetRomanticLevel(character.HeroObject, Hero.MainHero) == Romance.RomanceLevelEnum.CoupleAgreedOnMarriage;
		}

		public const string Id = "EngagedToPlayerTag";
	}
}
