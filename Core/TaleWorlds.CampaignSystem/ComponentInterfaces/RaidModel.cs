using System;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x02000179 RID: 377
	public abstract class RaidModel : GameModel
	{
		// Token: 0x06001925 RID: 6437
		public abstract MBReadOnlyList<ValueTuple<ItemObject, float>> GetCommonLootItemScores();

		// Token: 0x17000686 RID: 1670
		// (get) Token: 0x06001926 RID: 6438
		public abstract int GoldRewardForEachLostHearth { get; }

		// Token: 0x06001927 RID: 6439
		public abstract float CalculateHitDamage(MapEventSide attackerSide, float settlementHitPoints);
	}
}
