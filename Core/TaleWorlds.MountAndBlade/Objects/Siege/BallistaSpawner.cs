using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Objects.Siege
{
	public class BallistaSpawner : SpawnerBase
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
		}

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

		protected internal override void OnPreInit()
		{
			base.OnPreInit();
			this._spawnerMissionHelper = new SpawnerEntityMissionHelper(this, false);
			this._spawnerMissionHelperFire = new SpawnerEntityMissionHelper(this, true);
		}

		public override void AssignParameters(SpawnerEntityMissionHelper _spawnerMissionHelper)
		{
			_spawnerMissionHelper.SpawnedEntity.GetFirstScriptOfType<Ballista>().AddOnDeployTag = this.AddOnDeployTag;
			_spawnerMissionHelper.SpawnedEntity.GetFirstScriptOfType<Ballista>().RemoveOnDeployTag = this.RemoveOnDeployTag;
			_spawnerMissionHelper.SpawnedEntity.GetFirstScriptOfType<Ballista>().HorizontalDirectionRestriction = this.DirectionRestrictionDegree * 0.017453292f;
		}

		[EditorVisibleScriptComponentVariable(false)]
		public MatrixFrame wait_pos_ground = MatrixFrame.Zero;

		[EditorVisibleScriptComponentVariable(true)]
		public string AddOnDeployTag = "";

		[EditorVisibleScriptComponentVariable(true)]
		public string RemoveOnDeployTag = "";

		public bool ShowTrajectory;

		[EditorVisibleScriptComponentVariable(true)]
		public float DirectionRestrictionDegree = 90f;

		private Vec3 _projectileStartingPositionOffset = new Vec3(0f, -0.86f, 1.34f, -1f);
	}
}
