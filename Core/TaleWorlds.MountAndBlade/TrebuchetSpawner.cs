using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Objects.Siege;

namespace TaleWorlds.MountAndBlade
{
	public class TrebuchetSpawner : SpawnerBase
	{
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

		[SpawnerBase.SpawnerPermissionField]
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
	}
}
