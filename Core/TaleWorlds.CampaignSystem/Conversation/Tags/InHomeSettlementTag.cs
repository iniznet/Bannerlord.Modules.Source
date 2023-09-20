using System;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.Conversation.Tags
{
	// Token: 0x0200022E RID: 558
	public class InHomeSettlementTag : ConversationTag
	{
		// Token: 0x170007B2 RID: 1970
		// (get) Token: 0x06001E90 RID: 7824 RVA: 0x00087917 File Offset: 0x00085B17
		public override string StringId
		{
			get
			{
				return "InHomeSettlementTag";
			}
		}

		// Token: 0x06001E91 RID: 7825 RVA: 0x00087920 File Offset: 0x00085B20
		public override bool IsApplicableTo(CharacterObject character)
		{
			return (character.IsHero && Settlement.CurrentSettlement != null && character.HeroObject.HomeSettlement == Settlement.CurrentSettlement) || (character.IsHero && Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.OwnerClan.Leader == character.HeroObject);
		}

		// Token: 0x040009B1 RID: 2481
		public const string Id = "InHomeSettlementTag";
	}
}
