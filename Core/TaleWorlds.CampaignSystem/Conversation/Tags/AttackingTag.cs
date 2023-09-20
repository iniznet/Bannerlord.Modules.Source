using System;
using Helpers;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class AttackingTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "AttackingTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			return HeroHelper.WillLordAttack() || (Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.SiegeEvent != null && Settlement.CurrentSettlement.Parties.Contains(Hero.MainHero.PartyBelongedTo));
		}

		public const string Id = "AttackingTag";
	}
}
