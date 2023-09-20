using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Missions.Handlers;

namespace TaleWorlds.MountAndBlade
{
	public class MissionAgentSpawnLogic : MissionLogic, IMissionAgentSpawnLogic, IMissionBehavior
	{
		public static int MaxNumberOfAgentsForMission
		{
			get
			{
				if (MissionAgentSpawnLogic._maxNumberOfAgentsForMissionCache == 0)
				{
					MissionAgentSpawnLogic._maxNumberOfAgentsForMissionCache = MBAPI.IMBAgent.GetMaximumNumberOfAgents();
				}
				return MissionAgentSpawnLogic._maxNumberOfAgentsForMissionCache;
			}
		}

		private static int MaxNumberOfTroopsForMission
		{
			get
			{
				return MissionAgentSpawnLogic.MaxNumberOfAgentsForMission / 2;
			}
		}

		public event Action<BattleSideEnum, int> OnReinforcementsSpawned;

		public event Action<BattleSideEnum, int> OnInitialTroopsSpawned;

		public int NumberOfAgents
		{
			get
			{
				return base.Mission.AllAgents.Count;
			}
		}

		public int NumberOfRemainingTroops
		{
			get
			{
				MissionAgentSpawnLogic.SpawnPhase defenderActivePhase = this.DefenderActivePhase;
				int num = ((defenderActivePhase != null) ? defenderActivePhase.RemainingSpawnNumber : 0);
				MissionAgentSpawnLogic.SpawnPhase attackerActivePhase = this.AttackerActivePhase;
				return num + ((attackerActivePhase != null) ? attackerActivePhase.RemainingSpawnNumber : 0);
			}
		}

		public int NumberOfActiveDefenderTroops
		{
			get
			{
				MissionAgentSpawnLogic.SpawnPhase defenderActivePhase = this.DefenderActivePhase;
				if (defenderActivePhase == null)
				{
					return 0;
				}
				return defenderActivePhase.NumberActiveTroops;
			}
		}

		public int NumberOfActiveAttackerTroops
		{
			get
			{
				MissionAgentSpawnLogic.SpawnPhase attackerActivePhase = this.AttackerActivePhase;
				if (attackerActivePhase == null)
				{
					return 0;
				}
				return attackerActivePhase.NumberActiveTroops;
			}
		}

		public int NumberOfRemainingDefenderTroops
		{
			get
			{
				MissionAgentSpawnLogic.SpawnPhase defenderActivePhase = this.DefenderActivePhase;
				if (defenderActivePhase == null)
				{
					return 0;
				}
				return defenderActivePhase.RemainingSpawnNumber;
			}
		}

		public int NumberOfRemainingAttackerTroops
		{
			get
			{
				MissionAgentSpawnLogic.SpawnPhase attackerActivePhase = this.AttackerActivePhase;
				if (attackerActivePhase == null)
				{
					return 0;
				}
				return attackerActivePhase.RemainingSpawnNumber;
			}
		}

		public int BattleSize
		{
			get
			{
				return this._battleSize;
			}
		}

		public bool IsInitialSpawnOver
		{
			get
			{
				return this.DefenderActivePhase.InitialSpawnNumber + this.AttackerActivePhase.InitialSpawnNumber == 0;
			}
		}

		public bool IsDeploymentOver
		{
			get
			{
				return base.Mission.GetMissionBehavior<BattleDeploymentHandler>() == null && this.IsInitialSpawnOver;
			}
		}

		public ref readonly MissionSpawnSettings ReinforcementSpawnSettings
		{
			get
			{
				return ref this._spawnSettings;
			}
		}

		private int TotalSpawnNumber
		{
			get
			{
				MissionAgentSpawnLogic.SpawnPhase defenderActivePhase = this.DefenderActivePhase;
				int num = ((defenderActivePhase != null) ? defenderActivePhase.TotalSpawnNumber : 0);
				MissionAgentSpawnLogic.SpawnPhase attackerActivePhase = this.AttackerActivePhase;
				return num + ((attackerActivePhase != null) ? attackerActivePhase.TotalSpawnNumber : 0);
			}
		}

		private MissionAgentSpawnLogic.SpawnPhase DefenderActivePhase
		{
			get
			{
				return this._phases[0].FirstOrDefault<MissionAgentSpawnLogic.SpawnPhase>();
			}
		}

		private MissionAgentSpawnLogic.SpawnPhase AttackerActivePhase
		{
			get
			{
				return this._phases[1].FirstOrDefault<MissionAgentSpawnLogic.SpawnPhase>();
			}
		}

		public override void AfterStart()
		{
			this._bannerBearerLogic = base.Mission.GetMissionBehavior<BannerBearerLogic>();
			if (this._bannerBearerLogic != null)
			{
				for (int i = 0; i < 2; i++)
				{
					this._missionSides[i].SetBannerBearerLogic(this._bannerBearerLogic);
				}
			}
			MissionGameModels.Current.BattleSpawnModel.OnMissionStart();
		}

		public int GetNumberOfPlayerControllableTroops()
		{
			MissionAgentSpawnLogic.MissionSide playerSide = this._playerSide;
			if (playerSide == null)
			{
				return 0;
			}
			return playerSide.GetNumberOfPlayerControllableTroops();
		}

		public void InitWithSinglePhase(int defenderTotalSpawn, int attackerTotalSpawn, int defenderInitialSpawn, int attackerInitialSpawn, bool spawnDefenders, bool spawnAttackers, in MissionSpawnSettings spawnSettings)
		{
			this.AddPhase(BattleSideEnum.Defender, defenderTotalSpawn, defenderInitialSpawn);
			this.AddPhase(BattleSideEnum.Attacker, attackerTotalSpawn, attackerInitialSpawn);
			this.Init(spawnDefenders, spawnAttackers, spawnSettings);
		}

		public IEnumerable<IAgentOriginBase> GetAllTroopsForSide(BattleSideEnum side)
		{
			return this._missionSides[(int)side].GetAllTroops();
		}

		public override void OnMissionTick(float dt)
		{
			if (!MBNetwork.IsClient && !this.CheckDeployment())
			{
				return;
			}
			this.PhaseTick();
			if (this._reinforcementSpawnEnabled)
			{
				if (this._spawnSettings.ReinforcementTroopsTimingMethod == MissionSpawnSettings.ReinforcementTimingMethod.GlobalTimer)
				{
					this.CheckGlobalReinforcementBatch();
				}
				else if (this._spawnSettings.ReinforcementTroopsTimingMethod == MissionSpawnSettings.ReinforcementTimingMethod.CustomTimer)
				{
					this.CheckCustomReinforcementBatch();
				}
			}
			if (this._spawningReinforcements)
			{
				this.CheckReinforcementSpawn();
			}
		}

