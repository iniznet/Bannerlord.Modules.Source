using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Objects.Siege
{
	public class SpawnerBase : ScriptComponentBehavior
	{
		protected internal override void OnEditorTick(float dt)
		{
			if (base.GameEntity.IsSelectedOnEditor())
			{
				this.UpdateTrejectoryMeshHolderFrame();
			}
		}

		protected internal override bool OnCheckForProblems()
		{
			return !this._spawnerEditorHelper.IsValid;
		}

		public virtual void AssignParameters(SpawnerEntityMissionHelper _spawnerMissionHelper)
		{
			Debug.FailedAssert("Please override 'AssignParameters' function in the derived class.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Objects\\Siege\\SpawnerBase.cs", "AssignParameters", 46);
		}

		protected override void OnRemoved(int removeReason)
		{
			if (this.TrajectoryMeshHolder != null)
			{
				this.TrajectoryMeshHolder.Remove(removeReason);
			}
		}

		public virtual bool GetFireVersion()
		{
			return false;
		}

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

		public const uint MaxRangeColor = 4294901760U;

		public const uint MinRangeColor = 4294902015U;

		[EditorVisibleScriptComponentVariable(true)]
		public string ToBeSpawnedOverrideName = "";

		[EditorVisibleScriptComponentVariable(true)]
		public string ToBeSpawnedOverrideNameForFireVersion = "";

		protected SpawnerEntityEditorHelper _spawnerEditorHelper;

		protected SpawnerEntityMissionHelper _spawnerMissionHelper;

		protected SpawnerEntityMissionHelper _spawnerMissionHelperFire;

		protected GameEntity TrajectoryMeshHolder;

		private Vec3 _projectileStartingPositionOffset;
	}
}
