using System;
using System.Linq;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class PlayerIsSisterTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "PlayerIsSisterTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return Hero.MainHero.IsFemale && character.IsHero && character.HeroObject.Siblings.Contains(Hero.MainHero);
		}

		public const string Id = "PlayerIsSisterTag";
	}
}
