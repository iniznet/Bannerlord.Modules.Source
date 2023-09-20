using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class FormationDeploymentPlan : IFormationDeploymentPlan
	{
		public FormationClass Class
		{
			get
			{
				return this._class;
			}
		}

		public FormationClass SpawnClass
		{
			get
			{
				return this._spawnClass;
			}
		}

		public float PlannedWidth
		{
			get
			{
				return this._plannedWidth;
			}
		}

		public float PlannedDepth
		{
			get
			{
				return this._plannedDepth;
			}
		}

		public int PlannedTroopCount
		{
			get
			{
				return this._plannedFootTroopCount + this._plannedMountedTroopCount;
			}
		}

		public int PlannedFootTroopCount
		{
			get
			{
				return this._plannedFootTroopCount;
			}
		}

		public int PlannedMountedTroopCount
		{
			get
			{
				return this._plannedMountedTroopCount;
			}
		}

		public bool HasDimensions
		{
			get
			{
				return this._plannedWidth >= 1E-05f && this._plannedDepth >= 1E-05f;
			}
		}

		public bool HasSignificantMountedTroops
		{
			get
			{
				return MissionDeploymentPlan.HasSignificantMountedTroops(this._plannedFootTroopCount, this._plannedMountedTroopCount);
			}
		}

		public FormationDeploymentPlan(FormationClass fClass)
		{
			this._class = fClass;
			this._spawnClass = fClass;
			this.Clear();
		}

		public bool HasFrame()
		{
			return this._spawnFrame.IsValid;
		}

		public FormationDeploymentFlank GetDefaultFlank(bool spawnWithHorses, int formationTroopCount, int infantryCount)
		{
			FormationDeploymentFlank formationDeploymentFlank;
			if (this._class.IsInfantry() && formationTroopCount == 0)
			{
				formationDeploymentFlank = FormationDeploymentFlank.Rear;
			}
			else if (this.HasSignificantMountedTroops && (!spawnWithHorses || infantryCount == 0))
			{
				if (formationTroopCount == 0 || this._class == FormationClass.LightCavalry || this._class == FormationClass.HorseArcher)
				{
					formationDeploymentFlank = FormationDeploymentFlank.Rear;
				}
				else
				{
					formationDeploymentFlank = FormationDeploymentFlank.Front;
				}
			}
			else
			{
				switch (this._class)
				{
				case FormationClass.Ranged:
				case FormationClass.NumberOfRegularFormations:
				case FormationClass.Bodyguard:
				case FormationClass.NumberOfAllFormations:
					return FormationDeploymentFlank.Rear;
				case FormationClass.Cavalry:
				case FormationClass.HeavyCavalry:
					return FormationDeploymentFlank.Left;
				case FormationClass.HorseArcher:
				case FormationClass.LightCavalry:
					return FormationDeploymentFlank.Right;
				}
				formationDeploymentFlank = FormationDeploymentFlank.Front;
			}
			return formationDeploymentFlank;
		}

		public FormationDeploymentOrder GetFlankDeploymentOrder(int offset = 0)
		{
			return FormationDeploymentOrder.GetDeploymentOrder(this._class, offset);
		}

		public MatrixFrame GetGroundFrame()
		{
			return this._spawnFrame.ToGroundMatrixFrame();
		}

		public Vec3 GetGroundPosition()
		{
			return this._spawnFrame.Origin.GetGroundVec3();
		}

		public Vec2 GetDirection()
		{
			return this._spawnFrame.Rotation.f.AsVec2.Normalized();
		}

		public WorldPosition CreateNewDeploymentWorldPosition(WorldPosition.WorldPositionEnforcedCache worldPositionEnforcedCache)
		{
			if (worldPositionEnforcedCache == WorldPosition.WorldPositionEnforcedCache.NavMeshVec3)
			{
				return new WorldPosition(Mission.Current.Scene, UIntPtr.Zero, this._spawnFrame.Origin.GetNavMeshVec3(), false);
			}
			if (worldPositionEnforcedCache != WorldPosition.WorldPositionEnforcedCache.GroundVec3)
			{
				return this._spawnFrame.Origin;
			}
			return new WorldPosition(Mission.Current.Scene, UIntPtr.Zero, this._spawnFrame.Origin.GetGroundVec3(), false);
		}

		public void Clear()
		{
			this._plannedWidth = 0f;
			this._plannedDepth = 0f;
			this._plannedFootTroopCount = 0;
			this._plannedMountedTroopCount = 0;
			this._spawnFrame = WorldFrame.Invalid;
		}

		public void SetPlannedTroopCount(int footTroopCount, int mountedTroopCount)
		{
			this._plannedFootTroopCount = footTroopCount;
			this._plannedMountedTroopCount = mountedTroopCount;
		}

		public void SetPlannedDimensions(float width, float depth)
		{
			this._plannedWidth = MathF.Max(0f, width);
			this._plannedDepth = MathF.Max(0f, depth);
		}

		public void SetFrame(WorldFrame frame)
		{
			this._spawnFrame = frame;
		}

		public void SetSpawnClass(FormationClass spawnClass)
		{
			this._spawnClass = spawnClass;
		}

		private WorldFrame _spawnFrame;

		private FormationClass _spawnClass;

		private readonly FormationClass _class;

		private float _plannedWidth;

		private float _plannedDepth;

		private int _plannedFootTroopCount;

		private int _plannedMountedTroopCount;
	}
}
