using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001F7 RID: 503
	public class DeploymentPlan
	{
		// Token: 0x1700058B RID: 1419
		// (get) Token: 0x06001BF2 RID: 7154 RVA: 0x00063548 File Offset: 0x00061748
		public bool SpawnWithHorses
		{
			get
			{
				return this._spawnWithHorses;
			}
		}

		// Token: 0x1700058C RID: 1420
		// (get) Token: 0x06001BF3 RID: 7155 RVA: 0x00063550 File Offset: 0x00061750
		public int PlanCount
		{
			get
			{
				return this._planCount;
			}
		}

		// Token: 0x1700058D RID: 1421
		// (get) Token: 0x06001BF4 RID: 7156 RVA: 0x00063558 File Offset: 0x00061758
		// (set) Token: 0x06001BF5 RID: 7157 RVA: 0x00063560 File Offset: 0x00061760
		public bool IsPlanMade { get; private set; }

		// Token: 0x1700058E RID: 1422
		// (get) Token: 0x06001BF6 RID: 7158 RVA: 0x00063569 File Offset: 0x00061769
		// (set) Token: 0x06001BF7 RID: 7159 RVA: 0x00063571 File Offset: 0x00061771
		public float SpawnPathOffset { get; private set; }

		// Token: 0x1700058F RID: 1423
		// (get) Token: 0x06001BF8 RID: 7160 RVA: 0x0006357A File Offset: 0x0006177A
		// (set) Token: 0x06001BF9 RID: 7161 RVA: 0x00063582 File Offset: 0x00061782
		public bool HasDeploymentBoundaries { get; private set; }

		// Token: 0x17000590 RID: 1424
		// (get) Token: 0x06001BFA RID: 7162 RVA: 0x0006358B File Offset: 0x0006178B
		public bool IsSafeToDeploy
		{
			get
			{
				return this.SafetyScore >= 50f;
			}
		}

		// Token: 0x17000591 RID: 1425
		// (get) Token: 0x06001BFB RID: 7163 RVA: 0x0006359D File Offset: 0x0006179D
		// (set) Token: 0x06001BFC RID: 7164 RVA: 0x000635A5 File Offset: 0x000617A5
		public float SafetyScore { get; private set; }

		// Token: 0x17000592 RID: 1426
		// (get) Token: 0x06001BFD RID: 7165 RVA: 0x000635B0 File Offset: 0x000617B0
		public int FootTroopCount
		{
			get
			{
				int num = 0;
				for (int i = 0; i < 11; i++)
				{
					num += this._formationFootTroopCounts[i];
				}
				return num;
			}
		}

		// Token: 0x17000593 RID: 1427
		// (get) Token: 0x06001BFE RID: 7166 RVA: 0x000635D8 File Offset: 0x000617D8
		public int MountedTroopCount
		{
			get
			{
				int num = 0;
				for (int i = 0; i < 11; i++)
				{
					num += this._formationMountedTroopCounts[i];
				}
				return num;
			}
		}

		// Token: 0x17000594 RID: 1428
		// (get) Token: 0x06001BFF RID: 7167 RVA: 0x00063600 File Offset: 0x00061800
		public int TroopCount
		{
			get
			{
				int num = 0;
				for (int i = 0; i < 11; i++)
				{
					num += this._formationFootTroopCounts[i] + this._formationMountedTroopCounts[i];
				}
				return num;
			}
		}

		// Token: 0x17000595 RID: 1429
		// (get) Token: 0x06001C00 RID: 7168 RVA: 0x00063631 File Offset: 0x00061831
		public MatrixFrame DeploymentFrame
		{
			get
			{
				return this._deploymentFrame;
			}
		}

		// Token: 0x17000596 RID: 1430
		// (get) Token: 0x06001C01 RID: 7169 RVA: 0x00063639 File Offset: 0x00061839
		public float DeploymentWidth
		{
			get
			{
				return this._deploymentWidth;
			}
		}

		// Token: 0x17000597 RID: 1431
		// (get) Token: 0x06001C02 RID: 7170 RVA: 0x00063641 File Offset: 0x00061841
		public MBReadOnlyDictionary<string, List<Vec2>> DeploymentBoundaries
		{
			get
			{
				return this._deploymentBoundaries.GetReadOnlyDictionary<string, List<Vec2>>();
			}
		}

		// Token: 0x06001C03 RID: 7171 RVA: 0x0006364E File Offset: 0x0006184E
		public static DeploymentPlan CreateInitialPlan(Mission mission, BattleSideEnum side)
		{
			return new DeploymentPlan(mission, side, DeploymentPlanType.Initial, SpawnPathData.Invalid);
		}

		// Token: 0x06001C04 RID: 7172 RVA: 0x0006365D File Offset: 0x0006185D
		public static DeploymentPlan CreateReinforcementPlan(Mission mission, BattleSideEnum side)
		{
			return new DeploymentPlan(mission, side, DeploymentPlanType.Reinforcement, SpawnPathData.Invalid);
		}

		// Token: 0x06001C05 RID: 7173 RVA: 0x0006366C File Offset: 0x0006186C
		public static DeploymentPlan CreateReinforcementPlanWithSpawnPath(Mission mission, BattleSideEnum side, SpawnPathData spawnPathData)
		{
			return new DeploymentPlan(mission, side, DeploymentPlanType.Reinforcement, spawnPathData);
		}

		// Token: 0x06001C06 RID: 7174 RVA: 0x00063678 File Offset: 0x00061878
		private DeploymentPlan(Mission mission, BattleSideEnum side, DeploymentPlanType type, SpawnPathData spawnPathData)
		{
			this._mission = mission;
			this._planCount = 0;
			this.Side = side;
			this.Type = type;
			this.SpawnPathData = spawnPathData;
			this._formationPlans = new FormationDeploymentPlan[11];
			this._formationFootTroopCounts = new int[11];
			this._formationMountedTroopCounts = new int[11];
			this._deploymentFrame = MatrixFrame.Identity;
			this._deploymentWidth = 0f;
			this._deploymentBoundaries = new Dictionary<string, List<Vec2>>();
			this.IsPlanMade = false;
			this.SpawnPathOffset = 0f;
			this.SafetyScore = 100f;
			for (int i = 0; i < this._formationPlans.Length; i++)
			{
				FormationClass formationClass = (FormationClass)i;
				this._formationPlans[i] = new FormationDeploymentPlan(formationClass);
			}
			for (int j = 0; j < 4; j++)
			{
				this._deploymentFlanks[j] = new SortedList<FormationDeploymentOrder, FormationDeploymentPlan>(FormationDeploymentOrder.GetComparer());
			}
			this.ClearAddedTroops();
			this.ClearPlan();
		}

		// Token: 0x06001C07 RID: 7175 RVA: 0x0006376D File Offset: 0x0006196D
		public void SetSpawnWithHorses(bool value)
		{
			this._spawnWithHorses = value;
		}

		// Token: 0x06001C08 RID: 7176 RVA: 0x00063778 File Offset: 0x00061978
		public void ClearAddedTroops()
		{
			for (int i = 0; i < 11; i++)
			{
				this._formationFootTroopCounts[i] = 0;
				this._formationMountedTroopCounts[i] = 0;
			}
		}

		// Token: 0x06001C09 RID: 7177 RVA: 0x000637A4 File Offset: 0x000619A4
		public void ClearPlan()
		{
			FormationDeploymentPlan[] formationPlans = this._formationPlans;
			for (int i = 0; i < formationPlans.Length; i++)
			{
				formationPlans[i].Clear();
			}
			SortedList<FormationDeploymentOrder, FormationDeploymentPlan>[] deploymentFlanks = this._deploymentFlanks;
			for (int i = 0; i < deploymentFlanks.Length; i++)
			{
				deploymentFlanks[i].Clear();
			}
			this.IsPlanMade = false;
			this._deploymentBoundaries.Clear();
			this.HasDeploymentBoundaries = false;
		}

		// Token: 0x06001C0A RID: 7178 RVA: 0x00063804 File Offset: 0x00061A04
		public void AddTroops(FormationClass formationClass, int footTroopCount, int mountedTroopCount)
		{
			if (footTroopCount + mountedTroopCount > 0 && formationClass < (FormationClass)11)
			{
				this._formationFootTroopCounts[(int)formationClass] += footTroopCount;
				this._formationMountedTroopCounts[(int)formationClass] += mountedTroopCount;
			}
		}

		// Token: 0x06001C0B RID: 7179 RVA: 0x00063840 File Offset: 0x00061A40
		public void PlanBattleDeployment(FormationSceneSpawnEntry[,] formationSceneSpawnEntries, float spawnPathOffset = 0f)
		{
			this.SpawnPathOffset = spawnPathOffset;
			bool hasSpawnPath = this._mission.HasSpawnPath;
			bool isFieldBattle = this._mission.IsFieldBattle;
			this.PlanFormationDimensions();
			if (hasSpawnPath)
			{
				this.PlanFieldBattleDeploymentFromSpawnPath(spawnPathOffset);
			}
			else if (isFieldBattle)
			{
				this.PlanFieldBattleDeploymentFromSceneData(formationSceneSpawnEntries);
			}
			else
			{
				this.PlanBattleDeploymentFromSceneData(formationSceneSpawnEntries);
			}
			if (hasSpawnPath || isFieldBattle)
			{
				this.PlanDeploymentZone();
				return;
			}
			this.GetFormationDeploymentFrame(FormationClass.Infantry, out this._deploymentFrame);
			this._deploymentWidth = 0f;
			this._deploymentBoundaries.Clear();
			this.HasDeploymentBoundaries = false;
		}

		// Token: 0x06001C0C RID: 7180 RVA: 0x000638C6 File Offset: 0x00061AC6
		public FormationDeploymentPlan GetFormationPlan(FormationClass fClass)
		{
			return this._formationPlans[(int)fClass];
		}

		// Token: 0x06001C0D RID: 7181 RVA: 0x000638D0 File Offset: 0x00061AD0
		public bool GetFormationDeploymentFrame(FormationClass fClass, out MatrixFrame frame)
		{
			FormationDeploymentPlan formationPlan = this.GetFormationPlan(fClass);
			if (formationPlan.HasFrame())
			{
				frame = formationPlan.GetGroundFrame();
				return true;
			}
			frame = MatrixFrame.Identity;
			return false;
		}

		// Token: 0x06001C0E RID: 7182 RVA: 0x00063908 File Offset: 0x00061B08
		public bool IsPositionInsideDeploymentBoundaries(in Vec2 position)
		{
			bool flag = false;
			foreach (KeyValuePair<string, List<Vec2>> keyValuePair in this._deploymentBoundaries)
			{
				List<Vec2> value = keyValuePair.Value;
				if (MBSceneUtilities.IsPointInsideBoundaries(position, value, 0.01f))
				{
					flag = true;
					break;
				}
			}
			return flag;
		}

		// Token: 0x06001C0F RID: 7183 RVA: 0x00063974 File Offset: 0x00061B74
		public bool IsPlanSuitableForFormations(ValueTuple<int, int>[] troopDataPerFormationClass)
		{
			if (troopDataPerFormationClass.Length == 11)
			{
				for (int i = 0; i < 11; i++)
				{
					FormationClass formationClass = (FormationClass)i;
					FormationDeploymentPlan formationPlan = this.GetFormationPlan(formationClass);
					ValueTuple<int, int> valueTuple = troopDataPerFormationClass[i];
					if (formationPlan.PlannedFootTroopCount != valueTuple.Item1 || formationPlan.PlannedMountedTroopCount != valueTuple.Item2)
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		// Token: 0x06001C10 RID: 7184 RVA: 0x000639C8 File Offset: 0x00061BC8
		public Vec2 GetClosestBoundaryPosition(in Vec2 position)
		{
			Vec2 vec = position;
			float num = float.MaxValue;
			foreach (KeyValuePair<string, List<Vec2>> keyValuePair in this._deploymentBoundaries)
			{
				List<Vec2> value = keyValuePair.Value;
				if (value.Count > 2)
				{
					Vec2 vec2;
					float num2 = MBSceneUtilities.FindClosestPointToBoundaries(position, value, out vec2);
					if (num2 < num)
					{
						num = num2;
						vec = vec2;
					}
				}
			}
			return vec;
		}

		// Token: 0x06001C11 RID: 7185 RVA: 0x00063A4C File Offset: 0x00061C4C
		public void UpdateSafetyScore()
		{
			if (this._mission.Teams == null)
			{
				return;
			}
			float num = 100f;
			Vec2 asVec = this._deploymentFrame.origin.AsVec2;
			Team team = ((this.Side == BattleSideEnum.Attacker) ? this._mission.Teams.Defender : ((this.Side == BattleSideEnum.Defender) ? this._mission.Teams.Attacker : null));
			if (team != null)
			{
				foreach (Formation formation in team.FormationsIncludingEmpty)
				{
					if (formation.CountOfUnits > 0)
					{
						float num2 = asVec.Distance(formation.QuerySystem.AveragePosition);
						if (num >= num2)
						{
							num = num2;
						}
					}
				}
			}
			team = ((this.Side == BattleSideEnum.Attacker) ? this._mission.Teams.DefenderAlly : ((this.Side == BattleSideEnum.Defender) ? this._mission.Teams.AttackerAlly : null));
			if (team != null)
			{
				foreach (Formation formation2 in team.FormationsIncludingEmpty)
				{
					if (formation2.CountOfUnits > 0)
					{
						float num3 = asVec.Distance(formation2.QuerySystem.AveragePosition);
						if (num >= num3)
						{
							num = num3;
						}
					}
				}
			}
			this.SafetyScore = num;
		}

		// Token: 0x06001C12 RID: 7186 RVA: 0x00063BC4 File Offset: 0x00061DC4
		public WorldFrame GetFrameFromFormationSpawnEntity(GameEntity formationSpawnEntity, float depthOffset = 0f)
		{
			MatrixFrame globalFrame = formationSpawnEntity.GetGlobalFrame();
			globalFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
			WorldPosition worldPosition = new WorldPosition(this._mission.Scene, UIntPtr.Zero, globalFrame.origin, false);
			WorldPosition worldPosition2 = worldPosition;
			if (depthOffset != 0f)
			{
				worldPosition2.SetVec2(worldPosition2.AsVec2 - depthOffset * globalFrame.rotation.f.AsVec2);
				if (!worldPosition2.IsValid || worldPosition2.GetNavMesh() == UIntPtr.Zero)
				{
					worldPosition2 = worldPosition;
				}
			}
			return new WorldFrame(globalFrame.rotation, worldPosition2);
		}

		// Token: 0x06001C13 RID: 7187 RVA: 0x00063C60 File Offset: 0x00061E60
		public ValueTuple<float, float> GetFormationSpawnWidthAndDepth(FormationClass formationNo, int troopCount, bool hasMountedTroops, bool considerCavalryAsInfantry = false)
		{
			bool flag = !considerCavalryAsInfantry && hasMountedTroops;
			float defaultUnitDiameter = Formation.GetDefaultUnitDiameter(flag);
			int unitSpacingOf = ArrangementOrder.GetUnitSpacingOf(ArrangementOrder.ArrangementOrderEnum.Line);
			float num = (flag ? Formation.CavalryInterval(unitSpacingOf) : Formation.InfantryInterval(unitSpacingOf));
			float num2 = (flag ? Formation.CavalryDistance(unitSpacingOf) : Formation.InfantryDistance(unitSpacingOf));
			float num3 = (float)MathF.Max(0, troopCount - 1) * (num + defaultUnitDiameter) + defaultUnitDiameter;
			float num4 = (flag ? 18f : 9f);
			int num5 = (int)(num3 / MathF.Sqrt(num4 * (float)troopCount + 1f));
			num5 = MathF.Max(1, num5);
			float num6 = (float)troopCount / (float)num5;
			float num7 = MathF.Max(0f, num6 - 1f) * (num + defaultUnitDiameter) + defaultUnitDiameter;
			float num8 = (float)(num5 - 1) * (num2 + defaultUnitDiameter) + defaultUnitDiameter;
			return new ValueTuple<float, float>(num7, num8);
		}

		// Token: 0x06001C14 RID: 7188 RVA: 0x00063D1C File Offset: 0x00061F1C
		private void PlanFieldBattleDeploymentFromSpawnPath(float pathOffset = 0f)
		{
			for (int i = 0; i < this._formationPlans.Length; i++)
			{
				int num = this._formationFootTroopCounts[i] + this._formationMountedTroopCounts[i];
				FormationDeploymentPlan formationDeploymentPlan = this._formationPlans[i];
				FormationDeploymentFlank defaultFlank = formationDeploymentPlan.GetDefaultFlank(this._spawnWithHorses, num, this.FootTroopCount);
				FormationClass formationClass = (FormationClass)i;
				int num2 = ((num > 0 || formationClass == FormationClass.NumberOfRegularFormations) ? 0 : 1);
				FormationDeploymentOrder flankDeploymentOrder = formationDeploymentPlan.GetFlankDeploymentOrder(num2);
				this._deploymentFlanks[(int)defaultFlank].Add(flankDeploymentOrder, formationDeploymentPlan);
			}
			float num3 = this.ComputeHorizontalCenterOffset();
			SpawnPathData spawnPathData;
			if (this.Type == DeploymentPlanType.Initial)
			{
				spawnPathData = this._mission.GetInitialSpawnPathDataOfSide(this.Side);
			}
			else
			{
				spawnPathData = this.SpawnPathData;
			}
			Vec2 vec;
			Vec2 vec2;
			spawnPathData.GetOrientedSpawnPathPosition(out vec, out vec2, pathOffset);
			this.DeployFlanks(vec, vec2, num3);
			SortedList<FormationDeploymentOrder, FormationDeploymentPlan>[] deploymentFlanks = this._deploymentFlanks;
			for (int j = 0; j < deploymentFlanks.Length; j++)
			{
				deploymentFlanks[j].Clear();
			}
			this.IsPlanMade = true;
			this._planCount++;
		}

		// Token: 0x06001C15 RID: 7189 RVA: 0x00063E24 File Offset: 0x00062024
		private void PlanFieldBattleDeploymentFromSceneData(FormationSceneSpawnEntry[,] formationSceneSpawnEntries)
		{
			if (formationSceneSpawnEntries == null || formationSceneSpawnEntries.GetLength(0) != 2 || formationSceneSpawnEntries.GetLength(1) != this._formationPlans.Length)
			{
				return;
			}
			int side = (int)this.Side;
			int num = ((this.Side == BattleSideEnum.Attacker) ? 0 : 1);
			Dictionary<GameEntity, float> dictionary = new Dictionary<GameEntity, float>();
			bool flag = this.Type == DeploymentPlanType.Initial;
			for (int i = 0; i < this._formationPlans.Length; i++)
			{
				FormationDeploymentPlan formationDeploymentPlan = this._formationPlans[i];
				FormationSceneSpawnEntry formationSceneSpawnEntry = formationSceneSpawnEntries[side, i];
				FormationSceneSpawnEntry formationSceneSpawnEntry2 = formationSceneSpawnEntries[num, i];
				GameEntity gameEntity = (flag ? formationSceneSpawnEntry.SpawnEntity : formationSceneSpawnEntry.ReinforcementSpawnEntity);
				GameEntity gameEntity2 = (flag ? formationSceneSpawnEntry2.SpawnEntity : formationSceneSpawnEntry2.ReinforcementSpawnEntity);
				if (gameEntity != null && gameEntity2 != null)
				{
					WorldFrame worldFrame = this.ComputeFieldBattleDeploymentFrameForFormation(formationDeploymentPlan, gameEntity, gameEntity2, ref dictionary);
					formationDeploymentPlan.SetFrame(worldFrame);
				}
				else
				{
					formationDeploymentPlan.SetFrame(WorldFrame.Invalid);
				}
				formationDeploymentPlan.SetSpawnClass(formationSceneSpawnEntry.FormationClass);
			}
			this.IsPlanMade = true;
			this._planCount++;
		}

		// Token: 0x06001C16 RID: 7190 RVA: 0x00063F40 File Offset: 0x00062140
		private void PlanBattleDeploymentFromSceneData(FormationSceneSpawnEntry[,] formationSceneSpawnEntries)
		{
			if (formationSceneSpawnEntries == null || formationSceneSpawnEntries.GetLength(0) != 2 || formationSceneSpawnEntries.GetLength(1) != this._formationPlans.Length)
			{
				return;
			}
			int side = (int)this.Side;
			Dictionary<GameEntity, float> dictionary = new Dictionary<GameEntity, float>();
			bool flag = this.Type == DeploymentPlanType.Initial;
			for (int i = 0; i < this._formationPlans.Length; i++)
			{
				FormationDeploymentPlan formationDeploymentPlan = this._formationPlans[i];
				FormationSceneSpawnEntry formationSceneSpawnEntry = formationSceneSpawnEntries[side, i];
				GameEntity gameEntity = (flag ? formationSceneSpawnEntry.SpawnEntity : formationSceneSpawnEntry.ReinforcementSpawnEntity);
				if (gameEntity != null)
				{
					float andUpdateSpawnDepth = this.GetAndUpdateSpawnDepth(ref dictionary, gameEntity, formationDeploymentPlan);
					formationDeploymentPlan.SetFrame(this.GetFrameFromFormationSpawnEntity(gameEntity, andUpdateSpawnDepth));
				}
				else
				{
					formationDeploymentPlan.SetFrame(WorldFrame.Invalid);
				}
				formationDeploymentPlan.SetSpawnClass(formationSceneSpawnEntry.FormationClass);
			}
			this.IsPlanMade = true;
			this._planCount++;
		}

		// Token: 0x06001C17 RID: 7191 RVA: 0x0006401C File Offset: 0x0006221C
		private void PlanFormationDimensions()
		{
			for (int i = 0; i < this._formationPlans.Length; i++)
			{
				int num = this._formationFootTroopCounts[i];
				int num2 = this._formationMountedTroopCounts[i];
				int num3 = num + num2;
				if (num3 > 0)
				{
					FormationDeploymentPlan formationDeploymentPlan = this._formationPlans[i];
					bool flag = MissionDeploymentPlan.HasSignificantMountedTroops(num, num2);
					ValueTuple<float, float> formationSpawnWidthAndDepth = this.GetFormationSpawnWidthAndDepth(formationDeploymentPlan.Class, num3, flag, !this._spawnWithHorses);
					float item = formationSpawnWidthAndDepth.Item1;
					float item2 = formationSpawnWidthAndDepth.Item2;
					formationDeploymentPlan.SetPlannedDimensions(item, item2);
					formationDeploymentPlan.SetPlannedTroopCount(num, num2);
				}
			}
		}

		// Token: 0x06001C18 RID: 7192 RVA: 0x000640A8 File Offset: 0x000622A8
		private void PlanDeploymentZone()
		{
			this.GetFormationDeploymentFrame(FormationClass.Infantry, out this._deploymentFrame);
			float num = 0f;
			float num2 = 0f;
			for (int i = 0; i < 10; i++)
			{
				FormationClass formationClass = (FormationClass)i;
				FormationDeploymentPlan formationPlan = this.GetFormationPlan(formationClass);
				if (formationPlan.HasFrame())
				{
					MatrixFrame matrixFrame = this._deploymentFrame.TransformToLocal(formationPlan.GetGroundFrame());
					num = Math.Max(matrixFrame.origin.y, num);
					num2 = Math.Max(Math.Abs(matrixFrame.origin.x), num2);
				}
			}
			num += 10f;
			this._deploymentFrame.Advance(num);
			this._deploymentBoundaries.Clear();
			float num3 = 2f * num2 + 1.5f * (float)this.TroopCount;
			this._deploymentWidth = Math.Max(num3, 100f);
			foreach (KeyValuePair<string, ICollection<Vec2>> keyValuePair in this._mission.Boundaries)
			{
				string key = keyValuePair.Key;
				ICollection<Vec2> value = keyValuePair.Value;
				List<Vec2> list = this.ComputeDeploymentBoundariesFromMissionBoundaries(value);
				this._deploymentBoundaries.Add(key, list);
			}
			this.HasDeploymentBoundaries = true;
		}

		// Token: 0x06001C19 RID: 7193 RVA: 0x000641EC File Offset: 0x000623EC
		private void DeployFlanks(Vec2 deployPosition, Vec2 deployDirection, float horizontalCenterOffset)
		{
			ValueTuple<float, float> valueTuple = this.PlanFlankDeployment(FormationDeploymentFlank.Front, deployPosition, deployDirection, 0f, horizontalCenterOffset);
			float item = valueTuple.Item1;
			float num = valueTuple.Item2;
			num += 3f;
			float item2 = this.PlanFlankDeployment(FormationDeploymentFlank.Rear, deployPosition, deployDirection, num, horizontalCenterOffset).Item1;
			float num2 = MathF.Max(item, item2);
			float num3 = this.ComputeFlankDepth(FormationDeploymentFlank.Front, true);
			num3 += 3f;
			float num4 = this.ComputeFlankWidth(FormationDeploymentFlank.Left);
			float num5 = horizontalCenterOffset + 2f + 0.5f * (num2 + num4);
			this.PlanFlankDeployment(FormationDeploymentFlank.Left, deployPosition, deployDirection, num3, num5);
			float num6 = this.ComputeFlankWidth(FormationDeploymentFlank.Right);
			float num7 = horizontalCenterOffset - (2f + 0.5f * (num2 + num6));
			this.PlanFlankDeployment(FormationDeploymentFlank.Right, deployPosition, deployDirection, num3, num7);
		}

		// Token: 0x06001C1A RID: 7194 RVA: 0x000642A8 File Offset: 0x000624A8
		[return: TupleElementNames(new string[] { "flankWidth", "flankDepth" })]
		private ValueTuple<float, float> PlanFlankDeployment(FormationDeploymentFlank flankFlank, Vec2 deployPosition, Vec2 deployDirection, float verticalOffset = 0f, float horizontalOffset = 0f)
		{
			Mat3 identity = Mat3.Identity;
			identity.RotateAboutUp(deployDirection.RotationInRadians);
			float num = 0f;
			float num2 = 0f;
			Vec2 vec = deployDirection.LeftVec();
			WorldPosition worldPosition = new WorldPosition(this._mission.Scene, UIntPtr.Zero, deployPosition.ToVec3(0f), false);
			foreach (KeyValuePair<FormationDeploymentOrder, FormationDeploymentPlan> keyValuePair in this._deploymentFlanks[(int)flankFlank])
			{
				FormationDeploymentPlan value = keyValuePair.Value;
				Vec2 vec2 = worldPosition.AsVec2 - (num2 + verticalOffset) * deployDirection + horizontalOffset * vec;
				Vec3 lastPointOnNavigationMeshFromWorldPositionToDestination = this._mission.Scene.GetLastPointOnNavigationMeshFromWorldPositionToDestination(ref worldPosition, vec2);
				WorldPosition worldPosition2 = new WorldPosition(this._mission.Scene, UIntPtr.Zero, lastPointOnNavigationMeshFromWorldPositionToDestination, false);
				WorldFrame worldFrame = new WorldFrame(identity, worldPosition2);
				value.SetFrame(worldFrame);
				float num3 = value.PlannedDepth + 3f;
				num2 += num3;
				num = MathF.Max(num, value.PlannedWidth);
			}
			num2 = MathF.Max(num2 - 3f, 0f);
			return new ValueTuple<float, float>(num, num2);
		}

		// Token: 0x06001C1B RID: 7195 RVA: 0x000643F4 File Offset: 0x000625F4
		private WorldFrame ComputeFieldBattleDeploymentFrameForFormation(FormationDeploymentPlan formationPlan, GameEntity formationSceneEntity, GameEntity counterSideFormationSceneEntity, ref Dictionary<GameEntity, float> spawnDepths)
		{
			Vec3 globalPosition = formationSceneEntity.GlobalPosition;
			Vec2 asVec = (counterSideFormationSceneEntity.GlobalPosition - globalPosition).AsVec2;
			asVec.Normalize();
			float andUpdateSpawnDepth = this.GetAndUpdateSpawnDepth(ref spawnDepths, formationSceneEntity, formationPlan);
			WorldPosition worldPosition = new WorldPosition(this._mission.Scene, UIntPtr.Zero, globalPosition, false);
			worldPosition.SetVec2(worldPosition.AsVec2 - andUpdateSpawnDepth * asVec);
			Mat3 identity = Mat3.Identity;
			identity.RotateAboutUp(asVec.RotationInRadians);
			return new WorldFrame(identity, worldPosition);
		}

		// Token: 0x06001C1C RID: 7196 RVA: 0x00064484 File Offset: 0x00062684
		private float ComputeFlankWidth(FormationDeploymentFlank flank)
		{
			float num = 0f;
			foreach (KeyValuePair<FormationDeploymentOrder, FormationDeploymentPlan> keyValuePair in this._deploymentFlanks[(int)flank])
			{
				num = MathF.Max(num, keyValuePair.Value.PlannedWidth);
			}
			return num;
		}

		// Token: 0x06001C1D RID: 7197 RVA: 0x000644E8 File Offset: 0x000626E8
		private float ComputeFlankDepth(FormationDeploymentFlank flank, bool countPositiveNumTroops = false)
		{
			float num = 0f;
			foreach (KeyValuePair<FormationDeploymentOrder, FormationDeploymentPlan> keyValuePair in this._deploymentFlanks[(int)flank])
			{
				if (!countPositiveNumTroops)
				{
					num += keyValuePair.Value.PlannedDepth + 3f;
				}
				else if (keyValuePair.Value.PlannedTroopCount > 0)
				{
					num += keyValuePair.Value.PlannedDepth + 3f;
				}
			}
			num -= 3f;
			return num;
		}

		// Token: 0x06001C1E RID: 7198 RVA: 0x00064580 File Offset: 0x00062780
		private float ComputeHorizontalCenterOffset()
		{
			float num = MathF.Max(this.ComputeFlankWidth(FormationDeploymentFlank.Front), this.ComputeFlankWidth(FormationDeploymentFlank.Rear));
			float num2 = this.ComputeFlankWidth(FormationDeploymentFlank.Left);
			float num3 = this.ComputeFlankWidth(FormationDeploymentFlank.Right);
			float num4 = num / 2f + num2 + 2f;
			return (num / 2f + num3 + 2f - num4) / 2f;
		}

		// Token: 0x06001C1F RID: 7199 RVA: 0x000645D8 File Offset: 0x000627D8
		private float GetAndUpdateSpawnDepth(ref Dictionary<GameEntity, float> spawnDepths, GameEntity spawnEntity, FormationDeploymentPlan formationPlan)
		{
			float num;
			bool flag = spawnDepths.TryGetValue(spawnEntity, out num);
			float num2 = (formationPlan.HasDimensions ? (formationPlan.PlannedDepth + 3f) : 0f);
			if (!flag)
			{
				num = 0f;
				spawnDepths[spawnEntity] = num2;
			}
			else if (formationPlan.HasDimensions)
			{
				spawnDepths[spawnEntity] = num + num2;
			}
			return num;
		}

		// Token: 0x06001C20 RID: 7200 RVA: 0x00064634 File Offset: 0x00062834
		private List<Vec2> ComputeDeploymentBoundariesFromMissionBoundaries(ICollection<Vec2> missionBoundaries)
		{
			List<Vec2> list = new List<Vec2>();
			float num = this._deploymentWidth / 2f;
			if (missionBoundaries.Count > 2)
			{
				Vec2 asVec = this._deploymentFrame.origin.AsVec2;
				Vec2 vec = this._deploymentFrame.rotation.s.AsVec2.Normalized();
				Vec2 vec2 = this._deploymentFrame.rotation.f.AsVec2.Normalized();
				List<Vec2> list2 = missionBoundaries.ToList<Vec2>();
				List<ValueTuple<Vec2, Vec2>> list3 = new List<ValueTuple<Vec2, Vec2>>();
				Vec2 vec3 = this.ClampRayToMissionBoundaries(list2, asVec, vec, num);
				this.AddDeploymentBoundaryPoint(list, vec3);
				Vec2 vec4 = this.ClampRayToMissionBoundaries(list2, asVec, -vec, num);
				this.AddDeploymentBoundaryPoint(list, vec4);
				Vec2 vec5;
				if (MBMath.IntersectRayWithBoundaryList(vec3, -vec2, list2, out vec5) && (vec5 - vec3).Length > 0.1f)
				{
					list3.Add(new ValueTuple<Vec2, Vec2>(vec5, vec3));
					this.AddDeploymentBoundaryPoint(list, vec5);
				}
				list3.Add(new ValueTuple<Vec2, Vec2>(vec3, vec4));
				Vec2 vec6;
				if (MBMath.IntersectRayWithBoundaryList(vec4, -vec2, list2, out vec6) && (vec6 - vec4).Length > 0.1f)
				{
					list3.Add(new ValueTuple<Vec2, Vec2>(vec4, vec6));
					this.AddDeploymentBoundaryPoint(list, vec6);
				}
				foreach (Vec2 vec7 in missionBoundaries)
				{
					bool flag = true;
					foreach (ValueTuple<Vec2, Vec2> valueTuple in list3)
					{
						Vec2 vec8 = vec7 - valueTuple.Item1;
						Vec2 vec9 = valueTuple.Item2 - valueTuple.Item1;
						if (vec9.x * vec8.y - vec9.y * vec8.x <= 0f)
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						this.AddDeploymentBoundaryPoint(list, vec7);
					}
				}
				MBSceneUtilities.RadialSortBoundaries(ref list);
			}
			return list;
		}

		// Token: 0x06001C21 RID: 7201 RVA: 0x00064870 File Offset: 0x00062A70
		private void AddDeploymentBoundaryPoint(List<Vec2> deploymentBoundaries, Vec2 point)
		{
			if (!deploymentBoundaries.Exists((Vec2 boundaryPoint) => boundaryPoint.Distance(point) <= 0.1f))
			{
				deploymentBoundaries.Add(point);
			}
		}

		// Token: 0x06001C22 RID: 7202 RVA: 0x000648AC File Offset: 0x00062AAC
		private Vec2 ClampRayToMissionBoundaries(List<Vec2> boundaries, Vec2 origin, Vec2 direction, float maxLength)
		{
			if (this._mission.IsPositionInsideBoundaries(origin))
			{
				Vec2 vec = origin + direction * maxLength;
				if (this._mission.IsPositionInsideBoundaries(vec))
				{
					return vec;
				}
			}
			Vec2 vec2;
			if (MBMath.IntersectRayWithBoundaryList(origin, direction, boundaries, out vec2))
			{
				return vec2;
			}
			return origin;
		}

		// Token: 0x0400090D RID: 2317
		public const float VerticalFormationGap = 3f;

		// Token: 0x0400090E RID: 2318
		public const float HorizontalFormationGap = 2f;

		// Token: 0x0400090F RID: 2319
		public const float DeployZoneMinimumWidth = 100f;

		// Token: 0x04000910 RID: 2320
		public const float DeployZoneForwardMargin = 10f;

		// Token: 0x04000911 RID: 2321
		public const float DeployZoneExtraWidthPerTroop = 1.5f;

		// Token: 0x04000912 RID: 2322
		public const float MaxSafetyScore = 100f;

		// Token: 0x04000913 RID: 2323
		public readonly BattleSideEnum Side;

		// Token: 0x04000914 RID: 2324
		public readonly DeploymentPlanType Type;

		// Token: 0x04000915 RID: 2325
		public readonly SpawnPathData SpawnPathData;

		// Token: 0x0400091A RID: 2330
		private readonly Mission _mission;

		// Token: 0x0400091B RID: 2331
		private int _planCount;

		// Token: 0x0400091C RID: 2332
		private bool _spawnWithHorses;

		// Token: 0x0400091D RID: 2333
		private readonly int[] _formationMountedTroopCounts;

		// Token: 0x0400091E RID: 2334
		private readonly int[] _formationFootTroopCounts;

		// Token: 0x0400091F RID: 2335
		private readonly FormationDeploymentPlan[] _formationPlans;

		// Token: 0x04000920 RID: 2336
		private MatrixFrame _deploymentFrame;

		// Token: 0x04000921 RID: 2337
		private float _deploymentWidth;

		// Token: 0x04000922 RID: 2338
		private readonly Dictionary<string, List<Vec2>> _deploymentBoundaries;

		// Token: 0x04000923 RID: 2339
		private readonly SortedList<FormationDeploymentOrder, FormationDeploymentPlan>[] _deploymentFlanks = new SortedList<FormationDeploymentOrder, FormationDeploymentPlan>[4];
	}
}