		public MissionAgentSpawnLogic(IMissionTroopSupplier[] suppliers, BattleSideEnum playerSide, Mission.BattleSizeType battleSizeType)
		{
			switch (battleSizeType)
			{
			case Mission.BattleSizeType.Battle:
				this._battleSize = BannerlordConfig.GetRealBattleSize();
				break;
			case Mission.BattleSizeType.Siege:
				this._battleSize = BannerlordConfig.GetRealBattleSizeForSiege();
				break;
			case Mission.BattleSizeType.SallyOut:
				this._battleSize = BannerlordConfig.GetRealBattleSizeForSallyOut();
				break;
			}
			this._battleSize = MathF.Min(this._battleSize, MissionAgentSpawnLogic.MaxNumberOfTroopsForMission);
			this._globalReinforcementSpawnTimer = new BasicMissionTimer();
			this._spawnSettings = MissionSpawnSettings.CreateDefaultSpawnSettings();
			this._globalReinforcementInterval = this._spawnSettings.GlobalReinforcementInterval;
			this._missionSides = new MissionAgentSpawnLogic.MissionSide[2];
			for (int i = 0; i < 2; i++)
			{
				IMissionTroopSupplier missionTroopSupplier = suppliers[i];
				bool flag = i == (int)playerSide;
				MissionAgentSpawnLogic.MissionSide missionSide = new MissionAgentSpawnLogic.MissionSide(this, (BattleSideEnum)i, missionTroopSupplier, flag);
				if (flag)
				{
					this._playerSide = missionSide;
				}
				this._missionSides[i] = missionSide;
			}
			this._numberOfTroopsInTotal = new int[2];
			this._formationSpawnData = new MissionAgentSpawnLogic.FormationSpawnData[11];
			this._phases = new List<MissionAgentSpawnLogic.SpawnPhase>[2];
			for (int j = 0; j < 2; j++)
			{
				this._phases[j] = new List<MissionAgentSpawnLogic.SpawnPhase>();
			}
			this._reinforcementSpawnEnabled = false;
		}

		public void SetCustomReinforcementSpawnTimer(ICustomReinforcementSpawnTimer timer)
		{
			this._customReinforcementSpawnTimer = timer;
		}

		public void SetSpawnTroops(BattleSideEnum side, bool spawnTroops, bool enforceSpawning = false)
		{
			this._missionSides[(int)side].SetSpawnTroops(spawnTroops);
			if (spawnTroops && enforceSpawning)
			{
				this.CheckDeployment();
			}
		}

		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			MissionGameModels.Current.BattleInitializationModel.InitializeModel();
		}

		protected override void OnEndMission()
		{
			MissionGameModels.Current.BattleSpawnModel.OnMissionEnd();
			MissionGameModels.Current.BattleInitializationModel.FinalizeModel();
		}

		public void SetSpawnHorses(BattleSideEnum side, bool spawnHorses)
		{
			this._missionSides[(int)side].SetSpawnWithHorses(spawnHorses);
			base.Mission.SetDeploymentPlanSpawnWithHorses(side, spawnHorses);
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
			return this._missionSides[(int)side].TroopSpawnActive;
		}

		public void OnBattleSideDeployed(BattleSideEnum side)
		{
			foreach (Team team in base.Mission.Teams)
			{
				if (team.Side == side)
				{
					team.OnDeployed();
				}
			}
			foreach (Team team2 in base.Mission.Teams)
			{
				if (team2.Side == side)
				{
					foreach (Formation formation in team2.FormationsIncludingEmpty)
					{
						if (formation.CountOfUnits > 0)
						{
							formation.QuerySystem.EvaluateAllPreliminaryQueryData();
						}
					}
					team2.MasterOrderController.OnOrderIssued += this.OrderController_OnOrderIssued;
					for (int i = 8; i < 10; i++)
					{
						Formation formation2 = team2.FormationsIncludingSpecialAndEmpty[i];
						if (formation2.CountOfUnits > 0)
						{
							team2.MasterOrderController.SelectFormation(formation2);
							team2.MasterOrderController.SetOrderWithAgent(OrderType.FollowMe, team2.GeneralAgent);
							team2.MasterOrderController.ClearSelectedFormations();
							formation2.SetControlledByAI(true, false);
						}
					}
					team2.MasterOrderController.OnOrderIssued -= this.OrderController_OnOrderIssued;
				}
			}
		}

		public float GetReinforcementInterval()
		{
			return this._globalReinforcementInterval;
		}

		public void SetReinforcementsSpawnEnabled(bool value, bool resetTimers = true)
		{
			if (this._reinforcementSpawnEnabled != value)
			{
				this._reinforcementSpawnEnabled = value;
				if (resetTimers)
				{
					if (this._spawnSettings.ReinforcementTroopsTimingMethod == MissionSpawnSettings.ReinforcementTimingMethod.GlobalTimer)
					{
						this._globalReinforcementSpawnTimer.Reset();
						return;
					}
					if (this._spawnSettings.ReinforcementTroopsTimingMethod == MissionSpawnSettings.ReinforcementTimingMethod.CustomTimer)
					{
						for (int i = 0; i < 2; i++)
						{
							this._customReinforcementSpawnTimer.ResetTimer((BattleSideEnum)i);
						}
					}
				}
			}
		}

		public int GetTotalNumberOfTroopsForSide(BattleSideEnum side)
		{
			return this._numberOfTroopsInTotal[(int)side];
		}

		public BasicCharacterObject GetGeneralCharacterOfSide(BattleSideEnum side)
		{
			if (side >= BattleSideEnum.Defender && side < BattleSideEnum.NumSides)
			{
				this._missionSides[(int)side].GetGeneralCharacter();
			}
			return null;
		}

		public bool GetSpawnHorses(BattleSideEnum side)
		{
			return this._missionSides[(int)side].SpawnWithHorses;
		}

		private bool CheckMinimumBatchQuotaRequirement()
		{
			int num = MissionAgentSpawnLogic.MaxNumberOfAgentsForMission - this.NumberOfAgents;
			int num2 = 0;
			for (int i = 0; i < 2; i++)
			{
				num2 += this._missionSides[i].ReinforcementQuotaRequirement;
			}
			return num >= num2;
		}

		private void CheckGlobalReinforcementBatch()
		{
			if (this._globalReinforcementSpawnTimer.ElapsedTime >= this._globalReinforcementInterval)
			{
				bool flag = false;
				for (int i = 0; i < 2; i++)
				{
					BattleSideEnum battleSideEnum = (BattleSideEnum)i;
					this.NotifyReinforcementTroopsSpawned(battleSideEnum, false);
					bool flag2 = this._missionSides[i].CheckReinforcementBatch();
					flag = flag || flag2;
				}
				this._spawningReinforcements = flag && this.CheckMinimumBatchQuotaRequirement();
				this._globalReinforcementSpawnTimer.Reset();
			}
		}

