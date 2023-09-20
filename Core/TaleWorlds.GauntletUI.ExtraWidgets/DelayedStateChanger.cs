using System;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.GauntletUI.ExtraWidgets
{
	// Token: 0x02000004 RID: 4
	public class DelayedStateChanger : BrushWidget
	{
		// Token: 0x06000011 RID: 17 RVA: 0x00002224 File Offset: 0x00000424
		public DelayedStateChanger(UIContext context)
			: base(context)
		{
			this._isStarted = false;
			this._isFinished = false;
			this._timePassed = 0f;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002246 File Offset: 0x00000446
		protected override void OnConnectedToRoot()
		{
			this._defaultState = base.CurrentState;
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002254 File Offset: 0x00000454
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

		// Token: 0x06000014 RID: 20 RVA: 0x000022D0 File Offset: 0x000004D0
		public void Start()
		{
			this._isStarted = true;
			this._isFinished = false;
			this._timePassed = 0f;
			this._widget = this.TargetWidget ?? this;
			this.AddState(this._widget, this.State, this.IncludeChildren);
		}

		// Token: 0x06000015 RID: 21 RVA: 0x0000231F File Offset: 0x0000051F
		private void Reset()
		{
			this._isStarted = false;
			this._isFinished = true;
			this._widget = this.TargetWidget ?? this;
			this.SetState(this._widget, this._defaultState, this.IncludeChildren);
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00002358 File Offset: 0x00000558
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

		// Token: 0x06000017 RID: 23 RVA: 0x00002390 File Offset: 0x00000590
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

		// Token: 0x06000018 RID: 24 RVA: 0x000023C7 File Offset: 0x000005C7
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

		// Token: 0x06000019 RID: 25 RVA: 0x000023E6 File Offset: 0x000005E6
		private void TargetWidgetUpdated()
		{
			this._defaultState = ((this.TargetWidget == null) ? base.CurrentState : this.TargetWidget.CurrentState);
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600001A RID: 26 RVA: 0x00002409 File Offset: 0x00000609
		// (set) Token: 0x0600001B RID: 27 RVA: 0x00002411 File Offset: 0x00000611
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

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600001C RID: 28 RVA: 0x0000242F File Offset: 0x0000062F
		// (set) Token: 0x0600001D RID: 29 RVA: 0x00002437 File Offset: 0x00000637
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

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600001E RID: 30 RVA: 0x0000245B File Offset: 0x0000065B
		// (set) Token: 0x0600001F RID: 31 RVA: 0x00002463 File Offset: 0x00000663
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

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000020 RID: 32 RVA: 0x00002481 File Offset: 0x00000681
		// (set) Token: 0x06000021 RID: 33 RVA: 0x00002489 File Offset: 0x00000689
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

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000022 RID: 34 RVA: 0x000024A7 File Offset: 0x000006A7
		// (set) Token: 0x06000023 RID: 35 RVA: 0x000024AF File Offset: 0x000006AF
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

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000024 RID: 36 RVA: 0x000024CD File Offset: 0x000006CD
		// (set) Token: 0x06000025 RID: 37 RVA: 0x000024D5 File Offset: 0x000006D5
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

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000026 RID: 38 RVA: 0x000024F8 File Offset: 0x000006F8
		// (set) Token: 0x06000027 RID: 39 RVA: 0x00002500 File Offset: 0x00000700
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

		// Token: 0x04000009 RID: 9
		private bool _isStarted;

		// Token: 0x0400000A RID: 10
		private bool _isFinished;

		// Token: 0x0400000B RID: 11
		private float _timePassed;

		// Token: 0x0400000C RID: 12
		private Widget _widget;

		// Token: 0x0400000D RID: 13
		private string _defaultState;

		// Token: 0x0400000E RID: 14
		private bool _autoStart;

		// Token: 0x0400000F RID: 15
		private bool _trigger;

		// Token: 0x04000010 RID: 16
		private bool _stateResetable;

		// Token: 0x04000011 RID: 17
		private bool _includeChildren;

		// Token: 0x04000012 RID: 18
		private float _delay;

		// Token: 0x04000013 RID: 19
		private string _state;

		// Token: 0x04000014 RID: 20
		private Widget _targetWidget;
	}
}
