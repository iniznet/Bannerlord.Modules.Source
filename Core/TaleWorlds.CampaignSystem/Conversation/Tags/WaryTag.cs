using System;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class WaryTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "WaryTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return character.IsHero && character.HeroObject.MapFaction != Hero.MainHero.MapFaction && (Settlement.CurrentSettlement == null || Settlement.CurrentSettlement.SiegeEvent != null) && (Campaign.Current.ConversationManager.CurrentConversationIsFirst || FactionManager.IsAtWarAgainstFaction(character.HeroObject.MapFaction, Hero.MainHero.MapFaction));
		}

		public const string Id = "WaryTag";
	}
}
