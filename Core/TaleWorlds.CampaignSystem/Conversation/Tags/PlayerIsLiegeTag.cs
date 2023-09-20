using System;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class PlayerIsLiegeTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "PlayerIsLiegeTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.HeroObject.MapFaction.IsKingdomFaction && character.HeroObject.MapFaction == Hero.MainHero.MapFaction && Hero.MainHero.MapFaction.Leader == Hero.MainHero;
		}

		public const string Id = "PlayerIsLiegeTag";
	}
}
