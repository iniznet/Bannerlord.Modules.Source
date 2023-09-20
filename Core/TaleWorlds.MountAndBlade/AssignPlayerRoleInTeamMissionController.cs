using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.LinQuick;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200025B RID: 603
	public class AssignPlayerRoleInTeamMissionController : MissionLogic
	{
		// Token: 0x1400002B RID: 43
		// (add) Token: 0x0600207B RID: 8315 RVA: 0x00073BB0 File Offset: 0x00071DB0
		// (remove) Token: 0x0600207C RID: 8316 RVA: 0x00073BE8 File Offset: 0x00071DE8
		public event PlayerTurnToChooseFormationToLeadEvent OnPlayerTurnToChooseFormationToLead;

		// Token: 0x1400002C RID: 44
		// (add) Token: 0x0600207D RID: 8317 RVA: 0x00073C20 File Offset: 0x00071E20
		// (remove) Token: 0x0600207E RID: 8318 RVA: 0x00073C58 File Offset: 0x00071E58
		public event AllFormationsAssignedSergeantsEvent OnAllFormationsAssignedSergeants;

		// Token: 0x17000648 RID: 1608
		// (get) Token: 0x0600207F RID: 8319 RVA: 0x00073C8D File Offset: 0x00071E8D
		public bool IsPlayerInArmy { get; }

		// Token: 0x17000649 RID: 1609
		// (get) Token: 0x06002080 RID: 8320 RVA: 0x00073C95 File Offset: 0x00071E95
		public bool IsPlayerGeneral { get; }

		// Token: 0x06002081 RID: 8321 RVA: 0x00073C9D File Offset: 0x00071E9D
		public AssignPlayerRoleInTeamMissionController(bool isPlayerGeneral, bool isPlayerSergeant, bool isPlayerInArmy, List<string> charactersInPlayerSideByPriority = null, FormationClass preassignedFormationClass = FormationClass.NumberOfRegularFormations)
		{
			this.IsPlayerGeneral = isPlayerGeneral;
			this._isPlayerSergeant = isPlayerSergeant;
			this.IsPlayerInArmy = isPlayerInArmy;
			this._charactersInPlayerSideByPriority = charactersInPlayerSideByPriority;
			this._preassignedFormationClass = preassignedFormationClass;
		}

		// Token: 0x06002082 RID: 8322 RVA: 0x00073CDC File Offset: 0x00071EDC
		public override void AfterStart()
		{
			Mission.Current.PlayerTeam.SetPlayerRole(this.IsPlayerGeneral, this._isPlayerSergeant);
		}

		// Token: 0x06002083 RID: 8323 RVA: 0x00073CFC File Offset: 0x00071EFC
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

		// Token: 0x06002084 RID: 8324 RVA: 0x00073D95 File Offset: 0x00071F95
		private void AssignSergeant(Formation formationToLead, Agent sergeant)
		{
			sergeant.Formation = formationToLead;
			if (!sergeant.IsAIControlled || sergeant == Agent.Main)
			{
				formationToLead.PlayerOwner = sergeant;
			}
			formationToLead.Captain = sergeant;
		}

		// Token: 0x06002085 RID: 8325 RVA: 0x00073DBC File Offset: 0x00071FBC
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

		// Token: 0x06002086 RID: 8326 RVA: 0x00073FEC File Offset: 0x000721EC
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

		// Token: 0x06002087 RID: 8327 RVA: 0x00074178 File Offset: 0x00072378
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

		// Token: 0x06002088 RID: 8328 RVA: 0x00074208 File Offset: 0x00072408
		public void OnPlayerChoiceMade(FormationClass chosenFormationClass, FormationAI.BehaviorSide formationBehaviorSide = FormationAI.BehaviorSide.Middle)
		{
			Team playerTeam = base.Mission.PlayerTeam;
			Formation formation = playerTeam.FormationsIncludingEmpty.WhereQ((Formation f) => f.CountOfUnits > 0 && f.PrimaryClass == chosenFormationClass && f.AI.Side == formationBehaviorSide).MaxBy((Formation f) => f.QuerySystem.FormationPower);
			if (playerTeam.IsPlayerSergeant)
			{
				formation.PlayerOwner = Agent.Main;
				formation.SetControlledByAI(false, false);
			}
			if (formation != null && formation != Agent.Main.Formation)
			{
				MBTextManager.SetTextVariable("SIDE_STRING", formation.AI.Side.ToString(), false);
				MBTextManager.SetTextVariable("CLASS_NAME", formation.PrimaryClass.GetName(), false);
				MBInformationManager.AddQuickInformation(GameTexts.FindText("str_formation_soldier_join_text", null), 0, null, "");
			}
			Agent.Main.Formation = formation;
			playerTeam.TriggerOnFormationsChanged(formation);
		}

		// Token: 0x04000BED RID: 3053
		private bool _isPlayerSergeant;

		// Token: 0x04000BEE RID: 3054
		private FormationClass _preassignedFormationClass;

		// Token: 0x04000BEF RID: 3055
		private List<string> _charactersInPlayerSideByPriority = new List<string>();

		// Token: 0x04000BF0 RID: 3056
		private Queue<string> _characterNamesInPlayerSideByPriorityQueue;

		// Token: 0x04000BF1 RID: 3057
		private List<Formation> _remainingFormationsToAssignSergeantsTo;

		// Token: 0x04000BF2 RID: 3058
		private Dictionary<int, Agent> _formationsLockedWithSergeants;

		// Token: 0x04000BF3 RID: 3059
		private Dictionary<int, Agent> _formationsWithLooselyChosenSergeants;

		// Token: 0x04000BF4 RID: 3060
		private int _playerChosenIndex = -1;
	}
}
