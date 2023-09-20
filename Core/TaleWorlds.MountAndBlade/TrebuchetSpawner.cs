using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Objects.Siege;

namespace TaleWorlds.MountAndBlade
{
	public class TrebuchetSpawner : SpawnerBase
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
				if (gameEntity.GetFirstScriptOfType<Trebuchet>() != null)
				{
					gameEntity.GetFirstScriptOfType<Trebuchet>().AddOnDeployTag = this.AddOnDeployTag;
					gameEntity.GetFirstScriptOfType<Trebuchet>().RemoveOnDeployTag = this.RemoveOnDeployTag;
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

		private Vec3 _projectileStartingPositionOffset = new Vec3(0f, 0.86f, 18f, -1f);
	}
}
