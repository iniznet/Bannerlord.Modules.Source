using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000390 RID: 912
	public class Team : IMissionTeam
	{
		// Token: 0x14000095 RID: 149
		// (add) Token: 0x060031E7 RID: 12775 RVA: 0x000CF520 File Offset: 0x000CD720
		// (remove) Token: 0x060031E8 RID: 12776 RVA: 0x000CF558 File Offset: 0x000CD758
		public event Action<Team, Formation> OnFormationsChanged;

		// Token: 0x14000096 RID: 150
		// (add) Token: 0x060031E9 RID: 12777 RVA: 0x000CF590 File Offset: 0x000CD790
		// (remove) Token: 0x060031EA RID: 12778 RVA: 0x000CF5C8 File Offset: 0x000CD7C8
		public event OnOrderIssuedDelegate OnOrderIssued;

		// Token: 0x14000097 RID: 151
		// (add) Token: 0x060031EB RID: 12779 RVA: 0x000CF600 File Offset: 0x000CD800
		// (remove) Token: 0x060031EC RID: 12780 RVA: 0x000CF638 File Offset: 0x000CD838
		public event Action<Formation> OnFormationAIActiveBehaviorChanged;

		// Token: 0x170008E2 RID: 2274
		// (get) Token: 0x060031ED RID: 12781 RVA: 0x000CF66D File Offset: 0x000CD86D
		public BattleSideEnum Side { get; }

		// Token: 0x170008E3 RID: 2275
		// (get) Token: 0x060031EE RID: 12782 RVA: 0x000CF675 File Offset: 0x000CD875
		public Mission Mission { get; }

		// Token: 0x170008E4 RID: 2276
		// (get) Token: 0x060031EF RID: 12783 RVA: 0x000CF67D File Offset: 0x000CD87D
		// (set) Token: 0x060031F0 RID: 12784 RVA: 0x000CF685 File Offset: 0x000CD885
		public MBList<Formation> FormationsIncludingEmpty { get; private set; }

		// Token: 0x170008E5 RID: 2277
		// (get) Token: 0x060031F1 RID: 12785 RVA: 0x000CF68E File Offset: 0x000CD88E
		// (set) Token: 0x060031F2 RID: 12786 RVA: 0x000CF696 File Offset: 0x000CD896
		public MBList<Formation> FormationsIncludingSpecialAndEmpty { get; private set; }

		// Token: 0x170008E6 RID: 2278
		// (get) Token: 0x060031F3 RID: 12787 RVA: 0x000CF69F File Offset: 0x000CD89F
		// (set) Token: 0x060031F4 RID: 12788 RVA: 0x000CF6A7 File Offset: 0x000CD8A7
		public TeamAIComponent TeamAI { get; private set; }

		// Token: 0x170008E7 RID: 2279
		// (get) Token: 0x060031F5 RID: 12789 RVA: 0x000CF6B0 File Offset: 0x000CD8B0
		public bool IsPlayerTeam
		{
			get
			{
				return this.Mission.PlayerTeam == this;
			}
		}

		// Token: 0x170008E8 RID: 2280
		// (get) Token: 0x060031F6 RID: 12790 RVA: 0x000CF6C0 File Offset: 0x000CD8C0
		public bool IsPlayerAlly
		{
			get
			{
				return this.Mission.PlayerTeam != null && this.Mission.PlayerTeam.Side == this.Side;
			}
		}

		// Token: 0x170008E9 RID: 2281
		// (get) Token: 0x060031F7 RID: 12791 RVA: 0x000CF6E9 File Offset: 0x000CD8E9
		public bool IsDefender
		{
			get
			{
				return this.Side == BattleSideEnum.Defender;
			}
		}

		// Token: 0x170008EA RID: 2282
		// (get) Token: 0x060031F8 RID: 12792 RVA: 0x000CF6F4 File Offset: 0x000CD8F4
		public bool IsAttacker
		{
			get
			{
				return this.Side == BattleSideEnum.Attacker;
			}
		}

		// Token: 0x170008EB RID: 2283
		// (get) Token: 0x060031F9 RID: 12793 RVA: 0x000CF6FF File Offset: 0x000CD8FF
		// (set) Token: 0x060031FA RID: 12794 RVA: 0x000CF707 File Offset: 0x000CD907
		public uint Color { get; private set; }

		// Token: 0x170008EC RID: 2284
		// (get) Token: 0x060031FB RID: 12795 RVA: 0x000CF710 File Offset: 0x000CD910
		// (set) Token: 0x060031FC RID: 12796 RVA: 0x000CF718 File Offset: 0x000CD918
		public uint Color2 { get; private set; }

		// Token: 0x170008ED RID: 2285
		// (get) Token: 0x060031FD RID: 12797 RVA: 0x000CF721 File Offset: 0x000CD921
		public Banner Banner { get; }

		// Token: 0x170008EE RID: 2286
		// (get) Token: 0x060031FE RID: 12798 RVA: 0x000CF729 File Offset: 0x000CD929
		public OrderController MasterOrderController
		{
			get
			{
				return this._orderControllers[0];
			}
		}

		// Token: 0x170008EF RID: 2287
		// (get) Token: 0x060031FF RID: 12799 RVA: 0x000CF737 File Offset: 0x000CD937
		public OrderController PlayerOrderController
		{
			get
			{
				return this._orderControllers[1];
			}
		}

		// Token: 0x170008F0 RID: 2288
		// (get) Token: 0x06003200 RID: 12800 RVA: 0x000CF745 File Offset: 0x000CD945
		// (set) Token: 0x06003201 RID: 12801 RVA: 0x000CF74D File Offset: 0x000CD94D
		public TeamQuerySystem QuerySystem { get; private set; }

		// Token: 0x170008F1 RID: 2289
		// (get) Token: 0x06003202 RID: 12802 RVA: 0x000CF756 File Offset: 0x000CD956
		// (set) Token: 0x06003203 RID: 12803 RVA: 0x000CF75E File Offset: 0x000CD95E
		public DetachmentManager DetachmentManager { get; private set; }

		// Token: 0x170008F2 RID: 2290
		// (get) Token: 0x06003204 RID: 12804 RVA: 0x000CF767 File Offset: 0x000CD967
		// (set) Token: 0x06003205 RID: 12805 RVA: 0x000CF76F File Offset: 0x000CD96F
		public bool IsPlayerGeneral { get; private set; }

		// Token: 0x170008F3 RID: 2291
		// (get) Token: 0x06003206 RID: 12806 RVA: 0x000CF778 File Offset: 0x000CD978
		// (set) Token: 0x06003207 RID: 12807 RVA: 0x000CF780 File Offset: 0x000CD980
		public bool IsPlayerSergeant { get; private set; }

		// Token: 0x170008F4 RID: 2292
		// (get) Token: 0x06003208 RID: 12808 RVA: 0x000CF789 File Offset: 0x000CD989
		public MBReadOnlyList<Agent> ActiveAgents
		{
			get
			{
				return this._activeAgents;
			}
		}

		// Token: 0x170008F5 RID: 2293
		// (get) Token: 0x06003209 RID: 12809 RVA: 0x000CF791 File Offset: 0x000CD991
		public MBReadOnlyList<Agent> TeamAgents
		{
			get
			{
				return this._teamAgents;
			}
		}

		// Token: 0x170008F6 RID: 2294
		// (get) Token: 0x0600320A RID: 12810 RVA: 0x000CF799 File Offset: 0x000CD999
		public MBReadOnlyList<ValueTuple<float, WorldPosition, int, Vec2, Vec2, bool>> CachedEnemyDataForFleeing
		{
			get
			{
				return this._cachedEnemyDataForFleeing;
			}
		}

		// Token: 0x170008F7 RID: 2295
		// (get) Token: 0x0600320B RID: 12811 RVA: 0x000CF7A1 File Offset: 0x000CD9A1
		public int TeamIndex
		{
			get
			{
				return this.MBTeam.Index;
			}
		}

		// Token: 0x0600320C RID: 12812 RVA: 0x000CF7B0 File Offset: 0x000CD9B0
		public Team(MBTeam mbTeam, BattleSideEnum side, Mission mission, uint color = 4294967295U, uint color2 = 4294967295U, Banner banner = null)
		{
			this.MBTeam = mbTeam;
			this.Side = side;
			this.Mission = mission;
			this.Color = color;
			this.Color2 = color2;
			this.Banner = banner;
			this.IsPlayerGeneral = true;
			this.IsPlayerSergeant = false;
			if (this != Team._invalid)
			{
				this.Initialize();
			}
			this.MoraleChangeFactor = 1f;
		}

		// Token: 0x0600320D RID: 12813 RVA: 0x000CF818 File Offset: 0x000CDA18
		public void UpdateCachedEnemyDataForFleeing()
		{
			if (this._cachedEnemyDataForFleeing.IsEmpty<ValueTuple<float, WorldPosition, int, Vec2, Vec2, bool>>())
			{
				foreach (Team team in this.Mission.Teams)
				{
					if (team.IsEnemyOf(this))
					{
						foreach (Formation formation in team.FormationsIncludingSpecialAndEmpty)
						{
							int countOfUnits = formation.CountOfUnits;
							if (countOfUnits > 0)
							{
								WorldPosition medianPosition = formation.QuerySystem.MedianPosition;
								float movementSpeedMaximum = formation.QuerySystem.MovementSpeedMaximum;
								bool flag = (formation.QuerySystem.IsCavalryFormation || formation.QuerySystem.IsRangedCavalryFormation) && formation.HasAnyMountedUnit;
								if (countOfUnits == 1)
								{
									Vec2 asVec = medianPosition.AsVec2;
									this._cachedEnemyDataForFleeing.Add(new ValueTuple<float, WorldPosition, int, Vec2, Vec2, bool>(movementSpeedMaximum, medianPosition, countOfUnits, asVec, asVec, flag));
								}
								else
								{
									Vec2 vec = formation.QuerySystem.EstimatedDirection.LeftVec();
									float num = formation.Width / 2f;
									Vec2 vec2 = medianPosition.AsVec2 - vec * num;
									Vec2 vec3 = medianPosition.AsVec2 + vec * num;
									this._cachedEnemyDataForFleeing.Add(new ValueTuple<float, WorldPosition, int, Vec2, Vec2, bool>(movementSpeedMaximum, medianPosition, countOfUnits, vec2, vec3, flag));
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x170008F8 RID: 2296
		// (get) Token: 0x0600320E RID: 12814 RVA: 0x000CF9D0 File Offset: 0x000CDBD0
		// (set) Token: 0x0600320F RID: 12815 RVA: 0x000CF9D8 File Offset: 0x000CDBD8
		public float MoraleChangeFactor { get; private set; }

		// Token: 0x06003210 RID: 12816 RVA: 0x000CF9E4 File Offset: 0x000CDBE4
		private void Initialize()
		{
			this._activeAgents = new MBList<Agent>();
			this._teamAgents = new MBList<Agent>();
			this._cachedEnemyDataForFleeing = new MBList<ValueTuple<float, WorldPosition, int, Vec2, Vec2, bool>>();
			if (!GameNetwork.IsReplay)
			{
				this.FormationsIncludingSpecialAndEmpty = new MBList<Formation>(10);
				this.FormationsIncludingEmpty = new MBList<Formation>(8);
				for (int i = 0; i < 10; i++)
				{
					Formation formation = new Formation(this, i);
					this.FormationsIncludingSpecialAndEmpty.Add(formation);
					if (i < 8)
					{
						this.FormationsIncludingEmpty.Add(formation);
					}
					formation.AI.OnActiveBehaviorChanged += this.FormationAI_OnActiveBehaviorChanged;
				}
				if (this.Mission != null)
				{
					this._orderControllers = new List<OrderController>();
					OrderController orderController = new OrderController(this.Mission, this, null);
					this._orderControllers.Add(orderController);
					orderController.OnOrderIssued += this.OrderController_OnOrderIssued;
					OrderController orderController2 = new OrderController(this.Mission, this, null);
					this._orderControllers.Add(orderController2);
					orderController2.OnOrderIssued += this.OrderController_OnOrderIssued;
				}
				this.QuerySystem = new TeamQuerySystem(this);
				this.DetachmentManager = new DetachmentManager(this);
			}
		}

		// Token: 0x06003211 RID: 12817 RVA: 0x000CFB00 File Offset: 0x000CDD00
		public void Reset()
		{
			if (!GameNetwork.IsReplay)
			{
				foreach (Formation formation in this.FormationsIncludingSpecialAndEmpty)
				{
					formation.Reset();
				}
				List<OrderController> orderControllers = this._orderControllers;
				if (orderControllers != null && orderControllers.Count > 2)
				{
					for (int i = this._orderControllers.Count - 1; i >= 2; i--)
					{
						this._orderControllers[i].OnOrderIssued -= this.OrderController_OnOrderIssued;
						this._orderControllers.RemoveAt(i);
					}
				}
				this.QuerySystem = new TeamQuerySystem(this);
			}
			this._teamAgents.Clear();
			this._activeAgents.Clear();
		}

		// Token: 0x06003212 RID: 12818 RVA: 0x000CFBD4 File Offset: 0x000CDDD4
		public void Clear()
		{
			if (!GameNetwork.IsReplay)
			{
				foreach (Formation formation in this.FormationsIncludingSpecialAndEmpty)
				{
					formation.AI.OnActiveBehaviorChanged -= this.FormationAI_OnActiveBehaviorChanged;
				}
			}
			this.Reset();
		}

		// Token: 0x06003213 RID: 12819 RVA: 0x000CFC44 File Offset: 0x000CDE44
		private void OrderController_OnOrderIssued(OrderType orderType, MBReadOnlyList<Formation> appliedFormations, params object[] delegateParams)
		{
			OnOrderIssuedDelegate onOrderIssued = this.OnOrderIssued;
			if (onOrderIssued == null)
			{
				return;
			}
			onOrderIssued(orderType, appliedFormations, delegateParams);
		}

		// Token: 0x06003214 RID: 12820 RVA: 0x000CFC59 File Offset: 0x000CDE59
		public static bool DoesFirstFormationClassContainSecond(FormationClass f1, FormationClass f2)
		{
			return (f1 & f2) == f2;
		}

		// Token: 0x06003215 RID: 12821 RVA: 0x000CFC61 File Offset: 0x000CDE61
		public static FormationClass GetFormationFormationClass(Formation f)
		{
			if (f.QuerySystem.IsRangedCavalryFormation)
			{
				return FormationClass.HorseArcher;
			}
			if (f.QuerySystem.IsCavalryFormation)
			{
				return FormationClass.Cavalry;
			}
			if (!f.QuerySystem.IsRangedFormation)
			{
				return FormationClass.Infantry;
			}
			return FormationClass.Ranged;
		}

		// Token: 0x06003216 RID: 12822 RVA: 0x000CFC91 File Offset: 0x000CDE91
		public static FormationClass GetPlayerTeamFormationClass(Agent mainAgent)
		{
			if (mainAgent.IsRangedCached && mainAgent.HasMount)
			{
				return FormationClass.HorseArcher;
			}
			if (mainAgent.IsRangedCached)
			{
				return FormationClass.Ranged;
			}
			if (mainAgent.HasMount)
			{
				return FormationClass.Cavalry;
			}
			return FormationClass.Infantry;
		}

		// Token: 0x06003217 RID: 12823 RVA: 0x000CFCBC File Offset: 0x000CDEBC
		public void AssignPlayerAsSergeantOfFormation(MissionPeer peer, FormationClass formationClass)
		{
			Formation formation = this.GetFormation(formationClass);
			formation.PlayerOwner = peer.ControlledAgent;
			formation.BannerCode = peer.Peer.BannerCode;
			if (peer.IsMine)
			{
				this.PlayerOrderController.Owner = peer.ControlledAgent;
			}
			else
			{
				this.GetOrderControllerOf(peer.ControlledAgent).Owner = peer.ControlledAgent;
			}
			formation.SetControlledByAI(false, false);
			foreach (MissionBehavior missionBehavior in this.Mission.MissionBehaviors)
			{
				missionBehavior.OnAssignPlayerAsSergeantOfFormation(peer.ControlledAgent);
			}
			if (peer.IsMine)
			{
				this.PlayerOrderController.SelectAllFormations(false);
			}
			peer.ControlledFormation = formation;
			if (GameNetwork.IsServer)
			{
				peer.ControlledAgent.UpdateCachedAndFormationValues(false, false);
				if (!peer.IsMine)
				{
					GameNetwork.BeginModuleEventAsServer(peer.GetNetworkPeer());
					GameNetwork.WriteMessage(new AssignFormationToPlayer(peer.GetNetworkPeer(), formationClass));
					GameNetwork.EndModuleEventAsServer();
				}
			}
		}

		// Token: 0x06003218 RID: 12824 RVA: 0x000CFDD0 File Offset: 0x000CDFD0
		private void FormationAI_OnActiveBehaviorChanged(Formation formation)
		{
			if (formation.CountOfUnits > 0)
			{
				Action<Formation> onFormationAIActiveBehaviorChanged = this.OnFormationAIActiveBehaviorChanged;
				if (onFormationAIActiveBehaviorChanged == null)
				{
					return;
				}
				onFormationAIActiveBehaviorChanged(formation);
			}
		}

		// Token: 0x06003219 RID: 12825 RVA: 0x000CFDEC File Offset: 0x000CDFEC
		public void AddTacticOption(TacticComponent tacticOption)
		{
			if (this.HasTeamAi)
			{
				this.TeamAI.AddTacticOption(tacticOption);
			}
		}

		// Token: 0x0600321A RID: 12826 RVA: 0x000CFE02 File Offset: 0x000CE002
		public void RemoveTacticOption(Type tacticType)
		{
			if (this.HasTeamAi)
			{
				this.TeamAI.RemoveTacticOption(tacticType);
			}
		}

		// Token: 0x0600321B RID: 12827 RVA: 0x000CFE18 File Offset: 0x000CE018
		public void ClearTacticOptions()
		{
			if (this.HasTeamAi)
			{
				this.TeamAI.ClearTacticOptions();
			}
		}

		// Token: 0x0600321C RID: 12828 RVA: 0x000CFE2D File Offset: 0x000CE02D
		public void ResetTactic()
		{
			if (this.HasTeamAi)
			{
				this.TeamAI.ResetTactic(true);
			}
		}

		// Token: 0x0600321D RID: 12829 RVA: 0x000CFE44 File Offset: 0x000CE044
		public void AddTeamAI(TeamAIComponent teamAI, bool forceNotAIControlled = false)
		{
			this.TeamAI = teamAI;
			foreach (Formation formation in this.FormationsIncludingSpecialAndEmpty)
			{
				formation.SetControlledByAI(!forceNotAIControlled && (this != this.Mission.PlayerTeam || !this.IsPlayerGeneral), false);
			}
			this.TeamAI.InitializeDetachments(this.Mission);
			this.TeamAI.CreateMissionSpecificBehaviors();
			this.TeamAI.ResetTactic(true);
			foreach (Formation formation2 in this.FormationsIncludingSpecialAndEmpty)
			{
				if (formation2.CountOfUnits > 0)
				{
					formation2.AI.Tick();
				}
			}
			this.TeamAI.TickOccasionally();
		}

		// Token: 0x0600321E RID: 12830 RVA: 0x000CFF40 File Offset: 0x000CE140
		public void DelegateCommandToAI()
		{
			foreach (Formation formation in this.FormationsIncludingEmpty)
			{
				formation.SetControlledByAI(true, false);
			}
		}

		// Token: 0x0600321F RID: 12831 RVA: 0x000CFF94 File Offset: 0x000CE194
		public void RearrangeFormationsAccordingToFilters(List<Tuple<Formation, int, Team.TroopFilter, List<Agent>>> MassTransferData)
		{
			foreach (Tuple<Formation, int, Team.TroopFilter, List<Agent>> tuple in MassTransferData)
			{
				tuple.Item1.OnMassUnitTransferStart();
			}
			List<Agent>[] array = new List<Agent>[MassTransferData.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new List<Agent>();
			}
			List<Team.FormationPocket> list = new List<Team.FormationPocket>();
			for (int j = 0; j < MassTransferData.Count; j++)
			{
				Team.TroopFilter filter = MassTransferData[j].Item3;
				Func<Agent, int> func = delegate(Agent agent)
				{
					if (((agent != null) ? agent.Character : null) != null)
					{
						return (((filter & Team.TroopFilter.HighTier) == Team.TroopFilter.HighTier) ? agent.Character.GetBattleTier() : 0) + (((filter & Team.TroopFilter.LowTier) == Team.TroopFilter.LowTier) ? (7 - agent.Character.GetBattleTier()) : 0) + (((filter & Team.TroopFilter.Shield) == Team.TroopFilter.Shield && agent.HasShieldCached) ? 10 : 0) + (((filter & Team.TroopFilter.Spear) == Team.TroopFilter.Spear && agent.HasSpearCached) ? 10 : 0) + (((filter & Team.TroopFilter.Thrown) == Team.TroopFilter.Thrown && agent.HasThrownCached) ? 10 : 0) + (((filter & Team.TroopFilter.Armor) == Team.TroopFilter.Armor && MissionGameModels.Current.AgentStatCalculateModel.HasHeavyArmor(agent)) ? 10 : 0) + ((((filter & Team.TroopFilter.Melee) == Team.TroopFilter.Melee && (filter & Team.TroopFilter.Ranged) == (Team.TroopFilter)0 && !agent.IsRangedCached) || ((filter & Team.TroopFilter.Ranged) == Team.TroopFilter.Ranged && (filter & Team.TroopFilter.Melee) == (Team.TroopFilter)0 && agent.IsRangedCached)) ? 100 : 0) + (((filter & Team.TroopFilter.Mount) == Team.TroopFilter.Mount == agent.HasMount) ? 1000 : 0);
					}
					return (((filter & Team.TroopFilter.HighTier) == Team.TroopFilter.HighTier) ? 7 : 0) + (((filter & Team.TroopFilter.LowTier) == Team.TroopFilter.LowTier) ? 7 : 0) + (((filter & Team.TroopFilter.Shield) == Team.TroopFilter.Shield) ? 10 : 0) + (((filter & Team.TroopFilter.Spear) == Team.TroopFilter.Spear) ? 10 : 0) + (((filter & Team.TroopFilter.Thrown) == Team.TroopFilter.Thrown) ? 10 : 0) + (((filter & Team.TroopFilter.Armor) == Team.TroopFilter.Armor) ? 10 : 0) + (((filter & Team.TroopFilter.Melee) == (Team.TroopFilter)0 || (filter & Team.TroopFilter.Ranged) == (Team.TroopFilter)0) ? 100 : 0) + 1000;
				};
				int num = func(null);
				list.Add(new Team.FormationPocket(func, num, MassTransferData[j].Item2, j));
			}
			list.RemoveAll((Team.FormationPocket pfamv) => pfamv.TroopCount <= 0);
			list = list.OrderBy((Team.FormationPocket pfamv) => pfamv.TroopCount).ToList<Team.FormationPocket>();
			list = list.OrderByDescending((Team.FormationPocket pfamv) => pfamv.ScoreToSeek).ToList<Team.FormationPocket>();
			List<IFormationUnit> list2 = new List<IFormationUnit>();
			list2 = MassTransferData.SelectMany((Tuple<Formation, int, Team.TroopFilter, List<Agent>> mtd) => mtd.Item1.LooseDetachedUnits.Concat(mtd.Item1.Arrangement.GetAllUnits()).Except(mtd.Item4)).ToList<IFormationUnit>();
			int num2 = MassTransferData.Sum((Tuple<Formation, int, Team.TroopFilter, List<Agent>> mtd) => mtd.Item4.Count);
			int k = MassTransferData.Sum((Tuple<Formation, int, Team.TroopFilter, List<Agent>> mtd) => mtd.Item1.CountOfUnits) - num2;
			int num3 = list[0].ScoreToSeek;
			while (k > 0)
			{
				for (int l = 0; l < k; l++)
				{
					Agent agent3 = list2[l] as Agent;
					for (int m = 0; m < list.Count; m++)
					{
						Team.FormationPocket formationPocket = list[m];
						int num4 = formationPocket.PriorityFunction(agent3);
						if (num3 <= formationPocket.ScoreToSeek && num4 >= num3)
						{
							array[formationPocket.Index].Add(agent3);
							formationPocket.AddTroop();
							if (formationPocket.IsFormationPocketFilled())
							{
								list.RemoveAt(m);
							}
							k--;
							list2[l] = list2[k];
							l--;
							break;
						}
						if (num4 > formationPocket.BestFitSoFar)
						{
							formationPocket.BestFitSoFar = num4;
						}
					}
				}
				if (list.Count == 0)
				{
					break;
				}
				for (int n = 0; n < list.Count; n++)
				{
					list[n].UpdateScoreToSeek();
				}
				list.OrderByDescending((Team.FormationPocket pfamv) => pfamv.ScoreToSeek);
				num3 = list[0].ScoreToSeek;
			}
			for (int num5 = 0; num5 < array.Length; num5++)
			{
				foreach (Agent agent2 in array[num5])
				{
					agent2.Formation = MassTransferData[num5].Item1;
				}
			}
			foreach (Tuple<Formation, int, Team.TroopFilter, List<Agent>> tuple2 in MassTransferData)
			{
				this.TriggerOnFormationsChanged(tuple2.Item1);
				tuple2.Item1.OnMassUnitTransferEnd();
				if (tuple2.Item1.CountOfUnits > 0 && !tuple2.Item1.OrderPositionIsValid)
				{
					Vec2 averagePositionOfUnits = tuple2.Item1.GetAveragePositionOfUnits(false, false);
					float terrainHeight = this.Mission.Scene.GetTerrainHeight(averagePositionOfUnits, true);
					this.Mission.Scene.GetHeightAtPoint(averagePositionOfUnits, BodyFlags.None, ref terrainHeight);
					Vec3 vec = new Vec3(averagePositionOfUnits, terrainHeight, -1f);
					WorldPosition worldPosition = new WorldPosition(this.Mission.Scene, UIntPtr.Zero, vec, false);
					tuple2.Item1.SetPositioning(new WorldPosition?(worldPosition), null, null);
				}
			}
		}

		// Token: 0x170008F9 RID: 2297
		// (get) Token: 0x06003220 RID: 12832 RVA: 0x000D0414 File Offset: 0x000CE614
		// (set) Token: 0x06003221 RID: 12833 RVA: 0x000D041C File Offset: 0x000CE61C
		public Formation GeneralsFormation { get; set; }

		// Token: 0x170008FA RID: 2298
		// (get) Token: 0x06003222 RID: 12834 RVA: 0x000D0425 File Offset: 0x000CE625
		// (set) Token: 0x06003223 RID: 12835 RVA: 0x000D042D File Offset: 0x000CE62D
		public Formation BodyGuardFormation { get; set; }

		// Token: 0x170008FB RID: 2299
		// (get) Token: 0x06003224 RID: 12836 RVA: 0x000D0436 File Offset: 0x000CE636
		// (set) Token: 0x06003225 RID: 12837 RVA: 0x000D043E File Offset: 0x000CE63E
		public Agent GeneralAgent { get; set; }

		// Token: 0x06003226 RID: 12838 RVA: 0x000D0448 File Offset: 0x000CE648
		public void OnDeployed()
		{
			foreach (MissionBehavior missionBehavior in this.Mission.MissionBehaviors)
			{
				missionBehavior.OnTeamDeployed(this);
			}
		}

		// Token: 0x06003227 RID: 12839 RVA: 0x000D04A0 File Offset: 0x000CE6A0
		public void Tick(float dt)
		{
			if (!this._cachedEnemyDataForFleeing.IsEmpty<ValueTuple<float, WorldPosition, int, Vec2, Vec2, bool>>())
			{
				this._cachedEnemyDataForFleeing.Clear();
			}
			if (this.Mission.AllowAiTicking)
			{
				if (this.Mission.RetreatSide != BattleSideEnum.None && this.Side == this.Mission.RetreatSide)
				{
					using (List<Formation>.Enumerator enumerator = this.FormationsIncludingSpecialAndEmpty.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Formation formation = enumerator.Current;
							if (formation.CountOfUnits > 0)
							{
								formation.SetMovementOrder(MovementOrder.MovementOrderRetreat);
							}
						}
						goto IL_A8;
					}
				}
				if (this.TeamAI != null && this.HasBots)
				{
					this.TeamAI.Tick(dt);
				}
			}
			IL_A8:
			if (!GameNetwork.IsReplay)
			{
				this.DetachmentManager.TickDetachments();
				foreach (Formation formation2 in this.FormationsIncludingSpecialAndEmpty)
				{
					if (formation2.CountOfUnits > 0)
					{
						formation2.Tick(dt);
					}
				}
			}
		}

		// Token: 0x06003228 RID: 12840 RVA: 0x000D05C4 File Offset: 0x000CE7C4
		public Formation GetFormation(FormationClass formationClass)
		{
			return this.FormationsIncludingSpecialAndEmpty[(int)formationClass];
		}

		// Token: 0x06003229 RID: 12841 RVA: 0x000D05D4 File Offset: 0x000CE7D4
		public void SetIsEnemyOf(Team otherTeam, bool isEnemyOf)
		{
			this.MBTeam.SetIsEnemyOf(otherTeam.MBTeam, isEnemyOf);
			if (GameNetwork.IsServerOrRecorder)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new TeamSetIsEnemyOf(this, otherTeam, isEnemyOf));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
		}

		// Token: 0x0600322A RID: 12842 RVA: 0x000D0618 File Offset: 0x000CE818
		public bool IsEnemyOf(Team otherTeam)
		{
			return this.MBTeam.IsEnemyOf(otherTeam.MBTeam);
		}

		// Token: 0x0600322B RID: 12843 RVA: 0x000D063C File Offset: 0x000CE83C
		public bool IsFriendOf(Team otherTeam)
		{
			return this == otherTeam || !this.MBTeam.IsEnemyOf(otherTeam.MBTeam);
		}

		// Token: 0x170008FC RID: 2300
		// (get) Token: 0x0600322C RID: 12844 RVA: 0x000D0666 File Offset: 0x000CE866
		public IEnumerable<Agent> Heroes
		{
			get
			{
				Agent main = Agent.Main;
				if (main != null && main.Team == this)
				{
					yield return main;
				}
				yield break;
			}
		}

		// Token: 0x0600322D RID: 12845 RVA: 0x000D0676 File Offset: 0x000CE876
		public void AddAgentToTeam(Agent unit)
		{
			this._teamAgents.Add(unit);
			this._activeAgents.Add(unit);
		}

		// Token: 0x0600322E RID: 12846 RVA: 0x000D0690 File Offset: 0x000CE890
		public void RemoveAgentFromTeam(Agent unit)
		{
			this._teamAgents.Remove(unit);
			this._activeAgents.Remove(unit);
		}

		// Token: 0x0600322F RID: 12847 RVA: 0x000D06AC File Offset: 0x000CE8AC
		public void DeactivateAgent(Agent agent)
		{
			this._activeAgents.Remove(agent);
		}

		// Token: 0x170008FD RID: 2301
		// (get) Token: 0x06003230 RID: 12848 RVA: 0x000D06BC File Offset: 0x000CE8BC
		public bool HasBots
		{
			get
			{
				bool flag = false;
				for (int i = 0; i < this.ActiveAgents.Count; i++)
				{
					if (!this.ActiveAgents[i].IsMount && !this.ActiveAgents[i].IsPlayerControlled)
					{
						flag = true;
						break;
					}
				}
				return flag;
			}
		}

		// Token: 0x170008FE RID: 2302
		// (get) Token: 0x06003231 RID: 12849 RVA: 0x000D070C File Offset: 0x000CE90C
		public Agent Leader
		{
			get
			{
				if (Agent.Main != null && Agent.Main.Team == this)
				{
					return Agent.Main;
				}
				Agent agent = null;
				for (int i = 0; i < this.ActiveAgents.Count; i++)
				{
					if (agent == null || this.ActiveAgents[i].IsHero)
					{
						agent = this.ActiveAgents[i];
						if (agent.IsHero)
						{
							break;
						}
					}
				}
				return agent;
			}
		}

		// Token: 0x170008FF RID: 2303
		// (get) Token: 0x06003232 RID: 12850 RVA: 0x000D0777 File Offset: 0x000CE977
		// (set) Token: 0x06003233 RID: 12851 RVA: 0x000D0799 File Offset: 0x000CE999
		public static Team Invalid
		{
			get
			{
				if (Team._invalid == null)
				{
					Team._invalid = new Team(MBTeam.InvalidTeam, BattleSideEnum.None, null, uint.MaxValue, uint.MaxValue, null);
				}
				return Team._invalid;
			}
			internal set
			{
				Team._invalid = value;
			}
		}

		// Token: 0x17000900 RID: 2304
		// (get) Token: 0x06003234 RID: 12852 RVA: 0x000D07A4 File Offset: 0x000CE9A4
		public bool IsValid
		{
			get
			{
				return this.MBTeam.IsValid;
			}
		}

		// Token: 0x06003235 RID: 12853 RVA: 0x000D07C0 File Offset: 0x000CE9C0
		public override string ToString()
		{
			return this.MBTeam.ToString();
		}

		// Token: 0x17000901 RID: 2305
		// (get) Token: 0x06003236 RID: 12854 RVA: 0x000D07E1 File Offset: 0x000CE9E1
		public bool HasTeamAi
		{
			get
			{
				return this.TeamAI != null;
			}
		}

		// Token: 0x06003237 RID: 12855 RVA: 0x000D07EC File Offset: 0x000CE9EC
		public void OnMissionEnded()
		{
			if (this.HasTeamAi)
			{
				this.TeamAI.OnMissionEnded();
			}
		}

		// Token: 0x06003238 RID: 12856 RVA: 0x000D0801 File Offset: 0x000CEA01
		public void TriggerOnFormationsChanged(Formation formation)
		{
			Action<Team, Formation> onFormationsChanged = this.OnFormationsChanged;
			if (onFormationsChanged == null)
			{
				return;
			}
			onFormationsChanged(this, formation);
		}

		// Token: 0x06003239 RID: 12857 RVA: 0x000D0818 File Offset: 0x000CEA18
		public OrderController GetOrderControllerOf(Agent agent)
		{
			OrderController orderController = this._orderControllers.FirstOrDefault((OrderController oc) => oc.Owner == agent);
			if (orderController == null)
			{
				orderController = new OrderController(this.Mission, this, agent);
				this._orderControllers.Add(orderController);
				orderController.OnOrderIssued += this.OrderController_OnOrderIssued;
			}
			return orderController;
		}

		// Token: 0x0600323A RID: 12858 RVA: 0x000D087F File Offset: 0x000CEA7F
		public void ExpireAIQuerySystem()
		{
			this.QuerySystem.Expire();
		}

		// Token: 0x0600323B RID: 12859 RVA: 0x000D088C File Offset: 0x000CEA8C
		public void SetPlayerRole(bool isPlayerGeneral, bool isPlayerSergeant)
		{
			this.IsPlayerGeneral = isPlayerGeneral;
			this.IsPlayerSergeant = isPlayerSergeant;
			foreach (Formation formation in this.FormationsIncludingSpecialAndEmpty)
			{
				formation.SetControlledByAI(this != this.Mission.PlayerTeam || !this.IsPlayerGeneral, false);
			}
		}

		// Token: 0x0600323C RID: 12860 RVA: 0x000D0908 File Offset: 0x000CEB08
		public bool HasAnyEnemyTeamsWithAgents(bool ignoreMountedAgents)
		{
			foreach (Team team in this.Mission.Teams)
			{
				if (team != this && team.IsEnemyOf(this) && team.ActiveAgents.Count > 0)
				{
					if (ignoreMountedAgents)
					{
						using (List<Agent>.Enumerator enumerator2 = team.ActiveAgents.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								if (!enumerator2.Current.HasMount)
								{
									return true;
								}
							}
							continue;
						}
					}
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600323D RID: 12861 RVA: 0x000D09C4 File Offset: 0x000CEBC4
		public bool HasAnyFormationsIncludingSpecialThatIsNotEmpty()
		{
			using (List<Formation>.Enumerator enumerator = this.FormationsIncludingSpecialAndEmpty.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.CountOfUnits > 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600323E RID: 12862 RVA: 0x000D0A20 File Offset: 0x000CEC20
		public int GetFormationCount()
		{
			int num = 0;
			using (List<Formation>.Enumerator enumerator = this.FormationsIncludingEmpty.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.CountOfUnits > 0)
					{
						num++;
					}
				}
			}
			return num;
		}

		// Token: 0x0600323F RID: 12863 RVA: 0x000D0A7C File Offset: 0x000CEC7C
		public int GetAIControlledFormationCount()
		{
			int num = 0;
			foreach (Formation formation in this.FormationsIncludingEmpty)
			{
				if (formation.CountOfUnits > 0 && formation.IsAIControlled)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x06003240 RID: 12864 RVA: 0x000D0AE0 File Offset: 0x000CECE0
		public Vec2 GetAveragePositionOfEnemies()
		{
			Vec2 vec = Vec2.Zero;
			int num = 0;
			foreach (Team team in this.Mission.Teams)
			{
				if (team.MBTeam.IsValid && this.IsEnemyOf(team))
				{
					foreach (Agent agent in team.ActiveAgents)
					{
						vec += agent.Position.AsVec2;
						num++;
					}
				}
			}
			if (num > 0)
			{
				vec *= 1f / (float)num;
				return vec;
			}
			return Vec2.Invalid;
		}

		// Token: 0x06003241 RID: 12865 RVA: 0x000D0BC8 File Offset: 0x000CEDC8
		public Vec2 GetAveragePosition()
		{
			Vec2 vec = Vec2.Zero;
			List<Agent> activeAgents = this.ActiveAgents;
			int num = 0;
			foreach (Agent agent in activeAgents)
			{
				vec += agent.Position.AsVec2;
				num++;
			}
			if (num > 0)
			{
				vec *= 1f / (float)num;
			}
			else
			{
				vec = Vec2.Invalid;
			}
			return vec;
		}

		// Token: 0x06003242 RID: 12866 RVA: 0x000D0C54 File Offset: 0x000CEE54
		public WorldPosition GetMedianPosition(Vec2 averagePosition)
		{
			float num = float.MaxValue;
			Agent agent = null;
			foreach (Agent agent2 in this.ActiveAgents)
			{
				float num2 = agent2.Position.AsVec2.DistanceSquared(averagePosition);
				if (num2 <= num)
				{
					agent = agent2;
					num = num2;
				}
			}
			if (agent == null)
			{
				return WorldPosition.Invalid;
			}
			return agent.GetWorldPosition();
		}

		// Token: 0x06003243 RID: 12867 RVA: 0x000D0CDC File Offset: 0x000CEEDC
		public Vec2 GetWeightedAverageOfEnemies(Vec2 basePoint)
		{
			Vec2 vec = Vec2.Zero;
			float num = 0f;
			foreach (Team team in this.Mission.Teams)
			{
				if (team.MBTeam.IsValid && this.IsEnemyOf(team))
				{
					foreach (Agent agent in team.ActiveAgents)
					{
						Vec2 asVec = agent.Position.AsVec2;
						float lengthSquared = (basePoint - asVec).LengthSquared;
						float num2 = 1f / lengthSquared;
						vec += asVec * num2;
						num += num2;
					}
				}
			}
			if (num > 0f)
			{
				vec *= 1f / num;
				return vec;
			}
			return Vec2.Invalid;
		}

		// Token: 0x06003244 RID: 12868 RVA: 0x000D0DF4 File Offset: 0x000CEFF4
		[Conditional("DEBUG")]
		private void TickStandingPointDebug()
		{
		}

		// Token: 0x040014F1 RID: 5361
		public readonly MBTeam MBTeam;

		// Token: 0x040014FA RID: 5370
		private List<OrderController> _orderControllers;

		// Token: 0x040014FF RID: 5375
		private MBList<Agent> _activeAgents;

		// Token: 0x04001500 RID: 5376
		private MBList<Agent> _teamAgents;

		// Token: 0x04001501 RID: 5377
		private MBList<ValueTuple<float, WorldPosition, int, Vec2, Vec2, bool>> _cachedEnemyDataForFleeing;

		// Token: 0x04001506 RID: 5382
		private static Team _invalid;

		// Token: 0x0200069A RID: 1690
		[Flags]
		public enum TroopFilter
		{
			// Token: 0x040021DB RID: 8667
			HighTier = 256,
			// Token: 0x040021DC RID: 8668
			LowTier = 128,
			// Token: 0x040021DD RID: 8669
			Mount = 64,
			// Token: 0x040021DE RID: 8670
			Ranged = 32,
			// Token: 0x040021DF RID: 8671
			Melee = 16,
			// Token: 0x040021E0 RID: 8672
			Shield = 8,
			// Token: 0x040021E1 RID: 8673
			Spear = 4,
			// Token: 0x040021E2 RID: 8674
			Thrown = 2,
			// Token: 0x040021E3 RID: 8675
			Armor = 1
		}

		// Token: 0x0200069B RID: 1691
		private class FormationPocket
		{
			// Token: 0x170009E5 RID: 2533
			// (get) Token: 0x06003EE8 RID: 16104 RVA: 0x000F6578 File Offset: 0x000F4778
			// (set) Token: 0x06003EE9 RID: 16105 RVA: 0x000F6580 File Offset: 0x000F4780
			public Func<Agent, int> PriorityFunction { get; private set; }

			// Token: 0x170009E6 RID: 2534
			// (get) Token: 0x06003EEA RID: 16106 RVA: 0x000F6589 File Offset: 0x000F4789
			// (set) Token: 0x06003EEB RID: 16107 RVA: 0x000F6591 File Offset: 0x000F4791
			public int MaxValue { get; private set; }

			// Token: 0x170009E7 RID: 2535
			// (get) Token: 0x06003EEC RID: 16108 RVA: 0x000F659A File Offset: 0x000F479A
			// (set) Token: 0x06003EED RID: 16109 RVA: 0x000F65A2 File Offset: 0x000F47A2
			public int TroopCount { get; private set; }

			// Token: 0x170009E8 RID: 2536
			// (get) Token: 0x06003EEE RID: 16110 RVA: 0x000F65AB File Offset: 0x000F47AB
			// (set) Token: 0x06003EEF RID: 16111 RVA: 0x000F65B3 File Offset: 0x000F47B3
			public int Index { get; private set; }

			// Token: 0x170009E9 RID: 2537
			// (get) Token: 0x06003EF0 RID: 16112 RVA: 0x000F65BC File Offset: 0x000F47BC
			// (set) Token: 0x06003EF1 RID: 16113 RVA: 0x000F65C4 File Offset: 0x000F47C4
			public int AddedTroopCount { get; private set; }

			// Token: 0x06003EF2 RID: 16114 RVA: 0x000F65CD File Offset: 0x000F47CD
			public FormationPocket(Func<Agent, int> priorityFunction, int maxValue, int troopCount, int index)
			{
				this.PriorityFunction = priorityFunction;
				this.MaxValue = maxValue;
				this.TroopCount = troopCount;
				this.Index = index;
				this.AddedTroopCount = 0;
				this.ScoreToSeek = maxValue;
				this.BestFitSoFar = 0;
			}

			// Token: 0x06003EF3 RID: 16115 RVA: 0x000F6608 File Offset: 0x000F4808
			public void AddTroop()
			{
				int addedTroopCount = this.AddedTroopCount;
				this.AddedTroopCount = addedTroopCount + 1;
			}

			// Token: 0x06003EF4 RID: 16116 RVA: 0x000F6625 File Offset: 0x000F4825
			public bool IsFormationPocketFilled()
			{
				return this.AddedTroopCount >= this.TroopCount;
			}

			// Token: 0x06003EF5 RID: 16117 RVA: 0x000F6638 File Offset: 0x000F4838
			public void UpdateScoreToSeek()
			{
				this.ScoreToSeek = this.BestFitSoFar;
				this.BestFitSoFar = 0;
			}

			// Token: 0x040021E9 RID: 8681
			public int ScoreToSeek;

			// Token: 0x040021EA RID: 8682
			public int BestFitSoFar;
		}
	}
}
