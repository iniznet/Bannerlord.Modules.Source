using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001AF RID: 431
	public abstract class WallHitPointCalculationModel : GameModel
	{
		// Token: 0x06001AC8 RID: 6856
		public abstract float CalculateMaximumWallHitPoint(Town town);
	}
}
