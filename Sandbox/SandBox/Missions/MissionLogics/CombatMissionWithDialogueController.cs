using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.AI.AgentComponents;

namespace SandBox.Missions.MissionLogics
{
	public class CombatMissionWithDialogueController : MissionLogic, IMissionAgentSpawnLogic, IMissionBehavior
	{
		public CombatMissionWithDialogueController(IMissionTroopSupplier[] suppliers, BasicCharacterObject characterToTalkTo, BasicCharacterObject allyTroopsWithFixedTeam)
		{
			this._troopSuppliers = suppliers;
			this._characterToTalkTo = characterToTalkTo;
			this._allyTroopsWithFixedTeam = allyTroopsWithFixedTeam;
		}

		public override void OnCreated()
		{
			base.OnCreated();
			base.Mission.DoesMissionRequireCivilianEquipment = true;
		}

		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._battleAgentLogic = Mission.Current.GetMissionBehavior<BattleAgentLogic>();
		}

		public override void AfterStart()
		{
			base.AfterStart();
			base.Mission.MakeDefaultDeploymentPlans();
		}

		public override void OnMissionTick(float dt)
		{
			if (!this._isMissionInitialized)
			{
				this.SpawnAgents();
				this._isMissionInitialized = true;
				return;
			}
			if (!this._troopsInitialized)
			{
				this._troopsInitialized = true;
				foreach (Agent agent in base.Mission.Agents)
				{
					this._battleAgentLogic.OnAgentBuild(agent, null);
				}
			}
			if (!this._conversationInitialized && Agent.Main != null && Agent.Main.IsActive())
			{
				foreach (Agent agent2 in base.Mission.Agents)
				{
					ScriptedMovementComponent component = agent2.GetComponent<ScriptedMovementComponent>();
					if (component != null && component.ShouldConversationStartWithAgent())
					{
						this.StartConversation(agent2, true);
						this._conversationInitialized = true;
					}
				}
			}
		}

		public override void OnAgentHit(Agent affectedAgent, Agent affectorAgent, in MissionWeapon affectorWeapon, in Blow blow, in AttackCollisionData attackCollisionData)
		{
			if (!this._conversationInitialized && affectedAgent.Team != Mission.Current.PlayerTeam && affectorAgent == Agent.Main)
			{
				this._conversationInitialized = true;
				this.StartFight(false);
			}
		}

		public void StartFight(bool hasPlayerChangedSide)
		{
			base.Mission.SetMissionMode(2, false);
			if (hasPlayerChangedSide)
			{
				Agent.Main.SetTeam((Agent.Main.Team == base.Mission.AttackerTeam) ? base.Mission.DefenderTeam : base.Mission.AttackerTeam, true);
				Mission.Current.PlayerTeam = Agent.Main.Team;
			}
			foreach (Agent agent in base.Mission.Agents)
			{
				if (Agent.Main != agent)
				{
					if (hasPlayerChangedSide && agent.Team != Mission.Current.PlayerTeam && agent.Character != this._allyTroopsWithFixedTeam)
					{
						agent.SetTeam(Mission.Current.PlayerTeam, true);
					}
					AgentFlag agentFlags = agent.GetAgentFlags();
					agent.SetAgentFlags(agentFlags | 65536);
				}
			}
		}

		public void StartConversation(Agent agent, bool setActionsInstantly)
		{
			Campaign.Current.ConversationManager.SetupAndStartMissionConversation(agent, base.Mission.MainAgent, setActionsInstantly);
			foreach (IAgent agent2 in Campaign.Current.ConversationManager.ConversationAgents)
			{
				Agent agent3 = (Agent)agent2;
				agent3.ForceAiBehaviorSelection();
				agent3.AgentVisuals.SetClothComponentKeepStateOfAllMeshes(true);
			}
			base.Mission.MainAgentServer.AgentVisuals.SetClothComponentKeepStateOfAllMeshes(true);
			base.Mission.SetMissionMode(1, setActionsInstantly);
		}

		private void SpawnAgents()
		{
			Agent agent = null;
			IMissionTroopSupplier[] troopSuppliers = this._troopSuppliers;
			for (int i = 0; i < troopSuppliers.Length; i++)
			{
				foreach (IAgentOriginBase agentOriginBase in troopSuppliers[i].SupplyTroops(25).ToList<IAgentOriginBase>())
				{
					Agent agent2 = Mission.Current.SpawnTroop(agentOriginBase, agentOriginBase.IsUnderPlayersCommand || agentOriginBase.Troop.IsPlayerCharacter, false, false, false, 0, 0, false, true, true, null, null, null, null, 10, false);
					this._numSpawnedTroops++;
					if (!agent2.IsMainAgent)
					{
						agent2.AddComponent(new ScriptedMovementComponent(agent2, agent2.Character == this._characterToTalkTo, (float)(agentOriginBase.IsUnderPlayersCommand ? 5 : 2)));
						if (agent2.Character == this._characterToTalkTo)
						{
							agent = agent2;
						}
					}
				}
			}
			foreach (Agent agent3 in base.Mission.Agents)
			{
				ScriptedMovementComponent component = agent3.GetComponent<ScriptedMovementComponent>();
				if (component != null)
				{
					if (agent3.Team == Mission.Current.PlayerTeam)
					{
						component.SetTargetAgent(agent);
					}
					else
					{
						component.SetTargetAgent(Agent.Main);
					}
				}
			}
		}

		public void StartSpawner(BattleSideEnum side)
		{
		}

		public void StopSpawner(BattleSideEnum side)
		{
		}

		public bool IsSideSpawnEnabled(BattleSideEnum side)
		{
			return false;
		}

		public float GetReinforcementInterval()
		{
			return 0f;
		}

		public bool IsSideDepleted(BattleSideEnum side)
		{
			int num = this._numSpawnedTroops - this._troopSuppliers[side].NumRemovedTroops;
			if (Mission.Current.PlayerTeam == base.Mission.DefenderTeam)
			{
				if (side == 1)
				{
					num--;
				}
				else if (Agent.Main != null && Agent.Main.IsActive())
				{
					num++;
				}
			}
			return num == 0;
		}

		private BattleAgentLogic _battleAgentLogic;

		private readonly BasicCharacterObject _characterToTalkTo;

		private readonly BasicCharacterObject _allyTroopsWithFixedTeam;

		private bool _isMissionInitialized;

		private bool _troopsInitialized;

		private bool _conversationInitialized;

		private int _numSpawnedTroops;

		private readonly IMissionTroopSupplier[] _troopSuppliers;
	}
}
