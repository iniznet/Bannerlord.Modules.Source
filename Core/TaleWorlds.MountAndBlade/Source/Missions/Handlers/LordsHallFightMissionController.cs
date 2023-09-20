using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Objects;

namespace TaleWorlds.MountAndBlade.Source.Missions.Handlers
{
	public class LordsHallFightMissionController : MissionLogic, IMissionAgentSpawnLogic, IMissionBehavior
	{
		public LordsHallFightMissionController(IMissionTroopSupplier[] suppliers, float areaLostRatio, float attackerDefenderTroopCountRatio, int attackerSideTroopCountMax, int defenderSideTroopCountMax, BattleSideEnum playerSide)
		{
			this._areaLostRatio = areaLostRatio;
			this._attackerDefenderTroopCountRatio = attackerDefenderTroopCountRatio;
			this._attackerSideTroopCountMax = attackerSideTroopCountMax;
			this._defenderSideTroopCountMax = defenderSideTroopCountMax;
			this._missionSides = new LordsHallFightMissionController.MissionSide[2];
			for (int i = 0; i < 2; i++)
			{
				IMissionTroopSupplier missionTroopSupplier = suppliers[i];
				bool flag = i == (int)playerSide;
				this._missionSides[i] = new LordsHallFightMissionController.MissionSide((BattleSideEnum)i, missionTroopSupplier, flag);
			}
		}

		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			base.Mission.GetAgentTroopClass_Override += this.GetLordsHallFightTroopClass;
		}

		public override void OnMissionStateFinalized()
		{
			base.OnMissionStateFinalized();
			base.Mission.GetAgentTroopClass_Override -= this.GetLordsHallFightTroopClass;
		}

		public override void OnCreated()
		{
			base.OnCreated();
			base.Mission.DoesMissionRequireCivilianEquipment = false;
		}

