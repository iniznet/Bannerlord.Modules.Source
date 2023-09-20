using System;
using SandBox.Conversation;
using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;

namespace SandBox.Issues.IssueQuestTasks
{
	public class BeginConversationInitiatedByAIQuestTask : QuestTaskBase
	{
		public BeginConversationInitiatedByAIQuestTask(Agent agent, Action onSucceededAction, Action onFailedAction, Action onCanceledAction, DialogFlow dialogFlow = null)
			: base(dialogFlow, onSucceededAction, onFailedAction, onCanceledAction)
		{
			this._conversationAgent = agent;
			base.IsLogged = false;
		}

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

		private void OpenConversation(Agent agent)
		{
			ConversationMission.StartConversationWithAgent(agent);
		}

		protected override void OnFinished()
		{
			this._conversationAgent = null;
		}

		public override void SetReferences()
		{
			CampaignEvents.MissionTickEvent.AddNonSerializedListener(this, new Action<float>(this.MissionTick));
		}

		private bool _conversationOpened;

		private Agent _conversationAgent;
	}
}
