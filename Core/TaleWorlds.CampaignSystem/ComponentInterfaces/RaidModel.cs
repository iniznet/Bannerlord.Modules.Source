using System;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	public abstract class RaidModel : GameModel
	{
		public abstract MBReadOnlyList<ValueTuple<ItemObject, float>> GetCommonLootItemScores();

		public abstract int GoldRewardForEachLostHearth { get; }

		public abstract float CalculateHitDamage(MapEventSide attackerSide, float settlementHitPoints);
	}
}
