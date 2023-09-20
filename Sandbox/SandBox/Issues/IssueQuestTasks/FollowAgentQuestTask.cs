using System;
using SandBox.Missions.AgentBehaviors;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;

namespace SandBox.Issues.IssueQuestTasks
{
	// Token: 0x02000084 RID: 132
	public class FollowAgentQuestTask : QuestTaskBase
	{
		// Token: 0x0600058E RID: 1422 RVA: 0x00027453 File Offset: 0x00025653
		public FollowAgentQuestTask(Agent followedAgent, GameEntity targetEntity, Action onSucceededAction, Action onCanceledAction, DialogFlow dialogFlow = null)
			: base(dialogFlow, onSucceededAction, null, onCanceledAction)
		{
			this._followedAgent = followedAgent;
			this._followedAgentChar = (CharacterObject)this._followedAgent.Character;
			this._targetEntity = targetEntity;
			this.StartAgentMovement();
		}

		// Token: 0x0600058F RID: 1423 RVA: 0x0002748B File Offset: 0x0002568B
		public FollowAgentQuestTask(Agent followedAgent, Agent targetAgent, Action onSucceededAction, Action onCanceledAction, DialogFlow dialogFlow = null)
			: base(dialogFlow, onSucceededAction, null, onCanceledAction)
		{
			this._followedAgent = followedAgent;
			this._targetAgent = targetAgent;
			this.StartAgentMovement();
		}

		// Token: 0x06000590 RID: 1424 RVA: 0x000274B0 File Offset: 0x000256B0
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

		// Token: 0x06000591 RID: 1425 RVA: 0x00027500 File Offset: 0x00025700
		public void MissionTick(float dt)
		{
			ScriptBehavior scriptBehavior = (ScriptBehavior)this._followedAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehavior<ScriptBehavior>();
			if (scriptBehavior != null && scriptBehavior.IsNearTarget(this._targetAgent) && this._followedAgent.GetCurrentVelocity().LengthSquared < 0.0001f && this._followedAgent.Position.DistanceSquared(Mission.Current.MainAgent.Position) < 16f)
			{
				base.Finish(0);
			}
		}

		// Token: 0x06000592 RID: 1426 RVA: 0x00027588 File Offset: 0x00025788
		protected override void OnFinished()
		{
			this._followedAgent = null;
			this._followedAgentChar = null;
			this._targetEntity = null;
			this._targetAgent = null;
		}

		// Token: 0x06000593 RID: 1427 RVA: 0x000275A6 File Offset: 0x000257A6
		public override void SetReferences()
		{
			CampaignEvents.MissionTickEvent.AddNonSerializedListener(this, new Action<float>(this.MissionTick));
		}

		// Token: 0x040002B6 RID: 694
		private Agent _followedAgent;

		// Token: 0x040002B7 RID: 695
		private CharacterObject _followedAgentChar;

		// Token: 0x040002B8 RID: 696
		private GameEntity _targetEntity;

		// Token: 0x040002B9 RID: 697
		private Agent _targetAgent;
	}
}
