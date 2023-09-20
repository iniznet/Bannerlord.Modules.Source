using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200037A RID: 890
	public interface IOrderableWithInteractionArea : IOrderable
	{
		// Token: 0x0600305F RID: 12383
		bool IsPointInsideInteractionArea(Vec3 point);
	}
}
