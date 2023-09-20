using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class PlayerIsMotherTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "PlayerIsMotherTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.HeroObject.Mother == Hero.MainHero;
		}

		public const string Id = "PlayerIsMotherTag";
	}
}
