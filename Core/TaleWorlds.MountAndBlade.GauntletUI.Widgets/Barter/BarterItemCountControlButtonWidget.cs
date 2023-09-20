using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Barter
{
	public class BarterItemCountControlButtonWidget : ButtonWidget
	{
		public float IncreaseToHoldDelay { get; set; } = 1f;

		public BarterItemCountControlButtonWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			this._totalTime += dt;
			if (base.IsPressed && this._clickStartTime + this.IncreaseToHoldDelay < this._totalTime)
			{
				base.EventFired("MoveOne", Array.Empty<object>());
			}
		}

		protected override void OnMousePressed()
		{
			base.OnMousePressed();
			this._clickStartTime = this._totalTime;
			base.EventFired("MoveOne", Array.Empty<object>());
		}

		protected override void OnMouseReleased()
		{
			base.OnMouseReleased();
			this._clickStartTime = 0f;
		}

		private float _clickStartTime;

		private float _totalTime;
	}
}
