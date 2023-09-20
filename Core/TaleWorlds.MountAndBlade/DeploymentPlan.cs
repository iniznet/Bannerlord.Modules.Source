using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class DeploymentPlan
	{
		public bool SpawnWithHorses
		{
			get
			{
				return this._spawnWithHorses;
			}
		}

		public int PlanCount
		{
			get
			{
				return this._planCount;
			}
		}

		public bool IsPlanMade { get; private set; }

		public float SpawnPathOffset { get; private set; }

		public bool HasDeploymentBoundaries { get; private set; }

		public bool IsSafeToDeploy
		{
			get
			{
				return this.SafetyScore >= 50f;
			}
		}

		public float SafetyScore { get; private set; }

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

		public MatrixFrame DeploymentFrame
		{
			get
			{
				return this._deploymentFrame;
			}
		}

		public float DeploymentWidth
		{
			get
			{
				return this._deploymentWidth;
			}
		}

		public MBReadOnlyDictionary<string, List<Vec2>> DeploymentBoundaries
		{
			get
			{
				return this._deploymentBoundaries.GetReadOnlyDictionary<string, List<Vec2>>();
			}
		}

		public static DeploymentPlan CreateInitialPlan(Mission mission, BattleSideEnum side)
		{
			return new DeploymentPlan(mission, side, DeploymentPlanType.Initial, SpawnPathData.Invalid);
		}

		public static DeploymentPlan CreateReinforcementPlan(Mission mission, BattleSideEnum side)
		{
			return new DeploymentPlan(mission, side, DeploymentPlanType.Reinforcement, SpawnPathData.Invalid);
		}

		public static DeploymentPlan CreateReinforcementPlanWithSpawnPath(Mission mission, BattleSideEnum side, SpawnPathData spawnPathData)
		{
			return new DeploymentPlan(mission, side, DeploymentPlanType.Reinforcement, spawnPathData);
		}

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

		public void SetSpawnWithHorses(bool value)
		{
			this._spawnWithHorses = value;
		}

		public void ClearAddedTroops()
		{
			for (int i = 0; i < 11; i++)
			{
				this._formationFootTroopCounts[i] = 0;
				this._formationMountedTroopCounts[i] = 0;
			}
		}

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

		public void AddTroops(FormationClass formationClass, int footTroopCount, int mountedTroopCount)
		{
			if (footTroopCount + mountedTroopCount > 0 && formationClass < (FormationClass)11)
			{
				this._formationFootTroopCounts[(int)formationClass] += footTroopCount;
				this._formationMountedTroopCounts[(int)formationClass] += mountedTroopCount;
			}
		}

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

		public FormationDeploymentPlan GetFormationPlan(FormationClass fClass)
		{
			return this._formationPlans[(int)fClass];
		}

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

		private float ComputeFlankWidth(FormationDeploymentFlank flank)
		{
			float num = 0f;
			foreach (KeyValuePair<FormationDeploymentOrder, FormationDeploymentPlan> keyValuePair in this._deploymentFlanks[(int)flank])
			{
				num = MathF.Max(num, keyValuePair.Value.PlannedWidth);
			}
			return num;
		}

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

		private float ComputeHorizontalCenterOffset()
		{
			float num = MathF.Max(this.ComputeFlankWidth(FormationDeploymentFlank.Front), this.ComputeFlankWidth(FormationDeploymentFlank.Rear));
			float num2 = this.ComputeFlankWidth(FormationDeploymentFlank.Left);
			float num3 = this.ComputeFlankWidth(FormationDeploymentFlank.Right);
			float num4 = num / 2f + num2 + 2f;
			return (num / 2f + num3 + 2f - num4) / 2f;
		}

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

		private void AddDeploymentBoundaryPoint(List<Vec2> deploymentBoundaries, Vec2 point)
		{
			if (!deploymentBoundaries.Exists((Vec2 boundaryPoint) => boundaryPoint.Distance(point) <= 0.1f))
			{
				deploymentBoundaries.Add(point);
			}
		}

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

		public const float VerticalFormationGap = 3f;

		public const float HorizontalFormationGap = 2f;

		public const float DeployZoneMinimumWidth = 100f;

		public const float DeployZoneForwardMargin = 10f;

		public const float DeployZoneExtraWidthPerTroop = 1.5f;

		public const float MaxSafetyScore = 100f;

		public readonly BattleSideEnum Side;

		public readonly DeploymentPlanType Type;

		public readonly SpawnPathData SpawnPathData;

		private readonly Mission _mission;

		private int _planCount;

		private bool _spawnWithHorses;

		private readonly int[] _formationMountedTroopCounts;

		private readonly int[] _formationFootTroopCounts;

		private readonly FormationDeploymentPlan[] _formationPlans;

		private MatrixFrame _deploymentFrame;

		private float _deploymentWidth;

		private readonly Dictionary<string, List<Vec2>> _deploymentBoundaries;

		private readonly SortedList<FormationDeploymentOrder, FormationDeploymentPlan>[] _deploymentFlanks = new SortedList<FormationDeploymentOrder, FormationDeploymentPlan>[4];
	}
}
