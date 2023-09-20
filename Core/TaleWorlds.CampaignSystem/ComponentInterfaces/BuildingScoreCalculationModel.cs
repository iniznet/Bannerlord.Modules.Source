using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001BF RID: 447
	public abstract class BuildingScoreCalculationModel : GameModel
	{
		// Token: 0x06001B2D RID: 6957
		public abstract Building GetNextBuilding(Town town);
	}
}
