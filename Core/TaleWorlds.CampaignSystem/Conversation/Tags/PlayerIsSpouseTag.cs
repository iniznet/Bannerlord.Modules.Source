using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class PlayerIsSpouseTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "PlayerIsSpouseTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && Hero.MainHero.Spouse == character.HeroObject;
		}

		public const string Id = "PlayerIsSpouseTag";
	}
}
