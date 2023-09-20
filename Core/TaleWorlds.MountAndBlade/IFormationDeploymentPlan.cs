using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001FD RID: 509
	public interface IFormationDeploymentPlan
	{
		// Token: 0x170005A3 RID: 1443
		// (get) Token: 0x06001C43 RID: 7235
		FormationClass Class { get; }

		// Token: 0x170005A4 RID: 1444
		// (get) Token: 0x06001C44 RID: 7236
		FormationClass SpawnClass { get; }

		// Token: 0x170005A5 RID: 1445
		// (get) Token: 0x06001C45 RID: 7237
		float PlannedWidth { get; }

		// Token: 0x170005A6 RID: 1446
		// (get) Token: 0x06001C46 RID: 7238
		float PlannedDepth { get; }

		// Token: 0x170005A7 RID: 1447
		// (get) Token: 0x06001C47 RID: 7239
		int PlannedTroopCount { get; }

		// Token: 0x170005A8 RID: 1448
		// (get) Token: 0x06001C48 RID: 7240
		int PlannedFootTroopCount { get; }

		// Token: 0x170005A9 RID: 1449
		// (get) Token: 0x06001C49 RID: 7241
		int PlannedMountedTroopCount { get; }

		// Token: 0x170005AA RID: 1450
		// (get) Token: 0x06001C4A RID: 7242
		bool HasDimensions { get; }

		// Token: 0x170005AB RID: 1451
		// (get) Token: 0x06001C4B RID: 7243
		bool HasSignificantMountedTroops { get; }

		// Token: 0x06001C4C RID: 7244
		bool HasFrame();

		// Token: 0x06001C4D RID: 7245
		FormationDeploymentFlank GetDefaultFlank(bool spawnWithHorses, int formationTroopCount, int infantryCount);

		// Token: 0x06001C4E RID: 7246
		FormationDeploymentOrder GetFlankDeploymentOrder(int offset = 0);

		// Token: 0x06001C4F RID: 7247
		MatrixFrame GetGroundFrame();

		// Token: 0x06001C50 RID: 7248
		Vec3 GetGroundPosition();

		// Token: 0x06001C51 RID: 7249
		Vec2 GetDirection();

		// Token: 0x06001C52 RID: 7250
		WorldPosition CreateNewDeploymentWorldPosition(WorldPosition.WorldPositionEnforcedCache worldPositionEnforcedCache);
	}
}