		private void CheckCustomReinforcementBatch()
		{
			bool flag = false;
			for (int i = 0; i < 2; i++)
			{
				BattleSideEnum battleSideEnum = (BattleSideEnum)i;
				if (this._customReinforcementSpawnTimer.Check(battleSideEnum))
				{
					flag = true;
					this.NotifyReinforcementTroopsSpawned(battleSideEnum, false);
					this._missionSides[i].CheckReinforcementBatch();
				}
			}
			if (flag)
			{
				bool flag2 = false;
				for (int j = 0; j < 2; j++)
				{
					flag2 = flag2 || this._missionSides[j].ReinforcementSpawnActive;
				}
				this._spawningReinforcements = flag2 && this.CheckMinimumBatchQuotaRequirement();
			}
		}

		public bool IsSideDepleted(BattleSideEnum side)
		{
			return this._phases[(int)side].Count == 1 && this._missionSides[(int)side].NumberOfActiveTroops == 0 && this.GetActivePhaseForSide(side).RemainingSpawnNumber == 0;
		}

		public void AddPhaseChangeAction(BattleSideEnum side, MissionAgentSpawnLogic.OnPhaseChangedDelegate onPhaseChanged)
		{
			MissionAgentSpawnLogic.OnPhaseChangedDelegate[] onPhaseChanged2 = this._onPhaseChanged;
			onPhaseChanged2[(int)side] = (MissionAgentSpawnLogic.OnPhaseChangedDelegate)Delegate.Combine(onPhaseChanged2[(int)side], onPhaseChanged);
		}

		private void Init(bool spawnDefenders, bool spawnAttackers, in MissionSpawnSettings reinforcementSpawnSettings)
		{
			List<MissionAgentSpawnLogic.SpawnPhase>[] phases = this._phases;
			for (int i = 0; i < phases.Length; i++)
			{
				if (phases[i].Count <= 0)
				{
					return;
				}
			}
			this._spawnSettings = reinforcementSpawnSettings;
			int num = 0;
			int num2 = 1;
			this._globalReinforcementInterval = this._spawnSettings.GlobalReinforcementInterval;
			int[] array = new int[2];
			array[0] = this._phases[num].Sum((MissionAgentSpawnLogic.SpawnPhase p) => p.TotalSpawnNumber);
			array[1] = this._phases[num2].Sum((MissionAgentSpawnLogic.SpawnPhase p) => p.TotalSpawnNumber);
			int[] array2 = array;
			int num3 = array2.Sum();
			if (this._spawnSettings.InitialTroopsSpawnMethod == MissionSpawnSettings.InitialSpawnMethod.BattleSizeAllocating)
			{
				float[] array3 = new float[]
				{
					(float)array2[num] / (float)num3,
					(float)array2[num2] / (float)num3
				};
				array3[num] = MathF.Min(this._spawnSettings.MaximumBattleSideRatio, array3[num] * this._spawnSettings.DefenderAdvantageFactor);
				array3[num2] = 1f - array3[num];
				int num4 = ((array3[num] < array3[num2]) ? 0 : 1);
				int oppositeSide = (int)((BattleSideEnum)num4).GetOppositeSide();
				int num5 = num4;
				if (array3[oppositeSide] > this._spawnSettings.MaximumBattleSideRatio)
				{
					array3[oppositeSide] = this._spawnSettings.MaximumBattleSideRatio;
					array3[num5] = 1f - this._spawnSettings.MaximumBattleSideRatio;
				}
				int[] array4 = new int[2];
				int num6 = MathF.Ceiling(array3[num5] * (float)this._battleSize);
				array4[num5] = Math.Min(num6, array2[num5]);
				array4[oppositeSide] = this._battleSize - array4[num5];
				for (int j = 0; j < 2; j++)
				{
					foreach (MissionAgentSpawnLogic.SpawnPhase spawnPhase in this._phases[j])
					{
						if (spawnPhase.InitialSpawnNumber > array4[j])
						{
							int num7 = array4[j];
							int num8 = spawnPhase.InitialSpawnNumber - num7;
							spawnPhase.InitialSpawnNumber = num7;
							spawnPhase.RemainingSpawnNumber += num8;
						}
					}
				}
			}
			else if (this._spawnSettings.InitialTroopsSpawnMethod == MissionSpawnSettings.InitialSpawnMethod.FreeAllocation)
			{
				this._phases[num].Max((MissionAgentSpawnLogic.SpawnPhase p) => p.InitialSpawnNumber);
				this._phases[num2].Max((MissionAgentSpawnLogic.SpawnPhase p) => p.InitialSpawnNumber);
			}
			if (this._spawnSettings.ReinforcementTroopsSpawnMethod == MissionSpawnSettings.ReinforcementSpawnMethod.Wave)
			{
				for (int k = 0; k < 2; k++)
				{
					foreach (MissionAgentSpawnLogic.SpawnPhase spawnPhase2 in this._phases[k])
					{
						int num9 = (int)Math.Max(1f, (float)spawnPhase2.InitialSpawnNumber * this._spawnSettings.ReinforcementWavePercentage);
						if (this._spawnSettings.MaximumReinforcementWaveCount > 0)
						{
							int num10 = Math.Min(spawnPhase2.RemainingSpawnNumber, num9 * this._spawnSettings.MaximumReinforcementWaveCount);
							int num11 = Math.Max(0, spawnPhase2.RemainingSpawnNumber - num10);
							this._numberOfTroopsInTotal[k] -= num11;
							array2[k] -= num11;
							spawnPhase2.RemainingSpawnNumber = num10;
							spawnPhase2.TotalSpawnNumber = spawnPhase2.RemainingSpawnNumber + spawnPhase2.InitialSpawnNumber;
						}
					}
				}
			}
			base.Mission.SetBattleAgentCount(MathF.Min(this.DefenderActivePhase.InitialSpawnNumber, this.AttackerActivePhase.InitialSpawnNumber));
			base.Mission.SetInitialAgentCountForSide(BattleSideEnum.Defender, array2[num]);
			base.Mission.SetInitialAgentCountForSide(BattleSideEnum.Attacker, array2[num2]);
			this._missionSides[num].SetSpawnTroops(spawnDefenders);
			this._missionSides[num2].SetSpawnTroops(spawnAttackers);
		}

