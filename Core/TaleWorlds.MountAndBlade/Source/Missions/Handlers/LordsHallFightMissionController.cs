using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Objects;

namespace TaleWorlds.MountAndBlade.Source.Missions.Handlers
{
	// Token: 0x020003FA RID: 1018
	public class LordsHallFightMissionController : MissionLogic, IMissionAgentSpawnLogic, IMissionBehavior
	{
		// Token: 0x060034F3 RID: 13555 RVA: 0x000DC3C0 File Offset: 0x000DA5C0
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

		// Token: 0x060034F4 RID: 13556 RVA: 0x000DC423 File Offset: 0x000DA623
		public override void OnCreated()
		{
			base.OnCreated();
			base.Mission.DoesMissionRequireCivilianEquipment = false;
		}

		// Token: 0x060034F5 RID: 13557 RVA: 0x000DC438 File Offset: 0x000DA638
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

		// Token: 0x060034F6 RID: 13558 RVA: 0x000DC4C9 File Offset: 0x000DA6C9
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

		// Token: 0x060034F7 RID: 13559 RVA: 0x000DC504 File Offset: 0x000DA704
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

		// Token: 0x060034F8 RID: 13560 RVA: 0x000DC5BC File Offset: 0x000DA7BC
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

		// Token: 0x060034F9 RID: 13561 RVA: 0x000DC8C0 File Offset: 0x000DAAC0
		private void CheckForReinforcement()
		{
			if (this._spawnReinforcements)
			{
				this._missionSides[1].SpawnTroops(1, true);
				this._spawnReinforcements = false;
			}
		}

		// Token: 0x060034FA RID: 13562 RVA: 0x000DC8E0 File Offset: 0x000DAAE0
		public void StartSpawner(BattleSideEnum side)
		{
			this._missionSides[(int)side].SetSpawnTroops(true);
		}

		// Token: 0x060034FB RID: 13563 RVA: 0x000DC8F0 File Offset: 0x000DAAF0
		public void StopSpawner(BattleSideEnum side)
		{
			this._missionSides[(int)side].SetSpawnTroops(false);
		}

		// Token: 0x060034FC RID: 13564 RVA: 0x000DC900 File Offset: 0x000DAB00
		public bool IsSideSpawnEnabled(BattleSideEnum side)
		{
			return this._missionSides[(int)side].TroopSpawningActive;
		}

		// Token: 0x060034FD RID: 13565 RVA: 0x000DC90F File Offset: 0x000DAB0F
		public float GetReinforcementInterval()
		{
			return 0f;
		}

		// Token: 0x060034FE RID: 13566 RVA: 0x000DC916 File Offset: 0x000DAB16
		public bool IsSideDepleted(BattleSideEnum side)
		{
			return this._missionSides[(int)side].NumberOfActiveTroops == 0;
		}

		// Token: 0x060034FF RID: 13567 RVA: 0x000DC928 File Offset: 0x000DAB28
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

		// Token: 0x06003500 RID: 13568 RVA: 0x000DC9CC File Offset: 0x000DABCC
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

		// Token: 0x06003501 RID: 13569 RVA: 0x000DCA90 File Offset: 0x000DAC90
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

		// Token: 0x06003502 RID: 13570 RVA: 0x000DCB60 File Offset: 0x000DAD60
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

		// Token: 0x06003503 RID: 13571 RVA: 0x000DCBA8 File Offset: 0x000DADA8
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

		// Token: 0x06003504 RID: 13572 RVA: 0x000DCC24 File Offset: 0x000DAE24
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

		// Token: 0x06003505 RID: 13573 RVA: 0x000DCCB8 File Offset: 0x000DAEB8
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

		// Token: 0x06003506 RID: 13574 RVA: 0x000DCDD0 File Offset: 0x000DAFD0
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

		// Token: 0x040016AA RID: 5802
		private readonly float _areaLostRatio;

		// Token: 0x040016AB RID: 5803
		private readonly float _attackerDefenderTroopCountRatio;

		// Token: 0x040016AC RID: 5804
		private readonly int _attackerSideTroopCountMax;

		// Token: 0x040016AD RID: 5805
		private readonly int _defenderSideTroopCountMax;

		// Token: 0x040016AE RID: 5806
		private readonly LordsHallFightMissionController.MissionSide[] _missionSides;

