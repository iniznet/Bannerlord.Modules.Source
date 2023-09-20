using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Objects.Siege
{
	// Token: 0x020003A7 RID: 935
	public class MangonelSpawner : SpawnerBase
	{
		// Token: 0x060032D1 RID: 13009 RVA: 0x000D23D3 File Offset: 0x000D05D3
		protected internal override void OnEditorInit()
		{
			base.OnEditorInit();
			this._spawnerEditorHelper = new SpawnerEntityEditorHelper(this);
		}

		// Token: 0x060032D2 RID: 13010 RVA: 0x000D23E7 File Offset: 0x000D05E7
		protected internal override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			this._spawnerEditorHelper.Tick(dt);
			this.RefreshAmmoPositions();
		}

		// Token: 0x060032D3 RID: 13011 RVA: 0x000D2404 File Offset: 0x000D0604
		protected internal override void OnEditorVariableChanged(string variableName)
		{
			base.OnEditorVariableChanged(variableName);
			if (variableName == "ammo_pos_a_enabled" || variableName == "ammo_pos_b_enabled" || variableName == "ammo_pos_c_enabled" || variableName == "ammo_pos_d_enabled" || variableName == "ammo_pos_e_enabled" || variableName == "ammo_pos_f_enabled" || variableName == "ammo_pos_g_enabled" || variableName == "ammo_pos_h_enabled")
			{
				this.RefreshAmmoPositions();
			}
			if (variableName == "ShowTrajectory")
			{
				if (this.ShowTrajectory && this.TrajectoryMeshHolder == null && !base.GameEntity.IsGhostObject())
				{
					Mangonel firstScriptInFamilyDescending = GameEntity.Instantiate(null, this._spawnerEditorHelper.GetPrefabName(), false).GetFirstScriptInFamilyDescending<Mangonel>();
					float num = ((firstScriptInFamilyDescending.TopReleaseAngleRestriction > 0.7853982f) ? 0.7853982f : firstScriptInFamilyDescending.TopReleaseAngleRestriction);
					base.CreateTrajectoryVisualizer(this._projectileStartingPositionOffset, firstScriptInFamilyDescending.ProjectileSpeed, firstScriptInFamilyDescending.ProjectileSpeed, num, firstScriptInFamilyDescending.BottomReleaseAngleRestriction, 2.0943952f, ItemObject.GetAirFrictionConstant(WeaponClass.Boulder, WeaponFlags.AffectsArea));
				}
				GameEntity trajectoryMeshHolder = this.TrajectoryMeshHolder;
				if (trajectoryMeshHolder == null)
				{
					return;
				}
				trajectoryMeshHolder.SetVisibilityExcludeParents(this.ShowTrajectory);
			}
		}

		// Token: 0x060032D4 RID: 13012 RVA: 0x000D2534 File Offset: 0x000D0734
		private void RefreshAmmoPositions()
		{
			this._spawnerEditorHelper.GetGhostEntityOrChild("ammo_pos_a").SetVisibilityExcludeParents(this.ammo_pos_a_enabled);
			this._spawnerEditorHelper.GetGhostEntityOrChild("ammo_pos_b").SetVisibilityExcludeParents(this.ammo_pos_b_enabled);
			this._spawnerEditorHelper.GetGhostEntityOrChild("ammo_pos_c").SetVisibilityExcludeParents(this.ammo_pos_c_enabled);
			this._spawnerEditorHelper.GetGhostEntityOrChild("ammo_pos_d").SetVisibilityExcludeParents(this.ammo_pos_d_enabled);
			this._spawnerEditorHelper.GetGhostEntityOrChild("ammo_pos_e").SetVisibilityExcludeParents(this.ammo_pos_e_enabled);
			this._spawnerEditorHelper.GetGhostEntityOrChild("ammo_pos_f").SetVisibilityExcludeParents(this.ammo_pos_f_enabled);
			this._spawnerEditorHelper.GetGhostEntityOrChild("ammo_pos_g").SetVisibilityExcludeParents(this.ammo_pos_g_enabled);
			this._spawnerEditorHelper.GetGhostEntityOrChild("ammo_pos_h").SetVisibilityExcludeParents(this.ammo_pos_h_enabled);
		}

		// Token: 0x060032D5 RID: 13013 RVA: 0x000D2619 File Offset: 0x000D0819
		protected internal override void OnPreInit()
		{
			base.OnPreInit();
			this._spawnerMissionHelper = new SpawnerEntityMissionHelper(this, false);
			this._spawnerMissionHelperFire = new SpawnerEntityMissionHelper(this, true);
		}

		// Token: 0x060032D6 RID: 13014 RVA: 0x000D263C File Offset: 0x000D083C
		public override void AssignParameters(SpawnerEntityMissionHelper _spawnerMissionHelper)
		{
			foreach (GameEntity gameEntity in _spawnerMissionHelper.SpawnedEntity.GetChildren())
			{
				if (gameEntity.GetFirstScriptOfType<Mangonel>() != null)
				{
					gameEntity.GetFirstScriptOfType<Mangonel>().AddOnDeployTag = this.AddOnDeployTag;
					gameEntity.GetFirstScriptOfType<Mangonel>().RemoveOnDeployTag = this.RemoveOnDeployTag;
					break;
				}
			}
		}

		// Token: 0x04001578 RID: 5496
		[EditorVisibleScriptComponentVariable(false)]
		public MatrixFrame projectile_pile = MatrixFrame.Zero;

		// Token: 0x04001579 RID: 5497
		[EditorVisibleScriptComponentVariable(true)]
		public string AddOnDeployTag = "";

		// Token: 0x0400157A RID: 5498
		[EditorVisibleScriptComponentVariable(true)]
		public string RemoveOnDeployTag = "";

		// Token: 0x0400157B RID: 5499
		[EditorVisibleScriptComponentVariable(true)]
		public bool ammo_pos_a_enabled = true;

		// Token: 0x0400157C RID: 5500
		[EditorVisibleScriptComponentVariable(true)]
		public bool ammo_pos_b_enabled = true;

		// Token: 0x0400157D RID: 5501
		[EditorVisibleScriptComponentVariable(true)]
		public bool ammo_pos_c_enabled = true;

		// Token: 0x0400157E RID: 5502
		[EditorVisibleScriptComponentVariable(true)]
		public bool ammo_pos_d_enabled = true;

		// Token: 0x0400157F RID: 5503
		[EditorVisibleScriptComponentVariable(true)]
		public bool ammo_pos_e_enabled = true;

		// Token: 0x04001580 RID: 5504
		[EditorVisibleScriptComponentVariable(true)]
		public bool ammo_pos_f_enabled = true;

		// Token: 0x04001581 RID: 5505
		[EditorVisibleScriptComponentVariable(true)]
		public bool ammo_pos_g_enabled = true;

		// Token: 0x04001582 RID: 5506
		[EditorVisibleScriptComponentVariable(true)]
		public bool ammo_pos_h_enabled = true;

		// Token: 0x04001583 RID: 5507
		public bool ShowTrajectory;

		// Token: 0x04001584 RID: 5508
		private Vec3 _projectileStartingPositionOffset = new Vec3(0f, -0.4f, 3.85f, -1f);
	}
}
