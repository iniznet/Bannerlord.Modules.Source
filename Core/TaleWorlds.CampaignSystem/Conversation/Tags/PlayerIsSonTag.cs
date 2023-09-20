using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class PlayerIsSonTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "PlayerIsSonTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && !Hero.MainHero.IsFemale && (Hero.MainHero.Father == character.HeroObject || Hero.MainHero.Mother == character.HeroObject);
		}

		public const string Id = "PlayerIsSonTag";
	}
}