		// Token: 0x040016AF RID: 5807
		private Team[] _attackerTeams;

		// Token: 0x040016B0 RID: 5808
		private Team[] _defenderTeams;

		// Token: 0x040016B1 RID: 5809
		private Dictionary<int, Dictionary<int, LordsHallFightMissionController.AreaData>> _dividedAreaDictionary;

		// Token: 0x040016B2 RID: 5810
		private List<int> _areaIndexList;

		// Token: 0x040016B3 RID: 5811
		private int _lastAreaLostByDefender;

		// Token: 0x040016B4 RID: 5812
		private bool _troopsInitialized;

		// Token: 0x040016B5 RID: 5813
		private bool _isMissionInitialized;

		// Token: 0x040016B6 RID: 5814
		private bool _spawnReinforcements;

		// Token: 0x040016B7 RID: 5815
		private bool _setChargeOrderNextFrame;

		// Token: 0x020006D3 RID: 1747
		private class MissionSide
		{
			// Token: 0x17000A13 RID: 2579
			// (get) Token: 0x06003FEF RID: 16367 RVA: 0x000F9185 File Offset: 0x000F7385
			public bool TroopSpawningActive
			{
				get
				{
					return this._troopSpawningActive;
				}
			}

			// Token: 0x17000A14 RID: 2580
			// (get) Token: 0x06003FF0 RID: 16368 RVA: 0x000F918D File Offset: 0x000F738D
			public int NumberOfActiveTroops
			{
				get
				{
					return this._numberOfSpawnedTroops - this._troopSupplier.NumRemovedTroops;
				}
			}

			// Token: 0x06003FF1 RID: 16369 RVA: 0x000F91A1 File Offset: 0x000F73A1
			public MissionSide(BattleSideEnum side, IMissionTroopSupplier troopSupplier, bool isPlayerSide)
			{
				this._side = side;
				this._isPlayerSide = isPlayerSide;
				this._troopSupplier = troopSupplier;
			}

			// Token: 0x06003FF2 RID: 16370 RVA: 0x000F91C8 File Offset: 0x000F73C8
			public void SpawnTroops(Dictionary<int, Dictionary<int, LordsHallFightMissionController.AreaData>> areaMarkerDictionary, int spawnCount)
			{
				List<IAgentOriginBase> list = this._troopSupplier.SupplyTroops(spawnCount).OrderByDescending(delegate(IAgentOriginBase x)
				{
					if (x.Troop.GetFormationClass() != FormationClass.Ranged && x.Troop.GetFormationClass() != FormationClass.HorseArcher)
					{
						return 0;
					}
					return 1;
				}).ToList<IAgentOriginBase>();
				for (int i = 0; i < list.Count; i++)
				{
					IAgentOriginBase agentOriginBase = list[i];
					bool flag = agentOriginBase.Troop.GetFormationClass() == FormationClass.Ranged || agentOriginBase.Troop.GetFormationClass() == FormationClass.HorseArcher;
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
					agent.WieldInitialWeapons(Agent.WeaponWieldActionType.Instant);
					agent.SetWatchState(Agent.WatchState.Alarmed);
					MovementOrder.SetDefensiveArrangementMoveBehaviorValues(agent);
					areaEntityData.AssignAgent(agent);
				}
			}

			// Token: 0x06003FF3 RID: 16371 RVA: 0x000F93C8 File Offset: 0x000F75C8
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

			// Token: 0x06003FF4 RID: 16372 RVA: 0x000F944E File Offset: 0x000F764E
			public void SetSpawnTroops(bool spawnTroops)
			{
				this._troopSpawningActive = spawnTroops;
			}

			// Token: 0x040022CF RID: 8911
			private readonly BattleSideEnum _side;

			// Token: 0x040022D0 RID: 8912
			private readonly IMissionTroopSupplier _troopSupplier;

			// Token: 0x040022D1 RID: 8913
			private readonly bool _isPlayerSide;

			// Token: 0x040022D2 RID: 8914
			private bool _troopSpawningActive = true;

			// Token: 0x040022D3 RID: 8915
			private int _numberOfSpawnedTroops;
		}

