using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Objects.Siege
{
	public class MangonelSpawner : SpawnerBase
	{
		protected internal override void OnEditorInit()
		{
			base.OnEditorInit();
			this._spawnerEditorHelper = new SpawnerEntityEditorHelper(this);
		}

		protected internal override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			this._spawnerEditorHelper.Tick(dt);
			this.RefreshAmmoPositions();
		}

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

		protected internal override void OnPreInit()
		{
			base.OnPreInit();
			this._spawnerMissionHelper = new SpawnerEntityMissionHelper(this, false);
			this._spawnerMissionHelperFire = new SpawnerEntityMissionHelper(this, true);
		}

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

		[EditorVisibleScriptComponentVariable(false)]
		public MatrixFrame projectile_pile = MatrixFrame.Zero;

		[EditorVisibleScriptComponentVariable(true)]
		public string AddOnDeployTag = "";

		[EditorVisibleScriptComponentVariable(true)]
		public string RemoveOnDeployTag = "";

		[EditorVisibleScriptComponentVariable(true)]
		public bool ammo_pos_a_enabled = true;

		[EditorVisibleScriptComponentVariable(true)]
		public bool ammo_pos_b_enabled = true;

		[EditorVisibleScriptComponentVariable(true)]
		public bool ammo_pos_c_enabled = true;

		[EditorVisibleScriptComponentVariable(true)]
		public bool ammo_pos_d_enabled = true;

		[EditorVisibleScriptComponentVariable(true)]
		public bool ammo_pos_e_enabled = true;

		[EditorVisibleScriptComponentVariable(true)]
		public bool ammo_pos_f_enabled = true;

		[EditorVisibleScriptComponentVariable(true)]
		public bool ammo_pos_g_enabled = true;

		[EditorVisibleScriptComponentVariable(true)]
		public bool ammo_pos_h_enabled = true;

		public bool ShowTrajectory;

		private Vec3 _projectileStartingPositionOffset = new Vec3(0f, -0.4f, 3.85f, -1f);
	}
}
