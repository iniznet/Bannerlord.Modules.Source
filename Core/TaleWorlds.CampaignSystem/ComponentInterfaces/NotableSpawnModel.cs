using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x0200018E RID: 398
	public abstract class NotableSpawnModel : GameModel
	{
		// Token: 0x060019DC RID: 6620
		public abstract int GetTargetNotableCountForSettlement(Settlement settlement, Occupation occupation);
	}
}