		// Token: 0x020006D4 RID: 1748
		private class AreaData
		{
			// Token: 0x17000A15 RID: 2581
			// (get) Token: 0x06003FF5 RID: 16373 RVA: 0x000F9457 File Offset: 0x000F7657
			public IEnumerable<FightAreaMarker> AreaList
			{
				get
				{
					return this._areaList;
				}
			}

			// Token: 0x17000A16 RID: 2582
			// (get) Token: 0x06003FF6 RID: 16374 RVA: 0x000F945F File Offset: 0x000F765F
			public IEnumerable<LordsHallFightMissionController.AreaEntityData> ArcherUsablePoints
			{
				get
				{
					return this._archerUsablePoints;
				}
			}

			// Token: 0x17000A17 RID: 2583
			// (get) Token: 0x06003FF7 RID: 16375 RVA: 0x000F9467 File Offset: 0x000F7667
			public IEnumerable<LordsHallFightMissionController.AreaEntityData> InfantryUsablePoints
			{
				get
				{
					return this._infantryUsablePoints;
				}
			}

			// Token: 0x06003FF8 RID: 16376 RVA: 0x000F9470 File Offset: 0x000F7670
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

			// Token: 0x06003FF9 RID: 16377 RVA: 0x000F94E8 File Offset: 0x000F76E8
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

			// Token: 0x06003FFA RID: 16378 RVA: 0x000F9500 File Offset: 0x000F7700
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

			// Token: 0x06003FFB RID: 16379 RVA: 0x000F964C File Offset: 0x000F784C
			public LordsHallFightMissionController.AreaEntityData FindAgentMachine(Agent agent)
			{
				return this._infantryUsablePoints.FirstOrDefault((LordsHallFightMissionController.AreaEntityData x) => x.UserAgent == agent) ?? this._archerUsablePoints.FirstOrDefault((LordsHallFightMissionController.AreaEntityData x) => x.UserAgent == agent);
			}

			// Token: 0x040022D4 RID: 8916
			private const string ArcherSpawnPointTag = "defender_archer";

			// Token: 0x040022D5 RID: 8917
			private const string InfantrySpawnPointTag = "defender_infantry";

			// Token: 0x040022D6 RID: 8918
			private readonly List<FightAreaMarker> _areaList;

			// Token: 0x040022D7 RID: 8919
			private readonly List<LordsHallFightMissionController.AreaEntityData> _archerUsablePoints;

			// Token: 0x040022D8 RID: 8920
			private readonly List<LordsHallFightMissionController.AreaEntityData> _infantryUsablePoints;
		}

		// Token: 0x020006D5 RID: 1749
		private class AreaEntityData
		{
			// Token: 0x17000A18 RID: 2584
			// (get) Token: 0x06003FFC RID: 16380 RVA: 0x000F9698 File Offset: 0x000F7898
			// (set) Token: 0x06003FFD RID: 16381 RVA: 0x000F96A0 File Offset: 0x000F78A0
			public Agent UserAgent { get; private set; }

			// Token: 0x17000A19 RID: 2585
			// (get) Token: 0x06003FFE RID: 16382 RVA: 0x000F96A9 File Offset: 0x000F78A9
			public bool InUse
			{
				get
				{
					return this.UserAgent != null;
				}
			}

			// Token: 0x06003FFF RID: 16383 RVA: 0x000F96B4 File Offset: 0x000F78B4
			public AreaEntityData(GameEntity entity)
			{
				this.Entity = entity;
			}

			// Token: 0x06004000 RID: 16384 RVA: 0x000F96C4 File Offset: 0x000F78C4
			public void AssignAgent(Agent agent)
			{
				this.UserAgent = agent;
				MatrixFrame globalFrame = this.Entity.GetGlobalFrame();
				MovementOrder.SetDefaultMoveBehaviorValues(agent);
				this.UserAgent.SetFormationFrameEnabled(new WorldPosition(agent.Mission.Scene, globalFrame.origin), globalFrame.rotation.f.AsVec2.Normalized(), 0f);
			}

			// Token: 0x06004001 RID: 16385 RVA: 0x000F9729 File Offset: 0x000F7929
			public void StopUse()
			{
				if (this.UserAgent.IsActive())
				{
					this.UserAgent.SetFormationFrameDisabled();
				}
				this.UserAgent = null;
			}

			// Token: 0x040022D9 RID: 8921
			public readonly GameEntity Entity;
		}
	}
}
