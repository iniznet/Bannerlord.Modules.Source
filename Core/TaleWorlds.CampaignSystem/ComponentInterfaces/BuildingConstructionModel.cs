using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001AD RID: 429
	public abstract class BuildingConstructionModel : GameModel
	{
		// Token: 0x170006EF RID: 1775
		// (get) Token: 0x06001ABD RID: 6845
		public abstract int TownBoostCost { get; }

		// Token: 0x170006F0 RID: 1776
		// (get) Token: 0x06001ABE RID: 6846
		public abstract int TownBoostBonus { get; }

		// Token: 0x170006F1 RID: 1777
		// (get) Token: 0x06001ABF RID: 6847
		public abstract int CastleBoostCost { get; }

		// Token: 0x170006F2 RID: 1778
		// (get) Token: 0x06001AC0 RID: 6848
		public abstract int CastleBoostBonus { get; }

		// Token: 0x06001AC1 RID: 6849
		public abstract ExplainedNumber CalculateDailyConstructionPower(Town town, bool includeDescriptions = false);

		// Token: 0x06001AC2 RID: 6850
		public abstract int CalculateDailyConstructionPowerWithoutBoost(Town town);

		// Token: 0x06001AC3 RID: 6851
		public abstract int GetBoostCost(Town town);

		// Token: 0x06001AC4 RID: 6852
		public abstract int GetBoostAmount(Town town);
	}
}
