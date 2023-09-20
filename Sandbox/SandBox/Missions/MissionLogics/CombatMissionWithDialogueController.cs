using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.AI.AgentComponents;

namespace SandBox.Missions.MissionLogics
{
	// Token: 0x02000039 RID: 57
	public class CombatMissionWithDialogueController : MissionLogic, IMissionAgentSpawnLogic, IMissionBehavior
	{
		// Token: 0x060002B8 RID: 696 RVA: 0x00011D9A File Offset: 0x0000FF9A
		public CombatMissionWithDialogueController(IMissionTroopSupplier[] suppliers, BasicCharacterObject characterToTalkTo, BasicCharacterObject allyTroopsWithFixedTeam)
		{
			this._troopSuppliers = suppliers;
			this._characterToTalkTo = characterToTalkTo;
			this._allyTroopsWithFixedTeam = allyTroopsWithFixedTeam;
		}

		// Token: 0x060002B9 RID: 697 RVA: 0x00011DB7 File Offset: 0x0000FFB7
		public override void OnCreated()
		{
			base.OnCreated();
			base.Mission.DoesMissionRequireCivilianEquipment = true;
		}

		// Token: 0x060002BA RID: 698 RVA: 0x00011DCB File Offset: 0x0000FFCB
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._battleAgentLogic = Mission.Current.GetMissionBehavior<BattleAgentLogic>();
		}

		// Token: 0x060002BB RID: 699 RVA: 0x00011DE3 File Offset: 0x0000FFE3
		public override void AfterStart()
		{
			base.AfterStart();
			base.Mission.MakeDefaultDeploymentPlans();
		}

		// Token: 0x060002BC RID: 700 RVA: 0x00011DF8 File Offset: 0x0000FFF8
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

		// Token: 0x060002BD RID: 701 RVA: 0x00011EF8 File Offset: 0x000100F8
		public override void OnAgentHit(Agent affectedAgent, Agent affectorAgent, in MissionWeapon affectorWeapon, in Blow blow, in AttackCollisionData attackCollisionData)
		{
			if (!this._conversationInitialized && affectedAgent.Team != Mission.Current.PlayerTeam && affectorAgent == Agent.Main)
			{
				this._conversationInitialized = true;
				this.StartFight(false);
			}
		}

		// Token: 0x060002BE RID: 702 RVA: 0x00011F2C File Offset: 0x0001012C
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

		// Token: 0x060002BF RID: 703 RVA: 0x0001202C File Offset: 0x0001022C
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

		// Token: 0x060002C0 RID: 704 RVA: 0x000120D0 File Offset: 0x000102D0
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

		// Token: 0x060002C1 RID: 705 RVA: 0x0001225C File Offset: 0x0001045C
		public void StartSpawner(BattleSideEnum side)
		{
		}

		// Token: 0x060002C2 RID: 706 RVA: 0x0001225E File Offset: 0x0001045E
		public void StopSpawner(BattleSideEnum side)
		{
		}

		// Token: 0x060002C3 RID: 707 RVA: 0x00012260 File Offset: 0x00010460
		public bool IsSideSpawnEnabled(BattleSideEnum side)
		{
			return false;
		}

		// Token: 0x060002C4 RID: 708 RVA: 0x00012263 File Offset: 0x00010463
		public float GetReinforcementInterval()
		{
			return 0f;
		}

		// Token: 0x060002C5 RID: 709 RVA: 0x0001226C File Offset: 0x0001046C
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

		// Token: 0x04000152 RID: 338
		private BattleAgentLogic _battleAgentLogic;

		// Token: 0x04000153 RID: 339
		private readonly BasicCharacterObject _characterToTalkTo;

		// Token: 0x04000154 RID: 340
		private readonly BasicCharacterObject _allyTroopsWithFixedTeam;

		// Token: 0x04000155 RID: 341
		private bool _isMissionInitialized;

		// Token: 0x04000156 RID: 342
		private bool _troopsInitialized;

		// Token: 0x04000157 RID: 343
		private bool _conversationInitialized;

		// Token: 0x04000158 RID: 344
		private int _numSpawnedTroops;

		// Token: 0x04000159 RID: 345
		private readonly IMissionTroopSupplier[] _troopSuppliers;
	}
}
