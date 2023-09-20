using System;
using System.Collections.Generic;
using System.Linq;
using SandBox.Missions.AgentBehaviors;
using SandBox.Missions.MissionLogics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Objects;

namespace SandBox.Conversation.MissionLogics
{
	public class MissionConversationLogic : MissionLogic
	{
		public static MissionConversationLogic Current
		{
			get
			{
				return Mission.Current.GetMissionBehavior<MissionConversationLogic>();
			}
		}

		public MissionState State { get; private set; }

		public ConversationManager ConversationManager { get; private set; }

		public bool IsReadyForConversation
		{
			get
			{
				return this.ConversationAgent != null && this.ConversationManager != null && Agent.Main != null && Agent.Main.IsActive();
			}
		}

		public Agent ConversationAgent { get; private set; }

		public MissionConversationLogic(CharacterObject teleportNearChar)
		{
			this._teleportNearCharacter = teleportNearChar;
			this._conversationPoints = new Dictionary<string, MBList<GameEntity>>();
		}

		public MissionConversationLogic()
		{
		}

		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			CampaignEvents.LocationCharactersSimulatedEvent.AddNonSerializedListener(this, new Action(this.OnLocationCharactersSimulated));
		}

		public override void OnAgentBuild(Agent agent, Banner banner)
		{
			if (this._teleportNearCharacter != null && agent.Character == this._teleportNearCharacter)
			{
				this.ConversationAgent = agent;
				this._conversationAgentFound = true;
			}
		}

		public void SetSpawnArea(Alley alley)
		{
			this._customSpawnTag = alley.Tag;
		}

		public void SetSpawnArea(Workshop workshop)
		{
			this._customSpawnTag = workshop.Tag;
		}

		public void SetSpawnArea(string customTag)
		{
			this._customSpawnTag = customTag;
		}

