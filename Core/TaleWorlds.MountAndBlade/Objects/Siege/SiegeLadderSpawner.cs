using System;
using System.Collections.Generic;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Objects.Siege
{
	// Token: 0x020003B0 RID: 944
	public class SiegeLadderSpawner : SpawnerBase
	{
		// Token: 0x1700090C RID: 2316
		// (get) Token: 0x060032E8 RID: 13032 RVA: 0x000D2870 File Offset: 0x000D0A70
		public float UpperStateRotationRadian
		{
			get
			{
				return this.UpperStateRotationDegree * 0.017453292f;
			}
		}

		// Token: 0x1700090D RID: 2317
		// (get) Token: 0x060032E9 RID: 13033 RVA: 0x000D287E File Offset: 0x000D0A7E
		public float DownStateRotationRadian
		{
			get
			{
				return this.DownStateRotationDegree * 0.017453292f;
			}
		}

		// Token: 0x060032EA RID: 13034 RVA: 0x000D288C File Offset: 0x000D0A8C
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

		// Token: 0x060032EB RID: 13035 RVA: 0x000D2914 File Offset: 0x000D0B14
		protected internal override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			this._spawnerEditorHelper.Tick(dt);
		}

		// Token: 0x060032EC RID: 13036 RVA: 0x000D2929 File Offset: 0x000D0B29
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

		// Token: 0x060032ED RID: 13037 RVA: 0x000D2960 File Offset: 0x000D0B60
		private void OnLadderDownStateChange(float unusedArgument)
		{
			GameEntity ghostEntityOrChild = this._spawnerEditorHelper.GetGhostEntityOrChild("ladder_down_state");
			this.DownStateRotationDegree = Vec3.AngleBetweenTwoVectors(Vec3.Up, ghostEntityOrChild.GetFrame().rotation.u) * 57.29578f;
		}

		// Token: 0x060032EE RID: 13038 RVA: 0x000D29A4 File Offset: 0x000D0BA4
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

		// Token: 0x060032EF RID: 13039 RVA: 0x000D2A88 File Offset: 0x000D0C88
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

		// Token: 0x060032F0 RID: 13040 RVA: 0x000D2BA8 File Offset: 0x000D0DA8
		protected internal override void OnPreInit()
		{
			base.OnPreInit();
			this._spawnerMissionHelper = new SpawnerEntityMissionHelper(this, false);
		}

		// Token: 0x060032F1 RID: 13041 RVA: 0x000D2BC0 File Offset: 0x000D0DC0
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

		// Token: 0x0400158A RID: 5514
		[EditorVisibleScriptComponentVariable(false)]
		public MatrixFrame fork_holder = MatrixFrame.Zero;

		// Token: 0x0400158B RID: 5515
		[EditorVisibleScriptComponentVariable(false)]
		public MatrixFrame initial_wait_pos = MatrixFrame.Zero;

		// Token: 0x0400158C RID: 5516
		[EditorVisibleScriptComponentVariable(false)]
		public MatrixFrame use_push = MatrixFrame.Zero;

		// Token: 0x0400158D RID: 5517
		[EditorVisibleScriptComponentVariable(false)]
		public MatrixFrame stand_position_wall_push = MatrixFrame.Zero;

		// Token: 0x0400158E RID: 5518
		[EditorVisibleScriptComponentVariable(false)]
		public MatrixFrame distance_holder = MatrixFrame.Zero;

		// Token: 0x0400158F RID: 5519
		[EditorVisibleScriptComponentVariable(false)]
		public MatrixFrame stand_position_ground_wait = MatrixFrame.Zero;

		// Token: 0x04001590 RID: 5520
		[EditorVisibleScriptComponentVariable(true)]
		public string SideTag;

		// Token: 0x04001591 RID: 5521
		[EditorVisibleScriptComponentVariable(true)]
		public string TargetWallSegmentTag = "";

		// Token: 0x04001592 RID: 5522
		[EditorVisibleScriptComponentVariable(true)]
		public int OnWallNavMeshId = -1;

		// Token: 0x04001593 RID: 5523
		[EditorVisibleScriptComponentVariable(true)]
		public string AddOnDeployTag = "";

		// Token: 0x04001594 RID: 5524
		[EditorVisibleScriptComponentVariable(true)]
		public string RemoveOnDeployTag = "";

		// Token: 0x04001595 RID: 5525
		[EditorVisibleScriptComponentVariable(true)]
		public float UpperStateRotationDegree;

		// Token: 0x04001596 RID: 5526
		[EditorVisibleScriptComponentVariable(true)]
		public float DownStateRotationDegree = 90f;

		// Token: 0x04001597 RID: 5527
		public float TacticalPositionWidth = 1f;

		// Token: 0x04001598 RID: 5528
		[EditorVisibleScriptComponentVariable(true)]
		public string BarrierTagToRemove = string.Empty;

		// Token: 0x04001599 RID: 5529
		[EditorVisibleScriptComponentVariable(true)]
		public string IndestructibleMerlonsTag = string.Empty;
	}
}
