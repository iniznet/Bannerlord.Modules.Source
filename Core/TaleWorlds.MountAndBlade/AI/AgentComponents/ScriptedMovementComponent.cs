using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade.AI.AgentComponents
{
	public class ScriptedMovementComponent : AgentComponent
	{
		public ScriptedMovementComponent(Agent agent, bool isCharacterToTalkTo = false, float dialogueProximityOffset = 0f)
			: base(agent)
		{
			this._isCharacterToTalkTo = isCharacterToTalkTo;
			this._agentSpeedLimit = this.Agent.GetMaximumSpeedLimit();
			if (!this._isCharacterToTalkTo)
			{
				this.Agent.SetMaximumSpeedLimit(MBRandom.RandomFloatRanged(0.2f, 0.3f), true);
				this._dialogueTriggerProximity += dialogueProximityOffset;
			}
		}

		public void SetTargetAgent(Agent targetAgent)
		{
			this._targetAgent = targetAgent;
		}

		public override void OnTickAsAI(float dt)
		{
			if (this._targetAgent != null)
			{
				bool flag = this._targetAgent.State != AgentState.Routed && this._targetAgent.State != AgentState.Deleted;
				if (!this._isInDialogueRange)
				{
					float num = this._targetAgent.Position.DistanceSquared(this.Agent.Position);
					this._isInDialogueRange = num <= this._dialogueTriggerProximity * this._dialogueTriggerProximity;
					if (this._isInDialogueRange)
					{
						this.Agent.SetScriptedFlags(this.Agent.GetScriptedFlags() & ~Agent.AIScriptedFrameFlags.DoNotRun);
						this.Agent.DisableScriptedMovement();
						if (flag)
						{
							this.Agent.SetLookAgent(this._targetAgent);
						}
						this.Agent.SetMaximumSpeedLimit(this._agentSpeedLimit, false);
						return;
					}
					WorldPosition worldPosition = this._targetAgent.Position.ToWorldPosition();
					this.Agent.SetScriptedPosition(ref worldPosition, false, Agent.AIScriptedFrameFlags.DoNotRun);
					return;
				}
				else if (!flag)
				{
					this.Agent.SetLookAgent(null);
				}
			}
		}

		public bool ShouldConversationStartWithAgent()
		{
			return this._isInDialogueRange && this._isCharacterToTalkTo;
		}

		private bool _isInDialogueRange;

		private readonly bool _isCharacterToTalkTo;

		private readonly float _dialogueTriggerProximity = 10f;

		private readonly float _agentSpeedLimit;

		private Agent _targetAgent;
	}
}
