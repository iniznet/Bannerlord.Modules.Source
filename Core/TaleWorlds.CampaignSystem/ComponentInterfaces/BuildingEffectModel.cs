using System;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001AE RID: 430
	public abstract class BuildingEffectModel : GameModel
	{
		// Token: 0x06001AC6 RID: 6854
		public abstract float GetBuildingEffectAmount(Building building, BuildingEffectEnum effect);
	}
}
