using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001FE RID: 510
	public interface IMissionDeploymentPlan
	{
		// Token: 0x06001C53 RID: 7251
		bool IsPlanMadeForBattleSide(BattleSideEnum side, DeploymentPlanType planType);

		// Token: 0x06001C54 RID: 7252
		bool IsPositionInsideDeploymentBoundaries(BattleSideEnum battleSide, in Vec2 position, DeploymentPlanType planType);

		// Token: 0x06001C55 RID: 7253
		bool HasDeploymentBoundaries(BattleSideEnum side, DeploymentPlanType planType);

		// Token: 0x06001C56 RID: 7254
		MBReadOnlyDictionary<string, List<Vec2>> GetDeploymentBoundaries(BattleSideEnum side, DeploymentPlanType planType);

		// Token: 0x06001C57 RID: 7255
		Vec2 GetClosestDeploymentBoundaryPosition(BattleSideEnum battleSide, in Vec2 position, DeploymentPlanType planType);

		// Token: 0x06001C58 RID: 7256
		int GetTroopCountForSide(BattleSideEnum side, DeploymentPlanType planType);

		// Token: 0x06001C59 RID: 7257
		MatrixFrame GetBattleSideDeploymentFrame(BattleSideEnum side, DeploymentPlanType planType);

		// Token: 0x06001C5A RID: 7258
		IFormationDeploymentPlan GetFormationPlan(BattleSideEnum side, FormationClass fClass, DeploymentPlanType planType);
	}
}
