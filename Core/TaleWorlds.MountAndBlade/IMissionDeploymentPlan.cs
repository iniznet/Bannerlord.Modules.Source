using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public interface IMissionDeploymentPlan
	{
		bool IsPlanMadeForBattleSide(BattleSideEnum side, DeploymentPlanType planType);

		bool IsPositionInsideDeploymentBoundaries(BattleSideEnum battleSide, in Vec2 position, DeploymentPlanType planType);

		bool HasDeploymentBoundaries(BattleSideEnum side, DeploymentPlanType planType);

		MBReadOnlyDictionary<string, List<Vec2>> GetDeploymentBoundaries(BattleSideEnum side, DeploymentPlanType planType);

		Vec2 GetClosestDeploymentBoundaryPosition(BattleSideEnum battleSide, in Vec2 position, DeploymentPlanType planType);

		int GetTroopCountForSide(BattleSideEnum side, DeploymentPlanType planType);

		MatrixFrame GetBattleSideDeploymentFrame(BattleSideEnum side, DeploymentPlanType planType);

		IFormationDeploymentPlan GetFormationPlan(BattleSideEnum side, FormationClass fClass, DeploymentPlanType planType);
	}
}
