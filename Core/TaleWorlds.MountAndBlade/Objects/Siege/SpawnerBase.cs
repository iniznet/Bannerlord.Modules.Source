using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Objects.Siege
{
	// Token: 0x020003B2 RID: 946
	public class SpawnerBase : ScriptComponentBehavior
	{
		// Token: 0x060032FD RID: 13053 RVA: 0x000D3289 File Offset: 0x000D1489
		protected internal override void OnEditorTick(float dt)
		{
			if (base.GameEntity.IsSelectedOnEditor())
			{
				this.UpdateTrejectoryMeshHolderFrame();
			}
		}

		// Token: 0x060032FE RID: 13054 RVA: 0x000D329E File Offset: 0x000D149E
		protected internal override bool OnCheckForProblems()
		{
			return !this._spawnerEditorHelper.IsValid;
		}

		// Token: 0x060032FF RID: 13055 RVA: 0x000D32AE File Offset: 0x000D14AE
		public virtual void AssignParameters(SpawnerEntityMissionHelper _spawnerMissionHelper)
		{
			Debug.FailedAssert("Please override 'AssignParameters' function in the derived class.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Objects\\Siege\\SpawnerBase.cs", "AssignParameters", 46);
		}

		// Token: 0x06003300 RID: 13056 RVA: 0x000D32C6 File Offset: 0x000D14C6
		protected override void OnRemoved(int removeReason)
		{
			if (this.TrajectoryMeshHolder != null)
			{
				this.TrajectoryMeshHolder.Remove(removeReason);
			}
		}

		// Token: 0x06003301 RID: 13057 RVA: 0x000D32E2 File Offset: 0x000D14E2
		public virtual bool GetFireVersion()
		{
			return false;
		}

		// Token: 0x06003302 RID: 13058 RVA: 0x000D32E8 File Offset: 0x000D14E8
		protected void CreateTrajectoryVisualizer(Vec3 projectileStartingPositionOffset, float maxShootSpeed, float minShootSpeed, float maxAngle, float minAngle, float turnAngle, float frictionCoefficient)
		{
			this._projectileStartingPositionOffset = projectileStartingPositionOffset;
			if (this.TrajectoryMeshHolder == null)
			{
				this.TrajectoryMeshHolder = GameEntity.CreateEmpty(base.Scene, false);
			}
			this.UpdateTrejectoryMeshHolderFrame();
			this.TrajectoryMeshHolder.UpdateRestrictedTrajectoryBuilder(maxShootSpeed, minShootSpeed, maxAngle * 57.29578f, minAngle * 57.29578f, 57.29578f * turnAngle, frictionCoefficient);
			this.TrajectoryMeshHolder.SetVisibilityExcludeParents(false);
		}

		// Token: 0x06003303 RID: 13059 RVA: 0x000D3358 File Offset: 0x000D1558
		private void UpdateTrejectoryMeshHolderFrame()
		{
			if (this.TrajectoryMeshHolder != null)
			{
				MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
				Vec3 vec = globalFrame.origin + (globalFrame.rotation.s * this._projectileStartingPositionOffset.x + globalFrame.rotation.f * this._projectileStartingPositionOffset.y + globalFrame.rotation.u * this._projectileStartingPositionOffset.z);
				globalFrame.origin = vec;
				this.TrajectoryMeshHolder.SetGlobalFrame(globalFrame);
			}
		}

		// Token: 0x040015B1 RID: 5553
		public const uint MaxRangeColor = 4294901760U;

		// Token: 0x040015B2 RID: 5554
		public const uint MinRangeColor = 4294902015U;

		// Token: 0x040015B3 RID: 5555
		[EditorVisibleScriptComponentVariable(true)]
		public string ToBeSpawnedOverrideName = "";

		// Token: 0x040015B4 RID: 5556
		[EditorVisibleScriptComponentVariable(true)]
		public string ToBeSpawnedOverrideNameForFireVersion = "";

		// Token: 0x040015B5 RID: 5557
		protected SpawnerEntityEditorHelper _spawnerEditorHelper;

		// Token: 0x040015B6 RID: 5558
		protected SpawnerEntityMissionHelper _spawnerMissionHelper;

		// Token: 0x040015B7 RID: 5559
		protected SpawnerEntityMissionHelper _spawnerMissionHelperFire;

		// Token: 0x040015B8 RID: 5560
		protected GameEntity TrajectoryMeshHolder;

		// Token: 0x040015B9 RID: 5561
		private Vec3 _projectileStartingPositionOffset;
	}
}