		private void AddPhase(BattleSideEnum side, int totalSpawn, int initialSpawn)
		{
			MissionAgentSpawnLogic.SpawnPhase spawnPhase = new MissionAgentSpawnLogic.SpawnPhase
			{
				TotalSpawnNumber = totalSpawn,
				InitialSpawnNumber = initialSpawn,
				RemainingSpawnNumber = totalSpawn - initialSpawn
			};
			this._phases[(int)side].Add(spawnPhase);
			this._numberOfTroopsInTotal[(int)side] += totalSpawn;
		}

		private void PhaseTick()
		{
			for (int i = 0; i < 2; i++)
			{
				MissionAgentSpawnLogic.SpawnPhase activePhaseForSide = this.GetActivePhaseForSide((BattleSideEnum)i);
				activePhaseForSide.NumberActiveTroops = this._missionSides[i].NumberOfActiveTroops;
				if (activePhaseForSide.NumberActiveTroops == 0 && activePhaseForSide.RemainingSpawnNumber == 0 && this._phases[i].Count > 1)
				{
					this._phases[i].Remove(activePhaseForSide);
					BattleSideEnum battleSideEnum = (BattleSideEnum)i;
					if (this.GetActivePhaseForSide(battleSideEnum) != null)
					{
						if (this._onPhaseChanged[i] != null)
						{
							this._onPhaseChanged[i]();
						}
						IMissionDeploymentPlan deploymentPlan = base.Mission.DeploymentPlan;
						if (deploymentPlan.IsPlanMadeForBattleSide(battleSideEnum, DeploymentPlanType.Initial))
						{
							base.Mission.ClearAddedTroopsInDeploymentPlan(battleSideEnum, DeploymentPlanType.Initial);
							base.Mission.ClearDeploymentPlanForSide(battleSideEnum, DeploymentPlanType.Initial);
						}
						if (deploymentPlan.IsPlanMadeForBattleSide(battleSideEnum, DeploymentPlanType.Reinforcement))
						{
							base.Mission.ClearAddedTroopsInDeploymentPlan(battleSideEnum, DeploymentPlanType.Reinforcement);
							base.Mission.ClearDeploymentPlanForSide(battleSideEnum, DeploymentPlanType.Reinforcement);
						}
						Debug.Print("New spawn phase!", 0, Debug.DebugColor.Green, 64UL);
					}
				}
			}
		}

		private bool CheckDeployment()
		{
			bool flag = this.IsDeploymentOver;
			if (!flag)
			{
				for (int i = 0; i < 2; i++)
				{
					BattleSideEnum battleSideEnum = (BattleSideEnum)i;
					MissionAgentSpawnLogic.SpawnPhase activePhaseForSide = this.GetActivePhaseForSide(battleSideEnum);
					if (!base.Mission.DeploymentPlan.IsPlanMadeForBattleSide(battleSideEnum, DeploymentPlanType.Initial))
					{
						if (activePhaseForSide.InitialSpawnNumber > 0)
						{
							this._missionSides[i].ReserveTroops(activePhaseForSide.InitialSpawnNumber);
							this._missionSides[i].GetFormationSpawnData(this._formationSpawnData);
							for (int j = 0; j < this._formationSpawnData.Length; j++)
							{
								if (this._formationSpawnData[j].NumTroops > 0)
								{
									base.Mission.AddTroopsToDeploymentPlan(battleSideEnum, DeploymentPlanType.Initial, (FormationClass)j, this._formationSpawnData[j].FootTroopCount, this._formationSpawnData[j].MountedTroopCount);
								}
							}
						}
						float num = 0f;
						if (base.Mission.HasSpawnPath)
						{
							int battleSizeForActivePhase = this.GetBattleSizeForActivePhase();
							Path initialSpawnPath = base.Mission.GetInitialSpawnPath();
							num = Mission.GetBattleSizeOffset(battleSizeForActivePhase, initialSpawnPath);
						}
						base.Mission.MakeDeploymentPlanForSide(battleSideEnum, DeploymentPlanType.Initial, num);
					}
					if (!base.Mission.DeploymentPlan.IsPlanMadeForBattleSide(battleSideEnum, DeploymentPlanType.Reinforcement))
					{
						int num2 = Math.Max(this._battleSize / (2 * this._formationSpawnData.Length), 1);
						for (int k = 0; k < this._formationSpawnData.Length; k++)
						{
							if (((FormationClass)k).IsMounted())
							{
								base.Mission.AddTroopsToDeploymentPlan(battleSideEnum, DeploymentPlanType.Reinforcement, (FormationClass)k, 0, num2);
							}
							else
							{
								base.Mission.AddTroopsToDeploymentPlan(battleSideEnum, DeploymentPlanType.Reinforcement, (FormationClass)k, num2, 0);
							}
						}
						base.Mission.MakeDeploymentPlanForSide(battleSideEnum, DeploymentPlanType.Reinforcement, 0f);
					}
				}
				for (int l = 0; l < 2; l++)
				{
					BattleSideEnum battleSideEnum2 = (BattleSideEnum)l;
					MissionAgentSpawnLogic.SpawnPhase activePhaseForSide2 = this.GetActivePhaseForSide(battleSideEnum2);
					if (base.Mission.DeploymentPlan.IsPlanMadeForBattleSide(battleSideEnum2, DeploymentPlanType.Initial) && activePhaseForSide2.InitialSpawnNumber > 0 && this._missionSides[l].TroopSpawnActive)
					{
						int initialSpawnNumber = activePhaseForSide2.InitialSpawnNumber;
						this._missionSides[l].SpawnTroops(initialSpawnNumber, false);
						this.GetActivePhaseForSide(battleSideEnum2).OnInitialTroopsSpawned();
						this._missionSides[l].OnInitialSpawnOver();
						if (!this._sidesWhereSpawnOccured.Contains(battleSideEnum2))
						{
							this._sidesWhereSpawnOccured.Add(battleSideEnum2);
						}
						Action<BattleSideEnum, int> onInitialTroopsSpawned = this.OnInitialTroopsSpawned;
						if (onInitialTroopsSpawned != null)
						{
							onInitialTroopsSpawned(battleSideEnum2, initialSpawnNumber);
						}
					}
				}
				flag = this.IsDeploymentOver;
				if (flag)
				{
					foreach (BattleSideEnum battleSideEnum3 in this._sidesWhereSpawnOccured)
					{
						this.OnBattleSideDeployed(battleSideEnum3);
					}
				}
			}
			return flag;
		}

