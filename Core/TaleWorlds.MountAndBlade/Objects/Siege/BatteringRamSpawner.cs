using System;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Objects.Siege
{
	// Token: 0x020003A4 RID: 932
	public class BatteringRamSpawner : SpawnerBase
	{
		// Token: 0x060032C5 RID: 12997 RVA: 0x000D1FC2 File Offset: 0x000D01C2
		protected internal override void OnEditorInit()
		{
			base.OnEditorInit();
			this._spawnerEditorHelper = new SpawnerEntityEditorHelper(this);
			this._spawnerEditorHelper.LockGhostParent = false;
			if (this._spawnerEditorHelper.IsValid)
			{
				this._spawnerEditorHelper.SetupGhostMovement(this.PathEntityName);
			}
		}

		// Token: 0x060032C6 RID: 12998 RVA: 0x000D2000 File Offset: 0x000D0200
		protected internal override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			this._spawnerEditorHelper.Tick(dt);
		}

		// Token: 0x060032C7 RID: 12999 RVA: 0x000D2018 File Offset: 0x000D0218
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
			if (variableName == "SpeedModifierFactor")
			{
				this.SpeedModifierFactor = MathF.Clamp(this.SpeedModifierFactor, 0.8f, 1.2f);
			}
		}

		// Token: 0x060032C8 RID: 13000 RVA: 0x000D2094 File Offset: 0x000D0294
		protected internal override bool OnCheckForProblems()
		{
			bool flag = base.OnCheckForProblems();
			if (!base.Scene.IsMultiplayerScene() && base.Scene.FindEntitiesWithTag("ditch_filler").FirstOrDefault((GameEntity df) => df.HasTag(this.SideTag)) != null)
			{
				if (this.DitchNavMeshID_1 >= 0 && !base.Scene.IsAnyFaceWithId(this.DitchNavMeshID_1))
				{
					MBEditor.AddEntityWarning(base.GameEntity, "Couldn't find any face with 'DitchNavMeshID_1' id.");
					flag = true;
				}
				if (this.DitchNavMeshID_2 >= 0 && !base.Scene.IsAnyFaceWithId(this.DitchNavMeshID_2))
				{
					MBEditor.AddEntityWarning(base.GameEntity, "Couldn't find any face with 'DitchNavMeshID_2' id.");
					flag = true;
				}
				if (this.GroundToBridgeNavMeshID_1 >= 0 && !base.Scene.IsAnyFaceWithId(this.GroundToBridgeNavMeshID_1))
				{
					MBEditor.AddEntityWarning(base.GameEntity, "Couldn't find any face with 'GroundToBridgeNavMeshID_1' id.");
					flag = true;
				}
				if (this.GroundToBridgeNavMeshID_2 >= 0 && !base.Scene.IsAnyFaceWithId(this.GroundToBridgeNavMeshID_2))
				{
					MBEditor.AddEntityWarning(base.GameEntity, "Couldn't find any face with 'GroundToBridgeNavMeshID_1' id.");
					flag = true;
				}
				if (this.BridgeNavMeshID_1 >= 0 && !base.Scene.IsAnyFaceWithId(this.BridgeNavMeshID_1))
				{
					MBEditor.AddEntityWarning(base.GameEntity, "Couldn't find any face with 'BridgeNavMeshID_1' id.");
					flag = true;
				}
				if (this.BridgeNavMeshID_2 >= 0 && !base.Scene.IsAnyFaceWithId(this.BridgeNavMeshID_2))
				{
					MBEditor.AddEntityWarning(base.GameEntity, "Couldn't find any face with 'BridgeNavMeshID_2' id.");
					flag = true;
				}
			}
			return flag;
		}

		// Token: 0x060032C9 RID: 13001 RVA: 0x000D21F9 File Offset: 0x000D03F9
		protected internal override void OnPreInit()
		{
			base.OnPreInit();
			this._spawnerMissionHelper = new SpawnerEntityMissionHelper(this, false);
		}

		// Token: 0x060032CA RID: 13002 RVA: 0x000D2210 File Offset: 0x000D0410
		public override void AssignParameters(SpawnerEntityMissionHelper _spawnerMissionHelper)
		{
			BatteringRam firstScriptOfType = _spawnerMissionHelper.SpawnedEntity.GetFirstScriptOfType<BatteringRam>();
			firstScriptOfType.AddOnDeployTag = this.AddOnDeployTag;
			firstScriptOfType.RemoveOnDeployTag = this.RemoveOnDeployTag;
			firstScriptOfType.MaxSpeed *= this.SpeedModifierFactor;
			firstScriptOfType.MinSpeed *= this.SpeedModifierFactor;
			firstScriptOfType.AssignParametersFromSpawner(this.GateTag, this.SideTag, this.BridgeNavMeshID_1, this.BridgeNavMeshID_2, this.DitchNavMeshID_1, this.DitchNavMeshID_2, this.GroundToBridgeNavMeshID_1, this.GroundToBridgeNavMeshID_2, this.PathEntityName);
		}

		// Token: 0x04001568 RID: 5480
		private const float _modifierFactorUpperLimit = 1.2f;

		// Token: 0x04001569 RID: 5481
		private const float _modifierFactorLowerLimit = 0.8f;

		// Token: 0x0400156A RID: 5482
		[EditorVisibleScriptComponentVariable(false)]
		public MatrixFrame wait_pos_ground = MatrixFrame.Zero;

		// Token: 0x0400156B RID: 5483
		[EditorVisibleScriptComponentVariable(true)]
		public string SideTag;

		// Token: 0x0400156C RID: 5484
		[EditorVisibleScriptComponentVariable(true)]
		public string GateTag = "";

		// Token: 0x0400156D RID: 5485
		[EditorVisibleScriptComponentVariable(true)]
		public string PathEntityName = "Path";

		// Token: 0x0400156E RID: 5486
		[EditorVisibleScriptComponentVariable(true)]
		public int BridgeNavMeshID_1 = 8;

		// Token: 0x0400156F RID: 5487
		[EditorVisibleScriptComponentVariable(true)]
		public int BridgeNavMeshID_2 = 8;

		// Token: 0x04001570 RID: 5488
		[EditorVisibleScriptComponentVariable(true)]
		public int DitchNavMeshID_1 = 9;

		// Token: 0x04001571 RID: 5489
		[EditorVisibleScriptComponentVariable(true)]
		public int DitchNavMeshID_2 = 10;

		// Token: 0x04001572 RID: 5490
		[EditorVisibleScriptComponentVariable(true)]
		public int GroundToBridgeNavMeshID_1 = 12;

		// Token: 0x04001573 RID: 5491
		[EditorVisibleScriptComponentVariable(true)]
		public int GroundToBridgeNavMeshID_2 = 13;

		// Token: 0x04001574 RID: 5492
		[EditorVisibleScriptComponentVariable(true)]
		public string AddOnDeployTag = "";

		// Token: 0x04001575 RID: 5493
		[EditorVisibleScriptComponentVariable(true)]
		public string RemoveOnDeployTag = "";

		// Token: 0x04001576 RID: 5494
		[EditorVisibleScriptComponentVariable(true)]
		public float SpeedModifierFactor = 1f;

		// Token: 0x04001577 RID: 5495
		public bool EnableAutoGhostMovement;
	}
}
