using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class SettlementStatTextWidget : TextWidget
	{
		public SettlementStatTextWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			switch (this._state)
			{
			case SettlementStatTextWidget.State.Idle:
				if (this.IsWarning)
				{
					this.SetState("Warning");
				}
				else
				{
					this.SetState("Default");
				}
				this._state = SettlementStatTextWidget.State.Start;
				return;
			case SettlementStatTextWidget.State.Start:
				this._state = ((base.BrushRenderer.Brush != null) ? SettlementStatTextWidget.State.Playing : SettlementStatTextWidget.State.Start);
				return;
			case SettlementStatTextWidget.State.Playing:
				base.BrushRenderer.RestartAnimation();
				this._state = SettlementStatTextWidget.State.End;
				break;
			case SettlementStatTextWidget.State.End:
				break;
			default:
				return;
			}
		}

		[Editor(false)]
		public bool IsWarning
		{
			get
			{
				return this._isWarning;
			}
			set
			{
				if (value != this._isWarning)
				{
					this._isWarning = value;
					base.OnPropertyChanged(value, "IsWarning");
					this.SetState(this._isWarning ? "Warning" : "Default");
				}
			}
		}

		private SettlementStatTextWidget.State _state;

		private bool _isWarning;

		public enum State
		{
			Idle,
			Start,
			Playing,
			End
		}
	}
}
