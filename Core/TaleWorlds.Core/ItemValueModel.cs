using System;

namespace TaleWorlds.Core
{
	// Token: 0x0200006E RID: 110
	public abstract class ItemValueModel : GameModel
	{
		// Token: 0x06000708 RID: 1800
		public abstract float GetEquipmentValueFromTier(float itemTierf);

		// Token: 0x06000709 RID: 1801
		public abstract float CalculateTier(ItemObject item);

		// Token: 0x0600070A RID: 1802
		public abstract int CalculateValue(ItemObject item);
	}
}
