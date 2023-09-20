using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class PlayerIsFatherTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "PlayerIsFatherTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.HeroObject.Father == Hero.MainHero;
		}

		public const string Id = "PlayerIsFatherTag";
	}
}
