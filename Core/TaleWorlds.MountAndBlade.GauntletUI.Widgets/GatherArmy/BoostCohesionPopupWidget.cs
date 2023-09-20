using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.GatherArmy
{
	// Token: 0x0200012C RID: 300
	public class BoostCohesionPopupWidget : Widget
	{
		// Token: 0x06000FE2 RID: 4066 RVA: 0x0002D0C8 File Offset: 0x0002B2C8
		public BoostCohesionPopupWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000FE3 RID: 4067 RVA: 0x0002D0D4 File Offset: 0x0002B2D4
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (this.ClosePopupButton != null && !this.ClosePopupButton.ClickEventHandlers.Contains(new Action<Widget>(this.ClosePopup)))
			{
				this.ClosePopupButton.ClickEventHandlers.Add(new Action<Widget>(this.ClosePopup));
			}
		}

		// Token: 0x06000FE4 RID: 4068 RVA: 0x0002D12A File Offset: 0x0002B32A
		public void ClosePopup(Widget widget)
		{
			base.ParentWidget.IsVisible = false;
		}

		// Token: 0x1700059C RID: 1436
		// (get) Token: 0x06000FE5 RID: 4069 RVA: 0x0002D138 File Offset: 0x0002B338
		// (set) Token: 0x06000FE6 RID: 4070 RVA: 0x0002D140 File Offset: 0x0002B340
		[Editor(false)]
		public ButtonWidget ClosePopupButton
		{
			get
			{
				return this._closePopupButton;
			}
			set
			{
				if (this._closePopupButton != value)
				{
					this._closePopupButton = value;
					base.OnPropertyChanged<ButtonWidget>(value, "ClosePopupButton");
				}
			}
		}

		// Token: 0x04000759 RID: 1881
		private ButtonWidget _closePopupButton;
	}
}
