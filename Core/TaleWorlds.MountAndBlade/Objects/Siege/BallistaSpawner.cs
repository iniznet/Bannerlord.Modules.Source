using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Objects.Siege
{
	// Token: 0x020003A3 RID: 931
	public class BallistaSpawner : SpawnerBase
	{
		// Token: 0x060032BF RID: 12991 RVA: 0x000D1DF2 File Offset: 0x000CFFF2
		protected internal override void OnEditorInit()
		{
			base.OnEditorInit();
			this._spawnerEditorHelper = new SpawnerEntityEditorHelper(this);
		}

		// Token: 0x060032C0 RID: 12992 RVA: 0x000D1E06 File Offset: 0x000D0006
		protected internal override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			this._spawnerEditorHelper.Tick(dt);
		}

		// Token: 0x060032C1 RID: 12993 RVA: 0x000D1E1C File Offset: 0x000D001C
		protected internal override void OnEditorVariableChanged(string variableName)
		{
			base.OnEditorVariableChanged(variableName);
			if (variableName == "ShowTrajectory")
			{
				if (this.ShowTrajectory && this.TrajectoryMeshHolder == null && !base.GameEntity.IsGhostObject())
				{
					Ballista firstScriptInFamilyDescending = GameEntity.Instantiate(null, this._spawnerEditorHelper.GetPrefabName(), false).GetFirstScriptInFamilyDescending<Ballista>();
					float num = ((firstScriptInFamilyDescending.TopReleaseAngleRestriction > 0.7853982f) ? 0.7853982f : firstScriptInFamilyDescending.TopReleaseAngleRestriction);
					base.CreateTrajectoryVisualizer(this._projectileStartingPositionOffset, firstScriptInFamilyDescending.BallistaShootingSpeed, firstScriptInFamilyDescending.BallistaShootingSpeed, num, firstScriptInFamilyDescending.BottomReleaseAngleRestriction, this.DirectionRestrictionDegree * 0.017453292f, ItemObject.GetAirFrictionConstant(WeaponClass.Arrow, WeaponFlags.MultiplePenetration));
				}
				GameEntity trajectoryMeshHolder = this.TrajectoryMeshHolder;
				if (trajectoryMeshHolder == null)
				{
					return;
				}
				trajectoryMeshHolder.SetVisibilityExcludeParents(this.ShowTrajectory);
			}
		}

		// Token: 0x060032C2 RID: 12994 RVA: 0x000D1EE7 File Offset: 0x000D00E7
		protected internal override void OnPreInit()
		{
			base.OnPreInit();
			this._spawnerMissionHelper = new SpawnerEntityMissionHelper(this, false);
			this._spawnerMissionHelperFire = new SpawnerEntityMissionHelper(this, true);
		}

		// Token: 0x060032C3 RID: 12995 RVA: 0x000D1F0C File Offset: 0x000D010C
		public override void AssignParameters(SpawnerEntityMissionHelper _spawnerMissionHelper)
		{
			_spawnerMissionHelper.SpawnedEntity.GetFirstScriptOfType<Ballista>().AddOnDeployTag = this.AddOnDeployTag;
			_spawnerMissionHelper.SpawnedEntity.GetFirstScriptOfType<Ballista>().RemoveOnDeployTag = this.RemoveOnDeployTag;
			_spawnerMissionHelper.SpawnedEntity.GetFirstScriptOfType<Ballista>().HorizontalDirectionRestriction = this.DirectionRestrictionDegree * 0.017453292f;
		}

		// Token: 0x04001562 RID: 5474
		[EditorVisibleScriptComponentVariable(false)]
		public MatrixFrame wait_pos_ground = MatrixFrame.Zero;

		// Token: 0x04001563 RID: 5475
		[EditorVisibleScriptComponentVariable(true)]
		public string AddOnDeployTag = "";

		// Token: 0x04001564 RID: 5476
		[EditorVisibleScriptComponentVariable(true)]
		public string RemoveOnDeployTag = "";

		// Token: 0x04001565 RID: 5477
		public bool ShowTrajectory;

		// Token: 0x04001566 RID: 5478
		[EditorVisibleScriptComponentVariable(true)]
		public float DirectionRestrictionDegree = 90f;

		// Token: 0x04001567 RID: 5479
		private Vec3 _projectileStartingPositionOffset = new Vec3(0f, -0.86f, 1.34f, -1f);
	}
}