		private void CheckReinforcementSpawn()
		{
			int num = 0;
			int num2 = 1;
			MissionAgentSpawnLogic.MissionSide missionSide = this._missionSides[num];
			MissionAgentSpawnLogic.MissionSide missionSide2 = this._missionSides[num2];
			bool flag = missionSide.HasSpawnableReinforcements && ((float)missionSide.ReinforcementsSpawnedInLastBatch < missionSide.ReinforcementBatchSize || missionSide.ReinforcementBatchPriority >= missionSide2.ReinforcementBatchPriority);
			bool flag2 = missionSide2.HasSpawnableReinforcements && ((float)missionSide2.ReinforcementsSpawnedInLastBatch < missionSide2.ReinforcementBatchSize || missionSide2.ReinforcementBatchPriority >= missionSide.ReinforcementBatchPriority);
			int num3 = 0;
			if (flag && flag2)
			{
				if (missionSide.ReinforcementBatchPriority >= missionSide2.ReinforcementBatchPriority)
				{
					int num4 = missionSide.TryReinforcementSpawn();
					this.DefenderActivePhase.RemainingSpawnNumber -= num4;
					num3 += num4;
					num4 = missionSide2.TryReinforcementSpawn();
					this.AttackerActivePhase.RemainingSpawnNumber -= num4;
					num3 += num4;
				}
				else
				{
					int num4 = missionSide2.TryReinforcementSpawn();
					this.AttackerActivePhase.RemainingSpawnNumber -= num4;
					num3 += num4;
					num4 = missionSide.TryReinforcementSpawn();
					this.DefenderActivePhase.RemainingSpawnNumber -= num4;
					num3 += num4;
				}
			}
			else if (flag)
			{
				int num4 = missionSide.TryReinforcementSpawn();
				this.DefenderActivePhase.RemainingSpawnNumber -= num4;
				num3 += num4;
			}
			else if (flag2)
			{
				int num4 = missionSide2.TryReinforcementSpawn();
				this.AttackerActivePhase.RemainingSpawnNumber -= num4;
				num3 += num4;
			}
			if (num3 > 0)
			{
				for (int i = 0; i < 2; i++)
				{
					this.NotifyReinforcementTroopsSpawned((BattleSideEnum)i, true);
				}
			}
		}

		private void NotifyReinforcementTroopsSpawned(BattleSideEnum battleSide, bool checkEmptyReserves = false)
		{
			MissionAgentSpawnLogic.MissionSide missionSide = this._missionSides[(int)battleSide];
			int reinforcementsSpawnedInLastBatch = missionSide.ReinforcementsSpawnedInLastBatch;
			if (!missionSide.ReinforcementsNotifiedOnLastBatch && reinforcementsSpawnedInLastBatch > 0 && (!checkEmptyReserves || (checkEmptyReserves && !missionSide.HasReservedTroops)))
			{
				Action<BattleSideEnum, int> onReinforcementsSpawned = this.OnReinforcementsSpawned;
				if (onReinforcementsSpawned != null)
				{
					onReinforcementsSpawned(battleSide, reinforcementsSpawnedInLastBatch);
				}
				missionSide.SetReinforcementsNotifiedOnLastBatch(true);
			}
		}

		private void OrderController_OnOrderIssued(OrderType orderType, MBReadOnlyList<Formation> appliedFormations, params object[] delegateParams)
		{
			DeploymentHandler.OrderController_OnOrderIssued_Aux(orderType, appliedFormations, delegateParams);
		}

		private int GetBattleSizeForActivePhase()
		{
			return MathF.Max(this.DefenderActivePhase.TotalSpawnNumber, this.AttackerActivePhase.TotalSpawnNumber);
		}

