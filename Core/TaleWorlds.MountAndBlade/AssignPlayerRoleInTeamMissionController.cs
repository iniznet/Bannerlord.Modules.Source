using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.LinQuick;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	public class AssignPlayerRoleInTeamMissionController : MissionLogic
	{
		public event PlayerTurnToChooseFormationToLeadEvent OnPlayerTurnToChooseFormationToLead;

		public event AllFormationsAssignedSergeantsEvent OnAllFormationsAssignedSergeants;

		public bool IsPlayerInArmy { get; }

		public bool IsPlayerGeneral { get; }

		public AssignPlayerRoleInTeamMissionController(bool isPlayerGeneral, bool isPlayerSergeant, bool isPlayerInArmy, List<string> charactersInPlayerSideByPriority = null, FormationClass preassignedFormationClass = FormationClass.NumberOfRegularFormations)
		{
			this.IsPlayerGeneral = isPlayerGeneral;
			this._isPlayerSergeant = isPlayerSergeant;
			this.IsPlayerInArmy = isPlayerInArmy;
			this._charactersInPlayerSideByPriority = charactersInPlayerSideByPriority;
			this._preassignedFormationClass = preassignedFormationClass;
		}

		public override void AfterStart()
		{
			Mission.Current.PlayerTeam.SetPlayerRole(this.IsPlayerGeneral, this._isPlayerSergeant);
		}

		private Formation ChooseFormationToLead(IEnumerable<Formation> formationsToChooseFrom, Agent agent)
		{
			bool hasMount = agent.HasMount;
			bool flag = agent.HasRangedWeapon(false);
			List<Formation> list = formationsToChooseFrom.ToList<Formation>();
			while (list.Count > 0)
			{
				Formation formation = list.MaxBy((Formation ftcf) => ftcf.QuerySystem.FormationPower);
				list.Remove(formation);
				if ((flag || (!formation.QuerySystem.IsRangedFormation && !formation.QuerySystem.IsRangedCavalryFormation)) && (hasMount || (!formation.QuerySystem.IsCavalryFormation && !formation.QuerySystem.IsRangedCavalryFormation)))
				{
					return formation;
				}
			}
			return null;
		}

		private void AssignSergeant(Formation formationToLead, Agent sergeant)
		{
			sergeant.Formation = formationToLead;
			if (!sergeant.IsAIControlled || sergeant == Agent.Main)
			{
				formationToLead.PlayerOwner = sergeant;
			}
			formationToLead.Captain = sergeant;
		}

		public void OnPlayerChoiceMade(int chosenIndex, bool isFinal)
		{
			if (this._playerChosenIndex != chosenIndex)
			{
				this._playerChosenIndex = chosenIndex;
				this._formationsWithLooselyChosenSergeants.Clear();
				List<Formation> list = base.Mission.PlayerTeam.FormationsIncludingEmpty.WhereQ((Formation f) => f.CountOfUnits > 0 && !this._formationsLockedWithSergeants.ContainsKey(f.Index)).ToList<Formation>();
				if (chosenIndex != -1)
				{
					Formation formation = list.FirstOrDefault((Formation fr) => fr.Index == chosenIndex);
					this._formationsWithLooselyChosenSergeants.Add(chosenIndex, base.Mission.PlayerTeam.PlayerOrderController.Owner);
					list.Remove(formation);
				}
				Queue<string> queue = new Queue<string>(this._characterNamesInPlayerSideByPriorityQueue);
				while (list.Count > 0 && queue.Count > 0)
				{
					string nextAgentNameToProcess = queue.Dequeue();
					Agent agent = base.Mission.PlayerTeam.ActiveAgents.FirstOrDefault((Agent aa) => aa.Character.StringId.Equals(nextAgentNameToProcess));
					if (agent != null)
					{
						Formation formation2 = this.ChooseFormationToLead(list, agent);
						if (formation2 != null)
						{
							this._formationsWithLooselyChosenSergeants.Add(formation2.Index, agent);
							list.Remove(formation2);
						}
					}
				}
				if (this.OnAllFormationsAssignedSergeants != null)
				{
					this.OnAllFormationsAssignedSergeants(this._formationsWithLooselyChosenSergeants);
					return;
				}
			}
			else if (isFinal)
			{
				foreach (KeyValuePair<int, Agent> keyValuePair in this._formationsLockedWithSergeants)
				{
					this.AssignSergeant(keyValuePair.Value.Team.GetFormation((FormationClass)keyValuePair.Key), keyValuePair.Value);
				}
				foreach (KeyValuePair<int, Agent> keyValuePair2 in this._formationsWithLooselyChosenSergeants)
				{
					this.AssignSergeant(keyValuePair2.Value.Team.GetFormation((FormationClass)keyValuePair2.Key), keyValuePair2.Value);
				}
			}
		}

		public void OnPlayerTeamDeployed()
		{
			if (MissionGameModels.Current.BattleInitializationModel.CanPlayerSideDeployWithOrderOfBattle())
			{
				Team playerTeam = Mission.Current.PlayerTeam;
				this._formationsLockedWithSergeants = new Dictionary<int, Agent>();
				this._formationsWithLooselyChosenSergeants = new Dictionary<int, Agent>();
				if (playerTeam.IsPlayerGeneral)
				{
					this._characterNamesInPlayerSideByPriorityQueue = new Queue<string>();
					this._remainingFormationsToAssignSergeantsTo = new List<Formation>();
				}
				else
				{
					this._characterNamesInPlayerSideByPriorityQueue = ((this._charactersInPlayerSideByPriority != null) ? new Queue<string>(this._charactersInPlayerSideByPriority) : new Queue<string>());
					this._remainingFormationsToAssignSergeantsTo = playerTeam.FormationsIncludingSpecialAndEmpty.WhereQ((Formation f) => f.CountOfUnits > 0).ToList<Formation>();
					while (this._remainingFormationsToAssignSergeantsTo.Count > 0 && this._characterNamesInPlayerSideByPriorityQueue.Count > 0)
					{
						string nextAgentNameToProcess = this._characterNamesInPlayerSideByPriorityQueue.Dequeue();
						Agent agent = playerTeam.ActiveAgents.FirstOrDefault((Agent aa) => aa.Character.StringId.Equals(nextAgentNameToProcess));
						if (agent != null)
						{
							if (agent == Agent.Main)
							{
								break;
							}
							Formation formation = this.ChooseFormationToLead(this._remainingFormationsToAssignSergeantsTo, agent);
							if (formation != null)
							{
								this._formationsLockedWithSergeants.Add(formation.Index, agent);
								this._remainingFormationsToAssignSergeantsTo.Remove(formation);
							}
						}
					}
				}
				PlayerTurnToChooseFormationToLeadEvent onPlayerTurnToChooseFormationToLead = this.OnPlayerTurnToChooseFormationToLead;
				if (onPlayerTurnToChooseFormationToLead == null)
				{
					return;
				}
				onPlayerTurnToChooseFormationToLead(this._formationsLockedWithSergeants, this._remainingFormationsToAssignSergeantsTo.Select((Formation ftcsf) => ftcsf.Index).ToList<int>());
			}
		}

		public override void OnTeamDeployed(Team team)
		{
			base.OnTeamDeployed(team);
			if (team == base.Mission.PlayerTeam)
			{
				team.PlayerOrderController.Owner = Agent.Main;
				if (team.IsPlayerGeneral)
				{
					foreach (Formation formation in team.FormationsIncludingEmpty)
					{
						formation.PlayerOwner = Agent.Main;
					}
				}
				team.PlayerOrderController.SelectAllFormations(false);
			}
		}

		public void OnPlayerChoiceMade(FormationClass chosenFormationClass, FormationAI.BehaviorSide formationBehaviorSide = FormationAI.BehaviorSide.Middle)
		{
			Team playerTeam = base.Mission.PlayerTeam;
			Formation formation = playerTeam.FormationsIncludingEmpty.WhereQ((Formation f) => f.CountOfUnits > 0 && f.PhysicalClass == chosenFormationClass && f.AI.Side == formationBehaviorSide).MaxBy((Formation f) => f.QuerySystem.FormationPower);
			if (playerTeam.IsPlayerSergeant)
			{
				formation.PlayerOwner = Agent.Main;
				formation.SetControlledByAI(false, false);
			}
			if (formation != null && formation != Agent.Main.Formation)
			{
				MBTextManager.SetTextVariable("SIDE_STRING", formation.AI.Side.ToString(), false);
				MBTextManager.SetTextVariable("CLASS_NAME", formation.PhysicalClass.GetName(), false);
				MBInformationManager.AddQuickInformation(GameTexts.FindText("str_formation_soldier_join_text", null), 0, null, "");
			}
			Agent.Main.Formation = formation;
			playerTeam.TriggerOnFormationsChanged(formation);
		}

		private bool _isPlayerSergeant;

		private FormationClass _preassignedFormationClass;

		private List<string> _charactersInPlayerSideByPriority = new List<string>();

		private Queue<string> _characterNamesInPlayerSideByPriorityQueue;

		private List<Formation> _remainingFormationsToAssignSergeantsTo;

		private Dictionary<int, Agent> _formationsLockedWithSergeants;

		private Dictionary<int, Agent> _formationsWithLooselyChosenSergeants;

		private int _playerChosenIndex = -1;
	}
}
