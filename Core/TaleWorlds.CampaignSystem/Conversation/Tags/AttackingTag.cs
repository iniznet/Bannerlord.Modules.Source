using System;
using Helpers;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x02000222 RID: 546
	public class AttackingTag : ConversationTag
	{
		// Token: 0x170007A6 RID: 1958
		// (get) Token: 0x06001E6C RID: 7788 RVA: 0x000876BD File Offset: 0x000858BD
		public override string StringId
		{
			get
			{
				return "AttackingTag";
			}
		}

		// Token: 0x06001E6D RID: 7789 RVA: 0x000876C4 File Offset: 0x000858C4
		public override bool IsApplicableTo(CharacterObject character)
		{
			return HeroHelper.WillLordAttack() || (Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.SiegeEvent != null && Settlement.CurrentSettlement.Parties.Contains(Hero.MainHero.PartyBelongedTo));
		}

		// Token: 0x040009A5 RID: 2469
		public const string Id = "AttackingTag";
	}
}
