using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class PlayerIsEnemyTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "PlayerIsEnemyTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && FactionManager.IsAtWarAgainstFaction(character.HeroObject.MapFaction, Hero.MainHero.MapFaction);
		}

		public const string Id = "PlayerIsEnemyTag";
	}
}
