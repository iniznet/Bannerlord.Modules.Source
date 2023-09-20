using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Conversation
{
	public class PersuasionResultChanceContainerListPanel : BrushListPanel
	{
		public float StayTime { get; set; } = 1f;

		public Widget CritFailWidget { get; set; }

		public Widget FailWidget { get; set; }

		public Widget SuccessWidget { get; set; }

		public Widget CritSuccessWidget { get; set; }

		public bool IsResultReady { get; set; }

		public PersuasionResultChanceContainerListPanel(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this.IsResultReady)
			{
				if (this._delayStartTime == -1f && base.AlphaFactor <= 0.001f)
				{
					this._delayStartTime = base.EventManager.Time;
				}
				float num = Mathf.Lerp(base.AlphaFactor, 0f, 0.35f);
				this.SetGlobalAlphaRecursively(num);
				Widget resultVisualWidget = this._resultVisualWidget;
				if (resultVisualWidget != null)
				{
					resultVisualWidget.SetGlobalAlphaRecursively(1f);
				}
				if (this._delayStartTime != -1f && base.EventManager.Time - this._delayStartTime > this.StayTime)
				{
					base.EventFired("OnReadyToContinue", Array.Empty<object>());
				}
			}
		}

		[Editor(false)]
		public int ResultIndex
		{
			get
			{
				return this._resultIndex;
			}
			set
			{
				if (value != this._resultIndex)
				{
					this._resultIndex = value;
					base.OnPropertyChanged(value, "ResultIndex");
					switch (value)
					{
					case 0:
						this._resultVisualWidget = this.CritFailWidget;
						this.SetState("CriticalFail");
						return;
					case 1:
						this._resultVisualWidget = this.FailWidget;
						this.SetState("Fail");
						return;
					case 2:
						this._resultVisualWidget = this.SuccessWidget;
						this.SetState("Success");
						return;
					case 3:
						this._resultVisualWidget = this.CritSuccessWidget;
						this.SetState("CriticalSuccess");
						break;
					default:
						return;
					}
				}
			}
		}

		private Widget _resultVisualWidget;

		private float _delayStartTime = -1f;

		private int _resultIndex;
	}
}
