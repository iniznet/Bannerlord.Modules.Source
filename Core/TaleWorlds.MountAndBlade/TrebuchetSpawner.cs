using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Objects.Siege;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000368 RID: 872
	public class TrebuchetSpawner : SpawnerBase
	{
		// Token: 0x06002F8D RID: 12173 RVA: 0x000C2DD7 File Offset: 0x000C0FD7
		protected internal override void OnEditorInit()
		{
			base.OnEditorInit();
			this._spawnerEditorHelper = new SpawnerEntityEditorHelper(this);
		}

		// Token: 0x06002F8E RID: 12174 RVA: 0x000C2DEB File Offset: 0x000C0FEB
		protected internal override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			this._spawnerEditorHelper.Tick(dt);
			this.RefreshAmmoPositions();
		}

		// Token: 0x06002F8F RID: 12175 RVA: 0x000C2E08 File Offset: 0x000C1008
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
					Trebuchet firstScriptInFamilyDescending = GameEntity.Instantiate(null, this._spawnerEditorHelper.GetPrefabName(), false).GetFirstScriptInFamilyDescending<Trebuchet>();
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

		// Token: 0x06002F90 RID: 12176 RVA: 0x000C2F38 File Offset: 0x000C1138
		private void RefreshAmmoPositions()
		{
			if (this.ammo_pos_a_enabled)
			{
				this._spawnerEditorHelper.GetGhostEntityOrChild("ammo_pos_a").SetVisibilityExcludeParents(true);
			}
			else
			{
				this._spawnerEditorHelper.GetGhostEntityOrChild("ammo_pos_a").SetVisibilityExcludeParents(false);
			}
			if (this.ammo_pos_b_enabled)
			{
				this._spawnerEditorHelper.GetGhostEntityOrChild("ammo_pos_b").SetVisibilityExcludeParents(true);
			}
			else
			{
				this._spawnerEditorHelper.GetGhostEntityOrChild("ammo_pos_b").SetVisibilityExcludeParents(false);
			}
			if (this.ammo_pos_c_enabled)
			{
				this._spawnerEditorHelper.GetGhostEntityOrChild("ammo_pos_c").SetVisibilityExcludeParents(true);
			}
			else
			{
				this._spawnerEditorHelper.GetGhostEntityOrChild("ammo_pos_c").SetVisibilityExcludeParents(false);
			}
			if (this.ammo_pos_d_enabled)
			{
				this._spawnerEditorHelper.GetGhostEntityOrChild("ammo_pos_d").SetVisibilityExcludeParents(true);
			}
			else
			{
				this._spawnerEditorHelper.GetGhostEntityOrChild("ammo_pos_d").SetVisibilityExcludeParents(false);
			}
			if (this.ammo_pos_e_enabled)
			{
				this._spawnerEditorHelper.GetGhostEntityOrChild("ammo_pos_e").SetVisibilityExcludeParents(true);
			}
			else
			{
				this._spawnerEditorHelper.GetGhostEntityOrChild("ammo_pos_e").SetVisibilityExcludeParents(false);
			}
			if (this.ammo_pos_f_enabled)
			{
				this._spawnerEditorHelper.GetGhostEntityOrChild("ammo_pos_f").SetVisibilityExcludeParents(true);
			}
			else
			{
				this._spawnerEditorHelper.GetGhostEntityOrChild("ammo_pos_f").SetVisibilityExcludeParents(false);
			}
			if (this.ammo_pos_g_enabled)
			{
				this._spawnerEditorHelper.GetGhostEntityOrChild("ammo_pos_g").SetVisibilityExcludeParents(true);
			}
			else
			{
				this._spawnerEditorHelper.GetGhostEntityOrChild("ammo_pos_g").SetVisibilityExcludeParents(false);
			}
			if (this.ammo_pos_h_enabled)
			{
				this._spawnerEditorHelper.GetGhostEntityOrChild("ammo_pos_h").SetVisibilityExcludeParents(true);
				return;
			}
			this._spawnerEditorHelper.GetGhostEntityOrChild("ammo_pos_h").SetVisibilityExcludeParents(false);
		}

		// Token: 0x06002F91 RID: 12177 RVA: 0x000C30F4 File Offset: 0x000C12F4
		protected internal override void OnPreInit()
		{
			base.OnPreInit();
			this._spawnerMissionHelper = new SpawnerEntityMissionHelper(this, false);
			this._spawnerMissionHelperFire = new SpawnerEntityMissionHelper(this, true);
		}

		// Token: 0x06002F92 RID: 12178 RVA: 0x000C3118 File Offset: 0x000C1318
		public override void AssignParameters(SpawnerEntityMissionHelper _spawnerMissionHelper)
		{
			foreach (GameEntity gameEntity in _spawnerMissionHelper.SpawnedEntity.GetChildren())
			{
				if (gameEntity.GetFirstScriptOfType<Trebuchet>() != null)
				{
					gameEntity.GetFirstScriptOfType<Trebuchet>().AddOnDeployTag = this.AddOnDeployTag;
					gameEntity.GetFirstScriptOfType<Trebuchet>().RemoveOnDeployTag = this.RemoveOnDeployTag;
					break;
				}
			}
		}

		// Token: 0x040013A2 RID: 5026
		[EditorVisibleScriptComponentVariable(false)]
		public MatrixFrame projectile_pile = MatrixFrame.Zero;

		// Token: 0x040013A3 RID: 5027
		[EditorVisibleScriptComponentVariable(true)]
		public string AddOnDeployTag = "";

		// Token: 0x040013A4 RID: 5028
		[EditorVisibleScriptComponentVariable(true)]
		public string RemoveOnDeployTag = "";

		// Token: 0x040013A5 RID: 5029
		[EditorVisibleScriptComponentVariable(true)]
		public bool ammo_pos_a_enabled = true;

		// Token: 0x040013A6 RID: 5030
		[EditorVisibleScriptComponentVariable(true)]
		public bool ammo_pos_b_enabled = true;

		// Token: 0x040013A7 RID: 5031
		[EditorVisibleScriptComponentVariable(true)]
		public bool ammo_pos_c_enabled = true;

		// Token: 0x040013A8 RID: 5032
		[EditorVisibleScriptComponentVariable(true)]
		public bool ammo_pos_d_enabled = true;

		// Token: 0x040013A9 RID: 5033
		[EditorVisibleScriptComponentVariable(true)]
		public bool ammo_pos_e_enabled = true;

		// Token: 0x040013AA RID: 5034
		[EditorVisibleScriptComponentVariable(true)]
		public bool ammo_pos_f_enabled = true;

		// Token: 0x040013AB RID: 5035
		[EditorVisibleScriptComponentVariable(true)]
		public bool ammo_pos_g_enabled = true;

		// Token: 0x040013AC RID: 5036
		[EditorVisibleScriptComponentVariable(true)]
		public bool ammo_pos_h_enabled = true;

		// Token: 0x040013AD RID: 5037
		public bool ShowTrajectory;

		// Token: 0x040013AE RID: 5038
		private Vec3 _projectileStartingPositionOffset = new Vec3(0f, 0.86f, 18f, -1f);
	}
}
