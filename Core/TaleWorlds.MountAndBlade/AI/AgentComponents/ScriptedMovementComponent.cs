using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade.AI.AgentComponents
{
	// Token: 0x02000412 RID: 1042
	public class ScriptedMovementComponent : AgentComponent
	{
		// Token: 0x060035A3 RID: 13731 RVA: 0x000DEC24 File Offset: 0x000DCE24
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

		// Token: 0x060035A4 RID: 13732 RVA: 0x000DEC8C File Offset: 0x000DCE8C
		public void SetTargetAgent(Agent targetAgent)
		{
			this._targetAgent = targetAgent;
		}

		// Token: 0x060035A5 RID: 13733 RVA: 0x000DEC98 File Offset: 0x000DCE98
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

		// Token: 0x060035A6 RID: 13734 RVA: 0x000DED9A File Offset: 0x000DCF9A
		public bool ShouldConversationStartWithAgent()
		{
			return this._isInDialogueRange && this._isCharacterToTalkTo;
		}

		// Token: 0x040016FB RID: 5883
		private bool _isInDialogueRange;

		// Token: 0x040016FC RID: 5884
		private readonly bool _isCharacterToTalkTo;

		// Token: 0x040016FD RID: 5885
		private readonly float _dialogueTriggerProximity = 10f;

		// Token: 0x040016FE RID: 5886
		private readonly float _agentSpeedLimit;

		// Token: 0x040016FF RID: 5887
		private Agent _targetAgent;
	}
}
