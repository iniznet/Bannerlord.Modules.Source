using System;
using System.Linq;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000224 RID: 548
	public class PlayerBesiegingTag : ConversationTag
	{
		// Token: 0x170007A8 RID: 1960
		// (get) Token: 0x06001E72 RID: 7794 RVA: 0x0008771E File Offset: 0x0008591E
		public override string StringId
		{
			get
			{
				return "PlayerBesiegingTag";
			}
		}

		// Token: 0x06001E73 RID: 7795 RVA: 0x00087728 File Offset: 0x00085928
		public override bool IsApplicableTo(CharacterObject character)
		{
			if (Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.SiegeEvent != null)
			{
				return Settlement.CurrentSettlement.SiegeEvent.BesiegerCamp.GetInvolvedPartiesForEventType(MapEvent.BattleTypes.Siege).Any((PartyBase party) => party.MobileParty == Hero.MainHero.PartyBelongedTo);
			}
			return false;
		}

		// Token: 0x040009A7 RID: 2471
		public const string Id = "PlayerBesiegingTag";
	}
}
