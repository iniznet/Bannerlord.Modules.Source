using System;
using System.Collections.Generic;
using SandBox.View.Missions;
using SandBox.ViewModelCollection;
using SandBox.ViewModelCollection.Missions.NameMarker;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace SandBox.GauntletUI.Missions
{
	// Token: 0x02000016 RID: 22
	[OverrideView(typeof(MissionNameMarkerUIHandler))]
	public class MissionGauntletNameMarkerView : MissionView
	{
		// Token: 0x060000ED RID: 237 RVA: 0x0000803E File Offset: 0x0000623E
		public MissionGauntletNameMarkerView()
		{
			this._additionalTargetAgents = new Dictionary<Agent, SandBoxUIHelper.IssueQuestFlags>();
			this._additionalGenericTargets = new Dictionary<string, ValueTuple<Vec3, string, string>>();
		}

		// Token: 0x060000EE RID: 238 RVA: 0x0000805C File Offset: 0x0000625C
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this._dataSource = new MissionNameMarkerVM(base.Mission, base.MissionScreen.CombatCamera, this._additionalTargetAgents, this._additionalGenericTargets);
			this._gauntletLayer = new GauntletLayer(1, "GauntletLayer", false);
			this._gauntletLayer.LoadMovie("NameMarker", this._dataSource);
			base.MissionScreen.AddLayer(this._gauntletLayer);
			CampaignEvents.ConversationEnded.AddNonSerializedListener(this, new Action<IEnumerable<CharacterObject>>(this.OnConversationEnd));
		}

		// Token: 0x060000EF RID: 239 RVA: 0x000080E8 File Offset: 0x000062E8
		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
			this._gauntletLayer = null;
			this._dataSource.OnFinalize();
			this._dataSource = null;
			this._additionalTargetAgents.Clear();
			CampaignEvents.ConversationEnded.ClearListeners(this);
			InformationManager.ClearAllMessages();
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x00008140 File Offset: 0x00006340
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (base.Input.IsGameKeyDown(5))
			{
				this._dataSource.IsEnabled = true;
			}
			else
			{
				this._dataSource.IsEnabled = false;
			}
			this._dataSource.Tick(dt);
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x0000817D File Offset: 0x0000637D
		public override void OnAgentBuild(Agent affectedAgent, Banner banner)
		{
			base.OnAgentBuild(affectedAgent, banner);
			MissionNameMarkerVM dataSource = this._dataSource;
			if (dataSource == null)
			{
				return;
			}
			dataSource.OnAgentBuild(affectedAgent);
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x00008198 File Offset: 0x00006398
		public override void OnAgentDeleted(Agent affectedAgent)
		{
			this._dataSource.OnAgentDeleted(affectedAgent);
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x000081A6 File Offset: 0x000063A6
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			this._dataSource.OnAgentRemoved(affectedAgent);
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x000081B4 File Offset: 0x000063B4
		private void OnConversationEnd(IEnumerable<CharacterObject> conversationCharacters)
		{
			this._dataSource.OnConversationEnd();
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x000081C1 File Offset: 0x000063C1
		public override void OnPhotoModeActivated()
		{
			base.OnPhotoModeActivated();
			this._gauntletLayer._gauntletUIContext.ContextAlpha = 0f;
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x000081DE File Offset: 0x000063DE
		public override void OnPhotoModeDeactivated()
		{
			base.OnPhotoModeDeactivated();
			this._gauntletLayer._gauntletUIContext.ContextAlpha = 1f;
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x000081FC File Offset: 0x000063FC
		public void UpdateAgentTargetQuestStatus(Agent agent, SandBoxUIHelper.IssueQuestFlags issueQuestFlags)
		{
			if (agent != null)
			{
				MissionNameMarkerVM dataSource = this._dataSource;
				if (dataSource != null && dataSource.IsTargetsAdded)
				{
					this._dataSource.UpdateAdditionalTargetAgentQuestStatus(agent, issueQuestFlags);
					return;
				}
				SandBoxUIHelper.IssueQuestFlags issueQuestFlags2;
				if (this._additionalTargetAgents.TryGetValue(agent, out issueQuestFlags2))
				{
					this._additionalTargetAgents[agent] = issueQuestFlags;
				}
			}
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x0000824B File Offset: 0x0000644B
		public void AddGenericMarker(string markerIdentifier, Vec3 globalPosition, string name, string iconType)
		{
			MissionNameMarkerVM dataSource = this._dataSource;
			if (dataSource != null && dataSource.IsTargetsAdded)
			{
				this._dataSource.AddGenericMarker(markerIdentifier, globalPosition, name, iconType);
				return;
			}
			this._additionalGenericTargets.Add(markerIdentifier, new ValueTuple<Vec3, string, string>(globalPosition, name, iconType));
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x00008287 File Offset: 0x00006487
		public void RemoveGenericMarker(string markerIdentifier)
		{
			MissionNameMarkerVM dataSource = this._dataSource;
			if (dataSource != null && dataSource.IsTargetsAdded)
			{
				this._dataSource.RemoveGenericMarker(markerIdentifier);
				return;
			}
			this._additionalGenericTargets.Remove(markerIdentifier);
		}

		// Token: 0x060000FA RID: 250 RVA: 0x000082B8 File Offset: 0x000064B8
		public void AddAgentTarget(Agent agent, SandBoxUIHelper.IssueQuestFlags issueQuestFlags)
		{
			if (agent != null)
			{
				MissionNameMarkerVM dataSource = this._dataSource;
				if (dataSource != null && dataSource.IsTargetsAdded)
				{
					this._dataSource.AddAgentTarget(agent, true);
					this._dataSource.UpdateAdditionalTargetAgentQuestStatus(agent, issueQuestFlags);
					return;
				}
				SandBoxUIHelper.IssueQuestFlags issueQuestFlags2;
				if (!this._additionalTargetAgents.TryGetValue(agent, out issueQuestFlags2))
				{
					this._additionalTargetAgents.Add(agent, issueQuestFlags);
				}
			}
		}

		// Token: 0x060000FB RID: 251 RVA: 0x00008314 File Offset: 0x00006514
		public void RemoveAgentTarget(Agent agent)
		{
			if (agent != null)
			{
				MissionNameMarkerVM dataSource = this._dataSource;
				if (dataSource != null && dataSource.IsTargetsAdded)
				{
					this._dataSource.RemoveAgentTarget(agent);
					return;
				}
				SandBoxUIHelper.IssueQuestFlags issueQuestFlags;
				if (this._additionalTargetAgents.TryGetValue(agent, out issueQuestFlags))
				{
					this._additionalTargetAgents.Remove(agent);
				}
			}
		}

		// Token: 0x0400006F RID: 111
		private GauntletLayer _gauntletLayer;

		// Token: 0x04000070 RID: 112
		private MissionNameMarkerVM _dataSource;

		// Token: 0x04000071 RID: 113
		private readonly Dictionary<Agent, SandBoxUIHelper.IssueQuestFlags> _additionalTargetAgents;

		// Token: 0x04000072 RID: 114
		private Dictionary<string, ValueTuple<Vec3, string, string>> _additionalGenericTargets;
	}
}
