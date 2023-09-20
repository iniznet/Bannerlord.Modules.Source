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
	// Token: 0x0200026F RID: 623
	public class MissionAgentSpawnLogic : MissionLogic, IMissionAgentSpawnLogic, IMissionBehavior
	{
		// Token: 0x17000658 RID: 1624
		// (get) Token: 0x0600212D RID: 8493 RVA: 0x00078669 File Offset: 0x00076869
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

		// Token: 0x17000659 RID: 1625
		// (get) Token: 0x0600212E RID: 8494 RVA: 0x00078686 File Offset: 0x00076886
		private static int MaxNumberOfTroopsForMission
		{
			get
			{
				return MissionAgentSpawnLogic.MaxNumberOfAgentsForMission / 2;
			}
		}

		// Token: 0x1400002E RID: 46
		// (add) Token: 0x0600212F RID: 8495 RVA: 0x00078690 File Offset: 0x00076890
		// (remove) Token: 0x06002130 RID: 8496 RVA: 0x000786C8 File Offset: 0x000768C8
		public event Action<BattleSideEnum, int> OnReinforcementsSpawned;

		// Token: 0x1400002F RID: 47
		// (add) Token: 0x06002131 RID: 8497 RVA: 0x00078700 File Offset: 0x00076900
		// (remove) Token: 0x06002132 RID: 8498 RVA: 0x00078738 File Offset: 0x00076938
		public event Action<BattleSideEnum, int> OnInitialTroopsSpawned;

		// Token: 0x1700065A RID: 1626
		// (get) Token: 0x06002133 RID: 8499 RVA: 0x0007876D File Offset: 0x0007696D
		public int NumberOfAgents
		{
			get
			{
				return base.Mission.AllAgents.Count;
			}
		}

		// Token: 0x1700065B RID: 1627
		// (get) Token: 0x06002134 RID: 8500 RVA: 0x0007877F File Offset: 0x0007697F
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

		// Token: 0x1700065C RID: 1628
		// (get) Token: 0x06002135 RID: 8501 RVA: 0x000787A6 File Offset: 0x000769A6
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

		// Token: 0x1700065D RID: 1629
		// (get) Token: 0x06002136 RID: 8502 RVA: 0x000787B9 File Offset: 0x000769B9
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

		// Token: 0x1700065E RID: 1630
		// (get) Token: 0x06002137 RID: 8503 RVA: 0x000787CC File Offset: 0x000769CC
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

		// Token: 0x1700065F RID: 1631
		// (get) Token: 0x06002138 RID: 8504 RVA: 0x000787DF File Offset: 0x000769DF
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

		// Token: 0x17000660 RID: 1632
		// (get) Token: 0x06002139 RID: 8505 RVA: 0x000787F2 File Offset: 0x000769F2
		public int BattleSize
		{
			get
			{
				return this._battleSize;
			}
		}

		// Token: 0x17000661 RID: 1633
		// (get) Token: 0x0600213A RID: 8506 RVA: 0x000787FA File Offset: 0x000769FA
		public bool IsInitialSpawnOver
		{
			get
			{
				return this.DefenderActivePhase.InitialSpawnNumber + this.AttackerActivePhase.InitialSpawnNumber == 0;
			}
		}

		// Token: 0x17000662 RID: 1634
		// (get) Token: 0x0600213B RID: 8507 RVA: 0x00078816 File Offset: 0x00076A16
		public bool IsDeploymentOver
		{
			get
			{
				return base.Mission.GetMissionBehavior<BattleDeploymentHandler>() == null && this.IsInitialSpawnOver;
			}
		}

		// Token: 0x17000663 RID: 1635
		// (get) Token: 0x0600213C RID: 8508 RVA: 0x0007882D File Offset: 0x00076A2D
		public ref readonly MissionSpawnSettings ReinforcementSpawnSettings
		{
			get
			{
				return ref this._spawnSettings;
			}
		}

		// Token: 0x17000664 RID: 1636
		// (get) Token: 0x0600213D RID: 8509 RVA: 0x00078835 File Offset: 0x00076A35
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

		// Token: 0x17000665 RID: 1637
		// (get) Token: 0x0600213E RID: 8510 RVA: 0x0007885C File Offset: 0x00076A5C
		private MissionAgentSpawnLogic.SpawnPhase DefenderActivePhase
		{
			get
			{
				return this._phases[0].FirstOrDefault<MissionAgentSpawnLogic.SpawnPhase>();
			}
		}

		// Token: 0x17000666 RID: 1638
		// (get) Token: 0x0600213F RID: 8511 RVA: 0x0007886B File Offset: 0x00076A6B
		private MissionAgentSpawnLogic.SpawnPhase AttackerActivePhase
		{
			get
			{
				return this._phases[1].FirstOrDefault<MissionAgentSpawnLogic.SpawnPhase>();
			}
		}

		// Token: 0x06002140 RID: 8512 RVA: 0x0007887C File Offset: 0x00076A7C
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

		// Token: 0x06002141 RID: 8513 RVA: 0x000788D0 File Offset: 0x00076AD0
		public int GetNumberOfPlayerControllableTroops()
		{
			MissionAgentSpawnLogic.MissionSide playerSide = this._playerSide;
			if (playerSide == null)
			{
				return 0;
			}
			return playerSide.GetNumberOfPlayerControllableTroops();
		}

		// Token: 0x06002142 RID: 8514 RVA: 0x000788E3 File Offset: 0x00076AE3
		public void InitWithSinglePhase(int defenderTotalSpawn, int attackerTotalSpawn, int defenderInitialSpawn, int attackerInitialSpawn, bool spawnDefenders, bool spawnAttackers, in MissionSpawnSettings spawnSettings)
		{
			this.AddPhase(BattleSideEnum.Defender, defenderTotalSpawn, defenderInitialSpawn);
			this.AddPhase(BattleSideEnum.Attacker, attackerTotalSpawn, attackerInitialSpawn);
			this.Init(spawnDefenders, spawnAttackers, spawnSettings);
		}

		// Token: 0x06002143 RID: 8515 RVA: 0x00078904 File Offset: 0x00076B04
		public IEnumerable<IAgentOriginBase> GetAllTroopsForSide(BattleSideEnum side)
		{
			return this._missionSides[(int)side].GetAllTroops();
		}

		// Token: 0x06002144 RID: 8516 RVA: 0x00078920 File Offset: 0x00076B20
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

		// Token: 0x06002145 RID: 8517 RVA: 0x00078984 File Offset: 0x00076B84
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

		// Token: 0x06002146 RID: 8518 RVA: 0x00078AB3 File Offset: 0x00076CB3
		public void SetCustomReinforcementSpawnTimer(ICustomReinforcementSpawnTimer timer)
		{
			this._customReinforcementSpawnTimer = timer;
		}

		// Token: 0x06002147 RID: 8519 RVA: 0x00078ABC File Offset: 0x00076CBC
		public void SetSpawnTroops(BattleSideEnum side, bool spawnTroops, bool enforceSpawning = false)
		{
			this._missionSides[(int)side].SetSpawnTroops(spawnTroops);
			if (spawnTroops && enforceSpawning)
			{
				this.CheckDeployment();
			}
		}

		// Token: 0x06002148 RID: 8520 RVA: 0x00078AD8 File Offset: 0x00076CD8
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			MissionGameModels.Current.BattleInitializationModel.InitializeModel();
		}

		// Token: 0x06002149 RID: 8521 RVA: 0x00078AEF File Offset: 0x00076CEF
		protected override void OnEndMission()
		{
			MissionGameModels.Current.BattleSpawnModel.OnMissionEnd();
			MissionGameModels.Current.BattleInitializationModel.FinalizeModel();
		}

		// Token: 0x0600214A RID: 8522 RVA: 0x00078B0F File Offset: 0x00076D0F
		public void SetSpawnHorses(BattleSideEnum side, bool spawnHorses)
		{
			this._missionSides[(int)side].SetSpawnWithHorses(spawnHorses);
			base.Mission.SetDeploymentPlanSpawnWithHorses(side, spawnHorses);
		}

		// Token: 0x0600214B RID: 8523 RVA: 0x00078B2C File Offset: 0x00076D2C
		public void StartSpawner(BattleSideEnum side)
		{
			this._missionSides[(int)side].SetSpawnTroops(true);
		}

		// Token: 0x0600214C RID: 8524 RVA: 0x00078B3C File Offset: 0x00076D3C
		public void StopSpawner(BattleSideEnum side)
		{
			this._missionSides[(int)side].SetSpawnTroops(false);
		}

		// Token: 0x0600214D RID: 8525 RVA: 0x00078B4C File Offset: 0x00076D4C
		public bool IsSideSpawnEnabled(BattleSideEnum side)
		{
			return this._missionSides[(int)side].TroopSpawnActive;
		}

		// Token: 0x0600214E RID: 8526 RVA: 0x00078B5C File Offset: 0x00076D5C
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

		// Token: 0x0600214F RID: 8527 RVA: 0x00078CEC File Offset: 0x00076EEC
		public float GetReinforcementInterval()
		{
			return this._globalReinforcementInterval;
		}

		// Token: 0x06002150 RID: 8528 RVA: 0x00078CF4 File Offset: 0x00076EF4
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

		// Token: 0x06002151 RID: 8529 RVA: 0x00078D53 File Offset: 0x00076F53
		public int GetTotalNumberOfTroopsForSide(BattleSideEnum side)
		{
			return this._numberOfTroopsInTotal[(int)side];
		}

		// Token: 0x06002152 RID: 8530 RVA: 0x00078D60 File Offset: 0x00076F60
		public BasicCharacterObject GetGeneralCharacterOfSide(BattleSideEnum side)
		{
			if (side >= BattleSideEnum.Defender && side < BattleSideEnum.NumSides)
			{
				this._missionSides[(int)side].GetGeneralCharacter();
			}
			return null;
		}

		// Token: 0x06002153 RID: 8531 RVA: 0x00078D86 File Offset: 0x00076F86
		public bool GetSpawnHorses(BattleSideEnum side)
		{
			return this._missionSides[(int)side].SpawnWithHorses;
		}

		// Token: 0x06002154 RID: 8532 RVA: 0x00078D98 File Offset: 0x00076F98
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

		// Token: 0x06002155 RID: 8533 RVA: 0x00078DD8 File Offset: 0x00076FD8
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

		// Token: 0x06002156 RID: 8534 RVA: 0x00078E40 File Offset: 0x00077040
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

		// Token: 0x06002157 RID: 8535 RVA: 0x00078EBF File Offset: 0x000770BF
		public bool IsSideDepleted(BattleSideEnum side)
		{
			return this._phases[(int)side].Count == 1 && this._missionSides[(int)side].NumberOfActiveTroops == 0 && this.GetActivePhaseForSide(side).RemainingSpawnNumber == 0;
		}

		// Token: 0x06002158 RID: 8536 RVA: 0x00078EF1 File Offset: 0x000770F1
		public void AddPhaseChangeAction(BattleSideEnum side, MissionAgentSpawnLogic.OnPhaseChangedDelegate onPhaseChanged)
		{
			MissionAgentSpawnLogic.OnPhaseChangedDelegate[] onPhaseChanged2 = this._onPhaseChanged;
			onPhaseChanged2[(int)side] = (MissionAgentSpawnLogic.OnPhaseChangedDelegate)Delegate.Combine(onPhaseChanged2[(int)side], onPhaseChanged);
		}

		// Token: 0x06002159 RID: 8537 RVA: 0x00078F10 File Offset: 0x00077110
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

		// Token: 0x0600215A RID: 8538 RVA: 0x00079324 File Offset: 0x00077524
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

		// Token: 0x0600215B RID: 8539 RVA: 0x00079370 File Offset: 0x00077570
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

		// Token: 0x0600215C RID: 8540 RVA: 0x00079468 File Offset: 0x00077668
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

		// Token: 0x0600215D RID: 8541 RVA: 0x0007971C File Offset: 0x0007791C
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

		// Token: 0x0600215E RID: 8542 RVA: 0x000798C0 File Offset: 0x00077AC0
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

		// Token: 0x0600215F RID: 8543 RVA: 0x00079911 File Offset: 0x00077B11
		private void OrderController_OnOrderIssued(OrderType orderType, MBReadOnlyList<Formation> appliedFormations, params object[] delegateParams)
		{
			DeploymentHandler.OrderController_OnOrderIssued_Aux(orderType, appliedFormations, delegateParams);
		}

		// Token: 0x06002160 RID: 8544 RVA: 0x0007991B File Offset: 0x00077B1B
		private int GetBattleSizeForActivePhase()
		{
			return MathF.Max(this.DefenderActivePhase.TotalSpawnNumber, this.AttackerActivePhase.TotalSpawnNumber);
		}

		// Token: 0x06002161 RID: 8545 RVA: 0x00079938 File Offset: 0x00077B38
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

		// Token: 0x04000C4A RID: 3146
		private static int _maxNumberOfAgentsForMissionCache;

		// Token: 0x04000C4D RID: 3149
		private readonly MissionAgentSpawnLogic.OnPhaseChangedDelegate[] _onPhaseChanged = new MissionAgentSpawnLogic.OnPhaseChangedDelegate[2];

		// Token: 0x04000C4E RID: 3150
		private readonly List<MissionAgentSpawnLogic.SpawnPhase>[] _phases;

		// Token: 0x04000C4F RID: 3151
		private readonly int[] _numberOfTroopsInTotal;

		// Token: 0x04000C50 RID: 3152
		private readonly MissionAgentSpawnLogic.FormationSpawnData[] _formationSpawnData;

		// Token: 0x04000C51 RID: 3153
		private readonly int _battleSize;

		// Token: 0x04000C52 RID: 3154
		private bool _reinforcementSpawnEnabled = true;

		// Token: 0x04000C53 RID: 3155
		private bool _spawningReinforcements;

		// Token: 0x04000C54 RID: 3156
		private readonly BasicMissionTimer _globalReinforcementSpawnTimer;

		// Token: 0x04000C55 RID: 3157
		private ICustomReinforcementSpawnTimer _customReinforcementSpawnTimer;

		// Token: 0x04000C56 RID: 3158
		private float _globalReinforcementInterval;

		// Token: 0x04000C57 RID: 3159
		private MissionSpawnSettings _spawnSettings;

		// Token: 0x04000C58 RID: 3160
		private readonly MissionAgentSpawnLogic.MissionSide[] _missionSides;

		// Token: 0x04000C59 RID: 3161
		private BannerBearerLogic _bannerBearerLogic;

		// Token: 0x04000C5A RID: 3162
		private List<BattleSideEnum> _sidesWhereSpawnOccured = new List<BattleSideEnum>();

		// Token: 0x04000C5B RID: 3163
		private readonly MissionAgentSpawnLogic.MissionSide _playerSide;

		// Token: 0x0200057A RID: 1402
		private struct FormationSpawnData
		{
			// Token: 0x1700098E RID: 2446
			// (get) Token: 0x06003AC9 RID: 15049 RVA: 0x000ECE0F File Offset: 0x000EB00F
			public int NumTroops
			{
				get
				{
					return this.FootTroopCount + this.MountedTroopCount;
				}
			}

			// Token: 0x04001D3B RID: 7483
			public int FootTroopCount;

			// Token: 0x04001D3C RID: 7484
			public int MountedTroopCount;
		}

		// Token: 0x0200057B RID: 1403
		private class MissionSide
		{
			// Token: 0x1700098F RID: 2447
			// (get) Token: 0x06003ACA RID: 15050 RVA: 0x000ECE1E File Offset: 0x000EB01E
			// (set) Token: 0x06003ACB RID: 15051 RVA: 0x000ECE26 File Offset: 0x000EB026
			public bool TroopSpawnActive { get; private set; }

			// Token: 0x17000990 RID: 2448
			// (get) Token: 0x06003ACC RID: 15052 RVA: 0x000ECE2F File Offset: 0x000EB02F
			public bool IsPlayerSide { get; }

			// Token: 0x17000991 RID: 2449
			// (get) Token: 0x06003ACD RID: 15053 RVA: 0x000ECE37 File Offset: 0x000EB037
			// (set) Token: 0x06003ACE RID: 15054 RVA: 0x000ECE3F File Offset: 0x000EB03F
			public bool ReinforcementSpawnActive { get; private set; }

			// Token: 0x17000992 RID: 2450
			// (get) Token: 0x06003ACF RID: 15055 RVA: 0x000ECE48 File Offset: 0x000EB048
			public bool SpawnWithHorses
			{
				get
				{
					return this._spawnWithHorses;
				}
			}

			// Token: 0x17000993 RID: 2451
			// (get) Token: 0x06003AD0 RID: 15056 RVA: 0x000ECE50 File Offset: 0x000EB050
			// (set) Token: 0x06003AD1 RID: 15057 RVA: 0x000ECE58 File Offset: 0x000EB058
			public bool ReinforcementsNotifiedOnLastBatch { get; private set; }

			// Token: 0x17000994 RID: 2452
			// (get) Token: 0x06003AD2 RID: 15058 RVA: 0x000ECE61 File Offset: 0x000EB061
			public int NumberOfActiveTroops
			{
				get
				{
					return this._numSpawnedTroops - this._troopSupplier.NumRemovedTroops;
				}
			}

			// Token: 0x17000995 RID: 2453
			// (get) Token: 0x06003AD3 RID: 15059 RVA: 0x000ECE75 File Offset: 0x000EB075
			public int ReinforcementQuotaRequirement
			{
				get
				{
					return this._reinforcementQuotaRequirement;
				}
			}

			// Token: 0x17000996 RID: 2454
			// (get) Token: 0x06003AD4 RID: 15060 RVA: 0x000ECE7D File Offset: 0x000EB07D
			public int ReinforcementsSpawnedInLastBatch
			{
				get
				{
					return this._reinforcementsSpawnedInLastBatch;
				}
			}

			// Token: 0x17000997 RID: 2455
			// (get) Token: 0x06003AD5 RID: 15061 RVA: 0x000ECE85 File Offset: 0x000EB085
			public float ReinforcementBatchSize
			{
				get
				{
					return (float)this._reinforcementBatchSize;
				}
			}

			// Token: 0x17000998 RID: 2456
			// (get) Token: 0x06003AD6 RID: 15062 RVA: 0x000ECE8E File Offset: 0x000EB08E
			public bool HasReservedTroops
			{
				get
				{
					return this._reservedTroops.Count > 0;
				}
			}

			// Token: 0x17000999 RID: 2457
			// (get) Token: 0x06003AD7 RID: 15063 RVA: 0x000ECE9E File Offset: 0x000EB09E
			public float ReinforcementBatchPriority
			{
				get
				{
					return this._reinforcementBatchPriority;
				}
			}

			// Token: 0x06003AD8 RID: 15064 RVA: 0x000ECEA6 File Offset: 0x000EB0A6
			public int GetNumberOfPlayerControllableTroops()
			{
				return this._troopSupplier.GetNumberOfPlayerControllableTroops();
			}

			// Token: 0x1700099A RID: 2458
			// (get) Token: 0x06003AD9 RID: 15065 RVA: 0x000ECEB3 File Offset: 0x000EB0B3
			public int ReservedTroopsCount
			{
				get
				{
					return this._reservedTroops.Count;
				}
			}

			// Token: 0x06003ADA RID: 15066 RVA: 0x000ECEC0 File Offset: 0x000EB0C0
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

			// Token: 0x06003ADB RID: 15067 RVA: 0x000ECF3C File Offset: 0x000EB13C
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

			// Token: 0x06003ADC RID: 15068 RVA: 0x000ECFF8 File Offset: 0x000EB1F8
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

			// Token: 0x06003ADD RID: 15069 RVA: 0x000ED0D0 File Offset: 0x000EB2D0
			public void ReserveTroops(int number)
			{
				if (number > 0 && this._troopSupplier.AnyTroopRemainsToBeSupplied)
				{
					this._reservedTroops.AddRange(this._troopSupplier.SupplyTroops(number));
				}
			}

			// Token: 0x1700099B RID: 2459
			// (get) Token: 0x06003ADE RID: 15070 RVA: 0x000ED0FA File Offset: 0x000EB2FA
			public bool HasSpawnableReinforcements
			{
				get
				{
					return this.ReinforcementSpawnActive && this.HasReservedTroops && this.ReinforcementBatchSize > 0f;
				}
			}

			// Token: 0x06003ADF RID: 15071 RVA: 0x000ED11B File Offset: 0x000EB31B
			public BasicCharacterObject GetGeneralCharacter()
			{
				return this._troopSupplier.GetGeneralCharacter();
			}

			// Token: 0x06003AE0 RID: 15072 RVA: 0x000ED128 File Offset: 0x000EB328
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

			// Token: 0x06003AE1 RID: 15073 RVA: 0x000ED2C0 File Offset: 0x000EB4C0
			public IEnumerable<IAgentOriginBase> GetAllTroops()
			{
				return this._troopSupplier.GetAllTroops();
			}

			// Token: 0x06003AE2 RID: 15074 RVA: 0x000ED2D0 File Offset: 0x000EB4D0
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

			// Token: 0x06003AE3 RID: 15075 RVA: 0x000ED934 File Offset: 0x000EBB34
			public void SetSpawnWithHorses(bool spawnWithHorses)
			{
				this._spawnWithHorses = spawnWithHorses;
			}

			// Token: 0x06003AE4 RID: 15076 RVA: 0x000ED940 File Offset: 0x000EBB40
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

			// Token: 0x06003AE5 RID: 15077 RVA: 0x000ED9E8 File Offset: 0x000EBBE8
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

			// Token: 0x06003AE6 RID: 15078 RVA: 0x000EDA68 File Offset: 0x000EBC68
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

			// Token: 0x06003AE7 RID: 15079 RVA: 0x000EDAED File Offset: 0x000EBCED
			public void SetBannerBearerLogic(BannerBearerLogic bannerBearerLogic)
			{
				this._bannerBearerLogic = bannerBearerLogic;
			}

			// Token: 0x06003AE8 RID: 15080 RVA: 0x000EDAF8 File Offset: 0x000EBCF8
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

			// Token: 0x06003AE9 RID: 15081 RVA: 0x000EDB8C File Offset: 0x000EBD8C
			public void SetReinforcementsNotifiedOnLastBatch(bool value)
			{
				this.ReinforcementsNotifiedOnLastBatch = value;
			}

			// Token: 0x06003AEA RID: 15082 RVA: 0x000EDB98 File Offset: 0x000EBD98
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

			// Token: 0x06003AEB RID: 15083 RVA: 0x000EDC68 File Offset: 0x000EBE68
			public void SetSpawnTroops(bool spawnTroops)
			{
				this.TroopSpawnActive = spawnTroops;
			}

			// Token: 0x06003AEC RID: 15084 RVA: 0x000EDC71 File Offset: 0x000EBE71
			private int GetReservedTroopQuota(int index)
			{
				if (!this._spawnWithHorses || !this._reservedTroops[index].Troop.IsMounted)
				{
					return 1;
				}
				return 2;
			}

			// Token: 0x06003AED RID: 15085 RVA: 0x000EDC98 File Offset: 0x000EBE98
			public void OnInitialSpawnOver()
			{
				foreach (Formation formation in this._spawnedFormations)
				{
					formation.EndSpawn();
				}
			}

			// Token: 0x04001D3D RID: 7485
			private readonly MissionAgentSpawnLogic _spawnLogic;

			// Token: 0x04001D3E RID: 7486
			private readonly BattleSideEnum _side;

			// Token: 0x04001D3F RID: 7487
			private readonly IMissionTroopSupplier _troopSupplier;

			// Token: 0x04001D40 RID: 7488
			private BannerBearerLogic _bannerBearerLogic;

			// Token: 0x04001D41 RID: 7489
			private readonly MBArrayList<Formation> _spawnedFormations;

			// Token: 0x04001D42 RID: 7490
			private bool _spawnWithHorses;

			// Token: 0x04001D43 RID: 7491
			private float _reinforcementBatchPriority;

			// Token: 0x04001D44 RID: 7492
			private int _reinforcementQuotaRequirement;

			// Token: 0x04001D45 RID: 7493
			private int _reinforcementBatchSize;

			// Token: 0x04001D46 RID: 7494
			private int _reinforcementsSpawnedInLastBatch;

			// Token: 0x04001D47 RID: 7495
			private int _numSpawnedTroops;

			// Token: 0x04001D48 RID: 7496
			private readonly List<IAgentOriginBase> _reservedTroops = new List<IAgentOriginBase>();

			// Token: 0x04001D49 RID: 7497
			[TupleElementNames(new string[] { "team", "origins" })]
			private List<ValueTuple<Team, List<IAgentOriginBase>>> _troopOriginsToSpawnPerTeam;

			// Token: 0x04001D4E RID: 7502
			[TupleElementNames(new string[] { "currentTroopIndex", "troopCount" })]
			private readonly ValueTuple<int, int>[] _reinforcementSpawnedUnitCountPerFormation;

			// Token: 0x04001D4F RID: 7503
			private readonly Dictionary<IAgentOriginBase, int> _reinforcementTroopFormationAssignments;
		}

		// Token: 0x0200057C RID: 1404
		private class SpawnPhase
		{
			// Token: 0x06003AEE RID: 15086 RVA: 0x000EDCE4 File Offset: 0x000EBEE4
			public void OnInitialTroopsSpawned()
			{
				this.InitialSpawnedNumber = this.InitialSpawnNumber;
				this.InitialSpawnNumber = 0;
			}

			// Token: 0x04001D50 RID: 7504
			public int TotalSpawnNumber;

			// Token: 0x04001D51 RID: 7505
			public int InitialSpawnedNumber;

			// Token: 0x04001D52 RID: 7506
			public int InitialSpawnNumber;

			// Token: 0x04001D53 RID: 7507
			public int RemainingSpawnNumber;

			// Token: 0x04001D54 RID: 7508
			public int NumberActiveTroops;
		}

		// Token: 0x0200057D RID: 1405
		// (Invoke) Token: 0x06003AF1 RID: 15089
		public delegate void OnPhaseChangedDelegate();
	}
}
