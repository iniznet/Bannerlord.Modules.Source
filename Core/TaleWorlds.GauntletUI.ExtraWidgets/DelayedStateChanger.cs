using System;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.GauntletUI.ExtraWidgets
{
	public class DelayedStateChanger : BrushWidget
	{
		public DelayedStateChanger(UIContext context)
			: base(context)
		{
			this._isStarted = false;
			this._isFinished = false;
			this._timePassed = 0f;
		}

		protected override void OnConnectedToRoot()
		{
			this._defaultState = base.CurrentState;
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (this._isFinished || string.IsNullOrEmpty(this.State))
			{
				return;
			}
			if (!this._isStarted)
			{
				if (this.AutoStart)
				{
					this.Start();
					return;
				}
			}
			else
			{
				this._timePassed += dt;
				if (this._timePassed >= this.Delay)
				{
					this._isFinished = true;
					this.SetState(this._widget, this.State, this.IncludeChildren);
				}
			}
		}

		public void Start()
		{
			this._isStarted = true;
			this._isFinished = false;
			this._timePassed = 0f;
			this._widget = this.TargetWidget ?? this;
			this.AddState(this._widget, this.State, this.IncludeChildren);
		}

		private void Reset()
		{
			this._isStarted = false;
			this._isFinished = true;
			this._widget = this.TargetWidget ?? this;
			this.SetState(this._widget, this._defaultState, this.IncludeChildren);
		}

		private void AddState(Widget widget, string state, bool includeChildren)
		{
			widget.AddState(state);
			if (includeChildren)
			{
				for (int i = 0; i < widget.ChildCount; i++)
				{
					this.AddState(widget.GetChild(i), state, true);
				}
			}
		}

		private void SetState(Widget widget, string state, bool includeChildren)
		{
			widget.SetState(state);
			if (includeChildren)
			{
				for (int i = 0; i < widget.ChildCount; i++)
				{
					this.SetState(widget.GetChild(i), state, true);
				}
			}
		}

		private void TriggerUpdated()
		{
			if (this.Trigger)
			{
				this.Start();
				return;
			}
			if (this.StateResetable)
			{
				this.Reset();
			}
		}

		private void TargetWidgetUpdated()
		{
			this._defaultState = ((this.TargetWidget == null) ? base.CurrentState : this.TargetWidget.CurrentState);
		}

		[Editor(false)]
		public bool AutoStart
		{
			get
			{
				return this._autoStart;
			}
			set
			{
				if (this._autoStart != value)
				{
					this._autoStart = value;
					base.OnPropertyChanged(value, "AutoStart");
				}
			}
		}

		[Editor(false)]
		public bool Trigger
		{
			get
			{
				return this._trigger;
			}
			set
			{
				if (this._trigger != value)
				{
					this._trigger = value;
					base.OnPropertyChanged(value, "Trigger");
					this.TriggerUpdated();
				}
			}
		}

		[Editor(false)]
		public bool StateResetable
		{
			get
			{
				return this._stateResetable;
			}
			set
			{
				if (this._stateResetable != value)
				{
					this._stateResetable = value;
					base.OnPropertyChanged(value, "StateResetable");
				}
			}
		}

		[Editor(false)]
		public bool IncludeChildren
		{
			get
			{
				return this._includeChildren;
			}
			set
			{
				if (this._includeChildren != value)
				{
					this._includeChildren = value;
					base.OnPropertyChanged(value, "IncludeChildren");
				}
			}
		}

		[Editor(false)]
		public float Delay
		{
			get
			{
				return this._delay;
			}
			set
			{
				if (this._delay != value)
				{
					this._delay = value;
					base.OnPropertyChanged(value, "Delay");
				}
			}
		}

		[Editor(false)]
		public string State
		{
			get
			{
				return this._state;
			}
			set
			{
				if (this._state != value)
				{
					this._state = value;
					base.OnPropertyChanged<string>(value, "State");
				}
			}
		}

		[Editor(false)]
		public Widget TargetWidget
		{
			get
			{
				return this._targetWidget;
			}
			set
			{
				if (this._targetWidget != value)
				{
					this._targetWidget = value;
					base.OnPropertyChanged<Widget>(value, "TargetWidget");
					this.TargetWidgetUpdated();
				}
			}
		}

		private bool _isStarted;

		private bool _isFinished;

		private float _timePassed;

		private Widget _widget;

		private string _defaultState;

		private bool _autoStart;

		private bool _trigger;

		private bool _stateResetable;

		private bool _includeChildren;

		private float _delay;

		private string _state;

		private Widget _targetWidget;
	}
}
