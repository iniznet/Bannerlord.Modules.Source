using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001C6 RID: 454
	public abstract class PrisonBreakModel : GameModel
	{
		// Token: 0x06001B62 RID: 7010
		public abstract bool CanPlayerStagePrisonBreak(Settlement settlement);

		// Token: 0x06001B63 RID: 7011
		public abstract int GetPrisonBreakStartCost(Hero prisonerHero);

		// Token: 0x06001B64 RID: 7012
		public abstract int GetRelationRewardOnPrisonBreak(Hero prisonerHero);

		// Token: 0x06001B65 RID: 7013
		public abstract float GetRogueryRewardOnPrisonBreak(Hero prisonerHero, bool isSuccess);
	}
}
