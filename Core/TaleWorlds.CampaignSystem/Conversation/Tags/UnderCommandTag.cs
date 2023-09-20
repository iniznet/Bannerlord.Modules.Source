using System;
using Helpers;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class UnderCommandTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "UnderCommandTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.HeroObject.Spouse != Hero.MainHero && HeroHelper.UnderPlayerCommand(character.HeroObject);
		}

		public const string Id = "UnderCommandTag";
	}
}