		public override void OnMissionTick(float dt)
		{
			if (!this._isMissionInitialized)
			{
				this.InitializeMission();
				this._isMissionInitialized = true;
				return;
			}
			if (!this._troopsInitialized)
			{
				this._troopsInitialized = true;
			}
			if (this._setChargeOrderNextFrame)
			{
				if (base.Mission.PlayerTeam.ActiveAgents.Count > 0)
				{
					base.Mission.PlayerTeam.PlayerOrderController.SelectAllFormations(false);
					base.Mission.PlayerTeam.PlayerOrderController.SetOrder(OrderType.Charge);
				}
				this._setChargeOrderNextFrame = false;
			}
			this.CheckForReinforcement();
			this.CheckIfAnyAreaIsLostByDefender();
		}

		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			if (!affectedAgent.Team.IsDefender)
			{
				this._setChargeOrderNextFrame = affectedAgent.IsMainAgent;
				this._spawnReinforcements = true;
				return;
			}
			Tuple<int, LordsHallFightMissionController.AreaEntityData> tuple = this.FindAgentMachine(affectedAgent);
			if (tuple == null)
			{
				return;
			}
			tuple.Item2.StopUse();
		}

		private Tuple<int, LordsHallFightMissionController.AreaEntityData> FindAgentMachine(Agent agent)
		{
			Tuple<int, LordsHallFightMissionController.AreaEntityData> tuple = null;
			foreach (KeyValuePair<int, Dictionary<int, LordsHallFightMissionController.AreaData>> keyValuePair in this._dividedAreaDictionary)
			{
				if (tuple != null)
				{
					break;
				}
				foreach (KeyValuePair<int, LordsHallFightMissionController.AreaData> keyValuePair2 in keyValuePair.Value)
				{
					LordsHallFightMissionController.AreaEntityData areaEntityData = keyValuePair2.Value.FindAgentMachine(agent);
					if (areaEntityData != null)
					{
						tuple = new Tuple<int, LordsHallFightMissionController.AreaEntityData>(keyValuePair.Key, areaEntityData);
						break;
					}
				}
			}
			return tuple;
		}

		private void InitializeMission()
		{
			this._areaIndexList = new List<int>();
			this._dividedAreaDictionary = new Dictionary<int, Dictionary<int, LordsHallFightMissionController.AreaData>>();
			IEnumerable<FightAreaMarker> enumerable = from area in base.Mission.ActiveMissionObjects.FindAllWithType<FightAreaMarker>()
				orderby area.AreaIndex
				select area;
			base.Mission.MakeDefaultDeploymentPlans();
			foreach (FightAreaMarker fightAreaMarker in enumerable)
			{
				if (!this._dividedAreaDictionary.ContainsKey(fightAreaMarker.AreaIndex))
				{
					this._dividedAreaDictionary.Add(fightAreaMarker.AreaIndex, new Dictionary<int, LordsHallFightMissionController.AreaData>());
				}
				if (!this._dividedAreaDictionary[fightAreaMarker.AreaIndex].ContainsKey(fightAreaMarker.SubAreaIndex))
				{
					this._dividedAreaDictionary[fightAreaMarker.AreaIndex].Add(fightAreaMarker.SubAreaIndex, new LordsHallFightMissionController.AreaData(new List<FightAreaMarker> { fightAreaMarker }));
				}
				else
				{
					this._dividedAreaDictionary[fightAreaMarker.AreaIndex][fightAreaMarker.SubAreaIndex].AddAreaMarker(fightAreaMarker);
				}
			}
			this._areaIndexList = this._dividedAreaDictionary.Keys.ToList<int>();
			this._missionSides[0].SpawnTroops(this._dividedAreaDictionary, this._defenderSideTroopCountMax);
			int numberOfActiveTroops = this._missionSides[0].NumberOfActiveTroops;
			this._defenderTeams = new Team[2];
			this._defenderTeams[0] = Mission.Current.DefenderTeam;
			this._defenderTeams[1] = Mission.Current.DefenderAllyTeam;
			int num = MathF.Max(1, MathF.Min(this._attackerSideTroopCountMax, MathF.Round((float)numberOfActiveTroops * this._attackerDefenderTroopCountRatio)));
			this._missionSides[1].SpawnTroops(num, false);
			bool flag = Mission.Current.AttackerTeam == Mission.Current.PlayerTeam || (Mission.Current.AttackerAllyTeam != null && Mission.Current.AttackerAllyTeam == Mission.Current.PlayerTeam);
			this._attackerTeams = new Team[2];
			this._attackerTeams[0] = Mission.Current.AttackerTeam;
			this._attackerTeams[1] = Mission.Current.AttackerAllyTeam;
			foreach (Team team in this._attackerTeams)
			{
				if (team != null)
				{
					foreach (Formation formation in team.FormationsIncludingEmpty)
					{
						if (formation.CountOfUnits > 0)
						{
							formation.ArrangementOrder = ArrangementOrder.ArrangementOrderSquare;
							formation.FormOrder = FormOrder.FormOrderDeep;
						}
						formation.SetMovementOrder(MovementOrder.MovementOrderCharge);
						formation.FiringOrder = FiringOrder.FiringOrderHoldYourFire;
						if (flag)
						{
							formation.PlayerOwner = Mission.Current.MainAgent;
						}
					}
				}
			}
		}

		private void CheckForReinforcement()
		{
			if (this._spawnReinforcements)
			{
				this._missionSides[1].SpawnTroops(1, true);
				this._spawnReinforcements = false;
			}
		}

		public void StartSpawner(BattleSideEnum side)
		{
			this._missionSides[(int)side].SetSpawnTroops(true);
		}

		public void StopSpawner(BattleSideEnum side)
		{
			this._missionSides[(int)side].SetSpawnTroops(false);
		}

		public bool IsSideSpawnEnabled(BattleSideEnum side)
		{
			return this._missionSides[(int)side].TroopSpawningActive;
		}

		public float GetReinforcementInterval()
		{
			return 0f;
		}

		public bool IsSideDepleted(BattleSideEnum side)
		{
			return this._missionSides[(int)side].NumberOfActiveTroops == 0;
		}

		private void CheckIfAnyAreaIsLostByDefender()
		{
			int num = -1;
			for (int i = 0; i < this._areaIndexList.Count; i++)
			{
				int num2 = this._areaIndexList[i];
				if (num2 > this._lastAreaLostByDefender && num < 0)
				{
					foreach (KeyValuePair<int, LordsHallFightMissionController.AreaData> keyValuePair in this._dividedAreaDictionary[num2])
					{
						if (this.IsAreaLostByDefender(keyValuePair.Value))
						{
							num = num2;
							break;
						}
					}
				}
			}
			if (num > 0)
			{
				this.OnAreaLost(num);
			}
		}

		private void OnAreaLost(int areaIndex)
		{
			int num = MathF.Min(this._areaIndexList.IndexOf(areaIndex) + 1, this._areaIndexList.Count - 1);
			for (int i = MathF.Max(0, this._areaIndexList.IndexOf(this._lastAreaLostByDefender)); i < num; i++)
			{
				int num2 = this._areaIndexList[i];
				foreach (KeyValuePair<int, LordsHallFightMissionController.AreaData> keyValuePair in this._dividedAreaDictionary[num2])
				{
					this.StartAreaPullBack(keyValuePair.Value, this._areaIndexList[num]);
				}
			}
			this._lastAreaLostByDefender = areaIndex;
		}

		private void StartAreaPullBack(LordsHallFightMissionController.AreaData areaData, int nextAreaIndex)
		{
			foreach (LordsHallFightMissionController.AreaEntityData areaEntityData in areaData.ArcherUsablePoints)
			{
				if (areaEntityData.InUse)
				{
					Agent userAgent = areaEntityData.UserAgent;
					areaEntityData.StopUse();
					LordsHallFightMissionController.AreaEntityData areaEntityData2 = this.FindPosition(nextAreaIndex, true);
					if (areaEntityData2 != null)
					{
						areaEntityData2.AssignAgent(userAgent);
					}
				}
			}
			foreach (LordsHallFightMissionController.AreaEntityData areaEntityData3 in areaData.InfantryUsablePoints)
			{
				if (areaEntityData3.InUse)
				{
					Agent userAgent2 = areaEntityData3.UserAgent;
					areaEntityData3.StopUse();
					LordsHallFightMissionController.AreaEntityData areaEntityData4 = this.FindPosition(nextAreaIndex, false);
					if (areaEntityData4 != null)
					{
						areaEntityData4.AssignAgent(userAgent2);
					}
				}
			}
		}

		private LordsHallFightMissionController.AreaEntityData FindPosition(int nextAreaIndex, bool isArcher)
		{
			int num = this.SelectBestSubArea(nextAreaIndex, isArcher);
			if (num < 0)
			{
				isArcher = !isArcher;
				num = this.SelectBestSubArea(nextAreaIndex, isArcher);
			}
			return this._dividedAreaDictionary[nextAreaIndex][num].GetAvailableMachines(isArcher).GetRandomElementInefficiently<LordsHallFightMissionController.AreaEntityData>();
		}

		private int SelectBestSubArea(int areaIndex, bool isArcher)
		{
			int num = -1;
			float num2 = 0f;
			foreach (KeyValuePair<int, LordsHallFightMissionController.AreaData> keyValuePair in this._dividedAreaDictionary[areaIndex])
			{
				float areaAvailabilityRatio = this.GetAreaAvailabilityRatio(keyValuePair.Value, isArcher);
				if (areaAvailabilityRatio > num2)
				{
					num2 = areaAvailabilityRatio;
					num = keyValuePair.Key;
				}
			}
			return num;
		}

		private float GetAreaAvailabilityRatio(LordsHallFightMissionController.AreaData areaData, bool isArcher)
		{
			int num = (isArcher ? areaData.ArcherUsablePoints.Count<LordsHallFightMissionController.AreaEntityData>() : areaData.InfantryUsablePoints.Count<LordsHallFightMissionController.AreaEntityData>());
			int num2;
			if (!isArcher)
			{
				num2 = areaData.InfantryUsablePoints.Count((LordsHallFightMissionController.AreaEntityData x) => !x.InUse);
			}
			else
			{
				num2 = areaData.ArcherUsablePoints.Count((LordsHallFightMissionController.AreaEntityData x) => !x.InUse);
			}
			int num3 = num2;
			if (num != 0)
			{
				return (float)num3 / (float)num;
			}
			return 0f;
		}

		private bool IsAreaLostByDefender(LordsHallFightMissionController.AreaData areaData)
		{
			int num = 0;
			foreach (Team team in this._defenderTeams)
			{
				if (team != null)
				{
					foreach (Agent agent in team.ActiveAgents)
					{
						if (this.IsAgentInArea(agent, areaData))
						{
							num++;
						}
					}
				}
			}
			int num2 = MathF.Round((float)num * this._areaLostRatio);
			bool flag = num2 == 0;
			if (!flag)
			{
				foreach (Team team2 in this._attackerTeams)
				{
					if (team2 != null)
					{
						foreach (Agent agent2 in team2.ActiveAgents)
						{
							if (this.IsAgentInArea(agent2, areaData))
							{
								num2--;
								if (num2 == 0)
								{
									flag = true;
									break;
								}
							}
						}
						if (flag)
						{
							break;
						}
					}
				}
			}
			return flag;
		}

		private bool IsAgentInArea(Agent agent, LordsHallFightMissionController.AreaData areaData)
		{
			bool flag = false;
			Vec3 position = agent.Position;
			using (IEnumerator<FightAreaMarker> enumerator = areaData.AreaList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsPositionInRange(position))
					{
						flag = true;
						break;
					}
				}
			}
			return flag;
		}

		private FormationClass GetLordsHallFightTroopClass(BattleSideEnum side, BasicCharacterObject agentCharacter)
		{
			return agentCharacter.GetFormationClass().DismountedClass();
		}

		private readonly float _areaLostRatio;

		private readonly float _attackerDefenderTroopCountRatio;

		private readonly int _attackerSideTroopCountMax;

		private readonly int _defenderSideTroopCountMax;

		private readonly LordsHallFightMissionController.MissionSide[] _missionSides;

		private Team[] _attackerTeams;

		private Team[] _defenderTeams;

		private Dictionary<int, Dictionary<int, LordsHallFightMissionController.AreaData>> _dividedAreaDictionary;

		private List<int> _areaIndexList;

		private int _lastAreaLostByDefender;

		private bool _troopsInitialized;

		private bool _isMissionInitialized;

		private bool _spawnReinforcements;

		private bool _setChargeOrderNextFrame;

		private class MissionSide
		{
			public bool TroopSpawningActive
			{
				get
				{
					return this._troopSpawningActive;
				}
			}

			public int NumberOfActiveTroops
			{
				get
				{
					return this._numberOfSpawnedTroops - this._troopSupplier.NumRemovedTroops;
				}
			}

			public MissionSide(BattleSideEnum side, IMissionTroopSupplier troopSupplier, bool isPlayerSide)
			{
				this._side = side;
				this._isPlayerSide = isPlayerSide;
				this._troopSupplier = troopSupplier;
			}

			public void SpawnTroops(Dictionary<int, Dictionary<int, LordsHallFightMissionController.AreaData>> areaMarkerDictionary, int spawnCount)
			{
				List<IAgentOriginBase> list = this._troopSupplier.SupplyTroops(spawnCount).OrderByDescending(delegate(IAgentOriginBase x)
				{
					FormationClass agentTroopClass = Mission.Current.GetAgentTroopClass(this._side, x.Troop);
					if (agentTroopClass != FormationClass.Ranged && agentTroopClass != FormationClass.HorseArcher)
					{
						return 0;
					}
					return 1;
				}).ToList<IAgentOriginBase>();
				for (int i = 0; i < list.Count; i++)
				{
					IAgentOriginBase agentOriginBase = list[i];
					bool flag = Mission.Current.GetAgentTroopClass(this._side, agentOriginBase.Troop).IsRanged();
					List<KeyValuePair<int, LordsHallFightMissionController.AreaData>> list2 = areaMarkerDictionary.ElementAt(i % areaMarkerDictionary.Count).Value.ToList<KeyValuePair<int, LordsHallFightMissionController.AreaData>>();
					List<ValueTuple<KeyValuePair<int, LordsHallFightMissionController.AreaData>, float>> list3 = new List<ValueTuple<KeyValuePair<int, LordsHallFightMissionController.AreaData>, float>>();
					foreach (KeyValuePair<int, LordsHallFightMissionController.AreaData> keyValuePair in list2)
					{
						int num = 1000 * keyValuePair.Value.GetAvailableMachines(flag).Count<LordsHallFightMissionController.AreaEntityData>() + keyValuePair.Value.GetAvailableMachines(!flag).Count<LordsHallFightMissionController.AreaEntityData>();
						list3.Add(new ValueTuple<KeyValuePair<int, LordsHallFightMissionController.AreaData>, float>(new KeyValuePair<int, LordsHallFightMissionController.AreaData>(keyValuePair.Key, keyValuePair.Value), (float)num));
					}
					KeyValuePair<int, LordsHallFightMissionController.AreaData> keyValuePair2 = MBRandom.ChooseWeighted<KeyValuePair<int, LordsHallFightMissionController.AreaData>>(list3);
					LordsHallFightMissionController.AreaEntityData areaEntityData = keyValuePair2.Value.GetAvailableMachines(flag).GetRandomElementInefficiently<LordsHallFightMissionController.AreaEntityData>() ?? keyValuePair2.Value.GetAvailableMachines(!flag).GetRandomElementInefficiently<LordsHallFightMissionController.AreaEntityData>();
					MatrixFrame globalFrame = areaEntityData.Entity.GetGlobalFrame();
					Agent agent = Mission.Current.SpawnTroop(agentOriginBase, false, false, false, false, 0, 0, false, false, false, new Vec3?(globalFrame.origin), new Vec2?(globalFrame.rotation.f.AsVec2.Normalized()), null, null, FormationClass.NumberOfAllFormations, false);
					this._numberOfSpawnedTroops++;
					AgentFlag agentFlags = agent.GetAgentFlags();
					agent.SetAgentFlags(agentFlags & ~AgentFlag.CanRetreat);
					agent.WieldInitialWeapons(Agent.WeaponWieldActionType.Instant, Equipment.InitialWeaponEquipPreference.Any);
					agent.SetWatchState(Agent.WatchState.Alarmed);
					agent.SetBehaviorValueSet(HumanAIComponent.BehaviorValueSet.DefensiveArrangementMove);
					areaEntityData.AssignAgent(agent);
				}
			}

			public void SpawnTroops(int spawnCount, bool isReinforcement)
			{
				if (this._troopSpawningActive)
				{
					List<IAgentOriginBase> list = this._troopSupplier.SupplyTroops(spawnCount).ToList<IAgentOriginBase>();
					for (int i = 0; i < list.Count; i++)
					{
						if (BattleSideEnum.Attacker == this._side)
						{
							Mission.Current.SpawnTroop(list[i], this._isPlayerSide, true, false, isReinforcement, spawnCount, i, true, true, true, null, null, null, null, FormationClass.NumberOfAllFormations, false);
							this._numberOfSpawnedTroops++;
						}
					}
				}
			}

			public void SetSpawnTroops(bool spawnTroops)
			{
				this._troopSpawningActive = spawnTroops;
			}

			private readonly BattleSideEnum _side;

			private readonly IMissionTroopSupplier _troopSupplier;

			private readonly bool _isPlayerSide;

			private bool _troopSpawningActive = true;

			private int _numberOfSpawnedTroops;
		}

		private class AreaData
		{
			public IEnumerable<FightAreaMarker> AreaList
			{
				get
				{
					return this._areaList;
				}
			}

			public IEnumerable<LordsHallFightMissionController.AreaEntityData> ArcherUsablePoints
			{
				get
				{
					return this._archerUsablePoints;
				}
			}

			public IEnumerable<LordsHallFightMissionController.AreaEntityData> InfantryUsablePoints
			{
				get
				{
					return this._infantryUsablePoints;
				}
			}

			public AreaData(List<FightAreaMarker> areaList)
			{
				this._areaList = new List<FightAreaMarker>();
				this._archerUsablePoints = new List<LordsHallFightMissionController.AreaEntityData>();
				this._infantryUsablePoints = new List<LordsHallFightMissionController.AreaEntityData>();
				foreach (FightAreaMarker fightAreaMarker in areaList)
				{
					this.AddAreaMarker(fightAreaMarker);
				}
			}

			public IEnumerable<LordsHallFightMissionController.AreaEntityData> GetAvailableMachines(bool isArcher)
			{
				List<LordsHallFightMissionController.AreaEntityData> list = (isArcher ? this._archerUsablePoints : this._infantryUsablePoints);
				foreach (LordsHallFightMissionController.AreaEntityData areaEntityData in list)
				{
					if (!areaEntityData.InUse)
					{
						yield return areaEntityData;
					}
				}
				List<LordsHallFightMissionController.AreaEntityData>.Enumerator enumerator = default(List<LordsHallFightMissionController.AreaEntityData>.Enumerator);
				yield break;
				yield break;
			}

			public void AddAreaMarker(FightAreaMarker marker)
			{
				this._areaList.Add(marker);
				using (List<GameEntity>.Enumerator enumerator = marker.GetGameEntitiesWithTagInRange("defender_archer").GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						GameEntity entity2 = enumerator.Current;
						PathFaceRecord nullFaceRecord = PathFaceRecord.NullFaceRecord;
						Mission.Current.Scene.GetNavMeshFaceIndex(ref nullFaceRecord, entity2.GetGlobalFrame().origin, true);
						if (nullFaceRecord.FaceIndex != -1 && this._archerUsablePoints.All((LordsHallFightMissionController.AreaEntityData x) => x.Entity != entity2))
						{
							this._archerUsablePoints.Add(new LordsHallFightMissionController.AreaEntityData(entity2));
						}
					}
				}
				using (List<GameEntity>.Enumerator enumerator = marker.GetGameEntitiesWithTagInRange("defender_infantry").GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						GameEntity entity = enumerator.Current;
						if (this._infantryUsablePoints.All((LordsHallFightMissionController.AreaEntityData x) => x.Entity != entity))
						{
							this._infantryUsablePoints.Add(new LordsHallFightMissionController.AreaEntityData(entity));
						}
					}
				}
			}

			public LordsHallFightMissionController.AreaEntityData FindAgentMachine(Agent agent)
			{
				return this._infantryUsablePoints.FirstOrDefault((LordsHallFightMissionController.AreaEntityData x) => x.UserAgent == agent) ?? this._archerUsablePoints.FirstOrDefault((LordsHallFightMissionController.AreaEntityData x) => x.UserAgent == agent);
			}

			private const string ArcherSpawnPointTag = "defender_archer";

			private const string InfantrySpawnPointTag = "defender_infantry";

			private readonly List<FightAreaMarker> _areaList;

			private readonly List<LordsHallFightMissionController.AreaEntityData> _archerUsablePoints;

			private readonly List<LordsHallFightMissionController.AreaEntityData> _infantryUsablePoints;
		}

		private class AreaEntityData
		{
			public Agent UserAgent { get; private set; }

			public bool InUse
			{
				get
				{
					return this.UserAgent != null;
				}
			}

			public AreaEntityData(GameEntity entity)
			{
				this.Entity = entity;
			}

			public void AssignAgent(Agent agent)
			{
				this.UserAgent = agent;
				MatrixFrame globalFrame = this.Entity.GetGlobalFrame();
				agent.SetBehaviorValueSet(HumanAIComponent.BehaviorValueSet.DefaultMove);
				this.UserAgent.SetFormationFrameEnabled(new WorldPosition(agent.Mission.Scene, globalFrame.origin), globalFrame.rotation.f.AsVec2.Normalized(), Vec2.Zero, 0f);
			}

			public void StopUse()
			{
				if (this.UserAgent.IsActive())
				{
					this.UserAgent.SetFormationFrameDisabled();
				}
				this.UserAgent = null;
			}

			public readonly GameEntity Entity;
		}
	}
}
