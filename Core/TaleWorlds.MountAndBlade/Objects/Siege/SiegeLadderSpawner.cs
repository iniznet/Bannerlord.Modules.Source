using System;
using System.Collections.Generic;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Objects.Siege
{
	public class SiegeLadderSpawner : SpawnerBase
	{
		public float UpperStateRotationRadian
		{
			get
			{
				return this.UpperStateRotationDegree * 0.017453292f;
			}
		}

		public float DownStateRotationRadian
		{
			get
			{
				return this.DownStateRotationDegree * 0.017453292f;
			}
		}

		protected internal override void OnEditorInit()
		{
			base.OnEditorInit();
			this._spawnerEditorHelper = new SpawnerEntityEditorHelper(this);
			if (this._spawnerEditorHelper.IsValid)
			{
				this._spawnerEditorHelper.GivePermission("ladder_up_state", new SpawnerEntityEditorHelper.Permission(SpawnerEntityEditorHelper.PermissionType.rotation, SpawnerEntityEditorHelper.Axis.x), new Action<float>(this.OnLadderUpStateChange));
				this._spawnerEditorHelper.GivePermission("ladder_down_state", new SpawnerEntityEditorHelper.Permission(SpawnerEntityEditorHelper.PermissionType.rotation, SpawnerEntityEditorHelper.Axis.x), new Action<float>(this.OnLadderDownStateChange));
			}
			this.OnEditorVariableChanged("UpperStateRotationDegree");
			this.OnEditorVariableChanged("DownStateRotationDegree");
		}

		protected internal override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			this._spawnerEditorHelper.Tick(dt);
		}

		private void OnLadderUpStateChange(float rotation)
		{
			if (rotation > -0.20135832f)
			{
				rotation = -0.20135832f;
				this.UpperStateRotationDegree = rotation * 57.29578f;
				this.OnEditorVariableChanged("UpperStateRotationDegree");
				return;
			}
			this.UpperStateRotationDegree = rotation * 57.29578f;
		}

		private void OnLadderDownStateChange(float unusedArgument)
		{
			GameEntity ghostEntityOrChild = this._spawnerEditorHelper.GetGhostEntityOrChild("ladder_down_state");
			this.DownStateRotationDegree = Vec3.AngleBetweenTwoVectors(Vec3.Up, ghostEntityOrChild.GetFrame().rotation.u) * 57.29578f;
		}

		protected internal override void OnEditorVariableChanged(string variableName)
		{
			base.OnEditorVariableChanged(variableName);
			if (variableName == "UpperStateRotationDegree")
			{
				if (this.UpperStateRotationDegree > -11.536982f)
				{
					this.UpperStateRotationDegree = -11.536982f;
				}
				MatrixFrame frame = this._spawnerEditorHelper.GetGhostEntityOrChild("ladder_up_state").GetFrame();
				frame.rotation = Mat3.Identity;
				frame.rotation.RotateAboutSide(this.UpperStateRotationRadian);
				this._spawnerEditorHelper.ChangeStableChildMatrixFrameAndApply("ladder_up_state", frame, true);
				return;
			}
			if (variableName == "DownStateRotationDegree")
			{
				MatrixFrame frame2 = this._spawnerEditorHelper.GetGhostEntityOrChild("ladder_down_state").GetFrame();
				frame2.rotation = Mat3.Identity;
				frame2.rotation.RotateAboutUp(1.5707964f);
				frame2.rotation.RotateAboutSide(this.DownStateRotationRadian);
				this._spawnerEditorHelper.ChangeStableChildMatrixFrameAndApply("ladder_down_state", frame2, true);
			}
		}

		protected internal override bool OnCheckForProblems()
		{
			bool flag = base.OnCheckForProblems();
			if (base.Scene.IsMultiplayerScene())
			{
				if (this.OnWallNavMeshId == 0 || this.OnWallNavMeshId % 10 == 1)
				{
					MBEditor.AddEntityWarning(base.GameEntity, "OnWallNavMeshId's ones digit cannot be 1 and OnWallNavMeshId cannot be 0 in a multiplayer scene.");
					flag = true;
				}
			}
			else if (this.OnWallNavMeshId == -1 || this.OnWallNavMeshId == 0 || this.OnWallNavMeshId % 10 == 1)
			{
				MBEditor.AddEntityWarning(base.GameEntity, "OnWallNavMeshId's ones digit cannot be 1 and OnWallNavMeshId cannot be -1 or 0 in a singleplayer scene.");
				flag = true;
			}
			if (this.OnWallNavMeshId != -1)
			{
				List<GameEntity> list = new List<GameEntity>();
				base.Scene.GetEntities(ref list);
				foreach (GameEntity gameEntity in list)
				{
					SiegeLadderSpawner firstScriptOfType = gameEntity.GetFirstScriptOfType<SiegeLadderSpawner>();
					if (firstScriptOfType != null && gameEntity != base.GameEntity && this.OnWallNavMeshId == firstScriptOfType.OnWallNavMeshId && base.GameEntity.GetVisibilityLevelMaskIncludingParents() == gameEntity.GetVisibilityLevelMaskIncludingParents())
					{
						MBEditor.AddEntityWarning(base.GameEntity, "OnWallNavMeshId must not be shared with any other siege ladder.");
					}
				}
			}
			return flag;
		}

		protected internal override void OnPreInit()
		{
			base.OnPreInit();
			this._spawnerMissionHelper = new SpawnerEntityMissionHelper(this, false);
		}

		public override void AssignParameters(SpawnerEntityMissionHelper _spawnerMissionHelper)
		{
			SiegeLadder firstScriptOfType = _spawnerMissionHelper.SpawnedEntity.GetFirstScriptOfType<SiegeLadder>();
			firstScriptOfType.AddOnDeployTag = this.AddOnDeployTag;
			firstScriptOfType.RemoveOnDeployTag = this.RemoveOnDeployTag;
			firstScriptOfType.AssignParametersFromSpawner(this.SideTag, this.TargetWallSegmentTag, this.OnWallNavMeshId, this.DownStateRotationRadian, this.UpperStateRotationRadian, this.BarrierTagToRemove, this.IndestructibleMerlonsTag);
			List<GameEntity> list = new List<GameEntity>();
			_spawnerMissionHelper.SpawnedEntity.GetChildrenRecursive(ref list);
			list.Find((GameEntity x) => x.Name == "initial_wait_pos").GetFirstScriptOfType<TacticalPosition>().SetWidth(this.TacticalPositionWidth);
		}

		[SpawnerBase.SpawnerPermissionField]
		public MatrixFrame fork_holder = MatrixFrame.Zero;

		[SpawnerBase.SpawnerPermissionField]
		public MatrixFrame initial_wait_pos = MatrixFrame.Zero;

		[SpawnerBase.SpawnerPermissionField]
		public MatrixFrame use_push = MatrixFrame.Zero;

		[SpawnerBase.SpawnerPermissionField]
		public MatrixFrame stand_position_wall_push = MatrixFrame.Zero;

		[SpawnerBase.SpawnerPermissionField]
		public MatrixFrame distance_holder = MatrixFrame.Zero;

		[SpawnerBase.SpawnerPermissionField]
		public MatrixFrame stand_position_ground_wait = MatrixFrame.Zero;

		[EditorVisibleScriptComponentVariable(true)]
		public string SideTag;

		[EditorVisibleScriptComponentVariable(true)]
		public string TargetWallSegmentTag = "";

		[EditorVisibleScriptComponentVariable(true)]
		public int OnWallNavMeshId = -1;

		[EditorVisibleScriptComponentVariable(true)]
		public string AddOnDeployTag = "";

		[EditorVisibleScriptComponentVariable(true)]
		public string RemoveOnDeployTag = "";

		[EditorVisibleScriptComponentVariable(true)]
		public float UpperStateRotationDegree;

		[EditorVisibleScriptComponentVariable(true)]
		public float DownStateRotationDegree = 90f;

		public float TacticalPositionWidth = 1f;

		[EditorVisibleScriptComponentVariable(true)]
		public string BarrierTagToRemove = string.Empty;

		[EditorVisibleScriptComponentVariable(true)]
		public string IndestructibleMerlonsTag = string.Empty;
	}
}
