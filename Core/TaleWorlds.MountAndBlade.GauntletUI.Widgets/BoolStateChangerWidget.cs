using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x02000008 RID: 8
	public class BoolStateChangerWidget : Widget
	{
		// Token: 0x0600001C RID: 28 RVA: 0x0000235A File Offset: 0x0000055A
		public BoolStateChangerWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002364 File Offset: 0x00000564
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

		// Token: 0x0600001E RID: 30 RVA: 0x0000239C File Offset: 0x0000059C
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

		// Token: 0x0600001F RID: 31 RVA: 0x000023D4 File Offset: 0x000005D4
		private void TriggerUpdated()
		{
			string text = (this.BooleanCheck ? this.TrueState : this.FalseState);
			Widget widget = this.TargetWidget ?? this;
			this.AddState(widget, text, this.IncludeChildren);
			this.SetState(widget, text, this.IncludeChildren);
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000020 RID: 32 RVA: 0x00002420 File Offset: 0x00000620
		// (set) Token: 0x06000021 RID: 33 RVA: 0x00002428 File Offset: 0x00000628
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

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000022 RID: 34 RVA: 0x0000244C File Offset: 0x0000064C
		// (set) Token: 0x06000023 RID: 35 RVA: 0x00002454 File Offset: 0x00000654
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

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000024 RID: 36 RVA: 0x00002477 File Offset: 0x00000677
		// (set) Token: 0x06000025 RID: 37 RVA: 0x0000247F File Offset: 0x0000067F
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

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000026 RID: 38 RVA: 0x000024A2 File Offset: 0x000006A2
		// (set) Token: 0x06000027 RID: 39 RVA: 0x000024AA File Offset: 0x000006AA
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

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000028 RID: 40 RVA: 0x000024C8 File Offset: 0x000006C8
		// (set) Token: 0x06000029 RID: 41 RVA: 0x000024D0 File Offset: 0x000006D0
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

		// Token: 0x0400000A RID: 10
		private bool _booleanCheck;

		// Token: 0x0400000B RID: 11
		private string _trueState;

		// Token: 0x0400000C RID: 12
		private string _falseState;

		// Token: 0x0400000D RID: 13
		private Widget _targetWidget;

		// Token: 0x0400000E RID: 14
		private bool _includeChildren;
	}
}