		private MissionAgentSpawnLogic.SpawnPhase GetActivePhaseForSide(BattleSideEnum side)
		{
			if (side == BattleSideEnum.Defender)
			{
				return this.DefenderActivePhase;
			}
			if (side != BattleSideEnum.Attacker)
			{
				Debug.FailedAssert("Wrong Side", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\MissionLogics\\MissionAgentSpawnLogic.cs", "GetActivePhaseForSide", 1507);
				return null;
			}
			return this.AttackerActivePhase;
		}

		private static int _maxNumberOfAgentsForMissionCache;

		private readonly MissionAgentSpawnLogic.OnPhaseChangedDelegate[] _onPhaseChanged = new MissionAgentSpawnLogic.OnPhaseChangedDelegate[2];

		private readonly List<MissionAgentSpawnLogic.SpawnPhase>[] _phases;

		private readonly int[] _numberOfTroopsInTotal;

		private readonly MissionAgentSpawnLogic.FormationSpawnData[] _formationSpawnData;

		private readonly int _battleSize;

		private bool _reinforcementSpawnEnabled = true;

		private bool _spawningReinforcements;

		private readonly BasicMissionTimer _globalReinforcementSpawnTimer;

		private ICustomReinforcementSpawnTimer _customReinforcementSpawnTimer;

		private float _globalReinforcementInterval;

		private MissionSpawnSettings _spawnSettings;

		private readonly MissionAgentSpawnLogic.MissionSide[] _missionSides;

		private BannerBearerLogic _bannerBearerLogic;

		private List<BattleSideEnum> _sidesWhereSpawnOccured = new List<BattleSideEnum>();

		private readonly MissionAgentSpawnLogic.MissionSide _playerSide;

		private struct FormationSpawnData
		{
			public int NumTroops
			{
				get
				{
					return this.FootTroopCount + this.MountedTroopCount;
				}
			}

			public int FootTroopCount;

			public int MountedTroopCount;
		}

		private class MissionSide
		{
			public bool TroopSpawnActive { get; private set; }

			public bool IsPlayerSide { get; }

			public bool ReinforcementSpawnActive { get; private set; }

			public bool SpawnWithHorses
			{
				get
				{
					return this._spawnWithHorses;
				}
			}

			public bool ReinforcementsNotifiedOnLastBatch { get; private set; }

			public int NumberOfActiveTroops
			{
				get
				{
					return this._numSpawnedTroops - this._troopSupplier.NumRemovedTroops;
				}
			}

			public int ReinforcementQuotaRequirement
			{
				get
				{
					return this._reinforcementQuotaRequirement;
				}
			}

			public int ReinforcementsSpawnedInLastBatch
			{
				get
				{
					return this._reinforcementsSpawnedInLastBatch;
				}
			}

			public float ReinforcementBatchSize
			{
				get
				{
					return (float)this._reinforcementBatchSize;
				}
			}

			public bool HasReservedTroops
			{
				get
				{
					return this._reservedTroops.Count > 0;
				}
			}

			public float ReinforcementBatchPriority
			{
				get
				{
					return this._reinforcementBatchPriority;
				}
			}

			public int GetNumberOfPlayerControllableTroops()
			{
				return this._troopSupplier.GetNumberOfPlayerControllableTroops();
			}

			public int ReservedTroopsCount
			{
				get
				{
					return this._reservedTroops.Count;
				}
			}

			public MissionSide(MissionAgentSpawnLogic spawnLogic, BattleSideEnum side, IMissionTroopSupplier troopSupplier, bool isPlayerSide)
			{
				this._spawnLogic = spawnLogic;
				this._side = side;
				this._spawnWithHorses = true;
				this._spawnedFormations = new MBArrayList<Formation>();
				this._troopSupplier = troopSupplier;
				this._reinforcementQuotaRequirement = 0;
				this._reinforcementBatchSize = 0;
				this._reinforcementSpawnedUnitCountPerFormation = new ValueTuple<int, int>[8];
				this._reinforcementTroopFormationAssignments = new Dictionary<IAgentOriginBase, int>();
				this.IsPlayerSide = isPlayerSide;
				this.ReinforcementsNotifiedOnLastBatch = false;
			}

			public int TryReinforcementSpawn()
			{
				int num = 0;
				if (this.ReinforcementSpawnActive && this.TroopSpawnActive && this._reservedTroops.Count > 0)
				{
					int num2 = MissionAgentSpawnLogic.MaxNumberOfAgentsForMission - this._spawnLogic.NumberOfAgents;
					int reservedTroopQuota = this.GetReservedTroopQuota(0);
					if (num2 >= reservedTroopQuota)
					{
						num = this.SpawnTroops(1, true);
						if (num > 0)
						{
							this._reinforcementQuotaRequirement -= reservedTroopQuota;
							if (this._reservedTroops.Count >= this._reinforcementBatchSize)
							{
								this._reinforcementQuotaRequirement += this.GetReservedTroopQuota(this._reinforcementBatchSize - 1);
							}
							this._reinforcementBatchPriority /= 2f;
						}
					}
				}
				this._reinforcementsSpawnedInLastBatch += num;
				return num;
			}

			public void GetFormationSpawnData(MissionAgentSpawnLogic.FormationSpawnData[] formationSpawnData)
			{
				if (formationSpawnData != null && formationSpawnData.Length == 11)
				{
					for (int i = 0; i < formationSpawnData.Length; i++)
					{
						formationSpawnData[i].FootTroopCount = 0;
						formationSpawnData[i].MountedTroopCount = 0;
					}
					using (List<IAgentOriginBase>.Enumerator enumerator = this._reservedTroops.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							IAgentOriginBase agentOriginBase = enumerator.Current;
							FormationClass formationClass = agentOriginBase.Troop.GetFormationClass();
							if (agentOriginBase.Troop.HasMount())
							{
								FormationClass formationClass2 = formationClass;
								formationSpawnData[(int)formationClass2].MountedTroopCount = formationSpawnData[(int)formationClass2].MountedTroopCount + 1;
							}
							else
							{
								FormationClass formationClass3 = formationClass;
								formationSpawnData[(int)formationClass3].FootTroopCount = formationSpawnData[(int)formationClass3].FootTroopCount + 1;
							}
						}
						return;
					}
				}
				Debug.FailedAssert("Formation troop counts parameter is not set correctly.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\MissionLogics\\MissionAgentSpawnLogic.cs", "GetFormationSpawnData", 155);
			}

			public void ReserveTroops(int number)
			{
				if (number > 0 && this._troopSupplier.AnyTroopRemainsToBeSupplied)
				{
					this._reservedTroops.AddRange(this._troopSupplier.SupplyTroops(number));
				}
			}

			public bool HasSpawnableReinforcements
			{
				get
				{
					return this.ReinforcementSpawnActive && this.HasReservedTroops && this.ReinforcementBatchSize > 0f;
				}
			}

			public BasicCharacterObject GetGeneralCharacter()
			{
				return this._troopSupplier.GetGeneralCharacter();
			}

			public unsafe bool CheckReinforcementBatch()
			{
				MissionAgentSpawnLogic.SpawnPhase spawnPhase = ((this._side == BattleSideEnum.Defender) ? this._spawnLogic.DefenderActivePhase : this._spawnLogic.AttackerActivePhase);
				this._reinforcementsSpawnedInLastBatch = 0;
				this.ReinforcementsNotifiedOnLastBatch = false;
				int num = 0;
				MissionSpawnSettings missionSpawnSettings = *this._spawnLogic.ReinforcementSpawnSettings;
				switch (missionSpawnSettings.ReinforcementTroopsSpawnMethod)
				{
				case MissionSpawnSettings.ReinforcementSpawnMethod.Balanced:
					num = this.ComputeBalancedBatch(spawnPhase);
					break;
				case MissionSpawnSettings.ReinforcementSpawnMethod.Wave:
					num = this.ComputeWaveBatch(spawnPhase);
					break;
				case MissionSpawnSettings.ReinforcementSpawnMethod.Fixed:
					num = this.ComputeFixedBatch(spawnPhase);
					break;
				}
				num = Math.Min(num, spawnPhase.RemainingSpawnNumber);
				num -= this._reservedTroops.Count;
				if (num > 0)
				{
					int count = this._reservedTroops.Count;
					this.ReserveTroops(num);
					if (count < this._reinforcementBatchSize)
					{
						int num2 = Math.Min(this._reservedTroops.Count, this._reinforcementBatchSize);
						for (int i = count; i < num2; i++)
						{
							this._reinforcementQuotaRequirement += this.GetReservedTroopQuota(i);
						}
					}
				}
				this._reinforcementBatchPriority = (float)this._reservedTroops.Count;
				bool flag;
				if (missionSpawnSettings.ReinforcementTroopsSpawnMethod == MissionSpawnSettings.ReinforcementSpawnMethod.Wave)
				{
					flag = this._reservedTroops.Count > 0;
				}
				else
				{
					flag = this._reservedTroops.Count > 0 && (this._reservedTroops.Count >= this._reinforcementBatchSize || spawnPhase.RemainingSpawnNumber <= this._reinforcementBatchSize);
				}
				this.ReinforcementSpawnActive = flag;
				if (this.ReinforcementSpawnActive)
				{
					this.ResetReinforcementSpawnedUnitCountsPerFormation();
					Mission.Current.UpdateReinforcementPlan(this._side);
				}
				return this.ReinforcementSpawnActive;
			}

			public IEnumerable<IAgentOriginBase> GetAllTroops()
			{
				return this._troopSupplier.GetAllTroops();
			}

			public int SpawnTroops(int number, bool isReinforcement)
			{
				if (number <= 0)
				{
					return 0;
				}
				List<IAgentOriginBase> list = new List<IAgentOriginBase>();
				int num = MathF.Min(this._reservedTroops.Count, number);
				if (num > 0)
				{
					for (int i = 0; i < num; i++)
					{
						IAgentOriginBase agentOriginBase = this._reservedTroops[i];
						list.Add(agentOriginBase);
					}
					this._reservedTroops.RemoveRange(0, num);
				}
				int num2 = number - num;
				list.AddRange(this._troopSupplier.SupplyTroops(num2));
				Mission mission = Mission.Current;
				if (this._troopOriginsToSpawnPerTeam == null)
				{
					this._troopOriginsToSpawnPerTeam = new List<ValueTuple<Team, List<IAgentOriginBase>>>();
					using (List<Team>.Enumerator enumerator = mission.Teams.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Team team = enumerator.Current;
							bool flag = team.Side == mission.PlayerTeam.Side;
							if ((this.IsPlayerSide && flag) || (!this.IsPlayerSide && !flag))
							{
								this._troopOriginsToSpawnPerTeam.Add(new ValueTuple<Team, List<IAgentOriginBase>>(team, new List<IAgentOriginBase>()));
							}
						}
						goto IL_136;
					}
				}
				foreach (ValueTuple<Team, List<IAgentOriginBase>> valueTuple in this._troopOriginsToSpawnPerTeam)
				{
					valueTuple.Item2.Clear();
				}
				IL_136:
				int num3 = 0;
				foreach (IAgentOriginBase agentOriginBase2 in list)
				{
					Team agentTeam = Mission.GetAgentTeam(agentOriginBase2, this.IsPlayerSide);
					foreach (ValueTuple<Team, List<IAgentOriginBase>> valueTuple2 in this._troopOriginsToSpawnPerTeam)
					{
						if (agentTeam == valueTuple2.Item1)
						{
							num3++;
							valueTuple2.Item2.Add(agentOriginBase2);
						}
					}
				}
				int num4 = 0;
				List<IAgentOriginBase> list2 = new List<IAgentOriginBase>();
				foreach (ValueTuple<Team, List<IAgentOriginBase>> valueTuple3 in this._troopOriginsToSpawnPerTeam)
				{
					if (!valueTuple3.Item2.IsEmpty<IAgentOriginBase>())
					{
						int num5 = 0;
						int num6 = 0;
						int num7 = 0;
						List<ValueTuple<IAgentOriginBase, int>> list3 = null;
						if (isReinforcement)
						{
							list3 = new List<ValueTuple<IAgentOriginBase, int>>();
							using (List<IAgentOriginBase>.Enumerator enumerator3 = valueTuple3.Item2.GetEnumerator())
							{
								while (enumerator3.MoveNext())
								{
									IAgentOriginBase agentOriginBase3 = enumerator3.Current;
									int num8;
									this._reinforcementTroopFormationAssignments.TryGetValue(agentOriginBase3, out num8);
									list3.Add(new ValueTuple<IAgentOriginBase, int>(agentOriginBase3, num8));
								}
								goto IL_280;
							}
							goto IL_262;
						}
						goto IL_262;
						IL_280:
						for (int j = 0; j < 8; j++)
						{
							list2.Clear();
							IAgentOriginBase agentOriginBase4 = null;
							foreach (ValueTuple<IAgentOriginBase, int> valueTuple4 in list3)
							{
								IAgentOriginBase item = valueTuple4.Item1;
								int item2 = valueTuple4.Item2;
								if (j == item2)
								{
									if (item.Troop == Game.Current.PlayerTroop)
									{
										agentOriginBase4 = item;
									}
									else
									{
										if (item.Troop.HasMount())
										{
											num6++;
										}
										else
										{
											num7++;
										}
										list2.Add(item);
									}
								}
							}
							if (agentOriginBase4 != null)
							{
								if (agentOriginBase4.Troop.HasMount())
								{
									num6++;
								}
								else
								{
									num7++;
								}
								list2.Add(agentOriginBase4);
							}
							int count = list2.Count;
							if (count > 0)
							{
								bool flag2 = this._spawnWithHorses && MissionDeploymentPlan.HasSignificantMountedTroops(num7, num6);
								int num9 = 0;
								int num10 = count;
								if (this.ReinforcementSpawnActive)
								{
									num9 = this._reinforcementSpawnedUnitCountPerFormation[j].Item1;
									num10 = this._reinforcementSpawnedUnitCountPerFormation[j].Item2;
								}
								foreach (IAgentOriginBase agentOriginBase5 in list2)
								{
									Formation formation = valueTuple3.Item1.GetFormation((FormationClass)j);
									if (formation != null && !formation.HasBeenPositioned)
									{
										formation.BeginSpawn(num10, flag2);
										mission.SpawnFormation(formation);
										this._spawnedFormations.Add(formation);
									}
									if (this._bannerBearerLogic != null && mission.Mode != MissionMode.Deployment && this._bannerBearerLogic.GetMissingBannerCount(formation) > 0)
									{
										this._bannerBearerLogic.SpawnBannerBearer(agentOriginBase5, this.IsPlayerSide, formation, this._spawnWithHorses, isReinforcement, num10, num9, true, true, false, null, null, null, mission.IsSallyOutBattle);
									}
									else
									{
										mission.SpawnTroop(agentOriginBase5, this.IsPlayerSide, true, this._spawnWithHorses, isReinforcement, num10, num9, true, true, false, null, null, null, null, formation.FormationIndex, mission.IsSallyOutBattle);
									}
									this._numSpawnedTroops++;
									num9++;
									num5++;
								}
								if (this.ReinforcementSpawnActive)
								{
									this._reinforcementSpawnedUnitCountPerFormation[j].Item1 = num9;
								}
							}
						}
						if (num5 > 0)
						{
							valueTuple3.Item1.QuerySystem.Expire();
						}
						num4 += num5;
						foreach (Formation formation2 in valueTuple3.Item1.FormationsIncludingEmpty)
						{
							if (formation2.CountOfUnits > 0 && formation2.IsSpawning)
							{
								formation2.EndSpawn();
							}
						}
						continue;
						IL_262:
						list3 = MissionGameModels.Current.BattleSpawnModel.GetInitialSpawnAssignments(this._side, valueTuple3.Item2);
						goto IL_280;
					}
				}
				return num4;
			}

