using System;
using SandBox.Conversation;
using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;

namespace SandBox.Issues.IssueQuestTasks
{
	// Token: 0x02000083 RID: 131
	public class BeginConversationInitiatedByAIQuestTask : QuestTaskBase
	{
		// Token: 0x06000589 RID: 1417 RVA: 0x000273D5 File Offset: 0x000255D5
		public BeginConversationInitiatedByAIQuestTask(Agent agent, Action onSucceededAction, Action onFailedAction, Action onCanceledAction, DialogFlow dialogFlow = null)
			: base(dialogFlow, onSucceededAction, onFailedAction, onCanceledAction)
		{
			this._conversationAgent = agent;
			base.IsLogged = false;
		}

		// Token: 0x0600058A RID: 1418 RVA: 0x000273F1 File Offset: 0x000255F1
		public void MissionTick(float dt)
		{
			if (Mission.Current.MainAgent == null || this._conversationAgent == null)
			{
				return;
			}
			if (!this._conversationOpened && Mission.Current.Mode != 1)
			{
				this.OpenConversation(this._conversationAgent);
			}
		}

		// Token: 0x0600058B RID: 1419 RVA: 0x00027429 File Offset: 0x00025629
		private void OpenConversation(Agent agent)
		{
			ConversationMission.StartConversationWithAgent(agent);
		}

		// Token: 0x0600058C RID: 1420 RVA: 0x00027431 File Offset: 0x00025631
		protected override void OnFinished()
		{
			this._conversationAgent = null;
		}

		// Token: 0x0600058D RID: 1421 RVA: 0x0002743A File Offset: 0x0002563A
		public override void SetReferences()
		{
			CampaignEvents.MissionTickEvent.AddNonSerializedListener(this, new Action<float>(this.MissionTick));
		}

		// Token: 0x040002B4 RID: 692
		private bool _conversationOpened;

		// Token: 0x040002B5 RID: 693
		private Agent _conversationAgent;
	}
}
