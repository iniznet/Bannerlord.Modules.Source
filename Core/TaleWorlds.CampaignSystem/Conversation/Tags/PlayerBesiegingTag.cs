using System;
using System.Linq;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	public class PlayerBesiegingTag : ConversationTag
	{
		public override string StringId
		{
			get
			{
				return "PlayerBesiegingTag";
			}
		}

		public override bool IsApplicableTo(CharacterObject character)
		{
			if (Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.SiegeEvent != null)
			{
				return Settlement.CurrentSettlement.SiegeEvent.BesiegerCamp.GetInvolvedPartiesForEventType(MapEvent.BattleTypes.Siege).Any((PartyBase party) => party.MobileParty == Hero.MainHero.PartyBelongedTo);
			}
			return false;
		}

		public const string Id = "PlayerBesiegingTag";
	}
}