			public void SetSpawnWithHorses(bool spawnWithHorses)
			{
				this._spawnWithHorses = spawnWithHorses;
			}

			private unsafe int ComputeBalancedBatch(MissionAgentSpawnLogic.SpawnPhase activePhase)
			{
				int num = 0;
				if (activePhase != null && activePhase.RemainingSpawnNumber > 0)
				{
					MissionSpawnSettings missionSpawnSettings = *this._spawnLogic.ReinforcementSpawnSettings;
					int reinforcementBatchSize = this._reinforcementBatchSize;
					this._reinforcementBatchSize = (int)((float)this._spawnLogic.BattleSize * missionSpawnSettings.ReinforcementBatchPercentage);
					if (reinforcementBatchSize != this._reinforcementBatchSize)
					{
						this.UpdateReinforcementQuotaRequirement(reinforcementBatchSize);
					}
					int num2 = activePhase.TotalSpawnNumber - activePhase.InitialSpawnedNumber;
					num = MathF.Max(1, this._reservedTroops.Count + (int)((float)num2 * missionSpawnSettings.DesiredReinforcementPercentage));
					num = MathF.Min(num, activePhase.InitialSpawnedNumber - this.NumberOfActiveTroops);
				}
				return num;
			}

			private unsafe int ComputeFixedBatch(MissionAgentSpawnLogic.SpawnPhase activePhase)
			{
				int num = 0;
				if (activePhase != null && activePhase.RemainingSpawnNumber > 0)
				{
					MissionSpawnSettings missionSpawnSettings = *this._spawnLogic.ReinforcementSpawnSettings;
					float num2 = ((this._side == BattleSideEnum.Defender) ? missionSpawnSettings.DefenderReinforcementBatchPercentage : missionSpawnSettings.AttackerReinforcementBatchPercentage);
					int reinforcementBatchSize = this._reinforcementBatchSize;
					this._reinforcementBatchSize = (int)((float)this._spawnLogic.TotalSpawnNumber * num2);
					if (reinforcementBatchSize != this._reinforcementBatchSize)
					{
						this.UpdateReinforcementQuotaRequirement(reinforcementBatchSize);
					}
					num = MathF.Max(1, this._reinforcementBatchSize);
				}
				return num;
			}

