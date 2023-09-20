using System;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001BD RID: 445
	public abstract class SiegeLordsHallFightModel : GameModel
	{
		// Token: 0x17000702 RID: 1794
		// (get) Token: 0x06001B22 RID: 6946
		public abstract float AreaLostRatio { get; }

		// Token: 0x17000703 RID: 1795
		// (get) Token: 0x06001B23 RID: 6947
		public abstract float AttackerDefenderTroopCountRatio { get; }

		// Token: 0x17000704 RID: 1796
		// (get) Token: 0x06001B24 RID: 6948
		public abstract int DefenderTroopNumberForSuccessfulPullBack { get; }

		// Token: 0x17000705 RID: 1797
		// (get) Token: 0x06001B25 RID: 6949
		public abstract float DefenderMaxArcherRatio { get; }

		// Token: 0x17000706 RID: 1798
		// (get) Token: 0x06001B26 RID: 6950
		public abstract int MaxDefenderSideTroopCount { get; }

		// Token: 0x17000707 RID: 1799
		// (get) Token: 0x06001B27 RID: 6951
		public abstract int MaxDefenderArcherCount { get; }

		// Token: 0x17000708 RID: 1800
		// (get) Token: 0x06001B28 RID: 6952
		public abstract int MaxAttackerSideTroopCount { get; }

		// Token: 0x06001B29 RID: 6953
		public abstract FlattenedTroopRoster GetPriorityListForLordsHallFightMission(MapEvent playerMapEvent, BattleSideEnum side, int troopCount);
	}
}
