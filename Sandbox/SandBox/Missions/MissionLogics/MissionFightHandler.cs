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
	// Token: 0x0200004A RID: 74
	public class MissionFightHandler : MissionLogic
	{
		// Token: 0x17000050 RID: 80
		// (get) Token: 0x06000388 RID: 904 RVA: 0x0001A4B5 File Offset: 0x000186B5
		private static MissionFightHandler _current
		{
			get
			{
				return Mission.Current.GetMissionBehavior<MissionFightHandler>();
			}
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x06000389 RID: 905 RVA: 0x0001A4C1 File Offset: 0x000186C1
		public IEnumerable<Agent> PlayerSideAgents
		{
			get
			{
				return this._playerSideAgents.AsReadOnly();
			}
		}

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x0600038A RID: 906 RVA: 0x0001A4CE File Offset: 0x000186CE
		public IEnumerable<Agent> OpponentSideAgents
		{
			get
			{
				return this._opponentSideAgents.AsReadOnly();
			}
		}

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x0600038B RID: 907 RVA: 0x0001A4DB File Offset: 0x000186DB
		public bool IsPlayerSideWon
		{
			get
			{
				return this._isPlayerSideWon;
			}
		}

		// Token: 0x0600038C RID: 908 RVA: 0x0001A4E3 File Offset: 0x000186E3
		public override void OnBehaviorInitialize()
		{
			base.Mission.IsAgentInteractionAllowed_AdditionalCondition += this.IsAgentInteractionAllowed_AdditionalCondition;
		}

		// Token: 0x0600038D RID: 909 RVA: 0x0001A4FC File Offset: 0x000186FC
		public override void EarlyStart()
		{
			this._playerSideAgents = new List<Agent>();
			this._opponentSideAgents = new List<Agent>();
		}

		// Token: 0x0600038E RID: 910 RVA: 0x0001A514 File Offset: 0x00018714
		public override void AfterStart()
		{
		}

		// Token: 0x0600038F RID: 911 RVA: 0x0001A518 File Offset: 0x00018718
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

		// Token: 0x06000390 RID: 912 RVA: 0x0001A578 File Offset: 0x00018778
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

		// Token: 0x06000391 RID: 913 RVA: 0x0001A634 File Offset: 0x00018834
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

		// Token: 0x06000392 RID: 914 RVA: 0x0001A768 File Offset: 0x00018968
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

		// Token: 0x06000393 RID: 915 RVA: 0x0001A7B8 File Offset: 0x000189B8
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

		// Token: 0x06000394 RID: 916 RVA: 0x0001A7EA File Offset: 0x000189EA
		protected override void OnEndMission()
		{
			base.Mission.IsAgentInteractionAllowed_AdditionalCondition -= this.IsAgentInteractionAllowed_AdditionalCondition;
		}

		// Token: 0x06000395 RID: 917 RVA: 0x0001A804 File Offset: 0x00018A04
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

		// Token: 0x06000396 RID: 918 RVA: 0x0001A8F8 File Offset: 0x00018AF8
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

		// Token: 0x06000397 RID: 919 RVA: 0x0001AA10 File Offset: 0x00018C10
		private bool IsAgentInteractionAllowed_AdditionalCondition()
		{
			return this._state != MissionFightHandler.State.Fighting;
		}

		// Token: 0x06000398 RID: 920 RVA: 0x0001AA20 File Offset: 0x00018C20
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

		// Token: 0x06000399 RID: 921 RVA: 0x0001AA6C File Offset: 0x00018C6C
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

		// Token: 0x0600039A RID: 922 RVA: 0x0001AAA4 File Offset: 0x00018CA4
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

		// Token: 0x0600039B RID: 923 RVA: 0x0001AB80 File Offset: 0x00018D80
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

		// Token: 0x0600039C RID: 924 RVA: 0x0001ACB4 File Offset: 0x00018EB4
		public bool IsThereActiveFight()
		{
			return this._state == MissionFightHandler.State.Fighting;
		}

		// Token: 0x0600039D RID: 925 RVA: 0x0001ACC0 File Offset: 0x00018EC0
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

		// Token: 0x0600039E RID: 926 RVA: 0x0001AD30 File Offset: 0x00018F30
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

		// Token: 0x0600039F RID: 927 RVA: 0x0001AD98 File Offset: 0x00018F98
		public static bool IsAgentAggressive(Agent agent)
		{
			CharacterObject characterObject = agent.Character as CharacterObject;
			return agent.HasWeapon() || (characterObject != null && (characterObject.Occupation == 2 || MissionFightHandler.IsAgentVillian(characterObject) || MissionFightHandler.IsAgentJusticeWarrior(characterObject)));
		}

		// Token: 0x060003A0 RID: 928 RVA: 0x0001ADD9 File Offset: 0x00018FD9
		public static bool IsAgentJusticeWarrior(CharacterObject character)
		{
			return character.Occupation == 7 || character.Occupation == 24 || character.Occupation == 23;
		}

		// Token: 0x060003A1 RID: 929 RVA: 0x0001ADFA File Offset: 0x00018FFA
		public static bool IsAgentVillian(CharacterObject character)
		{
			return character.Occupation == 27 || character.Occupation == 21 || character.Occupation == 15;
		}

		// Token: 0x040001B3 RID: 435
		private static MissionFightHandler.OnFightEndDelegate _onFightEnd;

		// Token: 0x040001B4 RID: 436
		private List<Agent> _playerSideAgents;

		// Token: 0x040001B5 RID: 437
		private List<Agent> _opponentSideAgents;

		// Token: 0x040001B6 RID: 438
		private Dictionary<Agent, Team> _playerSideAgentsOldTeamData;

		// Token: 0x040001B7 RID: 439
		private Dictionary<Agent, Team> _opponentSideAgentsOldTeamData;

		// Token: 0x040001B8 RID: 440
		private MissionFightHandler.State _state;

		// Token: 0x040001B9 RID: 441
		private BasicMissionTimer _finishTimer;

		// Token: 0x040001BA RID: 442
		private BasicMissionTimer _prepareTimer;

		// Token: 0x040001BB RID: 443
		private bool _isPlayerSideWon;

		// Token: 0x040001BC RID: 444
		private MissionMode _oldMissionMode;

		// Token: 0x0200012C RID: 300
		private enum State
		{
			// Token: 0x040005AE RID: 1454
			NoFight,
			// Token: 0x040005AF RID: 1455
			Fighting,
			// Token: 0x040005B0 RID: 1456
			FightEnded
		}

		// Token: 0x0200012D RID: 301
		// (Invoke) Token: 0x06000D1F RID: 3359
		public delegate void OnFightEndDelegate(bool isPlayerSideWon);
	}
}