			private unsafe int ComputeWaveBatch(MissionAgentSpawnLogic.SpawnPhase activePhase)
			{
				int num = 0;
				if (activePhase != null && activePhase.RemainingSpawnNumber > 0 && this._reservedTroops.IsEmpty<IAgentOriginBase>())
				{
					MissionSpawnSettings missionSpawnSettings = *this._spawnLogic.ReinforcementSpawnSettings;
					int reinforcementBatchSize = this._reinforcementBatchSize;
					int num2 = (int)Math.Max(1f, (float)activePhase.InitialSpawnedNumber * missionSpawnSettings.ReinforcementWavePercentage);
					this._reinforcementBatchSize = num2;
					if (reinforcementBatchSize != this._reinforcementBatchSize)
					{
						this.UpdateReinforcementQuotaRequirement(reinforcementBatchSize);
					}
					if (activePhase.InitialSpawnedNumber - activePhase.NumberActiveTroops >= num2)
					{
						num = num2;
					}
				}
				return num;
			}

			public void SetBannerBearerLogic(BannerBearerLogic bannerBearerLogic)
			{
				this._bannerBearerLogic = bannerBearerLogic;
			}

			private void UpdateReinforcementQuotaRequirement(int previousBatchSize)
			{
				if (this._reinforcementBatchSize < previousBatchSize)
				{
					for (int i = MathF.Min(this._reservedTroops.Count - 1, previousBatchSize - 1); i >= this._reinforcementBatchSize; i--)
					{
						this._reinforcementQuotaRequirement -= this.GetReservedTroopQuota(i);
					}
					return;
				}
				if (this._reinforcementBatchSize > previousBatchSize)
				{
					int num = MathF.Min(this._reservedTroops.Count - 1, this._reinforcementBatchSize - 1);
					for (int j = previousBatchSize; j <= num; j++)
					{
						this._reinforcementQuotaRequirement += this.GetReservedTroopQuota(j);
					}
				}
			}

			public void SetReinforcementsNotifiedOnLastBatch(bool value)
			{
				this.ReinforcementsNotifiedOnLastBatch = value;
			}

			private void ResetReinforcementSpawnedUnitCountsPerFormation()
			{
				for (int i = 0; i < 8; i++)
				{
					this._reinforcementSpawnedUnitCountPerFormation[i].Item1 = 0;
					this._reinforcementSpawnedUnitCountPerFormation[i].Item2 = 0;
				}
				this._reinforcementTroopFormationAssignments.Clear();
				foreach (ValueTuple<IAgentOriginBase, int> valueTuple in MissionGameModels.Current.BattleSpawnModel.GetReinforcementAssignments(this._side, this._reservedTroops))
				{
					int item = valueTuple.Item2;
					this._reinforcementTroopFormationAssignments.Add(valueTuple.Item1, valueTuple.Item2);
					ValueTuple<int, int>[] reinforcementSpawnedUnitCountPerFormation = this._reinforcementSpawnedUnitCountPerFormation;
					int num = item;
					reinforcementSpawnedUnitCountPerFormation[num].Item2 = reinforcementSpawnedUnitCountPerFormation[num].Item2 + 1;
				}
			}

			public void SetSpawnTroops(bool spawnTroops)
			{
				this.TroopSpawnActive = spawnTroops;
			}

			private int GetReservedTroopQuota(int index)
			{
				if (!this._spawnWithHorses || !this._reservedTroops[index].Troop.IsMounted)
				{
					return 1;
				}
				return 2;
			}

			public void OnInitialSpawnOver()
			{
				foreach (Formation formation in this._spawnedFormations)
				{
					formation.EndSpawn();
				}
			}

			private readonly MissionAgentSpawnLogic _spawnLogic;

			private readonly BattleSideEnum _side;

			private readonly IMissionTroopSupplier _troopSupplier;

			private BannerBearerLogic _bannerBearerLogic;

			private readonly MBArrayList<Formation> _spawnedFormations;

			private bool _spawnWithHorses;

			private float _reinforcementBatchPriority;

			private int _reinforcementQuotaRequirement;

			private int _reinforcementBatchSize;

			private int _reinforcementsSpawnedInLastBatch;

			private int _numSpawnedTroops;

			private readonly List<IAgentOriginBase> _reservedTroops = new List<IAgentOriginBase>();

			[TupleElementNames(new string[] { "team", "origins" })]
			private List<ValueTuple<Team, List<IAgentOriginBase>>> _troopOriginsToSpawnPerTeam;

			[TupleElementNames(new string[] { "currentTroopIndex", "troopCount" })]
			private readonly ValueTuple<int, int>[] _reinforcementSpawnedUnitCountPerFormation;

			private readonly Dictionary<IAgentOriginBase, int> _reinforcementTroopFormationAssignments;
		}

		private class SpawnPhase
		{
			public void OnInitialTroopsSpawned()
			{
				this.InitialSpawnedNumber = this.InitialSpawnNumber;
				this.InitialSpawnNumber = 0;
			}

			public int TotalSpawnNumber;

			public int InitialSpawnedNumber;

			public int InitialSpawnNumber;

			public int RemainingSpawnNumber;

			public int NumberActiveTroops;
		}

		public delegate void OnPhaseChangedDelegate();
	}
}
