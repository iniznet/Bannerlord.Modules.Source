using System;
using SandBox.Missions.AgentBehaviors;
using SandBox.Objects.Usables;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace SandBox.Source.Missions.AgentBehaviors
{
	public class BoardGameAgentBehavior : AgentBehavior
	{
		public BoardGameAgentBehavior(AgentBehaviorGroup behaviorGroup)
			: base(behaviorGroup)
		{
		}

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

		protected override void OnDeactivate()
		{
			base.Navigator.ClearTarget();
			this._chair = null;
			this._state = BoardGameAgentBehavior.State.Idle;
		}

		public override string GetDebugInfo()
		{
			return "BoardGameAgentBehavior";
		}

		public override float GetAvailability(bool isSimulation)
		{
			return 1f;
		}

		private void RemoveBoardGameBehaviorInternal()
		{
			InterruptingBehaviorGroup behaviorGroup = base.OwnerAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<InterruptingBehaviorGroup>();
			if (behaviorGroup.GetBehavior<BoardGameAgentBehavior>() != null)
			{
				behaviorGroup.RemoveBehavior<BoardGameAgentBehavior>();
			}
		}

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

		public static void RemoveBoardGameBehaviorOfAgent(Agent ownerAgent)
		{
			BoardGameAgentBehavior behavior = ownerAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<InterruptingBehaviorGroup>().GetBehavior<BoardGameAgentBehavior>();
			if (behavior != null)
			{
				behavior._chair = null;
				behavior._state = BoardGameAgentBehavior.State.Finish;
			}
		}

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

		private const int FinishDelayAsSeconds = 3;

		private Chair _chair;

		private BoardGameAgentBehavior.State _state;

		private Timer _waitTimer;

		private enum State
		{
			Idle,
			MovingToChair,
			Finish
		}
	}
}
