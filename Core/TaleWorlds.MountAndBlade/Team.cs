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
	public class Team : IMissionTeam
	{
		public event Action<Team, Formation> OnFormationsChanged;

		public event OnOrderIssuedDelegate OnOrderIssued;

		public event Action<Formation> OnFormationAIActiveBehaviorChanged;

		public BattleSideEnum Side { get; }

		public Mission Mission { get; }

		public MBList<Formation> FormationsIncludingEmpty { get; private set; }

		public MBList<Formation> FormationsIncludingSpecialAndEmpty { get; private set; }

		public TeamAIComponent TeamAI { get; private set; }

		public bool IsPlayerTeam
		{
			get
			{
				return this.Mission.PlayerTeam == this;
			}
		}

		public bool IsPlayerAlly
		{
			get
			{
				return this.Mission.PlayerTeam != null && this.Mission.PlayerTeam.Side == this.Side;
			}
		}

		public bool IsDefender
		{
			get
			{
				return this.Side == BattleSideEnum.Defender;
			}
		}

		public bool IsAttacker
		{
			get
			{
				return this.Side == BattleSideEnum.Attacker;
			}
		}

		public uint Color { get; private set; }

		public uint Color2 { get; private set; }

		public Banner Banner { get; }

		public OrderController MasterOrderController
		{
			get
			{
				return this._orderControllers[0];
			}
		}

		public OrderController PlayerOrderController
		{
			get
			{
				return this._orderControllers[1];
			}
		}

		public TeamQuerySystem QuerySystem { get; private set; }

		public DetachmentManager DetachmentManager { get; private set; }

		public bool IsPlayerGeneral { get; private set; }

		public bool IsPlayerSergeant { get; private set; }

		public MBReadOnlyList<Agent> ActiveAgents
		{
			get
			{
				return this._activeAgents;
			}
		}

		public MBReadOnlyList<Agent> TeamAgents
		{
			get
			{
				return this._teamAgents;
			}
		}

		public MBReadOnlyList<ValueTuple<float, WorldPosition, int, Vec2, Vec2, bool>> CachedEnemyDataForFleeing
		{
			get
			{
				return this._cachedEnemyDataForFleeing;
			}
		}

		public int TeamIndex
		{
			get
			{
				return this.MBTeam.Index;
			}
		}

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

		public float MoraleChangeFactor { get; private set; }

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

		private void OrderController_OnOrderIssued(OrderType orderType, MBReadOnlyList<Formation> appliedFormations, params object[] delegateParams)
		{
			OnOrderIssuedDelegate onOrderIssued = this.OnOrderIssued;
			if (onOrderIssued == null)
			{
				return;
			}
			onOrderIssued(orderType, appliedFormations, delegateParams);
		}

		public static bool DoesFirstFormationClassContainSecond(FormationClass f1, FormationClass f2)
		{
			return (f1 & f2) == f2;
		}

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

		public void AddTacticOption(TacticComponent tacticOption)
		{
			if (this.HasTeamAi)
			{
				this.TeamAI.AddTacticOption(tacticOption);
			}
		}

		public void RemoveTacticOption(Type tacticType)
		{
			if (this.HasTeamAi)
			{
				this.TeamAI.RemoveTacticOption(tacticType);
			}
		}

		public void ClearTacticOptions()
		{
			if (this.HasTeamAi)
			{
				this.TeamAI.ClearTacticOptions();
			}
		}

		public void ResetTactic()
		{
			if (this.HasTeamAi)
			{
				this.TeamAI.ResetTactic(true);
			}
		}

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

		public void DelegateCommandToAI()
		{
			foreach (Formation formation in this.FormationsIncludingEmpty)
			{
				formation.SetControlledByAI(true, false);
			}
		}

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

		public Formation GeneralsFormation { get; set; }

		public Formation BodyGuardFormation { get; set; }

		public Agent GeneralAgent { get; set; }

		public void OnDeployed()
		{
			foreach (MissionBehavior missionBehavior in this.Mission.MissionBehaviors)
			{
				missionBehavior.OnTeamDeployed(this);
			}
		}

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

		public Formation GetFormation(FormationClass formationClass)
		{
			return this.FormationsIncludingSpecialAndEmpty[(int)formationClass];
		}

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

		public bool IsEnemyOf(Team otherTeam)
		{
			return this.MBTeam.IsEnemyOf(otherTeam.MBTeam);
		}

		public bool IsFriendOf(Team otherTeam)
		{
			return this == otherTeam || !this.MBTeam.IsEnemyOf(otherTeam.MBTeam);
		}

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

		public void AddAgentToTeam(Agent unit)
		{
			this._teamAgents.Add(unit);
			this._activeAgents.Add(unit);
		}

		public void RemoveAgentFromTeam(Agent unit)
		{
			this._teamAgents.Remove(unit);
			this._activeAgents.Remove(unit);
		}

		public void DeactivateAgent(Agent agent)
		{
			this._activeAgents.Remove(agent);
		}

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

		public bool IsValid
		{
			get
			{
				return this.MBTeam.IsValid;
			}
		}

		public override string ToString()
		{
			return this.MBTeam.ToString();
		}

		public bool HasTeamAi
		{
			get
			{
				return this.TeamAI != null;
			}
		}

		public void OnMissionEnded()
		{
			if (this.HasTeamAi)
			{
				this.TeamAI.OnMissionEnded();
			}
		}

		public void TriggerOnFormationsChanged(Formation formation)
		{
			Action<Team, Formation> onFormationsChanged = this.OnFormationsChanged;
			if (onFormationsChanged == null)
			{
				return;
			}
			onFormationsChanged(this, formation);
		}

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

		public void ExpireAIQuerySystem()
		{
			this.QuerySystem.Expire();
		}

		public void SetPlayerRole(bool isPlayerGeneral, bool isPlayerSergeant)
		{
			this.IsPlayerGeneral = isPlayerGeneral;
			this.IsPlayerSergeant = isPlayerSergeant;
			foreach (Formation formation in this.FormationsIncludingSpecialAndEmpty)
			{
				formation.SetControlledByAI(this != this.Mission.PlayerTeam || !this.IsPlayerGeneral, false);
			}
		}

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

		[Conditional("DEBUG")]
		private void TickStandingPointDebug()
		{
		}

		public readonly MBTeam MBTeam;

		private List<OrderController> _orderControllers;

		private MBList<Agent> _activeAgents;

		private MBList<Agent> _teamAgents;

		private MBList<ValueTuple<float, WorldPosition, int, Vec2, Vec2, bool>> _cachedEnemyDataForFleeing;

		private static Team _invalid;

		[Flags]
		public enum TroopFilter
		{
			HighTier = 256,
			LowTier = 128,
			Mount = 64,
			Ranged = 32,
			Melee = 16,
			Shield = 8,
			Spear = 4,
			Thrown = 2,
			Armor = 1
		}

		private class FormationPocket
		{
			public Func<Agent, int> PriorityFunction { get; private set; }

			public int MaxValue { get; private set; }

			public int TroopCount { get; private set; }

			public int Index { get; private set; }

			public int AddedTroopCount { get; private set; }

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

			public void AddTroop()
			{
				int addedTroopCount = this.AddedTroopCount;
				this.AddedTroopCount = addedTroopCount + 1;
			}

			public bool IsFormationPocketFilled()
			{
				return this.AddedTroopCount >= this.TroopCount;
			}

			public void UpdateScoreToSeek()
			{
				this.ScoreToSeek = this.BestFitSoFar;
				this.BestFitSoFar = 0;
			}

			public int ScoreToSeek;

			public int BestFitSoFar;
		}
	}
}
