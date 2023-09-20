using System;
using System.Linq;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class PlayerIsKinTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "PlayerIsKinTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && (character.HeroObject.Siblings.Contains(Hero.MainHero) || character.HeroObject.Mother == Hero.MainHero || character.HeroObject.Father == Hero.MainHero || character.HeroObject.Spouse == Hero.MainHero);
		}

		public const string Id = "PlayerIsKinTag";
	}
}
