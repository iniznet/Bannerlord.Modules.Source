using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace SandBox.View.Missions
{
	// Token: 0x02000015 RID: 21
	public class MissionConversationCameraView : MissionView
	{
		// Token: 0x06000078 RID: 120 RVA: 0x000052F6 File Offset: 0x000034F6
		public override void AfterStart()
		{
			this._missionMainAgentController = base.Mission.GetMissionBehavior<MissionMainAgentController>();
		}

		// Token: 0x06000079 RID: 121 RVA: 0x0000530C File Offset: 0x0000350C
		public override bool UpdateOverridenCamera(float dt)
		{
			MissionMode mode = base.Mission.Mode;
			if ((mode == 1 || mode == 5) && !base.MissionScreen.IsCheatGhostMode)
			{
				this.UpdateAgentLooksForConversation();
			}
			else if (this._missionMainAgentController.CustomLookDir.IsNonZero)
			{
				this._missionMainAgentController.CustomLookDir = Vec3.Zero;
			}
			return false;
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00005368 File Offset: 0x00003568
		private void UpdateAgentLooksForConversation()
		{
			Agent agent = null;
			ConversationManager conversationManager = Campaign.Current.ConversationManager;
			if (conversationManager.ConversationAgents != null && conversationManager.ConversationAgents.Count > 0)
			{
				this._speakerAgent = (Agent)conversationManager.SpeakerAgent;
				this._listenerAgent = (Agent)conversationManager.ListenerAgent;
				agent = Agent.Main.GetLookAgent();
				if (this._speakerAgent == null)
				{
					return;
				}
				foreach (IAgent agent2 in conversationManager.ConversationAgents)
				{
					if (agent2 != this._speakerAgent)
					{
						this.MakeAgentLookToSpeaker((Agent)agent2);
					}
				}
				this.MakeSpeakerLookToListener();
			}
			this.SetFocusedObjectForCameraFocus();
			if (Agent.Main.GetLookAgent() != agent && this._speakerAgent != null)
			{
				this.SpeakerAgentIsChanged();
			}
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00005448 File Offset: 0x00003648
		private void SpeakerAgentIsChanged()
		{
			Mission.Current.ConversationCharacterChanged();
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00005454 File Offset: 0x00003654
		private void SetFocusedObjectForCameraFocus()
		{
			if (this._speakerAgent == Agent.Main)
			{
				this._missionMainAgentController.InteractionComponent.SetCurrentFocusedObject(this._listenerAgent, null, true);
				this._missionMainAgentController.CustomLookDir = (this._listenerAgent.Position - Agent.Main.Position).NormalizedCopy();
				Agent.Main.SetLookAgent(this._listenerAgent);
				return;
			}
			this._missionMainAgentController.InteractionComponent.SetCurrentFocusedObject(this._speakerAgent, null, true);
			this._missionMainAgentController.CustomLookDir = (this._speakerAgent.Position - Agent.Main.Position).NormalizedCopy();
			Agent.Main.SetLookAgent(this._speakerAgent);
		}

		// Token: 0x0600007D RID: 125 RVA: 0x0000551C File Offset: 0x0000371C
		private void MakeAgentLookToSpeaker(Agent agent)
		{
			Vec3 position = agent.Position;
			Vec3 position2 = this._speakerAgent.Position;
			position.z = agent.AgentVisuals.GetGlobalStableEyePoint(true).z;
			position2.z = this._speakerAgent.AgentVisuals.GetGlobalStableEyePoint(true).z;
			agent.SetLookToPointOfInterest(this._speakerAgent.AgentVisuals.GetGlobalStableEyePoint(true));
			agent.AgentVisuals.GetSkeleton().ForceUpdateBoneFrames();
			agent.LookDirection = (position2 - position).NormalizedCopy();
			agent.SetLookAgent(this._speakerAgent);
		}

		// Token: 0x0600007E RID: 126 RVA: 0x000055BC File Offset: 0x000037BC
		private void MakeSpeakerLookToListener()
		{
			Vec3 position = this._speakerAgent.Position;
			Vec3 position2 = this._listenerAgent.Position;
			position.z = this._speakerAgent.AgentVisuals.GetGlobalStableEyePoint(true).z;
			position2.z = this._listenerAgent.AgentVisuals.GetGlobalStableEyePoint(true).z;
			this._speakerAgent.SetLookToPointOfInterest(this._listenerAgent.AgentVisuals.GetGlobalStableEyePoint(true));
			this._speakerAgent.AgentVisuals.GetSkeleton().ForceUpdateBoneFrames();
			this._speakerAgent.LookDirection = (position2 - position).NormalizedCopy();
			this._speakerAgent.SetLookAgent(this._listenerAgent);
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00005678 File Offset: 0x00003878
		private void SetConversationLook(Agent agent1, Agent agent2)
		{
			Vec3 position = agent2.Position;
			Vec3 position2 = agent1.Position;
			agent2.AgentVisuals.GetSkeleton().ForceUpdateBoneFrames();
			agent1.AgentVisuals.GetSkeleton().ForceUpdateBoneFrames();
			position.z = agent2.AgentVisuals.GetGlobalStableEyePoint(true).z;
			position2.z = agent1.AgentVisuals.GetGlobalStableEyePoint(true).z;
			agent1.SetLookToPointOfInterest(agent2.AgentVisuals.GetGlobalStableEyePoint(true));
			agent2.SetLookToPointOfInterest(agent1.AgentVisuals.GetGlobalStableEyePoint(true));
			agent1.LookDirection = (position2 - position).NormalizedCopy();
			agent2.LookDirection = (position - position2).NormalizedCopy();
			agent2.SetLookAgent(agent1);
			agent1.SetLookAgent(agent2);
		}

		// Token: 0x0400003C RID: 60
		private MissionMainAgentController _missionMainAgentController;

		// Token: 0x0400003D RID: 61
		private Agent _speakerAgent;

		// Token: 0x0400003E RID: 62
		private Agent _listenerAgent;
	}
}
