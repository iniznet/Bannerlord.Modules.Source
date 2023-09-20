using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Objects.Siege
{
	public class SiegeTowerSpawner : SpawnerBase
	{
		public float RampRotationRadian
		{
			get
			{
				return this.RampRotationDegree * 0.017453292f;
			}
		}

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

		private void SetAIBarrierRight(float barrierScale)
		{
			this.BarrierLength = barrierScale;
			MatrixFrame frame = this._spawnerEditorHelper.GetGhostEntityOrChild("ai_barrier_l").GetFrame();
			MatrixFrame frame2 = this._spawnerEditorHelper.GetGhostEntityOrChild("ai_barrier_r").GetFrame();
			frame.rotation.u = frame2.rotation.u;
			this._spawnerEditorHelper.ChangeStableChildMatrixFrameAndApply("ai_barrier_l", frame, false);
		}

		private void SetAIBarrierLeft(float barrierScale)
		{
			this.BarrierLength = barrierScale;
			MatrixFrame frame = this._spawnerEditorHelper.GetGhostEntityOrChild("ai_barrier_l").GetFrame();
			MatrixFrame frame2 = this._spawnerEditorHelper.GetGhostEntityOrChild("ai_barrier_r").GetFrame();
			frame2.rotation.u = frame.rotation.u;
			this._spawnerEditorHelper.ChangeStableChildMatrixFrameAndApply("ai_barrier_r", frame2, false);
		}

		protected internal override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			this._spawnerEditorHelper.Tick(dt);
		}

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

		protected internal override void OnPreInit()
		{
			base.OnPreInit();
			this._spawnerMissionHelper = new SpawnerEntityMissionHelper(this, false);
		}

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

		private const float _modifierFactorUpperLimit = 1.2f;

		private const float _modifierFactorLowerLimit = 0.8f;

		[EditorVisibleScriptComponentVariable(false)]
		public MatrixFrame wait_pos_ground = MatrixFrame.Zero;

		[EditorVisibleScriptComponentVariable(true)]
		public string SideTag;

		[EditorVisibleScriptComponentVariable(true)]
		public string TargetWallSegmentTag = "";

		[EditorVisibleScriptComponentVariable(true)]
		public string PathEntityName = "Path";

		[EditorVisibleScriptComponentVariable(true)]
		public int SoilNavMeshID1 = -1;

		[EditorVisibleScriptComponentVariable(true)]
		public int SoilNavMeshID2 = -1;

		[EditorVisibleScriptComponentVariable(true)]
		public int DitchNavMeshID1 = -1;

		[EditorVisibleScriptComponentVariable(true)]
		public int DitchNavMeshID2 = -1;

		[EditorVisibleScriptComponentVariable(true)]
		public int GroundToSoilNavMeshID1 = -1;

		[EditorVisibleScriptComponentVariable(true)]
		public int GroundToSoilNavMeshID2 = -1;

		[EditorVisibleScriptComponentVariable(true)]
		public int SoilGenericNavMeshID = -1;

		[EditorVisibleScriptComponentVariable(true)]
		public int GroundGenericNavMeshID = -1;

		[EditorVisibleScriptComponentVariable(true)]
		public string AddOnDeployTag = "";

		[EditorVisibleScriptComponentVariable(true)]
		public string RemoveOnDeployTag = "";

		[EditorVisibleScriptComponentVariable(true)]
		public float RampRotationDegree;

		[EditorVisibleScriptComponentVariable(true)]
		public float BarrierLength = 1f;

		[EditorVisibleScriptComponentVariable(true)]
		public float SpeedModifierFactor = 1f;

		public bool EnableAutoGhostMovement;

		[EditorVisibleScriptComponentVariable(false)]
		[RestrictedAccess]
		public MatrixFrame ai_barrier_l = MatrixFrame.Zero;

		[EditorVisibleScriptComponentVariable(false)]
		[RestrictedAccess]
		public MatrixFrame ai_barrier_r = MatrixFrame.Zero;

		[EditorVisibleScriptComponentVariable(true)]
		public string BarrierTagToRemove = string.Empty;
	}
}
