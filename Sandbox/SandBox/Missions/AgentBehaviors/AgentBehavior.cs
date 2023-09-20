using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.AgentBehaviors
{
	public abstract class AgentBehavior
	{
		public AgentNavigator Navigator
		{
			get
			{
				return this.BehaviorGroup.Navigator;
			}
		}

		public bool IsActive
		{
			get
			{
				return this._isActive;
			}
			set
			{
				if (this._isActive != value)
				{
					this._isActive = value;
					if (this._isActive)
					{
						this.OnActivate();
						return;
					}
					this.OnDeactivate();
				}
			}
		}

		public Agent OwnerAgent
		{
			get
			{
				return this.Navigator.OwnerAgent;
			}
		}

		public Mission Mission { get; private set; }

		protected AgentBehavior(AgentBehaviorGroup behaviorGroup)
		{
			this.Mission = behaviorGroup.Mission;
			this.CheckTime = 40f + MBRandom.RandomFloat * 20f;
			this.BehaviorGroup = behaviorGroup;
			this._isActive = false;
		}

		public virtual float GetAvailability(bool isSimulation)
		{
			return 0f;
		}

		public virtual void Tick(float dt, bool isSimulation)
		{
		}

		public virtual void ConversationTick()
		{
		}

		protected virtual void OnActivate()
		{
		}

		protected virtual void OnDeactivate()
		{
		}

		public virtual bool CheckStartWithBehavior()
		{
			return false;
		}

		public virtual void OnSpecialTargetChanged()
		{
		}

		public virtual void SetCustomWanderTarget(UsableMachine customUsableMachine)
		{
		}

		public virtual void OnAgentRemoved(Agent agent)
		{
		}

		public abstract string GetDebugInfo();

		public float CheckTime = 15f;

		protected readonly AgentBehaviorGroup BehaviorGroup;

		private bool _isActive;
	}
}
