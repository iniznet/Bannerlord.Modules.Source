using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class BoolStateChangerWidget : Widget
	{
		public BoolStateChangerWidget(UIContext context)
			: base(context)
		{
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
			string text = (this.BooleanCheck ? this.TrueState : this.FalseState);
			Widget widget = this.TargetWidget ?? this;
			this.AddState(widget, text, this.IncludeChildren);
			this.SetState(widget, text, this.IncludeChildren);
		}

		[Editor(false)]
		public bool BooleanCheck
		{
			get
			{
				return this._booleanCheck;
			}
			set
			{
				if (this._booleanCheck != value)
				{
					this._booleanCheck = value;
					base.OnPropertyChanged(value, "BooleanCheck");
					this.TriggerUpdated();
				}
			}
		}

		[Editor(false)]
		public string TrueState
		{
			get
			{
				return this._trueState;
			}
			set
			{
				if (this._trueState != value)
				{
					this._trueState = value;
					base.OnPropertyChanged<string>(value, "TrueState");
				}
			}
		}

		[Editor(false)]
		public string FalseState
		{
			get
			{
				return this._falseState;
			}
			set
			{
				if (this._falseState != value)
				{
					this._falseState = value;
					base.OnPropertyChanged<string>(value, "FalseState");
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

		private bool _booleanCheck;

		private string _trueState;

		private string _falseState;

		private Widget _targetWidget;

		private bool _includeChildren;
	}
}
