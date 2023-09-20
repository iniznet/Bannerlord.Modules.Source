using System;
using System.Collections.Generic;
using Helpers;
using SandBox.Conversation;
using SandBox.Conversation.MissionLogics;
using SandBox.Missions.AgentBehaviors;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	// Token: 0x02000037 RID: 55
	public class CampaignMissionComponent : MissionLogic, ICampaignMission
	{
		// Token: 0x17000040 RID: 64
		// (get) Token: 0x06000282 RID: 642 RVA: 0x00010FD3 File Offset: 0x0000F1D3
		public GameState State
		{
			get
			{
				return this._state;
			}
		}

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x06000283 RID: 643 RVA: 0x00010FDB File Offset: 0x0000F1DB
		// (set) Token: 0x06000284 RID: 644 RVA: 0x00010FE3 File Offset: 0x0000F1E3
		public IMissionTroopSupplier AgentSupplier { get; set; }

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x06000285 RID: 645 RVA: 0x00010FEC File Offset: 0x0000F1EC
		// (set) Token: 0x06000286 RID: 646 RVA: 0x00010FF4 File Offset: 0x0000F1F4
		public Location Location { get; set; }

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x06000287 RID: 647 RVA: 0x00010FFD File Offset: 0x0000F1FD
		// (set) Token: 0x06000288 RID: 648 RVA: 0x00011005 File Offset: 0x0000F205
		public Alley LastVisitedAlley { get; set; }

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x06000289 RID: 649 RVA: 0x0001100E File Offset: 0x0000F20E
		MissionMode ICampaignMission.Mode
		{
			get
			{
				return base.Mission.Mode;
			}
		}

		// Token: 0x0600028A RID: 650 RVA: 0x0001101B File Offset: 0x0000F21B
		void ICampaignMission.SetMissionMode(MissionMode newMode, bool atStart)
		{
			base.Mission.SetMissionMode(newMode, atStart);
		}

		// Token: 0x0600028B RID: 651 RVA: 0x0001102C File Offset: 0x0000F22C
		public override void OnAgentCreated(Agent agent)
		{
			base.OnAgentCreated(agent);
			agent.AddComponent(new CampaignAgentComponent(agent));
			CharacterObject characterObject = (CharacterObject)agent.Character;
			if (((characterObject != null) ? characterObject.HeroObject : null) != null && characterObject.HeroObject.IsPlayerCompanion)
			{
				agent.AgentRole = new TextObject("{=kPTp6TPT}({AGENT_ROLE})", null);
				agent.AgentRole.SetTextVariable("AGENT_ROLE", GameTexts.FindText("str_companion", null));
			}
		}

		// Token: 0x0600028C RID: 652 RVA: 0x000110A0 File Offset: 0x0000F2A0
		public override void OnPreDisplayMissionTick(float dt)
		{
			base.OnPreDisplayMissionTick(dt);
			if (this._soundEvent != null && !this._soundEvent.IsPlaying())
			{
				this.RemovePreviousAgentsSoundEvent();
				this._soundEvent.Stop();
				this._soundEvent = null;
			}
		}

		// Token: 0x0600028D RID: 653 RVA: 0x000110D6 File Offset: 0x0000F2D6
		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			if (Campaign.Current != null)
			{
				CampaignEventDispatcher.Instance.MissionTick(dt);
			}
		}

		// Token: 0x0600028E RID: 654 RVA: 0x000110F4 File Offset: 0x0000F2F4
		protected override void OnObjectDisabled(DestructableComponent missionObject)
		{
			SiegeWeapon firstScriptOfType = missionObject.GameEntity.GetFirstScriptOfType<SiegeWeapon>();
			if (firstScriptOfType != null && Campaign.Current != null && Campaign.Current.GameMode == 1)
			{
				CampaignSiegeStateHandler missionBehavior = Mission.Current.GetMissionBehavior<CampaignSiegeStateHandler>();
				if (missionBehavior != null && missionBehavior.IsSallyOut)
				{
					ISiegeEventSide siegeEventSide = missionBehavior.Settlement.SiegeEvent.GetSiegeEventSide(firstScriptOfType.Side);
					siegeEventSide.SiegeEvent.BreakSiegeEngine(siegeEventSide, firstScriptOfType.GetSiegeEngineType());
				}
			}
			base.OnObjectDisabled(missionObject);
		}

		// Token: 0x0600028F RID: 655 RVA: 0x0001116A File Offset: 0x0000F36A
		public override void EarlyStart()
		{
			this._state = Game.Current.GameStateManager.ActiveState as MissionState;
		}

		// Token: 0x06000290 RID: 656 RVA: 0x00011186 File Offset: 0x0000F386
		public override void OnCreated()
		{
			CampaignMission.Current = this;
			this._isMainAgentAnimationSet = false;
		}

		// Token: 0x06000291 RID: 657 RVA: 0x00011195 File Offset: 0x0000F395
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			CampaignEventDispatcher.Instance.OnMissionStarted(base.Mission);
		}

		// Token: 0x06000292 RID: 658 RVA: 0x000111AD File Offset: 0x0000F3AD
		public override void AfterStart()
		{
			base.AfterStart();
			CampaignEventDispatcher.Instance.OnAfterMissionStarted(base.Mission);
		}

		// Token: 0x06000293 RID: 659 RVA: 0x000111C8 File Offset: 0x0000F3C8
		private static void SimulateRunningAwayAgents()
		{
			foreach (Agent agent in Mission.Current.Agents)
			{
				PartyBase ownerParty = agent.GetComponent<CampaignAgentComponent>().OwnerParty;
				if (ownerParty != null && !agent.IsHero && agent.IsRunningAway && MBRandom.RandomFloat < 0.5f)
				{
					CharacterObject characterObject = (CharacterObject)agent.Character;
					ownerParty.MemberRoster.AddToCounts(characterObject, -1, false, 0, 0, true, -1);
				}
			}
		}

		// Token: 0x06000294 RID: 660 RVA: 0x00011264 File Offset: 0x0000F464
		public override void OnMissionResultReady(MissionResult missionResult)
		{
			if (Campaign.Current.GameMode == 1 && PlayerEncounter.IsActive && PlayerEncounter.Battle != null)
			{
				if (missionResult.PlayerVictory)
				{
					PlayerEncounter.SetPlayerVictorious();
				}
				else if (missionResult.BattleState == 3)
				{
					PlayerEncounter.SetPlayerSiegeContinueWithDefenderPullBack();
				}
				MissionResult missionResult2 = base.Mission.MissionResult;
				PlayerEncounter.CampaignBattleResult = CampaignBattleResult.GetResult((missionResult2 != null) ? missionResult2.BattleState : 0, missionResult.EnemyRetreated);
			}
		}

		// Token: 0x06000295 RID: 661 RVA: 0x000112D0 File Offset: 0x0000F4D0
		protected override void OnEndMission()
		{
			if (Campaign.Current.GameMode == 1)
			{
				if (PlayerEncounter.Battle != null && PlayerEncounter.Battle.IsSiegeAssault && (Mission.Current.MissionTeamAIType == 2 || Mission.Current.MissionTeamAIType == 3))
				{
					IEnumerable<IMissionSiegeWeapon> enumerable;
					IEnumerable<IMissionSiegeWeapon> enumerable2;
					Mission.Current.GetMissionBehavior<MissionSiegeEnginesLogic>().GetMissionSiegeWeapons(ref enumerable, ref enumerable2);
					PlayerEncounter.Battle.GetLeaderParty(1).SiegeEvent.SetSiegeEngineStatesAfterSiegeMission(enumerable2, enumerable);
				}
				if (this._soundEvent != null)
				{
					this.RemovePreviousAgentsSoundEvent();
					this._soundEvent.Stop();
					this._soundEvent = null;
				}
			}
			CampaignEventDispatcher.Instance.OnMissionEnded(base.Mission);
			CampaignMission.Current = null;
		}

		// Token: 0x06000296 RID: 662 RVA: 0x00011378 File Offset: 0x0000F578
		void ICampaignMission.OnCloseEncounterMenu()
		{
			if (base.Mission.Mode == 1)
			{
				Campaign.Current.ConversationManager.EndConversation();
				if (Game.Current.GameStateManager.ActiveState is MissionState)
				{
					Game.Current.GameStateManager.PopState(0);
				}
			}
		}

		// Token: 0x06000297 RID: 663 RVA: 0x000113C8 File Offset: 0x0000F5C8
		bool ICampaignMission.AgentLookingAtAgent(IAgent agent1, IAgent agent2)
		{
			return base.Mission.AgentLookingAtAgent((Agent)agent1, (Agent)agent2);
		}

		// Token: 0x06000298 RID: 664 RVA: 0x000113E4 File Offset: 0x0000F5E4
		void ICampaignMission.OnCharacterLocationChanged(LocationCharacter locationCharacter, Location fromLocation, Location toLocation)
		{
			MissionAgentHandler missionBehavior = base.Mission.GetMissionBehavior<MissionAgentHandler>();
			if (toLocation == null)
			{
				missionBehavior.FadeoutExitingLocationCharacter(locationCharacter);
				return;
			}
			missionBehavior.SpawnEnteringLocationCharacter(locationCharacter, fromLocation);
		}

		// Token: 0x06000299 RID: 665 RVA: 0x00011410 File Offset: 0x0000F610
		void ICampaignMission.OnProcessSentence()
		{
		}

		// Token: 0x0600029A RID: 666 RVA: 0x00011412 File Offset: 0x0000F612
		void ICampaignMission.OnConversationContinue()
		{
		}

		// Token: 0x0600029B RID: 667 RVA: 0x00011414 File Offset: 0x0000F614
		bool ICampaignMission.CheckIfAgentCanFollow(IAgent agent)
		{
			AgentNavigator agentNavigator = ((Agent)agent).GetComponent<CampaignAgentComponent>().AgentNavigator;
			if (agentNavigator != null)
			{
				DailyBehaviorGroup behaviorGroup = agentNavigator.GetBehaviorGroup<DailyBehaviorGroup>();
				return behaviorGroup != null && behaviorGroup.GetBehavior<FollowAgentBehavior>() == null;
			}
			return false;
		}

		// Token: 0x0600029C RID: 668 RVA: 0x0001144C File Offset: 0x0000F64C
		void ICampaignMission.AddAgentFollowing(IAgent agent)
		{
			Agent agent2 = (Agent)agent;
			if (agent2.GetComponent<CampaignAgentComponent>().AgentNavigator != null)
			{
				DailyBehaviorGroup behaviorGroup = agent2.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<DailyBehaviorGroup>();
				behaviorGroup.AddBehavior<FollowAgentBehavior>();
				behaviorGroup.SetScriptedBehavior<FollowAgentBehavior>();
			}
		}

		// Token: 0x0600029D RID: 669 RVA: 0x0001148C File Offset: 0x0000F68C
		bool ICampaignMission.CheckIfAgentCanUnFollow(IAgent agent)
		{
			Agent agent2 = (Agent)agent;
			if (agent2.GetComponent<CampaignAgentComponent>().AgentNavigator != null)
			{
				DailyBehaviorGroup behaviorGroup = agent2.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<DailyBehaviorGroup>();
				return behaviorGroup != null && behaviorGroup.GetBehavior<FollowAgentBehavior>() != null;
			}
			return false;
		}

		// Token: 0x0600029E RID: 670 RVA: 0x000114D0 File Offset: 0x0000F6D0
		void ICampaignMission.RemoveAgentFollowing(IAgent agent)
		{
			Agent agent2 = (Agent)agent;
			if (agent2.GetComponent<CampaignAgentComponent>().AgentNavigator != null)
			{
				agent2.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<DailyBehaviorGroup>().RemoveBehavior<FollowAgentBehavior>();
			}
		}

		// Token: 0x0600029F RID: 671 RVA: 0x00011506 File Offset: 0x0000F706
		void ICampaignMission.EndMission()
		{
			base.Mission.EndMission();
		}

		// Token: 0x060002A0 RID: 672 RVA: 0x00011514 File Offset: 0x0000F714
		private string GetIdleAnimationId(Agent agent, string selectedId, bool startingConversation)
		{
			Agent.ActionCodeType currentActionType = agent.GetCurrentActionType(0);
			if (currentActionType == 41)
			{
				return "sit";
			}
			if (currentActionType == 42)
			{
				return "sit_floor";
			}
			if (currentActionType == 43)
			{
				return "sit_throne";
			}
			if (agent.MountAgent != null)
			{
				ValueTuple<string, ConversationAnimData> animDataForMountAgent = this.GetAnimDataForMountAgent(agent);
				this.SetMountAgentAnimation(agent.MountAgent, animDataForMountAgent.Item2, startingConversation);
				return animDataForMountAgent.Item1;
			}
			if (agent == Agent.Main)
			{
				return "normal";
			}
			if (startingConversation)
			{
				return CharacterHelper.GetStandingBodyIdle((CharacterObject)agent.Character);
			}
			return selectedId;
		}

		// Token: 0x060002A1 RID: 673 RVA: 0x00011598 File Offset: 0x0000F798
		private ValueTuple<string, ConversationAnimData> GetAnimDataForMountAgent(Agent riderAgent)
		{
			string text = "";
			ConversationAnimData conversationAnimData = null;
			foreach (KeyValuePair<string, ConversationAnimData> keyValuePair in Campaign.Current.ConversationManager.ConversationAnimationManager.ConversationAnims)
			{
				if (keyValuePair.Value != null && keyValuePair.Value.FamilyType == riderAgent.Monster.FamilyType && keyValuePair.Value.MountFamilyType == riderAgent.MountAgent.Monster.FamilyType)
				{
					text = keyValuePair.Key;
					conversationAnimData = keyValuePair.Value;
					break;
				}
			}
			return new ValueTuple<string, ConversationAnimData>(text, conversationAnimData);
		}

		// Token: 0x060002A2 RID: 674 RVA: 0x00011654 File Offset: 0x0000F854
		private int GetActionChannelNoForConversation(Agent agent)
		{
			if (agent.IsSitting())
			{
				return 0;
			}
			if (agent.MountAgent != null)
			{
				return 1;
			}
			return 0;
		}

		// Token: 0x060002A3 RID: 675 RVA: 0x0001166C File Offset: 0x0000F86C
		private void SetMountAgentAnimation(IAgent agent, ConversationAnimData mountAnimData, bool startingConversation)
		{
			Agent agent2 = (Agent)agent;
			if (mountAnimData != null)
			{
				if (startingConversation)
				{
					this._conversationAgents.Add(new CampaignMissionComponent.AgentConversationState(agent2));
				}
				ActionIndexCache actionIndexCache = (string.IsNullOrEmpty(mountAnimData.IdleAnimStart) ? ActionIndexCache.Create(mountAnimData.IdleAnimLoop) : ActionIndexCache.Create(mountAnimData.IdleAnimStart));
				this.SetConversationAgentActionAtChannel(agent2, actionIndexCache, this.GetActionChannelNoForConversation(agent2), false, false);
			}
		}

		// Token: 0x060002A4 RID: 676 RVA: 0x000116D0 File Offset: 0x0000F8D0
		void ICampaignMission.OnConversationStart(IAgent iAgent, bool setActionsInstantly)
		{
			((Agent)iAgent).AgentVisuals.SetAgentLodZeroOrMax(true);
			Agent.Main.AgentVisuals.SetAgentLodZeroOrMax(true);
			if (!this._isMainAgentAnimationSet)
			{
				this._isMainAgentAnimationSet = true;
				this.StartConversationAnimations(Agent.Main, setActionsInstantly);
			}
			this.StartConversationAnimations(iAgent, setActionsInstantly);
		}

		// Token: 0x060002A5 RID: 677 RVA: 0x00011724 File Offset: 0x0000F924
		private void StartConversationAnimations(IAgent iAgent, bool setActionsInstantly)
		{
			Agent agent = (Agent)iAgent;
			this._conversationAgents.Add(new CampaignMissionComponent.AgentConversationState(agent));
			string idleAnimationId = this.GetIdleAnimationId(agent, "", true);
			string defaultFaceIdle = CharacterHelper.GetDefaultFaceIdle((CharacterObject)agent.Character);
			int actionChannelNoForConversation = this.GetActionChannelNoForConversation(agent);
			ConversationAnimData conversationAnimData;
			if (Campaign.Current.ConversationManager.ConversationAnimationManager.ConversationAnims.TryGetValue(idleAnimationId, out conversationAnimData))
			{
				ActionIndexCache actionIndexCache = (string.IsNullOrEmpty(conversationAnimData.IdleAnimStart) ? ActionIndexCache.Create(conversationAnimData.IdleAnimLoop) : ActionIndexCache.Create(conversationAnimData.IdleAnimStart));
				this.SetConversationAgentActionAtChannel(agent, actionIndexCache, actionChannelNoForConversation, setActionsInstantly, false);
				this.SetFaceIdle(agent, defaultFaceIdle);
			}
			if (agent.IsUsingGameObject)
			{
				agent.CurrentlyUsedGameObject.OnUserConversationStart();
			}
		}

		// Token: 0x060002A6 RID: 678 RVA: 0x000117E0 File Offset: 0x0000F9E0
		private void EndConversationAnimations(IAgent iAgent)
		{
			Agent agent = (Agent)iAgent;
			if (agent.IsHuman)
			{
				agent.SetAgentFacialAnimation(0, "", false);
				agent.SetAgentFacialAnimation(1, "", false);
				if (agent.HasMount)
				{
					this.EndConversationAnimations(agent.MountAgent);
				}
			}
			int num = -1;
			int count = this._conversationAgents.Count;
			for (int i = 0; i < count; i++)
			{
				CampaignMissionComponent.AgentConversationState agentConversationState = this._conversationAgents[i];
				if (agentConversationState.Agent == agent)
				{
					for (int j = 0; j < 2; j++)
					{
						if (agentConversationState.IsChannelModified(j))
						{
							agent.SetActionChannel(j, ActionIndexCache.act_none, false, (ulong)((long)agent.GetCurrentActionPriority(j)), 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
						}
					}
					if (agent.IsUsingGameObject)
					{
						agent.CurrentlyUsedGameObject.OnUserConversationEnd();
					}
					num = i;
					break;
				}
			}
			if (num != -1)
			{
				this._conversationAgents.RemoveAt(num);
			}
		}

		// Token: 0x060002A7 RID: 679 RVA: 0x000118E0 File Offset: 0x0000FAE0
		void ICampaignMission.OnConversationPlay(string idleActionId, string idleFaceAnimId, string reactionId, string reactionFaceAnimId, string soundPath)
		{
			this._currentAgent = (Agent)Campaign.Current.ConversationManager.SpeakerAgent;
			this.RemovePreviousAgentsSoundEvent();
			this.StopPreviousSound();
			string idleAnimationId = this.GetIdleAnimationId(this._currentAgent, idleActionId, false);
			ConversationAnimData conversationAnimData;
			if (!string.IsNullOrEmpty(idleAnimationId) && Campaign.Current.ConversationManager.ConversationAnimationManager.ConversationAnims.TryGetValue(idleAnimationId, out conversationAnimData))
			{
				if (!string.IsNullOrEmpty(reactionId))
				{
					this.SetConversationAgentActionAtChannel(this._currentAgent, ActionIndexCache.Create(conversationAnimData.Reactions[reactionId]), 0, false, true);
				}
				else
				{
					ActionIndexCache actionIndexCache = (string.IsNullOrEmpty(conversationAnimData.IdleAnimStart) ? ActionIndexCache.Create(conversationAnimData.IdleAnimLoop) : ActionIndexCache.Create(conversationAnimData.IdleAnimStart));
					this.SetConversationAgentActionAtChannel(this._currentAgent, actionIndexCache, this.GetActionChannelNoForConversation(this._currentAgent), false, false);
				}
			}
			if (!string.IsNullOrEmpty(reactionFaceAnimId))
			{
				this._currentAgent.SetAgentFacialAnimation(1, reactionFaceAnimId, false);
			}
			else if (!string.IsNullOrEmpty(idleFaceAnimId))
			{
				this.SetFaceIdle(this._currentAgent, idleFaceAnimId);
			}
			else
			{
				this._currentAgent.SetAgentFacialAnimation(0, "", false);
			}
			if (!string.IsNullOrEmpty(soundPath))
			{
				this.PlayConversationSoundEvent(soundPath);
			}
		}

		// Token: 0x060002A8 RID: 680 RVA: 0x00011A0C File Offset: 0x0000FC0C
		private string GetRhubarbXmlPathFromSoundPath(string soundPath)
		{
			int num = soundPath.LastIndexOf('.');
			return soundPath.Substring(0, num) + ".xml";
		}

		// Token: 0x060002A9 RID: 681 RVA: 0x00011A34 File Offset: 0x0000FC34
		public void PlayConversationSoundEvent(string soundPath)
		{
			Vec3 position = ConversationMission.OneToOneConversationAgent.Position;
			Debug.Print(string.Concat(new object[] { "Conversation sound playing: ", soundPath, ", position: ", position }), 5, 12, 17592186044416UL);
			this._soundEvent = SoundEvent.CreateEventFromExternalFile("event:/Extra/voiceover", soundPath, Mission.Current.Scene);
			this._soundEvent.SetPosition(position);
			this._soundEvent.Play();
			int soundId = this._soundEvent.GetSoundId();
			this._agentSoundEvents.Add(this._currentAgent, soundId);
			string rhubarbXmlPathFromSoundPath = this.GetRhubarbXmlPathFromSoundPath(soundPath);
			this._currentAgent.AgentVisuals.StartRhubarbRecord(rhubarbXmlPathFromSoundPath, soundId);
		}

		// Token: 0x060002AA RID: 682 RVA: 0x00011AF0 File Offset: 0x0000FCF0
		private void StopPreviousSound()
		{
			if (this._soundEvent != null)
			{
				this._soundEvent.Stop();
				this._soundEvent = null;
			}
		}

		// Token: 0x060002AB RID: 683 RVA: 0x00011B0C File Offset: 0x0000FD0C
		private void RemovePreviousAgentsSoundEvent()
		{
			if (this._soundEvent != null && this._agentSoundEvents.ContainsValue(this._soundEvent.GetSoundId()))
			{
				Agent agent = null;
				foreach (KeyValuePair<Agent, int> keyValuePair in this._agentSoundEvents)
				{
					if (keyValuePair.Value == this._soundEvent.GetSoundId())
					{
						agent = keyValuePair.Key;
					}
				}
				this._agentSoundEvents.Remove(agent);
				agent.AgentVisuals.StartRhubarbRecord("", -1);
			}
		}

		// Token: 0x060002AC RID: 684 RVA: 0x00011BB8 File Offset: 0x0000FDB8
		void ICampaignMission.OnConversationEnd(IAgent iAgent)
		{
			Agent agent = (Agent)iAgent;
			agent.ResetLookAgent();
			agent.DisableLookToPointOfInterest();
			Agent.Main.ResetLookAgent();
			Agent.Main.DisableLookToPointOfInterest();
			if (Settlement.CurrentSettlement != null && !base.Mission.HasMissionBehavior<ConversationMissionLogic>())
			{
				agent.AgentVisuals.SetAgentLodZeroOrMax(true);
				Agent.Main.AgentVisuals.SetAgentLodZeroOrMax(true);
			}
			if (this._soundEvent != null)
			{
				this.RemovePreviousAgentsSoundEvent();
				this._soundEvent.Stop();
			}
			if (this._isMainAgentAnimationSet)
			{
				this._isMainAgentAnimationSet = false;
				this.EndConversationAnimations(Agent.Main);
			}
			this.EndConversationAnimations(iAgent);
			this._soundEvent = null;
		}

		// Token: 0x060002AD RID: 685 RVA: 0x00011C5D File Offset: 0x0000FE5D
		private void SetFaceIdle(Agent agent, string idleFaceAnimId)
		{
			agent.SetAgentFacialAnimation(1, idleFaceAnimId, true);
		}

		// Token: 0x060002AE RID: 686 RVA: 0x00011C68 File Offset: 0x0000FE68
		private void SetConversationAgentActionAtChannel(Agent agent, ActionIndexCache action, int channelNo, bool setInstantly, bool forceFaceMorphRestart)
		{
			agent.SetActionChannel(channelNo, action, false, 0UL, 0f, 1f, setInstantly ? 0f : (-0.2f), 0.4f, 0f, false, -0.2f, 0, forceFaceMorphRestart);
			int count = this._conversationAgents.Count;
			for (int i = 0; i < count; i++)
			{
				if (this._conversationAgents[i].Agent == agent)
				{
					this._conversationAgents[i].SetChannelModified(channelNo);
					return;
				}
			}
		}

		// Token: 0x04000146 RID: 326
		private MissionState _state;

		// Token: 0x0400014A RID: 330
		private SoundEvent _soundEvent;

		// Token: 0x0400014B RID: 331
		private Agent _currentAgent;

		// Token: 0x0400014C RID: 332
		private bool _isMainAgentAnimationSet;

		// Token: 0x0400014D RID: 333
		private readonly Dictionary<Agent, int> _agentSoundEvents = new Dictionary<Agent, int>();

		// Token: 0x0400014E RID: 334
		private readonly List<CampaignMissionComponent.AgentConversationState> _conversationAgents = new List<CampaignMissionComponent.AgentConversationState>();

		// Token: 0x0200010F RID: 271
		private class AgentConversationState
		{
			// Token: 0x170000E7 RID: 231
			// (get) Token: 0x06000CBB RID: 3259 RVA: 0x00061F0D File Offset: 0x0006010D
			// (set) Token: 0x06000CBC RID: 3260 RVA: 0x00061F15 File Offset: 0x00060115
			public Agent Agent { get; private set; }

			// Token: 0x06000CBD RID: 3261 RVA: 0x00061F1E File Offset: 0x0006011E
			public AgentConversationState(Agent agent)
			{
				this.Agent = agent;
				this._actionAtChannelModified = default(StackArray.StackArray2Bool);
				this._actionAtChannelModified[0] = false;
				this._actionAtChannelModified[1] = false;
			}

			// Token: 0x06000CBE RID: 3262 RVA: 0x00061F53 File Offset: 0x00060153
			public bool IsChannelModified(int channelNo)
			{
				return this._actionAtChannelModified[channelNo];
			}

			// Token: 0x06000CBF RID: 3263 RVA: 0x00061F61 File Offset: 0x00060161
			public void SetChannelModified(int channelNo)
			{
				this._actionAtChannelModified[channelNo] = true;
			}

			// Token: 0x0400054A RID: 1354
			private StackArray.StackArray2Bool _actionAtChannelModified;
		}
	}
}