		private void OnLocationCharactersSimulated()
		{
			if (this._conversationAgentFound)
			{
				if (this.FillConversationPointList())
				{
					this.DetermineSpawnPoint();
					this._teleported = this.TryToTeleportBothToCertainPoints();
					return;
				}
				MissionAgentHandler missionBehavior = base.Mission.GetMissionBehavior<MissionAgentHandler>();
				if (missionBehavior == null)
				{
					return;
				}
				missionBehavior.TeleportTargetAgentNearReferenceAgent(this.ConversationAgent, Agent.Main, true, false);
			}
		}

		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			if (!this.IsReadyForConversation)
			{
				return;
			}
			if (!this._teleported)
			{
				base.Mission.GetMissionBehavior<MissionAgentHandler>().TeleportTargetAgentNearReferenceAgent(this.ConversationAgent, Agent.Main, true, false);
				this._teleported = true;
			}
			if (this._teleportNearCharacter != null && !this._conversationStarted)
			{
				this.StartConversation(this.ConversationAgent, true, true);
				if (this.ConversationManager.NeedsToActivateForMapConversation && !GameNetwork.IsReplay)
				{
					this.ConversationManager.BeginConversation();
				}
			}
		}

		private bool TryToTeleportBothToCertainPoints()
		{
			bool missionBehavior = base.Mission.GetMissionBehavior<MissionAgentHandler>() != null;
			bool flag = Agent.Main.MountAgent != null;
			WorldFrame worldFrame;
			worldFrame..ctor(this._selectedConversationPoint.GetGlobalFrame().rotation, new WorldPosition(Agent.Main.Mission.Scene, this._selectedConversationPoint.GetGlobalFrame().origin));
			worldFrame.Origin.SetVec2(worldFrame.Origin.AsVec2 + worldFrame.Rotation.f.AsVec2 * (flag ? 1f : 0.5f));
			WorldFrame worldFrame2;
			worldFrame2..ctor(this._selectedConversationPoint.GetGlobalFrame().rotation, new WorldPosition(Agent.Main.Mission.Scene, this._selectedConversationPoint.GetGlobalFrame().origin));
			worldFrame2.Origin.SetVec2(worldFrame2.Origin.AsVec2 - worldFrame2.Rotation.f.AsVec2 * (flag ? 1f : 0.5f));
			Vec3 vec;
			vec..ctor(worldFrame.Origin.AsVec2 - worldFrame2.Origin.AsVec2, 0f, -1f);
			Vec3 vec2;
			vec2..ctor(worldFrame2.Origin.AsVec2 - worldFrame.Origin.AsVec2, 0f, -1f);
			worldFrame.Rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
			this.ConversationAgent.LookDirection = vec2.NormalizedCopy();
			this.ConversationAgent.TeleportToPosition(worldFrame.Origin.GetGroundVec3());
			worldFrame2.Rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
			if (Agent.Main.MountAgent != null)
			{
				Vec2 vec3 = vec2.AsVec2;
				vec3 = vec3.RightVec();
				Vec3 vec4 = vec3.ToVec3(0f);
				Agent.Main.MountAgent.LookDirection = vec4.NormalizedCopy();
			}
			base.Mission.MainAgent.LookDirection = vec.NormalizedCopy();
			base.Mission.MainAgent.TeleportToPosition(worldFrame2.Origin.GetGroundVec3());
			this.SetConversationAgentAnimations(this.ConversationAgent);
			WorldPosition origin = worldFrame2.Origin;
			origin.SetVec2(origin.AsVec2 - worldFrame2.Rotation.s.AsVec2 * 2f);
			if (missionBehavior)
			{
				foreach (Agent agent in base.Mission.Agents)
				{
					LocationCharacter locationCharacter = LocationComplex.Current.FindCharacter(agent);
					AccompanyingCharacter accompanyingCharacter = PlayerEncounter.LocationEncounter.GetAccompanyingCharacter(locationCharacter);
					if (accompanyingCharacter != null && accompanyingCharacter.IsFollowingPlayerAtMissionStart)
					{
						if (agent.MountAgent != null && Agent.Main.MountAgent != null)
						{
							agent.MountAgent.LookDirection = Agent.Main.MountAgent.LookDirection;
						}
						if (accompanyingCharacter.LocationCharacter.Character == this._teleportNearCharacter)
						{
							agent.LookDirection = vec2.NormalizedCopy();
							Agent agent2 = agent;
							Vec2 vec3 = worldFrame2.Rotation.f.AsVec2;
							agent2.SetMovementDirection(ref vec3);
							agent.TeleportToPosition(worldFrame.Origin.GetGroundVec3());
						}
						else
						{
							agent.LookDirection = vec.NormalizedCopy();
							Agent agent3 = agent;
							Vec2 vec3 = worldFrame.Rotation.f.AsVec2;
							agent3.SetMovementDirection(ref vec3);
							agent.TeleportToPosition(origin.GetGroundVec3());
						}
					}
				}
			}
			this._teleported = true;
			return true;
		}

		private void SetConversationAgentAnimations(Agent conversationAgent)
		{
			CampaignAgentComponent component = conversationAgent.GetComponent<CampaignAgentComponent>();
			AgentNavigator agentNavigator = component.AgentNavigator;
			AgentBehavior agentBehavior = ((agentNavigator != null) ? agentNavigator.GetActiveBehavior() : null);
			if (agentBehavior != null)
			{
				agentBehavior.IsActive = false;
				component.AgentNavigator.ForceThink(0f);
				conversationAgent.SetActionChannel(0, ActionIndexCache.act_none, false, (ulong)((long)conversationAgent.GetCurrentActionPriority(0)), 0f, 1f, 0f, 0.4f, 0f, false, -0.2f, 0, true);
				conversationAgent.TickActionChannels(0.1f);
				conversationAgent.AgentVisuals.GetSkeleton().TickAnimationsAndForceUpdate(0.1f, conversationAgent.AgentVisuals.GetGlobalFrame(), true);
			}
		}

		private void OnConversationEnd()
		{
			foreach (IAgent agent in this.ConversationManager.ConversationAgents)
			{
				Agent agent2 = (Agent)agent;
				agent2.AgentVisuals.SetVisible(true);
				agent2.AgentVisuals.SetClothComponentKeepStateOfAllMeshes(false);
				Agent mountAgent = agent2.MountAgent;
				if (mountAgent != null)
				{
					mountAgent.AgentVisuals.SetVisible(true);
				}
			}
			if (base.Mission.Mode == 1)
			{
				base.Mission.SetMissionMode(this._oldMissionMode, false);
			}
			if (Agent.Main != null)
			{
				Agent.Main.AgentVisuals.SetVisible(true);
				Agent.Main.AgentVisuals.SetClothComponentKeepStateOfAllMeshes(false);
				if (Agent.Main.MountAgent != null)
				{
					Agent.Main.MountAgent.AgentVisuals.SetVisible(true);
				}
			}
			base.Mission.MainAgentServer.Controller = 2;
			this.ConversationManager.ConversationEnd -= this.OnConversationEnd;
		}

		public override void EarlyStart()
		{
			this.State = Game.Current.GameStateManager.ActiveState as MissionState;
		}

		protected override void OnEndMission()
		{
			if (this.ConversationManager != null && this.ConversationManager.IsConversationInProgress)
			{
				this.ConversationManager.EndConversation();
			}
			this.State = null;
			CampaignEvents.LocationCharactersSimulatedEvent.ClearListeners(this);
		}

		public override void OnAgentInteraction(Agent userAgent, Agent agent)
		{
			if (Campaign.Current.GameMode == 1)
			{
				if (Game.Current.GameStateManager.ActiveState is MissionState)
				{
					if (this.IsThereAgentAction(userAgent, agent))
					{
						this.StartConversation(agent, false, false);
						return;
					}
				}
				else
				{
					Debug.FailedAssert("Agent interaction must occur in MissionState.", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\Conversation\\Logics\\MissionConversationLogic.cs", "OnAgentInteraction", 281);
				}
			}
		}

		public void StartConversation(Agent agent, bool setActionsInstantly, bool isInitialization = false)
		{
			this._oldMissionMode = base.Mission.Mode;
			this.ConversationManager = Campaign.Current.ConversationManager;
			this.ConversationManager.SetupAndStartMissionConversation(agent, base.Mission.MainAgent, setActionsInstantly);
			this.ConversationManager.ConversationEnd += this.OnConversationEnd;
			this._conversationStarted = true;
			foreach (IAgent agent2 in this.ConversationManager.ConversationAgents)
			{
				Agent agent3 = (Agent)agent2;
				agent3.ForceAiBehaviorSelection();
				agent3.AgentVisuals.SetClothComponentKeepStateOfAllMeshes(true);
			}
			base.Mission.MainAgentServer.AgentVisuals.SetClothComponentKeepStateOfAllMeshes(true);
			base.Mission.SetMissionMode(1, setActionsInstantly);
		}

		public override bool IsThereAgentAction(Agent userAgent, Agent otherAgent)
		{
			return base.Mission.Mode != 2 && base.Mission.Mode != 3 && base.Mission.Mode != 1 && !this._disableStartConversation && otherAgent.IsActive() && !otherAgent.IsEnemyOf(userAgent);
		}

		public override void OnRenderingStarted()
		{
			this.ConversationManager = Campaign.Current.ConversationManager;
			if (this.ConversationManager == null)
			{
				throw new ArgumentNullException("conversationManager");
			}
		}

		public void DisableStartConversation(bool isDisabled)
		{
			this._disableStartConversation = isDisabled;
		}

		private bool FillConversationPointList()
		{
			List<GameEntity> list = base.Mission.Scene.FindEntitiesWithTag("sp_player_conversation").ToList<GameEntity>();
			bool flag = false;
			if (!Extensions.IsEmpty<GameEntity>(list))
			{
				List<AreaMarker> list2 = MBExtensions.FindAllWithType<AreaMarker>(base.Mission.ActiveMissionObjects).ToList<AreaMarker>();
				foreach (GameEntity gameEntity in list)
				{
					bool flag2 = false;
					foreach (AreaMarker areaMarker in list2)
					{
						if (areaMarker.IsPositionInRange(gameEntity.GlobalPosition))
						{
							if (this._conversationPoints.ContainsKey(areaMarker.Tag))
							{
								this._conversationPoints[areaMarker.Tag].Add(gameEntity);
							}
							else
							{
								Dictionary<string, MBList<GameEntity>> conversationPoints = this._conversationPoints;
								string tag = areaMarker.Tag;
								MBList<GameEntity> mblist = new MBList<GameEntity>();
								mblist.Add(gameEntity);
								conversationPoints.Add(tag, mblist);
							}
							flag2 = true;
							break;
						}
					}
					if (!flag2)
					{
						if (this._conversationPoints.ContainsKey("CenterConversationPoint"))
						{
							this._conversationPoints["CenterConversationPoint"].Add(gameEntity);
						}
						else
						{
							Dictionary<string, MBList<GameEntity>> conversationPoints2 = this._conversationPoints;
							string text = "CenterConversationPoint";
							MBList<GameEntity> mblist2 = new MBList<GameEntity>();
							mblist2.Add(gameEntity);
							conversationPoints2.Add(text, mblist2);
						}
					}
				}
				flag = true;
			}
			else
			{
				Debug.FailedAssert("Scene must have at least one 'sp_player_conversation' game entity. Scene Name: " + Mission.Current.Scene.GetName(), "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\Conversation\\Logics\\MissionConversationLogic.cs", "FillConversationPointList", 375);
			}
			return flag;
		}

		private void DetermineSpawnPoint()
		{
			MBList<GameEntity> mblist;
			if (this._customSpawnTag != null && this._conversationPoints.TryGetValue(this._customSpawnTag, out mblist))
			{
				this._selectedConversationPoint = Extensions.GetRandomElement<GameEntity>(mblist);
				return;
			}
			string agentsTag = this.ConversationAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.SpecialTargetTag;
			if (agentsTag != null)
			{
				MBList<GameEntity> value = this._conversationPoints.FirstOrDefault((KeyValuePair<string, MBList<GameEntity>> x) => agentsTag.Contains(x.Key)).Value;
				this._selectedConversationPoint = ((value != null) ? Extensions.GetRandomElement<GameEntity>(value) : null);
			}
			if (this._selectedConversationPoint == null)
			{
				if (this._conversationPoints.ContainsKey("CenterConversationPoint"))
				{
					this._selectedConversationPoint = Extensions.GetRandomElement<GameEntity>(this._conversationPoints["CenterConversationPoint"]);
					return;
				}
				this._selectedConversationPoint = Extensions.GetRandomElement<GameEntity>(Extensions.GetRandomElementInefficiently<KeyValuePair<string, MBList<GameEntity>>>(this._conversationPoints).Value);
			}
		}

		private const string CenterConversationPointMappingTag = "CenterConversationPoint";

		private MissionMode _oldMissionMode;

		private readonly CharacterObject _teleportNearCharacter;

		private GameEntity _selectedConversationPoint;

		private bool _conversationStarted;

		private bool _teleported;

		private bool _conversationAgentFound;

		private bool _disableStartConversation;

		private readonly Dictionary<string, MBList<GameEntity>> _conversationPoints;

		private string _customSpawnTag;
	}
}
