using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200037B RID: 891
	public interface ITargetable
	{
		// Token: 0x06003060 RID: 12384
		TargetFlags GetTargetFlags();

		// Token: 0x06003061 RID: 12385
		float GetTargetValue(List<Vec3> referencePositions);

		// Token: 0x06003062 RID: 12386
		GameEntity GetTargetEntity();

		// Token: 0x06003063 RID: 12387
		Vec3 GetTargetingOffset();

		// Token: 0x06003064 RID: 12388
		BattleSideEnum GetSide();

		// Token: 0x06003065 RID: 12389
		GameEntity Entity();
	}
}
