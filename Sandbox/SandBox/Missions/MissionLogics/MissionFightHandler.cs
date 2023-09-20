using System;
using System.Collections.Generic;
using SandBox.Missions.AgentBehaviors;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	public class MissionFightHandler : MissionLogic
	{
		private static MissionFightHandler _current
		{
			get
			{
				return Mission.Current.GetMissionBehavior<MissionFightHandler>();
			}
		}

		public IEnumerable<Agent> PlayerSideAgents
		{
			get
			{
				return this._playerSideAgents.AsReadOnly();
			}
		}

		public IEnumerable<Agent> OpponentSideAgents
		{
			get
			{
				return this._opponentSideAgents.AsReadOnly();
			}
		}

		public bool IsPlayerSideWon
		{
			get
			{
				return this._isPlayerSideWon;
			}
		}

		public override void OnBehaviorInitialize()
		{
			base.Mission.IsAgentInteractionAllowed_AdditionalCondition += this.IsAgentInteractionAllowed_AdditionalCondition;
		}

		public override void EarlyStart()
		{
			this._playerSideAgents = new List<Agent>();
			this._opponentSideAgents = new List<Agent>();
		}

		public override void AfterStart()
		{
		}

		public override void OnMissionTick(float dt)
		{
			if (this._finishTimer != null && this._finishTimer.ElapsedTime > 5f)
			{
				this._finishTimer = null;
				this.EndFight();
				this._prepareTimer = new BasicMissionTimer();
			}
			if (this._prepareTimer != null && this._prepareTimer.ElapsedTime > 3f)
			{
				this._prepareTimer = null;
			}
		}

		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			if (this._state != MissionFightHandler.State.Fighting)
			{
				return;
			}
			if (affectedAgent == Agent.Main)
			{
				Mission.Current.NextCheckTimeEndMission += 8f;
			}
			if (affectorAgent != null && this._playerSideAgents.Contains(affectedAgent))
			{
				this._playerSideAgents.Remove(affectedAgent);
				if (this._playerSideAgents.Count == 0)
				{
					this._isPlayerSideWon = false;
					this.ResetScriptedBehaviors();
					this._finishTimer = new BasicMissionTimer();
					return;
				}
			}
			else if (affectorAgent != null && this._opponentSideAgents.Contains(affectedAgent))
			{
				this._opponentSideAgents.Remove(affectedAgent);
				if (this._opponentSideAgents.Count == 0)
				{
					this._isPlayerSideWon = true;
					this.ResetScriptedBehaviors();
					this._finishTimer = new BasicMissionTimer();
				}
			}
		}

		public void StartCustomFight(List<Agent> playerSideAgents, List<Agent> opponentSideAgents, bool dropWeapons, bool isItemUseDisabled, MissionFightHandler.OnFightEndDelegate onFightEndDelegate)
		{
			this._state = MissionFightHandler.State.Fighting;
			this._opponentSideAgents = opponentSideAgents;
			this._playerSideAgents = playerSideAgents;
			this._playerSideAgentsOldTeamData = new Dictionary<Agent, Team>();
			this._opponentSideAgentsOldTeamData = new Dictionary<Agent, Team>();
			MissionFightHandler._onFightEnd = onFightEndDelegate;
			Mission.Current.MainAgent.IsItemUseDisabled = isItemUseDisabled;
			this._oldMissionMode = Mission.Current.Mode;
			Mission.Current.SetMissionMode(2, false);
			foreach (Agent agent in this._opponentSideAgents)
			{
				if (dropWeapons)
				{
					this.DropAllWeapons(agent);
				}
				this._opponentSideAgentsOldTeamData.Add(agent, agent.Team);
				this.ForceAgentForFight(agent);
			}
			foreach (Agent agent2 in this._playerSideAgents)
			{
				if (dropWeapons)
				{
					this.DropAllWeapons(agent2);
				}
				this._playerSideAgentsOldTeamData.Add(agent2, agent2.Team);
				this.ForceAgentForFight(agent2);
			}
			this.SetTeamsForFightAndDuel();
		}

		public override InquiryData OnEndMissionRequest(out bool canPlayerLeave)
		{
			canPlayerLeave = true;
			if (this._state == MissionFightHandler.State.Fighting && (this._opponentSideAgents.Count > 0 || this._playerSideAgents.Count > 0))
			{
				MBInformationManager.AddQuickInformation(new TextObject("{=Fpk3BUBs}Your duel has not ended yet!", null), 0, null, "");
				canPlayerLeave = false;
			}
			return null;
		}

		private void ForceAgentForFight(Agent agent)
		{
			if (agent.GetComponent<CampaignAgentComponent>().AgentNavigator != null)
			{
				AlarmedBehaviorGroup behaviorGroup = agent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<AlarmedBehaviorGroup>();
				behaviorGroup.DisableCalmDown = true;
				behaviorGroup.AddBehavior<FightBehavior>();
				behaviorGroup.SetScriptedBehavior<FightBehavior>();
			}
		}

		protected override void OnEndMission()
		{
			base.Mission.IsAgentInteractionAllowed_AdditionalCondition -= this.IsAgentInteractionAllowed_AdditionalCondition;
		}

		private void SetTeamsForFightAndDuel()
		{
			Mission.Current.PlayerEnemyTeam.SetIsEnemyOf(Mission.Current.PlayerTeam, true);
			foreach (Agent agent in this._playerSideAgents)
			{
				if (agent.IsHuman)
				{
					if (agent.IsAIControlled)
					{
						agent.SetWatchState(2);
					}
					agent.SetTeam(Mission.Current.PlayerTeam, true);
				}
			}
			foreach (Agent agent2 in this._opponentSideAgents)
			{
				if (agent2.IsHuman)
				{
					if (agent2.IsAIControlled)
					{
						agent2.SetWatchState(2);
					}
					agent2.SetTeam(Mission.Current.PlayerEnemyTeam, true);
				}
			}
		}

		private void ResetTeamsForFightAndDuel()
		{
			foreach (Agent agent in this._playerSideAgents)
			{
				if (agent.IsAIControlled)
				{
					agent.ResetEnemyCaches();
					agent.InvalidateTargetAgent();
					agent.InvalidateAIWeaponSelections();
					agent.SetWatchState(0);
				}
				agent.SetTeam(new Team(this._playerSideAgentsOldTeamData[agent].MBTeam, -1, base.Mission, uint.MaxValue, uint.MaxValue, null), true);
			}
			foreach (Agent agent2 in this._opponentSideAgents)
			{
				if (agent2.IsAIControlled)
				{
					agent2.ResetEnemyCaches();
					agent2.InvalidateTargetAgent();
					agent2.InvalidateAIWeaponSelections();
					agent2.SetWatchState(0);
				}
				agent2.SetTeam(new Team(this._opponentSideAgentsOldTeamData[agent2].MBTeam, -1, base.Mission, uint.MaxValue, uint.MaxValue, null), true);
			}
		}

		private bool IsAgentInteractionAllowed_AdditionalCondition()
		{
			return this._state != MissionFightHandler.State.Fighting;
		}

		public static Agent GetAgentToSpectate()
		{
			MissionFightHandler current = MissionFightHandler._current;
			if (current._playerSideAgents.Count > 0)
			{
				return current._playerSideAgents[0];
			}
			if (current._opponentSideAgents.Count > 0)
			{
				return current._opponentSideAgents[0];
			}
			return null;
		}

		private void DropAllWeapons(Agent agent)
		{
			for (EquipmentIndex equipmentIndex = 0; equipmentIndex < 5; equipmentIndex++)
			{
				if (!agent.Equipment[equipmentIndex].IsEmpty)
				{
					agent.DropItem(equipmentIndex, 0);
				}
			}
		}

		private void ResetScriptedBehaviors()
		{
			foreach (Agent agent in this._playerSideAgents)
			{
				if (agent.IsActive() && agent.GetComponent<CampaignAgentComponent>().AgentNavigator != null)
				{
					agent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<AlarmedBehaviorGroup>().DisableScriptedBehavior();
				}
			}
			foreach (Agent agent2 in this._opponentSideAgents)
			{
				if (agent2.IsActive() && agent2.GetComponent<CampaignAgentComponent>().AgentNavigator != null)
				{
					agent2.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<AlarmedBehaviorGroup>().DisableScriptedBehavior();
				}
			}
		}

		public void EndFight()
		{
			this.ResetTeamsForFightAndDuel();
			this._state = MissionFightHandler.State.FightEnded;
			foreach (Agent agent in this._playerSideAgents)
			{
				agent.TryToSheathWeaponInHand(0, 3);
				agent.TryToSheathWeaponInHand(1, 3);
			}
			foreach (Agent agent2 in this._opponentSideAgents)
			{
				agent2.TryToSheathWeaponInHand(0, 3);
				agent2.TryToSheathWeaponInHand(1, 3);
			}
			this._playerSideAgents.Clear();
			this._opponentSideAgents.Clear();
			if (Mission.Current.MainAgent != null)
			{
				Mission.Current.MainAgent.IsItemUseDisabled = false;
			}
			if (this._oldMissionMode == 1 && !Campaign.Current.ConversationManager.IsConversationFlowActive)
			{
				this._oldMissionMode = 0;
			}
			Mission.Current.SetMissionMode(this._oldMissionMode, false);
			if (MissionFightHandler._onFightEnd != null)
			{
				MissionFightHandler._onFightEnd(this._isPlayerSideWon);
				MissionFightHandler._onFightEnd = null;
			}
		}

		public bool IsThereActiveFight()
		{
			return this._state == MissionFightHandler.State.Fighting;
		}

		public void AddAgentToSide(Agent agent, bool isPlayerSide)
		{
			if (!this.IsThereActiveFight() || this._playerSideAgents.Contains(agent) || this._opponentSideAgents.Contains(agent))
			{
				return;
			}
			if (isPlayerSide)
			{
				agent.SetTeam(Mission.Current.PlayerTeam, true);
				this._playerSideAgents.Add(agent);
				return;
			}
			agent.SetTeam(Mission.Current.PlayerEnemyTeam, true);
			this._opponentSideAgents.Add(agent);
		}

		public IEnumerable<Agent> GetDangerSources(Agent ownerAgent)
		{
			if (!(ownerAgent.Character is CharacterObject))
			{
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\Missions\\MissionLogics\\MissionFightHandler.cs", "GetDangerSources", 369);
				return new List<Agent>();
			}
			if (this.IsThereActiveFight() && !MissionFightHandler.IsAgentAggressive(ownerAgent) && Agent.Main != null)
			{
				return new List<Agent> { Agent.Main };
			}
			return new List<Agent>();
		}

		public static bool IsAgentAggressive(Agent agent)
		{
			CharacterObject characterObject = agent.Character as CharacterObject;
			return agent.HasWeapon() || (characterObject != null && (characterObject.Occupation == 2 || MissionFightHandler.IsAgentVillian(characterObject) || MissionFightHandler.IsAgentJusticeWarrior(characterObject)));
		}

		public static bool IsAgentJusticeWarrior(CharacterObject character)
		{
			return character.Occupation == 7 || character.Occupation == 24 || character.Occupation == 23;
		}

		public static bool IsAgentVillian(CharacterObject character)
		{
			return character.Occupation == 27 || character.Occupation == 21 || character.Occupation == 15;
		}

		private static MissionFightHandler.OnFightEndDelegate _onFightEnd;

		private List<Agent> _playerSideAgents;

		private List<Agent> _opponentSideAgents;

		private Dictionary<Agent, Team> _playerSideAgentsOldTeamData;

		private Dictionary<Agent, Team> _opponentSideAgentsOldTeamData;

		private MissionFightHandler.State _state;

		private BasicMissionTimer _finishTimer;

		private BasicMissionTimer _prepareTimer;

		private bool _isPlayerSideWon;

		private MissionMode _oldMissionMode;

		private enum State
		{
			NoFight,
			Fighting,
			FightEnded
		}

		public delegate void OnFightEndDelegate(bool isPlayerSideWon);
	}
}
