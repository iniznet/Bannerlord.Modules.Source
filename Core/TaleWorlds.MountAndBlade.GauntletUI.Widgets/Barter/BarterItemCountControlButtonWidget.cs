using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Barter
{
	// Token: 0x0200016D RID: 365
	public class BarterItemCountControlButtonWidget : ButtonWidget
	{
		// Token: 0x17000697 RID: 1687
		// (get) Token: 0x060012AC RID: 4780 RVA: 0x000338E0 File Offset: 0x00031AE0
		// (set) Token: 0x060012AD RID: 4781 RVA: 0x000338E8 File Offset: 0x00031AE8
		public float IncreaseToHoldDelay { get; set; } = 1f;

		// Token: 0x060012AE RID: 4782 RVA: 0x000338F1 File Offset: 0x00031AF1
		public BarterItemCountControlButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060012AF RID: 4783 RVA: 0x00033908 File Offset: 0x00031B08
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			this._totalTime += dt;
			if (base.IsPressed && this._clickStartTime + this.IncreaseToHoldDelay < this._totalTime)
			{
				base.EventFired("MoveOne", Array.Empty<object>());
			}
		}

		// Token: 0x060012B0 RID: 4784 RVA: 0x00033957 File Offset: 0x00031B57
		protected override void OnMousePressed()
		{
			base.OnMousePressed();
			this._clickStartTime = this._totalTime;
			base.EventFired("MoveOne", Array.Empty<object>());
		}

		// Token: 0x060012B1 RID: 4785 RVA: 0x0003397B File Offset: 0x00031B7B
		protected override void OnMouseReleased()
		{
			base.OnMouseReleased();
			this._clickStartTime = 0f;
		}

		// Token: 0x0400088E RID: 2190
		private float _clickStartTime;

		// Token: 0x0400088F RID: 2191
		private float _totalTime;
	}
}
