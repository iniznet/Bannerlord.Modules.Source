using System;

namespace TaleWorlds.Core
{
	// Token: 0x0200006C RID: 108
	public abstract class RidingModel : GameModel
	{
		// Token: 0x06000704 RID: 1796
		public abstract float CalculateAcceleration(in EquipmentElement mountElement, in EquipmentElement harnessElement, int ridingSkill);
	}
}
