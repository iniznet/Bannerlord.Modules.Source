using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001FB RID: 507
	public class FormationDeploymentPlan : IFormationDeploymentPlan
	{
		// Token: 0x1700059A RID: 1434
		// (get) Token: 0x06001C2C RID: 7212 RVA: 0x000649C2 File Offset: 0x00062BC2
		public FormationClass Class
		{
			get
			{
				return this._class;
			}
		}

		// Token: 0x1700059B RID: 1435
		// (get) Token: 0x06001C2D RID: 7213 RVA: 0x000649CA File Offset: 0x00062BCA
		public FormationClass SpawnClass
		{
			get
			{
				return this._spawnClass;
			}
		}

		// Token: 0x1700059C RID: 1436
		// (get) Token: 0x06001C2E RID: 7214 RVA: 0x000649D2 File Offset: 0x00062BD2
		public float PlannedWidth
		{
			get
			{
				return this._plannedWidth;
			}
		}

		// Token: 0x1700059D RID: 1437
		// (get) Token: 0x06001C2F RID: 7215 RVA: 0x000649DA File Offset: 0x00062BDA
		public float PlannedDepth
		{
			get
			{
				return this._plannedDepth;
			}
		}

		// Token: 0x1700059E RID: 1438
		// (get) Token: 0x06001C30 RID: 7216 RVA: 0x000649E2 File Offset: 0x00062BE2
		public int PlannedTroopCount
		{
			get
			{
				return this._plannedFootTroopCount + this._plannedMountedTroopCount;
			}
		}

		// Token: 0x1700059F RID: 1439
		// (get) Token: 0x06001C31 RID: 7217 RVA: 0x000649F1 File Offset: 0x00062BF1
		public int PlannedFootTroopCount
		{
			get
			{
				return this._plannedFootTroopCount;
			}
		}

		// Token: 0x170005A0 RID: 1440
		// (get) Token: 0x06001C32 RID: 7218 RVA: 0x000649F9 File Offset: 0x00062BF9
		public int PlannedMountedTroopCount
		{
			get
			{
				return this._plannedMountedTroopCount;
			}
		}

		// Token: 0x170005A1 RID: 1441
		// (get) Token: 0x06001C33 RID: 7219 RVA: 0x00064A01 File Offset: 0x00062C01
		public bool HasDimensions
		{
			get
			{
				return this._plannedWidth >= 1E-05f && this._plannedDepth >= 1E-05f;
			}
		}

		// Token: 0x170005A2 RID: 1442
		// (get) Token: 0x06001C34 RID: 7220 RVA: 0x00064A22 File Offset: 0x00062C22
		public bool HasSignificantMountedTroops
		{
			get
			{
				return MissionDeploymentPlan.HasSignificantMountedTroops(this._plannedFootTroopCount, this._plannedMountedTroopCount);
			}
		}

		// Token: 0x06001C35 RID: 7221 RVA: 0x00064A35 File Offset: 0x00062C35
		public FormationDeploymentPlan(FormationClass fClass)
		{
			this._class = fClass;
			this._spawnClass = fClass;
			this.Clear();
		}

		// Token: 0x06001C36 RID: 7222 RVA: 0x00064A51 File Offset: 0x00062C51
		public bool HasFrame()
		{
			return this._spawnFrame.IsValid;
		}

		// Token: 0x06001C37 RID: 7223 RVA: 0x00064A60 File Offset: 0x00062C60
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

		// Token: 0x06001C38 RID: 7224 RVA: 0x00064AF6 File Offset: 0x00062CF6
		public FormationDeploymentOrder GetFlankDeploymentOrder(int offset = 0)
		{
			return FormationDeploymentOrder.GetDeploymentOrder(this._class, offset);
		}

		// Token: 0x06001C39 RID: 7225 RVA: 0x00064B04 File Offset: 0x00062D04
		public MatrixFrame GetGroundFrame()
		{
			return this._spawnFrame.ToGroundMatrixFrame();
		}

		// Token: 0x06001C3A RID: 7226 RVA: 0x00064B11 File Offset: 0x00062D11
		public Vec3 GetGroundPosition()
		{
			return this._spawnFrame.Origin.GetGroundVec3();
		}

		// Token: 0x06001C3B RID: 7227 RVA: 0x00064B24 File Offset: 0x00062D24
		public Vec2 GetDirection()
		{
			return this._spawnFrame.Rotation.f.AsVec2.Normalized();
		}

		// Token: 0x06001C3C RID: 7228 RVA: 0x00064B50 File Offset: 0x00062D50
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

		// Token: 0x06001C3D RID: 7229 RVA: 0x00064BBE File Offset: 0x00062DBE
		public void Clear()
		{
			this._plannedWidth = 0f;
			this._plannedDepth = 0f;
			this._plannedFootTroopCount = 0;
			this._plannedMountedTroopCount = 0;
			this._spawnFrame = WorldFrame.Invalid;
		}

		// Token: 0x06001C3E RID: 7230 RVA: 0x00064BEF File Offset: 0x00062DEF
		public void SetPlannedTroopCount(int footTroopCount, int mountedTroopCount)
		{
			this._plannedFootTroopCount = footTroopCount;
			this._plannedMountedTroopCount = mountedTroopCount;
		}

		// Token: 0x06001C3F RID: 7231 RVA: 0x00064BFF File Offset: 0x00062DFF
		public void SetPlannedDimensions(float width, float depth)
		{
			this._plannedWidth = MathF.Max(0f, width);
			this._plannedDepth = MathF.Max(0f, depth);
		}

		// Token: 0x06001C40 RID: 7232 RVA: 0x00064C23 File Offset: 0x00062E23
		public void SetFrame(WorldFrame frame)
		{
			this._spawnFrame = frame;
		}

		// Token: 0x06001C41 RID: 7233 RVA: 0x00064C2C File Offset: 0x00062E2C
		public void SetSpawnClass(FormationClass spawnClass)
		{
			this._spawnClass = spawnClass;
		}

		// Token: 0x04000930 RID: 2352
		private WorldFrame _spawnFrame;

		// Token: 0x04000931 RID: 2353
		private FormationClass _spawnClass;

		// Token: 0x04000932 RID: 2354
		private readonly FormationClass _class;

		// Token: 0x04000933 RID: 2355
		private float _plannedWidth;

		// Token: 0x04000934 RID: 2356
		private float _plannedDepth;

		// Token: 0x04000935 RID: 2357
		private int _plannedFootTroopCount;

		// Token: 0x04000936 RID: 2358
		private int _plannedMountedTroopCount;
	}
}
