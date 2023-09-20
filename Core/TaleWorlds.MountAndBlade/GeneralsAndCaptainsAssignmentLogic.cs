using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace TaleWorlds.MountAndBlade
{
	public class GeneralsAndCaptainsAssignmentLogic : MissionLogic
	{
		public GeneralsAndCaptainsAssignmentLogic(TextObject attackerGeneralName, TextObject defenderGeneralName, TextObject attackerAllyGeneralName = null, TextObject defenderAllyGeneralName = null, bool createBodyguard = true)
		{
			this._attackerGeneralName = attackerGeneralName;
			this._defenderGeneralName = defenderGeneralName;
			this._attackerAllyGeneralName = attackerAllyGeneralName;
			this._defenderAllyGeneralName = defenderAllyGeneralName;
			this._createBodyguard = createBodyguard;
			this._isPlayerTeamGeneralFormationSet = false;
		}

		public override void AfterStart()
		{
			this._bannerLogic = base.Mission.GetMissionBehavior<BannerBearerLogic>();
		}

		public override void OnTeamDeployed(Team team)
		{
			this.SetGeneralAgentOfTeam(team);
			if (team.IsPlayerTeam)
			{
				if (!MissionGameModels.Current.BattleInitializationModel.CanPlayerSideDeployWithOrderOfBattle())
				{
					if (this.CanTeamHaveGeneralsFormation(team))
					{
						this.CreateGeneralFormationForTeam(team);
						this._isPlayerTeamGeneralFormationSet = true;
					}
					this.AssignBestCaptainsForTeam(team);
					return;
				}
			}
			else
			{
				if (this.CanTeamHaveGeneralsFormation(team))
				{
					this.CreateGeneralFormationForTeam(team);
				}
				this.AssignBestCaptainsForTeam(team);
			}
		}

		public override void OnDeploymentFinished()
		{
			Team playerTeam = base.Mission.PlayerTeam;
			if (!this._isPlayerTeamGeneralFormationSet && this.CanTeamHaveGeneralsFormation(playerTeam))
			{
				this.CreateGeneralFormationForTeam(playerTeam);
				this._isPlayerTeamGeneralFormationSet = true;
			}
			Agent mainAgent;
			if (this._isPlayerTeamGeneralFormationSet && (mainAgent = base.Mission.MainAgent) != null && playerTeam.GeneralAgent != mainAgent)
			{
				mainAgent.SetCanLeadFormationsRemotely(true);
				Formation formation = playerTeam.GetFormation(FormationClass.NumberOfRegularFormations);
				mainAgent.Formation = formation;
				mainAgent.Team.TriggerOnFormationsChanged(formation);
				formation.QuerySystem.Expire();
			}
		}

		protected virtual void SortCaptainsByPriority(Team team, ref List<Agent> captains)
		{
			captains = captains.OrderByDescending(delegate(Agent captain)
			{
				if (team.GeneralAgent != captain)
				{
					return captain.Character.GetPower();
				}
				return float.MaxValue;
			}).ToList<Agent>();
		}

		protected virtual Formation PickBestRegularFormationToLead(Agent agent, List<Formation> candidateFormations)
		{
			Formation formation = null;
			float num = 0f;
			foreach (Formation formation2 in candidateFormations)
			{
				int countOfUnits = formation2.CountOfUnits;
				if ((float)countOfUnits > num)
				{
					num = (float)countOfUnits;
					formation = formation2;
				}
			}
			return formation;
		}

		private bool CanTeamHaveGeneralsFormation(Team team)
		{
			Agent generalAgent = team.GeneralAgent;
			return generalAgent != null && (generalAgent == base.Mission.MainAgent || team.QuerySystem.MemberCount >= 50);
		}

		private void AssignBestCaptainsForTeam(Team team)
		{
			List<Agent> list = team.ActiveAgents.Where((Agent agent) => agent.IsHero).ToList<Agent>();
			this.SortCaptainsByPriority(team, ref list);
			int numRegularFormations = 8;
			List<Formation> list2 = team.FormationsIncludingEmpty.WhereQ((Formation f) => f.CountOfUnits > 0 && f.FormationIndex < (FormationClass)numRegularFormations).ToList<Formation>();
			List<Agent> list3 = new List<Agent>();
			foreach (Agent agent4 in list)
			{
				Formation formation = null;
				BattleBannerBearersModel battleBannerBearersModel = MissionGameModels.Current.BattleBannerBearersModel;
				if (agent4 == team.GeneralAgent && team.GeneralsFormation != null)
				{
					int num = ((battleBannerBearersModel != null) ? battleBannerBearersModel.GetMinimumFormationTroopCountToBearBanners() : 0);
					num = Math.Max(num, this.MinimumAgentCountToLeadGeneralFormation);
					if (team.GeneralsFormation.CountOfUnits >= num)
					{
						formation = team.GeneralsFormation;
					}
				}
				if (formation == null)
				{
					formation = this.PickBestRegularFormationToLead(agent4, list2);
					if (formation != null)
					{
						list2.Remove(formation);
					}
				}
				if (formation != null)
				{
					list3.Add(agent4);
					this.OnCaptainAssignedToFormation(agent4, formation);
				}
			}
			foreach (Agent agent2 in list3)
			{
				list.Remove(agent2);
			}
			foreach (Agent agent3 in list)
			{
				if (list2.IsEmpty<Formation>())
				{
					break;
				}
				Formation formation2 = list2[list2.Count - 1];
				this.OnCaptainAssignedToFormation(agent3, formation2);
				list2.Remove(formation2);
			}
		}

		private void SetGeneralAgentOfTeam(Team team)
		{
			Agent agent = null;
			if (team.IsPlayerTeam && team.IsPlayerGeneral)
			{
				agent = base.Mission.MainAgent;
			}
			else
			{
				List<IFormationUnit> list = team.FormationsIncludingEmpty.SelectMany((Formation f) => f.UnitsWithoutLooseDetachedOnes).ToList<IFormationUnit>();
				TextObject generalName = ((team == base.Mission.AttackerTeam) ? this._attackerGeneralName : ((team == base.Mission.DefenderTeam) ? this._defenderGeneralName : ((team == base.Mission.AttackerAllyTeam) ? this._attackerAllyGeneralName : ((team == base.Mission.DefenderAllyTeam) ? this._defenderAllyGeneralName : null))));
				if (generalName != null && list.Count((IFormationUnit ta) => ((Agent)ta).Character != null && ((Agent)ta).Character.GetName().Equals(generalName)) == 1)
				{
					agent = (Agent)list.First((IFormationUnit ta) => ((Agent)ta).Character != null && ((Agent)ta).Character.GetName().Equals(generalName));
				}
				else if (list.Any((IFormationUnit u) => !((Agent)u).IsMainAgent && ((Agent)u).IsHero))
				{
					agent = (Agent)list.Where((IFormationUnit u) => !((Agent)u).IsMainAgent && ((Agent)u).IsHero).MaxBy((IFormationUnit u) => ((Agent)u).CharacterPowerCached);
				}
			}
			if (agent != null)
			{
				agent.SetCanLeadFormationsRemotely(true);
			}
			team.GeneralAgent = agent;
		}

		private void CreateGeneralFormationForTeam(Team team)
		{
			Agent generalAgent = team.GeneralAgent;
			Formation formation = team.GetFormation(FormationClass.NumberOfRegularFormations);
			base.Mission.SpawnFormation(formation);
			WorldPosition worldPosition = formation.CreateNewOrderWorldPosition(WorldPosition.WorldPositionEnforcedCache.GroundVec3);
			formation.SetMovementOrder(MovementOrder.MovementOrderMove(worldPosition));
			formation.SetControlledByAI(true, false);
			team.GeneralsFormation = formation;
			generalAgent.Formation = formation;
			generalAgent.Team.TriggerOnFormationsChanged(formation);
			formation.QuerySystem.Expire();
			TacticComponent.SetDefaultBehaviorWeights(formation);
			formation.AI.SetBehaviorWeight<BehaviorGeneral>(1f);
			formation.PlayerOwner = null;
			if (this._createBodyguard && generalAgent != base.Mission.MainAgent)
			{
				List<IFormationUnit> list = team.FormationsIncludingEmpty.SelectMany((Formation f) => f.UnitsWithoutLooseDetachedOnes).ToList<IFormationUnit>();
				list.Remove(generalAgent);
				List<IFormationUnit> list2 = list.Where(delegate(IFormationUnit u)
				{
					Agent agent;
					if ((agent = u as Agent) == null || (agent.Character != null && agent.Character.IsHero) || agent.Banner != null)
					{
						return false;
					}
					if (generalAgent.MountAgent == null)
					{
						return !agent.HasMount;
					}
					return agent.HasMount;
				}).ToList<IFormationUnit>();
				int num = MathF.Min((int)((float)list2.Count / 10f), 20);
				if (num != 0)
				{
					Formation formation2 = team.GetFormation(FormationClass.Bodyguard);
					formation2.SetMovementOrder(MovementOrder.MovementOrderMove(worldPosition));
					formation2.SetControlledByAI(true, false);
					List<IFormationUnit> list3 = list2.OrderByDescending((IFormationUnit u) => ((Agent)u).CharacterPowerCached).Take(num).ToList<IFormationUnit>();
					IEnumerable<Formation> enumerable = list3.Select((IFormationUnit bu) => ((Agent)bu).Formation).Distinct<Formation>();
					foreach (IFormationUnit formationUnit in list3)
					{
						((Agent)formationUnit).Formation = formation2;
					}
					foreach (Formation formation3 in enumerable)
					{
						team.TriggerOnFormationsChanged(formation3);
						formation3.QuerySystem.Expire();
					}
					TacticComponent.SetDefaultBehaviorWeights(formation2);
					formation2.AI.SetBehaviorWeight<BehaviorProtectGeneral>(1f);
					formation2.PlayerOwner = null;
					formation2.QuerySystem.Expire();
					team.BodyGuardFormation = formation2;
					team.TriggerOnFormationsChanged(formation2);
				}
			}
		}

		private void OnCaptainAssignedToFormation(Agent captain, Formation formation)
		{
			if (captain.Formation != formation && captain != formation.Team.GeneralAgent)
			{
				captain.Formation = formation;
				formation.Team.TriggerOnFormationsChanged(formation);
				formation.QuerySystem.Expire();
			}
			formation.Captain = captain;
			if (this._bannerLogic != null && captain.FormationBanner != null)
			{
				this._bannerLogic.SetFormationBanner(formation, captain.FormationBanner);
			}
		}

		public int MinimumAgentCountToLeadGeneralFormation = 3;

		private BannerBearerLogic _bannerLogic;

		private readonly TextObject _attackerGeneralName;

		private readonly TextObject _defenderGeneralName;

		private readonly TextObject _attackerAllyGeneralName;

		private readonly TextObject _defenderAllyGeneralName;

		private readonly bool _createBodyguard;

		private bool _isPlayerTeamGeneralFormationSet;
	}
}
