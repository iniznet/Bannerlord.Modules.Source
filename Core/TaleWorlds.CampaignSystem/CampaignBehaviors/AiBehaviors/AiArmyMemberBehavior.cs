using System;
using TaleWorlds.CampaignSystem.Party;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors.AiBehaviors
{
	// Token: 0x020003FE RID: 1022
	public class AiArmyMemberBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003D1D RID: 15645 RVA: 0x00122C50 File Offset: 0x00120E50
		public override void RegisterEvents()
		{
			CampaignEvents.AiHourlyTickEvent.AddNonSerializedListener(this, new Action<MobileParty, PartyThinkParams>(this.AiHourlyTick));
		}

		// Token: 0x06003D1E RID: 15646 RVA: 0x00122C69 File Offset: 0x00120E69
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06003D1F RID: 15647 RVA: 0x00122C6C File Offset: 0x00120E6C
		public void AiHourlyTick(MobileParty mobileParty, PartyThinkParams p)
		{
			if (mobileParty.Army == null || mobileParty.Army.LeaderParty == mobileParty)
			{
				return;
			}
			AIBehaviorTuple aibehaviorTuple = new AIBehaviorTuple(mobileParty.Army.LeaderParty, AiBehavior.EscortParty, false);
			ValueTuple<AIBehaviorTuple, float> valueTuple = new ValueTuple<AIBehaviorTuple, float>(aibehaviorTuple, 0.25f);
			p.AddBehaviorScore(valueTuple);
		}

		// Token: 0x0400125F RID: 4703
		private const float FollowingArmyLeaderDefaultScore = 0.25f;
	}
}
