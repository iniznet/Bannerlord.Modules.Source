using System;
using SandBox.Missions.AgentBehaviors;
using SandBox.Objects.Usables;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace SandBox.Source.Missions.AgentBehaviors
{
	// Token: 0x02000033 RID: 51
	public class BoardGameAgentBehavior : AgentBehavior
	{
		// Token: 0x06000264 RID: 612 RVA: 0x00010654 File Offset: 0x0000E854
		public BoardGameAgentBehavior(AgentBehaviorGroup behaviorGroup)
			: base(behaviorGroup)
		{
		}

		// Token: 0x06000265 RID: 613 RVA: 0x00010660 File Offset: 0x0000E860
		public override void Tick(float dt, bool isSimulation)
		{
			switch (this._state)
			{
			case BoardGameAgentBehavior.State.Idle:
				if (base.Navigator.TargetUsableMachine != this._chair && !this._chair.IsAgentFullySitting(base.OwnerAgent))
				{
					base.Navigator.SetTarget(this._chair, false);
					this._state = BoardGameAgentBehavior.State.MovingToChair;
					return;
				}
				break;
			case BoardGameAgentBehavior.State.MovingToChair:
				if (this._chair.IsAgentFullySitting(base.OwnerAgent))
				{
					this._state = BoardGameAgentBehavior.State.Idle;
					return;
				}
				break;
			case BoardGameAgentBehavior.State.Finish:
				if (base.OwnerAgent.IsUsingGameObject && this._waitTimer == null)
				{
					base.Navigator.ClearTarget();
					this._waitTimer = new Timer(base.Mission.CurrentTime, 3f, true);
					return;
				}
				if (this._waitTimer != null)
				{
					if (this._waitTimer.Check(base.Mission.CurrentTime))
					{
						this.RemoveBoardGameBehaviorInternal();
						return;
					}
				}
				else
				{
					this.RemoveBoardGameBehaviorInternal();
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06000266 RID: 614 RVA: 0x00010752 File Offset: 0x0000E952
		protected override void OnDeactivate()
		{
			base.Navigator.ClearTarget();
			this._chair = null;
			this._state = BoardGameAgentBehavior.State.Idle;
		}

		// Token: 0x06000267 RID: 615 RVA: 0x0001076D File Offset: 0x0000E96D
		public override string GetDebugInfo()
		{
			return "BoardGameAgentBehavior";
		}

		// Token: 0x06000268 RID: 616 RVA: 0x00010774 File Offset: 0x0000E974
		public override float GetAvailability(bool isSimulation)
		{
			return 1f;
		}

		// Token: 0x06000269 RID: 617 RVA: 0x0001077C File Offset: 0x0000E97C
		private void RemoveBoardGameBehaviorInternal()
		{
			InterruptingBehaviorGroup behaviorGroup = base.OwnerAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<InterruptingBehaviorGroup>();
			if (behaviorGroup.GetBehavior<BoardGameAgentBehavior>() != null)
			{
				behaviorGroup.RemoveBehavior<BoardGameAgentBehavior>();
			}
		}

		// Token: 0x0600026A RID: 618 RVA: 0x000107B0 File Offset: 0x0000E9B0
		public static void AddTargetChair(Agent ownerAgent, Chair chair)
		{
			InterruptingBehaviorGroup behaviorGroup = ownerAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<InterruptingBehaviorGroup>();
			bool flag = behaviorGroup.GetBehavior<BoardGameAgentBehavior>() == null;
			BoardGameAgentBehavior boardGameAgentBehavior = behaviorGroup.GetBehavior<BoardGameAgentBehavior>() ?? behaviorGroup.AddBehavior<BoardGameAgentBehavior>();
			boardGameAgentBehavior._chair = chair;
			boardGameAgentBehavior._state = BoardGameAgentBehavior.State.Idle;
			if (flag)
			{
				behaviorGroup.SetScriptedBehavior<BoardGameAgentBehavior>();
			}
		}

		// Token: 0x0600026B RID: 619 RVA: 0x000107FC File Offset: 0x0000E9FC
		public static void RemoveBoardGameBehaviorOfAgent(Agent ownerAgent)
		{
			BoardGameAgentBehavior behavior = ownerAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<InterruptingBehaviorGroup>().GetBehavior<BoardGameAgentBehavior>();
			if (behavior != null)
			{
				behavior._chair = null;
				behavior._state = BoardGameAgentBehavior.State.Finish;
			}
		}

		// Token: 0x0600026C RID: 620 RVA: 0x00010830 File Offset: 0x0000EA30
		public static bool IsAgentMovingToChair(Agent ownerAgent)
		{
			if (ownerAgent == null)
			{
				return false;
			}
			InterruptingBehaviorGroup behaviorGroup = ownerAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<InterruptingBehaviorGroup>();
			BoardGameAgentBehavior boardGameAgentBehavior = ((behaviorGroup != null) ? behaviorGroup.GetBehavior<BoardGameAgentBehavior>() : null);
			return boardGameAgentBehavior != null && boardGameAgentBehavior._state == BoardGameAgentBehavior.State.MovingToChair;
		}

		// Token: 0x0400013A RID: 314
		private const int FinishDelayAsSeconds = 3;

		// Token: 0x0400013B RID: 315
		private Chair _chair;

		// Token: 0x0400013C RID: 316
		private BoardGameAgentBehavior.State _state;

		// Token: 0x0400013D RID: 317
		private Timer _waitTimer;

		// Token: 0x0200010E RID: 270
		private enum State
		{
			// Token: 0x04000546 RID: 1350
			Idle,
			// Token: 0x04000547 RID: 1351
			MovingToChair,
			// Token: 0x04000548 RID: 1352
			Finish
		}
	}
}
