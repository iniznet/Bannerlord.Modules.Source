using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.AgentBehaviors
{
	public abstract class AgentBehaviorGroup
	{
		public Agent OwnerAgent
		{
			get
			{
				return this.Navigator.OwnerAgent;
			}
		}

		public AgentBehavior ScriptedBehavior { get; private set; }

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

		public Mission Mission { get; private set; }

		protected AgentBehaviorGroup(AgentNavigator navigator, Mission mission)
		{
			this.Mission = mission;
			this.Behaviors = new List<AgentBehavior>();
			this.Navigator = navigator;
			this._isActive = false;
			this.ScriptedBehavior = null;
		}

		public T AddBehavior<T>() where T : AgentBehavior
		{
			T t = Activator.CreateInstance(typeof(T), new object[] { this }) as T;
			if (t != null)
			{
				foreach (AgentBehavior agentBehavior in this.Behaviors)
				{
					if (agentBehavior.GetType() == t.GetType())
					{
						return agentBehavior as T;
					}
				}
				this.Behaviors.Add(t);
				return t;
			}
			return t;
		}

		public T GetBehavior<T>() where T : AgentBehavior
		{
			foreach (AgentBehavior agentBehavior in this.Behaviors)
			{
				if (agentBehavior is T)
				{
					return (T)((object)agentBehavior);
				}
			}
			return default(T);
		}

		public bool HasBehavior<T>() where T : AgentBehavior
		{
			using (List<AgentBehavior>.Enumerator enumerator = this.Behaviors.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current is T)
					{
						return true;
					}
				}
			}
			return false;
		}

		public void RemoveBehavior<T>() where T : AgentBehavior
		{
			for (int i = 0; i < this.Behaviors.Count; i++)
			{
				if (this.Behaviors[i] is T)
				{
					bool isActive = this.Behaviors[i].IsActive;
					this.Behaviors[i].IsActive = false;
					if (this.ScriptedBehavior == this.Behaviors[i])
					{
						this.ScriptedBehavior = null;
					}
					this.Behaviors.RemoveAt(i);
					if (isActive)
					{
						this.ForceThink(0f);
					}
				}
			}
		}

		public void SetScriptedBehavior<T>() where T : AgentBehavior
		{
			foreach (AgentBehavior agentBehavior in this.Behaviors)
			{
				if (agentBehavior is T)
				{
					this.ScriptedBehavior = agentBehavior;
					this.ForceThink(0f);
					break;
				}
			}
		}

		public void DisableScriptedBehavior()
		{
			if (this.ScriptedBehavior != null)
			{
				this.ScriptedBehavior.IsActive = false;
				this.ScriptedBehavior = null;
				this.ForceThink(0f);
			}
		}

		public void DisableAllBehaviors()
		{
			foreach (AgentBehavior agentBehavior in this.Behaviors)
			{
				agentBehavior.IsActive = false;
			}
		}

		public AgentBehavior GetActiveBehavior()
		{
			foreach (AgentBehavior agentBehavior in this.Behaviors)
			{
				if (agentBehavior.IsActive)
				{
					return agentBehavior;
				}
			}
			return null;
		}

		public virtual void Tick(float dt, bool isSimulation)
		{
		}

		public virtual void ConversationTick()
		{
		}

		public virtual void OnAgentRemoved(Agent agent)
		{
		}

		protected virtual void OnActivate()
		{
		}

		protected virtual void OnDeactivate()
		{
			foreach (AgentBehavior agentBehavior in this.Behaviors)
			{
				agentBehavior.IsActive = false;
			}
		}

		public virtual float GetScore(bool isSimulation)
		{
			return 0f;
		}

		public virtual void ForceThink(float inSeconds)
		{
		}

		public AgentNavigator Navigator;

		public List<AgentBehavior> Behaviors;

		protected float CheckBehaviorTime = 5f;

		protected Timer CheckBehaviorTimer;

		private bool _isActive;
	}
}
