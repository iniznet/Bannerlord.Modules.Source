using System;
using SandBox.Missions.AgentBehaviors;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;

namespace SandBox.Issues.IssueQuestTasks
{
	public class FollowAgentQuestTask : QuestTaskBase
	{
		public FollowAgentQuestTask(Agent followedAgent, GameEntity targetEntity, Action onSucceededAction, Action onCanceledAction, DialogFlow dialogFlow = null)
			: base(dialogFlow, onSucceededAction, null, onCanceledAction)
		{
			this._followedAgent = followedAgent;
			this._followedAgentChar = (CharacterObject)this._followedAgent.Character;
			this._targetEntity = targetEntity;
			this.StartAgentMovement();
		}

		public FollowAgentQuestTask(Agent followedAgent, Agent targetAgent, Action onSucceededAction, Action onCanceledAction, DialogFlow dialogFlow = null)
			: base(dialogFlow, onSucceededAction, null, onCanceledAction)
		{
			this._followedAgent = followedAgent;
			this._targetAgent = targetAgent;
			this.StartAgentMovement();
		}

		private void StartAgentMovement()
		{
			if (this._targetEntity != null)
			{
				UsableMachine firstScriptOfType = this._targetEntity.GetFirstScriptOfType<UsableMachine>();
				ScriptBehavior.AddUsableMachineTarget(this._followedAgent, firstScriptOfType);
				return;
			}
			if (this._targetAgent != null)
			{
				ScriptBehavior.AddAgentTarget(this._followedAgent, this._targetAgent);
			}
		}

		public void MissionTick(float dt)
		{
			ScriptBehavior scriptBehavior = (ScriptBehavior)this._followedAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehavior<ScriptBehavior>();
			if (scriptBehavior != null && scriptBehavior.IsNearTarget(this._targetAgent) && this._followedAgent.GetCurrentVelocity().LengthSquared < 0.0001f && this._followedAgent.Position.DistanceSquared(Mission.Current.MainAgent.Position) < 16f)
			{
				base.Finish(0);
			}
		}

		protected override void OnFinished()
		{
			this._followedAgent = null;
			this._followedAgentChar = null;
			this._targetEntity = null;
			this._targetAgent = null;
		}

		public override void SetReferences()
		{
			CampaignEvents.MissionTickEvent.AddNonSerializedListener(this, new Action<float>(this.MissionTick));
		}

		private Agent _followedAgent;

		private CharacterObject _followedAgentChar;

		private GameEntity _targetEntity;

		private Agent _targetAgent;
	}
}
