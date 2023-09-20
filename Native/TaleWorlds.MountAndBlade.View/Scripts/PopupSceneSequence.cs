using System;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade.View.Scripts
{
	public class PopupSceneSequence : ScriptComponentBehavior
	{
		public void InitializeWithAgentVisuals(AgentVisuals visuals)
		{
			this._agentVisuals = visuals;
			this._time = 0f;
			this._triggered = false;
			this._state = 0;
		}

		protected override void OnInit()
		{
			base.OnInit();
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return 2 | base.GetTickRequirement();
		}

		protected override void OnTick(float dt)
		{
			this._time += dt;
			if (!this._triggered)
			{
				if (this._state == 0 && this._time >= this.InitialActivationTime)
				{
					this._triggered = true;
					this.OnInitialState();
				}
				if (this._state == 1 && this._time >= this.PositiveActivationTime)
				{
					this._triggered = true;
					this.OnPositiveState();
				}
				if (this._state == 2 && this._time >= this.NegativeActivationTime)
				{
					this._triggered = true;
					this.OnNegativeState();
				}
			}
		}

		public virtual void OnInitialState()
		{
		}

		public virtual void OnPositiveState()
		{
		}

		public virtual void OnNegativeState()
		{
		}

		public void SetInitialState()
		{
			this._triggered = false;
			this._state = 0;
			this._time = 0f;
		}

		public void SetPositiveState()
		{
			this._triggered = false;
			this._state = 1;
			this._time = 0f;
		}

		public void SetNegativeState()
		{
			this._triggered = false;
			this._state = 2;
			this._time = 0f;
		}

		public float InitialActivationTime;

		public float PositiveActivationTime;

		public float NegativeActivationTime;

		protected AgentVisuals _agentVisuals;

		protected float _time;

		protected bool _triggered;

		protected int _state;
	}
}
