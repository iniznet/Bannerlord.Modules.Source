using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Objects.Siege
{
	// Token: 0x020003B1 RID: 945
	public class SiegeTowerSpawner : SpawnerBase
	{
		// Token: 0x1700090E RID: 2318
		// (get) Token: 0x060032F3 RID: 13043 RVA: 0x000D2D11 File Offset: 0x000D0F11
		public float RampRotationRadian
		{
			get
			{
				return this.RampRotationDegree * 0.017453292f;
			}
		}

		// Token: 0x060032F4 RID: 13044 RVA: 0x000D2D20 File Offset: 0x000D0F20
		protected internal override void OnEditorInit()
		{
			base.OnEditorInit();
			this._spawnerEditorHelper = new SpawnerEntityEditorHelper(this);
			this._spawnerEditorHelper.LockGhostParent = false;
			if (this._spawnerEditorHelper.IsValid)
			{
				this._spawnerEditorHelper.SetupGhostMovement(this.PathEntityName);
				this._spawnerEditorHelper.GivePermission("ramp", new SpawnerEntityEditorHelper.Permission(SpawnerEntityEditorHelper.PermissionType.rotation, SpawnerEntityEditorHelper.Axis.x), new Action<float>(this.SetRampRotation));
				this._spawnerEditorHelper.GivePermission("ai_barrier_r", new SpawnerEntityEditorHelper.Permission(SpawnerEntityEditorHelper.PermissionType.scale, SpawnerEntityEditorHelper.Axis.z), new Action<float>(this.SetAIBarrierRight));
				this._spawnerEditorHelper.GivePermission("ai_barrier_l", new SpawnerEntityEditorHelper.Permission(SpawnerEntityEditorHelper.PermissionType.scale, SpawnerEntityEditorHelper.Axis.z), new Action<float>(this.SetAIBarrierLeft));
			}
			this.OnEditorVariableChanged("RampRotationDegree");
			this.OnEditorVariableChanged("BarrierLength");
		}

		// Token: 0x060032F5 RID: 13045 RVA: 0x000D2DE8 File Offset: 0x000D0FE8
		private void SetRampRotation(float unusedArgument)
		{
			MatrixFrame frame = this._spawnerEditorHelper.GetGhostEntityOrChild("ramp").GetFrame();
			Vec3 vec = new Vec3(-frame.rotation.u.y, frame.rotation.u.x, 0f, -1f);
			float z = frame.rotation.u.z;
			float num = MathF.Atan2(vec.Length, z);
			if ((double)vec.x < 0.0)
			{
				num = -num;
				num += 6.2831855f;
			}
			float num2 = num;
			this.RampRotationDegree = num2 * 57.29578f;
		}

		// Token: 0x060032F6 RID: 13046 RVA: 0x000D2E8C File Offset: 0x000D108C
		private void SetAIBarrierRight(float barrierScale)
		{
			this.BarrierLength = barrierScale;
			MatrixFrame frame = this._spawnerEditorHelper.GetGhostEntityOrChild("ai_barrier_l").GetFrame();
			MatrixFrame frame2 = this._spawnerEditorHelper.GetGhostEntityOrChild("ai_barrier_r").GetFrame();
			frame.rotation.u = frame2.rotation.u;
			this._spawnerEditorHelper.ChangeStableChildMatrixFrameAndApply("ai_barrier_l", frame, false);
		}

		// Token: 0x060032F7 RID: 13047 RVA: 0x000D2EF8 File Offset: 0x000D10F8
		private void SetAIBarrierLeft(float barrierScale)
		{
			this.BarrierLength = barrierScale;
			MatrixFrame frame = this._spawnerEditorHelper.GetGhostEntityOrChild("ai_barrier_l").GetFrame();
			MatrixFrame frame2 = this._spawnerEditorHelper.GetGhostEntityOrChild("ai_barrier_r").GetFrame();
			frame2.rotation.u = frame.rotation.u;
			this._spawnerEditorHelper.ChangeStableChildMatrixFrameAndApply("ai_barrier_r", frame2, false);
		}

		// Token: 0x060032F8 RID: 13048 RVA: 0x000D2F61 File Offset: 0x000D1161
		protected internal override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			this._spawnerEditorHelper.Tick(dt);
		}

		// Token: 0x060032F9 RID: 13049 RVA: 0x000D2F78 File Offset: 0x000D1178
		protected internal override void OnEditorVariableChanged(string variableName)
		{
			base.OnEditorVariableChanged(variableName);
			if (variableName == "PathEntityName")
			{
				this._spawnerEditorHelper.SetupGhostMovement(this.PathEntityName);
				return;
			}
			if (variableName == "EnableAutoGhostMovement")
			{
				this._spawnerEditorHelper.SetEnableAutoGhostMovement(this.EnableAutoGhostMovement);
				return;
			}
			if (variableName == "RampRotationDegree")
			{
				MatrixFrame frame = this._spawnerEditorHelper.GetGhostEntityOrChild("ramp").GetFrame();
				frame.rotation = Mat3.Identity;
				frame.rotation.RotateAboutSide(this.RampRotationRadian);
				this._spawnerEditorHelper.ChangeStableChildMatrixFrameAndApply("ramp", frame, true);
				return;
			}
			if (variableName == "BarrierLength")
			{
				MatrixFrame frame2 = this._spawnerEditorHelper.GetGhostEntityOrChild("ai_barrier_l").GetFrame();
				frame2.rotation.u.Normalize();
				frame2.rotation.u = frame2.rotation.u * MathF.Max(0.01f, MathF.Abs(this.BarrierLength));
				MatrixFrame frame3 = this._spawnerEditorHelper.GetGhostEntityOrChild("ai_barrier_r").GetFrame();
				frame3.rotation.u = frame2.rotation.u;
				this._spawnerEditorHelper.ChangeStableChildMatrixFrameAndApply("ai_barrier_l", frame2, true);
				this._spawnerEditorHelper.ChangeStableChildMatrixFrameAndApply("ai_barrier_r", frame3, true);
				return;
			}
			if (variableName == "SpeedModifierFactor")
			{
				this.SpeedModifierFactor = MathF.Clamp(this.SpeedModifierFactor, 0.8f, 1.2f);
			}
		}

		// Token: 0x060032FA RID: 13050 RVA: 0x000D3101 File Offset: 0x000D1301
		protected internal override void OnPreInit()
		{
			base.OnPreInit();
			this._spawnerMissionHelper = new SpawnerEntityMissionHelper(this, false);
		}

		// Token: 0x060032FB RID: 13051 RVA: 0x000D3118 File Offset: 0x000D1318
		public override void AssignParameters(SpawnerEntityMissionHelper _spawnerMissionHelper)
		{
			SiegeTower firstScriptOfType = _spawnerMissionHelper.SpawnedEntity.GetFirstScriptOfType<SiegeTower>();
			firstScriptOfType.AddOnDeployTag = this.AddOnDeployTag;
			firstScriptOfType.RemoveOnDeployTag = this.RemoveOnDeployTag;
			firstScriptOfType.MaxSpeed *= this.SpeedModifierFactor;
			firstScriptOfType.MinSpeed *= this.SpeedModifierFactor;
			Mat3 identity = Mat3.Identity;
			identity.RotateAboutSide(this.RampRotationRadian);
			firstScriptOfType.AssignParametersFromSpawner(this.PathEntityName, this.TargetWallSegmentTag, this.SideTag, this.SoilNavMeshID1, this.SoilNavMeshID2, this.DitchNavMeshID1, this.DitchNavMeshID2, this.GroundToSoilNavMeshID1, this.GroundToSoilNavMeshID2, this.SoilGenericNavMeshID, this.GroundGenericNavMeshID, identity, this.BarrierTagToRemove);
		}

		// Token: 0x0400159A RID: 5530
		private const float _modifierFactorUpperLimit = 1.2f;

		// Token: 0x0400159B RID: 5531
		private const float _modifierFactorLowerLimit = 0.8f;

		// Token: 0x0400159C RID: 5532
		[EditorVisibleScriptComponentVariable(false)]
		public MatrixFrame wait_pos_ground = MatrixFrame.Zero;

		// Token: 0x0400159D RID: 5533
		[EditorVisibleScriptComponentVariable(true)]
		public string SideTag;

		// Token: 0x0400159E RID: 5534
		[EditorVisibleScriptComponentVariable(true)]
		public string TargetWallSegmentTag = "";

		// Token: 0x0400159F RID: 5535
		[EditorVisibleScriptComponentVariable(true)]
		public string PathEntityName = "Path";

		// Token: 0x040015A0 RID: 5536
		[EditorVisibleScriptComponentVariable(true)]
		public int SoilNavMeshID1 = -1;

		// Token: 0x040015A1 RID: 5537
		[EditorVisibleScriptComponentVariable(true)]
		public int SoilNavMeshID2 = -1;

		// Token: 0x040015A2 RID: 5538
		[EditorVisibleScriptComponentVariable(true)]
		public int DitchNavMeshID1 = -1;

		// Token: 0x040015A3 RID: 5539
		[EditorVisibleScriptComponentVariable(true)]
		public int DitchNavMeshID2 = -1;

		// Token: 0x040015A4 RID: 5540
		[EditorVisibleScriptComponentVariable(true)]
		public int GroundToSoilNavMeshID1 = -1;

		// Token: 0x040015A5 RID: 5541
		[EditorVisibleScriptComponentVariable(true)]
		public int GroundToSoilNavMeshID2 = -1;

		// Token: 0x040015A6 RID: 5542
		[EditorVisibleScriptComponentVariable(true)]
		public int SoilGenericNavMeshID = -1;

		// Token: 0x040015A7 RID: 5543
		[EditorVisibleScriptComponentVariable(true)]
		public int GroundGenericNavMeshID = -1;

		// Token: 0x040015A8 RID: 5544
		[EditorVisibleScriptComponentVariable(true)]
		public string AddOnDeployTag = "";

		// Token: 0x040015A9 RID: 5545
		[EditorVisibleScriptComponentVariable(true)]
		public string RemoveOnDeployTag = "";

		// Token: 0x040015AA RID: 5546
		[EditorVisibleScriptComponentVariable(true)]
		public float RampRotationDegree;

		// Token: 0x040015AB RID: 5547
		[EditorVisibleScriptComponentVariable(true)]
		public float BarrierLength = 1f;

		// Token: 0x040015AC RID: 5548
		[EditorVisibleScriptComponentVariable(true)]
		public float SpeedModifierFactor = 1f;

		// Token: 0x040015AD RID: 5549
		public bool EnableAutoGhostMovement;

		// Token: 0x040015AE RID: 5550
		[EditorVisibleScriptComponentVariable(false)]
		[RestrictedAccess]
		public MatrixFrame ai_barrier_l = MatrixFrame.Zero;

		// Token: 0x040015AF RID: 5551
		[EditorVisibleScriptComponentVariable(false)]
		[RestrictedAccess]
		public MatrixFrame ai_barrier_r = MatrixFrame.Zero;

		// Token: 0x040015B0 RID: 5552
		[EditorVisibleScriptComponentVariable(true)]
		public string BarrierTagToRemove = string.Empty;
	}
}
