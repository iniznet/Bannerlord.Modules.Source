using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class TrajectoryVisualizer : ScriptComponentBehavior
	{
		public void SetTrajectoryParams(Vec3 missileShootingPositionOffset, float missileSpeed, float verticalAngleMinInDegrees, float verticalAngleMaxInDegrees, float horizontalAngleRangeInDegrees, float airFrictionConstant)
		{
			this._trajectoryParams.MissileShootingPositionOffset = missileShootingPositionOffset;
			this._trajectoryParams.MissileSpeed = missileSpeed;
			this._trajectoryParams.VerticalAngleMinInDegrees = verticalAngleMinInDegrees;
			this._trajectoryParams.VerticalAngleMaxInDegrees = verticalAngleMaxInDegrees;
			this._trajectoryParams.HorizontalAngleRangeInDegrees = horizontalAngleRangeInDegrees;
			this._trajectoryParams.AirFrictionConstant = airFrictionConstant;
			this._trajectoryParams.IsValid = true;
		}

		protected internal override void OnEditorInit()
		{
			base.OnEditorInit();
		}

		protected internal override void OnEditorVariableChanged(string variableName)
		{
			if (variableName == "ShowTrajectory")
			{
				if (this.ShowTrajectory && this._trajectoryMeshHolder == null && !base.GameEntity.IsGhostObject() && this._trajectoryParams.IsValid)
				{
					this._trajectoryMeshHolder = GameEntity.CreateEmpty(base.Scene, false);
					if (this._trajectoryMeshHolder != null)
					{
						this._trajectoryMeshHolder.EntityFlags |= EntityFlags.DontSaveToScene;
						MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
						Vec3 vec = globalFrame.origin + (globalFrame.rotation.s * this._trajectoryParams.MissileShootingPositionOffset.x + globalFrame.rotation.f * this._trajectoryParams.MissileShootingPositionOffset.y + globalFrame.rotation.u * this._trajectoryParams.MissileShootingPositionOffset.z);
						globalFrame.origin = vec;
						this._trajectoryMeshHolder.SetGlobalFrame(globalFrame);
						this._trajectoryMeshHolder.ComputeTrajectoryVolume(this._trajectoryParams.MissileSpeed, this._trajectoryParams.VerticalAngleMaxInDegrees, this._trajectoryParams.VerticalAngleMinInDegrees, this._trajectoryParams.HorizontalAngleRangeInDegrees, this._trajectoryParams.AirFrictionConstant);
						base.GameEntity.AddChild(this._trajectoryMeshHolder, true);
						this._trajectoryMeshHolder.SetVisibilityExcludeParents(false);
					}
				}
				if (this._trajectoryMeshHolder != null)
				{
					this._trajectoryMeshHolder.SetVisibilityExcludeParents(this.ShowTrajectory);
				}
			}
		}

		protected override void OnRemoved(int removeReason)
		{
			if (this._trajectoryMeshHolder != null)
			{
				this._trajectoryMeshHolder.Remove(removeReason);
			}
		}

		public bool ShowTrajectory;

		private GameEntity _trajectoryMeshHolder;

		private TrajectoryVisualizer.TrajectoryParams _trajectoryParams;

		private struct TrajectoryParams
		{
			public Vec3 MissileShootingPositionOffset;

			public float MissileSpeed;

			public float VerticalAngleMinInDegrees;

			public float VerticalAngleMaxInDegrees;

			public float HorizontalAngleRangeInDegrees;

			public float AirFrictionConstant;

			public bool IsValid;
		}
	}
}
